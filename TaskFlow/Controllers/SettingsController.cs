using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Models;
using TaskFlow.ViewModels;

namespace TaskFlow.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public SettingsController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewData["Stanowiska"] = Stanowiska.Wszystkie;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Imie = model.Imie,
                    Nazwisko = model.Nazwisko,
                    Stanowisko = model.Stanowisko
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Assign role based on Stanowisko
                    string role = model.Stanowisko switch
                    {
                        "Administrator" => "Admin",
                        "Kierownik budowy" or "Kierownik działu" => "Kierownik",
                        "Specjalista" or "Mistrz" => "Specjalista",
                        _ => "Pracownik"
                    };
                    await _userManager.AddToRoleAsync(user, role);
                    
                    TempData["SuccessMessage"] = $"Użytkownik {model.Imie} {model.Nazwisko} został pomyślnie zarejestrowany.";
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewData["Stanowiska"] = Stanowiska.Wszystkie;
            return View("Index", model);
        }
    }
}
