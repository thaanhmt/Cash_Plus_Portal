using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class FunctionRole
    {
        public int FunctionRoleId { get; set; }
        public int TargetId { get; set; }
        public int FunctionId { get; set; }
        public string ActiveKey { get; set; }
        public byte? Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }

        public virtual Function Function { get; set; }
    }
}
