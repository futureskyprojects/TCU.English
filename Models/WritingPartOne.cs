using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCU.English.Models
{
    public class WritingPartOne : BaseEntity
    {
        [DisplayName("Default sentence")]
        [Required]
        public string DefaultSentence { get; set; }
        [DisplayName("Second sentence")]
        [Required]
        public string SecondSentence { get; set; }
        public string Hint { get; set; }
        public string Answers { get; set; }
        [NotMapped]
        public List<BaseAnswer> BaseAnswers { get; set; } = BaseAnswer.Generate(Config.MAX_WRITING_PART_1_QUESTION);
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
    }
}
