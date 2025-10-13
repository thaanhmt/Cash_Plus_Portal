using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class NewsApproved
    {
        public Guid NewsApprovedId { get; set; }
        public int? NewsId { get; set; }
        public string Confirms { get; set; }
        public byte? NewsStatus { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
