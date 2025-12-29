using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;

namespace TaskFlow.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SettingsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Audit Log Viewer (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AuditLog(string action = null, string entity = null, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 50)
        {
            var query = _context.AuditLogs.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(action))
            {
                query = query.Where(a => a.Action == action);
            }

            if (!string.IsNullOrEmpty(entity))
            {
                query = query.Where(a => a.EntityType == entity);
            }

            if (from.HasValue)
            {
                query = query.Where(a => a.Timestamp >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(a => a.Timestamp <= to.Value.AddDays(1).AddSeconds(-1));
            }

            // Get total count for pagination
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Get paginated results
            var logs = await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Get distinct actions and entities for filter dropdowns
            ViewBag.Actions = await _context.AuditLogs.Select(a => a.Action).Distinct().ToListAsync();
            ViewBag.Entities = await _context.AuditLogs.Select(a => a.EntityType).Distinct().ToListAsync();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.Action = action;
            ViewBag.Entity = entity;
            ViewBag.From = from;
            ViewBag.To = to;

            return View(logs);
        }

        // Backup System (Admin only)
        [Authorize(Roles = "Admin")]
        public IActionResult Backup()
        {
            var backupDir = Path.Combine(_env.ContentRootPath, "Backups");
            
            if (!Directory.Exists(backupDir))
            {
                Directory.CreateDirectory(backupDir);
            }

            var backups = Directory.GetFiles(backupDir, "taskflow_backup_*.db")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTime)
                .Select(f => new {
                    FileName = f.Name,
                    CreationTime = f.CreationTime,
                    Size = f.Length
                })
                .ToList();

            return View(backups);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateBackup()
        {
            try
            {
                var backupDir = Path.Combine(_env.ContentRootPath, "Backups");
                
                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                }

                var dbPath = Path.Combine(_env.ContentRootPath, "taskflow.db");
                var backupFileName = $"taskflow_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";
                var backupPath = Path.Combine(backupDir, backupFileName);

                // Copy database file
                System.IO.File.Copy(dbPath, backupPath, true);

                TempData["SuccessMessage"] = $"Backup utworzony pomyślnie: {backupFileName}";
                return RedirectToAction(nameof(Backup));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Błąd podczas tworzenia backupu: {ex.Message}";
                return RedirectToAction(nameof(Backup));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult RestoreBackup(string fileName)
        {
            try
            {
                var backupDir = Path.Combine(_env.ContentRootPath, "Backups");
                var backupPath = Path.Combine(backupDir, fileName);

                if (!System.IO.File.Exists(backupPath))
                {
                    TempData["ErrorMessage"] = "Plik backupu nie istnieje.";
                    return RedirectToAction(nameof(Backup));
                }

                var dbPath = Path.Combine(_env.ContentRootPath, "taskflow.db");

                // Create backup of current database before restoring
                var preRestoreBackup = $"taskflow_backup_pre_restore_{DateTime.Now:yyyyMMdd_HHmmss}.db";
                var preRestoreBackupPath = Path.Combine(backupDir, preRestoreBackup);
                System.IO.File.Copy(dbPath, preRestoreBackupPath, true);

                // Restore from backup
                System.IO.File.Copy(backupPath, dbPath, true);

                TempData["SuccessMessage"] = $"Baza danych przywrócona z backupu: {fileName}. Utworzono backup przed przywróceniem: {preRestoreBackup}";
                return RedirectToAction(nameof(Backup));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Błąd podczas przywracania backupu: {ex.Message}";
                return RedirectToAction(nameof(Backup));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteBackup(string fileName)
        {
            try
            {
                var backupDir = Path.Combine(_env.ContentRootPath, "Backups");
                var backupPath = Path.Combine(backupDir, fileName);

                if (System.IO.File.Exists(backupPath))
                {
                    System.IO.File.Delete(backupPath);
                    TempData["SuccessMessage"] = $"Backup usunięty: {fileName}";
                }
                else
                {
                    TempData["ErrorMessage"] = "Plik backupu nie istnieje.";
                }

                return RedirectToAction(nameof(Backup));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Błąd podczas usuwania backupu: {ex.Message}";
                return RedirectToAction(nameof(Backup));
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DownloadBackup(string fileName)
        {
            try
            {
                var backupDir = Path.Combine(_env.ContentRootPath, "Backups");
                var backupPath = Path.Combine(backupDir, fileName);

                if (!System.IO.File.Exists(backupPath))
                {
                    TempData["ErrorMessage"] = "Plik backupu nie istnieje.";
                    return RedirectToAction(nameof(Backup));
                }

                var memory = new MemoryStream();
                using (var stream = new FileStream(backupPath, FileMode.Open))
                {
                    stream.CopyTo(memory);
                }
                memory.Position = 0;

                return File(memory, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Błąd podczas pobierania backupu: {ex.Message}";
                return RedirectToAction(nameof(Backup));
            }
        }
    }
}
