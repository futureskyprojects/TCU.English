using System;
using System.Collections.Generic;
using System.Linq;
using TCU.English.Models.Repository;

namespace TCU.English.Models.DataManager
{
    public class ListeningMediaManager : IDataRepository<ListeningMedia>
    {
        public readonly SystemDatabaseContext instantce;
        public ListeningMediaManager(SystemDatabaseContext instantce)
        {
            this.instantce = instantce;
        }

        public void Add(ListeningMedia entity)
        {
            entity.CreatedTime = DateTime.Now;
            entity.UpdatedTime = DateTime.Now;
            entity.Active = true;
            instantce.ListeningMedias.Add(entity);
            instantce.SaveChanges();
        }

        public long Count()
        {
            return instantce.ListeningMedias.Count();
        }

        public void Delete(ListeningMedia entity)
        {
            instantce.ListeningMedias.Remove(entity);
            instantce.SaveChanges();
        }

        public ListeningMedia Get(long id)
        {
            return instantce.ListeningMedias.First(it => it.Id == id);
        }
        public ListeningMedia GetByCategory(TestCategory testCategory)
        {
            return GetByCategory(testCategory.Id);
        }
        public ListeningMedia GetByCategory(long categoryId)
        {
            return instantce.ListeningMedias.First(it => it.TestCategoryId == categoryId);
        }

        public IEnumerable<ListeningMedia> GetAll()
        {
            return instantce.ListeningMedias.ToList();
        }
        public IEnumerable<ListeningMedia> GetAll(long categoryId)
        {
            return instantce.ListeningMedias.Where(x => x.TestCategoryId == categoryId).ToList();
        }
   
        public IEnumerable<ListeningMedia> GetByPagination(int start, int limit)
        {
            return instantce.ListeningMedias.OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }

        public void Update(ListeningMedia entity)
        {
            entity.UpdatedTime = DateTime.Now;
            instantce.ListeningMedias.Update(entity);
            instantce.SaveChanges();
        }
    }
}
