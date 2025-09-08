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
        private readonly IUserService _userService; // Add this line

        private readonly ILogger<TaskController> _logger;

        public TaskController(
            ITaskService taskService,
            ILogger<TaskController> logger,
            IUserService userService
        )
        {
            _taskService = taskService;
            _userService = userService;
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
                            Tags = t.TaskTags?.Select(tt => new
                            {
                                tt.Tag.Id,
                                tt.Tag.Name,
                                tt.Tag.Color,
                                tt.Tag.Description,
                                tt.Tag.CreatedAt,
                                tt.Tag.UpdatedAt,
                            }) ?? Enumerable.Empty<object>(),
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
                        Tags = task.TaskTags?.Select(tt => new
                        {
                            tt.Tag.Id,
                            tt.Tag.Name,
                            tt.Tag.Color,
                            tt.Tag.Description,
                            tt.Tag.CreatedAt,
                            tt.Tag.UpdatedAt,
                        }) ?? Enumerable.Empty<object>(),
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

        // POST /api/tasks/{id}/assign (assign task to user)
        [HttpPost("{id}/assign")]
        public async Task<IActionResult> AssignTask(int id, [FromBody] AssignTaskRequest request)
        {
            try
            {
                var assignedByUserId = GetCurrentUserId();
                if (assignedByUserId == null)
                    return Unauthorized("User not authenticated");

                if (request == null || string.IsNullOrEmpty(request.UserEmail))
                    return BadRequest("User email is required");

                // Look up user by email
                var user = await _userService.GetUserByEmailAsync(request.UserEmail);
                if (user == null)
                    return NotFound("User not found");

                // Check if user is trying to assign to themselves
                if (user.Id == assignedByUserId.Value)
                    return BadRequest("Cannot assign task to yourself");

                // Assign the task
                var success = await _taskService.AssignTaskToUserAsync(
                    id,
                    user.Id,
                    assignedByUserId.Value,
                    request.Role ?? AssignmentRole.Viewer
                );

                if (!success)
                    return NotFound("Task not found or access denied");

                return Ok(
                    new
                    {
                        message = "Task assigned successfully",
                        taskId = id,
                        assignedTo = new
                        {
                            userId = user.Id,
                            username = user.Username,
                            email = user.Email,
                        },
                        role = request.Role ?? AssignmentRole.Viewer,
                        assignedAt = DateTime.UtcNow,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning task {TaskId}", id);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // Add these additional endpoints for managing assignments

        [HttpGet("{id}/assignments")]
        public async Task<IActionResult> GetTaskAssignments(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized("User not authenticated");

                // Check if user has access to this task
                var task = await _taskService.GetTaskByIdAsync(id, userId.Value);
                if (task == null)
                    return NotFound("Task not found");

                // Get assignments for this task
                var assignments = await _taskService.GetTaskAssignmentsAsync(id);

                return Ok(
                    new
                    {
                        taskId = id,
                        assignments = assignments.Select(a => new
                        {
                            a.Id,
                            userId = a.UserId,
                            username = a.User?.Username,
                            email = a.User?.Email,
                            role = a.Role,
                            assignedAt = a.AssignedAt,
                            assignedBy = a.AssignedBy?.Username,
                        }),
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assignments for task {TaskId}", id);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpDelete("{id}/assignments/{assignmentId}")]
        public async Task<IActionResult> RemoveTaskAssignment(int id, int assignmentId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized("User not authenticated");

                var success = await _taskService.RemoveTaskAssignmentAsync(
                    assignmentId,
                    userId.Value
                );
                if (!success)
                    return NotFound("Assignment not found or access denied");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing assignment {AssignmentId}", assignmentId);
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

    public class AssignTaskRequest
    {
        public string UserEmail { get; set; } = string.Empty;
        public AssignmentRole? Role { get; set; }
    }
}
