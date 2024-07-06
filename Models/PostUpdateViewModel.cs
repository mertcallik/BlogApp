using System.ComponentModel.DataAnnotations;
using BlogApp.Entity;

namespace BlogApp.Models
{
    public class PostUpdateViewModel
    {
        public int PostId { get; set; }

        [Required]
        [Display(Name = "Başlık")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "İçerik")]
        public string Content { get; set; }
        [Required]
        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Resim")]
        public string? Image { get; set; }

        public bool IsActive { get; set; } = false;

        public List<Tag> AllTags { get; set; } = new List<Tag>();
        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
}
