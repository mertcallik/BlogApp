using BlogApp.Entity;

namespace BlogApp.Data.Abstract
{
    public interface IUserRepository
    {
        IQueryable<User> Users { get; }
        Task AddUserAsync(User user);
        Task UpdateAsync(User user);
    }
}
