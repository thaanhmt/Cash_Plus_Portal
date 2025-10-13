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
exports.UserDataComponent = exports.MY_CUSTOM_FORMATS = void 0;
var core_1 = require("@angular/core");
var modal_1 = require("ngx-bootstrap/modal");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var model_1 = require("../../../data/model");
var ngx_toastr_1 = require("ngx-toastr");
var common_1 = require("@angular/common");
var common_service_1 = require("../../../service/common.service");
var const_1 = require("../../../data/const");
var dt_1 = require("../../../data/dt");
var ng_pick_datetime_1 = require("ng-pick-datetime");
var ng_pick_datetime_moment_1 = require("ng-pick-datetime-moment");
exports.MY_CUSTOM_FORMATS = {
    parseInput: 'DD/MM/YYYY HH:mm',
    fullPickerInput: 'DD/MM/YYYY HH:mm',
    datePickerInput: 'DD/MM/YYYY',
    timePickerInput: ' HH:mm',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY'
};
var UserDataComponent = /** @class */ (function () {
    function UserDataComponent(http, modalDialogService, viewRef, toastr, datePipe, common) {
        this.http = http;
        this.modalDialogService = modalDialogService;
        this.viewRef = viewRef;
        this.toastr = toastr;
        this.datePipe = datePipe;
        this.common = common;
        this.isActiveMedia = true;
        this.isActiveUpload = false;
        this.isDelay = false;
        this.domain = const_1.domain;
        this.listItemMedia = [];
        this.domainMedia = const_1.domainMedia;
        this.listUser = [];
        this.listChucDanh = [];
        this.listHocHam = [];
        this.listHocVi = [];
        this.listResearchArea = [];
        this.listUnit = [];
        this.listRole = [];
        this.listCountry = [];
        this.listSexs = const_1.listSexs;
        this.listTypeId = const_1.listTypeId;
        this.listUserTypes = const_1.listUserTypes;
        this.listUserStatus = const_1.listUserStatus;
        this.domainImage = const_1.domainImage;
        this.TypeUserSy = const_1.TypeUserSy;
        this.ActionTable = const_1.ActionTable;
        this.isNoitify = false;
        this.domainDebug = const_1.domainDebug;
        this.Item = new model_1.Customer();
        this.Item1 = new model_1.Customer();
        this.paging = new dt_1.Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "";
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
        this.Action = {
            View: false,
            Create: false,
            Update: false,
            Delete: false,
            Import: false,
            Export: false,
            Print: false,
            Other: false,
            Menu: false,
        };
        //this.IsAll = true;
        this.httpOptions = {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        };
    }
    UserDataComponent.prototype.ngOnInit = function () {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListUser();
        this.GetListTypeAttributeItem(4);
        this.GetListTypeAttributeItem(25);
        this.GetListTypeAttributeItem(26);
        this.GetListResearchArea();
        this.GetListRole(1);
        this.GetListFiles();
        this.GetDomainStatic();
        this.GetListUnit();
        this.GetListCountry();
        this.CheckRole = new dt_1.CheckRole();
        var code = "QLTT";
        this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 0);
        this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 1);
        this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 2);
        this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 3);
        this.CheckRole.Export = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 5);
    };
    UserDataComponent.prototype.GetDomainStatic = function () {
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
    //Get danh sách danh user
    UserDataComponent.prototype.GetListUser = function () {
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
        this.http.post('/api/customer/GetByPagePost', data, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listUser = res["data"];
                //this.listUser.forEach(item => {
                //  item.IsShow = item.Status == 1 ? true : false;
                //});
                _this.paging.item_count = res["metadata"].Sum;
                //this.total = res["metadata"];
                //this.total1 = res["metadata"].Normal;
                //this.total2 = res["metadata"].Publishing;
                //this.total3 = res["metadata"].UnPublish;
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
        //this.http.get('/api/customer/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
        //  (res) => {
        //    if (res["meta"]["error_code"] == 200) {
        //      this.listUser = res["data"];
        //      this.paging.item_count = res["metadata"];
        //    }
        //  },
        //  (err) => {
        //    console.log("Error: connect to API");
        //  }
        //);
    };
    UserDataComponent.prototype.GetListTypeAttributeItem = function (id) {
        var _this = this;
        this.http.get('/api/TypeAttributeItem/GetByPage?page=1&query=TypeAttributeId=' + id + '&order_by=TypeAttributeItemId Asc', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                if (id == 4)
                    _this.listChucDanh = res["data"];
                else if (id == 25)
                    _this.listHocHam = res["data"];
                else if (id == 26)
                    _this.listHocVi = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    UserDataComponent.prototype.GetListResearchArea = function () {
        var _this = this;
        var type = 15;
        this.http.get('/api/category/GetByPage?page=1&query=TypeCategoryId=' + type + '&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listResearchArea = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    UserDataComponent.prototype.GetListUnit = function () {
        var _this = this;
        this.http.get('/api/unit/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listUnit = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    UserDataComponent.prototype.GetListCountry = function () {
        var _this = this;
        this.http.get('/api/country/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.listCountry = res["data"];
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    UserDataComponent.prototype.GetListRole = function (type) {
        var _this = this;
        //let arr = [];
        //if (this.Item.CustomerId) {
        //  arr = Object.assign(this.Item["listRole"]);
        //}
        var query = "Type==" + type;
        this.http.get('/api/role/GetByPage?page=1&query=' + query + '&order_by=', this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                //if (this.Item.CustomerId == undefined) {
                _this.listRole = res["data"];
                //}
                //else {
                //  this.listRole = res["data"];
                //  for (let i = 0; i < this.listRole.length; i++) {
                //    for (let j = 0; j < arr.length; j++) {
                //      if (this.listRole[i].RoleId == arr[j].RoleId) {
                //        this.listRole[i].Check = true;
                //        break;
                //      }
                //    }
                //  }
                //}
            }
        }, function (err) {
            console.log("Error: connect to API");
        });
    };
    //Chuyển trang
    UserDataComponent.prototype.PageChanged = function (event) {
        this.paging.page = event.page;
        this.GetListUser();
    };
    //Thông báo
    UserDataComponent.prototype.toastWarning = function (msg) {
        this.toastr.warning(msg, 'Cảnh báo');
    };
    UserDataComponent.prototype.toastSuccess = function (msg) {
        this.toastr.success(msg, 'Hoàn thành');
    };
    UserDataComponent.prototype.toastError = function (msg) {
        this.toastr.error(msg, 'Lỗi');
    };
    //
    UserDataComponent.prototype.QueryChanged = function () {
        var query = '1=1';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            query += ' and (FullName.Contains("' + this.q.txtSearch + '") Or Email.Contains("' + this.q.txtSearch + '") Or Phone.Contains("' + this.q.txtSearch + '"))';
        }
        if (this.q.Type != undefined) {
            query += ' and Type=' + this.q.Type;
        }
        if (this.q.Status != undefined) {
            query += ' and Status=' + this.q.Status;
        }
        if (this.q.UnitId != undefined) {
            query += ' and UnitId=' + this.q.UnitId;
        }
        if (this.q.RoleId != undefined) {
            query += ' and RoleId=' + this.q.RoleId;
        }
        this.paging.query = query;
        this.GetListUser();
    };
    //Mở modal thêm mới
    UserDataComponent.prototype.OpenAddModal = function () {
        this.Item = new model_1.Customer();
        this.Item.Type = "1";
        this.Item.Sex = 1;
        this.Item.CountryId = 1;
        this.Item.ListRoles = [];
        this.Item.ListResearchArea = [];
        this.Item.ListUnitManager = [];
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.userModal.show();
    };
    UserDataComponent.prototype.OpenAddModal2 = function () {
        this.Item = new model_1.Customer();
        this.GetListRole(1);
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.Action = {
            View: false,
            Create: false,
            Update: false,
            Delete: false,
            Import: false,
            Export: false,
            Print: false,
            Other: false,
            Menu: false,
        };
        this.UserModal1.show();
    };
    UserDataComponent.prototype.OpenResetModal = function (item) {
        this.Item = new model_1.Customer();
        this.Item = Object.assign(this.Item, item);
        this.ResetModal.show();
    };
    //Thêm mới người dùng
    UserDataComponent.prototype.AddUserFunc = function () {
        var _this = this;
        if (this.Item.Email == undefined || this.Item.Email == '') {
            this.toastWarning("Chưa nhập Email!");
            return;
        }
        else if (this.Item.Email.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập Email!");
            return;
        }
        else if (this.Item.UnitId == undefined && this.Item.Type == 1) {
            this.toastWarning("Chưa chọn Cơ quan/Tổ chức!");
            return;
        }
        else if (this.Item.FullName == undefined || this.Item.FullName == '') {
            this.toastWarning("Chưa nhập Họ và tên!");
            return;
        }
        else if (this.Item.FullName.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên người dùng!");
            return;
        }
        else if (this.Item.RoleId == undefined) {
            this.toastWarning("Chưa chọn nhóm quyền!");
            return;
        }
        var data = Object.assign({}, this.Item);
        if (data.Birthday != undefined) {
            var obj2 = new Date(data.Birthday);
            data.Birthday = obj2.getFullYear() + "-" + (obj2.getMonth() + 1) + "-" + obj2.getDate() + " " + obj2.getHours() + ":" + obj2.getMinutes() + ":0";
        }
        if (data.DateNumber != undefined) {
            var obj2 = new Date(data.DateNumber);
            data.DateNumber = obj2.getFullYear() + "-" + (obj2.getMonth() + 1) + "-" + obj2.getDate() + " " + obj2.getHours() + ":" + obj2.getMinutes() + ":0";
        }
        data.ListRoles = [];
        if (data.RoleId != undefined)
            data.ListRoles.push(data.RoleId);
        if (this.Item.CustomerId) {
            this.http.put('/api/customer/' + this.Item.CustomerId, data, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListUser();
                    _this.userModal.hide();
                    _this.toastSuccess("Cập nhật thành công!");
                }
                else {
                    _this.toastError(res["meta"]["error_message"]);
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
        else {
            this.http.post('/api/customer', data, this.httpOptions).subscribe(function (res) {
                if (res["meta"]["error_code"] == 200) {
                    _this.GetListUser();
                    _this.userModal.hide();
                    _this.toastSuccess("Thêm mới thành công!");
                }
                else {
                    _this.toastError(res["meta"]["error_message"]);
                }
            }, function (err) {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            });
        }
    };
    UserDataComponent.prototype.ResetPass = function () {
        var _this = this;
        if (this.Item.PasswordNew == undefined || this.Item.PasswordNew == '') {
            this.toastWarning("Chưa nhập mật khẩu mới!");
            return;
        }
        this.Item1.CustomerId = this.Item.CustomerId;
        this.Item1.PasswordNew = this.Item.PasswordNew;
        this.http.put('/api/User/adminChangePass/' + this.Item.CustomerId, this.Item1, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListUser();
                _this.ResetModal.hide();
                _this.toastSuccess("Thay đổi mật khẩu thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    UserDataComponent.prototype.LockUser = function (id, type) {
        var _this = this;
        this.http.put('api/customer/lockUser/' + id + '/' + type, undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListUser();
                _this.ResetModal.hide();
                if (type == 98)
                    _this.toastSuccess("Khóa tài khoản thành công!");
                else
                    _this.toastSuccess("Kích hoạt tài khoản thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    UserDataComponent.prototype.OpenUser = function (id) {
        var _this = this;
        this.http.put('api/User/lockUser/' + id + '/1', undefined, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListUser();
                _this.ResetModal.hide();
                _this.toastSuccess("Mở tài khoản thành công!");
            }
            else {
                _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            }
        }, function (err) {
            _this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    };
    UserDataComponent.prototype.OpenEditModal = function (item, type) {
        this.typeAction = type;
        this.Item = new model_1.Customer();
        this.Item = Object.assign(this.Item, item);
        if (this.Item.Type == 1)
            this.GetListRole(1);
        else
            this.GetListRole(3);
        this.Item.Type = this.Item.Type + "";
        if (this.Item.ListResearchArea == undefined)
            this.Item.ListResearchArea = [];
        if (this.Item.ListUnitManager == undefined)
            this.Item.ListUnitManager = [];
        if (this.Item.ListRoles != undefined)
            this.Item.RoleId = this.Item.ListRoles[0];
        //console.log(this.Item);
        this.file.nativeElement.value = "";
        //this.GetListRole();
        this.userModal.show();
    };
    //Popup xác nhận xóa
    UserDataComponent.prototype.ShowConfirmDelete = function (Id) {
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
                        _this.DeleteCustomer(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-danger',
                }
            ],
        });
    };
    UserDataComponent.prototype.DeleteCustomer = function (Id) {
        var _this = this;
        this.http.delete('/api/customer/' + Id, this.httpOptions).subscribe(function (res) {
            if (res["meta"]["error_code"] == 200) {
                _this.GetListUser();
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
    UserDataComponent.prototype.upload = function (files) {
        var _this = this;
        console.log(files);
        if (files.length === 0)
            return;
        var formData = new FormData();
        for (var _i = 0, files_1 = files; _i < files_1.length; _i++) {
            var file = files_1[_i];
            formData.append(file.name, file);
        }
        var uploadReq = new http_1.HttpRequest('POST', 'api/upload/uploadImage/6', formData, {
            headers: new http_1.HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
        this.http.request(uploadReq).subscribe(function (event) {
            if (event.type === http_1.HttpEventType.UploadProgress)
                _this.progress = Math.round(100 * event.loaded / event.total);
            else if (event.type === http_1.HttpEventType.Response) {
                _this.message = event.body["data"].toString();
                _this.Item.Avata = _this.message;
            }
        });
    };
    //changeAction(cs) {
    //  this.listFunc.forEach(item => {
    //    switch (cs) {
    //      case 1:
    //        item.View = this.Action.View;
    //        break;
    //      case 2:
    //        item.Create = this.Action.Create;
    //        break;
    //      case 3:
    //        item.Update = this.Action.Update;
    //        break;
    //      case 4:
    //        item.Delete = this.Action.Delete;
    //        break;
    //      case 5:
    //        item.Import = this.Action.Import;
    //        break;
    //      case 6:
    //        item.Export = this.Action.Export;
    //        break;
    //      case 7:
    //        item.Print = this.Action.Print;
    //        break;
    //      case 8:
    //        item.Other = this.Action.Other;
    //        break;
    //      case 9:
    //        item.Menu = this.Action.Menu;
    //        break;
    //      default:
    //        break;
    //    }
    //    if (item.View && item.Create && item.Update && item.Delete && item.Import && item.Export && item.Print && item.Other && item.Menu) {
    //      item.Full = true;
    //    }
    //    else {
    //      item.Full = false;
    //    }
    //  });
    //}
    //changeFull(i) {
    //  if (i != undefined) {
    //    this.listFunc[i].View = this.listFunc[i].Full;
    //    this.listFunc[i].Create = this.listFunc[i].Full;
    //    this.listFunc[i].Update = this.listFunc[i].Full;
    //    this.listFunc[i].Delete = this.listFunc[i].Full;
    //    this.listFunc[i].Import = this.listFunc[i].Full;
    //    this.listFunc[i].Export = this.listFunc[i].Full;
    //    this.listFunc[i].Print = this.listFunc[i].Full;
    //    this.listFunc[i].Other = this.listFunc[i].Full;
    //    this.listFunc[i].Menu = this.listFunc[i].Full;
    //  }
    //  if (this.listFunc.filter(l => l.View == false).length > 0) {
    //    this.Action.View = false;
    //  }
    //  else {
    //    this.Action.View = true;
    //  }
    //  if (this.listFunc.filter(l => l.Create == false).length > 0) {
    //    this.Action.Create = false;
    //  }
    //  else {
    //    this.Action.Create = true;
    //  }
    //  if (this.listFunc.filter(l => l.Update == false).length > 0) {
    //    this.Action.Update = false;
    //  }
    //  else {
    //    this.Action.Update = true;
    //  }
    //  if (this.listFunc.filter(l => l.Delete == false).length > 0) {
    //    this.Action.Delete = false;
    //  }
    //  else {
    //    this.Action.Delete = true;
    //  }
    //  if (this.listFunc.filter(l => l.Import == false).length > 0) {
    //    this.Action.Import = false;
    //  }
    //  else {
    //    this.Action.Import = true;
    //  }
    //  if (this.listFunc.filter(l => l.Export == false).length > 0) {
    //    this.Action.Export = false;
    //  }
    //  else {
    //    this.Action.Export = true;
    //  }
    //  if (this.listFunc.filter(l => l.Print == false).length > 0) {
    //    this.Action.Print = false;
    //  }
    //  else {
    //    this.Action.Print = true;
    //  }
    //  if (this.listFunc.filter(l => l.Other == false).length > 0) {
    //    this.Action.Other = false;
    //  }
    //  else {
    //    this.Action.Other = true;
    //  }
    //  if (this.listFunc.filter(l => l.Menu == false).length > 0) {
    //    this.Action.Menu = false;
    //  }
    //  else {
    //    this.Action.Menu = true;
    //  }
    //}
    //changeCell() {
    //  this.changeAction(10);
    //  this.changeFull(undefined);
    //}
    UserDataComponent.prototype.SortTable = function (str) {
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
        this.GetListUser();
    };
    UserDataComponent.prototype.GetClassSortTable = function (str) {
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
    UserDataComponent.prototype.CheckActionTable = function (UserId) {
        if (UserId == undefined) {
            var CheckAll_1 = this.CheckAll;
            this.listUser.forEach(function (item) {
                item.Action = CheckAll_1;
            });
        }
        else {
            var CheckAll = true;
            for (var i = 0; i < this.listUser.length; i++) {
                if (!this.listUser[i].Action) {
                    CheckAll = false;
                    break;
                }
            }
            this.CheckAll = CheckAll == true ? true : false;
        }
    };
    UserDataComponent.prototype.ActionTableFunc = function () {
        var _this = this;
        switch (this.ActionId) {
            case 1:
                var data_1 = [];
                this.listUser.forEach(function (item) {
                    if (item.Action == true) {
                        data_1.push(item.UserId);
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
                                    _this.http.put('/api/customer/deletes', data_1, _this.httpOptions).subscribe(function (res) {
                                        if (res["meta"]["error_code"] == 200) {
                                            _this.toastSuccess("Xóa thành công!");
                                            _this.GetListUser();
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
    UserDataComponent.prototype.closeNoityfy = function () {
        this.isNoitify = true;
    };
    UserDataComponent.prototype.OpenMediaModal = function () {
        this.OpenMediaFile.show();
    };
    UserDataComponent.prototype.CloseMediaModal = function () {
        this.OpenMediaFile.hide();
    };
    UserDataComponent.prototype.tabHandleMedia = function () {
        this.isActiveMedia = true;
        this.isActiveUpload = false;
    };
    UserDataComponent.prototype.tabHandleMediaUpload = function () {
        this.isActiveMedia = false;
        this.isActiveUpload = true;
    };
    UserDataComponent.prototype.upload3 = function (files, cs) {
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
    UserDataComponent.prototype.RemoveImage = function () {
        this.Item.Avata = undefined;
        this.file.nativeElement.value = "";
        this.message = undefined;
        this.progress = undefined;
    };
    UserDataComponent.prototype.loadMore = function () {
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
    UserDataComponent.prototype.GetListFiles = function () {
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
    UserDataComponent.prototype.SeclectMedia = function (item) {
        this.Item.Avata = item.url + "/" + item.name;
        this.OpenMediaFile.hide();
    };
    __decorate([
        core_1.ViewChild('UserModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], UserDataComponent.prototype, "userModal", void 0);
    __decorate([
        core_1.ViewChild('UserModal1'),
        __metadata("design:type", modal_1.ModalDirective)
    ], UserDataComponent.prototype, "UserModal1", void 0);
    __decorate([
        core_1.ViewChild('ResetModal'),
        __metadata("design:type", modal_1.ModalDirective)
    ], UserDataComponent.prototype, "ResetModal", void 0);
    __decorate([
        core_1.ViewChild('file'),
        __metadata("design:type", core_1.ElementRef)
    ], UserDataComponent.prototype, "file", void 0);
    __decorate([
        core_1.ViewChild('OpenMediaFile'),
        __metadata("design:type", modal_1.ModalDirective)
    ], UserDataComponent.prototype, "OpenMediaFile", void 0);
    __decorate([
        core_1.ViewChild('dateStart'),
        __metadata("design:type", core_1.ElementRef)
    ], UserDataComponent.prototype, "dateStart", void 0);
    __decorate([
        core_1.ViewChild('dateEnd'),
        __metadata("design:type", core_1.ElementRef)
    ], UserDataComponent.prototype, "dateEnd", void 0);
    UserDataComponent = __decorate([
        core_1.Component({
            selector: 'app-user-data',
            templateUrl: './user-data.component.html',
            styleUrls: ['./user-data.component.scss'],
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
    ], UserDataComponent);
    return UserDataComponent;
}());
exports.UserDataComponent = UserDataComponent;
//# sourceMappingURL=user-data.component.js.map