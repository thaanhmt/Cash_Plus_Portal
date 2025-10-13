import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { ToastrService } from 'ngx-toastr';
import { Order, OrderItem } from '../../../data/model';
import { OrderStatus, PaymentOrderStatus, ActionTable } from '../../../data/const';



@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.scss']
})
export class OrderComponent implements OnInit {
  @ViewChild('ViewModal') public viewModal: ModalDirective;

  public paging: any;
  public pagingItem: any;
  public q: any;

  public listOrder = [];

  public ckeConfig: any;

  public Item: Order;
  public httpOptions: any;
  public listOrderStatus = OrderStatus;
  public listPaymentOrderStatus = PaymentOrderStatus;

  public ActionTable = ActionTable;
  public ActionId: number;
  public CheckAll: boolean;

  PriceCurrencyMaskConfig = {
    align: "left",
    allowNegative: false,
    decimal: ".",
    precision: 0,
    prefix: "",
    suffix: " vnđ",
    thousands: ","
  };

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService
  ) {
    this.Item = new Order();

    this.paging = {
      page: 1,
      page_size: 10,
      query: '1=1',
      order_by: 'OrderId Desc',
      item_count: 0
    };

    this.pagingItem = {
      page: 1,
      page_size: 10,
      query: '',
      order_by: '',
      item_count: 0
    };

    this.q = {
      txtSearch: ''
    }

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

    this.GetListOrder();
  }

  //Get danh sách danh mục đơn hàng
  GetListOrder() {
    this.http.get('/api/order/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listOrder = res["data"];
          this.paging.item_count = res["metadata"];
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
    this.GetListOrder();
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

  //
  QueryChanged() {
    let query = '';
    if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
      if (query != '') {
        query += ' and Code.Contains("' + this.q.txtSearch + '")';
      }
      else {
        query += 'Code.Contains("' + this.q.txtSearch + '")';
      }
    }

    if (this.q.TypeOrderStatus != undefined) {
      if (query != '') {
        query += ' and OrderStatusId=' + this.q.TypeOrderStatus;
      }
      else {
        query += 'OrderStatusId=' + this.q.TypeOrderStatus;
      }
    }

    if (this.q.TypePaymentOrderStatus != undefined) {
      if (query != '') {
        query += ' and PaymentStatusId=' + this.q.TypePaymentOrderStatus;
      }
      else {
        query += 'PaymentStatusId=' + this.q.TypePaymentOrderStatus;
      }
    }

    if (query == '')
      this.paging.query = '1=1';
    else
      this.paging.query = query;

    this.GetListOrder();
  }
  //

  //Open modal view
  OpenViewModal(item) {
    this.Item = new Order();
    this.Item = Object.assign({}, item);
    this.viewModal.show();
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
            this.http.delete('/api/Order/' + Id, this.httpOptions).subscribe(
              (res) => {
                if (res["meta"]["error_code"] == 200) {
                  this.GetListOrder();
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
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-default',

        }
      ],
    });
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

    this.GetListOrder();
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

  ChangeOrderStatus(OrderId, Status) {
    this.http.put('/api/order/ChangeOrderStatus/' + OrderId + '/' + Status, undefined, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListOrder();
          this.toastSuccess(res["meta"]["error_message"]);
        }
        else {
          this.toastError(res["meta"]["error_message"]);
          this.GetListOrder();
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        this.GetListOrder();
      }
    );
  }

  ChangePaymentOrderStatus(OrderId, Status) {
    this.http.put('/api/order/ChangePaymentOrderStatus/' + OrderId + '/' + Status, undefined, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListOrder();
          this.toastSuccess(res["meta"]["error_message"]);
        }
        else {
          this.toastError(res["meta"]["error_message"]);
          this.GetListOrder();
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        this.GetListOrder();
      }
    );
  }

  CheckActionTable(OrderId) {
    if (OrderId == undefined) {
      let CheckAll = this.CheckAll;
      this.listOrder.forEach(item => {
        item.Action = CheckAll;
      });
    }
    else {
      let CheckAll = true;
      for (let i = 0; i < this.listOrder.length; i++) {
        if (!this.listOrder[i].Action) {
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
        this.listOrder.forEach(item => {
          if (item.Action == true) {
            data.push(item.OrderId);
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
                  this.http.put('/api/order/deletes', data, this.httpOptions).subscribe(
                    (res) => {
                      if (res["meta"]["error_code"] == 200) {
                        this.toastSuccess("Xóa thành công!");
                        this.GetListOrder();
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
                buttonClass: 'btn btn-default',

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
