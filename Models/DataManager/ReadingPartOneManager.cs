using System;
using System.Collections.Generic;
using System.Linq;
using TCU.English.Models.Repository;

namespace TCU.English.Models.DataManager
{
    public class ReadingPartOneManager : IDataRepository<ReadingPartOne>
    {
        public readonly SystemDatabaseContext instantce;
        public ReadingPartOneManager(SystemDatabaseContext instantce)
        {
            this.instantce = instantce;
        }

        public void Add(ReadingPartOne entity)
        {
            entity.CreatedTime = DateTime.Now;
            entity.UpdatedTime = DateTime.Now;
            entity.Active = true;
            instantce.ReadingPartOnes.Add(entity);
            instantce.SaveChanges();
        }

        public long Count()
        {
            return instantce.ReadingPartOnes.Count();
        }

        public void Delete(ReadingPartOne entity)
        {
            instantce.ReadingPartOnes.Remove(entity);
            instantce.SaveChanges();
        }

        public ReadingPartOne Get(long id)
        {
            return instantce.ReadingPartOnes.First(it => it.Id == id);
        }

        public IEnumerable<ReadingPartOne> GetAll()
        {
            return instantce.ReadingPartOnes.ToList();
        }

        public IEnumerable<ReadingPartOne> GetByPagination(long categoryId, int start, int limit)
        {
            return instantce.ReadingPartOnes.Where(x => x.TestCategoryId == categoryId).OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }

        public IEnumerable<ReadingPartOne> GetByPagination(int start, int limit)
        {
            return instantce.ReadingPartOnes.OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }

        public void Update(ReadingPartOne entity)
        {
            entity.UpdatedTime = DateTime.Now;
            instantce.ReadingPartOnes.Update(entity);
            instantce.SaveChanges();
        }
    }
}
