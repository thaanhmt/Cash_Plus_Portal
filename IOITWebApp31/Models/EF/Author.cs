using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public int? UserMapId { get; set; }
        public byte? Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
        public string Address { get; set; }
        public string Cccd { get; set; }
        public string NumberPhone { get; set; }
        public string FullName { get; set; }
    }
}
