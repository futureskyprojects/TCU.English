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
            throw new NotImplementedException();
        }

        public long Count()
        {
            throw new NotImplementedException();
        }

        public void Delete(Discussion entity)
        {
            throw new NotImplementedException();
        }

        public Discussion Get(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Discussion> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Discussion> GetByPagination(int start, int limit)
        {
            throw new NotImplementedException();
        }

        public void Update(Discussion entity)
        {
            throw new NotImplementedException();
        }
    }
}
