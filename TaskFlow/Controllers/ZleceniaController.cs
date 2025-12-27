using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;

namespace TaskFlow.Controllers
{
    [Authorize]
    public class ZleceniaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ZleceniaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Zlecenia
        public async Task<IActionResult> Index()
        {
            return View(await _context.Zlecenia.OrderByDescending(z => z.DataUtworzenia).ToListAsync());
        }

        // GET: Zlecenia/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zlecenie = await _context.Zlecenia
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zlecenie == null)
            {
                return NotFound();
            }

            return View(zlecenie);
        }

        // GET: Zlecenia/Create
        [Authorize(Roles = "Admin,Kierownik")]
        public IActionResult Create()
        {
            ViewData["Statusy"] = StatusyZlecenia.Wszystkie;
            return View();
        }

        // POST: Zlecenia/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Kierownik")]
        public async Task<IActionResult> Create([Bind("Id,Nazwa,Opis,DataRozpoczecia,DataZakonczenia,Status")] Zlecenie zlecenie)
        {
            if (ModelState.IsValid)
            {
                zlecenie.LiczbaDniRoboczych = CalculateWorkingDays(zlecenie.DataRozpoczecia, zlecenie.DataZakonczenia);
                zlecenie.DataUtworzenia = DateTime.Now;
                _context.Add(zlecenie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Statusy"] = StatusyZlecenia.Wszystkie;
            return View(zlecenie);
        }

        // GET: Zlecenia/Edit/5
        [Authorize(Roles = "Admin,Kierownik,Specjalista")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zlecenie = await _context.Zlecenia.FindAsync(id);
            if (zlecenie == null)
            {
                return NotFound();
            }
            ViewData["Statusy"] = StatusyZlecenia.Wszystkie;
            return View(zlecenie);
        }

        // POST: Zlecenia/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Kierownik,Specjalista")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nazwa,Opis,DataRozpoczecia,DataZakonczenia,Status")] Zlecenie zlecenie)
        {
            if (id != zlecenie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    zlecenie.LiczbaDniRoboczych = CalculateWorkingDays(zlecenie.DataRozpoczecia, zlecenie.DataZakonczenia);
                    _context.Update(zlecenie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZlecenieExists(zlecenie.Id))
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
            ViewData["Statusy"] = StatusyZlecenia.Wszystkie;
            return View(zlecenie);
        }

        // GET: Zlecenia/Delete/5
        [Authorize(Roles = "Admin,Kierownik")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zlecenie = await _context.Zlecenia
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zlecenie == null)
            {
                return NotFound();
            }

            return View(zlecenie);
        }

        // POST: Zlecenia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Kierownik")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zlecenie = await _context.Zlecenia.FindAsync(id);
            if (zlecenie != null)
            {
                _context.Zlecenia.Remove(zlecenie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ZlecenieExists(int id)
        {
            return _context.Zlecenia.Any(e => e.Id == id);
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
