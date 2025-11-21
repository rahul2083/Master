using master.core.Entities;
using master.core.Interfaces;
using Master.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace Master.DAL.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _db;
        public TaskRepository(ApplicationDbContext db) { _db = db; }

        public Task<TaskItem?> GetByIdAsync(int id)
            => _db.Tasks.Include(t => t.Assignee)
                        .Include(t => t.Subtasks)
                        .FirstOrDefaultAsync(t => t.Id == id);

        public Task<List<TaskItem>> GetAllAsync()
            => _db.Tasks.Include(t => t.Assignee)
                        .Include(t => t.Subtasks)
                        .OrderByDescending(t => t.Id)
                        .ToListAsync();

        public Task<List<TaskItem>> GetByAssigneeIdAsync(int assigneeId)
            => _db.Tasks.Include(t => t.Assignee)
                        .Include(t => t.Subtasks)
                        .Where(t => t.AssigneeId == assigneeId)
                        .OrderByDescending(t => t.Id)
                        .ToListAsync();

        public async Task AddAsync(TaskItem task)
        {
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskItem task)
        {
            _db.Tasks.Update(task);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem task)
        {
            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
        }

        public Task SaveAsync() => _db.SaveChangesAsync();

        Task ITaskRepository.DetailAsync(TaskItem task)
        {
            throw new NotImplementedException();
        }



    }
}
