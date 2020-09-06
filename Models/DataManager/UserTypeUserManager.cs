﻿using System;
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
            entity.CreatedTime = DateTime.UtcNow;
            entity.UpdatedTime = DateTime.UtcNow;
            entity.Active = true;
            instantce.UserTypeUser.Add(entity);
            instantce.SaveChanges();
        }

        public long Count()
        {
            return instantce.UserTypeUser.Count();
        }

        public void Delete(long userId)
        {
            foreach (var item in instantce.UserTypeUser.Where(it => it.UserId == userId).ToList())
            {
                instantce.UserTypeUser.Remove(item);
            }
            instantce.SaveChanges();
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
        public IEnumerable<UserTypeUser> GetAll(int userId)
        {
            try
            {
                return instantce.UserTypeUser.Where(utu => utu.UserId == userId).ToList();
            }
            catch (Exception)
            {
                return new List<UserTypeUser>();
            }
        }

        public IEnumerable<UserTypeUser> GetByPagination(int start, int limit)
        {
            return instantce.UserTypeUser.OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }

        public void Update(UserTypeUser entity)
        {
            entity.UpdatedTime = DateTime.UtcNow;
            instantce.UserTypeUser.Update(entity);
            instantce.SaveChanges();
        }
    }
}
