using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public interface IAuditService
    {
        Task LogAsync(string action, string entityType, int? entityId, object oldValues, object newValues, string description = null);
        Task<List<AuditLog>> GetLogsAsync(int page = 1, int pageSize = 50, string entityType = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetTotalCountAsync(string entityType = null, DateTime? startDate = null, DateTime? endDate = null);
    }
}
