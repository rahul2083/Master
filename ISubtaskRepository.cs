using master.core.Entities;

namespace master.core.Interfaces
{
    public interface ISubtaskRepository
    {
        Task<Subtask?> GetByIdAsync(int id);
        Task<List<Subtask>> GetByTaskIdAsync(int taskId);

        Task AddAsync(Subtask subtask);
        Task UpdateAsync(Subtask subtask);
        Task DeleteAsync(Subtask subtask);
    }
}
