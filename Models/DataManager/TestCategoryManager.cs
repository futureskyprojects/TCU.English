using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TCU.English.Models.Repository;
using TCU.English.Utils.PasswordUtils;

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
            var category = instantce.TestCategories.Where(x => x.TypeCode == TestCategory.LISTENING);
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
            return instantce.TestCategories
                .Include(x => x.ReadingPartOnes)
                .Include(x => x.ReadingPartTwos)
                .Where(it =>
                    it.TypeCode.ToLower() == type.ToLower() &&
                    it.PartId == partId &&
                    ((it.ReadingPartOnes.Count >= minQuestions) ||
                    it.ReadingPartTwos.Count >= minQuestions))
                .ToList();
        }
        public IEnumerable<TestCategory> GetAll(string type, int partId)
        {
            return instantce.TestCategories.Where(it => it.TypeCode.ToLower() == type.ToLower() && it.PartId == partId).ToList();
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
