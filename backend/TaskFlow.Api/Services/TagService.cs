using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services
{
    public class TagService : ITagService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TagService> _logger;

        public TagService(AppDbContext context, ILogger<TagService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Tag>> GetTagsForUserAsync(int userId)
        {
            return await _context.Tags
                .Where(t => t.CreatedByUserId == userId)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<Tag?> GetTagByIdAsync(int tagId, int userId)
        {
            return await _context.Tags
                .FirstOrDefaultAsync(t => t.Id == tagId && t.CreatedByUserId == userId);
        }

        public async Task<Tag> CreateTagAsync(Tag tag)
        {
            // Check if tag with same name already exists for this user
            var existingTag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Name == tag.Name && t.CreatedByUserId == tag.CreatedByUserId);

            if (existingTag != null)
            {
                throw new InvalidOperationException($"A tag with the name '{tag.Name}' already exists.");
            }

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag> UpdateTagAsync(Tag tag, int userId)
        {
            var existingTag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Id == tag.Id && t.CreatedByUserId == userId);

            if (existingTag == null)
            {
                throw new ArgumentException("Tag not found or access denied.");
            }

            // Check if another tag with the same name exists for this user
            var duplicateTag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Name == tag.Name && 
                                         t.CreatedByUserId == userId && 
                                         t.Id != tag.Id);

            if (duplicateTag != null)
            {
                throw new InvalidOperationException($"A tag with the name '{tag.Name}' already exists.");
            }

            existingTag.Name = tag.Name;
            existingTag.Color = tag.Color;
            existingTag.Description = tag.Description;
            existingTag.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingTag;
        }

        public async Task<bool> DeleteTagAsync(int tagId, int userId)
        {
            var tag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Id == tagId && t.CreatedByUserId == userId);

            if (tag == null)
            {
                return false;
            }

            // Remove all task associations first
            var taskTags = await _context.TaskTags
                .Where(tt => tt.TagId == tagId)
                .ToListAsync();

            _context.TaskTags.RemoveRange(taskTags);
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Tag>> SearchTagsAsync(int userId, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetTagsForUserAsync(userId);
            }

            return await _context.Tags
                .Where(t => t.CreatedByUserId == userId && 
                           (t.Name.Contains(searchTerm) || 
                            (t.Description != null && t.Description.Contains(searchTerm))))
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<bool> AssignTagToTaskAsync(int taskId, int tagId, int userId)
        {
            // Check if task exists and user has access
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && t.OwnerId == userId);

            if (task == null)
            {
                return false;
            }

            // Check if tag exists and belongs to user
            var tag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Id == tagId && t.CreatedByUserId == userId);

            if (tag == null)
            {
                return false;
            }

            // Check if association already exists
            var existingAssociation = await _context.TaskTags
                .FirstOrDefaultAsync(tt => tt.TaskId == taskId && tt.TagId == tagId);

            if (existingAssociation != null)
            {
                return true; // Already associated
            }

            var taskTag = new TaskTag
            {
                TaskId = taskId,
                TagId = tagId,
                AddedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.TaskTags.Add(taskTag);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveTagFromTaskAsync(int taskId, int tagId, int userId)
        {
            // Check if task exists and user has access
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && t.OwnerId == userId);

            if (task == null)
            {
                return false;
            }

            var taskTag = await _context.TaskTags
                .FirstOrDefaultAsync(tt => tt.TaskId == taskId && tt.TagId == tagId);

            if (taskTag == null)
            {
                return false;
            }

            _context.TaskTags.Remove(taskTag);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Tag>> GetTagsForTaskAsync(int taskId, int userId)
        {
            // Check if task exists and user has access
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && t.OwnerId == userId);

            if (task == null)
            {
                return Enumerable.Empty<Tag>();
            }

            return await _context.TaskTags
                .Where(tt => tt.TaskId == taskId)
                .Include(tt => tt.Tag)
                .Select(tt => tt.Tag)
                .ToListAsync();
        }
    }
}
