using BlogApp.Entity;

namespace BlogApp.Models
{
    public class PostViewModel
    {
        public List<Post> Posts { get; set; } = new();
        public PostInfo PostInfo { get; set; } = new PostInfo();
    }

    public class PostInfo
    {
        public int PostPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPosts { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalPosts / PostPerPage);
    }
}
