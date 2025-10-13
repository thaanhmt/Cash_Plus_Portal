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
exports.DatasetComponent = exports.MY_CUSTOM_FORMATS = void 0;
var core_1 = require("@angular/core");
var platform_browser_1 = require("@angular/platform-browser");
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var const_1 = require("../../../data/const");
var model_1 = require("../../../data/model");
var ngx_toastr_1 = require("ngx-toastr");
var common_1 = require("@angular/common");
var common_service_1 = require("../../../service/common.service");
var dt_1 = require("../../../data/dt");
var tabs_1 = require("ngx-bootstrap/tabs");
//import { RequestOptions, ResponseContentType, Headers, ResponseType } from '@angular/http';
//import { getFileNameFromResponseContentDisposition, saveFile } from '../../../service/file-download-helper';
//import { InterceptorService } from 'ng2-interceptors';
exports.MY_CUSTOM_FORMATS = {
    parseInput: 'DD/MM/YYYY HH:mm',
    fullPickerInput: 'DD/MM/YYYY HH:mm',
    datePickerInput: 'DD/MM/YYYY',
    timePickerInput: ' HH:mm',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY'
};
var DatasetComponent = /** @class */ (function () {
    function DatasetComponent(http, modalDialogService, viewRef, toastr, datePipe, common, sanitizer) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.datePipe = datePipe;
        this.common = common;
        this.sanitizer = sanitizer;
        //@ViewChild('dateStart') dateStart: ElementRef;
        //@ViewChild('dateEnd') dateEnd: ElementRef;
        this.listItemMedia = [];
        this.domainMedia = const_1.domainMedia;
        this.isActiveMedia = true;
        this.isActiveUpload = false;
        this.isDelay = false;
        this.listDatas = [];
        this.listUnit = [];
        this.listApplicationRange = [];
        this.listResearchArea = [];
        this.listStatus = const_1.listDataSetStatus;
        this.listTypes = const_1.listDataSetTypes;
        this.listAuthors = [];
        this.listFiles = const_1.listDataSetFiles;
        this.listConfirmData = const_1.listConfirmData;
        this.listLanguage = [];
        this.listLanguageTemp = [];
        this.isNoitify = false;
        this.domainVideos = const_1.domainVideos;
        this.domainImage = const_1.domainImage;
        this.ActionTable = const_1.ActionTable;
        this.Status = const_1.Status;
        this.listData = [];
        this.activeD = false;
        this.activeU = false;
        this.RoleCode = localStorage.getItem("roleCode") || '';
        this.NameAuthor = localStorage.getItem("fullName") || '';
        this.domainDebug = const_1.domainDebug;
        this.PriceMaskConfig = {
            align: "left",
            allowNegative: false,
            decimal: ".",
            precision: 0,
            prefix: "",
            suffix: "",
            thousands: ","
        };
        this.pagingFile = new dt_1.Paging();
        this.pagingFile.page = 1;
        this.pagingFile.page_size = 24;
        this.pagingFile.query = "1=1";
        this.pagingFile.order_by = "";
        this.pagingFile.item_count = 0;
        this.countMedia = 24;
        this.Item = new model_1.DataSet();
        this.ItemApproved = new model_1.DataSetApproved();
        this.fileView = new model_1.Attactment();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "";
        this.paging.item_count = 0;
        this.q = new dt_1.QueryFilter();
        this.q.txtSearch = "";
        this.IsAll = false;
        this.CheckConfirmNews = this.common.CheckAccessKey(localStorage.getItem("access_key"), "DBBTV") && this.RoleCode != 'BTV';
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
        this.CheckRole = new dt_1.CheckRole();
        var code = "QLDL";
        this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 0);
        this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 1);
        this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 2);
        this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 3);
        this.CheckRole.Export = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 5);
        //
        this.CheckRole2 = new dt_1.CheckRole();
        var code2 = "DNB";
        this.CheckRole2.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code2, 2);
        //
        this.CheckRole3 = new dt_1.CheckRole();
        var code3 = "DCK";
        this.CheckRole3.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code3, 2);
    }
    DatasetComponent.prototype.ngOnInit = function () {
        this.RoleCode = localStorage.getItem("roleCode");
        this.NameAuthor = localStorage.getItem("fullName");
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.paging.query = "1=1";
        this.domain = const_1.domain;
        this.GetListData();
        this.GetListCategory(14);
        this.GetListCategory(15);
        this.GetListUnit();
        this.GetListAuthor();
        this.GetListLanguage();
        this.GetListFiles();
        this.GetDomainStatic();
    };
    DatasetComponent.prototype.GetDomainStatic = function () {
        var _this = this;
        this.http.get('api/Config/1', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.staticDomain = res["data"].Website;
                if (res["data"].ModeSite) {
                    _this.staticDomain = res["data"].Website;
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
    DatasetComponent.prototype.GetListData = function () {
        var _this = this;
        var data = Object.assign({}, this.q);
        //if (this.dateStart.nativeElement.value) {
        //  let obj = this.dateStart.nativeElement.value.split("/");
        //  data.DateStart = obj[2] + "-" + obj[1] + "-" + obj[0] + " 0:0:0";
        //}
        //if (this.dateEnd.nativeElement.value) {
        //  let obj = this.dateEnd.nativeElement.value.split("/");
        //  data.DateEnd = obj[2] + "-" + obj[1] + "-" + obj[0] + " 23:59:59";
        //}
        data.page = this.paging.page;
        data.page_size = this.paging.page_size;
        data.query = this.paging.query;
        data.order_by = this.paging.order_by;
        if (data.ApplicationRangeId == undefined)
            data.ApplicationRangeId = -1;
        if (data.ResearchAreaId == undefined)
            data.ResearchAreaId = -1;
        if (data.Extention == undefined)
            data.Extention = -1;
        if (data.UnitId == undefined)
            data.UnitId = -1;
        this.http.post('/api/dataSet/GetByPagePost', data, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listDatas = res["data"];
                _this.paging.item_count = res["metadata"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
        //this.http.get('/api/dataSet/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
        //  (res) => {
        //    if (res["meta"]["error_code"] == 200) {
        //      this.listDatas = res["data"];
        //      this.paging.item_count = res["metadata"];
        //    }
        //  },
        //  (err) => {
        //    console.log("Error: connect to API");
        //  }
        //);
    };
    DatasetComponent.prototype.GetListCategory = function (type) {
        var _this = this;
        this.http.get('/api/category/GetByPage?page=1&query=TypeCategoryId=' + type + '&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                if (type == 14)
                    _this.listResearchArea = res["data"];
                else
                    _this.listApplicationRange = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    DatasetComponent.prototype.GetListUnit = function () {
        var _this = this;
        this.http.get('/api/unit/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listUnit = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    DatasetComponent.prototype.GetListAuthor = function () {
        var _this = this;
        this.http.get('/api/customer/GetByPage?page=1&page_size=200&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listAuthors = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    // Get danh sách ngôn ngữ
    DatasetComponent.prototype.GetListLanguage = function () {
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
    //Chuyển trang
    DatasetComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListData();
    };
    //Thông báo
    DatasetComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    DatasetComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    DatasetComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    DatasetComponent.prototype.QueryChanged = function () {
        var query = "1=1";
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            //if (query != '') {
            query += ' and (Title.Contains("' + this.q.txtSearch + '") or AuthorName.Contains("' + this.q.txtSearch + '"))';
        }
        if (this.q.Type != undefined) {
            query += ' and Type=' + this.q.Type;
        }
        if (this.q.LanguageId != undefined) {
            query += ' and LanguageId=' + this.q.LanguageId;
        }
        if (this.q.Status != undefined) {
            if (this.q.Status != 4)
                query += ' and Status=' + this.q.Status;
            else
                query += ' and (Status=4 OR Status=5)';
        }
        if (this.q.CustomerId != undefined) {
            query += ' and UserCreatedId=' + this.q.CustomerId;
        }
        this.paging.query = query;
        this.GetListData();
    };
    //
    DatasetComponent.prototype.StatusChanged = function (status) {
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
        query += ' and Status=' + status;
        this.paging.query = query;
        this.GetListData();
    };
    //Mở modal thêm mới
    DatasetComponent.prototype.OpenDataModal = function (item, type) {
        this.Item = new model_1.DataSet();
        this.Item.Contents = "";
        this.listLanguageTemp = this.listLanguage;
        this.Item.LanguageId = this.languageId;
        this.Item.Location = this.paging.item_count + 1;
        this.Item.ViewNumber = 1;
        // this.Tag = undefined;
        this.IsAll = true;
        if (this.file)
            this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
        this.progressAttachment = undefined;
        this.Item.Type = 1;
        this.CheckBoxStatus = true;
        if (item != undefined) {
            //item.Note = undefined;
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
                this.Item.DataSetId = undefined;
                this.Item.DataSetRootId = item.NewsId;
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
            }
        }
        this.DataModal.show();
    };
    DatasetComponent.prototype.SaveData = function (type) {
        var _this = this;
        if (this.Item.Title == undefined || this.Item.Title == '') {
            this.toastWarning("Chưa nhập Tiêu đề!");
            return;
        }
        else if (this.Item.Title.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tiêu đề!");
            return;
        }
        else if (this.Item.ApplicationRangeId == undefined || this.Item.ApplicationRangeId < 0) {
            this.toastWarning("Chưa chọn phạm vi ứng dụng!");
            return;
        }
        else if (this.Item.ResearchAreaId == undefined || this.Item.ResearchAreaId < 0) {
            this.toastWarning("Chưa chọn lĩnh vực nghiên cứu!");
            return;
        }
        else if (this.Item.Description == undefined || this.Item.Description == '') {
            this.toastWarning("Chưa nhập nội dung mô tả!");
            return;
        }
        else if (this.Item.Description.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập nội dung mô tả!");
            return;
        }
        else if (this.Item.AuthorName == undefined || this.Item.AuthorName == '') {
            this.toastWarning("Chưa nhập tên tác giả!");
            return;
        }
        else if (this.Item.AuthorName.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên tác giả!");
            return;
        }
        //else if (this.Item.LanguageId == undefined) {
        //  this.toastWarning("Chưa chọn Ngôn ngữ!");
        //  return;
        //}
        //this.Item.Status = status;
        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        this.Item.UserCreatedId = parseInt(localStorage.getItem("userId"));
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
        if (obj.DateStartActive != undefined) {
            var obj2 = new Date(obj.DateStartActive);
            obj.DateStartActive = obj2.getFullYear() + "-" + (obj2.getMonth() + 1) + "-" + obj2.getDate() + " " + obj2.getHours() + ":" + obj2.getMinutes() + ":0";
            if (obj.TimeStartActive != undefined) {
                var obj1 = new Date(obj.TimeStartActive);
                obj.DateStartActive = obj2.getFullYear() + "-" + (obj2.getMonth() + 1) + "-" + obj2.getDate() + " " + obj1.getHours() + ":" + obj1.getMinutes() + ":0";
            }
        }
        //obj.listProductRelated = [];
        //this.listSuggestProduct.forEach(item => {
        //  if (item.Check == true) {
        //    let it = { TargetRelatedId: item.ProductId }
        //    obj.listProductRelated.push(it);
        //  }
        //});
        if (this.Item.DataSetId == undefined) {
            //let arr = [];
            //obj.listCategory.forEach(item => {
            //  var flag = false;
            //  for (var i = 0; i < this.listCateNews.length; i++) {
            //    if (item.CategoryId == this.listCateNews[i].CategoryId && this.listCateNews[i].Check == true) {
            //      flag = true;
            //      break;
            //    }
            //  }
            //  if (!flag) {
            //    item.Check = false;
            //    arr.push(item);
            //  }
            //});
            //obj.listCategory = arr.concat(this.listCateNews.filter(e => e.Check == true));
            this.http.post('/api/dataset', obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListData();
                    _this.toastSuccess("Thêm mới thành công!");
                    _this.DataModal.hide();
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
            //let arr = [];
            //obj.listCategory.forEach(item => {
            //  var flag = false;
            //  for (var i = 0; i < this.listCateNews.length; i++) {
            //    if (item.CategoryId == this.listCateNews[i].CategoryId && this.listCateNews[i].Check == true) {
            //      flag = true;
            //      break;
            //    }
            //  }
            //  if (!flag) {
            //    item.Check = false;
            //    arr.push(item);
            //  }
            //});
            //obj.listCategory = arr.concat(this.listCateNews.filter(e => e.Check == true));
            this.http.put('/api/dataset/' + obj.DataSetId, obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListData();
                    _this.DataModal.hide();
                    //if (status == 1)
                    //  this.ConfigDateModal.hide();
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
    DatasetComponent.prototype.ChangeTitle = function (key) {
        if (this.Item.DataSetId == undefined) {
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
    //Mở modal xem
    DatasetComponent.prototype.OpenViewModal = function (item, type) {
        this.Item = new model_1.DataSet();
        //this.Item.Contents = "";
        //this.listLanguageTemp = this.listLanguage;
        //this.Item.LanguageId = this.languageId;
        //this.Item.Location = this.paging.item_count + 1;
        //this.Item.ViewNumber = 1;
        //this.Item.listCategory = [];
        //this.Item.listTag = [];
        //this.Item.listAttachment = [];
        // this.Tag = undefined;
        //this.IsAll = true;
        //if (this.file) this.file.nativeElement.value = "";
        //this.message = undefined;
        //this.progress = undefined;
        //this.progressAttachment = undefined;
        //this.Item.Type = 1;
        //this.CheckBoxStatus = true;
        if (item != undefined) {
            //item.Note = undefined;
            this.Item = JSON.parse(JSON.stringify(item));
            this.CheckBoxStatus = this.Item.Status == 1 ? true : false;
        }
        this.ViewModal.show();
    };
    DatasetComponent.prototype.OpenConfirmModal = function (item, type, status) {
        this.StatusApproved = status;
        this.ItemApproved = new model_1.DataSetApproved();
        this.ItemApproved.Type = type;
        this.disabledStatus = false;
        if (status == 1 || status == 2) {
            this.ItemApproved.DataSetStatus = 2;
            this.disabledStatus = true;
        }
        else
            this.ItemApproved.DataSetStatus = 1;
        if (item != undefined) {
            this.Item = JSON.parse(JSON.stringify(item));
            this.ItemApproved.DataSetId = this.Item.DataSetId;
        }
        this.ConfirmModal.show();
    };
    DatasetComponent.prototype.OpenModalDetailCate = function (item, type) {
        this.nameTitle = item['Title'];
        if (type == 1) {
            this.NameCate = "phạm vi ứng dụng";
            this.listData = item['applicationRange'];
        }
        else if (type == 2) {
            this.NameCate = "lĩnh vực nghiên cứu";
            this.listData = item['researchArea'];
        }
        else if (type == 3) {
            this.NameCate = "cơ quan/tổ chức";
            this.listData = item['unit'];
        }
        this.DetailCategory.show();
    };
    DatasetComponent.prototype.SaveConfirm = function () {
        var _this = this;
        if (this.ItemApproved.DataSetStatus == undefined || this.ItemApproved.DataSetStatus < 0) {
            this.toastWarning("Chưa chọn trạng thái phê duyệt!");
            return;
        }
        else if (this.ItemApproved.DataSetStatus == 2) {
            if (this.ItemApproved.Confirms == undefined || this.ItemApproved.Confirms == '') {
                this.toastWarning("Chưa nhập lý do không phê duyệt!");
                return;
            }
            else if (this.ItemApproved.Confirms.replace(/ /g, '') == '') {
                this.toastWarning("Chưa nhập lý do không phê duyệt!");
                return;
            }
        }
        this.ItemApproved.CreatedId = parseInt(localStorage.getItem("userId"));
        this.ItemApproved.UpdatedId = parseInt(localStorage.getItem("userId"));
        var obj = Object.assign({}, this.ItemApproved);
        if (this.Item.DataSetId != undefined) {
            this.http.post('/api/dataSetApproved', obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListData();
                    _this.toastSuccess("Phê duyệt thành công!");
                    _this.ViewModal.hide();
                    _this.ConfirmModal.hide();
                    _this.viewRef.clear();
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
    DatasetComponent.prototype.ShowConfirmDelete = function (Id) {
        var _this = this;
        this.modalDialogService.openDialog(this.viewRef, {
            title: 'Xác nhận',
            childComponent: ngx_modal_dialog_1.SimpleModalComponent,
            data: {
                text: "Bạn có chắc chắn muốn xóa bộ dữ liệu này?"
            },
            actionButtons: [
                {
                    text: 'Đồng ý',
                    buttonClass: 'btn btn-success',
                    onAction: function () {
                        _this.DeleteData(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-danger',
                }
            ],
        });
    };
    DatasetComponent.prototype.DeleteData = function (Id) {
        var _this = this;
        this.http.delete('/api/dataSet/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListData();
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
    DatasetComponent.prototype.upload2 = function (files) {
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
    DatasetComponent.prototype.RemoveUpFile = function () {
        this.Item.LinkVideo = undefined;
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
    };
    DatasetComponent.prototype.upload = function (files, cs) {
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
                        /*console.log(event.body["data"])*/ ;
                        event.body["data"].forEach(function (item) {
                            var attachment = new model_1.Attactment();
                            attachment.Url = item;
                            attachment.IsImageMain = false;
                            attachment.Status = 1;
                            attachment.Note = undefined;
                            //this.Item.listAttachment.push(attachment);
                        });
                        break;
                    default:
                        break;
                }
                /* console.log(this.Item.listAttachment);*/
            }
        });
    };
    DatasetComponent.prototype.uploadVideo = function (files, cs) {
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
                            //this.Item.listAttachment.push(attachment);
                        });
                        break;
                    default:
                        break;
                }
                /* console.log(this.Item.listAttachment);*/
            }
        });
    };
    DatasetComponent.prototype.findAuthor = function (item) {
        if (item == undefined) {
            return "";
        }
        else {
            return item.FullName;
        }
    };
    DatasetComponent.prototype.RemoveImage = function () {
        this.Item.Image = undefined;
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
    };
    DatasetComponent.prototype.RemoveVideo = function () {
        this.Item.LinkVideo = undefined;
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
    };
    DatasetComponent.prototype.ConfirmShowHide = function (item, i) {
        var _this = this;
        var stt = this.listDatas[i].IsShow ? 1 : 19;
        if (stt == 1) {
            this.modalDialogService.openDialog(this.viewRef, {
                title: 'Xác nhận',
                childComponent: ngx_modal_dialog_1.SimpleModalComponent,
                data: {
                    text: "Bạn có muốn hẹn ngày xuất bản?"
                },
                actionButtons: [
                    {
                        text: 'Đặt lịch',
                        buttonClass: 'btn btn-primary',
                        onAction: function () {
                            //this.OpenModalConfigDate(item);
                            _this.viewRef.clear();
                        }
                    },
                    {
                        text: 'Xuất bản luôn',
                        buttonClass: 'btn btn-success',
                        onAction: function () {
                            _this.ShowHide(item.NewsId, i, 0);
                        }
                    },
                    {
                        text: 'Đóng',
                        buttonClass: 'btn btn-danger',
                        onAction: function () {
                            _this.listDatas[i].IsShow = !_this.listDatas[i].IsShow;
                            _this.viewRef.clear();
                        }
                    }
                ],
            });
        }
        else {
            this.ShowHide(item.NewsId, i, 0);
        }
    };
    DatasetComponent.prototype.ShowHide = function (id, i, isAll) {
        var _this = this;
        var stt = this.listDatas[i].IsShow ? 1 : 19;
        this.http.put('/api/news/ShowHide/' + id + "/" + stt + "/" + isAll, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                if (stt == 1) {
                    _this.toastSuccess("Bài viết đã được xuất bản!");
                }
                else if (stt == 19) {
                    _this.toastWarning("Bài viết đã được gỡ xuất bản!");
                }
                else {
                    _this.toastSuccess("Thay đổi trạng thái thành công!");
                }
                _this.GetListData();
                _this.viewRef.clear();
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                _this.listDatas[i].IsShow = !_this.listDatas[i].IsShow;
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            _this.listDatas[i].IsShow = !_this.listDatas[i].IsShow;
        });
    };
    DatasetComponent.prototype.SortTable = function (str) {
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
        this.GetListData();
    };
    DatasetComponent.prototype.GetClassSortTable = function (str) {
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
    DatasetComponent.prototype.GetClassSortTablePublic = function (str) {
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
    //RemoveAttachment(idx) {
    //  if (this.Item.listAttachment[idx].AttactmentId == undefined) {
    //    this.Item.listAttachment.splice(idx, 1);
    //  }
    //  else {
    //    this.Item.listAttachment[idx].Status = 99;
    //  }
    //}
    //SetIsMain(idx) {
    //  for (let i = 0; i < this.Item.listAttachment.length; i++) {
    //    this.Item.listAttachment[i].IsImageMain = false;
    //    if (idx == i) {
    //      this.Item.listAttachment[i].IsImageMain = true;
    //    }
    //  }
    //}
    DatasetComponent.prototype.CheckActionTable = function (NewsId) {
        if (NewsId == undefined) {
            var CheckAll_1 = this.CheckAll;
            this.listDatas.forEach(function (item) {
                item.Action = CheckAll_1;
            });
        }
        else {
            var CheckAll = true;
            for (var i = 0; i < this.listDatas.length; i++) {
                if (!this.listDatas[i].Action) {
                    CheckAll = false;
                    break;
                }
            }
            this.CheckAll = CheckAll == true ? true : false;
        }
    };
    DatasetComponent.prototype.ActionTableFunc = function () {
        var _this = this;
        if (this.RoleCode == 'ADMIN') {
            switch (this.ActionId) {
                case 1:
                    var data_1 = [];
                    this.listDatas.forEach(function (item) {
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
                                        _this.http.put('/api/news/DeleteMultiNewsPublic', data_1, _this.httpOptions).subscribe(function (res) {
                                            if (res["meta"]["error_code"] == 200) {
                                                _this.toastSuccess("Xóa thành công!");
                                                _this.GetListData();
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
        }
        else {
            this.toastError("Bạn không đủ quyền để xoá");
        }
    };
    //ChangeLinkDetailNews(TypeNewsId, Url, NewsId) {
    //  return this.staticDomain + this.listTypeNews.filter(x => x.Id == TypeNewsId)[0].ConstUrl + "/" + Url + "-" + NewsId + ".html";
    //}
    DatasetComponent.prototype.closeNoityfy = function () {
        this.isNoitify = true;
    };
    DatasetComponent.prototype.ShowConfirmStatus = function (id, status) {
        var title = "Bạn có chắc chắn muốn xuất bản bài viết này?";
        if (status == 19)
            title = "Bạn có chắc chắn muốn gỡ bài viết này?";
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
                        //this.SendStatus(id, status);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-danger',
                }
            ],
        });
    };
    DatasetComponent.prototype.ExportExcel = function () {
        var _this = this;
        if (this.q.TypePaymentOrderStatus == 1)
            this.q.CashStatus = undefined;
        else if (this.q.TypePaymentOrderStatus == 2)
            this.q.CashStatus = true;
        else if (this.q.TypePaymentOrderStatus == 3)
            this.q.CashStatus = false;
        var data = Object.assign({}, this.q);
        data.TypeExport = 4;
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
    DatasetComponent.prototype.OpenMediaModal = function (type) {
        this.typeMedia = type;
        this.OpenMediaFile.show();
    };
    DatasetComponent.prototype.CloseMediaModal = function () {
        this.OpenMediaFile.hide();
    };
    DatasetComponent.prototype.tabHandleMedia = function () {
        this.isActiveMedia = true;
        this.isActiveUpload = false;
    };
    DatasetComponent.prototype.tabHandleMediaUpload = function () {
        this.isActiveMedia = false;
        this.isActiveUpload = true;
    };
    DatasetComponent.prototype.upload3 = function (files, cs) {
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
    DatasetComponent.prototype.loadMore = function () {
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
    DatasetComponent.prototype.GetListFiles = function () {
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
    DatasetComponent.prototype.SeclectMedia = function (item) {
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
            //for (var kk = 0; kk < this.Item.listAttachment.length; kk++) {
            //  if (this.Item.listAttachment[kk].Url == attachment.Url) {
            //    checkDuplicate = true;
            //    break;
            //  }
            //}
            //if (!checkDuplicate)
            //  this.Item.listAttachment.push(attachment);
            //else {
            //  this.toastWarning("Bạn đã chọn ảnh này!");
            //}
        }
        this.OpenMediaFile.hide();
    };
    DatasetComponent.prototype.ViewFile = function (id) {
        var _this = this;
        this.IdFile = id;
        this.http.get('/api/S3File/viewFile/' + id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.fileView = res["data"];
                _this.dataFile = 'data:image/png;base64,' + _this.fileView.Note;
                _this.pdfBase64 = _this.sanitizer.bypassSecurityTrustResourceUrl('data:application/pdf;base64,' + _this.fileView.Note);
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    DatasetComponent.prototype.DownloadFileRar = function (id) {
        var _this = this;
        this.http.get('/api/S3File/downloadFiles/' + id + '/-1', this.httpOptionsBlob).subscribe(function (res) {
            //console.log(res);
            var url = window.URL.createObjectURL(res["body"]);
            var a = document.createElement('a');
            document.body.appendChild(a);
            a.setAttribute('style', 'display: none');
            a.href = url;
            a.download = "dataset.rar";
            a.click();
            window.URL.revokeObjectURL(url);
            a.remove();
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    DatasetComponent.prototype.DownloadFile = function (item) {
        var _this = this;
        this.http.get('/api/S3File/downloadOneFile/-1/-1/' + item.AttactmentId, this.httpOptionsBlob).subscribe(function (res) {
            //console.log(res);
            var url = window.URL.createObjectURL(res["body"]);
            var a = document.createElement('a');
            document.body.appendChild(a);
            a.setAttribute('style', 'display: none');
            a.href = url;
            a.download = item.Name;
            a.click();
            window.URL.revokeObjectURL(url);
            a.remove();
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
        //const url = '/api/download/downloadOneFile/' + id;
        //const options = new RequestOptions({
        //  headers: new Headers({
        //    'Authorization': 'bearer ' + localStorage.getItem("access_token")
        //  }),
        //  responseType: ResponseContentType.Blob
        //});
        //this.httpDownload.get(url, options).subscribe(res => {
        //  const fileName = getFileNameFromResponseContentDisposition(res);
        //  saveFile(res.blob(), fileName);
        //});
    };
    DatasetComponent.prototype.onKeydownHandler = function (event) {
        if (event.key === "Escape") {
            this.DetailCategory.hide();
            this.ViewModal.hide();
        }
    };
    __decorate([
        core_1.ViewChild('DataModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], DatasetComponent.prototype, "DataModal", void 0);
    __decorate([
        core_1.ViewChild('ViewModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], DatasetComponent.prototype, "ViewModal", void 0);
    __decorate([
        core_1.ViewChild('ConfirmModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], DatasetComponent.prototype, "ConfirmModal", void 0);
    __decorate([
        core_1.ViewChild('DetailCategory'),
        __metadata("design:type", modal_1.ModalDirective)
    ], DatasetComponent.prototype, "DetailCategory", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], DatasetComponent.prototype, "file", void 0);
    __decorate([
        core_1.ViewChild('attachment'),
        __metadata("design:type", core_1.ElementRef)
    ], DatasetComponent.prototype, "attachment", void 0);
    __decorate([
        core_1.ViewChild('tabset'),
        __metadata("design:type", tabs_1.TabsetComponent)
    ], DatasetComponent.prototype, "tabset", void 0);
    __decorate([
        core_1.ViewChild('filevideo'),
        __metadata("design:type", core_1.ElementRef)
    ], DatasetComponent.prototype, "filevideo", void 0);
    __decorate([
        core_1.ViewChild('OpenMediaFile'),
        __metadata("design:type", modal_1.ModalDirective)
    ], DatasetComponent.prototype, "OpenMediaFile", void 0);
    __decorate([
        core_1.ViewChild('OpenMediaFileVideo'),
        __metadata("design:type", modal_1.ModalDirective)
    ], DatasetComponent.prototype, "OpenMediaFileVideo", void 0);
    __decorate([
        core_1.HostListener('document:keydown', ['$event']),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [KeyboardEvent]),
        __metadata("design:returntype", void 0)
    ], DatasetComponent.prototype, "onKeydownHandler", null);
    DatasetComponent = __decorate([
        core_1.Component({
            selector: 'app-dataset',
            templateUrl: './dataset.component.html',
            styleUrls: ['./dataset.component.scss']
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_1.DatePipe,
            common_service_1.CommonService,
            platform_browser_1.DomSanitizer])
    ], DatasetComponent);
    return DatasetComponent;
}());
exports.DatasetComponent = DatasetComponent;
//# sourceMappingURL=dataset.component.js.map