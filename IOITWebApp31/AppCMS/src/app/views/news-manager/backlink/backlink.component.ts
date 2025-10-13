import { Component, ElementRef, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { HttpClient, HttpEventType, HttpHeaders, HttpRequest } from '@angular/common/http';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { ToastrService } from 'ngx-toastr';
import { CommonService } from '../../../service/common.service';
import { Paging, QueryFilter } from '../../../data/dt';
import { Author, Backlink, Function } from '../../../data/model';
import { ActionTable, domainImage } from '../../../data/const';

@Component({
  selector: 'app-backlink',
  templateUrl: './backlink.component.html',
  styleUrls: ['./backlink.component.scss']
})

export class BacklinkComponent implements OnInit {
  @ViewChild('modalFunction') public modalFunction: ModalDirective;

  public paging: Paging;
  public q: QueryFilter;

  public listBacklink = [];

  public Item: Backlink;

  public httpOptions: any;

  public ActionTable = ActionTable;
  public ActionId: number;
  public CheckAll: boolean;
  public isNoitify: boolean = false;

  public progress: number;
  public message: string;
  public domainImage = domainImage;

  constructor(public http: HttpClient, public modalDialogService: ModalDialogService, public viewRef: ViewContainerRef, public toastr: ToastrService) {
    this.Item = new Backlink();

    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "CreatedAt Desc";
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
    this.GetListBacklink();
    //this.GetListUser();
  }

  GetListBacklink() {
    this.http.get('/api/backlink/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listBacklink = res["data"];
          this.paging.item_count = res["metadata"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      });
  }

  //GetListUser() {
  //  this.http.get('/api/userRole/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(
  //    (res) => {
  //      if (res["meta"]["error_code"] == 200) {
  //        this.listUsers = res["data"];
  //      }
  //    },
  //    (err) => {
  //      console.log("Error: connect to API");
  //    });
  //}

  //Chuyển trang
  PageChanged(event) {
    this.paging.page = event.page;
    this.GetListBacklink();
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
    let query = '1=1 AND Type=1';
    if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
      //if (query != '') {
      query += ' and (Name.Contains("' + this.q.txtSearch + '") OR Code.Contains("' + this.q.txtSearch + '"))';
      //}
      //else {
      //  query += '(Name.Contains("' + this.q.txtSearch + '") OR Code.Contains("' + this.q.txtSearch + '"))';
      //}
    }

    //if (query == '')
    //  this.paging.query = '1=1';
    //else
    this.paging.query = query;

    this.GetListBacklink();
  }


  OpenModalFunction(item) {
    this.Item = new Backlink();
    if (item == undefined) {
    }
    else {
      this.Item = Object.assign(this.Item, item);
    }

    this.modalFunction.show();
  }

  SaveFunc() {
    if (this.Item.LinkIn == undefined || this.Item.LinkIn == '') {
      this.toastWarning("Chưa nhập liên kết gốc!");
      return;
    } else if (this.Item.LinkIn.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập liên kết gốc!");
      return;
    }

    this.Item.UserId = parseInt(localStorage.getItem("userId"));
    let obj = JSON.parse(JSON.stringify(this.Item));

    if (this.Item.BackLinkId == undefined) {
      this.http.post('/api/backlink', this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListBacklink();
            this.modalFunction.hide();
            this.toastSuccess("Thêm mới thành công!");
          }
          else if (res["meta"]["error_code"] == 211) {
            this.toastError("Thông tin không đủ. Xin vui lòng thử lại!");
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
      //if (obj.FunctionParentId == null) obj.FunctionParentId = 0;

      this.http.put('/api/backlink/' + obj.BackLinkId, obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListBacklink();
            this.modalFunction.hide();
            this.toastSuccess("Cập nhật thành công!");
          }
          else if (res["meta"]["error_code"] == 211) {
            this.toastError("Thông tin không đủ. Xin vui lòng thử lại!");
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
            this.Delete(Id);
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-danger',

        }
      ],
    });
  }

  Delete(Id) {
    this.http.delete('/api/backlink/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListBacklink();
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

    this.GetListBacklink();
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

  CheckActionTable(FunctionId) {
    if (FunctionId == undefined) {
      let CheckAll = this.CheckAll;
      this.listBacklink.forEach(item => {
        item.Action = CheckAll;
      });
    }
    else {
      let CheckAll = true;
      for (let i = 0; i < this.listBacklink.length; i++) {
        if (!this.listBacklink[i].Action) {
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
        this.listBacklink.forEach(item => {
          if (item.Action == true) {
            data.push(item.FunctionId);
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
                  this.http.put('/api/backlink/deletes', data, this.httpOptions).subscribe(
                    (res) => {
                      if (res["meta"]["error_code"] == 200) {
                        this.toastSuccess("Xóa thành công!");
                        this.GetListBacklink();
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
  closeNoityfy() {
    this.isNoitify = true;
  }

}
