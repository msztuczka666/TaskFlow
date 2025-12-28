using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Models
{
    public class Pracownik
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane")]
        [StringLength(30, ErrorMessage = "Imię nie może przekraczać 30 znaków")]
        [Display(Name = "Imię")]
        public string Imie { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [StringLength(50, ErrorMessage = "Nazwisko nie może przekraczać 50 znaków")]
        [Display(Name = "Nazwisko")]
        public string Nazwisko { get; set; } = string.Empty;

        [StringLength(30, ErrorMessage = "Nazwa firmy nie może przekraczać 30 znaków")]
        [Display(Name = "Firma")]
        public string? Firma { get; set; }

        [StringLength(10, ErrorMessage = "Nr przepustki nie może przekraczać 10 cyfr")]
        [RegularExpression(@"^\d{1,10}$", ErrorMessage = "Nr przepustki może zawierać tylko cyfry (max 10)")]
        [Display(Name = "Nr przepustki")]
        public string? NrPrzepustki { get; set; }

        [StringLength(3, MinimumLength = 3, ErrorMessage = "MPK musi zawierać dokładnie 3 cyfry")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "MPK może zawierać tylko 3 cyfry")]
        [Display(Name = "MPK")]
        public string? MPK { get; set; }

        [StringLength(30, ErrorMessage = "Firma ID nie może przekraczać 30 znaków")]
        [Display(Name = "Firma Id")]
        public string? FirmaId { get; set; }

        [StringLength(30, ErrorMessage = "Stanowisko nie może przekraczać 30 znaków")]
        [Display(Name = "Stanowisko")]
        public string? Stanowisko { get; set; }

        [StringLength(20, ErrorMessage = "SEP nr nie może przekraczać 20 znaków")]
        [Display(Name = "SEP nr")]
        public string? SEPNr { get; set; }

        [Display(Name = "SEP ważne do")]
        public DateTime? DataWaznosciSEP { get; set; }

        [StringLength(20, ErrorMessage = "SEP Napięcie nie może przekraczać 20 znaków")]
        [Display(Name = "SEP Napięcie")]
        public string? SEPNapiecie { get; set; }

        [Range(0, 999, ErrorMessage = "Liczba dni wolnych musi być między 0 a 999")]
        [Display(Name = "Liczba dni wolnych")]
        public int LiczbaDniWolnych { get; set; }

        [Range(0, 999, ErrorMessage = "Liczba dni na zeszycie musi być między 0 a 999")]
        [Display(Name = "Liczba dni na zeszycie")]
        public int LiczbaDniNaZeszycie { get; set; }

        [Range(0, 999, ErrorMessage = "Liczba dni wykorzystanych musi być między 0 a 999")]
        [Display(Name = "Liczba dni wykorzystanych")]
        public int LiczbaDniWykorzystanych { get; set; }

        [Range(0, 999, ErrorMessage = "Liczba dni na żądanie musi być między 0 a 4")]
        [Display(Name = "Liczba dni na żądanie")]
        public int LiczbaDniNaZadanie { get; set; }

        [Required(ErrorMessage = "Typ pracownika jest wymagany")]
        [StringLength(20)]
        [Display(Name = "Typ pracownika")]
        public string TypPracownika { get; set; } = "Pracownik"; // "Nadzór" lub "Pracownik"

        [Display(Name = "Stawka zł/h")]
        [Range(0, 999999.99, ErrorMessage = "Stawka musi być między 0 a 999999,99")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? StawkaZlH { get; set; }

        [StringLength(50, ErrorMessage = "Informacje nie mogą przekraczać 50 znaków")]
        [Display(Name = "Informacje")]
        public string? Informacje { get; set; }

        [Required(ErrorMessage = "Stan zatrudnienia jest wymagany")]
        [StringLength(20)]
        [Display(Name = "Stan zatrudnienia")]
        public string StanZatrudnienia { get; set; } = "Zatrudniony"; // "Zatrudniony" lub "Zwolniony"

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

    public static class StanyZatrudnienia
    {
        public const string Zatrudniony = "Zatrudniony";
        public const string Zwolniony = "Zwolniony";

        public static List<string> Wszystkie => new()
        {
            Zatrudniony,
            Zwolniony
        };
    }
}
