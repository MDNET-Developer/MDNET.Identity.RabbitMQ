using MDNET.Identity.RabbitMQ.Web.Enums;
using MDNET.Identity.RabbitMQ.Web.Models;
using MDNET.Identity.RabbitMQ.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MDNET.Identity.RabbitMQ.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly RabbitMQPublisher _rabbitMQPublisher;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ILogger<HomeController> logger,
            AppDbContext appDbContext,
            RabbitMQPublisher rabbitMQPublisher,
            UserManager<AppUser> userManager)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _rabbitMQPublisher = rabbitMQPublisher;
            _userManager = userManager;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("signup")]
        public IActionResult SignUp()
        {
            return View();
        }

        [Authorize]
        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        [HttpGet("files")]
        public IActionResult Files()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var userFiles = _appDbContext.UserFiles
                .Where(f => f.CreatedUserId == user.Id)
                .OrderByDescending(f => f.CreatedTime)
                .ToList();

            return View(userFiles);
        }

        
        [Authorize]
        [HttpPost("file-create")]
        public async Task<IActionResult> CreateFileAsync()
        {
            try
            {
                var userName = User.Identity?.Name;
                if (string.IsNullOrEmpty(userName))
                    return Json(new { success = false, message = "İstifadəçi daxil olmayıb." });

                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                    return Json(new { success = false, message = "İstifadəçi tapılmadı." });

                var fileName = $"{user.UserName}-product-excel-{DateTime.UtcNow:yyyyMMddHHmmss}";
                UserFile userFile = new UserFile()
                {
                    CreatedUserId = user.Id,
                    FileName = fileName,
                    FileStatus = (int)FileStatus.Creating,
                };
                await _appDbContext.UserFiles.AddAsync(userFile);
                await _appDbContext.SaveChangesAsync();

                Shared.CreateFileMessage createFileMessage = new Shared.CreateFileMessage()
                {
                    FileId = userFile.Id,
                    UserId = userFile.CreatedUserId
                };
                _ = Task.Run(() => _rabbitMQPublisher.Publish(createFileMessage));

                return Json(new { success = true, message = "Fayl yaradıldı və proses başladı." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fayl yaradılarkən xəta baş verdi.");
                return Json(new { success = false, message = "Serverdə xəta baş verdi." });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
