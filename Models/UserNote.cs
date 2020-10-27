using System.ComponentModel.DataAnnotations.Schema;

namespace TCU.English.Models
{
    public class UserNote:BaseEntity
    {
        public int UserId { get; set; }
        public string WYSIWYGContent { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}
