using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;
using TaskFlow.ViewModels;

namespace TaskFlow.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index(DateTime? data)
    {
        var wybranaData = data ?? DateTime.Today;

        var model = new DziennikGodzinViewModel
        {
            WybranaData = wybranaData,
            DostepneZlecenia = await _context.Zlecenia
                .Where(z => z.Status == StatusyZlecenia.Aktywne)
                .OrderBy(z => z.NrZlecenia)
                .ToListAsync()
        };

        // Pobierz wszystkich pracowników
        var wszyscyPracownicy = await _context.Pracownicy
            .OrderBy(p => p.Nazwisko)
            .ThenBy(p => p.Imie)
            .ToListAsync();

        // Pobierz ewidencję czasu dla wybranej daty
        var ewidencja = await _context.EwidencjaCzasu
            .Include(e => e.Pracownik)
            .Include(e => e.Zlecenie)
            .Where(e => e.Data.Date == wybranaData.Date)
            .ToListAsync();

        // Przygotuj dane dla każdego pracownika
        foreach (var pracownik in wszyscyPracownicy)
        {
            var segmentyPracownika = ewidencja
                .Where(e => e.PracownikId == pracownik.Id)
                .Select(e => new SegmentPracyViewModel
                {
                    Id = e.Id,
                    GodzinaOd = e.GodzinaOd,
                    GodzinaDo = e.GodzinaDo,
                    ZlecenieId = e.ZlecenieId,
                    OpisPrac = e.OpisPrac,
                    LiczbaGodzin = e.LiczbaGodzin,
                    CzyNadgodziny = e.Nadgodziny > 0
                })
                .ToList();

            var sumaGodzin = segmentyPracownika.Sum(s => s.LiczbaGodzin);
            var sumaNadgodzin = segmentyPracownika.Where(s => s.CzyNadgodziny).Sum(s => s.LiczbaGodzin);

            model.Pracownicy.Add(new PracownikDzienViewModel
            {
                PracownikId = pracownik.Id,
                Imie = pracownik.Imie,
                Nazwisko = pracownik.Nazwisko,
                Firma = pracownik.Firma ?? "",
                FirmaId = pracownik.FirmaId ?? "",
                Stanowisko = pracownik.Stanowisko ?? "",
                Inicjaly = GetInitials(pracownik.Imie, pracownik.Nazwisko),
                Segmenty = segmentyPracownika,
                SumaGodzin = sumaGodzin,
                SumaNadgodzin = sumaNadgodzin
            });
        }

        // Oblicz podsumowanie
        model.Podsumowanie = new PodsumowanieDniaViewModel
        {
            LiczbaPracownikow = model.Pracownicy.Count(p => p.Segmenty.Any()),
            RazemGodzin = model.Pracownicy.Sum(p => p.SumaGodzin - p.SumaNadgodzin),
            Nadgodziny = model.Pracownicy.Sum(p => p.SumaNadgodzin),
            DoRozliczenia = model.Pracownicy.Sum(p => p.SumaGodzin)
        };

        return View(model);
    }

    private string GetInitials(string imie, string nazwisko)
    {
        var i = !string.IsNullOrEmpty(imie) ? imie[0].ToString().ToUpper() : "";
        var n = !string.IsNullOrEmpty(nazwisko) ? nazwisko[0].ToString().ToUpper() : "";
        return i + n;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
