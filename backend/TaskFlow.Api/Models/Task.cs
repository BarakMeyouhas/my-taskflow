using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.Models
{
    public enum TaskStatus
    {
        ToDo = 0,
        InProgress = 1,
        Done = 2,
    }

    public enum TaskPriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
    }

    public class Task
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.ToDo;

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public DateTime? DueDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key to User (Owner)
        public int OwnerId { get; set; }

        // Navigation Property
        public User Owner { get; set; } = null!;

        // Navigation Property for many-to-many relationship with Tags
        public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    }
}
