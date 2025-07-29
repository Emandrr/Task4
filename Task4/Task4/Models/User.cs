using Microsoft.AspNetCore.Identity;

namespace Task4.Models
{
    public class User :IdentityUser
    {
        public virtual string? Login { get; set; }
        public virtual bool? IsEnable { get; set; }
        public virtual DateTime? LastLoginTime { get; set; }
        public virtual DateTime? RegistrationDate { get; set; }
    }
}
