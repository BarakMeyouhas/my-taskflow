using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Models;
using TaskFlow.Api.Services;
using TaskPriority = TaskFlow.Api.Models.TaskPriority;
using TaskStatus = TaskFlow.Api.Models.TaskStatus;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(ITaskService taskService, ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        // GET /api/tasks (list all tasks for user)
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized("User not authenticated");

                var tasks = await _taskService.GetTasksForUserAsync(userId.Value);

                return Ok(
                    new
                    {
                        count = tasks.Count(),
                        tasks = tasks.Select(t => new
                        {
                            t.Id,
                            t.Title,
                            t.Description,
                            t.Status,
                            t.Priority,
                            t.DueDate,
                            t.CreatedAt,
                            t.UpdatedAt,
                            OwnerId = t.OwnerId,
                            OwnerName = t.Owner?.Username,
                        }),
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks for user");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // GET /api/tasks/{id} (get specific task)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized("User not authenticated");

                var task = await _taskService.GetTaskByIdAsync(id, userId.Value);
                if (task == null)
                    return NotFound("Task not found");

                return Ok(
                    new
                    {
                        task.Id,
                        task.Title,
                        task.Description,
                        task.Status,
                        task.Priority,
                        task.DueDate,
                        task.CreatedAt,
                        task.UpdatedAt,
                        OwnerId = task.OwnerId,
                        OwnerName = task.Owner?.Username,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task {TaskId}", id);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // POST /api/tasks (create new task)
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized("User not authenticated");

                if (request == null || string.IsNullOrEmpty(request.Title))
                    return BadRequest("Title is required");

                var task = new Models.Task
                {
                    Title = request.Title,
                    Description = request.Description,
                    Status = request.Status ?? TaskStatus.ToDo,
                    Priority = request.Priority ?? TaskPriority.Medium,
                    DueDate = request.DueDate,
                    OwnerId = userId.Value,
                };

                var createdTask = await _taskService.CreateTaskAsync(task);

                return CreatedAtAction(
                    nameof(GetTask),
                    new { id = createdTask.Id },
                    new
                    {
                        createdTask.Id,
                        createdTask.Title,
                        createdTask.Description,
                        createdTask.Status,
                        createdTask.Priority,
                        createdTask.DueDate,
                        createdTask.CreatedAt,
                        createdTask.UpdatedAt,
                        OwnerId = createdTask.OwnerId,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // PUT /api/tasks/{id} (update task)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized("User not authenticated");

                if (request == null || string.IsNullOrEmpty(request.Title))
                    return BadRequest("Title is required");

                var task = new Models.Task
                {
                    Id = id,
                    Title = request.Title,
                    Description = request.Description,
                    Status = request.Status ?? TaskStatus.ToDo,
                    Priority = request.Priority ?? TaskPriority.Medium,
                    DueDate = request.DueDate,
                    OwnerId = userId.Value,
                };

                var updatedTask = await _taskService.UpdateTaskAsync(task, userId.Value);

                return Ok(
                    new
                    {
                        updatedTask.Id,
                        updatedTask.Title,
                        updatedTask.Description,
                        updatedTask.Status,
                        updatedTask.Priority,
                        updatedTask.DueDate,
                        updatedTask.CreatedAt,
                        updatedTask.UpdatedAt,
                        OwnerId = updatedTask.OwnerId,
                    }
                );
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId}", id);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // DELETE /api/tasks/{id} (delete task)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized("User not authenticated");

                var deleted = await _taskService.DeleteTaskAsync(id, userId.Value);
                if (!deleted)
                    return NotFound("Task not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {TaskId}", id);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // Helper method to get current user ID from JWT token
        private int? GetCurrentUserId()
        {
            // For now, we'll extract from Authorization header
            // In a real implementation, you'd use proper JWT middleware
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
                return null;

            try
            {
                // This is a simplified approach - in production you'd use proper JWT middleware
                // For now, we'll return a hardcoded user ID for testing
                // TODO: Implement proper JWT token parsing
                return 1; // Hardcoded for testing - replace with actual JWT parsing
            }
            catch
            {
                return null;
            }
        }
    }

    // Request DTOs
    public class CreateTaskRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatus? Status { get; set; }
        public TaskPriority? Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class UpdateTaskRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatus? Status { get; set; }
        public TaskPriority? Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
