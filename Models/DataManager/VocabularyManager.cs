using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Models.Repository;

namespace TCU.English.Models.DataManager
{
    public class VocabularyManager : IDataRepository<Vocabulary>
    {
        private readonly SystemDatabaseContext instance;

        public VocabularyManager(SystemDatabaseContext instance)
        {
            this.instance = instance;
        }

        public void Add(Vocabulary entity)
        {
            instance.Vocabularies.Add(entity);
            instance.SaveChanges();
        }

        public long Count()
        {
            return instance.Vocabularies.Count();
        }

        public void Delete(Vocabulary entity)
        {
            instance.Vocabularies.Remove(entity);
            instance.SaveChanges();
        }

        public Vocabulary Get(long id)
        {
            return instance.Vocabularies.Find(id);
        }

        public IEnumerable<Vocabulary> GetAll()
        {
            return instance.Vocabularies.AsEnumerable();
        }

        public IEnumerable<Vocabulary> GetByPagination(int start, int limit)
        {
            return instance.Vocabularies.OrderByDescending(x => x.Id).Skip(start).Take(limit);
        }

        public void Update(Vocabulary entity)
        {
            instance.Vocabularies.Update(entity);
            instance.SaveChanges();
        }
    }
}
