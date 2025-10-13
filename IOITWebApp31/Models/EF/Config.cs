using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class Config
    {
        public int ConfigId { get; set; }
        public bool? IsLog { get; set; }
        public string EmailReceive { get; set; }
        public string EmailHost { get; set; }
        public string EmailSender { get; set; }
        public bool? EmailEnableSsl { get; set; }
        public string EmailUserName { get; set; }
        public string EmailDisplayName { get; set; }
        public string EmailPasswordHash { get; set; }
        public int? EmailPort { get; set; }
        public string EmailColorBody { get; set; }
        public string EmailColorHeader { get; set; }
        public string EmailColorFooter { get; set; }
        public string EmailLogo { get; set; }
        public int? ConpanyId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public bool? IsOnePay { get; set; }
        public string OpMerchant { get; set; }
        public string OpAccessCode { get; set; }
        public string OpKey { get; set; }
        public string OpUser { get; set; }
        public string OpPassword { get; set; }
        public decimal? ExchangRate { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public string HeaderScript { get; set; }
        public string BodyScript { get; set; }
        public string CustomCss { get; set; }
        public string FooterScript { get; set; }
        public bool? ModeSite { get; set; }
    }
}
