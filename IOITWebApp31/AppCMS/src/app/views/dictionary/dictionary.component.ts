import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { domainImage, ActionTable, domainImageFile } from '../../data/const';
import { Dictionary } from '../../data/model';
import { ToastrService } from 'ngx-toastr';
import { DatePipe } from '@angular/common';
import { Md5 } from 'ts-md5/dist/md5';
import { Paging, QueryFilter } from '../../data/dt';
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
  selector: 'app-dictionary',
  templateUrl: './dictionary.component.html',
  styleUrls: ['./dictionary.component.scss']
})
export class DictionaryComponent implements OnInit {
  @ViewChild('DictionaryModal') public DictionaryModal: ModalDirective;
  @ViewChild('file') file: ElementRef;

  public paging: Paging;
  public q: QueryFilter;
  public listDictionary = [];
  public ckeConfig: any;
  public Item: Dictionary;
  public progress: number;
  public httpOptions: any;
  public languageId: number;
  public ActionTable = ActionTable;
  public ActionId: number;
  public CheckAll: boolean;
  public listId = [];

  public page_pp = [];
  public Checkitem: boolean;
  public CheckitemAll: boolean;
  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService
  ) {

    this.Item = new Dictionary();
    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "DictionaryId Desc";
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
    this.GetListDictionary();
  }

  //Mở modal thêm mới
  OpenDictionaryModal(item) {
    this.Item = new Dictionary();
    if (item != undefined) {
      this.Item = JSON.parse(JSON.stringify(item));
    }
    this.DictionaryModal.show();
  }


  //Get danh sách tu dien
  GetListDictionary() {
    this.http.get('/api/Dictionary/GetByPage?page=' + this.paging.page + '&query=' + this.paging.query + '&order_by='+this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listDictionary = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  // luu tu dien
  SaveDictionary() {
    if (this.Item.StringVn == undefined || this.Item.StringVn == '') {
      this.toastWarning("Chưa nhập chuỗi tiếng việt !");
      return;
    } else if (this.Item.StringEn == undefined || this.Item.StringEn == ''){
      this.toastWarning("Chưa nhập chuỗi tiếng anh!");
      return;
    }
    if (this.Item.DictionaryId == undefined) {
      this.http.post('/api/Dictionary', this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListDictionary();
            this.DictionaryModal.hide();
            this.toastSuccess("Thêm mới thành công!");
          }
          else {
            this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
          }
        },
        (err) => {
          this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
        }
      );
    } else {
      this.http.put('/api/Dictionary/' + this.Item.DictionaryId, this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListDictionary();
            this.DictionaryModal.hide();
            this.toastSuccess("Cập nhật thành công!");
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

  //Chuyển trang
  PageChanged(event) {
    this.paging.page = event.page;
    this.GetListDictionary();
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
            this.http.delete('/api/Dictionary/' + Id, this.httpOptions).subscribe(
              (res) => {
                if (res["meta"]["error_code"] == 200) {
                  this.GetListDictionary();
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
  QueryChanged() {
    let query = "";
    if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
      if (query != '') {
        query += ' and Title.Contains("' + this.q.txtSearch + '")';
      }
      else {
        query += 'Title.Contains("' + this.q.txtSearch + '")';
      }
    }


    if (query == '')
      this.paging.query = '1=1';
    else
      this.paging.query = query;

    this.GetListDictionary();
  }
}

