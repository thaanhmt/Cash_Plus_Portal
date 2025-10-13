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
exports.CustomerComponent = exports.MY_CUSTOM_FORMATS = void 0;
var core_1 = require("@angular/core");
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var const_1 = require("../../../data/const");
var model_1 = require("../../../data/model");
var ngx_toastr_1 = require("ngx-toastr");
var common_1 = require("@angular/common");
var md5_1 = require("ts-md5/dist/md5");
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
var CustomerComponent = /** @class */ (function () {
    function CustomerComponent(http, modalDialogService, viewRef, toastr, datePipe) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.datePipe = datePipe;
        this.listCustomer = [];
        this.orders = [];
        this.domainImage = const_1.domainImage;
        this.TypeUser = const_1.TypeUser;
        this.ActionTable = const_1.ActionTable;
        this.Item = new model_1.Customer();
        this.ItemResetPassword = new model_1.ResetPasswordCustomerDTO();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "CustomerId Desc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    CustomerComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListCustomer();
    };
    //Get danh sach khach hang
    CustomerComponent.prototype.GetListCustomer = function () {
        var _this = this;
        this.http.get('/api/customer/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCustomer = res["data"];
                _this.paging.item_count = res["metadata"].Sum;
                _this.total = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    CustomerComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListCustomer();
    };
    //Toast cảnh báo
    CustomerComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    CustomerComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    CustomerComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //Search
    CustomerComponent.prototype.QueryChanged = function () {
        var query = '';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and (FullName.Contains("' + this.q.txtSearch + '") Or Email.Contains("' + this.q.txtSearch + '"))';
            }
            else {
                query += '(FullName.Contains("' + this.q.txtSearch + '") or Email.Contains("' + this.q.txtSearch + '"))';
            }
        }
        if (this.q["TypeUsertId"] != undefined) {
            if (this.q["TypeUsertId"] == 1) {
                if (query != '') {
                    query += 'IsEmailConfirm = true';
                }
                else {
                    query += 'IsEmailConfirm = true';
                }
            }
            else {
                if (query != '') {
                    query += 'IsEmailConfirm = false';
                }
                else {
                    query += 'IsEmailConfirm = false';
                }
            }
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListCustomer();
    };
    //Mở modal thêm mới
    CustomerComponent.prototype.OpenCustomerModal = function (item) {
        this.Item = new model_1.Customer();
        this.file.nativeElement.value = "";
        this.progress = undefined;
        if (item != undefined) {
            this.Item = JSON.parse(JSON.stringify(item));
        }
        this.CustomerModal.show();
    };
    //Thêm mới khách hàng
    CustomerComponent.prototype.SaveCustomer = function () {
        var _this = this;
        console.log('ee');
        if (this.Item.FullName == undefined || this.Item.FullName == '') {
            this.toastWarning("Chưa nhập tên!");
            return;
        }
        else if (this.Item.FullName.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên!");
            return;
        }
        else if (this.Item.Email == undefined || this.Item.Email == '') {
            this.toastWarning("Chưa nhập email!");
            return;
        }
        else if (this.Item.Email.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập email!");
            return;
        }
        else if ((this.Item.Password == undefined || this.Item.Password == '') && this.Item.CustomerId == undefined) {
            this.toastWarning("Chưa nhập mật khẩu!");
            return;
        }
        else if (this.Item.Phone == undefined || this.Item.Phone == '') {
            this.toastWarning("Chưa chọn số điện thoại!");
            return;
        }
        else if (this.Item.Phone.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập số điện thoại!");
            return;
        }
        else if ((this.Item.Password.length < 6) && this.Item.CustomerId == undefined) {
            this.toastWarning("Mật khẩu ít nhất 6 ký tự!");
            return;
        }
        else if ((this.Item.ConfirmPassword != this.Item.Password) && this.Item.CustomerId == undefined) {
            this.toastWarning("Mật khẩu không trùng khớp!");
            return;
        }
        for (var i = 0; i < this.listCustomer.length; i++) {
            if (this.listCustomer[i].Email == this.Item.Email) {
                this.toastWarning("Email đã tồn tại trong hệ thống!");
                return;
            }
        }
        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));
        if (typeof this.Item.Birthday === 'object' && this.Item.Birthday != undefined) {
            var Birthday = this.Item.Birthday.add(7, 'hours');
            this.Item.Birthday = Birthday.toISOString();
        }
        if (this.Item.CustomerId == undefined) {
            var obj = JSON.parse(JSON.stringify(this.Item));
            obj.Password = md5_1.Md5.hashStr(this.Item.Password).toString();
            // this.Item.Password = Md5.hashStr(this.Item.Password).toString();
            this.http.post('/api/Customer', obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListCustomer();
                    _this.CustomerModal.hide();
                    _this.toastSuccess("Thêm mới thành công!");
                }
                else {
                    _this.toastError(res["meta"]["error_message"]);
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            this.http.put('/api/Customer/' + this.Item.CustomerId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListCustomer();
                    _this.CustomerModal.hide();
                    _this.toastSuccess("Cập nhật thành công!");
                }
                else {
                    _this.toastError(res["meta"]["error_message"]);
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
    };
    //Popup xác nhận xóa
    CustomerComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        _this.DeleteCatePage(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-danger',
                }
            ],
        });
    };
    CustomerComponent.prototype.DeleteCatePage = function (Id) {
        var _this = this;
        this.http.delete('/api/Customer/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListCustomer();
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
    CustomerComponent.prototype.upload = function (files) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
            formData.append(file.name, file);
        }
        console.log(formData);
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload/uploadImage/6', formData, {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
        this.http.request(uploadReq).subscribe(function (event) {
            if (event.type === http_1.HttpEventType.UploadProgress)
                _this.progress = Math.round(100 * event.loaded / event.total);
            else if (event.type === http_1.HttpEventType.Response) {
                _this.Item.Avata = event.body["data"].toString();
            }
        });
    };
    //
    CustomerComponent.prototype.RemoveImage = function () {
        this.Item.Avata = undefined;
        this.file.nativeElement.value = "";
        this.progress = undefined;
    };
    CustomerComponent.prototype.SortTable = function (str) {
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
        this.GetListCustomer();
    };
    CustomerComponent.prototype.GetClassSortTable = function (str) {
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
    //Cấp lại mật khẩu cho khách hàng - gửi mật khẩu vào email
    CustomerComponent.prototype.ResetPasswordCustomerModal = function (CustomerId, FullName) {
        this.ItemResetPassword = new model_1.ResetPasswordCustomerDTO();
        this.ItemResetPassword.FullName = FullName;
        this.ItemResetPassword.CustomerId = CustomerId;
        this.ResetPasswordModal.show();
    };
    CustomerComponent.prototype.ResetPassCustomer = function () {
        var _this = this;
        if (this.ItemResetPassword.Password == undefined || this.ItemResetPassword.Password == '') {
            this.toastWarning("Chưa nhập mật khẩu!");
            return;
        }
        else if (this.ItemResetPassword.Password.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập mật khẩu!");
            return;
        }
        else if (this.ItemResetPassword.ConfirmPassword == undefined || this.ItemResetPassword.ConfirmPassword == '') {
            this.toastWarning("Chưa nhập mật khẩu xác nhận!");
            return;
        }
        else if (this.ItemResetPassword.ConfirmPassword.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập mật khẩu xác nhận!");
            return;
        }
        else if (this.ItemResetPassword.Password != this.ItemResetPassword.ConfirmPassword) {
            this.toastWarning("Mật khẩu xác nhận không chính xác!");
        }
        var obj = JSON.parse(JSON.stringify(this.ItemResetPassword));
        obj.PasswordInit = obj.Password;
        obj.Password = md5_1.Md5.hashStr(obj.Password).toString();
        obj.ConfirmPassword = md5_1.Md5.hashStr(obj.ConfirmPassword).toString();
        this.http.post('/api/Customer/ResetPassword/' + obj.CustomerId, obj, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.ItemResetPassword = new model_1.ResetPasswordCustomerDTO();
                _this.ResetPasswordModal.hide();
                _this.toastSuccess(res["meta"]["error_message"]);
            }
            else {
                _this.toastError(res["meta"]["error_message"]);
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    CustomerComponent.prototype.OpenOrdersModal = function (CustomerId, CustomerName) {
        var _this = this;
        this.CustomerName = CustomerName;
        this.orders = [];
        var query = "CustomerId=" + CustomerId;
        this.http.get('/api/order/GetByPage?page=1&query=' + query + '&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.orders = res["data"];
                _this.donhangcon = _this.orders[0].listOrderItems.length;
                console.log(_this.donhangcon);
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
        this.OrdersModal.show();
    };
    CustomerComponent.prototype.CheckActionTable = function (CustomerId) {
        if (CustomerId == undefined) {
            var CheckAll_1 = this.CheckAll;
            this.listCustomer.forEach(function (item) {
                item.Action = CheckAll_1;
            });
        }
        else {
            var CheckAll = true;
            for (var i = 0; i < this.listCustomer.length; i++) {
                if (!this.listCustomer[i].Action) {
                    CheckAll = false;
                    break;
                }
            }
            this.CheckAll = CheckAll == true ? true : false;
        }
    };
    CustomerComponent.prototype.ActionTableFunc = function () {
        var _this = this;
        switch (this.ActionId) {
            case 1:
                var data_1 = [];
                this.listCustomer.forEach(function (item) {
                    if (item.Action == true) {
                        data_1.push(item.CustomerId);
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
                                    _this.http.put('/api/customer/deletes', data_1, _this.httpOptions).subscribe(function (res) {
                                        if (res["meta"]["error_code"] == 200) {
                                            _this.toastSuccess("Xóa thành công!");
                                            _this.GetListCustomer();
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
    __decorate([
        core_1.ViewChild('CustomerModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], CustomerComponent.prototype, "CustomerModal", void 0);
    __decorate([
        core_1.ViewChild('ResetPasswordModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], CustomerComponent.prototype, "ResetPasswordModal", void 0);
    __decorate([
        core_1.ViewChild('OrdersModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], CustomerComponent.prototype, "OrdersModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], CustomerComponent.prototype, "file", void 0);
    CustomerComponent = __decorate([
        core_1.Component({
            selector: 'app-customer',
            templateUrl: './customer.component.html',
            styleUrls: ['./customer.component.scss'],
            providers: [
                { provide: ng_pick_datetime_1.DateTimeAdapter, useClass: ng_pick_datetime_moment_1.MomentDateTimeAdapter, deps: [ng_pick_datetime_1.OWL_DATE_TIME_LOCALE] },
                { provide: ng_pick_datetime_1.OWL_DATE_TIME_FORMATS, useValue: exports.MY_CUSTOM_FORMATS }
            ]
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_1.DatePipe])
    ], CustomerComponent);
    return CustomerComponent;
}());
exports.CustomerComponent = CustomerComponent;
//# sourceMappingURL=customer.component.js.map