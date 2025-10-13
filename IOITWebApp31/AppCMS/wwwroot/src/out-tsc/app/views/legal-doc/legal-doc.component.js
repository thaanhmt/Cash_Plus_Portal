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
exports.LegalDocComponent = exports.MY_CUSTOM_FORMATS = void 0;
var core_1 = require("@angular/core");
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var const_1 = require("../../data/const");
var model_1 = require("../../data/model");
var ngx_toastr_1 = require("ngx-toastr");
var dt_1 = require("../../data/dt");
var ng_pick_datetime_1 = require("ng-pick-datetime");
var ng_pick_datetime_moment_1 = require("ng-pick-datetime-moment");
var common_service_1 = require("../../service/common.service");
exports.MY_CUSTOM_FORMATS = {
    parseInput: 'DD/MM/YYYY HH:mm',
    fullPickerInput: 'DD/MM/YYYY HH:mm',
    datePickerInput: 'DD/MM/YYYY',
    timePickerInput: ' HH:mm',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY'
};
var LegalDocComponent = /** @class */ (function () {
    function LegalDocComponent(http, modalDialogService, viewRef, toastr, common) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.common = common;
        this.isActiveMedia = true;
        this.isActiveUpload = false;
        this.isDelay = false;
        this.listItemMedia = [];
        this.domainMedia = const_1.domainMedia;
        this.domain = const_1.domain;
        this.listLegalDoc = [];
        this.listNewsT = [];
        this.listLanguage = [];
        this.listLanguageTemp = [];
        this.domainImage = const_1.domainImage;
        this.domainFile = const_1.domainFile;
        this.ActionTable = const_1.ActionTable;
        this.listId = [];
        this.page_pp = [];
        this.listCateNews = [];
        this.listTypeAttributeItem = [];
        this.isNoitify = false;
        this.domainDebug = const_1.domainDebug;
        this.Item = new model_1.LegalDoc();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "LegalDocId Desc";
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
        this.languageId = localStorage.getItem("languageId") != undefined ? parseInt(localStorage.getItem("languageId")) : 1;
        //this.paging.query = "LanguageId=" + this.languageId;
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
        this.CheckRole = new dt_1.CheckRole();
        var code = "DSCH";
        this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 0);
        this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 1);
        this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 2);
        this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 3);
    }
    LegalDocComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListLegalDoc();
        this.GetListLanguage();
        this.GetListCateNews();
        this.GetListTypeAttributeItem();
        this.GetListFiles();
        this.GetDomainStatic();
    };
    LegalDocComponent.prototype.GetDomainStatic = function () {
        var _this = this;
        this.http.get('api/Config/1', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
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
    //Get danh sách typeAttributeItem phong ban
    LegalDocComponent.prototype.GetListTypeAttributeItem = function () {
        var _this = this;
        this.http.get('/api/TypeAttributeItem/GetByPage?page=1&query=TypeAttributeId=4&order_by=TypeAttributeItemId Asc', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listTypeAttributeItem = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    LegalDocComponent.prototype.GetListCateNews = function () {
        var _this = this;
        this.Item.LanguageId = this.Item.LanguageId ? this.Item.LanguageId : 1;
        this.http.get('/api/category/GetByTree?arr=12&langId=' + this.Item.LanguageId, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCateNews = res["data"];
                if (_this.Item.LegalDocId != undefined) {
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
    LegalDocComponent.prototype.GetListLegalDoc = function () {
        var _this = this;
        this.http.get('/api/LegalDoc/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listLegalDoc = res["data"];
                _this.paging.item_count = res["metadata"];
                for (var i = 0; i < _this.listLegalDoc.length; i++) {
                    _this.listLegalDoc[i].IsCheck = false;
                }
                for (var i = 0; i < _this.paging.item_count; i++) {
                    _this.page_pp.push(i);
                }
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    // Get danh sách ngôn ngữ
    LegalDocComponent.prototype.GetListLanguage = function () {
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
    LegalDocComponent.prototype.GetTranslate = function (id) {
        var _this = this;
        var sl = this.Item.LanguageRootCode;
        var tl = this.Item.LanguageCode;
        this.ItemTranslate = new model_1.LegalDoc();
        this.http.get('/api/translate/' + id + '/' + sl + '/' + tl + '/4', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.ItemTranslate = res["data"];
                _this.Item.LegalDocId = undefined;
                _this.Item.Name = _this.ItemTranslate.Name;
                //this.Item.Url = this.ItemTranslate.Url;
                //this.Item.Description = this.ItemTranslate.Description;
                _this.Item.Contents = _this.ItemTranslate.Contents;
                _this.Item.Note = _this.ItemTranslate.Note;
                _this.Item.TichYeu = _this.ItemTranslate.TichYeu;
                //this.Item.MetaKeyword = this.ItemTranslate.MetaKeyword;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    LegalDocComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListLegalDoc();
    };
    //Toast cảnh báo
    LegalDocComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    //Toast thành công
    LegalDocComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    //Toast thành công
    LegalDocComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //Search
    LegalDocComponent.prototype.QueryChanged = function () {
        var query = '1=1';
        if (this.q.LanguageId != undefined) {
            query = 'LanguageId=' + this.q.LanguageId;
        }
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
        this.GetListLegalDoc();
    };
    //Mở modal thêm mới
    LegalDocComponent.prototype.OpenLegalDocModal = function (item, type) {
        this.typeAction = type;
        this.Item = new model_1.LegalDoc();
        this.listLanguageTemp = this.listLanguage;
        this.Item.LanguageId = this.languageId;
        //if (this.file) this.file.nativeElement.value = "";
        //this.message = undefined;
        //this.progress = undefined;
        this.Item.listCategory = [];
        if (item != undefined) {
            this.Item = JSON.parse(JSON.stringify(item));
            if (type == 1 || type == 3) {
                this.Item.AgencyIssued = +this.Item.AgencyIssued;
                //this.CheckBoxStatus = this.Item.Status == 1 ? true : false;
            }
            else if (type == 2) {
                if (this.listLanguage.length == item.listLanguage.length + 1) {
                    this.toastWarning("Bạn đã thêm đủ ngôn ngữ!");
                    return;
                }
                this.listLanguageTemp = [];
                this.Item.LegalDocId = undefined;
                this.Item.LegalDocRootId = item.LegalDocId;
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
                //this.GetTranslate(this.Item.LegalDocRootId);
            }
        }
        //let listName = this.Item.Attactment.split('/');
        //this.Item.AttactmentName = listName[listName.length - 1];
        this.GetListCateNews();
        this.LegalDocModal.show();
    };
    //Thêm mới khách hàng
    LegalDocComponent.prototype.SaveLegalDoc = function () {
        var _this = this;
        //if (this.Item.Code == undefined || this.Item.Code == '') {
        //  this.toastWarning("Chưa nhập mã!");
        //  return;
        //} else if (this.Item.Code.replace(/ /g, '') == '') {
        //  this.toastWarning("Chưa nhập mã!");
        //  return;
        //} else
        if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập tên câu hỏi!");
            return;
        }
        else if (this.Item.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên câu hỏi!");
            return;
        }
        //  else if (this.Item.AgencyIssued == undefined) {
        //  this.toastWarning("Chưa nhập cơ quan ban hành !");
        //  return;
        //} else if (this.Item.DateIssue == undefined || this.Item.DateIssue == '') {
        //  this.toastWarning("Chưa nhập ngày ban hành!");
        //  return;
        //} else if (this.Item.DateEffect == undefined || this.Item.DateEffect == '') {
        //  this.toastWarning("Chưa nhập ngày hợp lực!");
        //  return;
        //} else if (this.Item.Signer == undefined || this.Item.Signer == '') {
        //  this.toastWarning("Chưa nhập người ký");
        //  return;
        //} else if (this.Item.Attactment == undefined || this.Item.Attactment == '') {
        //  this.toastWarning("Chưa nhập file đính kèm");
        //  return;
        //  } 
        else if (this.Item.Contents == undefined || this.Item.Contents == '') {
            this.toastWarning("Chưa nhập nội dung");
            return;
        }
        //else if (this.Item.Note == undefined || this.Item.Note == '') {
        //  this.toastWarning("Chưa nhập ghi chú");
        //  return;
        //}
        //  else if (this.Item.TichYeu == undefined || this.Item.TichYeu == '') {
        //  this.toastWarning("Chưa nhập trích yếu");
        //  return;
        //} else if (this.Item.YearIssue == undefined || this.Item.YearIssue == null) {
        //  this.toastWarning("Chưa nhập năm ban hành");
        //  return;
        //}
        //if (typeof this.Item.DateIssue === 'object' && this.Item.DateIssue != undefined) {
        //  let DateIssue = this.Item.DateIssue.add(7, 'hours');
        //  this.Item.DateIssue = DateIssue.toISOString();
        //}
        //if (typeof this.Item.DateEffect === 'object' && this.Item.DateEffect != undefined) {
        //  let DateEffect = this.Item.DateEffect.add(7, 'hours');
        //  this.Item.DateEffect = DateEffect.toISOString();
        //}
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        var obj = JSON.parse(JSON.stringify(this.Item));
        if (this.Item.LegalDocId == undefined) {
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
            this.http.post('/api/LegalDoc', obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListLegalDoc();
                    _this.LegalDocModal.hide();
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
            this.http.put('/api/LegalDoc/' + this.Item.LegalDocId, obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListLegalDoc();
                    _this.LegalDocModal.hide();
                    _this.toastSuccess(res["meta"]["error_message"]);
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
    LegalDocComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        _this.DeleteLegalDoc(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-danger',
                }
            ],
        });
    };
    LegalDocComponent.prototype.DeleteLegalDoc = function (Id) {
        var _this = this;
        this.http.delete('/api/LegalDoc/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListLegalDoc();
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
    //upload(files) {
    //  if (files.length === 0)
    //    return;
    //  const formData = new FormData();
    //  for (let file of files)
    //    formData.append(file.name, file);
    //  console.log(formData);
    //  const uploadReq = new HttpRequest('POST', 'api/upload/uploadImage/6', formData, {
    //    headers: new HttpHeaders({
    //      'Authorization': 'bearer ' + localStorage.getItem("access_token")
    //    }),
    //    reportProgress: true,
    //  });
    //  this.http.request(uploadReq).subscribe(event => {
    //    if (event.type === HttpEventType.UploadProgress)
    //      this.progress = Math.round(100 * event.loaded / event.total);
    //    else if (event.type === HttpEventType.Response) {
    //      this.Item.Attactment = event.body["data"].toString();
    //    }
    //  });
    //}
    //
    LegalDocComponent.prototype.RemoveImage = function () {
        this.Item.Attactment = undefined;
        this.file.nativeElement.value = "";
        this.progress = undefined;
    };
    LegalDocComponent.prototype.SortTable = function (str) {
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
        this.GetListLegalDoc();
    };
    LegalDocComponent.prototype.GetClassSortTable = function (str) {
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
    LegalDocComponent.prototype.CheckActionTable = function (LegalDocId) {
        if (LegalDocId == undefined) {
            var CheckAll_1 = this.CheckAll;
            this.listLegalDoc.forEach(function (item) {
                item.Action = CheckAll_1;
            });
        }
        else {
            var CheckAll = true;
            for (var i = 0; i < this.listLegalDoc.length; i++) {
                if (!this.listLegalDoc[i].Action) {
                    CheckAll = false;
                    break;
                }
            }
            this.CheckAll = CheckAll == true ? true : false;
        }
    };
    LegalDocComponent.prototype.upload = function (files) {
        var _this = this;
        if (files.length === 0)
            return;
        console.log(files[0]);
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
            formData.append(file.name, file);
        }
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload/uploadFileVbpq', formData, {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
        this.http.request(uploadReq).subscribe(function (event) {
            if (event.type === http_1.HttpEventType.UploadProgress)
                _this.progress = Math.round(100 * event.loaded / event.total);
            else if (event.type === http_1.HttpEventType.Response) {
                console.log(event.body);
                _this.Item.Attactment = event.body["data"][0].LinkFile;
                _this.Item.AttactmentBit = event.body["data"][0].DataByte;
                _this.Item.Extension = event.body["data"][0].Extension;
            }
        });
    };
    // Chon hoac bo chon tat ca van ban
    LegalDocComponent.prototype.CheckAllAddress = function () {
        this.listId = [];
        console.log(this.CheckitemAll);
        if (this.CheckitemAll == true) {
            for (var i = 0; i < this.listLegalDoc.length; i++) {
                this.listLegalDoc[i].IsCheck = false;
            }
        }
        else {
            for (var i = 0; i < this.listLegalDoc.length; i++) {
                this.listLegalDoc[i].IsCheck = true;
            }
        }
        for (var i = 0; i < this.listLegalDoc.length; i++) {
            if (this.listLegalDoc[i].IsCheck == true) {
                this.listId.push(this.listLegalDoc[i].LegalDocId);
            }
        }
        console.log(this.listLegalDoc);
        console.log(this.listId);
    };
    //Chon binh luan xoa
    LegalDocComponent.prototype.CheckDeleteAddress = function (item) {
        console.log(item.IsCheck);
        this.listId = [];
        if (item.IsCheck == false || item.IsCheck == undefined) {
            for (var i = 0; i < this.listLegalDoc.length; i++) {
                if (this.listLegalDoc[i].LegalDocId == item.LegalDocId) {
                    this.listLegalDoc[i].IsCheck = false;
                }
            }
        }
        else {
            for (var i = 0; i < this.listLegalDoc.length; i++) {
                if (this.listLegalDoc[i].LegalDocId == item.LegalDocId) {
                    this.listLegalDoc[i].IsCheck = true;
                }
            }
        }
        for (var i = 0; i < this.listLegalDoc.length; i++) {
            if (this.listLegalDoc[i].IsCheck == true) {
                this.listId.push(this.listLegalDoc[i].LegalDocId);
            }
        }
        if (this.listId.length < this.listLegalDoc.length) {
            this.CheckitemAll = false;
        }
        if (this.listId.length == this.listLegalDoc.length) {
            this.CheckitemAll = true;
        }
        console.log(this.listId);
    };
    //Xoa nhieu
    LegalDocComponent.prototype.DeleteMuntiAddress = function () {
        var _this = this;
        console.log(this.listId.length);
        if (this.listId.length > 0) {
            this.http.put('/api/LegalDoc/deletes', this.listId, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListLegalDoc();
                    _this.viewRef.clear();
                    _this.toastSuccess("Xóa thành công!");
                    _this.listId = [];
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            this.toastError("Chưa có văn bản nào được chọn !");
        }
    };
    LegalDocComponent.prototype.CheckCategory = function (CategoryId, curItem) {
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
    LegalDocComponent.prototype.closeNoityfy = function () {
        this.isNoitify = true;
    };
    LegalDocComponent.prototype.ChangeTitle = function (key) {
        if (this.Item.LegalDocId == undefined) {
            switch (key) {
                case 1:
                    this.Item.Url = this.common.ConvertUrl(this.Item.Name);
                    break;
                case 2:
                    break;
                default:
                    break;
            }
        }
    };
    LegalDocComponent.prototype.OpenMediaModal = function () {
        this.OpenMediaFile.show();
    };
    LegalDocComponent.prototype.CloseMediaModal = function () {
        this.OpenMediaFile.hide();
    };
    LegalDocComponent.prototype.tabHandleMedia = function () {
        this.isActiveMedia = true;
        this.isActiveUpload = false;
    };
    LegalDocComponent.prototype.tabHandleMediaUpload = function () {
        this.isActiveMedia = false;
        this.isActiveUpload = true;
    };
    LegalDocComponent.prototype.upload3 = function (files, cs) {
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
    LegalDocComponent.prototype.RemoveFile = function () {
        this.Item.Attactment = undefined;
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
    };
    LegalDocComponent.prototype.loadMore = function () {
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
    LegalDocComponent.prototype.GetListFiles = function () {
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
    LegalDocComponent.prototype.SeclectMedia = function (item) {
        var _this = this;
        //if (item.extension)
        var obj = {
            "LinkFile": item.url + "/" + item.name,
            "Extension": item.extension,
        };
        this.http.post('/api/upload/convertFileToByte', obj, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.Item.Attactment = item.url + "/" + item.name;
                _this.Item.AttactmentName = item.name;
                _this.Item.AttactmentBit = res["data"]["DataByte"];
                _this.Item.Extension = item.extension;
                console.log(_this.Item);
                _this.OpenMediaFile.hide();
            }
            else {
                _this.toastError(res["meta"]["error_message"]);
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    __decorate([
        core_1.ViewChild('LegalDocModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], LegalDocComponent.prototype, "LegalDocModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], LegalDocComponent.prototype, "file", void 0);
    __decorate([
        core_1.ViewChild('OpenMediaFile'),
        __metadata("design:type", modal_1.ModalDirective)
    ], LegalDocComponent.prototype, "OpenMediaFile", void 0);
    LegalDocComponent = __decorate([
        core_1.Component({
            selector: 'app-legal-doc',
            templateUrl: './legal-doc.component.html',
            styleUrls: ['./legal-doc.component.scss'],
            providers: [
                { provide: ng_pick_datetime_1.DateTimeAdapter, useClass: ng_pick_datetime_moment_1.MomentDateTimeAdapter, deps: [ng_pick_datetime_1.OWL_DATE_TIME_LOCALE] },
                { provide: ng_pick_datetime_1.OWL_DATE_TIME_FORMATS, useValue: exports.MY_CUSTOM_FORMATS }
            ]
        }),
        __metadata("design:paramtypes", [http_1.HttpClient,
            ngx_modal_dialog_1.ModalDialogService,
            core_1.ViewContainerRef,
            ngx_toastr_1.ToastrService,
            common_service_1.CommonService])
    ], LegalDocComponent);
    return LegalDocComponent;
}());
exports.LegalDocComponent = LegalDocComponent;
//# sourceMappingURL=legal-doc.component.js.map