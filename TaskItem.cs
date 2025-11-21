using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using master.core.Enums;
using TaskStatus = master.core.Enums.TaskStatus;

namespace master.core.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required, StringLength(100)]
        public string ProjectName { get; set; } = "Master";

        [Required]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.Todo;

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        // Assignee
        [ForeignKey(nameof(Assignee))]
        public int? AssigneeId { get; set; }
        public User? Assignee { get; set; }

        // Navigation
        public List<Subtask> Subtasks { get; set; } = new();
    }
}
