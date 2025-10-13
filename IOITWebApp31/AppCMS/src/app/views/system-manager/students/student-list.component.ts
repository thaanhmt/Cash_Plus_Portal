import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpParams, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { Customer, User } from '../../../data/model';
import { ToastrService } from 'ngx-toastr';
import { DatePipe } from '@angular/common';
import { debug } from 'util';
import { CommonService } from '../../../service/common.service';
import { domainImage, ActionTable, TypeUserSy, domainMedia, domain, domainDebug } from '../../../data/const';
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
  templateUrl: './student-list.component.html',
  styleUrls: ['sudent-list.component.scss'],
  providers: [
    { provide: DateTimeAdapter, useClass: MomentDateTimeAdapter, deps: [OWL_DATE_TIME_LOCALE] },
    { provide: OWL_DATE_TIME_FORMATS, useValue: MY_CUSTOM_FORMATS }
  ]
})
export class StudentListComponent implements OnInit {
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
  public SchoolName: any;
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

  // filter
  public listUser = [];
  public listSchool = [
    { id: 1, value: 'TRƯỜNG CAO ĐẲNG DU LỊCH VÀ THƯƠNG MẠI HÀ NỘI' },
    { id: 2, value: 'TRƯỜNG CAO ĐẲNG CÔNG THƯƠNG VIỆT NAM' },
    { id: 3, value: 'TRƯỜNG KHÁC' }
  ];

  public listSexs = [
    { id: 1, value: "Nam" },
    { id: 2, value: "Nữ" },
    { id: 3, value: "Khác" },
  ]
  public ckeConfig: any;
  public Action: any;

  public Item: Customer;

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
  public unCheck: boolean;
  public httpOptionsBlob: any;

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public datePipe: DatePipe,
    public common: CommonService
  ) {
    this.Item = new Customer();
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
      Update: false,
      Delete: false,
    };
    //this.IsAll = true;
    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }
    this.httpOptionsBlob = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
      observe: 'response',
      responseType: 'blob' as 'json'
    }
  }

  ngOnInit() {
    this.ckeConfig = {
      allowedContent: false,
      extraPlugins: 'divarea',
      forcePasteAsPlainText: true
    };

    this.GetListUser();
    this.GetDomainStatic();

    this.CheckRole = new CheckRole();
    let code = "QLTT";
    this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 0);
    this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 1);
    this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 2);
    this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 3);
  }
  GetDomainStatic() {
    this.http.get('api/Config/1', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.staticDomain = res["data"].Website;
          if (res["data"].ModeSite) {
            this.staticDomainMedia = this.domainDebug + 'uploads';
            this.staticDomain = this.domainDebug;
          } else {
            this.staticDomainMedia = this.staticDomain + 'uploads';
            this.staticDomain = res["data"].Website;
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

    data.page = this.paging.page;
    data.page_size = this.paging.page_size;
    data.query = this.paging.query;
    data.order_by = this.paging.order_by;

    this.http.post('/api/customer/GetAllStudent', data, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listUser = res["data"];
          this.paging.item_count = res["metadata"].Sum;
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
      query += ' and (FullName.Contains("' + this.q.txtSearch + '"))';
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
    if (this.q.SchoolId != undefined) {
      query += ' and SchoolCode=' + this.q.SchoolId;
    }
    if (this.q.StudentCode != undefined && this.q.StudentCode != '') {
      query += ' and (StudentCode.Contains("' + this.q.StudentCode + '"))';
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
    this.message = undefined;
    console.log('his.Item.IsViewInfo', this.Item.IsViewInfo);
    this.userModal.show();
  }

  OpenAddModal2() {
    this.Item = new Customer();
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
    if (this.Item.FullName == undefined || this.Item.FullName == '') {
      this.toastWarning("Chưa nhập Họ và tên!");
      return;
    }
    else if (this.Item.StudentCode == undefined || this.Item.StudentCode == '') {
      this.toastWarning("Chưa nhập mã sinh viên!");
      return;
    } else if (this.Item.StudentClass == undefined || this.Item.StudentClass == '') {
      this.toastWarning("Chưa nhập lớp học!");
    }
    else if (this.Item.StudentYear == undefined || this.Item.StudentYear == '') {
      this.toastWarning("Chưa nhập khoá học!");
    }
    else if (this.Item.SchoolCode == undefined) {
      this.toastWarning("Chưa chọn trường!");
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
      this.http.put('/api/customer/putStudent/' + this.Item.CustomerId, data, this.httpOptions).subscribe(
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


  OpenEditModal(item, type) {
    this.typeAction = type;
    this.Item = new Customer();
    this.Item = Object.assign(this.Item, item);
    console.log(this.Item);
    if(this.Item.SchoolCode == 3) {
      this.SchoolName = this.Item.Note;
    }
    if(this.Item.SchoolCode !== 3) {
      this.SchoolName = this.listSchool.filter((item: any) => item.id == this.Item.SchoolCode)[0].value;
    }
    this.Item.Type = this.Item.Type + "";
    if (this.Item.ListResearchArea == undefined)
      this.Item.ListResearchArea = [];
    if (this.Item.ListUnitManager == undefined)
      this.Item.ListUnitManager = [];
    if (this.Item.ListRoles != undefined)
      this.Item.RoleId = this.Item.ListRoles[0];

    if (this.Item.IsViewInfo === null || this.Item.IsViewInfo === undefined) {
      this.Item.IsViewInfo = false;
    }
    if (this.Item.StepTwo === null || this.Item.StepTwo === undefined) {
      this.Item.StepTwo = false;
    }
    if (this.Item.StepFour === null || this.Item.StepFour === undefined) {
      this.Item.StepFour = false;
    }
    if (this.Item.StepFive === null || this.Item.StepFive === undefined) {
      this.Item.StepFive = false;
    }
    if (this.Item.TopThree === null || this.Item.TopThree === undefined) {
      this.Item.TopThree = false;
    }
    //this.GetListRole();
    this.userModal.show();
  }

  

  upload(files) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);
    console.log(formData);
    const uploadReq = new HttpRequest('POST', 'api/upload/uploadImages/6', formData, {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
      reportProgress: true,
    });

    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress)
        this.progress = Math.round(100 * event.loaded / event.total);
      else if (event.type === HttpEventType.Response) {
        this.Item.Avata = event.body["data"].toString();
        console.log("this.Item.Avata", this.Item.Avata)
      }
    });
  }

  // upload(files) {
  //   console.log(files)
  //   if (files.length === 0)
  //     return;

  //   const formData = new FormData();

  //   for (let file of files)
  //     formData.append(file.name, file);

  //   const uploadReq = new HttpRequest('POST', 'api/upload/uploadImage/6', formData, {
  //     headers: new HttpHeaders({
  //       'Authorization': 'bearer ' + localStorage.getItem("access_token")
  //     }),
  //     reportProgress: true,
  //   });

  //   this.http.request(uploadReq).subscribe(event => {
  //     if (event.type === HttpEventType.UploadProgress)
  //       this.progress = Math.round(100 * event.loaded / event.total);
  //     else if (event.type === HttpEventType.Response) {
  //       this.message = event.body["data"].toString();
  //       this.Item.Avata = this.message;
  //     }
  //   });
  // }




  //
  RemoveImage() {
    this.Item.Avata = undefined;
    this.file.nativeElement.value = "";
    this.progress = undefined;
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

  ShowHide(isViewInfo: any) {
    console.log(this.Item);
  }
  ExportExcel() {
    let data = Object.assign({}, this.listUser);
    this.http.post<Blob>('/api/customer/ExportExcel', data,this.httpOptionsBlob).subscribe(
      (res) => {
        //console.log(res);
        var url = window.URL.createObjectURL(res["body"]);
        var a = document.createElement('a');
        document.body.appendChild(a);
        a.setAttribute('style', 'display: none');
        a.href = url;
        a.download = "DanhSachSinhVien.xlsx";
        a.click();
        window.URL.revokeObjectURL(url);
        a.remove();
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
      }
    );

  }
}
