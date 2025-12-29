using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TaskFlow.Models;
using System.Text.Json;

namespace TaskFlow.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
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

        public override int SaveChanges()
        {
            AddAuditLogs();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddAuditLogs();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void AddAuditLogs()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is not AuditLog && // Don't log audit logs themselves
                           (e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted))
                .ToList();

            if (!entries.Any()) return;

            var userName = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";
            var ipAddress = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = _httpContextAccessor?.HttpContext?.Request?.Headers["User-Agent"].ToString() ?? "Unknown";

            foreach (var entry in entries)
            {
                var auditLog = new AuditLog
                {
                    EntityType = entry.Entity.GetType().Name,
                    Action = entry.State.ToString(),
                    Timestamp = DateTime.UtcNow,
                    UserId = userName,
                    UserName = userName,
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                };

                if (entry.State == EntityState.Added)
                {
                    auditLog.NewValues = SerializeEntity(entry);
                }
                else if (entry.State == EntityState.Modified)
                {
                    auditLog.OldValues = SerializeOriginalValues(entry);
                    auditLog.NewValues = SerializeCurrentValues(entry);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    auditLog.OldValues = SerializeEntity(entry);
                }

                AuditLogs.Add(auditLog);
            }
        }

        private string SerializeEntity(EntityEntry entry)
        {
            var values = new Dictionary<string, object?>();
            foreach (var property in entry.Properties)
            {
                values[property.Metadata.Name] = property.CurrentValue;
            }
            return JsonSerializer.Serialize(values);
        }

        private string SerializeOriginalValues(EntityEntry entry)
        {
            var values = new Dictionary<string, object?>();
            foreach (var property in entry.Properties.Where(p => p.IsModified))
            {
                values[property.Metadata.Name] = property.OriginalValue;
            }
            return JsonSerializer.Serialize(values);
        }

        private string SerializeCurrentValues(EntityEntry entry)
        {
            var values = new Dictionary<string, object?>();
            foreach (var property in entry.Properties.Where(p => p.IsModified))
            {
                values[property.Metadata.Name] = property.CurrentValue;
            }
            return JsonSerializer.Serialize(values);
        }
    }
}
