using Microsoft.AspNetCore.Identity;

namespace MDNET.Identity.RabbitMQ.Web.Models
{
    public class AppUser : IdentityUser
    {
        public string? CityName { get; set; }
    }
}
