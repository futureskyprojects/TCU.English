using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCU.English.Models
{
    public class Vocabulary : BaseEntity
    {
        // Từ thuộc topic gì
        public int TopicId { get; set; }
        public string Word { get; set; }
        public string Spelling { get; set; }
        public string TypeOfWord { get; set; }
        public string Use { get; set; }

        // Dẫn khóa đến topic
        public virtual Topic Topic { get; set; }
    }
}
