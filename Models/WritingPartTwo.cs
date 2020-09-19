using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCU.English.Models
{
    public class WritingPartTwo : BaseEntity
    {
        [DisplayName("Questions")]
        [Required]
        public string Questions { get; set; }
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
