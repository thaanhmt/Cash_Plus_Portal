import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { typeCategoryPage, domainImage } from '../../../data/const';
import { Category, Attactment } from '../../../data/model';
import { Paging, QueryFilter } from '../../../data/dt';
import { ToastrService } from 'ngx-toastr';
import { CommonService } from '../../../service/common.service';
declare var loadNestable;
import { Subscription } from 'rxjs/Subscription';
import { CallCategoryFunctionService } from '../../../service/call-category-function.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-product',
    templateUrl: './product.component.html',
    styleUrls: ['./product.component.scss']
})

export class CateProductComponent implements OnInit, OnDestroy {
    @ViewChild('CateProductModal') public CateProductModal: ModalDirective;
    @ViewChild('file') file: ElementRef;
    @ViewChild('fileIcon') fileIcon: ElementRef;
    //@ViewChild('fileSlide') fileSlide: ElementRef;

    subscription: Subscription;

    public paging: Paging;
    public q: QueryFilter;
    public listCateProduct = [];
    public listCateParent = [];
    public listLanguage = [];
    public listLanguageTemp = [];
    public listOrderByCatProduct = [];
    public ckeConfig: any;
    public typeCategoryPage = typeCategoryPage;
    public Item: Category;
    public ItemTranslate: Category;
    public ImageAttact: Attactment;
    public progress: number;
    public progressIcon: number;
    public progressSlide: number;
    public message: string;
    public messageIcon: string;
    public domainImage = domainImage;
    public languageId: number;
    public httpOptions: any;

    public query = "arr=11";
    public total_item: number;

    key: string = 'categorySorts';

    public listFullCate = [];

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
        this.paging.query = "TypeCategoryId=11";
        this.paging.order_by = "CategoryId Desc";
        this.paging.item_count = 0;

        this.q = new QueryFilter();
        this.q.txtSearch = "";
        this.languageId = localStorage.getItem("languageId") != undefined ? parseInt(localStorage.getItem("languageId")) : 1;
        this.httpOptions = {
            headers: new HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        }

        this.subscription = this.callCategoryFunctionService.getAction().subscribe(action => {
            if (action.TypeAction == 1) {
                this.OpenCateProductModal(undefined, action.CategoryId, 2);
            }
            else if (action.TypeAction == 2) {
                this.OpenCateProductModal(action.CategoryId, undefined, 3);
            }
            else if (action.TypeAction == 3) {
                this.ShowConfirmDelete(action.CategoryId);
            }
            else if (action.TypeAction == 5) {
                this.OpenCateProductModal(action.CategoryId, undefined, 5); // thêm danh mục vs ngôn ngữ mới
            }
            else if (action.TypeAction == 6) {
                this.OpenCateProductModal(action.CategoryId, undefined, 6); // sửa danh mục vs ngôn ngữ mới
            }
            else if (action.TypeAction == 7) {
                this.ShowHide(action.CategoryId, action.IsShow); // Đổi trạng thái danh mục sản phẩ
            }
        });
    }

    ngOnInit() {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };

        this.GetListCateProduct();
        this.GetListLanguage();
        this.GetListFullCate();
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
        this.router.onSameUrlNavigation = 'ignore';
    }

    //Get danh sách danh mục sản phẩm
    GetListCateProduct() {
        this.listCateProduct = [];
        this.http.get('/api/category/GetCategorySort?' + this.query, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.listCateProduct = res["data"];
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
    GetListCateParent(Id) {
        this.http.get('/api/category/GetByTree?arr=11&langId=' + this.Item.LanguageId, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.listCateParent = res["data"];
                    this.listCateParent.forEach(item => {
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

    GetListOrderByCat() {
        this.http.get('api/category/listproduct/' + this.Item.CategoryId, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.listOrderByCatProduct = res["data"];
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
        this.GetListCateProduct();
    }
    //Cảnh báo
    toastWarning(msg): void {
        this.toastr.warning(msg, 'Cảnh báo');
    }
    //Hoàn thành
    toastSuccess(msg): void {
        this.toastr.success(msg, 'Hoàn thành');
    }
    //Lỗi
    toastError(msg): void {
        this.toastr.error(msg, 'Lỗi');
    }
    //
    QueryChanged() {
        let query = 'arr=11';
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

        if (this.q.LanguageId != undefined) {
            query = query + "&langId=" + this.q.LanguageId;
        }

        this.query = query;

        this.GetListCateProduct();
    }

    selectLanguage() {
        this.GetListCateParent(undefined);
    }

    //Mở modal thêm mới
    OpenCateProductModal(CategoryId, CategoryParentId, type) {
        //this.GetListLanguage();
        this.Item = new Category();
        this.Item.Contents = "";
        this.Item.CategoryParentId = CategoryParentId;
        this.Item.TypeCategoryId = 1;
        this.Item.LanguageId = this.languageId;
        this.Item.Location = this.total_item + 1;
        this.file.nativeElement.value = "";
        this.fileIcon.nativeElement.value = "";
        //this.fileSlide.nativeElement.value = "";
        this.message = undefined;
        this.messageIcon = undefined;
        this.progress = undefined;
        this.progressIcon = undefined;
        this.progressSlide = undefined;
        this.Item.listImage = [];

        if (CategoryId != undefined) {
            let Cate = this.listFullCate.filter(x => x.CategoryId == CategoryId)[0];
            if (Cate) {
                this.Item = JSON.parse(JSON.stringify(Cate));
                console.log(this.Item);
                if (type == 3 || type == 6) {
                    if (this.Item.CategoryParentId == 0) this.Item.CategoryParentId = undefined;
                    this.GetListCateParent(this.Item.CategoryId);
                    this.CateProductModal.show();
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
                    this.GetListCateParent(this.Item.CategoryId);
                    this.CateProductModal.show();
                }
            }
            else {
                this.toastError("Không tìm thấy danh mục trên hệ thống!");
                return;
            }
        }
        else {
            this.GetListCateParent(undefined);
            this.CateProductModal.show();
        }
    }

    //Thêm mới danh mục trang
    SaveCateProduct() {
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
            this.toastWarning("Chưa nhập tên danh mục");
            return;
        } else if (this.Item.Url == undefined || this.Item.Url == '') {
            this.toastWarning("Chưa nhập Đường dẫn!");
            return;
        } else if (this.Item.Url.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập đường dẫn !");
            return;
        }

        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));
        this.Item.TypeCategoryId = 11;
        if (!this.Item.LanguageId) {
            this.Item.LanguageId = parseInt(localStorage.getItem("languageId"));
        }

        if (this.Item.CategoryId) {
            this.http.put('/api/Category/' + this.Item.CategoryId, this.Item, this.httpOptions).subscribe(
                (res) => {
                    if (res["meta"]["error_code"] == 200) {
                        this.GetListCateProduct();
                        this.GetListFullCate();
                        this.CateProductModal.hide();
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
                        this.GetListCateProduct();
                        this.GetListFullCate();
                        this.CateProductModal.hide();
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
                        console.log('OnAction');
                        this.DeleteCateProduct(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-default',

                }
            ],
        });
    }

    DeleteCateProduct(Id) {
        this.http.delete('/api/Category/' + Id, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.GetListCateProduct();
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

    upload(files, Type) {
        if (files.length === 0)
            return;

        const formData = new FormData();

        for (let file of files)
            formData.append(file.name, file);
        console.log(formData);
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
                else if (Type == 3) {
                    this.progressSlide = Math.round(100 * event.loaded / event.total);
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
                else if (Type == 3) {
                    console.log(this.Item.listImage);
                    //this.message = event.body["data"].toString();
                    event.body["data"].forEach(item => {
                        this.ImageAttact = new Attactment();
                        this.ImageAttact.Url = item;
                        this.ImageAttact.Thumb = item;
                        this.ImageAttact.IsImageMain = false;
                        this.ImageAttact.Status = 1;
                        this.Item.listImage.push(this.ImageAttact);
                    });
                }
                else {
                    this.messageIcon = event.body["data"].toString();
                    this.Item.Icon = this.messageIcon;
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

    RemoveImageSlide(idx) {
        if (this.Item.listImage[idx].AttactmentId == undefined) {
            this.Item.listImage.splice(idx, 1);
        }
        else {
            this.Item.listImage[idx].Status = 99;
        }
    }

    SetIsMain(idx) {
        for (let i = 0; i < this.Item.listImage.length; i++) {
            this.Item.listImage[i].IsImageMain = false;
            if (idx == i) {
                this.Item.listImage[i].IsImageMain = true;
            }
        }
    }

    ShowHide(id, IsShow) {
        let stt = IsShow ? 1 : 10;
        this.http.put('/api/Category/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.toastSuccess("Thay đổi trạng thái thành công!");
                    this.GetListCateProduct();
                }
                else {
                    this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                    this.GetListCateProduct();
                }
            },
            (err) => {
                this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                this.GetListCateProduct();
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

        this.GetListCateProduct();
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
                    this.CateProductModal.hide();
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
        let query = "TypeCategoryId=11";
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
}
