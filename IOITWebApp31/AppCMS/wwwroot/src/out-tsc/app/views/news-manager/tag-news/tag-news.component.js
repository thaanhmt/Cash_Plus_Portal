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
exports.TagNewsComponent = void 0;
var core_1 = require("@angular/core");
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var model_1 = require("../../../data/model");
var common_service_1 = require("../../../service/common.service");
var ngx_toastr_1 = require("ngx-toastr");
var const_1 = require("../../../data/const");
var dt_1 = require("../../../data/dt");
var TagNewsComponent = /** @class */ (function () {
    function TagNewsComponent(http, modalDialogService, viewRef, toastr, common) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.common = common;
        this.listTags = [];
        this.ActionTable = const_1.ActionTable;
        this.isNoitify = false;
        this.Item = new model_1.Tag();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "TargetType=1";
        this.paging.order_by = "";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
        this.CheckRole = new dt_1.CheckRole();
        var code = "QLTTT";
        this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 0);
        this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 1);
        this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 2);
        this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 3);
    }
    TagNewsComponent.prototype.ngOnInit = function () {
        this.GetListTags();
    };
    TagNewsComponent.prototype.GetListTags = function () {
        var _this = this;
        this.http.get('/api/tag/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listTags = res["data"];
                _this.listTags.forEach(function (item) {
                    item.IsShow = item.Status == 1 ? true : false;
                });
                _this.paging.item_count = res["metadata"].Sum;
                _this.total = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    TagNewsComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListTags();
    };
    //Thông báo
    TagNewsComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    TagNewsComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    TagNewsComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    TagNewsComponent.prototype.QueryChanged = function () {
        var query = 'TargetType=1';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and Name.Contains("' + this.q.txtSearch + '")';
            }
            else {
                query += 'Name.Contains("' + this.q.txtSearch + '")';
            }
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListTags();
    };
    TagNewsComponent.prototype.OpenModalTag = function (item) {
        this.Item = new model_1.Tag();
        if (item != undefined) {
            this.Item = Object.assign(this.Item, item);
        }
        this.TagModal.show();
    };
    TagNewsComponent.prototype.ChangeTitle = function () {
        this.Item.Url = this.common.ConvertUrl(this.Item.Name);
    };
    // cập nhật 
    TagNewsComponent.prototype.SaveTag = function () {
        var _this = this;
        if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập Tên hiển thị!");
            return;
        }
        else if (this.Item.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập Tên hiển thị!");
            return;
        }
        else if (this.Item.Url == undefined || this.Item.Url == '') {
            this.toastWarning("Chưa nhập Đường dẫn!");
            return;
        }
        else if (this.Item.Url.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập Đường dẫn!");
            return;
        }
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        this.Item.TargetType = 1;
        if (this.Item.TagId) {
            this.http.put('/api/tag/' + this.Item.TagId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListTags();
                    _this.TagModal.hide();
                    _this.toastSuccess("Cập nhật thành công!");
                }
                else {
                    _this.toastError(res["meta"]["error_message"]);
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            this.http.post('/api/tag', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListTags();
                    _this.TagModal.hide();
                    _this.toastSuccess("Thêm thành công!");
                }
                else {
                    _this.toastError(res["meta"]["error_message"]);
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
            });
        }
    };
    //Popup xác nhận xóa
    TagNewsComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        _this.http.delete('/api/tag/' + Id, _this.httpOptions).subscribe(function (res) {
                            if (res["meta"]["error_code"] == 200) {
                                _this.GetListTags();
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
                    buttonClass: 'btn btn-danger',
                }
            ],
        });
    };
    TagNewsComponent.prototype.ShowHide = function (id, i) {
        var _this = this;
        var stt = this.listTags[i].IsShow ? 1 : 10;
        this.http.put('/api/tag/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.toastSuccess("Thay đổi trạng thái thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                _this.listTags[i].IsShow = !_this.listTags[i].IsShow;
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            _this.listTags[i].IsShow = !_this.listTags[i].IsShow;
        });
    };
    TagNewsComponent.prototype.SortTable = function (str) {
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
        this.GetListTags();
    };
    TagNewsComponent.prototype.GetClassSortTable = function (str) {
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
    TagNewsComponent.prototype.CheckActionTable = function (TagId) {
        if (TagId == undefined) {
            var CheckAll_1 = this.CheckAll;
            this.listTags.forEach(function (item) {
                item.Action = CheckAll_1;
            });
        }
        else {
            var CheckAll = true;
            for (var i = 0; i < this.listTags.length; i++) {
                if (!this.listTags[i].Action) {
                    CheckAll = false;
                    break;
                }
            }
            this.CheckAll = CheckAll == true ? true : false;
        }
    };
    TagNewsComponent.prototype.ActionTableFunc = function () {
        var _this = this;
        switch (this.ActionId) {
            case 1:
                var data_1 = [];
                this.listTags.forEach(function (item) {
                    if (item.Action == true) {
                        data_1.push(item.TagId);
                    }
                });
                if (data_1.length == 0) {
                    this.toastWarning("Chưa chọn bản ghi cần xóa!");
                }
                else {
                    this.modalDialogService.openDialog(this.viewRef, {
                        title: 'Xác nhận',
                        childComponent: ngx_modal_dialog_1.SimpleModalComponent,
                        data: {
                            text: "Bạn có chắc chắn muốn xóa các bản ghi đã chọn?"
                        },
                        actionButtons: [
                            {
                                text: 'Đồng ý',
                                buttonClass: 'btn btn-success',
                                onAction: function () {
                                    _this.http.put('/api/tag/deletes', data_1, _this.httpOptions).subscribe(function (res) {
                                        if (res["meta"]["error_code"] == 200) {
                                            _this.toastSuccess("Xóa thành công!");
                                            _this.GetListTags();
                                            _this.ActionId = undefined;
                                        }
                                        else {
                                            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                                        }
                                    }, function (err) {
                                        _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                                    });
                                    _this.viewRef.clear();
                                }
                            },
                            {
                                text: 'Đóng',
                                buttonClass: 'btn btn-danger',
                            }
                        ],
                    });
                }
                break;
            default:
                break;
        }
    };
    TagNewsComponent.prototype.closeNoityfy = function () {
        this.isNoitify = true;
    };
    __decorate([
        core_1.ViewChild('TagModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], TagNewsComponent.prototype, "TagModal", void 0);
    TagNewsComponent = __decorate([
        core_1.Component({
            selector: 'app-tag-news',
            templateUrl: './tag-news.component.html',
            styleUrls: ['./tag-news.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_service_1.CommonService])
    ], TagNewsComponent);
    return TagNewsComponent;
}());
exports.TagNewsComponent = TagNewsComponent;
//# sourceMappingURL=tag-news.component.js.map