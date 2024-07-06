using BlogApp.Data.Abstract;
using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concreate.EfCore
{
    public class EfTagRepository : ITagRepository
    {
        private readonly BlogContext _context;
        public EfTagRepository(BlogContext context)
        {
            _context = context;
        }

        public IQueryable<Tag> Tags => _context.Tags.Include(x => x.Posts);

        public async void AddTag(Tag tag)
        {
           await _context.Tags.AddAsync(tag);
           await _context.SaveChangesAsync();
        }
    }
}
