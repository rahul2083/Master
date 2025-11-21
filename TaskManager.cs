using master.core.Entities;
using master.core.Enums;
using master.core.Interfaces;

namespace Master.BAL.Services
{
    public class TaskManager : ITaskManager
    {
        private readonly ITaskRepository _tasks;
        private readonly IUserRepository _users;

        public TaskManager(ITaskRepository tasks, IUserRepository users)
        {
            _tasks = tasks;
            _users = users;
        }

        public async Task<(bool ok, string error)> CreateAsync(TaskItem task, int currentUserId, UserRole currentRole)
        {
            if (currentRole != UserRole.Manager) return (false, "Only managers can create tasks");
            if (task.DueDate.HasValue && task.StartDate.HasValue && task.DueDate < task.StartDate)
                return (false, "Due Date cannot be before Start Date");

            if (task.AssigneeId.HasValue)
            {
                var assignee = await _users.GetByIdAsync(task.AssigneeId.Value);
                if (assignee == null || assignee.Role != UserRole.User) return (false, "Invalid assignee");
            }

            await _tasks.AddAsync(task);
            return (true, "");
        }

        public async Task<(bool ok, string error)> UpdateAsync(TaskItem updated, int currentUserId, UserRole currentRole)
        {
            var existing = await _tasks.GetByIdAsync(updated.Id);
            if (existing == null) return (false, "Task not found");
            if (currentRole != UserRole.Manager) return (false, "Only managers can update tasks");
            if (updated.DueDate.HasValue && updated.StartDate.HasValue && updated.DueDate < updated.StartDate)
                return (false, "Due Date cannot be before Start Date");

            if (updated.AssigneeId.HasValue)
            {
                var assignee = await _users.GetByIdAsync(updated.AssigneeId.Value);
                if (assignee == null || assignee.Role != UserRole.User) return (false, "Invalid assignee");
            }

            existing.Title = updated.Title;
            existing.Description = updated.Description;
            existing.ProjectName = string.IsNullOrWhiteSpace(updated.ProjectName) ? "Master" : updated.ProjectName;
            existing.Priority = updated.Priority;
            existing.Status = updated.Status;
            existing.StartDate = updated.StartDate;
            existing.DueDate = updated.DueDate;
            existing.AssigneeId = updated.AssigneeId;

            await _tasks.UpdateAsync(existing);
            return (true, "");
        }

        public async Task<(bool ok, string error)> DeleteAsync(int taskId, int currentUserId, UserRole currentRole)
        {
            if (currentRole != UserRole.Manager) return (false, "Only managers can delete tasks");
            var existing = await _tasks.GetByIdAsync(taskId);
            if (existing == null) return (false, "Task not found");
            await _tasks.DeleteAsync(existing);
            return (true, "");
        }

        public async Task<List<TaskItem>> GetListForUserAsync(int currentUserId, UserRole currentRole)
        {
            if (currentRole == UserRole.Manager) return await _tasks.GetAllAsync();
            return await _tasks.GetByAssigneeIdAsync(currentUserId);
        }

        // IMPLEMENTED: replace the previous explicit interface stub
        public async Task<(bool ok, TaskItem? item, string error)> GetByIdAsync(int id, int currentUserId, UserRole currentRole)
        {
            var task = await _tasks.GetByIdAsync(id);
            if (task == null) return (false, null, "Task not found");
            if (currentRole == UserRole.Manager) return (true, task, "");
            if (task.AssigneeId == currentUserId) return (true, task, "");
            return (false, null, "Forbidden");
        }

        public Task<string?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        // Remove this stub if it exists:
        // Task<string?> ITaskManager.GetByIdAsync(int id) => throw new NotImplementedException();
    }
}
