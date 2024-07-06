using BlogApp.Entity;

namespace BlogApp.Data.Abstract
{
    public interface IReplyRepository
    {
        IQueryable<Reply>Replies { get; }
        Task AddReply(Reply reply);
    }
}
