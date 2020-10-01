using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TCU.English.Models
{
    // Đối tượng thể hiện thành viên trong cuộc hội thoại
    public class DiscussionUser : BaseEntity
    {
        // Mã cuộc hội thoại mà người dùng tham gia
        public int DiscussionId { get; set; }

        // Mã người dùng là thành viên của cuộc hội thoại
        public int UserId { get; set; }

        [ForeignKey(nameof(DiscussionId))]
        public virtual Discussion Discussion { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        // Mỗi thành viên trong nhóm có thể gửi nhiều tin nhắn
        public virtual ICollection<DiscussionUserMessage> DiscussionUserMessages { get; set; }
    }
}
