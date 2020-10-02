using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Models.Repository;

namespace TCU.English.Models.DataManager
{
    public class DiscussionUserManager : IDataRepository<DiscussionUser>
    {
        public readonly SystemDatabaseContext instantce;
        public DiscussionUserManager(SystemDatabaseContext instantce)
        {
            this.instantce = instantce;
        }

        public void Add(DiscussionUser entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.UpdatedTime = DateTime.UtcNow;
            entity.Active = false;

            instantce.DiscussionUsers.Add(entity);
            instantce.SaveChanges();
        }

        public long Count()
        {
            return instantce.DiscussionUsers.Count();
        }

        public void Delete(DiscussionUser entity)
        {
            instantce.DiscussionUsers.Remove(entity);
            instantce.SaveChanges();
        }

        public DiscussionUser Get(long id)
        {
            return instantce.DiscussionUsers.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<DiscussionUser> GetAll()
        {
            return instantce.DiscussionUsers.ToList();
        }

        public IEnumerable<DiscussionUser> GetByPagination(int start, int limit)
        {
            return instantce.DiscussionUsers.OrderByDescending(x => x.Id).Skip(start).Take(limit);
        }

        public void Update(DiscussionUser entity)
        {
            entity.UpdatedTime = DateTime.UtcNow;

            instantce.DiscussionUsers.Update(entity);
            instantce.SaveChanges();
        }
    }
}
