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
exports.CateProductComponent = void 0;
var core_1 = require("@angular/core");
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var const_1 = require("../../../data/const");
var model_1 = require("../../../data/model");
var dt_1 = require("../../../data/dt");
var ngx_toastr_1 = require("ngx-toastr");
var common_service_1 = require("../../../service/common.service");
var call_category_function_service_1 = require("../../../service/call-category-function.service");
var router_1 = require("@angular/router");
var CateProductComponent = /** @class */ (function () {
    function CateProductComponent(http, modalDialogService, viewRef, toastr, common, callCategoryFunctionService, elm, router) {
        var _this = this;
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.common = common;
        this.callCategoryFunctionService = callCategoryFunctionService;
        this.elm = elm;
        this.router = router;
        this.listCateProduct = [];
        this.listCateParent = [];
        this.listLanguage = [];
        this.listLanguageTemp = [];
        this.listOrderByCatProduct = [];
        this.typeCategoryPage = const_1.typeCategoryPage;
        this.domainImage = const_1.domainImage;
        this.query = "arr=11";
        this.key = 'categorySorts';
        this.listFullCate = [];
        this.Item = new model_1.Category();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "TypeCategoryId=11";
        this.paging.order_by = "CategoryId Desc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.languageId = localStorage.getItem("languageId") != undefined ? parseInt(localStorage.getItem("languageId")) : 1;
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
        this.subscription = this.callCategoryFunctionService.getAction().subscribe(function (action) {
            if (action.TypeAction == 1) {
                _this.OpenCateProductModal(undefined, action.CategoryId, 2);
            }
            else if (action.TypeAction == 2) {
                _this.OpenCateProductModal(action.CategoryId, undefined, 3);
            }
            else if (action.TypeAction == 3) {
                _this.ShowConfirmDelete(action.CategoryId);
            }
            else if (action.TypeAction == 5) {
                _this.OpenCateProductModal(action.CategoryId, undefined, 5); // thêm danh mục vs ngôn ngữ mới
            }
            else if (action.TypeAction == 6) {
                _this.OpenCateProductModal(action.CategoryId, undefined, 6); // sửa danh mục vs ngôn ngữ mới
            }
            else if (action.TypeAction == 7) {
                _this.ShowHide(action.CategoryId, action.IsShow); // Đổi trạng thái danh mục sản phẩ
            }
        });
    }
    CateProductComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListCateProduct();
        this.GetListLanguage();
        this.GetListFullCate();
    };
    CateProductComponent.prototype.ngOnDestroy = function () {
        this.subscription.unsubscribe();
        this.router.onSameUrlNavigation = 'ignore';
    };
    //Get danh sách danh mục sản phẩm
    CateProductComponent.prototype.GetListCateProduct = function () {
        var _this = this;
        this.listCateProduct = [];
        this.http.get('/api/category/GetCategorySort?' + this.query, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCateProduct = res["data"];
                _this.total_item = res["metadata"];
                loadNestable();
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Get danh sách danh mục cha
    CateProductComponent.prototype.GetListCateParent = function (Id) {
        var _this = this;
        this.http.get('/api/category/GetByTree?arr=11&langId=' + this.Item.LanguageId, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCateParent = res["data"];
                _this.listCateParent.forEach(function (item) {
                    if (item.CategoryId == Id) {
                        item.disabled = true;
                    }
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
    // Get danh sách ngôn ngữ
    CateProductComponent.prototype.GetListLanguage = function () {
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
    CateProductComponent.prototype.GetTranslate = function (id) {
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
    CateProductComponent.prototype.GetListOrderByCat = function () {
        var _this = this;
        this.http.get('api/category/listproduct/' + this.Item.CategoryId, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listOrderByCatProduct = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    CateProductComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListCateProduct();
    };
    //Cảnh báo
    CateProductComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Hoàn thành
    CateProductComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Lỗi
    CateProductComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    CateProductComponent.prototype.QueryChanged = function () {
        var query = 'arr=11';
        //if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
        //    if (query != '') {
        //        query += ' and Name.Contains("' + this.q.txtSearch + '")';
        //    }
        //    else {
        //        query += 'Name.Contains("' + this.q.txtSearch + '")';
        //    }
        //}
        //if (query == '')
        //    this.paging.query = '1=1';
        //else
        //    this.paging.query = query;
        if (this.q.LanguageId != undefined) {
            query = query + "&langId=" + this.q.LanguageId;
        }
        this.query = query;
        this.GetListCateProduct();
    };
    CateProductComponent.prototype.selectLanguage = function () {
        this.GetListCateParent(undefined);
    };
    //Mở modal thêm mới
    CateProductComponent.prototype.OpenCateProductModal = function (CategoryId, CategoryParentId, type) {
        //this.GetListLanguage();
        this.Item = new model_1.Category();
        this.Item.Contents = "";
        this.Item.CategoryParentId = CategoryParentId;
        this.Item.TypeCategoryId = 1;
        this.Item.LanguageId = this.languageId;
        this.Item.Location = this.total_item + 1;
        this.file.nativeElement.value = "";
        this.fileIcon.nativeElement.value = "";
        //this.fileSlide.nativeElement.value = "";
        this.message = undefined;
        this.messageIcon = undefined;
        this.progress = undefined;
        this.progressIcon = undefined;
        this.progressSlide = undefined;
        this.Item.listImage = [];
        if (CategoryId != undefined) {
            var Cate = this.listFullCate.filter(function (x) { return x.CategoryId == CategoryId; })[0];
            if (Cate) {
                this.Item = JSON.parse(JSON.stringify(Cate));
                console.log(this.Item);
                if (type == 3 || type == 6) {
                    if (this.Item.CategoryParentId == 0)
                        this.Item.CategoryParentId = undefined;
                    this.GetListCateParent(this.Item.CategoryId);
                    this.CateProductModal.show();
                }
                else if (type == 5) {
                    if (this.listLanguage.length == Cate.listLanguage.length + 1) {
                        this.toastWarning("Bạn đã thêm đủ ngôn ngữ!");
                        return;
                    }
                    this.listLanguageTemp = [];
                    this.Item.CategoryId = undefined;
                    this.Item.CategoryRootId = Cate.CategoryId;
                    this.Item.LanguageRootId = this.Item.LanguageId;
                    this.Item.LanguageRootCode = this.Item["language"]["Code"];
                    this.Item.LanguageId = undefined;
                    this.Item.LanguageCode = undefined;
                    //check ngôn ngữ
                    for (var i = 0; i < this.listLanguage.length; i++) {
                        var check = false;
                        if (this.listLanguage[i].LanguageId == this.languageId) {
                            check = true;
                        }
                        if (Cate.listLanguage.length > 0) {
                            for (var j = 0; j < Cate.listLanguage.length; j++) {
                                if (this.listLanguage[i].LanguageId == Cate.listLanguage[j].LanguageId2) {
                                    check = true;
                                    break;
                                }
                            }
                        }
                        if (!check) {
                            this.listLanguageTemp.push(this.listLanguage[i]);
                        }
                    }
                    if (this.listLanguageTemp.length > 0) {
                        this.Item.LanguageId = this.listLanguageTemp[0].LanguageId;
                        this.Item.LanguageCode = this.listLanguageTemp[0].Code;
                    }
                    //Gọi api dịch ở đây
                    this.GetTranslate(this.Item.CategoryRootId);
                    //
                    this.Item.CategoryParentId = undefined;
                    this.GetListCateParent(this.Item.CategoryId);
                    this.CateProductModal.show();
                }
            }
            else {
                this.toastError("Không tìm thấy danh mục trên hệ thống!");
                return;
            }
        }
        else {
            this.GetListCateParent(undefined);
            this.CateProductModal.show();
        }
    };
    //Thêm mới danh mục trang
    CateProductComponent.prototype.SaveCateProduct = function () {
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
            this.toastWarning("Chưa nhập đường dẫn !");
            return;
        }
        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));
        this.Item.TypeCategoryId = 11;
        if (!this.Item.LanguageId) {
            this.Item.LanguageId = parseInt(localStorage.getItem("languageId"));
        }
        if (this.Item.CategoryId) {
            this.http.put('/api/Category/' + this.Item.CategoryId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListCateProduct();
                    _this.GetListFullCate();
                    _this.CateProductModal.hide();
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
            this.http.post('/api/Category', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListCateProduct();
                    _this.GetListFullCate();
                    _this.CateProductModal.hide();
                    _this.toastSuccess("Thêm mới thành công!");
                }
                else if (res["meta"]["error_code"] == 213) {
                    _this.toastWarning("Tên đã tồn tại!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
    };
    CateProductComponent.prototype.ChangeTitle = function (key) {
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
    CateProductComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        _this.DeleteCateProduct(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-default',
                }
            ],
        });
    };
    CateProductComponent.prototype.DeleteCateProduct = function (Id) {
        var _this = this;
        this.http.delete('/api/Category/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListCateProduct();
                _this.GetListFullCate();
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
    CateProductComponent.prototype.upload = function (files, Type) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
            formData.append(file.name, file);
        }
        console.log(formData);
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
                else if (Type == 3) {
                    _this.progressSlide = Math.round(100 * event.loaded / event.total);
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
                else if (Type == 3) {
                    console.log(_this.Item.listImage);
                    //this.message = event.body["data"].toString();
                    event.body["data"].forEach(function (item) {
                        _this.ImageAttact = new model_1.Attactment();
                        _this.ImageAttact.Url = item;
                        _this.ImageAttact.Thumb = item;
                        _this.ImageAttact.IsImageMain = false;
                        _this.ImageAttact.Status = 1;
                        _this.Item.listImage.push(_this.ImageAttact);
                    });
                }
                else {
                    _this.messageIcon = event.body["data"].toString();
                    _this.Item.Icon = _this.messageIcon;
                }
            }
        });
    };
    CateProductComponent.prototype.findParent = function (item) {
        if (item == undefined) {
            return "";
        }
        else {
            return item.Name;
        }
    };
    CateProductComponent.prototype.RemoveImage = function (Type) {
        if (Type == 5) {
            this.file.nativeElement.value = "";
            this.Item.Image = undefined;
            this.message = undefined;
            this.progress = undefined;
        }
        else {
            this.fileIcon.nativeElement.value = "";
            this.Item.Icon = undefined;
            this.messageIcon = undefined;
            this.progressIcon = undefined;
        }
    };
    CateProductComponent.prototype.RemoveImageSlide = function (idx) {
        if (this.Item.listImage[idx].AttactmentId == undefined) {
            this.Item.listImage.splice(idx, 1);
        }
        else {
            this.Item.listImage[idx].Status = 99;
        }
    };
    CateProductComponent.prototype.SetIsMain = function (idx) {
        for (var i = 0; i < this.Item.listImage.length; i++) {
            this.Item.listImage[i].IsImageMain = false;
            if (idx == i) {
                this.Item.listImage[i].IsImageMain = true;
            }
        }
    };
    CateProductComponent.prototype.ShowHide = function (id, IsShow) {
        var _this = this;
        var stt = IsShow ? 1 : 10;
        this.http.put('/api/Category/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.toastSuccess("Thay đổi trạng thái thành công!");
                _this.GetListCateProduct();
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                _this.GetListCateProduct();
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            _this.GetListCateProduct();
        });
    };
    CateProductComponent.prototype.SortTable = function (str) {
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
        this.GetListCateProduct();
    };
    CateProductComponent.prototype.GetClassSortTable = function (str) {
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
    CateProductComponent.prototype.SaveSortCategory = function () {
        var _this = this;
        var attribute = document.getElementById("nestable");
        var Arr = [];
        this.common.ConvertHtmlToJson(Arr, attribute, "#nestable", 0, 1);
        this.http.post('/api/Category/SaveCategorySort', Arr, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.ResetCurrentRouter();
                _this.CateProductModal.hide();
                _this.toastSuccess("Lưu thông tin sắp xếp thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    CateProductComponent.prototype.ResetCurrentRouter = function () {
        this.router.routeReuseStrategy.shouldReuseRoute = function () {
            return false;
        };
        this.router.onSameUrlNavigation = 'reload';
        this.router.navigateByUrl(this.router.url);
    };
    CateProductComponent.prototype.GetListFullCate = function () {
        var _this = this;
        var query = "TypeCategoryId=11";
        this.http.get('/api/category/GetByPage?page=1&query=' + query + '&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listFullCate = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    __decorate([
        core_1.ViewChild('CateProductModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], CateProductComponent.prototype, "CateProductModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], CateProductComponent.prototype, "file", void 0);
    __decorate([
        core_1.ViewChild('fileIcon'),
        __metadata("design:type", core_1.ElementRef)
    ], CateProductComponent.prototype, "fileIcon", void 0);
    CateProductComponent = __decorate([
        core_1.Component({
            selector: 'app-product',
            templateUrl: './product.component.html',
            styleUrls: ['./product.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_service_1.CommonService,
            call_category_function_service_1.CallCategoryFunctionService,
            core_1.ElementRef,
            router_1.Router])
    ], CateProductComponent);
    return CateProductComponent;
}());
exports.CateProductComponent = CateProductComponent;
//# sourceMappingURL=product.component.js.map