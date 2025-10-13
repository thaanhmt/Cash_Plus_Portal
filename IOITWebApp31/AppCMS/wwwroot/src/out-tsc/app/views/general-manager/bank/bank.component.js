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
exports.BankComponent = void 0;
var core_1 = require("@angular/core");
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var model_1 = require("../../../data/model");
var ngx_toastr_1 = require("ngx-toastr");
var BankComponent = /** @class */ (function () {
    function BankComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.listBank = [];
        this.listCompany = [];
        this.showSort = [true, false];
        this.newItem = new model_1.Bank();
        this.editItem = new model_1.Bank();
        this.paging = {
            page: 1,
            page_size: 10,
            query: '1=1',
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
    BankComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListBank();
    };
    BankComponent.prototype.Sort = function (IsK, s, i) {
        if (IsK) {
            this.paging.order_by = s + " asc";
        }
        else {
            this.paging.order_by = s + " desc";
        }
        this.GetListBank();
        //this.showSort = !IsK;
        for (var j = 0; j < this.showSort.length; j++) {
            if (j == i) {
                this.showSort[j] = IsK;
            }
            else {
                this.showSort[j] = false;
            }
        }
    };
    //GET
    BankComponent.prototype.GetListBank = function () {
        var _this = this;
        this.http.get('/api/bank/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listBank = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    BankComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListBank();
    };
    //Thông báo
    BankComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    BankComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    BankComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    BankComponent.prototype.QueryChanged = function () {
        var query = '';
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
        this.GetListBank();
    };
    //Mở modal
    BankComponent.prototype.OpenAddModal = function () {
        this.newItem = new model_1.Bank();
        //this.GetListCompany(undefined);
        this.addModal.show();
    };
    //Thêm mới
    BankComponent.prototype.AddFunc = function () {
        var _this = this;
        if (this.newItem.Name == undefined || this.newItem.Name == '') {
            this.toastWarning("Chưa nhập Tên!");
            return;
        }
        else if (this.newItem.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên!");
            return;
        }
        else if (this.newItem.AccountId == undefined || this.newItem.AccountId == '') {
            this.toastWarning("Chưa nhập tài khoản!");
            return;
        }
        else if (this.newItem.AccountId.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tài khoản!");
            return;
        }
        else if (this.newItem.AccountName == undefined || this.newItem.AccountName == '') {
            this.toastWarning("Chưa nhập tên tài khoản");
            return;
        }
        else if (this.newItem.AccountName.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên tài khoản!");
            return;
        }
        else if (this.newItem.BranchName == undefined || this.newItem.BranchName == '') {
            this.toastWarning("Chưa nhập tên chi nhánh!");
            return;
        }
        else if (this.newItem.BranchName.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập chi nhánh!");
            return;
        }
        this.newItem.UserId = parseInt(localStorage.getItem("userId"));
        this.newItem.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.http.post('/api/Bank', this.newItem, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListBank();
                _this.addModal.hide();
                _this.toastSuccess("Thêm thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
        });
    };
    //Mở edit Modal
    BankComponent.prototype.OpenEditModal = function (item) {
        this.editItem = new model_1.Bank();
        this.editItem = Object.assign(this.editItem, item);
        this.editModal.show();
    };
    // cập nhật
    BankComponent.prototype.EditFunc = function () {
        var _this = this;
        if (this.editItem.Name == undefined || this.editItem.Name == '') {
            this.toastWarning("Chưa nhập Tên!");
            return;
        }
        else if (this.editItem.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên!");
            return;
        }
        else if (this.editItem.AccountId == undefined || this.editItem.AccountId == '') {
            this.toastWarning("Chưa nhập tài khoản!");
            return;
        }
        else if (this.editItem.AccountId.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tài khoản!");
            return;
        }
        else if (this.editItem.AccountName == undefined || this.editItem.AccountName == '') {
            this.toastWarning("Chưa nhập tên tài khoản");
            return;
        }
        else if (this.editItem.AccountName.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên tài khoản!");
            return;
        }
        else if (this.editItem.BranchName == undefined || this.editItem.BranchName == '') {
            this.toastWarning("Chưa nhập tên chi nhánh!");
            return;
        }
        else if (this.editItem.BranchName.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên chi nhánh!");
            return;
        }
        this.editItem.UserId = parseInt(localStorage.getItem("userId"));
        this.editItem.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.http.put('/api/Bank/' + this.editItem.BankId, this.editItem, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListBank();
                _this.editModal.hide();
                _this.toastSuccess("Cập nhật thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    //Popup xác nhận xóa
    BankComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        console.log('OnAction');
                        _this.Delete(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-default',
                }
            ],
        });
    };
    BankComponent.prototype.Delete = function (Id) {
        var _this = this;
        this.http.delete('/api/Bank/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListBank();
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
    __decorate([
        core_1.ViewChild('AddModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], BankComponent.prototype, "addModal", void 0);
    __decorate([
        core_1.ViewChild('EditModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], BankComponent.prototype, "editModal", void 0);
    BankComponent = __decorate([
        core_1.Component({
            selector: 'app-bank',
            templateUrl: './bank.component.html',
            styleUrls: ['./bank.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService])
    ], BankComponent);
    return BankComponent;
}());
exports.BankComponent = BankComponent;
//# sourceMappingURL=bank.component.js.map