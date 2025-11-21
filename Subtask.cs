using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using master.core.Enums;
using TaskStatus = master.core.Enums.TaskStatus;

namespace master.core.Entities
{
    public class Subtask
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        public bool IsDone { get; set; } = false;

        public TaskStatus Status { get; set; } = TaskStatus.Todo; // Use your enum value

        [ForeignKey(nameof(TaskItem))]
        public int TaskItemId { get; set; }
        public TaskItem? TaskItem { get; set; }
    }
}
