import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef, Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { typeCategoryNews, domainImage, ActionTable, Status, domain, listHotNews, domainVideos, domainMedia, domainDebug, listDataSetStatus, listDataSetTypes, listDataSetFiles } from '../../../data/const';
import { News, Attactment, Website, Tag, Product, User, Role, NewsNote, DataSet } from '../../../data/model';
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
  selector: 'app-dataset-down',
  templateUrl: './dataset-down.component.html',
  styleUrls: ['./dataset-down.component.scss']
})
export class DatasetDownComponent implements OnInit {
  //@ViewChild('dateStart') dateStart: ElementRef;
  //@ViewChild('dateEnd') dateEnd: ElementRef;
  public domainMedia = domainMedia;

  public paging: Paging;
  public q: QueryFilter;
  public IsAll: boolean;
  public listDatas = [];
  public listUnit = [];
  public listApplicationRange = [];
  public listResearchArea = [];
  public listStatus = listDataSetStatus;
  public listTypes = listDataSetTypes;
  public listAuthors = [];
  public listFiles = listDataSetFiles;
  public listLanguage = [];
  public listLanguageTemp = [];
  public CheckBoxStatus: boolean;
  public isNoitify: boolean = false;

  public Item: DataSet;
  public httpOptions: any;
  //public httpOptionsBlob: any;

  public ActionTable = ActionTable;
  public ActionId: number;
  public CheckAll: boolean;
  public staticDomain: string;
  public staticDomainMedia: string;
  public domainDebug = domainDebug;
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

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public datePipe: DatePipe,
    public common: CommonService
  ) {

    this.Item = new DataSet();
    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "";
    this.paging.item_count = 0;

    this.q = new QueryFilter();
    this.q.txtSearch = "";

    this.IsAll = false;

    //this.CheckConfirmNews = this.common.CheckAccessKey(localStorage.getItem("access_key"), "DBBTV") && this.RoleCode != 'BTV';
    //this.languageId = localStorage.getItem("languageId") != undefined ? parseInt(localStorage.getItem("languageId")) : 1;
    //this.languageCode = localStorage.getItem("languageCode") != undefined ? localStorage.getItem("languageCode") : "vi";
    //this.paging.query = "LanguageId=" + this.languageId;
    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }
    //this.httpOptionsBlob = {
    //  headers: new HttpHeaders({
    //    'Authorization': 'bearer ' + localStorage.getItem("access_token")
    //  }),
    //  observe: 'response',
    //  responseType: 'blob' as 'json'
    //}

    this.CheckRole = new CheckRole();
    let code = "QLDL";
    this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 0);
    this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 1);
    this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 2);
    this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 3);
    this.CheckRole.Export = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 5);

  }
  ngOnInit() {
    //this.RoleCode = localStorage.getItem("roleCode");
    //this.NameAuthor = localStorage.getItem("fullName");
    //this.ckeConfig = {
    //  allowedContent: false,
    //  extraPlugins: 'divarea',
    //  forcePasteAsPlainText: true
    //};
    this.paging.query = "1=1";
    //this.domain = domain;
    this.GetListData();
    this.GetListCategory(14);
    this.GetListCategory(15);
    this.GetListUnit();
    this.GetListAuthor();
    //this.GetListLanguage();
    //this.GetListFiles();
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
    this.http.get('/api/dataSetDown/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
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
  }

  GetListCategory(type) {

    this.http.get('/api/category/GetByPage?page=1&query=TypeCategoryId=' + type + '&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          if (type == 14)
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
      query += ' and Title.Contains("' + this.q.txtSearch + '")';
    }

    if (this.q.ApplicationRangeId != undefined) {
      query += ' and ApplicationRangeId=' + this.q.ApplicationRangeId;
    }

    if (this.q.ResearchAreaId != undefined) {
      query += ' and ResearchAreaId=' + this.q.ResearchAreaId;
    }

    if (this.q.UnitId != undefined) {
      query += ' and UnitId=' + this.q.UnitId;
    }

    if (this.q.CustomerId != undefined) {
      query += ' and CreatedId=' + this.q.CustomerId;
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

  findAuthor(item) {
    if (item == undefined) {
      return "";
    }
    else {
      return item.FullName;
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

  closeNoityfy() {
    this.isNoitify = true;
  }

}
