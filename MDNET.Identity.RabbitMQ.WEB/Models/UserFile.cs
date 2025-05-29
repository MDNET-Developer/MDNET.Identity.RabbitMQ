namespace MDNET.Identity.RabbitMQ.Web.Models
{
  
    public class UserFile
    {
        public Guid? Id { get; set; } 
        public string? FileName { get; set; }   
        public string? FilePath { get; set; } 
        public string? FileExtension { get; set; }
        public int FileStatus { get; set; } 
        public DateTime? CreatedTime { get; set; } = DateTime.UtcNow;
        public string? CreatedUserId { get; set; } 
    }
}
