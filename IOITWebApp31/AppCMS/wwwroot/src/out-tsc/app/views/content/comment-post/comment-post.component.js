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
exports.CommentPostComponent = exports.MY_CUSTOM_FORMATS = void 0;
var core_1 = require("@angular/core");
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var const_1 = require("../../../data/const");
var model_1 = require("../../../data/model");
var ngx_toastr_1 = require("ngx-toastr");
var common_1 = require("@angular/common");
var common_service_1 = require("../../../service/common.service");
var dt_1 = require("../../../data/dt");
var ng_pick_datetime_1 = require("ng-pick-datetime");
var ng_pick_datetime_moment_1 = require("ng-pick-datetime-moment");
exports.MY_CUSTOM_FORMATS = {
    parseInput: 'DD/MM/YYYY HH:mm',
    fullPickerInput: 'DD/MM/YYYY HH:mm',
    datePickerInput: 'DD/MM/YYYY',
    timePickerInput: ' HH:mm',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY'
};
var CommentPostComponent = /** @class */ (function () {
    function CommentPostComponent(http, modalDialogService, viewRef, toastr, datePipe, common) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.datePipe = datePipe;
        this.common = common;
        this.listCommentPost = [];
        this.ActionTable = const_1.ActionTable;
        this.domain = const_1.domain;
        this.listCustomer = [];
        this.listNews = [];
        this.ChoDuyet = 0;
        this.DaDuyet = 0;
        this.KhongDuyet = 0;
        this.Spam = 0;
        this.listId = [];
        this.page_pp = [];
        this.Item = new model_1.CommentPost();
        this.Itemc = new model_1.CommentPost();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "NewsId Desc";
        this.paging.item_count = 0;
        this.IsAll = false;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    CommentPostComponent.prototype.ngOnInit = function () {
        this.GetListCommentPost();
        //this.GetListCustomer();
        //this.GetListNews();
        //this.GetListCommentPostChoDuyet();
        //this.GetListCommentPostDaDuyet();
        //this.GetListCommentPostKhongDuyet();
        //this.GetListCommentPostSpam();
    };
    //Danh sách tất cả bình luận
    CommentPostComponent.prototype.GetListCommentPost = function () {
        var _this = this;
        this.page_pp = [];
        this.http.get('/api/comment/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=TargetType=1 AND (Status=1 OR Status=2 OR Status=3)&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCommentPost = res["data"];
                _this.paging.item_count = res["metadata"];
                _this.total = res["metadata"];
                for (var i = 0; i < _this.listCommentPost.length; i++) {
                    _this.listCommentPost[i].IsCheck = false;
                }
                _this.paging.item_count = res["success"]["totalPages"];
                for (var i = 0; i < _this.paging.item_count; i++) {
                    _this.page_pp.push(i);
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Danh sách tất cả bình luận cho duyet
    CommentPostComponent.prototype.GetListCommentPostChoDuyet = function () {
        var _this = this;
        this.http.get('/api/comment/GetByPage?page=' + this.paging.page + '&query=TargetType=1 AND Status=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.ChoDuyet = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Danh sách tất cả bình luận cho duyet
    CommentPostComponent.prototype.GetListCommentPostDaDuyet = function () {
        var _this = this;
        this.http.get('/api/comment/GetByPage?page=' + this.paging.page + '&query=TargetType=1 AND Status=2&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.DaDuyet = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Danh sách tất cả bình luận cho duyet
    CommentPostComponent.prototype.GetListCommentPostKhongDuyet = function () {
        var _this = this;
        this.http.get('/api/comment/GetByPage?page=' + this.paging.page + '&query=TargetType=1 AND Status=3&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.KhongDuyet = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Danh sách tất cả bình luận SPAM
    CommentPostComponent.prototype.GetListCommentPostSpam = function () {
        var _this = this;
        this.http.get('/api/comment/GetByPage?page=' + this.paging.page + '&query=TargetType=1 AND Status=4&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.Spam = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Get danh sach khach hang
    CommentPostComponent.prototype.GetListCustomer = function () {
        var _this = this;
        this.http.get('/api/customer/GetByPage?page=' + this.paging.page + '&query=' + this.paging.query + '&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCustomer = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Get danh sách danh bài viết
    CommentPostComponent.prototype.GetListNews = function () {
        var _this = this;
        this.http.get('/api/news/GetByPage?page=' + this.paging.page + '&query=' + this.paging.query + '&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listNews = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    // Duyet binh luan
    CommentPostComponent.prototype.DuyetComment = function (item, id) {
        var _this = this;
        this.Item = JSON.parse(JSON.stringify(item));
        this.http.put('/api/comment/ShowHide/' + item.CommentId + '/' + id, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListCommentPost();
                _this.GetListCommentPostChoDuyet();
                _this.GetListCommentPostDaDuyet();
                _this.GetListCommentPostKhongDuyet();
                _this.GetListCommentPostSpam();
                _this.CommentModal.hide();
                _this.toastSuccess("Duyệt bình luận thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    CommentPostComponent.prototype.DeleteComment = function (item, id) {
        var _this = this;
        this.Item = JSON.parse(JSON.stringify(item));
        this.http.put('/api/comment/ShowHide/' + item.CommentId + '/' + id, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListCommentPost();
                _this.GetListCommentPostChoDuyet();
                _this.GetListCommentPostDaDuyet();
                _this.GetListCommentPostKhongDuyet();
                _this.GetListCommentPostSpam();
                _this.CommentModal.hide();
                _this.toastSuccess("Xoá BL thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    // Loc theo trang thai
    CommentPostComponent.prototype.ChangeStatust = function (id) {
        var _this = this;
        if (id == 0) {
            this.GetListCommentPost();
            this.GetListCustomer();
            this.GetListNews();
            this.GetListCommentPostChoDuyet();
            this.GetListCommentPostDaDuyet();
            this.GetListCommentPostKhongDuyet();
            this.GetListCommentPostSpam();
        }
        else {
            this.http.get('/api/comment/GetByPage?page=' + this.paging.page + '&query=TargetType=1 AND Status =' + id + '&order_by=', this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.listCommentPost = res["data"];
                    _this.paging.item_count = res["metadata"];
                }
            }, function (err) {
                console.log("Error: connect to API");
            });
        }
    };
    CommentPostComponent.prototype.ActionTableFunc = function () {
        var _this = this;
        switch (this.ActionId) {
            case 1:
                var data_1 = [];
                this.listCommentPost.forEach(function (item) {
                    if (item.Action == true) {
                        data_1.push(item.NewsId);
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
                                    _this.http.put('/api/news/deletes', data_1, _this.httpOptions).subscribe(function (res) {
                                        if (res["meta"]["error_code"] == 200) {
                                            _this.toastSuccess("Xóa thành công!");
                                            _this.GetListCommentPost();
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
    //Mở modal thêm mới
    CommentPostComponent.prototype.OpenCommentModal = function (item) {
        this.Item = new model_1.CommentPost();
        this.Item.CommentParentId = 0;
        if (item != undefined) {
            this.Item = JSON.parse(JSON.stringify(item));
        }
        this.CommentModal.show();
    };
    CommentPostComponent.prototype.OpenRepComment = function (item) {
        if (item != undefined) {
            this.Item = JSON.parse(JSON.stringify(item));
        }
        this.Item.CommentParentId = this.Item.CommentId;
        this.Item.Contents = undefined;
        this.Item.CommentId = undefined;
        this.Item.CustomerId = undefined;
        this.RepComment.show();
    };
    //Thêm mới
    CommentPostComponent.prototype.SaveCommnent = function () {
        var _this = this;
        if (this.Item.TargetId == undefined || this.Item.TargetId == null) {
            this.toastWarning("Chưa nhập bài viết cần bình luận!");
            return;
        }
        else if (this.Item.CustomerId == undefined || this.Item.CustomerId == null) {
            this.toastWarning("Chưa chọn khách hàng bình luận!");
            return;
        }
        else if (this.Item.Contents == undefined || this.Item.Contents == '') {
            this.toastWarning("Chưa nhập nội dung bình luận!!");
            return;
        }
        this.Item.TargetType = 1;
        this.Item.Status = 1;
        if (this.Item.CommentId) {
            this.http.put('/api/comment/' + this.Item.CommentId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListCommentPost();
                    _this.CommentModal.hide();
                    _this.toastSuccess("Cập nhật thành công!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            this.http.post('/api/comment', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListCommentPost();
                    _this.GetListCommentPostChoDuyet();
                    _this.GetListCommentPostDaDuyet();
                    _this.GetListCommentPostKhongDuyet();
                    _this.GetListCommentPostSpam();
                    _this.CommentModal.hide();
                    _this.toastSuccess("Thêm thành công!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
            });
        }
    };
    // tra loi binh luan
    CommentPostComponent.prototype.ChangeCustomer = function (id) {
        console.log(id);
        this.Item.CustomerId = id;
    };
    CommentPostComponent.prototype.SaveCommnentChild = function () {
        var _this = this;
        console.log(this.Item.CustomerId);
        if (this.Item.CustomerId == undefined || this.Item.CustomerId == null) {
            this.toastWarning("Chưa chọn khách hàng bình luận!");
            return;
        }
        else if (this.Item.Contents == undefined || this.Item.Contents == '') {
            this.toastWarning("Chưa nhập nội dung bình luận!!");
            return;
        }
        this.http.post('/api/comment', this.Item, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListCommentPost();
                _this.GetListCommentPostChoDuyet();
                _this.GetListCommentPostDaDuyet();
                _this.GetListCommentPostKhongDuyet();
                _this.GetListCommentPostSpam();
                _this.RepComment.hide();
                _this.toastSuccess("Trả lời thành công thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
        });
    };
    //Chuyển trang
    CommentPostComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListCommentPost();
    };
    //Thông báo
    CommentPostComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    CommentPostComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    CommentPostComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    //Popup xác nhận xóa
    CommentPostComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        _this.http.delete('/api/comment/' + Id, _this.httpOptions).subscribe(function (res) {
                            if (res["meta"]["error_code"] == 200) {
                                _this.GetListCommentPost();
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
    // Chon hoac bo chon tat ca binh luan
    CommentPostComponent.prototype.CheckAllAddress = function () {
        this.listId = [];
        console.log(this.CheckitemAll);
        if (this.CheckitemAll == true) {
            for (var i = 0; i < this.listCommentPost.length; i++) {
                this.listCommentPost[i].IsCheck = false;
            }
        }
        else {
            for (var i = 0; i < this.listCommentPost.length; i++) {
                this.listCommentPost[i].IsCheck = true;
            }
        }
        for (var i = 0; i < this.listCommentPost.length; i++) {
            if (this.listCommentPost[i].IsCheck == true) {
                this.listId.push(this.listCommentPost[i].CommentId);
            }
        }
        console.log(this.listCommentPost);
        console.log(this.listId);
    };
    //Chon binh luan xoa
    CommentPostComponent.prototype.CheckDeleteAddress = function (item) {
        console.log(item.IsCheck);
        this.listId = [];
        if (item.IsCheck == false || item.IsCheck == undefined) {
            for (var i = 0; i < this.listCommentPost.length; i++) {
                if (this.listCommentPost[i].CommentId == item.CommentId) {
                    this.listCommentPost[i].IsCheck = false;
                }
            }
        }
        else {
            for (var i = 0; i < this.listCommentPost.length; i++) {
                if (this.listCommentPost[i].CommentId == item.CommentId) {
                    this.listCommentPost[i].IsCheck = true;
                }
            }
        }
        for (var i = 0; i < this.listCommentPost.length; i++) {
            if (this.listCommentPost[i].IsCheck == true) {
                this.listId.push(this.listCommentPost[i].CommentId);
            }
        }
        if (this.listId.length < this.listCommentPost.length) {
            this.CheckitemAll = false;
        }
        if (this.listId.length == this.listCommentPost.length) {
            this.CheckitemAll = true;
        }
        console.log(this.listId);
    };
    //Xoa nhieu
    CommentPostComponent.prototype.DeleteMuntiAddress = function () {
        var _this = this;
        console.log(this.listId.length);
        if (this.listId.length > 0) {
            this.http.put('/api/comment/deletes', this.listId, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListCommentPost();
                    _this.GetListCommentPostChoDuyet();
                    _this.GetListCommentPostDaDuyet();
                    _this.GetListCommentPostKhongDuyet();
                    _this.GetListCommentPostSpam();
                    _this.viewRef.clear();
                    _this.toastSuccess("Xóa thành công!");
                    _this.listId = [];
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            this.toastError("Chưa có bình luận nào được chọn !");
        }
    };
    __decorate([
        core_1.ViewChild('CommentModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], CommentPostComponent.prototype, "CommentModal", void 0);
    __decorate([
        core_1.ViewChild('RepComment'),
        __metadata("design:type", modal_1.ModalDirective)
    ], CommentPostComponent.prototype, "RepComment", void 0);
    CommentPostComponent = __decorate([
        core_1.Component({
            selector: 'app-comment-post',
            templateUrl: './comment-post.component.html',
            styleUrls: ['./comment-post.component.scss'],
            providers: [
                { provide: ng_pick_datetime_1.DateTimeAdapter, useClass: ng_pick_datetime_moment_1.MomentDateTimeAdapter, deps: [ng_pick_datetime_1.OWL_DATE_TIME_LOCALE] },
                { provide: ng_pick_datetime_1.OWL_DATE_TIME_FORMATS, useValue: exports.MY_CUSTOM_FORMATS }
            ]
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_1.DatePipe,
            common_service_1.CommonService])
    ], CommentPostComponent);
    return CommentPostComponent;
}());
exports.CommentPostComponent = CommentPostComponent;
//# sourceMappingURL=comment-post.component.js.map