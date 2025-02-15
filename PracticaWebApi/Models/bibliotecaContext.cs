using Microsoft.EntityFrameworkCore;

namespace PracticaWebApi.Models
{
    public class bibliotecaContext : DbContext
    {
        public bibliotecaContext(DbContextOptions<bibliotecaContext> options) : base(options)
        {

        }
        public DbSet<autor> autor { get; set; }
        public DbSet<libro> libro { get; set; }
    }
}
