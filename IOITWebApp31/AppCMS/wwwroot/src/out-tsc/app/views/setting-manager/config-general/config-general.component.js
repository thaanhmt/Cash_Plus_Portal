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
exports.ConfigGeneralComponent = void 0;
var core_1 = require("@angular/core");
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var model_1 = require("../../../data/model");
var ngx_toastr_1 = require("ngx-toastr");
var const_1 = require("../../../data/const");
var process_1 = require("process");
var dt_1 = require("../../../data/dt");
var ConfigGeneralComponent = /** @class */ (function () {
    function ConfigGeneralComponent(http, modalDialogService, viewRef, toastr) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.listConfig = [];
        this.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.typeUpload = const_1.TypeUpload;
        this.isNoitify = false;
        this.listItemMedia = [];
        this.domainMedia = const_1.domainMedia;
        this.domain = process_1.domain;
        this.isActiveMedia = true;
        this.isActiveUpload = false;
        this.isDelay = false;
        this.domainDebug = const_1.domainDebug;
        this.Item = new model_1.Website();
        //
        this.updateItem = new model_1.Config();
        //
        this.pagingFile = new dt_1.Paging();
        this.pagingFile.page = 1;
        this.pagingFile.page_size = 24;
        this.pagingFile.query = "1=1";
        this.pagingFile.order_by = "";
        this.pagingFile.item_count = 0;
        this.countMedia = 24;
        this.tabActive = 1;
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    ConfigGeneralComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListConfig(this.CompanyId);
        this.GetListFiles();
        this.GetDomainStatic();
        this.GetInfoWebsite();
    };
    ConfigGeneralComponent.prototype.GetListConfig = function (CompanyId) {
        var _this = this;
        this.http.get('/api/Config/' + CompanyId, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listConfig = res["data"];
                _this.updateItem = Object.assign(_this.updateItem, _this.listConfig);
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    ConfigGeneralComponent.prototype.GetDomainStatic = function () {
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
    ConfigGeneralComponent.prototype.GetInfoWebsite = function () {
        var _this = this;
        this.http.get('api/website/1', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.Item = new model_1.Website();
                _this.Item = JSON.parse(JSON.stringify(res["data"]));
                if (_this.Item.WebsiteParentId == 0)
                    _this.Item.WebsiteParentId = undefined;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Thông báo
    ConfigGeneralComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    ConfigGeneralComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    ConfigGeneralComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    ConfigGeneralComponent.prototype.changeTab = function (tab) {
        this.tabActive = tab;
    };
    ConfigGeneralComponent.prototype.Update = function () {
        var _this = this;
        if (this.tabActive == 1) {
            if (this.Item.Name == undefined || this.Item.Name == '') {
                this.toastWarning("Chưa nhập Tên!");
                return;
            }
            else if (this.Item.Name.replace(/ /g, '') == '') {
                this.toastWarning("Chưa nhập tên!");
                return;
            }
            else if (this.Item.Url == undefined || this.Item.Url == '') {
                this.toastWarning("Chưa nhập đường dẫn!");
                return;
            }
            else if (this.Item.Url.replace(/ /g, '') == '') {
                this.toastWarning("Chưa nhập đường dẫn!");
                return;
            }
            //else if (this.Item.LanguageId == undefined) {
            //  this.toastWarning("Chưa chọn ngôn ngữ!");
            //  return;
            //}
            this.Item.UserId = parseInt(localStorage.getItem("userId"));
            this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
            var obj = JSON.parse(JSON.stringify(this.Item));
            obj.WebsiteParentId = obj.WebsiteParentId == undefined ? 0 : obj.WebsiteParentId;
            this.http.put('/api/website/' + obj.WebsiteId, obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
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
            if (this.updateItem.EmailHost == undefined) {
                this.toastWarning("Chưa nhập Email Host!");
                return;
            }
            else if (this.updateItem.EmailHost.replace(/ /g, '') == '') {
                this.toastWarning("Chưa nhập Email host!");
                return;
            }
            else if (this.updateItem.EmailSender == undefined) {
                this.toastWarning("Chưa nhập Email Sender!");
                return;
            }
            else if (this.updateItem.EmailSender.replace(/ /g, '') == '') {
                this.toastWarning("Chưa nhập Email Sender!");
                return;
            }
            else if (this.updateItem.EmailEnableSsl == undefined) {
                this.toastWarning("Chưa nhập Email Enable SSl");
                return;
            }
            else if (this.updateItem.EmailUserName == undefined) {
                this.toastWarning("Chưa nhập Email User Name!");
                return;
            }
            else if (this.updateItem.EmailUserName.replace(/ /g, '') == '') {
                this.toastWarning("Chưa nhập Email UserName!");
                return;
            }
            else if (this.updateItem.EmailPasswordHash == undefined) {
                this.toastWarning("Chưa nhập Email Password Hash");
                return;
            }
            else if (this.updateItem.EmailPasswordHash.replace(/ /g, '') == '') {
                this.toastWarning("Chưa nhập EmailPasswordHash!");
                return;
            }
            else if (this.updateItem.EmailPort == undefined) {
                this.toastWarning("Chưa nhập Email Port!");
                return;
            }
            this.http.put('/api/Config/' + this.updateItem.ConfigId, this.updateItem, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.toastSuccess("Cập nhật thành công!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
    };
    ConfigGeneralComponent.prototype.closeNoityfy = function () {
        this.isNoitify = true;
    };
    ConfigGeneralComponent.prototype.UpdateInfoWebsite = function () {
        var _this = this;
        if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập Tên!");
            return;
        }
        else if (this.Item.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên!");
            return;
        }
        else if (this.Item.Url == undefined || this.Item.Url == '') {
            this.toastWarning("Chưa nhập đường dẫn!");
            return;
        }
        else if (this.Item.Url.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập đường dẫn!");
            return;
        }
        //else if (this.Item.LanguageId == undefined) {
        //  this.toastWarning("Chưa chọn ngôn ngữ!");
        //  return;
        //}
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        var obj = JSON.parse(JSON.stringify(this.Item));
        obj.WebsiteParentId = obj.WebsiteParentId == undefined ? 0 : obj.WebsiteParentId;
        this.http.put('/api/website/' + obj.WebsiteId, obj, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.toastSuccess("Cập nhật thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    ConfigGeneralComponent.prototype.OpenMediaModal = function (type) {
        this.selectMedia = type;
        this.OpenMediaFile.show();
    };
    ConfigGeneralComponent.prototype.CloseMediaModal = function () {
        this.OpenMediaFile.hide();
    };
    ConfigGeneralComponent.prototype.tabHandleMedia = function () {
        this.isActiveMedia = true;
        this.isActiveUpload = false;
    };
    ConfigGeneralComponent.prototype.tabHandleMediaUpload = function () {
        this.isActiveMedia = false;
        this.isActiveUpload = true;
    };
    ConfigGeneralComponent.prototype.upload3 = function (files, cs) {
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
    ConfigGeneralComponent.prototype.loadMore = function () {
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
    ConfigGeneralComponent.prototype.GetListFiles = function () {
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
    ConfigGeneralComponent.prototype.SeclectMedia = function (item) {
        if (this.selectMedia == 0)
            this.Item.LogoHeader = item.url + "/" + item.name;
        else if (this.selectMedia == 1)
            this.Item.Icon1 = item.url + "/" + item.name;
        else if (this.selectMedia == 2)
            this.Item.Icon2 = item.url + "/" + item.name;
        else if (this.selectMedia == 3)
            this.Item.Icon3 = item.url + "/" + item.name;
        else if (this.selectMedia == 4)
            this.Item.Icon4 = item.url + "/" + item.name;
        else if (this.selectMedia == 5)
            this.Item.Icon5 = item.url + "/" + item.name;
        else if (this.selectMedia == 6)
            this.Item.Icon6 = item.url + "/" + item.name;
        else if (this.selectMedia == 7)
            this.Item.IconBct = item.url + "/" + item.name;
        this.OpenMediaFile.hide();
    };
    ConfigGeneralComponent.prototype.RemoveImage = function (key) {
        switch (key) {
            case 0:
                this.file.nativeElement.value = "";
                this.Item.LogoHeader = undefined;
                this.message = undefined;
                this.progress = undefined;
                break;
            case 1:
                this.file.nativeElement.value = "";
                this.Item.Icon1 = undefined;
                this.message = undefined;
                this.progress = undefined;
                break;
            case 2:
                this.file.nativeElement.value = "";
                this.Item.Icon2 = undefined;
                this.message = undefined;
                this.progress = undefined;
                break;
            case 3:
                this.file.nativeElement.value = "";
                this.Item.Icon3 = undefined;
                this.message = undefined;
                this.progress = undefined;
                break;
            case 4:
                this.file.nativeElement.value = "";
                this.Item.Icon4 = undefined;
                this.message = undefined;
                this.progress = undefined;
                break;
            case 5:
                this.file.nativeElement.value = "";
                this.Item.Icon5 = undefined;
                this.message = undefined;
                this.progress = undefined;
                break;
            case 6:
                this.file.nativeElement.value = "";
                this.Item.Icon6 = undefined;
                this.message = undefined;
                this.progress = undefined;
                break;
            case 7:
                this.file.nativeElement.value = "";
                this.Item.IconBct = undefined;
                this.message = undefined;
                this.progress = undefined;
                break;
            //case 8:
            //  this.fileFooter.nativeElement.value = "";
            //  this.Item.LogoFooter = undefined;
            //  this.messageFooter = undefined;
            //  this.progressFooter = undefined;
            //  break;
            //case 9:
            //  this.Item.Banner = undefined;
            //  this.messageBanner = undefined;
            //  this.progressBanner = undefined;
            //  break;
            default:
                break;
        }
    };
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], ConfigGeneralComponent.prototype, "file", void 0);
    __decorate([
        core_1.ViewChild('OpenMediaFile'),
        __metadata("design:type", modal_1.ModalDirective)
    ], ConfigGeneralComponent.prototype, "OpenMediaFile", void 0);
    ConfigGeneralComponent = __decorate([
        core_1.Component({
            selector: 'app-config-general',
            templateUrl: './config-general.component.html',
            styleUrls: ['./config-general.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService])
    ], ConfigGeneralComponent);
    return ConfigGeneralComponent;
}());
exports.ConfigGeneralComponent = ConfigGeneralComponent;
//# sourceMappingURL=config-general.component.js.map