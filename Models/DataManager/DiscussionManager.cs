using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Models.Repository;

namespace TCU.English.Models.DataManager
{
    public class DiscussionManager : IDataRepository<Discussion>
    {
        public readonly SystemDatabaseContext instantce;
        public DiscussionManager(SystemDatabaseContext instantce)
        {
            this.instantce = instantce;
        }

        public void Add(Discussion entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.UpdatedTime = DateTime.UtcNow;

            entity.Active = true;
            instantce.Discussions.Add(entity);
            instantce.SaveChanges();
        }

        public long Count()
        {
            return instantce.Discussions.Count();
        }

        public void Delete(Discussion entity)
        {
            instantce.Discussions.Remove(entity);
            instantce.SaveChanges();
        }

        public Discussion Get(long id)
        {
            return instantce.Discussions.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Discussion> GetAll()
        {
            return instantce.Discussions.ToList();
        }

        public IEnumerable<Discussion> GetAllFor(int userId)
        {
            return instantce.Discussions.Where(x => x.CreatorId == userId).OrderByDescending(x => x.Id).ToArray();
        }

        public IEnumerable<Discussion> GetByPagination(int start, int limit)
        {
            return instantce.Discussions.OrderByDescending(x => x.Id).Skip(start).Take(limit);
        }

        public IEnumerable<Discussion> GetByPaginationFor(int userId, int start, int limit)
        {
            return instantce.Discussions.Where(x => x.Id == userId).OrderByDescending(x => x.Id).Skip(start).Take(limit);
        }

        public void Update(Discussion entity)
        {
            entity.UpdatedTime = DateTime.UtcNow;
            instantce.Discussions.Update(entity);
            instantce.SaveChanges();
        }
    }
}
