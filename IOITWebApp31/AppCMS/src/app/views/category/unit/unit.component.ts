import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { typeCategoryNews, domainImage, domainMedia, domain, domainDebug, listStatus } from '../../../data/const';
import { ToastrService } from 'ngx-toastr';
import { Unit } from '../../../data/model';
import { CheckRole, Paging, QueryFilter } from '../../../data/dt';
import { CommonService } from '../../../service/common.service';
import * as $ from 'jquery';
declare var loadNestable;
import { Subscription } from 'rxjs/Subscription';
import { CallCategoryFunctionService } from '../../../service/call-category-function.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-unit',
  templateUrl: './unit.component.html',
  styleUrls: ['./unit.component.scss']
})

export class UnitComponent implements OnInit, OnDestroy {
  @ViewChild('CateNewsModal') public CateNewsModal: ModalDirective;
  @ViewChild('file') file: ElementRef;
  @ViewChild('OpenMediaFile') public OpenMediaFile: ModalDirective;
  subscription: Subscription;

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

  public q: QueryFilter;

  public listCateNews = [];
  public listCateParent = [];
  public listLanguage = [];
  public listLanguageTemp = [];
  public listOrderByCat = [];
  public listProvinces = [];
  public listDistricts = [];
  public listWards = [];
  public listStatus = listStatus;

  public ckeConfig: any;

  public typeCategoryNews = typeCategoryNews;

  public Item: Unit;
  public ItemTranslate: Unit;

  public progressIcon: number;
  public messageIcon: string;

  public domainImage = domainImage;

  public languageId: number;
  public httpOptions: any;

  public total_item: number;
  public txtSearch: string;
  public query = "1=1";
  public isNoitify: boolean = false;
  key: string = 'categorySorts';
  public functionCode: string;
  public listFullCate = [];

  public CheckRole: CheckRole;
  public staticDomain: string;
  public staticDomainMedia: string;
  public domainDebug = domainDebug;
  public typeAction: number;

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public common: CommonService,
    public callCategoryFunctionService: CallCategoryFunctionService,
    public elm: ElementRef,
    public router: Router
  ) {
    this.q = new QueryFilter();
    this.Item = new Unit();

    this.pagingFile = new Paging();
    this.pagingFile.page = 1;
    this.pagingFile.page_size = 24;
    this.pagingFile.query = "1=1";
    this.pagingFile.order_by = "";
    this.pagingFile.item_count = 0;
    this.countMedia = 24;

    this.languageId = localStorage.getItem("languageId") != undefined ? parseInt(localStorage.getItem("languageId")) : 1;
    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }

    this.CheckRole = new CheckRole();
    this.functionCode = "QLDMN";
    this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 0);
    this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 1);
    this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 2);
    this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 3);

    this.subscription = this.callCategoryFunctionService.getAction().subscribe(action => {
      if (action.TypeAction == 1) {
        this.OpenCateNewsModal(undefined, action.CategoryId, 2); // thêm danh mục con
      }
      else if (action.TypeAction == 2) {
        this.OpenCateNewsModal(action.CategoryId, undefined, 3); // sửa danh mục
      }
      else if (action.TypeAction == 3) {
        this.ShowConfirmDelete(action.CategoryId); // xóa danh mục
      }
      else if (action.TypeAction == 5) {
        this.OpenCateNewsModal(action.CategoryId, undefined, 5); // thêm danh mục vs ngôn ngữ mới
      }
      else if (action.TypeAction == 6) {
        this.OpenCateNewsModal(action.CategoryId, undefined, 6); // sửa danh mục vs ngôn ngữ mới
      }
      else if (action.TypeAction == 7) {
        this.ShowHide(action.CategoryId, action.IsShow); // Đổi trạng thái danh mục
      }
      else if (action.TypeAction == 8) {
        this.OpenCateNewsModal(action.CategoryId, undefined, 8); //view danh mục
      }
    });
  }

  ngOnInit() {
    this.GetListCateNews();
    this.GetListLanguage();
    this.GetListFullCate();
    this.GetListFiles();
    this.GetDomainStatic();
    this.GetListProvince();
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

  ngOnDestroy() {
    this.subscription.unsubscribe();
    this.router.onSameUrlNavigation = 'ignore';
  }

  //Get danh sách tin
  GetListCateNews() {
    this.listCateNews = [];
    //console.log(this.query);
    this.http.get('/api/unit/GetUnitSort?' + this.query, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCateNews = res["data"];
          this.total_item = res["metadata"]
          loadNestable();
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  QueryChanged() {
    let query = "1=1";
    if (this.q.LanguageId != undefined) {
      query = query + "&langId=" + this.q.LanguageId;
    }
    if (this.q.Status != undefined) {
      query = query + "&status=" + this.q.Status;
    }
    if (this.q.txtSearch != undefined && this.q.txtSearch != "") {
      query = query + "&txtSearch=" + this.q.txtSearch;
    }
    console.log(query);
    console.log(this.q.Status);
    this.query = query;

    this.GetListCateNews();
  }

  // Get danh sách ngôn ngữ
  GetListLanguage() {
    this.http.get('/api/language/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(
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

  GetListProvince() {
    this.http.get('/api/province/GetByPage?page=1&query=1=1&order_by=ProvinceId asc', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listProvinces = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListDistrict(reset) {
    if (reset) {
      this.Item.WardId = undefined;
      this.Item.DistrictId = undefined;
    }
    let query = "ProvinceId=" + this.Item.ProvinceId;
    this.http.get('/api/district/GetByPage?page=1&query=' + query +'&order_by=DistrictId asc', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listDistricts = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  GetListWard(reset) {
    if (reset) {
      this.Item.WardId = undefined;
    }
    let query = "DistrictId=" + this.Item.DistrictId;
    this.http.get('/api/wards/GetByPage?page=1&query=' + query +'&order_by=WardId asc', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listWards = res["data"];
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
    this.ItemTranslate = new Unit();
    this.http.get('/api/translate/' + id + '/' + sl + '/' + tl + '/1', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.ItemTranslate = res["data"];
          this.Item.UnitId = undefined;
          this.Item.UnitParentId = undefined;
          this.Item.Name = this.ItemTranslate.Name;
          this.Item.Url = this.ItemTranslate.Url;
          this.Item.Description = this.ItemTranslate.Description;
          this.Item.Contents = this.ItemTranslate.Contents;
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

  //GetListOrderByCat() {
  //  this.http.get('api/category/listNews/' + this.Item.CategoryId, this.httpOptions).subscribe(
  //    (res) => {
  //      if (res["meta"]["error_code"] == 200) {
  //        this.listOrderByCat = res["data"];
  //      }
  //    },
  //    (err) => {
  //      console.log("Error: connect to API");
  //    }
  //  );
  //}

  //Get danh sách danh mục cha

  GetListCateParent(Id) {
    this.http.get('/api/unit/GetByTree?arr=1&langId=' + this.Item.LanguageId, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCateParent = res["data"];
          this.listCateParent.forEach(item => {
            if (item.UnitId == Id || item.Genealogy.indexOf(Id) != -1)
              item.disabled = true;
            item.Space = "";
            for (var i = 0; i < (item.Level - 1) * 2; i++) {
              item.Space += "-";
            }
          })
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  selectLanguage() {
    this.GetListCateParent(undefined);
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

  //Mở modal thêm mới
  OpenCateNewsModal(CategoryId, CategoryParentId, type) {
    this.typeAction = type;
    this.Item = new Unit();
    this.Item.UnitParentId = CategoryParentId;
    this.Item.Type = 1;
    this.Item.Status = 1;
    this.Item.LanguageId = this.languageId;
    this.Item.Location = this.total_item + 1;
    this.file.nativeElement.value = "";
    /*this.fileIcon.nativeElement.value = "";*/
    this.message = undefined;
    this.messageIcon = undefined;
    this.progress = undefined;
    this.progressIcon = undefined;
    if (CategoryId != undefined) {
      let Cate = this.listFullCate.filter(x => x.UnitId == CategoryId)[0];
      if (Cate) {
        this.Item = JSON.parse(JSON.stringify(Cate));
        this.GetListDistrict(false);
        this.GetListWard(false);
        if (this.Item.Status == 1)
          this.Item.StatusView = true;
        else
          this.Item.StatusView = false;
        if (type == 3 || type == 6 || type==8) {
          if (this.Item.UnitParentId == 0) this.Item.UnitParentId = undefined;
          this.GetListCateParent(this.Item.UnitId);
          this.CateNewsModal.show();
        }
        else if (type == 5) {
          if (this.listLanguage.length == Cate.listLanguage.length + 1) {
            this.toastWarning("Bạn đã thêm đủ ngôn ngữ!");
            return;
          }
          this.listLanguageTemp = [];
          this.Item.UnitId = undefined;
          this.Item.UnitRootId = Cate.CategoryId;
          this.Item.LanguageRootId = this.Item.LanguageId;
          this.Item.LanguageRootCode = this.Item["language"]["Code"];
          this.Item.LanguageId = undefined;
          this.Item.LanguageCode = undefined;
          //check ngôn ngữ
          for (var i = 0; i < this.listLanguage.length; i++) {
            let check = false;
            if (this.listLanguage[i].LanguageId == this.languageId) {
              check = true;
            }
            if (Cate.listLanguage.length > 0) {
              for (var j = 0; j < Cate.listLanguage.length; j++) {
                if (this.listLanguage[i].LanguageId == Cate.listLanguage[j].LanguageId2) {
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
          this.GetTranslate(this.Item.UnitRootId);
          //
          this.Item.UnitParentId = undefined;
          this.GetListCateParent(this.Item.UnitId);
          this.CateNewsModal.show();
        }
      }
      else {
        this.toastError("Không tìm thấy danh mục trên hệ thống!");
        return;
      }
    }
    else {
      this.GetListCateParent(undefined);
      if (this.Item.Status == 1)
        this.Item.StatusView = true;
      else
        this.Item.StatusView = false;
      this.CateNewsModal.show();
    }
  }

  //Thêm mới danh mục trang
  SaveCateNews() {
    if (this.Item.Code == undefined || this.Item.Code == '') {
      this.toastWarning("Chưa nhập mã định danh cơ quan/tổ chức!");
      return;
    } else if (this.Item.Code.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập mã định danh cơ quan/tổ chức");
      return;
    } else if (this.Item.Name == undefined || this.Item.Name == '') {
      this.toastWarning("Chưa nhập tên cơ quan/tổ chức!");
      return;
    } else if (this.Item.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên cơ quan/tổ chức");
      return;
    }
    //else if (this.Item.Url == undefined || this.Item.Url == '') {
    //  this.toastWarning("Chưa nhập Đường dẫn!");
    //  return;
    //}
    //else if (this.Item.Url.replace(/ /g, '') == '') {
    //  this.toastWarning("Chưa nhập đường dẫn!");
    //  return;
    //}
    //else if (this.Item.Type == undefined || this.Item.Type == 0) {
    //  this.toastWarning("Chưa chọn Loại danh mục!");
    //  return;
    //}
    else if (this.Item.LanguageId == undefined) {
      this.toastWarning("Chưa chọn ngôn ngữ!");
      return;
    }

    this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
    this.Item.CreatedId = parseInt(localStorage.getItem("userId"));
    this.Item.UpdatedId = parseInt(localStorage.getItem("userId"));
    this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));
    if (!this.Item.LanguageId) {
      this.Item.LanguageId = parseInt(localStorage.getItem("languageId"));
    }
    if (this.Item.StatusView ==true)
      this.Item.Status = 1;
    else
      this.Item.Status = 98;
    if (this.Item.UnitId) {
      this.http.put('/api/unit/' + this.Item.UnitId, this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.ResetCurrentRouter();
            this.CateNewsModal.hide();
            this.toastSuccess("Cập nhật thành công!");
          }
          else if (res["meta"]["error_code"] == 213) {
            this.toastWarning("Mã đã tồn tại!");
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
      this.http.post('/api/unit', this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.ResetCurrentRouter();
            this.CateNewsModal.hide();
            this.toastSuccess("Thêm mới thành công!");
          }
          else if (res["meta"]["error_code"] == 213) {
            this.toastWarning("Mã đã tồn tại!");
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

  ChangeTitle(key) {
    if (this.Item.UnitId == undefined) {
      switch (key) {
        case 1:
          this.Item.MetaTitle = this.Item.Name;
          this.Item.MetaKeyword = this.Item.Name;
          this.Item.Url = this.common.ConvertUrl(this.Item.Name);
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
        text: "Bạn có chắc chắn muốn xóa cơ quan/tổ chức này và các cơ quan/tổ chức con của nó?"
      },
      actionButtons: [
        {
          text: 'Đồng ý',
          buttonClass: 'btn btn-success',
          onAction: () => {
            this.DeleteCateNews(Id);
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-danger',

        }
      ],
    });
  }

  DeleteCateNews(Id) {
    this.http.delete('/api/unit/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.ResetCurrentRouter();
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

  findParent(item) {
    if (item == undefined) {
      return "";
    }
    else {
      return item.Name;
    }
  }

  upload(files, Type) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);
    const uploadReq = new HttpRequest('POST', 'api/upload/uploadImage/' + Type, formData, {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
      reportProgress: true,
    });

    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress) {
        if (Type == 5) {
          this.progress = Math.round(100 * event.loaded / event.total);
        }
        else {
          this.progressIcon = Math.round(100 * event.loaded / event.total);
        }
      }
      else if (event.type === HttpEventType.Response) {
        if (Type == 5) {
          this.message = event.body["data"].toString();
          this.Item.Image = this.message
        }
        else {
          this.messageIcon = event.body["data"].toString();
          this.Item.Icon = this.messageIcon;
        }
      }
    });
  }

  RemoveImage(Type) {
    if (Type == 5) {
      this.file.nativeElement.value = "";
      this.Item.Image = undefined;
      this.message = undefined;
      this.progress = undefined;
    }
    else {
      /*this.fileIcon.nativeElement.value = "";*/
      this.Item.Icon = undefined;
      this.messageIcon = undefined;
      this.progressIcon = undefined;
    }
  }

  ShowHide(id, IsShow) {
    let stt = IsShow ? 1 : 10;
    this.http.put('/api/unit/showHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListCateNews();
          this.GetListFullCate();
          this.ResetCurrentRouter();
          this.toastSuccess("Thay đổi trạng thái thành công!");
        }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          this.GetListCateNews();
          this.GetListFullCate();
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        this.GetListCateNews();
        this.GetListFullCate();
      }
    );
  }

  SaveSortCategory() {
    let attribute = document.getElementById("nestable");
    let Arr = [];
    this.common.ConvertHtmlToJson(Arr, attribute, "#nestable", 0, 1);

    this.http.post('/api/unit/SaveUnitSort', Arr, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.ResetCurrentRouter();
          this.CateNewsModal.hide();
          this.toastSuccess("Lưu thông tin sắp xếp thành công!");
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

  ResetCurrentRouter() {
    this.router.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
    };
    this.router.onSameUrlNavigation = 'reload';
    this.router.navigateByUrl(this.router.url);
  }

  GetListFullCate() {
    let query = "Type=1";
    this.http.get('/api/unit/GetByPage?page=1&query=' + query + '&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listFullCate = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  closeNoityfy() {
    this.isNoitify = true;
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
    this.Item.Image = undefined;
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
    this.Item.Image = item.url + "/" + item.name;
    this.OpenMediaFile.hide();
  }
}
