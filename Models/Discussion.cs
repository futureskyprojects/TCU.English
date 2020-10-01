using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TCU.English.Models
{
    // Đối tượng cuộc hội thoại
    public class Discussion : BaseEntity
    {
        // Mã người tạo
        public int CreatorId { get; set; }

        [ForeignKey(nameof(CreatorId))]
        public virtual User User { get; set; }

        // Mỗi cuộc hội thoại có thể có nhiều thành viên tham dự
        public virtual ICollection<DiscussionUser> DiscussionUsers { get; set; }
    }
}
