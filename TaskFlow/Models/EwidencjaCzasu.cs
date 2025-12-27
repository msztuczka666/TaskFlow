using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Models
{
    public class EwidencjaCzasu
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Data")]
        public DateTime Data { get; set; }

        [Required]
        [Display(Name = "Pracownik")]
        public int PracownikId { get; set; }

        [Required]
        [Display(Name = "Zlecenie")]
        public int ZlecenieId { get; set; }

        [Required]
        [Display(Name = "Godzina rozpoczęcia")]
        public TimeSpan GodzinaOd { get; set; }

        [Required]
        [Display(Name = "Godzina zakończenia")]
        public TimeSpan GodzinaDo { get; set; }

        [StringLength(1000)]
        [Display(Name = "Opis prac")]
        public string? OpisPrac { get; set; }

        [Display(Name = "Liczba godzin")]
        public decimal LiczbaGodzin { get; set; }

        [Display(Name = "Nadgodziny")]
        public decimal Nadgodziny { get; set; }

        [Display(Name = "Data utworzenia")]
        public DateTime DataUtworzenia { get; set; } = DateTime.Now;

        [Display(Name = "Data modyfikacji")]
        public DateTime? DataModyfikacji { get; set; }

        [StringLength(200)]
        [Display(Name = "Utworzony przez")]
        public string? UtworzonyPrzez { get; set; }

        [StringLength(200)]
        [Display(Name = "Zmodyfikowany przez")]
        public string? ZmodyfikowanyPrzez { get; set; }

        // Navigation properties
        public Pracownik Pracownik { get; set; } = null!;
        public Zlecenie Zlecenie { get; set; } = null!;
    }
}
