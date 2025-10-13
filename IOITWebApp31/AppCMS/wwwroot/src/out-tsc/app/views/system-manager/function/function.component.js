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
exports.FunctionComponent = void 0;
var core_1 = require("@angular/core");
var http_1 = require("@angular/common/http");
var modal_1 = require("ngx-bootstrap/modal");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var ngx_toastr_1 = require("ngx-toastr");
var dt_1 = require("../../../data/dt");
var model_1 = require("../../../data/model");
var const_1 = require("../../../data/const");
var FunctionComponent = /** @class */ (function () {
    function FunctionComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.listFunction = [];
        this.listFunctionParent = [];
        this.ActionTable = const_1.ActionTable;
        this.isNoitify = false;
        this.Item = new model_1.Function();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "FunctionId Desc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.q.txtCode = "";
        this.q.txtSlug = "";
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    FunctionComponent.prototype.ngOnInit = function () {
        this.GetListFunction();
    };
    //Get danh sách chức năng
    FunctionComponent.prototype.GetListFunction = function () {
        var _this = this;
        this.http.get('/api/function/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listFunction = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    FunctionComponent.prototype.GetListFunctionParent = function (Id) {
        var _this = this;
        this.http.get('/api/function/listFunction', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listFunctionParent = res["data"];
                _this.listFunctionParent.forEach(function (item) {
                    if (item.FunctionId == Id) {
                        item.disabled = true;
                    }
                    item.Space = "";
                    for (var i = 0; i < (item.Level) * 7; i++) {
                        item.Space += "&nbsp;";
                    }
                });
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    FunctionComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListFunction();
    };
    //Toast cảnh báo
    FunctionComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    FunctionComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    FunctionComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    FunctionComponent.prototype.QueryChanged = function () {
        var query = "1=1";
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            query += ' and Name.Contains("' + this.q.txtSearch + '")';
        }
        if (this.q.txtCode != undefined && this.q.txtCode != '') {
            query += ' and Code.Contains("' + this.q.txtCode + '")';
        }
        if (this.q.txtSlug != undefined && this.q.txtSlug != '') {
            query += ' and Url.Contains("' + this.q.txtSlug + '")';
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListFunction();
    };
    FunctionComponent.prototype.OpenModalFunction = function (item) {
        this.Item = new model_1.Function();
        if (item == undefined) {
            this.GetListFunctionParent(undefined);
        }
        else {
            this.Item = Object.assign(this.Item, item);
            if (this.Item.FunctionParentId == 0)
                this.Item.FunctionParentId = undefined;
            this.GetListFunctionParent(item.FunctionId);
        }
        this.modalFunction.show();
    };
    FunctionComponent.prototype.SaveFunc = function () {
        var _this = this;
        if (this.Item.Code == undefined || this.Item.Code == '') {
            this.toastWarning("Chưa nhập Mã chức năng!");
            return;
        }
        else if (this.Item.Code.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập mã chức năng!");
            return;
        }
        else if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập Tên chức năng!");
            return;
        }
        else if (this.Item.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên chức năng!");
            return;
        }
        else if (this.Item.Url == undefined || this.Item.Url == '') {
            this.toastWarning("Chưa nhập Đường dẫn!");
            return;
        }
        else if (this.Item.Url.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập đường dẫ!");
            return;
        }
        var obj = JSON.parse(JSON.stringify(this.Item));
        obj.FunctionParentId = obj.FunctionParentId != undefined ? obj.FunctionParentId : 0;
        if (this.Item.FunctionId == undefined) {
            this.http.post('/api/function', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListFunction();
                    _this.modalFunction.hide();
                    _this.toastSuccess("Thêm mới thành công!");
                }
                else if (res["meta"]["error_code"] == 211) {
                    _this.toastError("Thông tin không đủ. Xin vui lòng thử lại!");
                }
                else if (res["meta"]["error_code"] == 212) {
                    _this.toastError("Mã chức năng đã tồn tại. Xin vui lòng thử lại!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            if (obj.FunctionParentId == null)
                obj.FunctionParentId = 0;
            this.http.put('/api/function/' + obj.FunctionId, obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListFunction();
                    _this.modalFunction.hide();
                    _this.toastSuccess("Cập nhật thành công!");
                }
                else if (res["meta"]["error_code"] == 211) {
                    _this.toastError("Thông tin không đủ. Xin vui lòng thử lại!");
                }
                else if (res["meta"]["error_code"] == 212) {
                    _this.toastError("Mã chức năng đã tồn tại. Xin vui lòng thử lại!");
                }
                else if (res["meta"]["error_code"] == 215) {
                    _this.toastError("Chức năng cha không hợp lệ. Xin vui lòng thử lại!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
    };
    //Popup xác nhận xóa
    FunctionComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        _this.Delete(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-danger',
                }
            ],
        });
    };
    FunctionComponent.prototype.Delete = function (Id) {
        var _this = this;
        this.http.delete('/api/function/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListFunction();
                _this.viewRef.clear();
                _this.toastSuccess("Xóa thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    FunctionComponent.prototype.SortTable = function (str) {
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
        this.GetListFunction();
    };
    FunctionComponent.prototype.GetClassSortTable = function (str) {
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
    FunctionComponent.prototype.CheckActionTable = function (FunctionId) {
        if (FunctionId == undefined) {
            var CheckAll_1 = this.CheckAll;
            this.listFunction.forEach(function (item) {
                item.Action = CheckAll_1;
            });
        }
        else {
            var CheckAll = true;
            for (var i = 0; i < this.listFunction.length; i++) {
                if (!this.listFunction[i].Action) {
                    CheckAll = false;
                    break;
                }
            }
            this.CheckAll = CheckAll == true ? true : false;
        }
    };
    FunctionComponent.prototype.ActionTableFunc = function () {
        var _this = this;
        switch (this.ActionId) {
            case 1:
                var data_1 = [];
                this.listFunction.forEach(function (item) {
                    if (item.Action == true) {
                        data_1.push(item.FunctionId);
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
                                    _this.http.put('/api/function/deletes', data_1, _this.httpOptions).subscribe(function (res) {
                                        if (res["meta"]["error_code"] == 200) {
                                            _this.toastSuccess("Xóa thành công!");
                                            _this.GetListFunction();
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
    FunctionComponent.prototype.closeNoityfy = function () {
        this.isNoitify = true;
    };
    __decorate([
        core_1.ViewChild('modalFunction'),
        __metadata("design:type", modal_1.ModalDirective)
    ], FunctionComponent.prototype, "modalFunction", void 0);
    FunctionComponent = __decorate([
        core_1.Component({
            selector: 'app-function',
            templateUrl: './function.component.html',
            styleUrls: ['./function.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient, ngx_modal_dialog_1.ModalDialogService, core_1.ViewContainerRef, ngx_toastr_1.ToastrService])
    ], FunctionComponent);
    return FunctionComponent;
}());
exports.FunctionComponent = FunctionComponent;
//# sourceMappingURL=function.component.js.map