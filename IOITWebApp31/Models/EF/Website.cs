using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class Website
    {
        public int WebsiteId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int? LanguageId { get; set; }
        public int? CompanyId { get; set; }
        public int? WebsiteParentId { get; set; }
        public string LogoHeader { get; set; }
        public string LogoFooter { get; set; }
        public string Banner { get; set; }
        public string Hotline { get; set; }
        public string Hotmail { get; set; }
        public string Fax { get; set; }
        public string OrganizationsUp { get; set; }
        public string Organizations { get; set; }
        public string UnitName { get; set; }
        public string SystemName { get; set; }
        public string Address { get; set; }
        public string GoogleAnalitics { get; set; }
        public string LinkMap { get; set; }
        public string Link1 { get; set; }
        public string Link2 { get; set; }
        public string Link3 { get; set; }
        public string Link4 { get; set; }
        public string Link5 { get; set; }
        public string Link6 { get; set; }
        public string LinkOther1 { get; set; }
        public string LinkOther2 { get; set; }
        public string LinkOther3 { get; set; }
        public string Icon1 { get; set; }
        public string Icon2 { get; set; }
        public string Icon3 { get; set; }
        public string Icon4 { get; set; }
        public string Icon5 { get; set; }
        public string Icon6 { get; set; }
        public string IconBct { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public byte? Status { get; set; }
        public int? HighlightsNewsId { get; set; }
        public string TechNiQuePhone { get; set; }
        public string GuaRanTeePhone { get; set; }
        public string AddressEn { get; set; }
        public string Address2En { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
