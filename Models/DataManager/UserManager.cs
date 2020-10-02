using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TCU.English.Models.Repository;
using TCU.English.Utils.PasswordUtils;

namespace TCU.English.Models.DataManager
{
    public class UserManager : IDataRepository<User>
    {
        public readonly SystemDatabaseContext instantce;
        public UserManager(SystemDatabaseContext instantce)
        {
            this.instantce = instantce;
        }
        public void Add(User entity)
        {
            if (entity.HashPassword != null && entity.HashPassword.Length > 0)
            {
                entity.HashPassword = PasswordHasher.HashPassword(entity.HashPassword);
            }
            entity.CreatedTime = DateTime.UtcNow;
            entity.UpdatedTime = DateTime.UtcNow;
            entity.Active = true;
            instantce.User.Add(entity);
            instantce.SaveChanges();
        }

        public bool IsExists(int id)
        {
            return instantce.User.Any(user => user.Id == id);
        }

        public long Count()
        {
            try
            {
                return instantce.User.Distinct().Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public long Count(string roleCode)
        {
            if (roleCode == null)
            {
                var CountUserHaveRole = 0;
                try
                {
                    CountUserHaveRole = instantce.UserTypeUser.Select(utu => utu.UserId).Distinct().Count();
                }
                catch (Exception)
                {
                    CountUserHaveRole = 0;
                }
                var CountUserAll = Count();
                return CountUserAll - CountUserHaveRole;
            }
            else
            {
                try
                {
                    var s = instantce.UserType
                        .Join(
                            instantce.UserTypeUser,
                            userType => userType.Id,
                            userTypeUser => userTypeUser.UserTypeId,
                            (userType, userTypeUser) => new { userType, userTypeUser }
                        )
                        .Join(
                            instantce.User,
                            combinedEntry => combinedEntry.userTypeUser.UserId,
                            user => user.Id,
                            (combinedEntry, user) => new { combinedEntry, user }
                        )
                        .Where(fullEntry => fullEntry.combinedEntry.userType.UserTypeName == roleCode)
                        .Select(fullEntry => fullEntry.combinedEntry.userType)
                        .ToList();
                    return s.Count;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public void Delete(User entity)
        {
            instantce.User.Remove(entity);
            instantce.SaveChanges();
        }

        public bool IsUsernameAlreadyInUse(string username)
        {
            return instantce.User.Any(user => user.Username.ToLower() == username.ToLower());
        }
        public bool IsEmailAlreadyInUse(string email)
        {
            return instantce.User.Any(user => user.Email.ToLower() == email.ToLower());
        }
        public bool IsEmailAlreadyInUse(string username, string email)
        {
            return instantce.User.Any(user => user.Email.ToLower() == email.ToLower() && user.Username.ToLower() != username.ToLower());
        }

        public User Get(long id)
        {
            try
            {
                return instantce.User.Where(user => user.Id == id).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public User Get(string identity)
        {
            try
            {
                if (identity != null && identity.Length > 0 && identity.Contains("@"))
                    return instantce.User.Where(user => user.Email == identity).FirstOrDefault();
                else
                    return instantce.User.Where(user => user.Username == identity).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<User> GetAllInstructors()
        {
            try
            {
                return instantce.User
                        .Join(
                        instantce.UserTypeUser,
                        user => user.Id,
                        userTypeUser => userTypeUser.UserId,
                        (u, utu) => new { u, utu })
                        .Join(
                        instantce.UserType,
                        prv => prv.utu.UserTypeId,
                        ut => ut.Id,
                        (prv, ut) => new { prv, ut })
                        .Where(full =>
                            full.ut.UserTypeName.ToUpper().Equals(UserType.ROLE_INSTRUCTOR_USER) ||
                            full.ut.UserTypeName.ToUpper().Equals(UserType.ROLE_ALL)
                        )
                        .Select(full => full.prv.u)
                        .OrderByDescending(x => x.Id)
                        .ToList();
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }

        public IEnumerable<User> GetAllInstructorsOfStudent(int id, int start, int limit, string instructorName = "")
        {
            try
            {
                return instantce.User
                         .Join(
                         instantce.PieceOfTests,
                         user => user.Id,
                         piceOfTest => piceOfTest.InstructorId,
                         (u, pot) => new { u, pot })
                         .Where(full => full.pot.UserId == id &&
                                (string.IsNullOrEmpty(instructorName) ||
                                instructorName.Contains(full.u.Username) ||
                                instructorName.Contains(full.u.FirstName) ||
                                instructorName.Contains(full.u.LastName) ||
                                full.u.Username.Contains(instructorName) ||
                                full.u.FirstName.Contains(instructorName) ||
                                full.u.LastName.Contains(instructorName)))
                         .Select(full => full.u)
                         .OrderByDescending(x => x.Id)
                         .Skip(start)
                         .Take(limit)
                         .ToList();
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }

        public IEnumerable<User> GetAllStudentsOfInstructor(int id, int start, int limit, string studentName = "")
        {
            try
            {
                return instantce.User
                         .Join(
                         instantce.PieceOfTests,
                         user => user.Id,
                         piceOfTest => piceOfTest.UserId,
                         (u, pot) => new { u, pot })
                         .Where(full => full.pot.InstructorId != null && full.pot.InstructorId == id &&
                                (string.IsNullOrEmpty(studentName) ||
                                studentName.Contains(full.u.Username) ||
                                studentName.Contains(full.u.FirstName) ||
                                studentName.Contains(full.u.LastName) ||
                                full.u.Username.Contains(studentName) ||
                                full.u.FirstName.Contains(studentName) ||
                                full.u.LastName.Contains(studentName)))
                         .Select(full => full.u)
                         .OrderByDescending(x => x.Id)
                         .Skip(start)
                         .Take(limit)
                         .ToList();
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }
        public IEnumerable<User> GetAll()
        {
            try
            {
                return instantce.User.ToList();
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }

        public IEnumerable<User> GetByPagination(int start, int limit)
        {
            return instantce.User.OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }
        public IEnumerable<User> GetByPagination(string searchKey, int start, int limit)
        {
            if (searchKey == null || searchKey.Length <= 0)
                return GetByPagination(start, limit);
            else
            {
                return (from u in instantce.User
                        where (
                            ((u.Username.ToUpper().Contains(searchKey.ToUpper()) ||
                            u.FirstName.ToUpper().Contains(searchKey.ToUpper()) ||
                            u.LastName.ToUpper().Contains(searchKey.ToUpper())) &&
                            !searchKey.Contains("@")) ||
                            (u.Email.ToUpper().Contains(searchKey.ToUpper()) && searchKey.Contains("@"))
                        )
                        select u)
                       .OrderByDescending(x => x.Id)
                       .Skip(start)
                       .Take(limit)
                       .ToList();
            }
        }
        public IEnumerable<User> GetByPagination(int start, int limit, string roleName = UserType.ROLE_ALL)
        {
            if (roleName == null)
            {
                return (from u in instantce.User
                        where !instantce.UserTypeUser.Any(utu => utu.UserId == u.Id)
                        select u)
                        .OrderByDescending(x => x.Id)
                        .Skip(start)
                        .Take(limit)
                        .ToList();
            }
            else if (roleName == "")
            {
                return GetByPagination(start, limit);
            }
            else
            {
                return instantce.User
                        .Join(
                        instantce.UserTypeUser,
                        user => user.Id,
                        userTypeUser => userTypeUser.UserId,
                        (u, utu) => new { u, utu })
                        .Join(
                        instantce.UserType,
                        prv => prv.utu.UserTypeId,
                        ut => ut.Id,
                        (prv, ut) => new { prv, ut })
                        .Where(full => full.ut.UserTypeName.ToUpper().Equals(roleName.ToUpper()))
                        .Select(full => full.prv.u)
                        .OrderByDescending(x => x.Id)
                        .Skip(start)
                        .Take(limit)
                        .ToList();
            }
        }

        public void Update(User entity)
        {
            var oldUser = Get(entity.Id);
            if (entity.FirstName != null && entity.FirstName.Length > 0 && entity.FirstName != oldUser.FirstName)
                oldUser.FirstName = entity.FirstName;

            if (entity.LastName != null && entity.LastName.Length > 0 && entity.LastName != oldUser.LastName)
                oldUser.LastName = entity.LastName;

            if (entity.Active != oldUser.Active)
                oldUser.Active = entity.Active;

            if (entity.Avatar != null && entity.Avatar.Length > 0 && entity.Avatar != oldUser.Avatar)
                oldUser.Avatar = entity.Avatar;

            if (entity.BirthDay != null && entity.BirthDay != oldUser.BirthDay)
                oldUser.BirthDay = entity.BirthDay;

            if (entity.Email != null && entity.Email.Length > 0 && entity.Email != oldUser.Email)
                oldUser.Email = entity.Email;

            if (entity.Gender != oldUser.Gender)
                oldUser.Gender = entity.Gender;

            oldUser.UpdatedTime = DateTime.UtcNow;
            if (entity.HashPassword != null && entity.HashPassword.Length > 0 && entity.HashPassword != oldUser.HashPassword)
            {
                oldUser.HashPassword = PasswordHasher.HashPassword(entity.HashPassword);
            }
            entity = oldUser;
            instantce.User.Update(entity);
            instantce.SaveChanges();
        }
    }
}
