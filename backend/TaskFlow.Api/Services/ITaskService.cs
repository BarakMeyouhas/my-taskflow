using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<Models.Task>> GetTasksForUserAsync(int userId);
        Task<Models.Task?> GetTaskByIdAsync(int taskId, int userId);
        Task<Models.Task> CreateTaskAsync(Models.Task task);
        Task<Models.Task> UpdateTaskAsync(Models.Task task, int userId);
        Task<bool> DeleteTaskAsync(int taskId, int userId);
        Task<bool> AssignTaskToUserAsync(
            int taskId,
            int userId,
            int assignedByUserId,
            AssignmentRole role
        );
        Task<IEnumerable<TaskAssignment>> GetTaskAssignmentsAsync(int taskId);
        Task<bool> RemoveTaskAssignmentAsync(int assignmentId, int userId);
        Task<IEnumerable<Models.Task>> SearchTasksAsync(int userId, string searchTerm);
    }
}
