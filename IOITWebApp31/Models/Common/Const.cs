namespace IOITWebApp31.Models
{
    public class Const
    {


        public static readonly int COMPANYID = 1;
        public static readonly int WEBSITEID = 1;
        public static readonly int LANGUAGEID = 1;
        public static string ROOT_UPLOADS = "uploads/";
        public static string ROOT_IMAGES = "uploads/images";
        public static string ROOT_THUMBS = "uploads/thumbs";
        public static string ROOT_THUMBS_THUMB = "uploads/thumbs/_thumb";

        public static string CATEGORY_NEWS = "category-news";
        //public static string CATEGORY_GROUP_PRODUCT = "nhom-san-pham";
        public static string CATEGORY_IMAGE = "library-image";
        public static string CATEGORY_VIDEO = "library-video";
        public static string CATEGORY_ATTACTMENT = "category-attactment";
        public static string CATEGORY_NOTIFICATION = "category-notification";
        public static string CATEGORY_PRODUCT = "group-product";
        public static string CATEGORY_PRODUCT_CHILD = "category-product";
        public static string CATEGORY_LEGAL_DOC = "category-document";
        public static string CATEGORY_PARTNER = "partner";
        public static string PAGE_NOMAL = "page";
        public static string PAGE_NOMAL_ABOUT = "about-us";
        public static string PAGE_NOMAL_ABOUT_VN = "gioi-thieu";
        public static string PAGE_NOMAL_CONTACT = "contact-us";
        public static string PAGE_NOMAL_CONTACT_VN = "lien-he";
        public static string PAGE_ABOUT_HTML = "organizational-structure";
        public static string PAGE_ABOUT_HTML_VN = "co-cau-to-chuc";
        public static string PAGE_ALL_PRODUCT = "product";
        public static string PAGE_ALL_PRODUCT_VN = "san-pham";
        public static string PAGE_SOLUTION = "solution";
        public static string PAGE_SOLUTION_VN = "giai-phap";
        public static string PAGE_NOMAL_TIMELINE = "techpro-day";
        public static string PAGE_NOMAL_TIMELINE_VN = "hanh-trinh-phat-trien";
        public static string PAGE_NOMAL_FAQ = "faq";
        public static string PAGE_NOMAL_FAQ_VN = "hoi-dap";
        public static string PAGE_DATA = "du-lieu";


        public static string DETAIL_NEWS = "news-detail";
        public static string DETAIL_IMAGE = "library-image-detail";
        public static string DETAIL_VIDEO = "library-video-detail";
        public static string DETAIL_ATTACTMENT = "attactment-detail";
        public static string DETAIL_NOTIFICATION = "notification-detail";
        public static string DETAIL_PRODUCT = "product-detail";
        public static string DETAIL_PARTNER = "partner-detail";
        public static string DETAIL_AUCTION = "phien-dau-gia";

        public static string PAGE_AUCTION = "dau-gia";
        public static string PAGE_AUCTION_LIST = "danh-sach-koi-dau-gia";
        public static string PAGE_AUCTION_TOP = "top-tra-gia-cao-nhat";


        public static string TAG_NEWS = "tag-news";
        public static string TAG_PRODUCT = "tag-product";

        public static readonly string PRICE_CONTACT = "Giá: Liên hệ";

        public enum Status
        {
            NORMAL = 1, // Bài viết đã được xuất bản
            OK = 2,
            NOT_OK = 3,
            TEMP = 10,
            PENDING = 11, // Chờ duyệt
            RATIFY = 12, // Duyệt lần 1
            DRAFT = 13,   // Nháp
            LOCK = 98,   // Bài viết riêng tư
            DELETED = 99, // Bài viết trong thùng rác
        }

        public enum NewsStatus
        {
            NORMAL = 1, // Bài viết đã được xuất bản
            //OK = 2,
            //NOT_OK = 3,
            TEMP = 10, //Bài viết nháp
            NEW = 11, // Bài viết mới
            RE_NEW = 12, // Bài viết mới bị trả lại
            EDITING = 13,   // Chờ biên tập
            EDITED = 14,   // Đã biên tập
            RE_EDITED = 15,   // Biên tập lại
            APPROVING = 16,   // Chờ duyệt
            NOT_APPROVED = 17,   // Không duyệt
            PUBLISHING = 18,   // Chờ xuất bản
            UN_PUBLISH = 19,   // Gỡ xuất bản
            LOCK = 98,   // Bài viết riêng tư
            DELETED = 99, // Bài viết trong thùng rác
        }

        public enum Action
        {
            VIEW = 0,
            CREATE = 1,
            UPDATE = 2,
            DELETED = 3,
            IMPORT = 4,
            EXPORT = 5,
            PRINT = 6,
            EDIT_ANOTHER_USER = 7,
            MENU = 8
        }

        public enum TypeAttribute    // Thuốc tính loại hình
        {
            THTDA = 1, // Thuộc tính dự án
            LHDA = 2,   // Loại hình dự án
            LDA = 3, // Loại dự án
            TTDA = 4, // Tình trạng dự án

        }

        public enum TypeCategory    // loại danh mục
        {
            CATEGORY_NEWS_TEXT = 1, // Danh mục tin văn bản
            CATEGORY_NEWS_NOTIFICATION = 2, // danh mục tin thông báo
            CATEGORY_NEWS_IMAGE = 3, // danh mục tin hình ảnh
            CATEGORY_NEWS_VIDEO = 4, // danh mục tin video
            CATEGORY_NEWS_ATTACTMENT = 5, // danh mục tệp đính kèm
            //
            CATEGORY_PAGE_NORMAL = 6,    // danh mục trang bình thường
            CATEGORY_PAGE_LINK_DO_NORMAL = 7,    // danh mục trang liên kết dofollow on tab
            CATEGORY_PAGE_LINK_DO_BLALK = 8,    // danh mục trang liên kết dofollow new tab
            CATEGORY_PAGE_LINK_NO_NORMAL = 9,    // danh mục trang liên kết nofollow on tab
            CATEGORY_PAGE_LINK_NO_BLALK = 10,    // danh mục trang liên kết nofollow new tab
            //
            CATEGORY_PRODUCT = 11,   // danh mục sản phẩm
            //
            CATEGORY_LEGAL_DOC = 12, // danh mục văn bản pháp quy
            //
            CATEGORY_RESEARCH_AREA = 14, // lĩnh vực nghiên cứu
            CATEGORY_APPLICATION_RANGE = 15, // phạm vi ứng dụng
        }

        public enum TypeNews    // loại tin tức
        {
            NEWS_TEXT = 1, // tin văn bản
            NEWS_NOTIFICATION = 2,    // tin thông báo
            NEWS_IMAGE = 3,   // tin hình ảnh
            NEWS_VIDEO = 4, // tin video
            NEWS_ATTACTMENT = 5, // tin tệp đính kèm
            NEWS_EVENT = 7, // tin sự kiện
            NEWS_NEWS = 9, // tin bài viết
            NEWS_LICENSE = 10 // tin bài viết
        }

        public enum TypeFunction    // Phân quyền chức năng với người dùng và nhóm quyền
        {
            FUNCTION_USER = 1, // Người dùng - Chức năng
            FUNCTION_ROLE = 2,    // Nhóm quyền - Chức năng
        }

        public enum TypeFolder    // loại folder
        {
            CATEDOGY = 1, // danh mục + trang
            NEWS = 2,    // tin tức
            PRODUCT = 3,   // sản phẩm
            SLIDE = 3,   // trình chiếu
        }

        public enum TypeThumb    // loại thumb
        {
            CATEDOGY = 5, // danh mục + trang
            NEWS = 1,    // tin tức
            PRODUCT = 2,   // sản phẩm
            SLIDE = 3,   // slide
            ICON = 4,   // icon
            OTHER = 6,   // icon
        }

        public enum TypeAction    // hành động
        {
            ACTION = 1, // Hành động
            WARNING = 2,    // Cảnh báo
            NOTIFICATION = 3,    // Thông báo
            AUTION = 4, // Đấu giá
        }

        public enum ActionType    // hành động
        {
            VIEW = 0, // Xem
            CREATE = 1, // Thêm
            UPDATE = 2,    // Sửa
            DELETE = 3,    // Xóa
            WARNING = 4,    // Cảnh báo
            IMPORT = 5,
            EXPORT = 6,
            PRINT = 7,
            EDIT_ANOTHER_USER = 8,
            LOGIN = 9, // ĐĂNG NHẬP
        }

        public enum TypeFile    // loại file
        {
            DOCUMENTS = 1, // file văn bản
            VIDEO = 2,    // file video
            AUDIO = 3,    // file âm thanh
            ELECTRONIC_BOOKS = 4,    // file sách điện tử
            IMAGES = 5,    // file hình ảnh
            ARCHIVES = 6,    // file nén
        }

        public enum PaymentStatus    // trang thái thanh toán
        {
            INIT = 1, // chưa thanh toán
            FULL = 2,    // đã thanh toán hết
            NOT_ENOUGH = 3,    // chưa thanh toán hết
            NOT_PAYMENT = 4,     // không thanh toán
            ERROR_PAYMENT = 5     // thanh toán lỗi
        }

        public enum PaymentMethod    // trang thái thanh toán
        {
            NOTPAY = 108, // ko thanh toán
            COD = 100, // ko thanh toán
            VIETELPAY = 99, // ko thanh toán
            PAYPAL = 91,    // paypal
            WECHATPAY = 90,    // wechat pay
            ALIPAY = 89,     // alipay
            ONEPAY_OUT = 88,     // onepay
            ONEPAY_IN = 87,     // onepay
            MOMO = 86,     // momo
        }

        public enum ShippingStatus // trang thái giao hàng
        {
            NOT_SHIP = 0, //chon van chuyen bang nha cung cap nhung chua tao shiporder
            INIT = 1, // da khoi tao thanh cong nhung chua tiep nhan
            DELIVERING = 4, //2 Da lay hang va dang van chuyen 
            ORDER_RETURN = 3,// khong giao duoc hang
            CANCELED = 5,//4 Huy giao hang 
            COMPLETED = 2,//5 Da hoan thanh
            LOST = 6, // khong lay duoc hang
            DELIVERER = 7, // da tiep nhan , dang dieu phoi lay hang
            DELETE = 99
        }

        public enum ShippingMethod // trang thái giao hàng
        {
            SHOP_SHIP = 1,
            GHTK = 2,
            GHN = 3,
            GHVT = 4,
            GHVN = 5,
            OTHER = 88,
        }

        public enum AcceptCash    // trang thái duyệt phiếu
        {
            INIT = 1, // chưa duyệt
            ACCEPT = 2,    // đã duyệt
            NOT_ACCEPT = 3,    // không duyệt
        }

        public enum ContractStatus    // trang thái duyệt phiếu
        {
            INIT = 49, // Chuẩn bị triển khai
            DEPLOYING = 50,    // Đang triển khai
            ACCEPTED = 51,    // Đã nghiệm thu
            PAYMENTED = 52,    // Đã quyết toán
            PAUSE = 53,    // Tạm dừng
        }

        public enum TypePlatform    // loại nền tảng
        {
            WEB = 1,    // 
            ANDROID = 2, // 
            IOS = 23, // 
        }

        public enum RoleLevel  //Loại quyền
        {
            ADMIN = 1,   //admin (chỉ có 1 tài khoản duy nhất và ng cài đặt web lắm giữ)
            MANAGER = 3,   //manager quản trị trang web
            USER = 2,   //user sử dụng trang web
        }

        public enum TypeTag  //Loại tag
        {
            TAG_NEWS = 1,   //
            TAG_PRODUCT = 2,   //
        }

        public enum TypeCategoryMapping  //Loại map nội dung vs danh mục
        {
            CATEGORY_NEWS = 1,   //
            CATEGORY_PRODUCT = 2,   //
            CATEGORY_LEGAL_DOC = 3
        }

        public enum TypeLanguageMapping  //Loại map ngôn ngữ
        {
            LANGUAGE_CATEGORY = 1,
            LANGUAGE_NEWS = 2,   //
            LANGUAGE_PRODUCT = 3,   //
            LANGUAGE_LEGALDOC = 4,   //
            LANGUAGE_DATASET = 5,   //
        }

        public enum TypeRelated  //Loại liên quan
        {
            PRODUCT_PRODUCT = 1,   //
            PRODUCT_NEWS = 2,   //
            NEWS_NEWS = 3,   //
            NEWS_PRODUCT = 4,   //
        }

        public enum TypeSlide  //Loại slide
        {
            SLIDE_HOME = 1,   //
            SLIDE_PRODUCT = 2,   //
            SLIDE_PATNER = 3,   //
            SLIDE_ADS = 4,   //
        }

        public enum TypeOrigin  //Loại bảng Manufacture
        {
            MANUFACTURER = 1,   // Nhà sản xuất(đối tác)
            TRADEMARK = 2,   // Loại cái Koi
            PARTNER = 3     // Trại cá
        }

        public enum TypeUpload  //Loại upload image
        {
            UPLOAD_IMAGE_NEWS = 1,   //
            UPLOAD_IMAGE_PRODUCT = 2,   //
            UPLOAD_IMAGE_SLIDE = 3,   //
            UPLOAD_IMAGE_ICON = 4,   //
            UPLOAD_IMAGE_CATEGORY = 5,   //
            UPLOAD_IMAGE_OTHER = 6,   //
        }

        public enum TypeProduct  //Loại sản phẩm
        {
            NORMAL = 1,   //Sản phẩm bình thường
            KOI = 2   //Cá Koi
        }

        public enum TypeAuction
        {
            AUCTION_SILENT = 1,
            AUCTION_BT = 2,
            AUCTION_ONLINE = 3
        }

        public enum TypeAttachment
        {
            NEWS_IMAGE = 1,
            NEWS_VIDEO = 2,
            FILE_DOC = 3,
            CATEGORY_IMAGE = 4,
            FILE_DATASET = 5,
        }

        public enum OrderStatus
        {
            INIT = 1,   // Trạng thái khởi tạo / chờ xác nhận đơn
            CONFIRM = 2,    //Đơn hàng đã được xác nhận
            DELIVERY = 3,   // Trạng thái đang giao hàng
            DELIVED = 4,    // Trạng thái đã giao hàng
            ORDER_RETURNED = 5  // Đơn hàng bị trả lại - Trạng thái HỦY
        }

        public enum TypeProductCustomer
        {
            FOLLOW = 1,   // Theo dõi
            LOVE = 2    // Thích
        }

        public enum TypeDevice
        {
            DESKTOP = 0,    // Máy tính
            MOBILE = 2   // Di động
        }

        public enum TypeOrderBy //Các loại sắp xếp
        {
            PRODUCT_IS_HOME = 10,    //Sắp xếp sản phẩm hiển thị ra trang chủ
            NEWS_IS_HOME = 11,   //Sắp xếp hiển thị tin tức ra trang chủ
            SESSION_AUCTION_IS_HOME = 12    //Sắp xếp tin đấu giá ra trang chủ
        }

        public enum TypeOrderByCategoryProduct //Các loại sắp xếp trong trang danh mục sản phẩm
        {
            DEFAULT = -1,    // Mặc định
            A_Z = 1,   // A=>Z
            Z_A = 2,    // Z=>A
            PRICE_INCREASE = 3,      // Giá từ thấp tới cao
            PRICE_REDUCTION = 4     // Giá từ cao tới thấp
        }

        public enum TypeRank
        {
            S = 1,  // Diện tích
            K_G = 2 // Khoảng giá
        }

        public enum TypeComment  //Loại comment
        {
            COMMENT_NEWS = 1,   // Bình luận tin tức
            COMMENT_PRODUCT = 2,   // Bình luận sản phẩm
        }

        public enum TypePermaLink  //Loại link
        {

            PERMALINK_CATEGORY_PAGE_NOMAL = 6,   // link danh mục trang
            PERMALINK_CATEGORY_PAGE_LINK_DO_NORMAL = 7,   // link danh mục trang
            PERMALINK_CATEGORY_PAGE_LINK_DO_BLALK = 8,   // link danh mục trang
            PERMALINK_CATEGORY_PAGE_LINK_NO_NORMAL = 9,   // link danh mục trang
            PERMALINK_CATEGORY_PAGE_LINK_NO_BLALK = 10,   // link danh mục trang
            PERMALINK_CATEGORY_NEWS_TEXT = 1,   // link danh mục tin tức
            PERMALINK_CATEGORY_NEWS_NOTIFICATION = 2,   // link danh mục tin tức
            PERMALINK_CATEGORY_NEWS_IMAGE = 3,   // link danh mục tin tức
            PERMALINK_CATEGORY_NEWS_VIDEO = 4,   // link danh mục tin tức
            PERMALINK_CATEGORY_NEWS_ATTACTMENT = 5,   // link danh mục tin tức
            PERMALINK_CATEGORY_PRODUCT = 11,   // link danh mục sản phẩm
            PERMALINK_CATEGORY_LEGAL_DOC = 12,   // link danh mục sản phẩm
            PERMALINK_DETAI_NEWS = 13,   // link chi tiết tin tức
            PERMALINK_DETAI_PRODUCT = 14,   // link chi tiết sản phẩm
            PERMALINK_TAG_NEWS = 15,   // link chi tiết tin tức
            PERMALINK_TAG_PRODUCT = 16,   // link chi tiết sản phẩm
            PERMALINK_APPLICATION_RANGE = 17,   // link phạm vi ứng dụng
            PERMALINK_RESEARCH_AREA = 18,   // link lĩnh vực nghiên cứu
            PERMALINK_DETAIL_DATASET = 19,   // link chi tiết dataset
        }

        public enum TypeAuthor  //Loại tác giả
        {
            AUTHOR_NEWS = 1,   // tác giả tin tức
            AUTHOR_PRODUCTION = 2,   // tác giả ấn phẩm
        }

        public enum TypeRole    // Loại nhóm quyền
        {
            ROLE_UNIT = 1, // Tổ chức
            ROLE_SYSTEM = 2,    // Hệ thống
            ROLE_PERSONAL = 3,    // Cá nhân
        }

        public enum TargetCustomerMapping    //mapping dữ liệu vs người dùng
        {
            CUSTOMER_ROLE = 1, // map quyền
            CUSTOMER_APPLICATION = 2,    // map lĩnh vực nghiên cứu
            CUSTOMER_UNIT = 3,    // map cơ quan quản trị
        }

        public enum TypeThird    // Loại tạo tk
        {
            CUSTOMER_ADMIN = 1, // Admin tạo
            CUSTOMER_USER = 2,    // Người dùng tự đăng ký
            CUSTOMER_KEYLOCK = 3,    // Login qua Keylook
        }

        public enum TypeCustomer    // Loại tạo tk
        {
            CUSTOMER_UNIT = 1, // Tổ chức
            CUSTOMER_PERSONAL = 2,    //Cá nhân
        }

        public enum DataSetStatus    // Trạng thái
        {
            TEMP = 10, // Mới
            PENDING = 3,    //Chở duyệt
            APPROVED = 2,    //Đã duyệt nội bộ
            NORMAL = 1,   //Đã duyệt công khai
            NOT_APPROVED = 4,   //Không duyệt nội bộ
            NOT_APPROVED_PUBLISH = 5,   //Không duyệt công khai
        }

        public enum DataSetType   // Loại dữ liệu
        {
            DATA_UNIT = 1, // Tổ chức
            DATA_PERSONAL = 2,    //Cá nhân
        }

        public enum DataSetTypeFile   // Loại dữ liệu
        {
            FILE_PDF = 1, // pdf
            FILE_CSV = 2, // csv
            FILE_KML = 3, //kml
            FILE_SHP = 4, //shp
            FILE_API = 5, //api
        }

        public enum DataSetMapping  // Loại dữ liệu
        {
            DATA_UNIT = 1, // Tổ chức
            DATA_APPLICATION_RANGE = 2,    //Pv ứng dụng
            DATA_RESEARCH_AREA = 3,    //LV nghiên cứu
        }

        public enum DataSetConfirmStatus    // Trạng thái
        {
            APPROVED = 1,    //Duyệt
            NOT_APPROVED = 2,   //Không duyệt
        }

        public enum DataSetConfirmType    // Loại duyệt
        {
            CONFIRM_PRIVATE = 1,    //Duyệt nội bộ
            CONFIRM_PUBLISH = 2,   //Duyệt công khai
        }

        public enum OperatorType    // Loại duyệt
        {
            AND = 1,    //và
            OR = 2,   //hoăch
        }

        public enum NotificationTargetType    // Loại thông báo
        {
            CUSTOMER = 1,
            DATASET = 2,
        }

    }
}