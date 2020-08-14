using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Models.Repository;

namespace TCU.English.Models.DataManager
{
    public class UserTypeUserManager : IDataRepository<UserTypeUser>
    {
        public readonly SystemDatabaseContext instantce;
        public UserTypeUserManager(SystemDatabaseContext instantce)
        {
            this.instantce = instantce;
        }

        public void Add(UserTypeUser entity)
        {
            instantce.UserTypeUser.Add(entity);
            instantce.SaveChanges();
        }

        public long Count()
        {
            return instantce.UserTypeUser.Count();
        }

        public void Delete(UserTypeUser entity)
        {
            instantce.UserTypeUser.Remove(entity);
            instantce.SaveChanges();
        }

        public UserTypeUser Get(long id)
        {
            try
            {
                return instantce.UserTypeUser.Where(type => type.Id == id).First();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<UserTypeUser> GetAll()
        {
            try
            {
                return instantce.UserTypeUser.ToList();
            }
            catch (Exception)
            {
                return new List<UserTypeUser>();
            }
        }

        public void Update(UserTypeUser entity)
        {
            instantce.UserTypeUser.Update(entity);
            instantce.SaveChanges();
        }
    }
}
