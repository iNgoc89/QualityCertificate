using HoangThach.AccountShared.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace QualityCertificate.Datas.Entity
{
    public class WSMDbContext : DbContext
    {
        public WSMDbContext(DbContextOptions<WSMDbContext> options)
        : base(options)
        {
        }

        public DbSet<BulkCementCQ> BulkCementCQs { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("KCSs");
        }
    }
}
