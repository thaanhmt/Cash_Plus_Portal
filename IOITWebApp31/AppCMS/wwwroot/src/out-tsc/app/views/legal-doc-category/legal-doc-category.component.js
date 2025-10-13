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
exports.LegalDocCategoryComponent = void 0;
var core_1 = require("@angular/core");
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var const_1 = require("../../data/const");
var ngx_toastr_1 = require("ngx-toastr");
var model_1 = require("../../data/model");
var dt_1 = require("../../data/dt");
var common_service_1 = require("../../service/common.service");
var call_category_function_service_1 = require("../../service/call-category-function.service");
var router_1 = require("@angular/router");
var LegalDocCategoryComponent = /** @class */ (function () {
    function LegalDocCategoryComponent(http, modalDialogService, viewRef, toastr, common, callCategoryFunctionService, elm, router) {
        var _this = this;
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.common = common;
        this.callCategoryFunctionService = callCategoryFunctionService;
        this.elm = elm;
        this.router = router;
        this.isActiveMedia = true;
        this.isActiveUpload = false;
        this.isDelay = false;
        this.listItemMedia = [];
        this.domainMedia = const_1.domainMedia;
        this.domain = const_1.domain;
        this.listCateNews = [];
        this.listCateParent = [];
        this.listLanguage = [];
        this.listLanguageTemp = [];
        this.listOrderByCat = [];
        this.typeCategoryNews = const_1.typeCategoryNews;
        this.domainImage = const_1.domainImage;
        this.query = "arr=12";
        this.domainDebug = const_1.domainDebug;
        this.key = 'categorySorts';
        this.q = new dt_1.QueryFilter();
        this.Item = new model_1.Category();
        this.pagingFile = new dt_1.Paging();
        this.pagingFile.page = 1;
        this.pagingFile.page_size = 24;
        this.pagingFile.query = "1=1";
        this.pagingFile.order_by = "";
        this.pagingFile.item_count = 0;
        this.countMedia = 24;
        this.languageId = localStorage.getItem("languageId") != undefined ? parseInt(localStorage.getItem("languageId")) : 1;
        //this. = this.common.CheckAccessKey(localStorage.getItem("access_key"), "DBBTV") && this.RoleCode != 'BTV';
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
        this.CheckRole = new dt_1.CheckRole();
        this.functionCode = "DMCH";
        this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 0);
        this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 1);
        this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 2);
        this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 3);
        this.subscription = this.callCategoryFunctionService.getAction().subscribe(function (action) {
            if (action.TypeAction == 1) {
                _this.OpenCateNewsModal(undefined, action.CategoryId, 2); // thêm danh mục con
            }
            else if (action.TypeAction == 2) {
                _this.OpenCateNewsModal(action.CategoryId, undefined, 3); // sửa danh mục
            }
            else if (action.TypeAction == 3) {
                _this.ShowConfirmDelete(action.CategoryId); // xóa danh mục
            }
            else if (action.TypeAction == 5) {
                _this.OpenCateNewsModal(action.CategoryId, undefined, 5); // thêm danh mục vs ngôn ngữ mới
            }
            else if (action.TypeAction == 6) {
                _this.OpenCateNewsModal(action.CategoryId, undefined, 6); // sửa danh mục vs ngôn ngữ mới
            }
            else if (action.TypeAction == 7) {
                _this.ShowHide(action.CategoryId, action.IsShow); // Đổi trạng thái danh mục
            }
            else if (action.TypeAction == 8) {
                _this.OpenCateNewsModal(action.CategoryId, undefined, 8); //view danh mục
            }
        });
    }
    LegalDocCategoryComponent.prototype.ngOnInit = function () {
        this.GetListCateNews();
        this.GetListLanguage();
        this.GetListFiles();
        this.GetDomainStatic();
    };
    LegalDocCategoryComponent.prototype.GetDomainStatic = function () {
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
    LegalDocCategoryComponent.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
        this.router.onSameUrlNavigation = 'ignore';
    };
    //Get danh sách tin
    LegalDocCategoryComponent.prototype.GetListCateNews = function () {
        var _this = this;
        this.listCateNews = [];
        this.http.get('/api/category/GetCategorySort?' + this.query, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCateNews = res["data"];
                _this.total_item = res["metadata"];
                loadNestable();
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    LegalDocCategoryComponent.prototype.QueryChanged = function () {
        //      let query = "arr=12";
        //if (this.txtSearch != undefined && this.txtSearch != "") {
        //	this.query = query + "&txtSearch=" + this.txtSearch;
        //}
        //else {
        //	this.query = query;
        //}
        var query = "arr=12";
        if (this.q.LanguageId != undefined) {
            query = query + "&langId=" + this.q.LanguageId;
        }
        if (this.q.txtSearch != undefined && this.q.txtSearch != "") {
            query = query + "&txtSearch=" + this.q.txtSearch;
        }
        this.query = query;
        this.GetListCateNews();
    };
    // Get danh sách ngôn ngữ
    LegalDocCategoryComponent.prototype.GetListLanguage = function () {
        var _this = this;
        this.http.get('/api/Language/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listLanguage = res["data"];
                if (_this.listLanguage == undefined || _this.listLanguage == null)
                    _this.listLanguageTemp = [];
                else
                    _this.listLanguageTemp = _this.listLanguage;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    LegalDocCategoryComponent.prototype.GetTranslate = function (id) {
        var _this = this;
        var sl = this.Item.LanguageRootCode;
        var tl = this.Item.LanguageCode;
        this.ItemTranslate = new model_1.Category();
        this.http.get('/api/translate/' + id + '/' + sl + '/' + tl + '/1', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.ItemTranslate = res["data"];
                _this.Item.CategoryId = undefined;
                _this.Item.CategoryParentId = undefined;
                _this.Item.Name = _this.ItemTranslate.Name;
                _this.Item.Url = _this.ItemTranslate.Url;
                _this.Item.Description = _this.ItemTranslate.Description;
                _this.Item.Contents = _this.ItemTranslate.Contents;
                _this.Item.MetaTitle = _this.ItemTranslate.MetaTitle;
                _this.Item.MetaDescription = _this.ItemTranslate.MetaDescription;
                _this.Item.MetaKeyword = _this.ItemTranslate.MetaKeyword;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    LegalDocCategoryComponent.prototype.GetListOrderByCat = function () {
        var _this = this;
        this.http.get('api/category/listNews/' + this.Item.CategoryId, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listOrderByCat = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Open sắp xếp tin
    LegalDocCategoryComponent.prototype.OpenSortNewsModal = function (item) {
        this.Item = JSON.parse(JSON.stringify(item));
        this.listOrderByCat = [];
        this.GetListOrderByCat();
        this.SortNewsModal.show();
    };
    LegalDocCategoryComponent.prototype.SaveSortNews = function () {
        var _this = this;
        for (var i = this.listOrderByCat.length; i > 0; i--) {
            this.listOrderByCat[i - 1].Location = (this.listOrderByCat.length - i) + 1;
        }
        this.http.put('/api/category/sortCategoryMapping/' + this.Item.CategoryId, this.listOrderByCat, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.SortNewsModal.hide();
                _this.toastSuccess("Lưu thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    //Get danh sách danh mục cha
    LegalDocCategoryComponent.prototype.GetListCateParent = function (Id) {
        var _this = this;
        this.http.get('/api/category/GetByTree?arr=12&langId=' + this.Item.LanguageId, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCateParent = res["data"];
                _this.listCateParent.forEach(function (item) {
                    if (item.CategoryId == Id || item.Genealogy.indexOf(Id) != -1)
                        item.disabled = true;
                    item.Space = "";
                    for (var i = 0; i < (item.Level - 1) * 2; i++) {
                        item.Space += "-";
                    }
                });
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Thông báo
    LegalDocCategoryComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    LegalDocCategoryComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    LegalDocCategoryComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //Mở modal thêm mới
    LegalDocCategoryComponent.prototype.OpenCateNewsModal = function (CategoryId, CategoryParentId, type) {
        var _this = this;
        this.typeAction = type;
        this.Item = new model_1.Category();
        this.Item.Contents = undefined;
        this.Item.CategoryParentId = CategoryParentId;
        this.Item.TypeCategoryId = 1;
        this.Item.LanguageId = this.languageId;
        this.Item.Location = this.total_item + 1;
        this.file.nativeElement.value = "";
        /*this.fileIcon.nativeElement.value = "";*/
        this.message = undefined;
        this.messageIcon = undefined;
        this.progress = undefined;
        this.progressIcon = undefined;
        if (CategoryId != undefined) {
            this.http.get('/api/category/' + CategoryId, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.Item = Object.assign(_this.Item, res["data"]);
                    if (type == 3 || type == 6 || type == 8) {
                        if (_this.Item.CategoryParentId == 0)
                            _this.Item.CategoryParentId = undefined;
                    }
                    else if (type == 5) {
                        if (_this.listLanguage.length == res["data"].listLanguage.length + 1) {
                            _this.toastWarning("Bạn đã thêm đủ ngôn ngữ!");
                            return;
                        }
                        _this.listLanguageTemp = [];
                        _this.Item.CategoryId = undefined;
                        _this.Item.CategoryRootId = res["data"].CategoryId;
                        _this.Item.LanguageRootId = _this.Item.LanguageId;
                        _this.Item.LanguageRootCode = _this.Item["language"]["Code"];
                        _this.Item.LanguageId = undefined;
                        _this.Item.LanguageCode = undefined;
                        //check ngôn ngữ
                        for (var i = 0; i < _this.listLanguage.length; i++) {
                            var check = false;
                            if (_this.listLanguage[i].LanguageId == _this.languageId) {
                                check = true;
                            }
                            if (res["data"].listLanguage.length > 0) {
                                for (var j = 0; j < res["data"].listLanguage.length; j++) {
                                    if (_this.listLanguage[i].LanguageId == res["data"].listLanguage[j].LanguageId2) {
                                        check = true;
                                        break;
                                    }
                                }
                            }
                            if (!check) {
                                _this.listLanguageTemp.push(_this.listLanguage[i]);
                            }
                        }
                        if (_this.listLanguageTemp.length > 0) {
                            _this.Item.LanguageId = _this.listLanguageTemp[0].LanguageId;
                            _this.Item.LanguageCode = _this.listLanguageTemp[0].Code;
                        }
                        //Gọi api dịch ở đây
                        _this.GetTranslate(_this.Item.CategoryRootId);
                        //
                        _this.Item.CategoryParentId = undefined;
                    }
                    _this.GetListCateParent(_this.Item.CategoryId);
                    _this.CateNewsModal.show();
                }
                else {
                    _this.toastError("Không tìm thấy danh mục trên hệ thống!");
                    return;
                }
            }, function (err) {
                _this.toastError("Không tìm thấy danh mục trên hệ thống!");
                return;
            });
        }
        else {
            this.GetListCateParent(undefined);
            this.CateNewsModal.show();
        }
    };
    //Thêm mới danh mục trang
    LegalDocCategoryComponent.prototype.SaveCateNews = function () {
        var _this = this;
        if (this.Item.Code == undefined || this.Item.Code == '') {
            this.toastWarning("Chưa nhập Mã danh mục!");
            return;
        }
        else if (this.Item.Code.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập mã danh mục");
            return;
        }
        else if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập Tên danh mục!");
            return;
        }
        else if (this.Item.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên danh mục");
            return;
        }
        else if (this.Item.Url == undefined || this.Item.Url == '') {
            this.toastWarning("Chưa nhập Đường dẫn!");
            return;
        }
        else if (this.Item.Url.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập đường dẫn!");
            return;
        }
        else if (this.Item.LanguageId == undefined) {
            this.toastWarning("Chưa chọn ngôn ngữ!");
            return;
        }
        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));
        this.Item.TypeCategoryId = 12;
        if (!this.Item.LanguageId) {
            this.Item.LanguageId = parseInt(localStorage.getItem("languageId"));
        }
        if (this.Item.CategoryId) {
            this.http.put('/api/Category/' + this.Item.CategoryId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.ResetCurrentRouter();
                    _this.CateNewsModal.hide();
                    _this.toastSuccess("Cập nhật thành công!");
                }
                else if (res["meta"]["error_code"] == 213) {
                    _this.toastWarning("Mã đã tồn tại!");
                }
                else {
                    _this.toastError(res["meta"]["error_message"]);
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            this.http.post('/api/Category', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.ResetCurrentRouter();
                    _this.CateNewsModal.hide();
                    _this.toastSuccess("Thêm mới thành công!");
                }
                else if (res["meta"]["error_code"] == 213) {
                    _this.toastWarning("Mã đã tồn tại!");
                }
                else {
                    _this.toastError(res["meta"]["error_message"]);
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
    };
    LegalDocCategoryComponent.prototype.ChangeTitle = function (key) {
        if (this.Item.CategoryId == undefined) {
            switch (key) {
                case 1:
                    this.Item.MetaTitle = this.Item.Name;
                    this.Item.MetaKeyword = this.Item.Name;
                    this.Item.Url = this.common.ConvertUrl(this.Item.Name);
                    break;
                case 2:
                    this.Item.MetaDescription = this.Item.Description;
                    break;
                default:
                    break;
            }
        }
    };
    //Popup xác nhận xóa
    LegalDocCategoryComponent.prototype.ShowConfirmDelete = function (Id) {
        var _this = this;
        this.modalDialogService.openDialog(this.viewRef, {
            title: 'Xác nhận',
            childComponent: ngx_modal_dialog_1.SimpleModalComponent,
            data: {
                text: "Bạn có chắc chắn muốn xóa danh mục này và các danh mục con của nó?"
            },
            actionButtons: [
                {
                    text: 'Đồng ý',
                    buttonClass: 'btn btn-success',
                    onAction: function () {
                        _this.DeleteCateNews(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-danger',
                }
            ],
        });
    };
    LegalDocCategoryComponent.prototype.DeleteCateNews = function (Id) {
        var _this = this;
        this.http.delete('/api/Category/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.ResetCurrentRouter();
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
    LegalDocCategoryComponent.prototype.findParent = function (item) {
        if (item == undefined) {
            return "";
        }
        else {
            return item.Name;
        }
    };
    LegalDocCategoryComponent.prototype.upload = function (files, Type) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
            formData.append(file.name, file);
        }
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload/uploadImage/' + Type, formData, {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
        this.http.request(uploadReq).subscribe(function (event) {
            if (event.type === http_1.HttpEventType.UploadProgress) {
                if (Type == 5) {
                    _this.progress = Math.round(100 * event.loaded / event.total);
                }
                else {
                    _this.progressIcon = Math.round(100 * event.loaded / event.total);
                }
            }
            else if (event.type === http_1.HttpEventType.Response) {
                if (Type == 5) {
                    _this.message = event.body["data"].toString();
                    _this.Item.Image = _this.message;
                }
                else {
                    _this.messageIcon = event.body["data"].toString();
                    _this.Item.Icon = _this.messageIcon;
                }
            }
        });
    };
    LegalDocCategoryComponent.prototype.RemoveImage = function (Type) {
        if (Type == 5) {
            this.file.nativeElement.value = "";
            this.Item.Image = undefined;
            this.message = undefined;
            this.progress = undefined;
        }
        else {
            /*this.fileIcon.nativeElement.value = "";*/
            this.Item.Icon = undefined;
            this.messageIcon = undefined;
            this.progressIcon = undefined;
        }
    };
    LegalDocCategoryComponent.prototype.ShowHide = function (id, IsShow) {
        var _this = this;
        var stt = IsShow ? 1 : 10;
        this.http.put('/api/Category/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListCateNews();
                _this.toastSuccess("Thay đổi trạng thái thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                _this.GetListCateNews();
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            _this.GetListCateNews();
        });
    };
    LegalDocCategoryComponent.prototype.SaveSortCategory = function () {
        var _this = this;
        var attribute = document.getElementById("nestable");
        var Arr = [];
        this.common.ConvertHtmlToJson(Arr, attribute, "#nestable", 0, 1);
        this.http.post('/api/Category/SaveCategorySort', Arr, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.ResetCurrentRouter();
                _this.CateNewsModal.hide();
                _this.toastSuccess("Lưu thông tin sắp xếp thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    LegalDocCategoryComponent.prototype.ResetCurrentRouter = function () {
        this.router.routeReuseStrategy.shouldReuseRoute = function () {
            return false;
        };
        this.router.onSameUrlNavigation = 'reload';
        this.router.navigateByUrl(this.router.url);
    };
    LegalDocCategoryComponent.prototype.OpenMediaModal = function () {
        this.OpenMediaFile.show();
    };
    LegalDocCategoryComponent.prototype.CloseMediaModal = function () {
        this.OpenMediaFile.hide();
    };
    LegalDocCategoryComponent.prototype.tabHandleMedia = function () {
        this.isActiveMedia = true;
        this.isActiveUpload = false;
    };
    LegalDocCategoryComponent.prototype.tabHandleMediaUpload = function () {
        this.isActiveMedia = false;
        this.isActiveUpload = true;
    };
    LegalDocCategoryComponent.prototype.upload3 = function (files, cs) {
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
    LegalDocCategoryComponent.prototype.RemoveImageAvatar = function () {
        this.Item.Image = undefined;
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
    };
    LegalDocCategoryComponent.prototype.loadMore = function () {
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
    LegalDocCategoryComponent.prototype.GetListFiles = function () {
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
    LegalDocCategoryComponent.prototype.SeclectMedia = function (item) {
        this.Item.Image = item.url + "/" + item.name;
        this.OpenMediaFile.hide();
    };
    __decorate([
        core_1.ViewChild('CateNewsModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], LegalDocCategoryComponent.prototype, "CateNewsModal", void 0);
    __decorate([
        core_1.ViewChild('SortNewsModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], LegalDocCategoryComponent.prototype, "SortNewsModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], LegalDocCategoryComponent.prototype, "file", void 0);
    __decorate([
        core_1.ViewChild('OpenMediaFile'),
        __metadata("design:type", modal_1.ModalDirective)
    ], LegalDocCategoryComponent.prototype, "OpenMediaFile", void 0);
    LegalDocCategoryComponent = __decorate([
        core_1.Component({
            selector: 'app-legal-doc-category',
            templateUrl: './legal-doc-category.component.html',
            styleUrls: ['./legal-doc-category.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_service_1.CommonService,
            call_category_function_service_1.CallCategoryFunctionService,
            core_1.ElementRef,
            router_1.Router])
    ], LegalDocCategoryComponent);
    return LegalDocCategoryComponent;
}());
exports.LegalDocCategoryComponent = LegalDocCategoryComponent;
//# sourceMappingURL=legal-doc-category.component.js.map