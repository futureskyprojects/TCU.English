using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCU.English.Models
{
    public class ListeningMedia : BaseEntity
    {
        public string Audio { get; set; }
        public string Transcript { get; set; }
        public int TestCategoryId { get; set; }

        [ForeignKey(nameof(TestCategoryId))]
        [JsonIgnore]
        public virtual TestCategory TestCategory { get; set; }

        public static ListeningMedia Generate()
        {
            return new ListeningMedia
            {
                Audio = "",
                Transcript = ""
            };
        }
    }
}
