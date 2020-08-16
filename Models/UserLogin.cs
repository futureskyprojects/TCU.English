namespace TCU.English.Models
{
    public class UserLogin
    {
        public string Identity { get; set; }
        public string Password { get; set; }
        public bool IsRemember { get; set; } = true;
    }
}
