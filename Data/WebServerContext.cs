using Microsoft.EntityFrameworkCore;

namespace WebServer.Data
{
    public class WebServerContext : DbContext
    {
        public WebServerContext (DbContextOptions<WebServerContext> options)
            : base(options)
        {
        }
        public DbSet<WebServer.Models.Comment>? Comment { get; set; }
        public DbSet<WebServer.Models.User>? User { get; set; }
        public DbSet<WebServer.Models.Contact>? Contact { get; set; }
        public DbSet<WebServer.Models.Chat>? Chat { get; set; }
        public DbSet<WebServer.Models.Message>? Message { get; set; }
    }
}
