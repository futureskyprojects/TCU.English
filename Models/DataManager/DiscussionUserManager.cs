using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English.Models.DataManager
{
    public class DiscussionUserManager : IDataRepository<DiscussionUser>
    {
        public readonly SystemDatabaseContext instantce;
        public DiscussionUserManager(SystemDatabaseContext instantce)
        {
            this.instantce = instantce;
        }

        public void Add(DiscussionUser entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.UpdatedTime = DateTime.UtcNow;
            entity.Active = false;

            instantce.DiscussionUsers.Add(entity);
            instantce.SaveChanges();
        }

        public long Count()
        {
            return instantce.DiscussionUsers.Count();
        }

        public void Delete(DiscussionUser entity)
        {
            instantce.DiscussionUsers.Remove(entity);
            instantce.SaveChanges();
        }

        public DiscussionUser Get(long id)
        {
            return instantce.DiscussionUsers.FirstOrDefault(x => x.Id == id);
        }

        public DiscussionUser GetBy(long discussId, long userId)
        {
            // Lấy người dùng là thành viên trong nhóm
            DiscussionUser du = instantce.DiscussionUsers.FirstOrDefault(x => x.DiscussionId == discussId && x.UserId == userId);

            // Có thì trả về luôn
            if (du != null && du.Id > 0)
                return du;

            // Nếu không có khì thêm chính người tạo vào
            du = new DiscussionUser
            {
                DiscussionId = discussId.ToInt(),
                UserId = userId.ToInt()
            };
            Add(du);
            return du;
        }

        public IEnumerable<DiscussionUser> GetAll()
        {
            return instantce.DiscussionUsers.ToList();
        }

        public IEnumerable<DiscussionUser> GetByPagination(int start, int limit)
        {
            return instantce.DiscussionUsers.OrderByDescending(x => x.Id).Skip(start).Take(limit);
        }

        public void Update(DiscussionUser entity)
        {
            entity.UpdatedTime = DateTime.UtcNow;

            instantce.DiscussionUsers.Update(entity);
            instantce.SaveChanges();
        }
    }
}
