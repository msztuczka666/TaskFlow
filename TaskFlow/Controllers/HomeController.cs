using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;

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

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalPracownicy = await _context.Pracownicy.CountAsync();
        ViewBag.TotalZlecenia = await _context.Zlecenia.CountAsync();
        ViewBag.AktywneZlecenia = await _context.Zlecenia
            .Where(z => z.Status == StatusyZlecenia.WTrakcie || z.Status == StatusyZlecenia.Nowe)
            .CountAsync();
        
        // SEP Expiry warnings (30 days)
        var sepExpiryDate = DateTime.Now.AddDays(30);
        ViewBag.SEPWarnings = await _context.Pracownicy
            .Where(p => p.DataWaznosciSEP.HasValue && p.DataWaznosciSEP.Value <= sepExpiryDate && p.DataWaznosciSEP.Value >= DateTime.Now)
            .ToListAsync();

        // Recent absences
        ViewBag.RecentNieobecnosci = await _context.Nieobecnosci
            .Include(n => n.Pracownik)
            .OrderByDescending(n => n.DataOd)
            .Take(5)
            .ToListAsync();

        return View();
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
