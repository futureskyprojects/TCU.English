using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Models.Repository;

namespace TCU.English.Models.DataManager
{
    public class UserTypeManager : IDataRepository<UserType>
    {
        public readonly SystemDatabaseContext instantce;
        public UserTypeManager(SystemDatabaseContext instantce)
        {
            this.instantce = instantce;
        }

        public void Add(UserType entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.UpdatedTime = DateTime.UtcNow;
            entity.Active = true;
            instantce.UserType.Add(entity);
            instantce.SaveChanges();
        }

        public long Count()
        {
            return instantce.UserType.Count();
        }

        public void Delete(UserType entity)
        {
            instantce.UserType.Remove(entity);
            instantce.SaveChanges();
        }

        public UserType Get(long id)
        {
            try
            {
                return instantce.UserType.Where(type => type.Id == id).First();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public UserType Get(string typeCode)
        {
            try
            {
                return instantce.UserType.Where(type => type.UserTypeName == typeCode).First();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<UserType> GetAll(int userId)
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
                    .Where(fullEntry => fullEntry.user.Id == userId)
                    .Select(fullEntry => fullEntry.combinedEntry.userType)
                    .ToList();
                return s;
            }
            catch (Exception)
            {
                return new List<UserType>();
            }
        }

        public IEnumerable<UserType> GetAll()
        {
            try
            {
                return instantce.UserType.ToList();
            }
            catch (Exception)
            {
                return new List<UserType>();
            }
        }

        public IEnumerable<UserType> GetByPagination(int start, int limit)
        {
            return instantce.UserType.OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }

        public void Update(UserType entity)
        {
            entity.UpdatedTime = DateTime.UtcNow;
            instantce.UserType.Update(entity);
            instantce.SaveChanges();
        }
    }
}
