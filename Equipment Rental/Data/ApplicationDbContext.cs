using Equipment_Rental.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Equipment_Rental.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<EquipmentItem> EquipmentItems { get; set; }
        public DbSet<RentalRequest> RentalRequests { get; set; }
        public DbSet<RentalRequestItem> RentalRequestItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RentalRequestItem>()
                .HasKey(x => new { x.RentalRequestId, x.EquipmentItemId });
        }
    }
}
