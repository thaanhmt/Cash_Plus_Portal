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
exports.UploadComponent = void 0;
var core_1 = require("@angular/core");
var platform_browser_1 = require("@angular/platform-browser");
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var const_1 = require("../../data/const");
var ngx_toastr_1 = require("ngx-toastr");
var dt_1 = require("../../data/dt");
var UploadComponent = /** @class */ (function () {
    function UploadComponent(http, modalDialogService, viewRef, toastr, sanitizer) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.sanitizer = sanitizer;
        this.isNoitify = false;
        this.isDelay = false;
        this.ActionTable = const_1.ActionTable;
        this.listTypeMedia = const_1.listTypeMedia;
        this.listItemMedia = const_1.listItemMedia;
        this.domainMedia = const_1.domainMedia;
        this.domain = const_1.domain;
        this.domainDebug = const_1.domainDebug;
        this.pagingFile = new dt_1.Paging();
        this.pagingFile.page = 1;
        this.pagingFile.page_size = 24;
        this.pagingFile.query = "1=1";
        this.pagingFile.order_by = "";
        this.pagingFile.item_count = 0;
        this.countMedia = 24;
        //
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    UploadComponent.prototype.ngOnInit = function () {
        this.GetListFiles();
        this.GetDomainStatic();
    };
    UploadComponent.prototype.GetDomainStatic = function () {
        var _this = this;
        this.http.get('api/Config/1', this.httpOptions).subscribe(function (res) {
            _this.staticDomain = res["data"].Website;
            if (res["meta"]["error_code"] == 200) {
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
    //Thông báo
    UploadComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    UploadComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    UploadComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    /* To copy Text from Textbox */
    UploadComponent.prototype.copyInputMessage = function (inputElement) {
        inputElement.select();
        document.execCommand('copy');
        inputElement.setSelectionRange(0, 0);
        this.toastSuccess("Đã copy vào bộ nhớ tạm!");
    };
    UploadComponent.prototype.OpenAddUploadModal = function (item, type) {
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
        this.NewUploadModal.show();
    };
    UploadComponent.prototype.OpenMediaModal = function (item) {
        this.UploadModal.show();
        this.DetailMediaName = item.name;
        this.DetailMediaType = item.type;
        this.DetailMediaExtension = item.extension;
        this.DetailMediaDate = item.dateCreate;
        this.DetailMediaSize = item.size;
        this.DetailMediaUrl = item.url;
        this.DetailMediaWidth = item.width;
        this.DetailMediaHeight = item.height;
        this.DetailMediaAlt = item.alt;
        this.DetailMediaNote = item.note;
        this.DetailMediaUserName = item.userName;
    };
    UploadComponent.prototype.CloseMediaModal = function () {
        this.UploadModal.hide();
    };
    UploadComponent.prototype.CloseAddUploadModal = function () {
        this.NewUploadModal.hide();
    };
    UploadComponent.prototype.closeNoityfy = function () {
        this.isNoitify = true;
    };
    UploadComponent.prototype.upload = function (files, cs) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
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
                        _this.GetListFiles();
                        _this.message = event.body["data"].toString();
                        _this.toastSuccess("Tải lên thành công");
                        _this.file.nativeElement.value = "";
                        _this.message = undefined;
                        _this.progress = undefined;
                        _this.CloseAddUploadModal();
                        break;
                    default:
                        break;
                }
            }
        });
    };
    UploadComponent.prototype.RemoveImage = function () {
        this.message = undefined;
        this.progress = undefined;
    };
    UploadComponent.prototype.QueryTypeMedia = function () {
        this.pagingFile.page = 1;
        this.countMedia = 24;
        this.pagingFile.select = undefined;
        if (this.typeMedia != undefined)
            this.pagingFile.select = this.typeMedia + "";
        this.GetListFiles();
    };
    UploadComponent.prototype.QuerySearchMedia = function () {
        this.pagingFile.page = 1;
        this.countMedia = 24;
        var query = "1=1";
        if (this.searchMedia != undefined && this.searchMedia != '') {
            if (query != '') {
                query += ' and name.Contains("' + this.searchMedia + '")';
            }
        }
        this.pagingFile.query = query;
        this.GetListFiles();
    };
    UploadComponent.prototype.GetListFiles = function () {
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
    UploadComponent.prototype.loadMore = function () {
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
    UploadComponent.prototype.UpdateMedia = function () {
        var _this = this;
        var obj = {};
        obj["Name"] = this.DetailMediaName;
        obj["TargetId"] = this.DetailMediaType;
        obj["Url"] = this.DetailMediaAlt;
        obj["Thumb"] = this.DetailMediaNote;
        obj["Note"] = this.DetailMediaName;
        this.http.post('/api/FileManager/UpdateInfoFile', obj, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListFiles();
                _this.CloseMediaModal();
                _this.viewRef.clear();
                _this.toastSuccess("Cập nhật thành công!");
            }
            else {
                _this.toastError(res["meta"]["error_message"]);
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    UploadComponent.prototype.ShowConfirmDelete = function () {
        var _this = this;
        var text = "Bạn có chắc chắn muốn xóa file này?";
        if (this.DetailMediaType == 1) {
            text = "Bạn có chắc chắn muốn xóa hình ảnh này?";
        }
        this.modalDialogService.openDialog(this.viewRef, {
            title: 'Xác nhận',
            childComponent: ngx_modal_dialog_1.SimpleModalComponent,
            data: {
                text: text
            },
            actionButtons: [
                {
                    text: 'Đồng ý',
                    buttonClass: 'btn btn-success',
                    onAction: function () {
                        _this.DeleteMedia();
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-danger',
                }
            ],
        });
    };
    UploadComponent.prototype.DeleteMedia = function () {
        var _this = this;
        var obj = {};
        obj["currentFolder"] = this.DetailMediaUrl;
        obj["fileName"] = this.DetailMediaName;
        obj["type"] = this.DetailMediaType;
        this.http.post('/api/FileManager/DeleteFile', obj, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListFiles();
                _this.CloseMediaModal();
                _this.viewRef.clear();
                _this.toastSuccess("Xóa thành công!");
            }
            else {
                _this.toastError(res["meta"]["error_message"]);
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    __decorate([
        core_1.ViewChild('NewUploadModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], UploadComponent.prototype, "NewUploadModal", void 0);
    __decorate([
        core_1.ViewChild('UploadModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], UploadComponent.prototype, "UploadModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], UploadComponent.prototype, "file", void 0);
    UploadComponent = __decorate([
        core_1.Component({
            selector: 'app-uploadt',
            templateUrl: './upload.component.html',
            styleUrls: ['./upload.component.scss'],
            providers: []
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            platform_browser_1.DomSanitizer])
    ], UploadComponent);
    return UploadComponent;
}());
exports.UploadComponent = UploadComponent;
//# sourceMappingURL=upload.component.js.map