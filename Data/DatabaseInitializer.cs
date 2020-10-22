﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCU.English.Models;
using TCU.English.Utils;
using TCU.English.Utils.PasswordUtils;

namespace TCU.English.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize(SystemDatabaseContext context)
        {
            context.Database.EnsureCreated();
            //// Khởi tạo các quyền
            //InitializeRoles(context);
            //// Khởi tạo tài khoản quản trị viên cấp cao mặc định
            //InitializeAdministration(context);
            //// Cấp Full quyền cho tài khoản quản trị viên này
            //InitializeAdministrationRoles(context);
            StandardData(context);
        }

        private static void StandardData(SystemDatabaseContext context)
        {
            // Lấy danh sách các bản ghi của Reading part 4
            var rp4 = context.TestCategories.Where(x => x.TypeCode == TestCategory.READING && x.PartId == 4 && !string.IsNullOrEmpty(x.WYSIWYGContent)).ToList();

            // Cập nhật các chỉ số có liên quan
            for (int i = 0; i < rp4.Count; i++)
            {
                rp4[i].WYSIWYGContent = rp4[i].WYSIWYGContent.Replace(@"\(<strong>{$$}</strong>\)", "(<strong>{$$}</strong>)");
                context.TestCategories.Update(rp4[i]);

                Console.WriteLine($"progess: {i+1}/{rp4.Count}");
            }
            context.SaveChanges();
        }

        private static void InitializeAdministrationRoles(SystemDatabaseContext context)
        {
            // Đối với tài khoản quản trị viên
            User administration = context.User.Where(user => user.Username.ToLower().Equals(nameof(administration).ToLower())).FirstOrDefault();
            if (administration != null)
            {
                // Tìm quyền all
                UserType RoleAll = context.UserType.Where(role => role.UserTypeName.ToLower().Equals(UserType.ROLE_ALL.ToLower())).FirstOrDefault();
                // Thêm quyền cao nhất cho Admin
                UserTypeUser userTypeUser = new UserTypeUser
                {
                    User = administration,
                    UserType = RoleAll
                };

                if (!context.UserTypeUser.Any(it => it.UserId == administration.Id && it.UserTypeId == RoleAll.Id))
                {
                    // Nếu admin chưa có quyền này, tiến hành thêm mới cho Admin
                    context.UserTypeUser.Add(userTypeUser);
                    context.SaveChanges();
                }
            }
            else
            {
                // Nếu chưa tiến hành tạo mới
                InitializeAdministration(context);
                // Sau đó tiếp tục lại phương thức này
                InitializeAdministrationRoles(context);
            }
        }

        private static void InitializeAdministration(SystemDatabaseContext context)
        {
            User administration = new User
            {
                Username = "administration",
                Email = "administration@tcu.english.edu.vn",
                HashPassword = PasswordHasher.HashPassword(Base64Utils.Base64Decode("MTIzNDU2")),
                FirstName = "System",
                LastName = "Admin",
                Avatar = "/img/no_avatar.png",
                BirthDay = DateTime.Now,
                Active = true
            };
            if (!context.User.Any(it => it.Username == administration.Username))
            {
                // Nếu chưa có tài khoản quản trị viên cấp cao này, tiến hành tạo mới
                context.User.Add(administration);
                context.SaveChanges();
            }
        }

        private static void InitializeRoles(SystemDatabaseContext context)
        {
            // Khởi tạo các quyền đặc trưng cho hệ thống
            foreach (UserType role in UserType.Roles)
            {
                if (!context.UserType.Any(it => it.UserTypeName == role.UserTypeName))
                {
                    // Nếu chưa có quyền này, thì thêm vào
                    context.UserType.Add(role);
                    context.SaveChanges();
                }
            }
        }
    }
}
