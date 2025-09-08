using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(7)] // For hex color codes like #FF5733
        public string? Color { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key to User (Creator/Owner)
        public int CreatedByUserId { get; set; }

        // Navigation Property
        public User CreatedBy { get; set; } = null!;

        // Navigation Property for many-to-many relationship with Tasks
        public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    }
}
