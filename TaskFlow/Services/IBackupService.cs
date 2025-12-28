using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskFlow.Services
{
    public interface IBackupService
    {
        Task<string> CreateBackupAsync();
        Task<bool> RestoreBackupAsync(string backupFileName);
        List<BackupInfo> GetAvailableBackups();
        Task<bool> DeleteBackupAsync(string backupFileName);
    }

    public class BackupInfo
    {
        public string FileName { get; set; }
        public DateTime CreatedDate { get; set; }
        public long FileSizeBytes { get; set; }
        public string FileSizeFormatted => FormatFileSize(FileSizeBytes);

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
