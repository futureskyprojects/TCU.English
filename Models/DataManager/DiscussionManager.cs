using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Models.Repository;

namespace TCU.English.Models.DataManager
{
    public class DiscussionManager : IDataRepository<Discussion>
    {
        public readonly SystemDatabaseContext instantce;
        public DiscussionManager(SystemDatabaseContext instantce)
        {
            this.instantce = instantce;
        }

        public void Add(Discussion entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.UpdatedTime = DateTime.UtcNow;

            entity.Active = true;
            instantce.Discussions.Add(entity);
            instantce.SaveChanges();
        }

        public long Count()
        {
            return instantce.Discussions.Count();
        }

        public long CountAllFor(int userId)
        {
            return instantce.Discussions.Where(x => x.Id == userId).Count();
        }

        public void Delete(Discussion entity)
        {
            instantce.Discussions.Remove(entity);
            instantce.SaveChanges();
        }

        public Discussion Get(long id)
        {
            return instantce.Discussions.FirstOrDefault(x => x.Id == id);
        }

        public long GetFirstMemberId(long discussId)
        {
            var query = instantce.Discussions
                .Join(instantce.DiscussionUsers,
                d => d.Id,
                du => du.DiscussionId,
                (d, du) => new { d, du })
                .Where(z => z.d.Id == discussId);
            if (query.Any(x => x.d.CreatorId != x.du.UserId))
                return query.Select(x => x.du.UserId).FirstOrDefault();
            else
                return query.Select(x => x.d.CreatorId).FirstOrDefault();
        }

        // Kiểm tra xem người dùng có trong cuộc hội thoại không
        public bool IsIn(long discussId, long userId)
        {
            return instantce.Discussions
                .Join(instantce.DiscussionUsers,
                d => d.Id,
                du => du.DiscussionId,
                (d, du) => new { d, du })
                .Any(x => x.d.Id == discussId &&
                (x.du.UserId == userId || x.d.CreatorId == userId));
        }

        public IEnumerable<Discussion> GetAll()
        {
            return instantce.Discussions.ToList();
        }

        public IEnumerable<Discussion> GetAllFor(int userId)
        {
            return instantce.Discussions.Where(x => x.CreatorId == userId).OrderByDescending(x => x.Id).ToArray();
        }

        public IEnumerable<Discussion> GetByPagination(int start, int limit)
        {
            return instantce.Discussions.OrderByDescending(x => x.Id).Skip(start).Take(limit);
        }

        public long CountUnReadFor(int dmId)
        {
            // Nếu id sai
            if (dmId <= 0) return 0;

            // Lấy model cũ
            return instantce.Discussions
                .Join(instantce.DiscussionUsers,
                d => d.Id,
                du => du.DiscussionId,
                (d, du) => new { d, du })
                .Join(instantce.DiscussionUserMessages,
                ddu => ddu.du.Id,
                dum => dum.SenderId,
                (ddu, dum) => new { ddu, dum })
                .Count();

        }

        internal Discussion GetExistP2PDiscuss(int currentUser, int friendId)
        {
            return instantce.Discussions
                .Join(instantce.DiscussionUsers,
                d => d.Id,
                du => du.DiscussionId,
                (d, du) => new { d, du })
                .Where(x => (x.du.UserId == friendId && x.d.CreatorId == currentUser) ||
                (x.du.UserId == currentUser && x.d.CreatorId == friendId))
                .Select(x => x.d)
                .FirstOrDefault<Discussion>();
        }

        public IEnumerable<Discussion> GetByPaginationFor(int userId, int start, int limit)
        {
            return instantce.Discussions
                .Include(x => x.DiscussionUsers)
                .Join(instantce.DiscussionUsers,
                d => d.Id,
                du => du.DiscussionId,
                (d, du) => new { d, du })
                .Where(x => x.d.CreatorId == userId || x.du.UserId == userId)
                .Select(x => x.d)
                .OrderByDescending(x => x.Id)
                .Skip(start)
                .Take(limit)
                .ToList();
        }

        public void Update(Discussion entity)
        {
            entity.UpdatedTime = DateTime.UtcNow;
            instantce.Discussions.Update(entity);
            instantce.SaveChanges();
        }
    }
}
