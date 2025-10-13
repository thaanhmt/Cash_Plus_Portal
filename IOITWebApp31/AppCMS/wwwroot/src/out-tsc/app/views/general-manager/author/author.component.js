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
exports.AuthorComponent = void 0;
var core_1 = require("@angular/core");
var http_1 = require("@angular/common/http");
var modal_1 = require("ngx-bootstrap/modal");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var ngx_toastr_1 = require("ngx-toastr");
var dt_1 = require("../../../data/dt");
var model_1 = require("../../../data/model");
var const_1 = require("../../../data/const");
var AuthorComponent = /** @class */ (function () {
    function AuthorComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.isActiveMedia = true;
        this.isActiveUpload = false;
        this.isDelay = false;
        this.listItemMedia = [];
        this.domainMedia = const_1.domainMedia;
        this.domain = const_1.domain;
        this.listAuthors = [];
        this.listUsers = [];
        this.listTypeAuthor = const_1.listTypeAuthor;
        this.ActionTable = const_1.ActionTable;
        this.isNoitify = false;
        this.domainImage = const_1.domainImage;
        this.domainDebug = const_1.domainDebug;
        this.Item = new model_1.Author();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "AuthorId Desc";
        this.paging.item_count = 0;
        this.pagingFile = new dt_1.Paging();
        this.pagingFile.page = 1;
        this.pagingFile.page_size = 24;
        this.pagingFile.query = "1=1";
        this.pagingFile.order_by = "";
        this.pagingFile.item_count = 0;
        this.countMedia = 24;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    AuthorComponent.prototype.ngOnInit = function () {
        this.GetListAuthor();
        this.GetListUser();
        this.GetListFiles();
        this.GetDomainStatic();
    };
    AuthorComponent.prototype.GetDomainStatic = function () {
        var _this = this;
        this.http.get('api/Config/1', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.staticDomain = res["data"].Website;
                if (res["data"].ModeSite) {
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
    AuthorComponent.prototype.GetListAuthor = function () {
        var _this = this;
        this.http.get('/api/author/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listAuthors = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    AuthorComponent.prototype.GetListUser = function () {
        var _this = this;
        this.http.get('/api/userRole/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listUsers = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    AuthorComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListAuthor();
    };
    //Toast cảnh báo
    AuthorComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    AuthorComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    AuthorComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    AuthorComponent.prototype.QueryChanged = function () {
        var query = '1=1';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            query += ' and Name.Contains("' + this.q.txtSearch + '")';
        }
        if (this.q["Type"] != undefined) {
            query += ' and Type=' + this.q["Type"];
        }
        this.paging.query = query;
        this.GetListAuthor();
    };
    AuthorComponent.prototype.OpenModalFunction = function (item) {
        this.Item = new model_1.Author();
        if (this.file)
            this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
        if (item == undefined) {
            //this.GetListFunctionParent(undefined);
        }
        else {
            this.Item = Object.assign(this.Item, item);
            //if (this.Item.FunctionParentId == 0) this.Item.FunctionParentId = undefined;
            //this.GetListFunctionParent(item.FunctionId);
        }
        this.modalFunction.show();
    };
    AuthorComponent.prototype.SaveFunc = function () {
        var _this = this;
        if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập Tên tác giả!");
            return;
        }
        else if (this.Item.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập Tên tác giả!");
            return;
        }
        else if (this.Item.FullName.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập Tên đầy đủ tác giả!");
            return;
        }
        else if (this.Item.FullName == undefined || this.Item.FullName == '') {
            this.toastWarning("Chưa nhập Tên đầy đủ tác giả!");
            return;
        }
        else if (this.Item.Address == undefined || this.Item.Address == '') {
            this.toastWarning("Chưa nhập địa chỉ tác giả!");
            return;
        }
        else if (this.Item.Address.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập địa chỉ tác giả!");
            return;
        }
        else if (this.Item.NumberPhone == undefined || this.Item.NumberPhone == '') {
            this.toastWarning("Chưa nhập số điện thoại tác giả!");
            return;
        }
        else if (this.Item.Cccd == undefined || this.Item.Cccd == '') {
            this.toastWarning("Chưa nhập Căn cước/CMT tác giả!");
            return;
        }
        //this.Item.Type = 1;
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        var obj = JSON.parse(JSON.stringify(this.Item));
        if (this.Item.AuthorId == undefined) {
            this.http.post('/api/author', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListAuthor();
                    _this.modalFunction.hide();
                    _this.toastSuccess("Thêm mới thành công!");
                }
                else if (res["meta"]["error_code"] == 211) {
                    _this.toastError("Thông tin không đủ. Xin vui lòng thử lại!");
                }
                //else if (res["meta"]["error_code"] == 212) {
                //  this.toastError("Mã chức năng đã tồn tại. Xin vui lòng thử lại!");
                //}
                else {
                    _this.toastError(res["meta"]["error_message"]);
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            //if (obj.FunctionParentId == null) obj.FunctionParentId = 0;
            this.http.put('/api/author/' + obj.AuthorId, obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListAuthor();
                    _this.modalFunction.hide();
                    _this.toastSuccess("Cập nhật thành công!");
                }
                else if (res["meta"]["error_code"] == 211) {
                    _this.toastError("Thông tin không đủ. Xin vui lòng thử lại!");
                }
                //else if (res["meta"]["error_code"] == 212) {
                //  this.toastError("Mã chức năng đã tồn tại. Xin vui lòng thử lại!");
                //}
                //else if (res["meta"]["error_code"] == 215) {
                //  this.toastError("Chức năng cha không hợp lệ. Xin vui lòng thử lại!");
                //}
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
    };
    //Popup xác nhận xóa
    AuthorComponent.prototype.ShowConfirmDelete = function (Id) {
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
    AuthorComponent.prototype.Delete = function (Id) {
        var _this = this;
        this.http.delete('/api/author/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListAuthor();
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
    AuthorComponent.prototype.SortTable = function (str) {
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
        this.GetListAuthor();
    };
    AuthorComponent.prototype.GetClassSortTable = function (str) {
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
    AuthorComponent.prototype.CheckActionTable = function (Id) {
        if (Id == undefined) {
            var CheckAll_1 = this.CheckAll;
            this.listAuthors.forEach(function (item) {
                item.Action = CheckAll_1;
            });
        }
        else {
            var CheckAll = true;
            for (var i = 0; i < this.listAuthors.length; i++) {
                if (!this.listAuthors[i].Action) {
                    CheckAll = false;
                    break;
                }
            }
            this.CheckAll = CheckAll == true ? true : false;
        }
    };
    AuthorComponent.prototype.ActionTableFunc = function () {
        var _this = this;
        switch (this.ActionId) {
            case 1:
                var data_1 = [];
                this.listAuthors.forEach(function (item) {
                    if (item.Action == true) {
                        data_1.push(item.AuthorId);
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
                                    _this.http.put('/api/author/deletes', data_1, _this.httpOptions).subscribe(function (res) {
                                        if (res["meta"]["error_code"] == 200) {
                                            _this.toastSuccess("Xóa thành công!");
                                            _this.GetListAuthor();
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
    AuthorComponent.prototype.closeNoityfy = function () {
        this.isNoitify = true;
    };
    AuthorComponent.prototype.upload = function (files, cs) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
            formData.append(file.name, file);
        }
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload/uploadImage/8', formData, {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
        this.http.request(uploadReq).subscribe(function (event) {
            if (event.type === http_1.HttpEventType.UploadProgress)
                switch (cs) {
                    case 1:
                        _this.progress = Math.round(100 * event.loaded / event.total);
                        break;
                    default:
                        break;
                }
            else if (event.type === http_1.HttpEventType.Response) {
                switch (cs) {
                    case 1:
                        _this.message = event.body["data"].toString();
                        _this.Item.Avatar = _this.message;
                        break;
                    default:
                        break;
                }
            }
        });
    };
    AuthorComponent.prototype.RemoveImage = function () {
        this.Item.Avatar = undefined;
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
    };
    AuthorComponent.prototype.OpenMediaModal = function () {
        this.OpenMediaFile.show();
    };
    AuthorComponent.prototype.CloseMediaModal = function () {
        this.OpenMediaFile.hide();
    };
    AuthorComponent.prototype.tabHandleMedia = function () {
        this.isActiveMedia = true;
        this.isActiveUpload = false;
    };
    AuthorComponent.prototype.tabHandleMediaUpload = function () {
        this.isActiveMedia = false;
        this.isActiveUpload = true;
    };
    AuthorComponent.prototype.upload3 = function (files, cs) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_2 = files; _i < files_2.length; _i++) {
            var file = files_2[_i];
            formData.append(file.name, file);
        }
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload/uploadMedia/8', formData, {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
        this.http.request(uploadReq).subscribe(function (event) {
            if (event.type === http_1.HttpEventType.UploadProgress)
                switch (cs) {
                    case 1:
                        _this.progress = Math.round(100 * event.loaded / event.total);
                        break;
                    default:
                        break;
                }
            else if (event.type === http_1.HttpEventType.Response) {
                switch (cs) {
                    case 1:
                        _this.isActiveMedia = true;
                        _this.isActiveUpload = false;
                        _this.pagingFile.page = 1;
                        _this.GetListFiles();
                        _this.message = event.body["data"].toString();
                        _this.toastSuccess("Tải lên thành công");
                        break;
                    default:
                        break;
                }
            }
        });
    };
    AuthorComponent.prototype.RemoveImageAvatar = function () {
        this.Item.Avatar = undefined;
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
    };
    AuthorComponent.prototype.loadMore = function () {
        var _this = this;
        this.isDelay = true;
        setTimeout(function () {
            _this.isDelay = false;
            _this.pagingFile.page++;
            _this.http.get('/api/fileManager/GetFiles?page=' + _this.pagingFile.page + '&page_size=' + _this.pagingFile.page_size + '&query='
                + _this.pagingFile.query + '&order_by=' + '&select=' + _this.pagingFile.select, _this.httpOptions).subscribe(function (res) {
                var _a;
                if (res["meta"]["error_code"] == 200) {
                    (_a = _this.listItemMedia).push.apply(_a, res["data"]);
                    if (_this.countMedia >= _this.countAllMedia) {
                        _this.countMedia = _this.countAllMedia;
                    }
                    else {
                        if ((_this.countMedia + 24) >= _this.countAllMedia) {
                            _this.countMedia = _this.countAllMedia;
                        }
                        else {
                            _this.countMedia += 24;
                        }
                    }
                }
            }, function (err) {
                console.log("Error: connect to API");
            });
        }, 1000);
    };
    AuthorComponent.prototype.GetListFiles = function () {
        var _this = this;
        this.http.get('/api/fileManager/GetFiles?page=' + this.pagingFile.page + '&page_size=' + this.pagingFile.page_size + '&query='
            + this.pagingFile.query + '&order_by=' + '&select=' + this.pagingFile.select, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listItemMedia = res["data"];
                _this.countAllMedia = res["metadata"];
                if (_this.countAllMedia < 24)
                    _this.countMedia = _this.countAllMedia;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    AuthorComponent.prototype.SeclectMedia = function (item) {
        this.Item.Avatar = item.url + "/" + item.name;
        this.OpenMediaFile.hide();
    };
    __decorate([
        core_1.ViewChild('modalFunction'),
        __metadata("design:type", modal_1.ModalDirective)
    ], AuthorComponent.prototype, "modalFunction", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], AuthorComponent.prototype, "file", void 0);
    __decorate([
        core_1.ViewChild('OpenMediaFile'),
        __metadata("design:type", modal_1.ModalDirective)
    ], AuthorComponent.prototype, "OpenMediaFile", void 0);
    AuthorComponent = __decorate([
        core_1.Component({
            selector: 'app-author',
            templateUrl: './author.component.html',
            styleUrls: ['./author.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient, ngx_modal_dialog_1.ModalDialogService, core_1.ViewContainerRef, ngx_toastr_1.ToastrService])
    ], AuthorComponent);
    return AuthorComponent;
}());
exports.AuthorComponent = AuthorComponent;
//# sourceMappingURL=author.component.js.map