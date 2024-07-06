using BlogApp.Data.Abstract;
using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concreate.EfCore
{
    public class EfUserRepository:IUserRepository
    {
        private readonly BlogContext _context;
        public EfUserRepository(BlogContext context)
        {
            _context = context;
        }
        public IQueryable<User> Users => _context.Users;
        public async Task AddUserAsync(User user)
        {
           await _context.Users.AddAsync(user);
           await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            var userToUpdate= await _context.Users.FirstOrDefaultAsync(x => x.UserId == user.UserId);
            if (userToUpdate != null)
            {
                userToUpdate.Email = user.Email??userToUpdate.Email;
                userToUpdate.UserName = user.UserName?? userToUpdate.UserName;
                userToUpdate.Image = user.Image ?? userToUpdate.Image;
                userToUpdate.Name=user.Name?? userToUpdate.Name;
                userToUpdate.SurName = user.SurName ?? userToUpdate.SurName;
                userToUpdate.Password = user.Password ?? userToUpdate.Password;
                await _context.SaveChangesAsync();
                
            }
            

        }
    }
}
