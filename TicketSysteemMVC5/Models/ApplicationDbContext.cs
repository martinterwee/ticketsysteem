using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace TicketSysteemMVC5.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Applicatie> Applicaties { get; set; }

        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>()
                .HasRequired(t => t.Klant)
                .WithMany(u => u.Tickets)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }

        public System.Data.Entity.DbSet<TicketSysteemMVC5.Models.Categorie> Categories { get; set; }

        public System.Data.Entity.DbSet<TicketSysteemMVC5.Models.Status> Status { get; set; }
    }
}