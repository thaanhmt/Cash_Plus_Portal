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
exports.LogComponent = exports.MY_CUSTOM_FORMATS = void 0;
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
var LogComponent = /** @class */ (function () {
    function LogComponent(http, modalDialogService, viewRef, toastr, datePipe, common) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.datePipe = datePipe;
        this.common = common;
        this.listLog = [];
        this.linkNews = [];
        this.linkCatNews = [];
        this.listOrderByProduct = [];
        this.listSuggestProduct = [];
        this.listNewsT = [];
        this.listCateNews = [];
        this.listSuggestNews = [];
        this.listLanguage = [];
        this.listLanguageTemp = [];
        this.listAuthor = [];
        this.listNumberOfTopic = [];
        this.listDepartment = [];
        this.listNewsNote = [];
        this.isNoitify = false;
        this.listTypeNews = const_1.typeCategoryNews;
        this.domainImage = const_1.domainImage;
        this.ActionTable = const_1.ActionTable;
        this.Status = const_1.Status;
        this.listHotNews = const_1.listHotNews;
        this.tags = [];
        this.activeD = false;
        this.activeU = false;
        this.isNumberOfTopic = false;
        this.isAuthor = false;
        this.isDepartment = false;
        this.RoleCode = localStorage.getItem("roleCode") || '';
        this.NameAuthor = localStorage.getItem("fullName") || '';
        this.Item = new model_1.Publication();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "ActionId Desc";
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
        this.Tag = new model_1.Tag();
        this.CheckRole = new dt_1.CheckRole();
        var code = "QLAP";
        this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 0);
        this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 1);
        this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 2);
        this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 3);
    }
    LogComponent.prototype.ngOnInit = function () {
        this.RoleCode = localStorage.getItem("roleCode");
        this.NameAuthor = localStorage.getItem("fullName");
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.paging.query = "1=1 AND (Status=1 OR Status= 18 OR Status=19)";
        this.domain = const_1.domain;
        this.GetListPublication();
        this.GetListLanguage();
        this.GetListNumberOfTopic();
        this.GetListAuthor();
        this.GetListDepartment();
    };
    //Get danh sách log
    LogComponent.prototype.GetListPublication = function () {
        var _this = this;
        this.http.get('/api/Action/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listLog = res["data"];
                _this.paging.item_count = res["metadata"].Sum;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    // Get danh sách ngôn ngữ
    LogComponent.prototype.GetListLanguage = function () {
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
    LogComponent.prototype.GetListNumberOfTopic = function () {
        var _this = this;
        this.http.get('/api/TypeAttributeItem/GetByPage?page=1&query=TypeAttributeId=25&order_by=Location Asc', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listNumberOfTopic = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    LogComponent.prototype.GetListAuthor = function () {
        var _this = this;
        this.http.get('/api/Author/GetByPageUser', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listAuthor = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    LogComponent.prototype.GetListDepartment = function () {
        var _this = this;
        this.http.get('/api/TypeAttributeItem/GetByPage?page=1&query=TypeAttributeId=26&order_by=Location Asc', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listDepartment = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    LogComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListPublication();
    };
    //Thông báo
    LogComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    LogComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    LogComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    LogComponent.prototype.QueryChanged = function () {
        var query = "1=1";
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            query += ' and ActionName.Contains("' + this.q.txtSearch + '")';
        }
        if (this.q.UserId != undefined) {
            query += ' and UserId=' + this.q.UserId;
        }
        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;
        this.GetListPublication();
    };
    //
    LogComponent.prototype.StatusChanged = function (status) {
        var query = "1=1";
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            query += ' and Title.Contains("' + this.q.txtSearch + '")';
        }
        if (this.q.LanguageId != undefined) {
            query += ' and LanguageId=' + this.q.LanguageId;
        }
        query += ' and Status=' + status;
        this.paging.query = query;
        this.GetListPublication();
    };
    //Mở modal thêm mới
    LogComponent.prototype.OpenNewsModal = function (item, type) {
        this.Item = new model_1.Publication();
        this.Item.Contents = "";
        this.listLanguageTemp = this.listLanguage;
        this.Item.LanguageId = this.languageId;
        this.Item.Location = this.paging.item_count + 1;
        this.Item.ViewNumber = 1;
        this.IsAll = true;
        if (this.file)
            this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
        this.progressAttachment = undefined;
        this.CheckBoxStatus = true;
        if (item != undefined) {
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
                this.Item.PublicationId = undefined;
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
        this.NewsModal.show();
    };
    //Thêm mới danh mục trang
    LogComponent.prototype.SaveNews = function (status) {
        var _this = this;
        if (this.Item.Title == undefined || this.Item.Title == '') {
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
        this.Item.Status = status;
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
        //this.listSuggestNews.forEach(item => {
        //  if (item.Check == true) {
        //    let it = { TargetRelatedId: item.NewsId }
        //  }
        //});
        //this.listSuggestProduct.forEach(item => {
        //  if (item.Check == true) {
        //    let it = { TargetRelatedId: item.ProductId }
        //  }
        //});
        if (this.Item.PublicationId == undefined) {
            this.http.post('/api/publication', obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListPublication();
                    _this.NewsModal.hide();
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
            this.http.put('/api/publication/' + obj.PublicationId, obj, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListPublication();
                    _this.NewsModal.hide();
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
    LogComponent.prototype.AddType = function (type) {
        if (type == 1) {
            this.isNumberOfTopic = true;
            this.ItemType = new model_1.TypeAttributeItem();
        }
        else if (type == 2) {
            this.isAuthor = true;
            this.ItemAuthor = new model_1.Author();
        }
        else if (type == 3) {
            this.isDepartment = true;
            this.ItemType = new model_1.TypeAttributeItem();
        }
    };
    LogComponent.prototype.SaveNumberOfTopic = function () {
        var _this = this;
        if (this.Item.NumberOfTopicText == undefined || this.Item.Title == '') {
            this.toastWarning("Chưa nhập số chuyên đề!");
            return;
        }
        else if (this.Item.NumberOfTopicText.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập số chuyên đề!");
            return;
        }
        this.ItemType.Name = this.Item.NumberOfTopicText;
        this.ItemType.TypeAttributeId = 25;
        this.ItemType.UserId = parseInt(localStorage.getItem("userId"));
        var obj = Object.assign({}, this.ItemType);
        this.http.post('/api/TypeAttributeItem', obj, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListNumberOfTopic();
                _this.isNumberOfTopic = false;
                _this.toastSuccess("Cập nhật thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    LogComponent.prototype.SaveAuthor = function () {
        var _this = this;
        if (this.Item.AuthorText == undefined || this.Item.AuthorText == '') {
            this.toastWarning("Chưa nhập tác giả!");
            return;
        }
        else if (this.Item.AuthorText.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tác giả!");
            return;
        }
        this.ItemAuthor.Name = this.Item.AuthorText;
        this.ItemAuthor.Type = 2;
        this.ItemAuthor.UserId = parseInt(localStorage.getItem("userId"));
        var obj = Object.assign({}, this.ItemAuthor);
        this.http.post('/api/author', obj, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListAuthor();
                _this.isAuthor = false;
                _this.toastSuccess("Cập nhật thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    LogComponent.prototype.SaveDepartment = function () {
        var _this = this;
        if (this.Item.DepartmentText == undefined || this.Item.Title == '') {
            this.toastWarning("Chưa nhập cơ quan ban ngành!");
            return;
        }
        else if (this.Item.DepartmentText.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập cơ quan ban ngành!");
            return;
        }
        this.ItemType.Name = this.Item.DepartmentText;
        this.ItemType.TypeAttributeId = 26;
        this.ItemType.UserId = parseInt(localStorage.getItem("userId"));
        var obj = Object.assign({}, this.ItemType);
        this.http.post('/api/TypeAttributeItem', obj, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListDepartment();
                _this.isDepartment = false;
                _this.toastSuccess("Cập nhật thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    LogComponent.prototype.ChangeTitle = function (key) {
        if (this.Item.PublicationId == undefined) {
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
    LogComponent.prototype.ShowConfirmDelete = function (Id) {
        var _this = this;
        this.modalDialogService.openDialog(this.viewRef, {
            title: 'Xác nhận',
            childComponent: ngx_modal_dialog_1.SimpleModalComponent,
            data: {
                text: "Bài viết sẽ được đưa vào thùng rác?"
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
    LogComponent.prototype.DeleteNews = function (Id) {
        var _this = this;
        if (this.RoleCode == 'ADMIN') {
            this.http.delete('/api/Action/' + Id, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListPublication();
                    _this.viewRef.clear();
                    _this.toastSuccess("Xóa thành công!");
                }
                else {
                    _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            this.toastError("Bạn không có quyền xoá bài viết này");
        }
    };
    LogComponent.prototype.upload2 = function (files) {
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
    };
    LogComponent.prototype.RemoveUpFile = function () {
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
    };
    LogComponent.prototype.upload = function (files, cs) {
        var _this = this;
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_2 = files; _i < files_2.length; _i++) {
            var file = files_2[_i];
            formData.append(file.name, file);
        }
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload/uploadImage/1', formData, {
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
                        });
                        break;
                    default:
                        break;
                }
                /* console.log(this.Item.listAttachment);*/
            }
        });
    };
    LogComponent.prototype.SortTable = function (str) {
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
        this.GetListPublication();
    };
    LogComponent.prototype.GetClassSortTable = function (str) {
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
    LogComponent.prototype.CheckActionTable = function (Id) {
        if (Id == undefined) {
            var CheckAll_1 = this.CheckAll;
            this.listLog.forEach(function (item) {
                item.Action = CheckAll_1;
            });
        }
        else {
            var CheckAll = true;
            for (var i = 0; i < this.listLog.length; i++) {
                if (!this.listLog[i].Action) {
                    CheckAll = false;
                    break;
                }
            }
            this.CheckAll = CheckAll == true ? true : false;
        }
    };
    LogComponent.prototype.ActionTableFunc = function () {
        var _this = this;
        if (this.RoleCode == 'ADMIN') {
            switch (this.ActionId) {
                case 1:
                    var data_1 = [];
                    this.listLog.forEach(function (item) {
                        if (item.Action == true) {
                            data_1.push(item.ActionId);
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
                                        _this.http.put('/api/action/DeleteMultiAction', data_1, _this.httpOptions).subscribe(function (res) {
                                            if (res["meta"]["error_code"] == 200) {
                                                _this.toastSuccess("Xóa thành công!");
                                                _this.GetListPublication();
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
    LogComponent.prototype.ChangeLinkDetailNews = function (TypeNewsId, Url, CategoryId, NewsId) {
        var Link = '';
        var Idw = 1;
        switch (TypeNewsId) {
            case 1:
                Link = "/" + this.listTypeNews.filter(function (x) { return x.Id == TypeNewsId; })[0].ConstUrl + "/" + Url + "-" + CategoryId + "-" + NewsId + ".html";
                break;
            default:
                Link = "/" + this.listTypeNews.filter(function (x) { return x.Id == TypeNewsId; })[0].ConstUrl + "/" + Url + "-" + Idw + "-" + NewsId + ".html";
                break;
        }
        //console.log(Link);
        return Link;
    };
    LogComponent.prototype.OpenModalTag = function () {
        this.Tag = new model_1.Tag();
        this.TagModal.show();
    };
    LogComponent.prototype.closeNoityfy = function () {
        this.isNoitify = true;
    };
    __decorate([
        core_1.ViewChild('NewsModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], LogComponent.prototype, "NewsModal", void 0);
    __decorate([
        core_1.ViewChild('HighlightNewsModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], LogComponent.prototype, "HighlightNewsModal", void 0);
    __decorate([
        core_1.ViewChild('TagModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], LogComponent.prototype, "TagModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], LogComponent.prototype, "file", void 0);
    __decorate([
        core_1.ViewChild('attachment'),
        __metadata("design:type", core_1.ElementRef)
    ], LogComponent.prototype, "attachment", void 0);
    __decorate([
        core_1.ViewChild('tabset'),
        __metadata("design:type", tabs_1.TabsetComponent)
    ], LogComponent.prototype, "tabset", void 0);
    LogComponent = __decorate([
        core_1.Component({
            selector: 'app-log',
            templateUrl: './log.component.html',
            styleUrls: ['./log.component.scss'],
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
    ], LogComponent);
    return LogComponent;
}());
exports.LogComponent = LogComponent;
//# sourceMappingURL=log.component.js.map