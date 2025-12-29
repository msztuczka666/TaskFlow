using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;

namespace TaskFlow.Controllers
{
    [Authorize]
    public class NieobecnosciController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NieobecnosciController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Nieobecnosci
        public async Task<IActionResult> Index()
        {
            var nieobecnosci = await _context.Nieobecnosci
                .Include(n => n.Pracownik)
                .OrderByDescending(n => n.DataOd)
                .ToListAsync();
            return View(nieobecnosci);
        }

        // GET: Nieobecnosci/Calendar
        public async Task<IActionResult> Calendar()
        {
            var nieobecnosci = await _context.Nieobecnosci
                .Include(n => n.Pracownik)
                .ToListAsync();
            return View(nieobecnosci);
        }

        // GET: Nieobecnosci/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nieobecnosc = await _context.Nieobecnosci
                .Include(n => n.Pracownik)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nieobecnosc == null)
            {
                return NotFound();
            }

            return View(nieobecnosc);
        }

        // GET: Nieobecnosci/Create
        [Authorize(Roles = "Admin,Kierownik,Specjalista")]
        public IActionResult Create()
        {
            // Get only employed workers, sorted A-Z by first name
            var pracownicy = _context.Pracownicy
                .Where(p => p.StanZatrudnienia == "Zatrudniony")
                .OrderBy(p => p.Imie)
                .ThenBy(p => p.Nazwisko)
                .Select(p => new { p.Id, FullName = p.Imie + " " + p.Nazwisko })
                .ToList();
            
            ViewData["PracownikId"] = new SelectList(pracownicy, "Id", "FullName");
            ViewData["TypyNieobecnosci"] = TypyNieobecnosci.Wszystkie;
            return View();
        }

        // POST: Nieobecnosci/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Kierownik,Specjalista")]
        public async Task<IActionResult> Create([Bind("Id,PracownikId,DataOd,DataDo,TypNieobecnosci,Uwagi")] Nieobecnosc nieobecnosc)
        {
            if (ModelState.IsValid)
            {
                // Calculate working days
                nieobecnosc.LiczbaDni = CalculateWorkingDays(nieobecnosc.DataOd, nieobecnosc.DataDo);

                // Apply automatic day reservation for specific absence types
                if (TypyNieobecnosci.DomyslneLimitDni.ContainsKey(nieobecnosc.TypNieobecnosci))
                {
                    nieobecnosc.LiczbaDni = TypyNieobecnosci.DomyslneLimitDni[nieobecnosc.TypNieobecnosci];
                }

                // Calculate hours based on absence type
                nieobecnosc.LiczbaGodzin = TypyNieobecnosci.ObliczGodziny(nieobecnosc.TypNieobecnosci, nieobecnosc.LiczbaDni);

                // Deduct vacation days if this is "Urlop wypoczynkowy" (vacation)
                if (nieobecnosc.TypNieobecnosci == "Urlop wypoczynkowy")
                {
                    var pracownik = await _context.Pracownicy.FindAsync(nieobecnosc.PracownikId);
                    if (pracownik != null)
                    {
                        // Deduct vacation days from employee's available vacation days
                        pracownik.LiczbaDniWykorzystanych += nieobecnosc.LiczbaDni;
                    }
                }

                _context.Add(nieobecnosc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            var pracownicy = _context.Pracownicy
                .Where(p => p.StanZatrudnienia == "Zatrudniony")
                .OrderBy(p => p.Imie)
                .ThenBy(p => p.Nazwisko)
                .Select(p => new { p.Id, FullName = p.Imie + " " + p.Nazwisko })
                .ToList();
            
            ViewData["PracownikId"] = new SelectList(pracownicy, "Id", "FullName", nieobecnosc.PracownikId);
            ViewData["TypyNieobecnosci"] = TypyNieobecnosci.Wszystkie;
            return View(nieobecnosc);
        }

        // GET: Nieobecnosci/Edit/5
        [Authorize(Roles = "Admin,Kierownik,Specjalista")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nieobecnosc = await _context.Nieobecnosci.FindAsync(id);
            if (nieobecnosc == null)
            {
                return NotFound();
            }
            
            var pracownicy = _context.Pracownicy
                .Where(p => p.StanZatrudnienia == "Zatrudniony")
                .OrderBy(p => p.Imie)
                .ThenBy(p => p.Nazwisko)
                .Select(p => new { p.Id, FullName = p.Imie + " " + p.Nazwisko })
                .ToList();
            
            ViewData["PracownikId"] = new SelectList(pracownicy, "Id", "FullName", nieobecnosc.PracownikId);
            ViewData["TypyNieobecnosci"] = TypyNieobecnosci.Wszystkie;
            return View(nieobecnosc);
        }

        // POST: Nieobecnosci/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Kierownik,Specjalista")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PracownikId,DataOd,DataDo,TypNieobecnosci,Uwagi,LiczbaDni")] Nieobecnosc nieobecnosc)
        {
            if (id != nieobecnosc.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Recalculate working days
                    nieobecnosc.LiczbaDni = CalculateWorkingDays(nieobecnosc.DataOd, nieobecnosc.DataDo);

                    // Apply automatic day reservation for specific absence types
                    if (TypyNieobecnosci.DomyslneLimitDni.ContainsKey(nieobecnosc.TypNieobecnosci))
                    {
                        nieobecnosc.LiczbaDni = TypyNieobecnosci.DomyslneLimitDni[nieobecnosc.TypNieobecnosci];
                    }

                    // Recalculate hours based on absence type
                    nieobecnosc.LiczbaGodzin = TypyNieobecnosci.ObliczGodziny(nieobecnosc.TypNieobecnosci, nieobecnosc.LiczbaDni);

                    _context.Update(nieobecnosc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NieobecnoscExists(nieobecnosc.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            var pracownicy = _context.Pracownicy
                .Where(p => p.StanZatrudnienia == "Zatrudniony")
                .OrderBy(p => p.Imie)
                .ThenBy(p => p.Nazwisko)
                .Select(p => new { p.Id, FullName = p.Imie + " " + p.Nazwisko })
                .ToList();
            
            ViewData["PracownikId"] = new SelectList(pracownicy, "Id", "FullName", nieobecnosc.PracownikId);
            ViewData["TypyNieobecnosci"] = TypyNieobecnosci.Wszystkie;
            return View(nieobecnosc);
        }

        // GET: Nieobecnosci/Delete/5
        [Authorize(Roles = "Admin,Kierownik")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nieobecnosc = await _context.Nieobecnosci
                .Include(n => n.Pracownik)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nieobecnosc == null)
            {
                return NotFound();
            }

            return View(nieobecnosc);
        }

        // POST: Nieobecnosci/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Kierownik")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nieobecnosc = await _context.Nieobecnosci.FindAsync(id);
            if (nieobecnosc != null)
            {
                _context.Nieobecnosci.Remove(nieobecnosc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NieobecnoscExists(int id)
        {
            return _context.Nieobecnosci.Any(e => e.Id == id);
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
