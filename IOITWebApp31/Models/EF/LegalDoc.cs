using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class LegalDoc
    {
        public int LegalDocId { get; set; }
        public int? LegalDocRootId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? DateIssue { get; set; }
        public DateTime? DateEffect { get; set; }
        public string Signer { get; set; }
        public byte? AgencyIssue { get; set; }
        public int? YearIssue { get; set; }
        public byte? TypeText { get; set; }
        public int? Field { get; set; }
        public string Attactment { get; set; }
        public byte[] AttactmentBit { get; set; }
        public string Contents { get; set; }
        public int? LanguageId { get; set; }
        public string Note { get; set; }
        public string TichYeu { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
        public int? AgencyIssued { get; set; }
        public string Extension { get; set; }
    }
}
