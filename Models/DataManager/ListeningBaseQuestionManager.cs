using System;
using System.Collections.Generic;
using System.Linq;
using TCU.English.Models.Repository;

namespace TCU.English.Models.DataManager
{
    public class ListeningBaseQuestionManager : IDataRepository<ListeningBaseQuestion>
    {
        public readonly SystemDatabaseContext instantce;
        public ListeningBaseQuestionManager(SystemDatabaseContext instantce)
        {
            this.instantce = instantce;
        }

        public void Add(ListeningBaseQuestion entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.UpdatedTime = DateTime.UtcNow;
            entity.Active = true;
            instantce.ListeningBaseQuestions.Add(entity);
            instantce.SaveChanges();
        }

        public long Count()
        {
            return instantce.ListeningBaseQuestions.Count();
        }

        public void Delete(ListeningBaseQuestion entity)
        {
            instantce.ListeningBaseQuestions.Remove(entity);
            instantce.SaveChanges();
        }

        public ListeningBaseQuestion Get(long id)
        {
            return instantce.ListeningBaseQuestions.First(it => it.Id == id);
        }

        public IEnumerable<ListeningBaseQuestion> GetAll()
        {
            return instantce.ListeningBaseQuestions.ToList();
        }
        public IEnumerable<ListeningBaseQuestion> GetAll(long categoryId)
        {
            return instantce.ListeningBaseQuestions.Where(x => x.TestCategoryId == categoryId).ToList();
        }
        public IEnumerable<ListeningBaseQuestion> GetByPagination(string testTypeCode, long partId, int start, int limit)
        {
            return instantce.ListeningBaseQuestions
                .Join(instantce.TestCategories,
                listening => listening.TestCategoryId,
                category => category.Id,
                (listening, category) => new { listening, category })
                .Where(full => full.category.TypeCode.ToLower() == testTypeCode.ToLower() && full.category.PartId == partId)
                .Select(full => full.listening)
                .OrderByDescending(x => x.Id)
                .Skip(start)
                .Take(limit)
                .ToList();
        }
        public IEnumerable<ListeningBaseQuestion> GetByPagination(long categoryId, string testTypeCode, long partId, int start, int limit)
        {
            return instantce.ListeningBaseQuestions
                .Join(instantce.TestCategories,
                listening => listening.TestCategoryId,
                category => category.Id,
                (listening, category) => new { listening, category })
                .Where(full => full.category.Id == categoryId && full.category.TypeCode.ToLower() == testTypeCode.ToLower() && full.category.PartId == partId)
                .Select(full => full.listening)
                .OrderByDescending(x => x.Id)
                .Skip(start)
                .Take(limit)
                .ToList();
        }

        public IEnumerable<ListeningBaseQuestion> GetByPagination(int start, int limit)
        {
            return instantce.ListeningBaseQuestions.OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }

        public void Update(ListeningBaseQuestion entity)
        {
            entity.UpdatedTime = DateTime.UtcNow;
            instantce.ListeningBaseQuestions.Update(entity);
            instantce.SaveChanges();
        }
    }
}
