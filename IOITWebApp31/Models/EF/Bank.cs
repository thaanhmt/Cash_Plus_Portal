using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class Bank
    {
        public int BankId { get; set; }
        public string Name { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string BranchName { get; set; }
        public string Note { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }
}
