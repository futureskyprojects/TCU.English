using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TCU.English.Models.Repository;

namespace TCU.English.Models.DataManager
{
    public class UserNoteManager : IDataRepository<UserNote>
    {
        public readonly SystemDatabaseContext instantce;
        public UserNoteManager(SystemDatabaseContext instantce)
        {
            this.instantce = instantce;
        }

        public void Add(UserNote entity)
        {
            entity.CreatedTime = DateTime.Now;
            entity.UpdatedTime = DateTime.Now;
            entity.Active = true;
            instantce.UserNotes.Add(entity);
            instantce.SaveChanges();
        }

        public long Count()
        {
            return instantce.UserNotes.Count();
        }

        public void Delete(UserNote entity)
        {
            instantce.UserNotes.Remove(entity);
            instantce.SaveChanges();
        }

        public string GetWYSIWYGContent(long id)
        {
            return instantce.UserNotes.First(it => it.Id == id)
                .WYSIWYGContent;
        }

        public UserNote Get(long id)
        {
            return instantce.UserNotes.First(it => it.Id == id);
        }

        public IEnumerable<UserNote> GetAll()
        {
            return instantce.UserNotes.ToList();
        }

        public IEnumerable<UserNote> GetAll(int userId)
        {
            return instantce.UserNotes.Where(it => it.UserId == userId).ToList();
        }

        public int CountFor(int userId)
        {
            return instantce.UserNotes.Where(x => x.UserId == userId).Count();
        }

        public IEnumerable<UserNote> GetByPagination(int userId, int start, int limit, string key = "")
        {
            key ??= "";
            key = key.Trim().ToLower();
            return instantce
                .UserNotes
                .Where(it =>
                    it.UserId == userId &&
                    (it.WYSIWYGContent.Trim().ToLower().Contains(key) ||
                    key.Contains(it.WYSIWYGContent.Trim().ToLower())))
                .OrderByDescending(x => x.Id)
                .Skip(start)
                .Take(limit)
                .ToList();
        }

        public IEnumerable<UserNote> GetByPagination(int start, int limit)
        {
            return instantce.UserNotes.OrderByDescending(x => x.Id).Skip(start).Take(limit).ToList();
        }

        public void Update(UserNote entity)
        {
            entity.UpdatedTime = DateTime.Now;
            instantce.UserNotes.Update(entity);
            instantce.SaveChanges();
        }
    }
}
