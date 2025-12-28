using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TaskFlow.Services
{
    public class BackupService : IBackupService
    {
        private readonly string _dbPath;
        private readonly string _backupFolder;

        public BackupService(IConfiguration configuration)
        {
            // Get database path from connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            _dbPath = connectionString.Replace("Data Source=", "").Split(';')[0];

            // Create backups folder if it doesn't exist
            _backupFolder = Path.Combine(Path.GetDirectoryName(_dbPath), "Backups");
            if (!Directory.Exists(_backupFolder))
            {
                Directory.CreateDirectory(_backupFolder);
            }
        }

        public async Task<string> CreateBackupAsync()
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var backupFileName = $"taskflow_backup_{timestamp}.db";
                var backupPath = Path.Combine(_backupFolder, backupFileName);

                // Copy the database file
                await Task.Run(() => File.Copy(_dbPath, backupPath, true));

                return backupFileName;
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas tworzenia backupu: {ex.Message}", ex);
            }
        }

        public async Task<bool> RestoreBackupAsync(string backupFileName)
        {
            try
            {
                var backupPath = Path.Combine(_backupFolder, backupFileName);

                if (!File.Exists(backupPath))
                {
                    throw new FileNotFoundException($"Plik backupu nie istnieje: {backupFileName}");
                }

                // Create a backup of current database before restoring
                var currentBackupName = $"before_restore_{DateTime.Now:yyyyMMdd_HHmmss}.db";
                var currentBackupPath = Path.Combine(_backupFolder, currentBackupName);
                File.Copy(_dbPath, currentBackupPath, true);

                // Restore the backup
                await Task.Run(() =>
                {
                    File.Copy(backupPath, _dbPath, true);
                });

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas przywracania backupu: {ex.Message}", ex);
            }
        }

        public List<BackupInfo> GetAvailableBackups()
        {
            try
            {
                var backupFiles = Directory.GetFiles(_backupFolder, "*.db");

                return backupFiles.Select(f => new BackupInfo
                {
                    FileName = Path.GetFileName(f),
                    CreatedDate = File.GetCreationTime(f),
                    FileSizeBytes = new FileInfo(f).Length
                })
                .OrderByDescending(b => b.CreatedDate)
                .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas pobierania listy backupów: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteBackupAsync(string backupFileName)
        {
            try
            {
                var backupPath = Path.Combine(_backupFolder, backupFileName);

                if (File.Exists(backupPath))
                {
                    await Task.Run(() => File.Delete(backupPath));
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas usuwania backupu: {ex.Message}", ex);
            }
        }
    }
}
