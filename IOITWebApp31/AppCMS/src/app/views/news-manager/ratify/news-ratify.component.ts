import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { typeCategoryNews, domainImage, ActionTable, Status, domain, domainVideos, domainMedia, domainDebug } from '../../../data/const';
import { News, Attactment, Website, Tag, Product, User, Role, Upload } from '../../../data/model';
import { ToastrService } from 'ngx-toastr';
import { DatePipe } from '@angular/common';
import { CommonService } from '../../../service/common.service';
import { CheckRole, Paging, QueryFilter } from '../../../data/dt';
import { DateTimeAdapter, OWL_DATE_TIME_FORMATS, OWL_DATE_TIME_LOCALE } from 'ng-pick-datetime';
import { MomentDateTimeAdapter } from 'ng-pick-datetime-moment';
import { TabsetComponent } from 'ngx-bootstrap/tabs';
import { forEach } from '@angular/router/src/utils/collection';

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
  selector: 'app-news-retify',
  templateUrl: './news-ratify.component.html',
  styleUrls: ['./news-ratify.component.scss'],
  providers: [
    { provide: DateTimeAdapter, useClass: MomentDateTimeAdapter, deps: [OWL_DATE_TIME_LOCALE] },
    { provide: OWL_DATE_TIME_FORMATS, useValue: MY_CUSTOM_FORMATS }
  ]
})
export class NewsRatifyComponent implements OnInit {
  @ViewChild('NewsModal') public NewsModal: ModalDirective;
  @ViewChild('HighlightNewsModal') public HighlightNewsModal: ModalDirective;
  @ViewChild('TagModal') public TagModal: ModalDirective;
  @ViewChild('file') file: ElementRef;
  @ViewChild('filevideo') filevideo: ElementRef;
  @ViewChild('attachment') attachment: ElementRef;
  @ViewChild('tabset') tabset: TabsetComponent;
  @ViewChild('OpenMediaFile') public OpenMediaFile: ModalDirective;
  @ViewChild('OpenMediaFileVideo') public OpenMediaFileVideo: ModalDirective;

  @ViewChild('dateStart') dateStart: ElementRef;
  @ViewChild('dateEnd') dateEnd: ElementRef;

  public listItemMedia = [];
  public domainMedia = domainMedia;
  public paging: Paging;
  public q: QueryFilter;
  public IsAll: boolean;
  public listNews = [];
  public linkNews = [];
  public linkCatNews = [];
  public ItemPr: Product;
  public listOrderByProduct = [];
  public listSuggestProduct = [];
  public listAuthor = [];
  public listUsers = [];
  public listNewsNote = [];
  public listNewsT = [];
  public listCateNews = [];
  public listSuggestNews = [];
  public listLanguage = [];
  public listLanguageTemp = [];
  public Tag: Tag;
  public CheckBoxStatus: boolean;

  public isNoitify: boolean = false;
  public isActiveMedia: boolean = true;
  public isActiveUpload: boolean = false;
  public isDelay: boolean = false;

  public ckeConfig: any;
  public objNew: any;
  public roleName: Role;
  public listTypeNews = typeCategoryNews;
  public Title: string;
  public Item: News;
  public ItemUserRole: Role;

  public ItemTranslate: News;
  public checkAttach: boolean;
  public progress: number;
  public progressAttachment: number;

  public message: string;
  public message_video: string;
  public domainVideos = domainVideos;
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
  public total4: any;

  public Status = Status;

  public tags = [];

  public userId: string;
  public StatusId: number;

  public RoleCode: string = localStorage.getItem("roleCode") || '';

  public active: boolean = false;
  public activeU: boolean = false;
  public activeD: boolean = false;
  public domain: string;
  public pagingFile: Paging;
  public countMedia: number;
  public countAllMedia: number;
  public typeMedia: number;

  public CheckRole: CheckRole;
  public staticDomain: string;
  public staticDomainMedia: string;
  public domainDebug = domainDebug;
  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public datePipe: DatePipe,
    public common: CommonService
  ) {
    this.Item = new News();
    this.ItemPr = new Product();
    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "NewsId Desc";
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

    this.IsAll = false;
    this.StatusId = 11;

    this.CheckConfirmNews = this.common.CheckAccessKey(localStorage.getItem("access_key"), "DBBTV");
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

    this.Tag = new Tag();

    this.CheckRole = new CheckRole();
    let code = "BVVB";
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
    this.paging.query = "1=1 AND (Status=10 OR Status= 11 OR Status=12)";
    this.domain = domain;
    this.GetListAllNews();
    this.GetListCatnew();
    this.GetListUser();
    this.GetListNews();
    this.GetListAuthor();
    this.GetListCateNews();
    this.GetListLanguage();
    this.GetListTag(undefined);
    this.GetListFiles();
    //this.loadMore();
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
  //Lấy toàn bộ danh sách sản phẩm
  GetListAllProduct() {
    let query = "LanguageId=" + this.Item.LanguageId;
    if (this.ItemPr.ProductId != undefined) {
      query += " and TypeProduct=1 or TypeProduct=2  and ProductId !=" + this.ItemPr.ProductId
    }
    else {
      query += " and TypeProduct=1 or TypeProduct=2"
    }
    this.http.get('/api/product/GetByPage?page=1&query=' + query + '&order_by=&select=ProductId,PriceSpecial,Name,Image', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listSuggestProduct = res["data"];
          this.listSuggestProduct.forEach(item => {
            item.Check = false;
          });

          if (this.ItemPr.ProductId != undefined) {
            for (var i = 0; i < this.listSuggestProduct.length; i++) {
              for (var j = 0; j < this.Item.listRelated.length; j++) {
                if (this.listSuggestProduct[i].ProductId == this.Item.listRelated[j].TargetRelatedId) {
                  this.listSuggestProduct[i].Check = true;
                  break;
                }
              }
            }
          }
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  //Lấy toàn bộ danh sách tin văn bản

  GetListAllNews() {
    let query = "LanguageId=" + this.Item.LanguageId;
    if (this.Item.NewsId != undefined) {
      query = "TypeNewsId=1 and NewsId!=" + this.Item.NewsId + " and LanguageId=" + this.Item.LanguageId + " and Status=1";
    }
    else {
      query = "TypeNewsId=1 and LanguageId=" + this.Item.LanguageId + " and Status=1";
    }

    this.http.get('/api/news/GetByPage/?page=1&page_size=200&query=' + query + '&order_by=&select=NewsId,Title,Url,Image,TypeNewsId,LanguageId', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listSuggestNews = res["data"];
          this.listSuggestNews.forEach(item => {
            item.Check = false;
          });

          if (this.Item.NewsId != undefined) {
            for (var i = 0; i < this.listSuggestNews.length; i++) {
              for (var j = 0; j < this.Item.listRelated.length; j++) {
                if (this.listSuggestNews[i].NewsId == this.Item.listRelated[j].TargetRelatedId) {
                  this.listSuggestNews[i].Check = true;
                  break;
                }
              }
            }
          }
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  //Danh sách tags
  GetListTag(obj) {
    this.http.get('/api/tag/GetByPage?page=1&query=TargetType=1&order_by=&select=TagId,Name', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          if (obj != undefined) {
            let listTag = JSON.parse(JSON.stringify(this.Item.listTag))
            listTag.push(obj);
            this.Item.listTag = listTag;
          }
          this.tags = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  // get category all
  GetListCatnew() {
    this.Item.LanguageId = this.Item.LanguageId ? this.Item.LanguageId : 1;
    this.http.get('/api/Category/GetAllCatNew?page=1&query=1=1&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCateNews = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListNews() {
    let data = Object.assign({}, this.q);

    if (this.dateStart.nativeElement.value) {
      let obj = this.dateStart.nativeElement.value.split("/");
      data.DateStart = obj[2] + "-" + obj[1] + "-" + obj[0] + " 0:0:0";
    }
    if (this.dateEnd.nativeElement.value) {
      let obj = this.dateEnd.nativeElement.value.split("/");
      data.DateEnd = obj[2] + "-" + obj[1] + "-" + obj[0] + " 23:59:59";
    }
    data.page = this.paging.page;
    data.page_size = this.paging.page_size;
    data.query = this.paging.query;
    data.order_by = this.paging.order_by;

    this.http.post('/api/news/GetByPageCash', data, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listNews = res["data"];
          this.listNews.forEach(item => {
            item.IsShow = false;
          });
          this.paging.item_count = res["metadata"].Sum;
          this.total1 = res["metadata"].Temp;
          this.total2 = res["metadata"].New;
          this.total3 = res["metadata"].ReNew;
          this.active = true;
          this.activeU = false;
          this.activeD = false;
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
    //Get danh bài viết đang chờ duyệt
    //this.http.get('/api/news/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
    //  (res) => {
    //    if (res["meta"]["error_code"] == 200) {
    //      this.listNews = res["data"];
    //      this.listNews.forEach(item => {
    //        item.IsShow = false;
    //      });
    //      this.paging.item_count = res["metadata"].Sum;
    //      this.total1 = res["metadata"].Temp;
    //      this.total2 = res["metadata"].New;
    //      this.total3 = res["metadata"].ReNew;
    //      this.active = true;
    //      this.activeU = false;
    //      this.activeD = false;
    //    }
    //  },
    //  (err) => {
    //    console.log("Error: connect to API");
    //  }
    //);
  }

  GetListNewsNote(newsId) {
    //Get danh sách góp ý bài viết
    var query = "NewsId=" + newsId;
    this.http.get('/api/newsNote/GetByPage/?page=1&query=' + query + '&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listNewsNote = res["data"];
          //this.paging.item_count = res["metadata"].Sum;
          //this.total = res["metadata"];
          //this.activeU = true;
          //this.activeD = false;
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

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

    this.StatusId = status;
    if (status != 20)
      query += ' and Status=' + status;
    else {
      query += ' and UserCreatedId=' + parseInt(localStorage.getItem("userId"));
    }

    this.paging.query = query;

    this.GetListNews();
  }

  getMyPosts() {
    //Get danh sách tất cả bài viết của user Cộng tác viên và Nhân viên
    this.http.get('/api/news/GetByPageUser/?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listNews = res["data"];
          this.listNews.forEach(item => {
            item.IsShow = item.Status == 12 ? true : false;
          });
          this.paging.item_count = res["metadata"].Sum;
          this.total = res["metadata"];
          this.active = false;
          this.activeU = true;
          this.activeD = false;
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  getPostsDraft() {
    //Get danh sách bài viết rác của nhân viên
    this.http.get('/api/news/GetByPageDraft/?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listNews = res["data"];
          this.listNews.forEach(item => {
            item.IsShow = item.Status == 12 ? true : false;
          });
          this.paging.item_count = res["metadata"].Sum;
          this.total = res["metadata"];
          this.active = false;
          this.activeU = false;
          this.activeD = true;
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListCateNews() {
    this.Item.LanguageId = this.Item.LanguageId ? this.Item.LanguageId : 1;
    this.http.get('/api/category/GetByTree?arr=1&arr=2&arr=3&arr=4&arr=5&langId=' + this.Item.LanguageId, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCateNews = res["data"];

          if (this.Item.NewsId != undefined) {
            for (var i = 0; i < this.listCateNews.length; i++) {
              for (var j = 0; j < this.Item.listCategory.length; j++) {
                if (this.listCateNews[i].CategoryId == this.Item.listCategory[j].CategoryId) {
                  this.listCateNews[i].Check = true;
                  break;
                }
              }
            }
          }

        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );

  }

  GetListOrderBy() {
    this.http.get('/api/orderby/GetOrderBy/11', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listOrderByProduct = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  // Get danh sách tác giả
  GetListAuthor() {
    var query = "Type=1";
    this.http.get('/api/author/GetByPage/?page=1&query=' + query + '&order_by=', this.httpOptions).subscribe(
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
  GetListUser() {
    this.http.get('/api/news/GetAuthor', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listUsers = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      });
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

  GetTranslate(id) {
    let sl = this.Item.LanguageRootCode;
    let tl = this.Item.LanguageCode;
    this.ItemTranslate = new News();
    this.http.get('/api/translate/' + id + '/' + sl + '/' + tl + '/2', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.ItemTranslate = res["data"];
          this.Item.NewsId = undefined;
          this.Item.Title = this.ItemTranslate.Title;
          this.Item.Url = this.ItemTranslate.Url;
          this.Item.Description = this.ItemTranslate.Description;
          this.Item.Contents = this.ItemTranslate.Contents;
          this.Item.Introduce = this.ItemTranslate.Introduce;
          this.Item.SystemDiagram = this.ItemTranslate.SystemDiagram;
          this.Item.MetaTitle = this.ItemTranslate.MetaTitle;
          this.Item.MetaDescription = this.ItemTranslate.MetaDescription;
          this.Item.MetaKeyword = this.ItemTranslate.MetaKeyword;
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  OpenChooseHighlightsNews() {
    this.listOrderByProduct = [];
    this.GetListOrderBy();
    this.HighlightNewsModal.show();
  }

  SaveHighlightNews() {
    this.http.post('/api/orderby', this.listOrderByProduct, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.HighlightNewsModal.hide();
          this.toastSuccess("Lưu thành công!");
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

  DeleteOrderBy(item) {
    for (let i = 0; i < this.listOrderByProduct.length; i++) {
      if (this.listOrderByProduct[i].CategoryMappingId == item.CategoryMappingId) {
        this.listOrderByProduct[i].Status = 99;
        break;
      }
    }
  }

  SelectTypeNews() {
    this.GetListCateNews();
    this.GetListAllNews();
    this.GetListAllProduct();
  }

  SelectLanguage() {
    this.GetListCateNews();
    this.GetListAllNews();
    this.GetListAllProduct();
  }

  //Chuyển trang
  PageChanged(event) {
    this.paging.page = event.page;
    if (this.active) {
      this.GetListNews();
    } else if (this.activeU) {
      this.getMyPosts();
    } else if (this.activeD) {
      this.getPostsDraft();
    }
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
    let query = "1=1 AND (Status=10 OR Status=11 OR Status=12)";
    if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
        query += ' and Title.Contains("' + this.q.txtSearch + '")';
    }

    if (this.q["Type"] != undefined) {
        query += ' and TypeNewsId=' + this.q["Type"];
    }

    if (this.q["CategoryId"] != undefined) {
      //if (query != '') {
        query += ' and CategoryId=' + this.q["CategoryId"];
      //}
      //else {
      //  query += 'CategoryId=' + this.q["CategoryId"];
      //}
    }

    if (this.q.LanguageId != undefined) {
      //if (query != '') {
        query += ' and LanguageId=' + this.q.LanguageId;
      //}
      //else {
      //  query += 'LanguageId=' + this.q.LanguageId;
      //}
    }
    if (this.q["UserId"] != undefined) {
      query += ' and UserCreatedId=' + this.q["UserId"];
    }
    if (this.q.AuthorId != undefined) {
      query += ' and AuthorId=' + this.q.AuthorId;
    }
    //if (this.q["StatusId"] != undefined) {
    //  if (query != '') {
    //    query += ' and Status=' + this.q["StatusId"];
    //  }
    //  else {
    //    query += 'Status=' + this.q["StatusId"];
    //  }
    //}
    //if (query == '')
    //  this.paging.query = '1=1';
    //else
      this.paging.query = query;
    /*if (this.active) {*/
      this.GetListNews();
    //} else if (this.activeU) {
    //  this.getMyPosts();
    //} else if (this.activeD) {
    //  this.getPostsDraft();
    //}

  }

  CheckCategory(CategoryId, curItem) {
    let Check = curItem["Check"];
    let CategoryParentId = curItem["CategoryParentId"];

    let CheckParent = false;

    this.listCateNews.forEach(item => {
      if (Check) {
        if (item.Genealogy.indexOf(CategoryId.toString()) != -1) {
          item.Check = !Check;
        }
      }


      if (Check == false) {
        CheckParent = true;
      }
      else {
        if (item.CategoryParentId == CategoryParentId) {
          if (item.Check == true) {
            CheckParent = true;
          }
        }
      }

    });

    if (CheckParent) {
      this.listCateNews.forEach(item => {
        if (item.CategoryId == CategoryParentId) {
          item.Check = true;
        }
      });
    }
  }
  // chon tin lien quan
  SelectListNew(id) {
    console.log(this.listSuggestNews);
    console.log(id);
  }

  public onAttachChanged(value: boolean) {
    this.checkAttach = value;
    if (this.checkAttach == true) {
      this.GetListAllNews();
    }
  }
  //Mở modal thêm mới
  OpenNewsModal(item, type) {
    this.Item = new News();
    this.Item.Status = undefined;
    this.Item.Contents = "";
    this.listLanguageTemp = this.listLanguage;
    this.Item.LanguageId = this.languageId;
    this.Item.Location = this.paging.item_count + 1;
    this.Item.ViewNumber = 1;
    this.Item.listCategory = [];
    this.Item.listTag = [];
    this.Item.listAttachment = [];
    // this.Tag = undefined;
    this.IsAll = true;
    if (this.file) this.file.nativeElement.value = "";
    this.message = undefined;
    this.progress = undefined;
    this.progressAttachment = undefined;
    this.Item.TypeNewsId = 1;
    this.CheckBoxStatus = true;

    if (item != undefined) {
      this.GetListNewsNote(item.NewsId);
      item.Note = undefined;
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
        this.Item.NewsId = undefined;
        this.Item.NewsRootId = item.NewsId;
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

        //Gọi api dịch ở đây
        //this.GetTranslate(this.Item.NewsRootId);

        //this.Item["LangName"] = item.language != undefined ? item.language.Name : "";
        //this.Item["LangFlag"] = item.language != undefined ? item.language.Flag : "";
        //this.Item["LangTitle"] = item.Title;
      }
    }
    if (this.Item.TypeNewsId == 7) {
      this.GetListAllNews();
    }
    this.GetListCateNews();
    this.GetListAllProduct();
    if (type == 2) {
      this.Item.listAttachment = [];
    }

    this.NewsModal.show();

  }
  //Thêm mới bài viết
  SaveNews(status) {
    if (this.Item.TypeNewsId == undefined) {
      this.toastWarning("Chưa chọn Loại tin!");
      return;
    } else if (this.Item.Title == undefined || this.Item.Title == '') {
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
    else if (this.Item.TypeNewsId != 3 && this.Item.TypeNewsId != 4
      && this.Item.TypeNewsId != 6 && this.Item.TypeNewsId != 7
      && (this.Item.Contents == undefined || this.Item.Contents == '')) {
      this.toastWarning("Chưa nhập Nội dung!");
      return;
    }
    //else if (this.Item.TypeNewsId == 7 && (this.Item.YearTimeline == undefined || this.Item.YearTimeline == null)) {
    //  this.toastWarning("Vui lòng nhập năm!");
    //  return;
    //}
    else if (this.Item.LanguageId == undefined) {
      this.toastWarning("Chưa chọn Ngôn ngữ!");
      return;
    }
    this.Item.Status = status == undefined ? 11 : status; // Mắc dịnh là trạng thái 11 (Bài viết mới)
    this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
    this.Item.UserId = parseInt(localStorage.getItem("userId"));
    this.Item.IsComment = true;
    if (typeof this.Item.DateStartActive === 'object' && this.Item.DateStartActive != undefined) {
      console.log(this.Item.DateStartActive);
      let DateStartActive = this.Item.DateStartActive.add(7, 'hours');
      this.Item.DateStartActive = DateStartActive.toISOString();
    }

    if (typeof this.Item.DateStartOn === 'object' && this.Item.DateStartOn != undefined) {
      let DateStartOn = this.Item.DateStartOn.add(7, 'hours');
      this.Item.DateStartOn = DateStartOn.toISOString();
    }
    if (this.Item.DateEndOn == undefined) {
      this.Item.DateEndOn = this.Item.DateStartOn;
    }

    if (typeof this.Item.DateEndOn === 'object' && this.Item.DateEndOn != undefined) {
      let DateEndOn = this.Item.DateEndOn.add(7, 'hours');
      this.Item.DateEndOn = DateEndOn.toISOString();
    }
    

    let obj = Object.assign({}, this.Item);
    obj.listRelated = [];
    this.listSuggestNews.forEach(item => {
      if (item.Check == true) {
        let it = { TargetRelatedId: item.NewsId }
        obj.listRelated.push(it);
      }
    });
    obj.listProductRelated = [];
    this.listSuggestProduct.forEach(item => {
      if (item.Check == true) {
        let it = { TargetRelatedId: item.ProductId }
        obj.listProductRelated.push(it);
      }
    });

    if (this.Item.NewsId == undefined) {
      let arr = [];
      obj.listCategory.forEach(item => {
        var flag = false;
        for (var i = 0; i < this.listCateNews.length; i++) {
          if (item.CategoryId == this.listCateNews[i].CategoryId && this.listCateNews[i].Check == true) {
            flag = true;
            break;
          }
        }

        if (!flag) {
          item.Check = false;
          arr.push(item);
        }
      });

      obj.listCategory = arr.concat(this.listCateNews.filter(e => e.Check == true));

      this.http.post('/api/news', obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListNews();
            this.toastSuccess("Thêm mới thành công!");
            this.NewsModal.hide();
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
      let arr = [];
      obj.listCategory.forEach(item => {
        var flag = false;
        for (var i = 0; i < this.listCateNews.length; i++) {
          if (item.CategoryId == this.listCateNews[i].CategoryId && this.listCateNews[i].Check == true) {
            flag = true;
            break;
          }
        }

        if (!flag) {
          item.Check = false;
          arr.push(item);
        }
      });

      obj.listCategory = arr.concat(this.listCateNews.filter(e => e.Check == true));

      this.http.put('/api/news/' + obj.NewsId, obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListNews();
            this.NewsModal.hide();
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
  }


  //Thêm mới bài viết nháp
  //SaveDraft() {
  //  if (this.Item.TypeNewsId == undefined) {
  //    this.toastWarning("Chưa chọn Loại tin!");
  //    return;
  //  } else if (this.Item.Title == undefined || this.Item.Title == '') {
  //    this.toastWarning("Chưa nhập Tiêu đề!");
  //    return;
  //  } else if (this.Item.Title.replace(/ /g, '') == '') {
  //    this.toastWarning("Chưa nhập tiêu đề!");
  //    return;
  //  } else if (this.Item.Url == undefined || this.Item.Url == '') {
  //    this.toastWarning("Chưa nhập Đường dẫn!");
  //    return;
  //  } else if (this.Item.Url.replace(/ /g, '') == '') {
  //    this.toastWarning("Chưa nhập đường dẫn!");
  //    return;
  //  }
  //  else if (this.Item.TypeNewsId != 3 && this.Item.TypeNewsId != 4 && this.Item.TypeNewsId != 6 && (this.Item.Contents == undefined || this.Item.Contents == '')) {
  //    this.toastWarning("Chưa nhập Nội dung!");
  //    return;
  //  }
  //  else if (this.Item.TypeNewsId == 7 && (this.Item.YearTimeline == undefined || this.Item.YearTimeline == null)) {
  //    this.toastWarning("Vui lòng nhập năm!");
  //    return;
  //  }
  //  else if (this.Item.LanguageId == undefined) {
  //    this.toastWarning("Chưa chọn Ngôn ngữ!");
  //    return;
  //  }
  //  this.Item.Status = 10; // Mắc dịnh là trạng thái 13 (Bài viết nháp)
  //  this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
  //  this.Item.UserId = parseInt(localStorage.getItem("userId"));

  //  if (typeof this.Item.DateStartActive === 'object' && this.Item.DateStartActive != undefined) {
  //    console.log(this.Item.DateStartActive);
  //    let DateStartActive = this.Item.DateStartActive.add(7, 'hours');
  //    this.Item.DateStartActive = DateStartActive.toISOString();
  //  }

  //  if (typeof this.Item.DateStartOn === 'object' && this.Item.DateStartOn != undefined) {
  //    let DateStartOn = this.Item.DateStartOn.add(7, 'hours');
  //    this.Item.DateStartOn = DateStartOn.toISOString();
  //  }

  //  if (typeof this.Item.DateEndOn === 'object' && this.Item.DateEndOn != undefined) {
  //    let DateEndOn = this.Item.DateEndOn.add(7, 'hours');
  //    this.Item.DateEndOn = DateEndOn.toISOString();
  //  }

  //  let obj = Object.assign({}, this.Item);
  //  obj.listRelated = [];
  //  this.listSuggestNews.forEach(item => {
  //    if (item.Check == true) {
  //      let it = { TargetRelatedId: item.NewsId }
  //      obj.listRelated.push(it);
  //    }
  //  });
  //  obj.listProductRelated = [];
  //  this.listSuggestProduct.forEach(item => {
  //    if (item.Check == true) {
  //      let it = { TargetRelatedId: item.ProductId }
  //      obj.listProductRelated.push(it);
  //    }
  //  });

  //  if (this.Item.NewsId == undefined) {
  //    let arr = [];
  //    obj.listCategory.forEach(item => {
  //      var flag = false;
  //      for (var i = 0; i < this.listCateNews.length; i++) {
  //        if (item.CategoryId == this.listCateNews[i].CategoryId && this.listCateNews[i].Check == true) {
  //          flag = true;
  //          break;
  //        }
  //      }

  //      if (!flag) {
  //        item.Check = false;
  //        arr.push(item);
  //      }
  //    });

  //    obj.listCategory = arr.concat(this.listCateNews.filter(e => e.Check == true));

  //    this.http.post('/api/news', obj, this.httpOptions).subscribe(
  //      (res) => {
  //        if (res["meta"]["error_code"] == 200) {
  //          this.GetListNews();
  //          this.toastSuccess("Đã thêm vào danh sách nháp!");
  //          this.NewsModal.hide();
  //          this.viewRef.clear();
  //        }
  //        else if (res["meta"]["error_code"] == 228) {
  //          this.toastError("Ngôn ngữ này đã có bài viết!");
  //        }
  //        else {
  //          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
  //        }
  //      },
  //      (err) => {
  //        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
  //      }
  //    );
  //  }
  //  else {
  //    let arr = [];
  //    obj.listCategory.forEach(item => {
  //      var flag = false;
  //      for (var i = 0; i < this.listCateNews.length; i++) {
  //        if (item.CategoryId == this.listCateNews[i].CategoryId && this.listCateNews[i].Check == true) {
  //          flag = true;
  //          break;
  //        }
  //      }

  //      if (!flag) {
  //        item.Check = false;
  //        arr.push(item);
  //      }
  //    });

  //    obj.listCategory = arr.concat(this.listCateNews.filter(e => e.Check == true));

  //    this.http.put('/api/news/' + obj.NewsId, obj, this.httpOptions).subscribe(
  //      (res) => {
  //        if (res["meta"]["error_code"] == 200) {
  //          this.GetListNews();
  //          this.NewsModal.hide();
  //          this.toastSuccess("Cập nhật thành công!");
  //        }
  //        else {
  //          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
  //        }
  //      },
  //      (err) => {
  //        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
  //      }
  //    );
  //  }
  //}

  ToggleCateToList(id) {
    this.listNewsT = [];
    for (var i = 0; i < this.listCateNews.length; i++) {
      if (this.listCateNews[i].Check == true) {
        this.objNew = this.listCateNews[i];
        this.listNewsT.push(this.objNew);
      }
    }
  }

  // AddTag() {
  //     if (this.Tag != undefined && this.Tag != '') {
  //         this.Item.listTag.push({ TagId: null, Name: this.Tag, Check: false });
  //         this.Tag = '';
  //     }
  // }ss

  // RemoveTag(i) {
  //     if (this.Item.NewsId == undefined) {
  //         this.Item.listTag.splice(i, 1);
  //     }
  //     else {
  //         if (this.Item.listTag[i].TagId != null) {
  //             this.Item.listTag[i].Check = false;
  //         }
  //         else {
  //             this.Item.listTag.splice(i, 1);
  //         }
  //     }
  // }

  ChangeTitle(key) {
    if (this.Item.NewsId == undefined) {
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
        text: "Bạn có chắc chắn muốn xóa bản ghi này?"
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
    this.http.delete('/api/news/' + Id + '/1', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListNews();
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
            console.log(event.body["data"]);
            event.body["data"].forEach(item => {
              let attachment = new Attactment();
              attachment.Url = item;
              attachment.IsImageMain = false;
              attachment.Status = 1;
              attachment.Note = undefined;
              this.Item.listAttachment.push(attachment);
            });
            break;
          default:
            break;
        }
        console.log(this.Item.listAttachment);
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
              this.Item.listAttachment.push(attachment);
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
    this.message_video = undefined;
    this.progress = undefined;
  }

  ConfirmShowHide(item, i) {
    /*if (item.listLanguage.length > 0) {*/
      this.modalDialogService.openDialog(this.viewRef, {
        title: 'Xác nhận',
        childComponent: SimpleModalComponent,
        data: {
          text: "Bạn có muốn gửi biên tập bài viết?"
        },
        actionButtons: [
          {
            text: 'Đồng ý',
            buttonClass: 'btn btn-success',
            onAction: () => {
              this.ShowHide(item.NewsId, i, 0);
            }
          },
          {
            text: 'Không đồng ý',
            buttonClass: 'btn btn-danger',
            onAction: () => {
              this.listNews[i].IsShow = !this.listNews[i].IsShow;
              this.viewRef.clear();
            }
          }
        ],
      });
    //}
    //else {
    //  this.ShowHide(item.NewsId, i, 0);
    //}
  }

  ShowHide(id, i, isAll) {
    /*if (this.CheckConfirmNews == true) {*/
      let stt = 13;
      this.http.put('/api/news/ShowHide/' + id + "/" + stt + "/" + isAll, undefined, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            if (stt == 12) {
              this.toastSuccess("Bài viết đã được duyệt");
            } else {
              this.toastSuccess("Bài viết đã được cập nhật");
            }
            this.GetListNews();
            this.viewRef.clear();
          }
          else {
            this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
            this.listNews[i].IsShow = !this.listNews[i].IsShow;
          }

        },
        (err) => {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          this.listNews[i].IsShow = !this.listNews[i].IsShow;
        }
      );
    this.viewRef.clear();
    /*}*/
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

    if (this.active) {
      this.GetListNews();
    } else if (this.activeU) {
      this.getMyPosts();
    } else if (this.activeD) {
      this.getPostsDraft();
    }
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

  RemoveAttachment(idx) {
    if (this.Item.listAttachment[idx].AttactmentId == undefined) {
      this.Item.listAttachment.splice(idx, 1);
    }
    else {
      this.Item.listAttachment[idx].Status = 99;
    }
  }

  SetIsMain(idx) {
    for (let i = 0; i < this.Item.listAttachment.length; i++) {
      this.Item.listAttachment[i].IsImageMain = false;
      if (idx == i) {
        this.Item.listAttachment[i].IsImageMain = true;
      }
    }
  }

  CheckActionTable(NewsId) {
    if (NewsId == undefined) {
      let CheckAll = this.CheckAll;
      this.listNews.forEach(item => {
        item.Action = CheckAll;
      });
    }
    else {
      let CheckAll = true;
      for (let i = 0; i < this.listNews.length; i++) {
        if (!this.listNews[i].Action) {
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
        this.listNews.forEach(item => {
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
                        this.GetListNews();
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

  ChangeLinkDetailNews(TypeNewsId, Url, NewsId) {
    return domain + this.listTypeNews.filter(x => x.Id == TypeNewsId)[0].ConstUrl + "/" + Url + "-" + NewsId + ".html";
  }

  OpenModalTag() {
    this.Tag = new Tag();
    this.TagModal.show();
  }

  SaveTag() {
    if (this.Tag.Name == undefined || this.Tag.Name == '') {
      this.toastWarning("Chưa nhập Tên hiển thị!");
      return;
    } else if (this.Tag.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập Tên hiển thị!");
      return;
    } else if (this.Tag.Url == undefined || this.Tag.Url == '') {
      this.toastWarning("Chưa nhập Đường dẫn!");
      return;
    } else if (this.Tag.Url.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập Đường dẫn!");
      return;
    }
    this.Tag.UserId = parseInt(localStorage.getItem("userId"));
    this.Tag.TargetType = 1;

    this.http.post('/api/tag', this.Tag, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          let obj = { TagId: res["data"].TagId, Name: res["data"].Name };
          // console.log(obj);
          // this.Item.listTag.push(obj);
          // console.log(this.Item.listTag);
          this.GetListTag(obj);
          this.TagModal.hide();
          this.toastSuccess("Thêm thành công!");
        }
        else {
          this.toastError(res["meta"]["error_message"]);
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
      }
    );
  }

  ChangeTitleTag() {
    this.Tag.Url = this.common.ConvertUrl(this.Tag.Name);
  }
  closeNoityfy() {
    this.isNoitify = true;
  }

  ShowConfirmStatus(id, status) {
    let title = "Bạn có chắc chắn muốn gửi biên tập bài viết này?";
    if (status == 11)
      title = "Bạn có chắc chắn muốn chuyển lên bài viết mới?";
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
            this.SendStatus(id, status);
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-danger',

        }
      ],
    });
  }

  SendStatus(id, status) {
    this.http.put('/api/news/ShowHide/' + id + "/" + status + "/0", undefined, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          if(status != 11)
            this.toastSuccess("Bài viết đã được gửi biên tập thành công!");
          else
            this.toastSuccess("Bài viết đã được chuyển thành bài viết mới thành công!");
          this.GetListNews();
          this.NewsModal.hide();
          this.viewRef.clear();
        }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          //this.listNews[i].IsShow = !this.listNews[i].IsShow;
        }

      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        //this.listNews[i].IsShow = !this.listNews[i].IsShow;
      }
    );
  }

  SaveNoteNews() {
    if (this.Item.Note == undefined) {
      this.toastWarning("Chưa nhập góp ý!");
      return;
    }

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
    obj.listRelated = [];
    this.listSuggestNews.forEach(item => {
      if (item.Check == true) {
        let it = { TargetRelatedId: item.NewsId }
        obj.listRelated.push(it);
      }
    });
    obj.listProductRelated = [];
    this.listSuggestProduct.forEach(item => {
      if (item.Check == true) {
        let it = { TargetRelatedId: item.ProductId }
        obj.listProductRelated.push(it);
      }
    });

    if (this.Item.NewsId == undefined) {
      let arr = [];
      obj.listCategory.forEach(item => {
        var flag = false;
        for (var i = 0; i < this.listCateNews.length; i++) {
          if (item.CategoryId == this.listCateNews[i].CategoryId && this.listCateNews[i].Check == true) {
            flag = true;
            break;
          }
        }

        if (!flag) {
          item.Check = false;
          arr.push(item);
        }
      });

      obj.listCategory = arr.concat(this.listCateNews.filter(e => e.Check == true));

      this.http.post('/api/news', obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListNews();
            this.toastSuccess("Đã thêm vào danh sách nháp!");
            this.NewsModal.hide();
            this.viewRef.clear();
          }
          else if (res["meta"]["error_code"] == 228) {
            this.toastError("Ngôn ngữ này đã có bài viết!");
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
    else {
      let arr = [];
      obj.listCategory.forEach(item => {
        var flag = false;
        for (var i = 0; i < this.listCateNews.length; i++) {
          if (item.CategoryId == this.listCateNews[i].CategoryId && this.listCateNews[i].Check == true) {
            flag = true;
            break;
          }
        }

        if (!flag) {
          item.Check = false;
          arr.push(item);
        }
      });

      obj.listCategory = arr.concat(this.listCateNews.filter(e => e.Check == true));

      this.http.put('/api/news/' + obj.NewsId, obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            //this.GetListNews();
            this.GetListNewsNote(this.Item.NewsId);
            this.Item.Note = undefined;
            this.toastSuccess("Góp ý thành công!");
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

  ExportExcel() {
    if (this.q.TypePaymentOrderStatus == 1)
      this.q.CashStatus = undefined;
    else if (this.q.TypePaymentOrderStatus == 2)
      this.q.CashStatus = true;
    else if (this.q.TypePaymentOrderStatus == 3)
      this.q.CashStatus = false;
    let data = Object.assign({}, this.q);

    data.TypeExport = 1;
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
  //OpenMediaModalVideo() {

  //  this.OpenMediaFileVideo.show();
  //}
  //CloseMediaModalVideo() {
  //  this.OpenMediaFileVideo.hide();
  //}
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
      
      for (var kk = 0; kk < this.Item.listAttachment.length; kk++) {
        if (this.Item.listAttachment[kk].Url == attachment.Url) {
          checkDuplicate = true;
          break;
        }
      }
      if (!checkDuplicate)
        this.Item.listAttachment.push(attachment);
      else {
        this.toastWarning("Bạn đã chọn ảnh này!");
      }
    }

    this.OpenMediaFile.hide();
  }
  //SeclectMediaVideo(item) {
  //  this.Item.LinkVideo = item.url + "/" + item.name;
  //  this.OpenMediaFileVideo.hide();
  //}
}
