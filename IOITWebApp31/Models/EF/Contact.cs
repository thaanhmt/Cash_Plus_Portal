using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class Contact
    {
        public int ContactId { get; set; }
        public int? CustomerId { get; set; }
        public string Title { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Note { get; set; }
        public int? NewsId { get; set; }
        public int? TypeContact { get; set; }
        public int? CompanyId { get; set; }
        public int? BranchId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
        public string Contents { get; set; }
        public string Address { get; set; }
        public string Attactment { get; set; }
    }
}
