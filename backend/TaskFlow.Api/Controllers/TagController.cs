using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Models;
using TaskFlow.Api.Services;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly ILogger<TagController> _logger;

        public TagController(ITagService tagService, ILogger<TagController> logger)
        {
            _tagService = tagService;
            _logger = logger;
        }

        // GET /api/tags (list all tags for user)
        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized("User not authenticated");

                var tags = await _tagService.GetTagsForUserAsync(userId.Value);

                return Ok(new
                {
                    count = tags.Count(),
                    tags = tags.Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Color,
                        t.Description,
                        t.CreatedAt,
                        t.UpdatedAt,
                        CreatedByUserId = t.CreatedByUserId,
                        CreatedBy = t.CreatedBy?.Username
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tags for user");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // GET /api/tags/{id} (get specific tag)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTag(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized("User not authenticated");

                var tag = await _tagService.GetTagByIdAsync(id, userId.Value);
                if (tag == null)
                    return NotFound("Tag not found");

                return Ok(new
                {
                    tag.Id,
                    tag.Name,
                    tag.Color,
                    tag.Description,
                    tag.CreatedAt,
                    tag.UpdatedAt,
                    CreatedByUserId = tag.CreatedByUserId,
                    CreatedBy = tag.CreatedBy?.Username
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tag {TagId}", id);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // POST /api/tags (create new tag)
        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized("User not authenticated");

                if (request == null || string.IsNullOrEmpty(request.Name))
                    return BadRequest("Tag name is required");

                var tag = new Tag
                {
                    Name = request.Name.Trim(),
                    Color = request.Color,
                    Description = request.Description,
                    CreatedByUserId = userId.Value
                };

                var createdTag = await _tagService.CreateTagAsync(tag);

                return CreatedAtAction(
                    nameof(GetTag),
                    new { id = createdTag.Id },
                    new
                    {
                        createdTag.Id,
                        createdTag.Name,
                        createdTag.Color,
                        createdTag.Description,
                        createdTag.CreatedAt,
                        createdTag.UpdatedAt,
                        CreatedByUserId = createdTag.CreatedByUserId
                    }
                );
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tag");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // PUT /api/tags/{id} (update tag)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] UpdateTagRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized("User not authenticated");

                if (request == null || string.IsNullOrEmpty(request.Name))
                    return BadRequest("Tag name is required");

                var tag = new Tag
                {
                    Id = id,
                    Name = request.Name.Trim(),
                    Color = request.Color,
                    Description = request.Description,
                    CreatedByUserId = userId.Value
                };

                var updatedTag = await _tagService.UpdateTagAsync(tag, userId.Value);

                return Ok(new
                {
                    updatedTag.Id,
                    updatedTag.Name,
                    updatedTag.Color,
                    updatedTag.Description,
                    updatedTag.CreatedAt,
                    updatedTag.UpdatedAt,
                    CreatedByUserId = updatedTag.CreatedByUserId
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tag {TagId}", id);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // DELETE /api/tags/{id} (delete tag)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized("User not authenticated");

                var deleted = await _tagService.DeleteTagAsync(id, userId.Value);
                if (!deleted)
                    return NotFound("Tag not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tag {TagId}", id);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // GET /api/tags/search?q={searchTerm} (search tags)
        [HttpGet("search")]
        public async Task<IActionResult> SearchTags([FromQuery] string q)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized("User not authenticated");

                var tags = await _tagService.SearchTagsAsync(userId.Value, q ?? string.Empty);

                return Ok(new
                {
                    count = tags.Count(),
                    searchTerm = q,
                    tags = tags.Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Color,
                        t.Description,
                        t.CreatedAt,
                        t.UpdatedAt,
                        CreatedByUserId = t.CreatedByUserId,
                        CreatedBy = t.CreatedBy?.Username
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching tags");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // POST /api/tags/{tagId}/assign/{taskId} (assign tag to task)
        [HttpPost("{tagId}/assign/{taskId}")]
        public async Task<IActionResult> AssignTagToTask(int tagId, int taskId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized("User not authenticated");

                var success = await _tagService.AssignTagToTaskAsync(taskId, tagId, userId.Value);
                if (!success)
                    return NotFound("Task or tag not found, or access denied");

                return Ok(new
                {
                    message = "Tag assigned to task successfully",
                    taskId = taskId,
                    tagId = tagId,
                    assignedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning tag {TagId} to task {TaskId}", tagId, taskId);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // DELETE /api/tags/{tagId}/assign/{taskId} (remove tag from task)
        [HttpDelete("{tagId}/assign/{taskId}")]
        public async Task<IActionResult> RemoveTagFromTask(int tagId, int taskId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized("User not authenticated");

                var success = await _tagService.RemoveTagFromTaskAsync(taskId, tagId, userId.Value);
                if (!success)
                    return NotFound("Task or tag not found, or access denied");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing tag {TagId} from task {TaskId}", tagId, taskId);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // GET /api/tags/task/{taskId} (get tags for a specific task)
        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetTagsForTask(int taskId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized("User not authenticated");

                var tags = await _tagService.GetTagsForTaskAsync(taskId, userId.Value);

                return Ok(new
                {
                    taskId = taskId,
                    count = tags.Count(),
                    tags = tags.Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Color,
                        t.Description,
                        t.CreatedAt,
                        t.UpdatedAt,
                        CreatedByUserId = t.CreatedByUserId,
                        CreatedBy = t.CreatedBy?.Username
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tags for task {TaskId}", taskId);
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
    public class CreateTagRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Color { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateTagRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Color { get; set; }
        public string? Description { get; set; }
    }
}
