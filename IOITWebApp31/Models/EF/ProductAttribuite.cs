using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class ProductAttribuite
    {
        public int ProductAttributeId { get; set; }
        public int? ProductId { get; set; }
        public string Code { get; set; }
        public string Image { get; set; }
        public bool? IsDownload { get; set; }
        public bool? IsVirtual { get; set; }
        public bool? IsBranch { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceSpecial { get; set; }
        public DateTime? PriceSpecialStart { get; set; }
        public DateTime? PriceSpecialEnd { get; set; }
        public byte? BranchStatus { get; set; }
        public string Description { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public int? MinStock { get; set; }
        public int? MaxStock { get; set; }
        public int? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }
}
