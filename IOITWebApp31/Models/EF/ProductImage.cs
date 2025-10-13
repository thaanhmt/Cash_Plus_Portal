using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class ProductImage
    {
        public int ProductImageId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int? ProductId { get; set; }
        public bool? IsImageMain { get; set; }
        public int? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }
}
