using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCU.English.Models
{
    public class ReadingPartOne : BaseEntity
    {
        [Required]
        public string QuestionText { get; set; }
        public string Hint { get; set; }
        [Required]
        public string Answers { get; set; }
        public string ExplainLink { get; set; }
        public int CreatorId { get; set; }
        [ForeignKey(nameof(CreatorId))]
        public virtual User User { get; set; }
        public int TestCategoryId { get; set; }
        [ForeignKey(nameof(TestCategoryId))]
        public virtual TestCategory TestCategory { get; set; }
    }
}
