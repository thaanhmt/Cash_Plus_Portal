using IOITWebApp31.Models.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IOITWebApp31.Models.Data
{
    public class DTO
    {
    }

    public partial class UserLogin
    {
        public int? userId { get; set; }
        public int? userMapId { get; set; }
        public int? companyId { get; set; }
        public int? languageId { get; set; }
        public int? websiteId { get; set; }
        public string languageCode { get; set; }
        public string logoWebsite { get; set; }
        public string fullName { get; set; }
        public string avata { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public int? status { get; set; }
        public int? roleMax { get; set; }
        public int? roleLevel { get; set; }
        public bool isRoleGroup { get; set; }
        public string access_token { get; set; }
        public string access_key { get; set; }
        public string roleCode { get; set; }
        public string roleName { get; set; }
        public List<MenuDTO> listMenus { get; set; }
    }

    //public partial class UserDTO
    //{
    //    public Nullable<int> UserId { get; set; }
    //    public Nullable<int> BranchId { get; set; }
    //    public string Code { get; set; }
    //    public string FullName { get; set; }
    //    public string Password { get; set; }
    //    public string Address { get; set; }
    //    public string Phone { get; set; }
    //    public string Email { get; set; }
    //    public Nullable<int> Status { get; set; }
    //    public Nullable<int> GroupId { get; set; }
    //    public string Group { get; set; }
    //    public Nullable<DateTime> CreatedAt { get; set; }
    //    public Nullable<DateTime> UpdatedAt { get; set; }
    //    public Nullable<DateTime> TokenSince { get; set; }
    //    public string FacebookUserId { get; set; }
    //    public string RegEmail { get; set; }
    //}

    public partial class MenuDTO
    {
        public int MenuId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int MenuParent { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string ActiveKey { get; set; }
        public int? Status { get; set; }
        public List<MenuDTO> listMenus { get; set; }
    }

    public partial class FunctionDTO
    {
        public int FunctionId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Url { get; set; }
        public string Note { get; set; }
        public int FunctionParent { get; set; }
        public int? Location { get; set; }
        public string Icon { get; set; }
        public bool? IsMenu { get; set; }
        public int? Status { get; set; }
        public List<FunctionDTO> functionParent { get; set; }
    }

    public partial class FunctionDT
    {
        public int id { get; set; }
        public string label { get; set; }
        public string icon { get; set; }
        public int? location { get; set; }
        public bool? selected { get; set; }
        public bool? is_max { get; set; }
        public List<FunctionDT> children { get; set; }
    }

    public partial class FunctionRoleDTO
    {
        public int FunctionRoleId { get; set; }
        public int TargetId { get; set; }
        public int FunctionId { get; set; }
        public string ActiveKey { get; set; }
        public int? Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public int? Status { get; set; }
        public bool? Selected { get; set; }
        public List<FunctionRoleDTO> functionRole { get; set; }
    }

    public partial class FunctionRoleDT
    {
        public int FunctionRoleId { get; set; }
        public int TargetId { get; set; }
        public int FunctionId { get; set; }
        public string ActiveKey { get; set; }
    }

    public partial class UserRoleDT
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string UserName { get; set; }
        public string Avata { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public int? UnitId { get; set; }
        public int? CompanyId { get; set; }
        public int? PositionId { get; set; }
        public int? DepartmentId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int? UserCreateId { get; set; }
        public int? ProjectId { get; set; }
        public bool? IsRoleGroup { get; set; }
        public List<RoleDT> listRole { get; set; }
        public List<FunctionRoleDT> listFunction { get; set; }
        //public List<UserProjectDTO> listUnit { get; set; }
        //public List<UserProjectDTO> listProject { get; set; }
    }

    public partial class UserChangePass
    {
        public long UserId { get; set; }
        public string PasswordOld { get; set; }
        public string PasswordNew { get; set; }
        public string UserName { get; set; }
    }

    public partial class UpdateAvata
    {
        public long CustomerId { get; set; }
        public string Avata { get; set; }
    }

    public partial class RoleDT
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public partial class RoleDTO : Role
    {
        public List<FunctionRoleDT> listFunction { get; set; }
    }


    public partial class SmallFunctionDTO
    {
        public int FunctionId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? Level { get; set; }
    }

    public partial class SmallCategoryDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Genealogy { get; set; }    //Gia phả của danh mục này
        public int CategoryParentId { get; set; }
        public byte? Status { get; set; }
        public int? Level { get; set; }
        public int? Location { get; set; }
        public bool? Check { get; set; }
    }

    public partial class SmallUnitDTO
    {
        public int UnitId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Genealogy { get; set; }    //Gia phả của danh mục này
        public int UnitParentId { get; set; }
        public byte? Status { get; set; }
        public int? Level { get; set; }
        public int? Location { get; set; }
        public bool? Check { get; set; }
    }

    public partial class AttactmentDTO : Attactment
    {
        public string STT { get; set; }
    }

    public partial class BankDTO
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

    public partial class BlockDTO
    {
        public int BlockId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
        public string Icon { get; set; }
        public int? LanguageId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }

    public partial class CategoryDTO
    {
        public int CategoryId { get; set; }
        public int? CategoryRootId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? CategoryParentId { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string Icon { get; set; }
        public string IconFa { get; set; }

        public bool? IconText { get; set; }
        public int? Location { get; set; }
        public int? TypeCategoryId { get; set; }
        public int? LanguageId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public byte? Status { get; set; }
        public int? NumberDisplayMobile { get; set; }
        public int? TemplatePage { get; set; }
        public bool? IsComment { get; set; }
        public List<AttactmentDTO> listImage { get; set; }
    }

    public partial class CategoryMappingDTO
    {
        public int CategoryMappingId { get; set; }
        public int? CategoryId { get; set; }
        public int? TargetId { get; set; }
        public int? TargetType { get; set; }
        public int? Location { get; set; }
        public int? LanguageId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class CategoryRankDTO
    {
        public int CategoryRankId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? RankStart { get; set; }
        public int? RankEnd { get; set; }
        public int? TypeRankId { get; set; }
        public int? LanguageId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }

    public partial class CommentDTO
    {
        public int CommentId { get; set; }
        public int? CustomerId { get; set; }
        public int? TargetId { get; set; }
        public byte? TargetType { get; set; }
        public string Contents { get; set; }
        public int? CommentParentId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class CompanyDTO
    {
        public int CompanyId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Fax { get; set; }
        public string Representative { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }

    public partial class ConfigDTO
    {
        public int ConfigId { get; set; }
        public bool? IsLog { get; set; }
        public string EmailHost { get; set; }
        public string EmailSender { get; set; }
        public bool? EmailEnableSSL { get; set; }
        public string EmailUserName { get; set; }
        public string EmailDisplayName { get; set; }
        public string EmailPasswordHash { get; set; }
        public int? EmailPort { get; set; }
        public int? ConpanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class ConfigTableItemDTO
    {
        public int ConfigTableItemId { get; set; }
        public int? ConfigTableId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool? IsNull { get; set; }
        public int? RankMin { get; set; }
        public int? RankMax { get; set; }
        public string Note { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class ConfigTableDTO
    {
        public int ConfigTableId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public List<ConfigTableItemDTO> listConfigTableItem { get; set; }

    }

    public partial class ConfigThumbDTO
    {
        public int ConfigThumbId { get; set; }
        public int? Width { get; set; }
        public int? Hieght { get; set; }
        public byte? Type { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class ContactDTO
    {
        public int ContactId { get; set; }
        public int? CustomerId { get; set; }
        public string Title { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Note { get; set; }
        public string Contents { get; set; }
        public string Address { get; set; }
        public int? TypeContactId { get; set; }
        public int? TypeContact { get; set; }
        public int? CompanyId { get; set; }
        public int? BranchId { get; set; }
        public int? NewsId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
        public string Attactment { get; set; }
    }

    public partial class CustomerDTO : Customer
    {
        public string UnitName { get; set; }
        public string ConfirmPassword { get; set; }
        public bool IsUnit { get; set; }
        public List<CategoryDTL> listRA { get; set; }
        public List<UnitDT> listMU { get; set; }
        public List<int> ListRoles { get; set; }
        public List<int> ListResearchArea { get; set; }
        public List<int> ListUnitManager { get; set; }
    }

    public partial class DepartmentDTO
    {
        public int DepartmentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? CompanyId { get; set; }
        public int? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }


    public partial class LanguageDTO
    {
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Flag { get; set; }
        public bool? IsMain { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class ManufacturerDTO
    {
        public int ManufacturerId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string Url { get; set; }
        public int? Location { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class MenuItemDTO
    {
        public int MenuItemId { get; set; }
        public int? CategoryId { get; set; }
        public int? MenuId { get; set; }
        public int? MenuParentId { get; set; }
        public int? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }

    public partial class MenuOutDTO
    {
        public int MenuId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Note { get; set; }
        public int? LanguageId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class NewsDTO : News
    {
        public int? NewsRootId { get; set; }
        public string NameCategory { get; set; }
        public string LinkCategory { get; set; }
        public string Note { get; set; }
        public string AuthorName { get; set; }
        public string EditName { get; set; }
        public string ApproveName { get; set; }
        public string PublicName { get; set; }
        public List<CateOfNews> listCategory { get; set; }
        public List<TagOfNews> listTag { get; set; }
        public List<AttactmentDTO> listAttachment { get; set; }
        public List<RelatedDTO> listRelated { get; set; }
        public List<RelatedDTO> listProductRelated { get; set; }
        public List<LanguageMappingDTO> listLanguage { get; set; }
    }
    public partial class PublicationDTO : Publication
    {
        public int? PublicatioRootId { get; set; }
    }
    public partial class RatifyDTO
    {
        public int RatifyId { get; set; }
        public int? RatifyRootId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public string Author { get; set; }
        public DateTime? DateStartActive { get; set; }
        public DateTime? DateStartOn { get; set; }
        public DateTime? DateEndOn { get; set; }
        public int? ViewNumber { get; set; }
        public int? Location { get; set; }
        public bool? IsHome { get; set; }
        public bool? IsHot { get; set; }
        public int? TypeNewsId { get; set; }
        public int? YearTimeline { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public string Introduce { get; set; }
        public string SystemDiagram { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? LanguageId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        //public int? NumLang { get; set; }
        public string LinkVideo { get; set; }
        public List<CateOfNews> listCategory { get; set; }
        public List<TagOfNews> listTag { get; set; }
        public List<AttactmentDTO> listAttachment { get; set; }
        public List<RelatedDTO> listRelated { get; set; }
        public List<RelatedDTO> listProductRelated { get; set; }
        public List<LanguageMappingDTO> listLanguage { get; set; }
    }
    public partial class CateOfNews
    {
        public int? CategoryId { get; set; }
        public string Name { get; set; }
        public bool? Check { get; set; }
    }

    public partial class TagOfNews
    {
        public int? TagId { get; set; }
        public string Name { get; set; }
        public bool? Check { get; set; }
    }

    public partial class OrderItemDTO
    {
        public int? OrderItemId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string ProductUrl { get; set; }
        public int? Quantity { get; set; }
        public decimal? PriceOld { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceTax { get; set; }
        public decimal? PriceDiscount { get; set; }
        public decimal? PriceTotal { get; set; }
        public byte? Status { get; set; }
    }

    public partial class OrderDTO
    {
        public int OrderId { get; set; }
        public int? NumberOrder { get; set; }
        public int CustomerId { get; set; }
        public int? PaymentMethodId { get; set; }
        public string BillingAddress { get; set; }
        public int? ShippingMethodId { get; set; }
        public string ShippingAddress { get; set; }
        public int OrderStatusId { get; set; }
        public decimal? OrderTax { get; set; }
        public decimal? OrderDiscount { get; set; }
        public decimal? OrderTotal { get; set; }
        public string CustomerNote { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class PositionDTO
    {
        public int PositionId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? LevelId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class ProductAttributeDTO
    {
        public int? ProductAttributeId { get; set; }
        public int? ProductId { get; set; }
        public string Code { get; set; }
        public string Image { get; set; }
        public bool? IsDownload { get; set; }
        public bool? IsVirtual { get; set; }
        public bool? IsBranch { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceSpecial { get; set; }
        public DateTime? PriceSpecialStart { get; set; }
        public DateTime? PriceSpecialEnd { get; set; }
        public byte? BranchStatus { get; set; }
        public string Description { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public int? MinStock { get; set; }
        public int? MaxStock { get; set; }
        public int? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
        public List<AttributeMappingDTO> listAttribute { get; set; }
    }

    public partial class AttributeMappingDTO
    {
        public Guid AttributeMappingId { get; set; }
        public int? AttributeId { get; set; }
        public int? ProductAttributeId { get; set; }
        public int? AttributeValueId { get; set; }
        public bool? IsMain { get; set; }
        public bool? IsView { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }

    public partial class ProductCustomerDTO
    {
        public int ProductCustomerId { get; set; }
        public int? TargetId { get; set; }
        public byte? TargetType { get; set; }
        public int? CustomerId { get; set; }
        public int? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class ProductReviewDTO
    {
        public int? ProductReviewId { get; set; }
        public int? CustomerId { get; set; }
        public int? ProductId { get; set; }
        public string Contents { get; set; }
        public int? NumberStar { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

    }

    public partial class ProductDTO
    {
        public int ProductId { get; set; }
        public int? ProductRootId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public bool? IsHome { get; set; }
        public bool? IsHot { get; set; }
        public bool? IsSale { get; set; }
        public int? StockQuantity { get; set; }
        public decimal? PriceSale { get; set; }
        public decimal? PriceImport { get; set; }
        public decimal? PriceSpecial { get; set; }
        public decimal? PriceOther { get; set; }
        public int? Discount { get; set; }
        public string Image { get; set; }
        public byte? TypeProduct { get; set; }
        public string Url { get; set; }
        public DateTime? DateStartActive { get; set; }
        public DateTime? DateStartOn { get; set; }
        public DateTime? DateEndOn { get; set; }
        public string ProductAttributes { get; set; }
        public string ProductNote { get; set; }
        public string NoteTech { get; set; }
        public string NotePromotion { get; set; }
        public int? ViewNumber { get; set; }
        public int? LikeNumber { get; set; }
        public int? CommentNumber { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public int? TypeImagePromotionId { get; set; }
        public int? ManufacturerId { get; set; }
        public int? TrademarkId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? LanguageId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Introduce { get; set; }
        public string Feature { get; set; }
        public string Configuration { get; set; }
        public string OriginProduct { get; set; }
        public string GuaranteeProduct { get; set; }
        public byte? Status { get; set; }
        public List<CateOfNews> listCategory { get; set; }
        public List<TagOfNews> listTag { get; set; }
        public List<ImageProductDTO> listImage { get; set; }
        public List<ProductAttributeDTO> listProductAttribute { get; set; }
        public List<RelatedDTO> listRelated { get; set; }
        public List<LanguageMappingDTO> listLanguage { get; set; }
        public List<AttactmentDTO> listDocument { get; set; }
    }

    public partial class ImageProductDTO
    {
        public int? ProductImageId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool? IsImageMain { get; set; }
        public int? Location { get; set; }
        public byte? Status { get; set; }
    }

    public partial class RelatedDTO
    {
        public int RelatedId { get; set; }
        public int? TargetId { get; set; }
        public int? TargetRelatedId { get; set; }
        public byte? TargetType { get; set; }
        public int? Location { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class SlideDTO
    {
        public int SlideId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public int? TypeSlideId { get; set; }
        public bool? IsImageMain { get; set; }
        public int? Location { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }

    public partial class TagDTO
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int TargetId { get; set; }
        public byte? TargetType { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
        public int? CountTag { get; set; }
    }

    public partial class TypeAttributeItemDTO
    {
        public int? TypeAttributeItemId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int TypeAttributeId { get; set; }
        public int? Location { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
    }

    public partial class TypeAttributeDTO
    {
        public int TypeAttributeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Location { get; set; }
        public bool? IsUpdate { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsGroup { get; set; }
        public int? TypeAttribuiteParentId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public List<TypeAttributeItemDTO> listAttributeItem { get; set; }
        public List<Customer> listCustomer { get; set; }

    }

    public partial class UserMappingDTO
    {
        public int UserMappingId { get; set; }
        public int? UserId { get; set; }
        public int? TargetId { get; set; }
        public byte? TargetType { get; set; }
        public int? UserIdCreatedId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class UserRoleDTO
    {
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class UserDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public string Avata { get; set; }
        public int? UnitId { get; set; }
        public int? PositionId { get; set; }
        public int? DepartmentId { get; set; }
        public int? CompanyId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string KeyLock { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? TokenSince { get; set; }
        public string RegEmail { get; set; }
        public int? RoleMax { get; set; }
        public byte? RoleLevel { get; set; }
        public bool? IsRoleGroup { get; set; }
        public int? UserCreateId { get; set; }
        public int? UserEditId { get; set; }
        public byte? Status { get; set; }
    }

    public partial class WebsiteDTO
    {
        public int WebsiteId { get; set; }
        public string Name { get; set; }
        public int? LanguageId { get; set; }
        public int? CompanyId { get; set; }
        public int? WebsiteParentId { get; set; }
        public string LogoHeader { get; set; }
        public string LogoFooter { get; set; }
        public string Hotline { get; set; }
        public string Hotmail { get; set; }
        public string LinkGooglePlus { get; set; }
        public string LinkFacebookPage { get; set; }
        public string LinkTwitter { get; set; }
        public string LinkYoutube { get; set; }
        public string LinkInstagram { get; set; }
        public string LinkLinkedIn { get; set; }
        public string LinkOther1 { get; set; }
        public string LinkOther2 { get; set; }
        public string LinkOther3 { get; set; }
        public string GoogleAnalitics { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public byte? Status { get; set; }
    }

    public partial class MenuDT
    {
        public int MenuId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public int? LanguageId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? Status { get; set; }
        public List<MenuItemDT> listMenuItem { get; set; }
    }

    public partial class MenuItemDT
    {
        public int? MenuItemId { get; set; }
        public int? id { get; set; }
        public int? MenuId { get; set; }
        public int? MenuParentId { get; set; }
        public int? Status { get; set; }
        public List<MenuItemDT> children { get; set; }
    }

    public partial class MenuChildren
    {
        public int? MenuItemId { get; set; }
        public int? CategoryId { get; set; }
        public int? MenuId { get; set; }
        public int? MenuParentId { get; set; }
        public List<MenuChildren> children { get; set; }
    }

    public partial class CategoryMenu
    {
        public int CategoryId { get; set; }
        public int CategoryParentId { get; set; }
        public string Name { get; set; }
        public int MenuItemId { get; set; }
        public int? Location { get; set; }
        public bool? IsParent { get; set; }
        public List<CategoryMenu> Children { get; set; }
    }

    public partial class SessionAutionDTO
    {
        public int? SessionAutionId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Image { get; set; }
        public bool? IsHome { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public byte? Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public byte? Status { get; set; }
        public List<SessionProductDTO> ListSessionProduct { get; set; }
    }

    public partial class SessionProductDTO
    {
        public int? SessionProductId { get; set; }
        public int? SessionAutionId { get; set; }
        public int? ProductId { get; set; }
        public int? CustomerWinId { get; set; }
        public decimal? PriceStart { get; set; }
        public decimal? PriceWin { get; set; }
        public bool? IsHome { get; set; }
        public DateTime? DateTimeWin { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
        public int? TypeAuction { get; set; }
        public decimal? BidPriceDistance { get; set; }
    }

    public partial class AutionHistoryDTO
    {
        public int? AutionHistoryId { get; set; }
        public int? SessionAutionId { get; set; }
        public int? CustomerId { get; set; }
        public int? ProductId { get; set; }
        public decimal? PriceOld { get; set; }
        public decimal? PriceNew { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }

    public partial class KoiDTO
    {
        public int? ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? PriceStart { get; set; }
        public string Image { get; set; }
        public bool? Check { get; set; }
        public bool? IsHome { get; set; }
        public int? TypeAuction { get; set; }
        public decimal? BidPriceDistance { get; set; }
    }

    public partial class SessionAutionMVC
    {
        public int? SessionAutionId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public byte? Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public byte? Status { get; set; }
        public List<KoiAuction> ListSessionProduct { get; set; }
    }

    public partial class KoiAuction
    {
        public int ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? PriceStart { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public string ProductNote { get; set; }
        public string TrademarkName { get; set; }
        public string ManufacturerName { get; set; }
        public string ProductSex { get; set; }
        public int? ProductAge { get; set; }
        public int? Width { get; set; }
        public string LinkYoutube { get; set; }
        public string ImageLeft { get; set; }
        public string ImageRight { get; set; }
    }

    public partial class OrderWebDTO
    {
        public int OrderId { get; set; }
        public string Code { get; set; }
        public int? CustomerId { get; set; }
        public Guid? CustomerAddressId { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? PaymentStatusId { get; set; }
        public int? ShippingMethodId { get; set; }
        public int? ShippingStatusId { get; set; }
        public int? OrderStatusId { get; set; }
        public decimal? OrderTax { get; set; }
        public decimal? OrderDelivery { get; set; }
        public decimal? OrderDiscount { get; set; }
        public decimal? OrderPaid { get; set; }
        public decimal? OrderTotal { get; set; }
        public string CustomerNote { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        //Khách hàng
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PassHash { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public string Address { get; set; }
        //Thanh toán
        public string IpAdress { get; set; }
        public string Locale { get; set; }
        public string ReturnUrl { get; set; }
        public string CardList { get; set; }
        public string AgainLink { get; set; }
        public string HashKey { get; set; }
        public string PaymentHistoryId { get; set; }
        public string PaymentRequest { get; set; }
        //
        public string ProductName { get; set; }
        public CustomerAddressDTO customerAddress { get; set; }
        public List<OrderItemDTO> listOrderItem { get; set; }
    }

    public partial class BranchDTO
    {
        public int? BranchId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Contents { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
        public int? LanguageId { get; set; }
        public int? Location { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }

    }

    public partial class UnitDTO : Unit
    {


    }

    public partial class ProductOutput
    {
        public int ProductId { get; set; }
        public int SessionAutionId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public bool? IsHome { get; set; }
        public bool? IsHot { get; set; }
        public bool? IsSale { get; set; }
        public int? StockQuantity { get; set; }
        public decimal? PriceSale { get; set; }
        public decimal? PriceImport { get; set; }
        public decimal? PriceSpecial { get; set; }
        public decimal? PriceOther { get; set; }
        public int? Discount { get; set; }
        public string Image { get; set; }
        public string ImageLeft { get; set; }
        public string ImageRight { get; set; }
        public string LinkYoutube { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public byte? ProductSex { get; set; }
        public int? ProductAge { get; set; }
        public byte? TypeProduct { get; set; }
        public int? TypeBid { get; set; }
        public string TypeBidStr { get; set; }
        public string Url { get; set; }
        public DateTime? DateStartActive { get; set; }
        public DateTime? DateStartOn { get; set; }
        public DateTime? DateEndOn { get; set; }
        public string ProductAttributes { get; set; }
        public string ProductNote { get; set; }
        public string NoteTech { get; set; }
        public string NotePromotion { get; set; }
        public int? ViewNumber { get; set; }
        public int? LikeNumber { get; set; }
        public int? CommentNumber { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public int? TypeImagePromotionId { get; set; }
        public int? ManufacturerId { get; set; }
        public int? TrademarkId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public string Introduce { get; set; }
        public string Feature { get; set; }
        public string Configuration { get; set; }
        public string OriginProduct { get; set; }
        public string GuaranteeProduct { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public string ManufacturerName { get; set; }
        public string TrademarkName { get; set; }
        public int? IsAuction { get; set; }
        public ProductReviewDTO productReviews { get; set; }
        public List<AttactmentDTO> listAttackment { get; set; }
        public double? PointStar { get; set; }
        public double? PointStar5 { get; set; }
        public double? PointStar4 { get; set; }
        public double? PointStar3 { get; set; }
        public double? PointStar2 { get; set; }
        public double? PointStar1 { get; set; }
        public int? TotalReviews { get; set; }


    }

    public partial class GeneralProductReview
    {
        public int? TotalStar { get; set; }
        public List<GeneralProductReviewItem> ListGeneralProductReviewItem { get; set; }
        public List<ProductReview> ListProductReview { get; set; }
    }

    public partial class GeneralProductReviewItem
    {
        public int? TypeStar { get; set; }
        public int? CountReview { get; set; }
    }

    public partial class Producer
    {
        public int ManufacturerId { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
        public string Logo { get; set; }
    }

    public partial class HighlightNews
    {
        public int SessionAutionId { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public DateTime? DateStart { get; set; }
        public int? Location { get; set; }
    }

    public partial class NewsAndEventDTO
    {
        public int? NewsId { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DateStartActive { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? Type { get; set; }
        public int? Location { get; set; }
    }

    //DT để sắp sếp kéo thả
    public partial class CategorySort
    {
        public int CategoryId { get; set; }
        public int? LanguageId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? CategoryParentId { get; set; }
        public string CategoryParentName { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public string Descriptions { get; set; }
        public int? Location { get; set; }
        public int? Level { get; set; }
        public bool? IsShow { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<CategorySort> categorySorts { get; set; }
        public LanguageDT language { get; set; }
        public List<LanguageCategoryDT> listLanguage { get; set; }
    }

    //DT đăng ký nhận bản tin
    public partial class ReceiveNews
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Vui lòng nhập Email!")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ!")]
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Contents { get; set; }
        public string Address { get; set; }
        public int TypeContact { get; set; }
    }

    public partial class CustomerAddressDTO
    {
        public Guid CustomerAddressId { get; set; }
        public int? CustomerId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public int? DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public bool? IsMain { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    //DTO đổi mật khẩu của khách hàng
    public partial class ResetPasswordCustomerDTO
    {
        public string PasswordInit { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public partial class LanguageMappingDTO : LanguageMapping
    {

    }

    //Sắp xếp kéo thả có thêm tổng số bản ghi
    public partial class FullCategorySort
    {
        public int? Sum { get; set; }
        public int? SumOnline { get; set; }
        public int? SumOffline { get; set; }
        public List<CategorySort> categorySorts { get; set; }
    }

    public partial class LegalDocDTO : LegalDoc
    {
        public List<LanguageMappingDTO> listLanguage { get; set; }
        public List<CateOfNews> listCategory { get; set; }
    }

    public partial class AuthorDTO : Author
    {
    }

    public partial class FullUnitSort
    {
        public int? Sum { get; set; }
        public int? SumOnline { get; set; }
        public int? SumOffline { get; set; }
        public List<UnitSort> categorySorts { get; set; }
    }

    public partial class UnitSort
    {
        public int UnitId { get; set; }
        public int? LanguageId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int? UnitParentId { get; set; }
        public string UnitParentName { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public string Descriptions { get; set; }
        public int? Location { get; set; }
        public int? Level { get; set; }
        public bool? IsShow { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<UnitSort> categorySorts { get; set; }
        public LanguageDT language { get; set; }
        public List<LanguageCategoryDT> listLanguage { get; set; }
    }

    public partial class DataSetDTO : DataSet
    {
        public long? DataSetRootId { get; set; }
        public List<AttactmentDTO> listFiles { get; set; }
        public List<CategoryDTL> applicationRange { get; set; }
        public List<CategoryDTL> researchArea { get; set; }
        public List<UnitDT> unit { get; set; }
        public CustomerDT userCreated { get; set; }
        public CustomerDT userApproved { get; set; }
        public CustomerDT userPublished { get; set; }
        public NewsDTO licecses { get; set; }
        public List<LanguageMappingDTO> listLanguage { get; set; }
        public string AvataCustomer { get; set; }
        public int SumDataCustomer { get; set; }

        public FolderFileCeph folderFileCeph { get; set; }
    }

    public partial class DashboardDT
    {
        public long? DataSetNumber { get; set; }
        public long? ViewNumber { get; set; }
        public long? DownNumber { get; set; }
        public int? UserNumber { get; set; }

    }


}