using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Models.Repository;
using TCU.English.Utils;

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

        public bool Exists(long topicId, string word)
        {
            return instance.Vocabularies.Any(x => x.Word.Trim().ToLower().Equals(word.Trim().ToLower()) && x.TopicId == topicId);
        }
        public long Count()
        {
            return instance.Vocabularies.Count();
        }

        public long CountFor(long topicId)
        {
            if (topicId <= 0)
                return Count();

            return instance.Vocabularies.Where(x => x.TopicId == topicId).Count();
        }

        public void Delete(Vocabulary entity)
        {
            instance.Vocabularies.Remove(entity);
            instance.SaveChanges();
        }

        public Vocabulary Get(long id)
        {
            return instance.Vocabularies.Find(id.ToInt());
        }

        public IEnumerable<Vocabulary> LookUp(string vocabulary)
        {
            if (string.IsNullOrEmpty(vocabulary))
                return new List<Vocabulary>();

            vocabulary = vocabulary.Trim().ToLower();
            return instance.Vocabularies.Where(x => x.Word.Trim().Contains(vocabulary) || vocabulary.Contains(x.Word.Trim())).ToList();
        }

        public IEnumerable<Vocabulary> GetAll()
        {
            return instance.Vocabularies.ToList();
        }

        public IEnumerable<Vocabulary> GetAllFor(long topicId)
        {
            return instance.Vocabularies.Where(x => x.TopicId == topicId).ToList();
        }

        public IEnumerable<Vocabulary> GetByPagination(int start, int limit)
        {
            return instance.Vocabularies.OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }
        public IEnumerable<Vocabulary> GetByPagination(long topicId, int start, int limit)
        {
            if (topicId <= 0)
                return GetByPagination(start, limit);

            return instance.Vocabularies
                .Select(x => new Vocabulary
                {
                    Id = x.Id,
                    TopicId = x.TopicId,
                    Word = x.Word
                })
                .Where(x => x.TopicId == topicId)
                .OrderByDescending(x => x.Id)
                .Skip(start)
                .Take(limit)
                .ToList();
        }

        public void Update(Vocabulary entity)
        {
            instance.Vocabularies.Update(entity);
            instance.SaveChanges();
        }
    }
}
