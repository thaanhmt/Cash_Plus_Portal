using System;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class Customer
    {
        public int CustomerId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Avata { get; set; }
        public int? Sex { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string KeyRandom { get; set; }
        public bool? IsEmailConfirm { get; set; }
        public bool? IsSentEmailConfirm { get; set; }
        public bool? IsPhoneConfirm { get; set; }
        public int? Type { get; set; }
        public int? UnitId { get; set; }
        public int? CountryId { get; set; }
        public int? TypeId { get; set; }
        public string IdNumber { get; set; }
        public DateTime? DateNumber { get; set; }
        public string AddressNumber { get; set; }
        public int? PositionId { get; set; }
        public int? AcademicRankId { get; set; }
        public int? DegreeId { get; set; }
        public int? RoleId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? TypeThirdId { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool? IsNotificationMail { get; set; }
        public bool? IsNotificationWeb { get; set; }
        public bool? IsViewInfo { get; set; }
        public string KeyToken { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }

        // haohv - bổ sung dữ liệu
        public string? StudentCode { get; set; } // mã sinh viên
        public int? SchoolCode { get; set; } // trường
        public string? StudentYear { get; set; } // khoá
        public string? StudentClass { get; set; } // lớp
        public string? AchievementNote { get; set; } // thành tích nổi bật
        public string? HobbyNote { get; set; } // sở thích
        public string? PersonSummary { get; set; } // tóm tắt bản thân
        public string? SocialNetworks { get; set; } // tài khoản mạng xã hội
        public bool? StepTwo { get; set; } // hiển thị top3 bước 2
        public bool? StepFour { get; set; } //  hiển thị top3 bước 4
        public bool? StepFive { get; set; } //  hiển thị top3 bước 5
        public bool? TopThree { get; set; } //  hiển thị top3 vòng 4
        public int? TypeAttributeId { get; set; } //  hiển thị top3 vòng 4
    }
}
