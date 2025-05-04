using MDNET.Identity.RabbitMQ.Web.ErrorsDescriber;
using MDNET.Identity.RabbitMQ.Web.Models;
using Microsoft.Extensions.Options;

namespace MDNET.Identity.RabbitMQ.Web.Extensions
{
    public static class CustomProgramcs
    {
        public static void AddIdentityWithPolicy(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                // Minimum uzunluq
                options.Password.RequiredLength = 8;

                // Ən az bir rəqəm tələb olunur
                options.Password.RequireDigit = true;

                // Kiçik hərf tələb olunur
                options.Password.RequireLowercase = true;

                // Böyük hərf tələb olunur
                options.Password.RequireUppercase = true;

                // Xüsusi simvol tələb olunur (məsələn: !, @, #)
                options.Password.RequireNonAlphanumeric = true;

                // Unikal simvol sayı (ən azı neçə fərqli simvol istifadə olunmalıdır)
                //options.Password.RequiredUniqueChars = 4;
            }).AddEntityFrameworkStores<AppDbContext>().AddErrorDescriber<CustomIdentityErrorDescriber>();
        }

        public static void AddCookieConfigure(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(14);// Sessiyanın ümumi müddəti 14 gün
                options.SlidingExpiration = true; // Sessiya istifadəçi aktiv olduğu müddətcə yenilənəcək
                options.LoginPath = "/User/Login";
            });
        }
    }
}
