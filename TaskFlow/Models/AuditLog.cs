using System;
using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string UserId { get; set; }

        [Required]
        [MaxLength(256)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Action { get; set; } // Create, Update, Delete

        [Required]
        [MaxLength(100)]
        public string EntityType { get; set; } // Pracownik, Zlecenie, Nieobecnosc, User, etc.

        public int? EntityId { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        public string OldValues { get; set; } // JSON

        public string NewValues { get; set; } // JSON

        [MaxLength(45)]
        public string IpAddress { get; set; }

        [MaxLength(500)]
        public string UserAgent { get; set; } // Browser information

        [MaxLength(1000)]
        public string Description { get; set; }
    }
}
