using master.core.Entities;
using master.core.Interfaces;
using Master.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace Master.DAL.Repositories
{
    public class SubtaskRepository : ISubtaskRepository
    {
        private readonly ApplicationDbContext _db;

        public SubtaskRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Subtask?> GetByIdAsync(int id)
        {
            return await _db.Subtasks.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Subtask>> GetByTaskIdAsync(int taskId)
        {
            return await _db.Subtasks
                .Where(s => s.TaskItemId == taskId)
                .ToListAsync();
        }

        public async Task AddAsync(Subtask subtask)
        {
            _db.Subtasks.Add(subtask);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Subtask subtask)
        {
            _db.Subtasks.Update(subtask);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Subtask subtask)
        {
            _db.Subtasks.Remove(subtask);
            await _db.SaveChangesAsync();
        }
    }
}
