using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class DataSet
    {
        public long DataSetId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public string LinkVideo { get; set; }
        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }
        public string AuthorPhone { get; set; }
        public string Version { get; set; }
        public string Note { get; set; }
        public DateTime? DateStartActive { get; set; }
        public DateTime? DateStartOn { get; set; }
        public DateTime? DateEndOn { get; set; }
        public long? DownNumber { get; set; }
        public long? ViewNumber { get; set; }
        public byte? RateStar { get; set; }
        public int? Location { get; set; }
        public bool? IsHot { get; set; }
        public int? Type { get; set; }
        public int? ApplicationRangeId { get; set; }
        public int? ResearchAreaId { get; set; }
        public int? UnitId { get; set; }
        public bool? IsPublish { get; set; }
        public string ConfirmsPrivate { get; set; }
        public string ConfirmsPublish { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public int? LicenseId { get; set; }
        public int? LanguageId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserCreatedId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UserEditedId { get; set; }
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
    }
}
