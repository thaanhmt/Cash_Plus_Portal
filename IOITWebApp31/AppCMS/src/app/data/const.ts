
/*export const domain = 'https://localhost:44393/';*/
export const domain = '/';
export const domainDebug = 'https://localhost:44393/';
export const domainImage = domain+ 'uploads';
export const domainImageFile = domain+ 'uploads/';
export const domainMedia = domain + 'uploads';
export const domainFile = domain + 'uploads/files';
export const domainVideos = domain + 'uploads/videos/';

export const navItems = [
	{
		name: 'Trang tổng quan',
		url: '/dashboard',
		icon: 'icon-home'
	}
];

export const typeCategoryNews = [
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
  
]

export const typeCategoryPage = [
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

export const Language = [
	{
		Id: 1,
		Name: 'Tiếng Việt'
	},
	{
		Id: 7,
		Name: 'Tiếng Anh'
	}
];

export const Paging = {
	page: 1,
	page_size: 10,
	query: '1=1',
	order_by: '',
	item_count: 0
};

export const Filter = {
	txtSearch: ''
}

export const typeSlide = [
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
]

export const typeRank = [
	{
		Id: 1,
		Name: 'Diện tích'
	},
	{
		Id: 2,
		Name: 'Đơn giá'
	}
]

export const typeProduct = [
  {
    Id: 1,
    Name: 'Sản phẩm đơn giản'
  },
  {
    Id: 2,
    Name: 'Sản phẩm có biến thể'
  }
]

export const TypeUpload = [
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
]

export const OrderStatus = [
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
]

export const TypeContact = [
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
]

export const TypeUser = [
  {
    Id: 1,
    Name: 'Xác nhận'
  },
  {
    Id: 2,
    Name: 'Chưa xác nhận'
  }
]

export const TypeUserSy = [
  {
    Id: 1,
    Name: 'Hoạt động'
  },
  {
    Id: 98,
    Name: 'Khóa'
  }
]


export const ProductReviewStatus = [
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
]

export const PaymentOrderStatus = [
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
]

export const ActionTable = [
	{
		Id: 1,
		Name: 'Xóa đã chọn'
	}
]

export const Status = [
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
]

export const NewsStatus = [
  {
    Id: 1,
    Name: 'Hiển thị'
  },
  {
    Id: 10,
    Name: 'Không hiển thị'
  },
  
]

export const listHotNews = [
  {
    Id: true,
    Name: 'Tin Nóng'
  },
  {
    Id: false,
    Name: 'Tin Thường'
  },
]

export const listCashNews = [
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
]

export const listTypeAuthor = [
  {
    Id: 1,
    Name: 'Tác giả tin tức'
  },
  {
    Id: 2,
    Name: 'Tác giả ấn phẩm'
  }
]
export const listTemplate = [
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
]
export const listTypeMedia = [
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
]

export const listItemMedia = [
  {
    Id: 1,
    Type: 1,
    Url:'https://stywin.com/wp-content/uploads/2022/08/TMS-01-1024x659.png'
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
]

export const listSexs = [
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
]

//Loại giấy tờ
export const listTypeId = [
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
]

//Loại giấy tờ
export const listNumberImages = [
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
]

export const listOperators = [
  {
    Id: 1,
    Name: 'AND'
  },
  {
    Id: 2,
    Name: 'OR'
  },
]

export const listStatus = [
  {
    Id: 1,
    Name: 'Hiển thị'
  },
  {
    Id: 10,
    Name: 'Không hiển thị'
  },
]

export const listDataSetStatus = [
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
]

export const listDataSetTypes = [
  {
    Id: 1,
    Name: 'Dữ liệu tổ chức'
  },
  {
    Id: 2,
    Name: 'Dữ liệu cá nhân'
  }
]

export const listDataSetFiles = [
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
]

export const listUserTypes = [
  {
    Id: 1,
    Name: 'Tổ chức'
  },
  {
    Id: 2,
    Name: 'Cá nhân'
  }
]

export const listUserStatus = [
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
]

export const listConfirmData = [
  {
    Id: 1,
    Name: 'Duyệt'
  },
  {
    Id: 2,
    Name: 'Không duyệt'
  }
]

export const listDataSetHots = [
  {
    Id: 1,
    Name: 'Dữ liệu nổi bật'
  },
  {
    Id: 2,
    Name: 'Dữ liệu bình thường'
  }
]


