using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TCU.English.Models
{
    // Đối tượng thể hiện tin nhắn của mỗi người trong cuộc hội thoại
    public class DiscussionUserMessage : BaseEntity
    {
        // Mã người gửi trong nhóm
        public int SenderId { get; set; }

        // Nội dung tin nhắn
        public string Message { get; set; }

        [ForeignKey(nameof(SenderId))]
        public virtual DiscussionUser DiscussionUser { get; set; }
    }
}
