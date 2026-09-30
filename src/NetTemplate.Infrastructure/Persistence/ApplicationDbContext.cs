using Microsoft.EntityFrameworkCore;
using NetTemplate.Core.Entities;

namespace NetTemplate.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Data> Datas { get; set; }
    }
}
