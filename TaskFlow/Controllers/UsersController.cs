using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Models;
using TaskFlow.ViewModels;

namespace TaskFlow.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.Nazwisko)
                .ThenBy(u => u.Imie)
                .ToListAsync();

            var userViewModels = new List<UserListViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserListViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    Imie = user.Imie ?? "",
                    Nazwisko = user.Nazwisko ?? "",
                    Stanowisko = user.Stanowisko ?? "",
                    Roles = string.Join(", ", roles)
                });
            }

            return View(userViewModels);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["Stanowiska"] = Stanowiska.Wszystkie;
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterViewModel model)
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
                    
                    TempData["Success"] = $"Użytkownik {model.Imie} {model.Nazwisko} został pomyślnie utworzony.";
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewData["Stanowiska"] = Stanowiska.Wszystkie;
            return View(model);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? "",
                Imie = user.Imie ?? "",
                Nazwisko = user.Nazwisko ?? "",
                Stanowisko = user.Stanowisko ?? "",
                CurrentRoles = roles.ToList()
            };

            ViewData["Stanowiska"] = Stanowiska.Wszystkie;
            return View(model);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Email = model.Email;
                user.UserName = model.Email;
                user.Imie = model.Imie;
                user.Nazwisko = model.Nazwisko;
                user.Stanowisko = model.Stanowisko;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    // Update password if provided
                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                    }

                    // Update roles based on Stanowisko
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                    string newRole = model.Stanowisko switch
                    {
                        "Administrator" => "Admin",
                        "Kierownik budowy" or "Kierownik działu" => "Kierownik",
                        "Specjalista" or "Mistrz" => "Specjalista",
                        _ => "Pracownik"
                    };
                    await _userManager.AddToRoleAsync(user, newRole);

                    TempData["Success"] = $"Użytkownik {model.Imie} {model.Nazwisko} został zaktualizowany.";
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewData["Stanowiska"] = Stanowiska.Wszystkie;
            return View(model);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserListViewModel
            {
                Id = user.Id,
                Email = user.Email ?? "",
                Imie = user.Imie ?? "",
                Nazwisko = user.Nazwisko ?? "",
                Stanowisko = user.Stanowisko ?? "",
                Roles = string.Join(", ", roles)
            };

            return View(model);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = $"Użytkownik {user.Imie} {user.Nazwisko} został usunięty.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Nie udało się usunąć użytkownika.";
            return RedirectToAction(nameof(Index));
        }
    }
}
