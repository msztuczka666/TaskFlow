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

        // Pobierz wszystkich pracowników ZATRUDNIONYCH (sortowanie A-Z po imieniu i nazwisku)
        var wszyscyPracownicy = await _context.Pracownicy
            .Where(p => p.StanZatrudnienia == "Zatrudniony")
            .OrderBy(p => p.Imie)
            .ThenBy(p => p.Nazwisko)
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

    [HttpPost]
    public async Task<IActionResult> SaveDay([FromBody] SaveDayRequest request)
    {
        try
        {
            var dataObj = DateTime.Parse(request.Data);
            
            // Remove existing entries for this day
            var existingEntries = await _context.EwidencjaCzasu
                .Where(e => e.Data.Date == dataObj.Date)
                .ToListAsync();
            
            _context.EwidencjaCzasu.RemoveRange(existingEntries);

            // Check if this is Sunday or holiday - for vacation day addition
            bool isSundayOrHoliday = dataObj.DayOfWeek == DayOfWeek.Sunday || 
                                      TaskFlow.Helpers.PolishHolidays.IsHoliday(dataObj);

            // Track which employees worked on Sunday/holiday for vacation day addition
            var employeesWorkedSundayHoliday = new HashSet<int>();

            // Add new entries
            foreach (var entry in request.Entries)
            {
                var godzinaOd = TimeSpan.Parse(entry.GodzinaOd);
                var godzinaDo = TimeSpan.Parse(entry.GodzinaDo);
                var totalHours = (godzinaDo - godzinaOd).TotalHours;
                
                // Calculate overtime (after 14:30)
                var regularEndTime = new TimeSpan(14, 30, 0);
                decimal nadgodziny = 0;
                decimal godzinyPodstawowe = 0;

                if (godzinaDo <= regularEndTime)
                {
                    godzinyPodstawowe = (decimal)totalHours;
                }
                else if (godzinaOd >= regularEndTime)
                {
                    nadgodziny = (decimal)totalHours;
                }
                else
                {
                    godzinyPodstawowe = (decimal)(regularEndTime - godzinaOd).TotalHours;
                    nadgodziny = (decimal)(godzinaDo - regularEndTime).TotalHours;
                }

                var ewidencja = new EwidencjaCzasu
                {
                    Data = dataObj,
                    PracownikId = entry.PracownikId,
                    ZlecenieId = entry.ZlecenieId,
                    GodzinaOd = godzinaOd,
                    GodzinaDo = godzinaDo,
                    OpisPrac = entry.OpisPrac,
                    LiczbaGodzin = (decimal)totalHours,
                    Nadgodziny = nadgodziny,
                    DataUtworzenia = DateTime.Now,
                    UtworzonyPrzez = User.Identity?.Name ?? "System"
                };

                _context.EwidencjaCzasu.Add(ewidencja);

                // Track employee for vacation day if worked on Sunday/holiday
                if (isSundayOrHoliday && totalHours > 0)
                {
                    employeesWorkedSundayHoliday.Add(entry.PracownikId);
                }
            }

            // Add vacation days for employees who worked on Sunday/holiday
            if (employeesWorkedSundayHoliday.Any())
            {
                var employees = await _context.Pracownicy
                    .Where(p => employeesWorkedSundayHoliday.Contains(p.Id))
                    .ToListAsync();

                foreach (var employee in employees)
                {
                    employee.LiczbaDniWolnych += 1; // Add 1 vacation day
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving day");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetDayEntries(string data)
    {
        try
        {
            var dataObj = DateTime.Parse(data);
            
            var entries = await _context.EwidencjaCzasu
                .Where(e => e.Data.Date == dataObj.Date)
                .Select(e => new
                {
                    pracownikId = e.PracownikId,
                    godzinaOd = e.GodzinaOd.ToString(@"hh\:mm"),
                    godzinaDo = e.GodzinaDo.ToString(@"hh\:mm"),
                    zlecenieId = e.ZlecenieId,
                    opisPrac = e.OpisPrac
                })
                .ToListAsync();

            return Json(entries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting day entries");
            return Json(new List<object>());
        }
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

public class SaveDayRequest
{
    public string Data { get; set; } = string.Empty;
    public List<TimeEntryDto> Entries { get; set; } = new();
}

public class TimeEntryDto
{
    public int PracownikId { get; set; }
    public string GodzinaOd { get; set; } = string.Empty;
    public string GodzinaDo { get; set; } = string.Empty;
    public int ZlecenieId { get; set; }
    public string? OpisPrac { get; set; }
}
