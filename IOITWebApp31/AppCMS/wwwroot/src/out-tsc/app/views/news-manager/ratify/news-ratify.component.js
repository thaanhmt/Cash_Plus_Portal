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
exports.NewsRatifyComponent = exports.MY_CUSTOM_FORMATS = void 0;
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
var NewsRatifyComponent = /** @class */ (function () {
    function NewsRatifyComponent(http, modalDialogService, viewRef, toastr, datePipe, common) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.datePipe = datePipe;
        this.common = common;
        this.listItemMedia = [];
        this.domainMedia = const_1.domainMedia;
        this.listNews = [];
        this.linkNews = [];
        this.linkCatNews = [];
        this.listOrderByProduct = [];
        this.listSuggestProduct = [];
        this.listAuthor = [];
        this.listUsers = [];
        this.listNewsNote = [];
        this.listNewsT = [];
        this.listCateNews = [];
        this.listSuggestNews = [];
        this.listLanguage = [];
        this.listLanguageTemp = [];
        this.isNoitify = false;
        this.isActiveMedia = true;
        this.isActiveUpload = false;
        this.isDelay = false;
        this.listTypeNews = const_1.typeCategoryNews;
        this.domainVideos = const_1.domainVideos;
        this.domainImage = const_1.domainImage;
        this.ActionTable = const_1.ActionTable;
        this.Status = const_1.Status;
        this.tags = [];
        this.RoleCode = localStorage.getItem("roleCode") || '';
        this.active = false;
        this.activeU = false;
        this.activeD = false;
        this.domainDebug = const_1.domainDebug;
        this.Item = new model_1.News();
        this.ItemPr = new model_1.Product();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "NewsId Desc";
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
        this.IsAll = false;
        this.StatusId = 11;
        this.CheckConfirmNews = this.common.CheckAccessKey(localStorage.getItem("access_key"), "DBBTV");
        this.languageId = localStorage.getItem("languageId") != undefined ? parseInt(localStorage.getItem("languageId")) : 1;
        this.languageCode = localStorage.getItem("languageCode") != undefined ? localStorage.getItem("languageCode") : "vi";
        //this.paging.query = "LanguageId=" + this.languageId;
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
        this.httpOptionsBlob = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            observe: 'response',
            responseType: 'blob'
        };
        this.Tag = new model_1.Tag();
        this.CheckRole = new dt_1.CheckRole();
        var code = "BVVB";
        this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 0);
        this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 1);
        this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 2);
        this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 3);
        this.CheckRole.Export = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 5);
    }
    NewsRatifyComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.paging.query = "1=1 AND (Status=10 OR Status= 11 OR Status=12)";
        this.domain = const_1.domain;
        this.GetListAllNews();
        this.GetListCatnew();
        this.GetListUser();
        this.GetListNews();
        this.GetListAuthor();
        this.GetListCateNews();
        this.GetListLanguage();
        this.GetListTag(undefined);
        this.GetListFiles();
        //this.loadMore();
        this.GetDomainStatic();
    };
    NewsRatifyComponent.prototype.GetDomainStatic = function () {
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
    //Lấy toàn bộ danh sách sản phẩm
    NewsRatifyComponent.prototype.GetListAllProduct = function () {
        var _this = this;
        var query = "LanguageId=" + this.Item.LanguageId;
        if (this.ItemPr.ProductId != undefined) {
            query += " and TypeProduct=1 or TypeProduct=2  and ProductId !=" + this.ItemPr.ProductId;
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
                if (_this.ItemPr.ProductId != undefined) {
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
    //Lấy toàn bộ danh sách tin văn bản
    NewsRatifyComponent.prototype.GetListAllNews = function () {
        var _this = this;
        var query = "LanguageId=" + this.Item.LanguageId;
        if (this.Item.NewsId != undefined) {
            query = "TypeNewsId=1 and NewsId!=" + this.Item.NewsId + " and LanguageId=" + this.Item.LanguageId + " and Status=1";
        }
        else {
            query = "TypeNewsId=1 and LanguageId=" + this.Item.LanguageId + " and Status=1";
        }
        this.http.get('/api/news/GetByPage/?page=1&page_size=200&query=' + query + '&order_by=&select=NewsId,Title,Url,Image,TypeNewsId,LanguageId', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listSuggestNews = res["data"];
                _this.listSuggestNews.forEach(function (item) {
                    item.Check = false;
                });
                if (_this.Item.NewsId != undefined) {
                    for (var i = 0; i < _this.listSuggestNews.length; i++) {
                        for (var j = 0; j < _this.Item.listRelated.length; j++) {
                            if (_this.listSuggestNews[i].NewsId == _this.Item.listRelated[j].TargetRelatedId) {
                                _this.listSuggestNews[i].Check = true;
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
    //Danh sách tags
    NewsRatifyComponent.prototype.GetListTag = function (obj) {
        var _this = this;
        this.http.get('/api/tag/GetByPage?page=1&query=TargetType=1&order_by=&select=TagId,Name', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                if (obj != undefined) {
                    var listTag = JSON.parse(JSON.stringify(_this.Item.listTag));
                    listTag.push(obj);
                    _this.Item.listTag = listTag;
                }
                _this.tags = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    // get category all
    NewsRatifyComponent.prototype.GetListCatnew = function () {
        var _this = this;
        this.Item.LanguageId = this.Item.LanguageId ? this.Item.LanguageId : 1;
        this.http.get('/api/Category/GetAllCatNew?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCateNews = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    NewsRatifyComponent.prototype.GetListNews = function () {
        var _this = this;
        var data = Object.assign({}, this.q);
        if (this.dateStart.nativeElement.value) {
            var obj = this.dateStart.nativeElement.value.split("/");
            data.DateStart = obj[2] + "-" + obj[1] + "-" + obj[0] + " 0:0:0";
        }
        if (this.dateEnd.nativeElement.value) {
            var obj = this.dateEnd.nativeElement.value.split("/");
            data.DateEnd = obj[2] + "-" + obj[1] + "-" + obj[0] + " 23:59:59";
        }
        data.page = this.paging.page;
        data.page_size = this.paging.page_size;
        data.query = this.paging.query;
        data.order_by = this.paging.order_by;
        this.http.post('/api/news/GetByPageCash', data, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listNews = res["data"];
                _this.listNews.forEach(function (item) {
                    item.IsShow = false;
                });
                _this.paging.item_count = res["metadata"].Sum;
                _this.total1 = res["metadata"].Temp;
                _this.total2 = res["metadata"].New;
                _this.total3 = res["metadata"].ReNew;
                _this.active = true;
                _this.activeU = false;
                _this.activeD = false;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
        //Get danh bài viết đang chờ duyệt
        //this.http.get('/api/news/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
        //  (res) => {
        //    if (res["meta"]["error_code"] == 200) {
        //      this.listNews = res["data"];
        //      this.listNews.forEach(item => {
        //        item.IsShow = false;
        //      });
        //      this.paging.item_count = res["metadata"].Sum;
        //      this.total1 = res["metadata"].Temp;
        //      this.total2 = res["metadata"].New;
        //      this.total3 = res["metadata"].ReNew;
        //      this.active = true;
        //      this.activeU = false;
        //      this.activeD = false;
        //    }
        //  },
        //  (err) => {
        //    console.log("Error: connect to API");
        //  }
        //);
    };
    NewsRatifyComponent.prototype.GetListNewsNote = function (newsId) {
        var _this = this;
        //Get danh sách góp ý bài viết
        var query = "NewsId=" + newsId;
        this.http.get('/api/newsNote/GetByPage/?page=1&query=' + query + '&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listNewsNote = res["data"];
                //this.paging.item_count = res["metadata"].Sum;
                //this.total = res["metadata"];
                //this.activeU = true;
                //this.activeD = false;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    NewsRatifyComponent.prototype.StatusChanged = function (status) {
        var query = "1=1";
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            query += ' and Title.Contains("' + this.q.txtSearch + '")';
        }
        if (this.q["Type"] != undefined) {
            query += ' and TypeNewsId=' + this.q["Type"];
        }
        if (this.q["CategoryId"] != undefined) {
            query += ' and CategoryId=' + this.q["CategoryId"];
        }
        if (this.q.LanguageId != undefined) {
            query += ' and LanguageId=' + this.q.LanguageId;
        }
        if (this.q.IsHot != undefined) {
            query += ' and IsHot=' + this.q.IsHot;
        }
        if (this.q["UserId"] != undefined) {
            query += ' and UserCreatedId=' + this.q["UserId"];
        }
        if (this.q.AuthorId != undefined) {
            query += ' and AuthorId=' + this.q.AuthorId;
        }
        this.StatusId = status;
        if (status != 20)
            query += ' and Status=' + status;
        else {
            query += ' and UserCreatedId=' + parseInt(localStorage.getItem("userId"));
        }
        this.paging.query = query;
        this.GetListNews();
    };
    NewsRatifyComponent.prototype.getMyPosts = function () {
        var _this = this;
        //Get danh sách tất cả bài viết của user Cộng tác viên và Nhân viên
        this.http.get('/api/news/GetByPageUser/?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listNews = res["data"];
                _this.listNews.forEach(function (item) {
                    item.IsShow = item.Status == 12 ? true : false;
                });
                _this.paging.item_count = res["metadata"].Sum;
                _this.total = res["metadata"];
                _this.active = false;
                _this.activeU = true;
                _this.activeD = false;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    NewsRatifyComponent.prototype.getPostsDraft = function () {
        var _this = this;
        //Get danh sách bài viết rác của nhân viên
        this.http.get('/api/news/GetByPageDraft/?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listNews = res["data"];
                _this.listNews.forEach(function (item) {
                    item.IsShow = item.Status == 12 ? true : false;
                });
                _this.paging.item_count = res["metadata"].Sum;
                _this.total = res["metadata"];
                _this.active = false;
                _this.activeU = false;
                _this.activeD = true;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    NewsRatifyComponent.prototype.GetListCateNews = function () {
        var _this = this;
        this.Item.LanguageId = this.Item.LanguageId ? this.Item.LanguageId : 1;
        this.http.get('/api/category/GetByTree?arr=1&arr=2&arr=3&arr=4&arr=5&langId=' + this.Item.LanguageId, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCateNews = res["data"];
                if (_this.Item.NewsId != undefined) {
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
    };
    NewsRatifyComponent.prototype.GetListOrderBy = function () {
        var _this = this;
        this.http.get('/api/orderby/GetOrderBy/11', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listOrderByProduct = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    // Get danh sách tác giả
    NewsRatifyComponent.prototype.GetListAuthor = function () {
        var _this = this;
        var query = "Type=1";
        this.http.get('/api/author/GetByPage/?page=1&query=' + query + '&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listAuthor = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    NewsRatifyComponent.prototype.GetListUser = function () {
        var _this = this;
        this.http.get('/api/news/GetAuthor', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listUsers = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    // Get danh sách ngôn ngữ
    NewsRatifyComponent.prototype.GetListLanguage = function () {
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
    NewsRatifyComponent.prototype.GetTranslate = function (id) {
        var _this = this;
        var sl = this.Item.LanguageRootCode;
        var tl = this.Item.LanguageCode;
        this.ItemTranslate = new model_1.News();
        this.http.get('/api/translate/' + id + '/' + sl + '/' + tl + '/2', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.ItemTranslate = res["data"];
                _this.Item.NewsId = undefined;
                _this.Item.Title = _this.ItemTranslate.Title;
                _this.Item.Url = _this.ItemTranslate.Url;
                _this.Item.Description = _this.ItemTranslate.Description;
                _this.Item.Contents = _this.ItemTranslate.Contents;
                _this.Item.Introduce = _this.ItemTranslate.Introduce;
                _this.Item.SystemDiagram = _this.ItemTranslate.SystemDiagram;
                _this.Item.MetaTitle = _this.ItemTranslate.MetaTitle;
                _this.Item.MetaDescription = _this.ItemTranslate.MetaDescription;
                _this.Item.MetaKeyword = _this.ItemTranslate.MetaKeyword;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    NewsRatifyComponent.prototype.OpenChooseHighlightsNews = function () {
        this.listOrderByProduct = [];
        this.GetListOrderBy();
        this.HighlightNewsModal.show();
    };
    NewsRatifyComponent.prototype.SaveHighlightNews = function () {
        var _this = this;
        this.http.post('/api/orderby', this.listOrderByProduct, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.HighlightNewsModal.hide();
                _this.toastSuccess("Lưu thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    NewsRatifyComponent.prototype.DeleteOrderBy = function (item) {
        for (var i = 0; i < this.listOrderByProduct.length; i++) {
            if (this.listOrderByProduct[i].CategoryMappingId == item.CategoryMappingId) {
                this.listOrderByProduct[i].Status = 99;
                break;
            }
        }
    };
    NewsRatifyComponent.prototype.SelectTypeNews = function () {
        this.GetListCateNews();
        this.GetListAllNews();
        this.GetListAllProduct();
    };
    NewsRatifyComponent.prototype.SelectLanguage = function () {
        this.GetListCateNews();
        this.GetListAllNews();
        this.GetListAllProduct();
    };
    //Chuyển trang
    NewsRatifyComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        if (this.active) {
            this.GetListNews();
        }
        else if (this.activeU) {
            this.getMyPosts();
        }
        else if (this.activeD) {
            this.getPostsDraft();
        }
    };
    //Thông báo
    NewsRatifyComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    NewsRatifyComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    NewsRatifyComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    NewsRatifyComponent.prototype.QueryChanged = function () {
        var query = "1=1 AND (Status=10 OR Status=11 OR Status=12)";
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            query += ' and Title.Contains("' + this.q.txtSearch + '")';
        }
        if (this.q["Type"] != undefined) {
            query += ' and TypeNewsId=' + this.q["Type"];
        }
        if (this.q["CategoryId"] != undefined) {
            //if (query != '') {
            query += ' and CategoryId=' + this.q["CategoryId"];
            //}
            //else {
            //  query += 'CategoryId=' + this.q["CategoryId"];
            //}
        }
        if (this.q.LanguageId != undefined) {
            //if (query != '') {
            query += ' and LanguageId=' + this.q.LanguageId;
            //}
            //else {
            //  query += 'LanguageId=' + this.q.LanguageId;
            //}
        }
        if (this.q["UserId"] != undefined) {
            query += ' and UserCreatedId=' + this.q["UserId"];
        }
        if (this.q.AuthorId != undefined) {
            query += ' and AuthorId=' + this.q.AuthorId;
        }
        //if (this.q["StatusId"] != undefined) {
        //  if (query != '') {
        //    query += ' and Status=' + this.q["StatusId"];
        //  }
        //  else {
        //    query += 'Status=' + this.q["StatusId"];
        //  }
        //}
        //if (query == '')
        //  this.paging.query = '1=1';
        //else
        this.paging.query = query;
        /*if (this.active) {*/
        this.GetListNews();
        //} else if (this.activeU) {
        //  this.getMyPosts();
        //} else if (this.activeD) {
        //  this.getPostsDraft();
        //}
    };
    NewsRatifyComponent.prototype.CheckCategory = function (CategoryId, curItem) {
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
    // chon tin lien quan
    NewsRatifyComponent.prototype.SelectListNew = function (id) {
        console.log(this.listSuggestNews);
        console.log(id);
    };
    NewsRatifyComponent.prototype.onAttachChanged = function (value) {
        this.checkAttach = value;
        if (this.checkAttach == true) {
            this.GetListAllNews();
        }
    };
    //Mở modal thêm mới
    NewsRatifyComponent.prototype.OpenNewsModal = function (item, type) {
        this.Item = new model_1.News();
        this.Item.Status = undefined;
        this.Item.Contents = "";
        this.listLanguageTemp = this.listLanguage;
        this.Item.LanguageId = this.languageId;
        this.Item.Location = this.paging.item_count + 1;
        this.Item.ViewNumber = 1;
        this.Item.listCategory = [];
        this.Item.listTag = [];
        this.Item.listAttachment = [];
        // this.Tag = undefined;
        this.IsAll = true;
        if (this.file)
            this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
        this.progressAttachment = undefined;
        this.Item.TypeNewsId = 1;
        this.CheckBoxStatus = true;
        if (item != undefined) {
            this.GetListNewsNote(item.NewsId);
            item.Note = undefined;
            this.Item = JSON.parse(JSON.stringify(item));
            if (type == 1 || type == 3) {
                this.CheckBoxStatus = this.Item.Status == 1 ? true : false;
            }
            else if (type == 2) {
                this.CheckBoxStatus = this.Item.Status == 1 ? true : false;
                if (this.listLanguage.length == item.listLanguage.length + 1) {
                    this.toastWarning("Bạn đã thêm đủ ngôn ngữ!");
                    return;
                }
                this.listLanguageTemp = [];
                this.Item.NewsId = undefined;
                this.Item.NewsRootId = item.NewsId;
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
                            if (this.listLanguage[i].LanguageId == item.listLanguage[j].lang.LanguageId) {
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
                //this.GetTranslate(this.Item.NewsRootId);
                //this.Item["LangName"] = item.language != undefined ? item.language.Name : "";
                //this.Item["LangFlag"] = item.language != undefined ? item.language.Flag : "";
                //this.Item["LangTitle"] = item.Title;
            }
        }
        if (this.Item.TypeNewsId == 7) {
            this.GetListAllNews();
        }
        this.GetListCateNews();
        this.GetListAllProduct();
        if (type == 2) {
            this.Item.listAttachment = [];
        }
        this.NewsModal.show();
    };
    //Thêm mới bài viết
    NewsRatifyComponent.prototype.SaveNews = function (status) {
        var _this = this;
        if (this.Item.TypeNewsId == undefined) {
            this.toastWarning("Chưa chọn Loại tin!");
            return;
        }
        else if (this.Item.Title == undefined || this.Item.Title == '') {
            this.toastWarning("Chưa nhập Tiêu đề!");
            return;
        }
        else if (this.Item.Title.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tiêu đề!");
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
        else if (this.Item.TypeNewsId != 3 && this.Item.TypeNewsId != 4
            && this.Item.TypeNewsId != 6 && this.Item.TypeNewsId != 7
            && (this.Item.Contents == undefined || this.Item.Contents == '')) {
            this.toastWarning("Chưa nhập Nội dung!");
            return;
        }
        //else if (this.Item.TypeNewsId == 7 && (this.Item.YearTimeline == undefined || this.Item.YearTimeline == null)) {
        //  this.toastWarning("Vui lòng nhập năm!");
        //  return;
        //}
        else if (this.Item.LanguageId == undefined) {
            this.toastWarning("Chưa chọn Ngôn ngữ!");
            return;
        }
        this.Item.Status = status == undefined ? 11 : status; // Mắc dịnh là trạng thái 11 (Bài viết mới)
        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        this.Item.IsComment = true;
        if (typeof this.Item.DateStartActive === 'object' && this.Item.DateStartActive != undefined) {
            console.log(this.Item.DateStartActive);
            var DateStartActive = this.Item.DateStartActive.add(7, 'hours');
            this.Item.DateStartActive = DateStartActive.toISOString();
        }
        if (typeof this.Item.DateStartOn === 'object' && this.Item.DateStartOn != undefined) {
            var DateStartOn = this.Item.DateStartOn.add(7, 'hours');
            this.Item.DateStartOn = DateStartOn.toISOString();
        }
        if (this.Item.DateEndOn == undefined) {
            this.Item.DateEndOn = this.Item.DateStartOn;
        }
        if (typeof this.Item.DateEndOn === 'object' && this.Item.DateEndOn != undefined) {
            var DateEndOn = this.Item.DateEndOn.add(7, 'hours');
            this.Item.DateEndOn = DateEndOn.toISOString();
        }
        var obj = Object.assign({}, this.Item);
        obj.listRelated = [];
        this.listSuggestNews.forEach(function (item) {
            if (item.Check == true) {
                var it_1 = { TargetRelatedId: item.NewsId };
                obj.listRelated.push(it_1);
            }
        });
        obj.listProductRelated = [];
        this.listSuggestProduct.forEach(function (item) {
            if (item.Check == true) {
                var it_2 = { TargetRelatedId: item.ProductId };
                obj.listProductRelated.push(it_2);
            }
        });
        if (this.Item.NewsId == undefined) {
            var arr_1 = [];
            obj.listCategory.forEach(function (item) {
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
            obj.listCategory = arr_1.concat(this.listCateNews.filter(function (e) { return e.Check == true; }));
            this.http.post('/api/news', obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListNews();
                    _this.toastSuccess("Thêm mới thành công!");
                    _this.NewsModal.hide();
                    _this.viewRef.clear();
                }
                else if (res["meta"]["error_code"] == 228) {
                    _this.toastError("Ngôn ngữ này đã có bài viết!");
                }
                else {
                    _this.toastError(res["meta"]["error_message"]);
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            var arr_2 = [];
            obj.listCategory.forEach(function (item) {
                var flag = false;
                for (var i = 0; i < _this.listCateNews.length; i++) {
                    if (item.CategoryId == _this.listCateNews[i].CategoryId && _this.listCateNews[i].Check == true) {
                        flag = true;
                        break;
                    }
                }
                if (!flag) {
                    item.Check = false;
                    arr_2.push(item);
                }
            });
            obj.listCategory = arr_2.concat(this.listCateNews.filter(function (e) { return e.Check == true; }));
            this.http.put('/api/news/' + obj.NewsId, obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListNews();
                    _this.NewsModal.hide();
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
    //Thêm mới bài viết nháp
    //SaveDraft() {
    //  if (this.Item.TypeNewsId == undefined) {
    //    this.toastWarning("Chưa chọn Loại tin!");
    //    return;
    //  } else if (this.Item.Title == undefined || this.Item.Title == '') {
    //    this.toastWarning("Chưa nhập Tiêu đề!");
    //    return;
    //  } else if (this.Item.Title.replace(/ /g, '') == '') {
    //    this.toastWarning("Chưa nhập tiêu đề!");
    //    return;
    //  } else if (this.Item.Url == undefined || this.Item.Url == '') {
    //    this.toastWarning("Chưa nhập Đường dẫn!");
    //    return;
    //  } else if (this.Item.Url.replace(/ /g, '') == '') {
    //    this.toastWarning("Chưa nhập đường dẫn!");
    //    return;
    //  }
    //  else if (this.Item.TypeNewsId != 3 && this.Item.TypeNewsId != 4 && this.Item.TypeNewsId != 6 && (this.Item.Contents == undefined || this.Item.Contents == '')) {
    //    this.toastWarning("Chưa nhập Nội dung!");
    //    return;
    //  }
    //  else if (this.Item.TypeNewsId == 7 && (this.Item.YearTimeline == undefined || this.Item.YearTimeline == null)) {
    //    this.toastWarning("Vui lòng nhập năm!");
    //    return;
    //  }
    //  else if (this.Item.LanguageId == undefined) {
    //    this.toastWarning("Chưa chọn Ngôn ngữ!");
    //    return;
    //  }
    //  this.Item.Status = 10; // Mắc dịnh là trạng thái 13 (Bài viết nháp)
    //  this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
    //  this.Item.UserId = parseInt(localStorage.getItem("userId"));
    //  if (typeof this.Item.DateStartActive === 'object' && this.Item.DateStartActive != undefined) {
    //    console.log(this.Item.DateStartActive);
    //    let DateStartActive = this.Item.DateStartActive.add(7, 'hours');
    //    this.Item.DateStartActive = DateStartActive.toISOString();
    //  }
    //  if (typeof this.Item.DateStartOn === 'object' && this.Item.DateStartOn != undefined) {
    //    let DateStartOn = this.Item.DateStartOn.add(7, 'hours');
    //    this.Item.DateStartOn = DateStartOn.toISOString();
    //  }
    //  if (typeof this.Item.DateEndOn === 'object' && this.Item.DateEndOn != undefined) {
    //    let DateEndOn = this.Item.DateEndOn.add(7, 'hours');
    //    this.Item.DateEndOn = DateEndOn.toISOString();
    //  }
    //  let obj = Object.assign({}, this.Item);
    //  obj.listRelated = [];
    //  this.listSuggestNews.forEach(item => {
    //    if (item.Check == true) {
    //      let it = { TargetRelatedId: item.NewsId }
    //      obj.listRelated.push(it);
    //    }
    //  });
    //  obj.listProductRelated = [];
    //  this.listSuggestProduct.forEach(item => {
    //    if (item.Check == true) {
    //      let it = { TargetRelatedId: item.ProductId }
    //      obj.listProductRelated.push(it);
    //    }
    //  });
    //  if (this.Item.NewsId == undefined) {
    //    let arr = [];
    //    obj.listCategory.forEach(item => {
    //      var flag = false;
    //      for (var i = 0; i < this.listCateNews.length; i++) {
    //        if (item.CategoryId == this.listCateNews[i].CategoryId && this.listCateNews[i].Check == true) {
    //          flag = true;
    //          break;
    //        }
    //      }
    //      if (!flag) {
    //        item.Check = false;
    //        arr.push(item);
    //      }
    //    });
    //    obj.listCategory = arr.concat(this.listCateNews.filter(e => e.Check == true));
    //    this.http.post('/api/news', obj, this.httpOptions).subscribe(
    //      (res) => {
    //        if (res["meta"]["error_code"] == 200) {
    //          this.GetListNews();
    //          this.toastSuccess("Đã thêm vào danh sách nháp!");
    //          this.NewsModal.hide();
    //          this.viewRef.clear();
    //        }
    //        else if (res["meta"]["error_code"] == 228) {
    //          this.toastError("Ngôn ngữ này đã có bài viết!");
    //        }
    //        else {
    //          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
    //        }
    //      },
    //      (err) => {
    //        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
    //      }
    //    );
    //  }
    //  else {
    //    let arr = [];
    //    obj.listCategory.forEach(item => {
    //      var flag = false;
    //      for (var i = 0; i < this.listCateNews.length; i++) {
    //        if (item.CategoryId == this.listCateNews[i].CategoryId && this.listCateNews[i].Check == true) {
    //          flag = true;
    //          break;
    //        }
    //      }
    //      if (!flag) {
    //        item.Check = false;
    //        arr.push(item);
    //      }
    //    });
    //    obj.listCategory = arr.concat(this.listCateNews.filter(e => e.Check == true));
    //    this.http.put('/api/news/' + obj.NewsId, obj, this.httpOptions).subscribe(
    //      (res) => {
    //        if (res["meta"]["error_code"] == 200) {
    //          this.GetListNews();
    //          this.NewsModal.hide();
    //          this.toastSuccess("Cập nhật thành công!");
    //        }
    //        else {
    //          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
    //        }
    //      },
    //      (err) => {
    //        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
    //      }
    //    );
    //  }
    //}
    NewsRatifyComponent.prototype.ToggleCateToList = function (id) {
        this.listNewsT = [];
        for (var i = 0; i < this.listCateNews.length; i++) {
            if (this.listCateNews[i].Check == true) {
                this.objNew = this.listCateNews[i];
                this.listNewsT.push(this.objNew);
            }
        }
    };
    // AddTag() {
    //     if (this.Tag != undefined && this.Tag != '') {
    //         this.Item.listTag.push({ TagId: null, Name: this.Tag, Check: false });
    //         this.Tag = '';
    //     }
    // }ss
    // RemoveTag(i) {
    //     if (this.Item.NewsId == undefined) {
    //         this.Item.listTag.splice(i, 1);
    //     }
    //     else {
    //         if (this.Item.listTag[i].TagId != null) {
    //             this.Item.listTag[i].Check = false;
    //         }
    //         else {
    //             this.Item.listTag.splice(i, 1);
    //         }
    //     }
    // }
    NewsRatifyComponent.prototype.ChangeTitle = function (key) {
        if (this.Item.NewsId == undefined) {
            switch (key) {
                case 1:
                    this.Item.MetaTitle = this.Item.Title;
                    this.Item.MetaKeyword = this.Item.Title;
                    this.Item.Url = this.common.ConvertUrl(this.Item.Title);
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
    NewsRatifyComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        _this.DeleteNews(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-danger',
                }
            ],
        });
    };
    NewsRatifyComponent.prototype.DeleteNews = function (Id) {
        var _this = this;
        this.http.delete('/api/news/' + Id + '/1', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListNews();
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
    NewsRatifyComponent.prototype.upload2 = function (files) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
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
                _this.Item.LinkVideo = event.body["data"];
            }
        });
    };
    NewsRatifyComponent.prototype.RemoveUpFile = function () {
        this.Item.LinkVideo = undefined;
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
    };
    NewsRatifyComponent.prototype.upload = function (files, cs) {
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
                    case 2:
                        _this.progressAttachment = Math.round(100 * event.loaded / event.total);
                        _this.attachment.nativeElement.value = "";
                        break;
                    default:
                        break;
                }
            else if (event.type === http_1.HttpEventType.Response) {
                switch (cs) {
                    case 1:
                        _this.message = event.body["data"].toString();
                        _this.Item.Image = _this.message;
                        break;
                    case 2:
                        _this.attachment.nativeElement.value = "";
                        console.log(event.body["data"]);
                        event.body["data"].forEach(function (item) {
                            var attachment = new model_1.Attactment();
                            attachment.Url = item;
                            attachment.IsImageMain = false;
                            attachment.Status = 1;
                            attachment.Note = undefined;
                            _this.Item.listAttachment.push(attachment);
                        });
                        break;
                    default:
                        break;
                }
                console.log(_this.Item.listAttachment);
            }
        });
    };
    NewsRatifyComponent.prototype.uploadVideo = function (files, cs) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_3 = files; _i < files_3.length; _i++) {
            var file = files_3[_i];
            formData.append(file.name, file);
        }
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload/uploadVideo/1', formData, {
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
                    case 2:
                        _this.progressAttachment = Math.round(100 * event.loaded / event.total);
                        _this.attachment.nativeElement.value = "";
                        break;
                    default:
                        break;
                }
            else if (event.type === http_1.HttpEventType.Response) {
                switch (cs) {
                    case 1:
                        _this.message_video = event.body["data"].toString();
                        _this.Item.LinkVideo = _this.message_video;
                        break;
                    case 2:
                        _this.attachment.nativeElement.value = "";
                        /*console.log(event.body["data"])*/ ;
                        event.body["data"].forEach(function (item) {
                            var attachment = new model_1.Attactment();
                            attachment.Url = item;
                            attachment.IsImageMain = false;
                            attachment.Status = 1;
                            attachment.Note = undefined;
                            _this.Item.listAttachment.push(attachment);
                        });
                        break;
                    default:
                        break;
                }
                /* console.log(this.Item.listAttachment);*/
            }
        });
    };
    NewsRatifyComponent.prototype.findAuthor = function (item) {
        if (item == undefined) {
            return "";
        }
        else {
            return item.FullName;
        }
    };
    NewsRatifyComponent.prototype.RemoveImage = function () {
        this.Item.Image = undefined;
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
    };
    NewsRatifyComponent.prototype.RemoveVideo = function () {
        this.Item.LinkVideo = undefined;
        this.file.nativeElement.value = "";
        this.message_video = undefined;
        this.progress = undefined;
    };
    NewsRatifyComponent.prototype.ConfirmShowHide = function (item, i) {
        var _this = this;
        /*if (item.listLanguage.length > 0) {*/
        this.modalDialogService.openDialog(this.viewRef, {
            title: 'Xác nhận',
            childComponent: ngx_modal_dialog_1.SimpleModalComponent,
            data: {
                text: "Bạn có muốn gửi biên tập bài viết?"
            },
            actionButtons: [
                {
                    text: 'Đồng ý',
                    buttonClass: 'btn btn-success',
                    onAction: function () {
                        _this.ShowHide(item.NewsId, i, 0);
                    }
                },
                {
                    text: 'Không đồng ý',
                    buttonClass: 'btn btn-danger',
                    onAction: function () {
                        _this.listNews[i].IsShow = !_this.listNews[i].IsShow;
                        _this.viewRef.clear();
                    }
                }
            ],
        });
        //}
        //else {
        //  this.ShowHide(item.NewsId, i, 0);
        //}
    };
    NewsRatifyComponent.prototype.ShowHide = function (id, i, isAll) {
        var _this = this;
        /*if (this.CheckConfirmNews == true) {*/
        var stt = 13;
        this.http.put('/api/news/ShowHide/' + id + "/" + stt + "/" + isAll, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                if (stt == 12) {
                    _this.toastSuccess("Bài viết đã được duyệt");
                }
                else {
                    _this.toastSuccess("Bài viết đã được cập nhật");
                }
                _this.GetListNews();
                _this.viewRef.clear();
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                _this.listNews[i].IsShow = !_this.listNews[i].IsShow;
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            _this.listNews[i].IsShow = !_this.listNews[i].IsShow;
        });
        this.viewRef.clear();
        /*}*/
    };
    NewsRatifyComponent.prototype.SortTable = function (str) {
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
        if (this.active) {
            this.GetListNews();
        }
        else if (this.activeU) {
            this.getMyPosts();
        }
        else if (this.activeD) {
            this.getPostsDraft();
        }
    };
    NewsRatifyComponent.prototype.GetClassSortTable = function (str) {
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
    NewsRatifyComponent.prototype.RemoveAttachment = function (idx) {
        if (this.Item.listAttachment[idx].AttactmentId == undefined) {
            this.Item.listAttachment.splice(idx, 1);
        }
        else {
            this.Item.listAttachment[idx].Status = 99;
        }
    };
    NewsRatifyComponent.prototype.SetIsMain = function (idx) {
        for (var i = 0; i < this.Item.listAttachment.length; i++) {
            this.Item.listAttachment[i].IsImageMain = false;
            if (idx == i) {
                this.Item.listAttachment[i].IsImageMain = true;
            }
        }
    };
    NewsRatifyComponent.prototype.CheckActionTable = function (NewsId) {
        if (NewsId == undefined) {
            var CheckAll_1 = this.CheckAll;
            this.listNews.forEach(function (item) {
                item.Action = CheckAll_1;
            });
        }
        else {
            var CheckAll = true;
            for (var i = 0; i < this.listNews.length; i++) {
                if (!this.listNews[i].Action) {
                    CheckAll = false;
                    break;
                }
            }
            this.CheckAll = CheckAll == true ? true : false;
        }
    };
    NewsRatifyComponent.prototype.ActionTableFunc = function () {
        var _this = this;
        switch (this.ActionId) {
            case 1:
                var data_1 = [];
                this.listNews.forEach(function (item) {
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
                                            _this.GetListNews();
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
    NewsRatifyComponent.prototype.ChangeLinkDetailNews = function (TypeNewsId, Url, NewsId) {
        return const_1.domain + this.listTypeNews.filter(function (x) { return x.Id == TypeNewsId; })[0].ConstUrl + "/" + Url + "-" + NewsId + ".html";
    };
    NewsRatifyComponent.prototype.OpenModalTag = function () {
        this.Tag = new model_1.Tag();
        this.TagModal.show();
    };
    NewsRatifyComponent.prototype.SaveTag = function () {
        var _this = this;
        if (this.Tag.Name == undefined || this.Tag.Name == '') {
            this.toastWarning("Chưa nhập Tên hiển thị!");
            return;
        }
        else if (this.Tag.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập Tên hiển thị!");
            return;
        }
        else if (this.Tag.Url == undefined || this.Tag.Url == '') {
            this.toastWarning("Chưa nhập Đường dẫn!");
            return;
        }
        else if (this.Tag.Url.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập Đường dẫn!");
            return;
        }
        this.Tag.UserId = parseInt(localStorage.getItem("userId"));
        this.Tag.TargetType = 1;
        this.http.post('/api/tag', this.Tag, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                var obj = { TagId: res["data"].TagId, Name: res["data"].Name };
                // console.log(obj);
                // this.Item.listTag.push(obj);
                // console.log(this.Item.listTag);
                _this.GetListTag(obj);
                _this.TagModal.hide();
                _this.toastSuccess("Thêm thành công!");
            }
            else {
                _this.toastError(res["meta"]["error_message"]);
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
        });
    };
    NewsRatifyComponent.prototype.ChangeTitleTag = function () {
        this.Tag.Url = this.common.ConvertUrl(this.Tag.Name);
    };
    NewsRatifyComponent.prototype.closeNoityfy = function () {
        this.isNoitify = true;
    };
    NewsRatifyComponent.prototype.ShowConfirmStatus = function (id, status) {
        var _this = this;
        var title = "Bạn có chắc chắn muốn gửi biên tập bài viết này?";
        if (status == 11)
            title = "Bạn có chắc chắn muốn chuyển lên bài viết mới?";
        this.modalDialogService.openDialog(this.viewRef, {
            title: 'Xác nhận',
            childComponent: ngx_modal_dialog_1.SimpleModalComponent,
            data: {
                text: title
            },
            actionButtons: [
                {
                    text: 'Đồng ý',
                    buttonClass: 'btn btn-success',
                    onAction: function () {
                        _this.SendStatus(id, status);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-danger',
                }
            ],
        });
    };
    NewsRatifyComponent.prototype.SendStatus = function (id, status) {
        var _this = this;
        this.http.put('/api/news/ShowHide/' + id + "/" + status + "/0", undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                if (status != 11)
                    _this.toastSuccess("Bài viết đã được gửi biên tập thành công!");
                else
                    _this.toastSuccess("Bài viết đã được chuyển thành bài viết mới thành công!");
                _this.GetListNews();
                _this.NewsModal.hide();
                _this.viewRef.clear();
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                //this.listNews[i].IsShow = !this.listNews[i].IsShow;
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            //this.listNews[i].IsShow = !this.listNews[i].IsShow;
        });
    };
    NewsRatifyComponent.prototype.SaveNoteNews = function () {
        var _this = this;
        if (this.Item.Note == undefined) {
            this.toastWarning("Chưa nhập góp ý!");
            return;
        }
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        if (typeof this.Item.DateStartActive === 'object' && this.Item.DateStartActive != undefined) {
            var DateStartActive = this.Item.DateStartActive.add(7, 'hours');
            this.Item.DateStartActive = DateStartActive.toISOString();
        }
        if (typeof this.Item.DateStartOn === 'object' && this.Item.DateStartOn != undefined) {
            var DateStartOn = this.Item.DateStartOn.add(7, 'hours');
            this.Item.DateStartOn = DateStartOn.toISOString();
        }
        if (typeof this.Item.DateEndOn === 'object' && this.Item.DateEndOn != undefined) {
            var DateEndOn = this.Item.DateEndOn.add(7, 'hours');
            this.Item.DateEndOn = DateEndOn.toISOString();
        }
        var obj = Object.assign({}, this.Item);
        obj.listRelated = [];
        this.listSuggestNews.forEach(function (item) {
            if (item.Check == true) {
                var it_3 = { TargetRelatedId: item.NewsId };
                obj.listRelated.push(it_3);
            }
        });
        obj.listProductRelated = [];
        this.listSuggestProduct.forEach(function (item) {
            if (item.Check == true) {
                var it_4 = { TargetRelatedId: item.ProductId };
                obj.listProductRelated.push(it_4);
            }
        });
        if (this.Item.NewsId == undefined) {
            var arr_3 = [];
            obj.listCategory.forEach(function (item) {
                var flag = false;
                for (var i = 0; i < _this.listCateNews.length; i++) {
                    if (item.CategoryId == _this.listCateNews[i].CategoryId && _this.listCateNews[i].Check == true) {
                        flag = true;
                        break;
                    }
                }
                if (!flag) {
                    item.Check = false;
                    arr_3.push(item);
                }
            });
            obj.listCategory = arr_3.concat(this.listCateNews.filter(function (e) { return e.Check == true; }));
            this.http.post('/api/news', obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListNews();
                    _this.toastSuccess("Đã thêm vào danh sách nháp!");
                    _this.NewsModal.hide();
                    _this.viewRef.clear();
                }
                else if (res["meta"]["error_code"] == 228) {
                    _this.toastError("Ngôn ngữ này đã có bài viết!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            var arr_4 = [];
            obj.listCategory.forEach(function (item) {
                var flag = false;
                for (var i = 0; i < _this.listCateNews.length; i++) {
                    if (item.CategoryId == _this.listCateNews[i].CategoryId && _this.listCateNews[i].Check == true) {
                        flag = true;
                        break;
                    }
                }
                if (!flag) {
                    item.Check = false;
                    arr_4.push(item);
                }
            });
            obj.listCategory = arr_4.concat(this.listCateNews.filter(function (e) { return e.Check == true; }));
            this.http.put('/api/news/' + obj.NewsId, obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    //this.GetListNews();
                    _this.GetListNewsNote(_this.Item.NewsId);
                    _this.Item.Note = undefined;
                    _this.toastSuccess("Góp ý thành công!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
    };
    NewsRatifyComponent.prototype.ExportExcel = function () {
        var _this = this;
        if (this.q.TypePaymentOrderStatus == 1)
            this.q.CashStatus = undefined;
        else if (this.q.TypePaymentOrderStatus == 2)
            this.q.CashStatus = true;
        else if (this.q.TypePaymentOrderStatus == 3)
            this.q.CashStatus = false;
        var data = Object.assign({}, this.q);
        data.TypeExport = 1;
        //console.log(this.q);
        //if (this.dateStart.nativeElement.value) {
        //  let obj = this.dateStart.nativeElement.value.split("/");
        //  data.DateStart = obj[2] + "-" + obj[1] + "-" + obj[0] + " 0:0:0";
        //}
        //if (this.dateEnd.nativeElement.value) {
        //  let obj = this.dateEnd.nativeElement.value.split("/");
        //  data.DateEnd = obj[2] + "-" + obj[1] + "-" + obj[0] + " 23:59:59";
        //}
        this.http.post('/api/news/exportNews', data, this.httpOptionsBlob).subscribe(function (res) {
            //console.log(res);
            var url = window.URL.createObjectURL(res["body"]);
            var a = document.createElement('a');
            document.body.appendChild(a);
            a.setAttribute('style', 'display: none');
            a.href = url;
            a.download = "ds-bai-viet.xlsx";
            a.click();
            window.URL.revokeObjectURL(url);
            a.remove();
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    NewsRatifyComponent.prototype.OpenMediaModal = function (type) {
        this.typeMedia = type;
        this.OpenMediaFile.show();
    };
    NewsRatifyComponent.prototype.CloseMediaModal = function () {
        this.OpenMediaFile.hide();
    };
    //OpenMediaModalVideo() {
    //  this.OpenMediaFileVideo.show();
    //}
    //CloseMediaModalVideo() {
    //  this.OpenMediaFileVideo.hide();
    //}
    NewsRatifyComponent.prototype.tabHandleMedia = function () {
        this.isActiveMedia = true;
        this.isActiveUpload = false;
    };
    NewsRatifyComponent.prototype.tabHandleMediaUpload = function () {
        this.isActiveMedia = false;
        this.isActiveUpload = true;
    };
    NewsRatifyComponent.prototype.upload3 = function (files, cs) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_4 = files; _i < files_4.length; _i++) {
            var file = files_4[_i];
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
    NewsRatifyComponent.prototype.loadMore = function () {
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
    NewsRatifyComponent.prototype.GetListFiles = function () {
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
    NewsRatifyComponent.prototype.SeclectMedia = function (item) {
        if (this.typeMedia == 1)
            this.Item.Image = item.url + "/" + item.name;
        else if (this.typeMedia == 2)
            this.Item.LinkVideo = item.url + "/" + item.name;
        else if (this.typeMedia == 3) {
            var attachment = new model_1.Attactment();
            attachment.Url = item.url + "/" + item.name;
            attachment.IsImageMain = false;
            attachment.Status = 1;
            attachment.Note = undefined;
            //check xem có chưa thì add
            var checkDuplicate = false;
            for (var kk = 0; kk < this.Item.listAttachment.length; kk++) {
                if (this.Item.listAttachment[kk].Url == attachment.Url) {
                    checkDuplicate = true;
                    break;
                }
            }
            if (!checkDuplicate)
                this.Item.listAttachment.push(attachment);
            else {
                this.toastWarning("Bạn đã chọn ảnh này!");
            }
        }
        this.OpenMediaFile.hide();
    };
    __decorate([
        core_1.ViewChild('NewsModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], NewsRatifyComponent.prototype, "NewsModal", void 0);
    __decorate([
        core_1.ViewChild('HighlightNewsModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], NewsRatifyComponent.prototype, "HighlightNewsModal", void 0);
    __decorate([
        core_1.ViewChild('TagModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], NewsRatifyComponent.prototype, "TagModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], NewsRatifyComponent.prototype, "file", void 0);
    __decorate([
        core_1.ViewChild('filevideo'),
        __metadata("design:type", core_1.ElementRef)
    ], NewsRatifyComponent.prototype, "filevideo", void 0);
    __decorate([
        core_1.ViewChild('attachment'),
        __metadata("design:type", core_1.ElementRef)
    ], NewsRatifyComponent.prototype, "attachment", void 0);
    __decorate([
        core_1.ViewChild('tabset'),
        __metadata("design:type", tabs_1.TabsetComponent)
    ], NewsRatifyComponent.prototype, "tabset", void 0);
    __decorate([
        core_1.ViewChild('OpenMediaFile'),
        __metadata("design:type", modal_1.ModalDirective)
    ], NewsRatifyComponent.prototype, "OpenMediaFile", void 0);
    __decorate([
        core_1.ViewChild('OpenMediaFileVideo'),
        __metadata("design:type", modal_1.ModalDirective)
    ], NewsRatifyComponent.prototype, "OpenMediaFileVideo", void 0);
    __decorate([
        core_1.ViewChild('dateStart'),
        __metadata("design:type", core_1.ElementRef)
    ], NewsRatifyComponent.prototype, "dateStart", void 0);
    __decorate([
        core_1.ViewChild('dateEnd'),
        __metadata("design:type", core_1.ElementRef)
    ], NewsRatifyComponent.prototype, "dateEnd", void 0);
    NewsRatifyComponent = __decorate([
        core_1.Component({
            selector: 'app-news-retify',
            templateUrl: './news-ratify.component.html',
            styleUrls: ['./news-ratify.component.scss'],
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
    ], NewsRatifyComponent);
    return NewsRatifyComponent;
}());
exports.NewsRatifyComponent = NewsRatifyComponent;
//# sourceMappingURL=news-ratify.component.js.map