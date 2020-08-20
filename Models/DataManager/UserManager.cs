using System;
using System.Collections.Generic;
using System.Linq;
using TCU.English.Models.Repository;

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
            instantce.User.Add(entity);
            instantce.SaveChanges();
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

        public User Get(long id)
        {
            try
            {
                return instantce.User.Where(user => user.Id == id).First();
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
                if (identity.Contains("@"))
                    return instantce.User.Where(user => user.Email == identity).First();
                else
                    return instantce.User.Where(user => user.Username == identity).First();
            }
            catch (Exception)
            {
                return null;
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
            instantce.User.Update(entity);
            instantce.SaveChanges();
        }
    }
}
