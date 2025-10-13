import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { domainImage, ActionTable, TypeUser } from '../../../data/const';
import { Customer, ResetPasswordCustomerDTO } from '../../../data/model';
import { ToastrService } from 'ngx-toastr';
import { DatePipe } from '@angular/common';
import { Md5 } from 'ts-md5/dist/md5';
import { Paging, QueryFilter } from '../../../data/dt';
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
  selector: 'app-customer',
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.scss'],
  providers: [
    { provide: DateTimeAdapter, useClass: MomentDateTimeAdapter, deps: [OWL_DATE_TIME_LOCALE] },
    { provide: OWL_DATE_TIME_FORMATS, useValue: MY_CUSTOM_FORMATS }
  ]
})


export class CustomerComponent implements OnInit {
  @ViewChild('CustomerModal') public CustomerModal: ModalDirective;
  @ViewChild('ResetPasswordModal') public ResetPasswordModal: ModalDirective;
  @ViewChild('OrdersModal') public OrdersModal: ModalDirective;

  @ViewChild('file') file: ElementRef;

  public paging: Paging;
  public q: QueryFilter;
  public listCustomer = [];
  public orders = [];
  public ckeConfig: any;
  public donhangcon: any;
  public Item: Customer;
  public ItemResetPassword: ResetPasswordCustomerDTO;
  public progress: number;
  public domainImage = domainImage;
  public httpOptions: any;
  public CustomerName: string;
  public TypeUser = TypeUser;

  public ActionTable = ActionTable;
  public ActionId: number;
  public CheckAll: boolean;
  public total: any;


  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public datePipe: DatePipe
  ) {

    this.Item = new Customer();
    this.ItemResetPassword = new ResetPasswordCustomerDTO();
    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "CustomerId Desc";
    this.paging.item_count = 0;

    this.q = new QueryFilter();
    this.q.txtSearch = "";

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
    this.GetListCustomer();
  }

  //Get danh sach khach hang
  GetListCustomer() {
    this.http.get('/api/customer/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCustomer = res["data"];
          this.paging.item_count = res["metadata"].Sum;
          this.total = res["metadata"];
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
    this.GetListCustomer();
  }
  //Toast cảnh báo
  toastWarning(msg): void {
    this.toastr.warning(msg, 'Cảnh báo');
  }
  //Toast thành công
  toastSuccess(msg): void {
    this.toastr.success(msg, 'Hoàn thành');
  }
  //Toast thành công
  toastError(msg): void {
    this.toastr.error(msg, 'Lỗi');
  }
  //Search
  QueryChanged() {
    let query = '';
    if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
      if (query != '') {
        query += ' and (FullName.Contains("' + this.q.txtSearch + '") Or Email.Contains("' + this.q.txtSearch + '"))';
      }
      else {
        query += '(FullName.Contains("' + this.q.txtSearch + '") or Email.Contains("' + this.q.txtSearch + '"))';
      }
    }

    if (this.q["TypeUsertId"] != undefined) {
      if (this.q["TypeUsertId"] == 1) {
        if (query != '') {
          query += 'IsEmailConfirm = true';
        }
        else {
          query += 'IsEmailConfirm = true';
        }

      } else {
        if (query != '') {
          query += 'IsEmailConfirm = false';
        }
        else {
          query += 'IsEmailConfirm = false';
        }
      }
      
    }

    if (query == '')
      this.paging.query = '1=1';
    else
      this.paging.query = query;

    this.GetListCustomer();
  }

  //Mở modal thêm mới
  OpenCustomerModal(item) {
    this.Item = new Customer();
    this.file.nativeElement.value = "";
    this.progress = undefined;
    if (item != undefined) {
      this.Item = JSON.parse(JSON.stringify(item));
    }
    this.CustomerModal.show();
  }

  //Thêm mới khách hàng
  SaveCustomer() {
    console.log('ee');
    if (this.Item.FullName == undefined || this.Item.FullName == '') {
      this.toastWarning("Chưa nhập tên!");
      return;
    } else if (this.Item.FullName.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên!");
      return;
    } else if (this.Item.Email == undefined || this.Item.Email == '') {
      this.toastWarning("Chưa nhập email!");
      return;
    } else if (this.Item.Email.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập email!");
      return;
    } else if ((this.Item.Password == undefined || this.Item.Password == '') && this.Item.CustomerId == undefined) {
      this.toastWarning("Chưa nhập mật khẩu!");
      return;
    } else if (this.Item.Phone == undefined || this.Item.Phone == '') {
      this.toastWarning("Chưa chọn số điện thoại!");
      return;
    } else if (this.Item.Phone.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập số điện thoại!");
      return;
    } else if ((this.Item.Password.length < 6) && this.Item.CustomerId == undefined) {
      this.toastWarning("Mật khẩu ít nhất 6 ký tự!");
      return;
    } else if ((this.Item.ConfirmPassword != this.Item.Password) && this.Item.CustomerId == undefined) {
      this.toastWarning("Mật khẩu không trùng khớp!");
      return;
    }
    for (let i = 0; i < this.listCustomer.length; i++) {
      if (this.listCustomer[i].Email == this.Item.Email) {
        this.toastWarning("Email đã tồn tại trong hệ thống!");
        return;
      }
    }
    this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
    this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));

    if (typeof this.Item.Birthday === 'object' && this.Item.Birthday != undefined) {
      let Birthday = this.Item.Birthday.add(7, 'hours');
      this.Item.Birthday = Birthday.toISOString();
    }
   

    if (this.Item.CustomerId == undefined) {
      let obj = JSON.parse(JSON.stringify(this.Item));
      obj.Password = Md5.hashStr(this.Item.Password).toString();
      // this.Item.Password = Md5.hashStr(this.Item.Password).toString();

      this.http.post('/api/Customer', obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListCustomer();
            this.CustomerModal.hide();
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
    else {
      this.http.put('/api/Customer/' + this.Item.CustomerId, this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListCustomer();
            this.CustomerModal.hide();
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
            this.DeleteCatePage(Id);
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-danger',

        }
      ],
    });
  }

  DeleteCatePage(Id) {
    this.http.delete('/api/Customer/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListCustomer();
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
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);
    console.log(formData);
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
        this.Item.Avata = event.body["data"].toString();
      }
    });
  }
  //
  RemoveImage() {
    this.Item.Avata = undefined;
    this.file.nativeElement.value = "";
    this.progress = undefined;
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

    this.GetListCustomer();
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

  //Cấp lại mật khẩu cho khách hàng - gửi mật khẩu vào email
  ResetPasswordCustomerModal(CustomerId, FullName) {
    this.ItemResetPassword = new ResetPasswordCustomerDTO();
    this.ItemResetPassword.FullName = FullName;
    this.ItemResetPassword.CustomerId = CustomerId;
    this.ResetPasswordModal.show();
  }

  ResetPassCustomer() {
    if (this.ItemResetPassword.Password == undefined || this.ItemResetPassword.Password == '') {
      this.toastWarning("Chưa nhập mật khẩu!");
      return;
    } else if (this.ItemResetPassword.Password.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập mật khẩu!");
      return;
    } else if (this.ItemResetPassword.ConfirmPassword == undefined || this.ItemResetPassword.ConfirmPassword == '') {
      this.toastWarning("Chưa nhập mật khẩu xác nhận!");
      return;
    } else if (this.ItemResetPassword.ConfirmPassword.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập mật khẩu xác nhận!");
      return;
    } else if (this.ItemResetPassword.Password != this.ItemResetPassword.ConfirmPassword) {
      this.toastWarning("Mật khẩu xác nhận không chính xác!");
    }

    let obj = JSON.parse(JSON.stringify(this.ItemResetPassword));
    obj.PasswordInit = obj.Password;
    obj.Password = Md5.hashStr(obj.Password).toString();
    obj.ConfirmPassword = Md5.hashStr(obj.ConfirmPassword).toString();

    this.http.post('/api/Customer/ResetPassword/' + obj.CustomerId, obj, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.ItemResetPassword = new ResetPasswordCustomerDTO();
          this.ResetPasswordModal.hide();
          this.toastSuccess(res["meta"]["error_message"]);
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

  OpenOrdersModal(CustomerId, CustomerName) {
    this.CustomerName = CustomerName;
    this.orders = [];

    let query = "CustomerId=" + CustomerId;

    this.http.get('/api/order/GetByPage?page=1&query=' + query + '&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.orders = res["data"];
          this.donhangcon = this.orders[0].listOrderItems.length;
          console.log(this.donhangcon);
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );

    this.OrdersModal.show();
  }

  CheckActionTable(CustomerId) {
    if (CustomerId == undefined) {
      let CheckAll = this.CheckAll;
      this.listCustomer.forEach(item => {
        item.Action = CheckAll;
      });
    }
    else {
      let CheckAll = true;
      for (let i = 0; i < this.listCustomer.length; i++) {
        if (!this.listCustomer[i].Action) {
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
        this.listCustomer.forEach(item => {
          if (item.Action == true) {
            data.push(item.CustomerId);
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
                        this.GetListCustomer();
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
}
