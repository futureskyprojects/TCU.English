using System;
using System.Collections.Generic;
using System.Linq;
using TCU.English.Models.Repository;

namespace TCU.English.Models.DataManager
{
    public class SpeakingEmbedManager : IDataRepository<SpeakingEmbed>
    {
        public readonly SystemDatabaseContext instantce;
        public SpeakingEmbedManager(SystemDatabaseContext instantce)
        {
            this.instantce = instantce;
        }

        public void Add(SpeakingEmbed entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.UpdatedTime = DateTime.UtcNow;
            entity.Active = true;
            instantce.SpeakingEmbeds.Add(entity);
            instantce.SaveChanges();
        }

        public long Count()
        {
            return instantce.SpeakingEmbeds.Count();
        }

        public void Delete(SpeakingEmbed entity)
        {
            instantce.SpeakingEmbeds.Remove(entity);
            instantce.SaveChanges();
        }

        public SpeakingEmbed Get(long id)
        {
            return instantce.SpeakingEmbeds.First(it => it.Id == id);
        }
        public SpeakingEmbed GetByCategoryId(long categoryId)
        {
            return instantce.SpeakingEmbeds.FirstOrDefault(it => it.TestCategoryId == categoryId);
        }

        public IEnumerable<SpeakingEmbed> GetAll(long testCategoryId)
        {
            return instantce.SpeakingEmbeds.Where(x => x.TestCategoryId == testCategoryId).ToList();
        }
        public IEnumerable<SpeakingEmbed> GetAll()
        {
            return instantce.SpeakingEmbeds.ToList();
        }

        public IEnumerable<SpeakingEmbed> GetByPagination(long categoryId, int start, int limit)
        {
            return instantce.SpeakingEmbeds.Where(x => x.TestCategoryId == categoryId).OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }

        public IEnumerable<SpeakingEmbed> GetByPagination(int start, int limit)
        {
            return instantce.SpeakingEmbeds.OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }

        public void Update(SpeakingEmbed entity)
        {
            entity.UpdatedTime = DateTime.UtcNow;
            instantce.SpeakingEmbeds.Update(entity);
            instantce.SaveChanges();
        }
    }
}
