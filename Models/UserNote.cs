using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCU.English.Models
{
    public class UserNote : BaseEntity
    {
        public int UserId { get; set; }
        [DisplayName("Note content")]

        [Required(ErrorMessage = "You must input name for note")]
        public string Note { get; set; }

        [Required(ErrorMessage = "You must input content for note")]
        public string WYSIWYGContent { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}
