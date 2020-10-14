using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TCU.English.Models.Repository;

namespace TCU.English.Models.DataManager
{
    public class TestCategoryManager : IDataRepository<TestCategory>
    {
        public readonly SystemDatabaseContext instantce;
        public TestCategoryManager(SystemDatabaseContext instantce)
        {
            this.instantce = instantce;
        }

        public void Add(TestCategory entity)
        {
            entity.CreatedTime = DateTime.Now;
            entity.UpdatedTime = DateTime.Now;
            entity.Active = true;
            instantce.TestCategories.Add(entity);
            instantce.SaveChanges();
        }

        public bool IsExists(TestCategory entity)
        {
            return instantce.TestCategories.Any(iterator => iterator.Name.ToLower() == entity.Name.ToLower() && iterator.TypeCode.ToLower() == entity.TypeCode.ToLower() && iterator.PartId == entity.PartId);
        }
        public long ListeningQuestionCount()
        {
            var category = instantce
                .TestCategories
                .Where(x => x.TypeCode == TestCategory.LISTENING);
            return category.Sum(x => x.ListeningBaseQuestions.Count);
        }

        public long ReadingQuestionCount()
        {
            var category = instantce.TestCategories.Where(x => x.TypeCode == TestCategory.READING);
            long part1Count = category.Sum(x => x.ReadingPartOnes.Count);
            long part2Count = category.Sum(x => x.ReadingPartTwos.Count);

            return part1Count + part2Count;
        }

        public long SpeakingQuestionCount()
        {
            var category = instantce.TestCategories.Where(x => x.TypeCode == TestCategory.SPEAKING);
            return category.Sum(x => x.SpeakingEmbeds.Count);
        }

        public long WritingQuestionCount()
        {
            var category = instantce.TestCategories.Where(x => x.TypeCode == TestCategory.WRITING);
            return category.Sum(x => x.WritingPartOnes.Count) +
                category.Sum(x => x.WritingPartTwos.Count);
        }

        public long Count()
        {
            return instantce.TestCategories.Count();
        }

        public void Delete(TestCategory entity)
        {
            instantce.TestCategories.Remove(entity);
            instantce.SaveChanges();
        }

        public string GetWYSIWYGContent(long id)
        {
            return instantce.TestCategories.First(it => it.Id == id)
                .WYSIWYGContent;
        }

        public TestCategory Get(long id)
        {
            return instantce.TestCategories.First(it => it.Id == id);
        }

        public IEnumerable<TestCategory> GetAll()
        {
            return instantce.TestCategories.ToList();
        }
        public IEnumerable<TestCategory> GetForGenerateTest(string type, int partId, int minQuestions = 1)
        {
            var query = instantce.TestCategories
                .Where(it =>
                    it.TypeCode.ToLower() == type.ToLower() &&
                    it.PartId == partId);

            if (type == TestCategory.READING && partId == 1)
                return query.Include(x => x.ReadingPartOnes).Where(x => x.ReadingPartOnes.Count >= minQuestions).ToList();
            if (type == TestCategory.READING && partId >= 2)
                return query.Include(x => x.ReadingPartTwos).Where(x => x.ReadingPartTwos.Count >= minQuestions).ToList();

            if (type == TestCategory.LISTENING)
                return query.Include(x => x.ListeningBaseQuestions).Where(x => x.ListeningBaseQuestions.Count >= minQuestions).ToList();

            if (type == TestCategory.WRITING && partId == 1)
                return query.Include(x => x.WritingPartOnes).Where(x => x.WritingPartOnes.Count >= minQuestions).ToList();
            if (type == TestCategory.WRITING && partId == 2)
                return query.Include(x => x.WritingPartTwos).Where(x => x.WritingPartTwos.Count >= minQuestions).ToList();

            if (type == TestCategory.SPEAKING)
                return query.Include(x => x.SpeakingEmbeds).Where(x => x.SpeakingEmbeds.Count >= minQuestions).ToList();

            return null;
        }
        public IEnumerable<TestCategory> GetAll(string type, int partId)
        {
            return instantce.TestCategories.Where(it => it.TypeCode.ToLower() == type.ToLower() && it.PartId == partId).ToList();
        }

        public int CountFor(string type, int partId)
        {
            return instantce.TestCategories.Where(it => it.TypeCode.ToLower() == type.ToLower() && it.PartId == partId).Count();
        }

        public IEnumerable<TestCategory> GetByPagination(string type, int partId, int start, int limit)
        {
            return instantce.TestCategories.Where(it => it.TypeCode.ToLower() == type.ToLower() && it.PartId == partId).OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }

        public IEnumerable<TestCategory> GetByPagination(int start, int limit)
        {
            return instantce.TestCategories.OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }

        public void Update(TestCategory entity)
        {
            entity.UpdatedTime = DateTime.Now;
            instantce.TestCategories.Update(entity);
            instantce.SaveChanges();
        }
    }
}
