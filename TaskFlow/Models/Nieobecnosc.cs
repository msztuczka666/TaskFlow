using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Models
{
    public class Nieobecnosc
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pracownik jest wymagany")]
        public int PracownikId { get; set; }

        [Required(ErrorMessage = "Data od jest wymagana")]
        [Display(Name = "Data od")]
        [DataType(DataType.Date)]
        public DateTime DataOd { get; set; }

        [Required(ErrorMessage = "Data do jest wymagana")]
        [Display(Name = "Data do")]
        [DataType(DataType.Date)]
        public DateTime DataDo { get; set; }

        [Required(ErrorMessage = "Typ nieobecności jest wymagany")]
        [StringLength(100)]
        [Display(Name = "Typ nieobecności")]
        public string TypNieobecnosci { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Uwagi")]
        public string? Uwagi { get; set; }

        [Display(Name = "Liczba dni")]
        public int LiczbaDni { get; set; }

        [Display(Name = "Liczba godzin")]
        public decimal? LiczbaGodzin { get; set; }

        // Navigation properties
        public Pracownik Pracownik { get; set; } = null!;
    }

    public static class TypyNieobecnosci
    {
        // Typ nieobecności constants
        public const string UrlopWypoczynkowy = "urlop wypoczynkowy";
        public const string ZwolnienieLekarskie = "zwolnienie lekarskie";
        public const string UrlopNaZadanie = "urlop na żądanie";
        public const string UrlopZeszyt = "urlop zeszyt";
        public const string SzkolenieBHP = "szkolenie bhp";
        public const string BadaniaOkresowe = "badania okresowe";
        public const string UrlopOkolicznosciowy = "urlop okolicznościowy";
        public const string Opiekunczy = "opiekuńcze";
        public const string Szkolenie = "szkolenie";
        public const string OddanieKrwi = "oddanie krwi";
        public const string NieobecnoscNieusprawiedliwiona = "nieobecność nieusprawiedliwiona";

        // Typy które liczą 8 godzin (traktowane jako zlecenie)
        public static readonly HashSet<string> TypyZ8Godzinami = new()
        {
            SzkolenieBHP,
            BadaniaOkresowe,
            Szkolenie,
            OddanieKrwi  // Oddanie krwi też liczy 8 godzin
        };

        // Typy które nie liczą godzin na zleceniach
        public static readonly HashSet<string> TypyBezGodzin = new()
        {
            UrlopWypoczynkowy,
            ZwolnienieLekarskie,
            UrlopNaZadanie,
            UrlopZeszyt,
            UrlopOkolicznosciowy,
            Opiekunczy,
            NieobecnoscNieusprawiedliwiona
        };

        // Oddanie krwi daje dodatkowy wolny dzień (w sumie 2 dni: dzień oddania + kolejny dzień roboczy)
        // Zasada: Dni wolne muszą następować bezpośrednio po sobie
        public static readonly Dictionary<string, int> DomyslneLimitDni = new()
        {
            { OddanieKrwi, 2 }
        };

        // Urlop na żądanie - max 4 dni w roku
        public const int MaxUrlopNaZadanie = 4;

        // Lista wszystkich typów w kolejności zgodnej z wymaganiami użytkownika
        public static List<string> Wszystkie => new()
        {
            UrlopWypoczynkowy,
            ZwolnienieLekarskie,
            UrlopNaZadanie,
            UrlopZeszyt,
            SzkolenieBHP,
            BadaniaOkresowe,
            UrlopOkolicznosciowy,
            Opiekunczy,
            Szkolenie,
            OddanieKrwi,
            NieobecnoscNieusprawiedliwiona
        };

        public static decimal? ObliczGodziny(string typNieobecnosci, int liczbaDni)
        {
            if (TypyZ8Godzinami.Contains(typNieobecnosci))
            {
                // Typy traktowane jako zlecenie - liczą 8 godzin dziennie
                return liczbaDni * 8;
            }
            else if (TypyBezGodzin.Contains(typNieobecnosci))
            {
                // Typy które nie liczą godzin
                return null;
            }
            else
            {
                // Domyślnie nie liczone
                return null;
            }
        }
    }
}
