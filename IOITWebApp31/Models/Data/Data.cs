using System;
using System.Collections.Generic;

namespace IOITWebApp31.Models.Data
{
    public class Data
    {

    }

    public partial class UserInfo
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public string Avata { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }

    public partial class ActionPush
    {
        public string ActionId { get; set; }
        public string ActionName { get; set; }
        public string FullName { get; set; }
        public string Avata { get; set; }
        public string TargetName { get; set; }
    }


    public partial class DownloadFile
    {
        public string Link { get; set; }

    }

    public partial class MenuItems
    {
        public int CategoryId { get; set; }
        public int MenuItemId { get; set; }
        public int MenuId { get; set; }
        public int? MenuParentId { get; set; }
        public int? Location { get; set; }
        public string CategoryName { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public int? TypeCategoryId { get; set; }
    }

    public partial class Search
    {
        [System.ComponentModel.DefaultValue("")]
        public string sName { get; set; }
        [System.ComponentModel.DefaultValue("")]
        public string sNameA { get; set; }
        [System.ComponentModel.DefaultValue(-1)]
        public int sType { get; set; }
        [System.ComponentModel.DefaultValue(-1)]
        public int typeS { get; set; }
        [System.ComponentModel.DefaultValue(-1)]
        public int sCategory { get; set; }
    }

    public partial class Register
    {
        public string sName { get; set; }
        public int sType { get; set; }
    }

    public partial class CustomerLogin
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Avata { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int? Sex { get; set; }
        public string Address { get; set; }
        public string PhomeNumber { get; set; }
        public string Email { get; set; }
        public int? Type { get; set; }
        public int? RoleId { get; set; }
        public int? Status { get; set; }
        public int? TypeThirdId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedAtStr { get; set; }
        public string access_token { get; set; }
        public string access_key { get; set; }
        public List<MenuDTO> listMenus { get; set; }
        public List<int> listUnits { get; set; }
        public bool? IsEmailConfirm { get; set; }
        public int? NunberNotification { get; set; }
        public string KeyRandom { get; set; }
        public string KeyToken { get; set; }
    }

    public partial class ProductDT
    {
        public int ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? PriceSale { get; set; }
        public decimal? PriceSpecial { get; set; }
        public byte? TypeProduct { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public DateTime? DateStartActive { get; set; }
        public double? Discount { get; set; }
        public double? PointStar { get; set; }
        public int? CategoryId { get; set; }
        public int? ManufacturerId { get; set; }
        public int? TrademarkId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    //public partial class BidKoiDT
    //{
    //    public Guid AutionHistoryId { get; set; }
    //    public string NickName { get; set; }
    //    public int SessionAutionId { get; set; }
    //    public int CustomerId { get; set; }
    //    public int ProductId { get; set; }
    //    public string ProductCode { get; set; }
    //    public byte? TypeBid { get; set; }
    //    public decimal PriceOld { get; set; }
    //    public decimal? PriceNew { get; set; }
    //    public DateTime? CreatedAt { get; set; }
    //    public byte? Status { get; set; }
    //}

    //public partial class NickNameDT
    //{
    //    public Guid NickNameId { get; set; }
    //    public string NickName { get; set; }
    //    public int? CustomerId { get; set; }
    //    public int? SessionAutionId { get; set; }
    //    public int? ProductId { get; set; }
    //    public DateTime? CreatedAt { get; set; }
    //    public byte? Status { get; set; }
    //}

    public partial class DetailRatingStar
    {
        public int? item_count { get; set; }
        public float? star { get; set; }
        public int? star1 { get; set; }
        public int? star2 { get; set; }
        public int? star3 { get; set; }
        public int? star4 { get; set; }
        public int? star5 { get; set; }
        public int? countStar1 { get; set; }
        public int? countStar2 { get; set; }
        public int? countStar3 { get; set; }
        public int? countStar4 { get; set; }
        public int? countStar5 { get; set; }
    }

    public partial class CategoryDT
    {
        public int CategoryMappingId { get; set; }
        public int? CategoryId { get; set; }
        public int? TargetId { get; set; }
        public string TargetName { get; set; }
        public int? TargetType { get; set; }
        public int? Location { get; set; }
        public int? Status { get; set; }
    }

    //public partial class SessionProductDT
    //{
    //    public int SessionProductId { get; set; }
    //    public int? SessionAutionId { get; set; }
    //    public int? ProductId { get; set; }
    //    public int? CustomerWinId { get; set; }
    //    public decimal? PriceStart { get; set; }
    //    public decimal? PriceWin { get; set; }
    //    public string NickName { get; set; }
    //    public DateTime? DateTimeWin { get; set; }
    //    public DateTime? CreatedAt { get; set; }
    //    public DateTime? UpdatedAt { get; set; }
    //    public int? UserId { get; set; }
    //    public byte? Status { get; set; }
    //    public bool? IsHome { get; set; }
    //    public int? TypeAuction { get; set; }
    //    public decimal? BidPriceDistance { get; set; }
    //}

    //public partial class ResultAuctionDT
    //{
    //    public int? SessionAuctionId { get; set; }
    //    public string AuctionName { get; set; }
    //    public int? ProductId { get; set; }
    //    public string KoiName { get; set; }
    //    public string KoiImage { get; set; }
    //    public int? Typebid { get; set; }
    //    public List<TopCustomerAuctionDT> listTop { get; set; }
    //}

    //public partial class TopCustomerAuctionDT
    //{
    //    public Guid? AutionHistoryId { get; set; }
    //    public int? CustomerId { get; set; }
    //    public string CustomerName { get; set; }
    //    public string NickName { get; set; }
    //    public DateTime? Time { get; set; }
    //    public decimal? PriceBid { get; set; }
    //}

    //public partial class FollowKoiAutionDT
    //{
    //    public int SessionAutionId { get; set; }
    //    public double TimeEnd { get; set; }
    //    public List<ProductDT> ListProductKoi { get; set; }
    //}

    public partial class ProductAttribuiteDT
    {
        public int ProductAttributesId { get; set; }
        public int? ProductId { get; set; }
        public int? AttribuiteId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }

    public partial class ProductReviewDT
    {
        public int ProductReviewId { get; set; }
        public int? CustomerId { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string Contents { get; set; }
        public int? NumberStar { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public partial class MetaDataDT
    {
        public int? Sum { get; set; }
        public int? Normal { get; set; }
        public int? Temp { get; set; }
        public int? New { get; set; }
        public int? ReNew { get; set; }
        public int? Editing { get; set; }
        public int? Edited { get; set; }
        public int? ReEdited { get; set; }
        public int? Approving { get; set; }
        public int? NotApproved { get; set; }
        public int? Publishing { get; set; }
        public int? UnPublish { get; set; }
        public int? BaiViet { get; set; }
        public int? BienTap { get; set; }
        public int? KiemDuyet { get; set; }
        public int? NoPublic { get; set; }
        public int? Lock { get; set; }

        public int? AnPham { get; set; }

        public int? VanBan { get; set; }
    }

    public partial class LanguageCategoryDT
    {
        public LanguageDT lang { get; set; }
        public CategoryDTL category { get; set; }
    }

    public partial class CategoryDTL
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Url { get; set; }
        public int? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public partial class CommentDT
    {
        public int? CommentId { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public int? TargetId { get; set; }
        public byte? TargetType { get; set; }
        public string TargetName { get; set; }
        public string Contents { get; set; }
        public string Link { get; set; }
        public int? SumComment { get; set; }
        public int? SumLike { get; set; }
        public int? CommentParentId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public byte? Status { get; set; }
        public List<CommentDT> commentChild { get; set; }
        public string Name { get; set; }
        public string EmailComment { get; set; }
    }
    public partial class LegalDocDT
    {
        public int LegalDocId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime? DateIssue { get; set; }
        public DateTime? DateEffect { get; set; }
        public string Signer { get; set; }
        public byte? AgencyIssue { get; set; }
        public string AgencyIssueName { get; set; }
        public int? YearIssue { get; set; }
        public byte? TypeText { get; set; }
        public string TypeTextName { get; set; }
        public int? Field { get; set; }
        public string Note { get; set; }
        public string Attactment { get; set; }
        public string Contents { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
        public string TichYeu { get; set; }
        public string Url { get; set; }
        public int? AgencyIssued { get; set; }
    }
    public partial class PublicationDT
    {
        public int PublicationId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public int Author { get; set; }
        public DateTime? DateStartActive { get; set; }
        public DateTime? DateStartOn { get; set; }
        public DateTime? DateEndOn { get; set; }
        public int? ViewNumber { get; set; }
        public int? Location { get; set; }
        public bool? IsHome { get; set; }
        public bool? IsHot { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public int? LanguageId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public string NumberOfTopic { get; set; }
        public DateTime? PublishingYear { get; set; }
        public string Department { get; set; }
        public bool? IsLanguage { get; set; }
        public string TitleEn { get; set; }
        public string DescriptionEn { get; set; }
        public string ContentsEn { get; set; }
        public string DatePublic { get; set; }
    }

    public partial class LanguageDT
    {
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Flag { get; set; }
        public bool? IsMain { get; set; }
        public byte? Status { get; set; }
    }

    public partial class TranslateDT
    {
        public string text { get; set; }
        public string sl { get; set; }
        public string tl { get; set; }
    }

    public partial class FilterReport
    {
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int? Type { get; set; }
        public int? CategoryId { get; set; }
        public int? AuthorId { get; set; }
        public int? UserId { get; set; }
        public int? LanguageId { get; set; }
        public int? Status { get; set; }
        public int? TypeExport { get; set; }
        public bool? CashStatus { get; set; }
        public int? ApplicationRangeId { get; set; }
        public int? ResearchAreaId { get; set; }
        public int? UnitId { get; set; }
        public int? Extention { get; set; }
        public string query { get; set; }
        public int page { get; set; }
        public int page_size { get; set; }
        public string order_by { get; set; }
        public string select { get; set; }

    }

    public partial class WritetingMoney
    {
        public int NewsId { get; set; }
        public int? TypeNewsId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string ListCategory { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Cmtnd { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsCash { get; set; }
        public int? ViewNumber { get; set; }
        public int? Status { get; set; }
        public int? NumberWord { get; set; }
        public double? NumberPage { get; set; }
        public int? NumberImage { get; set; }
        public List<CateOfNews> listCategory { get; set; }

    }

    public partial class DataFile
    {
        public string LinkFile { get; set; }
        public byte[] DataByte { get; set; }
        public string Extension { get; set; }
    }

    public partial class DataSearch
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public DateTime? DateStartActive { get; set; }
        public string LinkVideo { get; set; }
        public int? Type { get; set; }
        public string Attactment { get; set; }
        public string TichYeu { get; set; }
        public DateTime? DateIssue { get; set; }
        public DateTime? DateEffect { get; set; }
    }

    public partial class HomeSearch
    {
        public int p { get; set; }
        public string textS { get; set; }
        public string tieuChi { get; set; }
        public int? departments { get; set; }
        public int? cateCq { get; set; }
        public string dateStart { get; set; }
        public string dateEnd { get; set; }

    }

    public partial class UnitDT
    {
        public int UnitId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public partial class CustomerDT
    {
        public long UserId { get; set; }
        public string FullName { get; set; }
        public string UnitName { get; set; }
        public string Email { get; set; }
    }

    public partial class CategoryAR
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public int? Location { get; set; }
        public long DataSetNumber { get; set; }
        public long? ViewNumber { get; set; }
        public long? DownNumber { get; set; }
    }

    public partial class ExtensionAR
    {
        public byte? Extension { get; set; }
        public string ExtensionName { get; set; }
        public string Name { get; set; }
        public long DataSetNumber { get; set; }
    }

    public partial class ChartLine
    {
        public List<string> labels { get; set; }
        public List<string> dataP { get; set; }
        public List<string> dataU { get; set; }
        public long max { get; set; }
        public int day { get; set; }
    }

    public partial class ChartCircle
    {
        public List<string> labels { get; set; }
        public List<string> datas { get; set; }
        public List<string> backgroundColors { get; set; }
        public int height { get; set; }
        public int day { get; set; }
    }

    public partial class TopUserHome
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Avata { get; set; }
        public long DataSetNumber { get; set; }
        public long DataSetId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }

    public partial class TopUnitHome
    {
        public int UnitId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public long DataSetNumber { get; set; }
    }

    public partial class CustomerSetting
    {
        public int CustomerId { get; set; }
        public bool? IsNotificationMail { get; set; }
        public bool? IsNotificationWeb { get; set; }
    }

    public partial class DataSetExcel : DataSetDTO
    {
        public string Error { get; set; }
        public string STT { get; set; }
        public string ListPvud { get; set; }
        public string ListLvnc { get; set; }
        public string ListAttactment { get; set; }
        public string ListUnit { get; set; }
        public string UserCreated { get; set; }
        public long? Storage { get; set; }
    }

    public partial class FolderFileCeph
    {
        public string STT { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string FileType { get; set; }
        public long? Storage { get; set; }
        public bool? IsView { get; set; }
        public List<AttactmentDTO>? listFiles { get; set; }
    }

    public partial class DataCeph
    {
        public string tieudebodulieu { get; set; }
        public string motanoidungbodulieu { get; set; }
        public List<string> phamviungdung { get; set; }
        public List<string> linhvucnghiencuu { get; set; }
        public string tentacgia { get; set; }
        public string diachiemail { get; set; }
        public string sdttacgia { get; set; }
        public string phienbantailieu { get; set; }
        public string nguon { get; set; }
    }

    public partial class DataCeph2
    {
        public string tieudebodulieu { get; set; }
        public List<string> phamviungdung { get; set; }
        public List<string> linhvucnghiencuu { get; set; }
        public string tentacgia { get; set; }
        public string diachiemail { get; set; }
        public string sdttacgia { get; set; }
        public string phienbantailieu { get; set; }
        public string nguon { get; set; }
    }

    public partial class DataCeph3
    {
        public string tieudebodulieu { get; set; }
        public string tentacgia { get; set; }
        public string diachiemail { get; set; }
        public string sdttacgia { get; set; }
        public string phienbantailieu { get; set; }
        public string nguon { get; set; }
    }


}