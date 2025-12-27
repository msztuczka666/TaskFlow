using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Models
{
    public class Pracownik
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Imie { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Nazwisko { get; set; } = string.Empty;

        [StringLength(200)]
        public string? FirmaGlowna { get; set; }

        [StringLength(200)]
        public string? FirmaIdentyfikatorowa { get; set; }

        public DateTime? DataWaznosciSEP { get; set; }

        public bool CzySEPWazny => DataWaznosciSEP.HasValue && DataWaznosciSEP.Value > DateTime.Now;

        public int DniDoWygasniecia => DataWaznosciSEP.HasValue 
            ? (DataWaznosciSEP.Value - DateTime.Now).Days 
            : -1;

        // Navigation properties
        public ICollection<Nieobecnosc> Nieobecnosci { get; set; } = new List<Nieobecnosc>();
    }
}
