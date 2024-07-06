namespace BlogApp.Entity
{
    public class Tag
    {
        public enum TagColors
        {
            primary,danger,success,warning,info,secondary
        }
        public int TagId { get; set; }
        public string? Text { get; set; }
        public TagColors? Color { get; set; }
        public string? Url { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }=new List<Post>();
    }
}
