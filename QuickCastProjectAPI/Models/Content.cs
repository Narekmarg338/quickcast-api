namespace QuickCastProjectAPI.Models
{
    public class Content
    {
        public string ContentType { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ContentText { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public List<string> TargetAudience { get; set; }
        public string Priority { get; set; }
        public int? Severity { get; set; }
        public List<string> Attachments { get; set; }
        public List<string> Category { get; set; }
        public string FeaturedImage { get; set; }
        public List<string> ExternalLinks { get; set; }
        public string Source { get; set; }
    }
}
