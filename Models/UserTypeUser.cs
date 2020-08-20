using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCU.English.Models
{
    public class UserTypeUser : BaseEntity
    {
        public int UserId { get; set; }
        public int UserTypeId { get; set; }
        public virtual User User { get; set; }
        public virtual UserType UserType { get; set; }

    }
}
