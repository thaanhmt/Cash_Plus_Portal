import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef, Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { typeCategoryNews, domainImage, ActionTable, Status, domain, listHotNews } from '../../data/const';
import { News, Attactment, Website, Tag, Product, User, Role} from '../../data/model';
import { ToastrService } from 'ngx-toastr';
import { DatePipe, LocationStrategy, PathLocationStrategy, Location } from '@angular/common';
import { CommonService } from '../../service/common.service';
import { Paging, QueryFilter } from '../../data/dt';
import { DateTimeAdapter, OWL_DATE_TIME_FORMATS, OWL_DATE_TIME_LOCALE } from 'ng-pick-datetime';
import { MomentDateTimeAdapter } from 'ng-pick-datetime-moment';
import { TabsetComponent } from 'ngx-bootstrap/tabs';
import { and, forEach } from '@angular/router/src/utils/collection';

@Component({
    templateUrl: 'dashboard.component.html',
    providers: [Location, {
      provide: LocationStrategy,
      useClass: PathLocationStrategy
    }],
})
export class DashboardComponent implements OnInit {

    @ViewChild('NewsModal') public NewsModal: ModalDirective;
    @ViewChild('HighlightNewsModal') public HighlightNewsModal: ModalDirective;
    @ViewChild('TagModal') public TagModal: ModalDirective;

    @ViewChild('file') file: ElementRef;
    @ViewChild('attachment') attachment: ElementRef;
  @ViewChild('tabset') tabset: TabsetComponent;
  
  public countDataSet: number;
  public countDataSetView: number;
  public countDataSetDown: number;
  public countUser: number;
  //public countBienTap: number;
  //public countBaiViet: number;
  //public countAnPham: number;
  //public countVanBan: number;

  public listAuthor = [];
    public message: string;
    public domainImage = domainImage;
    public CheckConfirmNews: boolean;
    public languageId: number;
    public languageCode: string;
    public httpOptions: any;
    public RoleCode: string = localStorage.getItem("roleCode") || '';
    public NameAuthor: string = localStorage.getItem("fullName") || '';
    constructor(
        public http: HttpClient,
        public modalDialogService: ModalDialogService,
        public viewRef: ViewContainerRef,
        public toastr: ToastrService,
        public datePipe: DatePipe,
        public common: CommonService
    ) { 
        this.CheckConfirmNews = this.common.CheckAccessKey(localStorage.getItem("access_key"), "DBBTV") && this.RoleCode != 'BTV';
        this.languageId = localStorage.getItem("languageId") != undefined ? parseInt(localStorage.getItem("languageId")) : 1;
        this.languageCode = localStorage.getItem("languageCode") != undefined ? localStorage.getItem("languageCode") : "vi";
        //this.paging.query = "LanguageId=" + this.languageId;
        this.httpOptions = {
            headers: new HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        }
    }
  ngOnInit() {
    this.GetDataSet();
  }
    //Lấy toàn bộ danh sách tin văn bản

  GetListAllNews() {
    let query = '1=1';
    this.http.get('/api/news/GetByPageAll?page=1&query=' + query, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                  //this.countIsNormal = res["metadata"].Normal;
                  //this.countIsPending = res["metadata"].NoPublic;
                  //this.countIsRatify = res["metadata"].KiemDuyet;
                  //this.countBienTap = res["metadata"].BienTap;
                  //this.countBaiViet = res["metadata"].BaiViet;
                  //this.countIsDarft = res["metadata"].Temp;
                  //this.countAnPham = res["metadata"].AnPham;
                  //this.countVanBan = res["metadata"].VanBan;

                }
            },
            (err) => {
                console.log("Error: connect to API");
            }
        );
    }

    // Get danh sách tác giả
    GetListAuthor() {
      this.http.get('/api/News/GetAuthor', this.httpOptions).subscribe(
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

  GetDataSet() {
    let query = '1=1';
    this.http.get('/api/dashboard/GetDataSet', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.countDataSet = res["data"].DataSetNumber;
          this.countDataSetView = res["data"].ViewNumber;
          this.countDataSetDown = res["data"].DownNumber;
          this.countUser = res["data"].UserNumber;
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }


}
