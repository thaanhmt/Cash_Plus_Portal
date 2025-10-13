export class UserInfo {
    userId: number;
    access_token: string;
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

export class Paging {
    page: number;
    page_size: number;
    query: string;
    order_by: string;
    item_count: number;
    select: string;
}

export class QueryFilter {
  txtCode: string;
  txtSlug: string;
  txtSearch: string;
    Type: number;
    Title: number;
    // CountryId: number;
    TypeAttributeId: number;
    TypeFormId: number;
    TypeUsertId: number;
    UserId: number;
    ManufacturerId: number;
    TrademarkId: number;
    TypeOrderStatus: number;
    TypeContactId: any;
    CategoryId: any;
    TypePaymentOrderStatus: number;
    Status: number;
  LanguageId: number;
  AuthorId: number;
  Hot: number;
  IsHot: boolean;
  CashStatus: boolean;
  DateStart: any;
  DateEnd: any;
  TypeExport: any;
  ApplicationRangeId: number;
  ResearchAreaId: number;
  Extention: number;
  UnitId: number;
  RoleId: number;
  CustomerId: number;
  page: number;
  page_size: number;
  query: string;
  order_by: string;
  item_count: number;
  select: string;
  // haohv
  SchoolId: number;
  StudentCode: string;
}

export class CheckRole {
  View: boolean;
  Create: boolean;
  Update: boolean;
  Delete: boolean;
  Import: boolean;
  Export: boolean;
  Print: boolean;
}
