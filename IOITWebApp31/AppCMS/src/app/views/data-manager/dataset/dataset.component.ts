import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef, Pipe, PipeTransform, HostListener } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { typeCategoryNews, domainImage, ActionTable, Status, domain, listHotNews, domainVideos, domainMedia, domainDebug, listDataSetStatus, listDataSetTypes, listDataSetFiles, listConfirmData, listDataSetHots } from '../../../data/const';
import { News, Attactment, Website, Tag, Product, User, Role, NewsNote, DataSet, DataSetApproved } from '../../../data/model';
import { ToastrService } from 'ngx-toastr';
import { DatePipe } from '@angular/common';
import { CommonService } from '../../../service/common.service';
import { CheckRole, Paging, QueryFilter } from '../../../data/dt';
import { DateTimeAdapter, OWL_DATE_TIME_FORMATS, OWL_DATE_TIME_LOCALE } from 'ng-pick-datetime';
import { MomentDateTimeAdapter } from 'ng-pick-datetime-moment';
import { TabsetComponent } from 'ngx-bootstrap/tabs';
import { and, forEach } from '@angular/router/src/utils/collection';
//import { RequestOptions, ResponseContentType, Headers, ResponseType } from '@angular/http';
//import { getFileNameFromResponseContentDisposition, saveFile } from '../../../service/file-download-helper';
//import { InterceptorService } from 'ng2-interceptors';


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
  selector: 'app-dataset',
  templateUrl: './dataset.component.html',
  styleUrls: ['./dataset.component.scss']
})

export class DatasetComponent implements OnInit {

  @ViewChild('DataModal') public DataModal: ModalDirective;
  @ViewChild('ViewModal') public ViewModal: ModalDirective;
  @ViewChild('ConfirmModal') public ConfirmModal: ModalDirective;
  @ViewChild('DetailCategory') public DetailCategory: ModalDirective;

  @ViewChild('file') file: ElementRef;
  @ViewChild('attachment') attachment: ElementRef;
  @ViewChild('tabset') tabset: TabsetComponent;

  @ViewChild('filevideo') filevideo: ElementRef;
  @ViewChild('OpenMediaFile') public OpenMediaFile: ModalDirective;
  @ViewChild('OpenMediaFileVideo') public OpenMediaFileVideo: ModalDirective;

  //@ViewChild('dateStart') dateStart: ElementRef;
  //@ViewChild('dateEnd') dateEnd: ElementRef;

  public listItemMedia = [];
  public domainMedia = domainMedia;
  public isActiveMedia: boolean = true;
  public isActiveUpload: boolean = false;
  public isDelay: boolean = false;
  public message_video: string;
  public progressAttachment: number;
  public countMedia: number;
  public countAllMedia: number;
  public typeMedia: number;
  public pagingFile: Paging;

  public paging: Paging;
  public q: QueryFilter;
  public IsAll: boolean;
  public listDatas = [];
  public listUnit = [];
  public listApplicationRange = [];
  public listResearchArea = [];
  public listStatus = listDataSetStatus;
  public listTypes = listDataSetTypes;
  public listHots = listDataSetHots;
  public listAuthors = [];
  public listFiles = listDataSetFiles;
  public listConfirmData = listConfirmData;
  public listLanguage = [];
  public listLanguageTemp = [];
  public CheckBoxStatus: boolean;
  public isNoitify: boolean = false;
  public ckeConfig: any;

  public Item: DataSet;
  public ItemTranslate: DataSet;
  public ItemApproved: DataSetApproved;
  public fileView: Attactment;
  public progress: number;

  public domainVideos = domainVideos;
  public message: string;
  public domainImage = domainImage;
  public CheckConfirmNews: boolean;
  public languageId: number;
  public languageCode: string;

  public httpOptions: any;
  public httpOptionsBlob: any;

  public ActionTable = ActionTable;
  public ActionId: number;
  public CheckAll: boolean;
  public total: any;
  public total1: any;
  public total2: any;
  public total3: any;
  public domain: string;
  public Status = Status;

  public listData = [];
  public nameTitle: string;
  public NameCate: string;

  public Title: string;
  public userId: string;
  public activeD: boolean = false;
  public activeU: boolean = false;
  public RoleCode: string = localStorage.getItem("roleCode") || '';
  public NameAuthor: string = localStorage.getItem("fullName") || '';
  public staticDomain: string;
  public staticDomainMedia: string;
  public domainDebug = domainDebug;
  public IdFile: any;
  public dataFile: string;
  public pdfBase64: any;
  public StatusApproved: any;
  public disabledStatus: any;

  PriceMaskConfig = {
    align: "left",
    allowNegative: false,
    decimal: ".",
    precision: 0,
    prefix: "",
    suffix: "",
    thousands: ","
  };

  public CheckRole: CheckRole;
  public CheckRole2: CheckRole;
  public CheckRole3: CheckRole;

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public datePipe: DatePipe,
    public common: CommonService,
    private sanitizer: DomSanitizer,
    //private httpDownload: InterceptorService,
  ) {
    this.pagingFile = new Paging();
    this.pagingFile.page = 1;
    this.pagingFile.page_size = 24;
    this.pagingFile.query = "1=1";
    this.pagingFile.order_by = "";
    this.pagingFile.item_count = 0;
    this.countMedia = 24;

    this.Item = new DataSet();
    this.ItemApproved = new DataSetApproved();
    this.fileView = new Attactment();
    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "";
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
    this.httpOptionsBlob = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
      observe: 'response',
      responseType: 'blob' as 'json'
    }

    this.CheckRole = new CheckRole();
    let code = "QLDL";
    this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 0);
    this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 1);
    this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 2);
    this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 3);
    this.CheckRole.Export = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 5);

    //
    this.CheckRole2 = new CheckRole();
    let code2 = "DNB";
    this.CheckRole2.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code2, 2);
    //
    this.CheckRole3 = new CheckRole();
    let code3 = "DCK";
    this.CheckRole3.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code3, 2);

  }
  ngOnInit() {
    this.RoleCode = localStorage.getItem("roleCode");
    this.NameAuthor = localStorage.getItem("fullName");
    this.ckeConfig = {
      allowedContent: false,
      extraPlugins: 'divarea',
      forcePasteAsPlainText: true
    };
    this.paging.query = "1=1";
    this.domain = domain;
    this.GetListData();
    this.GetListCategory(14);
    this.GetListCategory(15);
    this.GetListUnit();
    this.GetListAuthor();
    this.GetListLanguage();
    this.GetListFiles();
    this.GetDomainStatic();
  }

  GetDomainStatic() {
    this.http.get('api/Config/1', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.staticDomain = res["data"].Website;
          if (res["data"].ModeSite) {
            this.staticDomain = res["data"].Website;
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

  GetListData() {
    let data = Object.assign({}, this.q);

    //if (this.dateStart.nativeElement.value) {
    //  let obj = this.dateStart.nativeElement.value.split("/");
    //  data.DateStart = obj[2] + "-" + obj[1] + "-" + obj[0] + " 0:0:0";
    //}
    //if (this.dateEnd.nativeElement.value) {
    //  let obj = this.dateEnd.nativeElement.value.split("/");
    //  data.DateEnd = obj[2] + "-" + obj[1] + "-" + obj[0] + " 23:59:59";
    //}
    data.page = this.paging.page;
    data.page_size = this.paging.page_size;
    data.query = this.paging.query;
    data.order_by = this.paging.order_by;
    if (data.ApplicationRangeId == undefined)
      data.ApplicationRangeId = -1;
    if (data.ResearchAreaId == undefined)
      data.ResearchAreaId = -1;
    if (data.Extention == undefined)
      data.Extention = -1;
    if (data.UnitId == undefined)
      data.UnitId = -1;

    this.http.post('/api/dataSet/GetByPagePost', data, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listDatas = res["data"];
          this.paging.item_count = res["metadata"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
    //this.http.get('/api/dataSet/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
    //  (res) => {
    //    if (res["meta"]["error_code"] == 200) {
    //      this.listDatas = res["data"];
    //      this.paging.item_count = res["metadata"];
    //    }
    //  },
    //  (err) => {
    //    console.log("Error: connect to API");
    //  }
    //);
  }

  GetListCategory(type) {
    
    this.http.get('/api/category/GetByPage?page=1&query=TypeCategoryId=' + type + '&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          if(type==14)
            this.listResearchArea = res["data"];
          else
            this.listApplicationRange = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListUnit() {
    this.http.get('/api/unit/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listUnit = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListAuthor() {
    this.http.get('/api/customer/GetByPage?page=1&page_size=200&query=1=1&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listAuthors = res["data"];
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
    let query = "1=1";
    if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
      //if (query != '') {
      query += ' and (Title.Contains("' + this.q.txtSearch + '") or AuthorName.Contains("' + this.q.txtSearch + '"))';
    }

    if (this.q.Type != undefined) {
      query += ' and Type=' + this.q.Type;
    }

    if (this.q.LanguageId != undefined) {
      query += ' and LanguageId=' + this.q.LanguageId;
    }

    if (this.q.Status != undefined) {
      if (this.q.Status!=4)
        query += ' and Status=' + this.q.Status;
      else
        query += ' and (Status=4 OR Status=5)';
    }

    if (this.q.CustomerId != undefined) {
      query += ' and UserCreatedId=' + this.q.CustomerId;
    }

    if (this.q.Hot != undefined) {
      if (this.q.Hot == 1)
        query += ' and IsHot=true';
      else
        query += ' and IsHot!=true';
    }

    this.paging.query = query;

    this.GetListData();
  }
  //
  StatusChanged(status) {
    let query = "1=1";
    if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
      query += ' and Title.Contains("' + this.q.txtSearch + '")';
    }

    if (this.q["Type"] != undefined) {
      query += ' and TypeNewsId=' + this.q["Type"];
    }

    if (this.q["CategoryId"] != undefined) {
      query += ' and CategoryId=' + this.q["CategoryId"];
    }

    if (this.q.LanguageId != undefined) {
      query += ' and LanguageId=' + this.q.LanguageId;
    }

    if (this.q.IsHot != undefined) {
      query += ' and IsHot=' + this.q.IsHot;
    }

    if (this.q["UserId"] != undefined) {
      query += ' and UserCreatedId=' + this.q["UserId"];
    }
    if (this.q.AuthorId != undefined) {
      query += ' and AuthorId=' + this.q.AuthorId;
    }
    query += ' and Status=' + status;

    this.paging.query = query;

    this.GetListData();
  }

  //Mở modal thêm mới
  OpenDataModal(item, type) {

    this.Item = new DataSet();
    this.Item.Contents = "";
    this.listLanguageTemp = this.listLanguage;
    this.Item.LanguageId = this.languageId;
    this.Item.Location = this.paging.item_count + 1;
    this.Item.ViewNumber = 1;
    // this.Tag = undefined;
    this.IsAll = true;
    if (this.file) this.file.nativeElement.value = "";
    this.message = undefined;
    this.progress = undefined;
    this.progressAttachment = undefined;
    this.Item.Type = 1;
    this.CheckBoxStatus = true;

    if (item != undefined) {
      //item.Note = undefined;
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
        this.Item.DataSetId = undefined;
        this.Item.DataSetRootId = item.NewsId;
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
    
    this.DataModal.show();

  }

  SaveData(type) {
    if (this.Item.Title == undefined || this.Item.Title == '') {
      this.toastWarning("Chưa nhập Tiêu đề!");
      return;
    }
    else if (this.Item.Title.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tiêu đề!");
      return;
    }
    else if (this.Item.ApplicationRangeId == undefined || this.Item.ApplicationRangeId < 0) {
      this.toastWarning("Chưa chọn phạm vi ứng dụng!");
      return;
    }
    else if (this.Item.ResearchAreaId == undefined || this.Item.ResearchAreaId < 0) {
      this.toastWarning("Chưa chọn lĩnh vực nghiên cứu!");
      return;
    }
    else if (this.Item.Description == undefined || this.Item.Description == '') {
      this.toastWarning("Chưa nhập nội dung mô tả!");
      return;
    }
    else if (this.Item.Description.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập nội dung mô tả!");
      return;
    }
    else if (this.Item.AuthorName == undefined || this.Item.AuthorName == '') {
      this.toastWarning("Chưa nhập tên tác giả!");
      return;
    }
    else if (this.Item.AuthorName.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên tác giả!");
      return;
    }
    //else if (this.Item.LanguageId == undefined) {
    //  this.toastWarning("Chưa chọn Ngôn ngữ!");
    //  return;
    //}
    //this.Item.Status = status;
    this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
    this.Item.UserId = parseInt(localStorage.getItem("userId"));
    this.Item.UserCreatedId = parseInt(localStorage.getItem("userId"));

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
    if (obj.DateStartActive != undefined) {
      let obj2 = new Date(obj.DateStartActive);
      obj.DateStartActive = obj2.getFullYear() + "-" + (obj2.getMonth() + 1) + "-" + obj2.getDate() + " " + obj2.getHours() + ":" + obj2.getMinutes() + ":0";
      if (obj.TimeStartActive != undefined) {
        let obj1 = new Date(obj.TimeStartActive);
        obj.DateStartActive = obj2.getFullYear() + "-" + (obj2.getMonth() + 1) + "-" + obj2.getDate() + " " + obj1.getHours() + ":" + obj1.getMinutes() + ":0";
      }
    }
    //obj.listProductRelated = [];
    //this.listSuggestProduct.forEach(item => {
    //  if (item.Check == true) {
    //    let it = { TargetRelatedId: item.ProductId }
    //    obj.listProductRelated.push(it);
    //  }
    //});

    if (this.Item.DataSetId == undefined) {
      //let arr = [];
      //obj.listCategory.forEach(item => {
      //  var flag = false;
      //  for (var i = 0; i < this.listCateNews.length; i++) {
      //    if (item.CategoryId == this.listCateNews[i].CategoryId && this.listCateNews[i].Check == true) {
      //      flag = true;
      //      break;
      //    }
      //  }

      //  if (!flag) {
      //    item.Check = false;
      //    arr.push(item);
      //  }
      //});

      //obj.listCategory = arr.concat(this.listCateNews.filter(e => e.Check == true));

      this.http.post('/api/dataset', obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListData();
            this.toastSuccess("Thêm mới thành công!");
            this.DataModal.hide();
            this.viewRef.clear();
          }
          else if (res["meta"]["error_code"] == 228) {
            this.toastError("Ngôn ngữ này đã có bài viết!");
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
      //let arr = [];
      //obj.listCategory.forEach(item => {
      //  var flag = false;
      //  for (var i = 0; i < this.listCateNews.length; i++) {
      //    if (item.CategoryId == this.listCateNews[i].CategoryId && this.listCateNews[i].Check == true) {
      //      flag = true;
      //      break;
      //    }
      //  }

      //  if (!flag) {
      //    item.Check = false;
      //    arr.push(item);
      //  }
      //});

      //obj.listCategory = arr.concat(this.listCateNews.filter(e => e.Check == true));

      this.http.put('/api/dataset/' + obj.DataSetId, obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListData();
            this.DataModal.hide();
            //if (status == 1)
            //  this.ConfigDateModal.hide();
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

  ChangeTitle(key) {
    if (this.Item.DataSetId == undefined) {
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

  //Mở modal xem
  OpenViewModal(item, type) {

    this.Item = new DataSet();
    //this.Item.Contents = "";
    //this.listLanguageTemp = this.listLanguage;
    //this.Item.LanguageId = this.languageId;
    //this.Item.Location = this.paging.item_count + 1;
    //this.Item.ViewNumber = 1;
    //this.Item.listCategory = [];
    //this.Item.listTag = [];
    //this.Item.listAttachment = [];
    // this.Tag = undefined;
    //this.IsAll = true;
    //if (this.file) this.file.nativeElement.value = "";
    //this.message = undefined;
    //this.progress = undefined;
    //this.progressAttachment = undefined;
    //this.Item.Type = 1;
    //this.CheckBoxStatus = true;

    if (item != undefined) {
      //item.Note = undefined;
      this.Item = JSON.parse(JSON.stringify(item));
        this.CheckBoxStatus = this.Item.Status == 1 ? true : false;
    }

    this.ViewModal.show();

  }

  OpenConfirmModal(item, type, status) {
    this.StatusApproved = status;
    this.ItemApproved = new DataSetApproved();
    this.ItemApproved.Type = type;
    this.disabledStatus = false;
    if (status == 1 || status == 2) {
      this.ItemApproved.DataSetStatus = 2;
      this.disabledStatus = true;
    }
    else
      this.ItemApproved.DataSetStatus = 1;
    if (item != undefined) {
      this.Item = JSON.parse(JSON.stringify(item));
      this.ItemApproved.DataSetId = this.Item.DataSetId;
    }
    this.ConfirmModal.show();
  }
  OpenModalDetailCate(item, type) {
    this.nameTitle = item['Title'];
    if (type == 1) {
      this.NameCate = "phạm vi ứng dụng";
      this.listData = item['applicationRange'];
    } else if (type == 2) {
      this.NameCate = "lĩnh vực nghiên cứu";
      this.listData = item['researchArea'];
    } else if (type == 3) {
      this.NameCate = "cơ quan/tổ chức";
      this.listData = item['unit'];
    }
    this.DetailCategory.show();
  }
  SaveConfirm() {
    if (this.ItemApproved.DataSetStatus == undefined || this.ItemApproved.DataSetStatus < 0) {
      this.toastWarning("Chưa chọn trạng thái phê duyệt!");
      return;
    }
    else if (this.ItemApproved.DataSetStatus == 2) { 
      if (this.ItemApproved.Confirms == undefined || this.ItemApproved.Confirms == '') {
        this.toastWarning("Chưa nhập lý do không phê duyệt!");
        return;
      }
      else if (this.ItemApproved.Confirms.replace(/ /g, '') == '') {
        this.toastWarning("Chưa nhập lý do không phê duyệt!");
        return;
      }
    }
    
    this.ItemApproved.CreatedId = parseInt(localStorage.getItem("userId"));
    this.ItemApproved.UpdatedId = parseInt(localStorage.getItem("userId"));

    let obj = Object.assign({}, this.ItemApproved);

    if (this.Item.DataSetId != undefined) {
      this.http.post('/api/dataSetApproved', obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListData();
            this.toastSuccess("Phê duyệt thành công!");
            this.ViewModal.hide();
            this.ConfirmModal.hide();
            this.viewRef.clear();
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

  //Popup xác nhận xóa
  ShowConfirmDelete(Id) {
    this.modalDialogService.openDialog(this.viewRef, {
      title: 'Xác nhận',
      childComponent: SimpleModalComponent,
      data: {
        text: "Bạn có chắc chắn muốn xóa bộ dữ liệu này?"
      },
      actionButtons: [
        {
          text: 'Đồng ý',
          buttonClass: 'btn btn-success',
          onAction: () => {
            this.DeleteData(Id);
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-danger',

        }
      ],
    });
  }

  DeleteData(Id) {

    this.http.delete('/api/dataSet/' + Id, this.httpOptions).subscribe(
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

  UpdateHot(item, isHot) {
    
    this.Item = JSON.parse(JSON.stringify(item));
    this.Item.IsHot = isHot;
    this.Item.UserId = parseInt(localStorage.getItem("userId"));
    let obj = Object.assign({}, this.Item);

    if (this.Item.DataSetId != undefined) {
                                
      this.http.put('/api/dataSet/updateHot/' + this.Item.DataSetId, obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListData();
            this.toastSuccess("Cập nhật thành công!");
            this.viewRef.clear();
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

    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress)
        this.progress = Math.round(100 * event.loaded / event.total);
      else if (event.type === HttpEventType.Response) {
        this.Item.LinkVideo = event.body["data"];
      }
    });
  }

  RemoveUpFile() {
    this.Item.LinkVideo = undefined;
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
              //this.Item.listAttachment.push(attachment);
            });
            break;
          default:
            break;
        }
        /* console.log(this.Item.listAttachment);*/
      }
    });
  }

  uploadVideo(files, cs) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);

    const uploadReq = new HttpRequest('POST', 'api/upload/uploadVideo/1', formData, {
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
            this.message_video = event.body["data"].toString();
            this.Item.LinkVideo = this.message_video;
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
              //this.Item.listAttachment.push(attachment);
            });
            break;
          default:
            break;
        }
        /* console.log(this.Item.listAttachment);*/
      }
    });
  }

  findAuthor(item) {
    if (item == undefined) {
      return "";
    }
    else {
      return item.FullName;
    }
  }

  RemoveImage() {
    this.Item.Image = undefined;
    this.file.nativeElement.value = "";
    this.message = undefined;
    this.progress = undefined;
  }

  RemoveVideo() {
    this.Item.LinkVideo = undefined;
    this.file.nativeElement.value = "";
    this.message = undefined;
    this.progress = undefined;
  }

  ConfirmShowHide(item, i) {
    let stt = this.listDatas[i].IsShow ? 1 : 19;
    if (stt == 1) {
      this.modalDialogService.openDialog(this.viewRef, {
        title: 'Xác nhận',
        childComponent: SimpleModalComponent,
        data: {
          text: "Bạn có muốn hẹn ngày xuất bản?"
        },
        actionButtons: [
          {
            text: 'Đặt lịch',
            buttonClass: 'btn btn-primary',
            onAction: () => {
              //this.OpenModalConfigDate(item);
              this.viewRef.clear();
            }
          },
          {
            text: 'Xuất bản luôn',
            buttonClass: 'btn btn-success',
            onAction: () => {
              this.ShowHide(item.NewsId, i, 0);
            }
          },
          {
            text: 'Đóng',
            buttonClass: 'btn btn-danger',
            onAction: () => {
              this.listDatas[i].IsShow = !this.listDatas[i].IsShow;
              this.viewRef.clear()
            }
          }
        ],
      });
    }
    else {
      this.ShowHide(item.NewsId, i, 0);
    }
  }

  ShowHide(id, i, isAll) {
    let stt = this.listDatas[i].IsShow ? 1 : 19;
    this.http.put('/api/news/ShowHide/' + id + "/" + stt + "/" + isAll, undefined, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          if (stt == 1) {
            this.toastSuccess("Bài viết đã được xuất bản!");
          } else if (stt == 19) {
            this.toastWarning("Bài viết đã được gỡ xuất bản!");
          } else {
            this.toastSuccess("Thay đổi trạng thái thành công!");
          }

          this.GetListData();
          this.viewRef.clear();
        }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          this.listDatas[i].IsShow = !this.listDatas[i].IsShow;
        }

      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        this.listDatas[i].IsShow = !this.listDatas[i].IsShow;
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
  GetClassSortTablePublic(str) {
    if (this.paging.order_by != (str + " Desc") && this.paging.order_by != (str + " Asc")) {
      return "sorting";
    }
    else {
      if (this.paging.order_by == (str + " Desc")) return "sorting_desc";
      else return "sorting_asc";
    }
  }
  //RemoveAttachment(idx) {
  //  if (this.Item.listAttachment[idx].AttactmentId == undefined) {
  //    this.Item.listAttachment.splice(idx, 1);
  //  }
  //  else {
  //    this.Item.listAttachment[idx].Status = 99;
  //  }
  //}

  //SetIsMain(idx) {
  //  for (let i = 0; i < this.Item.listAttachment.length; i++) {
  //    this.Item.listAttachment[i].IsImageMain = false;
  //    if (idx == i) {
  //      this.Item.listAttachment[i].IsImageMain = true;
  //    }
  //  }
  //}

  CheckActionTable(NewsId) {
    if (NewsId == undefined) {
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
    if (this.RoleCode == 'ADMIN') {
      switch (this.ActionId) {
        case 1:
          let data = [];
          this.listDatas.forEach(item => {
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
                    this.http.put('/api/news/DeleteMultiNewsPublic', data, this.httpOptions).subscribe(
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
    } else {
      this.toastError("Bạn không đủ quyền để xoá");
    }
  }

  //ChangeLinkDetailNews(TypeNewsId, Url, NewsId) {
  //  return this.staticDomain + this.listTypeNews.filter(x => x.Id == TypeNewsId)[0].ConstUrl + "/" + Url + "-" + NewsId + ".html";
  //}


  closeNoityfy() {
    this.isNoitify = true;
  }

  ShowConfirmStatus(id, status) {
    let title = "Bạn có chắc chắn muốn xuất bản bài viết này?";
    if (status == 19)
      title = "Bạn có chắc chắn muốn gỡ bài viết này?";
    this.modalDialogService.openDialog(this.viewRef, {
      title: 'Xác nhận',
      childComponent: SimpleModalComponent,
      data: {
        text: title
      },
      actionButtons: [
        {
          text: 'Đồng ý',
          buttonClass: 'btn btn-success',
          onAction: () => {
            //this.SendStatus(id, status);
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-danger',

        }
      ],
    });
  }

  ExportExcel() {
    if (this.q.TypePaymentOrderStatus == 1)
      this.q.CashStatus = undefined;
    else if (this.q.TypePaymentOrderStatus == 2)
      this.q.CashStatus = true;
    else if (this.q.TypePaymentOrderStatus == 3)
      this.q.CashStatus = false;
    let data = Object.assign({}, this.q);

    data.TypeExport = 4;
    //console.log(this.q);

    //if (this.dateStart.nativeElement.value) {
    //  let obj = this.dateStart.nativeElement.value.split("/");
    //  data.DateStart = obj[2] + "-" + obj[1] + "-" + obj[0] + " 0:0:0";
    //}
    //if (this.dateEnd.nativeElement.value) {
    //  let obj = this.dateEnd.nativeElement.value.split("/");
    //  data.DateEnd = obj[2] + "-" + obj[1] + "-" + obj[0] + " 23:59:59";
    //}

    this.http.post<Blob>('/api/news/exportNews', data, this.httpOptionsBlob).subscribe(
      (res) => {
        //console.log(res);
        var url = window.URL.createObjectURL(res["body"]);
        var a = document.createElement('a');
        document.body.appendChild(a);
        a.setAttribute('style', 'display: none');
        a.href = url;
        a.download = "ds-bai-viet.xlsx";
        a.click();
        window.URL.revokeObjectURL(url);
        a.remove();
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
      }
    );

  }

  OpenMediaModal(type) {
    this.typeMedia = type;
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

    if (this.typeMedia == 1)
      this.Item.Image = item.url + "/" + item.name;
    else if (this.typeMedia == 2)
      this.Item.LinkVideo = item.url + "/" + item.name;
    else if (this.typeMedia == 3) {
      let attachment = new Attactment();
      attachment.Url = item.url + "/" + item.name;
      attachment.IsImageMain = false;
      attachment.Status = 1;
      attachment.Note = undefined;
      //check xem có chưa thì add
      var checkDuplicate = false;

      //for (var kk = 0; kk < this.Item.listAttachment.length; kk++) {
      //  if (this.Item.listAttachment[kk].Url == attachment.Url) {
      //    checkDuplicate = true;
      //    break;
      //  }
      //}
      //if (!checkDuplicate)
      //  this.Item.listAttachment.push(attachment);
      //else {
      //  this.toastWarning("Bạn đã chọn ảnh này!");
      //}
    }
    this.OpenMediaFile.hide();
  }

  ViewFile(id) {
    this.IdFile = id;
    this.http.get('/api/S3File/viewFile/'+id, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.fileView = res["data"];
            this.dataFile = 'data:image/png;base64,' + this.fileView.Note;
            this.pdfBase64 = this.sanitizer.bypassSecurityTrustResourceUrl('data:application/pdf;base64,' + this.fileView.Note);
          }
        },
        (err) => {
          console.log("Error: connect to API");
        }
      );
  }

  DownloadFileRar(id) {
    this.http.get<Blob>('/api/S3File/downloadFiles/'+id+'/-1' , this.httpOptionsBlob).subscribe(
      (res) => {
        //console.log(res);
        var url = window.URL.createObjectURL(res["body"]);
        var a = document.createElement('a');
        document.body.appendChild(a);
        a.setAttribute('style', 'display: none');
        a.href = url;
        a.download = "dataset.rar";
        a.click();
        window.URL.revokeObjectURL(url);
        a.remove();
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
      }
    );
  }

  DownloadFile(item) {
    this.http.get<Blob>('/api/S3File/downloadOneFile/-1/-1/' + item.AttactmentId, this.httpOptionsBlob).subscribe(
      (res) => {
        //console.log(res);
        var url = window.URL.createObjectURL(res["body"]);
        var a = document.createElement('a');
        document.body.appendChild(a);
        a.setAttribute('style', 'display: none');
        a.href = url;
        a.download = item.Name;
        a.click();
        window.URL.revokeObjectURL(url);
        a.remove();
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
      }
    );
    //const url = '/api/download/downloadOneFile/' + id;
    //const options = new RequestOptions({
    //  headers: new Headers({
    //    'Authorization': 'bearer ' + localStorage.getItem("access_token")
    //  }),
    //  responseType: ResponseContentType.Blob
    //});

    //this.httpDownload.get(url, options).subscribe(res => {
    //  const fileName = getFileNameFromResponseContentDisposition(res);
    //  saveFile(res.blob(), fileName);
    //});
  }
  @HostListener('document:keydown', ['$event']) onKeydownHandler(event: KeyboardEvent) {
    if (event.key === "Escape") {
      this.DetailCategory.hide();
      this.ViewModal.hide();
    }
  }


}
