using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.Models
{
    public enum AssignmentRole
    {
        Owner = 0,
        Editor = 1,
        Viewer = 2,
    }

    public class TaskAssignment
    {
        public int Id { get; set; }

        public int TaskId { get; set; }
        public Task Task { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public AssignmentRole Role { get; set; } = AssignmentRole.Viewer;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public int AssignedByUserId { get; set; }
        public User AssignedBy { get; set; } = null!;
    }
}
