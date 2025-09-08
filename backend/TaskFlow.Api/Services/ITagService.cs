using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services
{
    public interface ITagService
    {
        Task<IEnumerable<Tag>> GetTagsForUserAsync(int userId);
        Task<Tag?> GetTagByIdAsync(int tagId, int userId);
        Task<Tag> CreateTagAsync(Tag tag);
        Task<Tag> UpdateTagAsync(Tag tag, int userId);
        Task<bool> DeleteTagAsync(int tagId, int userId);
        Task<IEnumerable<Tag>> SearchTagsAsync(int userId, string searchTerm);
        Task<bool> AssignTagToTaskAsync(int taskId, int tagId, int userId);
        Task<bool> RemoveTagFromTaskAsync(int taskId, int tagId, int userId);
        Task<IEnumerable<Tag>> GetTagsForTaskAsync(int taskId, int userId);
    }
}
