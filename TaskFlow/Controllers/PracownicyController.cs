using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;

namespace TaskFlow.Controllers
{
    [Authorize]
    public class PracownicyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PracownicyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pracownicy
        public async Task<IActionResult> Index()
        {
            var pracownicy = await _context.Pracownicy
                .Include(p => p.Nieobecnosci)
                .ToListAsync();
            return View(pracownicy);
        }

        // GET: Pracownicy/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pracownik = await _context.Pracownicy
                .Include(p => p.Nieobecnosci)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pracownik == null)
            {
                return NotFound();
            }

            return View(pracownik);
        }

        // GET: Pracownicy/Create
        [Authorize(Roles = "Admin,Kierownik")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pracownicy/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Kierownik")]
        public async Task<IActionResult> Create([Bind("Id,Imie,Nazwisko,Firma,NrPrzepustki,MPK,FirmaId,Stanowisko,SEPNr,DataWaznosciSEP,SEPNapiecie,LiczbaDniWolnych,LiczbaDniNaZeszycie,LiczbaDniWykorzystanych,LiczbaDniNaZadanie,TypPracownika,StawkaZlH,StanZatrudnienia,Informacje")] Pracownik pracownik)
        {
            if (ModelState.IsValid)
            {
                // Set default value if not provided
                if (string.IsNullOrEmpty(pracownik.StanZatrudnienia))
                {
                    pracownik.StanZatrudnienia = "Zatrudniony";
                }
                _context.Add(pracownik);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pracownik);
        }

        // GET: Pracownicy/Edit/5
        [Authorize(Roles = "Admin,Kierownik")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pracownik = await _context.Pracownicy.FindAsync(id);
            if (pracownik == null)
            {
                return NotFound();
            }
            return View(pracownik);
        }

        // POST: Pracownicy/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Kierownik")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Imie,Nazwisko,Firma,NrPrzepustki,MPK,FirmaId,Stanowisko,SEPNr,DataWaznosciSEP,SEPNapiecie,LiczbaDniWolnych,LiczbaDniNaZeszycie,LiczbaDniWykorzystanych,LiczbaDniNaZadanie,TypPracownika,StawkaZlH,StanZatrudnienia,Informacje")] Pracownik pracownik)
        {
            if (id != pracownik.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pracownik);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PracownikExists(pracownik.Id))
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
            return View(pracownik);
        }

        // GET: Pracownicy/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pracownik = await _context.Pracownicy
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pracownik == null)
            {
                return NotFound();
            }

            return View(pracownik);
        }

        // POST: Pracownicy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pracownik = await _context.Pracownicy.FindAsync(id);
            if (pracownik != null)
            {
                _context.Pracownicy.Remove(pracownik);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PracownikExists(int id)
        {
            return _context.Pracownicy.Any(e => e.Id == id);
        }
    }
}
