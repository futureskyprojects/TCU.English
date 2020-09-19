using System;
using System.Collections.Generic;
using System.Linq;
using TCU.English.Models.Repository;

namespace TCU.English.Models.DataManager
{
    public class WritingPartTwoManager : IDataRepository<WritingPartTwo>
    {
        public readonly SystemDatabaseContext instantce;
        public WritingPartTwoManager(SystemDatabaseContext instantce)
        {
            this.instantce = instantce;
        }

        public void Add(WritingPartTwo entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.UpdatedTime = DateTime.UtcNow;
            entity.Active = true;
            instantce.WritingPartTwos.Add(entity);
            instantce.SaveChanges();
        }

        public long Count()
        {
            return instantce.WritingPartTwos.Count();
        }

        public void Delete(WritingPartTwo entity)
        {
            instantce.WritingPartTwos.Remove(entity);
            instantce.SaveChanges();
        }

        public WritingPartTwo Get(long id)
        {
            return instantce.WritingPartTwos.First(it => it.Id == id);
        }
        public WritingPartTwo GetByCategoryId(long categoryId)
        {
            return instantce.WritingPartTwos.FirstOrDefault(it => it.TestCategoryId == categoryId);
        }

        public IEnumerable<WritingPartTwo> GetAll(long testCategoryId)
        {
            return instantce.WritingPartTwos.Where(x => x.TestCategoryId == testCategoryId).ToList();
        }
        public IEnumerable<WritingPartTwo> GetAll()
        {
            return instantce.WritingPartTwos.ToList();
        }

        public IEnumerable<WritingPartTwo> GetByPagination(long categoryId, int start, int limit)
        {
            return instantce.WritingPartTwos.Where(x => x.TestCategoryId == categoryId).OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }

        public IEnumerable<WritingPartTwo> GetByPagination(int start, int limit)
        {
            return instantce.WritingPartTwos.OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }

        public void Update(WritingPartTwo entity)
        {
            entity.UpdatedTime = DateTime.UtcNow;
            instantce.WritingPartTwos.Update(entity);
            instantce.SaveChanges();
        }
    }
}
