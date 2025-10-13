using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class Unit
    {
        public int UnitId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string NameEn { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string IdNumber { get; set; }
        public DateTime? DateNumber { get; set; }
        public string AddressNumber { get; set; }
        public int UnitParentId { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string Icon { get; set; }
        public string IconFa { get; set; }
        public bool? IconText { get; set; }
        public int? Location { get; set; }
        public int? Type { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public string Address { get; set; }
        public int? AdminId { get; set; }
        public string EmailAdmin { get; set; }
        public string NameAdmin { get; set; }
        public int? LanguageId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public int? CreatedId { get; set; }
        public int? UpdatedId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
