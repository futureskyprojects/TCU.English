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

        public void Update(User entity)
        {
            instantce.User.Update(entity);
            instantce.SaveChanges();
        }
    }
}
