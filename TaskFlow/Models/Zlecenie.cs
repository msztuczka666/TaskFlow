using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Models
{
    public class Zlecenie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nr zlecenia jest wymagany")]
        [StringLength(30, ErrorMessage = "Nr zlecenia nie może przekraczać 30 znaków")]
        [Display(Name = "Nr zlecenia")]
        public string NrZlecenia { get; set; } = string.Empty;

        [Required(ErrorMessage = "Opis zlecenia jest wymagany")]
        [StringLength(110, ErrorMessage = "Opis zlecenia nie może przekraczać 110 znaków")]
        [Display(Name = "Opis zlecenia")]
        public string Opis { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data rozpoczęcia jest wymagana")]
        [Display(Name = "Data rozpoczęcia")]
        [DataType(DataType.Date)]
        public DateTime DataRozpoczecia { get; set; }

        [Required(ErrorMessage = "Data zakończenia jest wymagana")]
        [Display(Name = "Data zakończenia")]
        [DataType(DataType.Date)]
        public DateTime DataZakonczenia { get; set; }

        [Display(Name = "Ilość dni")]
        public int LiczbaDniRoboczych { get; set; }

        [Required(ErrorMessage = "Status jest wymagany")]
        [StringLength(20)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Aktywne";

        [StringLength(110, ErrorMessage = "Informacje nie mogą przekraczać 110 znaków")]
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
