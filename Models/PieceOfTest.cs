using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TCU.English.Models
{
    public class PieceOfTest : BaseEntity
    {
        [Required]
        public string TypeCode { get; set; }
        [Required]
        public int PartId { get; set; }
        [Required]
        public string ResultOfUserJson { get; set; }
        [Required]
        public string ResultOfTestJson { get; set; }
        public float Scores { get; set; } = 0;
        public float TimeToFinished { get; set; } = -1;
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}
