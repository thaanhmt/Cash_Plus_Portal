import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpParams, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { Customer, User } from '../../../data/model';
import { ToastrService } from 'ngx-toastr';
import { DatePipe } from '@angular/common';
import { debug } from 'util';
import { CommonService } from '../../../service/common.service';
import { domainImage, ActionTable, TypeUserSy, domainMedia, domain, domainDebug, listSexs, listTypeId, listUserTypes, listUserStatus } from '../../../data/const';
import { CheckRole, Paging, QueryFilter, UserChangePass } from '../../../data/dt';
import { DateTimeAdapter, OWL_DATE_TIME_FORMATS, OWL_DATE_TIME_LOCALE } from 'ng-pick-datetime';
import { MomentDateTimeAdapter } from 'ng-pick-datetime-moment';

export const MY_CUSTOM_FORMATS = {
  parseInput: 'DD/MM/YYYY HH:mm',
  fullPickerInput: 'DD/MM/YYYY HH:mm',
  datePickerInput: 'DD/MM/YYYY',
  timePickerInput: ' HH:mm',
  monthYearLabel: 'MMM YYYY',
  dateA11yLabel: 'LL',
  monthYearA11yLabel: 'MMMM YYYY'
};

@Component({
  selector: 'app-user-data',
  templateUrl: './user-data.component.html',
  styleUrls: ['./user-data.component.scss'],
  providers: [
    { provide: DateTimeAdapter, useClass: MomentDateTimeAdapter, deps: [OWL_DATE_TIME_LOCALE] },
    { provide: OWL_DATE_TIME_FORMATS, useValue: MY_CUSTOM_FORMATS }
  ]
})
export class UserDataComponent implements OnInit {
  @ViewChild('UserModal') public userModal: ModalDirective;
  @ViewChild('UserModal1') public UserModal1: ModalDirective;
  @ViewChild('ResetModal') public ResetModal: ModalDirective;
  @ViewChild('file') file: ElementRef;
  @ViewChild('OpenMediaFile') public OpenMediaFile: ModalDirective;

  @ViewChild('dateStart') dateStart: ElementRef;
  @ViewChild('dateEnd') dateEnd: ElementRef;

  public isActiveMedia: boolean = true;
  public isActiveUpload: boolean = false;
  public isDelay: boolean = false;

  public domain = domain;
  public pagingFile: Paging;
  public countMedia: number;
  public countAllMedia: number;

  public listItemMedia = [];
  public domainMedia = domainMedia;
  public message: string;
  public progress: number;
  public paging: Paging;
  public q: QueryFilter;

  public listUser = [];
  public listChucDanh = [];
  public listHocHam = [];
  public listHocVi = [];
  public listResearchArea = [];
  public listUnit = [];
  public listRole = [];
  public listCountry = [];
  public listSexs = listSexs;
  public listTypeId = listTypeId;
  public listUserTypes = listUserTypes;
  public listUserStatus = listUserStatus;
  public ckeConfig: any;
  public Action: any;

  public Item: Customer;
  public Item1: Customer;

  public domainImage = domainImage;
  public TypeUserSy = TypeUserSy;

  public httpOptions: any;

  public ActionTable = ActionTable;
  public ActionId: number;
  public CheckAll: boolean;
  public isNoitify: boolean = false;
  public staticDomain: string;
  public staticDomainMedia: string;
  public domainDebug = domainDebug;

  public CheckRole: CheckRole;
  public typeAction: number;

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public datePipe: DatePipe,
    public common: CommonService
  ) {
    this.Item = new Customer();
    this.Item1 = new Customer();
    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "";
    this.paging.item_count = 0;
    this.pagingFile = new Paging();
    this.pagingFile.page = 1;
    this.pagingFile.page_size = 24;
    this.pagingFile.query = "1=1";
    this.pagingFile.order_by = "";
    this.pagingFile.item_count = 0;
    this.countMedia = 24;
    this.q = new QueryFilter();
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
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }
  }

  ngOnInit() {
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

    this.CheckRole = new CheckRole();
    let code = "QLTT";
    this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 0);
    this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 1);
    this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 2);
    this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 3);
    this.CheckRole.Export = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 5);

  }

  GetDomainStatic() {
    this.http.get('api/Config/1', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.staticDomain = res["data"].Website;
          console.log(this.staticDomain)
          if (res["data"].ModeSite) {
            this.staticDomainMedia = this.domainDebug + 'uploads';
            this.staticDomain = this.domainDebug;
            console.log(this.staticDomain)
          } else {
            this.staticDomainMedia = this.staticDomain + 'uploads';
            this.staticDomain = res["data"].Website;
            console.log(this.staticDomain)
          }
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  //Get danh sách danh user
  GetListUser() {
    let data = Object.assign({}, this.q);

    if (this.dateStart.nativeElement.value) {
      let obj = this.dateStart.nativeElement.value.split("/");
      data.DateStart = obj[2] + "-" + obj[1] + "-" + obj[0] + " 0:0:0";
    }
    if (this.dateEnd.nativeElement.value) {
      let obj = this.dateEnd.nativeElement.value.split("/");
      data.DateEnd = obj[2] + "-" + obj[1] + "-" + obj[0] + " 23:59:59";
    }
    data.page = this.paging.page;
    data.page_size = this.paging.page_size;
    data.query = this.paging.query;
    data.order_by = this.paging.order_by;
    
    this.http.post('/api/customer/GetByPagePost', data, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listUser = res["data"];
          //this.listUser.forEach(item => {
          //  item.IsShow = item.Status == 1 ? true : false;
          //});
          this.paging.item_count = res["metadata"].Sum;
          //this.total = res["metadata"];
          //this.total1 = res["metadata"].Normal;
          //this.total2 = res["metadata"].Publishing;
          //this.total3 = res["metadata"].UnPublish;
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
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
  }

  GetListTypeAttributeItem(id) {
    this.http.get('/api/TypeAttributeItem/GetByPage?page=1&query=TypeAttributeId=' + id + '&order_by=TypeAttributeItemId Asc', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          if(id==4)
            this.listChucDanh = res["data"];
          else if (id == 25)
            this.listHocHam = res["data"];
          else if (id == 26)
            this.listHocVi = res["data"];

        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListResearchArea() {
    let type = 15;
    this.http.get('/api/category/GetByPage?page=1&query=TypeCategoryId=' + type + '&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
            this.listResearchArea = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListUnit() {
    this.http.get('/api/unit/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listUnit = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListCountry() {
    this.http.get('/api/country/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCountry = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListRole(type) {
    //let arr = [];
    //if (this.Item.CustomerId) {
    //  arr = Object.assign(this.Item["listRole"]);
    //}
    let query = "Type==" + type;
    this.http.get('/api/role/GetByPage?page=1&query=' + query + '&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          //if (this.Item.CustomerId == undefined) {
            this.listRole = res["data"];
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
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  //Chuyển trang
  PageChanged(event) {
    this.paging.page = event.page;
    this.GetListUser();
  }

  //Thông báo
  toastWarning(msg): void {
    this.toastr.warning(msg, 'Cảnh báo');
  }

  toastSuccess(msg): void {
    this.toastr.success(msg, 'Hoàn thành');
  }

  toastError(msg): void {
    this.toastr.error(msg, 'Lỗi');
  }
  //
  QueryChanged() {
    let query = '1=1';
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
  }

  //Mở modal thêm mới
  OpenAddModal() {
    this.Item = new Customer();
    this.Item.Type = "1";
    this.Item.Sex = 1;
    this.Item.CountryId = 1;
    this.Item.ListRoles = [];
    this.Item.ListResearchArea = [];
    this.Item.ListUnitManager = [];
    this.file.nativeElement.value = "";
    this.message = undefined;
    this.userModal.show();
  }

  OpenAddModal2() {
    this.Item = new Customer();
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
  }

  OpenResetModal(item) {
    this.Item = new Customer();
    
    this.Item = Object.assign(this.Item, item);
    this.ResetModal.show();
  }
  //Thêm mới người dùng
  AddUserFunc() {
    if (this.Item.Email == undefined || this.Item.Email == '') {
      this.toastWarning("Chưa nhập Email!");
      return;
    } else if (this.Item.Email.replace(/ /g, '') == '') {
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

    let data = Object.assign({}, this.Item);
    if (data.Birthday != undefined) {
      let obj2 = new Date(data.Birthday);
      data.Birthday = obj2.getFullYear() + "-" + (obj2.getMonth() + 1) + "-" + obj2.getDate() + " " + obj2.getHours() + ":" + obj2.getMinutes() + ":0";
    }
    if (data.DateNumber != undefined) {
      let obj2 = new Date(data.DateNumber);
      data.DateNumber = obj2.getFullYear() + "-" + (obj2.getMonth() + 1) + "-" + obj2.getDate() + " " + obj2.getHours() + ":" + obj2.getMinutes() + ":0";
    }

    data.ListRoles = [];
    if (data.RoleId != undefined)
      data.ListRoles.push(data.RoleId);
    
    if (this.Item.CustomerId) {
      this.http.put('/api/customer/' + this.Item.CustomerId, data, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListUser();
            this.userModal.hide();
            this.toastSuccess("Cập nhật thành công!");
          }
          else {
            this.toastError(res["meta"]["error_message"]);
          }
        },
        (err) => {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        }
      );
    }
    else {
      this.http.post('/api/customer', data, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListUser();
            this.userModal.hide();
            this.toastSuccess("Thêm mới thành công!");
          }
          else {
            this.toastError(res["meta"]["error_message"]);
          }
        },
        (err) => {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        }
      );
    }
  }

  ResetPass() {
    if (this.Item.PasswordNew == undefined || this.Item.PasswordNew == '') {
      this.toastWarning("Chưa nhập mật khẩu mới!");
      return;
    }
    this.Item1.CustomerId = this.Item.CustomerId;
    this.Item1.PasswordNew = this.Item.PasswordNew;
    this.http.put('/api/User/adminChangePass/' + this.Item.CustomerId, this.Item1, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListUser();
          this.ResetModal.hide();
          this.toastSuccess("Thay đổi mật khẩu thành công!");
        }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
      }
    );

  }
  LockUser(id, type) {
    this.http.put('api/customer/lockUser/' + id + '/' + type, undefined, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListUser();
          this.ResetModal.hide();
          if(type==98)
            this.toastSuccess("Khóa tài khoản thành công!");
          else
            this.toastSuccess("Kích hoạt tài khoản thành công!");
        }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
      }
    );
  }
  OpenUser(id) {
    this.http.put('api/User/lockUser/' + id + '/1', undefined, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListUser();
          this.ResetModal.hide();
          this.toastSuccess("Mở tài khoản thành công!");
        }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
      }
    );
  }
  OpenEditModal(item, type) {
    this.typeAction = type;
    this.Item = new Customer();
    this.Item = Object.assign(this.Item, item);
    if (this.Item.Type== 1)
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
  }

  //Popup xác nhận xóa
  ShowConfirmDelete(Id) {
    this.modalDialogService.openDialog(this.viewRef, {
      title: 'Xác nhận',
      childComponent: SimpleModalComponent,
      data: {
        text: "Bạn có chắc chắn muốn xóa bản ghi này?"
      },
      actionButtons: [
        {
          text: 'Đồng ý',
          buttonClass: 'btn btn-success',
          onAction: () => {
            this.DeleteCustomer(Id);
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-danger',

        }
      ],
    });
  }

  DeleteCustomer(Id) {
    this.http.delete('/api/customer/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListUser();
          this.viewRef.clear();
          this.toastSuccess("Xóa thành công!");
        }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
      }
    );
  }

  upload(files) {
    console.log(files)
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);

    const uploadReq = new HttpRequest('POST', 'api/upload/uploadImage/6', formData, {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
      reportProgress: true,
    });

    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress)
        this.progress = Math.round(100 * event.loaded / event.total);
      else if (event.type === HttpEventType.Response) {
        this.message = event.body["data"].toString();
        this.Item.Avata = this.message;
      }
    });
  }

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

  SortTable(str) {
    let First = "";
    let Last = "";
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
  }

  GetClassSortTable(str) {
    if (this.paging.order_by != (str + " Desc") && this.paging.order_by != (str + " Asc")) {
      return "sorting";
    }
    else {
      if (this.paging.order_by == (str + " Desc")) return "sorting_desc";
      else return "sorting_asc";
    }
  }

  CheckActionTable(UserId) {
    if (UserId == undefined) {
      let CheckAll = this.CheckAll;
      this.listUser.forEach(item => {
        item.Action = CheckAll;
      });
    }
    else {
      let CheckAll = true;
      for (let i = 0; i < this.listUser.length; i++) {
        if (!this.listUser[i].Action) {
          CheckAll = false;
          break;
        }
      }

      this.CheckAll = CheckAll == true ? true : false;
    }
  }

  ActionTableFunc() {
    switch (this.ActionId) {
      case 1:
        let data = [];
        this.listUser.forEach(item => {
          if (item.Action == true) {
            data.push(item.UserId);
          }
        });
        if (data.length == 0) {
          this.toastWarning("Chưa chọn bản ghi cần xóa!");
        }
        else {
          this.modalDialogService.openDialog(this.viewRef, {
            title: 'Xác nhận',
            childComponent: SimpleModalComponent,
            data: {
              text: "Bạn có chắc chắn muốn xóa các bản ghi đã chọn?"
            },
            actionButtons: [
              {
                text: 'Đồng ý',
                buttonClass: 'btn btn-success',
                onAction: () => {
                  this.http.put('/api/customer/deletes', data, this.httpOptions).subscribe(
                    (res) => {
                      if (res["meta"]["error_code"] == 200) {
                        this.toastSuccess("Xóa thành công!");
                        this.GetListUser();
                        this.ActionId = undefined;
                      }
                      else {
                        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                      }
                    },
                    (err) => {
                      this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                    }
                  );
                  this.viewRef.clear();
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
  closeNoityfy() {
    this.isNoitify = true;
  }
  OpenMediaModal() {
    this.OpenMediaFile.show();
  }
  CloseMediaModal() {
    this.OpenMediaFile.hide();
  }
  tabHandleMedia() {
    this.isActiveMedia = true;
    this.isActiveUpload = false;
  }
  tabHandleMediaUpload() {
    this.isActiveMedia = false;
    this.isActiveUpload = true;
  }
  upload3(files, cs) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);

    const uploadReq = new HttpRequest('POST', 'api/upload/uploadMedia/8', formData, {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
      reportProgress: true,
    });

    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress)
        switch (cs) {
          case 1:
            this.progress = Math.round(100 * event.loaded / event.total);
            break;
          default:
            break;
        }
      else if (event.type === HttpEventType.Response) {

        switch (cs) {
          case 1:
            this.isActiveMedia = true;
            this.isActiveUpload = false;
            this.pagingFile.page = 1;
            this.GetListFiles();
            this.message = event.body["data"].toString();
            this.toastSuccess("Tải lên thành công");
            break;
          default:
            break;
        }
      }
    });
  }
  RemoveImage() {
    this.Item.Avata = undefined;
    this.file.nativeElement.value = "";
    this.message = undefined;
    this.progress = undefined;
  }
  loadMore() {
    this.isDelay = true;
    setTimeout(
      () => {
        this.isDelay = false;
        this.pagingFile.page++;
        this.http.get('/api/fileManager/GetFiles?page=' + this.pagingFile.page + '&page_size=' + this.pagingFile.page_size + '&query='
          + this.pagingFile.query + '&order_by=' + '&select=' + this.pagingFile.select, this.httpOptions).subscribe(
            (res) => {
              if (res["meta"]["error_code"] == 200) {
                this.listItemMedia.push(...res["data"]);
                if (this.countMedia >= this.countAllMedia) {
                  this.countMedia = this.countAllMedia;
                } else {
                  if ((this.countMedia + 24) >= this.countAllMedia) {
                    this.countMedia = this.countAllMedia;
                  } else {
                    this.countMedia += 24;
                  }
                }
              }
            },
            (err) => {
              console.log("Error: connect to API");
            }
          );
      },
      1000
    );
  }

  GetListFiles() {
    this.http.get('/api/fileManager/GetFiles?page=' + this.pagingFile.page + '&page_size=' + this.pagingFile.page_size + '&query='
      + this.pagingFile.query + '&order_by=' + '&select=' + this.pagingFile.select, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.listItemMedia = res["data"];
            this.countAllMedia = res["metadata"];
            if (this.countAllMedia < 24) this.countMedia = this.countAllMedia;
          }
        },
        (err) => {
          console.log("Error: connect to API");
        }
      );
  }

  SeclectMedia(item) {
    this.Item.Avata = item.url + "/" + item.name;
    this.OpenMediaFile.hide();
  }
}
