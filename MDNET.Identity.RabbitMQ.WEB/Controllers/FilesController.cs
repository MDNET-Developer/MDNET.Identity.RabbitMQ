using MDNET.Identity.RabbitMQ.Web.Enums;
using MDNET.Identity.RabbitMQ.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MDNET.Identity.RabbitMQ.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FilesController(AppDbContext context)
        {
            _context = context;
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
                var filePath = userFile.FileName + Path.GetExtension(file.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserFiles", filePath);
                using FileStream stream = new(path,FileMode.Create);
                await file.CopyToAsync(stream);
                userFile.CreatedTime = DateTime.Now;
                userFile.FilePath= path;
                userFile.FileStatus = (int)FileStatus.Created;
                await _context.SaveChangesAsync();
                return Ok();
            }
        }
    }
}
