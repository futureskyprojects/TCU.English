using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCU.English.Models
{
    public class TestCategory : BaseEntity
    {
        #region Test fixed Type
        [JsonIgnore]
        public const string LISTENING = "LISTENING";
        [JsonIgnore]
        public const string READING = "READING";
        [JsonIgnore]
        public const string WRITING = "WRITING";
        [JsonIgnore]
        public const string SPEAKING = "SPEAKING";
        [JsonIgnore]
        public const string TEST_ALL = "TEST_ALL";
        [JsonIgnore]
        public static string[] Types = new string[] { LISTENING, READING, SPEAKING, WRITING };
        #endregion
        [Required]
        public string TypeCode { get; set; }
        [Required]
        public int PartId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayName("WYSIWYG Content")]
        public string WYSIWYGContent { get; set; }
        public int CreatorId { get; set; }

        [ForeignKey(nameof(CreatorId))]
        [JsonIgnore]
        public virtual User User { get; set; }

        [JsonIgnore]
        public virtual ICollection<ReadingPartOne> ReadingPartOnes { get; set; }

        [JsonIgnore]
        public virtual ICollection<ReadingPartTwo> ReadingPartTwos { get; set; }

        [JsonIgnore]
        public virtual ICollection<ListeningBaseQuestion> ListeningBaseQuestions { get; set; }

        [JsonIgnore]
        public virtual ICollection<ListeningMedia> ListeningMedias { get; set; }

        [JsonIgnore]
        public virtual ICollection<WritingPartOne> WritingPartOnes { get; set; }
        public virtual ICollection<WritingPartTwo> WritingPartTwos { get; set; }
        public virtual ICollection<SpeakingEmbed> SpeakingEmbeds { get; set; }
        // ======================================================== //
        public TestCategory()
        {

        }
        public TestCategory(string TypeCode, int PartId, string Name, string Description)
        {
            this.TypeCode = TypeCode;
            this.PartId = PartId;
            this.Name = Name;
            this.Description = Description;
        }

        public static TestCategory ListeningCategory(int partId, string name = "", string description = "")
        {
            return new TestCategory(LISTENING, partId, name, description);
        }
        public static TestCategory ReadingCategory(int partId, string name = "", string description = "")
        {
            return new TestCategory(READING, partId, name, description);
        }
        public static TestCategory WritingCategory(int partId, string name = "", string description = "")
        {
            return new TestCategory(WRITING, partId, name, description);
        }
        public static TestCategory SpeakingCategory(int partId, string name = "", string description = "")
        {
            return new TestCategory(SPEAKING, partId, name, description);
        }
    }
}
