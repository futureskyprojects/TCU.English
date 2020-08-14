using System;
using System.Collections.Generic;

namespace TCU.English.Models
{
    public class User : BaseEntity
    {
        public User()
        {
            UserTypeUser = new HashSet<UserTypeUser>();
        }
        public string Username { get; set; }
        public string Email { get; set; }
        public string HashPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public int Gender { get; set; }
        public DateTime BirthDay { get; set; }
        public virtual ICollection<UserTypeUser> UserTypeUser { get; set; }
    }
}
