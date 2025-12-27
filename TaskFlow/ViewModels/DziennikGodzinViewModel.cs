using TaskFlow.Models;

namespace TaskFlow.ViewModels
{
    public class DziennikGodzinViewModel
    {
        public DateTime WybranaData { get; set; } = DateTime.Today;
        public List<PracownikDzienViewModel> Pracownicy { get; set; } = new();
        public PodsumowanieDniaViewModel Podsumowanie { get; set; } = new();
        public List<Zlecenie> DostepneZlecenia { get; set; } = new();
    }

    public class PracownikDzienViewModel
    {
        public int PracownikId { get; set; }
        public string Imie { get; set; } = string.Empty;
        public string Nazwisko { get; set; } = string.Empty;
        public string Firma { get; set; } = string.Empty;
        public string FirmaId { get; set; } = string.Empty;
        public string Stanowisko { get; set; } = string.Empty;
        public string Inicjaly { get; set; } = string.Empty;
        public List<SegmentPracyViewModel> Segmenty { get; set; } = new();
        public decimal SumaGodzin { get; set; }
        public decimal SumaNadgodzin { get; set; }
    }

    public class SegmentPracyViewModel
    {
        public int Id { get; set; }
        public TimeSpan GodzinaOd { get; set; }
        public TimeSpan GodzinaDo { get; set; }
        public int? ZlecenieId { get; set; }
        public string? OpisPrac { get; set; }
        public decimal LiczbaGodzin { get; set; }
        public bool CzyNadgodziny { get; set; }
    }

    public class PodsumowanieDniaViewModel
    {
        public int LiczbaPracownikow { get; set; }
        public decimal RazemGodzin { get; set; }
        public decimal Nadgodziny { get; set; }
        public decimal DoRozliczenia { get; set; }
    }
}
