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
exports.BacklinkComponent = void 0;
var core_1 = require("@angular/core");
var http_1 = require("@angular/common/http");
var modal_1 = require("ngx-bootstrap/modal");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var ngx_toastr_1 = require("ngx-toastr");
var dt_1 = require("../../../data/dt");
var model_1 = require("../../../data/model");
var const_1 = require("../../../data/const");
var BacklinkComponent = /** @class */ (function () {
    function BacklinkComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.listBacklink = [];
        this.ActionTable = const_1.ActionTable;
        this.isNoitify = false;
        this.domainImage = const_1.domainImage;
        this.Item = new model_1.Backlink();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "CreatedAt Desc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    BacklinkComponent.prototype.ngOnInit = function () {
        this.GetListBacklink();
        //this.GetListUser();
    };
    BacklinkComponent.prototype.GetListBacklink = function () {
        var _this = this;
        this.http.get('/api/backlink/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listBacklink = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //GetListUser() {
    //  this.http.get('/api/userRole/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(
    //    (res) => {
    //      if (res["meta"]["error_code"] == 200) {
    //        this.listUsers = res["data"];
    //      }
    //    },
    //    (err) => {
    //      console.log("Error: connect to API");
    //    });
    //}
    //Chuyển trang
    BacklinkComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListBacklink();
    };
    //Toast cảnh báo
    BacklinkComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    BacklinkComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    BacklinkComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    BacklinkComponent.prototype.QueryChanged = function () {
        var query = '1=1 AND Type=1';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            //if (query != '') {
            query += ' and (Name.Contains("' + this.q.txtSearch + '") OR Code.Contains("' + this.q.txtSearch + '"))';
            //}
            //else {
            //  query += '(Name.Contains("' + this.q.txtSearch + '") OR Code.Contains("' + this.q.txtSearch + '"))';
            //}
        }
        //if (query == '')
        //  this.paging.query = '1=1';
        //else
        this.paging.query = query;
        this.GetListBacklink();
    };
    BacklinkComponent.prototype.OpenModalFunction = function (item) {
        this.Item = new model_1.Backlink();
        if (item == undefined) {
        }
        else {
            this.Item = Object.assign(this.Item, item);
        }
        this.modalFunction.show();
    };
    BacklinkComponent.prototype.SaveFunc = function () {
        var _this = this;
        if (this.Item.LinkIn == undefined || this.Item.LinkIn == '') {
            this.toastWarning("Chưa nhập liên kết gốc!");
            return;
        }
        else if (this.Item.LinkIn.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập liên kết gốc!");
            return;
        }
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        var obj = JSON.parse(JSON.stringify(this.Item));
        if (this.Item.BackLinkId == undefined) {
            this.http.post('/api/backlink', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListBacklink();
                    _this.modalFunction.hide();
                    _this.toastSuccess("Thêm mới thành công!");
                }
                else if (res["meta"]["error_code"] == 211) {
                    _this.toastError("Thông tin không đủ. Xin vui lòng thử lại!");
                }
                else {
                    _this.toastError(res["meta"]["error_message"]);
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            //if (obj.FunctionParentId == null) obj.FunctionParentId = 0;
            this.http.put('/api/backlink/' + obj.BackLinkId, obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListBacklink();
                    _this.modalFunction.hide();
                    _this.toastSuccess("Cập nhật thành công!");
                }
                else if (res["meta"]["error_code"] == 211) {
                    _this.toastError("Thông tin không đủ. Xin vui lòng thử lại!");
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
    BacklinkComponent.prototype.ShowConfirmDelete = function (Id) {
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
    BacklinkComponent.prototype.Delete = function (Id) {
        var _this = this;
        this.http.delete('/api/backlink/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListBacklink();
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
    BacklinkComponent.prototype.SortTable = function (str) {
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
        this.GetListBacklink();
    };
    BacklinkComponent.prototype.GetClassSortTable = function (str) {
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
    BacklinkComponent.prototype.CheckActionTable = function (FunctionId) {
        if (FunctionId == undefined) {
            var CheckAll_1 = this.CheckAll;
            this.listBacklink.forEach(function (item) {
                item.Action = CheckAll_1;
            });
        }
        else {
            var CheckAll = true;
            for (var i = 0; i < this.listBacklink.length; i++) {
                if (!this.listBacklink[i].Action) {
                    CheckAll = false;
                    break;
                }
            }
            this.CheckAll = CheckAll == true ? true : false;
        }
    };
    BacklinkComponent.prototype.ActionTableFunc = function () {
        var _this = this;
        switch (this.ActionId) {
            case 1:
                var data_1 = [];
                this.listBacklink.forEach(function (item) {
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
                                    _this.http.put('/api/backlink/deletes', data_1, _this.httpOptions).subscribe(function (res) {
                                        if (res["meta"]["error_code"] == 200) {
                                            _this.toastSuccess("Xóa thành công!");
                                            _this.GetListBacklink();
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
                                buttonClass: 'btn btn-default',
                            }
                        ],
                    });
                }
                break;
            default:
                break;
        }
    };
    BacklinkComponent.prototype.closeNoityfy = function () {
        this.isNoitify = true;
    };
    __decorate([
        core_1.ViewChild('modalFunction'),
        __metadata("design:type", modal_1.ModalDirective)
    ], BacklinkComponent.prototype, "modalFunction", void 0);
    BacklinkComponent = __decorate([
        core_1.Component({
            selector: 'app-backlink',
            templateUrl: './backlink.component.html',
            styleUrls: ['./backlink.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient, ngx_modal_dialog_1.ModalDialogService, core_1.ViewContainerRef, ngx_toastr_1.ToastrService])
    ], BacklinkComponent);
    return BacklinkComponent;
}());
exports.BacklinkComponent = BacklinkComponent;
//# sourceMappingURL=backlink.component.js.map