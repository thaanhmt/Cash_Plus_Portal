import { Component, ElementRef, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpEventType, HttpHeaders, HttpParams, HttpRequest } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConfigStar } from '../../../data/model';
import { ToastrService } from 'ngx-toastr';
import { domainDebug, domainMedia, listOperators, TypeUpload } from '../../../data/const';
import { domain } from 'process';
import { Paging } from '../../../data/dt';

@Component({
  selector: 'app-config-star',
  templateUrl: './config-star.component.html',
  styleUrls: ['./config-star.component.scss']
})

export class ConfigStarComponent implements OnInit {
  @ViewChild('file') file: ElementRef;
  @ViewChild('OpenMediaFile') public OpenMediaFile: ModalDirective;

  public listConfigStar = [];
  public listOperators = listOperators;
  public CompanyId = parseInt(localStorage.getItem("companyId"));
  public ckeConfig: any;
  //public typeUpload = TypeUpload;
  public Item: ConfigStar;
  public httpOptions: any;
  public isNoitify: boolean = false;
  public paging: Paging;

/*  public listItemMedia = [];*/
  public domainMedia = domainMedia;
  public domain = domain;
  //public isActiveMedia: boolean = true;
  //public isActiveUpload: boolean = false;
  //public isDelay: boolean = false;
  //public message_video: string;
  //public progressAttachment: number;
  //public countMedia: number;
  //public countAllMedia: number;
  //public pagingFile: Paging;
  //public progress: number;
  //public message: string;
  public staticDomain: string;
  public staticDomainMedia: string;
  public domainDebug = domainDebug;
  public selectMedia: number;
  public tabActive: number;

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
  ) {
    this.Item = new ConfigStar();
    //
    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 5;
    this.paging.query = "1=1";
    this.paging.order_by = "";
    this.paging.item_count = 0;
    this.tabActive = 1;

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
    this.GetListConfigStar();
    this.GetDomainStatic();
  }

  GetListConfigStar() {
    this.http.get('/api/configStar/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listConfigStar = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    )
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

  changeTab(tab): void {
    this.tabActive = tab;
  }

  Update() {
    if (this.tabActive == 1) {
      //if (this.Item.Name == undefined || this.Item.Name == '') {
      //  this.toastWarning("Chưa nhập Tên!");
      //  return;
      //} else if (this.Item.Name.replace(/ /g, '') == '') {
      //  this.toastWarning("Chưa nhập tên!");
      //  return;
      //} else if (this.Item.Url == undefined || this.Item.Url == '') {
      //  this.toastWarning("Chưa nhập đường dẫn!");
      //  return;
      //} else if (this.Item.Url.replace(/ /g, '') == '') {
      //  this.toastWarning("Chưa nhập đường dẫn!");
      //  return;
      //}
      //else if (this.Item.LanguageId == undefined) {
      //  this.toastWarning("Chưa chọn ngôn ngữ!");
      //  return;
      //}

      //this.Item.UserId = parseInt(localStorage.getItem("userId"));
      //this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));

      //let obj = JSON.parse(JSON.stringify(this.Item));

      this.http.put('/api/configStar', this.listConfigStar, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
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
  //tabHandleMedia() {
  //  this.isActiveMedia = true;
  //  this.isActiveUpload = false;
  //}
  //tabHandleMediaUpload() {
  //  this.isActiveMedia = false;
  //  this.isActiveUpload = true;
  //}
  //upload3(files, cs) {
  //  if (files.length === 0)
  //    return;

  //  const formData = new FormData();

  //  for (let file of files)
  //    formData.append(file.name, file);

  //  const uploadReq = new HttpRequest('POST', 'api/upload/uploadMedia/8', formData, {
  //    headers: new HttpHeaders({
  //      'Authorization': 'bearer ' + localStorage.getItem("access_token")
  //    }),
  //    reportProgress: true,
  //  });

  //  this.http.request(uploadReq).subscribe(event => {
  //    if (event.type === HttpEventType.UploadProgress)
  //      switch (cs) {
  //        case 1:
  //          this.progress = Math.round(100 * event.loaded / event.total);
  //          break;
  //        default:
  //          break;
  //      }
  //    else if (event.type === HttpEventType.Response) {

  //      switch (cs) {
  //        case 1:
  //          this.isActiveMedia = true;
  //          this.isActiveUpload = false;
  //          this.pagingFile.page = 1;
  //          this.GetListFiles();
  //          this.message = event.body["data"].toString();
  //          this.toastSuccess("Tải lên thành công");
  //          break;
  //        default:
  //          break;
  //      }
  //    }
  //  });
  //}

  //loadMore() {
  //  this.isDelay = true;
  //  setTimeout(
  //    () => {
  //      this.isDelay = false;
  //      this.pagingFile.page++;
  //      this.http.get('/api/fileManager/GetFiles?page=' + this.pagingFile.page + '&page_size=' + this.pagingFile.page_size + '&query='
  //        + this.pagingFile.query + '&order_by=' + '&select=' + this.pagingFile.select, this.httpOptions).subscribe(
  //          (res) => {
  //            if (res["meta"]["error_code"] == 200) {
  //              this.listItemMedia.push(...res["data"]);
  //              if (this.countMedia >= this.countAllMedia) {
  //                this.countMedia = this.countAllMedia;
  //              } else {
  //                if ((this.countMedia + 24) >= this.countAllMedia) {
  //                  this.countMedia = this.countAllMedia;
  //                } else {
  //                  this.countMedia += 24;
  //                }
  //              }
  //            }
  //          },
  //          (err) => {
  //            console.log("Error: connect to API");
  //          }
  //        );
  //    },
  //    1000
  //  );
  //}

  //GetListFiles() {
  //  this.http.get('/api/fileManager/GetFiles?page=' + this.pagingFile.page + '&page_size=' + this.pagingFile.page_size + '&query='
  //    + this.pagingFile.query + '&order_by=' + '&select=' + this.pagingFile.select, this.httpOptions).subscribe(
  //      (res) => {
  //        if (res["meta"]["error_code"] == 200) {
  //          this.listItemMedia = res["data"];
  //          this.countAllMedia = res["metadata"];
  //          if (this.countAllMedia < 24) this.countMedia = this.countAllMedia;
  //        }
  //      },
  //      (err) => {
  //        console.log("Error: connect to API");
  //      }
  //    );
  //}

  //SeclectMedia(item) {
  //  if (this.selectMedia == 0)
  //    this.Item.LogoHeader = item.url + "/" + item.name;
  //  else if (this.selectMedia == 1)
  //    this.Item.Icon1 = item.url + "/" + item.name;
  //  else if (this.selectMedia == 2)
  //    this.Item.Icon2 = item.url + "/" + item.name;
  //  else if (this.selectMedia == 3)
  //    this.Item.Icon3 = item.url + "/" + item.name;
  //  else if (this.selectMedia == 4)
  //    this.Item.Icon4 = item.url + "/" + item.name;
  //  else if (this.selectMedia == 5)
  //    this.Item.Icon5 = item.url + "/" + item.name;
  //  else if (this.selectMedia == 6)
  //    this.Item.Icon6 = item.url + "/" + item.name;
  //  else if (this.selectMedia == 7)
  //    this.Item.IconBct = item.url + "/" + item.name;
  //  this.OpenMediaFile.hide();
  //}

  //RemoveImage(key) {
  //  switch (key) {
  //    case 0:
  //      this.file.nativeElement.value = "";
  //      this.Item.LogoHeader = undefined;
  //      this.message = undefined;
  //      this.progress = undefined;
  //      break;
  //    case 1:
  //      this.file.nativeElement.value = "";
  //      this.Item.Icon1 = undefined;
  //      this.message = undefined;
  //      this.progress = undefined;
  //      break;
  //    case 2:
  //      this.file.nativeElement.value = "";
  //      this.Item.Icon2 = undefined;
  //      this.message = undefined;
  //      this.progress = undefined;
  //      break;
  //    case 3:
  //      this.file.nativeElement.value = "";
  //      this.Item.Icon3 = undefined;
  //      this.message = undefined;
  //      this.progress = undefined;
  //      break;
  //    case 4:
  //      this.file.nativeElement.value = "";
  //      this.Item.Icon4 = undefined;
  //      this.message = undefined;
  //      this.progress = undefined;
  //      break;
  //    case 5:
  //      this.file.nativeElement.value = "";
  //      this.Item.Icon5 = undefined;
  //      this.message = undefined;
  //      this.progress = undefined;
  //      break;
  //    case 6:
  //      this.file.nativeElement.value = "";
  //      this.Item.Icon6 = undefined;
  //      this.message = undefined;
  //      this.progress = undefined;
  //      break;
  //    case 7:
  //      this.file.nativeElement.value = "";
  //      this.Item.IconBct = undefined;
  //      this.message = undefined;
  //      this.progress = undefined;
  //      break;
  //    //case 8:
  //    //  this.fileFooter.nativeElement.value = "";
  //    //  this.Item.LogoFooter = undefined;
  //    //  this.messageFooter = undefined;
  //    //  this.progressFooter = undefined;
  //    //  break;
  //    //case 9:
  //    //  this.Item.Banner = undefined;
  //    //  this.messageBanner = undefined;
  //    //  this.progressBanner = undefined;
  //    //  break;
  //    default:
  //      break;
  //  }
  //}

}
