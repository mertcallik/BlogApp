namespace BlogApp.Entity
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string? CommentText { get; set; }
        public DateTime PublishedOn { get; set; }

        public int? UserId { get; set; } = null!;
        public User? User { get; set; } 
        public int? PostId { get; set; } = null!;
        public Post? Post { get; set; } 
        public List<Reply> Replies { get; set; } = new List<Reply>();
    }
}
