using System.ComponentModel.DataAnnotations;
using BlogApp.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogApp.Models
{
    public class PostCreateViewModel
    {
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
        [Display(Name = "Url")] 
        public string Url { get; set; } = @$"{Guid.NewGuid().ToString().Substring(1,5)}blog";

        [Required]
        [Display(Name = "Kapat")] 
        public string? Image { get; set; } = null??"emptypost.jpg";

        

        public SelectList? Tags { get; set; }
    }
}
