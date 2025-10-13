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
Object.defineProperty(exports, "__esModule", { value: true });
exports.ConfigStarComponent = void 0;
var core_1 = require("@angular/core");
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var model_1 = require("../../../data/model");
var ngx_toastr_1 = require("ngx-toastr");
var const_1 = require("../../../data/const");
var process_1 = require("process");
var dt_1 = require("../../../data/dt");
var ConfigStarComponent = /** @class */ (function () {
    function ConfigStarComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.listConfigStar = [];
        this.listOperators = const_1.listOperators;
        this.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.isNoitify = false;
        /*  public listItemMedia = [];*/
        this.domainMedia = const_1.domainMedia;
        this.domain = process_1.domain;
        this.domainDebug = const_1.domainDebug;
        this.Item = new model_1.ConfigStar();
        //
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 5;
        this.paging.query = "1=1";
        this.paging.order_by = "";
        this.paging.item_count = 0;
        this.tabActive = 1;
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    ConfigStarComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListConfigStar();
        this.GetDomainStatic();
    };
    ConfigStarComponent.prototype.GetListConfigStar = function () {
        var _this = this;
        this.http.get('/api/configStar/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listConfigStar = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    ConfigStarComponent.prototype.GetDomainStatic = function () {
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
    //Thông báo
    ConfigStarComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    ConfigStarComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    ConfigStarComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    ConfigStarComponent.prototype.changeTab = function (tab) {
        this.tabActive = tab;
    };
    ConfigStarComponent.prototype.Update = function () {
        var _this = this;
        if (this.tabActive == 1) {
            //if (this.Item.Name == undefined || this.Item.Name == '') {
            //  this.toastWarning("Chưa nhập Tên!");
            //  return;
            //} else if (this.Item.Name.replace(/ /g, '') == '') {
            //  this.toastWarning("Chưa nhập tên!");
            //  return;
            //} else if (this.Item.Url == undefined || this.Item.Url == '') {
            //  this.toastWarning("Chưa nhập đường dẫn!");
            //  return;
            //} else if (this.Item.Url.replace(/ /g, '') == '') {
            //  this.toastWarning("Chưa nhập đường dẫn!");
            //  return;
            //}
            //else if (this.Item.LanguageId == undefined) {
            //  this.toastWarning("Chưa chọn ngôn ngữ!");
            //  return;
            //}
            //this.Item.UserId = parseInt(localStorage.getItem("userId"));
            //this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
            //let obj = JSON.parse(JSON.stringify(this.Item));
            this.http.put('/api/configStar', this.listConfigStar, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.toastSuccess("Cập nhật thành công!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
    };
    ConfigStarComponent.prototype.closeNoityfy = function () {
        this.isNoitify = true;
    };
    ConfigStarComponent.prototype.OpenMediaModal = function (type) {
        this.selectMedia = type;
        this.OpenMediaFile.show();
    };
    ConfigStarComponent.prototype.CloseMediaModal = function () {
        this.OpenMediaFile.hide();
    };
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], ConfigStarComponent.prototype, "file", void 0);
    __decorate([
        core_1.ViewChild('OpenMediaFile'),
        __metadata("design:type", modal_1.ModalDirective)
    ], ConfigStarComponent.prototype, "OpenMediaFile", void 0);
    ConfigStarComponent = __decorate([
        core_1.Component({
            selector: 'app-config-star',
            templateUrl: './config-star.component.html',
            styleUrls: ['./config-star.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService])
    ], ConfigStarComponent);
    return ConfigStarComponent;
}());
exports.ConfigStarComponent = ConfigStarComponent;
//# sourceMappingURL=config-star.component.js.map