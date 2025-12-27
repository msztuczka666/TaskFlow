using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Models
{
    public class Zlecenie
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nazwa { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Opis { get; set; }

        [Required]
        public DateTime DataRozpoczecia { get; set; }

        [Required]
        public DateTime DataZakonczenia { get; set; }

        public int LiczbaDniRoboczych { get; set; }

        [StringLength(100)]
        public string? Status { get; set; }

        public DateTime DataUtworzenia { get; set; } = DateTime.Now;
    }

    public static class StatusyZlecenia
    {
        public const string Nowe = "Nowe";
        public const string WTrakcie = "W trakcie";
        public const string Zakonczone = "Zakończone";
        public const string Wstrzymane = "Wstrzymane";

        public static List<string> Wszystkie => new()
        {
            Nowe,
            WTrakcie,
            Zakonczone,
            Wstrzymane
        };
    }
}
