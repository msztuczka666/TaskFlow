using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Models
{
    public class Zlecenie
    {
        public int Id { get; set; }

        [StringLength(200)]
        [Display(Name = "Nr zlecenia")]
        public string? NrZlecenia { get; set; }

        [Required]
        [StringLength(1000)]
        [Display(Name = "Opis zlecenia")]
        public string Opis { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Data rozpoczęcia")]
        public DateTime DataRozpoczecia { get; set; }

        [Required]
        [Display(Name = "Data zakończenia")]
        public DateTime DataZakonczenia { get; set; }

        [Display(Name = "Ilość dni roboczych")]
        public int LiczbaDniRoboczych { get; set; }

        [StringLength(100)]
        [Display(Name = "Status")]
        public string? Status { get; set; }

        [StringLength(1000)]
        [Display(Name = "Informacje")]
        public string? Informacje { get; set; }

        public DateTime DataUtworzenia { get; set; } = DateTime.Now;
    }

    public static class StatusyZlecenia
    {
        public const string Aktywne = "Aktywne";
        public const string Zakonczone = "Zakończone";

        public static List<string> Wszystkie => new()
        {
            Aktywne,
            Zakonczone
        };
    }
}
