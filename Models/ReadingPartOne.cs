using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TCU.English.Models
{
    public class ReadingPartOne : BaseEntity
    {
        [DisplayName("Question Text")]
        [Required]
        public string QuestionText { get; set; }
        [DisplayName("Explain")]
        public string Hint { get; set; }
        [Required]
        public string Answers { get; set; }
        [DisplayName("Explain Link")]
        public string ExplainLink { get; set; }
        public int CreatorId { get; set; }

        [ForeignKey(nameof(CreatorId))]
        [JsonIgnore]
        public virtual User User { get; set; }
        public int TestCategoryId { get; set; }

        [ForeignKey(nameof(TestCategoryId))]
        [JsonIgnore]
        public virtual TestCategory TestCategory { get; set; }

        [NotMapped]
        public List<BaseAnswer> AnswerList { get; set; }
    }
}
