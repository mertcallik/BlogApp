using System.Collections;
using BlogApp.Data.Abstract;
using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concreate.EfCore
{
    public class EfPostRepository : IPostRepository
    {
        private BlogContext _context;
        public EfPostRepository(BlogContext context)
        {
            _context = context;
        }
        public IQueryable<Post> Posts => _context.Posts.Include(x => x.Tags);

        public async Task CreatePostAsync(Post post)
        {

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
        }

        public async Task BatchUpdatePostAsync(Post entity)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.PostId == entity.PostId);
            post!.IsActive = entity.IsActive;
            _context.Update(post);

            await _context.SaveChangesAsync();
        }

        public async Task UpdatePostAsync(Post entity)
        {
            var post = await _context.Posts.Include(x=>x.Tags).FirstOrDefaultAsync(p => p.PostId == entity.PostId);
            
            if (post != null)
            {
                post.Title = entity.Title;
                post.Content = entity.Content;
                post.Image = entity.Image;
                post.Description = entity.Description;
                post.IsActive = entity.IsActive;
                post.Tags= entity.Tags;
                await _context.SaveChangesAsync();
            }

        }

        public async Task Delete(Post post)
        {
            var comments = post.Comments;
            foreach (var comment in comments)
            {
                var replies = comment.Replies;
                foreach (var reply in replies)
                {
                    _context.Replies.Remove(reply);
                }
                _context.Comments.Remove(comment);
            }
            _context.Posts.Remove(post);
           await _context.SaveChangesAsync();
        }
    }
}
