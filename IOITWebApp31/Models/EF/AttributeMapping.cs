using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class AttributeMapping
    {
        public Guid AttributeMappingId { get; set; }
        public int? AttributeId { get; set; }
        public int? ProductAttributeId { get; set; }
        public int? AttributeValueId { get; set; }
        public bool? IsMain { get; set; }
        public bool? IsView { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }
}
