using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;

namespace TaskFlow.Controllers
{
    [Authorize(Roles = "Admin,Kierownik")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Monthly Absence Report
        public async Task<IActionResult> MonthlyAbsences(int? year, int? month)
        {
            year ??= DateTime.Now.Year;
            month ??= DateTime.Now.Month;

            var startDate = new DateTime(year.Value, month.Value, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var absences = await _context.Nieobecnosci
                .Include(n => n.Pracownik)
                .Where(n => n.DataOd <= endDate && n.DataDo >= startDate)
                .OrderBy(n => n.Pracownik.Nazwisko)
                .ThenBy(n => n.DataOd)
                .ToListAsync();

            ViewBag.Year = year.Value;
            ViewBag.Month = month.Value;
            ViewBag.MonthName = new DateTime(year.Value, month.Value, 1).ToString("MMMM yyyy");

            return View(absences);
        }

        // SEP Expiry Warnings
        public async Task<IActionResult> SEPExpiryWarnings(int days = 30)
        {
            var expiryDate = DateTime.Now.AddDays(days);
            var warnings = await _context.Pracownicy
                .Where(p => p.DataWaznosciSEP.HasValue && 
                           p.DataWaznosciSEP.Value <= expiryDate && 
                           p.DataWaznosciSEP.Value >= DateTime.Now)
                .OrderBy(p => p.DataWaznosciSEP)
                .ToListAsync();

            ViewBag.Days = days;
            return View(warnings);
        }

        // Absence Summary by Type
        public async Task<IActionResult> AbsenceSummaryByType(int? year)
        {
            year ??= DateTime.Now.Year;
            var startDate = new DateTime(year.Value, 1, 1);
            var endDate = new DateTime(year.Value, 12, 31);

            var absences = await _context.Nieobecnosci
                .Include(n => n.Pracownik)
                .Where(n => n.DataOd <= endDate && n.DataDo >= startDate)
                .ToListAsync();

            var summary = absences
                .GroupBy(n => n.TypNieobecnosci)
                .Select(g => new
                {
                    Type = g.Key,
                    Count = g.Count(),
                    TotalDays = g.Sum(n => n.LiczbaDni)
                })
                .OrderByDescending(s => s.TotalDays)
                .ToList();

            ViewBag.Year = year.Value;
            ViewBag.Summary = summary;
            return View();
        }
    }
}
