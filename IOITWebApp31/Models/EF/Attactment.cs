using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class Attactment
    {
        public Guid AttactmentId { get; set; }
        public string Name { get; set; }
        public long? TargetId { get; set; }
        public byte? TargetType { get; set; }
        public string Url { get; set; }
        public string Thumb { get; set; }
        public bool? IsImageMain { get; set; }
        public string Note { get; set; }
        public byte? Extension { get; set; }
        public string ExtensionName { get; set; }
        public long? Storage { get; set; }
        public int? CreatedId { get; set; }
        public int? UpdatedId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
