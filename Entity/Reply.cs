using System.ComponentModel.DataAnnotations;

namespace BlogApp.Entity
{
    public class Reply
    {
        [Key]
        public int ReplayId { get; set; }
        public string? Text { get; set; }

        public int? CommentId { get; set; } = null!;
        public Comment? Comment { get; set; }

        public int? UserId { get; set; } = null!;
        public User? User { get; set; }
    }
}
