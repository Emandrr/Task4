using System.ComponentModel.DataAnnotations;

namespace Task4.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Не указан Login")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
