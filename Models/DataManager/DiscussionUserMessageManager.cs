using System;
using System.Collections.Generic;
using System.Linq;
using TCU.English.Models.Repository;

namespace TCU.English.Models.DataManager
{
    public class DiscussionUserMessageManager : IDataRepository<DiscussionUserMessage>
    {
        public readonly SystemDatabaseContext instantce;
        public DiscussionUserMessageManager(SystemDatabaseContext instantce)
        {
            this.instantce = instantce;
        }

        public void Add(DiscussionUserMessage entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.UpdatedTime = DateTime.UtcNow;
            entity.Active = false;
            instantce.DiscussionUserMessages.Add(entity);
            instantce.SaveChanges();
        }

        public long Count()
        {
            return instantce.DiscussionUserMessages.Count();
        }

        public void Delete(DiscussionUserMessage entity)
        {
            instantce.DiscussionUserMessages.Remove(entity);
            instantce.SaveChanges();
        }

        public DiscussionUserMessage Get(long id)
        {
            return instantce.DiscussionUserMessages.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<DiscussionUserMessage> GetAll()
        {
            return instantce.DiscussionUserMessages.ToList();
        }

        public IEnumerable<DiscussionUserMessage> GetByPagination(int start, int limit)
        {
            return instantce.DiscussionUserMessages.OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }

        public void Update(DiscussionUserMessage entity)
        {
            entity.UpdatedTime = DateTime.UtcNow;
            instantce.DiscussionUserMessages.Update(entity);
            instantce.SaveChanges();
        }
    }
}
