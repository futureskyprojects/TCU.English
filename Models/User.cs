using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TCU.English.Models
{
    public class User : BaseEntity
    {
        public User()
        {
            UserTypeUser = new HashSet<UserTypeUser>();
        }
        [Required, MinLength(5), MaxLength(30)]
        public string Username { get; set; }
        [Required, MinLength(4), MaxLength(125), EmailAddress]
        public string Email { get; set; }
        [Required, PasswordPropertyText, MinLength(8), MaxLength(128)]
        public string HashPassword { get; set; }
        [Required, MinLength(1), MaxLength(15)]
        public string FirstName { get; set; }
        [Required, MinLength(1), MaxLength(30)]
        public string LastName { get; set; }
        public string Avatar { get; set; }
        [Required]
        public int Gender { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime BirthDay { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserTypeUser> UserTypeUser { get; set; }

        [JsonIgnore]
        public virtual ICollection<TestCategory> TestCategories { get; set; }

        [JsonIgnore]
        public virtual ICollection<ReadingPartOne> ReadingPartOnes { get; set; }

        [JsonIgnore]
        public virtual ICollection<WritingPartOne> WritingPartOnes { get; set; }

        [JsonIgnore]
        public static (int, string)[] Genders = new (int, string)[]
        {
            (1, "Male"),
            (2, "Female"),
            (-1, "Ignore")
        };
    }
}
