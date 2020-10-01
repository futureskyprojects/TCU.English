using System;
using System.Collections.Generic;
using System.Linq;
using TCU.English.Models.Repository;

namespace TCU.English.Models.DataManager
{
    public class WritingPartOneManager : IDataRepository<WritingPartOne>
    {
        public readonly SystemDatabaseContext instantce;
        public WritingPartOneManager(SystemDatabaseContext instantce)
        {
            this.instantce = instantce;
        }

        public void Add(WritingPartOne entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.UpdatedTime = DateTime.UtcNow;
            entity.Active = true;
            instantce.WritingPartOnes.Add(entity);
            instantce.SaveChanges();
        }

        public long Count()
        {
            return instantce.WritingPartOnes.Count();
        }

        public int CountAll(int categoryId)
        {
            return instantce.WritingPartOnes
                .Join(instantce.TestCategories,
                    wtp1 => wtp1.TestCategoryId,
                    tc => tc.Id,
                    (wtp1, tc) => new { wtp1, tc })
                .Where(
                    f => f.tc.TypeCode.Equals(TestCategory.WRITING) &&
                    (categoryId <= 0 || f.tc.Id == categoryId)
                ).Select(s => s.wtp1).Count();
        }

        public void Delete(WritingPartOne entity)
        {
            instantce.WritingPartOnes.Remove(entity);
            instantce.SaveChanges();
        }

        public WritingPartOne Get(long id)
        {
            return instantce.WritingPartOnes.First(it => it.Id == id);
        }

        public IEnumerable<WritingPartOne> GetAll()
        {
            return instantce.WritingPartOnes.ToList();
        }

        public IEnumerable<WritingPartOne> GetByPagination(long categoryId, int start, int limit)
        {
            return instantce.WritingPartOnes.Where(x => x.TestCategoryId == categoryId).OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }

        public IEnumerable<WritingPartOne> GetByPagination(int start, int limit)
        {
            return instantce.WritingPartOnes.OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }

        public void Update(WritingPartOne entity)
        {
            entity.UpdatedTime = DateTime.UtcNow;
            instantce.WritingPartOnes.Update(entity);
            instantce.SaveChanges();
        }
    }
}
