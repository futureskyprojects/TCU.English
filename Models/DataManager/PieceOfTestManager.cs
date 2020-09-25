﻿using System;
using System.Collections.Generic;
using System.Linq;
using TCU.English.Models.Repository;

namespace TCU.English.Models.DataManager
{
    public class PieceOfTestManager : IDataRepository<PieceOfTest>
    {
        public readonly SystemDatabaseContext instantce;
        public PieceOfTestManager(SystemDatabaseContext instantce)
        {
            this.instantce = instantce;
        }

        public void Add(PieceOfTest entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.UpdatedTime = DateTime.UtcNow;
            entity.Active = true;
            instantce.PieceOfTests.Add(entity);
            instantce.SaveChanges();
        }

        public long Count()
        {
            return instantce.PieceOfTests.Count();
        }
        public long CountOfInstructor(int instructorId)
        {
            return instantce.PieceOfTests.Where(x => x.InstructorId != null && x.InstructorId == instructorId).Count();
        }

        #region FOR HOME INDEX
        public long CompletedTestsCount(long userId)
        {
            try
            {
                return instantce.PieceOfTests.Where(x => x.UserId == userId && x.ResultOfUserJson != null && x.ResultOfUserJson.Length > 0).Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public long PassedTestsCount(long userId)
        {
            try
            {
                return instantce.PieceOfTests.Where(x => x.UserId == userId && x.ResultOfUserJson != null && x.ResultOfUserJson.Length > 0 && x.Scores >= Config.THRESHOLD_POINT).Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public long FaildTestsCount(long userId)
        {
            try
            {
                return instantce.PieceOfTests.Where(x => x.UserId == userId && x.ResultOfUserJson != null && x.ResultOfUserJson.Length > 0 && x.Scores < Config.THRESHOLD_POINT).Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public float CalculateGPA(long userId, string typeCode = "ALL")
        {
            long totalPiceOfTests = 0;
            float totalScores = 0;
            if (typeCode.ToUpper() == "ALL")
            {
                totalPiceOfTests = CompletedTestsCount(userId);
                totalScores = instantce.PieceOfTests.Where(x => x.UserId == userId && x.ResultOfUserJson != null && x.ResultOfUserJson.Length > 0).Sum(x => x.Scores);
            }
            else
            {
                var pieceOfTests = instantce.PieceOfTests.Where(x => x.UserId == userId && x.TypeCode.ToUpper() == typeCode.ToUpper() && x.ResultOfUserJson != null && x.ResultOfUserJson.Length > 0);
                totalPiceOfTests = pieceOfTests.Count();
                totalScores = pieceOfTests.Sum(x => x.Scores);
            }
            if (totalPiceOfTests > 0)
                return (totalScores / ((float)totalPiceOfTests));
            else
                return 0;
        }
        #endregion

        private IQueryable<PieceOfTest> QueryableOfUserTest(long userId, string typeCode)
        {
            IQueryable<PieceOfTest> query = null;
            if (typeCode.ToUpper() == "ALL".ToUpper())
            {
                query = instantce.PieceOfTests.Where(x => x.UserId == userId);
            }
            else if (typeCode.ToUpper() == "CRASH".ToUpper())
            {
                query = instantce.PieceOfTests.Where(x =>
                            x.UserId == userId &&
                            (x.ResultOfUserJson == null ||
                            x.ResultOfUserJson.Length <= 0));
            }
            else
            {
                query = instantce.PieceOfTests
                    .Where(x =>
                            x.UserId == userId &&
                            x.ResultOfUserJson != null &&
                            x.ResultOfUserJson.Length > 0 &&
                            x.TypeCode.ToUpper().Trim() == typeCode.ToUpper().Trim());
            }
            return query;
        }
        private IQueryable<PieceOfTest> QueryableOfTestsForInstructor(long instructorId, string typeCode)
        {
            IQueryable<PieceOfTest> query = null;
            if (typeCode.ToUpper() == "ALL".ToUpper())
            {
                query = instantce.PieceOfTests.Where(x => x.InstructorId == instructorId);
            }
            else if (typeCode.ToUpper() == "CRASH".ToUpper())
            {
                query = instantce.PieceOfTests.Where(x =>
                            x.InstructorId == instructorId &&
                            (x.ResultOfUserJson == null ||
                            x.ResultOfUserJson.Length <= 0));
            }
            else
            {
                query = instantce.PieceOfTests
                    .Where(x =>
                            x.InstructorId == instructorId &&
                            x.ResultOfUserJson != null &&
                            x.ResultOfUserJson.Length > 0 &&
                            x.TypeCode.ToUpper().Trim() == typeCode.ToUpper().Trim());
            }
            return query;
        }

        #region FOR TEST HISTORY
        public long StudentTestCountOfType(long instructorId, string typeCode, string searchKey = "", long studentId = -1)
        {
            try
            {
                return QueryableOfTestsForInstructor(instructorId, typeCode)
                        .Where(x => x.ResultOfTestJson.Contains(searchKey) &&
                            (studentId <= 0 || x.UserId == studentId))
                        .Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public long UserTestCountOfType(long userId, string typeCode, string searchKey = "")
        {
            try
            {
                var x = QueryableOfUserTest(userId, typeCode)
                    .Where(x => x.ResultOfTestJson.Contains(searchKey))
                    .Count();
                return x;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public IEnumerable<PieceOfTest> GetByPagination(long userId, string typeCode, int start, int limit, string searchKey = "")
        {
            var query = QueryableOfUserTest(userId, typeCode);
            if (query == null)
                return new List<PieceOfTest>();
            return query.Where(x => x.ResultOfTestJson.Contains(searchKey)).OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }
        public IEnumerable<PieceOfTest> GetByPaginationSimple(long userId, string typeCode, int start, int limit, string searchKey = "")
        {
            var query = QueryableOfUserTest(userId, typeCode);
            if (query == null)
                return new List<PieceOfTest>();
            return query.Where(x => x.ResultOfTestJson.Contains(searchKey)).Select(x => new PieceOfTest
            {
                Id = x.Id,
                CreatedTime = x.CreatedTime,
                TypeCode = x.TypeCode,
                Scores = x.Scores,
                ResultOfUserJson = (x.ResultOfUserJson != null && x.ResultOfUserJson.Length > 0) ? "OK" : "",
                UserId = x.UserId,
                InstructorId = x.InstructorId,
                InstructorComments = !string.IsNullOrEmpty(x.InstructorComments) ? "Have" : ""
            }).OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }
        public IEnumerable<PieceOfTest> GetByPaginationSimpleForInstructor(long instructorId, string typeCode, int start, int limit, string searchKey = "", int studentId = -1)
        {
            var query = QueryableOfTestsForInstructor(instructorId, typeCode);
            if (query == null)
                return new List<PieceOfTest>();
            return query.Where(x => x.ResultOfTestJson.Contains(searchKey) && (studentId <= 0 || studentId == x.UserId)).Select(x => new PieceOfTest
            {
                Id = x.Id,
                CreatedTime = x.CreatedTime,
                TypeCode = x.TypeCode,
                Scores = x.Scores,
                ResultOfUserJson = (x.ResultOfUserJson != null && x.ResultOfUserJson.Length > 0) ? "OK" : "",
                UserId = x.UserId,
                InstructorId = x.InstructorId,
                InstructorComments = !string.IsNullOrEmpty(x.InstructorComments) ? "Have" : ""
            }).OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }
        #endregion

        public void Delete(PieceOfTest entity)
        {
            instantce.PieceOfTests.Remove(entity);
            instantce.SaveChanges();
        }

        public string GetTestType(int id)
        {
            return instantce.PieceOfTests.Where(x => x.Id == id).Select(x => x.TypeCode).FirstOrDefault();
        }

        public PieceOfTest Get(long id)
        {
            return instantce.PieceOfTests.FirstOrDefault(it => it.Id == id);
        }

        public PieceOfTest GetForInstructorTool(long id)
        {
            return instantce.PieceOfTests
                .Where(it => it.Id == id)
                .Join(instantce.User,
                    pot => pot.InstructorId,
                    u => u.Id,
                    (pot, u) => new { PiceOfTest = pot, Instructor = u })
                .Select(x => new PieceOfTest
                {
                    UserId = x.PiceOfTest.UserId,
                    InstructorId = x.PiceOfTest.InstructorId,
                    TypeCode = x.PiceOfTest.TypeCode,
                    InstructorComments = x.PiceOfTest.InstructorComments,
                    Instructor = x.Instructor
                }).FirstOrDefault();
        }

        public IEnumerable<PieceOfTest> GetAll()
        {
            return instantce.PieceOfTests.ToList();
        }

        public IEnumerable<PieceOfTest> GetByPagination(int start, int limit)
        {
            return instantce.PieceOfTests.OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }

        public void Update(PieceOfTest entity)
        {
            entity.UpdatedTime = DateTime.UtcNow;
            instantce.PieceOfTests.Update(entity);
            instantce.SaveChanges();
        }
    }
}
