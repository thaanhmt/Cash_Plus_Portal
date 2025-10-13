using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class Related
    {
        public int RelatedId { get; set; }
        public int? TargetId { get; set; }
        public int? TargetRelatedId { get; set; }
        public byte? TargetType { get; set; }
        public int? Location { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
