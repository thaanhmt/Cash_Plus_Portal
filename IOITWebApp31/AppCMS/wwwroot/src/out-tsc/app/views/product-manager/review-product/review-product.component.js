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
exports.ReviewProductComponent = void 0;
var core_1 = require("@angular/core");
var dt_1 = require("../../../data/dt");
var const_1 = require("../../../data/const");
var http_1 = require("@angular/common/http");
var ngx_toastr_1 = require("ngx-toastr");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var ReviewProductComponent = /** @class */ (function () {
    function ReviewProductComponent(http, toastr, modalDialogService, viewRef) {
        this.http = http;
        this.toastr = toastr;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.listProductReviews = [];
        this.ProductReviewStatus = const_1.ProductReviewStatus;
        this.ActionTable = const_1.ActionTable;
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    ReviewProductComponent.prototype.ngOnInit = function () {
        this.GetListProductReviews();
    };
    ReviewProductComponent.prototype.GetListProductReviews = function () {
        var _this = this;
        this.http.get('/api/product/ProductReview/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listProductReviews = res["data"];
                _this.paging.item_count = res["metadata"].Sum;
                _this.total = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    ReviewProductComponent.prototype.SortTable = function (str) {
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
        this.GetListProductReviews();
    };
    ReviewProductComponent.prototype.GetClassSortTable = function (str) {
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
    ReviewProductComponent.prototype.ChangeStatusProductReview = function (ProductReviewId, Status) {
        var _this = this;
        this.http.put('/api/Product/ChangeStatusProductReview/' + ProductReviewId + "/" + Status, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.toastSuccess("Thay đổi trạng thái thành công!");
                _this.GetListProductReviews();
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                _this.GetListProductReviews();
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            _this.GetListProductReviews();
        });
    };
    //Chuyển trang
    ReviewProductComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListProductReviews();
    };
    //Toast cảnh báo
    ReviewProductComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    ReviewProductComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    ReviewProductComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    ReviewProductComponent.prototype.QueryChanged = function () {
        var query = '';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and (Name.Contains("' + this.q.txtSearch + '") OR Email.Contains("' + this.q.txtSearch + '") OR ProductName.Contains("' + this.q.txtSearch + '"))';
            }
            else {
                query += '(Name.Contains("' + this.q.txtSearch + '") OR Email.Contains("' + this.q.txtSearch + '") OR ProductName.Contains("' + this.q.txtSearch + '"))';
            }
        }
        if (this.q["Type"] != undefined) {
            if (query != '') {
                query += ' and Status=' + this.q["Type"];
            }
            else {
                query += 'Status=' + this.q["Type"];
            }
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListProductReviews();
    };
    ReviewProductComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        _this.http.delete('/api/Product/deleteProductReview/' + Id, _this.httpOptions).subscribe(function (res) {
                            if (res["meta"]["error_code"] == 200) {
                                _this.GetListProductReviews();
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
    ReviewProductComponent.prototype.CheckActionTable = function (ProductReviewId) {
        if (ProductReviewId == undefined) {
            var CheckAll_1 = this.CheckAll;
            this.listProductReviews.forEach(function (item) {
                item.Action = CheckAll_1;
            });
        }
        else {
            var CheckAll = true;
            for (var i = 0; i < this.listProductReviews.length; i++) {
                if (!this.listProductReviews[i].Action) {
                    CheckAll = false;
                    break;
                }
            }
            this.CheckAll = CheckAll == true ? true : false;
        }
    };
    ReviewProductComponent.prototype.ActionTableFunc = function () {
        var _this = this;
        switch (this.ActionId) {
            case 1:
                var data_1 = [];
                this.listProductReviews.forEach(function (item) {
                    if (item.Action == true) {
                        data_1.push(item.ProductReviewId);
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
                                    _this.http.put('/api/Product/deleteProductReviews', data_1, _this.httpOptions).subscribe(function (res) {
                                        if (res["meta"]["error_code"] == 200) {
                                            _this.toastSuccess("Xóa thành công!");
                                            _this.GetListProductReviews();
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
    ReviewProductComponent = __decorate([
        core_1.Component({
            selector: 'app-review-product',
            templateUrl: './review-product.component.html',
            styleUrls: ['./review-product.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient, ngx_toastr_1.ToastrService, ngx_modal_dialog_1.ModalDialogService, core_1.ViewContainerRef])
    ], ReviewProductComponent);
    return ReviewProductComponent;
}());
exports.ReviewProductComponent = ReviewProductComponent;
//# sourceMappingURL=review-product.component.js.map