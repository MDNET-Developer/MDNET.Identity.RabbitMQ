using MDNET.Identity.RabbitMQ.Web.ErrorsDescriber;
using MDNET.Identity.RabbitMQ.Web.Models;
using MDNET.Identity.RabbitMQ.Web.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
namespace MDNET.Identity.RabbitMQ.Web.Extensions
{
    public static class CustomProgramcs
    {
        public static void AddDbContextExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
        }
        public static void AddRabbitMQClientServiceSingleton(this IServiceCollection services, IConfiguration configuration)
        {
            var uri = configuration.GetSection("RabbitMQ")["URI"];
            services.AddSingleton(x => new ConnectionFactory()
            {
                Uri = new Uri(uri),
                DispatchConsumersAsync = true//Asinxron metod isletditimiz ucun
            });
        }
        public static void CustomServices(this IServiceCollection services)
        {
            services.AddSingleton<RabbitMQClientService>();
            services.AddSingleton<RabbitMQPublisher>();
        }
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

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Blok müddəti
                options.Lockout.MaxFailedAccessAttempts = 7; // Maksimum uğursuz cəhd sayı
                options.Lockout.AllowedForNewUsers = true; // Yeni istifadəçilər üçün aktiv olsunmu?
            }).AddEntityFrameworkStores<AppDbContext>().AddErrorDescriber<CustomIdentityErrorDescriber>();
        }

        public static void AddCookieConfigure(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(14);// Sessiyanın ümumi müddəti 14 gün
                options.SlidingExpiration = true; // Sessiya istifadəçi aktiv olduğu müddətcə yenilənəcək
                options.LoginPath = "/User/LogIn";
                options.Cookie.SameSite = SameSiteMode.Lax; // CSRF hücumlarına qarşı tədbir (Lax, Strict, None)
                options.Cookie.Name = "MyApp.Auth.Cookie"; // Cookie faylının adı
                options.Cookie.HttpOnly = true; // JavaScript ilə cookie-yə girişin qarşısı alınır (təhlükəsizlik)


            });
        }
    }
}
