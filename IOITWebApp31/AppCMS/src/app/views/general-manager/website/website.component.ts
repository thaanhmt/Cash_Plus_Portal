import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { ToastrService } from 'ngx-toastr';
import { domainImage, ActionTable, domainMedia, domain, domainDebug } from '../../../data/const';
import { Website } from '../../../data/model';
import { Paging, QueryFilter } from '../../../data/dt';

@Component({
  selector: 'app-website',
  templateUrl: './website.component.html',
  styleUrls: ['./website.component.scss']
})
export class WebsiteComponent implements OnInit {
  @ViewChild('WebsiteModal') public WebsiteModal: ModalDirective;
  @ViewChild('fileHeader') fileHeader: ElementRef;
  @ViewChild('fileFooter') fileFooter: ElementRef;

  @ViewChild('OpenMediaFile') public OpenMediaFile: ModalDirective;
  @ViewChild('OpenMediaFileVideo') public OpenMediaFileVideo: ModalDirective;

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
  public listWebsite = [];
  public listWebsiteParent = [];
  public ckeConfig: any;
  public Item: Website;

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

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
  ) {
    this.Item = new Website();
    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "WebsiteId Desc";
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
    this.ckeConfig = {
      allowedContent: false,
      extraPlugins: 'divarea',
      forcePasteAsPlainText: true
    };

    this.GetListWebsite();
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
  //GET
  GetListWebsite() {
    this.http.get('/api/website/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listWebsite = res["data"];
          this.paging.item_count = res["metadata"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListWebsiteParent(Id) {
    this.http.get('/api/website/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listWebsiteParent = res["data"];
          this.listWebsiteParent.forEach(item => {
            item.Space = "";
            for (var i = 0; i < this.listWebsiteParent.length; i++) {
              item.Space += "&nbsp;";
              if (item.WebsiteId == Id) {
                item.disabled = true;
              }
            }
          })
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
    this.GetListWebsite();
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
        query += ' and Name.Contains("' + this.q.txtSearch + '")';
      }
      else {
        query += 'Name.Contains("' + this.q.txtSearch + '")';
      }
    }

    if (query == '')
      this.paging.query = '1=1';
    else
      this.paging.query = query;

    this.GetListWebsite();
  }

  //Mở modal
  OpenWebsiteModal(item) {
    this.Item = new Website();
    this.fileHeader.nativeElement.value = "";
    this.fileFooter.nativeElement.value = "";

    this.messageBanner = undefined;
    this.messageFooter = undefined;
    this.messageHeader = undefined;

    this.progressHeader = undefined;
    this.progressFooter = undefined;
    this.progressBanner = undefined;

    if (item == undefined) {
      this.GetListWebsiteParent(undefined);
    }
    else {
      this.Item = JSON.parse(JSON.stringify(item));
      if (this.Item.WebsiteParentId == 0) this.Item.WebsiteParentId = undefined;
      this.GetListWebsiteParent(this.Item.WebsiteId);
    }

    this.GetListLanguage();
    this.WebsiteModal.show();
  }

  //Thêm mới
  SaveWebsite() {
    if (this.Item.Name == undefined || this.Item.Name == '') {
      this.toastWarning("Chưa nhập Tên!");
      return;
    } else if (this.Item.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên!");
      return;
    } else if (this.Item.Url == undefined || this.Item.Url == '') {
      this.toastWarning("Chưa nhập đường dẫn!");
      return;
    } else if (this.Item.Url.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập đường dẫn!");
      return;
    }
    //else if (this.Item.LanguageId == undefined) {
    //  this.toastWarning("Chưa chọn ngôn ngữ!");
    //  return;
    //}

    this.Item.UserId = parseInt(localStorage.getItem("userId"));
    this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));

    let obj = JSON.parse(JSON.stringify(this.Item));
    obj.WebsiteParentId = obj.WebsiteParentId == undefined ? 0 : obj.WebsiteParentId;

    if (this.Item.WebsiteId == undefined) {
      this.http.post('/api/website', obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListWebsite();
            this.WebsiteModal.hide();
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
      this.http.put('/api/website/' + obj.WebsiteId, obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListWebsite();
            this.WebsiteModal.hide();
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
    this.http.delete('/api/Website/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListWebsite();
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

  upload(files, key) {
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
      if (event.type === HttpEventType.UploadProgress) {
        switch (key) {
          case 1:
            this.progressHeader = Math.round(100 * event.loaded / event.total);
            break;
          case 2:
            this.progressFooter = Math.round(100 * event.loaded / event.total);
            break;
          case 3:
            this.progressBanner = Math.round(100 * event.loaded / event.total);
            break;
          default:
            break;
        }
      }
      else if (event.type === HttpEventType.Response) {
        switch (key) {
          case 1:
            this.messageHeader = event.body["data"].toString();
            this.Item.LogoHeader = this.messageHeader
            break;
          case 2:
            this.messageFooter = event.body["data"].toString();
            this.Item.LogoFooter = this.messageFooter
            break;
          case 3:
            this.messageBanner = event.body["data"].toString();
            this.Item.Banner = this.messageBanner
            break;
          default:
            break;
        }
      }
    });
  }

  findParent(item) {
    if (item == undefined) {
      return "";
    }
    else {
      return item.Name;
    }
  }

  RemoveImage(key) {
    console.log("aaaa");
    console.log(key);
    switch (key) {
      case 0:
        this.fileHeader.nativeElement.value = "";
        this.Item.LogoHeader = undefined;
        this.messageHeader = undefined;
        this.progressHeader = undefined;
        break;
      case 1:
        this.fileHeader.nativeElement.value = "";
        this.Item.Icon1 = undefined;
        this.messageHeader = undefined;
        this.progressHeader = undefined;
        break;
      case 2:
        this.fileHeader.nativeElement.value = "";
        this.Item.Icon2 = undefined;
        this.messageHeader = undefined;
        this.progressHeader = undefined;
        break;
      case 3:
        this.fileHeader.nativeElement.value = "";
        this.Item.Icon3 = undefined;
        this.messageHeader = undefined;
        this.progressHeader = undefined;
        break;
      case 4:
        this.fileHeader.nativeElement.value = "";
        this.Item.Icon4 = undefined;
        this.messageHeader = undefined;
        this.progressHeader = undefined;
        break;
      case 5:
        this.fileHeader.nativeElement.value = "";
        this.Item.Icon5 = undefined;
        this.messageHeader = undefined;
        this.progressHeader = undefined;
        break;
      case 6:
        this.fileHeader.nativeElement.value = "";
        this.Item.Icon6 = undefined;
        this.messageHeader = undefined;
        this.progressHeader = undefined;
        break;
      case 7:
        this.fileHeader.nativeElement.value = "";
        this.Item.IconBct = undefined;
        this.messageHeader = undefined;
        this.progressHeader = undefined;
        break;
      case 8:
        this.fileFooter.nativeElement.value = "";
        this.Item.LogoFooter = undefined;
        this.messageFooter = undefined;
        this.progressFooter = undefined;
        break;
      case 9:
        this.Item.Banner = undefined;
        this.messageBanner = undefined;
        this.progressBanner = undefined;
        break;
      default:
        break;
    }
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

    this.GetListWebsite();
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

  CheckActionTable(WebsiteId) {
    if (WebsiteId == undefined) {
      let CheckAll = this.CheckAll;
      this.listWebsite.forEach(item => {
        item.Action = CheckAll;
      });
    }
    else {
      let CheckAll = true;
      for (let i = 0; i < this.listWebsite.length; i++) {
        if (!this.listWebsite[i].Action) {
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
        this.listWebsite.forEach(item => {
          if (item.Action == true) {
            data.push(item.WebsiteId);
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
                  this.http.put('/api/website/deletes', data, this.httpOptions).subscribe(
                    (res) => {
                      if (res["meta"]["error_code"] == 200) {
                        this.toastSuccess("Xóa thành công!");
                        this.GetListWebsite();
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

  OpenMediaModal(type) {
    this.selectMedia = type;
    this.OpenMediaFile.show();
  }
  CloseMediaModal() {
    this.OpenMediaFile.hide();
  }
  OpenMediaModalVideo() {
    this.OpenMediaFileVideo.show();
  }
  CloseMediaModalVideo() {
    this.OpenMediaFileVideo.hide();
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

  SeclectMedia(item) {
    if(this.selectMedia == 0)
      this.Item.LogoHeader = item.url + "/" + item.name;
    else if (this.selectMedia == 1)
      this.Item.Icon1 = item.url + "/" + item.name;
    else if (this.selectMedia == 2)
      this.Item.Icon2 = item.url + "/" + item.name;
    else if (this.selectMedia == 3)
      this.Item.Icon3 = item.url + "/" + item.name;
    else if (this.selectMedia == 4)
      this.Item.Icon4 = item.url + "/" + item.name;
    else if (this.selectMedia == 5)
      this.Item.Icon5 = item.url + "/" + item.name;
    else if (this.selectMedia == 6)
      this.Item.Icon6 = item.url + "/" + item.name;
    else if (this.selectMedia == 7)
      this.Item.IconBct = item.url + "/" + item.name;
    this.OpenMediaFile.hide();
  }
  SeclectMediaVideo(item) {
    this.Item.LogoFooter = item.url + "/" + item.name;
    this.OpenMediaFileVideo.hide();
  }
}
