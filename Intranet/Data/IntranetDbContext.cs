using Intranet.Entities;
using Intranet.Security;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Data
{
    public class IntranetDbContext: IdentityDbContext<AppUser>
    {
        public IntranetDbContext(DbContextOptions<IntranetDbContext> options): base(options)
        {}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
