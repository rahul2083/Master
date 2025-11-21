// master.core/Interfaces/ITaskManager.cs
using master.core.Entities;
using master.core.Enums;

namespace master.core.Interfaces
{
    public interface ITaskManager
    {
        Task<(bool ok, string error)> CreateAsync(TaskItem task, int currentUserId, UserRole currentRole);
        Task<(bool ok, string error)> UpdateAsync(TaskItem task, int currentUserId, UserRole currentRole);
        Task<(bool ok, string error)> DeleteAsync(int taskId, int currentUserId, UserRole currentRole);
        Task<List<TaskItem>> GetListForUserAsync(int currentUserId, UserRole currentRole);

        // This signature must exist and be implemented
        Task<(bool ok, TaskItem? item, string error)> GetByIdAsync(int id, int currentUserId, UserRole currentRole);
    }
}
