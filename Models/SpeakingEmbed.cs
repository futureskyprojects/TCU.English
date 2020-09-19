using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCU.English.Models
{
    public class SpeakingEmbed : BaseEntity
    {
        [DisplayName("Youtube Video Address")]
        [Required]
        [Url]
        public string YoutubeVideo { get; set; }
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
