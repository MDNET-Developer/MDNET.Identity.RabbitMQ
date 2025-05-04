using System.ComponentModel.DataAnnotations;

namespace MDNET.Identity.RabbitMQ.Web.ViewModels
{
    public class SignUpViewModel
    {
        [Display(Name ="İstifadəçi adı:")]
        public string? UserName { get; set; }

        [Display(Name = "Email:")]
        public string? Email { get; set; }

        [Display(Name = "Telefon:")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Şifrə:")]
        public string? Password { get; set; }

        [Display(Name = "Təkrar şifrə:")]
        public string? PasswordConfirm { get; set; }
    }
}
