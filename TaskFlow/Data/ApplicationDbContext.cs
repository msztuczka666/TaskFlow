using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Models;

namespace TaskFlow.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pracownik> Pracownicy { get; set; }
        public DbSet<Nieobecnosc> Nieobecnosci { get; set; }
        public DbSet<Zlecenie> Zlecenia { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Pracownik
            modelBuilder.Entity<Pracownik>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Imie).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Nazwisko).IsRequired().HasMaxLength(100);
                entity.HasMany(e => e.Nieobecnosci)
                      .WithOne(e => e.Pracownik)
                      .HasForeignKey(e => e.PracownikId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Nieobecnosc
            modelBuilder.Entity<Nieobecnosc>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TypNieobecnosci).IsRequired().HasMaxLength(100);
            });

            // Configure Zlecenie
            modelBuilder.Entity<Zlecenie>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nazwa).IsRequired().HasMaxLength(200);
            });
        }
    }
}
