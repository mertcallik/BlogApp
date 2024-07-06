namespace BlogApp.Entity
{
    public class Post
    {
        public int PostId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; }
        public string? Url { get; set; }
        public string? Image { get; set; }
        public DateTime PublishedOn { get; set; }=DateTime.Now;
        public bool IsActive { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; } = null!;
        

        public virtual ICollection<Tag>? Tags { get; set; }=new List<Tag>();
        public virtual ICollection<Comment>? Comments { get; set; }=new List<Comment>();
    }
}
