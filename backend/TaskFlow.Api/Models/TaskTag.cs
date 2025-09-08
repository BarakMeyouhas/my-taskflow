using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.Models
{
    public class TaskTag
    {
        public int Id { get; set; }

        public int TaskId { get; set; }
        public Task Task { get; set; } = null!;

        public int TagId { get; set; }
        public Tag Tag { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key to User who added this tag to the task
        public int AddedByUserId { get; set; }
        public User AddedBy { get; set; } = null!;
    }
}
