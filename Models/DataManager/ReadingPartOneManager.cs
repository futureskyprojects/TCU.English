using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TCU.English.Models.PiceOfTest;
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
            entity.CreatedTime = DateTime.UtcNow;
            entity.UpdatedTime = DateTime.UtcNow;
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
        public IEnumerable<ReadingPartOne> GetAll(long categoryId)
        {
            if (categoryId <= 0)
                return GetAll();
            return instantce.ReadingPartOnes.Where(x => x.TestCategoryId == categoryId).ToList();
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
            entity.UpdatedTime = DateTime.UtcNow;
            instantce.ReadingPartOnes.Update(entity);
            instantce.SaveChanges();
        }

        internal List<ReadingPartOne> TakeRandom()
        {
            Random rand = new Random();
            int size = instantce.ReadingPartOnes.Count();

            List<ReadingPartOne> readings = new List<ReadingPartOne>();

            // Lấy từng câu hỏi trong bộ câu hỏi cho đủ 10 câu
            while (true)
            {
                // Nếu đã đủ 10 câu trong bộ rồi thì thoát ra
                if (readings.Count >= ReadingTestPaper.MAX_QUESTION_READING_PART_1)
                    break;

                // Tiến hành trộn và bốc ngẫu nhiên
                int toSkip = rand.Next(0, size);
                var res = instantce.ReadingPartOnes
                    .Include(x => x.TestCategory)
                    .Skip(toSkip).Take(1)
                    .FirstOrDefault();
                if (res != null && !readings.Any(x => x.Id == res.Id))
                {
                    //res.Hint = res.TestCategory?.Name;
                    //res.ExplainLink = res.TestCategory?.Name;
                    readings.Add(res);
                }
            }

            return readings;
        }
    }
}
