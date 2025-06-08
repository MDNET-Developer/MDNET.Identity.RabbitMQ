using MDNET.Identity.RabbitMQ.Web.ErrorsDescriber;
using MDNET.Identity.RabbitMQ.Web.Models;
using Microsoft.EntityFrameworkCore;
using MDNET.Identity.RabbitMQ.Web.Extensions;
using Microsoft.Extensions.Options;
using MDNET.Identity.RabbitMQ.Web.Confugration;
using MDNET.Identity.RabbitMQ.Web.Services;
using MDNET.Identity.RabbitMQ.Web.Hubs;

namespace MDNET.Identity.RabbitMQ.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.Configure<RabbitMQConfugration>(builder.Configuration.GetSection("RabbitMQ"));
            builder.Services.AddRabbitMQClientServiceSingleton(builder.Configuration);
            builder.Services.CustomServices();
            builder.Services.AddRazorPages();
            // Add services to the container.
            builder.Services.AddControllersWithViews();
           
            builder.Services.AddDbContextExtension(builder.Configuration);
            builder.Services.AddIdentityWithPolicy();
            builder.Services.AddCookieConfigure();
            var app = builder.Build();
           

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication(); 
            app.UseAuthorization();
            app.MapHub<MyHub>("/MyHub");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
