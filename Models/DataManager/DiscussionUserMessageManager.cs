using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            throw new NotImplementedException();
        }

        public long Count()
        {
            throw new NotImplementedException();
        }

        public void Delete(DiscussionUserMessage entity)
        {
            throw new NotImplementedException();
        }

        public DiscussionUserMessage Get(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DiscussionUserMessage> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DiscussionUserMessage> GetByPagination(int start, int limit)
        {
            throw new NotImplementedException();
        }

        public void Update(DiscussionUserMessage entity)
        {
            throw new NotImplementedException();
        }
    }
}
