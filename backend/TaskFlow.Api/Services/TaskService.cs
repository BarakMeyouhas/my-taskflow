using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TaskService> _logger;

        public TaskService(AppDbContext dbContext, ILogger<TaskService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<Models.Task>> GetTasksForUserAsync(int userId)
        {
            return await _dbContext
                .Tasks.Where(t => t.OwnerId == userId)
                .Include(t => t.Owner)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Models.Task?> GetTaskByIdAsync(int taskId, int userId)
        {
            return await _dbContext
                .Tasks.Where(t => t.Id == taskId && t.OwnerId == userId)
                .Include(t => t.Owner)
                .FirstOrDefaultAsync();
        }

        public async Task<Models.Task> CreateTaskAsync(Models.Task task)
        {
            task.CreatedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;

            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Task created: {TaskId} - {Title}", task.Id, task.Title);
            return task;
        }

        public async Task<Models.Task> UpdateTaskAsync(Models.Task task, int userId)
        {
            var existingTask = await GetTaskByIdAsync(task.Id, userId);
            if (existingTask == null)
                throw new ArgumentException("Task not found or access denied");

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.Status = task.Status;
            existingTask.Priority = task.Priority;
            existingTask.DueDate = task.DueDate;
            existingTask.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Task updated: {TaskId}", task.Id);
            return existingTask;
        }

        public async Task<bool> DeleteTaskAsync(int taskId, int userId)
        {
            var task = await GetTaskByIdAsync(taskId, userId);
            if (task == null)
                return false;

            _dbContext.Tasks.Remove(task);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Task deleted: {TaskId}", taskId);
            return true;
        }

        public async Task<bool> AssignTaskToUserAsync(
            int taskId,
            int userId,
            int assignedByUserId,
            AssignmentRole role
        )
        {
            var task = await _dbContext.Tasks.FindAsync(taskId);
            if (task == null)
                return false;

            var assignment = new TaskAssignment
            {
                TaskId = taskId,
                UserId = userId,
                AssignedByUserId = assignedByUserId,
                Role = role,
                AssignedAt = DateTime.UtcNow,
            };

            _dbContext.TaskAssignments.Add(assignment);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation(
                "Task {TaskId} assigned to user {UserId} with role {Role}",
                taskId,
                userId,
                role
            );
            return true;
        }
    }
}
