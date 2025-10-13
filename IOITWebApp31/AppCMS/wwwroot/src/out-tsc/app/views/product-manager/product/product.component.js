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
exports.ProductComponent = exports.MY_CUSTOM_FORMATS = void 0;
var core_1 = require("@angular/core");
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var const_1 = require("../../../data/const");
var model_1 = require("../../../data/model");
var ngx_toastr_1 = require("ngx-toastr");
var common_service_1 = require("../../../service/common.service");
var common_1 = require("@angular/common");
var dt_1 = require("../../../data/dt");
var ng_pick_datetime_1 = require("ng-pick-datetime");
var ng_pick_datetime_moment_1 = require("ng-pick-datetime-moment");
var tabs_1 = require("ngx-bootstrap/tabs");
exports.MY_CUSTOM_FORMATS = {
    parseInput: 'DD/MM/YYYY HH:mm',
    fullPickerInput: 'DD/MM/YYYY HH:mm',
    datePickerInput: 'DD/MM/YYYY',
    timePickerInput: ' HH:mm',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY'
};
var ProductComponent = /** @class */ (function () {
    function ProductComponent(http, modalDialogService, viewRef, toastr, common, datePipe) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.common = common;
        this.datePipe = datePipe;
        // public Tag: string;
        this.listProduct = [];
        this.listProductCat = [];
        this.listProductReview = [];
        this.listTrademark = [];
        this.listLanguage = [];
        this.listAttacment = [];
        this.listAttribuite = [];
        this.listAttributeParentId = [];
        this.listAttributeCustom = [];
        this.listLanguageTemp = [];
        this.PriceMin = 0;
        this.PriceMax = 0;
        this.ItemAttriBui = [];
        this.listPrice = [];
        this.listAttributeMapping = [];
        this.listProductAttribuiteChild = [];
        this.listManufacture = [];
        this.listCateNews = [];
        this.listOrderByProduct = [];
        this.domainImage = const_1.domainImage;
        this.typeProduct = const_1.typeProduct;
        this.domain = const_1.domain;
        this.listSuggestProduct = [];
        this.ProductName = "";
        this.ProductReviewStatus = const_1.ProductReviewStatus;
        this.attribuites = [];
        this.ActionTable = const_1.ActionTable;
        this.PriceCurrencyMaskConfig = {
            align: "left",
            allowNegative: false,
            decimal: ".",
            precision: 0,
            prefix: "",
            suffix: " Vnđ",
            thousands: ","
        };
        this.StockQuantityMaskConfig = {
            align: "left",
            allowNegative: false,
            decimal: ".",
            precision: 0,
            prefix: "",
            suffix: "",
            thousands: ","
        };
        this.Item = new model_1.Product();
        this.paging = new dt_1.Paging();
        this.ItemProductAttribuiteChild = new model_1.ProductAttribuiteChild();
        this.ItemAttributeMapping = new model_1.AttributeMapping();
        this.ItemAt = new model_1.Attactment();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "TypeProduct=1 OR TypeProduct=2";
        this.paging.order_by = "ProductId Desc";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.pagingReview = new dt_1.Paging();
        this.pagingReview.page = 1;
        this.pagingReview.page_size = 10;
        this.pagingReview.query = "1=1";
        this.pagingReview.order_by = "";
        this.pagingReview.item_count = 0;
        this.qReview = new dt_1.QueryFilter();
        this.qReview.txtSearch = "";
        this.languageId = localStorage.getItem("languageId") != undefined ? parseInt(localStorage.getItem("languageId")) : 1;
        //this.paging.query = "LanguageId=" + this.languageId;
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
        this.ItemAttribuite = new model_1.Attribute();
    }
    ProductComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListProduct();
        this.GetListCatPro();
        this.GetListLanguage();
        this.GetListManufacture();
        this.GetListTrademark();
        this.GetListAttribuite();
        this.GetAttribuites();
    };
    // get category all
    ProductComponent.prototype.GetListCatPro = function () {
        var _this = this;
        this.http.get('/api/Category/GetAllCatProduct?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listProductCat = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Get danh sách sản phẩm
    ProductComponent.prototype.GetListProduct = function () {
        var _this = this;
        this.http.get('/api/product/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listProduct = res["data"];
                _this.listProduct.forEach(function (item) {
                    item.IsShow = item.Status == 1 ? true : false;
                });
                _this.paging.item_count = res["metadata"].Sum;
                _this.total = res["metadata"];
                for (var i = 0; i < _this.listProduct.length; i++) {
                    if (_this.listProduct[i].TypeProduct == 2) {
                        _this.listPrice = [];
                        _this.PriceMin = 0;
                        _this.PriceMax = 0;
                        for (var j = 0; j < _this.listProduct[i].listProductAttribute.length; j++) {
                            _this.listPrice.push(_this.listProduct[i].listProductAttribute[j].Price);
                            console.log(_this.listPrice);
                            if (j == (_this.listProduct[i].listProductAttribute.length - 1)) {
                                var maxInNumbers = Math.max.apply(Math, _this.listPrice);
                                var minInNumbers = Math.min.apply(Math, _this.listPrice);
                                for (var a = 0; a < _this.listProduct[i].listProductAttribute.length; a++) {
                                    _this.listProduct[i].listProductAttribute[a].PriceMin = minInNumbers;
                                    _this.listProduct[i].listProductAttribute[a].PriceMax = maxInNumbers;
                                }
                            }
                        }
                        console.log('----------------');
                    }
                }
                //console.log(this.listProduct);
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //GET thuoc tinh
    ProductComponent.prototype.GetListAttribuite = function () {
        var _this = this;
        this.http.get('/api/attribute/GetByPage?page=' + this.paging.page + '&query=AttributeParentId=0&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listAttribuite = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    // chon bien the
    ProductComponent.prototype.SaveAttriBui = function () {
        console.log(this.listProductAttribuiteChild);
    };
    ProductComponent.prototype.CreateAttribui = function (list) {
        this.listAttributeMapping = [];
        this.ItemProductAttribuiteChild = new model_1.ProductAttribuiteChild();
        this.ItemProductAttribuiteChild.Status = 1;
        this.ItemProductAttribuiteChild.ProductId = this.Item.ProductId;
        for (var i = 0; i < list.length; i++) {
            for (var j = 0; j < this.listAttribuite.length; j++) {
                if (list[i] == this.listAttribuite[j].AttributeId) {
                    this.ItemAttributeMapping = new model_1.AttributeMapping();
                    if (this.listProductAttribuiteChild.length <= 0) {
                        this.listAttributeCustom.push(this.listAttribuite[j]);
                    }
                    this.ItemAttributeMapping.AttributeId = list[i];
                    this.ItemAttributeMapping["listAttributeChild"] = this.listAttribuite[j]["listAttributeChild"];
                    this.listAttributeMapping.push(this.ItemAttributeMapping);
                    break;
                }
            }
        }
        this.ItemProductAttribuiteChild["listAttribute"] = this.listAttributeMapping;
        this.listProductAttribuiteChild.push(this.ItemProductAttribuiteChild);
        console.log(this.listProductAttribuiteChild);
    };
    // Xoa bien thẻ
    ProductComponent.prototype.DeteleAtrri = function (id) {
        var stt = this.listProductAttribuiteChild.findIndex((function (obj) { return obj.ProductAttributeId == id; }));
        this.listProductAttribuiteChild[stt].Status = 99;
        console.log(this.listProductAttribuiteChild);
    };
    //Lấy toàn bộ danh sách sản phẩm
    ProductComponent.prototype.GetListAllProduct = function () {
        var _this = this;
        var query = "LanguageId=" + this.Item.LanguageId;
        ;
        if (this.Item.ProductId != undefined) {
            query += " and TypeProduct=1 or TypeProduct=2  and ProductId !=" + this.Item.ProductId;
        }
        else {
            query += " and TypeProduct=1 or TypeProduct=2";
        }
        this.http.get('/api/product/GetByPage?page=1&query=' + query + '&order_by=&select=ProductId,PriceSpecial,Name,Image', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listSuggestProduct = res["data"];
                _this.listSuggestProduct.forEach(function (item) {
                    item.Check = false;
                });
                if (_this.Item.ProductId != undefined) {
                    for (var i = 0; i < _this.listSuggestProduct.length; i++) {
                        for (var j = 0; j < _this.Item.listRelated.length; j++) {
                            if (_this.listSuggestProduct[i].ProductId == _this.Item.listRelated[j].TargetRelatedId) {
                                _this.listSuggestProduct[i].Check = true;
                                break;
                            }
                        }
                    }
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    // Get danh sách ngôn ngữ
    ProductComponent.prototype.GetListLanguage = function () {
        var _this = this;
        this.http.get('/api/Language/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listLanguage = res["data"];
                if (_this.listLanguage == undefined || _this.listLanguage == null)
                    _this.listLanguageTemp = [];
                else
                    _this.listLanguageTemp = _this.listLanguage;
                //if (this.listLanguage.length == 1 && (this.Item.NewsId == undefined || (this.Item.NewsId != undefined && this.Item.LanguageId == undefined))) {
                //    this.Item.LanguageId = this.listLanguage[0].LanguageId;
                //}
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    ProductComponent.prototype.GetTranslate = function (id) {
        var _this = this;
        var sl = this.Item.LanguageRootCode;
        var tl = this.Item.LanguageCode;
        this.ItemTranslate = new model_1.Product();
        this.http.get('/api/translate/' + id + '/' + sl + '/' + tl + '/3', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.ItemTranslate = res["data"];
                _this.Item.ProductId = undefined;
                _this.Item.Name = _this.ItemTranslate.Name;
                _this.Item.Url = _this.ItemTranslate.Url;
                _this.Item.Description = _this.ItemTranslate.Description;
                _this.Item.Contents = _this.ItemTranslate.Contents;
                _this.Item.Feature = _this.ItemTranslate.Feature;
                _this.Item.Configuration = _this.ItemTranslate.Configuration;
                _this.Item.NoteTech = _this.ItemTranslate.NoteTech;
                _this.Item.NotePromotion = _this.ItemTranslate.NotePromotion;
                _this.Item.MetaTitle = _this.ItemTranslate.MetaTitle;
                _this.Item.MetaDescription = _this.ItemTranslate.MetaDescription;
                _this.Item.MetaKeyword = _this.ItemTranslate.MetaKeyword;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Danh sách nhà sản xuất
    ProductComponent.prototype.GetListManufacture = function () {
        var _this = this;
        this.http.get('/api/manufacturer/GetByPage?page=1&query=TypeOriginId=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listManufacture = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Danh sách thương hiệu
    ProductComponent.prototype.GetListTrademark = function () {
        var _this = this;
        this.http.get('/api/manufacturer/GetByPage?page=1&query=TypeOriginId=2&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listTrademark = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    ProductComponent.prototype.selectLanguage = function () {
        this.GetListCateNews();
    };
    //Chuyển trang
    ProductComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListProduct();
    };
    //Toast cảnh báo
    ProductComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    ProductComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    ProductComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    ProductComponent.prototype.QueryChanged = function () {
        var query = '(TypeProduct=1 OR TypeProduct=2)';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and Name.Contains("' + this.q.txtSearch + '")';
            }
            else {
                query += 'Name.Contains("' + this.q.txtSearch + '")';
            }
        }
        if (this.q["CategoryId"] != undefined) {
            if (query != '') {
                query += ' and CategoryId=' + this.q["CategoryId"];
            }
            else {
                query += 'CategoryId=' + this.q["CategoryId"];
            }
        }
        if (this.q["TrademarkId"] != undefined) {
            if (query != '') {
                query += ' and TrademarkId=' + this.q["TrademarkId"];
            }
            else {
                query += 'TrademarkId=' + this.q["TrademarkId"];
            }
        }
        if (this.q.LanguageId != undefined) {
            if (query != '') {
                query += ' and LanguageId=' + this.q.LanguageId;
            }
            else {
                query += 'LanguageId=' + this.q.LanguageId;
            }
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListProduct();
    };
    //Mở modal thêm mới
    ProductComponent.prototype.OpenProductModal = function (item, type) {
        this.listAttributeParentId = [];
        this.listAttributeCustom = [];
        this.listProductAttribuiteChild = [];
        this.listAttacment = [];
        this.tabset.tabs[0].active = true;
        this.Item = new model_1.Product();
        this.Item.Contents = "";
        this.Item.Feature = "";
        this.Item.Configuration = "";
        this.Item.NoteTech = "";
        this.Item.NotePromotion = "";
        this.Item.Description = "";
        this.listLanguageTemp = this.listLanguage;
        this.Item.LanguageId = this.languageId;
        this.Item.ViewNumber = 1;
        this.file.nativeElement.value = "";
        this.progress = undefined;
        this.listCateNews = [];
        this.Item.TypeProduct = 1;
        this.Item.listImage = [];
        if (item != undefined) {
            this.listAttacment = item.listDocument;
            this.Item = JSON.parse(JSON.stringify(item));
            if (type == 1 || type == 3) {
                //console.log(this.Item);
                if (item.listProductAttribute.length > 0) {
                    this.Item.TypeProduct = 2;
                    for (var i_1 = 0; i_1 < item.listProductAttribute[0].listAttribute.length; i_1++) {
                        this.listAttributeParentId.push(item.listProductAttribute[0].listAttribute[i_1].AttributeId);
                    }
                    for (var i_2 = 0; i_2 < this.listAttributeParentId.length; i_2++) {
                        for (var j_1 = 0; j_1 < this.listAttribuite.length; j_1++) {
                            if (this.listAttributeParentId[i_2] == this.listAttribuite[j_1].AttributeId) {
                                this.listAttributeCustom.push(this.listAttribuite[j_1]);
                                break;
                            }
                        }
                    }
                    //console.log(this.listAttributeParentId);
                    //console.log(this.listAttributeCustom);
                    //console.log(this.listProductAttribuiteChild);
                }
                this.listProductAttribuiteChild = item.listProductAttribute;
            }
            else if (type == 2) {
                if (this.listLanguage.length == item.listLanguage.length + 1) {
                    this.toastWarning("Bạn đã thêm đủ ngôn ngữ!");
                    return;
                }
                this.listLanguageTemp = [];
                this.Item.ProductId = undefined;
                this.Item.ProductRootId = item.ProductId;
                this.Item.LanguageRootId = item.LanguageId;
                this.Item.LanguageRootCode = this.Item["language"]["Code"];
                this.Item.LanguageId = undefined;
                this.Item.LanguageCode = undefined;
                //check ngôn ngữ
                for (var i = 0; i < this.listLanguage.length; i++) {
                    var check = false;
                    if (this.listLanguage[i].LanguageId == this.languageId) {
                        check = true;
                    }
                    if (item.listLanguage.length > 0) {
                        for (var j = 0; j < item.listLanguage.length; j++) {
                            if (this.listLanguage[i].LanguageId == item.listLanguage[j].LanguageId2) {
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
                this.GetTranslate(this.Item.ProductRootId);
                //this.Item["LangName"] = item.language != undefined ? item.language.Name : "";
                //this.Item["LangFlag"] = item.language != undefined ? item.language.Flag : "";
                //this.Item["LangTitle"] = item.Name;
            }
            else {
                this.Item.listAttribute = JSON.parse(JSON.stringify(this.attribuites));
            }
        }
        this.GetListAllProduct();
        this.GetListCateNews();
        this.ProductModal.show();
    };
    // mo pupop them tai lieu
    ProductComponent.prototype.OpenDocumentModal = function () {
        this.fileDoc.nativeElement.value = "";
        this.ItemAt = new model_1.Attactment();
        this.DocumentModal.show();
    };
    // xoa file khoi list
    ProductComponent.prototype.DeteleAtt = function (i) {
        this.listAttacment[i].Status = 99;
    };
    // luu file danh sach file document
    ProductComponent.prototype.SaveFileDocument = function () {
        if (this.ItemAt.Name == '' || this.ItemAt.Name == undefined) {
            this.toastWarning('Chưa nhập tên file đính kèm !');
            return;
        }
        else if (this.ItemAt.Url == '' || this.ItemAt.Url == undefined) {
            this.toastWarning('Chưa chọn file đính kèm !');
            return;
        }
        this.listAttacment.push(this.ItemAt);
        this.DocumentModal.hide();
    };
    //Thêm mới danh mục trang
    ProductComponent.prototype.SaveProduct = function () {
        var _this = this;
        if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập Tên sản phẩm!");
            return;
        }
        else if (this.Item.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên sản phẩm!");
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
        else if (this.Item.StockQuantity == undefined) {
            this.toastWarning("Chưa nhập Số lượng sản phẩm!");
            return;
        }
        this.Item.listDocument = this.listAttacment;
        //for (let i = 0; i < this.listAttacment.length; i++) {
        //  if (this.listAttacment[i].Status != 99) {
        //    this.Item.listDocument.push(this.listAttacment[i]);
        //  }
        //}
        this.Item["listProductAttribute"] = this.listProductAttribuiteChild;
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));
        if (typeof this.Item.DateStartActive === 'object' && this.Item.DateStartActive != undefined) {
            var DateStartActive = this.Item.DateStartActive.add(7, 'hours');
            this.Item.DateStartActive = DateStartActive.toISOString();
        }
        this.Item.listRelated = [];
        this.listSuggestProduct.forEach(function (item) {
            if (item.Check == true) {
                var obj = { TargetRelatedId: item.ProductId };
                _this.Item.listRelated.push(obj);
            }
        });
        if (this.Item.ProductId == undefined) {
            this.Item.listCategory = [];
            this.listCateNews.forEach(function (item) {
                if (item.Check) {
                    _this.Item.listCategory.push(item);
                }
            });
            this.http.post('/api/Product', this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListProduct();
                    _this.ProductModal.hide();
                    _this.toastSuccess("Thêm mới thành công!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            var arr_1 = [];
            this.Item.listCategory.forEach(function (item) {
                var flag = false;
                for (var i = 0; i < _this.listCateNews.length; i++) {
                    if (item.CategoryId == _this.listCateNews[i].CategoryId && _this.listCateNews[i].Check == true) {
                        flag = true;
                        break;
                    }
                }
                if (!flag) {
                    item.Check = false;
                    arr_1.push(item);
                }
            });
            this.Item.listCategory = arr_1.concat(this.listCateNews.filter(function (e) { return e.Check == true; }));
            this.http.put('/api/product/' + this.Item.ProductId, this.Item, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListProduct();
                    _this.ProductModal.hide();
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
    //Popup xác nhận xóa
    ProductComponent.prototype.ShowConfirmDelete = function (Id) {
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
                    buttonClass: 'btn btn-default',
                }
            ],
        });
    };
    ProductComponent.prototype.Delete = function (Id) {
        var _this = this;
        this.http.delete('/api/Product/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListProduct();
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
    // check chữ
    ProductComponent.prototype.ChangeNameProduct = function (key) {
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
    };
    ProductComponent.prototype.GetListCateNews = function () {
        var _this = this;
        var arr = "arr=11&langId=" + this.Item.LanguageId;
        this.http.get('/api/category/GetByTree?' + arr, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCateNews = res["data"];
                if (_this.Item.ProductId != undefined) {
                    for (var i = 0; i < _this.listCateNews.length; i++) {
                        for (var j = 0; j < _this.Item.listCategory.length; j++) {
                            if (_this.listCateNews[i].CategoryId == _this.Item.listCategory[j].CategoryId) {
                                _this.listCateNews[i].Check = true;
                                break;
                            }
                        }
                    }
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
        // this.http.get('/api/category/GetByPage?page=1&query=TypeCategoryId=11', this.httpOptions).subscribe(
        // 	(res) => {
        // 		if (res["meta"]["error_code"] == 200) {
        // 			this.listCateNews = [];
        // 			if (res["data"].length > 0) {
        // 				res["data"].forEach(cate => {
        // 					this.listCateNews.push({ CategoryId: cate.CategoryId, Name: cate.Name, Check: false });
        // 				});
        // 				if (this.Item.ProductId != undefined) {
        // 					for (var i = 0; i < this.listCateNews.length; i++) {
        // 						for (var j = 0; j < this.Item.listCategory.length; j++) {
        // 							if (this.listCateNews[i].CategoryId == this.Item.listCategory[j].CategoryId) {
        // 								this.listCateNews[i].Check = true;
        // 								break;
        // 							}
        // 						}
        // 					}
        // 				}
        // 			}
        // 		}
        // 	},
        // 	(err) => {
        // 		console.log("Error: connect to API");
        // 	}
        // );
    };
    ProductComponent.prototype.upload = function (files) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
            formData.append(file.name, file);
        }
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload/uploadImage/2', formData, {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
        this.http.request(uploadReq).subscribe(function (event) {
            if (event.type === http_1.HttpEventType.UploadProgress)
                _this.progress = Math.round(100 * event.loaded / event.total);
            else if (event.type === http_1.HttpEventType.Response) {
                event.body["data"].forEach(function (item) {
                    _this.ImageProduct = new model_1.ImageProduct();
                    _this.ImageProduct.Image = item;
                    _this.ImageProduct.IsImageMain = false;
                    _this.ImageProduct.Status = 1;
                    _this.Item.listImage.push(_this.ImageProduct);
                });
            }
        });
    };
    ProductComponent.prototype.RemoveImage = function (idx) {
        if (this.Item.listImage[idx].ProductImageId == undefined) {
            this.Item.listImage.splice(idx, 1);
        }
        else {
            this.Item.listImage[idx].Status = 99;
        }
    };
    ProductComponent.prototype.findTrademark = function (item) {
        if (item == undefined) {
            return "";
        }
        else {
            return item.Name;
        }
    };
    ProductComponent.prototype.ShowHide = function (id, i) {
        var _this = this;
        var stt = this.listProduct[i].IsShow ? 1 : 10;
        this.http.put('/api/Product/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.toastSuccess("Thay đổi trạng thái thành công!");
                _this.GetListProduct();
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                _this.GetListProduct();
                _this.listProduct[i].IsShow = !_this.listProduct[i].IsShow;
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            _this.GetListProduct();
            _this.listProduct[i].IsShow = !_this.listProduct[i].IsShow;
        });
    };
    ProductComponent.prototype.SortTable = function (str) {
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
        this.GetListProduct();
    };
    ProductComponent.prototype.GetClassSortTable = function (str) {
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
    ProductComponent.prototype.SetIsMain = function (idx) {
        for (var i = 0; i < this.Item.listImage.length; i++) {
            this.Item.listImage[i].IsImageMain = false;
            if (idx == i) {
                this.Item.listImage[i].IsImageMain = true;
            }
        }
    };
    ProductComponent.prototype.GetListOrderBy = function () {
        var _this = this;
        this.http.get('/api/orderby/GetOrderBy/10', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listOrderByProduct = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    ProductComponent.prototype.OpenOrderByModal = function () {
        this.listOrderByProduct = [];
        this.GetListOrderBy();
        this.OrderByModal.show();
    };
    ProductComponent.prototype.DeleteOrderBy = function (item) {
        for (var i = 0; i < this.listOrderByProduct.length; i++) {
            if (this.listOrderByProduct[i].CategoryMappingId == item.CategoryMappingId) {
                this.listOrderByProduct[i].Status = 99;
                break;
            }
        }
    };
    ProductComponent.prototype.SaveOrderBy = function () {
        var _this = this;
        this.http.post('/api/orderby', this.listOrderByProduct, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListProduct();
                _this.OrderByModal.hide();
                _this.toastSuccess("Lưu thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    //Product Review
    ProductComponent.prototype.GetListProductReviews = function () {
        var _this = this;
        this.http.get('/api/product/ProductReview/GetByPage?page=' + this.pagingReview.page + '&page_size=' + this.pagingReview.page_size + '&query=' + this.pagingReview.query + '&order_by=' + this.pagingReview.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listProductReview = res["data"];
                _this.pagingReview.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    ProductComponent.prototype.ProductReviewsModal = function (ProductId, Name) {
        this.ProductName = Name;
        this.ProductId = ProductId;
        this.pagingReview = new dt_1.Paging();
        this.pagingReview.page = 1;
        this.pagingReview.page_size = 10;
        this.pagingReview.query = "ProductId=" + ProductId;
        this.pagingReview.order_by = "";
        this.pagingReview.item_count = 0;
        this.qReview = new dt_1.QueryFilter();
        this.qReview.txtSearch = "";
        this.qReview.Type = undefined;
        this.GetListProductReviews();
        this.ProductReviewModal.show();
    };
    ProductComponent.prototype.PageChangedReview = function (event) {
        this.pagingReview.page = event.page;
        this.GetListProductReviews();
    };
    ProductComponent.prototype.QueryReviewChanged = function () {
        var query = 'ProductId=' + this.ProductId;
        if (this.qReview["Type"] != undefined) {
            if (query != '') {
                query += ' and Status=' + this.qReview["Type"];
            }
            else {
                query += 'Status=' + this.qReview["Type"];
            }
        }
        if (query == '')
            this.pagingReview.query = '1=1';
        else
            this.pagingReview.query = query;
        this.GetListProductReviews();
    };
    ProductComponent.prototype.ChangeStatusProductReview = function (ProductReviewId, Status) {
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
    // tai tep dinh kem
    ProductComponent.prototype.uploadDoc = function (files) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_2 = files; _i < files_2.length; _i++) {
            var file = files_2[_i];
            formData.append(file.name, file);
        }
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload', formData, {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
        this.http.request(uploadReq).subscribe(function (event) {
            if (event.type === http_1.HttpEventType.UploadProgress)
                _this.progress = Math.round(100 * event.loaded / event.total);
            else if (event.type === http_1.HttpEventType.Response) {
                _this.ItemAt.Url = event.body["data"];
            }
        });
    };
    ProductComponent.prototype.CheckCategory = function (CategoryId, curItem) {
        var Check = curItem["Check"];
        var CategoryParentId = curItem["CategoryParentId"];
        var CheckParent = false;
        this.listCateNews.forEach(function (item) {
            if (Check) {
                if (item.Genealogy.indexOf(CategoryId.toString()) != -1) {
                    item.Check = !Check;
                }
            }
            if (Check == false) {
                CheckParent = true;
            }
            else {
                if (item.CategoryParentId == CategoryParentId) {
                    if (item.Check == true) {
                        CheckParent = true;
                    }
                }
            }
        });
        if (CheckParent) {
            this.listCateNews.forEach(function (item) {
                if (item.CategoryId == CategoryParentId) {
                    item.Check = true;
                }
            });
        }
    };
    //Lấy ra danh sách thuộc tính
    ProductComponent.prototype.GetAttribuites = function () {
        var _this = this;
        this.http.get('/api/attribuite/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.attribuites = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    ProductComponent.prototype.OpenAttribuiteModal = function () {
        this.ItemAttribuite = new model_1.Attribute();
        this.ItemAttribuite.Status = 1;
        this.AttribuiteModal.show();
    };
    ProductComponent.prototype.SaveAttribuite = function () {
        if (this.ItemAttribuite.AttribuiteId == undefined) {
            this.toastWarning("Chưa chọn Thuộc tính!");
            return;
        }
        else if (this.ItemAttribuite.Value == undefined || this.ItemAttribuite.Value == '') {
            this.toastWarning("Chưa nhập Giá trị thuộc tính!");
            return;
        }
        else if (this.ItemAttribuite.Value.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập Giá trị thuộc tính!");
            return;
        }
        else if (this.ItemAttribuite.Location == undefined) {
            this.toastWarning("Chưa nhập Thứ tự hiển thị!");
            return;
        }
        if (this.Item.listAttribute == undefined) {
            this.Item.listAttribute = [];
        }
        this.Item.listAttribute.push(this.ItemAttribuite);
        this.ItemAttribuite = new model_1.Attribute();
        this.AttribuiteModal.hide();
    };
    ProductComponent.prototype.ShowConfirmDeleteAttribuite = function (i) {
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
                        _this.Item.listAttribute[i].Status = 99;
                        _this.viewRef.clear();
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-default',
                }
            ],
        });
    };
    ProductComponent.prototype.CheckActionTable = function (ProductId) {
        if (ProductId == undefined) {
            var CheckAll_1 = this.CheckAll;
            this.listProduct.forEach(function (item) {
                item.Action = CheckAll_1;
            });
        }
        else {
            var CheckAll = true;
            for (var i = 0; i < this.listProduct.length; i++) {
                if (!this.listProduct[i].Action) {
                    CheckAll = false;
                    break;
                }
            }
            this.CheckAll = CheckAll == true ? true : false;
        }
    };
    ProductComponent.prototype.ActionTableFunc = function () {
        var _this = this;
        switch (this.ActionId) {
            case 1:
                var data_1 = [];
                this.listProduct.forEach(function (item) {
                    if (item.Action == true) {
                        data_1.push(item.ProductId);
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
                                    _this.http.put('/api/Product/deletes', data_1, _this.httpOptions).subscribe(function (res) {
                                        if (res["meta"]["error_code"] == 200) {
                                            _this.toastSuccess("Xóa thành công!");
                                            _this.GetListProduct();
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
        core_1.ViewChild('ProductModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], ProductComponent.prototype, "ProductModal", void 0);
    __decorate([
        core_1.ViewChild('OrderByModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], ProductComponent.prototype, "OrderByModal", void 0);
    __decorate([
        core_1.ViewChild('ProductReviewModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], ProductComponent.prototype, "ProductReviewModal", void 0);
    __decorate([
        core_1.ViewChild('AttribuiteModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], ProductComponent.prototype, "AttribuiteModal", void 0);
    __decorate([
        core_1.ViewChild('DocumentModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], ProductComponent.prototype, "DocumentModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], ProductComponent.prototype, "file", void 0);
    __decorate([
        core_1.ViewChild('fileDoc'),
        __metadata("design:type", core_1.ElementRef)
    ], ProductComponent.prototype, "fileDoc", void 0);
    __decorate([
        core_1.ViewChild('tabset'),
        __metadata("design:type", tabs_1.TabsetComponent)
    ], ProductComponent.prototype, "tabset", void 0);
    ProductComponent = __decorate([
        core_1.Component({
            selector: 'app-product',
            templateUrl: './product.component.html',
            styleUrls: ['./product.component.scss'],
            providers: [
                { provide: ng_pick_datetime_1.DateTimeAdapter, useClass: ng_pick_datetime_moment_1.MomentDateTimeAdapter, deps: [ng_pick_datetime_1.OWL_DATE_TIME_LOCALE] },
                { provide: ng_pick_datetime_1.OWL_DATE_TIME_FORMATS, useValue: exports.MY_CUSTOM_FORMATS }
            ]
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_service_1.CommonService,
            common_1.DatePipe])
    ], ProductComponent);
    return ProductComponent;
}());
exports.ProductComponent = ProductComponent;
//# sourceMappingURL=product.component.js.map