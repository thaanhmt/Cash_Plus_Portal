myApp.controller('UserUnitController', ['$scope', '$http', '$mdDialog', 'config', 'cfpLoadingBar', 'app', '$cookies', '$rootScope', '$window', 'ngDialog', '$uibModal', '$filter', 'vcRecaptchaService', function UserUnitController($scope, $http, $mdDialog, config, cfpLoadingBar, app, $cookies, $rootScope, $window, ngDialog, $uibModal, $filter, vcRecaptchaService) {
    $scope.page = 1;
    $scope.page_size = 10;
    $scope.query = "1=1";
    $scope.register = {};
    $scope.TinTuc = {};
    $scope.message = {};
    $scope.q = {};
    $scope.rowIndex = 1;
    $scope.orderby = "";
    $scope.item_count = 0;
    $scope.IdFile = 0;
    $scope.disableBtn = { btSubmit: false };
    $scope.linkFB = '';
    $scope.linkIns = '';
    $scope.linkTiktok = '';
    $scope.ItemId = '';
    $scope.SocialNetWorks = [];
    $scope.listSchool = [];
    $scope.disableFB = true;
    $scope.itemFb = false;
    $scope.itemIns = false;
    $scope.itemTikTok = false;
    $scope.item = false;
    $scope.items = 0;
    $scope.School = null;
    $scope.contact = {};
    $scope.workingTime = {};
    $scope.workingTimes = [];
    $scope.otherDocuments = [];
    $scope.otherDocument = {};
    $scope.RegisterCode = {
        phone_number: '',
        bank_account: '',
        bank_code: '',
        full_name: '',
        share_code: ''
    };
    $scope.UpdatePartner = {};
    $scope.merchant = {};
    $scope.formBankAccount = {};
    $scope.formSubMerchant = {};
    $scope.formOtherDocument = {};
    $scope.formPartnerUpdate = {};
    $scope.formListContact = {};
    $scope.formListTime = {};
    $scope.formListDocument = {};
    $scope.subMerchants = [];
    $scope.merchants = [];
    $scope.banks = [];
    $scope.LstBank = [];
    $scope.industries = [];
    $scope.industryGroup = [];
    $scope.documents = [];
    $scope.document = {};
    $scope.bankAccounts = [];
    $scope.bankAccountAdds = [];
    $scope.bankAccount = {};
    $scope.contacts = [];
    $scope.contact = {};
    $scope.bankAccountInput = [];
    $scope.Listmerchants = [];
    $scope.map;

    // list product merchant
    $scope.currentPageProduct = 1;
    $scope.pageSizeProduct = 12;
    $scope.loadedProducts = []; // Store all loaded products
    $scope.startIndexListProduct = 0;

    $scope.requiredAuthDocNumb = false;
    $scope.companyBranchs = [];
    var service_typename = "";
    $scope.bankInfo = "*Thông tin tài khoản ngân hàng\nTên ngân hàng: \nSố tài khoản: \nChủ tài khoản: ";
    $scope.accountingInfo = "*Thông tin kế toán\nHọ và tên: \nSố điện thoại: \nEmail: ";
    $scope.dataBaomat = '';
    $scope.countdown = 120;
    $scope.disableButton = false;
    $scope.checkBox = false;
    $scope.showAddChain = true;
    $scope.showFormUserDN = true;
    $scope.showFormUserCN = false;
    $scope.showFormUserHKD = false;
    $scope.showFormCoSo = true;
    $scope.showChuoiCoso = false;
    $scope.showPassword = false;
    $scope.checkDataOcrGPDKKD = false;
    $scope.disabledSettlementByBranch1 = true;
    $scope.bankAccountInvalid = false;
    $scope.disabledAccountType = false;
    $scope.Vido = 0;
    $scope.Kinhdo = 0;
    $scope.windowWidth = $window.innerWidth;
    $scope.detailmerchant;
    $scope.recentPartner

    $scope.apiVNPT = config.apiUrlVNPT;
    $scope.tokenIdVnpt = config.tokenIdVnpt;
    $scope.tokenKeyVnpt = config.tokenKeyVnpt;
    $scope.accesstokenVNPT = config.accesstokenVNPT
    $scope.tokenIdVnptv1 = config.tokenIdVnptv1;
    $scope.tokenKeyVnptv1 = config.tokenKeyVnptv1;

    $scope.bankName = '';
    $scope.listProductMerchant;
    $scope.totalProducts = 0;
    $scope.regexEmail = '/^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$/';
    $scope.bankAccountId = moment().valueOf();
    $scope.QRCode = '';
    $scope.labels = {
        "acc_numb": "Số tài khoản",
        "acc_holder": "Tên tài khoản",
        "bank_code": "Ngân hàng",
        "auth_doc_numb": "Số giấy ủy quyền",
        "auth_doc_link": "Link giấy ủy quyền",
        "company_name": "Tên doanh nghiệp/HKD",
        "business_model": "Mô hình kinh doanh",
        "sub_merchant_type": "Mô hình chuỗi/nhượng quyền",
        "franchise_brand": "Công ty bên nhượng quyền",
        "email": "Email",
        "company_short_name": "Tên viết tắt",
        "company_foreign_name": "Tên nước ngoài",
        "business_number": "Mã số giấy ĐKKD/ HKD",
        "tax_code": "Mã số thuế/ Mã số HKD",
        "registration_date": "Ngày đăng ký đăng ký kinh doanh",
        "change_date": "Ngày thay đổi đăng ký kinh doanh",
        "fax": "Số fax trên ĐKKD",
        "authorized_capital": "Vốn điều lệ trên ĐKKD",
        "business_registration_certificate_front": "Giấy ĐKKD mặt trước",
        "business_registration_certificate_behind": "Giấy ĐKKD mặt sau",
        "tax_certificate_front": "Giấy chứng nhận thuế mặt trước",
        "tax_certificate_behind": "Giấy chứng nhận thuế mặt sau",
        "mcc_code": "Ngành nghề",
        "gc_code": "Nhóm ngành",
        "phone": "Số điện thoại liên hệ",
        "phone_numb": "Số điện thoại liên hệ",
        "representative_name": "Họ tên người đại diện",
        "representative_title": "Chức danh người đại diện",
        "gender": "Giới tính",
        "dob": "Ngày sinh",
        "nation": "Dân tộc",
        "nationality": "Quốc tịch",
        "document_type": "Loại giấy tờ",
        "id_number": "Số giấy tờ",
        "issued_date": "Ngày cấp",
        "issued_place": "Nơi cấp",
        "valid_thru": "Ngày hết hạn",
        "current_address": "Địa chỉ hiện tại",
        "representative_job": "Công việc người đại diện",
        "hometown": "Quê quán",
        "permanent_address": "Địa chỉ thường trú",
        "religion": "Tôn giáo",
        "place_of_birth": "Nơi sinh",
        "identifiers": "Đặc điểm nhận dạng",
        "citizen_card_front": "Mặt trước CCCD/ CMND/ HC",
        "citizen_card_back": "Mặt sau CCCD/ CMND/ HC",
        "branch_name": "Tên cơ sở",
        "branch_address": "Địa chỉ cơ sở",
        "settlement_by_branch": "Thông tin quyết toán",
        "controlling_company": "Công ty tổng",
        "brand_name": "Tên thương hiệu của công ty con",
        "list_address": "Địa chỉ",
        "list_email": "Địa chỉ email",
        "list_phone_numb": "Số điện thoại",
        "list_website": "Website",
        "list_representative_info_br": "Thông tin người đại diện theo ĐKKD",
        "list_representative_info_id": "Thông tin người đại diện theo CCCD",
        "list_acc_no": "Danh sách tài khoản ngân hàng",
        "list_branch_acc_no": "Danh sách chuỗi",
    };

    $scope.listStatus = [
        {
            "Id": 1,
            "Name": "Sử dụng"
        },
        {
            "Id": 10,
            "Name": "Không sử dụng"
        }
    ];

    $scope.listSexs = [
        {
            "Id": 1,
            "Name": "Nam"
        },
        {
            "Id": 2,
            "Name": "Nữ"
        },
        {
            "Id": 3,
            "Name": "Khác"
        }
    ];

    $scope.listTypeId = [
        {
            "Id": 1,
            "Name": "Căn cước công dân"
        },
        {
            "Id": 2,
            "Name": "Hộ chiếu"
        }
    ];

    $scope.listSchool = [
        {
            "Id": 1,
            "Name": "TRƯỜNG CAO ĐẲNG DU LỊCH VÀ THƯƠNG MẠI HÀ NỘI"
        },
        {
            "Id": 2,
            "Name": "TRƯỜNG CAO ĐẲNG CÔNG THƯƠNG VIỆT NAM"
        },
        {
            "Id": 3,
            "Name": "TRƯỜNG KHÁC"
        }
    ];

    $scope.listLink = [
        {
            "Id": 1,
            "Name": undefined,
        }
    ];

    $scope.storeType = [
        { value: 1, old_value: 7, name: "ENTERPRISE" },
        { value: 2, old_value: 8, name: "PERSIONAL" },
        { value: 3, old_value: 17, name: "HOUSE_HOLD_BUSINESS" }
    ];
    /*trạng thái tài khoản ngân hàng*/
    $scope.accountStatus = [
        { value: 1, name: "Hoạt động" },
        { value: 2, name: "Ngừng hoạt động" }
    ];
    /*loại tài khoản ngân hàng*/
    $scope.accountType1 = [
        { value: 0, name: "Tài khoản chính chủ" },
        { value: 2, name: "Tên tài khoản viết tắt" }
    ];
    $scope.accountType2 = [
        { value: 1, name: "Ủy quyền" },
        { value: 2, name: "Tên tài khoản viết tắt" }
    ];
    /*loại tài khoản ngân hàng*/
    $scope.accountTypeClone = [
        { value: 0, name: "Tài khoản chính chủ" },
        { value: 1, name: "Ủy quyền" },
        { value: 2, name: "Tên tài khoản viết tắt" }
    ];
    /*giới tính*/
    $scope.genders = [
        { value: "MALE", name: "Nam" },
        { value: "FEMALE", name: "Nữ" },
        { value: "OTHER", name: "Khác" }
    ];
    /*loại giấy tờ*/
    $scope.documentType = [
        { value: 1, name: "Căn cước công dân" },
        { value: 2, name: "Chứng minh nhân dân" },
        { value: 3, name: "Hộ chiếu" },
        { value: 4, name: "Thị thực" }
    ];
    /*mô hình chuỗi*/
    $scope.subMerchantTypes = [
        { value: 1, name: "Chuỗi" },
        { value: 2, name: "Mô hình nhượng quyền" },
        //{ value: 0, name: "Không phải mô hình nhượng quyền hay công ty con" },
        { value: 3, name: "Công ty con quản lý thương hiệu của công ty tổng" },
    ];
    /*chức vu*/
    $scope.representativeTitles = [
        { value: 1, name: "Giám đốc" },
        { value: 9, name: "Chủ tịch HĐQT" },
        { value: 11, name: "Tổng giám đốc" },
        { value: 2, name: "Giám đốc kinh doanh" },
        { value: 3, name: "Giám đốc tài chính" },
        { value: 4, name: "Giám đốc vận hành" },
        { value: 5, name: "Phó giám đốc" },
        { value: 6, name: "Kế toán trưởng" },
        { value: 7, name: "Chủ hộ kinh doanh" },
        { value: 8, name: "Trưởng phòng kinh doanh" },
        { value: 10, name: "Khác" }
    ];
    /*nghề nghiệp*/
    $scope.representativeJobs = [
        { value: 1, name: "Giáo viên/Bác sĩ/Kỹ sư" },
        { value: 2, name: "Nhân viên văn phòng" },
        { value: 3, name: "Nhân viên dịch vụ/bán hàng" },
        { value: 4, name: "Kinh doanh tự do" },
        { value: 5, name: "Học sinh/sinh viên" },
        { value: 6, name: "Lực lượng vũ trang" },
        { value: 7, name: "Nông dân/Công nhân/Ngư dân" },
        { value: 8, name: "Công chức/viên chức" },
        { value: 9, name: "Khác" }
    ];
    /*Loại đầu mối liên hệ*/
    $scope.contactTypes = [
        { id: 1, name: "Hợp đồng & các phụ lục đính kèm", disabled: false },
        { id: 2, name: "Giải quyết khiếu nại và chăm sóc khách hàng", disabled: false }
    ];
    /*Kiểu ký hợp đồng*/
    $scope.contractTypes = [
        { value: 1, name: "Ký giấy" },
        { value: 2, name: "Ký số" }
    ];
    $scope.isPhoneNumberValid = false;


    $scope.startHours = [
        { value: "Mở cửa 7 giờ sáng", name: "Mở cửa 7 giờ sáng" },
        { value: "Mở cửa 8 giờ sáng", name: "Mở cửa 8 giờ sáng" },
        { value: "Mở cửa 9 giờ sáng", name: "Mở cửa 9 giờ sáng" },
        { value: "Mở cửa 10 giờ sáng", name: "Mở cửa 10 giờ sáng" }
    ];

    $scope.endHours = [
        { value: "Đóng cửa 4 giờ chiều", name: "Đóng cửa 4 giờ chiều" },
        { value: "Đóng cửa 5 giờ chiều", name: "Đóng cửa 5 giờ chiều" },
        { value: "Đóng cửa 6 giờ chiều", name: "Đóng cửa 6 giờ chiều" },
        { value: "Đóng cửa 7 giờ tối", name: "Đóng cửa 7 giờ tối" },
        { value: "Đóng cửa 8 giờ tối", name: "Đóng cửa 8 giờ tối" },
        { value: "Đóng cửa 9 giờ tối", name: "Đóng cửa 9 giờ tối" },
        { value: "Đóng cửa 10 giờ tối", name: "Đóng cửa 10 giờ tối" },
        { value: "Đóng cửa 11 giờ tối", name: "Đóng cửa 11 giờ tối" }
    ];

    $scope.otherDocumentTypes = [
        { id: "Hình ảnh cửa hàng", name: "Hình ảnh cửa hàng" },
        { id: "Menu cửa hàng", name: "Menu cửa hàng" },
        { id: "Giấy ủy quyền có liên quan", name: "Giấy ủy quyền có liên quan" },
        { id: "Điều lệ của tổ chức", name: "Điều lệ của tổ chức" },
        { id: "Giấy chứng nhận đầu tư", name: "Giấy chứng nhận đầu tư" },
        { id: "Giấy chuyển đổi CMND-CCCD", name: "Giấy chuyển đổi CMND-CCCD" },
        { id: "Giấy vệ sinh an toàn thực phẩm", name: "Giấy vệ sinh an toàn thực phẩm" },
        {
            id: "Ảnh chụp thông tin người nộp thuế trên trang (đối với trường hợp cá nhân)",
            name: "Ảnh chụp thông tin người nộp thuế trên trang (đối với trường hợp cá nhân)"
        }
    ];
    /*Thêm Giờ mở cửa - Giờ đóng cửa --> Để đa dạng cho cửa hàng. Ví dụ mở 7h đóng 9h; mở 11h đóng 13h; mở 16h đóng 22h. */

    $scope.listMccByGroup = [];

    /*enums*/
    $scope.enums = {
        subMerchantType: {
            NHUONG_QUYEN: 2,
            KHONG_NHUONG_QUYEN: 0,
            CHUOI: 1,
            CONG_TY_CON_QUAN_LY: 3
        },
        storeType: {
            PERSIONAL: 7, //business_model:2
            ENTERPRISE: 8, //business_model: 1
            HOUSE_HOLD_BUSINESS: 17, //business_model: 3
        },
        businessModel: {
            ENTERPRISE: 1, //doanh nghiệp
            PERSIONAL: 2, //cá nhân
            HOUSE_HOLD_BUSINESS: 3, //hộ kinh doanh
        },
        documentType: {
            CCCD: 1, //căn cước công dân
            CMND: 2, //chứng minh nhân dân
            PASSPORT: 3, //hộ chiếu
            VISA: 4, //thị thực
        },
        websiteType: {
            WEBSITE: 1, //website
            PLATFORM: 2, //nền tảng
        },
        gender: {
            MALE: "MALE", //nam
            FEMALE: "FEMALE", //nữ
            OTHER: "OTHER", //khác
        },
        accountStatus: {
            ACTIVE: 1, //Active
            DEACTIVE: 2, //Deactive
        },
        accountType: {
            CHINH_CHU: 0, //Tài khoản chính chủ
            UY_QUYEN: 1, //Ủy quyền (chỉ áp dụng khi business_model = 1 hoặc 3)
            VIET_TAT: 2, //Viết tắt
        },
        contractSignatureType: {
            KY_GIAY: 1,
            KY_SO: 2,
        },
        settlementByBranch: {
            QT_VE_CO_SO: 1,// quyết toán về nhiều cơ sở
            KHONG_QT_VE_CO_SO: 2,// không quyết toán theo cơ sở
        },
    };
    $scope.is_zns_OTP = false;

    var hostname = window.location.hostname;
    var apiUrl = '';
    var baseURL = '';
    if (hostname === 'stg.cashplus.vn') {//stg
        apiUrl = 'https://apistg.cashplus.vn/';
        baseUrl = 'https://cmsstg.cashplus.vn/'
    } else if (hostname === 'cashplus.vn') { //prod
        apiUrl = 'https://apigw.cashplus.vn/';
        baseUrl = 'https://cms.cashplus.vn/'
    }
    else if (hostname === 'www.cashplus.vn') { //prod
        apiUrl = 'https://apigw.cashplus.vn/';
        baseUrl = 'https://cms.cashplus.vn/'
    }
    else if (hostname === '103.72.98.97') { //dev
        apiUrl = 'http://103.72.98.97/';
        baseUrl = 'https://cmsstg.cashplus.vn/'
    }
    else if (hostname === 'localhost') { //dev
        apiUrl = 'http://localhost:44361/';
        baseUrl = 'https://cmsstg.cashplus.vn/'
    }
    else if (hostname === 'cmsdev.cashplus.vn') { //dev
        apiUrl = 'http://cmsdev.cashplus.vn:8633/';
        baseUrl = 'https://cmsstg.cashplus.vn/'
    } else { //localhost
        // apiUrl = 'http://103.72.98.97/';
        // baseUrl = 'https://cmsstg.cashplus.vn/'
        apiUrl = 'http://103.72.98.97/';
        baseUrl = 'http://cmsdev.cashplus.vn/'

    }

    $scope.url = "/";
    var DKKDname = ""
    $scope.checkloading = false; // Biến để kiểm soát hiển thị hiệu ứng loading
    $scope.publicKey = "6Ld_EMEfAAAAAJBZTDIdpXims5GZHQBRLhc0XErX";
    $scope.employee_data = [];
    $scope.access_token;
    var dataLogin = {
        "username": config?.usernameCMS,
        "password": config?.passwordCMS
    }
    var getToken = $http({
        method: 'POST',
        url: apiUrl + 'api/auth/adminLogin',
        data: dataLogin
    })
    getToken.then(function (res) {
        $scope.access_token = res.data.data.token
        $window.sessionStorage.setItem('access_tokenCMS', $scope.access_token);

    })
    $scope.init = function (data, id) {
        cfpLoadingBar.start();
        $scope.loadMerchants();

        if (data != undefined) {
            $scope.customerId = data.CustomerId;
            $scope.access_token = data.access_token;

        }
        $scope.q = {};
        $scope.customer = {};
        $scope.partner = {};
        $scope.register = {};
        $scope.GroupSV = {};
        $scope.merchant = {};
        $scope.customer.StatusView = true;


        if (id != undefined) {
            $scope.userId = id;
            $scope.loadDataById();
            $scope.loadDataByIdMe();

        } else {
            //$scope.loadProvince();
            $scope.loadUnit();
            $scope.loadTypeAttributeItem(4);
            $scope.loadTypeAttributeItem(25);
            $scope.loadTypeAttributeItem(26);
            $scope.loadCountry();
            $scope.loadResearchArea();
            $scope.loadRole(1);
            $scope.loadData();
        }
    };


    $scope.getListBanks = function () {
        var get = $http({
            method: 'GET',
            url: apiUrl + 'api/dropdown/public-bank',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        get.then(function (response) {
            $scope.banks = response?.data?.data ? response.data.data : [];
        });
    }

    $scope.getListIndustries = function () {
        var get = $http({
            method: 'GET',
            url: apiUrl + 'api/dropdown/industries',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        get.then(function (response) {
            $scope.industries = response?.data?.data ? response.data.data : [];
            $scope.getIndustryGroup(response?.data?.data ? response.data.data : []);
        });
    }

    $scope.validatePhoneNumber = function () {
        var phoneNumberPattern = /^0[0-9]{9}$/;
        $scope.isPhoneNumberValid = phoneNumberPattern.test($scope.RegisterCode.phone_number);
    };

    // Gọi hàm validatePhoneNumber khi người dùng nhập số điện thoại
    $scope.$watch('RegisterCode.phone_number', function (newVal, oldVal) {
        $scope.validatePhoneNumber();
    });

    $scope.getIndustryGroup = function (industries) {
        if (industries) {
            const groupedData = industries.reduce((acc, curr) => {
                const { group_id, group_code, group_name, definition_desc } = curr;

                if (!acc[group_id]) {
                    acc[group_id] = {
                        group_id,
                        group_code,
                        group_name,
                        definition_desc
                    };
                }

                return acc;
            }, {});

            $scope.industryGroup = Object.values(groupedData);
        } else {
            $scope.industryGroup = [];
        }
    }

    $scope.getFranchiseBrand = function () {
        var post = $http({
            method: 'POST',
            url: apiUrl + 'api/portal/searchMerchantName',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        post.then(function (response) {
            $scope.merchants = response?.data?.data ? response.data.data : [];
        });
    };

    $scope.getListCompanyBranch = function () {
        var get = $http({
            method: 'GET',
            url: apiUrl + 'api/portal/listCompanyBranch',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        get.then(function (response) {
            let data = response?.data?.data ? response.data.data : [];
            data.push({ id: 0, code: "0", name: "Khác" });
            $scope.companyBranchs = data;
        });
    };

    $scope.getGroupSV = function () {
        var get = $http({
            method: 'GET',
            url: apiUrl + 'web/customer/GetGroupByPage',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.GroupSV = response.data.data;
            $scope.GroupSVCT = $scope.GroupSV.filter(function (item) {
                return item.Location == 2;
            });
            $scope.GroupSVTM = $scope.GroupSV.filter(function (item) {
                return item.Location == 1;
            });
            $scope.topGroup = [];

            $scope.topGroup = $scope.GroupSV.filter(function (item) {
                return item.IsGroup == true;
            }).sort(function (item1, item2) {
                var datetop3gr = new Date(item1.UpdatedAt);
                var datetop3gr2 = new Date(item2.UpdatedAt);
                return datetop3gr - datetop3gr2;
            });
        });
    }

    $scope.getSlide = function () {
        var get = $http({
            method: 'GET',
            url: '/web/slide/GetSlide',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        get.then(function (response) {
            $scope.Slideanh = response.data.data[0].Image;
            $scope.UrlSlideanh = response.data.data[0].Url;
        });
    }

    $scope.getlistProvince = function () {
        var get = $http({
            method: 'GET',
            url: apiUrl + 'web/other / listProvince ? page = 1 & query=1= 1 & order_by=',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.listProvince = response.data.data;
        });
    }

    $scope.loadProvince = function () {
        //var query = "TypeCategoryId=15";
        $http.get("/web/other/listProvince?page=1&query=1=1&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listProvince = data.data.data;
                if ($scope.unitId != undefined) {
                    for (var i = 0; i < $scope.listProvince.length; i++) {
                        if ($scope.listProvince[i].ProvinceId == $scope.unit.ProvinceId) {
                            $scope.dataProvince = $scope.listProvince[i];
                            break;
                        }
                    }
                }
            }
        });
    };

    $scope.getGroupbyId = function () {
        var urlParams = new URLSearchParams(window.location.search);
        var typeAttribute = urlParams.get('TypeAttributeID');

        if (typeAttribute) {
            var url = apiUrl + 'web/customer/GetGroup/' + typeAttribute;
        } else {

            var typeId = localStorage.getItem('TypeAttId');
            var url = apiUrl + 'web/customer/GetGroup/' + typeId;
        }
        var config = {
            method: 'GET',
            url: url,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        };

        $http(config)
            .then(function (response) {
                $scope.NhomSV = response.data.data;
                $scope.listSV = response.data.data.listCustomer;
            });
    }

    $scope.goDetailGroupSV = function (id) {
        $scope.TypeAttID = id;
        var url = '/chi-tiet-nhom/' + id;
        if ($scope.ItemId !== '') {
            localStorage.setItem('TypeAttId', $scope.TypeAttID);
            localStorage.setItem('urldetailgroup', url);
            $window.location.href = url;
        }
    };

    $scope.getListRegister = function () {
        $scope.setTime();
        $scope.getGroupSV();
        var urlParams = new URLSearchParams(window.location.search);
        var schoolCode = urlParams.get('schoolCode');

        for (var i = 0; i < $scope.listSchool.length; i++) {
            if ($scope.listSchool[i].Id == schoolCode) {
                $scope.School = $scope.listSchool[i];
                break;
            }
        }
        var get = $http({
            method: 'GET',
            url: apiUrl + 'web/customer/list-register',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.register = response.data.data;
            $scope.customer = response.data.data;

            $scope.register = $scope.register.filter(function (item) {
                return item.SchoolCode == schoolCode;
            });

            $scope.register1 = response.data.data.filter(function (item) {
                return item.SchoolCode == 1;
            });

            $scope.register2 = response.data.data.filter(function (item) {
                return item.SchoolCode == 2;
            });


            // danh sách được chọn theo trường B2
            $scope.registerTM = response.data.data.filter(function (item) {
                return item.SchoolCode === 1 && item.StepTwo === true;
            }).sort(function (item1, item2) {
                var date1 = new Date(item1.UpdatedAt);
                var date2 = new Date(item2.UpdatedAt);
                return date1 - date2;
            });

            $scope.registerCT = response.data.data.filter(function (item) {
                return item.SchoolCode === 2 && item.StepTwo === true;
            }).sort(function (item1, item2) {
                var date1b2 = new Date(item1.UpdatedAt);
                var date2b2 = new Date(item2.UpdatedAt);
                return date1b2 - date2b2;
            });
            // danh sách được chọn theo trường B4
            $scope.registerTMb4 = response.data.data.filter(function (item) {
                return item.SchoolCode === 1 && item.StepFour === true;
            }).sort(function (item1, item2) {
                var date1b4 = new Date(item1.UpdatedAt);
                var date2b4 = new Date(item2.UpdatedAt);
                return date1b4 - date2b4;
            });

            $scope.registerCTb4 = response.data.data.filter(function (item) {
                return item.SchoolCode === 2 && item.StepFour === true;
            }).sort(function (item1, item2) {
                var date1b4 = new Date(item1.UpdatedAt);
                var date2b4 = new Date(item2.UpdatedAt);
                return date1b4 - date2b4;
            });
            // danh sách được chọn theo trường B5
            $scope.registerTMb5 = response.data.data.filter(function (item) {
                return item.SchoolCode === 1 && item.StepFive === true;
            }).sort(function (item1, item2) {
                var date1b5 = new Date(item1.UpdatedAt);
                var date2b5 = new Date(item2.UpdatedAt);
                return date1b5 - date2b5;
            });

            $scope.registerCTb5 = response.data.data.filter(function (item) {
                return item.SchoolCode === 2 && item.StepFive === true;
            }).sort(function (item1, item2) {
                var date1b5 = new Date(item1.UpdatedAt);
                var date2b5 = new Date(item2.UpdatedAt);
                return date1b5 - date2b5;
            });


            // danh sách được chọn theo trường vòng 4
            $scope.topThree = [];

            $scope.topThree = response.data.data.filter(function (item) {
                return item.TopThree === true;
            }).sort(function (item1, item2) {
                var datetop3sv = new Date(item1.UpdatedAt);
                var datetop3sv2 = new Date(item2.UpdatedAt);
                return datetop3sv - datetop3sv2;
            });



            //for (var i = 0; i < response.data.data.length; i++) {
            //    var total = response.data.data[i].AcademicRankId + response.data.data[i].TypeId + response.data.data[i].UnitId;
            //    if ($scope.topThree.length < 3 || total > $scope.topThree[2].total) {
            //        $scope.topThree.push({ data: response.data.data[i], total: total });
            //        $scope.topThree.sort(function (a, b) {
            //            return b.total - a.total;
            //        });
            //        $scope.topThree = $scope.topThree.slice(0, 3);
            //    }
            //}


        });
    };
    //$scope.UpdatePartner.store_type_id = 8
    $scope.goDetail = function (id) {
        $scope.ItemId = id;
        var url = 'chi-tiet/' + id;
        if ($scope.ItemId !== '') {
            localStorage.setItem('itemId', $scope.ItemId);
            localStorage.setItem('local', url);
            //$window.location.href = url;
        }
    };
    $scope.goDetails = function (id) {
        $scope.ItemId = id;
        var url = 'chi-tiet/';
        if ($scope.ItemId !== '') {
            localStorage.setItem('itemId', $scope.ItemId);
            localStorage.setItem('local', url);
            $window.location.href = url;
        }
    };

    $scope.getMessage = function () {
        var url = 'web/comment/GetCommentByJoinnerId/' + $scope.register.CustomerId;

        var get = $http({
            method: 'Get',
            //url: 'web/customer/new-register',
            url: apiUrl + url,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        get.then(function (data, status, headers) {
            $scope.message = data.data.data;
        });
    };
    $scope.getListRegisterById = function () {
        var urlParams = new URLSearchParams(window.location.search);
        var studentCode = urlParams.get('StudentCode');

        /*12/9*/

        var shareButton = document.getElementById("share-button");
        var shareLink = document.getElementById("share-link");

        var shareUrl = "https://www.facebook.com/sharer/sharer.php?u=https%3A%2F%2Fcashplus.vn%2Fchi-tiet%2F%3FStudentCode=" + studentCode;

        shareLink.setAttribute("href", shareUrl);
        var fbShare = window.location.href;


        shareButton.setAttribute("data-href", fbShare);

        if (studentCode) {
            var url = 'web/customer/getByStudentCode/' + studentCode;
        } else {

            var itemId = localStorage.getItem('itemId');
            var url = 'web/customer/getByStudentCode/' + itemId;
        }
        var config = {
            method: 'GET',
            url: apiUrl + url,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        };

        $http(config)
            .then(function (response) {
                var data = response.data;

                $scope.register = data.data;
                $scope.SocialNetworks = JSON.parse($scope.register.SocialNetworks);
                $scope.likeCount = $scope.register.AcademicRankId ? $scope.register.AcademicRankId : 0;

                var likeCountElement = document.getElementById('likeCount');
                likeCountElement.textContent = $scope.likeCount;

                if ($scope.SocialNetworks) {
                    for (let i = 0; i < $scope.SocialNetworks.length; i++) {
                        if ($scope.SocialNetworks[i].id === 1) {
                            $scope.itemFb = true;
                        }
                        if ($scope.SocialNetworks[i].id === 2) {
                            $scope.itemIns = true;
                        }
                        if ($scope.SocialNetworks[i].id === 3) {
                            $scope.itemTikTok = true;
                        }
                    }
                }
                $scope.SocialNetworks = Object.values(
                    $scope.SocialNetworks.reduce(function (acc, current) {
                        acc[current.id] = current;
                        return acc;
                    }, {})
                );

                $scope.getMessage()

            })
            .catch(function (error) {
            });
    };


    $scope.addComment = function () {
        $scope.message.CustomerId = $scope.register.CustomerId;
        var obj = {
            'CustomerId': $scope.message.CustomerId,
            'Name': $scope.message.Name,
            'Email': $scope.message.Email,
            'Contents': $scope.message.Contents,
        };
        var post = $http({
            method: 'POST',
            url: apiUrl + '/web/comment/AddNewComment',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent("Cảm ơn bạn đã bình chọn thành công!")
                        .ok('Đóng')
                        .fullscreen(false)
                );
                $scope.message.Name = '';
                $scope.message.Email = '';
                $scope.message.Contents = '';
            } else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });
    };


    // ham convert thong tin ngan hang
    $scope.removeAccents = function (str) {
        return str.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toUpperCase();
    };

    $scope.loadData = function () {
        $scope.q.page = $scope.page;
        $scope.q.page_size = $scope.page_size;
        $scope.q.query = $scope.query;
        $scope.q.orderby = $scope.orderby;
        var obj = angular.copy($scope.q);
        if (obj.DateStart != undefined && obj.DateStart != '') {
            obj.DateStart = obj.DateStart.getFullYear() + "/" + (obj.DateStart.getMonth() + 1) + "/" + obj.DateStart.getDate();
        }
        if (obj.DateEnd != undefined && obj.DateEnd != '') {
            obj.DateEnd = obj.DateEnd.getFullYear() + "/" + (obj.DateEnd.getMonth() + 1) + "/" + obj.DateEnd.getDate();
        }
        var post = $http({
            method: 'POST',
            url: apiUrl + '/web/customer/GetByPagePost',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $scope.listCustomers = data.data;
                $scope.item_count = data.metadata;
            } else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });

        //$http.get("/web/dataSet/GetByPage?page=" + $scope.page + "&page_size=" + $scope.page_size + "&query=" + $scope.query + "&order_by=" + $scope.orderby, {
        //    headers: { 'Authorization': 'bearer ' + $scope.access_token }
        //}).then(function (data, status, headers) {
        //    cfpLoadingBar.complete();
        //    if (data.data.meta.error_code === 200) {
        //        $scope.listDataSet = data.data.data;
        //        $scope.item_count = data.data.metadata;
        //    }
        //});
    };

    $scope.loadDataById = function () {

        $http.get("/web/customer/GetById/" + $scope.userId, {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.customer = angular.copy(data.data.data);
                $scope.customer.StatusView = $scope.customer.Status == 1 ? true : false;
                $scope.loadUnit();
                $scope.loadTypeAttributeItem(4);
                $scope.loadTypeAttributeItem(25);
                $scope.loadTypeAttributeItem(26);
                $scope.loadCountry();
                $scope.loadResearchArea();
                $scope.loadRole(1);
                for (var i = 0; i < $scope.listSexs.length; i++) {
                    if ($scope.listSexs[i].Id == $scope.customer.Sex) {
                        $scope.dataSex = $scope.listSexs[i];
                        break;
                    }
                }
                for (var i = 0; i < $scope.listTypeId.length; i++) {
                    if ($scope.listTypeId[i].Id == $scope.customer.TypeId) {
                        $scope.dataTypeId = $scope.listTypeId[i];
                        break;
                    }
                }
                for (var i = 0; i < $scope.listSchool.length; i++) {
                    $scope.dataSchool = $scope.listSchool[i];
                    break;
                }
            }
        });
    };


    $scope.loadDataByIdMe = function () {

        $http.get("/web/customer/getByIdMe/" + $scope.userId, {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.customer = angular.copy(data.data.data);
                $scope.customer.StatusView = $scope.customer.Status == 1 ? true : false;
                $scope.loadUnit();
                $scope.loadTypeAttributeItem(4);
                $scope.loadTypeAttributeItem(25);
                $scope.loadTypeAttributeItem(26);
                $scope.loadCountry();
                $scope.loadResearchArea();
                $scope.loadRole(1);
                for (var i = 0; i < $scope.listSexs.length; i++) {
                    if ($scope.listSexs[i].Id == $scope.customer.Sex) {
                        $scope.dataSex = $scope.listSexs[i];
                        break;
                    }
                }
                for (var i = 0; i < $scope.listTypeId.length; i++) {
                    if ($scope.listTypeId[i].Id == $scope.customer.TypeId) {
                        $scope.dataTypeId = $scope.listTypeId[i];
                        break;
                    }
                }
                for (var i = 0; i < $scope.listSchool.length; i++) {
                    $scope.dataSchool = $scope.listSchool[i];
                    break;
                }
            }
        });
    };

    $scope.loadTypeAttributeItem = function (id) {
        $http.get("/web/other/listTypeItem?page=1&query=TypeAttributeId=" + id + "&order_by=TypeAttributeItemId Asc", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                if (id == 4) {
                    $scope.listPosition = data.data.data;
                    if ($scope.userId != undefined) {
                        for (var i = 0; i < $scope.listPosition.length; i++) {
                            if ($scope.listPosition[i].TypeAttributeItemId == $scope.customer.PositionId) {
                                $scope.dataPosition = $scope.listPosition[i];
                                break;
                            }
                        }
                    }
                } else if (id == 25) {
                    $scope.listAcademicRank = data.data.data;
                    if ($scope.userId != undefined) {
                        for (var i = 0; i < $scope.listAcademicRank.length; i++) {
                            if ($scope.listAcademicRank[i].TypeAttributeItemId == $scope.customer.AcademicRankId) {
                                $scope.dataAcademicRank = $scope.listAcademicRank[i];
                                break;
                            }
                        }
                    }
                } else if (id == 26) {
                    $scope.listDegree = data.data.data;
                    if ($scope.userId != undefined) {
                        for (var i = 0; i < $scope.listDegree.length; i++) {
                            if ($scope.listDegree[i].TypeAttributeItemId == $scope.customer.DegreeId) {
                                $scope.dataDegree = $scope.listDegree[i];
                                break;
                            }
                        }
                    }
                }
            }
        });
    };

    $scope.loadResearchArea = function () {
        let type = 15;
        $http.get("/web/category/GetByPage?page=1&query=TypeCategoryId=" + type + "&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listResearchArea = data.data.data;
                //if ($scope.userId != undefined) {
                //    for (var i = 0; i < $scope.listRoles.length; i++) {
                //        if ($scope.listRoles[i].RoleId == $scope.customer.RoleId) {
                //            $scope.listRoles = $scope.listRoles[i];
                //            break;
                //        }
                //    }
                //}
            }
        });
    };


    $scope.loadCountry = function () {
        $http.get("/web/other/listCountry?page=1&query=1=1&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listCountry = data.data.data;
                if ($scope.userId != undefined) {
                    for (var i = 0; i < $scope.listCountry.length; i++) {
                        if ($scope.listCountry[i].CountryId == $scope.customer.CountryId) {
                            $scope.dataCountry = $scope.listCountry[i];
                            break;
                        }
                    }
                }
            }
        });
    };

    $scope.loadUnit = function () {
        $http.get("/web/unit/listUnit", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listUnits = data.data.data;
                if ($scope.userId != undefined) {
                    for (var i = 0; i < $scope.listUnits.length; i++) {
                        if ($scope.listUnits[i].UnitId == $scope.customer.UnitId) {
                            $scope.dataUnit = $scope.listUnits[i];
                            break;
                        }
                    }
                }
            }
        });
    };


    $scope.changeValue = function (object, type) {
        switch (type) {
            case 1:
                $scope.customer.UnitId = object ? object.UnitId : null;
                break;
            case 2:
                $scope.customer.Sex = object ? object.Id : null;
                break;
            case 3:
                $scope.customer.SchoolCode = object ? object.Id : null;
                $scope.items = $scope.customer.SchoolCode;
                break;
            case 4:
                $scope.customer.TypeId = object ? object.Id : null;
                break;
            case 5:
                $scope.customer.PositionId = object ? object.TypeAttributeItemId : null;
                break;
            case 6:
                $scope.customer.AcademicRankId = object ? object.TypeAttributeItemId : null;
                break;
            case 7:
                $scope.customer.DegreeId = object ? object.TypeAttributeItemId : null;
                break;
            //case 8:
            //    $scope.customer.listRA = object ? object.TypeAttributeItemId : null;
            //    break;
            //case 9:
            //    $scope.customer.listMU = object ? object.TypeAttributeItemId : null;
            //    break;
            case 10:
                $scope.customer.RoleId = object ? object.RoleId : null;
                break;
            case 11:
                $scope.q.Status = object ? object.Id : null;
                break;
            default:
                break;
        }
    }

    $scope.onQueryChange = function () {
        var query = '1=1';
        if ($scope.q.Status != undefined) {
            query += ' AND Status=' + $scope.q.Status;
        }
        if ($scope.q.txtSearch != undefined && $scope.q.txtSearch != '') {
            query += ' AND (FullName.Contains("' + $scope.q.txtSearch + '") or Email.Contains("' + $scope.q.txtSearch + '") or Phone.Contains("' + $scope.q.txtSearch + '"))';
        }
        $scope.query = query;
        $scope.loadData();
    }

    $scope.goManagerUser = function () {
        $window.location.href = '/nguoi-dung-to-chuc';
    };

    $scope.goInfoUser = function () {
        $window.location.href = '/thong-tin-tai-khoan';
    };


    $scope.SaveData = function (kk) {
        if ($scope.customer.Email === '' || $scope.customer.Email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập địa chỉ Email')
                    .ok('Đóng')
                    .fullscreen(false)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("FullNameDk");
                        break;
                    case 2:
                        $scope.focusElement("FullNameMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.customer.UnitId === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa chọn cơ quan/tổ chức')
                    .ok('Đóng')
                    .fullscreen(false)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("EmailDk");
                        break;
                    case 2:
                        $scope.focusElement("EmailMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.customer.FullName === '' || $scope.customer.FullName === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập tên người dùng!')
                    .ok('Đóng')
                    .fullscreen(false)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("EmailDk");
                        break;
                    case 2:
                        $scope.focusElement("EmailMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }

        $scope.disableBtn.btSubmit = true;
        cfpLoadingBar.start();
        var obj = angular.copy($scope.customer);
        obj.CreatedId = $scope.customerId;
        obj.UpdatedId = $scope.customerId;
        obj.Status = obj.StatusView ? 1 : 98;
        if (obj.DateNumber != undefined && obj.DateNumber != '') {
            var dateFull = new Date(obj.DateNumber);
            obj.DateNumber = dateFull.getFullYear() + "/" + (dateFull.getMonth() + 1) + "/" + dateFull.getDate();
        }
        if (obj.Birthday != undefined && obj.Birthday != '') {
            var dateFull = new Date(obj.Birthday);
            obj.Birthday = dateFull.getFullYear() + "/" + (dateFull.getMonth() + 1) + "/" + dateFull.getDate();
        }

        if ($scope.customer.CustomerId == undefined) {
            var post = $http({
                method: 'POST',
                url: apiUrl + '/web/customer',
                data: obj,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });

            post.success(function successCallback(data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                if (data.meta.error_code === 200) {
                    $scope.goManagerUser();
                } else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent(data.meta.error_message)
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }).error(function (data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            });
        } else {
            var post = $http({
                method: 'PUT',
                url: apiUrl + '/web/customer/' + obj.CustomerId,
                data: obj,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });

            post.success(function successCallback(data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                if (data.meta.error_code === 200) {
                    if (kk == 1) {
                        $mdDialog.show(
                            $mdDialog.alert()
                                .clickOutsideToClose(true)
                                .title('Thông báo')
                                .textContent('Cập nhật thông tin thành công!')
                                .ok('Đóng')
                                .fullscreen(false)
                        );
                    } else {
                        $scope.goManagerUser();
                    }
                } else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent(data.meta.error_message)
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }).error(function (data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            });
        }
    };

    $scope.SaveDataMe = function (kk) {

        if ($scope.customer.Email === '' || $scope.customer.Email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập địa chỉ Email')
                    .ok('Đóng')
                    .fullscreen(false)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("FullNameDk");
                        break;
                    case 2:
                        $scope.focusElement("FullNameMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.customer.UnitId === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa chọn cơ quan/tổ chức')
                    .ok('Đóng')
                    .fullscreen(false)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("EmailDk");
                        break;
                    case 2:
                        $scope.focusElement("EmailMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.customer.FullName === '' || $scope.customer.FullName === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập tên người dùng!')
                    .ok('Đóng')
                    .fullscreen(false)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("EmailDk");
                        break;
                    case 2:
                        $scope.focusElement("EmailMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }

        $scope.disableBtn.btSubmit = true;
        cfpLoadingBar.start();
        var obj = angular.copy($scope.customer);
        obj.CreatedId = $scope.customerId;
        obj.UpdatedId = $scope.customerId;
        obj.Status = obj.StatusView ? 1 : 98;
        if (obj.DateNumber != undefined && obj.DateNumber != '') {
            var dateFull = new Date(obj.DateNumber);
            obj.DateNumber = dateFull.getFullYear() + "/" + (dateFull.getMonth() + 1) + "/" + dateFull.getDate();
        }
        if (obj.Birthday != undefined && obj.Birthday != '') {
            var dateFull = new Date(obj.Birthday);
            obj.Birthday = dateFull.getFullYear() + "/" + (dateFull.getMonth() + 1) + "/" + dateFull.getDate();
        }

        var post = $http({
            method: 'PUT',
            url: apiUrl + '/web/customer/updateInfo/' + obj.CustomerId,
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                //if (kk == 1) {
                //    $mdDialog.show(
                //        $mdDialog.alert()
                //            .clickOutsideToClose(true)
                //            .title('Thông báo')
                //            .textContent('Cập nhật thông tin thành công!')
                //            .ok('Đóng')
                //            .fullscreen(false)
                //    );
                //}
                //else {
                $scope.goInfoUser();
                /*}*/
            } else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông tin')
                    .textContent(data.meta.error_message)
                    .ok('Đóng')
                    .fullscreen(false)
            );

        }).catch(function (error) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });
    };


    $scope.deleteData = function (item) {
        var confirm = $mdDialog.confirm()
            .title('Thông báo')
            .textContent('Bạn có chắc chắn muốn xóa bản ghi này?')
            .ok('Đồng ý')
            .cancel('Hủy');

        $mdDialog.show(confirm).then(function () {
            var remove = $http({
                method: 'DELETE',
                url: apiUrl + '/web/customer/' + item.CustomerId,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });

            remove.success(function (data, status, headers, config) {
                if (data.meta.error_code == 200) {
                    $scope.loadData();
                    $mdToast.show($mdToast.simple()
                        .textContent('Xóa thành công!')
                        .position('fixed bottom right')
                        .hideDelay(3000));
                } else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }).error(function () {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            });
        });
    }

    $scope.uploadAvatar = function (e) {

        $scope.checkloading = true; // Biến để kiểm soát hiển thị hiệu ứng loading
        if (e === undefined) return;
        if (e.files.length <= 0) return;

        var fd = new FormData();
        fd.append("file", e.files[0]);
        cfpLoadingBar.start();
        var post = $http({
            method: 'POST',
            url: apiUrl + '/web/upload/uploadImage/6',
            data: fd,
            headers: {
                "Content-Type": undefined,
                'Authorization': 'bearer ' + $scope.access_token
            }
        });
        // haohv 

        post.success(function successCallback(data, status, headers, config) {
            post.success(function successCallback(data, status, headers, config) {
                cfpLoadingBar.complete();
                if (data.meta.error_code === 200) {
                    $scope.checkloading = false;
                    if ($scope.customer === undefined) {
                        $scope.customer = {};
                        $scope.customer.Avata = data.data[0];
                    } else {
                        $scope.customer.Avata = data.data[0];
                    }
                } else {
                    $scope.checkloading = false;
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông báo')
                            .textContent(data.meta.error_message)
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }).error(function (data, status, headers, config) {
                cfpLoadingBar.complete();
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            });

            //    cfpLoadingBar.complete();
            //    if (data.meta.error_code === 200) {
            //        $scope.EditCustomer = {};
            //        $scope.EditCustomer.CustomerId = $scope.customerId;
            //        $scope.EditCustomer.Avata = data.data[0];
            //        var obj = angular.copy($scope.EditCustomer);
            //        var post = $http({
            //            method: 'PUT',
            //            url : apiUrl + '/web/customer/updateAvata/1',
            //            data: obj,
            //            headers: { 'Authorization': 'bearer ' + $scope.access_token }
            //        });

            //        post.success(function successCallback(data, status, headers, config) {
            //            $scope.disableBtn.btSubmit = false;
            //            cfpLoadingBar.complete();
            //            if (data.meta.error_code === 200) {
            //                $scope.goInfoUser();
            //            }
            //            else {
            //                $mdDialog.show(
            //                    $mdDialog.alert()
            //                        .clickOutsideToClose(true)
            //                        .title('Thông tin')
            //                        .textContent(data.meta.error_message)
            //                        .ok('Đóng')
            //                        .fullscreen(false)
            //                );
            //            }
            //        }).error(function (data, status, headers, config) {
            //            $scope.disableBtn.btSubmit = false;
            //            cfpLoadingBar.complete();
            //            $mdDialog.show(
            //                $mdDialog.alert()
            //                    .clickOutsideToClose(true)
            //                    .title('Thông báo')
            //                    .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
            //                    .ok('Đóng')
            //                    .fullscreen(false)
            //            );
            //        });
            //    }
            //    else {
            //        $mdDialog.show(
            //            $mdDialog.alert()
            //                .clickOutsideToClose(true)
            //                .title('Thông báo')
            //                .textContent(data.meta.error_message)
            //                .ok('Đóng')
            //                .fullscreen(false)
            //        );
            //    }
            //}).error(function (data, status, headers, config) {
            //    cfpLoadingBar.complete();
            //    $mdDialog.show(
            //        $mdDialog.alert()
            //            .clickOutsideToClose(true)
            //            .title('Thông báo')
            //            .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
            //            .ok('Đóng')
            //            .fullscreen(false)
            //    );
        });
    };


    $scope.ParseNumberToArray = function () {
        var floor = Math.floor($scope.item_count / $scope.page_size);
        var LayDu = $scope.item_count % $scope.page_size;
        floor = LayDu > 0 ? floor + 1 : floor;
        floor = floor === 0 ? 1 : floor;
        $scope.NumberOfPage = floor;
        return new Array(floor);
    };

    $scope.ChangePage = function (cs, page) {
        switch (cs) {
            case 1:
                $scope.page = $scope.page - 1;
                break;
            case 2:
                $scope.page = page;
                break;
            case 3:
                $scope.page = $scope.page + 1;
                break;
            case 4:
                $scope.page = $scope.page - 1;
                break;
            default:
                break;
        }
        $scope.loadProduct();
    };
    $scope.clearAvatar = function () {
        var confirm = $mdDialog.confirm()
            .title('Thông báo')
            .textContent('Bạn có chắc chắn muốn xóa ảnh đại diện?')
            .ok('Đồng ý')
            .cancel('Hủy');
        $mdDialog.show(confirm).then(function () {
            $scope.disableBtn.btSubmit = true;
            cfpLoadingBar.start();
            $scope.UpdatePartner.avatar == '';
        })
    }

    $scope.clearDocument = function (documentItem) {
        var index = $scope.otherDocuments.indexOf(documentItem);
        if (index !== -1) {
            $scope.otherDocuments.splice(index, 1);
        }
    };

    $scope.fillCurrentAddress = function () {
        if ($scope.useHometownAddress) {
            $scope.UpdatePartner.current_address = $scope.UpdatePartner.identifier_address;
        } else {
            $scope.UpdatePartner.current_address = '';
        }
    };

    $scope.clearFile = function () {
        var confirm = $mdDialog.confirm()
            .title('Thông báo')
            .textContent('Bạn có chắc chắn muốn xóa ảnh đại diện?')
            .ok('Đồng ý')
            .cancel('Hủy');

        $mdDialog.show(confirm).then(function () {
            $scope.disableBtn.btSubmit = true;
            cfpLoadingBar.start();
            $scope.EditCustomer = {};
            $scope.EditCustomer.CustomerId = $scope.customerId;
            $scope.EditCustomer.Avata = undefined;
            var obj = angular.copy($scope.EditCustomer);

            var post = $http({
                method: 'PUT',
                url: apiUrl + 'web/customer/updateAvata/2',
                data: obj,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });

            post.success(function successCallback(data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                if (data.meta.error_code === 200) {
                    angular.element("input[type='file']").val(null);
                    $scope.customer.Avata = undefined;
                    $scope.goInfoUser();
                } else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent(data.meta.error_message)
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }).error(function (data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            });
        });
    };
    $scope.onSubmit = function (kk) {
        if ($scope.customer?.FullName === '' || $scope.customer?.FullName === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập tên người dùng!')
                    .ok('Đóng')
                    .fullscreen(false)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("EmailDk");
                        break;
                    case 2:
                        $scope.focusElement("EmailMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.customer.StudentCode === '' || $scope.customer.StudentCode === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập mã học sinh viên!')
                    .ok('Đóng')
                    .fullscreen(false)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("EmailDk");
                        break;
                    case 2:
                        $scope.focusElement("EmailMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.customer.Sex === '' || $scope.customer.Sex === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa chọn giới tính!')
                    .ok('Đóng')
                    .fullscreen(false)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("EmailDk");
                        break;
                    case 2:
                        $scope.focusElement("EmailMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.customer.SchoolCode === '' || $scope.customer.SchoolCode === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa chọn trường!')
                    .ok('Đóng')
                    .fullscreen(false)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("EmailDk");
                        break;
                    case 2:
                        $scope.focusElement("EmailMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.customer.StudentYear === '' || $scope.customer.StudentYear === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập khóa học!')
                    .ok('Đóng')
                    .fullscreen(false)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("EmailDk");
                        break;
                    case 2:
                        $scope.focusElement("EmailMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.customer.StudentClass === '' || $scope.customer.StudentClass === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập lớp học!')
                    .ok('Đóng')
                    .fullscreen(false)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("EmailDk");
                        break;
                    case 2:
                        $scope.focusElement("EmailMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.customer.SchoolCode == 3) {
            if ($scope.customer.Note === '' || $scope.customer.Note === undefined) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Chưa nhập tên trường!')
                        .ok('Đóng')
                        .fullscreen(false)
                ).finally(function () {
                    switch (type) {
                        case 1:
                            $scope.focusElement("EmailDk");
                            break;
                        case 2:
                            $scope.focusElement("EmailMb");
                            break;
                        default:
                            break;
                    }
                });
                return;
            }
        }
        if ($scope.customer.Email === '' || $scope.customer.Email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập Email!')
                    .ok('Đóng')
                    .fullscreen(false)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("EmailDk");
                        break;
                    case 2:
                        $scope.focusElement("EmailMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.customer.Phone === '' || $scope.customer.Phone === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập số điện thoại!')
                    .ok('Đóng')
                    .fullscreen(false)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("EmailDk");
                        break;
                    case 2:
                        $scope.focusElement("EmailMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.linkFB === '' || $scope.linkFB === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập đường dẫn Facebook!')
                    .ok('Đóng')
                    .fullscreen(false)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("EmailDk");
                        break;
                    case 2:
                        $scope.focusElement("EmailMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }

        if ($scope.disableFB) {
            $scope.SocialNetWorks.push({ id: 1, link: $scope.linkFB });
        }
        if ($scope.disableIns) {
            $scope.SocialNetWorks.push({ id: 2, link: $scope.linkIns });
        }
        if ($scope.disableTiktok) {
            $scope.SocialNetWorks.push({ id: 3, link: $scope.linkTiktok });
        }
        cfpLoadingBar.start();
        var obj = angular.copy($scope.customer);

        obj.SocialNetWorks = JSON.stringify($scope.SocialNetWorks);// haohv

        if (vcRecaptchaService.getResponse() === "") {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông tin')
                    .textContent('Chưa xác thực người dùng!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        } else {
            var post = $http({
                method: 'POST',
                url: apiUrl + 'web/customer/new-register',
                data: obj,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });
            post.success(function successCallback(data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                if (data.meta.error_code === 200) {
                    if (kk == 1) {
                        $scope.showDialog()
                    }
                } else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent(data.meta.error_message)
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }).error(function (data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            });

        }
    };
    $scope.openPopup = function () {
        $mdDialog.show({
            templateUrl: '/dang-ky-thong-tin',
            controller: 'UserUnitController',
            parent: angular.element(document.body),
            clickOutsideToClose: true,
            fullscreen: false
        });
    };
    $scope.showDialog = function () {
        $mdDialog.show(
            $mdDialog.alert()
                .clickOutsideToClose(true)
                .title('Thông báo')
                .textContent('Cảm ơn bạn đã đăng ký tham dự chương trình Chiến Binh Khởi Nghiệp. Thông tin của bạn sẽ sớm được đăng tải trên Website của chương trình!')
                .ok('Đóng')
                .fullscreen(false)
        ).finally(function () {
            $window.location.href = '/';
        });
    };
    $scope.formatDate = function (dateString) {
        var dateObj = new Date(dateString);
        var options = {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
        };
        return dateObj.toLocaleString('en-US', options);
    };
    $scope.shareURL = window.location.href;
    $scope.encodedURL = 'https://www.facebook.com/sharer/sharer.php?u={{$scope.shareURL}}%2Fdocs%2Fplugins%2F&amp;src=sdkpreparse';

    $scope.setTime = function () {
        var currentDate = new Date();
        // Đặt ngày giới hạn (vd: 31/8)
        var limitDate = new Date(currentDate.getFullYear(), 8, 10); // Tháng 7 vì JavaScript tính tháng từ 0 (0 - 11)

        // Kiểm tra nếu ngày hiện tại sau ngày giới hạn

    }

    $scope.changeValueSchool = function (object) {

        if (object) {
            $scope.register = $scope.customer.reduce(function (acc, item) {
                if (item.SchoolCode === object.Id) {
                    acc.push(item);
                }
                return acc;
            }, []);
        } else {
            $scope.register = [];
        }

    };

    $scope.showScrollButton = false;

    // Xử lý sự kiện cuộn trang
    angular.element($window).bind('scroll', function () {
        var header = angular.element(document.getElementById("header"));
        var headerOffset = header.offset().top;
        var scrollTop = angular.element($window).scrollTop();


        if ($window.pageYOffset > 100) {
            $scope.isSticky = true;
            $scope.showScrollButton = true;
        } else {
            $scope.isSticky = false;
            $scope.showScrollButton = false;
        }
        $scope.$apply();
    });

    // Hàm xử lý khi nhấp vào nút "Quay lại đầu trang"
    $scope.scrollToTop = function () {
        $window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    };
    $scope.scrollToBottom = function () {
        // Tính toán chiều cao của tài liệu và trừ đi 100px
        var scrollHeight = document.documentElement.scrollHeight;
        var offset = 100;

        // Cuộn tới vị trí đã tính toán
        $window.scrollTo({
            top: scrollHeight - offset,
            behavior: 'smooth'
        });
    };

    //Xử lý nhận OTP
    $scope.SubmitOTP = function (checkIsZalo) {
        $scope.isPhoneNumberValid = false;
        // Kiểm tra định dạng số điện thoại
        var phoneNumberPattern = /^0[0-9]{9}$/;
        if (!phoneNumberPattern.test($scope.RegisterCode.phone_number)) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Số điện thoại không đúng định dạng. Vui lòng nhập lại.')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }



        var obj = {
            "phone_number": angular.copy($scope.RegisterCode.phone_number),
            "is_send_zalo": !!checkIsZalo,
        }

        var post = $http({
            method: 'POST',
            url: apiUrl + 'api/app/auth/sendOTPRegister',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });


        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.code == 200) {
                if (data.data.code === '500') {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent('Xảy ra lỗi khi thực hiện gửi OTP!')
                            .ok('Đóng')
                            .fullscreen(false)
                    )
                    return;
                }
                if (!data.data.is_account) {
                    $scope.disableButton = true;
                    var inputField = document.querySelector('[ng-click="showModalSelectionOTP()"]');
                    if (window.innerWidth <= 768) {
                        inputField.style.margin = '-3px -20px -1px 0px';
                    } else {
                        inputField.style.marginRight = '-22px';
                        inputField.style.marginLeft = '14px';
                        inputField.style.marginBottom = '-1px';
                        inputField.style.marginTop = '16px';
                    }


                    $scope.isOTP = false;
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent('Đã gửi mã OTP. Bạn hãy kiểm tra OTP ở điện thoại và nhập mã vào ô bên dưới!')
                            .ok('Đóng')
                            .fullscreen(false)
                    );

                    if ($scope.isPhoneNumberValid == false) {
                        $scope.onTime();
                    }
                } else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent('Số điện thoại đã đăng ký trên hệ thống. Vui lòng kiểm tra lại!')
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }

            } else {
                if (data.data.is_account) {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent('Số điện thoại đã đăng ký trên hệ thống. Vui lòng kiểm tra lại!')
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                } else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông báo')
                            .textContent('Có lỗi trong quá trình gửi OTP xác nhận. Xin vui lòng thử lại sau!')
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }

                $scope.isPhoneNumberValid = true;
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Có lỗi trong quá trình gửi OTP xác nhận. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
            $scope.isPhoneNumberValid = true;

        });
    }

    angular.element($window).bind('resize', function () {
        $scope.$apply(function () {
            $scope.windowWidth = $window.innerWidth;
        });
    });

    $scope.onTime = function () {
        var timer = setInterval(function () {
            $scope.countdown -= 1;
            $scope.$apply(); // Cập nhật giá trị trên giao diện

            if ($scope.countdown === 0) {

                clearInterval(timer);
                $scope.disableButton = false; // Kích hoạt lại nút
                $scope.isPhoneNumberValid = true;
                $scope.$apply(); // Cập nhật giá trị trên giao diện
                $scope.countdown = 120;
            }
        }, 1000);
    };

    $scope.SubmitContact = function () {
        if ($scope.contact.FullName === '' || $scope.contact?.FullName === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập họ tên!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.contact.Email === '' || $scope.contact?.Email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập email!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.contact.Phone === '' || $scope.contact?.Phone === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Số điện thoại không hợp lệ!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.contact.NewsId === '' || $scope.contact?.NewsId === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn thành phố!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.contact.Title === '' || $scope.contact?.Title === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập lĩnh vực kinh doanh!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }

        var obj = $scope.contact
        var post = $http({
            method: 'POST',
            url: apiUrl + 'web/contact/SendContact',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        post.success(function successCallback(data) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Bạn đã gửi thông tin liên hệ thành công. Trong thời gian sớm nhất chúng tôi sẽ liên hệ và trao đổi với bạn. Cảm ơn bạn!')
                        .ok('Đóng')
                        .fullscreen(false)
                ).finally(function () {
                    $window.location.href = 'https://cashplus.vn';
                });
            } else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi! Xin vui lòng thử lại sau.')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });
    }
    //Đăng ký tài khoản bằng mã giới thiệu
    $scope.SubmitRegister = function () {
        console.log($scope.RegisterCode)
        if ($scope.RegisterCode.phone_number === '' || $scope.RegisterCode.phone_number === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số điện thoại')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCode.full_name === '' || $scope.RegisterCode.full_name === undefined || $scope.RegisterCode.full_name === 'Không tìm thấy') {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng nhập tên. Trường tên sẽ hiển thị sau khi khai báo thông tin ngân hàng')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCode?.bank_code === '' || $scope.RegisterCode?.bank_code === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn ngân hàng')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCode.bank_account === '' || $scope.RegisterCode.bank_account === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số tài khoản')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.checkBox === false) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng đồng ý với các điều khoản sử dụng và chính sách bảo mật của CashPlus')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        var obj = {
            bankBin: $scope.RegisterCode.bank_code,
            username: $scope.RegisterCode.bank_account,
            full_name: $scope.RegisterCode.full_name,
            phone_number: $scope.RegisterCode.phone_number,
            share_code: $scope.RegisterCode.share_code,
            is_accept_policy: true,
            device_id: ''
        };
        var post = $http({
            method: 'POST',
            url: apiUrl + 'api/app/auth/regis-with-bankacc',
            data: obj
        });

        $scope.disableBtn.btSubmit = true;

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            console.log(data)
            cfpLoadingBar.complete();
            if (data.code == 200) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Xin chúc mừng, bạn đã đăng ký tài khoản CashPlus thành công. Bước tiếp theo bạn cài đặt app CashPlus, đăng nhập và chọn dịch vụ tiêu dùng để được HOÀN TIỀN!')
                        .ok('Đóng')
                        .fullscreen(false)
                ).finally(function () {
                    //Xử lý trả về link tương ứng với từng thiết bị
                    if (/(iPad|iPhone|iPod)/g.test(navigator.userAgent)) {
                        $window.location.href = 'https://apps.apple.com/vn/app/cashplus/id6459993279?l=vi';
                        setTimeout(function () {
                        }, 2000);
                    } else if (/Android/g.test(navigator.userAgent)) {
                        $window.location.href = 'https://play.google.com/store/apps/details?id=com.cashbackplus&hl=vi-VN';
                        setTimeout(function () {
                        }, 2000);
                    } else {
                        $window.location.href = 'https://cashplus.vn/tai-cashplus-ngay';
                        setTimeout(function () {
                        }, 2000);
                    }
                });
            } else {
                if (data.error === "ERROR_OTP_CODE_INCORRECT") {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent('Đã xảy ra lỗi khi đăng ký tài khoản CashPlus. Xin hãy đảm bảo bạn nhập đúng mã OTP!')
                            .ok('Đóng')
                            .fullscreen(false)
                    )
                } else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent('Đã xảy ra lỗi khi đăng ký tài khoản CashPlus. Xin vui lòng thử lại sau! ' + data.error)
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi khi đăng ký tài khoản CashPlus. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });
    }

    $scope.getQRInfo = function () {
        var urlParams = new URLSearchParams(window.location.search);
        var obj = urlParams.get('sharecode');

        var post = $http({
            method: 'POST',
            url: apiUrl + 'api/app/auth/checkShareCode',
            data: {
                "share_code": obj
            },
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.then(function (response) {
            $scope.QRInfo = response.data.data;
            if (response.data.data && response.data.code === '200') {
                $scope.QRCode = response.data.data.share_code
                $scope.RegisterCode.share_code = response.data.data.share_code
            } else {
                $scope.QRCode = null
                $scope.RegisterCode.share_code = null
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Không tìm thấy thông tin mã giới thiệu.')
                        .ok('Đóng')
                        .fullscreen(false)
                );

            }
            //$scope.RegisterCode.full_name = $scope.QRInfo.full_name;
        });
    };
    $scope.getLstBank = function () {
        var get = $http({
            method: 'GET',
            url: apiUrl + 'api/app/bank-acc/list-bank'
        });
        get.then(function (response) {
            console.log(response, 'responese')
            if (response.data.code === '200') {
                $scope.LstBank = response.data.data;
            } else {
                $scope.LstBank = [];
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi khi lấy dữ liệu danh sách ngân hàng.')
                        .ok('Đóng')
                        .fullscreen(false)
                );

            }
            //$scope.RegisterCode.full_name = $scope.QRInfo.full_name;
        });
    };
    $scope.onBankAccountChange = function () {
        if ($scope.RegisterCode.bank_code && $scope.RegisterCode.bank_account) {
            $scope.getBankAccountName();
        }
    };
    $scope.getBankAccountName = function () {
        $http({
            method: 'POST',
            url: apiUrl + 'api/app/bank-acc/get-bank-owner',
            data: {
                bankBin: $scope.RegisterCode.bank_code,
                bankNumber: $scope.RegisterCode.bank_account
            }
        }).then(function successCallback(response) {
            console.log(response)
            if (response.data.code === '200') {
                $scope.RegisterCode.full_name = response.data.data;
            } else {
                $scope.RegisterCode.full_name = 'Không tìm thấy';
                // Có thể thêm thông báo lỗi nếu cần
            }
        }, function errorCallback(response) {
            $scope.RegisterCode.full_name = 'Không tìm thấy';
            console.error('Error fetching bank account name:', response);
        });
    };
    //template affiliate
    $scope.ShowAffiliate = function () {
        var urlParams = new URLSearchParams(window.location.search);
        var obj = urlParams.get('g');

        var post = $http({
            method: 'POST',
            url: apiUrl + 'api/app/auth/checkShareCode',
            data: {
                "share_code": obj
            },
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.then(function (response) {
            $scope.QRInfo = response.data.data;
            //$scope.RegisterCode.full_name = $scope.QRInfo.full_name;
        });
    };

    $scope.onRegister = function () {
        var urlParams = new URLSearchParams(window.location.search);
        $scope.RegisterCode.share_code = urlParams;
        $scope.getQRInfo();
        $scope.getLstBank();
    };


    /*huy*/
    $scope.getTinTucBaoMat = function () {

        var get = $http({
            method: 'GET',
            url: apiUrl + 'api/staticpage/getByCode/cspl',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.dataBaomat = response.data.data.content;
        });
    };
    // ngan hang
    //$scope.getBank = function () {
    //    console.log("vao day")
    //    var post = $http({
    //        method: 'POST',
    //        url: apiUrl + 'api/bank/list',
    //        data: {
    //            "page_no": "1",
    //            "page_size": 999
    //        },
    //        headers: { 'Authorization': 'bearer ' + $scope.access_token }
    //    });
    //    post.then(function (response) {
    //        consosle.log("vao day", response)
    //        $scope.Bank = response.data.data;
    //    });
    //};

    $scope.changeValueBank = function (selectedBank) {
        console.log('Selected bank:', selectedBank);
        // Xử lý khi người dùng chọn một ngân hàng
    };

    // thành phố
    $scope.getProvince = function () {
        var get = $http({
            method: 'GET',
            url: apiUrl + 'api/portal/province',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        get.then(function (response) {
            $scope.Province = response.data.data;
        });
    };
    $scope.changeValueProvince = function (selectedItem) {
        console.log("vao ham", selectedItem)

        if (selectedItem) {
            $scope.selectedProvince = selectedItem;
            $scope.getDistrict(selectedItem.id);
            return selectedItem;
        } else {
            $scope.selectedProvince = {};
        }
    };
    // phường
    $scope.getDistrict = function (selectedProvince) {
        var get = $http({
            method: 'GET',
            url: apiUrl + 'api/portal/provinceBy/' + selectedProvince,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.District = response.data.data;
        });
    };
    $scope.changeValueDistrict = function (selectedItem) {
        if (selectedItem) {
            $scope.selectedDictrict = selectedItem;
            $scope.getWard(selectedItem.id);
            return selectedItem;

        } else {
            $scope.selectedDictrict = {};
        }
    };
    // xã
    $scope.getWard = function (selectedDictrict) {

        var get = $http({
            method: 'GET',
            url: apiUrl + 'api/portal/provinceBy/' + selectedDictrict,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.Ward = response.data.data;
        });
    };
    $scope.changeValueWard = function (selectedItem) {
        if (selectedItem) {
            $scope.selectedWard = selectedItem;
            return selectedItem;
        } else {
            $scope.selectedWard = {};
        }
    };
    $scope.getMerchantsByCoordinates = function (lat, lon) {
        const data = {
            "search": "",
            "latitude": lat,
            "longtitude": lon,
            "is_update_zoom": false
        }
        var dataAppLG = {
            "phone_number": config?.usernameAppCP,
            "password": config?.passwordAppCP,
            "is_android": true,
        };

        // Get the token first
        var getToken = $http({
            method: 'POST',
            url: apiUrl + 'api/app/auth/login',
            data: dataAppLG
        });

        getToken.then(function (res) {
            var post = $http({
                method: 'POST',
                url: apiUrl + 'api/app/customer/home/listPartnerV2',
                headers: { 'Authorization': 'bearer ' + res.data?.data?.token },
                data: data
            });
            post.then(function (response) {

                $scope.recentPartner = response.data.data.data;
                console.log($scope.recentPartner)
            });

        })

    }

    $scope.getInfoDetailMerchant = function () {
        try {
            console.log("da vao day r")
            const merchantId = sessionStorage.getItem('selectedMerchantId');
            if (window.location.pathname.includes('/shop/') && merchantId) {
                var dataLogin = {
                    "username": config?.usernameCMS,
                    "password": config?.passwordCMS
                }
                var getToken = $http({
                    method: 'POST',
                    url: apiUrl + 'api/auth/adminLogin',
                    data: dataLogin
                })
                getToken.then(function (res) {
                    const token = res.data.data.token
                    var get = $http({
                        method: 'GET',
                        url: apiUrl + `api/partner/${merchantId}`,
                        headers: { 'Authorization': 'bearer ' + token }
                    });
                    get.then(function (res) {
                        console.log(res)
                        $scope.detailmerchant = res.data.data;
                        document.title = `${$scope.detailmerchant.name} - Chi tiết đối tác`;
                        // Xóa sessionStorage sau khi lấy dữ liệu thành công
                        // sessionStorage.removeItem('selectedMerchantId');
                        $scope.getMerchantsByCoordinates($scope.detailmerchant.latitude, $scope.detailmerchant.longitude)
                    }).catch(function (error) {
                        console.error('Có lỗi xảy ra khi gọi API:', error);
                        // Hiển thị thông báo lỗi cho người dùng
                        $mdDialog.show(
                            $mdDialog.alert()
                                .clickOutsideToClose(true)
                                .title('Thông báo')
                                .textContent('Đã xảy ra lỗi khi tải thông tin đối tác. Xin vui lòng thử lại.')
                                .ok('Đóng')
                                .fullscreen(false)
                        );
                    });
                })

            }
        } catch (ex) {
            console.error(ex);
        }
    }

    $scope.gotoDetailMerchant = function (id, partnerName) {
        sessionStorage.setItem('selectedMerchantId', id);
        window.location.href = `/shop/${partnerName}`;
        $scope.getInfoDetailMerchant()
    };


    // quốc gia 
    // xã
    $scope.getCountries = function () {

        var get = $http({
            method: 'GET',
            url: apiUrl + 'api/portal/nation',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.Country = response.data.data;
        });
    };
    $scope.changeValueCountry = function (selectedItem) {
        if (selectedItem) {
            $scope.selectedCountry = selectedItem;
            return selectedItem;
        } else {
            $scope.selectedCountry = {};
        }
    };
    // mô hình kinh doanh
    $scope.getStoretype = function () {

        var get = $http({
            method: 'GET',
            url: apiUrl + 'api/portal/otherListByCode/store_type',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.Storetype = response.data.data;
        });
    };

    var mscn = document.getElementById("mscn");
    var msdn = document.getElementById("msdn");

    //$scope.changeValueTypeStore = function (selectedItem) {
    //    //console.log(selectedItem)
    //    if (selectedItem) {
    //        $scope.selectedStoreType = selectedItem;
    //        if (selectedItem.id == 7) {
    //            mscn.style.display = "block";
    //            msdn.style.display = "none";
    //        } else {
    //            msdn.style.display = "block";
    //            mscn.style.display = "none";
    //        }
    //    } else {
    //        $scope.selectedStoreType = {};
    //    }
    //};

    $scope.$watch('UpdatePartner.store_type_id', function (newValue, oldValue) {

        console.log('Mô hình kinh doanh: ', oldValue, 'to', newValue);
        // Kiểu ký hợp đồng 1: Ký giấy | 2: Ký số (Với doanh nghiệp) hoặc ký điện tử (với Hộ kinh doanh và Cá nhân)
        /*if(newValue === $scope.enums.storeType.ENTERPRISE) {
            $scope.UpdatePartner.contract_signature_type = $scope.enums.contractSignatureType.KY_GIAY;
        } else if(newValue && (newValue === $scope.enums.storeType.PERSIONAL || newValue === $scope.enums.storeType.HOUSE_HOLD_BUSINESS)) {
            $scope.UpdatePartner.contract_signature_type = $scope.enums.contractSignatureType.KY_SO;
        }*/
        //business_model: 1. Doanh nghiệp | 2. Cá nhân | 3. Hộ kinh doanh
        if (newValue == $scope.enums.storeType.ENTERPRISE) {
            $scope.requiredBusinessNumber = true;
            $scope.requiredTaxCode = false;
            $scope.UpdatePartner.business_model = $scope.enums.businessModel.ENTERPRISE;
            $scope.disabledAccountType = false;
        } else if (newValue == $scope.enums.storeType.PERSIONAL) {
            $scope.requiredBusinessNumber = false;
            $scope.requiredTaxCode = false;
            $scope.UpdatePartner.business_model = $scope.enums.businessModel.PERSIONAL;
            $scope.disabledAccountType = false;
        } else if (newValue == $scope.enums.storeType.HOUSE_HOLD_BUSINESS) {
            $scope.requiredBusinessNumber = true;
            $scope.requiredTaxCode = true;
            $scope.UpdatePartner.business_model = $scope.enums.businessModel.HOUSE_HOLD_BUSINESS;
            $scope.disabledAccountType = false;
        } else {
            $scope.requiredBusinessNumber = false;
            $scope.requiredTaxCode = false;
            $scope.UpdatePartner.business_model = '';
        }

        if (newValue == $scope.enums.storeType.ENTERPRISE || newValue == $scope.enums.storeType.HOUSE_HOLD_BUSINESS) {
            $scope.showFormUserDN = true;
            $scope.showFormUserCN = false;
            $scope.showFormCoSo = true;
            $scope.UpdatePartner.representative_title = null;
            $scope.UpdatePartner.representative_job = null;
            if (newValue == $scope.enums.storeType.HOUSE_HOLD_BUSINESS) {
                $scope.showFormUserHKD = true;
            }
        } else if (newValue == $scope.enums.storeType.PERSIONAL) {
            $scope.showFormUserDN = false;
            $scope.showFormUserCN = true;
            $scope.showFormCoSo = $scope.UpdatePartner.sub_merchant_type == 2; //mô hình nhượng quyền 
            $scope.UpdatePartner.representative_title = 7; //chủ hộ kinh doanh
            $scope.UpdatePartner.representative_job = 9; //khác
            $scope.showFormUserHKD = false;
        } else {
            $scope.showFormCoSo = true;
        }

        if ($scope.UpdatePartner.acc_holder) {
            if ($scope.showFormUserCN) {
                $scope.validateBankAccountCN();
            }
            if ($scope.showFormUserDN && $scope.UpdatePartner.acc_holder != $scope.toUpperCaseAndRemoveAccents($scope.UpdatePartner.company_name)) {
                $scope.UpdatePartner.account_type = 1;
            } else {
                $scope.UpdatePartner.account_type = 0;
            }
        }
    });

    $scope.$watch('UpdatePartner.business_model', function (newValue, oldValue) {
        console.log('business_model: ', oldValue, 'to', newValue);
        if (newValue) {
            if (newValue === $scope.enums.businessModel.PERSIONAL) {
                $scope.requiredAuthDocNumb = false;
                if ($scope.UpdatePartner.account_type == 1) $scope.UpdatePartner.account_type = null;
                $scope.accountType = $scope.accountTypeClone.filter(item => item.value !== $scope.enums.accountType.UY_QUYEN);
            } else {
                $scope.accountType = $scope.accountTypeClone;
            }
        }
    });

    $scope.$watchCollection('UpdatePartner.list_documents', function (newValue, oldValue) {
        console.log('list_documents: ', oldValue, 'to', newValue);
        $scope.UpdatePartner.cn_other_document = ($scope.showFormUserCN && newValue && newValue.length > 0) ? 'true' : null;
        $scope.UpdatePartner.other_document = ($scope.showFormUserDN && newValue && newValue.length > 0) ? 'true' : null;
    });

    $scope.$watch('UpdatePartner.account_type', function (newValue, oldValue) {
        console.log('account_type: ', oldValue, 'to', newValue);
        $scope.requiredAuthDocNumb = newValue === $scope.enums.accountType.UY_QUYEN ? true : false;
        if (newValue == $scope.enums.accountType.CHINH_CHU) {
            // $scope.disabledAccHolder = true;
            // if ($scope.showFormUserDN) $scope.UpdatePartner.acc_holder = $scope.UpdatePartner.company_name;
            // if ($scope.showFormUserCN) $scope.UpdatePartner.acc_holder = $scope.UpdatePartner.store_owner;
        } else {
            //$scope.disabledAccHolder = false;
        }
    });

    $scope.$watch('UpdatePartner.document_type', function (newValue, oldValue) {
        console.log('document_type: ', oldValue, 'to', newValue);
        $scope.requiredCitizenNumb = newValue === $scope.enums.documentType.PASSPORT ? true : false;
    });

    $scope.$watch('UpdatePartner.sub_merchant_type', function (newValue, oldValue) {
        console.log('sub_merchant_type: ', oldValue, 'to', newValue);
        $scope.showFranchiseBrand = newValue === $scope.enums.subMerchantType.NHUONG_QUYEN;
        $scope.showInfoChildCompany = newValue === $scope.enums.subMerchantType.CONG_TY_CON_QUAN_LY;
        if (newValue === undefined || newValue === null) {
            $scope.showChuoiCoso = false;
        } else {
            $scope.showChuoiCoso = true;
        }
    });

    $scope.$watch('UpdatePartner.settlement_by_branch', function (newValue, oldValue) {
        console.log('settlement_by_branch: ', oldValue, 'to', newValue);
        $scope.hasMultiBank = newValue == $scope.enums.settlementByBranch.QT_VE_CO_SO;
        //$scope.hasbankAccount = (newValue == $scope.enums.settlementByBranch.QT_VE_CO_SO && $scope.bankAccountAdds.length > 0);
        //$scope.hadInputBankAccount = newValue == $scope.enums.settlementByBranch.QT_VE_CO_SO;
    });

    $scope.$watchGroup(['UpdatePartner.name', 'UpdatePartner.province_id'], function (newValue, oldValue) {
        console.log('name: ', oldValue, 'to', newValue);
        $scope.generateMerchantCode($scope.UpdatePartner.name, $scope.UpdatePartner.province_id);
    });

    $scope.$watchGroup(['Kinhdo', 'Vido'], function () {
        //show popup thông báo bật chia sẻ vị trí
    });

    $scope.$watchCollection('subMerchants', function (newValue, oldValue) {
        console.log('subMerchants: ', oldValue, 'to', newValue);
        $scope.hasSubMerchant = newValue && newValue.length > 0;
    });

    $scope.$watchCollection('contacts', function (newValue, oldValue) {
        console.log('contacts: ', oldValue, 'to', newValue);
        $scope.hasContact = newValue && newValue.length > 0;
    });

    $scope.$watch('bankAccount.type', function (newValue, oldValue) {
        console.log('account_type: ', oldValue, 'to', newValue);
        $scope.checkAuthDocNumb = newValue === $scope.enums.accountType.UY_QUYEN ? true : false;
        if (newValue == $scope.enums.accountType.CHINH_CHU) {
            $scope.bankAccount.disabledAccHolder = true;
            //if ($scope.showFormUserDN) $scope.bankAccount.acc_holder = $scope.UpdatePartner.company_name;
            //if ($scope.showFormUserCN) $scope.bankAccount.acc_holder = $scope.UpdatePartner.store_owner;
        } else {
            $scope.bankAccount.disabledAccHolder = false;
        }
    });

    $scope.$watch('UpdatePartner.acc_holder', function (newValue, oldValue) {
        console.log('main acc_holder: ', oldValue, 'to', newValue);
        if ($scope.UpdatePartner.acc_holder) {
            if ($scope.showFormUserCN) {
                $scope.validateBankAccountCN(true);
            }
            if ($scope.showFormUserDN && newValue != $scope.toUpperCaseAndRemoveAccents($scope.UpdatePartner.company_name)) {
                $scope.UpdatePartner.account_type = 1;//uy quyen
                $scope.disabledAccountType = false;
            } else if ($scope.showFormUserHKD && newValue != $scope.toUpperCaseAndRemoveAccents($scope.UpdatePartner.license_owner) && $scope.UpdatePartner.license_owner) {
                $scope.UpdatePartner.account_type = 1;//uy quyen
                $scope.disabledAccountType = true;
            } else {
                $scope.UpdatePartner.account_type = 0;//chinh chu
                $scope.disabledAccountType = false;
            }
        }
    });

    $scope.$watch('UpdatePartner.license_owner', function (newValue, oldValue) {
        if ($scope.UpdatePartner.acc_holder && $scope.UpdatePartner.license_owner) {
            if ($scope.showFormUserCN) {
                //$scope.validateBankAccountCN(true);
            }
        }
    });

    $scope.$watch('formPartnerUpdate.license_owner.$touched', function (isTouched) {
        if (isTouched) {
            console.log('Input field was touched');
            $scope.validateBankAccountCN(true);
            if ($scope.showFormUserHKD && $scope.UpdatePartner.acc_holder != $scope.toUpperCaseAndRemoveAccents($scope.UpdatePartner.license_owner) && $scope.UpdatePartner.license_owner) {
                $scope.UpdatePartner.account_type = 1;//uy quyen
                $scope.disabledAccountType = true;
            } else {
                $scope.disabledAccountType = false;
            }
        }
    });

    $scope.$watch('UpdatePartner.company_name', function (newValue, oldValue) {
        console.log('company_name: ', oldValue, 'to', newValue);
        if ($scope.UpdatePartner.acc_holder) {
            if (($scope.showFormUserDN && $scope.toUpperCaseAndRemoveAccents(newValue) != $scope.toUpperCaseAndRemoveAccents($scope.UpdatePartner.acc_holder))) {
                $scope.UpdatePartner.account_type = 1;//uy quyen
            } else {
                $scope.UpdatePartner.account_type = 0;//chinh chu
            }
        }
    });

    $scope.$watch('bankAccount.acc_holder', function (newValue, oldValue) {
        console.log('bank acc_holder: ', oldValue, 'to', newValue);
        if ($scope.bankAccount.acc_holder && !$scope.bankAccount.id) {
            if ($scope.showFormUserCN) {
                $scope.validateBankAccountCN(false);
            }
            if ($scope.showFormUserDN && newValue != $scope.toUpperCaseAndRemoveAccents($scope.UpdatePartner.company_name)) {
                $scope.bankAccount.type = 1;//uy quyen
                //cá nhân -> viết tắt
                //doanh nghiệp -> ủy quyền
            } else {
                $scope.bankAccount.type = 0;//chinh chu
            }
        }
    });

    $scope.$watch('bankAccount.acc_numb', function (newValue, oldValue) {
        console.log('bank acc_numb: ', oldValue, 'to', newValue);
    });

    $scope.$watch('merchant.info_representative', function (newValue, oldValue) {
        console.log('info_representative: ', oldValue, 'to', newValue);
        if ($scope.merchant.info_representative) {
            $scope.merchant.branch_rep_name = $scope.UpdatePartner.license_owner;
            $scope.merchant.branch_rep_yob = $scope.UpdatePartner.license_birth_date;
            $scope.merchant.branch_rep_tax_no = $scope.UpdatePartner.tax_code;
            $scope.merchant.branch_rep_identity_no = $scope.UpdatePartner.indetifier_no;
            $scope.merchant.branch_rep_identity_issue_date = $scope.UpdatePartner.identifier_date;
            $scope.merchant.branch_rep_identity_issue_place = $scope.UpdatePartner.identifier_at;
            $scope.merchant.branch_rep_permanent_residence = $scope.UpdatePartner.identifier_address;
        } else {
            // $scope.merchant.branch_rep_name = '';
            // $scope.merchant.branch_rep_yob = null;
            // $scope.merchant.branch_rep_tax_no = '';
            // $scope.merchant.branch_rep_identity_no = '';
            // $scope.merchant.branch_rep_identity_issue_date = null;
            // $scope.merchant.branch_rep_identity_issue_place = '';
            // $scope.merchant.branch_rep_permanent_residence = '';
        }
    });

    $scope.$watch('contact.info_representative', function (newValue, oldValue) {
        console.log('info_representative: ', oldValue, 'to', newValue);
        if ($scope.contact.info_representative) {
            $scope.contact.name = $scope.UpdatePartner.license_owner;
            $scope.contact.phone = $scope.UpdatePartner.phone;
            $scope.contact.email = $scope.UpdatePartner.email;
        } else {
            // $scope.contact.name = '';
            // $scope.contact.phone = '';
            // $scope.contact.email = '';
        }
    });

    $scope.$watch('UpdatePartner.controlling_company', function (newValue, oldValue) {
        console.log('controlling_company: ', oldValue, 'to', newValue);
        $scope.showCompanyOther = newValue == "0" ? true : false;
    });

    $scope.$watchGroup(['UpdatePartner.acc_numb', 'UpdatePartner.acc_holder', 'UpdatePartner.bank_code', 'UpdatePartner.account_type', 'UpdatePartner.auth_doc_numb', 'UpdatePartner.auth_doc_link'], function () {
        //check
        if (!$scope.UpdatePartner.acc_numb || !$scope.UpdatePartner.acc_holder || !$scope.UpdatePartner.bank_code || ![0, 1, 2].includes($scope.UpdatePartner.account_type)) {
            $scope.hadInputBankAccount = false;
            $scope.bankAccounts = [];
        } else {
            $scope.hadInputBankAccount = true;
            $scope.bankAccounts = [{
                id: $scope.bankAccountId,
                operation: 1,
                status: 1,
                is_default: true, //default
                acc_numb: $scope.UpdatePartner.acc_numb,
                bank_code: $scope.UpdatePartner.bank_code,
                acc_holder: $scope.UpdatePartner.acc_holder,
                type: $scope.UpdatePartner.account_type,
                auth_doc_numb: $scope.UpdatePartner?.auth_doc_numb ? $scope.UpdatePartner.auth_doc_numb : '',
                auth_doc_link: $scope.UpdatePartner?.auth_doc_link ? $scope.UpdatePartner.auth_doc_link : '',
            }];
        }
    });

    $scope.$watchCollection('bankAccounts', function (newValues, oldValues) {
        // callback
        let bankAccounts = $scope.bankAccounts;
        let bankAccountAdds = $scope.bankAccountAdds;
        let accounts = [...bankAccounts, ...bankAccountAdds];
        $scope.bankAccountInput = accounts.map(item => {
            item.value = `${item.acc_numb} - ${item.acc_holder} - ${$scope.getSubMerchantBankName(item.bank_code)}`;
            return item;
        });
    });

    $scope.$watchCollection('bankAccountAdds', function (newValues, oldValues) {
        // callback
        let bankAccounts = $scope.bankAccounts;
        let bankAccountAdds = $scope.bankAccountAdds;
        let accounts = [...bankAccounts, ...bankAccountAdds];
        $scope.bankAccountInput = accounts.map(item => {
            item.value = `${item.acc_numb} - ${item.acc_holder} - ${$scope.getSubMerchantBankName(item.bank_code)}`;
            return item;
        });
        let checkDefault = bankAccountAdds.filter(item => item.is_default);
        if (checkDefault && checkDefault.length > 0) {
            $scope.UpdatePartner.bank_default = false;
        } else {
            $scope.UpdatePartner.bank_default = true;
        }
    });

    $scope.$watchCollection('bankAccountInput', function (newValues, oldValues) {
        // callback
        if ($scope.subMerchants && $scope.subMerchants.length > 0) {
            $scope.subMerchants.map(item => {
                let account = $scope.bankAccountInput.filter(ele => ele.id == item.account_id);
                if (account && account.length > 0) {
                    item.acc_holder = account[0].acc_holder;
                    item.acc_numb = account[0].acc_numb;
                    item.bank_code = account[0].bank_code;
                }
                return item;
            });
        }
    });

    $scope.toUpperCaseAndRemoveAccents = function (str) {
        if (!str) return '';
        str = str.toLowerCase();
        let diacriticsMap = {
            a: 'áàảãạăắằẳẵặâấầẩẫậ',
            d: 'đ',
            e: 'éèẻẽẹêếềểễệ',
            i: 'íìỉĩị',
            o: 'óòỏõọôốồổỗộơớờởỡợ',
            u: 'úùủũụưứừửữự',
            y: 'ýỳỷỹỵ'
        };
        Object.keys(diacriticsMap).forEach(function (k) {
            str = str.replace(new RegExp('[' + diacriticsMap[k] + ']', 'g'), k);
        });

        return str.toUpperCase();
    }

    $scope.getBusinessModel = function (storeTypeId) {
        //business_model: 1. Doanh nghiệp | 2. Cá nhân | 3. Hộ kinh doanh
        if (storeTypeId === $scope.enums.storeType.ENTERPRISE) {
            return $scope.enums.businessModel.ENTERPRISE;
        } else if (storeTypeId === $scope.enums.storeType.PERSIONAL) {
            return $scope.enums.businessModel.PERSIONAL;
        } else if (storeTypeId === $scope.enums.storeType.HOUSE_HOLD_BUSINESS) {
            return $scope.enums.businessModel.HOUSE_HOLD_BUSINESS;
        } else {
            return '';
        }
    }

    /*$scope.getContractSignatureType = function (storeTypeId) {
        // Kiểu ký hợp đồng 0: Ký giấy | 1: Ký số (Với doanh nghiệp) hoặc ký điện tử (với Hộ kinh doanh và Cá nhân)
        if(storeTypeId === $scope.enums.storeType.ENTERPRISE) {
            return $scope.enums.contractSignatureType.KY_GIAY;
        } else if(storeTypeId && (storeTypeId === $scope.enums.storeType.PERSIONAL || storeTypeId === $scope.enums.storeType.HOUSE_HOLD_BUSINESS)) {
            return $scope.enums.contractSignatureType.KY_SO;
        }
    }*/


    window.addEventListener("load", function () {
        // Gọi hàm changeValueTypeStore với giá trị mặc định hoặc giá trị từ dữ liệu ban đầu
        if ($scope?.UpdatePartner?.store_type_id) {
            $scope.changeValueTypeStore($scope?.UpdatePartner?.store_type_id)
        }
    });

    $scope.changeValueTypeStore = function (selectedItem) {
        console.log(selectedItem)
        var selectedId = selectedItem?.selected?.id;
        var storeTypeId = $scope?.UpdatePartner?.store_type_id;
        // Ensure store_type_id is defined before proceeding with the comparison
        if (typeof storeTypeId !== 'undefined' && ((storeTypeId === 8 || (selectedItem && selectedItem?.selected?.name === 'Doanh nghiệp')) || (storeTypeId === 17 || (selectedItem && selectedItem?.selected?.name === 'Hộ kinh doanh cá thể')))) {
            if (document.getElementById('frmfileDinhKem')) document.getElementById('frmfileDinhKem').style.display = 'block';
            //if(document.getElementById('header-text-regis-partnert')) document.getElementById('header-text-regis-partnert').style.display = 'block';
            if (document.getElementById('desFrmUserCN')) document.getElementById("desFrmUserCN").style.display = "none";
            if (document.getElementById('desFrmUserDN')) document.getElementById("desFrmUserDN").style.display = "block";
            //document.getElementById("mscn").style.display = "none";
            //document.getElementById("msdn").style.display = "block";
            //document.getElementById("dangkykinhdoanh").style.display = "block";
            //document.getElementById("dangkythue").style.display = "none";
            //document.getElementById('detailFileDinhKem').style.display = 'block'; // no use
        } else {
            if (document.getElementById('frmfileDinhKem')) document.getElementById('frmfileDinhKem').style.display = 'none';
            //if(document.getElementById('header-text-regis-partnert')) document.getElementById('header-text-regis-partnert').style.display = 'none';
            if (document.getElementById('desFrmUserDN')) document.getElementById("desFrmUserDN").style.display = "none";
            if (document.getElementById('desFrmUserCN')) document.getElementById("desFrmUserCN").style.display = "block";
            //document.getElementById("msdn").style.display = "none";
            //document.getElementById("mscn").style.display = "block";
            //document.getElementById("dangkykinhdoanh").style.display = "none";
            //document.getElementById("dangkythue").style.display = "block";
            //document.getElementById('detailFileDinhKem').style.display = 'none'; // no use
        }
    };

    //mã code loại dịch vụ
    $scope.randomString = function (length) {
        const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
        let result = '';
        const charactersLength = characters.length;

        for (let i = 0; i < length; i++) {
            const randomIndex = Math.floor(Math.random() * charactersLength);
            result += characters.charAt(randomIndex);
        }

        return result;
    }
    $scope.getServicetype = function () {

        var get = $http({
            method: 'GET',
            url: apiUrl + 'api/portal/servicetype',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.Servicetype = response.data.data;

        });
    };


    $scope.changeValueServicetype = function (selectedItem) {
        service_typename = selectedItem?.name
        if (selectedItem) {
            $scope.selectedServicetype = selectedItem;
            var get = $http({
                method: 'GET',
                url: apiUrl + 'api/portal/servicetype',
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });

            get.then(function (response) {
                $scope.Servicetype = response.data.data;
                $scope.codeservicetype = response.data.data.code;
                for (var i = 0; i < response.data.data.length; i++) {
                    if (response.data.data[i].id == selectedItem.id) {
                        //$scope.code_partner = response.data.data[i].code + $scope.randomString(8);
                        //$scope.discount_rate = response.data.data[i].discount_rate;
                        //if ($scope.discount_rate == null) {
                        //    $scope.discount_rate = 0;
                        //}
                    }
                }
            });
            return selectedItem;

        } else {
            $scope.selectedServicetype = {};
        }

    };

    // lấy kinh độ vĩ độ
    $scope.getKinhdovido = function (item) {
        if ("geolocation" in navigator) {
            // Xác định vị trí

            navigator.geolocation.getCurrentPosition(function (position) {
                function showPosition(position) {

                }

                var latitude = position.coords.latitude;
                var longitude = position.coords.longitude;
                $scope.Vido = latitude;
                $scope.Kinhdo = longitude;
                //console.log("Vĩ độ: " + latitude);
                //console.log("Kinh độ: " + longitude);

                // Sử dụng dữ liệu vĩ độ và kinh độ ở đây
            });
        } else {
        }
    }

    $scope.getAddress = function () {
        console.log('getAddress');
        const path = location.pathname;
        $scope.getProvince();
        $scope.getStoretype();
        $scope.getServicetype();
        $scope.getCountries();
        $scope.Xacnhandkdt();
        $scope.getKinhdovido();
        $scope.getDiscountConfig();

        if (path === "/nhap-thong-tin-dang-ky") {
            $scope.getEmployeeData();
            $scope.submitStoreInfo();
        } else {
            $scope.datas = localStorage.getItem('PartnerInfo');
            $scope.UpdatePartner = $scope.datas ? JSON.parse($scope.datas) : {};
            if ($scope?.UpdatePartner?.province_id) $scope.getDistrict($scope.UpdatePartner.province_id);
            if ($scope?.UpdatePartner?.district_id) $scope.getWard($scope.UpdatePartner.district_id);
        }
        $scope.getListBanks();
        $scope.getListIndustries();
        //$scope.getFranchiseBrand();
        $scope.getListCompanyBranch();
    };

    // ===================== LẤY GIÁ TRỊ CHIẾT KHẤU MẶC ĐỊNH =====================
    $scope.getDiscountConfig = function () {
        $http.get(apiUrl + "api/Partner/GetDiscountConfig").then(function (res) {
            $scope.discount_min = res.data.Data.MinRefundToUsers;
            $scope.discount_max = res.data.Data.MaxRefundToUsers;
            $scope.discount_rate = res.data.Data.DefaultRefund;

            // Tạo regex pattern validate động
            $scope.discountPattern = new RegExp("^(" + $scope.discount_min + "|[1-9][0-9]|" + $scope.discount_max + ")$");
        }, function (err) {
            console.error("Không lấy được cấu hình chiết khấu:", err);
            // Nếu lỗi thì fallback mặc định
            $scope.discount_min = 10;
            $scope.discount_max = 50;
            $scope.discountPattern = new RegExp("^(1[0-9]|2[0-9]|3[0-9]|4[0-9]|50)$");
        });
    };

    $scope.initContact = function () {
        $scope.getlistProvince();
        $scope.loadProvince();
    };


    /*huy*/
    $scope.goBack = function () {
        $window.history.back();
    };

    $scope.getTinTucMoi = function () {
        $scope.getSlide();
        var urlParams = new URLSearchParams(window.location.search);
        var newsId = urlParams.get('newsId');
        var get = $http({
            method: 'GET',
            url: '/web/news/GetNews',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        $scope.baiviet = [];
        get.then(function (response) {
            $scope.dataTinTuc = response.data.data;
        });
    };

    $scope.getchitietTinTucMoi = function () {
        var urlParams = new URLSearchParams(window.location.search);
        var newsId = urlParams.get('newsId');
        var post = $http({
            method: 'GET',
            url: 'web/news/GetNewById' + newsId,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        post.then(function (response) {
            $scope.TinTuc = response.data.data;

        });
    };

    $scope.like = function () {
        var isLiked = localStorage.getItem('isLiked');
        if (isLiked) {
            alert('Bạn đã bình chọn rồi!');
            return;
        }


        $scope.likeUpdate();
    }

    $scope.likeUpdate = function () {
        var obj = $scope.register;
        obj.TypeThirdId = $scope.likeCount;
        var post = $http({
            method: 'PUT',
            url: apiUrl + 'web/customer/updateRegister/' + $scope.register.CustomerId,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                // Tăng số lượng like
                var likeCountElement = document.getElementById('likeCount');
                $scope.likeCount += 1;
                likeCountElement.textContent = $scope.likeCount;
                // Lưu trạng thái đã like vào Local Storage
                localStorage.setItem('isLiked', true);
            } else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông tin')
                    .textContent(data.meta.error_message)
                    .ok('Đóng')
                    .fullscreen(false)
            );

        }).catch(function (error) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });
    };

    $scope.handleRead = function (event) {
        event.preventDefault();
        $scope.registerPartnerDisableAgree = false;
        window.open('/dieu-khoan-va-dieu-kien-cashplus', '_blank');
    }

    //api/portal/genMerchantCode
    $scope.generateMerchantCode = function (name, provinceId) {
        let obj = {}
        if (name) obj.name = name;
        if (provinceId) obj.province_id = provinceId;
        if (name) {
            var post = $http({
                method: 'POST',
                url: apiUrl + 'api/portal/genMerchantCode',
                data: obj,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });
            post.success(function successCallback(data, status, headers, config) {
                if (data.code == 200) {
                    $scope.code_partner = data.data;
                } else {
                    //fail
                }
            }).error(function (data, status, headers, config) {
                console.log(data);
            });
        }
    }

    /*Kiểm tra đăng ký đối tác - Buoc 1*/
    $scope.SubmitRegisterPartner = function () {
        if ($scope.RegisterPartner?.name === '' || $scope.RegisterPartner?.name === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập tên cửa hàng')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterPartner?.store_owner === '' || $scope.RegisterPartner?.store_owner === undefined) {
            $scope.isOTP = true;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập tên người đại diện')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterPartner?.phone === '' || $scope.RegisterPartner?.phone === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số điện thoại hoặc số điện thoại không đúng!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterPartner?.email === '' || $scope.RegisterPartner?.email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập email')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterPartner?.store_type_id == '' || $scope.RegisterPartner?.store_type_id == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn mô hình kinh doanh!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterPartner?.address === '' || $scope.RegisterPartner?.address === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập địa chỉ')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        var obj = {
            'name': $scope.RegisterPartner.name,
            'store_owner': $scope.RegisterPartner.store_owner,
            'store_type_id': $scope.RegisterPartner.store_type_id,
            'phone': $scope.RegisterPartner.phone,
            'email': $scope.RegisterPartner.email,
            'address': $scope.RegisterPartner.address,
            'province_id': $scope.RegisterPartner.province_id,
            'district_id': $scope.RegisterPartner.district_id,
            'ward_id': $scope.RegisterPartner.ward_id,
        };
        var post = $http({
            method: 'POST',
            url: apiUrl + 'api/portal/registerStore',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        $scope.disableBtn.btSubmit = true;

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.code == 200) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Đăng Ký Đối Tác Thành Công')
                        .textContent('Mời bạn kiểm tra email ' + $scope.RegisterPartner.email + ' để hoàn thiện đăng ký và xác thực đối tác.')
                        .ok('Đóng')
                        .fullscreen(false)
                ).then(function () {
                    // Chỉ redirect sau khi user bấm nút Đóng
                    $window.location.href = '/';
                });
            } else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo Đăng Ký Đối Tác')
                        .textContent('Đã có lỗi xảy ra khi đăng ký đối tác. Xin vui lòng thử lại! ' + data.error)
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo Đăng Ký Đối Tác')
                    .textContent('Đã có lỗi xảy ra khi đăng ký đối tác. Xin vui lòng thử lại!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });
    }
    /*Xác thức mã đăng ký và email người giới thiệu - Bước 2*/
    $scope.Xacnhandkdt = function () {
        var urlParams = new URLSearchParams(window.location.search);
        $scope.logincode = urlParams.get('login_code');
        $scope.login_code = $scope.logincode;

    }
    // hàm convert chữ cái đầu viết hoa
    $scope.capitalizeWords = function (str) {
        try {
            if (typeof str === 'string') {
                return str.toLowerCase().replace(/(?:^|\s)\S/g, function (char) {
                    return char.toUpperCase();
                });
            }
            return str;
        } catch (e) {
            console.error(e);
            return str;
        }
    };

    $scope.submitStoreInfo = function () {
        localStorage.removeItem('PartnerInfo');
        var obj = {
            'login_code': $scope.login_code,
            //'support_person_email': $scope.partner?.support_person_email ? $scope.partner.support_person_email : '',
        };
        if ($scope.partner.support_person_id) obj.support_person_id = $scope.partner.support_person_id;
        var post = $http({
            method: 'POST',
            url: apiUrl + 'api/portal/getStoreInfoByCode',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.code == 200) {
                let data_data = data.data;
                //data_data.support_person_id = $scope.partner?.support_person_id ? $scope.partner.support_person_id : null;

                $scope.showFormUserDN = (data_data.store_type_id == $scope.enums.storeType.ENTERPRISE || data_data.store_type_id == $scope.enums.storeType.HOUSE_HOLD_BUSINESS) ? true : false;
                $scope.showFormUserCN = data_data.store_type_id == $scope.enums.storeType.PERSIONAL ? true : false;
                $scope.showFormUserHKD = data_data.store_type_id == $scope.enums.storeType.HOUSE_HOLD_BUSINESS ? true : false;

                $scope.UpdatePartner = data_data;
                $scope.UpdatePartner.payment_realtime1 = true;
                $scope.UpdatePartner.payment_realtime2 = true;
                $scope.UpdatePartner.account_type = 0;
                $scope.UpdatePartner.nationality = 20000;
                $scope.UpdatePartner.license_nation_id = 20000;
                if (data_data.store_type_id == $scope.enums.storeType.PERSIONAL) {
                    $scope.UpdatePartner.document_type = $scope.enums.documentType.CCCD;
                }
                $scope.UpdatePartner.representative_title = $scope.showFormUserCN ? 7 : null; //chủ hộ kinh doanh
                $scope.UpdatePartner.representative_job = $scope.showFormUserCN ? 9 : null; //khác
                $scope.UpdatePartner.bank_default = true; //khác
                //$scope.generateMerchantCode(data_data.name, data_data.province_id);
                localStorage.setItem('PartnerInfo', JSON.stringify(data_data));
                if (data_data?.province_id) $scope.getDistrict(data_data.province_id);
                if (data_data?.district_id) $scope.getWard(data_data.district_id);
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Đăng Ký Đối Tác')
                        .textContent('Chúc mừng bạn đã gửi thông tin thành công. Mời bạn tiếp tục thêm các thông tin dưới đây để hoàn thành việc đăng ký!')
                        .ok('Đóng')
                        .fullscreen(false)
                ).finally(function () {
                    let path = location.pathname;
                    if (path === "/nhap-thong-tin-dang-ky" && $scope.Kinhdo == 0 && $scope.Kinhdo == 0) {
                        $mdDialog.show(
                            $mdDialog.alert()
                                .clickOutsideToClose(false)
                                .title('Thông báo')
                                .textContent('Hãy chia sẻ vị trí thiết bị để được hiển thị chính xác vị trí trên APP')
                                .ok('Đóng')
                                .fullscreen(false)
                        );
                    } else if (path === "/xac-nhan-dang-ky-doi-tac") {
                        $window.location.href = '/nhap-thong-tin-dang-ky?login_code=' + $scope.login_code;
                    }
                });
            } else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo Đăng Ký Đối Tác')
                        .textContent('Đã có lỗi trong quá trình đăng ký đối tác. Bạn vui lòng thử lại sau. Xem chi tiết lỗi: ' + data.error)
                        .ok('Đóng')
                        .fullscreen(false)
                ).then(function () {
                    // Chỉ redirect sau khi user bấm nút Đóng
                    $window.location.href = '/';
                });
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo Đăng Ký Đối Tác')
                    .textContent('Đã có lỗi trong quá trình đăng ký đối tác. Bạn vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });
    }
    var url = {
        "DemoUrl": "https://demo.econtract.vn",
        "ProductionUrl": "https://van.econtract.vn"
    };

    // Lấy hostname của trang web hiện tại
    var currentHostname = window.location.hostname;
    var urlToCall = url.ProductionUrl
    var user = {};
    // Kiểm tra nếu hostname là "cashplus.vn" (bản prod)
    if (currentHostname === "cashplus.vn") {
        urlToCall = url.ProductionUrl;
        var user = {
            username: config?.usernameBKAV,
            password: config?.passwordBKAV
        };
    } else {
        urlToCall = url.DemoUrl;
        var user = {
            username: config?.usernameBKAVDEMO,
            password: config?.passwordBKAVDEMO
        };
    }
    // loai bo khoang cach khi nhap mst
    $scope.preventSpace = function (event) {
        if (event.which === 32) {
            event.preventDefault();
        }
    };


    // kiem tra ô option lịch làm việc 
    $scope.isOtherSelected = false;
    $scope.checkOther = function () {
        if ($scope.selectedWorkingDay === 'other') {
            $scope.isOtherSelected = true; // Hiển thị ô input tùy chỉnh
            $scope.UpdatePartner.working_day = $scope.customWorkingDay;
        } else {
            $scope.isOtherSelected = false; // Ẩn ô input tùy chỉnh nếu không chọn "Khác"
            $scope.UpdatePartner.working_day = $scope.selectedWorkingDay;
            // Đặt giá trị cho customWorkingDay bằng giá trị mặc định của option khi chọn một lựa chọn khác
            $scope.customWorkingDay = '';
        }
    };
    // lăn đến vị trí thông báo

    $scope.scrollToElementCentered = function (elementId) {
        var element = document.getElementById(elementId);
        if (element) {
            var elementRect = element.getBoundingClientRect();
            var absoluteElementTop = elementRect.top + window.pageYOffset;
            var middle = absoluteElementTop - (window.innerHeight / 2) + (elementRect.height / 2);
            window.scrollTo({ top: middle, behavior: 'smooth' });
        }
    };

    /*Điền các thông tin đăng ký đối tác chuyên sâu - Bước 3*/
    $scope.submitupdateStore = function () {
        $scope.Vido = document.getElementById("Vido").value;
        $scope.Kinhdo = document.getElementById("Kinhdo").value;
        $scope.UpdatePartner.description = $scope.bankInfo + "\n\n" + $scope.accountingInfo;
        if ($scope.UpdatePartner?.name == '' || $scope.UpdatePartner?.name == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập tên trên đăng ký kinh doanh')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-UpdatePartner-name');
            });
            ;
            return;
        }
        if ($scope.UpdatePartner?.service_type_id == '' || $scope.UpdatePartner?.service_type_id == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn loại dịch vụ')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-UpdatePartner-service-type-id');
            });
            ;
            return;
        }
        if ($scope.UpdatePartner?.email == '' || $scope.UpdatePartner?.email == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập email')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-UpdatePartner-email');
            });
            return;
        }
        if ($scope.UpdatePartner.store_type_id == '' || $scope.UpdatePartner?.store_type_id == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn mô hình dịch vụ')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-UpdatePartner-store-type-id');
            });
            return;
        }
        //if (!$scope.UpdatePartner?.license_no || $scope.UpdatePartner.license_no.length < 9 || $scope.UpdatePartner.license_no.length > 14) {
        //    $mdDialog.show(
        //        $mdDialog.alert()
        //            .clickOutsideToClose(true)
        //            .title('Thông báo')
        //            .textContent('Mã số thuế/Mã số doanh nghiệp là chuỗi ký tự số có độ dài từ 10 đến 13 ký tự ')
        //            .ok('Đóng')
        //            .fullscreen(false)
        //    ).then(function () {
        //        // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
        //        $scope.scrollToElementCentered('input-UpdatePartner-license-no');
        //    });
        //    return;
        //}      
        if ($scope.UpdatePartner.phone == '' || $scope.UpdatePartner?.phone == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số điện thoại')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-UpdatePartner-phone');
            });
            return;
        }
        if ($scope.UpdatePartner.store_owner == '' || $scope.UpdatePartner?.store_owner == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập tên người đại diện')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-UpdatePartner-store-owner');
            });
            return;
        }
        if ($scope.UpdatePartner.start_hour == '' || $scope.UpdatePartner.start_hour == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập giờ làm việc')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-UpdatePartner-start-hour');
            });
            return;
        }
        if ($scope.UpdatePartner.end_hour == '' || $scope.UpdatePartner?.end_hour == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập giờ đóng cửa')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-UpdatePartner-end-hour');
            });
            return;
        }
        if ($scope.UpdatePartner.avatar == '' || $scope.UpdatePartner?.avatar == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn ảnh đại diện để hiển thị trên App')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-UpdatePartner-avatar');
            });
            return;
        }
        if ($scope.UpdatePartner.store_type_id === 8 || $scope.UpdatePartner.store_type_id === 17) {
            if (!$scope.UpdatePartner.license_date || $scope.UpdatePartner.license_date === undefined || $scope.UpdatePartner.license_date === '') {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Bạn chưa chọn ngày đăng ký kinh doanh')
                        .ok('Đóng')
                        .fullscreen(false)
                ).then(function () {
                    // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                    $scope.scrollToElementCentered('license-date-container');
                });
                return;
            }
        }

        if ($scope.UpdatePartner.license_owner == '' || $scope.UpdatePartner?.license_owner == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập tên người sở hữu')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-UpdatePartner-license-owner');
            });
            return;
        }
        if ($scope.UpdatePartner.license_person_number = 0) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Số người đồng sở hữu phải lớn hơn 0')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.license_birth_date == '' || $scope.UpdatePartner?.license_birth_date == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn ngày sinh')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-UpdatePartner-license-birth-date');
            });
            return;
        }
        if ($scope.UpdatePartner.identifier_nation_id == '' || $scope.UpdatePartner?.identifier_nation_id == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn quốc tịch')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-UpdatePartner-identifier-nation-id');
            });
            return;
        }
        if ($scope.UpdatePartner.identifier_province_id == '' || $scope.UpdatePartner?.identifier_province_id == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn thành phố')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-UpdatePartner-identifier-province-id');
            });
            return;
        }
        if ($scope.discount_rate == '' || $scope.discount_rate == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập phần trăm chiết khấu')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-discount-rate');
            });
            return;
        }
        if ($scope.discount_rate < 10) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Phầm trăm chiết khấu cần >= 10. Mời bạn nhập lại!')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-discount-rate');
            });
            return;
        }
        if ($scope.UpdatePartner.indetifier_no == '' || $scope.UpdatePartner.indetifier_no == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số CMND/CCCD/Hộ chiếu')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-Updatepartner-indetifier-no');
            });
            return;
        }
        if ($scope.UpdatePartner.working_day == "" || $scope.UpdatePartner.indetifier_no == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn thời gian làm việc')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-Updatepartner-working-day');
            });
            return;
        }
        if ($scope.UpdatePartner.identifier_date == '' || $scope.UpdatePartner?.identifier_date == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn ngày cấp')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-Updatepartner-identifier-date');
            });
            return;
        }
        if ($scope.UpdatePartner.identifier_at === '' || $scope.UpdatePartner?.identifier_at === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập nơi cấp')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-Updatepartner-identifier-at');
            });
            return;
        }
        if ($scope.UpdatePartner.identifier_date_expire === '' || $scope.UpdatePartner?.identifier_date_expire === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn ngày hết hạn')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-Updatepartner-identifier-date-expire');
            });
            return;
        }
        $scope.containsWhiteSpace = function (str) {
            return str && str.indexOf(' ') !== -1;
        };
        if ($scope.containsWhiteSpace($scope.UpdatePartner.username)) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Tên tài khoản đăng ký không được chứa khoảng trắng hoặc ký tự có dấu')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-Updatepartner-username');
            })
            return;
        }
        if ($scope.UpdatePartner.username === '' || $scope.UpdatePartner?.username === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập tài khoản')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-Updatepartner-username');
            });
            return;
        }
        if ($scope.UpdatePartner.description === "*Thông tin tài khoản ngân hàng\nTên ngân hàng: \nSố tài khoản: \nChủ tài khoản: \n\n*Thông tin kế toán\nHọ và tên: \nSố điện thoại: \nEmail: ") {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Yêu cầu nhập đầy đủ thông tin cho tất cả các trường thông tin ngân hàng. Vd: Tên ngân hàng:ABCBank \nSố tài khoản: 098765421 \nChủ tài khoản: Nguyễn Văn A')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-Updatepartner-description');
            });
            return;

        }
        if ($scope.UpdatePartner.password === '' || $scope.UpdatePartner?.password === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mật khẩu')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-Updatepartner-password');
            });
            return;
        }
        if ($scope.UpdatePartner.identifier_address === '' || $scope.UpdatePartner?.identifier_address === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập địa chỉ thường trú')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('input-Updatepartner-identifier_address');
            });
            return;
        }
        //if ($scope.UpdatePartner.license_image == '' || $scope.UpdatePartner?.license_image == undefined) {
        //    $mdDialog.show(
        //        $mdDialog.alert()
        //            .clickOutsideToClose(true)
        //            .title('Thông báo')
        //            .textContent('Bạn chưa chọn ảnh giấy phép đăng ký kinh doanh')
        //            .ok('Đóng')
        //            .fullscreen(false)
        //    )
        //    return;
        //}
        if ($scope.UpdatePartner.identifier_front_image == '' || $scope.UpdatePartner?.identifier_front_image == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn ảnh CMND/CCCD/Hộ chiếu mặt trước')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('imgContainer1');
            });
            return;
        }

        const regex = /^[0-9]*\.?[0-9]*$/;
        if (!regex.test($scope.Kinhdo) || !regex.test($scope.Vido)) {
            console.log($scope.Kinhdo)
            console.log($scope.Vido)
            // Hiển thị thông báo nếu longitude hoặc latitude không phù hợp định dạng
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Kinh độ hoặc Vĩ độ không đúng định dạng. Vui lòng kiểm tra lại!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
            return;
        }
        if ($scope.UpdatePartner.identifier_back_image == '' || $scope.UpdatePartner?.identifier_back_image == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn ảnh CMND/CCCD/Hộ chiếu mặt sau')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                $scope.scrollToElementCentered('imgContainer2');
            });
            return;
        }


        //xóa phần này, trước trường license_date là bắt buộc nên để ngày hiện tại cho trường hợp ko có ngày dkkd
        //if ($scope.UpdatePartner.store_type_id !== undefined && $scope.UpdatePartner.store_type_id === 7) {
        //   $scope.UpdatePartner.license_date = moment().format('DD/MM/YYYY');
        //} else {
        //    $scope.UpdatePartner.license_date = $scope.UpdatePartner.license_date ? moment($scope.UpdatePartner.license_date).format('DD/MM/YYYY') : '';
        //}
        var obj = {
            'login_code': $scope.login_code,
            'code': $scope.code_partner ? $scope.code_partner : '',
            'name': $scope.UpdatePartner.name ? $scope.capitalizeWords($scope.UpdatePartner.name) : '',
            'service_type_id': $scope.UpdatePartner.service_type_id ? $scope.UpdatePartner.service_type_id : '',
            'store_type_id': $scope.UpdatePartner.store_type_id ? $scope.UpdatePartner.store_type_id : '',
            'phone': $scope.UpdatePartner.phone ? $scope.UpdatePartner.phone : '',
            'email': $scope.UpdatePartner.email ? $scope.UpdatePartner.email : '',
            'store_owner': $scope.UpdatePartner.store_owner ? $scope.capitalizeWords($scope.UpdatePartner.store_owner) : '',
            'start_hour': $scope.UpdatePartner.start_hour ? $scope.UpdatePartner.start_hour.toLocaleTimeString() : '',
            'end_hour': $scope.UpdatePartner.end_hour ? $scope.UpdatePartner.end_hour.toLocaleTimeString() : '',
            'working_day': $scope.UpdatePartner.working_day ? $scope.UpdatePartner.working_day : '',
            'username': $scope.UpdatePartner.username ? $scope.UpdatePartner.username : '',
            'password': $scope.UpdatePartner.password ? $scope.UpdatePartner.password : '',
            'description': $scope.UpdatePartner.description ? $scope.UpdatePartner.description : '',
            'product_label_id': $scope.UpdatePartner.product_label_id ? $scope.UpdatePartner.product_label_id : '',
            'discount_rate': $scope.discount_rate ? $scope.discount_rate : 0,
            'province_id': $scope.UpdatePartner.province_id ? $scope.UpdatePartner.province_id : null,
            'district_id': $scope.UpdatePartner.district_id ? $scope.UpdatePartner.district_id : null,
            'ward_id': $scope.UpdatePartner.ward_id ? $scope.UpdatePartner.ward_id : null,
            'address': $scope.UpdatePartner.address ? $scope.capitalizeWords($scope.UpdatePartner.address) : null,
            'latitude': $scope.Vido ? $scope.Vido : null,
            'longtitude': $scope.Kinhdo ? $scope.Kinhdo : null,
            'license_no': $scope.UpdatePartner.license_no,
            'license_person_number': $scope.UpdatePartner.license_person_number ? $scope.UpdatePartner.license_person_number : 0,
            'license_image': $scope.UpdatePartner.license_image,
            'license_date': $scope.UpdatePartner.license_date,
            'license_owner': $scope.UpdatePartner.license_owner ? $scope.UpdatePartner.license_owner : '',
            'license_birth_date': $scope.UpdatePartner.license_birth_date ? moment($scope.UpdatePartner.license_birth_date).format('DD/MM/YYYY') : '',
            'license_nation_id': $scope.UpdatePartner.license_nation_id ? $scope.UpdatePartner.license_nation_id : null,
            'indetifier_no': $scope.UpdatePartner.indetifier_no ? $scope.UpdatePartner.indetifier_no : 0,
            'identifier_date': $scope.UpdatePartner.identifier_date ? moment($scope.UpdatePartner.identifier_date).format('DD/MM/YYYY') : '',
            'identifier_at': $scope.UpdatePartner.identifier_at ? $scope.UpdatePartner.identifier_at : '',
            'identifier_date_expire': $scope.UpdatePartner.identifier_date_expire ? moment($scope.UpdatePartner.identifier_date_expire).format('DD/MM/YYYY') : '',
            'identifier_address': $scope.UpdatePartner.identifier_address ? $scope.UpdatePartner.identifier_address : '',
            'identifier_nation_id': $scope.UpdatePartner.identifier_nation_id ? $scope.UpdatePartner.identifier_nation_id : null,
            'identifier_province_id': $scope.UpdatePartner.identifier_province_id ? $scope.UpdatePartner.identifier_province_id : null,
            'is_same_address': $scope.UpdatePartner.is_same_address ? $scope.UpdatePartner.is_same_address : false,
            'now_address': $scope.UpdatePartner.now_address ? $scope.UpdatePartner.now_address : '',
            'now_province_id': $scope.UpdatePartner.now_province_id ? $scope.UpdatePartner.now_province_id : null,
            'identifier_front_image': $scope.UpdatePartner.identifier_front_image ? $scope.UpdatePartner.identifier_front_image : '',
            'identifier_back_image': $scope.UpdatePartner.identifier_back_image ? $scope.UpdatePartner.identifier_back_image : '',
            'avatar': $scope.UpdatePartner.avatar ? $scope.UpdatePartner.avatar : '',
            'list_documents': $scope.UpdatePartner.list_documents ? $scope.UpdatePartner.list_documents : '',
            'now_address': $scope.UpdatePartner.now_address ? $scope.UpdatePartner.now_address : '',
            'now_nation_id': $scope.UpdatePartner.now_nation_id ? $scope.UpdatePartner.now_nation_id : '',
            'now_province_id': $scope.UpdatePartner.now_province_id ? $scope.UpdatePartner.now_province_id : '',
            'support_person_id': $scope.UpdatePartner.support_person_id
        };

        console.log(obj)
        var post = $http({
            method: 'POST',
            url: apiUrl + 'api/portal/updateInfoStore',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        $scope.checkloading = true
        $scope.disableBtn.btSubmit = true;

        post.success(function successCallback(data, status, headers, config) {

            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.code == 200) {
                var post = $http({
                    method: 'POST',
                    url: urlToCall + '/api/Integrated/Login',
                    data: user
                });

                post.then(function successCallback(response) {
                    var currentDate = new Date();
                    var day = currentDate.getDate().toString().padStart(2, '0');
                    var month = (currentDate.getMonth() + 1).toString().padStart(2, '0');

                    // Tách chuỗi thành các dòng
                    var lines = $scope.UpdatePartner.description.split('\n');
                    var bankName = '';
                    var accountNumber = '';
                    var accountantName = '';
                    var chutk = '';
                    // Lặp qua từng dòng và tìm các thông tin cần lấy
                    lines.forEach(function (line) {
                        // Kiểm tra nếu dòng chứa thông tin về tên ngân hàng
                        if (line.includes('Tên ngân hàng:')) {
                            bankName = line.split(':')[1].trim();
                        }
                        // Kiểm tra nếu dòng chứa thông tin về số tài khoản ngân hàng
                        else if (line.includes('Số tài khoản:')) {
                            accountNumber = line.split(':')[1].trim();
                        } else if (line.includes('Chủ tài khoản:')) {
                            chutk = line.split(':')[1].trim();
                        }
                        // Kiểm tra nếu dòng chứa thông tin về họ tên kế toán
                        else if (line.includes('Họ và tên:')) {
                            accountantName = line.split(':')[1].trim();
                        }
                    });
                    bankName = $scope.removeAccents(bankName);
                    chutk = $scope.removeAccents(chutk);


                    // tao thong tin ho so
                    var dataHS = {}
                    console.log("dv", service_typename)
                    if ($scope?.UpdatePartner?.store_type_id === 7) {

                        var ngaycapCCCD = $scope.reverseDateA($scope.UpdatePartner.identifier_date)
                        // tao hop dong ca nhan
                        dataHS = {
                            "ProfileIDPartner": $scope?.code_partner,
                            "ProfileTypeCode": "HD3B-002",
                            "ProfileStatusID": 1,
                            "SignDeadline": null,
                            "LtText": [
                                {
                                    "TextCode": "HD3B-HDCN",
                                    "Data": {
                                        "ngay": day,
                                        "thang": month,
                                        "maMrc": null,
                                        "TENNGUOIDAIDIEN": $scope?.UpdatePartner?.license_owner,
                                        "diachi": $scope?.UpdatePartner?.address,
                                        "sdt": $scope?.UpdatePartner?.phone,
                                        "masothue": $scope?.UpdatePartner?.license_no,
                                        "CCCD": $scope?.UpdatePartner?.indetifier_no,
                                        "ngaycapCCCD": ngaycapCCCD,
                                        "email": $scope?.UpdatePartner?.email,
                                    }
                                },
                                {
                                    "TextCode": "PL_001CN",
                                    "Data": {
                                        "ngay": day,
                                        "thang": month,
                                        "maMrc": null,
                                        "tenchutaikhoan": chutk,
                                        "dichvu": service_typename,
                                        "sotaikhoan": accountNumber,
                                        "tennganhang": bankName,
                                        "TENNGUOIDAIDIEN": $scope?.UpdatePartner?.license_owner,
                                    }
                                },
                                {
                                    "TextCode": "PL-002",
                                    "Data": {
                                        "ngay": day,
                                        "thang": month,
                                        "maMrc": null,
                                        "tenchutaikhoan": chutk,
                                        "dichvu": service_typename,
                                        "sotaikhoan": accountNumber,
                                        "tennganhang": bankName,
                                        "TENNGUOIDAIDIEN": $scope?.UpdatePartner?.license_owner,
                                        "chucvu": null,

                                    }
                                },
                                {
                                    "TextCode": "PL-003",
                                    "Data": {
                                        "ngay": day,
                                        "thang": month,
                                        "maMrc": null,
                                        "TENNGUOIDAIDIEN": $scope?.UpdatePartner?.license_owner,
                                        "chucvu": null
                                    }
                                },
                                {
                                    "TextCode": "PL-004",
                                    "Data": {
                                        "ngay": day,
                                        "thang": month,
                                        "maMrc": null,
                                        "TENNGUOIDAIDIEN": $scope?.UpdatePartner?.license_owner,
                                        "chucvu": null
                                    }
                                }
                            ],
                            "LtSigner": [
                                {
                                    "SignerCode": "ĐTK1",
                                    "SignerName": $scope?.UpdatePartner?.license_owner,
                                    "IdentifyCode": $scope?.UpdatePartner?.indetifier_no,
                                    "Email": $scope?.UpdatePartner?.email,
                                    "PhoneNumber": $scope?.UpdatePartner?.phone,
                                    "Address": $scope?.UpdatePartner?.now_address
                                }
                            ]
                        }
                    } else {
                        //tao hop dong doanh nghiep
                        var companyName = ""
                        if (DKKDname !== "" && DKKDname !== "N/A") {
                            companyName = DKKDname
                        } else {
                            companyName = $scope?.UpdatePartner?.name
                        }
                        dataHS = {
                            "ProfileIDPartner": $scope?.code_partner,
                            "ProfileTypeCode": "HD3B-001",
                            "ProfileStatusID": 1,
                            "SignDeadline": null,
                            "LtText": [
                                {
                                    "TextCode": "HD3B-HD",
                                    "Data": {
                                        "ngay": day,
                                        "thang": month,
                                        "maMrc": null,
                                        "TENNGUOIDAIDIEN": $scope?.UpdatePartner?.license_owner,
                                        "TENCONGTY": companyName,
                                        "diachi": $scope?.UpdatePartner?.address,
                                        "sdt": $scope?.UpdatePartner?.phone,
                                        "masothue": $scope?.UpdatePartner?.license_no,
                                        "chucvu": null,
                                        "chucvukhaibao": null,
                                        "email": $scope?.UpdatePartner?.email,
                                        "chucvu": null,

                                    }
                                },
                                {
                                    "TextCode": "PL_001CN",
                                    "Data": {
                                        "ngay": day,
                                        "thang": month,
                                        "maMrc": null,
                                        "tenchutaikhoan": chutk,
                                        "dichvu": service_typename,
                                        "sotaikhoan": accountNumber,
                                        "tennganhang": bankName,
                                        "TENNGUOIDAIDIEN": $scope?.UpdatePartner?.license_owner,
                                        "chucvu": null,
                                        "dichvu": service_typename

                                    }
                                },
                                {
                                    "TextCode": "PL-002",
                                    "Data": {
                                        "ngay": day,
                                        "thang": month,
                                        "maMrc": null,
                                        "tenchutaikhoan": chutk,
                                        "sotaikhoan": accountNumber,
                                        "tennganhang": bankName,
                                        "TENNGUOIDAIDIEN": $scope?.UpdatePartner?.license_owner,
                                        "dichvu": service_typename,
                                        "chucvu": null,
                                        "dichvu": service_typename
                                    }
                                },
                                {
                                    "TextCode": "PL-003",
                                    "Data": {
                                        "ngay": day,
                                        "thang": month,
                                        "maMrc": null,
                                        "TENNGUOIDAIDIEN": $scope?.UpdatePartner?.license_owner,
                                        "chucvu": null
                                    }
                                },
                                {
                                    "TextCode": "PL-004",
                                    "Data": {
                                        "ngay": day,
                                        "thang": month,
                                        "maMrc": null,
                                        "TENNGUOIDAIDIEN": $scope?.UpdatePartner?.license_owner,
                                        "chucvu": null
                                    }
                                }
                            ],
                            "LtSigner": [
                                {
                                    "SignerCode": "ĐTK1",
                                    "SignerName": $scope?.UpdatePartner?.license_owner,
                                    "IdentifyCode": $scope?.UpdatePartner?.indetifier_no,
                                    "Email": $scope?.UpdatePartner?.email,
                                    "PhoneNumber": $scope?.UpdatePartner?.phone,
                                    "Address": $scope?.UpdatePartner?.now_address
                                }
                            ]
                        }
                    }

                    var currentHostname = window.location.hostname;
                    var urlToCall = url
                    // Kiểm tra nếu hostname là "cashplus.vn" (bản prod)
                    if (currentHostname === "cashplus.vn") {
                        urlToCall = url.ProductionUrl;
                    } else {
                        urlToCall = url.DemoUrl;
                    }
                    var post = $http({
                        method: 'POST',
                        url: urlToCall + '/api/Integrated/CreateByDataObject',
                        data: dataHS,
                        headers: {
                            "Authorization": response?.data?.object?.token
                        }
                    });
                    post.then(function successCallback(response) {
                        if (response.data.isOk === true) {
                            $scope.checkloading = false; // Ẩn hiệu ứng loading
                            $mdDialog.show(
                                $mdDialog.alert()
                                    .clickOutsideToClose(true)
                                    .title('Đăng Ký Đối Tác Thành Công')
                                    .textContent('Chúc mừng bạn đã đăng ký đối tác thành công. Trong thời gian sớm nhất, bộ phận chuyên môn của CashPlus sẽ liên hệ hỗ trợ bạn hoàn thiện hợp đồng!')
                                    .ok('Đóng')
                                    .fullscreen(false)
                            ).finally(function () {
                                $window.location.href = '/'
                            });
                        } else {
                            $scope.checkloading = false; // Ẩn hiệu ứng loading
                            $mdDialog.show(
                                $mdDialog.alert()
                                    .clickOutsideToClose(true)
                                    .title('Đăng Ký Đối Tác Thành Công')
                                    .textContent('Chúc mừng bạn đã đăng ký đối tác thành công. Trong thời gian sớm nhất, bộ phận chuyên môn của CashPlus sẽ liên hệ hỗ trợ bạn hoàn thiện hợp đồng!')
                                    .ok('Đóng')
                                    .fullscreen(false)
                            )

                            // Sử dụng EmailJS để gửi email
                            var templateParams = {
                                to_email: 'kythuat@cashplus.vn', // Địa chỉ email của người nhận
                                subject: 'Kết quả đăng ký đối tác ' + $scope.UpdatePartner.name,
                                message: JSON.stringify(response)
                            };
                            emailjs.init("J9Mf2j-W7Fb_kP3Sl")
                            emailjs.send('service_kpjs45o', 'template_pw6ghov', templateParams)
                                .then(function (response) {
                                    console.log('Email sent successfully');
                                }, function (error) {
                                    console.error('Email sending failed:', error);
                                });
                        }

                    }).catch(function errorCallback(e) {
                        $scope.checkloading = false; // Ẩn hiệu ứng loading
                        cfpLoadingBar.complete();

                    });

                }).catch(function errorCallback(e) {
                    $scope.checkloading = false; // Ẩn hiệu ứng loading
                    cfpLoadingBar.complete();
                });

            } else {
                $scope.checkloading = false; // Ẩn hiệu ứng loading
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi khi thiết lập hồ sơ thông tin đối tác. Xin vui lòng thử lại sau! ' + data.error)
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.checkloading = false; // Biến để kiểm soát hiển thị hiệu ứng loading
            $scope.checkloading = false; // Ẩn hiệu ứng loading
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.error === "Tên tài khoản đã tồn tại trên hệ thống!!") {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Tên tài khoản đã tồn tại. Xin vui lòng thử lại!')
                        .ok('Đóng')
                        .fullscreen(false)
                ).then(function () {
                    // Cuộn đến phần tử chứa ô dữ liệu chưa được nhập
                    $scope.scrollToElementCentered('input-Updatepartner-username');
                });
                return
            } else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi khi cập nhật thông tin đối tác. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        });
    }


    $scope.handleCheckBankAccount = function (bankCode, accountNumber, isForm) {
        let obj = {
            bankBin: bankCode,
            bankNumber: accountNumber,
        }
        var post = $http({
            method: 'POST',
            url: apiUrl + 'api/store/bankaccount/get-bank-owner',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        $scope.checkloading = true
        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.code == 200) {
                $scope.checkloading = false; // on loading
                //fill acc_holder
                if (isForm) $scope.UpdatePartner.acc_holder = data?.data ? data.data : '';
                else {
                    $scope.bankAccount.acc_numb = accountNumber;
                    $scope.bankAccount.bank_code = bankCode;
                    $scope.bankAccount.acc_holder = data?.data ? data.data : '';
                }
            }
            $scope.checkloading = false;
            /*else {
                $scope.checkloading = false; // off loading
                if(isForm) $scope.UpdatePartner.acc_holder = ''
                $scope.bankAccount.acc_holder = '';
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(false)
                        .title('Xác thực tài khoản')
                        .textContent('Tài khoản ngân hàng không hợp lệ')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }*/
        }).error(function (data, status, headers, config) {
            $scope.checkloading = false; // Biến để kiểm soát hiển thị hiệu ứng loading
            $scope.checkloading = false; // Ẩn hiệu ứng loading
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(false)
                    .title('Thông báo')
                    .textContent('Xác thực tài khoản không thành công!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });
    }

    $scope.getListAddress = function () {
        return [
            {
                operation: 0,//0: Tạo mới | 1: Chỉnh sửa
                address: $scope.showFormUserCN ? $scope.UpdatePartner.address : $scope.UpdatePartner.license_address
            }
        ]
    }

    $scope.getListEmail = function () {
        return [
            {
                operation: 0,//0: Tạo mới | 1: Chỉnh sửa
                email: $scope.UpdatePartner.email
            }
        ]
    }

    $scope.getListPhone = function () {
        return [
            {
                operation: 0,//0: Tạo mới | 1: Chỉnh sửa
                phone_numb: $scope.UpdatePartner.phone
            }
        ]
    }

    $scope.getListWebsite = function () {
        return [
            {
                operation: 0,//0: Tạo mới | 1: Chỉnh sửa
                type: 1,//1: Website | 2: Nền tảng
                website: $scope.UpdatePartner.website
            }
        ]
    }

    $scope.getListContact = function () {
        if ($scope.contacts) {
            let arr = [];
            $scope.contacts.map(item => {
                let obj = {
                    operation: 0, //0: Tạo mới | 1: Chỉnh sửa
                    type: item.type,
                    name: item.name,
                    phone: item.phone,
                    email: item.email
                }
                arr.push(obj);
            });
            return arr;
        }
        return [];
    }

    $scope.getListWorkingTime = function () {
        if ($scope.workingTimes) {
            let arr = [];
            $scope.workingTimes.map(item => {
                let obj = {
                    operation: 0, //0: Tạo mới | 1: Chỉnh sửa
                    start_hour: item.start_hour,
                    end_hour: item.end_hour,
                }
                arr.push(obj);
            });
            return arr;
        }
        return [];
    }

    $scope.getListRepresentativeInfoBr = function () {
        return [
            {
                operation: 0,//0: Tạo mới | 1: Chỉnh sửa
                representative_name: $scope.UpdatePartner.license_owner,//Họ tên người đại diện
                representative_title: $scope.UpdatePartner.representative_title,//Chức danh người đại diện
                gender: $scope.UpdatePartner.gender, //Giới tính MALE: Nam | FEMALE: Nữ | OTHER: Khác
                dob: $scope.UpdatePartner.license_birth_date ? moment($scope.UpdatePartner.license_birth_date).format('YYYY-MM-DD') : '',//Ngày sinh, format Y-m-d
                nation: $scope.UpdatePartner.nation,//Dân tộc
                nationality: $scope.UpdatePartner.nationality,//Quốc tịch
                document_type: $scope.UpdatePartner.document_type,//Loại giấy tờ | 1: CCCD | 2: CMND | 3: Hộ chiếu | 4: Thị thực
                id_number: $scope.UpdatePartner.indetifier_no,//Số giấy tờ
                issued_date: $scope.UpdatePartner.identifier_date ? moment($scope.UpdatePartner.identifier_date).format('YYYY-MM-DD') : '',//Ngày cấp, format Y-m-d
                issued_place: $scope.UpdatePartner.identifier_at,//Nơi cấp
                valid_thru: $scope.UpdatePartner.identifier_date_expire ? moment($scope.UpdatePartner.identifier_date_expire).format('YYYY-MM-DD') : '',//Giá trị đến ngày, format Y-m-d
                current_address: $scope.UpdatePartner.current_address//Địa chỉ hiện tại
            },
        ];
    }

    $scope.getListRepresentativeInfoId = function () {
        return [
            {
                operation: 0,//0: Tạo mới | 1: Chỉnh sửa
                representative_name: $scope.UpdatePartner.license_owner,//Họ tên người đại diện
                representative_title: $scope.UpdatePartner.representative_title,//Chức danh người đại diện
                representative_job: $scope.UpdatePartner.representative_job,//Nghề nghiệp người đại diện
                gender: $scope.UpdatePartner.gender, //Giới tính MALE: Nam | FEMALE: Nữ | OTHER: Khác
                dob: $scope.UpdatePartner.license_birth_date ? moment($scope.UpdatePartner.license_birth_date).format('YYYY-MM-DD') : '',//Ngày sinh, format Y-m-d
                hometown: $scope.UpdatePartner.hometown,//Quê quán
                permanent_address: $scope.UpdatePartner.identifier_address,//Địa chỉ thường trú
                nation: $scope.UpdatePartner.nation,//Dân tộc
                nationality: $scope.UpdatePartner.nationality,//Quốc tịch
                document_type: $scope.UpdatePartner.document_type,//Loại giấy tờ | 1: CCCD | 2: CMND | 3: Hộ chiếu | 4: Thị thực
                id_number: $scope.UpdatePartner.indetifier_no,//Số giấy tờ
                issued_date: $scope.UpdatePartner.identifier_date ? moment($scope.UpdatePartner.identifier_date).format('YYYY-MM-DD') : '',//Ngày cấp, format Y-m-d
                issued_place: $scope.UpdatePartner.identifier_at,//Nơi cấp
                valid_thru: $scope.UpdatePartner.identifier_date_expire ? moment($scope.UpdatePartner.identifier_date_expire).format('YYYY-MM-DD') : '',//Giá trị đến ngày, format Y-m-d
                religion: $scope.UpdatePartner.religion,// Tôn giáo
                current_address: $scope.UpdatePartner.current_address,//Địa chỉ hiện tại
                place_of_birth: $scope.UpdatePartner.place_of_birth ? $scope.UpdatePartner.place_of_birth : '',// Nơi sinh
                identifiers: $scope.UpdatePartner.identifiers,// Đặc điểm nhận dạng
                citizen_numb: $scope.UpdatePartner.citizen_numb,// Mã số công dân (CCCD/CMND)
                citizen_card_front: $scope.UpdatePartner.identifier_front_image,// Link mặt trước CCCD/ CMND/ HC //$scope.UpdatePartner.identifier_front_image
                citizen_card_back: $scope.UpdatePartner.identifier_back_image,// Link mặt sau CCCD/ CMND/ HC //$scope.UpdatePartner.identifier_back_image
                profile_picture: $scope.UpdatePartner.profile_picture,// Link ảnh chân dung người đại diện
            }
        ];
    }

    $scope.getListAccNo = function () {
        let arr = [];
        if ($scope.bankAccountInput) {
            $scope.bankAccountInput.map((item) => {
                let obj = {
                    operation: 1,
                    is_default: item.is_default,
                    status: item.status,
                    acc_numb: item.acc_numb,
                    bank_code: item.bank_code,
                    acc_holder: item.acc_holder,
                    type: item.type,
                    auth_doc_numb: item?.auth_doc_numb ? item.auth_doc_numb : '',
                    auth_doc_link: item?.auth_doc_link ? item.auth_doc_link : '',
                }
                arr.push(obj);
            });
            return arr;
        }
        return arr;
    }

    $scope.getListBranchAccNo = function () {
        if ($scope.subMerchants) {
            let arr = [];
            $scope.subMerchants.map(item => {
                let obj = {
                    operation: 1, //1: Tạo mới | 2: Chỉnh sửa
                    status: item.status,//Trạng thái: 1: Active | 2: Deactive
                    branch_id: item.branch_id,//id cơ sở
                    branch_name: item.branch_name,//Tên cơ sở
                    branch_address: item.branch_address,//Địa chỉ cơ sở
                    acc_numb: item.acc_numb,//Số tài khoản
                    bank_code: item.bank_code,//Mã ngân hàng
                    acc_holder: item.acc_holder,//Tên chủ tài khoản
                    branch_rep_name: item.branch_rep_name,
                    branch_rep_yob: item.branch_rep_yob ? moment(item.branch_rep_yob).format('DD/MM/YYYY') : '',
                    branch_rep_tax_no: item.branch_rep_tax_no,
                    branch_rep_identity_no: item.branch_rep_identity_no,
                    branch_rep_identity_issue_date: item.branch_rep_identity_issue_date ? moment(item.branch_rep_identity_issue_date).format('DD/MM/YYYY') : '',
                    branch_rep_identity_issue_place: item.branch_rep_identity_issue_place,
                    branch_rep_permanent_residence: item.branch_rep_permanent_residence,
                    cashplus_account_name: item.branch_name,
                    //discount_rate: item.discount_rate,
                }
                arr.push(obj);
            });
            return arr;
        }
        return [];
    }

    $scope.getListOtherDocument = function () {
        if ($scope.otherDocuments) {
            let arr = [];
            $scope.otherDocuments.map(item => {
                let obj = {
                    operation: 1, //1: Tạo mới | 2: Chỉnh sửa
                    name: item.name,
                    link: item.link
                }
                arr.push(obj);
            });
            return arr;
        }
        return [];
    }

    $scope.getListDocuments = function () {
        if ($scope.otherDocuments) {
            let arr = [];
            $scope.otherDocuments.map(item => {
                let obj = {
                    name: item.name,
                    link: item.link
                }
                arr.push(obj);
            });
            return arr;
        }
        return [];
    }

    $scope.validateBankAccountCN = function (isMain = true) {
        let name = $scope.toUpperCaseAndRemoveAccents($scope.UpdatePartner.license_owner ? $scope.UpdatePartner.license_owner : $scope.UpdatePartner.store_owner);
        let accountName = isMain ? $scope.toUpperCaseAndRemoveAccents($scope.UpdatePartner.acc_holder) : $scope.toUpperCaseAndRemoveAccents($scope.bankAccount.acc_holder);
        let result = $scope.compareNames(name, accountName);
        if (result === 1 && isMain) $scope.UpdatePartner.account_type = 0;//chinh chu
        if (result === 1 && !isMain) $scope.bankAccount.type = 0;//chinh chu
        if (result === 2 && isMain) $scope.UpdatePartner.account_type = 1;//uy quyen
        if (result === 2 && !isMain) $scope.bankAccount.type = 1;//uy quyen
        if (result === 3 && name) {
            //if(!isMain) $scope.bankAccountInvalid = true;
            /*$mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Số tài khoản không khớp với thông tin người đại diện.')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                if(isMain && !$scope.showFormUserHKD) $scope.UpdatePartner.account_type = null;
                if(!isMain) $scope.bankAccount.type = null;
                $scope.scrollToElementCentered('bank_account_list');
            });
            return;*/
        }
    }

    $scope.validateBankAccountDN = function (isMain = true) {
        let companyName = $scope.toUpperCaseAndRemoveAccents($scope.UpdatePartner.company_name);
        let companyShortName = $scope.toUpperCaseAndRemoveAccents($scope.UpdatePartner.company_short_name);
        let accountName = isMain ? $scope.toUpperCaseAndRemoveAccents($scope.UpdatePartner.acc_holder) : $scope.toUpperCaseAndRemoveAccents($scope.bankAccount.acc_holder);

        // chính chủ
        if (companyName == accountName) {
            if (isMain) $scope.UpdatePartner.account_type = 0;
            if (!isMain) $scope.bankAccount.type = 0;
        } else if (companyShortName == accountName) { // viết tắt
            if (isMain) $scope.UpdatePartner.account_type = 2;
            if (!isMain) $scope.bankAccount.type = 2;
        } else { // ủy quyền
            if (isMain) $scope.UpdatePartner.account_type = 1;
            if (!isMain) $scope.bankAccount.type = 1;
        }
    }

    $scope.validateBankAccountHKD = function (isMain = true) {
        let companyName = $scope.toUpperCaseAndRemoveAccents($scope.UpdatePartner.company_name);
        let licenseOwner = $scope.toUpperCaseAndRemoveAccents($scope.UpdatePartner.license_owner);
        let accountName = isMain ? $scope.toUpperCaseAndRemoveAccents($scope.UpdatePartner.acc_holder) : $scope.toUpperCaseAndRemoveAccents($scope.bankAccount.acc_holder);

        // chính chủ
        if (companyName == accountName) {
            if (isMain) $scope.UpdatePartner.account_type = 0;
            if (!isMain) $scope.bankAccount.type = 0;
        } else if (licenseOwner == accountName) { // viết tắt
            if (isMain) $scope.UpdatePartner.account_type = 2;
            if (!isMain) $scope.bankAccount.type = 2;
        } else { // ủy quyền
            if (isMain) $scope.UpdatePartner.account_type = 1;
            if (!isMain) $scope.bankAccount.type = 1;
        }
    }

    $scope.compareNames = function (name, accountName) {
        let nameParts = name.trim().split(' ');
        let accountNameParts = accountName.trim().split(' ');

        let firstName = nameParts[nameParts.length - 1];
        let lastName = nameParts[0];

        let accountFirstName = accountNameParts[accountNameParts.length - 1];
        let accountLastName = accountNameParts[0];

        if (name === accountName) {
            console.log('TH1: Cả họ và tên trùng nhau')
            return 1;
        }

        if (firstName === accountFirstName && lastName === accountLastName) {
            console.log('TH2: Họ và tên trùng nhau, nhưng name và accountName khác nhau')
            return 2;
        }
        console.log('TH3: Không trùng hợp');
        return 3;
    }

    $scope.checkBankAccountDN = function () {
        if ($scope.UpdatePartner.account_type == $scope.enums.accountType.CHINH_CHU
            && $scope.toUpperCaseAndRemoveAccents($scope.UpdatePartner.company_name) != $scope.toUpperCaseAndRemoveAccents($scope.UpdatePartner.acc_holder)
        ) {
            return false;
        }
        return true;
    }

    $scope.updateInfoMerchant = function () {
        $scope.Vido = document.getElementById("Vido").value;
        $scope.Kinhdo = document.getElementById("Kinhdo").value;
        //$scope.UpdatePartner.description = $scope.bankInfo + "\n\n" + $scope.accountingInfo;
        if ($scope.formPartnerUpdate.$valid) {
            // Form is valid, perform submission
            if ($scope.UpdatePartner.store_type_id != $scope.enums.storeType.PERSIONAL && !$scope.UpdatePartner.business_registration_certificate_front) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Vui lòng tải mặt trước lên giấy phép kinh doanh.')
                        .ok('Đóng')
                        .fullscreen(false)
                ).then(function () {
                    $scope.scrollToElementCentered('business_registration_front');
                });
                return;
            }
            if ($scope.UpdatePartner.settlement_by_branch == $scope.enums.settlementByBranch.QT_VE_CO_SO && $scope.subMerchants.length === 0) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Vui lòng thêm cơ sở khi quyết toán về nhiều cơ sở.')
                        .ok('Đóng')
                        .fullscreen(false)
                ).then(function () {
                    $scope.scrollToElementCentered('settlement_by_branch');
                });
                return;
            }

            /*if ($scope.UpdatePartner.store_type_id != $scope.enums.storeType.PERSIONAL && !$scope.checkBankAccountDN()) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Khi chọn loại tài khoản "Chính chủ". Tên tài khoản phải trùng với tên doanh nghiệp/hộ kinh doanh')
                        .ok('Đóng')
                        .fullscreen(false)
                ).then(function () {
                    $scope.scrollToElementCentered('acc_holder');
                });
                return;
            }*/

            /*if ($scope.UpdatePartner.store_type_id == $scope.enums.storeType.PERSIONAL) {
                let name = $scope.toUpperCaseAndRemoveAccents($scope.UpdatePartner.license_owner);
                let accountName = $scope.toUpperCaseAndRemoveAccents($scope.UpdatePartner.acc_holder);
                let result = $scope.compareNames(name, accountName);
                if(result === 3) {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông báo')
                            .textContent('Số tài khoản không khớp với thông tin người đại diện.')
                            .ok('Đóng')
                            .fullscreen(false)
                    ).then(function () {
                        $scope.UpdatePartner.account_type = null;
                        $scope.scrollToElementCentered('bank_account_list');
                    });
                    return;
                }
            }*/

            if ($scope.contacts && $scope.contacts.length < 2) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Vui lòng thêm danh sách liên hệ. Hãy thêm đủ 2 loại liên hệ.')
                        .ok('Đóng')
                        .fullscreen(false)
                ).then(function () {
                    $scope.scrollToElementCentered('list_contact');
                });
                return;
            }

            if ($scope.workingTimes && $scope.workingTimes.length === 0) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Vui lòng thêm giờ đóng mở cửa.')
                        .ok('Đóng')
                        .fullscreen(false)
                ).then(function () {
                    $scope.scrollToElementCentered('list_working_time');
                });
                return;
            }

            if ($scope.otherDocuments && $scope.otherDocuments.length === 0) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Vui lòng thêm hình ảnh cửa hàng và các giấy tờ khác liên quan.')
                        .ok('Đóng')
                        .fullscreen(false)
                ).then(function () {
                    if ($scope.showFormUserCN) $scope.scrollToElementCentered('cn_list_other_documents');
                    else $scope.scrollToElementCentered('list_other_documents');
                });
                return;
            } else {
                let fileNames = $scope.otherDocuments.map(item => item.name);
                if (!fileNames.includes("Hình ảnh cửa hàng")) {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông báo')
                            .textContent('Vui lòng thêm hình ảnh cửa hàng.')
                            .ok('Đóng')
                            .fullscreen(false)
                    ).then(function () {
                        if ($scope.showFormUserCN) $scope.scrollToElementCentered('cn_other_document');
                        else $scope.scrollToElementCentered('list_other_documents');
                    });
                    return;
                }
                if (!fileNames.includes("Menu cửa hàng")) {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông báo')
                            .textContent('Vui lòng thêm menu cửa hàng.')
                            .ok('Đóng')
                            .fullscreen(false)
                    ).then(function () {
                        if ($scope.showFormUserCN) $scope.scrollToElementCentered('cn_other_document');
                        else $scope.scrollToElementCentered('list_other_documents');
                    });
                    return;
                }
            }
            console.log('Form submitted successfully');

            var obj = {
                'operation': 1,
                'login_code': $scope.login_code,
                'code': $scope.code_partner ? $scope.code_partner : '',
                'name': $scope.UpdatePartner.name ? $scope.capitalizeWords($scope.UpdatePartner.name) : '',
                'partner_id': $scope.UpdatePartner.id ? $scope.UpdatePartner.id : '',
                'service_type_id': $scope.UpdatePartner.service_type_id ? $scope.UpdatePartner.service_type_id : '',
                'store_type_id': $scope.UpdatePartner.store_type_id ? $scope.UpdatePartner.store_type_id : '',
                'phone': $scope.UpdatePartner.phone ? $scope.UpdatePartner.phone : '',
                'email': $scope.UpdatePartner.email ? $scope.UpdatePartner.email : '',
                'store_owner': $scope.UpdatePartner.store_owner ? $scope.capitalizeWords($scope.UpdatePartner.store_owner) : '',
                'start_hour': $scope.UpdatePartner.start_hour ? $scope.UpdatePartner.start_hour : '',
                'end_hour': $scope.UpdatePartner.end_hour ? $scope.UpdatePartner.end_hour : '',
                'working_day': $scope.UpdatePartner.working_day ? $scope.UpdatePartner.working_day : '',
                'username': $scope.UpdatePartner.username ? $scope.UpdatePartner.username : '',
                'password': $scope.UpdatePartner.password ? $scope.UpdatePartner.password : '',
                'description': $scope.description ? $scope.description : '',
                'product_label_id': $scope.UpdatePartner.product_label_id ? $scope.UpdatePartner.product_label_id : '',
                'discount_rate': $scope.discount_rate ? $scope.discount_rate : 0,
                'province_id': $scope.UpdatePartner.province_id ? $scope.UpdatePartner.province_id : null,
                'district_id': $scope.UpdatePartner.district_id ? $scope.UpdatePartner.district_id : null,
                'ward_id': $scope.UpdatePartner.ward_id ? $scope.UpdatePartner.ward_id : null,
                'address': $scope.UpdatePartner.address ? $scope.capitalizeWords($scope.UpdatePartner.address) : null,
                'latitude': $scope.Vido ? $scope.Vido : null,
                'longtitude': $scope.Kinhdo ? $scope.Kinhdo : null,
                'license_no': $scope.UpdatePartner.license_no,
                'license_person_number': $scope.UpdatePartner.license_person_number ? $scope.UpdatePartner.license_person_number : 0,
                'license_image': $scope.UpdatePartner.license_image,
                'license_date': $scope.UpdatePartner.store_type_id === $scope.enums.storeType.PERSIONAL && $scope.UpdatePartner.license_date ? moment($scope.UpdatePartner.license_date).format('DD/MM/YYYY') : moment().format('DD/MM/YYYY'),
                'license_owner': $scope.UpdatePartner.license_owner ? $scope.UpdatePartner.license_owner : '',
                'license_birth_date': $scope.UpdatePartner.license_birth_date ? moment($scope.UpdatePartner.license_birth_date).format('DD/MM/YYYY') : '',
                'license_nation_id': $scope.UpdatePartner.license_nation_id ? $scope.UpdatePartner.license_nation_id : null,
                'indetifier_no': $scope.UpdatePartner.indetifier_no ? $scope.UpdatePartner.indetifier_no : 0,
                'identifier_date': $scope.UpdatePartner.identifier_date ? moment($scope.UpdatePartner.identifier_date).format('DD/MM/YYYY') : '',
                'identifier_at': $scope.UpdatePartner.identifier_at ? $scope.UpdatePartner.identifier_at : '',
                'identifier_date_expire': $scope.UpdatePartner.identifier_date_expire ? moment($scope.UpdatePartner.identifier_date_expire).format('DD/MM/YYYY') : '',
                'identifier_address': $scope.UpdatePartner.identifier_address ? $scope.UpdatePartner.identifier_address : '',
                'identifier_nation_id': $scope.UpdatePartner.nationality ? $scope.UpdatePartner.nationality : null,
                'identifier_province_id': $scope.UpdatePartner.identifier_province_id ? $scope.UpdatePartner.identifier_province_id : null,
                'is_same_address': $scope.UpdatePartner.is_same_address ? $scope.UpdatePartner.is_same_address : false,
                'now_address': $scope.UpdatePartner.current_address ? $scope.UpdatePartner.current_address : '',
                'now_nation_id': $scope.UpdatePartner.now_nation_id ? $scope.UpdatePartner.now_nation_id : '',
                'now_province_id': $scope.UpdatePartner.now_province_id ? $scope.UpdatePartner.now_province_id : null,
                'identifier_front_image': $scope.UpdatePartner.identifier_front_image ? $scope.UpdatePartner.identifier_front_image : '',
                'identifier_back_image': $scope.UpdatePartner.identifier_back_image ? $scope.UpdatePartner.identifier_back_image : '',
                'avatar': $scope.UpdatePartner.avatar ? $scope.UpdatePartner.avatar : '',
                //'list_documents': $scope.UpdatePartner.list_documents ? $scope.UpdatePartner.list_documents : [],
                'support_person_id': $scope.UpdatePartner.support_person_id,//end
                'company_short_name': $scope.UpdatePartner.company_short_name,
                'company_foreign_name': $scope.UpdatePartner.company_foreign_name,
                'sub_merchant_type': $scope.UpdatePartner?.sub_merchant_type ? $scope.UpdatePartner.sub_merchant_type : 0,
                'franchise_brand': $scope.UpdatePartner.franchise_brand,
                'settlement_by_branch': $scope.UpdatePartner.settlement_by_branch ? $scope.UpdatePartner.settlement_by_branch : $scope.enums.settlementByBranch.KHONG_QT_VE_CO_SO,
                'business_model': $scope.getBusinessModel($scope.UpdatePartner.store_type_id),
                'tax_code': $scope.UpdatePartner.tax_code ? $scope.UpdatePartner.tax_code : '',
                //'registration_date': $scope.UpdatePartner.license_date ? moment($scope.UpdatePartner.license_date).format('DD/MM/YYYY') : '',
                'fax': $scope.UpdatePartner.fax,
                'authorized_capital': $scope.UpdatePartner.authorized_capital ? Number($scope.UpdatePartner.authorized_capital) : 0,
                'source': "ATS", //default
                'merchant_code': $scope.code_partner,
                'company_name': $scope.showFormUserCN ? $scope.UpdatePartner.license_owner : $scope.UpdatePartner.company_name,
                'business_number': $scope.UpdatePartner.license_no ? $scope.UpdatePartner.license_no : '',
                'merchant_type': 3, //default
                'business_registration_certificate_front': $scope.UpdatePartner?.business_registration_certificate_front ? $scope.UpdatePartner.business_registration_certificate_front : '',
                'business_registration_certificate_behind': $scope.UpdatePartner?.business_registration_certificate_behind ? $scope.UpdatePartner.business_registration_certificate_behind : '',
                'tax_certificate_front': $scope.UpdatePartner?.tax_certificate_front ? $scope.UpdatePartner.tax_certificate_front : '',
                'tax_certificate_behind': $scope.UpdatePartner?.tax_certificate_behind ? $scope.UpdatePartner.tax_certificate_behind : '',
                'mcc_code': $scope.UpdatePartner.mcc_code,
                'industry_id': $scope.UpdatePartner?.industry_id ? $scope.UpdatePartner.industry_id : '',
                'gc_code': $scope.UpdatePartner.gc_code,
                'contract_signature_type': $scope.enums.contractSignatureType.KY_SO,
                'representative_title': $scope.UpdatePartner.representative_title,
                'representative_job': $scope.UpdatePartner.representative_job,
                'gender': $scope.UpdatePartner.gender,
                'document_type': $scope.UpdatePartner.document_type,
                'citizen_card_front': $scope.UpdatePartner.identifier_front_image,// Link mặt trước CCCD/ CMND/ HC
                'citizen_card_back': $scope.UpdatePartner.identifier_back_image,// Link mặt sau CCCD/ CMND/ HC
                'hotline': $scope.UpdatePartner.hotline,// hotline
                'license_address': $scope.UpdatePartner.license_address,// địa chỉ trên GPKD
                'payment_realtime': $scope.UpdatePartner.payment_realtime1 === true ? 1 : 0,
                'realtime_payment_non_app_user': $scope.UpdatePartner.payment_realtime2 === true ? 1 : 0,
                'brand_name': $scope.UpdatePartner.brand_name ? $scope.UpdatePartner.brand_name : "",
                'user_district_id': $scope.UpdatePartner.user_district_id,
                'user_ward_id': $scope.UpdatePartner.user_ward_id,
                'nationality': $scope.UpdatePartner.nationality,
            };

            obj.list_acc_no = $scope.getListAccNo();
            obj.list_branch_acc_no = $scope.getListBranchAccNo();
            obj.list_address = $scope.getListAddress();
            obj.list_email = $scope.getListEmail();
            obj.list_phone_numb = $scope.getListPhone();
            obj.list_website = $scope.getListWebsite();
            obj.list_contact = $scope.getListContact();
            obj.working_times = $scope.getListWorkingTime();
            obj.list_other_document = $scope.getListOtherDocument();
            obj.list_representative_info_br = $scope.getListRepresentativeInfoBr();//required business_model: 1,3
            obj.list_representative_info_id = $scope.getListRepresentativeInfoId();
            obj.list_documents = $scope.getListDocuments();

            if ($scope.showFormUserDN) {
                obj.registration_date = $scope.UpdatePartner.license_date ? moment($scope.UpdatePartner.license_date).format('DD/MM/YYYY') : '';
            }

            /*if($scope.UpdatePartner.settlement_by_branch == $scope.enums.settlementByBranch.KHONG_QT_VE_CO_SO) {
                obj.list_branch_acc_no = null;
            } else {
                obj.list_branch_acc_no = $scope.getListBranchAccNo();
            }*/

            if (!$scope.UpdatePartner.business_registration_certificate_behind) {
                obj.business_registration_certificate_behind = $scope.UpdatePartner.business_registration_certificate_front;
            }

            if ($scope.UpdatePartner.change_date) {
                obj.change_date = moment($scope.UpdatePartner.change_date).format('DD/MM/YYYY');
            }

            if ($scope.UpdatePartner.controlling_company != "0") {
                obj.controlling_company = $scope.UpdatePartner.controlling_company;
            } else {
                obj.controlling_company_name = $scope.UpdatePartner.controlling_company_name;
            }

            var post = $http({
                method: 'POST',
                url: apiUrl + 'api/portal/updateInfoStore',
                data: obj,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });
            $scope.checkloading = true
            $scope.disableBtn.btSubmit = true;
            post.success(function successCallback(data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                if (data.code == 200) {
                    $scope.checkloading = false; // Ẩn hiệu ứng loading
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(false)
                            .title('Đăng Ký Đối Tác Thành Công')
                            .textContent('Chúc mừng bạn đã đăng ký đối tác thành công. Trong thời gian sớm nhất, bộ phận chuyên môn của CashPlus sẽ liên hệ hỗ trợ bạn hoàn thiện hợp đồng!')
                            .ok('Đóng')
                            .fullscreen(false)
                    ).finally(function () {
                        $window.location.href = '/'; //go to home page
                    });
                } else {
                    console.log('Error BK: ', data?.data);
                    $scope.checkloading = false; // Ẩn hiệu ứng loading
                    //handle convert error to string
                    //let messages = $scope.convertMessageError(data.data);
                    let message = 'Thông tin đăng ký không hợp lệ';
                    if (data && data?.error === 'USER_NAME_EXIST') {
                        message = "Tài khoản đăng nhập đã tồn tại!";
                    } else if (data && data?.error === 'ERROR_USERNAME_PASSWORD_MISSING') {
                        message = "Tải khoản/mật khẩu không hợp lệ";
                    } else if (data && data?.error === 'ERROR_MCCODE_EXISTS') {
                        message = "Merchant code đã tồn tại trong hệ thống";
                    } else if (data && data?.data === 'BM2_ACC_HOLDER_NOT_MATCH') {
                        message = "Tên người thụ hường phải trùng với Tên người đại diện theo CCCD";
                    } else if (data && data?.data === 'BM2_ACC_HOLDER_INVALID_0') {
                        message = "Loại tài khoản ngân hàng không hợp lệ.";
                    } else if (data && data?.data === 'BM2_ACC_HOLDER_INVALID_1') {
                        message = "Khách hàng cá nhân không được chọn loại tài khoản = Uỷ quyền";
                    } else if (data && data?.data === 'BM2_ACC_HOLDER_INVALID_2') {
                        message = "Loại tài khoản ngân hàng không hợp lệ. Vui lòng chọn loại tài khoản = Chính chủ";
                    } else if (data && data?.data === 'BM3_ACC_HOLDER_NOT_MATCH') {
                        message = "Tên người thụ hưởng phải trùng với tên Hộ kinh doanh hoặc tên Ngưòi đại diện theo ĐKKD";
                    } else if (data && data?.data === 'BM3_ACC_HOLDER_INVALID_0') {
                        message = "Loại tài khoản ngân hàng không hợp lệ.";
                    } else if (data && data?.data === 'BM3_ACC_HOLDER_INVALID_1') {
                        message = "Loại tài khoản ngân hàng không hợp lệ. Vui lòng chọn loại tài khoản = Chính chủ";
                    } else if (data && data?.data === 'BM1_ACC_HOLDER_NOT_MATCH') {
                        message = "Tên người thụ hường phải trùng với Tên Doanh Nghiệp/Hộ Kinh doanh";
                    } else if (data && data?.error === 'ERROR_LOGIN_CODE_IS_CONFIRM') {
                        message = "Tài khoản đã được đăng ký";
                    }
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(false)
                            .title('Thông báo')
                            .textContent(message)
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                    $scope.scrollToElementCentered('bank_account_list');
                }
            }).error(function (data, status, headers, config) {
                console.log('Error System: ', data?.error);
                $scope.checkloading = false; // Biến để kiểm soát hiển thị hiệu ứng loading
                $scope.checkloading = false; // Ẩn hiệu ứng loading
                $scope.disableBtn.btSubmit = false;
                let message = 'Đã xảy ra lỗi khi cập nhật thông tin đối tác. Xin vui lòng thử lại sau!';
                if (data && data?.error === 'USER_NAME_EXIST') {
                    message = "Tài khoản đăng nhập đã tồn tại!";
                } else if (data && data?.error === 'ERROR_USERNAME_PASSWORD_MISSING') {
                    message = "Tải khoản/mật khẩu không hợp lệ";
                } else if (data && data?.error === 'ERROR_MCCODE_EXISTS') {
                    message = "Merchant code đã tồn tại trong hệ thống";
                } else if (data && data?.data === 'BM2_ACC_HOLDER_NOT_MATCH') {
                    message = "Tên người thụ hường phải trùng với Tên người đại diện theo CCCD";
                } else if (data && data?.data === 'BM2_ACC_HOLDER_INVALID_0') {
                    message = "Loại tài khoản ngân hàng không hợp lệ.";
                } else if (data && data?.data === 'BM2_ACC_HOLDER_INVALID_1') {
                    message = "Khách hàng cá nhân không được chọn loại tài khoản = Uỷ quyền";
                } else if (data && data?.data === 'BM2_ACC_HOLDER_INVALID_2') {
                    message = "Loại tài khoản ngân hàng không hợp lệ. Vui lòng chọn loại tài khoản = Chính chủ";
                } else if (data && data?.data === 'BM3_ACC_HOLDER_NOT_MATCH') {
                    message = "Tên người thụ hưởng phải trùng với tên Hộ kinh doanh hoặc tên Ngưòi đại diện theo ĐKKD";
                } else if (data && data?.data === 'BM3_ACC_HOLDER_INVALID_0') {
                    message = "Loại tài khoản ngân hàng không hợp lệ.";
                } else if (data && data?.data === 'BM3_ACC_HOLDER_INVALID_1') {
                    message = "Loại tài khoản ngân hàng không hợp lệ. Vui lòng chọn loại tài khoản = Chính chủ";
                } else if (data && data?.data === 'BM1_ACC_HOLDER_NOT_MATCH') {
                    message = "Tên người thụ hường phải trùng với Tên Doanh Nghiệp/Hộ Kinh doanh";
                } else if (data && data?.error === 'ERROR_LOGIN_CODE_IS_CONFIRM') {
                    message = "Tài khoản đã được đăng ký";
                }
                cfpLoadingBar.complete();
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(false)
                        .title('Thông báo')
                        .textContent(message)
                        .ok('Đóng')
                        .fullscreen(false)
                );
            });
        } else {
            // Form is invalid, show error messages
            $scope.formPartnerUpdate.$submitted = true;

            // Scroll to the first invalid input
            var formElement = $('[name="formPartnerUpdate"]');
            var firstInvalidInput = formElement.find('.ng-invalid').first();
            console.log('invalid id: ', firstInvalidInput.attr('name'));
            $scope.scrollToElementCentered(firstInvalidInput.attr('name'));
        }
    }

    //handle convert error to string
    $scope.getAllValues = function (obj) {
        if (typeof obj === 'object' && obj !== null) {
            let values = [];

            function traverse(obj) {
                for (let key in obj) {
                    if (typeof obj[key] === 'object' && obj[key] !== null) {
                        traverse(obj[key]);
                    } else {
                        values.push(obj[key]);
                    }
                }
            }

            traverse(obj);
            return $scope.replaceValues(values);
        } else {
            return '';
        }
    }

    //replace label
    $scope.replaceValues = function (values) {
        let labels = $scope.labels;
        return values.map(value => {
            for (let key in labels) {
                if (value && value.includes(key)) {
                    return value.replace(key, labels[key]);
                }
            }
            return value;
        });
    }

    $scope.convertMessageError = function (obj) {
        if (typeof obj === 'object' && obj !== null) {
            let labels = $scope.labels;
            const keysArray = Object.keys(obj);

            const newKeysArray = keysArray.map(key => {
                return `${labels[key] || key} không hợp lệ`;
            });
            return newKeysArray;
        }
        return "";
    }

    $scope.uploadAvatarPartner = function (e) {
        $scope.checkloading = true; // Biến để kiểm soát hiển thị hiệu ứng loading

        if (e === undefined) {
            $scope.checkloading = false; // Đặt lại biến checkloading thành false khi hủy upload
            return;
        }
        if (e.files.length <= 0) {
            $scope.checkloading = false; // Đặt lại biến checkloading thành false khi hủy upload
            return;
        }
        var avatarpartner = document.getElementById("avatarpartner");
        var fd = new FormData();
        fd.append("files", e.files[0]);
        cfpLoadingBar.start();

        // Kiểm tra xem UpdatePartner đã được khởi tạo hay chưa
        if (!$scope.UpdatePartner) {
            $scope.UpdatePartner = {};
        }
        var post = $http({
            method: 'POST',
            url: apiUrl + 'api/upload/uploadfile',
            data: fd,
            headers: {
                "Content-Type": undefined,
                'Authorization': 'bearer ' + $scope.access_token
            }
        });

        // Thực hiện xử lý khi nhận được phản hồi từ server
        post.success(function successCallback(data, status, headers, config) {
            if (data.status === 200) {
                $scope.UpdatePartner.avatar = data.data[0].url;
                avatarpartner.style.display = "block";

            } else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Tải ảnh đại diện không thành công')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi khi tải ảnh đại diện. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        }).finally(function () {
            cfpLoadingBar.complete();
            $scope.checkloading = false; // Đặt lại biến checkloading thành false sau khi hoàn thành tất cả các xử lý
        });
    };

    //chuyen sang base64
    $scope.convertFileToBase64 = function (file) {
        function readFileAsDataURL(file) {
            return new Promise((resolve, reject) => {
                const reader = new FileReader();
                reader.onload = () => resolve(reader.result);
                reader.onerror = (error) => reject(error);
                reader.readAsDataURL(file);
            });
        }

        function extractBase64(dataUrl) {

            return dataUrl.split(',')[1]; // Extract the base64 part
        }

        function sendToApi(base64Data) {

            const apiData = {
                document: base64Data,
                document_type: "gpdkkd"
            };

            return $http({
                method: 'POST',
                url: 'https://cv-api.nexusti.pro/document/freeform/recognition',
                data: apiData,
                headers: {
                    "api-key": "7c8ba773-64cd-4ba5-a9bd-f035f06d0149",
                }
            });
        }

        readFileAsDataURL(file)
            .then((dataUrl) => {
                const base64Data = extractBase64(dataUrl);
                return sendToApi(base64Data);
            })
            .then((response) => {

                // ma so thue || ma so kinh doanh
                if (!$scope.UpdatePartner.license_no) {
                    $scope.UpdatePartner.license_no = response.data.company_id
                }

                // ngay dang ky kinh doanh
                if (!$scope.UpdatePartner.license_date) {

                    $scope.reversedDate = $scope.reverseDate(response.data.company_issue_first_date);
                    var datergs = new Date($scope.reversedDate)
                    $scope.UpdatePartner.license_date = datergs

                }
                DKKDname = response.data.company_name
            })
            .catch((error) => {
                console.error('Error:', error);
            }).finally(function () {
                $scope.checkloading = false; // Đặt lại biến checkloading thành false khi hoàn thành các xử lý
            });
    };

    $scope.uploadDKKDPartner = function (e) {
        console.log(e)
        $scope.checkloading = true; // Biến để kiểm soát hiển thị hiệu ứng loading
        if (e === undefined) {
            $scope.checkloading = false; // Đặt lại biến checkloading thành false khi hủy upload
            return;
        }
        if (e.files.length <= 0) {
            $scope.checkloading = false; // Đặt lại biến checkloading thành false khi hủy upload
            return;
        }
        var anhgpdk = document.getElementById("uploadedImage4");
        var anhgpdkupload = document.getElementById("preview4");
        var fd = new FormData();
        fd.append("files", e.files[0]);
        cfpLoadingBar.start();
        var post = $http({
            method: 'POST',
            url: apiUrl + 'api/upload/uploadfile',
            data: fd,
            headers: {
                "Content-Type": undefined,
                'Authorization': 'bearer ' + $scope.access_token
            }
        });

        // Thực hiện xử lý khi nhận được phản hồi từ server
        post.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            if (data.status === 200) {
                anhgpdk.style.display = "none";
                anhgpdkupload.style.display = "block";
                $scope.UpdatePartner.license_image = data.data[0].url;
                /*Link giấy ĐKKD mặt trước*/
                $scope.UpdatePartner.business_registration_certificate_front = data.data[0].url;

                /*goi ham doc du lieu gpkd*/
                //$scope.convertFileToBase64(e.files[0])

                //upload lay ma hash
                var fileUploadFormData = new FormData();
                fileUploadFormData.append("file", e.files[0]);
                fileUploadFormData.append("title", "upload");
                fileUploadFormData.append("description", "upload GPDKKD");

                var fileType = e.files[0].type; // Lấy kiểu MIME của tệp
                var fileTypeForAPI = "";

                if (fileType === "application/pdf") {
                    fileTypeForAPI = "pdf"; // Nếu là PDF
                } else if (fileType.startsWith("image/")) {
                    fileTypeForAPI = "image"; // Nếu là hình ảnh
                } else {
                    // Các loại tệp khác có thể xử lý tại đây (nếu cần)
                    console.warn("File type not supported for OCR:", fileType);
                    fileTypeForAPI = "unknown"; // Đặt giá trị mặc định nếu không xác định được
                }

                $http({
                    method: 'POST',
                    url: $scope.apiVNPT + "/file-service/v1/addFile",
                    headers: {
                        "Content-Type": undefined,
                        'Authorization': 'Bearer ' + $scope.accesstokenVNPT,
                        "Token-id": $scope.tokenIdVnpt,
                        "Token-key": $scope.tokenKeyVnpt
                    },
                    data: fileUploadFormData,
                }).then(function (response) {
                    $scope.checkloading = false;
                    if (response.data.message && response.data.message === "IDG-00000000") {
                        var ocrGPKD = {
                            "file_hash": response.data.object.hash,
                            "token": "gpdkkd",
                            "client_session": "ANDROID_nokia7.2_28_Simulator_2.4.2_08d2d8686ee5fa0e_1581910116532",
                            "file_type": fileTypeForAPI, // Thiết lập file_type dựa trên loại tệp
                            "details": false
                        }

                        // goi api ocr gpdkkd
                        if ($scope.UpdatePartner.store_type_id == 8) {
                            // api doanh nghiep
                            $http({
                                method: 'POST',
                                url: $scope.apiVNPT + "/rpa-service/aidigdoc/v1/ocr/dang-ky-kinh-doanh",
                                headers: {
                                    "Content-Type": "application/json",
                                    'Authorization': 'Bearer ' + $scope.accesstokenVNPT,
                                    "Token-id": $scope.tokenIdVnptv1,
                                    "Token-key": $scope.tokenKeyVnptv1
                                },
                                data: ocrGPKD,
                            }).then(function (response) {

                                if (response.data.object.warning_messages && response.data.object.warning_messages.length >= 1) {
                                    console.log("co vao day")
                                    var warningText = response.data.object.warning_messages.map(function (warning, index) {
                                        return (index + 1) + '. ' + warning + '.';
                                    }).join('\n');
                                    $mdDialog.show(
                                        $mdDialog.alert()
                                            .clickOutsideToClose(true)
                                            .title('Thông báo')
                                            .textContent(warningText)
                                            .ok('Đóng')
                                            .fullscreen(false)
                                    );
                                    return;
                                }
                                try {
                                    // ma so dang ky kinh doanh                               
                                    $scope.UpdatePartner.license_no = response.data.object?.MA_SO_DOANH_NGHIEP
                                } catch (e) {
                                    console.error("Lỗi:", e);
                                }
                                //Tên doanh nghiệp
                                try {
                                    //if (!$scope.UpdatePartner.license_owner) {
                                    $scope.UpdatePartner.company_name = response.data.object.TEN_CONG_TY_TIENG_VIET
                                } catch (e) {
                                    console.error("Lỗi:", e);
                                }
                                //}
                                //Tên viết tắt
                                $scope.UpdatePartner.company_short_name = response.data.object?.TEN_CONG_TY_VIET_TAT
                                //Tên công ty nước ngoài 
                                $scope.UpdatePartner.company_foreign_name = response.data.object?.TEN_CONG_TY_NUOC_NGOAI
                                //Mã số thuế
                                $scope.UpdatePartner.tax_code = response.data.object?.MA_SO_DOANH_NGHIEP
                                //Địa chỉ theo GPKD
                                $scope.UpdatePartner.license_address = response.data.object?.DIA_CHI_TRU_SO_CHINH

                                // Tên người đại diện
                                //if (!$scope.UpdatePartner.license_birth_date) {
                                $scope.UpdatePartner.license_owner = response.data.object?.THONG_TIN_NGUOI_DAI_DIEN_PHAP_LUAT_HO_VA_TEN[0]
                                //Ngày sinh
                                try {
                                    $scope.reversedDate = $scope.convertDate(response.data.object?.THONG_TIN_NGUOI_DAI_DIEN_PHAP_LUAT_NGAY_SINH[0]);
                                    var birthday = new Date($scope.reversedDate)
                                    $scope.UpdatePartner.license_birth_date = birthday
                                } catch (e) {
                                    console.error("Lỗi:", e);
                                }
                                //Chức danh
                                //Giới tính
                                try {
                                    let dataGender = response.data.object.THONG_TIN_NGUOI_DAI_DIEN_PHAP_LUAT_GIOI_TINH[0];
                                    let selectedGender = $scope.genders.find(g => g.name === dataGender);
                                    $scope.UpdatePartner.gender = selectedGender.value;
                                } catch (e) {
                                    console.error("Lỗi:", e);
                                }

                                // Số CCCD
                                //if (!$scope.UpdatePartner.indetifier_no) {
                                $scope.UpdatePartner.indetifier_no = response.data.object.THONG_TIN_NGUOI_DAI_DIEN_PHAP_LUAT_SO_GIAY_CHUNG_THUC_CA_NHAN[0]
                                //}

                                //}
                                //ngay cap - truong hop la cccd tren gpdkkd
                                try {
                                    $scope.reversedDate = $scope.convertDate(response.data.object.THONG_TIN_NGUOI_DAI_DIEN_PHAP_LUAT_NGAY_CAP[0]);
                                    var issue_date = new Date($scope.reversedDate)
                                    $scope.UpdatePartner.identifier_date = issue_date
                                } catch (e) {
                                    console.error("Lỗi:", e);
                                }

                                //noi cap - truong hop la ho chieu
                                try {
                                    $scope.UpdatePartner.identifier_at = response.data.object.THONG_TIN_NGUOI_DAI_DIEN_PHAP_LUAT_NOI_CAP[0].replace(/\n/g, ' ')
                                } catch (e) {
                                    console.error("Lỗi:", e);
                                }
                                //von dieu le
                                try {
                                    if (!$scope.UpdatePartner.authorized_capital && response.data.object.VON_DIEU_LE) {
                                        $scope.UpdatePartner.authorized_capital = parseInt(response.data.object.VON_DIEU_LE.replace(/[^0-9]/g, ""), 10)
                                    }
                                } catch (e) {
                                    console.error("Lỗi:", e);
                                }
                                // dia chỉ thuong tru
                                try {
                                    //if (!$scope.UpdatePartner.identifier_address) {
                                    $scope.UpdatePartner.identifier_address = response.data.object.THONG_TIN_NGUOI_DAI_DIEN_PHAP_LUAT_HO_KHAU_THUONG_TRU[0].replace(/\n/g, ', ')
                                    //}          
                                } catch (e) {
                                    console.error("Lỗi:", e);
                                }
                                // quoc tich 
                                //if (!$scope.UpdatePartner.identifier_nation_id) {
                                $scope.UpdatePartner.identifier_nation_id = 20000
                                //}
                                // quoc gia
                                //if (!$scope.UpdatePartner.license_nation_id) {
                                $scope.UpdatePartner.license_nation_id = 20000
                                //}
                                // thanh pho 
                                //if (!$scope.UpdatePartner.identifier_province_id) {
                                try {
                                    var get = $http({
                                        method: 'GET',
                                        url: apiUrl + 'api/portal/province',
                                        headers: {
                                            "Content-Type": undefined,
                                            'Authorization': 'bearer ' + $scope.access_token
                                        }
                                    });
                                    get.success(function successCallback(data, status, headers, config) {
                                        var idProvince = '';
                                        var listProvince = data.data
                                        var foundProvinceId = null;
                                        var addressMrc = response.data.object.THONG_TIN_NGUOI_DAI_DIEN_PHAP_LUAT_HO_KHAU_THUONG_TRU[0]
                                        var addressParts = addressMrc.split(',');
                                        var cityNameMrc = addressParts.slice(-2, -1)[0].trim();
                                        ;
                                        if (cityNameMrc === "Thành phố Hà Nội") {
                                            cityNameMrc = "Hà Nội";
                                        }
                                        console.log(cityNameMrc)
                                        for (let i = 0; i < listProvince.length; i++) {
                                            if (listProvince[i].name === cityNameMrc) {
                                                foundProvinceId = listProvince[i].id;
                                                break; // Dừng vòng lặp khi tìm thấy
                                            }
                                        }
                                        var valueProvince = {
                                            id: foundProvinceId,
                                            name: cityNameMrc

                                        };
                                        $scope.changeValueProvince(valueProvince) // Gọi hàm chọn thành phố để hiển thị danh sách quận/huyện
                                        $scope.UpdatePartner.identifier_province_id = foundProvinceId

                                        // Chỉ tiếp tục gọi API quận/huyện/phường/xã nếu tìm thấy idProvince
                                        if (foundProvinceId) {
                                            console.log(foundProvinceId)
                                            // Gọi API lấy thông tin quận/huyện/phường/xã
                                            var postCodeData = response.data.object.THONG_TIN_NGUOI_DAI_DIEN_PHAP_LUAT_HO_KHAU_THUONG_TRU[0].split(',').slice(-3, -2)[0].trim();
                                            ;
                                            var getDistricts = $http({
                                                method: 'GET',
                                                url: apiUrl + `api/portal/provinceBy/` + foundProvinceId, // Sử dụng foundProvinceId trong URL
                                                headers: {
                                                    "Content-Type": undefined,
                                                    'Authorization': 'bearer ' + $scope.access_token
                                                }
                                            });

                                            getDistricts.success(function successCallback(data, status, headers, config) {
                                                var listDistricts = data?.data;

                                                // Tìm quận/huyện theo tên trong danh sách API
                                                var foundDistrict = listDistricts.find(function (district) {
                                                    return district.name === postCodeData;
                                                });
                                                console.log(foundDistrict)

                                                if (foundDistrict) {
                                                    $scope.changeValueDistrict(foundDistrict)
                                                    $scope.UpdatePartner.user_district_id = foundDistrict.id;

                                                    // Tiếp tục gọi API lấy danh sách phường/xã dựa vào id quận/huyện
                                                    var dataWard = response.data.object.THONG_TIN_NGUOI_DAI_DIEN_PHAP_LUAT_HO_KHAU_THUONG_TRU[0].split(',').slice(-4, -3)[0].trim();


                                                    var getWards = $http({
                                                        method: 'GET',
                                                        url: apiUrl + `api/portal/provinceBy/${foundDistrict.id}`, // Sử dụng foundDistrict.id trong URL
                                                        headers: {
                                                            "Content-Type": undefined,
                                                            'Authorization': 'bearer ' + $scope.access_token
                                                        }
                                                    });

                                                    getWards.success(function successCallback(data, status, headers, config) {
                                                        var listWards = data?.data;
                                                        console.log(listWards, dataWard)
                                                        // Tìm phường/xã theo tên trong danh sách API
                                                        var foundWard = listWards.find(function (ward) {
                                                            return ward.name === dataWard;
                                                        });

                                                        if (foundWard) {
                                                            $scope.UpdatePartner.user_ward_id = foundWard.id;
                                                        } else {
                                                            console.error("Không tìm thấy phường/xã tương ứng");
                                                        }
                                                    });

                                                } else {
                                                    console.error("Không tìm thấy quận/huyện tương ứng");
                                                }
                                            });
                                        } else {
                                            console.error("Không tìm thấy tỉnh/thành phố tương ứng");
                                        }
                                    });
                                } catch (e) {
                                    console.error("Lỗi:", e);
                                }

                                // Ngày thay đổi đăng ký kinh doanh
                                if (response.data.object.DANG_KY_THAY_DOI) {
                                    try {
                                        $scope.reversedDate = $scope.convertDateDKKDVPNT(response.data.object.DANG_KY_THAY_DOI);
                                        $scope.UpdatePartner.license_date = new Date($scope.reversedDate);
                                    } catch (e) {
                                        console.error("Lỗi khi chuyển đổi ngày thay đổi đăng ký kinh doanh:", e);
                                    }
                                }



                                DKKDname = response.data.object.TEN_CONG_TY_TIENG_VIET;
                                if (response.data.object.cmnd_cccd === "") {
                                    $scope.checkDataOcrGPDKKD = true;
                                }


                            }).catch(function (error) {
                                $scope.checkDataOcrGPDKKD = true;
                                console.error(error);
                            })
                        } else if ($scope.UpdatePartner.store_type_id == 17) {
                            // api ho kinh doanh
                            $http({
                                method: 'POST',
                                url: $scope.apiVNPT + "/rpa-service/aidigdoc/v1/ocr/dang-ky-ho-kinh-doanh",
                                headers: {
                                    "Content-Type": "application/json",
                                    'Authorization': 'Bearer ' + $scope.accesstokenVNPT,
                                    "Token-id": $scope.tokenIdVnptv1,
                                    "Token-key": $scope.tokenKeyVnptv1
                                },
                                data: ocrGPKD,
                            }).then(function (response) {

                                try {
                                    // Tên hộ kinh doanh
                                    if (response.data.object?.ten_ho_kinh_doanh) {
                                        $scope.UpdatePartner.company_name = response.data.object.ten_ho_kinh_doanh;
                                    }
                                } catch (error) {
                                    console.error("Lỗi khi gán tên hộ kinh doanh:", error);
                                }

                                try {
                                    // Địa chỉ theo giấy phép kinh doanh
                                    if (response.data.object?.dia_diem_kinh_doanh) {
                                        $scope.UpdatePartner.license_address = response.data.object.dia_diem_kinh_doanh;
                                    }
                                } catch (error) {
                                    console.error("Lỗi khi gán địa chỉ theo giấy phép kinh doanh:", error);
                                }

                                try {
                                    // Tên người đại diện
                                    if (response.data.object?.ten_nguoi_dai_dien) {
                                        $scope.UpdatePartner.license_owner = response.data.object.ten_nguoi_dai_dien;
                                    }
                                } catch (error) {
                                    console.error("Lỗi khi gán tên người đại diện:", error);
                                }

                                try {
                                    // Ngày sinh
                                    if (response.data.object?.ngay_sinh_ho_kinh_doanh) {
                                        var convertHKD = $scope.convertDate(response.data.object.ngay_sinh_ho_kinh_doanh);
                                        $scope.UpdatePartner.license_birth_date = new Date(convertHKD);
                                    }
                                } catch (error) {
                                    console.error("Lỗi khi gán ngày sinh:", error);
                                }

                                try {
                                    // Ngày cấp
                                    if (response.data.object?.ngay_sinh_ho_kinh_doanh) {
                                        var convertBirtdayHKD = $scope.convertDate(response.data.object.ngay_sinh_ho_kinh_doanh);
                                        $scope.UpdatePartner.identifier_date = new Date(convertBirtdayHKD);
                                    }
                                } catch (error) {
                                    console.error("Lỗi khi gán ngày cấp:", error);
                                }

                                try {
                                    // Nơi cấp
                                    if (response.data.object?.noi_cap) {
                                        $scope.UpdatePartner.identifier_at = response.data.object.noi_cap;
                                    }
                                } catch (error) {
                                    console.error("Lỗi khi gán nơi cấp:", error);
                                }

                                try {
                                    // CCCD (nếu có)
                                    if (response.data.object?.cmnd_cccd) {
                                        $scope.UpdatePartner.indetifier_no = response.data.object.cmnd_cccd;
                                    }
                                } catch (error) {
                                    console.error("Lỗi khi gán CCCD:", error);
                                }

                                try {
                                    // Mã số đăng ký kinh doanh
                                    if (response.data.object?.dang_ky_lan && !$scope.UpdatePartner.license_no) {
                                        const pattern = /Mã số đăng ký hộ kinh doanh: (?<MaSo>[\d]+)\/Đăng ký lần đầu: (?<LanDau>.+?)\/Đăng ký thay đổi lần thứ: (?<ThayDoiLanThu>.+)/;
                                        const match = response.data.object?.dang_ky_lan.match(pattern);
                                        if (match) {
                                            $scope.UpdatePartner.license_no = match.groups.MaSo.trim()
                                        }
                                    }
                                } catch (error) {
                                    console.error("Lỗi khi gán mã số đăng ký kinh doanh:", error);
                                }

                                try {
                                    // Mã số thuế - HỘ KINH DOANH
                                    if (response.data.object?.so_giay_phep_kinh_doanh) {
                                        const licenseNumber = response.data.object?.so_giay_phep_kinh_doanh;
                                        const numberMatch = licenseNumber.match(/\d+/g);
                                        if (numberMatch) {
                                            $scope.UpdatePartner.tax_code = numberMatch.join('');
                                        }
                                    }
                                } catch (error) {
                                    console.error("Lỗi khi gán mã số thuế hộ kinh doanh:", error);
                                }

                                // Xử lý tỉnh thành và quận huyện
                                try {
                                    var get = $http({
                                        method: 'GET',
                                        url: apiUrl + 'api/portal/province',
                                        headers: {
                                            "Content-Type": undefined,
                                            'Authorization': 'bearer ' + $scope.access_token
                                        }
                                    });

                                    get.success(function successCallback(data) {
                                        var idProvince = '';
                                        var listProvince = data.data;
                                        var foundProvinceId = null;
                                        var addressMrc = response.data.object.cho_o_hien_tai;
                                        var addressParts = addressMrc.split(',');
                                        var cityNameMrc = addressParts?.slice(-2, -1)[0].trim();
                                        if (cityNameMrc === "Thành phố Hà Nội") {
                                            cityNameMrc = "Hà Nội";
                                        }
                                        console.log(cityNameMrc);
                                        for (let i = 0; i < listProvince.length; i++) {
                                            if (listProvince[i].name === cityNameMrc) {
                                                foundProvinceId = listProvince[i].id;
                                                break;
                                            }
                                        }
                                        if (foundProvinceId) {
                                            var valueProvince = {
                                                id: foundProvinceId,
                                                name: cityNameMrc
                                            };
                                            $scope.changeValueProvince(valueProvince); // Gọi hàm chọn thành phố để hiển thị danh sách quận/huyện
                                            $scope.UpdatePartner.identifier_province_id = foundProvinceId;

                                            // Chỉ tiếp tục gọi API quận/huyện/phường/xã nếu tìm thấy idProvince
                                            if (foundProvinceId) {
                                                var postCodeData = addressMrc.split(',').slice(-3, -2)[0].trim();
                                                var getDistricts = $http({
                                                    method: 'GET',
                                                    url: apiUrl + `api/portal/provinceBy/` + foundProvinceId,
                                                    headers: {
                                                        "Content-Type": undefined,
                                                        'Authorization': 'bearer ' + $scope.access_token
                                                    }
                                                });

                                                getDistricts.success(function successCallback(data) {
                                                    var listDistricts = data?.data;
                                                    var foundDistrict = listDistricts.find(function (district) {
                                                        return district.name === postCodeData;
                                                    });
                                                    if (foundDistrict) {
                                                        $scope.changeValueDistrict(foundDistrict);
                                                        $scope.UpdatePartner.user_district_id = foundDistrict.id;

                                                        // Tiếp tục gọi API lấy danh sách phường/xã dựa vào id quận/huyện
                                                        var dataWard = addressMrc.split(',').slice(-4, -3)[0].trim();
                                                        var getWards = $http({
                                                            method: 'GET',
                                                            url: apiUrl + `api/portal/provinceBy/${foundDistrict.id}`,
                                                            headers: {
                                                                "Content-Type": undefined,
                                                                'Authorization': 'bearer ' + $scope.access_token
                                                            }
                                                        });

                                                        getWards.success(function successCallback(data) {
                                                            var listWards = data?.data;
                                                            var foundWard = listWards.find(function (ward) {
                                                                return ward.name === dataWard;
                                                            });
                                                            if (foundWard) {
                                                                $scope.UpdatePartner.user_ward_id = foundWard.id;
                                                            } else {
                                                                console.error("Không tìm thấy phường/xã tương ứng");
                                                            }
                                                        });
                                                    } else {
                                                        console.error("Không tìm thấy quận/huyện tương ứng");
                                                    }
                                                });
                                            } else {
                                                console.error("Không tìm thấy tỉnh/thành phố tương ứng");
                                            }
                                        }
                                    });
                                } catch (error) {
                                    console.error("Lỗi khi xử lý tỉnh/thành phố:", error);
                                }

                                try {
                                    // Ngày đăng ký kinh doanh
                                    if (!$scope.UpdatePartner.license_date && response.data.object.dang_ky_lan) {
                                        $scope.reversedDate = $scope.convertDateDKKDVPNT(response.data.object.dang_ky_lan);
                                        $scope.UpdatePartner.license_date = new Date($scope.reversedDate);
                                    }
                                } catch (error) {
                                    console.error("Lỗi khi gán ngày đăng ký kinh doanh:", error);
                                }

                                if (response.data.object.cmnd_cccd === "") {
                                    $scope.checkDataOcrGPDKKD = true;
                                }
                                DKKDname = response.data.object.ten_ho_kinh_doanh;

                            }).catch(function (error) {
                                $scope.checkDataOcrGPDKKD = true;
                                console.error(error);
                            });
                        }

                    } else {
                        $scope.checkDataOcrGPDKKD = true;
                        $mdDialog.show(
                            $mdDialog.alert()
                                .clickOutsideToClose(true)
                                .title('Thông báo')
                                .textContent('Tải ảnh không thành công. Vui lòng thử lại')
                                .ok('Đóng')
                                .fullscreen(false)
                        )
                    }
                }).catch(function (error) {
                    $scope.checkDataOcrGPDKKD = true;
                    $scope.checkloading = false;
                    console.error(error);
                })
            } else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Tải giấy phép kinh doanh không thành công')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi khi tải GPKD. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        }).finally(function () {
            $scope.checkloading = false; // Đặt lại biến checkloading thành false sau khi hoàn thành tất cả các xử lý
            e.value = ""; // Reset giá trị của input file để đảm bảo onchange luôn được kích hoạt

        });
    }

    //Tai anh DKKD mat 2
    $scope.uploadDKKDPartner2 = function (e) {
        console.log("co vao ham")
        $scope.checkloading = true; // Biến để kiểm soát hiển thị hiệu ứng loading
        if (e === undefined) return;
        if (e.files.length <= 0) return;
        var anhgpdk2 = document.getElementById("uploadedImage5");
        var anhgpdkupload2 = document.getElementById("preview5");
        var fd = new FormData();
        fd.append("files", e.files[0]);
        cfpLoadingBar.start();

        if (!$scope.UpdatePartner) {
            $scope.UpdatePartner = {};
        }
        var post = $http({
            method: 'POST',
            url: apiUrl + 'api/upload/uploadfile',
            data: fd,
            headers: {
                "Content-Type": undefined,
                'Authorization': 'bearer ' + $scope.access_token
            }
        });
        // haohv 

        post.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            if (data.status === 200) {
                $scope.checkloading = false; // Biến để kiểm soát hiển thị hiệu ứng loading
                anhgpdk2.style.display = "none";
                anhgpdkupload2.style.display = "block";
                $scope.UpdatePartner.license_image2 = data.data[0].url;
                /*Link giấy ĐKKD mặt sau*/
                $scope.UpdatePartner.business_registration_certificate_behind = data.data[0].url;


                //upload lay ma hash
                var fileUploadFormData = new FormData();
                fileUploadFormData.append("file", e.files[0]);
                fileUploadFormData.append("title", "upload");
                fileUploadFormData.append("description", "upload GPDKKD mat 2");

                var fileType = e.files[0].type; // Lấy kiểu MIME của tệp
                var fileTypeForAPI = "";

                if (fileType === "application/pdf") {
                    fileTypeForAPI = "pdf"; // Nếu là PDF
                } else if (fileType.startsWith("image/")) {
                    fileTypeForAPI = "image"; // Nếu là hình ảnh
                } else {
                    // Các loại tệp khác có thể xử lý tại đây (nếu cần)
                    console.warn("File type not supported for OCR:", fileType);
                    fileTypeForAPI = "image";
                }
                // kiểm tra nếu gpkd mặt 1 ko fill đc ra thông tin cccd của người sở hữu thì gọi lại api ocr
                if (!$scope.UpdatePartner.indetifier_no || $scope.UpdatePartner.indetifier_no === "") {
                    $http({
                        method: 'POST',
                        url: $scope.apiVNPT + "/file-service/v1/addFile",
                        headers: {
                            "Content-Type": undefined,
                            'Authorization': 'Bearer ' + $scope.accesstokenVNPT,
                            "Token-id": $scope.tokenIdVnpt,
                            "Token-key": $scope.tokenKeyVnpt
                        },
                        data: fileUploadFormData,
                    }).then(function (response) {
                        $scope.checkloading = false;
                        if (response.data.message && response.data.message === "IDG-00000000") {
                            var ocrGPKD = {
                                "file_hash": response.data.object.hash,
                                "token": "gpdkkd",
                                "client_session": "ANDROID_nokia7.2_28_Simulator_2.4.2_08d2d8686ee5fa0e_1581910116532",
                                "file_type": fileTypeForAPI, // Thiết lập file_type dựa trên loại tệp
                                "details": false
                            }

                            // goi api ocr gpdkkd

                            $http({
                                method: 'POST',
                                url: $scope.apiVNPT + "/rpa-service/aidigdoc/v1/ocr/scan",
                                headers: {
                                    "Content-Type": "application/json",
                                    'Authorization': 'Bearer ' + $scope.accesstokenVNPT,
                                    "Token-id": $scope.tokenIdVnptv1,
                                    "Token-key": $scope.tokenKeyVnptv1
                                },
                                data: ocrGPKD,
                            }).then(function (response) {


                                let lines = response.data.object.lines[0];

                                for (let i = 0; i < lines.length; i++) {
                                    let line = lines[i].trim();

                                    if (line.startsWith("Họ và tên:") && line.includes("Giới tính:")) {

                                        let parts = line.split("Giới tính:");

                                        $scope.UpdatePartner.license_owner = parts[0].replace("Họ và tên:", "").trim();

                                        let dataGender = parts[1].trim();
                                        let selectedGender = $scope.genders.find(g => g.name === dataGender);
                                        $scope.UpdatePartner.gender = selectedGender ? selectedGender.value : null;
                                    }

                                    if (line.startsWith("Dân tộc:")) {
                                        $scope.UpdatePartner.nation = line.replace("Dân tộc:", "").trim();
                                    }
                                    if (line.startsWith("Số giấy chứng thực cá nhân:") || line.startsWith("Số giấy tờ pháp lý của cá nhân:")) {
                                        // Loại bỏ phần tiền tố và khoảng trắng thừa
                                        let identifierNumber = line.replace("Số giấy chứng thực cá nhân:", "").replace("Số giấy tờ pháp lý của cá nhân:", "").trim();

                                        // Chuyển chuỗi số thành số nguyên
                                        $scope.UpdatePartner.indetifier_no = parseInt(identifierNumber, 10);

                                        console.log($scope.UpdatePartner.indetifier_no);
                                    }
                                    if (line.startsWith("Sinh ngày:")) {
                                        // Tách lấy chỉ phần ngày tháng năm
                                        let datePart = line.replace("Sinh ngày:", "").trim().split(' ')[0];
                                        let dateStr = $scope.convertDate(datePart);
                                        console.log(dateStr);
                                        var dataBirthDate = new Date(dateStr);
                                        $scope.UpdatePartner.license_birth_date = dataBirthDate;
                                    }

                                    // Ngày cấp
                                    if (line.match(/\d{2}\/\d{2}\/\d{4}/)) {
                                        // Chỉ lấy ngày tháng năm từ chuỗi
                                        let datePart = line.match(/\d{2}\/\d{2}\/\d{4}/)[0];
                                        let dateStr = $scope.convertDate(datePart);
                                        var dataIssueDate = new Date(dateStr);
                                        $scope.UpdatePartner.identifier_date = dataIssueDate;

                                        // Tìm "Nơi cấp" sau ngày tháng năm
                                        if (line.includes("Nơi cấp:")) {
                                            let placeOfIssue = line.split("Nơi cấp:")[1].trim();
                                            $scope.UpdatePartner.identifier_at = placeOfIssue;
                                        }
                                    }

                                    if (line.startsWith("Nơi đăng ký hộ khẩu thường trú:")) {
                                        $scope.UpdatePartner.identifier_address = `${line.replace("Nơi đăng ký hộ khẩu thường trú:", "").trim()} ${lines[i + 1].trim()}`;
                                    } else if (line.startsWith("Địa chỉ thường trú:")) {
                                        $scope.UpdatePartner.identifier_address = line.replace("Địa chỉ thường trú:", "").trim()
                                    }

                                    if (line.startsWith("Chỗ ở hiện tại:")) {
                                        // Lấy thông tin từ dòng hiện tại và dòng tiếp theo
                                        $scope.UpdatePartner.current_address = `${line.replace("Chỗ ở hiện tại:", "").trim()} ${lines[i + 1] ? lines[i + 1].trim() : ''}`;
                                    } else if (line.startsWith("Địa chỉ liên lạc:")) {
                                        // Chỉ lấy thông tin từ dòng hiện tại
                                        $scope.UpdatePartner.current_address = line.replace("Địa chỉ liên lạc:", "").trim();
                                    }
                                }


                                // quoc tich
                                //if (!$scope.UpdatePartner.identifier_nation_id) {
                                $scope.UpdatePartner.identifier_nation_id = 20000
                                //}
                                // quoc gia
                                //if (!$scope.UpdatePartner.license_nation_id) {
                                $scope.UpdatePartner.license_nation_id = 20000
                                //}
                                //// thanh pho
                                ////if (!$scope.UpdatePartner.identifier_province_id) {
                                //var get = $http({
                                //    method: 'GET',
                                //    url: apiUrl + 'api/portal/province',
                                //    headers: {
                                //        "Content-Type": undefined,
                                //        'Authorization': 'bearer ' + $scope.access_token
                                //    }
                                //});
                                //get.success(function successCallback(data, status, headers, config) {
                                //    var idProvince = '';
                                //    var listProvince = data.data
                                //    var foundProvinceId = null;
                                //    var addressMrc = response.data.object.THONG_TIN_NGUOI_DAI_DIEN_PHAP_LUAT_HO_KHAU_THUONG_TRU[0]
                                //    var addressParts = addressMrc.split(',');
                                //    var cityNameMrc = addressParts.slice(-2, -1)[0].trim();;
                                //    if (cityNameMrc === "Thành phố Hà Nội") {
                                //        cityNameMrc = "Hà Nội";
                                //    }
                                //    console.log(cityNameMrc)
                                //    for (let i = 0; i < listProvince.length; i++) {
                                //        if (listProvince[i].name === cityNameMrc) {
                                //            foundProvinceId = listProvince[i].id;
                                //            break; // Dừng vòng lặp khi tìm thấy
                                //        }
                                //    }
                                //    var valueProvince = {
                                //        id: foundProvinceId,
                                //        name: cityNameMrc

                                //    };
                                //    $scope.changeValueProvince(valueProvince) // Gọi hàm chọn thành phố để hiển thị danh sách quận/huyện
                                //    $scope.UpdatePartner.identifier_province_id = foundProvinceId

                                //    // Chỉ tiếp tục gọi API quận/huyện/phường/xã nếu tìm thấy idProvince
                                //    if (foundProvinceId) {
                                //        console.log(foundProvinceId)
                                //        // Gọi API lấy thông tin quận/huyện/phường/xã
                                //        var postCodeData = response.data.object.THONG_TIN_NGUOI_DAI_DIEN_PHAP_LUAT_HO_KHAU_THUONG_TRU[0].split(',').slice(-3, -2)[0].trim();;
                                //        var getDistricts = $http({
                                //            method: 'GET',
                                //            url: apiUrl + `api/portal/provinceBy/` + foundProvinceId, // Sử dụng foundProvinceId trong URL
                                //            headers: {
                                //                "Content-Type": undefined,
                                //                'Authorization': 'bearer ' + $scope.access_token
                                //            }
                                //        });

                                //        getDistricts.success(function successCallback(data, status, headers, config) {
                                //            var listDistricts = data?.data;

                                //            // Tìm quận/huyện theo tên trong danh sách API
                                //            var foundDistrict = listDistricts.find(function (district) {
                                //                return district.name === postCodeData;
                                //            });
                                //            console.log(foundDistrict)

                                //            if (foundDistrict) {
                                //                $scope.changeValueDistrict(foundDistrict)
                                //                $scope.UpdatePartner.user_district_id = foundDistrict.id;

                                //                // Tiếp tục gọi API lấy danh sách phường/xã dựa vào id quận/huyện
                                //                var dataWard = response.data.object.THONG_TIN_NGUOI_DAI_DIEN_PHAP_LUAT_HO_KHAU_THUONG_TRU[0].split(',').slice(-4, -3)[0].trim();


                                //                var getWards = $http({
                                //                    method: 'GET',
                                //                    url: apiUrl + `api/portal/provinceBy/${foundDistrict.id}`, // Sử dụng foundDistrict.id trong URL
                                //                    headers: {
                                //                        "Content-Type": undefined,
                                //                        'Authorization': 'bearer ' + $scope.access_token
                                //                    }
                                //                });

                                //                getWards.success(function successCallback(data, status, headers, config) {
                                //                    var listWards = data?.data;
                                //                    console.log(listWards, dataWard)
                                //                    // Tìm phường/xã theo tên trong danh sách API
                                //                    var foundWard = listWards.find(function (ward) {
                                //                        return ward.name === dataWard;
                                //                    });

                                //                    if (foundWard) {
                                //                        $scope.UpdatePartner.user_ward_id = foundWard.id;
                                //                    } else {
                                //                        console.error("Không tìm thấy phường/xã tương ứng");
                                //                    }
                                //                });

                                //            } else {
                                //                console.error("Không tìm thấy quận/huyện tương ứng");
                                //            }
                                //        });
                                //    } else {
                                //        console.error("Không tìm thấy tỉnh/thành phố tương ứng");
                                //    }
                                //});



                                DKKDname = response.data.object.TEN_CONG_TY_TIENG_VIET
                                if (response.data.object.cmnd_cccd === "") {
                                    $scope.checkDataOcrGPDKKD = true
                                }


                            }).catch(function (error) {
                                $scope.checkDataOcrGPDKKD = true;
                                console.error(error);
                            })


                        } else {
                            $scope.checkDataOcrGPDKKD = true;
                            $mdDialog.show(
                                $mdDialog.alert()
                                    .clickOutsideToClose(true)
                                    .title('Thông báo')
                                    .textContent('Tải ảnh không thành công. Vui lòng thử lại')
                                    .ok('Đóng')
                                    .fullscreen(false)
                            )
                        }
                    }).catch(function (error) {
                        $scope.checkDataOcrGPDKKD = true;
                        $scope.checkloading = false;
                        console.error(error);
                    })
                }

            } else {
                $scope.checkloading = false; // Biến để kiểm soát hiển thị hiệu ứng loading
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Tải giấy phép kinh doanh mặt 2 không thành công')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi khi tải GPKD mặt 2. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });
    };
    //Ket thuc tai anh DKKD mat 2

    $scope.uploadAuthDocLink = function (e, isFrom = true) {
        $scope.checkloading = true; // Biến để kiểm soát hiển thị hiệu ứng loading
        if (e === undefined) {
            $scope.checkloading = false; // Đặt lại biến checkloading thành false khi hủy upload
            return;
        }
        if (e.files.length <= 0) {
            $scope.checkloading = false; // Đặt lại biến checkloading thành false khi hủy upload
            return;
        }
        var fd = new FormData();
        fd.append("files", e.files[0]);
        cfpLoadingBar.start();
        var post = $http({
            method: 'POST',
            url: apiUrl + 'api/upload/uploadfile',
            data: fd,
            headers: {
                "Content-Type": undefined,
                'Authorization': 'bearer ' + $scope.access_token
            }
        });

        // Thực hiện xử lý khi nhận được phản hồi từ server
        post.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            if (data.status === 200) {
                if (isFrom) $scope.UpdatePartner.auth_doc_link = data.data[0].url;
                if (!isFrom) $scope.bankAccount.auth_doc_link = data.data[0].url;
            } else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(false)
                        .title('Thông báo')
                        .textContent('Tải giấy ủy quyền không thành công')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(false)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi khi tải giấy ủy quyền. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        }).finally(function () {
            $scope.checkloading = false; // Đặt lại biến checkloading thành false sau khi hoàn thành tất cả các xử lý
            e.value = ""; // Reset giá trị của input file để đảm bảo onchange luôn được kích hoạt
        });
    }

    $scope.uploadOtherDocument = function (e) {
        $scope.checkloading = true; // Biến để kiểm soát hiển thị hiệu ứng loading
        if (e === undefined) {
            $scope.checkloading = false; // Đặt lại biến checkloading thành false khi hủy upload
            return;
        }
        if (e.files.length <= 0) {
            $scope.checkloading = false; // Đặt lại biến checkloading thành false khi hủy upload
            return;
        }
        var fd = new FormData();
        fd.append("files", e.files[0]);
        cfpLoadingBar.start();
        var post = $http({
            method: 'POST',
            url: apiUrl + 'api/upload/uploadfile',
            data: fd,
            headers: {
                "Content-Type": undefined,
                'Authorization': 'bearer ' + $scope.access_token
            }
        });

        // Thực hiện xử lý khi nhận được phản hồi từ server
        post.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            if (data.status === 200) {
                $scope.otherDocument.link = data.data[0].url;
            } else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(false)
                        .title('Thông báo')
                        .textContent('Tải file không thành công!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(false)
                    .title('Thông báo')
                    .textContent('Tải file không thành công!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        }).finally(function () {
            $scope.checkloading = false; // Đặt lại biến checkloading thành false sau khi hoàn thành tất cả các xử lý
            e.value = ""; // Reset giá trị của input file để đảm bảo onchange luôn được kích hoạt
        });
    }

    $scope.getFileName = function (link) {
        var parts = link.split('/');
        return parts[parts.length - 1];
    };

    //Tai cac file dinh kem GPKD
    $scope.uploadFilePartner = function (e) {
        $scope.checkloading = true; // Biến để kiểm soát hiển thị hiệu ứng loading
        if (e === undefined) {
            $scope.checkloading = false; // Đặt lại biến checkloading thành false khi hủy upload
            return;
        }
        if (e.files.length <= 0) {
            $scope.checkloading = false; // Đặt lại biến checkloading thành false khi hủy upload
            return;
        }

        // Khởi tạo FormData ngoài vòng lặp để tránh gửi các yêu cầu không đồng bộ
        var fd = new FormData();

        for (var i = 0; i < e.files.length; i++) {
            fd.append("files", e.files[i]); // Thêm từng file vào FormData
        }

        var post = $http({
            method: 'POST',
            url: apiUrl + 'api/upload/uploadfile',
            data: fd,
            headers: {
                "Content-Type": undefined
            }
        });

        cfpLoadingBar.start();

        post.success(function successCallback(data, status, headers, config) {
            $scope.checkloading = false; // Biến để kiểm soát hiển thị hiệu ứng loading
            cfpLoadingBar.complete();
            if (data.status === 200) {
                let datas = $scope.UpdatePartner.list_documents;
                // Lấy thông tin về các file đã tải lên từ response
                const uploadedFiles = data.data;
                for (var i = 0; i < uploadedFiles.length; i++) {
                    const fileData = uploadedFiles[i];
                    // Thêm mỗi file vào mảng list_documents với cấu trúc đúng
                    datas.push({
                        file_name: fileData.name,
                        links: fileData.url,
                        id: fileData.id // Chú ý: Đặt id của file tải lên vào đối tượng list_documents
                    });
                }
                $scope.UpdatePartner.list_documents = datas;
            } else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Tải tài liệu không thành công. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi khi tải tài liệu. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            ).finally(function () {
                $scope.checkloading = false; // Đặt lại biến checkloading thành false khi hoàn thành các xử lý
            });
        });
    };

    // ham revers yyyy/mm/dd
    $scope.reverseDate = function (dateString) {
        var parts = dateString.split("-");
        var reversedDateString = parts[2] + "-" + parts[1] + "-" + parts[0];
        return reversedDateString;
    };
    // convertDate vnpt
    $scope.convertDate = function (dateString) {
        console.log(dateString);
        if (dateString === "Không thời hạn") {
            return '2199-01-01'; // Thay đổi định dạng tháng và ngày để có hai chữ số
        } else {
            const [day, month, year] = dateString.split('/');

            // Đảm bảo rằng tháng và ngày đều có hai chữ số
            const formattedDay = day?.length === 1 ? `0${day}` : day;
            const formattedMonth = month?.length === 1 ? `0${month}` : month;
            const dateConvert = `${year}-${formattedMonth}-${formattedDay}`;
            return dateConvert;
        }
    };
    //convertDate dkkd vnpt
    $scope.convertDateDKKDVPNT = function (input) {
        const dateRegex = /ngày (\d{2}) tháng (\d{2}) năm (\d{4})/;
        const match = input.match(dateRegex);
        if (!match) {
            throw new Error('Invalid date format');
        }
        const [, day, month, year] = match;
        const formattedDate = `${year}-${month}-${day}`;

        return formattedDate;
    };

    // ham revers dd/mm/yyyy
    $scope.reverseDateA = function (dateStr) {
        if (!dateStr) return ''; // Kiểm tra xem chuỗi ngày có tồn tại không

        // Tạo một đối tượng Date từ chuỗi ngày
        var date = new Date(dateStr);

        // Lấy ra ngày, tháng và năm từ đối tượng Date
        var day = date.getDate();
        var month = date.getMonth() + 1; // Lưu ý: Tháng trong JavaScript bắt đầu từ 0
        var year = date.getFullYear();

        // Tạo chuỗi ngày tháng năm mới, bỏ qua giờ, phút và giây
        var formattedDate = day + '/' + month + '/' + year;

        return formattedDate;
    };

    $scope.uploadCMNDMT = function (e) {
        $scope.checkloading = true;
        if (e === undefined) {
            $scope.checkloading = false;
            return;
        }
        if (e.files.length <= 0) {
            $scope.checkloading = false;
            return;
        }
        var cmndmt = document.getElementById("cccd1");
        var cmnd1 = document.getElementById("uploadedImage2");
        var fd = new FormData();
        fd.append("files", e.files[0]);
        cfpLoadingBar.start();
        var post = $http({
            method: 'POST',
            url: apiUrl + 'api/upload/uploadfile',
            data: fd,
            headers: {
                "Content-Type": undefined,
                'Authorization': 'bearer ' + $scope.access_token
            }
        });
        // haohv 
        post.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            if (data.status == 200) {
                $scope.UpdatePartner.identifier_front_image = data.data[0].url;
                cmndmt.style.display = "block";
                cmnd1.style.display = "none";
                $scope.checkloading = false; // đặt lại biến checkloading thành false khi hoàn thành các xử lý
                var dataCCCDMT = {
                    "request_id": "CCCD_mattruoc",
                    "image": data.data[0].url
                }

                //upload lay ma hash
                var fileUploadFormData = new FormData();
                fileUploadFormData.append("file", e.files[0]);
                fileUploadFormData.append("title", "upload");
                fileUploadFormData.append("description", "upload ảnh CCCD mặt trước");


                // chỉ gọi api ocr cccd khi là mô hình cá nhân hoặc gpdkkd bị lỗi
                if ($scope.UpdatePartner.store_type_id == 7 || $scope.checkDataOcrGPDKKD === true) {
                    $http({
                        method: 'POST',
                        url: $scope.apiVNPT + "/file-service/v1/addFile",
                        headers: {
                            "Content-Type": undefined,
                            'Authorization': 'Bearer ' + $scope.accesstokenVNPT,
                            "Token-id": $scope.tokenIdVnpt,
                            "Token-key": $scope.tokenKeyVnpt
                        },
                        data: fileUploadFormData,
                    }).then(function (response) {
                        if (response.data.message && response.data.message === "IDG-00000000") {
                            var ocrCCCD = {
                                "img_front": response.data.object.hash,
                                "client_session": "ANDROID_nokia7.2_28_Simulator_2.4.2_08d2d8686ee5fa0e_1581910116532",
                                "type": -1,
                                "validate_postcode": true,
                                "token": "cccdmattruoc"
                            }
                            // goi api ocr cccd
                            $http({
                                method: 'POST',
                                url: $scope.apiVNPT + "/ai/v1/ocr/id/front",
                                headers: {
                                    "Content-Type": "application/json",
                                    'Authorization': 'Bearer ' + $scope.accesstokenVNPT,
                                    "Token-id": $scope.tokenIdVnpt,
                                    "Token-key": $scope.tokenKeyVnpt,
                                    "mac-address": "CCCDMT1607"
                                },
                                data: ocrCCCD,
                            }).then(function (response) {
                                $scope.checkloading = false;
                                if (response.data.object.general_warning && response.data.object.general_warning.includes('qua_han_mat_truoc')) {
                                    $scope.UpdatePartner.identifier_front_image = null;
                                    $mdDialog.show(
                                        $mdDialog.alert()
                                            .clickOutsideToClose(true)
                                            .title('Thông báo')
                                            .textContent('CCCD đã quá hạn sử dụng. Vui lòng kiểm tra lại')
                                            .ok('Đóng')
                                            .fullscreen(false)
                                    );
                                    return
                                }
                                if (response.data.object.general_warning && response.data.object.general_warning.includes('id_mo_nhoe')) {
                                    $mdDialog.show(
                                        $mdDialog.alert()
                                            .clickOutsideToClose(true)
                                            .title('Thông báo')
                                            .textContent('Thông tin căn cước bị mờ nhòe. Vui lòng kiểm tra lại')
                                            .ok('Đóng')
                                            .fullscreen(false)
                                    );
                                    return
                                }


                                if (response && response.status === 200) {
                                    //ten cccd
                                    //if (!$scope.UpdatePartner.license_owner) {
                                    $scope.UpdatePartner.license_owner = response.data.object.name
                                    //}
                                    // ngay sinh
                                    //if (!$scope.UpdatePartner.license_birth_date) {
                                    try {
                                        $scope.reversedDate = $scope.convertDate(response.data.object.birth_day);
                                        var birthday = new Date($scope.reversedDate)
                                        $scope.UpdatePartner.license_birth_date = birthday
                                    } catch (e) {
                                        console.error(e)
                                    }
                                    //}
                                    // Số CCCD
                                    //if (!$scope.UpdatePartner.indetifier_no) {
                                    try {
                                        $scope.UpdatePartner.indetifier_no = response.data.object.id
                                    } catch (e) {
                                        console.error(e)
                                    }
                                    //}
                                    // ngay het han
                                    //if (!$scope.UpdatePartner.identifier_date_expire) {
                                    try {
                                        $scope.reversedDate = $scope.convertDate(response.data.object.valid_date);
                                        var date_expire = new Date($scope.reversedDate);
                                        $scope.UpdatePartner.identifier_date_expire = date_expire;

                                        var currentDate = new Date();
                                        var oneMonthLater = new Date();
                                        oneMonthLater.setMonth(currentDate.getMonth() + 1);

                                        // Tạo biến để lưu kết quả so sánh
                                        $scope.isExpireSoon = date_expire <= oneMonthLater;

                                        if ($scope.isExpireSoon) {
                                            // Hiển thị cảnh báo cho người dùng
                                            $mdDialog.show(
                                                $mdDialog.alert()
                                                    .clickOutsideToClose(true)
                                                    .title('Thông báo')
                                                    .textContent('Ngày cấp của CCCD sắp hết hạn sử dụng. Vui lòng kiểm tra lại!')
                                                    .ok('Đóng')
                                                    .fullscreen(false)
                                            );
                                        }
                                    } catch (e) {
                                        console.error(e)
                                    }
                                    //}
                                    //ngay cap - truong hop la ho chieu                             
                                    //if (response.data.object.issue_date !== "" || response.data.object.issue_date !== "-") {
                                    //    $scope.reversedDate = $scope.convertDate(response.data.object.issue_date);
                                    //    var issue_date = new Date($scope.reversedDate)
                                    //    $scope.UpdatePartner.identifier_date = issue_date
                                    //}
                                    //noi cap - truong hop la ho chieu
                                    if (response.data.object.issue_place !== "" || response.data.object.issue_place !== "-") {
                                        $scope.UpdatePartner.identifier_at = response.data.object?.issue_place.replace(/\n/g, ' ')
                                    }
                                    // dia chỉ thuong tru
                                    //if (!$scope.UpdatePartner.identifier_address) {
                                    $scope.UpdatePartner.identifier_address = response.data.object.recent_location.replace(/\n/g, ', ')
                                    //}
                                    // gioi tinh
                                    let dataGender = response.data.object.gender;
                                    let selectedGender = $scope.genders.find(g => g.name === dataGender);
                                    $scope.UpdatePartner.gender = selectedGender.value;
                                    //Quê quán
                                    $scope.UpdatePartner.hometown = response.data.object.origin_location

                                    // quoc tich 
                                    //if (!$scope.UpdatePartner.identifier_nation_id) {
                                    $scope.UpdatePartner.identifier_nation_id = 20000
                                    //}
                                    // quoc gia
                                    //if (!$scope.UpdatePartner.license_nation_id) {
                                    $scope.UpdatePartner.license_nation_id = 20000
                                    //}

                                    // thanh pho 
                                    //if (!$scope.UpdatePartner.identifier_province_id) {
                                    var get = $http({
                                        method: 'GET',
                                        url: apiUrl + 'api/portal/province',
                                        headers: {
                                            "Content-Type": undefined,
                                            'Authorization': 'bearer ' + $scope.access_token
                                        }
                                    });
                                    get.success(function successCallback(data, status, headers, config) {
                                        var idProvince = '';
                                        var listProvince = data?.data
                                        var foundProvinceId = null;
                                        var addressMrc = response.data.object.recent_location
                                        var addressParts = addressMrc.split(',');
                                        var cityNameMrc = addressParts[addressParts.length - 1].trim();


                                        for (let i = 0; i < listProvince.length; i++) {
                                            if (listProvince[i].name === cityNameMrc) {
                                                foundProvinceId = listProvince[i].id;
                                                break; // Dừng vòng lặp khi tìm thấy
                                            }
                                        }
                                        var valueProvince = {
                                            id: foundProvinceId,
                                            name: cityNameMrc

                                        };
                                        $scope.changeValueProvince(valueProvince) // Gọi hàm chọn thành phố để hiển thị danh sách quận/huyện
                                        $scope.UpdatePartner.identifier_province_id = foundProvinceId

                                        // Chỉ tiếp tục gọi API quận/huyện/phường/xã nếu tìm thấy idProvince
                                        if (foundProvinceId) {
                                            console.log(foundProvinceId)
                                            // Gọi API lấy thông tin quận/huyện/phường/xã
                                            var addressDistrict = null;
                                            var postCodeData = response.data.object.post_code;

                                            // Tìm tên quận/huyện từ dữ liệu post_code
                                            for (let i = 0; i < postCodeData.length; i++) {
                                                if (postCodeData[i].type === "address") {
                                                    addressDistrict = postCodeData[i].district[1]; // Lấy tên quận/huyện
                                                    break;
                                                }
                                            }

                                            var getDistricts = $http({
                                                method: 'GET',
                                                url: apiUrl + `api/portal/provinceBy/` + foundProvinceId, // Sử dụng foundProvinceId trong URL
                                                headers: {
                                                    "Content-Type": undefined,
                                                    'Authorization': 'bearer ' + $scope.access_token
                                                }
                                            });

                                            getDistricts.success(function successCallback(data, status, headers, config) {
                                                var listDistricts = data?.data;

                                                // Tìm quận/huyện theo tên trong danh sách API
                                                var foundDistrict = listDistricts.find(function (district) {
                                                    return district.name === addressDistrict;
                                                });
                                                console.log(foundDistrict)

                                                if (foundDistrict) {
                                                    $scope.changeValueDistrict(foundDistrict)
                                                    $scope.UpdatePartner.user_district_id = foundDistrict.id;

                                                    // Tiếp tục gọi API lấy danh sách phường/xã dựa vào id quận/huyện
                                                    var addressWard = null;
                                                    for (let i = 0; i < postCodeData.length; i++) {
                                                        if (postCodeData[i].type === "address") {
                                                            addressWard = postCodeData[i].ward[1]; // Lấy tên phường/xã
                                                            break;
                                                        }
                                                    }

                                                    var getWards = $http({
                                                        method: 'GET',
                                                        url: apiUrl + `api/portal/provinceBy/${foundDistrict.id}`, // Sử dụng foundDistrict.id trong URL
                                                        headers: {
                                                            "Content-Type": undefined,
                                                            'Authorization': 'bearer ' + $scope.access_token
                                                        }
                                                    });

                                                    getWards.success(function successCallback(data, status, headers, config) {
                                                        var listWards = data?.data;

                                                        // Tìm phường/xã theo tên trong danh sách API
                                                        var foundWard = listWards.find(function (ward) {
                                                            return ward.name === addressWard;
                                                        });

                                                        if (foundWard) {
                                                            $scope.UpdatePartner.user_ward_id = foundWard.id;
                                                        } else {
                                                            console.error("Không tìm thấy phường/xã tương ứng");
                                                        }
                                                    });

                                                } else {
                                                    console.error("Không tìm thấy quận/huyện tương ứng");
                                                }
                                            });
                                        } else {
                                            console.error("Không tìm thấy tỉnh/thành phố tương ứng");
                                        }
                                    });

                                    //}
                                } else {
                                    $scope.checkloading = false;
                                    $mdDialog.show(
                                        $mdDialog.alert()
                                            .clickOutsideToClose(true)
                                            .title('Thông báo')
                                            .textContent('Đã xảy ra lỗi trong quá trình đọc. Vui lòng thử lại')
                                            .ok('Đóng')
                                            .fullscreen(false)
                                    )
                                    return
                                }
                            }).catch(function (error) {
                                $scope.checkloading = false;
                                console.log(error)
                                $mdDialog.show(
                                    $mdDialog.alert()
                                        .clickOutsideToClose(true)
                                        .title('Thông báo')
                                        .textContent('Không đọc được thông tin trong ảnh.')
                                        .ok('Đóng')
                                        .fullscreen(false)
                                )
                                return
                            })
                        } else {
                            $scope.checkloading = false;
                            $mdDialog.show(
                                $mdDialog.alert()
                                    .clickOutsideToClose(true)
                                    .title('Thông báo')
                                    .textContent('Tải ảnh không thành công. Vui lòng thử lại')
                                    .ok('Đóng')
                                    .fullscreen(false)
                            )
                            return
                        }
                    }).catch(function (error) {
                        $scope.checkloading = false;
                        console.error(error);
                    })
                }

            } else {
                $scope.checkloading = false;
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Tải ảnh CMND/CCCD/Hộ chiếu mặt trước không thành công')
                        .ok('Đóng')
                        .fullscreen(false)
                )

            }
        }).error(function (data, status, headers, config) {
            cfpLoadingBar.complete();
            $scope.checkloading = false; // Đặt lại biến checkloading thành false khi hoàn thành các xử lý
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi khi tải ảnh CMND/CCCD/Hộ chiếu mặt trước. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            ).finally(function () {
                $scope.checkloading = false; // Đặt lại biến checkloading thành false khi hoàn thành các xử lý
            });
        });
    };

    $scope.uploadCMNDMS = function (e) {
        $scope.checkloading = true; // Biến để kiểm soát hiển thị hiệu ứng loading
        if (e === undefined) {
            $scope.checkloading = false; // Đặt lại biến checkloading thành false khi hủy upload
            return;
        }
        if (e.files.length <= 0) {
            $scope.checkloading = false; // Đặt lại biến checkloading thành false khi hủy upload
            return;
        }
        var cmndmt = document.getElementById("cccd2");
        var cmnd1 = document.getElementById("uploadedImage3");
        var fd = new FormData();
        fd.append("files", e.files[0]);
        cfpLoadingBar.start();

        var post = $http({
            method: 'POST',
            url: apiUrl + 'api/upload/uploadfile',
            data: fd,
            headers: {
                "Content-Type": undefined,
                'Authorization': 'bearer ' + $scope.access_token
            }
        });
        // haohv 

        post.success(function successCallback(data, status, headers, config) {
            post.success(function successCallback(data, status, headers, config) {
                cfpLoadingBar.complete();
                $scope.checkloading = false; // Đặt lại biến checkloading thành false khi hủy upload
                if (data.status == 200) {

                    $scope.UpdatePartner.identifier_back_image = data.data[0].url;
                    cmndmt.style.display = "block";
                    cmnd1.style.display = "none";


                    //upload lay ma hash
                    var fileUploadFormData = new FormData();
                    fileUploadFormData.append("file", e.files[0]);
                    fileUploadFormData.append("title", "upload");
                    fileUploadFormData.append("description", "upload ảnh CCCD mặt sau");

                    // chỉ gọi api ocr cccd khi là mô hình cá nhân
                    if ($scope.UpdatePartner.store_type_id == 7 || $scope.checkDataOcrGPDKKD === true) {
                        $http({
                            method: 'POST',
                            url: $scope.apiVNPT + "/file-service/v1/addFile",
                            headers: {
                                "Content-Type": undefined,
                                'Authorization': 'Bearer ' + $scope.accesstokenVNPT,
                                "Token-id": $scope.tokenIdVnpt,
                                "Token-key": $scope.tokenKeyVnpt
                            },
                            data: fileUploadFormData,
                        }).then(function (response) {
                            $scope.checkloading = false;
                            if (response.data.message && response.data.message === "IDG-00000000") {
                                var ocrCCCD = {
                                    "img_back": response.data.object.hash,
                                    "client_session": "ANDROID_nokia7.2_28_Simulator_2.4.2_08d2d8686ee5fa0e_1581910116532",
                                    "type": -1,

                                    "token": "cccdmatsau"
                                }
                                // goi api ocr cccd
                                $http({
                                    method: 'POST',
                                    url: $scope.apiVNPT + "/ai/v1/ocr/id/back",
                                    headers: {
                                        "Content-Type": "application/json",
                                        'Authorization': 'Bearer ' + $scope.accesstokenVNPT,
                                        "Token-id": $scope.tokenIdVnpt,
                                        "Token-key": $scope.tokenKeyVnpt,
                                        "mac-address": "CCCDMS1607"
                                    },
                                    data: ocrCCCD,
                                }).then(function (response) {
                                    // ngay cap
                                    //if (!$scope.UpdatePartner.identifier_date) {
                                    $scope.reversedDate = $scope.convertDate(response.data.object.issue_date);
                                    var issue_date = new Date($scope.reversedDate)
                                    $scope.UpdatePartner.identifier_date = issue_date
                                    //}
                                    // noi cap
                                    //if (!$scope.UpdatePartner.identifier_at) {
                                    $scope.UpdatePartner.identifier_at = response.data.object.issue_place.replace(/\n/g, ' ')
                                    //}
                                    //Đặc điểm nhận dạng
                                    $scope.UpdatePartner.identifiers = response.data.object.features

                                }).catch(function (error) {
                                    console.error(error);
                                })
                            } else {
                                $mdDialog.show(
                                    $mdDialog.alert()
                                        .clickOutsideToClose(true)
                                        .title('Thông báo')
                                        .textContent('Tải ảnh không thành công. Vui lòng thử lại')
                                        .ok('Đóng')
                                        .fullscreen(false)
                                )
                            }
                        }).catch(function (error) {
                            $scope.checkloading = false;
                            console.error(error);
                        })
                    }


                } else {
                    $scope.checkloading = false; // Biến để kiểm soát hiển thị hiệu ứng loading
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông báo')
                            .textContent('Tải ảnh CMND/CCCD/Hộ chiếu mặt sau không thành công')
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }).error(function (data, status, headers, config) {
                cfpLoadingBar.complete();
                $scope.checkloading = false; // Đặt lại biến checkloading thành false khi hoàn thành các xử lý
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi khi tải ảnh CMND/CCCD/Hộ chiếu mặt sau. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                )
            });

        });
    };

    $scope.onChangeBankName = function () {
        //
    };

    $scope.onChangeFranchiseBrand = function () {
        console.log('franchise_brand: ', $scope.UpdatePartner.franchise_brand);
    };

    $scope.onChangeGcCode = function (selected) {
        /*reset lại giá trị*/
        if (selected && selected?.group_id) $scope.UpdatePartner.mcc_code = null;
        $scope.mcc_description = '';
        /*set lại danh sách ngành của nhóm*/
        let industries = $scope.industries;
        let group = $scope.UpdatePartner.gc_code ? $scope.UpdatePartner.gc_code : '';
        if (group) $scope.listMccByGroup = $scope.industries.filter(item => item.group_code == group);
        else $scope.listMccByGroup = [];
    };

    $scope.onChangeMccCode = function (selected) {
        if (selected && selected?.id) $scope.UpdatePartner.industry_id = selected.id;
        let mcc = $scope.industries.filter(item => item.code == $scope.UpdatePartner.mcc_code);
        if (mcc && mcc.length > 0) {
            $scope.mcc_description = mcc[0].definition_desc;
        }
    };

    $scope.onChangeSettlementByBranch = function (selected) {
        if (selected == $scope.enums.settlementByBranch.QT_VE_CO_SO) {
            $scope.showAddChain = true; //show button thêm chuỗi
        } else {
            $scope.showAddChain = true; // luôn hiện
        }
    };

    $scope.getSubMerchants = function () {
        //
    };

    $scope.openAddSubMerchant = function () {
        //
    };

    $scope.openEditSubMerchant = function () {
        //
    };

    $scope.onChangeListAccNoStatus = function () {
        //
    };

    $scope.onChangeGender = function () {
        //
    };

    $scope.onChangeDocumentType = function () {
        //
    };

    $scope.onChangeQtCoSo = function (val) {
        //
    };


    $scope.onChangePaymentRealtime = function () {
        console.log('payment realtime: ', $scope.UpdatePartner.payment_realtime1);
        //show popup thông báo
        if (!$scope.UpdatePartner.payment_realtime1) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(false)
                    .title('Thông báo')
                    .textContent('Đối tác cần tích chọn vào đây để nhận tiền thanh toán Realtime áp dụng với user đã cài đặt CashPlus.')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        }
    };

    $scope.onChangePaymentRealtime2 = function () {
        console.log('payment realtime: ', $scope.UpdatePartner.payment_realtime2);
        //show popup thông báo
        if (!$scope.UpdatePartner.payment_realtime2) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(false)
                    .title('Thông báo')
                    .textContent('Đối tác cần tích chọn vào đây để nhận tiền thanh toán Realtime áp dụng với user chưa cài đặt CashPlus.')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        }
    };

    $scope.onChangeBankAcc = function (selected) {
        if (selected && selected?.description) {
            $scope.bankName = selected.description;
        }
        if ($scope.UpdatePartner.acc_numb && $scope.UpdatePartner.bank_code) {
            $scope.handleCheckBankAccount($scope.UpdatePartner.bank_code, $scope.UpdatePartner.acc_numb, true);
        }
    };

    $scope.onChangeAccountName = function () {
        if ($scope.UpdatePartner.acc_numb && $scope.UpdatePartner.bank_code) {
            $scope.handleCheckBankAccount($scope.UpdatePartner.bank_code, $scope.UpdatePartner.acc_numb, true);
        }
    };

    $scope.onChangeBankAcc2 = function (selected) {
        if ($scope.bankAccount.acc_numb && $scope.bankAccount.bank_code) {
            $scope.handleCheckBankAccount($scope.bankAccount.bank_code, $scope.bankAccount.acc_numb, false);
        }
    };

    $scope.onChangeAccountName2 = function () {
        if ($scope.bankAccount.acc_numb && $scope.bankAccount.bank_code) {
            $scope.handleCheckBankAccount($scope.bankAccount.bank_code, $scope.bankAccount.acc_numb, false);
        }
    };

    $scope.onChangeSubMerchantStatus = function () {
        //
    };

    $scope.onChangeSubMerchantBank = function () {
        //
    };

    $scope.onChangeRepresentativeTitle = function () {
        //
    };

    $scope.onChangeRepresentativeJob = function () {
        //
    };

    $scope.toggleShowPassword = function () {
        $scope.showPassword = !$scope.showPassword;
    };

    $scope.validateEmail = function (value) {
        return value && /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
    }

    $scope.validateBranchRepTaxNo = function (value) {
        return value && /^(?=.*\d)[\d-]{10,14}$/.test(value);
    }

    $scope.validateIsNumber = function (value) {
        return value && /^\d+$/.test(value);
    }

    $scope.validateAccountName = function (value) {
        return value && /^[a-zA-Z0-9]+$/.test(value);
    }

    $scope.validateDiscountRate = function (value) {
        return value && /^(?:[1-9]\d(?:\.\d+)?|10(?:\.\d+)?|99(?:\.\d+)?|[1-9]\d\.\d+)$/.test(value);
    }

    $scope.validatePhone = function (value) {
        return value && /[0][0-9\s.-]{9,13}/.test(value);
    }

    $scope.validatePhoneMax10 = function (value) {
        return value && /^0\d{9}$/.test(value);
    }

    var modalBankAccountEdit;
    $scope.openBankAccountEditModal = function (accountType, banks, checkAuthDocNumb, isSub, item, index) {
        if (item) {
            $scope.title = "Sửa thông tin tài khoản";
            $scope.bankAccount.index = index;
            $scope.bankAccount.id = item.id;
            $scope.bankAccount.is_default = item.is_default;
            $scope.bankAccount.acc_numb = item.acc_numb;
            $scope.bankAccount.status = 1;
            $scope.bankAccount.acc_holder = item.acc_holder;
            $scope.bankAccount.disabledAccHolder = item.type == $scope.enums.accountType.CHINH_CHU;
            $scope.bankAccount.bank_code = item.bank_code;
            $scope.bankAccount.type = item.type;
            $scope.bankAccount.auth_doc_numb = item.auth_doc_numb;
            $scope.bankAccount.auth_doc_link = item.auth_doc_link;
        } else {
            $scope.title = "Thêm thông tin tài khoản";
            $scope.bankAccount.acc_numb = '';
            $scope.bankAccount.status = 1;
            $scope.bankAccount.acc_holder = '';
            $scope.bankAccount.disabledAccHolder = false;
            $scope.bankAccount.bank_code = null;
            $scope.bankAccount.type = null;
            $scope.bankAccount.auth_doc_numb = '';
            $scope.bankAccount.auth_doc_link = '';
            $scope.bankAccount.is_default = false;
        }
        //$scope.bankAccount.is_default = false;
        $scope.isSub = isSub === 'true';
        $scope.accountType = accountType;
        $scope.banks = banks;
        $scope.checkAuthDocNumb = checkAuthDocNumb;
        if ($scope.showFormUserCN) $scope.accountType = accountType.filter(item => item.value !== $scope.enums.accountType.UY_QUYEN);

        modalBankAccountEdit = $uibModal.open({
            templateUrl: '/template/angular/bank-account-edit.html',
            windowClass: 'fade modal-backdrop in',
            backdrop: 'static',
            scope: $scope,
            size: 'lg'
        });
    }

    $scope.closeBankAccountEdit = function () {
        $scope.bankAccount = {};
        modalBankAccountEdit.dismiss('cancel');
    };

    $scope.saveBankAccountEdit = function (isSub) {
        let check = $scope.bankAccountInput.filter(item => item.acc_numb == $scope.bankAccount.acc_numb && item.bank_code == $scope.bankAccount.bank_code && item.id != $scope.bankAccount.id);

        if (check && check.length > 0) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(false)
                    .title('Thông báo')
                    .textContent('Tài khoản đã tồn tại.')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                $scope.scrollToElementCentered('acc_numb');
            });
            return;
        }
        if ($scope.validateIsNumber($scope.bankAccount.acc_numb)
            && $scope.bankAccount.acc_holder
            && $scope.bankAccount.bank_code
            && [0, 1, 2].includes($scope.bankAccount.type)
        ) {
            let index = $scope.bankAccount.index;
            let obj = {
                id: $scope.bankAccount?.id ? $scope.bankAccount.id : moment().valueOf(),
                is_default: $scope.bankAccount.is_default,
                acc_numb: $scope.bankAccount.acc_numb,
                acc_holder: $scope.bankAccount.acc_holder,
                status: 1, //default
                type: $scope.bankAccount.type,
                bank_code: $scope.bankAccount.bank_code,
                auth_doc_numb: $scope.bankAccount.auth_doc_numb,
                auth_doc_link: $scope.bankAccount.auth_doc_link
            }
            if ($scope.bankAccount.is_default) {
                $scope.bankAccountAdds = $scope.bankAccountAdds.map(item => {
                    item.is_default = false;
                    return item;
                });

                $scope.bankAccounts = $scope.bankAccounts.map(item => {
                    item.is_default = false;
                    return item;
                });
            }
            if (index !== undefined) {
                const updatedArray = $scope.bankAccountAdds.map(item => {
                    if (item.id == obj.id) {
                        return {
                            ...item,
                            is_default: obj.is_default,
                            acc_numb: obj.acc_numb,
                            status: obj.status,
                            type: obj.type,
                            bank_code: obj.bank_code,
                            auth_doc_numb: obj.auth_doc_numb,
                            auth_doc_link: obj.auth_doc_link
                        };
                    }
                    return item;
                });
                $scope.bankAccountAdds = updatedArray;
            } else {
                $scope.bankAccountAdds.push(obj);
            }
            if (isSub) $scope.merchant.account_id = obj.id;
            $scope.hasbankAccount = $scope.bankAccountAdds && $scope.bankAccountAdds.length > 0;
            modalBankAccountEdit.dismiss('cancel');
        } else {
            console.log('form bank account error')
            // Form is invalid, show error messages
            $scope.formBankAccount.$submitted = true;
        }
    };

    $scope.delBankAccountEdit = function (bank) {
        let checkUse = $scope.subMerchants.filter(item => bank.acc_numb == item.acc_numb && bank.bank_code == item.bank_code);
        if (checkUse && checkUse.length > 0) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(false)
                    .title('Thông báo')
                    .textContent('Không thể xóa. Tài khoản đã được thêm trong chuỗi')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        } else {
            $mdDialog.show(
                $mdDialog.confirm()
                    .clickOutsideToClose(false)
                    .title('Xác nhận')
                    .textContent('Bạn có chắc chắn muốn xóa?')
                    .cancel('Thoát')
                    .ok('Xóa')
                    .fullscreen(false)
            ).then(function () {
                $scope.bankAccountAdds = $scope.bankAccountAdds.filter((item, index) => item.id != bank.id);
                $scope.hasbankAccount = $scope.bankAccountAdds && $scope.bankAccountAdds.length > 0;
            });
        }
    }

    var modalListContactEdit;
    $scope.openListContactEditModal = function (contactTypes, item, index) {
        //if(!item) $scope.contact.info_representative = false;
        let disabledValues = $scope.contacts.map(item => item.type);
        if (item) {
            $scope.title = "Sửa thông tin liên hệ";
            $scope.contact.index = index;
            $scope.contact.info_representative = item.info_representative;
            $scope.contact.id = item.id;
            $scope.contact.type = [item.type];
            $scope.contact.name = item.name;
            $scope.contact.phone = item.phone;
            $scope.contact.email = item.email;
        } else {
            $scope.contact = {}
            $scope.title = "Thêm thông tin liên hệ";
            $scope.contact.info_representative = false;
            $scope.contact.id = null;
            $scope.contact.type = null;
            $scope.contact.name = '';
            $scope.contact.phone = '';
            $scope.contact.email = '';
        }
        if (disabledValues && disabledValues.length === 2) {
            $scope.types = contactTypes.filter(ele => ele.id == item.type);
        } else if (disabledValues && disabledValues.length === 1) {
            if (item && item.id) {
                $scope.types = contactTypes;
            } else {
                $scope.types = contactTypes.filter(ele => !disabledValues.includes(ele.id));
            }
        } else {
            $scope.types = contactTypes;
        }
        /*$scope.types = contactTypes.map(item => {
            if(disabledValues.includes(item.id)) {
                item.disabled = true;
            }
            return item;
        });*/

        modalListContactEdit = $uibModal.open({
            templateUrl: '/template/angular/list-contact-edit.html',
            windowClass: 'fade modal-backdrop in',
            backdrop: 'static',
            scope: $scope,
            size: 'lg'
        });
    }

    $scope.closeListContactEdit = function () {
        $scope.contact = {};
        modalListContactEdit.dismiss('cancel');
    };

    $scope.saveListContactEdit = function () {
        if ($scope.contact.name
            && $scope.contact.type
            && $scope.validatePhoneMax10($scope.contact.phone)
            && $scope.validateEmail($scope.contact.email)
        ) {
            let index = $scope.contact.index;
            if ($scope.contact.type && $scope.contact.type.length > 0) {
                if ($scope.contact.type.length === 2) $scope.contacts = [];
                $scope.contact.type.map((item) => {
                    let obj = {
                        id: $scope.contact?.id ? $scope.contact.id : moment().valueOf(),
                        name: $scope.contact.name,
                        type: item,
                        phone: $scope.contact.phone,
                        email: $scope.contact.email,
                        info_representative: $scope.contact.info_representative
                    }
                    if (index !== undefined) {
                        if ($scope.contact.type.length === 2) {
                            $scope.contacts.push(obj);
                        } else {
                            $scope.contacts.splice(index, 1, obj);
                        }
                    } else {
                        $scope.contacts.push(obj);
                    }
                });
            }

            $scope.hasContact = $scope.contacts && $scope.contacts.length > 0;
            $scope.fullContact = $scope.contacts && $scope.contacts.length === 2;
            modalListContactEdit.dismiss('cancel');
        } else {
            console.log('form contact error')
            // Form is invalid, show error messages
            $scope.formListContact.$submitted = true;
        }
    };

    $scope.delListContactEdit = function (idx) {
        $mdDialog.show(
            $mdDialog.confirm()
                .clickOutsideToClose(false)
                .title('Xác nhận')
                .textContent('Bạn có chắc chắn muốn xóa?')
                .cancel('Thoát')
                .ok('Xóa')
                .fullscreen(false)
        ).then(function () {
            $scope.contacts = $scope.contacts.filter((item, index) => index != idx);
            $scope.hasContact = $scope.contacts && $scope.contacts.length > 0;
            $scope.fullContact = $scope.contacts && $scope.contacts.length === 2;
        });
    }

    var modalListDocumentEdit;
    $scope.openListDocumentEditModal = function (otherDocumentTypes, item, index) {
        if (item) {
            $scope.title = "Sửa thông tin tài liệu";
            $scope.otherDocument.index = index;
            $scope.otherDocument.id = item.id;
            $scope.otherDocument.name = item.name;
            $scope.otherDocument.link = item.link;
        } else {
            $scope.title = "Thêm thông tin tài liệu";
            $scope.otherDocument.name = '';
            $scope.otherDocument.link = '';
        }

        $scope.otherDocumentTypes = otherDocumentTypes;

        modalListDocumentEdit = $uibModal.open({
            templateUrl: '/template/angular/list-document-edit.html',
            windowClass: 'fade modal-backdrop in',
            backdrop: 'static',
            scope: $scope,
            size: 'lg'
        });
    }

    $scope.closeListDocumentEdit = function () {
        $scope.otherDocument = {};
        modalListDocumentEdit.dismiss('cancel');
    };

    $scope.saveListDocumentEdit = function () {
        if ($scope.otherDocument.name && $scope.otherDocument.link) {
            let index = $scope.otherDocument.index;
            let obj = {
                id: $scope.otherDocument?.id ? $scope.otherDocument.id : moment().valueOf(),
                name: $scope.otherDocument.name,
                link: $scope.otherDocument.link
            }
            if (index !== undefined) {
                const updatedArray = $scope.otherDocuments.map(item => {
                    if (item.id == obj.id) {
                        return {
                            ...item,
                            name: obj.name,
                            link: obj.link
                        };
                    }
                    return item;
                });
                $scope.otherDocuments = updatedArray;
            } else {
                $scope.otherDocuments.push(obj);
            }
            $scope.hasOtherDocument = $scope.otherDocuments && $scope.otherDocuments.length > 0;
            modalListDocumentEdit.dismiss('cancel');
        } else {
            console.log('form document error')
            // Form is invalid, show error messages
            $scope.formListDocument.$submitted = true;
        }
    };

    $scope.delListDocumentEdit = function (id) {
        $mdDialog.show(
            $mdDialog.confirm()
                .clickOutsideToClose(false)
                .title('Xác nhận')
                .textContent('Bạn có chắc chắn muốn xóa?')
                .cancel('Thoát')
                .ok('Xóa')
                .fullscreen(false)
        ).then(function () {
            $scope.otherDocuments = $scope.otherDocuments.filter((item, index) => item.id != id);
            $scope.hasOtherDocument = $scope.otherDocuments && $scope.otherDocuments.length > 0;
        });
    }

    var modalListTimeEdit;
    $scope.openListTimeEditModal = function (item, index) {
        if (item) {
            $scope.title = "Sửa thông tin đóng mở cửa";
            $scope.workingTime.index = index;
            $scope.workingTime.id = item.id;
            $scope.workingTime.start_hour = item.start_hour;
            $scope.workingTime.end_hour = item.end_hour;
        } else {
            $scope.title = "Thêm thông tin đóng mở cửa";
            $scope.workingTime.start_hour = '';
            $scope.workingTime.end_hour = '';
        }

        modalListTimeEdit = $uibModal.open({
            templateUrl: '/template/angular/list-time-edit.html',
            windowClass: 'fade modal-backdrop in',
            backdrop: 'static',
            scope: $scope,
            size: 'lg'
        });
    }

    $scope.closeListTimeEdit = function () {
        $scope.workingTime = {};
        modalListTimeEdit.dismiss('cancel');
    };

    $scope.saveListTimeEdit = function () {
        if ($scope.workingTime.start_hour && $scope.workingTime.end_hour) {
            let index = $scope.workingTime.index;
            let obj = {
                id: $scope.workingTime?.id ? $scope.workingTime.id : moment().valueOf(),
                start_hour: $scope.workingTime.start_hour.toLocaleTimeString(),
                end_hour: $scope.workingTime.end_hour.toLocaleTimeString()
            }
            if (index !== undefined) {
                const updatedArray = $scope.workingTimes.map(item => {
                    if (item.id == obj.id) {
                        return {
                            ...item,
                            start_hour: obj.start_hour,
                            end_hour: obj.end_hour
                        };
                    }
                    return item;
                });
                $scope.workingTimes = updatedArray;
            } else {
                $scope.workingTimes.push(obj);
            }
            $scope.hasWorkingTime = $scope.workingTimes && $scope.workingTimes.length > 0;
            modalListTimeEdit.dismiss('cancel');
        } else {
            console.log('form time error')
            // Form is invalid, show error messages
            $scope.formListTime.$submitted = true;
        }
    };

    $scope.delListTimeEdit = function (id) {
        $mdDialog.show(
            $mdDialog.confirm()
                .clickOutsideToClose(false)
                .title('Xác nhận')
                .textContent('Bạn có chắc chắn muốn xóa?')
                .cancel('Thoát')
                .ok('Xóa')
                .fullscreen(false)
        ).then(function () {
            $scope.workingTimes = $scope.workingTimes.filter((item, index) => item.id != id);
            $scope.hasWorkingTime = $scope.workingTimes && $scope.workingTimes.length > 0;
        });
    }

    $scope.getTime = function (time) {
        let isValid = moment(time, ["hh:mm:ss A", "HH:mm:ss"], true).isValid();

        if (isValid) {
            let formattedTime = moment(time, ["hh:mm:ss A", "HH:mm:ss"]).format("HH:mm");
            console.log("Formatted Time:", formattedTime);
            return formattedTime;
        } else {
            return "-";
        }
    }

    var modalSubMerchantEdit;
    $scope.openSubMerchantEditModal = function (bankAccountInput, accountType, banks, checkAuthDocNumb, hasMultiBank, isUserCN, licenseOwner, item, index) {
        if (!$scope.hadInputBankAccount || $scope.bankAccountInput.length === 0 || !$scope.bankAccountInput) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(false)
                    .title('Thông báo')
                    .textContent('Vui lòng điền đầy đủ thông tin STK quyết toán')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                $scope.scrollToElementCentered('bank_account_list');
            });
            return;
        }

        if (!$scope.UpdatePartner.settlement_by_branch) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(false)
                    .title('Thông báo')
                    .textContent('Vui lòng chọn loại quyết toán')
                    .ok('Đóng')
                    .fullscreen(false)
            ).then(function () {
                $scope.scrollToElementCentered('settlement_by_branch');
            });
            return;
        }
        //if(!item) $scope.merchant.info_representative = false;
        if (item) {
            $scope.title = "Sửa chuỗi cơ sở";
            $scope.merchant.index = index;
            $scope.merchant.status = 1;
            $scope.merchant.id = item.id;
            $scope.merchant.info_representative = item.info_representative;
            $scope.merchant.branch_name = item.branch_name;
            $scope.merchant.branch_address = item.branch_address;
            $scope.merchant.account_id = item.account_id;
            $scope.merchant.branch_rep_name = item.branch_rep_name;
            $scope.merchant.branch_rep_yob = item.branch_rep_yob;
            $scope.merchant.branch_rep_tax_no = item.branch_rep_tax_no;
            $scope.merchant.branch_rep_identity_no = item.branch_rep_identity_no;
            $scope.merchant.branch_rep_identity_issue_date = item.branch_rep_identity_issue_date;
            $scope.merchant.branch_rep_identity_issue_place = item.branch_rep_identity_issue_place;
            $scope.merchant.branch_rep_permanent_residence = item.branch_rep_permanent_residence;
            $scope.merchant.cashplus_account_name = item.branch_rep_name;
            //$scope.merchant.discount_rate = item.discount_rate;
        } else {
            let accountDefault = [];
            if ($scope.UpdatePartner.settlement_by_branch == $scope.enums.settlementByBranch.KHONG_QT_VE_CO_SO) {
                accountDefault = $scope.bankAccountInput;
            } else {
                accountDefault = $scope.bankAccountInput.filter(item => item.is_default === true);
            }
            $scope.title = "Thêm chuỗi cơ sở";
            $scope.merchant.status = 1;
            $scope.merchant.info_representative = false;
            $scope.merchant.branch_name = '';
            $scope.merchant.branch_address = '';
            $scope.merchant.account_id = (accountDefault && accountDefault.length > 0) ? accountDefault[0].id : null;
            $scope.merchant.branch_rep_name = '';
            $scope.merchant.branch_rep_yob = null;
            $scope.merchant.branch_rep_tax_no = '';
            $scope.merchant.branch_rep_identity_no = '';
            $scope.merchant.branch_rep_identity_issue_date = null;
            $scope.merchant.branch_rep_identity_issue_place = '';
            $scope.merchant.branch_rep_permanent_residence = '';
            $scope.merchant.cashplus_account_name = '';
            //$scope.merchant.discount_rate = '';
        }

        $scope.disableBankAccount = !hasMultiBank;
        $scope.bankAccountInput = bankAccountInput;
        $scope.accountType = accountType;
        $scope.banks = banks;
        $scope.checkAuthDocNumb = checkAuthDocNumb;
        $scope.isUserCN = isUserCN; // check là cá nhân hay không
        $scope.licenseOwner = licenseOwner; // tên người đại diện

        modalSubMerchantEdit = $uibModal.open({
            templateUrl: '/template/angular/sub-merchant-edit.html',
            windowClass: 'fade modal-backdrop in',
            backdrop: 'static',
            scope: $scope,
            size: 'lg'
        });
    };

    $scope.closeSubMerchantEdit = function () {
        $scope.merchant = {};
        modalSubMerchantEdit.dismiss('cancel');
    };

    $scope.saveSubMerchantEdit = function () {
        if ($scope.merchant.branch_name
            && $scope.merchant.branch_address
            && $scope.merchant.account_id
            && $scope.merchant.branch_rep_name
            && $scope.merchant.branch_rep_yob
            //&& $scope.validateBranchRepTaxNo($scope.merchant.branch_rep_tax_no)
            && $scope.validateIsNumber($scope.merchant.branch_rep_identity_no)
            && $scope.merchant.branch_rep_identity_issue_date
            && $scope.merchant.branch_rep_identity_issue_place
            && $scope.merchant.branch_rep_permanent_residence
            //&& $scope.validateDiscountRate($scope.merchant.discount_rate)
            //&& $scope.validatePhone($scope.merchant.phone)
        ) {
            let index = $scope.merchant.index;
            let account = $scope.bankAccountInput.filter(item => item.id == $scope.merchant.account_id);
            /*if($scope.isUserCN && $scope.toUpperCaseAndRemoveAccents($scope.licenseOwner) != account[0].acc_holder) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(false)
                        .title('Thông báo')
                        .textContent('Số tài khoản không khớp với thông tin người đại diện.')
                        .ok('Đóng')
                        .fullscreen(false)
                );
                return;
            }*/
            let obj = {
                id: $scope.merchant?.id ? $scope.merchant.id : moment().valueOf(),
                branch_name: $scope.merchant.branch_name,
                info_representative: $scope.merchant.info_representative,
                status: 1,
                branch_address: $scope.merchant.branch_address,
                account_id: $scope.merchant.account_id,
                acc_numb: account[0].acc_numb,
                acc_holder: account[0].acc_holder,
                bank_code: account[0].bank_code,
                branch_rep_name: $scope.merchant.branch_rep_name,
                branch_rep_yob: $scope.merchant.branch_rep_yob,
                branch_rep_tax_no: $scope.merchant.branch_rep_tax_no,
                branch_rep_identity_no: $scope.merchant.branch_rep_identity_no,
                branch_rep_identity_issue_date: $scope.merchant.branch_rep_identity_issue_date,
                branch_rep_identity_issue_place: $scope.merchant.branch_rep_identity_issue_place,
                branch_rep_permanent_residence: $scope.merchant.branch_rep_permanent_residence,
                cashplus_account_name: $scope.merchant.branch_name,
                //discount_rate: $scope.merchant.discount_rate,
            }
            if (index !== undefined) {
                $scope.subMerchants.splice(index, 1, obj);
            } else {
                $scope.subMerchants.push(obj);
            }
            modalSubMerchantEdit.dismiss('cancel');
            $scope.hasSubMerchant = $scope.subMerchants && $scope.subMerchants.length > 0;
        } else {
            console.log('form add merchant error')
            // Form is invalid, show error messages
            $scope.formSubMerchant.$submitted = true;
        }
    };

    $scope.delSubMerchant = function (idx) {
        $mdDialog.show(
            $mdDialog.confirm()
                .clickOutsideToClose(false)
                .title('Xác nhận')
                .textContent('Bạn có chắc chắn muốn xóa?')
                .cancel('Thoát')
                .ok('Xóa')
                .fullscreen(false)
        ).then(function () {
            $scope.subMerchants = $scope.subMerchants.filter((item, index) => index != idx);
            $scope.hasSubMerchant = $scope.subMerchants && $scope.subMerchants.length > 0;
        });
    }

    $scope.getIndex = function (index) {
        return index + 1;
    }

    $scope.getSubMerchantStatus = function (value) {
        let arr = $scope.accountStatus.filter(item => item.value == value);
        return arr ? arr[0]?.name : '-';
    }

    $scope.getSubMerchantBankName = function (value) {
        let arr = $scope.banks.filter(item => item.bank_code == value);
        return arr ? arr[0]?.description : '-';
    }

    $scope.getContactByType = function (value) {
        let arr = $scope.contactTypes.filter(item => item.id == value);
        return arr ? arr[0]?.name : '-';
    }

    $scope.getAccountType = function (value) {
        let arr = $scope.accountType.filter(item => item.value == value);
        return arr ? arr[0]?.name : '-';
    }

    $scope.hasSubMerchant = $scope.subMerchants && $scope.subMerchants.length > 0;

    $scope.changeValueDistrictUser = function (selectedItem) {
        if (selectedItem) {
            $scope.selectedDictrict = selectedItem;
            $scope.getWard(selectedItem.id);
            return selectedItem;

        } else {
            $scope.selectedDictrict = {};
        }
    };

    $scope.changeValueWardUser = function (selectedItem) {
        if (selectedItem) {
            $scope.selectedWard = selectedItem;
            return selectedItem;
        } else {
            $scope.selectedWard = {};
        }
    };


    $scope.updateNowAddress = function () {

        if ($scope.UpdatePartner.is_same_address && $scope.UpdatePartner.is_same_address == true) {
            $scope.UpdatePartner.now_address = $scope.UpdatePartner.identifier_address;
            $scope.UpdatePartner.now_nation_id = $scope.UpdatePartner.identifier_nation_id;
            $scope.UpdatePartner.now_province_id = $scope.UpdatePartner.identifier_province_id;
        } else {
            $scope.UpdatePartner.now_address = $scope.UpdatePartner.now_address;
            $scope.UpdatePartner.now_nation_id = $scope.UpdatePartner.now_nation_id;
            $scope.UpdatePartner.now_province_id = $scope.UpdatePartner.now_province_id;
        }
    };

    $scope.getEmployeeData = function () {
        $scope.partner = {};
        var get = $http({
            method: 'GET',
            url: apiUrl + 'api/dropdown/userAdminTest',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.employee_data = response.data.data;
        });
    };
    //Xy ly cac nghiep vu OTP Test
    //1.Xu ly dang ky tai khoan
    $scope.SubmitRecoverOTP = function () {

        if ($scope.RegisterCodeTest?.full_name === '' || $scope.RegisterCodeTest?.full_name === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập họ và tên')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }

        if ($scope.RegisterCodeTest?.email === '' || $scope.RegisterCodeTest?.email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập email')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.phone_number === '' || $scope.RegisterCodeTest?.phone_number === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số điện thoại')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.otp_code === '' || $scope.RegisterCodeTest?.otp_code === undefined) {
            $scope.isOTP = true;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mã OTP')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        $mdDialog.show(
            $mdDialog.alert()
                .clickOutsideToClose(true)
                .title('Thông báo')
                .textContent('Chúc mừng bạn đã đăng ký tài khoản CashPlus thành công!')
                .ok('Đóng')
                .fullscreen(false)
        ).finally(function () {
            $window.location.href = '/'
        });
    }

    $scope.fetchFranchiseBrand = function (searchText) {
        if (!searchText) {
            return;
        }
        //call api
        if (searchText) {
            var post = $http({
                method: 'POST',
                url: apiUrl + `api/portal/searchMerchantName`,
                data: { name: searchText },
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });
            post.then(function (response) {
                let data = response?.data?.data ? response.data.data : [];
                $scope.merchants = (data && data.length > 0) ? data.map(item => {
                    item.label = `${item.code} - ${item.name}`;
                    return item;
                }) : [];
            });
        }
    }

    $scope.restrictInput = function (event) {
        if (['e', 'E', '+', '-', '.'].includes(event.key)) {
            event.preventDefault();
        }
    };

    //2.Thay doi so dien thoai
    $scope.SubmitRecoverPhoneOTP = function () {

        if ($scope.RegisterCodeTest?.phone_number === '' || $scope.RegisterCodeTest?.phone_number === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số điện thoại')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.phone_number_new === '' || $scope.RegisterCodeTest?.phone_number_new === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số điện thoại mới')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.security_code === '' || $scope.RegisterCodeTest?.security_code === undefined) {
            $scope.isOTP = true;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mã bảo mật')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.otp_code === '' || $scope.RegisterCodeTest?.otp_code === undefined) {
            $scope.isOTP = true;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mã OTP')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        $mdDialog.show(
            $mdDialog.alert()
                .clickOutsideToClose(true)
                .title('Thông báo')
                .textContent('Chúc mừng bạn đã thay đổi số điện thoại thành công!')
                .ok('Đóng')
                .fullscreen(false)
        ).finally(function () {
            $window.location.href = '/'
        });
    }
    //3.Quen mat khau
    $scope.SubmitRecoverPassOTP = function () {

        if ($scope.RegisterCodeTest?.phone_number === '' || $scope.RegisterCodeTest?.phone_number === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số điện thoại')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.password === '' || $scope.RegisterCodeTest?.password === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mật khẩu')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.ConfirmPassword === '' || $scope.RegisterCodeTest?.ConfirmPassword === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập lại mật khẩu')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.ConfirmPassword !== $scope.RegisterCodeTest?.password) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Mật khẩu nhập lại chưa chính xác. Vui lòng nhập lại!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.otp_code === '' || $scope.RegisterCodeTest?.otp_code === undefined) {
            $scope.isOTP = true;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mã OTP')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        $mdDialog.show(
            $mdDialog.alert()
                .clickOutsideToClose(true)
                .title('Thông báo')
                .textContent('Chúc mừng bạn đã thay đổi mật khẩu thành công!')
                .ok('Đóng')
                .fullscreen(false)
        ).finally(function () {
            $window.location.href = '/'
        });
    }
    //4.Quên mã bảo mật
    $scope.SubmitRecoverSecurityOTP = function () {

        if ($scope.RegisterCodeTest?.phone_number === '' || $scope.RegisterCodeTest?.phone_number === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số điện thoại')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.security_code_new === '' || $scope.RegisterCodeTest?.security_code_new === undefined) {
            $scope.isOTP = true;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mã bảo mật mới')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.security_code_new_rep === '' || $scope.RegisterCodeTest?.security_code_new_rep === undefined) {
            $scope.isOTP = true;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập lại mã bảo mật mới')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.security_code_new !== $scope.RegisterCodeTest?.security_code_new_rep) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Mã bảo mật nhập lại chưa chính xác. Vui lòng nhập lại!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.otp_code === '' || $scope.RegisterCodeTest?.otp_code === undefined) {
            $scope.isOTP = true;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mã OTP')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        $mdDialog.show(
            $mdDialog.alert()
                .clickOutsideToClose(true)
                .title('Thông báo')
                .textContent('Chúc mừng bạn đã lấy lại mã bảo mật thành công!')
                .ok('Đóng')
                .fullscreen(false)
        ).finally(function () {
            $window.location.href = '/'
        });
    }
    //5.Thay đổi mã bảo mật
    $scope.SubmitRecoverChangeSecurityOTP = function () {
        if ($scope.RegisterCodeTest?.phone_number === '' || $scope.RegisterCodeTest?.phone_number === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số điện thoại')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.security_code === '' || $scope.RegisterCodeTest?.security_code === undefined) {
            $scope.isOTP = true;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mã bảo mật cũ')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.security_code_new === '' || $scope.RegisterCodeTest?.security_code_new === undefined) {
            $scope.isOTP = true;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mã bảo mật mới')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.security_code_new_rep === '' || $scope.RegisterCodeTest?.security_code_new_rep === undefined) {
            $scope.isOTP = true;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập lại mã bảo mật mới')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.security_code_new !== $scope.RegisterCodeTest?.security_code_new_rep) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Mã bảo mật nhập lại chưa chính xác. Vui lòng nhập lại!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.otp_code === '' || $scope.RegisterCodeTest?.otp_code === undefined) {
            $scope.isOTP = true;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mã OTP')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        $mdDialog.show(
            $mdDialog.alert()
                .clickOutsideToClose(true)
                .title('Thông báo')
                .textContent('Chúc mừng bạn đã thay đổi mã bảo mật thành công!')
                .ok('Đóng')
                .fullscreen(false)
        ).finally(function () {
            $window.location.href = '/'
        });
    }
    //6.Cài đặt mã bảo mật
    $scope.SubmitRecoverSetupSecurityOTP = function () {
        if ($scope.RegisterCodeTest?.phone_number === '' || $scope.RegisterCodeTest?.phone_number === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số điện thoại')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.security_code === '' || $scope.RegisterCodeTest?.security_code === undefined) {
            $scope.isOTP = true;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mã bảo mật')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCodeTest?.otp_code === '' || $scope.RegisterCodeTest?.otp_code === undefined) {
            $scope.isOTP = true;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mã OTP')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        $mdDialog.show(
            $mdDialog.alert()
                .clickOutsideToClose(true)
                .title('Thông báo')
                .textContent('Chúc mừng bạn đã cài đặt mã bảo mật thành công!')
                .ok('Đóng')
                .fullscreen(false)
        ).finally(function () {
            $window.location.href = '/'
        });
    }
    //Xử lý nhận OTP Test
    $scope.showModalSelectionOTP = function () {
        if ($scope.RegisterCode?.phone_number === '' || $scope.RegisterCode?.phone_number === undefined) {
            $scope.isOTP = true;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số điện thoại')
                    .ok('Đóng')
                    .fullscreen(false)
            );
            return;
        } else {
            $scope.SubmitOTP($scope.RegisterCode.otpType === 'ZALO');
        }
    };




    //Ket thuc
    $scope.calculateDiscount = function (discountRate) {
        return discountRate * 0.4;
    };

    $scope.getProductMerchant = function () {
        const merchantId = sessionStorage.getItem('selectedMerchantId');

        var merchantData = {
            "partner_id": merchantId,
            "page_size": $scope.pageSizeProduct * $scope.currentPageProduct // Request the right amount of products
        };

        var dataAppLG = {
            "phone_number": config?.usernameAppCP,
            "password": config?.passwordAppCP,
            "is_android": true,
        };

        // Get the token first
        var getToken = $http({
            method: 'POST',
            url: apiUrl + 'api/app/auth/login',
            data: dataAppLG
        });

        getToken.then(function (res) {
            var POST = $http({
                method: 'POST',
                url: apiUrl + 'api/app/customer/home/listProduct',
                headers: { 'Authorization': 'bearer ' + res.data?.data?.token },
                data: JSON.stringify(merchantData)
            });

            POST.then(function (response) {
                var allProducts = response?.data?.data ? response.data.data.data : [];
                $scope.totalProducts = response.data.data.total_elements; // Total number of products

                // Slice products for current page to only get the next 12 products
                var newProducts = allProducts.slice($scope.startIndexListProduct, $scope.startIndexListProduct + $scope.pageSizeProduct);
                $scope.startIndexListProduct += $scope.pageSizeProduct; // Update startIndex for the next slice

                // Add new products to the list
                $scope.listProductMerchant = newProducts;
                console.log($scope.listProductMerchant);
            });
        });
    };

    // Next page function
    $scope.nextPageProductMerchant = function () {
        if ($scope.currentPageProduct * $scope.pageSizeProduct < $scope.totalProducts) {
            $scope.currentPageProduct++; // Move to the next page
            $scope.getProductMerchant(); // Fetch products for the next page
        }
    };

    // Previous page function
    $scope.previousPageProductMerchant = function () {
        if ($scope.currentPageProduct > 1) {
            $scope.currentPageProduct--; // Move to the previous page
            $scope.startIndexListProduct -= $scope.pageSizeProduct; // Adjust index to load previous products
            $scope.getProductMerchant();
        }
    };

    $scope.getMinMaxPriceProductMerchant = function () {
        if (!$scope.listProductMerchant || $scope.listProductMerchant.length === 0) {
            return { min: 0, max: 0 }; // Trả về 0 nếu không có sản phẩm
        }

        let prices = $scope.listProductMerchant.map(product => product.price);
        let minPrice = Math.min(...prices);
        let maxPrice = Math.max(...prices);

        return { min: minPrice, max: maxPrice };
    };
}]);

