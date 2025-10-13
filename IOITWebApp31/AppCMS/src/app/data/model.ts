export class Action {
  ActionId: number;
  ActionName: string;
  ActionType: string;
  TargetId: number;
  TargetType: string;
  Logs: string;
  CreatedAt: Date;
  IPAddress: string;
  Time: number;
  Type: number;
  CompanyId: number;
  UserPushId: number;
  UserId: number;
  Status: number;
}

export class Attactment {
  AttactmentId: number;
  Name: string;
  TargetId: number;
  TargetType: number;
  Url: string;
  Thumb: string;
  Note: string;
  Extension: number;
  ExtensionName: string;
  CreatedAt: Date;
  UserId: number;
  Status: number;
  IsImageMain: boolean;
}

export class Bank {
  BankId: number;
  Name: string;
  AccountId: string;
  AccountName: string;
  BranchName: string;
  Note: string;
  CompanyId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
}

export class Block {
  BlockId: number;
  Code: string;
  Name: string;
  Contents: string;
  Icon: string;
  LanguageId: number;
  WebsiteId: number;
  CompanyId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
  IconText: any;
}

export class Category {
  CategoryId: number;
  CategoryRootId: number;
  Name: string;
  Code: string;
  CategoryParentId: number;
  Description: string;
  Contents: string;
  Url: string;
  Image: string;
  Icon: string;
  IconFa: string;
  IconText: boolean;
  Location: number;
  IsSpecial: boolean;
  TypeCategoryId: number;
  LanguageId: number;
  LanguageRootId: number;
  LanguageCode: string;
  LanguageRootCode: string;
  WebsiteId: number;
  CompanyId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  MetaTitle: string;
  MetaKeyword: string;
  MetaDescription: string;
  Status: number;
  StatusView: boolean;
  NumberDisplayMobile: number;
  TemplatePage: number;
  IsComment: boolean;
  IsFirst: boolean;
  listImage: Array<Attactment>;
}

export class CategoryMapping {
  CategoryMappingId: number;
  CategoryId: number;
  TargetId: number;
  TargetType: number;
  Location: number;
  CreatedAt: Date;
  Status: number;
}

export class CategoryRank {
  CategoryRankId: number;
  Name: string;
  Description: string;
  RankStart: number;
  RankEnd: number;
  TypeRankId: number;
  LanguageId: number;
  WebsiteId: number;
  CompanyId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
}

export class Comment {
  CommentId: number;
  CustomerId: number;
  TargetId: number;
  TargetType: number;
  Contents: string;
  CommentParentId: number;
  CreatedAt: Date;
  UpdateAt: Date;
  Status: number;
}

export class Company {
  CompanyId: number;
  Code: string;
  Name: string;
  Email: string;
  Phone: string;
  Address: string;
  Fax: string;
  Representative: string;
  ContactName: string;
  ContactEmail: string;
  ContactPhone: string;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
}

export class Config {
  ConfigId: number;
  IsLog: number;
  EmailReceive: string;
  EmailHost: string;
  EmailSender: string;
  EmailEnableSsl: number;
  EmailUserName: string;
  EmailDisplayName: string;
  EmailPasswordHash: string;
  EmailColorBody: string;
  EmailColorHeader: string;
  EmailColorFooter: string;
  EmailLogo: string;
  Website: string;
  Phone: string;
  EmailPort: number;
  ConpanyId: number;
  CreatedAt: Date;
  Status: number;
  IsOnePay: any;
  HeaderScript: string;
  BodyScript: string;
  FooterScript: string;
  CustomCss: string;
  ModeSite: boolean;
}

export class ConfigTableItem {
  ConfigTableItemId: number;
  ConfigTableId: number;
  Code: string;
  Name: string;
  DataType: string;
  IsNull: number;
  RankMin: number;
  RankMax: number;
  Note: string;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
}

export class ConfigTable {
  ConfigTableId: number;
  Name: string;
  Code: string;
  CompanyId: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
  listConfigTableItem: Array<ConfigTableItem>;
}

export class ConfigThumb {
  ConfigThumbId: number;
  Name: string;
  Width: number;
  Height: number;
  Type: number;
  WebsiteId: number;
  CompanyId: number;
  UserId: number;
  CreatedAt: Date;
  Status: number;
}

export class Contact {
  ContactId: number;
  CustomerId: number;
  FullName: string;
  Title: string;
  Email: string;
  Phone: string;
  Address: string;
  Note: string;
  Contents: string;
  TypeContactId: number;
  CompanyId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
  TypeContact: any;
  Attactment: string;
}

export class Customer {
  CustomerId: number;
  Username: string;
  Password: string;
  ConfirmPassword: string;
  PasswordNew: string;
  FullName: string;
  Email: string;
  Phone: string;
  Avata: string;
  Sex: any;
  Birthday: any;
  Address: string;
  Note: string;
  KeyRandom: string;
  IsEmailConfirm: number;
  IsPhoneConfirm: number;
  Type: any;
  UnitId: number;
  CountryId: any;
  TypeId: number;
  IdNumber: string;
  DateNumber: any;
  AddressNumber: string;
  PositionId: string;
  AcademicRankId: string;
  DegreeId: string;
  WebsiteId: number;
  CompanyId: number;
  TypeThirdId: number;
  LastLoginAt: any;
  CreatedAt: any;
  UpdatedAt: any;
  Status: number;
  StatusView: boolean;
  RoleId: any;
  ListResearchArea: any;
  ListUnitManager: any;
  ListRoles: any;
  // haohv
  StudentCode: any;
  StudentClass: any;
  StudentYear: any;
  SchoolCode: any;
  AchievementNote: any;
  HobbyNote: any;
  PersonSummary: any;
  SchoolName: any;
  IsViewInfo: boolean;
  KeyToken: any;
  StepTwo : any;
  StepFour: any;
  StepFive: any;
  TopThree: any;
}

export class Department {
  DepartmentId: number;
  Code: string;
  Name: string;
  Phone: string;
  Email: string;
  CompanyId: number;
  Location: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
}

export class FunctionRole {
  FunctionRoleId: number;
  TargetId: number;
  FunctionId: number;
  ActiveKey: string;
  Type: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
}

export class Function {
  FunctionId: number;
  Name: string;
  Code: string;
  FunctionParentId: number;
  Url: string;
  Note: string;
  Location: number;
  Icon: string;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
}

export class Language {
  LanguageId: number;
  Name: string;
  Code: string;
  Flag: string;
  IsMain: number;
  CompanyId: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
}

export class Log {
  LogId: string;
  Action: string;
  Contents: string;
  UserId: number;
  CretatedAt: Date;
}

export class Manufacturer {
  ManufacturerId: number;
  Name: string;
  Code: string;
  TypeOriginId: number;
  Description: string;
  Logo: string;
  Address: string;
  Mobile: string;
  Phone: string;
  Fax: string;
  Website: string;
  Url: string;
  Location: number;
  MetaTitle: string;
  MetaDescription: string;
  MetaKeywords: string;
  CompanyId: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
  Owner: string;
  AvatarOwner: string;
  NickName: string;
  Country: string;
  Contents: string;
}

export class MenuItem {
  MenuItemId: number;
  CategoryId: number;
  MenuId: number;
  MenuParentId: number;
  Location: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
}

export class Menu {
  MenuId: number;
  Name: string;
  Code: string;
  Note: string;
  LanguageId: number;
  WebsiteId: number;
  CompanyId: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
}

export class News {
  NewsId: number;
  NewsRootId: number;
  Title: string;
  Description: string;
  Contents: string;
  Image: string;
  Url: string;
  DateStartActive: any;
  TimeStartActive: any;
  DateStartOn: any;
  DateEndOn: any;
  ViewNumber: number;
  Location: number;
  IsHome: boolean;
  IsHot: boolean;
  IsAttach: boolean;
  FactorPrice: number;
  ValuePrice: number;
  TotalPrice: number;
  TypeNewsId: number;
  YearTimeline: number;
  MetaTitle: string;
  MetaKeyword: string;
  MetaDescription: string;
  Introduce: string;
  SystemDiagram: string;
  WebsiteId: number;
  CompanyId: number;
  LanguageId: number;
  LanguageCode: number;
  LanguageRootId: number;
  LanguageRootCode: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
  StatusView: boolean;
  IsService: boolean;
  LinkVideo: string;
  VideoUrl: string;
  Author: string;
  AuthorId: number;
  listCategory: Array<{ CategoryId: number, Name: string, Check: boolean }>;
  listTag: Array<{ TagId: number, Name: string }>;
  listAttachment: Array<Attactment>;
  listRelated: Array<{ TargetRelatedId: number }>;
  listProductRelated: Array<{ TargetRelatedId: number }>;
  listLanguage: Array<Language>;
  AuthorName: string;
  Note: string;
  IsNote: boolean;
  IsComment: boolean;
  NumberPage: number;
  NumberImage: number;
  IsFirst: boolean;
  IsShowView: boolean;
}
export class Publication {
  PublicationId: number;
  Title: string;
  Description: string;
  Contents: string;
  Image: string;
  Url: string;
  DateStartActive: any;
  DateStartOn: any;
  DateEndOn: any;
  ViewNumber: number;
  Location: number;
  IsHome: boolean;
  IsHot: boolean;
  IsAttach: boolean;
  MetaTitle: string;
  MetaKeyword: string;
  MetaDescription: string;
  LanguageId: number;
  LanguageCode: number;
  LanguageRootId: number;
  LanguageRootCode: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
  Author: number;
  AuthorText: string;
  NumberOfTopic: number;
  NumberOfTopicText: string;
  PublishingYear: any;
  Department: number;
  DepartmentText: string;
  ActionId: number;
  ActionName: string;
  ActionType: number;
  TargetId: number;
  TargetType: string;
  Logs: string;
  Ipaddress: string;
  Time: number;
  Type: number;
  TargetName: string;
  CompanyId: number;
  UserPushId: number;
  AuthorName: string;
  IsLanguage: boolean;
  TitleEn: string;
  DescriptionEn: string;
  ContentsEn: string;
  DatePublic: string;
}
export class Upload {
  date: any;
  extension: string;
  name: string;
  size: any;
  type: number;
  url: string;
}
export class Author {
  AuthorId: number;
  Name: string;
  Avatar: string;
  UserMapId: number;
  Type: number;
  CreatedAt: any;
  UpdatedAt: any;
  UserId: number;
  Status: number;
  FullName: string;
  Address: string;
  Cccd: string;
  NumberPhone: string;
}
export class Backlink {
  BackLinkId: number;
  LinkIn: string;
  LinkOut: string;
  Note: string;
  CreatedAt: any;
  UpdatedAt: any;
  UserId: number;
  Status: number;
}


export class OrderItem {
  OrderItemId: number;
  OrderId: number;
  ProductId: number;
  Quantity: number;
  Price: number;
  PriceTax: number;
  PriceDiscount: number;
  PriceTotal: number;
  Status: number;
}

export class Order {
  OrderId: number;
  Code: number;
  CustomerId: number;
  PaymentStatusId: number;
  PaymentMethodId: number;
  BillingAddress: string;
  ShippingMethodId: number;
  ShippingAddress: string;
  OrderStatusId: number;
  OrderTax: number;
  OrderDiscount: number;
  OrderTotal: number;
  CustomerNote: string;
  WebsiteId: number;
  CompanyId: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
  listOrderItems: Array<OrderItem>;
  ReceiverEmail: string;
  ReceiverName: string;
  ReceiverPhone: string;
  customerName?: CustomerName;
  customerAddress?: CustomerAddress;
}

export class CustomerName {
  CustomerId: number;
  FullName: string;
  PhomeNumber: string;
}

export class CustomerAddress {
  CustomerId: number;
  Name: string;
  Phone: string;
  Address: string;
}

export class Position {
  PositionId: number;
  Name: string;
  Code: string;
  LevelId: number;
  CompanyId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
}

export class ProductAttribuite {
  ProductAttributesId: number;
  ProductId: number;
  Name: string;
  Value: string;
  Price: number;
  AttribuiteParentId: number;
  CreatedAt: Date;
  UserId: number;
  Status: number;
}

export class ProductCustomer {
  ProductCustomerId: number;
  TargetId: number;
  TargetType: number;
  CustomerId: number;
  Location: number;
  CreatedAt: Date;
  Status: number;
}

export class ProductReview {
  ProductReviewId: number;
  CustomerId: number;
  ProductId: number;
  Contents: string;
  NumberStar: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
}

export class Product {
  ProductId: number;
  ProductRootId: number;
  Code: string;
  Name: string;
  Description: string;
  Contents: string;
  IsHome: boolean;
  IsHot: boolean;
  IsSale: boolean;
  StockQuantity: number;
  PriceSale: number;
  Discount: number;
  PriceImport: number;
  PriceSpecial: number;
  PriceOther: number;
  PriceMin: number;
  PriceMax: number;
  Image: string;
  ImageLeft: string;
  ImageRight: string;
  TypeProduct: number;
  Width: number;
  Height: number;
  ProductAge: number;
  ProductSex: number;
  LinkYoutube: string;
  Url: string;
  DateStartActive: any;
  DateStartOn: any;
  DateEndOn: any;
  ProductAttributes: string;
  ProductNote: string;
  NoteTech: string;
  NotePromotion: string;
  ViewNumber: number;
  LikeNumber: number;
  CommentNumber: number;
  MetaTitle: string;
  MetaKeyword: string;
  MetaDescription: string;
  Introduce: string;
  Feature: string;
  Configuration: string;
  OriginProduct: string;
  GuaranteeProduct: string;
  TypeImagePromotionId: number;
  ManufacturerId: number;
  TrademarkId: number;
  LanguageId: number;
  LanguageRootId: number;
  LanguageCode: string;
  LanguageRootCode: string;
  WebsiteId: number;
  CompanyId: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
  listCategory: Array<{ CategoryId: number, Name: string, Check: boolean }>;
  listTag: Array<{ TagId: number, Name: string, Check: boolean }>;
  listAttribute: Array<Attribute>;
  listRelated: Array<{ TargetRelatedId: number }>;
  listImage: Array<ImageProduct>;
  listDocument: Array<Attactment>;
}

export class ImageProduct {
  ProductImageId: number;
  Name: string;
  Image: string;
  Location: number;
  IsImageMain: boolean;
  Status: number
}

export class Attribute {
  ProductAttributesId: number;
  AttribuiteId: number;
  Location: number;
  Name: string;
  Value: string;
  Status: number;
  Image: string;
}

export class Related {
  RelatedId: number;
  TargetId: number;
  TargetRelatedId: number;
  TargetType: number;
  Location: number;
  UserId: number;
  CreatedAt: Date;
  Status: number;
}

export class Role {
  RoleId: number;
  Code: string;
  Name: string;
  Note: string;
  Type: any;
  LevelRole: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  CreatedId: number;
  UpdatedId: number;

  Status: number;
  listFunction: Array<FuncRole>;
}

export class FuncRole {
  FunctionRoleId: number;
  TargetId: number;
  FunctionId: number;
  ActiveKey: string;
}

export class Slide {
  SlideId: number;
  Name: string;
  Title: string;
  Description: string;
  TargetId: number;
  Image: string;
  Url: string;
  UrlYoutube: string;
  TypeSlideId: number;
  IsImageMain: boolean;
  IsLinkNewTab: boolean;
  Location: number;
  LanguageId: number;
  WebsiteId: number;
  CompanyId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
  StatusView: boolean;
}

// export interface sysdiagram {
//     diagram_id: number;
//     name: string;
//     principal_id: number;
//     version: number;
//     definition: byte[];
// }

export class Tag {
  TagId: number;
  Name: string;
  Url: string;
  TargetId: number;
  TargetType: number;
  WebsiteId: number;
  CompanyId: number;
  UserId: number;
  CreatedAt: Date;
  Status: number;
}

export class TypeAttributeItem {
  TypeAttributeItemId: number;
  Code: string;
  Name: string;
  TypeAttributeId: number;
  Image: string;
  Location: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
  Descripttion: string;
}
export class customer {
  CustomerId: number;
  StudentCode: string;
  FullName: string;
  TypeAttributeId: number;
  Avata: string;
  SchoolCode: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
  Descripttion: string;
}


export class TypeAttribute {
  TypeAttributeId: number;
  Name: string;
  IsUpdate: boolean;
  IsDelete: boolean;
  IsGroup: number;
  TypeAttribuiteParentId: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
  Size: string;
  Location: string;
  Image: string;
  Description: string;
  listAttributeItem: Array<TypeAttributeItem>;
  listCustomer: Array<customer>;
}
export class TypeAttributeItemE {
  TypeAttributeItemId: number;
  Code: string;
  Name: string;
  TypeAttributeId: number;
  Location: number;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
}

export class UserMapping {
  UserMappingId: number;
  UserId: number;
  TargetId: number;
  TargetType: number;
  UserIdCreatedId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  Status: number;
}

export class UserRole {
  UserRoleId: number;
  UserId: number;
  RoleId: number;
  CreatedAt: Date;
  Status: number;
}

export class User {
  UserId: number;
  FullName: string;
  PasswordNew: string;
  UserName: string;
  Password: string;
  Email: string;
  Code: string;
  Avata: string;
  UnitId: number;
  PositionId: number;
  DepartmentId: number;
  CompanyId: number;
  Address: string;
  Phone: string;
  KeyLock: string;
  CreatedAt: Date;
  UpdatedAt: Date;
  TokenSince: Date;
  RegEmail: string;
  RoleMax: number;
  RoleLevel: number;
  IsRoleGroup: boolean;
  UserCreateId: number;
  UserEditId: number;
  Status: number;
}

export class Website {
  WebsiteId: number;
  Name: string;
  Url: string;
  LanguageId: number;
  CompanyId: number;

  WebsiteParentId: number;
  LogoHeader: string;
  LogoFooter: string;
  Banner: string;
  Hotline: string;
  Hotmail: string;
  Fax: string;
  OrganizationsUp: string;
  Organizations: string;
  UnitName: string;
  SystemName: string;
  Address: string;
  GoogleAnalitics: string;
  LinkMap: string;
  Link1: string;
  Link2: string;
  Link3: string;
  Link4: string;
  Link5: string;
  Link6: string;
  LinkOther1: string;
  LinkOther2: string;
  LinkOther3: string;
  Icon1: string;
  Icon2: string;
  Icon3: string;
  Icon4: string;
  Icon5: string;
  Icon6: string;
  IconBct: string;
  UserId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  MetaTitle: string;
  MetaKeyword: string;
  MetaDescription: string;
  Status: number;
  HighlightsNewsId: number;
  TechNiQuePhone: string;
  GuaRanTeePhone: string;
  AddressEn: string;
  Address2En: string;
  Title: string;
  Description: string;
}

export class UserChangePass {
  UserId: number;
  PasswordOld: string;
  PasswordOldE: string;
  PasswordNew: string;
  PasswordNewE: string;
  UserName: string;
  Avatar: string;
  FullName: string;
  ConfirmPassword: string;
}

export class Material {
  MaterialId: number;
  CategoryMaterialId: number;
  Levels: number;
  UnitCalculateId: number;
  StandardId: number;
  LocaltionProviderId: number;
  Location: number;
  UnitPrice: number;
  ProvinceId: number;
  Month: number;
  Year: number;
  Note: string;
  CreatedAt: Date;
  UpdatedAt: Date;
  UserId: number;
  Status: number;
  listMaterialImportExcelChild: Array<{
    STT: string,
    Levels: number
    Name: string
    UnitCalculate: string
    LocaltionName: string
    Standard: string,
    UnitPrice: number,
    Location: number
  }>;
}

export class SessionAution {
  SessionAutionId: number;
  Name: string;
  Code: string;
  Url: string;
  Image: string;
  IsHome: boolean;
  Description: string;
  Contents: string;
  DateStart: any;
  DateEnd: any;
  TimeStart: any;
  TimeEnd: any;
  Type: number;
  CreatedAt: string;
  UpdatedAt: string;
  UserId: number;
  MetaTitle: string;
  MetaKeyword: string;
  MetaDescription: string;
  Status: number;
  ListSessionProduct: Array<SessionPoduct>
}

export class SessionPoduct {
  SessionProductId: number;
  SessionAutionId: number;
  ProductId: number;
  CustomerWinId: number;
  PriceStart: number;
  DateTimeWin: number;
  IsHome: boolean;
  CreatedAt: string;
  UpdatedAt: string;
  UserId: number;
  Status: number;
  TypeAuction: number;
  BidPriceDistance: number;
}

export class AutionHistory {
  AutionHistoryId: number;
  SessionAutionId: number;
  CustomerId: number;
  ProductId: number;
  PriceOld: number;
  PriceNew: number;
  CreatedAt: string;
  Status: number;
}


export class Branch {
  BranchId: number;
  LanguageId: number;
  Code: string;
  Name: string;
  Avatar: string;
  Email: string;
  Phone: string;
  Address: string;
  Contents: string;
  CreatedAt: any;
  UpdatedAt: any;
  UserId: number;
  Status: number;
  Location: number;
  Lat: string;
  Long: string;
}

export class ResetPasswordCustomerDTO {
  FullName: string;
  PasswordInit: string;
  Password: string;
  ConfirmPassword: string;
  CustomerId: number;
}

export class Attribuite {
  AttributeId: number;
  Name: string;
  IsCustom: boolean;
  Location: number;
  CreatedAt: any;
  UpdatedAt: any;
  UserId: number;
  Status: number;
  AttributeParentId: number;
}

export class CommentPost {
  CommentId: number;
  CustomerId: number;
  TargetId: number;
  TargetType: number;
  Contents: string;
  CommentParentId: number;
  CreatedAt: Date;
  UpdateAt: Date;
  Status: number;
  commentChild: any;
  IsCheck: any;
  CustomerName: string;
  Name: string;
  EmailComment: string;
}

export class LegalDoc {
  LegalDocId: number;
  LegalDocRootId: number;
  Code: string;
  Name: string;
  Url: string;
  Signer: string;
  Attactment: string;
  AttactmentName: string;
  AttactmentBit: any;
  Contents: string;
  LanguageId: number;
  LanguageRootId: number;
  LanguageCode: string;
  LanguageRootCode: string;
  Note: string;
  TichYeu: string;
  CreatedAt: Date;
  UpdateAt: Date;
  Status: number;
  Field: number;
  TypeText: number;
  AgencyIssue: number;
  UserId: number;
  YearIssue: number;
  DateIssue: any;
  DateEffect: any;
  IsCheck: any;
  AgencyIssued: number;
  Extension: string;
  listCategory: Array<{ CategoryId: number, Name: string, Check: boolean }>;
}
export class ProductAttribuiteChild {
  ProductAttributeId: number;
  ProductId: number;
  Code: string;
  Image: string;
  IsDownload: boolean;
  IsVirtual: boolean;
  IsBranch: boolean;
  Price: number;
  PriceSpecial: number;
  PriceSpecialStart: any;
  PriceSpecialEnd: any;
  pBranchStatus: number;
  Description: string;
  Weight: number;
  Length: number;
  Width: number;
  Height: number;
  MinStock: number;
  MaxStock: number;
  Location: number;
  CreatedAt: any;
  UserId: number;
  Status: number;
}
export class AttributeMapping {
  AttributeMappingId: string;
  AttributeId: number;
  ProductAttributeId: number;
  AttributeValueId: number;
  IsMain: boolean;
  IsView: boolean;
  CreatedAt: any;
  UserId: number;
  Status: number;
}
export class Dictionary {
  DictionaryId: number;
  StringVn: string;
  StringEn: string;
  Note: string;
  CreatedAt: any;
  UpdateAt: any;
  Status: number;
}

export class NewsNote {
  NewsNoteId: any;
  NewsId: string;
  Note: string;
  UserId: number;
  CreatedAt: any;
  UpdateAt: any;
  Status: number;
}

export class Unit {
  UnitId: number;
  UnitRootId: number;
  Name: string;
  Code: string;
  ShortName: string;
  NameEn: string;
  Email: string;
  Phone: string;
  Fax: string;
  Website: string;
  IdNumber: string;
  DateNumber: any;
  AddressNumber: string;
  UnitParentId: number;
  Description: string;
  Contents: string;
  Url: string;
  Image: string;
  Icon: string;
  IconFa: string;
  IconText: boolean;
  Location: number;
  Type: number;
  ProvinceId: number;
  DistrictId: number;
  WardId: number;
  Address: string;
  AdminId: number;
  EmailAdmin: string;
  NameAdmin: string;
  LanguageId: number;
  LanguageRootId: number;
  LanguageCode: string;
  LanguageRootCode: string;
  WebsiteId: number;
  CompanyId: number;
  CreatedAt: Date;
  UpdatedAt: Date;
  CreatedId: number;
  UpdatedId: number;
  MetaTitle: string;
  MetaKeyword: string;
  MetaDescription: string;
  Status: number;
  StatusView: boolean;
  listImage: Array<Attactment>;
}

export class TypeSlide {
  TypeSlideId: any;
  Name: string;
  Code: string;
  Description: string;
  NumberImage: number;
  TimeReset: number;
  Width: number;
  Height: number;
  Location: number;
  LanguageId: number;
  WebsiteId: number;
  CompanyId: number;
  CreatedId: number;
  UpdatedId: number;
  CreatedAt: any;
  UpdateAt: any;
  Status: number;
  StatusView: boolean;
}

export class ConfigStar {
  ConfigStarId: number;
  FromView: number;
  ToView: number;
  Operator: number;
  FromDownload: number;
  ToDownload: number;
  LanguageId: number;
  WebsiteId: number;
  CompanyId: number;
  CreatedAt: any;
  UpdatedAt: any;
  UserId: number;
  Status: number;
}

export class DataSet {
  DataSetId: number;
  DataSetRootId: number;
  Title: string;
  Description: string;
  Contents: string;
  Image: string;
  Url: string;
  LinkVideo: string;
  AuthorName: string;
  AuthorEmail: string;
  AuthorPhone: string;
  Version: string;
  Note: string;
  DateStartActive: any;
  TimeStartActive: any;
  DateStartOn: any;
  DateEndOn: any;
  DownNumber: number;
  ViewNumber: number;
  Location: number;
  IsHot: boolean;
  Type: number;
  ApplicationRangeId: number;
  ResearchAreaId: number;
  UnitId: number;
  IsPublish: boolean;
  MetaTitle: string;
  MetaKeyword: string;
  MetaDescription: string;
  WebsiteId: number;
  CompanyId: number;
  LanguageId: number;
  LanguageCode: number;
  LanguageRootId: number;
  LanguageRootCode: number;
  UserCreatedId: number;
  CreatedAt: any;
  UpdatedAt: any;
  UserEditedId: number;
  EditedAt: any;
  ApprovedAt: any;
  UserApprovedId: number;
  UserId: number;
  Status: number;
  Confirms: string;
  ApplicationRangeName: string;
  ResearchAreaName: string;
  CreateName: string;
  UnitName: string;
  DepartmentName: string;
  ApprovedStatus: string;
  ApprovedName: string;
  ApprovedUnitName: string;
  applicationRange: Array<Category>;
  researchArea: Array<Category>;
  userCreated: CustomerDTL;
  listFiles: Array<Attactment>;
  listLanguage: Array<Language>;
}

export class DataSetApproved {
  DataSetApprovedId: any;
  DataSetId: number;
  Confirms: string;
  DataSetStatus: number;
  Type: number;
  CreatedAt: any;
  UpdatedAt: any;
  CreatedId: number;
  UpdatedId: number;
  Status: number;
}

export class CustomerDTL {
  FullName: string;
  UnitName: string;
}
