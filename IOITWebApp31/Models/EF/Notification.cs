using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class Notification
    {
        public Guid NotificationId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public int? UserPushId { get; set; }
        public int? UserReadId { get; set; }
        public bool? IsSentEmail { get; set; }
        public string UrlLink { get; set; }
        public string TargetId { get; set; }
        public byte? TargetType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
