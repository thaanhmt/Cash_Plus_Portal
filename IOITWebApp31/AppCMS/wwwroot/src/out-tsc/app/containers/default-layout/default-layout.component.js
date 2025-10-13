"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.DefaultLayoutComponent = void 0;
var core_1 = require("@angular/core");
var const_1 = require("./../../data/const");
var auth_service_1 = require("../../service/auth.service");
var ngx_cookie_service_1 = require("ngx-cookie-service");
var model_1 = require("./../../data/model");
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var ngx_toastr_1 = require("ngx-toastr");
var md5_1 = require("ts-md5/dist/md5");
var model_2 = require("../../data/model");
var platform_browser_1 = require("@angular/platform-browser");
var dt_1 = require("../../data/dt");
var DefaultLayoutComponent = /** @class */ (function () {
    function DefaultLayoutComponent(auth, cookie, toastr, http, document) {
        var _this = this;
        this.auth = auth;
        this.cookie = cookie;
        this.toastr = toastr;
        this.http = http;
        this.document = document;
        this.listNotification = [];
        this.navItem = [];
        this.sidebarMinimized = true;
        /*public element: HTMLElement = document.body;*/
        this.userChangePass = new model_1.UserChangePass();
        this.domainImage = const_1.domainImage;
        this.domainMedia = const_1.domainMedia;
        this.ClassTheme = localStorage.getItem("ThemeStyle") || '';
        this.activeSide = localStorage.getItem("Morong") || 'non-active';
        this.ChucVu = localStorage.getItem("roleName") || '';
        this.isDark = localStorage.getItem("ThemeStyle") == "dark" ? true : false;
        this.isMorong = localStorage.getItem("Morong") == "active" ? true : false;
        this.stick = false;
        this.scroll = false;
        this.isActiveMedia = true;
        this.isActiveUpload = false;
        this.isDelay = false;
        this.listItemMedia = [];
        this.domainDebug = const_1.domainDebug;
        this.scrolling = function (s) {
            var sc = s.target.scrollingElement.scrollTop;
            if (sc >= 25) {
                _this.stick = true;
            }
            else {
                _this.stick = false;
            }
        };
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 200;
        this.paging.query = "1=1";
        this.paging.order_by = "";
        this.paging.item_count = 0;
        this.pagingFile = new dt_1.Paging();
        this.pagingFile.page = 1;
        this.pagingFile.page_size = 24;
        this.pagingFile.query = "1=1";
        this.pagingFile.order_by = "";
        this.pagingFile.item_count = 0;
        this.countMedia = 24;
        var json = JSON.parse(localStorage.getItem('menu'));
        this.Item = new model_2.User();
        this.navItem.push({
            icon: "fa-light fa-gauge",
            name: "Trang tổng quan",
            url: "/dashboard"
        });
        for (var i = 0; i < json.length; i++) {
            this.navItem.push(this.createMenu(json[i], undefined));
        }
        this.changes = new MutationObserver(function (mutations) {
            _this.sidebarMinimized = document.body.classList.contains('sidebar-minimized');
        });
        //this.changes.observe(<Element>this.element, {
        //  attributes: true
        //});
        this.myFuntion();
        this.userChangePass.UserId = parseInt(localStorage.getItem("userId"));
        this.userChangePass.UserName = localStorage.getItem("userName");
        this.userChangePass.Avatar = localStorage.getItem("avata");
        this.userChangePass.FullName = localStorage.getItem("fullName");
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
        setInterval(function () {
            var currentDate = new Date();
            _this.date = currentDate.toLocaleTimeString('en-US', { hour: '2-digit', hour12: true, minute: '2-digit' }).replace("AM", "SA").replace("PM", "CH");
        }, 30000);
        this.getDate = new Date().getDate();
        this.getDay = new Date().getDay();
        this.getMonth = new Date().getUTCMonth() + 1;
        this.getFullYear = new Date().getFullYear();
        if (this.getDay == 0) {
            this.nameDay = 'Chủ nhật';
        }
        else if (this.getDay == 1) {
            this.nameDay = 'Thứ hai';
        }
        else if (this.getDay == 2) {
            this.nameDay = 'Thứ ba';
        }
        else if (this.getDay == 3) {
            this.nameDay = 'Thứ tư';
        }
        else if (this.getDay == 4) {
            this.nameDay = 'Thứ năm';
        }
        else if (this.getDay == 5) {
            this.nameDay = 'Thứ sáu';
        }
        else if (this.getDay == 6) {
            this.nameDay = 'Thứ bảy';
        }
    }
    DefaultLayoutComponent.prototype.ngOnInit = function () {
        this.ChucVu = localStorage.getItem("roleName");
        var currentDate = new Date();
        this.date = currentDate.toLocaleTimeString('en-US', { hour: '2-digit', hour12: true, minute: '2-digit' }).replace("AM", "SA").replace("PM", "CH");
        this.elem = document.documentElement;
        window.addEventListener('scroll', this.scrolling, true);
        //get ds thông báo
        var userId = localStorage.getItem("userId");
        var userMapId = localStorage.getItem("userMapId");
        this.paging.query = "1=1 And UserReadId=" + userMapId;
        this.GetListNotification();
        this.GetListFiles();
        this.GetDomainStatic();
    };
    DefaultLayoutComponent.prototype.GetDomainStatic = function () {
        var _this = this;
        this.http.get('api/Config/1', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.staticDomain = res["data"].Website;
                if (res["data"].ModeSite) {
                    _this.staticDomainMedia = _this.domainDebug + 'uploads';
                    _this.staticDomain = _this.domainDebug;
                }
                else {
                    _this.staticDomainMedia = _this.staticDomain + 'uploads';
                    _this.staticDomain = res["data"].Website;
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    DefaultLayoutComponent.prototype.GetListNotification = function () {
        var _this = this;
        this.http.get('/api/notification/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listNotification = res["data"];
                _this.paging.item_count = res["metadata"].Sum;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    DefaultLayoutComponent.prototype.createMenu = function (item, urlParent) {
        item["name"] = item["Name"];
        item["url"] = urlParent == undefined ? "/" + item["Url"] : urlParent + "/" + item["Url"];
        item["icon"] = item["Icon"];
        delete item["MenuId"];
        delete item["Code"];
        delete item["Name"];
        delete item["MenuParent"];
        delete item["Url"];
        delete item["Icon"];
        delete item["ActiveKey"];
        delete item["Status"];
        if (item["listMenus"].length > 0) {
            item["children"] = [];
            for (var i = 0; i < item["listMenus"].length; i++) {
                item["children"].push(item["listMenus"][i]);
                this.createMenu(item["children"][i], item["url"]);
            }
        }
        delete item["listMenus"];
        return item;
    };
    DefaultLayoutComponent.prototype.logout = function () {
        this.auth.logout();
    };
    DefaultLayoutComponent.prototype.myFuntion = function () {
        var _this = this;
        // debugger;
        setInterval(function () {
            if (_this.cookie.get("Expire") == '' || _this.cookie.get("Expire") == undefined || localStorage.getItem('isLoggedIn') != "true") {
                _this.auth.logout();
            }
        }, 10000);
    };
    DefaultLayoutComponent.prototype.OpenChangePasswordModal = function () {
        this.userChangePass.PasswordOldE = undefined;
        this.userChangePass.PasswordNewE = undefined;
        this.userChangePass.ConfirmPassword = undefined;
        this.changePasswordModal.show();
    };
    DefaultLayoutComponent.prototype.ChangePassword = function () {
        var _this = this;
        if (this.userChangePass.PasswordOldE == undefined || this.userChangePass.PasswordOldE == '') {
            this.toastWarning("Chưa nhập Mật khẩu hiện tại!");
            return;
        }
        else if (this.userChangePass.PasswordNewE == undefined || this.userChangePass.PasswordNewE == '') {
            this.toastWarning("Chưa nhập Mật khẩu mới!");
            return;
        }
        else if (this.userChangePass.ConfirmPassword == undefined || this.userChangePass.ConfirmPassword == '') {
            this.toastWarning("Chưa nhập Mật khẩu xác nhận!");
            return;
        }
        else if (this.userChangePass.ConfirmPassword != this.userChangePass.PasswordNewE) {
            this.toastWarning("Mật khẩu xác nhận không đúng!");
            return;
        }
        this.userChangePass.PasswordOld = md5_1.Md5.hashStr(this.userChangePass.PasswordOldE).toString();
        this.userChangePass.PasswordNew = md5_1.Md5.hashStr(this.userChangePass.PasswordNewE).toString();
        this.http.put('/api/user/changePass/' + this.userChangePass.UserId, this.userChangePass, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.changePasswordModal.hide();
                _this.toastSuccess("Đổi mật khẩu tài khoản thành công!");
            }
            else if (res["meta"]["error_code"] == 213) {
                _this.toastError("Mật khẩu hiện tại không đúng. Vui lòng thử lại!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
        });
    };
    //Thông báo
    DefaultLayoutComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    DefaultLayoutComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    DefaultLayoutComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    DefaultLayoutComponent.prototype.OpenModalInfo = function () {
        var _this = this;
        this.Item = new model_2.User();
        this.file.nativeElement.value = "";
        this.http.get('/api/user/infoUser/' + this.userChangePass.UserId, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.Item = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
        this.modalMyInfo.show();
    };
    DefaultLayoutComponent.prototype.upload = function (files) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
            formData.append(file.name, file);
        }
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload/uploadImage/6', formData, {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
        this.http.request(uploadReq).subscribe(function (event) {
            if (event.type === http_1.HttpEventType.UploadProgress) {
            }
            else if (event.type === http_1.HttpEventType.Response) {
                _this.Item.Avata = event.body["data"].toString();
            }
        });
    };
    DefaultLayoutComponent.prototype.SaveInfo = function () {
        var _this = this;
        if (this.Item.FullName == undefined || this.Item.FullName == '') {
            this.toastWarning("Chưa nhập Tên người dùng!");
            return;
        }
        else if (this.Item.UserName == undefined || this.Item.UserName == '') {
            this.toastWarning("Chưa nhập Tên tài khoản!");
            return;
        }
        this.http.put('/api/user/changeInfoUser', this.Item, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.modalMyInfo.hide();
                _this.toastSuccess("Lưu thông tin thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    DefaultLayoutComponent.prototype.openFullscreen = function () {
        if (this.elem.requestFullscreen) {
            this.elem.requestFullscreen();
        }
        else if (this.elem.mozRequestFullScreen) {
            /* Firefox */
            this.elem.mozRequestFullScreen();
        }
        else if (this.elem.webkitRequestFullscreen) {
            /* Chrome, Safari and Opera */
            this.elem.webkitRequestFullscreen();
        }
        else if (this.elem.msRequestFullscreen) {
            /* IE/Edge */
            this.elem.msRequestFullscreen();
        }
        if (this.document.exitFullscreen) {
            this.document.exitFullscreen();
        }
        else if (this.document.mozCancelFullScreen) {
            /* Firefox */
            this.document.mozCancelFullScreen();
        }
        else if (this.document.webkitExitFullscreen) {
            /* Chrome, Safari and Opera */
            this.document.webkitExitFullscreen();
        }
        else if (this.document.msExitFullscreen) {
            /* IE/Edge */
            this.document.msExitFullscreen();
        }
    };
    DefaultLayoutComponent.prototype.changeDrankToggle = function () {
        if (this.isDark == undefined) {
            this.isDark = false;
        }
        this.isDark = !this.isDark;
        if (this.isDark == true) {
            localStorage.setItem('ThemeStyle', 'dark');
        }
        else {
            localStorage.setItem('ThemeStyle', '');
        }
        this.ClassTheme = localStorage.getItem("ThemeStyle");
    };
    DefaultLayoutComponent.prototype.OpenModleSupport = function () {
        if (this.isMorong == undefined) {
            this.isMorong = false;
        }
        this.isMorong = !this.isMorong;
        if (this.isMorong == true) {
            localStorage.setItem('Morong', 'active');
        }
        else {
            localStorage.setItem('Morong', 'non-active');
        }
        this.activeSide = localStorage.getItem("Morong");
    };
    DefaultLayoutComponent.prototype.messageToast = function () {
        this.toastr.warning('Hãy quay lại sau!', 'Thông báo');
    };
    DefaultLayoutComponent.prototype.OpenMediaModal = function () {
        this.OpenMediaFile.show();
    };
    DefaultLayoutComponent.prototype.CloseMediaModal = function () {
        this.OpenMediaFile.hide();
    };
    DefaultLayoutComponent.prototype.tabHandleMedia = function () {
        this.isActiveMedia = true;
        this.isActiveUpload = false;
    };
    DefaultLayoutComponent.prototype.tabHandleMediaUpload = function () {
        this.isActiveMedia = false;
        this.isActiveUpload = true;
    };
    DefaultLayoutComponent.prototype.upload3 = function (files, cs) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_2 = files; _i < files_2.length; _i++) {
            var file = files_2[_i];
            formData.append(file.name, file);
        }
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload/uploadMedia/8', formData, {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
        this.http.request(uploadReq).subscribe(function (event) {
            if (event.type === http_1.HttpEventType.UploadProgress)
                switch (cs) {
                    case 1:
                        _this.progress = Math.round(100 * event.loaded / event.total);
                        break;
                    default:
                        break;
                }
            else if (event.type === http_1.HttpEventType.Response) {
                switch (cs) {
                    case 1:
                        _this.isActiveMedia = true;
                        _this.isActiveUpload = false;
                        _this.pagingFile.page = 1;
                        _this.GetListFiles();
                        _this.message = event.body["data"].toString();
                        _this.toastSuccess("Tải lên thành công");
                        break;
                    default:
                        break;
                }
            }
        });
    };
    DefaultLayoutComponent.prototype.loadMore = function () {
        var _this = this;
        this.isDelay = true;
        setTimeout(function () {
            _this.isDelay = false;
            _this.pagingFile.page++;
            _this.http.get('/api/fileManager/GetFiles?page=' + _this.pagingFile.page + '&page_size=' + _this.pagingFile.page_size + '&query='
                + _this.pagingFile.query + '&order_by=' + '&select=' + _this.pagingFile.select, _this.httpOptions).subscribe(function (res) {
                var _a;
                if (res["meta"]["error_code"] == 200) {
                    (_a = _this.listItemMedia).push.apply(_a, res["data"]);
                    if (_this.countMedia >= _this.countAllMedia) {
                        _this.countMedia = _this.countAllMedia;
                    }
                    else {
                        if ((_this.countMedia + 24) >= _this.countAllMedia) {
                            _this.countMedia = _this.countAllMedia;
                        }
                        else {
                            _this.countMedia += 24;
                        }
                    }
                }
            }, function (err) {
                console.log("Error: connect to API");
            });
        }, 1000);
    };
    DefaultLayoutComponent.prototype.GetListFiles = function () {
        var _this = this;
        this.http.get('/api/fileManager/GetFiles?page=' + this.pagingFile.page + '&page_size=' + this.pagingFile.page_size + '&query='
            + this.pagingFile.query + '&order_by=' + '&select=' + this.pagingFile.select, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listItemMedia = res["data"];
                _this.countAllMedia = res["metadata"];
                if (_this.countAllMedia < 24)
                    _this.countMedia = _this.countAllMedia;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    DefaultLayoutComponent.prototype.RemoveImage = function () {
        this.Item.Avata = undefined;
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
    };
    DefaultLayoutComponent.prototype.SeclectMedia = function (item) {
        this.Item.Avata = item.url + "/" + item.name;
        this.OpenMediaFile.hide();
    };
    __decorate([
        core_1.ViewChild('ChangePasswordModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], DefaultLayoutComponent.prototype, "changePasswordModal", void 0);
    __decorate([
        core_1.ViewChild('modalMyInfo'),
        __metadata("design:type", modal_1.ModalDirective)
    ], DefaultLayoutComponent.prototype, "modalMyInfo", void 0);
    __decorate([
        core_1.ViewChild('supportModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], DefaultLayoutComponent.prototype, "supportModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], DefaultLayoutComponent.prototype, "file", void 0);
    __decorate([
        core_1.ViewChild('OpenMediaFile'),
        __metadata("design:type", modal_1.ModalDirective)
    ], DefaultLayoutComponent.prototype, "OpenMediaFile", void 0);
    DefaultLayoutComponent = __decorate([
        core_1.Component({
            selector: 'app-dashboard',
            templateUrl: './default-layout.component.html',
            styleUrls: ['./default-layout.component.css']
        }),
        __param(4, core_1.Inject(platform_browser_1.DOCUMENT)),
        __metadata("design:paramtypes", [auth_service_1.AuthService,
            ngx_cookie_service_1.CookieService,
            ngx_toastr_1.ToastrService,
            http_1.HttpClient, Object])
    ], DefaultLayoutComponent);
    return DefaultLayoutComponent;
}());
exports.DefaultLayoutComponent = DefaultLayoutComponent;
//# sourceMappingURL=default-layout.component.js.map