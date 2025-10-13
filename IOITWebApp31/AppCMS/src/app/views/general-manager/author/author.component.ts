import { Component, ElementRef, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { HttpClient, HttpEventType, HttpHeaders, HttpRequest } from '@angular/common/http';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { ToastrService } from 'ngx-toastr';
import { CommonService } from '../../../service/common.service';
import { Paging, QueryFilter } from '../../../data/dt';
import { Author, Function } from '../../../data/model';
import { ActionTable, domainImage, listTypeAuthor, domainMedia, domain, domainDebug } from '../../../data/const';

@Component({
  selector: 'app-author',
  templateUrl: './author.component.html',
  styleUrls: ['./author.component.scss']
})

export class AuthorComponent implements OnInit {
  @ViewChild('modalFunction') public modalFunction: ModalDirective;
  @ViewChild('file') file: ElementRef;
  @ViewChild('OpenMediaFile') public OpenMediaFile: ModalDirective;

  public isActiveMedia: boolean = true;
  public isActiveUpload: boolean = false;
  public isDelay: boolean = false;

  public pagingFile: Paging;
  public countMedia: number;
  public countAllMedia: number;

  public listItemMedia = [];
  public domainMedia = domainMedia;
  public domain = domain;

  public message: string;
  public progress: number;

  public paging: Paging;
  public q: QueryFilter;

  public listAuthors = [];
  public listUsers = [];
  public listTypeAuthor = listTypeAuthor;

  public Item: Author;

  public httpOptions: any;

  public ActionTable = ActionTable;
  public ActionId: number;
  public CheckAll: boolean;
  public isNoitify: boolean = false;

  public domainImage = domainImage;
  public staticDomain: string;
  public staticDomainMedia: string;
  public domainDebug = domainDebug;
  constructor(public http: HttpClient, public modalDialogService: ModalDialogService, public viewRef: ViewContainerRef, public toastr: ToastrService) {
    this.Item = new Author();

    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "AuthorId Desc";
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

    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }

  }

  ngOnInit() {
    this.GetListAuthor();
    this.GetListUser();
    this.GetListFiles();
    this.GetDomainStatic();
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
  GetListAuthor() {
    this.http.get('/api/author/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listAuthors = res["data"];
          this.paging.item_count = res["metadata"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      });
  }

  GetListUser() {
    this.http.get('/api/userRole/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listUsers = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      });
  }

  //Chuyển trang
  PageChanged(event) {
    this.paging.page = event.page;
    this.GetListAuthor();
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
    let query = '1=1';
    if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
        query += ' and Name.Contains("' + this.q.txtSearch + '")';
    }
    if (this.q["Type"] != undefined) {
      query += ' and Type=' + this.q["Type"];
    }

      this.paging.query = query;

    this.GetListAuthor();
  }

  OpenModalFunction(item) {
    this.Item = new Author();
    if (this.file) this.file.nativeElement.value = "";
    this.message = undefined;
    this.progress = undefined;
    if (item == undefined) {
      //this.GetListFunctionParent(undefined);
    }
    else {
      this.Item = Object.assign(this.Item, item);
      //if (this.Item.FunctionParentId == 0) this.Item.FunctionParentId = undefined;
      //this.GetListFunctionParent(item.FunctionId);
    }

    this.modalFunction.show();
  }

  SaveFunc() {
    if (this.Item.Name == undefined || this.Item.Name == '') {
      this.toastWarning("Chưa nhập Tên tác giả!");
      return;
    } else if (this.Item.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập Tên tác giả!");
      return;
    } else if (this.Item.FullName.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập Tên đầy đủ tác giả!");
      return;
    } else if (this.Item.FullName == undefined || this.Item.FullName == '') {
      this.toastWarning("Chưa nhập Tên đầy đủ tác giả!");
      return;
    } else if (this.Item.Address == undefined || this.Item.Address == '') {
      this.toastWarning("Chưa nhập địa chỉ tác giả!");
      return;
    } else if (this.Item.Address.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập địa chỉ tác giả!");
      return;
    } else if (this.Item.NumberPhone == undefined || this.Item.NumberPhone == '') {
      this.toastWarning("Chưa nhập số điện thoại tác giả!");
      return;
    } else if (this.Item.Cccd == undefined || this.Item.Cccd == '') {
      this.toastWarning("Chưa nhập Căn cước/CMT tác giả!");
      return;
    }
    //this.Item.Type = 1;
    this.Item.UserId = parseInt(localStorage.getItem("userId"));
    let obj = JSON.parse(JSON.stringify(this.Item));

    if (this.Item.AuthorId == undefined) {
      this.http.post('/api/author', this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListAuthor();
            this.modalFunction.hide();
            this.toastSuccess("Thêm mới thành công!");
          }
          else if (res["meta"]["error_code"] == 211) {
            this.toastError("Thông tin không đủ. Xin vui lòng thử lại!");
          }
          //else if (res["meta"]["error_code"] == 212) {
          //  this.toastError("Mã chức năng đã tồn tại. Xin vui lòng thử lại!");
          //}
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

      this.http.put('/api/author/' + obj.AuthorId, obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListAuthor();
            this.modalFunction.hide();
            this.toastSuccess("Cập nhật thành công!");
          }
          else if (res["meta"]["error_code"] == 211) {
            this.toastError("Thông tin không đủ. Xin vui lòng thử lại!");
          }
          //else if (res["meta"]["error_code"] == 212) {
          //  this.toastError("Mã chức năng đã tồn tại. Xin vui lòng thử lại!");
          //}
          //else if (res["meta"]["error_code"] == 215) {
          //  this.toastError("Chức năng cha không hợp lệ. Xin vui lòng thử lại!");
          //}
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
    this.http.delete('/api/author/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListAuthor();
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

    this.GetListAuthor();
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

  CheckActionTable(Id) {
    if (Id == undefined) {
      let CheckAll = this.CheckAll;
      this.listAuthors.forEach(item => {
        item.Action = CheckAll;
      });
    }
    else {
      let CheckAll = true;
      for (let i = 0; i < this.listAuthors.length; i++) {
        if (!this.listAuthors[i].Action) {
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
        this.listAuthors.forEach(item => {
          if (item.Action == true) {
            data.push(item.AuthorId);
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
                  this.http.put('/api/author/deletes', data, this.httpOptions).subscribe(
                    (res) => {
                      if (res["meta"]["error_code"] == 200) {
                        this.toastSuccess("Xóa thành công!");
                        this.GetListAuthor();
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

  upload(files, cs) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);

    const uploadReq = new HttpRequest('POST', 'api/upload/uploadImage/8', formData, {
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
            this.message = event.body["data"].toString();
            this.Item.Avatar = this.message;
            break;
          default:
            break;
        }
      }
    });
  }

  RemoveImage() {
    this.Item.Avatar = undefined;
    this.file.nativeElement.value = "";
    this.message = undefined;
    this.progress = undefined;
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
  RemoveImageAvatar() {
    this.Item.Avatar = undefined;
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
    this.Item.Avatar = item.url + "/" + item.name;
    this.OpenMediaFile.hide();
  }
}
