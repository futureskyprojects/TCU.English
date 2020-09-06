using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCU.English.Models
{
    public class ReadingPartTwo : BaseEntity
    {
        [DisplayName("Question Text")]
        public string QuestionText { get; set; } = "";
        [DisplayName("Question Image")]
        public string QuestionImage { get; set; } = "";
        public string Hint { get; set; } = "";
        [Required]
        public string Answers { get; set; } = "";
        [DisplayName("Explain Link")]
        public string ExplainLink { get; set; } = "";
        public int CreatorId { get; set; }

        [ForeignKey(nameof(CreatorId))]
        [JsonIgnore]
        public virtual User User { get; set; }
        public int TestCategoryId { get; set; }

        [ForeignKey(nameof(TestCategoryId))]
        [JsonIgnore]
        public virtual TestCategory TestCategory { get; set; }

        [NotMapped]
        [JsonIgnore]
        public List<BaseAnswer> AnswerList { get; set; }

        public static List<ReadingPartTwo> Generate(int size, int answerSize = 4)
        {
            List<ReadingPartTwo> readingPartTwos = new List<ReadingPartTwo>();
            for (int i = 0; i < size; i++)
            {
                var temp = new ReadingPartTwo
                {
                    Answers = JsonConvert.SerializeObject(BaseAnswer.Generate(answerSize)),
                    AnswerList = BaseAnswer.Generate(answerSize)
                };
                readingPartTwos.Add(temp);
            }
            return readingPartTwos;
        }
    }
}
