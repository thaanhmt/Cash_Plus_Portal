using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class Action
    {
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public int? ActionType { get; set; }
        public string TargetId { get; set; }
        public string TargetName { get; set; }
        public string Logs { get; set; }
        public string LastLogs { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Ipaddress { get; set; }
        public int? Time { get; set; }
        public byte? Type { get; set; }
        public int? CompanyId { get; set; }
        public long? UserPushId { get; set; }
        public long? UserId { get; set; }
        public byte? Status { get; set; }
    }
}
