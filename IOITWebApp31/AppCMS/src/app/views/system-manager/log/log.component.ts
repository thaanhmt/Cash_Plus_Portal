import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef, Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { typeCategoryNews, domainImage, ActionTable, Status, domain, listHotNews } from '../../../data/const';
import { Attactment, Website, Tag, Product, User, Role, NewsNote, Publication, Author, TypeAttributeItem, Action } from '../../../data/model';
import { ToastrService } from 'ngx-toastr';
import { DatePipe } from '@angular/common';
import { CommonService } from '../../../service/common.service';
import { CheckRole, Paging, QueryFilter } from '../../../data/dt';
import { DateTimeAdapter, OWL_DATE_TIME_FORMATS, OWL_DATE_TIME_LOCALE } from 'ng-pick-datetime';
import { MomentDateTimeAdapter } from 'ng-pick-datetime-moment';
import { TabsetComponent } from 'ngx-bootstrap/tabs';
import { and, forEach } from '@angular/router/src/utils/collection';

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
  selector: 'app-log',
  templateUrl: './log.component.html',
  styleUrls: ['./log.component.scss'],
  providers: [
    { provide: DateTimeAdapter, useClass: MomentDateTimeAdapter, deps: [OWL_DATE_TIME_LOCALE] },
    { provide: OWL_DATE_TIME_FORMATS, useValue: MY_CUSTOM_FORMATS }
  ]
})
export class LogComponent implements OnInit {

  @ViewChild('NewsModal') public NewsModal: ModalDirective;
  @ViewChild('HighlightNewsModal') public HighlightNewsModal: ModalDirective;
  @ViewChild('TagModal') public TagModal: ModalDirective;

  @ViewChild('file') file: ElementRef;
  @ViewChild('attachment') attachment: ElementRef;
  @ViewChild('tabset') tabset: TabsetComponent;
  public paging: Paging;
  public q: QueryFilter;
  public IsAll: boolean;
  public listLog = [];
  public linkNews = [];
  public linkCatNews = [];
  public ItemPr: Product;
  public listOrderByProduct = [];
  public listSuggestProduct = [];
  public listNewsT = [];
  public listCateNews = [];
  public listSuggestNews = [];
  public listLanguage = [];
  public listLanguageTemp = [];
  public listAuthor = [];
  public listNumberOfTopic = [];
  public listDepartment = [];
  public listNewsNote = [];
  public Tag: Tag;
  public CheckBoxStatus: boolean;
  public isNoitify: boolean = false;
  public ckeConfig: any;
  public objNew: any;
  public roleName: Role;
  public listTypeNews = typeCategoryNews;
  public Item: Publication;
  public ItemAuthor: Author;
  public ItemType: TypeAttributeItem;
  public ItemUserRole: Role;
  public ItemTranslate: Publication;
  public progress: number;
  public progressAttachment: number;
  public message: string;
  public domainImage = domainImage;
  public CheckConfirmNews: boolean;
  public languageId: number;
  public languageCode: string;
  public httpOptions: any;
  public ActionTable = ActionTable;
  public ActionId: number;
  public CheckAll: boolean;
  public total: any;
  public domain: string;
  public Status = Status;
  public listHotNews = listHotNews;
  public checkAttach: boolean;
  public tags = [];
  public Title: string;
  public userId: string;
  public activeD: boolean = false;
  public activeU: boolean = false;
  public isNumberOfTopic: boolean = false;
  public isAuthor: boolean = false;
  public isDepartment: boolean = false;
  public RoleCode: string = localStorage.getItem("roleCode") || '';
  public NameAuthor: string = localStorage.getItem("fullName") || '';
  public IsShow: number;
  public CheckRole: CheckRole;

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public datePipe: DatePipe,
    public common: CommonService
  ) {
    this.Item = new Publication();
    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "ActionId Desc";
    this.paging.item_count = 0;
    this.q = new QueryFilter();
    this.q.txtSearch = "";
    this.IsAll = false;
    this.CheckConfirmNews = this.common.CheckAccessKey(localStorage.getItem("access_key"), "DBBTV") && this.RoleCode != 'BTV';
    this.languageId = localStorage.getItem("languageId") != undefined ? parseInt(localStorage.getItem("languageId")) : 1;
    this.languageCode = localStorage.getItem("languageCode") != undefined ? localStorage.getItem("languageCode") : "vi";
    //this.paging.query = "LanguageId=" + this.languageId;
    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }

    this.Tag = new Tag();

    this.CheckRole = new CheckRole();
    let code = "QLAP";
    this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 0);
    this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 1);
    this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 2);
    this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 3);
  }
  ngOnInit() {
    this.RoleCode = localStorage.getItem("roleCode");
    this.NameAuthor = localStorage.getItem("fullName");
    this.ckeConfig = {
      allowedContent: false,
      extraPlugins: 'divarea',
      forcePasteAsPlainText: true
    };
    this.paging.query = "1=1 AND (Status=1 OR Status= 18 OR Status=19)";
    this.domain = domain;
    this.GetListPublication();
    this.GetListLanguage();
    this.GetListNumberOfTopic();
    this.GetListAuthor();
    this.GetListDepartment();
  }

  //Get danh sách log
  GetListPublication() {
    this.http.get('/api/Action/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listLog = res["data"];
          this.paging.item_count = res["metadata"].Sum;
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
          if (this.listLanguage == undefined || this.listLanguage == null)
            this.listLanguageTemp = [];
          else
            this.listLanguageTemp = this.listLanguage;
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListNumberOfTopic() {
    this.http.get('/api/TypeAttributeItem/GetByPage?page=1&query=TypeAttributeId=25&order_by=Location Asc', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listNumberOfTopic = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListAuthor() {
    this.http.get('/api/Author/GetByPageUser', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listAuthor = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListDepartment() {
    this.http.get('/api/TypeAttributeItem/GetByPage?page=1&query=TypeAttributeId=26&order_by=Location Asc', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listDepartment = res["data"];
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
    this.GetListPublication();
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
    let query = "1=1";
    if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
      query += ' and ActionName.Contains("' + this.q.txtSearch + '")';
    }
    if (this.q.UserId != undefined) {
      query += ' and UserId=' + this.q.UserId;
    }
    if (query == '')
      this.paging.query = '1=1';
    else
      this.paging.query = query;

    this.GetListPublication();
  }
  //
  StatusChanged(status) {
    let query = "1=1";
    if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
      query += ' and Title.Contains("' + this.q.txtSearch + '")';
    }

    if (this.q.LanguageId != undefined) {
      query += ' and LanguageId=' + this.q.LanguageId;
    }

    query += ' and Status=' + status;

    this.paging.query = query;

    this.GetListPublication();
  }

  //Mở modal thêm mới
  OpenNewsModal(item, type) {
    this.Item = new Publication();
    this.Item.Contents = "";
    this.listLanguageTemp = this.listLanguage;
    this.Item.LanguageId = this.languageId;
    this.Item.Location = this.paging.item_count + 1;
    this.Item.ViewNumber = 1;
    this.IsAll = true;
    if (this.file) this.file.nativeElement.value = "";
    this.message = undefined;
    this.progress = undefined;
    this.progressAttachment = undefined;
    this.CheckBoxStatus = true;

    if (item != undefined) {
      this.Item = JSON.parse(JSON.stringify(item));
      if (type == 1 || type == 3) {
        this.CheckBoxStatus = this.Item.Status == 1 ? true : false;
      }
      else if (type == 2) {
        this.CheckBoxStatus = this.Item.Status == 1 ? true : false;
        if (this.listLanguage.length == item.listLanguage.length + 1) {
          this.toastWarning("Bạn đã thêm đủ ngôn ngữ!");
          return;
        }
        this.listLanguageTemp = [];
        this.Item.PublicationId = undefined;
        this.Item.LanguageRootId = item.LanguageId;
        this.Item.LanguageRootCode = this.Item["language"]["Code"];
        this.Item.LanguageId = undefined;
        this.Item.LanguageCode = undefined;

        //check ngôn ngữ
        for (var i = 0; i < this.listLanguage.length; i++) {
          let check = false;
          if (this.listLanguage[i].LanguageId == this.languageId) {
            check = true;
          }
          if (item.listLanguage.length > 0) {
            for (var j = 0; j < item.listLanguage.length; j++) {
              if (this.listLanguage[i].LanguageId == item.listLanguage[j].lang.LanguageId) {
                check = true;
                break;
              }
            }
          }
          if (!check) {
            this.listLanguageTemp.push(this.listLanguage[i]);
          }
        }
        if (this.listLanguageTemp.length > 0) {
          this.Item.LanguageId = this.listLanguageTemp[0].LanguageId;
          this.Item.LanguageCode = this.listLanguageTemp[0].Code;
        }
      }
    }
    this.NewsModal.show();

  }
  //Thêm mới danh mục trang
  SaveNews(status) {
    if (this.Item.Title == undefined || this.Item.Title == '') {
      this.toastWarning("Chưa nhập Tiêu đề!");
      return;
    } else if (this.Item.Title.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tiêu đề!");
      return;
    } else if (this.Item.Url == undefined || this.Item.Url == '') {
      this.toastWarning("Chưa nhập Đường dẫn!");
      return;
    } else if (this.Item.Url.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập đường dẫn!");
      return;
    }
    this.Item.Status = status;
    this.Item.UserId = parseInt(localStorage.getItem("userId"));

    if (typeof this.Item.DateStartActive === 'object' && this.Item.DateStartActive != undefined) {
      let DateStartActive = this.Item.DateStartActive.add(7, 'hours');
      this.Item.DateStartActive = DateStartActive.toISOString();
    }

    if (typeof this.Item.DateStartOn === 'object' && this.Item.DateStartOn != undefined) {
      let DateStartOn = this.Item.DateStartOn.add(7, 'hours');
      this.Item.DateStartOn = DateStartOn.toISOString();
    }

    if (typeof this.Item.DateEndOn === 'object' && this.Item.DateEndOn != undefined) {
      let DateEndOn = this.Item.DateEndOn.add(7, 'hours');
      this.Item.DateEndOn = DateEndOn.toISOString();
    }

    let obj = Object.assign({}, this.Item);
    //this.listSuggestNews.forEach(item => {
    //  if (item.Check == true) {
    //    let it = { TargetRelatedId: item.NewsId }
    //  }
    //});
    //this.listSuggestProduct.forEach(item => {
    //  if (item.Check == true) {
    //    let it = { TargetRelatedId: item.ProductId }
    //  }
    //});

    if (this.Item.PublicationId == undefined) {
      this.http.post('/api/publication', obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListPublication();
            this.NewsModal.hide();
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
    } else {
      this.http.put('/api/publication/' + obj.PublicationId, obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListPublication();
            this.NewsModal.hide();
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

  AddType(type) {
    if (type == 1) {
      this.isNumberOfTopic = true;
      this.ItemType = new TypeAttributeItem();
    }
    else if (type == 2) {
      this.isAuthor = true;
      this.ItemAuthor = new Author();
    }
    else if (type == 3) {
      this.isDepartment = true;
      this.ItemType = new TypeAttributeItem();
    }
  }

  SaveNumberOfTopic() {
    if (this.Item.NumberOfTopicText == undefined || this.Item.Title == '') {
      this.toastWarning("Chưa nhập số chuyên đề!");
      return;
    } else if (this.Item.NumberOfTopicText.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập số chuyên đề!");
      return;
    } 
    this.ItemType.Name = this.Item.NumberOfTopicText;
    this.ItemType.TypeAttributeId = 25;
    this.ItemType.UserId = parseInt(localStorage.getItem("userId"));
    let obj = Object.assign({}, this.ItemType);

    this.http.post('/api/TypeAttributeItem', obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListNumberOfTopic();
            this.isNumberOfTopic = false;
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

  SaveAuthor() {
    if (this.Item.AuthorText == undefined || this.Item.AuthorText == '') {
      this.toastWarning("Chưa nhập tác giả!");
      return;
    } else if (this.Item.AuthorText.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tác giả!");
      return;
    }
    this.ItemAuthor.Name = this.Item.AuthorText;
    this.ItemAuthor.Type = 2;
    this.ItemAuthor.UserId = parseInt(localStorage.getItem("userId"));
    let obj = Object.assign({}, this.ItemAuthor);

    this.http.post('/api/author', obj, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListAuthor();
          this.isAuthor = false;
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

  SaveDepartment() {
    if (this.Item.DepartmentText == undefined || this.Item.Title == '') {
      this.toastWarning("Chưa nhập cơ quan ban ngành!");
      return;
    } else if (this.Item.DepartmentText.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập cơ quan ban ngành!");
      return;
    }
    this.ItemType.Name = this.Item.DepartmentText;
    this.ItemType.TypeAttributeId = 26;
    this.ItemType.UserId = parseInt(localStorage.getItem("userId"));
    let obj = Object.assign({}, this.ItemType);

    this.http.post('/api/TypeAttributeItem', obj, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListDepartment();
          this.isDepartment = false;
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

  ChangeTitle(key) {
    if (this.Item.PublicationId == undefined) {
      switch (key) {
        case 1:
          this.Item.MetaTitle = this.Item.Title;
          this.Item.MetaKeyword = this.Item.Title;
          this.Item.Url = this.common.ConvertUrl(this.Item.Title);
          break;
        case 2:
          this.Item.MetaDescription = this.Item.Description;
          break;
        default:
          break;
      }
    }
  }

  //Popup xác nhận xóa
  ShowConfirmDelete(Id) {
    this.modalDialogService.openDialog(this.viewRef, {
      title: 'Xác nhận',
      childComponent: SimpleModalComponent,
      data: {
        text: "Bài viết sẽ được đưa vào thùng rác?"
      },
      actionButtons: [
        {
          text: 'Đồng ý',
          buttonClass: 'btn btn-success',
          onAction: () => {
            this.DeleteNews(Id);
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-danger',

        }
      ],
    });
  }

  DeleteNews(Id) {
    if (this.RoleCode == 'ADMIN') {
      this.http.delete('/api/Action/' + Id, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListPublication();
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
    } else {
      this.toastError("Bạn không có quyền xoá bài viết này");
    }
  }

  upload2(files) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);

    const uploadReq = new HttpRequest('POST', 'api/upload', formData, {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
      reportProgress: true,
    });
  }

  RemoveUpFile() {
    this.file.nativeElement.value = "";
    this.message = undefined;
    this.progress = undefined;
  }

  upload(files, cs) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);

    const uploadReq = new HttpRequest('POST', 'api/upload/uploadImage/1', formData, {
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
          case 2:
            this.progressAttachment = Math.round(100 * event.loaded / event.total);
            this.attachment.nativeElement.value = "";
            break;
          default:
            break;
        }
      else if (event.type === HttpEventType.Response) {

        switch (cs) {
          case 1:
            this.message = event.body["data"].toString();
            this.Item.Image = this.message;
            break;
          case 2:
            this.attachment.nativeElement.value = "";
                    /*console.log(event.body["data"])*/;
            event.body["data"].forEach(item => {
              let attachment = new Attactment();
              attachment.Url = item;
              attachment.IsImageMain = false;
              attachment.Status = 1;
              attachment.Note = undefined;
            });
            break;
          default:
            break;
        }
        /* console.log(this.Item.listAttachment);*/
      }
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

    this.GetListPublication();
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
      this.listLog.forEach(item => {
        item.Action = CheckAll;
      });
    }
    else {
      let CheckAll = true;
      for (let i = 0; i < this.listLog.length; i++) {
        if (!this.listLog[i].Action) {
          CheckAll = false;
          break;
        }
      }

      this.CheckAll = CheckAll == true ? true : false;
    }
  }

  ActionTableFunc() {
    if (this.RoleCode == 'ADMIN') {
      switch (this.ActionId) {
        case 1:
          let data = [];
          this.listLog.forEach(item => {
            if (item.Action == true) {
              data.push(item.ActionId);
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
                    this.http.put('/api/action/DeleteMultiAction', data, this.httpOptions).subscribe(
                      (res) => {
                        if (res["meta"]["error_code"] == 200) {
                          this.toastSuccess("Xóa thành công!");
                          this.GetListPublication();
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
    } else {
      this.toastError("Bạn không đủ quyền để xoá");
    }
  }

  ChangeLinkDetailNews(TypeNewsId, Url, CategoryId, NewsId) {
    let Link = '';
    let Idw = 1;
    switch (TypeNewsId) {
      case 1:
        Link = "/" + this.listTypeNews.filter(x => x.Id == TypeNewsId)[0].ConstUrl + "/" + Url + "-" + CategoryId + "-" + NewsId + ".html";
        break;
      default:
        Link = "/" + this.listTypeNews.filter(x => x.Id == TypeNewsId)[0].ConstUrl + "/" + Url + "-" + Idw + "-" + NewsId + ".html";
        break;
    }
    //console.log(Link);
    return Link;

  }

  OpenModalTag() {
    this.Tag = new Tag();
    this.TagModal.show();
  }

  closeNoityfy() {
    this.isNoitify = true;
  }
}
