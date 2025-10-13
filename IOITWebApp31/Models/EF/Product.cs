using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class Product
    {
        public int ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public bool? IsHome { get; set; }
        public bool? IsHot { get; set; }
        public bool? IsSale { get; set; }
        public int? StockQuantity { get; set; }
        public decimal? PriceSale { get; set; }
        public decimal? PriceImport { get; set; }
        public decimal? PriceSpecial { get; set; }
        public decimal? PriceOther { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public DateTime? DateStartActive { get; set; }
        public DateTime? DateStartOn { get; set; }
        public DateTime? DateEndOn { get; set; }
        public string ProductAttributes { get; set; }
        public string ProductNote { get; set; }
        public string NoteTech { get; set; }
        public string NotePromotion { get; set; }
        public int? ViewNumber { get; set; }
        public int? LikeNumber { get; set; }
        public int? CommentNumber { get; set; }
        public int? Discount { get; set; }
        public double? PointStar { get; set; }
        public byte? TypeProduct { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public int? TypeImagePromotionId { get; set; }
        public int? TrademarkId { get; set; }
        public int? ManufacturerId { get; set; }
        public int? LanguageId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public string Introduce { get; set; }
        public string Feature { get; set; }
        public string Configuration { get; set; }
        public string OriginProduct { get; set; }
        public string GuaranteeProduct { get; set; }
    }
}
