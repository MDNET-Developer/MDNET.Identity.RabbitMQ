using MDNET.Identity.RabbitMQ.Web.Enums;
using MDNET.Identity.RabbitMQ.Web.Hubs;
using MDNET.Identity.RabbitMQ.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MDNET.Identity.RabbitMQ.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<MyHub> _hubContext;

        public FilesController(AppDbContext context, IHubContext<MyHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        [HttpPost]
        [Route("upload-file")]
        public async Task<IActionResult> UploadFile(IFormFile file, string? userId, Guid? fileId)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest();
            }
            else
            {
                var userFile = await _context.UserFiles.FirstOrDefaultAsync(x => x.Id == fileId && x.CreatedUserId == userId);

                var fileFullName = userFile.FileName + Path.GetExtension(file.FileName);

                var pathStream = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserFiles", fileFullName);
                using FileStream stream = new(pathStream, FileMode.Create);
                await file.CopyToAsync(stream);

                userFile.CreatedTime = DateTime.Now;
                userFile.FilePath= "~/UserFiles/" + fileFullName;
                userFile.FileStatus = (int)FileStatus.Created;
                userFile.FileExtension = Path.GetExtension(fileFullName);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.User(userFile.CreatedUserId).SendAsync("ComplatedFile");
                return Ok();
            }
        }
    }
}
