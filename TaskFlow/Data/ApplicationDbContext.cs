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
        public DbSet<EwidencjaCzasu> EwidencjaCzasu { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

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
                entity.Property(e => e.Opis).IsRequired().HasMaxLength(1000);
            });

            // Configure EwidencjaCzasu
            modelBuilder.Entity<EwidencjaCzasu>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Pracownik)
                      .WithMany()
                      .HasForeignKey(e => e.PracownikId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Zlecenie)
                      .WithMany()
                      .HasForeignKey(e => e.ZlecenieId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => new { e.Data, e.PracownikId });
            });
        }
    }
}
