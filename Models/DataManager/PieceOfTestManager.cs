using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TCU.English.Models.PiceOfTest;
using TCU.English.Models.Repository;
using TCU.English.Utils;

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
            entity.UpdatedTime = null;
            entity.Active = true;
            instantce.PieceOfTests.Add(entity);
            instantce.SaveChanges();
        }

        public long Count()
        {
            return instantce.PieceOfTests.Count();
        }

        /// <summary>
        /// Đếm tất cả các học viên chọn giáo viên này
        /// </summary>
        /// <param name="instructorId"></param>
        /// <returns></returns>
        public long CountAllStudentOfInstructor(int instructorId)
        {
            try
            {
                return instantce.User
                         .Join(
                         instantce.PieceOfTests,
                         user => user.Id,
                         piceOfTest => piceOfTest.UserId,
                         (u, pot) => new { u, pot })
                         .Where(full => full.pot.InstructorId == instructorId)
                         .Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Đếm tất cả các GVHD mà HV này đã chọn
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public long CountAllInstructorOfStudent(int studentId)
        {
            try
            {
                return instantce.User
                         .Join(
                         instantce.PieceOfTests,
                         user => user.Id,
                         piceOfTest => piceOfTest.InstructorId,
                         (u, pot) => new { u, pot })
                         .Where(full => full.pot.UserId == studentId)
                         .Count();
            }
            catch (Exception)
            {
                return 0;
            }
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
            float thresholdListening = ScoresUtils.GetThresholdPoint(TestCategory.LISTENING);
            float thresholdReading = ScoresUtils.GetThresholdPoint(TestCategory.READING);
            float thresholdSpeaking = ScoresUtils.GetThresholdPoint(TestCategory.SPEAKING);
            float thresholdWriting = ScoresUtils.GetThresholdPoint(TestCategory.WRITING);
            float thresholdGeneral = ScoresUtils.GetThresholdPoint(TestCategory.TEST_ALL);
            return instantce.PieceOfTests
                .Where(x => x.UserId == userId &&
                !string.IsNullOrEmpty(x.ResultOfUserJson) &&
                ((x.TypeCode == TestCategory.LISTENING && x.Scores >= thresholdListening) ||
                (x.TypeCode == TestCategory.READING && x.Scores >= thresholdReading) ||
                (x.TypeCode == TestCategory.SPEAKING && x.Scores >= thresholdSpeaking) ||
                (x.TypeCode == TestCategory.WRITING && x.Scores >= thresholdWriting) ||
                (x.TypeCode == TestCategory.TEST_ALL && x.Scores >= thresholdGeneral))).Count();
        }
        public long FaildTestsCount(long userId)
        {
            float thresholdListening = ScoresUtils.GetThresholdPoint(TestCategory.LISTENING);
            float thresholdReading = ScoresUtils.GetThresholdPoint(TestCategory.READING);
            float thresholdSpeaking = ScoresUtils.GetThresholdPoint(TestCategory.SPEAKING);
            float thresholdWriting = ScoresUtils.GetThresholdPoint(TestCategory.WRITING);
            float thresholdGeneral = ScoresUtils.GetThresholdPoint(TestCategory.TEST_ALL);
            return instantce.PieceOfTests
                .Where(x => x.UserId == userId &&
                !string.IsNullOrEmpty(x.ResultOfUserJson) &&
                ((x.TypeCode == TestCategory.LISTENING && x.Scores < thresholdListening) ||
                (x.TypeCode == TestCategory.READING && x.Scores < thresholdReading) ||
                (x.TypeCode == TestCategory.SPEAKING && x.Scores < thresholdSpeaking) ||
                (x.TypeCode == TestCategory.WRITING && x.Scores < thresholdWriting) ||
                (x.TypeCode == TestCategory.TEST_ALL && x.Scores < thresholdGeneral))).Count();
        }

        /// <summary>
        /// Phương thức tính điểm số trung bình
        /// </summary>
        /// <param name="userId">Mã của người cần tính</param>
        /// <param name="typeCode">Mã string của mục cần tính điểm trung bình. Để trống để tính điểm trung bình tổng</param>
        /// <returns></returns>
        public float CalculateGPA(long userId, string typeCode = "ALL")
        {
            long totalPiceOfTests = 0;
            float totalScores = 0;
            if (typeCode.ToUpper() == "ALL")
            {
                float gpaAvrg = CalculateGPA(userId, TestCategory.LISTENING) +
                    CalculateGPA(userId, TestCategory.READING) +
                    CalculateGPA(userId, TestCategory.WRITING) +
                    CalculateGPA(userId, TestCategory.SPEAKING);
                return gpaAvrg;
            }
            else
            {
                // Tính điểm kỹ năng theo cho từng phần
                var pieceOfTests = instantce
                    .PieceOfTests
                    .Select(x => new PieceOfTest
                    {
                        UserId = x.UserId,
                        TypeCode = x.TypeCode,
                        ResultOfUserJson = x.ResultOfUserJson,
                        Scores = x.Scores
                    })
                    .Where(x => x.UserId == userId && x.TypeCode.ToUpper() == typeCode.ToUpper() && x.ResultOfUserJson != null && x.ResultOfUserJson.Length > 0 && x.Scores >= 0);
                totalPiceOfTests = pieceOfTests.Count();
                totalScores = pieceOfTests.Where(x => x.Scores >= 0).Sum(x => x.Scores);

                try
                {
                    // Lấy điềm kỹ năng được tính từ bài thi tổng
                    pieceOfTests = instantce.PieceOfTests.Where(x =>
                        x.UserId == userId &&
                        x.TypeCode.ToUpper() == TestCategory.TEST_ALL &&
                        x.ResultOfUserJson != null &&
                        x.ResultOfUserJson.Length > 0);

                    // Nếu là bài thi đọc
                    if (typeCode == TestCategory.LISTENING)
                    {
                        totalPiceOfTests += pieceOfTests.Count();
                        totalScores += pieceOfTests.Sum(x => JsonConvert.DeserializeObject<GeneralTestPaper>(x.ResultOfUserJson).TotalReadingScores());
                    }
                    else if (typeCode == TestCategory.READING)
                    {
                        totalPiceOfTests += pieceOfTests.Count();
                        totalScores += pieceOfTests.Sum(x => JsonConvert.DeserializeObject<GeneralTestPaper>(x.ResultOfUserJson).TotalListeningScores());
                    }
                    else if (typeCode == TestCategory.WRITING)
                    {
                        pieceOfTests = pieceOfTests.Where(x => x.Scores >= 0);
                        totalPiceOfTests += pieceOfTests.Count();
                        totalScores += pieceOfTests.Sum(x => JsonConvert.DeserializeObject<GeneralTestPaper>(x.ResultOfUserJson).TotalWritingScores());
                    }
                    else if (typeCode == TestCategory.SPEAKING)
                    {
                        pieceOfTests = pieceOfTests.Where(x => x.Scores >= 0);
                        totalPiceOfTests += pieceOfTests.Count();
                        totalScores += pieceOfTests.Sum(x => JsonConvert.DeserializeObject<GeneralTestPaper>(x.ResultOfUserJson).SpeakingTestPaper.SpeakingPart.Scores.ToFloat());
                    }
                }
                catch (Exception)
                {

                }

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
        public long StudentTestCountOfType(long instructorId, string typeCode, string searchKey = "", long studentId = -1, bool isUnRead = false, bool isNotFailTest = false)
        {
            try
            {
                return QueryableOfTestsForInstructor(instructorId, typeCode)
                        .Where(x => x.ResultOfTestJson.Contains(searchKey) &&
                            (studentId <= 0 || x.UserId == studentId) &&
                            (!isUnRead || (x.Scores < 0 || string.IsNullOrEmpty(x.InstructorComments))) &&
                            (!isNotFailTest || !string.IsNullOrEmpty(x.ResultOfUserJson))
                            )
                        .Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public long UserTestCountOfType(long userId, string typeCode, string searchKey = "", int instructorId = -1)
        {
            try
            {
                var x = QueryableOfUserTest(userId, typeCode)
                    .Where(x => x.ResultOfTestJson.Contains(searchKey) && (instructorId <= 0 || x.InstructorId == instructorId))
                    .Count();
                return x;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public IEnumerable<PieceOfTest> GetByPagination(long userId, string typeCode, int start, int limit, string searchKey = "", int instructorId = -1)
        {
            var query = QueryableOfUserTest(userId, typeCode);
            if (query == null)
                return new List<PieceOfTest>();
            return query.Where(x =>
                    (x.ResultOfTestJson.Contains(searchKey) || searchKey.Contains(x.Id.ToString())) &&
                    (instructorId <= 0 || x.InstructorId == instructorId) &&
                    !string.IsNullOrEmpty(x.ResultOfUserJson))
                .Select(x => new PieceOfTest
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
        public IEnumerable<PieceOfTest> GetByPaginationSimpleForInstructor(long instructorId, string typeCode, int start, int limit, string searchKey = "", int studentId = -1, bool isUnRead = false)
        {
            var query = QueryableOfTestsForInstructor(instructorId, typeCode);
            if (query == null)
                return new List<PieceOfTest>();
            return query.Where(x => x.ResultOfTestJson.Contains(searchKey) &&
            (studentId <= 0 || studentId == x.UserId) &&
            !string.IsNullOrEmpty(x.ResultOfUserJson) &&
            (!isUnRead || (x.Scores < 0 || string.IsNullOrEmpty(x.InstructorComments))))
                .Select(x => new PieceOfTest
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


        public string GetUserResult(long potId)
        {
            return instantce.PieceOfTests
                .Where(pot => pot.Id == potId)
                .Select(x => x.ResultOfUserJson)
                .FirstOrDefault();
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
                    Instructor = x.Instructor,
                    Scores = x.PiceOfTest.Scores
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
            if (entity.UpdatedTime == null)
                entity.UpdatedTime = DateTime.UtcNow;
            instantce.PieceOfTests.Update(entity);
            instantce.SaveChanges();
        }
    }
}
