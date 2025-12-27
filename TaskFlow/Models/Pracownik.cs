using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Models
{
    public class Pracownik
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Imię")]
        public string Imie { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Nazwisko")]
        public string Nazwisko { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Firma")]
        public string? Firma { get; set; }

        [StringLength(100)]
        [Display(Name = "Nr przepustki")]
        public string? NrPrzepustki { get; set; }

        [StringLength(100)]
        [Display(Name = "MPK")]
        public string? MPK { get; set; }

        [StringLength(200)]
        [Display(Name = "Firma Id")]
        public string? FirmaId { get; set; }

        [StringLength(100)]
        [Display(Name = "Stanowisko")]
        public string? Stanowisko { get; set; }

        [StringLength(100)]
        [Display(Name = "SEP nr")]
        public string? SEPNr { get; set; }

        [Display(Name = "SEP ważne do")]
        public DateTime? DataWaznosciSEP { get; set; }

        [StringLength(100)]
        [Display(Name = "SEP Napięcie")]
        public string? SEPNapiecie { get; set; }

        [Display(Name = "Liczba dni wolnych")]
        public int LiczbaDniWolnych { get; set; }

        [Display(Name = "Liczba dni na zeszycie")]
        public int LiczbaDniNaZeszycie { get; set; }

        [Display(Name = "Liczba dni wykorzystanych")]
        public int LiczbaDniWykorzystanych { get; set; }

        [StringLength(50)]
        [Display(Name = "Typ pracownika")]
        public string? TypPracownika { get; set; } // "Nadzór" lub "Pracownik"

        [StringLength(1000)]
        [Display(Name = "Informacje")]
        public string? Informacje { get; set; }

        public bool CzySEPWazny => DataWaznosciSEP.HasValue && DataWaznosciSEP.Value > DateTime.Now;

        public int DniDoWygasniecia => DataWaznosciSEP.HasValue 
            ? (DataWaznosciSEP.Value - DateTime.Now).Days 
            : -1;

        // Navigation properties
        public ICollection<Nieobecnosc> Nieobecnosci { get; set; } = new List<Nieobecnosc>();
    }

    public static class TypyPracownika
    {
        public const string Nadzor = "Nadzór";
        public const string Pracownik = "Pracownik";

        public static List<string> Wszystkie => new()
        {
            Nadzor,
            Pracownik
        };
    }
}
