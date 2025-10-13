import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { domainImage, ActionTable, domain } from '../../../data/const';
import { CommentPost} from '../../../data/model';
import { ToastrService } from 'ngx-toastr';
import { DatePipe } from '@angular/common';
import { CommonService } from '../../../service/common.service';
import { Paging, QueryFilter } from '../../../data/dt';
import { DateTimeAdapter, OWL_DATE_TIME_FORMATS, OWL_DATE_TIME_LOCALE } from 'ng-pick-datetime';
import { MomentDateTimeAdapter } from 'ng-pick-datetime-moment';
import { TabsetComponent } from 'ngx-bootstrap/tabs';


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
  selector: 'app-comment-post',
  templateUrl: './comment-post.component.html',
  styleUrls: ['./comment-post.component.scss'],
  providers: [
    { provide: DateTimeAdapter, useClass: MomentDateTimeAdapter, deps: [OWL_DATE_TIME_LOCALE] },
    { provide: OWL_DATE_TIME_FORMATS, useValue: MY_CUSTOM_FORMATS }
  ]
})
export class CommentPostComponent implements OnInit {
  @ViewChild('CommentModal') public CommentModal: ModalDirective;
  @ViewChild('RepComment') public RepComment: ModalDirective;

  public paging: Paging;
  public q: QueryFilter;
  public IsAll: boolean;
  public listCommentPost = [];
  public Item: CommentPost;
  public Itemc: CommentPost;
  public ActionTable = ActionTable;
  public domain = domain;
  public ActionId: number;
  public CheckAll: boolean;
  public total: any;
  public listCustomer = [];
  public listNews = [];
  public ChoDuyet = 0;
  public DaDuyet = 0;
  public KhongDuyet = 0;
  public Spam = 0;
  public listId = [];
  public page_pp = [];
  public Checkitem: boolean;
  public CheckitemAll: boolean;

  public httpOptions: any;
  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public datePipe: DatePipe,
    public common: CommonService
  ) {
    this.Item = new CommentPost();
    this.Itemc = new CommentPost();
    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "NewsId Desc";
    this.paging.item_count = 0;
    this.IsAll = false;


    this.q = new QueryFilter();
    this.q.txtSearch = "";

    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }
  }

  ngOnInit() {
    this.GetListCommentPost();
    //this.GetListCustomer();
    //this.GetListNews();
    //this.GetListCommentPostChoDuyet();
    //this.GetListCommentPostDaDuyet();
    //this.GetListCommentPostKhongDuyet();
    //this.GetListCommentPostSpam();
  }


  //Danh sách tất cả bình luận
  GetListCommentPost() {
    this.page_pp = [];
    this.http.get('/api/comment/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size +'&query=TargetType=1 AND (Status=1 OR Status=2 OR Status=3)&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCommentPost = res["data"];
          this.paging.item_count = res["metadata"];
          this.total = res["metadata"];
          for (let i = 0; i < this.listCommentPost.length; i++) {
            this.listCommentPost[i].IsCheck = false;
          }
          this.paging.item_count = res["success"]["totalPages"];
          for (let i = 0; i < this.paging.item_count; i++) {
            this.page_pp.push(i);
          }
          
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  //Danh sách tất cả bình luận cho duyet
  GetListCommentPostChoDuyet() {
    this.http.get('/api/comment/GetByPage?page=' + this.paging.page + '&query=TargetType=1 AND Status=1&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.ChoDuyet = res["metadata"];

        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  //Danh sách tất cả bình luận cho duyet
  GetListCommentPostDaDuyet() {
    this.http.get('/api/comment/GetByPage?page=' + this.paging.page + '&query=TargetType=1 AND Status=2&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.DaDuyet = res["metadata"];

        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  //Danh sách tất cả bình luận cho duyet
  GetListCommentPostKhongDuyet() {
    this.http.get('/api/comment/GetByPage?page=' + this.paging.page + '&query=TargetType=1 AND Status=3&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.KhongDuyet = res["metadata"];

        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  //Danh sách tất cả bình luận SPAM
  GetListCommentPostSpam() {
    this.http.get('/api/comment/GetByPage?page=' + this.paging.page + '&query=TargetType=1 AND Status=4&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.Spam = res["metadata"];

        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }


  //Get danh sach khach hang
  GetListCustomer() {
    this.http.get('/api/customer/GetByPage?page=' + this.paging.page + '&query=' + this.paging.query + '&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCustomer = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  //Get danh sách danh bài viết
  GetListNews() {
    this.http.get('/api/news/GetByPage?page=' + this.paging.page + '&query=' + this.paging.query + '&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listNews = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  // Duyet binh luan
  DuyetComment(item,id) {
    this.Item = JSON.parse(JSON.stringify(item));
    this.http.put('/api/comment/ShowHide/' + item.CommentId + '/'+id, undefined, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListCommentPost();
          this.GetListCommentPostChoDuyet();
          this.GetListCommentPostDaDuyet();
          this.GetListCommentPostKhongDuyet();
          this.GetListCommentPostSpam();
          this.CommentModal.hide();
          this.toastSuccess("Duyệt bình luận thành công!");
        }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
      });
  }
  DeleteComment(item, id) {
    this.Item = JSON.parse(JSON.stringify(item));
    this.http.put('/api/comment/ShowHide/' + item.CommentId + '/' + id, undefined, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListCommentPost();
          this.GetListCommentPostChoDuyet();
          this.GetListCommentPostDaDuyet();
          this.GetListCommentPostKhongDuyet();
          this.GetListCommentPostSpam();
          this.CommentModal.hide();
          this.toastSuccess("Xoá BL thành công!");
        }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
      });
  }
  // Loc theo trang thai
  ChangeStatust(id) {
    if (id == 0) {
      this.GetListCommentPost();
      this.GetListCustomer();
      this.GetListNews();
      this.GetListCommentPostChoDuyet();
      this.GetListCommentPostDaDuyet();
      this.GetListCommentPostKhongDuyet();
      this.GetListCommentPostSpam();
    }
    else {
      this.http.get('/api/comment/GetByPage?page=' + this.paging.page + '&query=TargetType=1 AND Status =' + id + '&order_by=', this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.listCommentPost = res["data"];
            this.paging.item_count = res["metadata"];
          }
        },
        (err) => {
          console.log("Error: connect to API");
        }
      );

    }
  }
  ActionTableFunc() {
    switch (this.ActionId) {
      case 1:
        let data = [];
        this.listCommentPost.forEach(item => {
          if (item.Action == true) {
            data.push(item.NewsId);
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
                  this.http.put('/api/news/deletes', data, this.httpOptions).subscribe(
                    (res) => {
                      if (res["meta"]["error_code"] == 200) {
                        this.toastSuccess("Xóa thành công!");
                        this.GetListCommentPost();
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


  //Mở modal thêm mới
  OpenCommentModal(item) {
    this.Item = new CommentPost();
    this.Item.CommentParentId = 0;
    if (item != undefined) {
      this.Item = JSON.parse(JSON.stringify(item));
    }
    this.CommentModal.show();
  }
  OpenRepComment(item) {
    if (item != undefined) {
      this.Item = JSON.parse(JSON.stringify(item));
    }
    this.Item.CommentParentId = this.Item.CommentId;
    this.Item.Contents = undefined;
    this.Item.CommentId = undefined;
    this.Item.CustomerId = undefined
    this.RepComment.show();
  }
  //Thêm mới
  SaveCommnent() {
    if (this.Item.TargetId == undefined || this.Item.TargetId == null) {
      this.toastWarning("Chưa nhập bài viết cần bình luận!");
      return;
    } else if (this.Item.CustomerId == undefined || this.Item.CustomerId == null) {
      this.toastWarning("Chưa chọn khách hàng bình luận!");
      return;
    } else if (this.Item.Contents == undefined || this.Item.Contents == '') {
      this.toastWarning("Chưa nhập nội dung bình luận!!");
      return;
    }
    this.Item.TargetType = 1;
    this.Item.Status = 1;
   

    if (this.Item.CommentId) {
      this.http.put('/api/comment/' + this.Item.CommentId, this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListCommentPost();
            this.CommentModal.hide();
            this.toastSuccess("Cập nhật thành công!");
          }
          else {
            this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          }
        },
        (err) => {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        });
    }
    else {
      this.http.post('/api/comment', this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListCommentPost();
            this.GetListCommentPostChoDuyet();
            this.GetListCommentPostDaDuyet();
            this.GetListCommentPostKhongDuyet();
            this.GetListCommentPostSpam();
            this.CommentModal.hide();
            this.toastSuccess("Thêm thành công!");
          }
          else {
            this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
          }
        },
        (err) => {
          this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
        }
      );
    }
  }

  // tra loi binh luan
  ChangeCustomer(id) {
    console.log(id);
    this.Item.CustomerId = id;
  }
  SaveCommnentChild() {
    console.log(this.Item.CustomerId);
    if (this.Item.CustomerId == undefined || this.Item.CustomerId == null) {
      this.toastWarning("Chưa chọn khách hàng bình luận!");
      return;
    } else if (this.Item.Contents == undefined || this.Item.Contents == '') {
      this.toastWarning("Chưa nhập nội dung bình luận!!");
      return;
    }
    

    this.http.post('/api/comment', this.Item, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListCommentPost();
          this.GetListCommentPostChoDuyet();
          this.GetListCommentPostDaDuyet();
          this.GetListCommentPostKhongDuyet();
          this.GetListCommentPostSpam();
          this.RepComment.hide();
          this.toastSuccess("Trả lời thành công thành công!");
        }
        else {
          this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
      }
    );
  }

  //Chuyển trang
  PageChanged(event) {
    this.paging.page = event.page;
    this.GetListCommentPost();
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
            this.http.delete('/api/comment/' + Id, this.httpOptions).subscribe(
              (res) => {
                if (res["meta"]["error_code"] == 200) {
                  this.GetListCommentPost();
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
          buttonClass: 'btn btn-danger',

        }
      ],
    });
  }

  // Chon hoac bo chon tat ca binh luan
  CheckAllAddress() {
    this.listId = [];
    console.log(this.CheckitemAll);
    if (this.CheckitemAll == true) {
      for (let i = 0; i < this.listCommentPost.length; i++) {
        this.listCommentPost[i].IsCheck = false;
      }
    } else {
      for (let i = 0; i < this.listCommentPost.length; i++) {
        this.listCommentPost[i].IsCheck = true;
      }
    }

    for (let i = 0; i < this.listCommentPost.length; i++) {
      if (this.listCommentPost[i].IsCheck == true) {
        this.listId.push(this.listCommentPost[i].CommentId);
      }

    }

    console.log(this.listCommentPost);
    console.log(this.listId);

  }

  //Chon binh luan xoa
  CheckDeleteAddress(item) {
    console.log(item.IsCheck);
    this.listId = [];
    if (item.IsCheck == false || item.IsCheck == undefined) {
      for (let i = 0; i < this.listCommentPost.length; i++) {
        if (this.listCommentPost[i].CommentId == item.CommentId) {
          this.listCommentPost[i].IsCheck = false;
        }

      }
    } else {
      for (let i = 0; i < this.listCommentPost.length; i++) {
        if (this.listCommentPost[i].CommentId == item.CommentId) {
          this.listCommentPost[i].IsCheck = true;
        }

      }
    }

    for (let i = 0; i < this.listCommentPost.length; i++) {
      if (this.listCommentPost[i].IsCheck == true) {
        this.listId.push(this.listCommentPost[i].CommentId);
      }

    }
    if (this.listId.length < this.listCommentPost.length) {
      this.CheckitemAll = false;
    }
    if (this.listId.length == this.listCommentPost.length) {
      this.CheckitemAll = true;
    }
    console.log(this.listId);
  }

  //Xoa nhieu
  DeleteMuntiAddress() {
    console.log(this.listId.length);
    if (this.listId.length > 0) {
      this.http.put('/api/comment/deletes', this.listId, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListCommentPost();
            this.GetListCommentPostChoDuyet();
            this.GetListCommentPostDaDuyet();
            this.GetListCommentPostKhongDuyet();
            this.GetListCommentPostSpam();
            this.viewRef.clear();
            this.toastSuccess("Xóa thành công!");
            this.listId = [];
          }
          else {
            this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          }
        },
        (err) => {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        }
      );
    } else {
      this.toastError("Chưa có bình luận nào được chọn !");
    }
  }

}
