using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class DataSetView
    {
        public Guid DataSetViewId { get; set; }
        public int? ViewNumber { get; set; }
        public long? DataSetId { get; set; }
        public int? ApplicationRangeId { get; set; }
        public int? ResearchAreaId { get; set; }
        public int? UnitId { get; set; }
        public string IpAddress { get; set; }
        public int? CreatedId { get; set; }
        public int? UpdatedId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
