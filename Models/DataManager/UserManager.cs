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
            return instantce.User.Count();
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
