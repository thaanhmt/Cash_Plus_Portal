// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class Wards
    {
        public int WardId { get; set; }
        public string Name { get; set; }
        public int? DistrictId { get; set; }
        public string Code { get; set; }
    }
}
