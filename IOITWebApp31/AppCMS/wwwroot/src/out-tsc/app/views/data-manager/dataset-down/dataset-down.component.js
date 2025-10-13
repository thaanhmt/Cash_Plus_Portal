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
exports.DatasetDownComponent = exports.MY_CUSTOM_FORMATS = void 0;
var core_1 = require("@angular/core");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var const_1 = require("../../../data/const");
var model_1 = require("../../../data/model");
var ngx_toastr_1 = require("ngx-toastr");
var common_1 = require("@angular/common");
var common_service_1 = require("../../../service/common.service");
var dt_1 = require("../../../data/dt");
exports.MY_CUSTOM_FORMATS = {
    parseInput: 'DD/MM/YYYY HH:mm',
    fullPickerInput: 'DD/MM/YYYY HH:mm',
    datePickerInput: 'DD/MM/YYYY',
    timePickerInput: ' HH:mm',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY'
};
var DatasetDownComponent = /** @class */ (function () {
    function DatasetDownComponent(http, modalDialogService, viewRef, toastr, datePipe, common) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.datePipe = datePipe;
        this.common = common;
        //@ViewChild('dateStart') dateStart: ElementRef;
        //@ViewChild('dateEnd') dateEnd: ElementRef;
        this.domainMedia = const_1.domainMedia;
        this.listDatas = [];
        this.listUnit = [];
        this.listApplicationRange = [];
        this.listResearchArea = [];
        this.listStatus = const_1.listDataSetStatus;
        this.listTypes = const_1.listDataSetTypes;
        this.listAuthors = [];
        this.listFiles = const_1.listDataSetFiles;
        this.listLanguage = [];
        this.listLanguageTemp = [];
        this.isNoitify = false;
        //public httpOptionsBlob: any;
        this.ActionTable = const_1.ActionTable;
        this.domainDebug = const_1.domainDebug;
        this.PriceMaskConfig = {
            align: "left",
            allowNegative: false,
            decimal: ".",
            precision: 0,
            prefix: "",
            suffix: "",
            thousands: ","
        };
        this.Item = new model_1.DataSet();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.IsAll = false;
        //this.CheckConfirmNews = this.common.CheckAccessKey(localStorage.getItem("access_key"), "DBBTV") && this.RoleCode != 'BTV';
        //this.languageId = localStorage.getItem("languageId") != undefined ? parseInt(localStorage.getItem("languageId")) : 1;
        //this.languageCode = localStorage.getItem("languageCode") != undefined ? localStorage.getItem("languageCode") : "vi";
        //this.paging.query = "LanguageId=" + this.languageId;
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
        //this.httpOptionsBlob = {
        //  headers: new HttpHeaders({
        //    'Authorization': 'bearer ' + localStorage.getItem("access_token")
        //  }),
        //  observe: 'response',
        //  responseType: 'blob' as 'json'
        //}
        this.CheckRole = new dt_1.CheckRole();
        var code = "QLDL";
        this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 0);
        this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 1);
        this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 2);
        this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 3);
        this.CheckRole.Export = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 5);
    }
    DatasetDownComponent.prototype.ngOnInit = function () {
        //this.RoleCode = localStorage.getItem("roleCode");
        //this.NameAuthor = localStorage.getItem("fullName");
        //this.ckeConfig = {
        //  allowedContent: false,
        //  extraPlugins: 'divarea',
        //  forcePasteAsPlainText: true
        //};
        this.paging.query = "1=1";
        //this.domain = domain;
        this.GetListData();
        this.GetListCategory(14);
        this.GetListCategory(15);
        this.GetListUnit();
        this.GetListAuthor();
        //this.GetListLanguage();
        //this.GetListFiles();
        this.GetDomainStatic();
    };
    DatasetDownComponent.prototype.GetDomainStatic = function () {
        var _this = this;
        this.http.get('api/Config/1', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.staticDomain = res["data"].Website;
                if (res["data"].ModeSite) {
                    _this.staticDomain = res["data"].Website;
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
    DatasetDownComponent.prototype.GetListData = function () {
        var _this = this;
        this.http.get('/api/dataSetDown/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listDatas = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    DatasetDownComponent.prototype.GetListCategory = function (type) {
        var _this = this;
        this.http.get('/api/category/GetByPage?page=1&query=TypeCategoryId=' + type + '&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                if (type == 14)
                    _this.listResearchArea = res["data"];
                else
                    _this.listApplicationRange = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    DatasetDownComponent.prototype.GetListUnit = function () {
        var _this = this;
        this.http.get('/api/unit/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listUnit = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    DatasetDownComponent.prototype.GetListAuthor = function () {
        var _this = this;
        this.http.get('/api/customer/GetByPage?page=1&page_size=200&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listAuthors = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    // Get danh sách ngôn ngữ
    DatasetDownComponent.prototype.GetListLanguage = function () {
        var _this = this;
        this.http.get('/api/Language/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listLanguage = res["data"];
                if (_this.listLanguage == undefined || _this.listLanguage == null)
                    _this.listLanguageTemp = [];
                else
                    _this.listLanguageTemp = _this.listLanguage;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    DatasetDownComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListData();
    };
    //Thông báo
    DatasetDownComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    DatasetDownComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    DatasetDownComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    DatasetDownComponent.prototype.QueryChanged = function () {
        var query = "1=1";
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            //if (query != '') {
            query += ' and Title.Contains("' + this.q.txtSearch + '")';
        }
        if (this.q.ApplicationRangeId != undefined) {
            query += ' and ApplicationRangeId=' + this.q.ApplicationRangeId;
        }
        if (this.q.ResearchAreaId != undefined) {
            query += ' and ResearchAreaId=' + this.q.ResearchAreaId;
        }
        if (this.q.UnitId != undefined) {
            query += ' and UnitId=' + this.q.UnitId;
        }
        if (this.q.CustomerId != undefined) {
            query += ' and CreatedId=' + this.q.CustomerId;
        }
        this.paging.query = query;
        this.GetListData();
    };
    //
    DatasetDownComponent.prototype.StatusChanged = function (status) {
        var query = "1=1";
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            query += ' and Title.Contains("' + this.q.txtSearch + '")';
        }
        if (this.q["Type"] != undefined) {
            query += ' and TypeNewsId=' + this.q["Type"];
        }
        if (this.q["CategoryId"] != undefined) {
            query += ' and CategoryId=' + this.q["CategoryId"];
        }
        if (this.q.LanguageId != undefined) {
            query += ' and LanguageId=' + this.q.LanguageId;
        }
        if (this.q.IsHot != undefined) {
            query += ' and IsHot=' + this.q.IsHot;
        }
        if (this.q["UserId"] != undefined) {
            query += ' and UserCreatedId=' + this.q["UserId"];
        }
        if (this.q.AuthorId != undefined) {
            query += ' and AuthorId=' + this.q.AuthorId;
        }
        query += ' and Status=' + status;
        this.paging.query = query;
        this.GetListData();
    };
    DatasetDownComponent.prototype.findAuthor = function (item) {
        if (item == undefined) {
            return "";
        }
        else {
            return item.FullName;
        }
    };
    DatasetDownComponent.prototype.SortTable = function (str) {
        var First = "";
        var Last = "";
        if (this.paging.order_by != "") {
            First = this.paging.order_by.split(" ")[0];
            Last = this.paging.order_by.split(" ")[1];
        }
        if (First != str) {
            this.paging.order_by = str + " Desc";
        }
        else {
            Last = Last == "Asc" ? "Desc" : "Asc";
            this.paging.order_by = str + " " + Last;
        }
        this.GetListData();
    };
    DatasetDownComponent.prototype.GetClassSortTable = function (str) {
        if (this.paging.order_by != (str + " Desc") && this.paging.order_by != (str + " Asc")) {
            return "sorting";
        }
        else {
            if (this.paging.order_by == (str + " Desc"))
                return "sorting_desc";
            else
                return "sorting_asc";
        }
    };
    DatasetDownComponent.prototype.GetClassSortTablePublic = function (str) {
        if (this.paging.order_by != (str + " Desc") && this.paging.order_by != (str + " Asc")) {
            return "sorting";
        }
        else {
            if (this.paging.order_by == (str + " Desc"))
                return "sorting_desc";
            else
                return "sorting_asc";
        }
    };
    //RemoveAttachment(idx) {
    //  if (this.Item.listAttachment[idx].AttactmentId == undefined) {
    //    this.Item.listAttachment.splice(idx, 1);
    //  }
    //  else {
    //    this.Item.listAttachment[idx].Status = 99;
    //  }
    //}
    //SetIsMain(idx) {
    //  for (let i = 0; i < this.Item.listAttachment.length; i++) {
    //    this.Item.listAttachment[i].IsImageMain = false;
    //    if (idx == i) {
    //      this.Item.listAttachment[i].IsImageMain = true;
    //    }
    //  }
    //}
    DatasetDownComponent.prototype.CheckActionTable = function (NewsId) {
        if (NewsId == undefined) {
            var CheckAll_1 = this.CheckAll;
            this.listDatas.forEach(function (item) {
                item.Action = CheckAll_1;
            });
        }
        else {
            var CheckAll = true;
            for (var i = 0; i < this.listDatas.length; i++) {
                if (!this.listDatas[i].Action) {
                    CheckAll = false;
                    break;
                }
            }
            this.CheckAll = CheckAll == true ? true : false;
        }
    };
    DatasetDownComponent.prototype.closeNoityfy = function () {
        this.isNoitify = true;
    };
    DatasetDownComponent = __decorate([
        core_1.Component({
            selector: 'app-dataset-down',
            templateUrl: './dataset-down.component.html',
            styleUrls: ['./dataset-down.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_1.DatePipe,
            common_service_1.CommonService])
    ], DatasetDownComponent);
    return DatasetDownComponent;
}());
exports.DatasetDownComponent = DatasetDownComponent;
//# sourceMappingURL=dataset-down.component.js.map