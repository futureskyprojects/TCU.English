using System;
using System.Collections.Generic;
using System.Linq;
using TCU.English.Models.Repository;

namespace TCU.English.Models.DataManager
{
    public class ReadingPartTwoManager : IDataRepository<ReadingPartTwo>
    {
        public readonly SystemDatabaseContext instantce;
        public ReadingPartTwoManager(SystemDatabaseContext instantce)
        {
            this.instantce = instantce;
        }

        public void Add(ReadingPartTwo entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.UpdatedTime = DateTime.UtcNow;
            entity.Active = true;
            instantce.ReadingPartTwos.Add(entity);
            instantce.SaveChanges();
        }

        public long Count()
        {
            return instantce.ReadingPartTwos.Count();
        }

        public void Delete(ReadingPartTwo entity)
        {
            instantce.ReadingPartTwos.Remove(entity);
            instantce.SaveChanges();
        }

        public ReadingPartTwo Get(long id)
        {
            return instantce.ReadingPartTwos.First(it => it.Id == id);
        }

        public IEnumerable<ReadingPartTwo> GetAll()
        {
            return instantce.ReadingPartTwos.ToList();
        }
        public IEnumerable<ReadingPartTwo> GetAll(long categoryId)
        {
            return instantce.ReadingPartTwos.Where(x => x.TestCategoryId == categoryId).ToList();
        }
        public IEnumerable<ReadingPartTwo> GetByPagination(string testTypeCode, long partId, int start, int limit)
        {
            return instantce.ReadingPartTwos
                .Join(instantce.TestCategories,
                reading => reading.TestCategoryId,
                category => category.Id,
                (reading, category) => new { reading, category })
                .Where(full => full.category.TypeCode.ToLower() == testTypeCode.ToLower() && full.category.PartId == partId)
                .Select(full => full.reading)
                .OrderByDescending(x => x.Id)
                .Skip(start)
                .Take(limit)
                .ToList();
        }
        public IEnumerable<ReadingPartTwo> GetByPagination(long categoryId, string testTypeCode, long partId, int start, int limit)
        {
            return instantce.ReadingPartTwos
                .Join(instantce.TestCategories,
                reading => reading.TestCategoryId,
                category => category.Id,
                (reading, category) => new { reading, category })
                .Where(full => full.category.Id == categoryId && full.category.TypeCode.ToLower() == testTypeCode.ToLower() && full.category.PartId == partId)
                .Select(full => full.reading)
                .OrderByDescending(x => x.Id)
                .Skip(start)
                .Take(limit)
                .ToList();
        }

        public IEnumerable<ReadingPartTwo> GetByPagination(int start, int limit)
        {
            return instantce.ReadingPartTwos.OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }

        public void Update(ReadingPartTwo entity)
        {
            entity.UpdatedTime = DateTime.UtcNow;
            instantce.ReadingPartTwos.Update(entity);
            instantce.SaveChanges();
        }
    }
}
