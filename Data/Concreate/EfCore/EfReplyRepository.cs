using BlogApp.Data.Abstract;
using BlogApp.Entity;

namespace BlogApp.Data.Concreate.EfCore
{
    public class EfReplyRepository:IReplyRepository
    {
        private readonly BlogContext _context;
        public EfReplyRepository(BlogContext context)
        {
            _context = context;
        }

        public IQueryable<Reply> Replies => _context.Replies;
        public async Task AddReply(Reply reply)
        {
           await _context.Replies.AddAsync(reply);
           await _context.SaveChangesAsync();
        }
    }
}
