import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { domainImage, ProductReviewStatus, ActionTable, domain, typeProduct } from '../../../data/const';
import { Product, Attribute, ImageProduct, ProductAttribuiteChild, AttributeMapping, Attactment } from '../../../data/model';
import { ToastrService } from 'ngx-toastr';
import { CommonService } from '../../../service/common.service';
import { DatePipe } from '@angular/common';
import { Paging, QueryFilter } from '../../../data/dt';
import { DateTimeAdapter, OWL_DATE_TIME_FORMATS, OWL_DATE_TIME_LOCALE } from 'ng-pick-datetime';
import { MomentDateTimeAdapter } from 'ng-pick-datetime-moment';
import { TabsetComponent } from 'ngx-bootstrap/tabs';


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
    selector: 'app-product',
    templateUrl: './product.component.html',
    styleUrls: ['./product.component.scss'],
    providers: [
        { provide: DateTimeAdapter, useClass: MomentDateTimeAdapter, deps: [OWL_DATE_TIME_LOCALE] },
        { provide: OWL_DATE_TIME_FORMATS, useValue: MY_CUSTOM_FORMATS }
    ]
})
export class ProductComponent implements OnInit {
    @ViewChild('ProductModal') public ProductModal: ModalDirective;
    @ViewChild('OrderByModal') public OrderByModal: ModalDirective;
    @ViewChild('ProductReviewModal') public ProductReviewModal: ModalDirective;
    @ViewChild('AttribuiteModal') public AttribuiteModal: ModalDirective;
  @ViewChild('DocumentModal') public DocumentModal: ModalDirective;

    @ViewChild('file') file: ElementRef;
    @ViewChild('fileDoc') fileDoc: ElementRef;
    @ViewChild('tabset') tabset: TabsetComponent;

    public paging: Paging;
    public q: QueryFilter;

    public pagingReview: Paging;
    public qReview: QueryFilter;

    // public Tag: string;
    public listProduct = [];
    public listProductCat = [];
    public listProductReview = [];
    public listTrademark = [];
    public listLanguage = [];
    public listAttacment = [];
    public listAttribuite = [];
    public listAttributeParentId = [];
    public listAttributeCustom = [];
    public listLanguageTemp = [];
    public ckeConfig: any;
    public PriceMin = 0;
    public PriceMax = 0;
    public ItemAttriBui = [];
    public listPrice = [];
    public listAttributeMapping = [];
    public listProductAttribuiteChild = [];

    public Item: Product;
    public ItemTranslate: Product;
    public ItemAt: Attactment;
    public ImageProduct: ImageProduct;
    public ItemProductAttribuiteChild: ProductAttribuiteChild;
    public ItemAttributeMapping: AttributeMapping;
    public listManufacture = [];
    public listCateNews = [];
    public listOrderByProduct = [];

    public domainImage = domainImage;
    public typeProduct = typeProduct;
    public domain = domain;
    public progress: number;
    public languageId: number;

    public httpOptions: any;

    public listSuggestProduct = [];
    public ProductName = "";
    public ProductId: number;
    public ProductReviewStatus = ProductReviewStatus;

    public attribuites = [];
    public ItemAttribuite: Attribute;

    public ActionTable = ActionTable;
    public ActionId: number;
    public CheckAll: boolean;
    public total: any;

    PriceCurrencyMaskConfig = {
        align: "left",
        allowNegative: false,
        decimal: ".",
        precision: 0,
        prefix: "",
        suffix: " Vnđ",
        thousands: ","
    };

    StockQuantityMaskConfig = {
        align: "left",
        allowNegative: false,
        decimal: ".",
        precision: 0,
        prefix: "",
        suffix: "",
        thousands: ","
    };

    constructor(
        public http: HttpClient,
        public modalDialogService: ModalDialogService,
        public viewRef: ViewContainerRef,
        public toastr: ToastrService,
        public common: CommonService,
        public datePipe: DatePipe
    ) {
        this.Item = new Product();
      this.paging = new Paging();
        this.ItemProductAttribuiteChild = new ProductAttribuiteChild();
      this.ItemAttributeMapping = new AttributeMapping();
      this.ItemAt = new Attactment();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "TypeProduct=1 OR TypeProduct=2";
        this.paging.order_by = "ProductId Desc";
        this.paging.item_count = 0;

        this.q = new QueryFilter();
        this.q.txtSearch = "";

        this.pagingReview = new Paging();
        this.pagingReview.page = 1;
        this.pagingReview.page_size = 10;
        this.pagingReview.query = "1=1";
        this.pagingReview.order_by = "";
        this.pagingReview.item_count = 0;

        this.qReview = new QueryFilter();
        this.qReview.txtSearch = "";
        this.languageId = localStorage.getItem("languageId") != undefined ? parseInt(localStorage.getItem("languageId")) : 1;
        //this.paging.query = "LanguageId=" + this.languageId;
        this.httpOptions = {
            headers: new HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        }

        this.ItemAttribuite = new Attribute();
    }

    ngOnInit() {
        this.ckeConfig = {
            allowedContent: false,
            extraPlugins: 'divarea',
            forcePasteAsPlainText: true
        };

        this.GetListProduct();
      this.GetListCatPro();
        this.GetListLanguage();
        this.GetListManufacture();
        this.GetListTrademark();
      this.GetListAttribuite();
      this.GetAttribuites();


    }
  // get category all
  GetListCatPro() {
    this.http.get('/api/Category/GetAllCatProduct?page=1&query=1=1&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listProductCat = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
    //Get danh sách sản phẩm
    GetListProduct() {
        this.http.get('/api/product/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.listProduct = res["data"];
                    this.listProduct.forEach(item => {
                        item.IsShow = item.Status == 1 ? true : false;
                    });
                    this.paging.item_count = res["metadata"].Sum;
                  this.total = res["metadata"];
                
                  for (let i = 0; i < this.listProduct.length; i++) {
                    if (this.listProduct[i].TypeProduct == 2) {
                      this.listPrice = [];
                      this.PriceMin = 0;
                      this.PriceMax = 0;
                      for (let j = 0; j < this.listProduct[i].listProductAttribute.length; j++) {

                        this.listPrice.push(this.listProduct[i].listProductAttribute[j].Price);
                        console.log(this.listPrice);
                        if (j == (this.listProduct[i].listProductAttribute.length - 1)) {
                          var maxInNumbers = Math.max.apply(Math, this.listPrice);
                          var minInNumbers = Math.min.apply(Math, this.listPrice);
                          for (let a = 0; a < this.listProduct[i].listProductAttribute.length; a++) {
                            this.listProduct[i].listProductAttribute[a].PriceMin = minInNumbers;
                            this.listProduct[i].listProductAttribute[a].PriceMax = maxInNumbers;
                          }
                        }
                      }
                      console.log('----------------');
                    }
                    
                  }
                  //console.log(this.listProduct);
                }
            },
            (err) => {
                console.log("Error: connect to API");
            }
        );
    }
  //GET thuoc tinh
  GetListAttribuite() {
    this.http.get('/api/attribute/GetByPage?page=' + this.paging.page + '&query=AttributeParentId=0&order_by=', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listAttribuite = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  // chon bien the
  SaveAttriBui() {
    console.log(this.listProductAttribuiteChild);
  }
    CreateAttribui(list) {

    this.listAttributeMapping = [];
    this.ItemProductAttribuiteChild = new ProductAttribuiteChild();
    this.ItemProductAttribuiteChild.Status = 1;
    this.ItemProductAttribuiteChild.ProductId = this.Item.ProductId;
    for (let i = 0; i < list.length; i++) {
      for (let j = 0; j < this.listAttribuite.length; j++) {
        if (list[i] == this.listAttribuite[j].AttributeId) {
          this.ItemAttributeMapping = new AttributeMapping();
          if (this.listProductAttribuiteChild.length <= 0) {
            this.listAttributeCustom.push(this.listAttribuite[j]);
          }
          
          this.ItemAttributeMapping.AttributeId = list[i];
          
          this.ItemAttributeMapping["listAttributeChild"] = this.listAttribuite[j]["listAttributeChild"];
          this.listAttributeMapping.push(this.ItemAttributeMapping);
          break;
        }
      }
    }
    this.ItemProductAttribuiteChild["listAttribute"] = this.listAttributeMapping;
    this.listProductAttribuiteChild.push(this.ItemProductAttribuiteChild);
    console.log(this.listProductAttribuiteChild);
  }
  // Xoa bien thẻ
  DeteleAtrri(id) {
    var stt = this.listProductAttribuiteChild.findIndex((obj => obj.ProductAttributeId == id));
    this.listProductAttribuiteChild[stt].Status = 99;
    console.log(this.listProductAttribuiteChild);
  }
    //Lấy toàn bộ danh sách sản phẩm
    GetListAllProduct() {
        let query = "LanguageId=" + this.Item.LanguageId;;
        if (this.Item.ProductId != undefined) {
          query += " and TypeProduct=1 or TypeProduct=2  and ProductId !=" + this.Item.ProductId
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

                    if (this.Item.ProductId != undefined) {
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

                    //if (this.listLanguage.length == 1 && (this.Item.NewsId == undefined || (this.Item.NewsId != undefined && this.Item.LanguageId == undefined))) {
                    //    this.Item.LanguageId = this.listLanguage[0].LanguageId;
                    //}
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
        this.ItemTranslate = new Product();
        this.http.get('/api/translate/' + id + '/' + sl + '/' + tl + '/3', this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.ItemTranslate = res["data"];
                    this.Item.ProductId = undefined;
                    this.Item.Name = this.ItemTranslate.Name;
                    this.Item.Url = this.ItemTranslate.Url;
                    this.Item.Description = this.ItemTranslate.Description;
                    this.Item.Contents = this.ItemTranslate.Contents;
                    this.Item.Feature = this.ItemTranslate.Feature;
                    this.Item.Configuration = this.ItemTranslate.Configuration;
                    this.Item.NoteTech = this.ItemTranslate.NoteTech;
                    this.Item.NotePromotion = this.ItemTranslate.NotePromotion;
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

    //Danh sách nhà sản xuất
    GetListManufacture() {
        this.http.get('/api/manufacturer/GetByPage?page=1&query=TypeOriginId=1&order_by=', this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.listManufacture = res["data"];
                }
            },
            (err) => {
                console.log("Error: connect to API");
            }
        );
    }

    //Danh sách thương hiệu
    GetListTrademark() {
        this.http.get('/api/manufacturer/GetByPage?page=1&query=TypeOriginId=2&order_by=', this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.listTrademark = res["data"];
                }
            },
            (err) => {
                console.log("Error: connect to API");
            }
        );
    }

    selectLanguage() {
        this.GetListCateNews();
    }

    //Chuyển trang
    PageChanged(event) {
        this.paging.page = event.page;
        this.GetListProduct();
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
        let query = '(TypeProduct=1 OR TypeProduct=2)';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and Name.Contains("' + this.q.txtSearch + '")';
            }
            else {
                query += 'Name.Contains("' + this.q.txtSearch + '")';
            }
        }

      if (this.q["CategoryId"] != undefined) {
            if (query != '') {
              query += ' and CategoryId=' + this.q["CategoryId"];
            }
            else {
              query += 'CategoryId=' + this.q["CategoryId"];
            }
        }

        if (this.q["TrademarkId"] != undefined) {
            if (query != '') {
                query += ' and TrademarkId=' + this.q["TrademarkId"];
            }
            else {
                query += 'TrademarkId=' + this.q["TrademarkId"];
            }
        }

        if (this.q.LanguageId != undefined) {
            if (query != '') {
                query += ' and LanguageId=' + this.q.LanguageId;
            }
            else {
                query += 'LanguageId=' + this.q.LanguageId;
            }
        }

        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;

        this.GetListProduct();
    }

    //Mở modal thêm mới
  OpenProductModal(item, type) {
    this.listAttributeParentId = [];
    this.listAttributeCustom = [];
    this.listProductAttribuiteChild = [];
    this.listAttacment = [];
    this.tabset.tabs[0].active = true;
    this.Item = new Product();
    this.Item.Contents = "";
    this.Item.Feature = "";
    this.Item.Configuration = "";
    this.Item.NoteTech = "";
    this.Item.NotePromotion = "";
    this.Item.Description = "";
        this.listLanguageTemp = this.listLanguage;
        this.Item.LanguageId = this.languageId;
        this.Item.ViewNumber = 1;
        this.file.nativeElement.value = "";
        this.progress = undefined;
        this.listCateNews = [];
        this.Item.TypeProduct = 1;
        this.Item.listImage = [];
      
    if (item != undefined) {
        this.listAttacment = item.listDocument;
        this.Item = JSON.parse(JSON.stringify(item));
            if (type == 1 || type == 3) {
                //console.log(this.Item);
              if (item.listProductAttribute.length > 0) {
                this.Item.TypeProduct = 2;
                for (let i = 0; i < item.listProductAttribute[0].listAttribute.length; i++) {
                  this.listAttributeParentId.push(item.listProductAttribute[0].listAttribute[i].AttributeId);
                }

                for (let i = 0; i < this.listAttributeParentId.length; i++) {
                  for (let j = 0; j < this.listAttribuite.length; j++) {
                    if (this.listAttributeParentId[i] == this.listAttribuite[j].AttributeId) {
                      
                        this.listAttributeCustom.push(this.listAttribuite[j]);
                      
                      break;
                    }
                  }
                }
                //console.log(this.listAttributeParentId);
                //console.log(this.listAttributeCustom);
                //console.log(this.listProductAttribuiteChild);
              }
                this.listProductAttribuiteChild = item.listProductAttribute;

                
            }
            else if (type == 2) {
                if (this.listLanguage.length == item.listLanguage.length + 1) {
                    this.toastWarning("Bạn đã thêm đủ ngôn ngữ!");
                    return;
                }

                this.listLanguageTemp = [];
                this.Item.ProductId = undefined;
                this.Item.ProductRootId = item.ProductId;
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
                            if (this.listLanguage[i].LanguageId == item.listLanguage[j].LanguageId2) {
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
                this.GetTranslate(this.Item.ProductRootId);

                //this.Item["LangName"] = item.language != undefined ? item.language.Name : "";
                //this.Item["LangFlag"] = item.language != undefined ? item.language.Flag : "";
                //this.Item["LangTitle"] = item.Name;

            } else {
                this.Item.listAttribute = JSON.parse(JSON.stringify(this.attribuites));
            }
        }
        this.GetListAllProduct();
        this.GetListCateNews();

        this.ProductModal.show();
  }

  // mo pupop them tai lieu
  OpenDocumentModal() {
    this.fileDoc.nativeElement.value = "";
    this.ItemAt = new Attactment();
    this.DocumentModal.show();
  }
  // xoa file khoi list
  DeteleAtt(i) {
    this.listAttacment[i].Status = 99;
  }
  // luu file danh sach file document
  SaveFileDocument() {
    if (this.ItemAt.Name == '' || this.ItemAt.Name == undefined) {
      this.toastWarning('Chưa nhập tên file đính kèm !');
      return;
    } else if (this.ItemAt.Url == '' || this.ItemAt.Url == undefined){
      this.toastWarning('Chưa chọn file đính kèm !');
      return;
    }
    this.listAttacment.push(this.ItemAt);
    this.DocumentModal.hide();

  }

    //Thêm mới danh mục trang
    SaveProduct() {
        if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập Tên sản phẩm!");
            return;
        } else if (this.Item.Name.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập tên sản phẩm!");
            return;
        } else if (this.Item.Url == undefined || this.Item.Url == '') {
            this.toastWarning("Chưa nhập Đường dẫn!");
            return;
        } else if (this.Item.Url.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập đường dẫn!");
            return;
        } else if (this.Item.StockQuantity == undefined) {
            this.toastWarning("Chưa nhập Số lượng sản phẩm!");
            return;
        }
      this.Item.listDocument = this.listAttacment;
      //for (let i = 0; i < this.listAttacment.length; i++) {
      //  if (this.listAttacment[i].Status != 99) {
      //    this.Item.listDocument.push(this.listAttacment[i]);
      //  }
      //}

      this.Item["listProductAttribute"] = this.listProductAttribuiteChild;

        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));

        if (typeof this.Item.DateStartActive === 'object' && this.Item.DateStartActive != undefined) {
            let DateStartActive = this.Item.DateStartActive.add(7, 'hours');
            this.Item.DateStartActive = DateStartActive.toISOString();
        }

        this.Item.listRelated = [];
        this.listSuggestProduct.forEach(item => {
            if (item.Check == true) {
                let obj = { TargetRelatedId: item.ProductId }
                this.Item.listRelated.push(obj);
            }
        });

        if (this.Item.ProductId == undefined) {
            this.Item.listCategory = [];
            this.listCateNews.forEach(item => {
                if (item.Check) {
                    this.Item.listCategory.push(item);
                }
            });


            this.http.post('/api/Product', this.Item, this.httpOptions).subscribe(
                (res) => {
                    if (res["meta"]["error_code"] == 200) {
                        this.GetListProduct();
                        this.ProductModal.hide();
                        this.toastSuccess("Thêm mới thành công!");
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
            this.Item.listCategory.forEach(item => {
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

            this.Item.listCategory = arr.concat(this.listCateNews.filter(e => e.Check == true));

            this.http.put('/api/product/' + this.Item.ProductId, this.Item, this.httpOptions).subscribe(
                (res) => {
                    if (res["meta"]["error_code"] == 200) {
                        this.GetListProduct();
                        this.ProductModal.hide();
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
                        this.Delete(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-default',

                }
            ],
        });
    }

    Delete(Id) {
        this.http.delete('/api/Product/' + Id, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.GetListProduct();
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

    // check chữ
    ChangeNameProduct(key) {
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

    GetListCateNews() {
        let arr = "arr=11&langId=" + this.Item.LanguageId;
        this.http.get('/api/category/GetByTree?'+arr, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.listCateNews = res["data"];

                    if (this.Item.ProductId != undefined) {
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




        // this.http.get('/api/category/GetByPage?page=1&query=TypeCategoryId=11', this.httpOptions).subscribe(
        // 	(res) => {
        // 		if (res["meta"]["error_code"] == 200) {
        // 			this.listCateNews = [];
        // 			if (res["data"].length > 0) {
        // 				res["data"].forEach(cate => {
        // 					this.listCateNews.push({ CategoryId: cate.CategoryId, Name: cate.Name, Check: false });
        // 				});

        // 				if (this.Item.ProductId != undefined) {
        // 					for (var i = 0; i < this.listCateNews.length; i++) {
        // 						for (var j = 0; j < this.Item.listCategory.length; j++) {
        // 							if (this.listCateNews[i].CategoryId == this.Item.listCategory[j].CategoryId) {
        // 								this.listCateNews[i].Check = true;
        // 								break;
        // 							}
        // 						}
        // 					}
        // 				}
        // 			}
        // 		}
        // 	},
        // 	(err) => {
        // 		console.log("Error: connect to API");
        // 	}
        // );
    }

    upload(files) {
        if (files.length === 0)
            return;

        const formData = new FormData();

        for (let file of files)
            formData.append(file.name, file);

        const uploadReq = new HttpRequest('POST', 'api/upload/uploadImage/2', formData, {
            headers: new HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            }),
            reportProgress: true,
        });

        this.http.request(uploadReq).subscribe(event => {
            if (event.type === HttpEventType.UploadProgress)
                this.progress = Math.round(100 * event.loaded / event.total);
            else if (event.type === HttpEventType.Response) {
                event.body["data"].forEach(item => {
                    this.ImageProduct = new ImageProduct();
                    this.ImageProduct.Image = item;
                    this.ImageProduct.IsImageMain = false;
                    this.ImageProduct.Status = 1;
                    this.Item.listImage.push(this.ImageProduct);
                });
            }
        });
    }

    RemoveImage(idx) {
        if (this.Item.listImage[idx].ProductImageId == undefined) {
            this.Item.listImage.splice(idx, 1);
        }
        else {
            this.Item.listImage[idx].Status = 99;
        }
    }

    findTrademark(item) {
        if (item == undefined) {
            return "";
        }
        else {
            return item.Name;
        }
    }

    ShowHide(id, i) {
        let stt = this.listProduct[i].IsShow ? 1 : 10;
        this.http.put('/api/Product/ShowHide/' + id + "/" + stt, undefined, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.toastSuccess("Thay đổi trạng thái thành công!");
                    this.GetListProduct();
                }
                else {
                    this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                    this.GetListProduct();
                    this.listProduct[i].IsShow = !this.listProduct[i].IsShow;
                }
            },
            (err) => {
                this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                this.GetListProduct();
                this.listProduct[i].IsShow = !this.listProduct[i].IsShow;
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

        this.GetListProduct();
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

    SetIsMain(idx) {
        for (let i = 0; i < this.Item.listImage.length; i++) {
            this.Item.listImage[i].IsImageMain = false;
            if (idx == i) {
                this.Item.listImage[i].IsImageMain = true;
            }
        }
    }


    GetListOrderBy() {
        this.http.get('/api/orderby/GetOrderBy/10', this.httpOptions).subscribe(
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

    OpenOrderByModal() {
        this.listOrderByProduct = [];
        this.GetListOrderBy();
        this.OrderByModal.show();
    }

    DeleteOrderBy(item) {
        for (let i = 0; i < this.listOrderByProduct.length; i++) {
            if (this.listOrderByProduct[i].CategoryMappingId == item.CategoryMappingId) {
                this.listOrderByProduct[i].Status = 99;
                break;
            }
        }
    }

    SaveOrderBy() {
        this.http.post('/api/orderby', this.listOrderByProduct, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.GetListProduct();
                    this.OrderByModal.hide();
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

    //Product Review
    GetListProductReviews() {
        this.http.get('/api/product/ProductReview/GetByPage?page=' + this.pagingReview.page + '&page_size=' + this.pagingReview.page_size + '&query=' + this.pagingReview.query + '&order_by=' + this.pagingReview.order_by, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.listProductReview = res["data"];
                    this.pagingReview.item_count = res["metadata"];
                }
            },
            (err) => {
                console.log("Error: connect to API");
            }
        );
    }

    ProductReviewsModal(ProductId, Name) {
        this.ProductName = Name;
        this.ProductId = ProductId;

        this.pagingReview = new Paging();
        this.pagingReview.page = 1;
        this.pagingReview.page_size = 10;
        this.pagingReview.query = "ProductId=" + ProductId;
        this.pagingReview.order_by = "";
        this.pagingReview.item_count = 0;

        this.qReview = new QueryFilter();
        this.qReview.txtSearch = "";
        this.qReview.Type = undefined;

        this.GetListProductReviews();
        this.ProductReviewModal.show();
    }

    PageChangedReview(event) {
        this.pagingReview.page = event.page;
        this.GetListProductReviews();
    }

    QueryReviewChanged() {
        let query = 'ProductId=' + this.ProductId;
        if (this.qReview["Type"] != undefined) {
            if (query != '') {
                query += ' and Status=' + this.qReview["Type"];
            }
            else {
                query += 'Status=' + this.qReview["Type"];
            }
        }

        if (query == '')
            this.pagingReview.query = '1=1';
        else
            this.pagingReview.query = query;

        this.GetListProductReviews();
    }

    ChangeStatusProductReview(ProductReviewId, Status) {
        this.http.put('/api/Product/ChangeStatusProductReview/' + ProductReviewId + "/" + Status, undefined, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.toastSuccess("Thay đổi trạng thái thành công!");
                    this.GetListProductReviews();
                }
                else {
                    this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                    this.GetListProductReviews();
                }
            },
            (err) => {
                this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
                this.GetListProductReviews();
            }
        );
    }
  // tai tep dinh kem

  uploadDoc(files) {
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
        this.ItemAt.Url = event.body["data"];
      }
    });
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

    //Lấy ra danh sách thuộc tính
    GetAttribuites() {
        this.http.get('/api/attribuite/GetByPage?page=1&query=1=1&order_by=', this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.attribuites = res["data"];
                }
            },
            (err) => {
                console.log("Error: connect to API");
            }
        );
    }

    OpenAttribuiteModal() {
        this.ItemAttribuite = new Attribute();
        this.ItemAttribuite.Status = 1;
        this.AttribuiteModal.show();
    }

    SaveAttribuite() {
        if (this.ItemAttribuite.AttribuiteId == undefined) {
            this.toastWarning("Chưa chọn Thuộc tính!");
            return;
        } else if (this.ItemAttribuite.Value == undefined || this.ItemAttribuite.Value == '') {
            this.toastWarning("Chưa nhập Giá trị thuộc tính!");
            return;
        } else if (this.ItemAttribuite.Value.replace(/ /g, '') == '') {
            this.toastWarning("Chưa nhập Giá trị thuộc tính!");
            return;
        } else if (this.ItemAttribuite.Location == undefined) {
            this.toastWarning("Chưa nhập Thứ tự hiển thị!");
            return;
        }

        if (this.Item.listAttribute == undefined) {
            this.Item.listAttribute = [];
        }

        this.Item.listAttribute.push(this.ItemAttribuite);
        this.ItemAttribuite = new Attribute();
        this.AttribuiteModal.hide();
    }

    ShowConfirmDeleteAttribuite(i) {
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
                        this.Item.listAttribute[i].Status = 99;
                        this.viewRef.clear();
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-default',

                }
            ],
        });
    }

    CheckActionTable(ProductId) {
        if (ProductId == undefined) {
            let CheckAll = this.CheckAll;
            this.listProduct.forEach(item => {
                item.Action = CheckAll;
            });
        }
        else {
            let CheckAll = true;
            for (let i = 0; i < this.listProduct.length; i++) {
                if (!this.listProduct[i].Action) {
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
                this.listProduct.forEach(item => {
                    if (item.Action == true) {
                        data.push(item.ProductId);
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
                                    this.http.put('/api/Product/deletes', data, this.httpOptions).subscribe(
                                        (res) => {
                                            if (res["meta"]["error_code"] == 200) {
                                                this.toastSuccess("Xóa thành công!");
                                                this.GetListProduct();
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
                                buttonClass: 'btn btn-default',

                            }
                        ],
                    });
                }
                break;
            default:
                break;
        }
    }
}
