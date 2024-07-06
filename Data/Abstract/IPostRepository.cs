using BlogApp.Entity;

namespace BlogApp.Data.Abstract
{
    public interface IPostRepository
    {
        IQueryable<Post>Posts { get; }
        Task CreatePostAsync(Post post);
        Task BatchUpdatePostAsync(Post post);
        Task UpdatePostAsync(Post post);
        Task Delete(Post post);

    }
}
