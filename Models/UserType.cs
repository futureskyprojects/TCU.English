﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace TCU.English.Models
{
    public class UserType : BaseEntity
    {
        #region Default Role
        public const string ROLE_NORMAL_USER = nameof(ROLE_NORMAL_USER);
        public const string ROLE_INSTRUCTOR_USER = nameof(ROLE_INSTRUCTOR_USER);
        public const string ROLE_MANAGER_USER = nameof(ROLE_MANAGER_USER);
        public const string ROLE_ALL = nameof(ROLE_ALL);
        public const string ROLE_MANAGER_LIBRARY = nameof(ROLE_MANAGER_LIBRARY);
        #endregion

        public static UserType GetMaxUserType(string[] roleKeys)
        {
            return Parse(roleKeys).OrderBy(it => it.Priority).First();
        }

        public static List<UserType> Parse(string[] roleKeys)
        {
            List<UserType> uts = new List<UserType>();
            if (roleKeys != null)
            {
                foreach (string key in roleKeys)
                {
                    var temp = Roles.First(it => it.UserTypeName.ToLower() == key.ToLower());
                    if (temp != null)
                    {
                        uts.Add(temp);
                    }
                }
            }
            return uts;
        }
        public static int CompareRole(string currentUserRole, string destiantionUser)
        {
            var CurrentUser = Roles.First(it => it.UserTypeName == currentUserRole);
            var DestUser = Roles.First(it => it.UserTypeName == destiantionUser);

            return (CurrentUser?.Priority ?? -9999) - (DestUser?.Priority ?? 99999);
        }

        public UserType()
        {
            UserTypeUser = new HashSet<UserTypeUser>();
        }
        public string UserTypeName { get; set; }
        public int Priority { get; set; }
        public string Description { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserTypeUser> UserTypeUser { get; set; }

        //
        public string Display()
        {
            return Roles.First(it => it.UserTypeName == UserTypeName).Description ?? "Unknow Error";
        }


        //==================================================//
        [JsonIgnore]
        public static UserType NormalUser = new UserType
        {
            UserTypeName = nameof(ROLE_NORMAL_USER),
            Description = "Learner",
            Priority = 0
        };
        public static UserType InstructorUser = new UserType
        {
            UserTypeName = nameof(ROLE_INSTRUCTOR_USER),
            Description = "Instructor",
            Priority = 1
        };

        [JsonIgnore]
        public static UserType ManagerUser = new UserType
        {
            UserTypeName = nameof(ROLE_MANAGER_USER),
            Description = "User Manager",
            Priority = 999
        };

        [JsonIgnore]
        public static UserType All = new UserType
        {
            UserTypeName = nameof(ROLE_ALL),
            Description = "System Manager",
            Priority = 99999
        };

        [JsonIgnore]
        public static UserType ManagerLibrary = new UserType
        {
            UserTypeName = nameof(ROLE_MANAGER_LIBRARY),
            Description = "Question Library Manager",
            Priority = 0
        };
        //==================================================//

        [JsonIgnore]
        public static UserType[] Roles = new UserType[]
        {
            NormalUser,
            InstructorUser,
            ManagerUser,
            ManagerLibrary,
            All
        }.OrderBy(it => it.Priority).ToArray();
    }
}
