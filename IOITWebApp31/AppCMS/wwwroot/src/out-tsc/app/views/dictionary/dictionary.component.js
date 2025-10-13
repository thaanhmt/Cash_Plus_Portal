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
exports.DictionaryComponent = exports.MY_CUSTOM_FORMATS = void 0;
var core_1 = require("@angular/core");
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var const_1 = require("../../data/const");
var model_1 = require("../../data/model");
var ngx_toastr_1 = require("ngx-toastr");
var dt_1 = require("../../data/dt");
exports.MY_CUSTOM_FORMATS = {
    parseInput: 'DD/MM/YYYY HH:mm',
    fullPickerInput: 'DD/MM/YYYY HH:mm',
    datePickerInput: 'DD/MM/YYYY',
    timePickerInput: ' HH:mm',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY'
};
var DictionaryComponent = /** @class */ (function () {
    function DictionaryComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.listDictionary = [];
        this.ActionTable = const_1.ActionTable;
        this.listId = [];
        this.page_pp = [];
        this.Item = new model_1.Dictionary();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "DictionaryId Desc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    DictionaryComponent.prototype.ngOnInit = function () {
        this.GetListDictionary();
    };
    //Mở modal thêm mới
    DictionaryComponent.prototype.OpenDictionaryModal = function (item) {
        this.Item = new model_1.Dictionary();
        if (item != undefined) {
            this.Item = JSON.parse(JSON.stringify(item));
        }
        this.DictionaryModal.show();
    };
    //Get danh sách tu dien
    DictionaryComponent.prototype.GetListDictionary = function () {
        var _this = this;
        this.http.get('/api/Dictionary/GetByPage?page=' + this.paging.page + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listDictionary = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    // luu tu dien
    DictionaryComponent.prototype.SaveDictionary = function () {
        var _this = this;
        if (this.Item.StringVn == undefined || this.Item.StringVn == '') {
            this.toastWarning("Chưa nhập chuỗi tiếng việt !");
            return;
        }
        else if (this.Item.StringEn == undefined || this.Item.StringEn == '') {
            this.toastWarning("Chưa nhập chuỗi tiếng anh!");
            return;
        }
        if (this.Item.DictionaryId == undefined) {
            this.http.post('/api/Dictionary', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListDictionary();
                    _this.DictionaryModal.hide();
                    _this.toastSuccess("Thêm mới thành công!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
            });
        }
        else {
            this.http.put('/api/Dictionary/' + this.Item.DictionaryId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListDictionary();
                    _this.DictionaryModal.hide();
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
    //Chuyển trang
    DictionaryComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListDictionary();
    };
    //Thông báo
    DictionaryComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    DictionaryComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    DictionaryComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    //Popup xác nhận xóa
    DictionaryComponent.prototype.ShowConfirmDelete = function (Id) {
        var _this = this;
        this.modalDialogService.openDialog(this.viewRef, {
            title: 'Xác nhận',
            childComponent: ngx_modal_dialog_1.SimpleModalComponent,
            data: {
                text: "Bạn có chắc chắn muốn xóa bản ghi này?"
            },
            actionButtons: [
                {
                    text: 'Đồng ý',
                    buttonClass: 'btn btn-success',
                    onAction: function () {
                        _this.http.delete('/api/Dictionary/' + Id, _this.httpOptions).subscribe(function (res) {
                            if (res["meta"]["error_code"] == 200) {
                                _this.GetListDictionary();
                                _this.viewRef.clear();
                                _this.toastSuccess("Xóa thành công!");
                            }
                            else {
                                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                            }
                        }, function (err) {
                            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                        });
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-default',
                }
            ],
        });
    };
    DictionaryComponent.prototype.QueryChanged = function () {
        var query = "";
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and Title.Contains("' + this.q.txtSearch + '")';
            }
            else {
                query += 'Title.Contains("' + this.q.txtSearch + '")';
            }
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListDictionary();
    };
    __decorate([
        core_1.ViewChild('DictionaryModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], DictionaryComponent.prototype, "DictionaryModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], DictionaryComponent.prototype, "file", void 0);
    DictionaryComponent = __decorate([
        core_1.Component({
            selector: 'app-dictionary',
            templateUrl: './dictionary.component.html',
            styleUrls: ['./dictionary.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService])
    ], DictionaryComponent);
    return DictionaryComponent;
}());
exports.DictionaryComponent = DictionaryComponent;
//# sourceMappingURL=dictionary.component.js.map