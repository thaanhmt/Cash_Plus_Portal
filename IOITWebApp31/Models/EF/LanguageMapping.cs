using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class LanguageMapping
    {
        public long LanguageMappingId { get; set; }
        public int? LanguageId1 { get; set; }
        public int? LanguageId2 { get; set; }
        public long? TargetId1 { get; set; }
        public long? TargetId2 { get; set; }
        public byte? TargetType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
