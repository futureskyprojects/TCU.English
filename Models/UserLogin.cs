using System.ComponentModel.DataAnnotations;

namespace TCU.English.Models
{
    public class UserLogin
    {
        [Required, MinLength(5), MaxLength(30)]
        public string Identity { get; set; }
        [Required, MinLength(5), MaxLength(128)]
        public string Password { get; set; }
        public bool IsRemember { get; set; } = true;
        public string RequestPath { get; set; }
    }
}
