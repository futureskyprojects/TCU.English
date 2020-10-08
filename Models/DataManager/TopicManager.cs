using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Linq;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Models.DataManager
{
    public class TopicManager : IDataRepository<Topic>
    {
        private readonly SystemDatabaseContext context;
        public TopicManager(SystemDatabaseContext context)
        {
            this.context = context;
        }
        public void Add(Topic entity)
        {
            context.Topics.Add(entity);
            context.SaveChanges();
        }

        public bool Exists(string topicName)
        {
            return context.Topics.Any(x => x.Name.Trim().ToLower().Equals(topicName.Trim().ToLower()));
        }

        public long Count()
        {
            return context.Topics.Count();
        }

        public void Delete(Topic entity)
        {
            context.Topics.Remove(entity);
            context.SaveChanges();
        }

        public Topic Get(long id)
        {
            return context.Topics.Where(x => x.Id == id).FirstOrDefault();
        }

        public IEnumerable<Topic> GetAll()
        {
            return context.Topics.AsEnumerable();
        }

        public IEnumerable<Topic> GetByPagination(int start, int limit)
        {
            return context.Topics.OrderByDescending(x => x.Id).Skip(start).Take(limit).AsEnumerable();
        }

        public void Update(Topic entity)
        {
            context.Topics.Update(entity);
            context.SaveChanges();
        }
    }
}
