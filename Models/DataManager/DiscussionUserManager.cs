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
            throw new NotImplementedException();
        }

        public long Count()
        {
            throw new NotImplementedException();
        }

        public void Delete(DiscussionUser entity)
        {
            throw new NotImplementedException();
        }

        public DiscussionUser Get(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DiscussionUser> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DiscussionUser> GetByPagination(int start, int limit)
        {
            throw new NotImplementedException();
        }

        public void Update(DiscussionUser entity)
        {
            throw new NotImplementedException();
        }
    }
}
