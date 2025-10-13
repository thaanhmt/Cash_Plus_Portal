import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef, Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { ActionTable, listTypeMedia, listItemMedia, domainMedia, domain, domainDebug } from '../../data/const';
import { User, Upload} from '../../data/model';
import { ToastrService } from 'ngx-toastr';
import { CheckRole, Paging, QueryFilter } from '../../data/dt';


@Component({
  selector: 'app-uploadt',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.scss'],
  providers: [

  ]
})
export class UploadComponent implements OnInit {
  public isNoitify: boolean = false;
  public isDelay: boolean = false;
  public ActionTable = ActionTable;
  public listTypeMedia = listTypeMedia;
  public listItemMedia = listItemMedia;
  public domainMedia = domainMedia;
  public domain = domain;
  public progress: number;
  public message: string;
  public httpOptions: any;
  public DetailMediaName:string;
  public DetailMediaType: number;
  public DetailMediaExtension: string;
  public DetailMediaDate: Date;
  public DetailMediaSize: string;
  public DetailMediaUrl: string;
  public DetailMediaWidth: number;
  public DetailMediaHeight: number;
  public DetailMediaAlt: number;
  public DetailMediaNote: number;
  public DetailMediaUserName: number;
  //
  public searchMedia: string;
  public typeMedia: number;
  public pagingFile: Paging;
  public countMedia: number;
  public countAllMedia: number;

  public staticDomain: string;
  public staticDomainMedia: string;
  public domainDebug = domainDebug;
  //
  @ViewChild('NewUploadModal') public NewUploadModal: ModalDirective;
  @ViewChild('UploadModal') public UploadModal: ModalDirective;
  @ViewChild('file') file: ElementRef;
  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public sanitizer: DomSanitizer
  ) {
    this.pagingFile = new Paging();
    this.pagingFile.page = 1;
    this.pagingFile.page_size = 24;
    this.pagingFile.query = "1=1";
    this.pagingFile.order_by = "";
    this.pagingFile.item_count = 0;
    this.countMedia = 24;
    //
    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }

  }

  ngOnInit() {
    this.GetListFiles();
    this.GetDomainStatic();
  }
  GetDomainStatic() {
    this.http.get('api/Config/1', this.httpOptions).subscribe(
      (res) => {
        this.staticDomain = res["data"].Website;
        if (res["meta"]["error_code"] == 200) {
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
  /* To copy Text from Textbox */
  copyInputMessage(inputElement) {
    inputElement.select();
    document.execCommand('copy');
    inputElement.setSelectionRange(0, 0);
    this.toastSuccess("Đã copy vào bộ nhớ tạm!");
  }
  OpenAddUploadModal(item, type) {
    this.file.nativeElement.value = "";
    this.message = undefined;
    this.progress = undefined;
    this.NewUploadModal.show();
  }
  OpenMediaModal(item) {
    this.UploadModal.show();
    this.DetailMediaName = item.name;
    this.DetailMediaType = item.type;
    this.DetailMediaExtension = item.extension;
    this.DetailMediaDate = item.dateCreate;
    this.DetailMediaSize = item.size;
    this.DetailMediaUrl = item.url;
    this.DetailMediaWidth = item.width;
    this.DetailMediaHeight = item.height;
    this.DetailMediaAlt = item.alt;
    this.DetailMediaNote = item.note;
    this.DetailMediaUserName = item.userName;
  }
  CloseMediaModal() {
    this.UploadModal.hide();
  }
  CloseAddUploadModal() {
    this.NewUploadModal.hide();
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
            this.GetListFiles();
            this.message = event.body["data"].toString();
            this.toastSuccess("Tải lên thành công");
            this.file.nativeElement.value = "";
            this.message = undefined;
            this.progress = undefined;
            this.CloseAddUploadModal();
            break;
          default:
            break;
        }
      }
    });
  }

  RemoveImage() {
    this.message = undefined;
    this.progress = undefined;
  }

  QueryTypeMedia() {
    this.pagingFile.page = 1;
    this.countMedia = 24;
    this.pagingFile.select = undefined;
    if (this.typeMedia!=undefined)
      this.pagingFile.select = this.typeMedia+"";
    this.GetListFiles();
  }

  QuerySearchMedia() {
    this.pagingFile.page = 1;
    this.countMedia = 24;
    let query = "1=1";
    if (this.searchMedia != undefined && this.searchMedia != '') {
      if (query != '') {
        query += ' and name.Contains("' + this.searchMedia + '")';
      }
    }
    this.pagingFile.query = query;
    this.GetListFiles();
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

  UpdateMedia() {
    let obj = {};
    obj["Name"] = this.DetailMediaName;
    obj["TargetId"] = this.DetailMediaType;
    obj["Url"] = this.DetailMediaAlt;
    obj["Thumb"] = this.DetailMediaNote;
    obj["Note"] = this.DetailMediaName;

    this.http.post('/api/FileManager/UpdateInfoFile', obj, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListFiles();
          this.CloseMediaModal();
          this.viewRef.clear();
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


  ShowConfirmDelete() {
    let text = "Bạn có chắc chắn muốn xóa file này?"
    if (this.DetailMediaType == 1) {
      text = "Bạn có chắc chắn muốn xóa hình ảnh này?"
    }
    this.modalDialogService.openDialog(this.viewRef, {
      title: 'Xác nhận',
      childComponent: SimpleModalComponent,
      data: {
        text: text
      },
      actionButtons: [
        {
          text: 'Đồng ý',
          buttonClass: 'btn btn-success',
          onAction: () => {
            this.DeleteMedia();
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-danger',
        }
      ],
    });
  }

  DeleteMedia() {
    let obj = {};
    obj["currentFolder"] = this.DetailMediaUrl;
    obj["fileName"] = this.DetailMediaName;
    obj["type"] = this.DetailMediaType;

    this.http.post('/api/FileManager/DeleteFile', obj, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListFiles();
          this.CloseMediaModal();
          this.viewRef.clear();
          this.toastSuccess("Xóa thành công!");
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
