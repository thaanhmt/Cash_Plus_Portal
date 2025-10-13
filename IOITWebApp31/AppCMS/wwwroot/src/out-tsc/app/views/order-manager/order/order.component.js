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
exports.OrderComponent = void 0;
var core_1 = require("@angular/core");
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var ngx_toastr_1 = require("ngx-toastr");
var model_1 = require("../../../data/model");
var const_1 = require("../../../data/const");
var OrderComponent = /** @class */ (function () {
    function OrderComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.listOrder = [];
        this.listOrderStatus = const_1.OrderStatus;
        this.listPaymentOrderStatus = const_1.PaymentOrderStatus;
        this.ActionTable = const_1.ActionTable;
        this.PriceCurrencyMaskConfig = {
            align: "left",
            allowNegative: false,
            decimal: ".",
            precision: 0,
            prefix: "",
            suffix: " vnđ",
            thousands: ","
        };
        this.Item = new model_1.Order();
        this.paging = {
            page: 1,
            page_size: 10,
            query: '1=1',
            order_by: 'OrderId Desc',
            item_count: 0
        };
        this.pagingItem = {
            page: 1,
            page_size: 10,
            query: '',
            order_by: '',
            item_count: 0
        };
        this.q = {
            txtSearch: ''
        };
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    OrderComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListOrder();
    };
    //Get danh sách danh mục đơn hàng
    OrderComponent.prototype.GetListOrder = function () {
        var _this = this;
        this.http.get('/api/order/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listOrder = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    OrderComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListOrder();
    };
    //Toast cảnh báo
    OrderComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    OrderComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    OrderComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    OrderComponent.prototype.QueryChanged = function () {
        var query = '';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and Code.Contains("' + this.q.txtSearch + '")';
            }
            else {
                query += 'Code.Contains("' + this.q.txtSearch + '")';
            }
        }
        if (this.q.TypeOrderStatus != undefined) {
            if (query != '') {
                query += ' and OrderStatusId=' + this.q.TypeOrderStatus;
            }
            else {
                query += 'OrderStatusId=' + this.q.TypeOrderStatus;
            }
        }
        if (this.q.TypePaymentOrderStatus != undefined) {
            if (query != '') {
                query += ' and PaymentStatusId=' + this.q.TypePaymentOrderStatus;
            }
            else {
                query += 'PaymentStatusId=' + this.q.TypePaymentOrderStatus;
            }
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListOrder();
    };
    //
    //Open modal view
    OrderComponent.prototype.OpenViewModal = function (item) {
        this.Item = new model_1.Order();
        this.Item = Object.assign({}, item);
        this.viewModal.show();
    };
    //Popup xác nhận xóa
    OrderComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        _this.http.delete('/api/Order/' + Id, _this.httpOptions).subscribe(function (res) {
                            if (res["meta"]["error_code"] == 200) {
                                _this.GetListOrder();
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
    OrderComponent.prototype.SortTable = function (str) {
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
        this.GetListOrder();
    };
    OrderComponent.prototype.GetClassSortTable = function (str) {
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
    OrderComponent.prototype.ChangeOrderStatus = function (OrderId, Status) {
        var _this = this;
        this.http.put('/api/order/ChangeOrderStatus/' + OrderId + '/' + Status, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListOrder();
                _this.toastSuccess(res["meta"]["error_message"]);
            }
            else {
                _this.toastError(res["meta"]["error_message"]);
                _this.GetListOrder();
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            _this.GetListOrder();
        });
    };
    OrderComponent.prototype.ChangePaymentOrderStatus = function (OrderId, Status) {
        var _this = this;
        this.http.put('/api/order/ChangePaymentOrderStatus/' + OrderId + '/' + Status, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListOrder();
                _this.toastSuccess(res["meta"]["error_message"]);
            }
            else {
                _this.toastError(res["meta"]["error_message"]);
                _this.GetListOrder();
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            _this.GetListOrder();
        });
    };
    OrderComponent.prototype.CheckActionTable = function (OrderId) {
        if (OrderId == undefined) {
            var CheckAll_1 = this.CheckAll;
            this.listOrder.forEach(function (item) {
                item.Action = CheckAll_1;
            });
        }
        else {
            var CheckAll = true;
            for (var i = 0; i < this.listOrder.length; i++) {
                if (!this.listOrder[i].Action) {
                    CheckAll = false;
                    break;
                }
            }
            this.CheckAll = CheckAll == true ? true : false;
        }
    };
    OrderComponent.prototype.ActionTableFunc = function () {
        var _this = this;
        switch (this.ActionId) {
            case 1:
                var data_1 = [];
                this.listOrder.forEach(function (item) {
                    if (item.Action == true) {
                        data_1.push(item.OrderId);
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
                                    _this.http.put('/api/order/deletes', data_1, _this.httpOptions).subscribe(function (res) {
                                        if (res["meta"]["error_code"] == 200) {
                                            _this.toastSuccess("Xóa thành công!");
                                            _this.GetListOrder();
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
    __decorate([
        core_1.ViewChild('ViewModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], OrderComponent.prototype, "viewModal", void 0);
    OrderComponent = __decorate([
        core_1.Component({
            selector: 'app-order',
            templateUrl: './order.component.html',
            styleUrls: ['./order.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService])
    ], OrderComponent);
    return OrderComponent;
}());
exports.OrderComponent = OrderComponent;
//# sourceMappingURL=order.component.js.map