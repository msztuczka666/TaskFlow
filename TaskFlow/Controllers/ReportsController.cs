using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;
using TaskFlow.ViewModels;
using System.Globalization;

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

        // NEW REPORT 1: Employee Hours Report
        [HttpGet]
        public async Task<IActionResult> EmployeeHours()
        {
            var model = new EmployeeHoursReportViewModel();
            
            // Populate available filter options
            model.AvailableYears = await _context.EwidencjaCzasu
                .Select(e => e.Data.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();
                
            model.AvailableMPKs = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.MPK))
                .Select(p => p.MPK!)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
                
            model.AvailableFirmas = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.Firma))
                .Select(p => p.Firma!)
                .Distinct()
                .OrderBy(f => f)
                .ToListAsync();
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EmployeeHours(EmployeeHoursReportViewModel model)
        {
            // Repopulate filter options
            model.AvailableYears = await _context.EwidencjaCzasu
                .Select(e => e.Data.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();
                
            model.AvailableMPKs = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.MPK))
                .Select(p => p.MPK!)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
                
            model.AvailableFirmas = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.Firma))
                .Select(p => p.Firma!)
                .Distinct()
                .OrderBy(f => f)
                .ToListAsync();

            // Apply filters
            var query = _context.EwidencjaCzasu
                .Include(e => e.Pracownik)
                .AsQueryable();

            // Filter by years and months
            if (model.SelectedYears != null && model.SelectedYears.Any())
            {
                if (model.SelectedMonths != null && model.SelectedMonths.Any())
                {
                    query = query.Where(e => model.SelectedYears.Contains(e.Data.Year) && 
                                            model.SelectedMonths.Contains(e.Data.Month));
                }
                else
                {
                    query = query.Where(e => model.SelectedYears.Contains(e.Data.Year));
                }
            }

            // Filter by MPK
            if (model.SelectedMPKs != null && model.SelectedMPKs.Any())
            {
                query = query.Where(e => e.Pracownik.MPK != null && model.SelectedMPKs.Contains(e.Pracownik.MPK));
            }

            // Filter by Firma
            if (model.SelectedFirmas != null && model.SelectedFirmas.Any())
            {
                query = query.Where(e => e.Pracownik.Firma != null && model.SelectedFirmas.Contains(e.Pracownik.Firma));
            }

            var data = await query
                .OrderBy(e => e.Pracownik.Nazwisko)
                .ThenBy(e => e.Pracownik.Imie)
                .ThenBy(e => e.Data)
                .ToListAsync();

            // Group by employee and date
            var grouped = data
                .GroupBy(e => new { e.PracownikId, e.Pracownik.Imie, e.Pracownik.Nazwisko })
                .Select(g => new EmployeeHoursRow
                {
                    PracownikId = g.Key.PracownikId,
                    PracownikImie = g.Key.Imie,
                    PracownikNazwisko = g.Key.Nazwisko,
                    Days = g.GroupBy(e => e.Data)
                        .Select(d => new DayHours
                        {
                            Date = d.Key,
                            DayOfWeek = d.Key.ToString("dddd", new CultureInfo("pl-PL")),
                            Hours = d.Sum(e => e.LiczbaGodzin) // Regular hours only, no overtime multipliers
                        })
                        .OrderBy(d => d.Date)
                        .ToList()
                })
                .ToList();

            model.Data = grouped;
            return View(model);
        }

        // NEW REPORT 2: Order Hours Report
        [HttpGet]
        public async Task<IActionResult> OrderHours()
        {
            var model = new OrderHoursReportViewModel();
            
            // Populate available filter options
            model.AvailableYears = await _context.EwidencjaCzasu
                .Select(e => e.Data.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();
                
            model.AvailableMPKs = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.MPK))
                .Select(p => p.MPK!)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
                
            model.AvailableFirmas = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.Firma))
                .Select(p => p.Firma!)
                .Distinct()
                .OrderBy(f => f)
                .ToListAsync();
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> OrderHours(OrderHoursReportViewModel model)
        {
            // Repopulate filter options
            model.AvailableYears = await _context.EwidencjaCzasu
                .Select(e => e.Data.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();
                
            model.AvailableMPKs = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.MPK))
                .Select(p => p.MPK!)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
                
            model.AvailableFirmas = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.Firma))
                .Select(p => p.Firma!)
                .Distinct()
                .OrderBy(f => f)
                .ToListAsync();

            // Apply filters
            var query = _context.EwidencjaCzasu
                .Include(e => e.Pracownik)
                .Include(e => e.Zlecenie)
                .AsQueryable();

            // Filter by years and months
            if (model.SelectedYears != null && model.SelectedYears.Any())
            {
                if (model.SelectedMonths != null && model.SelectedMonths.Any())
                {
                    query = query.Where(e => model.SelectedYears.Contains(e.Data.Year) && 
                                            model.SelectedMonths.Contains(e.Data.Month));
                }
                else
                {
                    query = query.Where(e => model.SelectedYears.Contains(e.Data.Year));
                }
            }

            // Filter by MPK
            if (model.SelectedMPKs != null && model.SelectedMPKs.Any())
            {
                query = query.Where(e => e.Pracownik.MPK != null && model.SelectedMPKs.Contains(e.Pracownik.MPK));
            }

            // Filter by Firma
            if (model.SelectedFirmas != null && model.SelectedFirmas.Any())
            {
                query = query.Where(e => e.Pracownik.Firma != null && model.SelectedFirmas.Contains(e.Pracownik.Firma));
            }

            var data = await query.ToListAsync();

            // Group by order
            var grouped = data
                .GroupBy(e => new { e.ZlecenieId, e.Zlecenie.NrZlecenia, e.Zlecenie.Opis })
                .Select(g => new OrderHoursRow
                {
                    ZlecenieId = g.Key.ZlecenieId,
                    NrZlecenia = g.Key.NrZlecenia,
                    OpisZlecenia = g.Key.Opis,
                    TotalHours = g.Sum(e => e.LiczbaGodzin) // Regular hours only, no overtime multipliers
                })
                .OrderBy(o => o.NrZlecenia)
                .ToList();

            model.Data = grouped;
            return View(model);
        }

        // NEW REPORT 3: Vacation Balance Report
        [HttpGet]
        public async Task<IActionResult> VacationBalance()
        {
            var model = new VacationBalanceReportViewModel();
            
            // Populate available filter options
            model.AvailableYears = Enumerable.Range(DateTime.Now.Year - 5, 11).OrderByDescending(y => y).ToList();
                
            model.AvailableMPKs = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.MPK))
                .Select(p => p.MPK!)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
                
            model.AvailableFirmas = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.Firma))
                .Select(p => p.Firma!)
                .Distinct()
                .OrderBy(f => f)
                .ToListAsync();
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> VacationBalance(VacationBalanceReportViewModel model)
        {
            // Repopulate filter options
            model.AvailableYears = Enumerable.Range(DateTime.Now.Year - 5, 11).OrderByDescending(y => y).ToList();
                
            model.AvailableMPKs = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.MPK))
                .Select(p => p.MPK!)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
                
            model.AvailableFirmas = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.Firma))
                .Select(p => p.Firma!)
                .Distinct()
                .OrderBy(f => f)
                .ToListAsync();

            // Apply filters to employees
            var query = _context.Pracownicy
                .Where(p => p.StanZatrudnienia == "Zatrudniony")
                .AsQueryable();

            // Filter by MPK
            if (model.SelectedMPKs != null && model.SelectedMPKs.Any())
            {
                query = query.Where(p => p.MPK != null && model.SelectedMPKs.Contains(p.MPK));
            }

            // Filter by Firma
            if (model.SelectedFirmas != null && model.SelectedFirmas.Any())
            {
                query = query.Where(p => p.Firma != null && model.SelectedFirmas.Contains(p.Firma));
            }

            var employees = await query
                .OrderBy(p => p.Nazwisko)
                .ThenBy(p => p.Imie)
                .ToListAsync();

            // Calculate vacation usage for selected years
            DateTime? startDate = null;
            DateTime? endDate = null;
            
            if (model.SelectedYears != null && model.SelectedYears.Any())
            {
                var minYear = model.SelectedYears.Min();
                var maxYear = model.SelectedYears.Max();
                startDate = new DateTime(minYear, 1, 1);
                endDate = new DateTime(maxYear, 12, 31);
            }

            var data = new List<VacationBalanceRow>();

            foreach (var emp in employees)
            {
                // Count vacation days used
                var vacationQuery = _context.Nieobecnosci
                    .Where(n => n.PracownikId == emp.Id && n.TypNieobecnosci == "urlop wypoczynkowy");

                if (startDate.HasValue && endDate.HasValue)
                {
                    vacationQuery = vacationQuery.Where(n => n.DataOd <= endDate && n.DataDo >= startDate);
                }

                var vacationDaysUsed = await vacationQuery.SumAsync(n => (int?)n.LiczbaDni) ?? 0;

                data.Add(new VacationBalanceRow
                {
                    PracownikId = emp.Id,
                    PracownikImie = emp.Imie,
                    PracownikNazwisko = emp.Nazwisko,
                    LiczbaDniWolnych = emp.LiczbaDniWolnych,
                    UrlopWykorzystany = vacationDaysUsed
                });
            }

            model.Data = data;
            return View(model);
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

        // NEW REPORT 4: Order Settlement Report (Rozliczanie zlecenia)
        [HttpGet]
        public async Task<IActionResult> OrderSettlement()
        {
            var model = new OrderSettlementReportViewModel();
            
            // Populate available filter options
            model.AvailableYears = await _context.EwidencjaCzasu
                .Select(e => e.Data.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();
                
            model.AvailableMPKs = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.MPK))
                .Select(p => p.MPK!)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
                
            model.AvailableFirmas = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.Firma))
                .Select(p => p.Firma!)
                .Distinct()
                .OrderBy(f => f)
                .ToListAsync();

            model.AvailableOrderNumbers = await _context.Zlecenia
                .Select(z => z.NrZlecenia)
                .Distinct()
                .OrderBy(n => n)
                .ToListAsync();
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> OrderSettlement(OrderSettlementReportViewModel model)
        {
            // Repopulate filter options
            model.AvailableYears = await _context.EwidencjaCzasu
                .Select(e => e.Data.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();
                
            model.AvailableMPKs = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.MPK))
                .Select(p => p.MPK!)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
                
            model.AvailableFirmas = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.Firma))
                .Select(p => p.Firma!)
                .Distinct()
                .OrderBy(f => f)
                .ToListAsync();

            model.AvailableOrderNumbers = await _context.Zlecenia
                .Select(z => z.NrZlecenia)
                .Distinct()
                .OrderBy(n => n)
                .ToListAsync();

            // Apply filters
            var query = _context.EwidencjaCzasu
                .Include(e => e.Pracownik)
                .Include(e => e.Zlecenie)
                .AsQueryable();

            // Filter by years and months
            if (model.SelectedYears != null && model.SelectedYears.Any())
            {
                if (model.SelectedMonths != null && model.SelectedMonths.Any())
                {
                    query = query.Where(e => model.SelectedYears.Contains(e.Data.Year) && 
                                            model.SelectedMonths.Contains(e.Data.Month));
                }
                else
                {
                    query = query.Where(e => model.SelectedYears.Contains(e.Data.Year));
                }
            }

            // Filter by MPK
            if (model.SelectedMPKs != null && model.SelectedMPKs.Any())
            {
                query = query.Where(e => e.Pracownik.MPK != null && model.SelectedMPKs.Contains(e.Pracownik.MPK));
            }

            // Filter by Firma
            if (model.SelectedFirmas != null && model.SelectedFirmas.Any())
            {
                query = query.Where(e => e.Pracownik.Firma != null && model.SelectedFirmas.Contains(e.Pracownik.Firma));
            }

            // Filter by Order Numbers
            if (model.SelectedOrderNumbers != null && model.SelectedOrderNumbers.Any())
            {
                query = query.Where(e => model.SelectedOrderNumbers.Contains(e.Zlecenie.NrZlecenia));
            }

            var data = await query.ToListAsync();

            // Group by order, then by work description (OpisPrac), then by employee
            var grouped = data
                .GroupBy(e => new { e.ZlecenieId, e.Zlecenie.NrZlecenia, e.Zlecenie.Opis })
                .Select(orderGroup => new OrderSettlementRow
                {
                    ZlecenieId = orderGroup.Key.ZlecenieId,
                    NrZlecenia = orderGroup.Key.NrZlecenia,
                    OpisZlecenia = orderGroup.Key.Opis,
                    WorkGroups = orderGroup
                        .GroupBy(e => e.OpisPrac ?? string.Empty)
                        .Select(workGroup => new WorkDescriptionGroup
                        {
                            OpisPrac = workGroup.Key,
                            EmployeeDetails = workGroup
                                .GroupBy(e => new { e.PracownikId, e.Pracownik.Imie, e.Pracownik.Nazwisko, e.Pracownik.StawkaZlH })
                                .Select(empGroup => new EmployeeWorkDetail
                                {
                                    PracownikId = empGroup.Key.PracownikId,
                                    PracownikImie = empGroup.Key.Imie,
                                    PracownikNazwisko = empGroup.Key.Nazwisko,
                                    Hours = empGroup.Sum(e => e.LiczbaGodzin), // Regular hours only, no overtime multipliers
                                    StawkaZlH = empGroup.Key.StawkaZlH ?? 0m
                                })
                                .OrderBy(emp => emp.PracownikNazwisko)
                                .ThenBy(emp => emp.PracownikImie)
                                .ToList()
                        })
                        .OrderBy(wg => wg.OpisPrac)
                        .ToList()
                })
                .OrderBy(o => o.NrZlecenia)
                .ToList();

            model.Data = grouped;
            return View(model);
        }

        // NEW REPORT 5: Employee Overtime Report (Ilość nadgodzin pracowników)
        [HttpGet]
        public async Task<IActionResult> EmployeeOvertime()
        {
            var model = new EmployeeOvertimeReportViewModel();
            
            // Populate available filter options
            model.AvailableYears = await _context.EwidencjaCzasu
                .Select(e => e.Data.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();
                
            model.AvailableMPKs = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.MPK))
                .Select(p => p.MPK!)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
                
            model.AvailableFirmas = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.Firma))
                .Select(p => p.Firma!)
                .Distinct()
                .OrderBy(f => f)
                .ToListAsync();
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EmployeeOvertime(EmployeeOvertimeReportViewModel model)
        {
            // Repopulate filter options
            model.AvailableYears = await _context.EwidencjaCzasu
                .Select(e => e.Data.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();
                
            model.AvailableMPKs = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.MPK))
                .Select(p => p.MPK!)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
                
            model.AvailableFirmas = await _context.Pracownicy
                .Where(p => !string.IsNullOrEmpty(p.Firma))
                .Select(p => p.Firma!)
                .Distinct()
                .OrderBy(f => f)
                .ToListAsync();

            // Apply filters
            var query = _context.EwidencjaCzasu
                .Include(e => e.Pracownik)
                .AsQueryable();

            // Filter by years and months
            if (model.SelectedYears != null && model.SelectedYears.Any())
            {
                if (model.SelectedMonths != null && model.SelectedMonths.Any())
                {
                    query = query.Where(e => model.SelectedYears.Contains(e.Data.Year) && 
                                            model.SelectedMonths.Contains(e.Data.Month));
                }
                else
                {
                    query = query.Where(e => model.SelectedYears.Contains(e.Data.Year));
                }
            }

            // Filter by MPK
            if (model.SelectedMPKs != null && model.SelectedMPKs.Any())
            {
                query = query.Where(e => e.Pracownik.MPK != null && model.SelectedMPKs.Contains(e.Pracownik.MPK));
            }

            // Filter by Firma
            if (model.SelectedFirmas != null && model.SelectedFirmas.Any())
            {
                query = query.Where(e => e.Pracownik.Firma != null && model.SelectedFirmas.Contains(e.Pracownik.Firma));
            }

            var data = await query
                .OrderBy(e => e.Pracownik.Nazwisko)
                .ThenBy(e => e.Pracownik.Imie)
                .ThenBy(e => e.Data)
                .ToListAsync();

            // Filter only overtime entries using OvertimeCalculator
            var overtimeData = data
                .Where(e => TaskFlow.Helpers.OvertimeCalculator.IsOvertime(e.Data, e.GodzinaOd))
                .ToList();

            // Group by employee and calculate overtime with multipliers
            var grouped = overtimeData
                .GroupBy(e => new { e.PracownikId, e.Pracownik.Imie, e.Pracownik.Nazwisko })
                .Select(g => new EmployeeOvertimeRow
                {
                    PracownikId = g.Key.PracownikId,
                    PracownikImie = g.Key.Imie,
                    PracownikNazwisko = g.Key.Nazwisko,
                    Days = g.GroupBy(e => e.Data)
                        .Select(d =>
                        {
                            // Calculate overtime details for each day
                            var entries = d.ToList();
                            var firstEntry = entries.First();
                            var overtimeDetails = TaskFlow.Helpers.OvertimeCalculator.CalculateOvertimeMultiplier(
                                firstEntry.Data,
                                firstEntry.GodzinaOd,
                                firstEntry.GodzinaDo,
                                entries.Sum(e => e.LiczbaGodzin)
                            );

                            return new DayOvertime
                            {
                                Date = d.Key,
                                DayOfWeek = d.Key.ToString("dddd", new CultureInfo("pl-PL")),
                                StandardHours = overtimeDetails.StandardHours,
                                Multiplier = overtimeDetails.Multiplier,
                                TimeZone = overtimeDetails.TimeZone
                            };
                        })
                        .OrderBy(d => d.Date)
                        .ToList()
                })
                .ToList();

            model.Data = grouped;
            return View(model);
        }
    }
}
