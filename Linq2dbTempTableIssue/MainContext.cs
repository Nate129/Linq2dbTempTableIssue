using Microsoft.EntityFrameworkCore;

namespace Linq2dbTempTableIssue
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions<DbContext> options) : base(options)
        {
        }

        public MainContext()
        {
        }

        public DbSet<Person> People { get; set; }
    }
}
