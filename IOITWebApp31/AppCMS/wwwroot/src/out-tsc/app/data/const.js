"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.listConfirmData = exports.listUserStatus = exports.listUserTypes = exports.listDataSetFiles = exports.listDataSetTypes = exports.listDataSetStatus = exports.listStatus = exports.listOperators = exports.listNumberImages = exports.listTypeId = exports.listSexs = exports.listItemMedia = exports.listTypeMedia = exports.listTemplate = exports.listTypeAuthor = exports.listCashNews = exports.listHotNews = exports.NewsStatus = exports.Status = exports.ActionTable = exports.PaymentOrderStatus = exports.ProductReviewStatus = exports.TypeUserSy = exports.TypeUser = exports.TypeContact = exports.OrderStatus = exports.TypeUpload = exports.typeProduct = exports.typeRank = exports.typeSlide = exports.Filter = exports.Paging = exports.Language = exports.typeCategoryPage = exports.typeCategoryNews = exports.navItems = exports.domainVideos = exports.domainFile = exports.domainMedia = exports.domainImageFile = exports.domainImage = exports.domainDebug = exports.domain = void 0;
/*export const domain = 'https://localhost:44393/';*/
exports.domain = '/';
exports.domainDebug = 'https://localhost:44393/';
exports.domainImage = exports.domain + 'uploads';
exports.domainImageFile = exports.domain + 'uploads/';
exports.domainMedia = exports.domain + 'uploads';
exports.domainFile = exports.domain + 'uploads/files';
exports.domainVideos = exports.domain + 'uploads/videos/';
exports.navItems = [
    {
        name: 'Trang tổng quan',
        url: '/dashboard',
        icon: 'icon-home'
    }
];
exports.typeCategoryNews = [
    {
        Id: 1,
        Name: 'Tin viết',
        ConstUrl: 'xem-truoc-tin-van-ban'
    },
    //{
    //  Id: 9,
    //  Name: 'Bài viết',
    //  ConstUrl: 'xem-truoc-tin-bai-viet'
    //},
    //{
    //	Id: 2,
    //	Name: 'Thông tin có sẵn',
    //	ConstUrl: 'chi-tiet-thong-bao'
    //},
    //{
    //	Id: 3,
    //	Name: 'Hình ảnh',
    //   ConstUrl: 'xem-truoc-tin-hinh-anh'
    //},
    //{
    //	Id: 4,
    //	Name: 'Video',
    //   ConstUrl: 'xem-truoc-tin-video'
    // },
    //{
    //  Id: 8,
    //  Name: 'Văn bản',
    //  ConstUrl: 'chi-tiet-giai-phap'
    //},
    {
        Id: 10,
        Name: 'Bản quyền',
        ConstUrl: 'chi-tiet-ban-quyen'
    },
    // {
    // 	Id: 5,
    // 	Name: 'Tin đính kèm'
    // },
    //{
    //  Id: 7,
    //  Name: 'Tin sự kiện',
    //  ConstUrl: 'xem-truoc-tin-su-kien'
    //},
    //{
    //  Id: 8,
    //  Name: 'Tin Emagazine',
    //  ConstUrl: 'xem-truoc-tin-emagazine'
    //},
];
exports.typeCategoryPage = [
    {
        Id: 6,
        Name: 'Trang bình thường',
        Group: 'Trang bình thường'
    },
    {
        Id: 7,
        Name: 'Dofollow',
        Group: 'Liên kết trên trang'
    },
    {
        Id: 8,
        Name: 'Dofollow',
        Group: 'Liên kết ngoài trang'
    },
    {
        Id: 9,
        Name: 'Nofollow',
        Group: 'Liên kết trên trang'
    },
    {
        Id: 10,
        Name: 'Nofollow',
        Group: 'Liên kết ngoài trang'
    }
];
exports.Language = [
    {
        Id: 1,
        Name: 'Tiếng Việt'
    },
    {
        Id: 7,
        Name: 'Tiếng Anh'
    }
];
exports.Paging = {
    page: 1,
    page_size: 10,
    query: '1=1',
    order_by: '',
    item_count: 0
};
exports.Filter = {
    txtSearch: ''
};
exports.typeSlide = [
    {
        Id: 1,
        Name: 'loại 1'
    },
    {
        Id: 2,
        Name: 'loại 2'
    },
    {
        Id: 3,
        Name: 'loại 3'
    },
    {
        Id: 4,
        Name: 'loại 4'
    }
];
exports.typeRank = [
    {
        Id: 1,
        Name: 'Diện tích'
    },
    {
        Id: 2,
        Name: 'Đơn giá'
    }
];
exports.typeProduct = [
    {
        Id: 1,
        Name: 'Sản phẩm đơn giản'
    },
    {
        Id: 2,
        Name: 'Sản phẩm có biến thể'
    }
];
exports.TypeUpload = [
    {
        Id: 1,
        Name: 'Tin tức'
    },
    {
        Id: 2,
        Name: 'Sản phẩm'
    },
    {
        Id: 3,
        Name: 'Slide'
    },
    {
        Id: 4,
        Name: 'Icon'
    },
    {
        Id: 5,
        Name: 'Danh mục'
    },
    {
        Id: 6,
        Name: 'Loại khác'
    }
];
exports.OrderStatus = [
    {
        Id: 1,
        Name: 'Khởi tạo',
        Class: 'badge badge-primary'
    },
    {
        Id: 2,
        Name: 'Đã xác nhận',
        Class: 'badge badge-info'
    },
    {
        Id: 3,
        Name: 'Đang giao hàng',
        Class: 'badge badge-light'
    },
    {
        Id: 4,
        Name: 'Đã giao hàng',
        Class: 'badge badge-warning'
    },
    {
        Id: 5,
        Name: 'Bị hủy',
        Class: 'badge badge-danger'
    }
];
exports.TypeContact = [
    {
        Id: 1,
        Name: 'Liên hệ'
    },
    //{
    //	Id: 2,
    //	Name: 'Đăng ký nhận tin'
    //},
    //{
    //	Id: 3,
    //	Name: 'Đặt lịch và báo giá'
    // },
    // {
    //   Id: 10,
    //   Name: 'Ứng tuyển'
    // },
    // {
    //   Id: 11,
    //   Name: 'Đăng ký tư vấn'
    // }
    {
        Id: 12,
        Name: 'Đặt ấn phẩm'
    },
    {
        Id: 13,
        Name: 'Form liên hệ'
    }
];
exports.TypeUser = [
    {
        Id: 1,
        Name: 'Xác nhận'
    },
    {
        Id: 2,
        Name: 'Chưa xác nhận'
    }
];
exports.TypeUserSy = [
    {
        Id: 1,
        Name: 'Hoạt động'
    },
    {
        Id: 98,
        Name: 'Khóa'
    }
];
exports.ProductReviewStatus = [
    {
        Id: 1,
        Name: 'Khởi tạo'
    },
    {
        Id: 2,
        Name: 'Đã được duyệt'
    },
    {
        Id: 3,
        Name: 'Không được duyệt'
    }
];
exports.PaymentOrderStatus = [
    {
        Id: 1,
        Name: 'Chưa thanh toán',
        Class: 'badge badge-primary'
    },
    {
        Id: 2,
        Name: 'Đã thanh toán hết',
        Class: 'badge badge-info'
    },
    {
        Id: 3,
        Name: 'Chưa thanh toán hết',
        Class: 'badge badge-light'
    },
    {
        Id: 4,
        Name: 'Không thanh toán',
        Class: 'badge badge-warning'
    },
    {
        Id: 5,
        Name: 'Thanh toán lỗi',
        Class: 'badge badge-danger'
    }
];
exports.ActionTable = [
    {
        Id: 1,
        Name: 'Xóa đã chọn'
    }
];
exports.Status = [
    {
        Id: 1,
        Name: 'Xuất bản'
    },
    {
        Id: 11,
        Name: 'Chờ duyệt'
    },
    {
        Id: 12,
        Name: 'Đã duyệt'
    },
    {
        Id: 13,
        Name: 'Bài Nháp'
    },
    {
        Id: 98,
        Name: 'Riêng tư'
    }
];
exports.NewsStatus = [
    {
        Id: 1,
        Name: 'Hiển thị'
    },
    {
        Id: 10,
        Name: 'Không hiển thị'
    },
];
exports.listHotNews = [
    {
        Id: true,
        Name: 'Tin Nóng'
    },
    {
        Id: false,
        Name: 'Tin Thường'
    },
];
exports.listCashNews = [
    {
        Id: 1,
        Name: 'Tất cả'
    },
    {
        Id: 2,
        Name: 'Chưa có nhuận bút'
    },
    {
        Id: 3,
        Name: 'Đã có nhuận bút'
    },
];
exports.listTypeAuthor = [
    {
        Id: 1,
        Name: 'Tác giả tin tức'
    },
    {
        Id: 2,
        Name: 'Tác giả ấn phẩm'
    }
];
exports.listTemplate = [
    {
        Id: 1,
        Name: 'Mặc định'
    },
    {
        Id: 2,
        Name: 'Sự kiện'
    },
    {
        Id: 3,
        Name: 'Form bạn đọc'
    }
];
exports.listTypeMedia = [
    {
        Id: 1,
        Name: 'Hình ảnh'
    },
    {
        Id: 2,
        Name: 'Audio'
    },
    {
        Id: 3,
        Name: 'Video'
    },
    {
        Id: 4,
        Name: 'Tài liệu'
    },
    {
        Id: 5,
        Name: 'Khác'
    }
];
exports.listItemMedia = [
    {
        Id: 1,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 2,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 3,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 4,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 5,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 6,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 7,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 8,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 9,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 10,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 11,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 12,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 13,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 14,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 15,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 16,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 17,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 18,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 19,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 20,
        Type: 1,
        Url: 'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
    },
    {
        Id: 21,
        Type: 1,
        Url: 'https://glorydemo.sunny-marketing.de/wp-content/uploads/2022/10/isocal-instagram.png'
    },
    {
        Id: 22,
        Type: 1,
        Url: 'https://glorydemo.sunny-marketing.de/wp-content/uploads/2022/10/unsplash_EqoCUzG9200.png'
    },
    {
        Id: 23,
        Type: 4,
        Url: 'https://stywin.com/wp-content/uploads/2022/11/file-sample_100kB_20221115173826196.doc'
    },
    {
        Id: 24,
        Type: 2,
        Url: 'https://stywin.com/wp-content/uploads/2022/11/file_example_MP3_700KB.mp3'
    },
    {
        Id: 25,
        Type: 3,
        Url: 'https://stywin.com/wp-content/uploads/2022/11/52541876_364343168896913_4143561948734771414_n.mp4'
    },
];
exports.listSexs = [
    {
        Id: 1,
        Name: 'Nam'
    },
    {
        Id: 2,
        Name: 'Nữ'
    },
    {
        Id: 3,
        Name: 'Khác'
    }
];
//Loại giấy tờ
exports.listTypeId = [
    {
        Id: 1,
        Name: 'Căn cước công dân'
    },
    {
        Id: 2,
        Name: 'Hộ chiếu'
    },
    {
        Id: 3,
        Name: 'Khác'
    }
];
//Loại giấy tờ
exports.listNumberImages = [
    {
        Id: 1,
        Name: '1 ảnh'
    },
    {
        Id: 3,
        Name: '3 ảnh'
    },
    {
        Id: 5,
        Name: '5 ảnh'
    },
    {
        Id: 6,
        Name: '6 ảnh'
    },
    {
        Id: 8,
        Name: '8 ảnh'
    },
    {
        Id: 10,
        Name: '10 ảnh'
    },
    {
        Id: 15,
        Name: '15 ảnh'
    },
    {
        Id: 20,
        Name: '20 ảnh'
    }
];
exports.listOperators = [
    {
        Id: 1,
        Name: 'AND'
    },
    {
        Id: 2,
        Name: 'OR'
    },
];
exports.listStatus = [
    {
        Id: 1,
        Name: 'Hiển thị'
    },
    {
        Id: 10,
        Name: 'Không hiển thị'
    },
];
exports.listDataSetStatus = [
    {
        Id: 10,
        Name: 'Mới'
    },
    {
        Id: 1,
        Name: 'Đã công khai'
    },
    {
        Id: 2,
        Name: 'Đã duyệt'
    },
    {
        Id: 3,
        Name: 'Chờ duyệt'
    },
    {
        Id: 4,
        Name: 'Không duyệt'
    },
];
exports.listDataSetTypes = [
    {
        Id: 1,
        Name: 'Dữ liệu tổ chức'
    },
    {
        Id: 2,
        Name: 'Dữ liệu cá nhân'
    }
];
exports.listDataSetFiles = [
    {
        Id: 1,
        Name: 'PDF'
    },
    {
        Id: 2,
        Name: 'CSV'
    },
    {
        Id: 3,
        Name: 'KML'
    },
    {
        Id: 4,
        Name: 'SHP'
    },
    {
        Id: 5,
        Name: 'API'
    },
];
exports.listUserTypes = [
    {
        Id: 1,
        Name: 'Tổ chức'
    },
    {
        Id: 2,
        Name: 'Cá nhân'
    }
];
exports.listUserStatus = [
    {
        Id: 10,
        Name: 'Đăng ký mới'
    },
    {
        Id: 1,
        Name: 'Hoạt động'
    },
    {
        Id: 98,
        Name: 'Khóa'
    }
];
exports.listConfirmData = [
    {
        Id: 1,
        Name: 'Duyệt'
    },
    {
        Id: 2,
        Name: 'Không duyệt'
    }
];
//# sourceMappingURL=const.js.map