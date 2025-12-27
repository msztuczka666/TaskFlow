using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using TaskFlow.Data;
using TaskFlow.Models;

namespace TaskFlow.Controllers
{
    [Authorize(Roles = "Admin,Kierownik")]
    public class ImportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ImportController(ApplicationDbContext context)
        {
            _context = context;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportPracownicy(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Proszę wybrać plik do importu.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                int imported = 0;
                int errors = 0;

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var pracownik = new Pracownik
                        {
                            Imie = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                            Nazwisko = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                            Firma = worksheet.Cells[row, 3].Value?.ToString(),
                            FirmaId = worksheet.Cells[row, 4].Value?.ToString(),
                        };

                        // Parse SEP date if present
                        if (worksheet.Cells[row, 5].Value != null)
                        {
                            if (DateTime.TryParse(worksheet.Cells[row, 5].Value.ToString(), out DateTime sepDate))
                            {
                                pracownik.DataWaznosciSEP = sepDate;
                            }
                        }

                        _context.Pracownicy.Add(pracownik);
                        imported++;
                    }
                    catch
                    {
                        errors++;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"Zaimportowano {imported} pracowników. Błędy: {errors}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Błąd podczas importu: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ImportNieobecnosci(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Proszę wybrać plik do importu.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                int imported = 0;
                int errors = 0;

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var pracownikId = int.Parse(worksheet.Cells[row, 1].Value?.ToString() ?? "0");
                        var dataOd = DateTime.Parse(worksheet.Cells[row, 2].Value?.ToString() ?? "");
                        var dataDo = DateTime.Parse(worksheet.Cells[row, 3].Value?.ToString() ?? "");
                        var typ = worksheet.Cells[row, 4].Value?.ToString() ?? "";

                        var nieobecnosc = new Nieobecnosc
                        {
                            PracownikId = pracownikId,
                            DataOd = dataOd,
                            DataDo = dataDo,
                            TypNieobecnosci = typ,
                            Uwagi = worksheet.Cells[row, 5].Value?.ToString(),
                            LiczbaDni = CalculateWorkingDays(dataOd, dataDo)
                        };

                        // Apply automatic day reservation for specific absence types
                        if (TypyNieobecnosci.DomyslneLimitDni.ContainsKey(nieobecnosc.TypNieobecnosci))
                        {
                            nieobecnosc.LiczbaDni = TypyNieobecnosci.DomyslneLimitDni[nieobecnosc.TypNieobecnosci];
                        }

                        _context.Nieobecnosci.Add(nieobecnosc);
                        imported++;
                    }
                    catch
                    {
                        errors++;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"Zaimportowano {imported} nieobecności. Błędy: {errors}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Błąd podczas importu: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        private int CalculateWorkingDays(DateTime startDate, DateTime endDate)
        {
            int workingDays = 0;
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }
            return workingDays;
        }
    }
}
