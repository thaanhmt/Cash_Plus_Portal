using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class News
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public string LinkVideo { get; set; }
        public string Author { get; set; }
        public string Note { get; set; }
        public DateTime? DateStartActive { get; set; }
        public DateTime? DateStartOn { get; set; }
        public DateTime? DateEndOn { get; set; }
        public int? ViewNumber { get; set; }
        public int? Location { get; set; }
        public bool? IsHome { get; set; }
        public bool? IsHot { get; set; }
        public int? TypeNewsId { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public int? LanguageId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public string Introduce { get; set; }
        public string SystemDiagram { get; set; }
        public int? YearTimeline { get; set; }
        public bool? IsAttach { get; set; }
        public double? FactorPrice { get; set; }
        public decimal? ValuePrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public bool? IsCash { get; set; }
        public int? NumberWord { get; set; }
        public int? AuthorId { get; set; }
        public int? UserCreatedId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UserEditedId { get; set; }
        public DateTime? EditingAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public int? UserApprovedId { get; set; }
        public DateTime? ApprovingAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int? UserPublishedId { get; set; }
        public DateTime? PublishingAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public int? UserId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public double? NumberPage { get; set; }
        public int? NumberImage { get; set; }
        public bool? IsComment { get; set; }
        public bool? IsFirst { get; set; }
        public bool? IsShowView { get; set; }
    }
}
