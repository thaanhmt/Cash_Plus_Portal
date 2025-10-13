import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpRequest, HttpEventType, HttpHeaders } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { typeCategoryPage, listTemplate, domain, domainVideos, domainMedia, domainDebug } from '../../data/const';
import { Category } from '../../data/model';
import { CheckRole, Paging, QueryFilter } from '../../data/dt';
import { ToastrService } from 'ngx-toastr';
import { CommonService } from '../../service/common.service';
import { domainImage } from '../../data/const';
declare var loadNestable;
import { Subscription } from 'rxjs/Subscription';
import { CallCategoryFunctionService } from '../../service/call-category-function.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-page',
    templateUrl: './page.component.html',
    styleUrls: ['./page.component.scss']
})
export class PageComponent implements OnInit, OnDestroy {
    @ViewChild('CatePageModal') public CatePageModal: ModalDirective;
    @ViewChild('file') file: ElementRef;
    @ViewChild('fileIcon') fileIcon: ElementRef;
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

    public paging: Paging;
    public q: QueryFilter;
    public listLanguage = [];
    public listLanguageTemp = [];
    public listCatePage = [];
    public listCatePageParent = [];
    public ckeConfig: any;
    public typeCategoryPage = typeCategoryPage;
    public Item: Category;
    public ItemTranslate: Category;
    public progressIcon: number;
    public messageIcon: string;
    public domainImage = domainImage;
    public languageId: number;

    public httpOptions: any;
    public httpOptionsBlob: any;

    public RoleCode: string = localStorage.getItem("roleCode") || '';
    public query = "arr=6&arr=7&arr=8&arr=9&arr=10";
    public total_item: number;
    public isNoitify: boolean = false;
    public listFullCate = [];
    public listTemplate = listTemplate;
    public functionCode: string;

    public staticDomain: string;
    public staticDomainMedia: string;
    public domainDebug = domainDebug;

  key: string = 'categorySorts';

  public CheckRole: CheckRole;

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
        this.Item = new Category();
        this.paging = new Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "(TypeCategoryId=6 OR TypeCategoryId=7 OR TypeCategoryId=8 OR TypeCategoryId=9 OR TypeCategoryId=10)";
        this.paging.order_by = "CategoryId Desc";
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
        this.languageId = localStorage.getItem("languageId") != undefined ? parseInt(localStorage.getItem("languageId")) : 1;

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
      this.functionCode = "QLDMT";
      this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 0);
      this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 1);
      this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 2);
      this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 3);

        this.subscription = this.callCategoryFunctionService.getAction().subscribe(action => {
            if (action.TypeAction == 1) {
                this.OpenCatePageModal(undefined, action.CategoryId, 2);
            } else if (action.TypeAction == 2) {
                this.OpenCatePageModal(action.CategoryId, undefined, 3);
            } else if (action.TypeAction == 3) {
                this.ShowConfirmDelete(action.CategoryId);
            }
            else if (action.TypeAction == 5) {
                this.OpenCatePageModal(action.CategoryId, undefined, 5); // thêm danh mục vs ngôn ngữ mới
            }
            else if (action.TypeAction == 6) {
                this.OpenCatePageModal(action.CategoryId, undefined, 6); // sửa danh mục vs ngôn ngữ mới
            }
            else if (action.TypeAction == 7) {
              this.ShowHide(action.CategoryId, action.IsShow); // Đổi trạng thái danh mục
            }
        });
    }

    ngOnInit() {
        this.RoleCode = localStorage.getItem("roleCode");
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };
        this.GetListCatePage();
        this.GetListLanguage();
        this.GetListFullCate();
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
    ngOnDestroy() {
        this.subscription.unsubscribe();
        this.router.onSameUrlNavigation = 'ignore';
    }

    upload(files) {
        if (files.length === 0)
            return;

        const formData = new FormData();

        for (let file of files)
            formData.append(file.name, file);
        console.log(formData);
        const uploadReq = new HttpRequest('POST', 'api/upload/' , formData, {
            headers: new HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });
      this.http.request(uploadReq).subscribe(event => {
        if (event.type === HttpEventType.UploadProgress) {
        }
        else if (event.type === HttpEventType.Response) {
          this.Item.Image = event.body["data"].toString();
        }
      });
    }


    //Get danh sách danh mục trang
    GetListCatePage() {
        this.listCatePage = [];
        this.http.get('/api/category/GetCategorySort?' + this.query, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.listCatePage = res["data"];
                    this.total_item = res["metadata"]
                    loadNestable();
                }
            },
            (err) => {
                console.log("Error: connect to API");
            }
        );
    }

    //Get danh sách danh mục cha
    GetListCatePageParent(Id) {
        this.http.get('/api/category/GetByTree?arr=6&arr=7&arr=8&arr=9&arr=10&langId=' + this.Item.LanguageId, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.listCatePageParent = res["data"];
                    this.listCatePageParent.forEach(item => {
                        if (item.CategoryId == Id) {
                            item.disabled = true;
                        }

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
        this.ItemTranslate = new Category();
        this.http.get('/api/translate/' + id + '/' + sl + '/' + tl + '/1', this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.ItemTranslate = res["data"];
                    this.Item.CategoryId = undefined;
                    this.Item.CategoryParentId = undefined;
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

    selectLanguage() {
        this.GetListCatePageParent(undefined);
    }

    //Chuyển trang
    PageChanged(event) {
        this.paging.page = event.page;
        this.GetListCatePage();
    }

    //Toast cảnh báo
    toastWarning(msg): void {
        this.toastr.warning(msg, 'Cảnh báo');
    }

    //Toast thành công
    toastSuccess(msg): void {
        this.toastr.success(msg, 'Hoàn thành');
    }

    //Toast thành công
    toastError(msg): void {
        this.toastr.error(msg, 'Lỗi');
    }

    //
    QueryChanged() {
        //let query = '(TypeCategoryId=6 OR TypeCategoryId=7 OR TypeCategoryId=8 OR TypeCategoryId=9 OR TypeCategoryId=10)';
        //if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
        //    if (query != '') {
        //        query += ' and Name.Contains("' + this.q.txtSearch + '")';
        //    }
        //    else {
        //        query += 'Name.Contains("' + this.q.txtSearch + '")';
        //    }
        //}

        //if (query == '')
        //    this.paging.query = '1=1';
        //else
        //    this.paging.query = query;

        let query = "arr=6&arr=7&arr=8&arr=9&arr=10";
        if (this.q.LanguageId != undefined) {
            query = query + "&langId=" + this.q.LanguageId;
        }
        if (this.q.txtSearch != undefined && this.q.txtSearch != "") {
            query = query + "&txtSearch=" + this.q.txtSearch;
        }

        this.query = query;

        this.GetListCatePage();
    }

    //Mở modal thêm mới
    OpenCatePageModal(CategoryId, CategoryParentId, type) {
        this.Item = new Category();
        this.Item.Contents = "";
        this.Item.CategoryParentId = CategoryParentId;
        this.Item.TypeCategoryId = 6;
        this.Item.LanguageId = this.languageId;
        this.Item.Location = this.total_item + 1;
        this.file.nativeElement.value = "";
        this.fileIcon.nativeElement.value = "";
        this.message = undefined;
        this.messageIcon = undefined;
        this.progress = undefined;
        this.progressIcon = undefined;
        if (CategoryId != undefined) {
            
            let Cate = this.listFullCate.filter(x => x.CategoryId == CategoryId)[0];
            if (Cate) {
                this.Item = JSON.parse(JSON.stringify(Cate));
                if (type == 3 || type == 6) {
                    if (this.Item.CategoryParentId == 0) this.Item.CategoryParentId = undefined;
                    this.GetListCatePageParent(this.Item.CategoryId);
                    this.selectLanguage();
                    this.CatePageModal.show();
                }
                else if (type == 5) {
                    if (this.listLanguage.length == Cate.listLanguage.length + 1) {
                        this.toastWarning("Bạn đã thêm đủ ngôn ngữ!");
                        return;
                    }
                    this.listLanguageTemp = [];
                    this.Item.CategoryId = undefined;
                    this.Item.CategoryRootId = Cate.CategoryId;
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
                    this.GetTranslate(this.Item.CategoryRootId);
                    //
                    this.Item.CategoryParentId = undefined;
                    this.GetListCatePageParent(this.Item.CategoryId);
                    this.selectLanguage();
                    this.CatePageModal.show();
                }
            }
            else {
                this.toastError("Không tìm thấy danh mục trên hệ thống!");
                return;
            }
        }
        else {
            this.GetListCatePageParent(undefined);
            //this.selectLanguage();
            this.CatePageModal.show();
        }
    }

    //Thêm mới danh mục trang
    SaveCatePage() {
        if (this.Item.Code == undefined || this.Item.Code == '') {
            this.toastWarning("Chưa nhập Mã danh mục!");
            return;
        } else if (this.Item.Code.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập mã danh mục");
            return;
        } else if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập Tên danh mục!");
            return;
        } else if (this.Item.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên danh mục !");
            return;
        } else if (this.Item.TypeCategoryId == undefined || this.Item.TypeCategoryId == 0) {
            this.toastWarning("Chưa chọn Loại danh mục!");
            return;
        } else if (this.Item.LanguageId == undefined) {
            this.toastWarning("Chưa chọn ngôn ngữ!");
            return;
        }

        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));
        if (!this.Item.LanguageId) {
            this.Item.LanguageId = parseInt(localStorage.getItem("languageId"));
        }

        if (this.Item.CategoryId) {
            this.http.put('/api/Category/' + this.Item.CategoryId, this.Item, this.httpOptions).subscribe(
                (res) => {
                    if (res["meta"]["error_code"] == 200) {
                        this.GetListCatePage();
                        this.GetListFullCate();
                        this.CatePageModal.hide();
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
        else {
            this.http.post('/api/Category', this.Item, this.httpOptions).subscribe(
                (res) => {
                    if (res["meta"]["error_code"] == 200) {
                        this.GetListCatePage();
                        this.GetListFullCate();
                        this.CatePageModal.hide();
                        this.toastSuccess("Thêm mới thành công!");
                    }
                    else if (res["meta"]["error_code"] == 213) {
                        this.toastWarning("Tên đã tồn tại!");
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
    //Change title
    ChangeTitle(key) {
        if (this.Item.CategoryId == undefined) {
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
                text: "Bạn có chắc chắn muốn xóa bản ghi này?"
            },
            actionButtons: [
                {
                    text: 'Đồng ý',
                    buttonClass: 'btn btn-success',
                    onAction: () => {
                        this.DeleteCatePage(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-danger',

                }
            ],
        });
    }

    DeleteCatePage(Id) {
        this.http.delete('/api/Category/' + Id, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.GetListCatePage();
                    this.GetListFullCate();
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

    RemoveImage(Type) {
        if (Type == 5) {
            this.file.nativeElement.value = "";
            this.Item.Image = undefined;
            this.message = undefined;
            this.progress = undefined;
        }
        else {
            this.fileIcon.nativeElement.value = "";
            this.Item.Icon = undefined;
            this.messageIcon = undefined;
            this.progressIcon = undefined;
        }
    }

  ShowHide(id, IsShow) {
    let stt = IsShow ? 1 : 10;
    this.http.put('/api/category/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListCatePage();
          this.toastSuccess("Thay đổi trạng thái thành công!");
        }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
          this.GetListCatePage();
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        this.GetListCatePage();
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

        this.GetListCatePage();
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

    SaveSortCategory() {
        let attribute = document.getElementById("nestable");
        let Arr = [];
        this.common.ConvertHtmlToJson(Arr, attribute, "#nestable", 0, 1);

        this.http.post('/api/Category/SaveCategorySort', Arr, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.ResetCurrentRouter();
                    this.CatePageModal.hide();
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
        this.router.routeReuseStrategy.shouldReuseRoute = function() {
            return false;
        };
        this.router.onSameUrlNavigation = 'reload';
        this.router.navigateByUrl(this.router.url);
    }

    GetListFullCate() {
        let query = "(TypeCategoryId=6 OR TypeCategoryId=7 OR TypeCategoryId=8 OR TypeCategoryId=9 OR TypeCategoryId=10)";
        this.http.get('/api/category/GetByPage?page=1&query=' + query + '&order_by=', this.httpOptions).subscribe(
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
