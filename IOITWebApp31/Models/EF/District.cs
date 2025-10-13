// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class District
    {
        public int DistrictId { get; set; }
        public int? ProvinceId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? Priority { get; set; }
    }
}
