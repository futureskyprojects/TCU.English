﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TCU.English.Utils;

namespace TCU.English.Models
{
    public class PieceOfTest : BaseEntity
    {
        [Required]
        public string TypeCode { get; set; }
        [Required]
        public int PartId { get; set; }
        [Required]
        public string ResultOfUserJson { get; set; } = "";
        [Required]
        public string ResultOfTestJson { get; set; } = "";
        public float Scores { get; set; } = 0;
        public float TimeToFinished { get; set; } = -1;
        /// <summary>
        /// Đánh giá của giáo viên hướng dẫn
        /// </summary>
        public string InstructorComments { get; set; }
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public virtual User User { get; set; }

        /// <summary>
        /// Mã giáo viên hướng dẫn, có thể có hoặc không
        /// </summary>
        public int? InstructorId { get; set; }

        [ForeignKey(nameof(InstructorId))]
        [JsonIgnore]
        public virtual User Instructor { get; set; }

        public bool IsPassed()
        {
            return Scores >= ScoresUtils.GetThresholdPoint(TypeCode);
        }
    }
}
