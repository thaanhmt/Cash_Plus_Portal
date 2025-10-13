using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class ConfigStar
    {
        public int ConfigStarId { get; set; }
        public string StarColor { get; set; }
        public byte? Star { get; set; }
        public int? FromView { get; set; }
        public int? ToView { get; set; }
        public byte? Operator { get; set; }
        public int? FromDownload { get; set; }
        public int? ToDownload { get; set; }
        public int? LanguageId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }
}
