using master.core.Entities;

namespace master.core.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskItem?> GetByIdAsync(int id);
        Task<List<TaskItem>> GetAllAsync();
        Task<List<TaskItem>> GetByAssigneeIdAsync(int assigneeId);
        Task AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(TaskItem task);
        Task DetailAsync(TaskItem task);
        Task SaveAsync();
    }
}
