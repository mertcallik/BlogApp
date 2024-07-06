using System.Collections;
using System.Xml.Linq;

namespace BlogApp.Entity
{
    public class User
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? SurName { get; set; }
        public string? FullName => Name + SurName;
        public string? Password { get; set; }
        public string? UserName { get; set; }

        public string? Image { get; set; }

        public string? Email { get; set; }

        public virtual ICollection<Post>? Posts { get; set; }=new List<Post>();
        public virtual ICollection<Comment>? Comments { get; set; }=new List<Comment>();
        public virtual ICollection<Reply>? Replies { get; set; }=new List<Reply>();
    }
}
