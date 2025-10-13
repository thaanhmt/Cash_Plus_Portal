import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { ToastrService } from 'ngx-toastr';
import { domainImage, ActionTable, domainMedia, domain, domainDebug, listNumberImages } from '../../../data/const';
import { TypeSlide } from '../../../data/model';
import { CheckRole, Paging, QueryFilter } from '../../../data/dt';
import { CommonService } from '../../../service/common.service';

@Component({
  selector: 'app-type-slide',
  templateUrl: './type-slide.component.html',
  styleUrls: ['./type-slide.component.scss']
})

export class TypeSlideComponent implements OnInit {
  @ViewChild('CreateUpdateModal') public CreateUpdateModal: ModalDirective;

  public listItemMedia = [];
  public domainMedia = domainMedia;
  public domain = domain;
  public isActiveMedia: boolean = true;
  public isActiveUpload: boolean = false;
  public isDelay: boolean = false;
  public message_video: string;
  public progressAttachment: number;
  public countMedia: number;
  public countAllMedia: number;
  public pagingFile: Paging;

  public paging: Paging;
  public q: QueryFilter;

  public listLanguage = [];
  public Image = [];
  public listDatas = [];
  public listNumberImages = listNumberImages;
  public ckeConfig: any;
  public Item: TypeSlide;

  public progressHeader: number;
  public messageHeader: string;

  public progressFooter: number;
  public messageFooter: string;

  public progressBanner: number;
  public messageBanner: string;

  public progress: number;
  public message: string;

  public domainImage = domainImage;

  public httpOptions: any;
  public file: any;
  public ActionTable = ActionTable;
  public ActionId: number;
  public CheckAll: boolean;
  public isNoitify: boolean = false;

  public staticDomain: string;
  public staticDomainMedia: string;
  public domainDebug = domainDebug;
  public selectMedia: number;

  public CheckRole: CheckRole;

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public common: CommonService
  ) {
    this.Item = new TypeSlide();
    this.Item.Status = 1;
    this.Item.StatusView = true;
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

    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }

    this.CheckRole = new CheckRole();
    let code = "DMSL";
    this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 0);
    this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 1);
    this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 2);
    this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 3);
    this.CheckRole.Export = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 5);

  }

  ngOnInit() {
    this.ckeConfig = {
      allowedContent: false,
      extraPlugins: 'divarea',
      forcePasteAsPlainText: true
    };

    this.GetListData();
    //this.GetListFiles();
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
  //GET
  GetListData() {
    this.http.get('/api/typeSlide/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listDatas = res["data"];
          this.listDatas.forEach(item => {
            item.IsShow = item.Status == 1 ? true : false;
          });
          this.paging.item_count = res["metadata"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  // Get danh sách ngôn ngữ
  GetListLanguage() {
    this.http.get('/api/Language/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listLanguage = res["data"];
          if (this.listLanguage.length == 1 && (this.Item.WebsiteId == undefined || (this.Item.WebsiteId != undefined && this.Item.LanguageId == undefined))) {
            this.Item.LanguageId = this.listLanguage[0].LanguageId;
          }
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
    this.GetListData();
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
    let query = '';
    if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
      if (query != '') {
        query += ' AND Name.Contains("' + this.q.txtSearch + '") OR Code.Contains("' + this.q.txtSearch + '")';
      }
      else {
        query += 'Name.Contains("' + this.q.txtSearch + '") OR Code.Contains("' + this.q.txtSearch + '")';
      }
    }

    if (query == '')
      this.paging.query = '1=1';
    else
      this.paging.query = query;

    this.GetListData();
  }

  //Mở modal
  OpenDataModal(item) {
    this.Item = new TypeSlide();
    this.Item.StatusView = true;
    if (item != undefined) {
      this.Item = JSON.parse(JSON.stringify(item));
      if (this.Item.Status == 1)
        this.Item.StatusView = true;
      else
        this.Item.StatusView = false;
    }

    this.GetListLanguage();
    this.CreateUpdateModal.show();
  }

  //Thêm mới
  SaveData() {
    if (this.Item.Name == undefined || this.Item.Name == '') {
      this.toastWarning("Chưa nhập tên loại slide!");
      return;
    } else if (this.Item.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên loại slide!");
      return;
    } else if (this.Item.Code == undefined || this.Item.Code == '') {
      this.toastWarning("Chưa nhập mã loại slide!");
      return;
    } else if (this.Item.Code.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập mã loại slide!");
      return;
    }
    else if (this.Item.NumberImage == undefined) {
      this.toastWarning("Chưa nhập số ảnh hiển thị!");
      return;
    }
    else if (this.Item.TimeReset == undefined || this.Item.TimeReset < 0) {
      this.toastWarning("Chưa nhập số giây chuyển!");
      return;
    } 
    //else if (this.Item.LanguageId == undefined) {
    //  this.toastWarning("Chưa chọn ngôn ngữ!");
    //  return;
    //}

    this.Item.CreatedId = parseInt(localStorage.getItem("userId"));
    this.Item.UpdatedId = parseInt(localStorage.getItem("userId"));
    this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));
    this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
    this.Item.LanguageId = 1;

    let obj = JSON.parse(JSON.stringify(this.Item));
    if (obj.StatusView)
      obj.Status = 1;
    else
      obj.Status = 10;
    if (this.Item.TypeSlideId == undefined) {
      this.http.post('/api/typeSlide', obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListData();
            this.CreateUpdateModal.hide();
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
    else {
      this.http.put('/api/typeSlide/' + obj.TypeSlideId, obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListData();
            this.CreateUpdateModal.hide();
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
    this.http.delete('/api/typeSlide/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListData();
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

  ConfirmShowHide(item, i) {
    this.modalDialogService.openDialog(this.viewRef, {
      title: 'Xác nhận',
      childComponent: SimpleModalComponent,
      data: {
        text: "Bạn có chắc chắn muốn thay đổi trạng thái bản ghi này?"
      },
      actionButtons: [
        {
          text: 'Đồng ý',
          buttonClass: 'btn btn-success',
          onAction: () => {
            this.ShowHide(item, i);
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-danger',
          onAction: () => {
            item.IsShow = !item.IsShow;
            this.viewRef.clear()
          }
        }
      ],
    });
  }

  ShowHide(item, i) {
    let stt = item.IsShow ? 1 : 10;
    this.http.put('/api/typeSlide/ShowHide/' + item.TypeSlideId + "/" + stt, undefined, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
            this.toastSuccess("Thay đổi trạng thái thành công!");
          this.GetListData();
          this.viewRef.clear();
        }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          item.IsShow = !item.IsShow;
          this.viewRef.clear();
        }

      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        item.IsShow = !item.IsShow;
        this.viewRef.clear();
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

    this.GetListData();
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
      this.listDatas.forEach(item => {
        item.Action = CheckAll;
      });
    }
    else {
      let CheckAll = true;
      for (let i = 0; i < this.listDatas.length; i++) {
        if (!this.listDatas[i].Action) {
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
        this.listDatas.forEach(item => {
          if (item.Action == true) {
            data.push(item.TypeSlideId);
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
                  this.http.put('/api/typeSlide/deletes', data, this.httpOptions).subscribe(
                    (res) => {
                      if (res["meta"]["error_code"] == 200) {
                        this.toastSuccess("Xóa thành công!");
                        this.GetListData();
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


}
