using System.ComponentModel.DataAnnotations;

namespace MDNET.Identity.RabbitMQ.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email daxil edilməlidir.")]
        [EmailAddress(ErrorMessage = "Düzgün email ünvanı daxil edin.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Şifrə daxil edilməlidir.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
