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
exports.LoginComponent = void 0;
var core_1 = require("@angular/core");
var forms_1 = require("@angular/forms");
var router_1 = require("@angular/router");
var auth_service_1 = require("../../service/auth.service");
var http_1 = require("@angular/common/http");
var md5_1 = require("ts-md5/dist/md5");
var ngx_cookie_service_1 = require("ngx-cookie-service");
var ngx_toastr_1 = require("ngx-toastr");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var auth_guard_1 = require("../../auth.guard");
var platform_browser_1 = require("@angular/platform-browser");
var httpOptions = {
    headers: new http_1.HttpHeaders({
        'Content-Type': 'application/json'
    })
};
var md5 = new md5_1.Md5();
var formData = new FormData();
var LoginComponent = /** @class */ (function () {
    function LoginComponent(document, formBuilder, router, authService, http, cookieService, toastr, modalDialogService, viewRef, authGuard, title) {
        this.document = document;
        this.formBuilder = formBuilder;
        this.router = router;
        this.authService = authService;
        this.http = http;
        this.cookieService = cookieService;
        this.toastr = toastr;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.authGuard = authGuard;
        this.title = title;
        this.showPassword = false;
    }
    LoginComponent.prototype.ngOnInit = function () {
        this.elem = document.documentElement;
        //this.cookieService.set('Expire', '');
        this.title.setTitle("Đăng nhập");
        this.submitted = false;
        this.loginForm = this.formBuilder.group({
            username: ['', forms_1.Validators.required],
            password: ['', forms_1.Validators.required]
        });
        this.returnUrl = '/dashboard';
        // if(this.authGuard.canActivate) {
        //   this.router.navigate([this.returnUrl]);
        // }
        //this.authService.logout();
    };
    Object.defineProperty(LoginComponent.prototype, "f", {
        get: function () { return this.loginForm.controls; },
        enumerable: false,
        configurable: true
    });
    ;
    LoginComponent.prototype.login = function () {
        var _this = this;
        this.submitted = false;
        if (this.loginForm.invalid) {
            this.submitted = true;
            return;
        }
        else {
            var email = this.f.username.value;
            var password = md5_1.Md5.hashStr(this.f.password.value);
            this.http.post('/api/user/login', JSON.stringify({ email: email, password: password }), httpOptions).subscribe(function (res) {
                var data = JSON.stringify(res);
                if (res["meta"]["error_code"] == 200) {
                    _this.cookieService.set('accessToken', res["data"]["access_token"].toString(), 2147483647, '/');
                    _this.cookieService.set('Expire', Date.now().toLocaleString(), 0.1);
                    localStorage.setItem('isLoggedIn', "true");
                    localStorage.setItem('data', res.toString());
                    localStorage.setItem('access_token', res["data"]["access_token"].toString());
                    localStorage.setItem('access_key', res["data"]["access_key"].toString());
                    localStorage.setItem('userId', res["data"]["userId"].toString());
                    localStorage.setItem('userMapId', res["data"]["userMapId"].toString());
                    localStorage.setItem('userName', res["data"]["userName"].toString());
                    localStorage.setItem('avata', res["data"]["avata"] != undefined ? res["data"]["avata"].toString() : undefined);
                    localStorage.setItem('fullName', res["data"]["fullName"] != undefined ? res["data"]["fullName"].toString() : undefined);
                    localStorage.setItem('companyId', res["data"]["companyId"] != undefined ? res["data"]["companyId"].toString() : undefined);
                    localStorage.setItem('languageId', res["data"]["languageId"] ? res["data"]["languageId"].toString() : undefined);
                    localStorage.setItem('languageCode', res["data"]["languageCode"] ? res["data"]["languageCode"].toString() : undefined);
                    localStorage.setItem('websiteId', res["data"]["websiteId"] != undefined ? res["data"]["websiteId"].toString() : undefined);
                    localStorage.setItem('roleCode', res["data"]["roleCode"] != undefined ? res["data"]["roleCode"].toString() : undefined);
                    localStorage.setItem('roleName', res["data"]["roleName"] != undefined ? res["data"]["roleName"].toString() : undefined);
                    localStorage.setItem('menu', JSON.stringify(res["data"]["listMenus"]));
                    _this.cookieService.set('Expire', Date.now().toLocaleString(), 0.1);
                    _this.router.navigate([_this.returnUrl]);
                }
                else {
                    _this.submitted = true;
                    _this.message = "Tài khoản hoặc mật khẩu không đúng";
                    _this.router.navigate(['/login']);
                }
            }, function (err) {
                _this.showConfirm("Đăng nhập không thành công. Xin vui lòng thử lại sau!");
            });
        }
    };
    LoginComponent.prototype.toat = function () {
        this.toastr.success('Website hiện tạm đóng tạo tài khoản', 'Thông báo');
    };
    LoginComponent.prototype.toatCommingSoon = function () {
        this.toastr.warning('Chức năng tạm đóng', 'Comming Soon');
    };
    LoginComponent.prototype.showConfirm = function (message) {
        this.modalDialogService.openDialog(this.viewRef, {
            title: 'Thông báo',
            childComponent: ngx_modal_dialog_1.SimpleModalComponent,
            data: {
                text: message
            },
            actionButtons: [
                {
                    text: 'Xác nhận',
                    buttonClass: 'btn btn-success'
                }
            ],
        });
    };
    LoginComponent.prototype.openToggleShowPassWord = function () {
        this.showPassword = !this.showPassword;
    };
    LoginComponent.prototype.openFullscreen = function () {
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
    LoginComponent = __decorate([
        core_1.Component({
            selector: 'app-dashboard',
            templateUrl: 'login.component.html',
            styleUrls: ['login.component.css']
        }),
        __param(0, core_1.Inject(platform_browser_1.DOCUMENT)),
        __metadata("design:paramtypes", [Object, forms_1.FormBuilder,
            router_1.Router,
            auth_service_1.AuthService,
            http_1.HttpClient,
            ngx_cookie_service_1.CookieService,
            ngx_toastr_1.ToastrService,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            auth_guard_1.AuthGuard,
            platform_browser_1.Title])
    ], LoginComponent);
    return LoginComponent;
}());
exports.LoginComponent = LoginComponent;
//# sourceMappingURL=login.component.js.map