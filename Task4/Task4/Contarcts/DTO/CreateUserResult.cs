using Microsoft.AspNetCore.Identity;
using Task4.Models;

namespace Task4.Contarcts.DTO
{
    public record CreateUserResult
    {
        public IdentityResult Result { get; set; }
        public User User { get; set; }
    }
}
