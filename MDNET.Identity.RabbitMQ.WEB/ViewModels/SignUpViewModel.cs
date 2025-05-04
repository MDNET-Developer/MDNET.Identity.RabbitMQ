using System.ComponentModel.DataAnnotations;

namespace MDNET.Identity.RabbitMQ.Web.ViewModels
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage ="İstifadəçi adını daxil edin")]
        [Display(Name ="İstifadəçi adı:")]
        public string? UserName { get; set; }

        [Display(Name = "Email:")]
        [Required(ErrorMessage = "E-poçt adını daxil edin")]
        public string? Email { get; set; }

        [Display(Name = "Telefon:")]
        [Required(ErrorMessage = "Mobil nömrənizi  daxil edin")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Şifrə:")]
        [Required(ErrorMessage = "Şifrəni daxil edin")]
        public string? Password { get; set; }

        [Display(Name = "Təkrar şifrə:")]
        [Required(ErrorMessage = "Təkrar şifrəni daxil edin")]
        [Compare(nameof(Password),ErrorMessage ="Təkrar şifrə eyni deyil")]
        public string? PasswordConfirm { get; set; }
    }
}
