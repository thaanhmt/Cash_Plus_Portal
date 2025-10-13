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
exports.DashboardComponent = void 0;
var core_1 = require("@angular/core");
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var const_1 = require("../../data/const");
var ngx_toastr_1 = require("ngx-toastr");
var common_1 = require("@angular/common");
var common_service_1 = require("../../service/common.service");
var tabs_1 = require("ngx-bootstrap/tabs");
var DashboardComponent = /** @class */ (function () {
    function DashboardComponent(http, modalDialogService, viewRef, toastr, datePipe, common) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.datePipe = datePipe;
        this.common = common;
        //public countBienTap: number;
        //public countBaiViet: number;
        //public countAnPham: number;
        //public countVanBan: number;
        this.listAuthor = [];
        this.domainImage = const_1.domainImage;
        this.RoleCode = localStorage.getItem("roleCode") || '';
        this.NameAuthor = localStorage.getItem("fullName") || '';
        this.CheckConfirmNews = this.common.CheckAccessKey(localStorage.getItem("access_key"), "DBBTV") && this.RoleCode != 'BTV';
        this.languageId = localStorage.getItem("languageId") != undefined ? parseInt(localStorage.getItem("languageId")) : 1;
        this.languageCode = localStorage.getItem("languageCode") != undefined ? localStorage.getItem("languageCode") : "vi";
        //this.paging.query = "LanguageId=" + this.languageId;
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    DashboardComponent.prototype.ngOnInit = function () {
        this.GetDataSet();
    };
    //Lấy toàn bộ danh sách tin văn bản
    DashboardComponent.prototype.GetListAllNews = function () {
        var query = '1=1';
        this.http.get('/api/news/GetByPageAll?page=1&query=' + query, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                //this.countIsNormal = res["metadata"].Normal;
                //this.countIsPending = res["metadata"].NoPublic;
                //this.countIsRatify = res["metadata"].KiemDuyet;
                //this.countBienTap = res["metadata"].BienTap;
                //this.countBaiViet = res["metadata"].BaiViet;
                //this.countIsDarft = res["metadata"].Temp;
                //this.countAnPham = res["metadata"].AnPham;
                //this.countVanBan = res["metadata"].VanBan;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    // Get danh sách tác giả
    DashboardComponent.prototype.GetListAuthor = function () {
        var _this = this;
        this.http.get('/api/News/GetAuthor', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listAuthor = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    DashboardComponent.prototype.GetDataSet = function () {
        var _this = this;
        var query = '1=1';
        this.http.get('/api/dashboard/GetDataSet', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.countDataSet = res["data"].DataSetNumber;
                _this.countDataSetView = res["data"].ViewNumber;
                _this.countDataSetDown = res["data"].DownNumber;
                _this.countUser = res["data"].UserNumber;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    __decorate([
        core_1.ViewChild('NewsModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], DashboardComponent.prototype, "NewsModal", void 0);
    __decorate([
        core_1.ViewChild('HighlightNewsModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], DashboardComponent.prototype, "HighlightNewsModal", void 0);
    __decorate([
        core_1.ViewChild('TagModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], DashboardComponent.prototype, "TagModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], DashboardComponent.prototype, "file", void 0);
    __decorate([
        core_1.ViewChild('attachment'),
        __metadata("design:type", core_1.ElementRef)
    ], DashboardComponent.prototype, "attachment", void 0);
    __decorate([
        core_1.ViewChild('tabset'),
        __metadata("design:type", tabs_1.TabsetComponent)
    ], DashboardComponent.prototype, "tabset", void 0);
    DashboardComponent = __decorate([
        core_1.Component({
            templateUrl: 'dashboard.component.html',
            providers: [common_1.Location, {
                    provide: common_1.LocationStrategy,
                    useClass: common_1.PathLocationStrategy
                }],
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_1.DatePipe,
            common_service_1.CommonService])
    ], DashboardComponent);
    return DashboardComponent;
}());
exports.DashboardComponent = DashboardComponent;
//# sourceMappingURL=dashboard.component.js.map