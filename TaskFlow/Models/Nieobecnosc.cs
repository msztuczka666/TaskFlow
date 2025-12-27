using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Models
{
    public class Nieobecnosc
    {
        public int Id { get; set; }

        [Required]
        public int PracownikId { get; set; }

        [Required]
        [Display(Name = "Data od")]
        public DateTime DataOd { get; set; }

        [Required]
        [Display(Name = "Data do")]
        public DateTime DataDo { get; set; }

        [Required]
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
        public const string SzkolenieBHP = "Szkolenie BHP";
        public const string BadaniaOkresowe = "Badania okresowe";
        public const string UrlopWypoczynkowy = "Urlop wypoczynkowy";
        public const string UrlopNaZadanie = "Urlop na żądanie";
        public const string UrlopOkolicznosciowy = "Urlop okolicznościowy";
        public const string UrlopZeszyt = "Urlop zeszyt";
        public const string ZwolnienieLekarskie = "Zwolnienie lekarskie";
        public const string Opiekunczy = "Opiekuńcze";
        public const string Szkolenie = "Szkolenie";
        public const string OddanieKrwi = "Oddanie krwi";
        public const string NieobecnoscNieusprawiedliwiona = "Nieobecność nieusprawiedliwiona";

        // Typy które liczą 8 godzin
        public static readonly HashSet<string> TypyZ8Godzinami = new()
        {
            SzkolenieBHP,
            BadaniaOkresowe,
            Szkolenie
        };

        // Typy które nie liczą godzin
        public static readonly HashSet<string> TypyBezGodzin = new()
        {
            UrlopWypoczynkowy,
            UrlopNaZadanie,
            UrlopOkolicznosciowy,
            UrlopZeszyt,
            Opiekunczy,
            OddanieKrwi,
            NieobecnoscNieusprawiedliwiona
        };

        // Oddanie krwi daje dodatkowy wolny dzień
        public static readonly Dictionary<string, int> DomyslneLimitDni = new()
        {
            { OddanieKrwi, 2 }
        };

        public static List<string> Wszystkie => new()
        {
            SzkolenieBHP,
            BadaniaOkresowe,
            UrlopWypoczynkowy,
            UrlopNaZadanie,
            UrlopOkolicznosciowy,
            UrlopZeszyt,
            ZwolnienieLekarskie,
            Opiekunczy,
            Szkolenie,
            OddanieKrwi,
            NieobecnoscNieusprawiedliwiona
        };

        public static decimal? ObliczGodziny(string typNieobecnosci, int liczbaDni)
        {
            if (TypyZ8Godzinami.Contains(typNieobecnosci))
            {
                return liczbaDni * 8;
            }
            else if (TypyBezGodzin.Contains(typNieobecnosci))
            {
                return null; // Nie liczone
            }
            else
            {
                // Domyślnie dla innych typów (np. zwolnienie lekarskie)
                return liczbaDni * 8;
            }
        }
    }
}
