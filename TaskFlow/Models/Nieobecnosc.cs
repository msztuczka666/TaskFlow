using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Models
{
    public class Nieobecnosc
    {
        public int Id { get; set; }

        [Required]
        public int PracownikId { get; set; }

        [Required]
        public DateTime DataOd { get; set; }

        [Required]
        public DateTime DataDo { get; set; }

        [Required]
        [StringLength(100)]
        public string TypNieobecnosci { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Uwagi { get; set; }

        public int LiczbaDni { get; set; }

        // Navigation properties
        public Pracownik Pracownik { get; set; } = null!;
    }

    public static class TypyNieobecnosci
    {
        public const string Urlop = "Urlop";
        public const string Choroba = "Choroba";
        public const string Szkolenie = "Szkolenie";
        public const string OdborKrwi = "Odbiór krwi";
        public const string OkresZasilek = "Okres zasiłek";
        public const string DzialalnoscGospodarcza = "Działalność gospodarcza";

        public static readonly Dictionary<string, int> DomyslneLimitDni = new()
        {
            { OdborKrwi, 2 }
        };

        public static List<string> Wszystkie => new()
        {
            Urlop,
            Choroba,
            Szkolenie,
            OdborKrwi,
            OkresZasilek,
            DzialalnoscGospodarcza
        };
    }
}
