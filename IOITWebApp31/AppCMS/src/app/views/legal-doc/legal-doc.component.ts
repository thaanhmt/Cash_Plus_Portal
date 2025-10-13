import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { domainImage, ActionTable, domainImageFile, domainFile, domainMedia, domain, domainDebug } from '../../data/const';
import { LegalDoc } from '../../data/model';
import { ToastrService } from 'ngx-toastr';
import { DatePipe } from '@angular/common';
import { Md5 } from 'ts-md5/dist/md5';
import { CheckRole, Paging, QueryFilter } from '../../data/dt';
import { DateTimeAdapter, OWL_DATE_TIME_FORMATS, OWL_DATE_TIME_LOCALE } from 'ng-pick-datetime';
import { MomentDateTimeAdapter } from 'ng-pick-datetime-moment';
import { CommonService } from '../../service/common.service';
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
  selector: 'app-legal-doc',
  templateUrl: './legal-doc.component.html',
  styleUrls: ['./legal-doc.component.scss'],
  providers: [
    { provide: DateTimeAdapter, useClass: MomentDateTimeAdapter, deps: [OWL_DATE_TIME_LOCALE] },
    { provide: OWL_DATE_TIME_FORMATS, useValue: MY_CUSTOM_FORMATS }
  ]
})
export class LegalDocComponent implements OnInit {

  @ViewChild('LegalDocModal') public LegalDocModal: ModalDirective;
  @ViewChild('file') file: ElementRef;
  @ViewChild('OpenMediaFile') public OpenMediaFile: ModalDirective;
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
  public listLegalDoc = [];
  public listNewsT = [];
  public listLanguage = [];
  public listLanguageTemp = [];
  public ckeConfig: any;
  public Item: LegalDoc;
  public ItemTranslate: LegalDoc;
  public domainImage = domainImage;
  public domainFile = domainFile;
  public httpOptions: any;
  public languageId: number;
  public ActionTable = ActionTable;
  public ActionId: number;
  public CheckAll: boolean;
  public listId = [];

  public page_pp = [];
  public Checkitem: boolean;
  public CheckitemAll: boolean;

  public listCateNews = [];
  public listTypeAttributeItem = [];
  public isNoitify: boolean = false;

  public CheckRole: CheckRole;

  public staticDomain: string;
  public staticDomainMedia: string;
  public domainDebug = domainDebug;
  public typeAction : number;

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public common: CommonService
  ) {
    this.Item = new LegalDoc();
    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "LegalDocId Desc";
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
    //this.paging.query = "LanguageId=" + this.languageId;
    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }

    this.CheckRole = new CheckRole();
    let code = "DSCH";
    this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 0);
    this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 1);
    this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 2);
    this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), code, 3);

  }

  ngOnInit() {
    this.ckeConfig = {
      allowedContent: false,
      extraPlugins: 'divarea',
      forcePasteAsPlainText: true
    };
    this.GetListLegalDoc();
    this.GetListLanguage();
    this.GetListCateNews();
    this.GetListTypeAttributeItem();
    this.GetListFiles();
    this.GetDomainStatic();
  }
  GetDomainStatic() {
    this.http.get('api/Config/1', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
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
  //Get danh sách typeAttributeItem phong ban
  GetListTypeAttributeItem() {
    this.http.get('/api/TypeAttributeItem/GetByPage?page=1&query=TypeAttributeId=4&order_by=TypeAttributeItemId Asc', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listTypeAttributeItem = res["data"];

        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  GetListCateNews() {
    this.Item.LanguageId = this.Item.LanguageId ? this.Item.LanguageId : 1;
      this.http.get('/api/category/GetByTree?arr=12&langId=' + this.Item.LanguageId, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listCateNews = res["data"];

          if (this.Item.LegalDocId != undefined) {
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

  GetListLegalDoc() {
    this.http.get('/api/LegalDoc/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listLegalDoc = res["data"];
          this.paging.item_count = res["metadata"];
          for (let i = 0; i < this.listLegalDoc.length; i++) {
            this.listLegalDoc[i].IsCheck = false;
          }
          for (let i = 0; i < this.paging.item_count; i++) {
            this.page_pp.push(i);
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
        this.ItemTranslate = new LegalDoc();
        this.http.get('/api/translate/' + id + '/' + sl + '/' + tl + '/4', this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.ItemTranslate = res["data"];
                    this.Item.LegalDocId = undefined;
                    this.Item.Name = this.ItemTranslate.Name;
                    //this.Item.Url = this.ItemTranslate.Url;
                    //this.Item.Description = this.ItemTranslate.Description;
                    this.Item.Contents = this.ItemTranslate.Contents;
                    this.Item.Note = this.ItemTranslate.Note;
                    this.Item.TichYeu = this.ItemTranslate.TichYeu;
                    //this.Item.MetaKeyword = this.ItemTranslate.MetaKeyword;
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
    this.GetListLegalDoc();
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
  //Search
  QueryChanged() {
      let query = '1=1';

      if (this.q.LanguageId != undefined) {
          query = 'LanguageId=' + this.q.LanguageId;
      }
    if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
      if (query != '') {
        query += ' and Name.Contains("' + this.q.txtSearch + '")';
      }
      else {
        query += 'Name.Contains("' + this.q.txtSearch + '")';
      }
    }

    if (query == '')
      this.paging.query = '1=1';
    else
      this.paging.query = query;

    this.GetListLegalDoc();
  }

  //Mở modal thêm mới
    OpenLegalDocModal(item, type) {
      this.typeAction = type;
        this.Item = new LegalDoc();
        this.listLanguageTemp = this.listLanguage;
        this.Item.LanguageId = this.languageId;
      //if (this.file) this.file.nativeElement.value = "";
      //this.message = undefined;
      //this.progress = undefined;
      this.Item.listCategory = [];
        if (item != undefined) {
            this.Item = JSON.parse(JSON.stringify(item));
            if (type == 1 || type == 3) {
                
                this.Item.AgencyIssued = +this.Item.AgencyIssued;

                //this.CheckBoxStatus = this.Item.Status == 1 ? true : false;
            }
            else if (type == 2) {
                if (this.listLanguage.length == item.listLanguage.length + 1) {
                    this.toastWarning("Bạn đã thêm đủ ngôn ngữ!");
                    return;
                }
                this.listLanguageTemp = [];
                this.Item.LegalDocId = undefined;
                this.Item.LegalDocRootId = item.LegalDocId;
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
                //this.GetTranslate(this.Item.LegalDocRootId);
            }
        }
      //let listName = this.Item.Attactment.split('/');
      //this.Item.AttactmentName = listName[listName.length - 1];
        this.GetListCateNews();

        this.LegalDocModal.show();
    }

  //Thêm mới khách hàng
  SaveLegalDoc() {
    //if (this.Item.Code == undefined || this.Item.Code == '') {
    //  this.toastWarning("Chưa nhập mã!");
    //  return;
    //} else if (this.Item.Code.replace(/ /g, '') == '') {
    //  this.toastWarning("Chưa nhập mã!");
    //  return;
    //} else
      if (this.Item.Name == undefined || this.Item.Name == '') {
      this.toastWarning("Chưa nhập tên câu hỏi!");
      return;
    } else if (this.Item.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên câu hỏi!");
      return;
      }
    //  else if (this.Item.AgencyIssued == undefined) {
    //  this.toastWarning("Chưa nhập cơ quan ban hành !");
    //  return;
    //} else if (this.Item.DateIssue == undefined || this.Item.DateIssue == '') {
    //  this.toastWarning("Chưa nhập ngày ban hành!");
    //  return;
    //} else if (this.Item.DateEffect == undefined || this.Item.DateEffect == '') {
    //  this.toastWarning("Chưa nhập ngày hợp lực!");
    //  return;
    //} else if (this.Item.Signer == undefined || this.Item.Signer == '') {
    //  this.toastWarning("Chưa nhập người ký");
    //  return;
    //} else if (this.Item.Attactment == undefined || this.Item.Attactment == '') {
    //  this.toastWarning("Chưa nhập file đính kèm");
    //  return;
    //  } 
      else if (this.Item.Contents == undefined || this.Item.Contents == '') {
      this.toastWarning("Chưa nhập nội dung");
      return;
    }
      //else if (this.Item.Note == undefined || this.Item.Note == '') {
    //  this.toastWarning("Chưa nhập ghi chú");
    //  return;
    //}
    //  else if (this.Item.TichYeu == undefined || this.Item.TichYeu == '') {
    //  this.toastWarning("Chưa nhập trích yếu");
    //  return;
    //} else if (this.Item.YearIssue == undefined || this.Item.YearIssue == null) {
    //  this.toastWarning("Chưa nhập năm ban hành");
    //  return;
    //}

    //if (typeof this.Item.DateIssue === 'object' && this.Item.DateIssue != undefined) {
    //  let DateIssue = this.Item.DateIssue.add(7, 'hours');
    //  this.Item.DateIssue = DateIssue.toISOString();
    //}

    //if (typeof this.Item.DateEffect === 'object' && this.Item.DateEffect != undefined) {
    //  let DateEffect = this.Item.DateEffect.add(7, 'hours');
    //  this.Item.DateEffect = DateEffect.toISOString();
    //}

    this.Item.UserId = parseInt(localStorage.getItem("userId"));
    let obj = JSON.parse(JSON.stringify(this.Item));

    if (this.Item.LegalDocId == undefined) {
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
      this.http.post('/api/LegalDoc', obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListLegalDoc();
            this.LegalDocModal.hide();
            this.toastSuccess("Thêm mới thành công!");
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
      this.http.put('/api/LegalDoc/' + this.Item.LegalDocId, obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListLegalDoc();
            this.LegalDocModal.hide();
            this.toastSuccess(res["meta"]["error_message"]);
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
        text: "Bạn có chắc chắn muốn xóa bản ghi này?"
      },
      actionButtons: [
        {
          text: 'Đồng ý',
          buttonClass: 'btn btn-success',
          onAction: () => {
            this.DeleteLegalDoc(Id);
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-danger',

        }
      ],
    });
  }

  DeleteLegalDoc(Id) {
    this.http.delete('/api/LegalDoc/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListLegalDoc();
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

  //upload(files) {
  //  if (files.length === 0)
  //    return;

  //  const formData = new FormData();

  //  for (let file of files)
  //    formData.append(file.name, file);
  //  console.log(formData);
  //  const uploadReq = new HttpRequest('POST', 'api/upload/uploadImage/6', formData, {
  //    headers: new HttpHeaders({
  //      'Authorization': 'bearer ' + localStorage.getItem("access_token")
  //    }),
  //    reportProgress: true,
  //  });

  //  this.http.request(uploadReq).subscribe(event => {
  //    if (event.type === HttpEventType.UploadProgress)
  //      this.progress = Math.round(100 * event.loaded / event.total);
  //    else if (event.type === HttpEventType.Response) {
  //      this.Item.Attactment = event.body["data"].toString();
  //    }
  //  });
  //}
  //
  RemoveImage() {
    this.Item.Attactment = undefined;
    this.file.nativeElement.value = "";
    this.progress = undefined;
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

    this.GetListLegalDoc();
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

  CheckActionTable(LegalDocId) {
    if (LegalDocId == undefined) {
      let CheckAll = this.CheckAll;
      this.listLegalDoc.forEach(item => {
        item.Action = CheckAll;
      });
    }
    else {
      let CheckAll = true;
      for (let i = 0; i < this.listLegalDoc.length; i++) {
        if (!this.listLegalDoc[i].Action) {
          CheckAll = false;
          break;
        }
      }

      this.CheckAll = CheckAll == true ? true : false;
    }
  }



  upload(files) {
    if (files.length === 0)
      return;
    console.log(files[0]);
    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);

    const uploadReq = new HttpRequest('POST', 'api/upload/uploadFileVbpq', formData, {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
      reportProgress: true,
    });

    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress)
        this.progress = Math.round(100 * event.loaded / event.total);
      else if (event.type === HttpEventType.Response) {
        console.log(event.body);
        this.Item.Attactment = event.body["data"][0].LinkFile;
        this.Item.AttactmentBit = event.body["data"][0].DataByte;
        this.Item.Extension = event.body["data"][0].Extension;
      }
    });
  }

  // Chon hoac bo chon tat ca van ban
  CheckAllAddress() {
    this.listId = [];
    console.log(this.CheckitemAll);
    if (this.CheckitemAll == true) {
      for (let i = 0; i < this.listLegalDoc.length; i++) {
        this.listLegalDoc[i].IsCheck = false;
      }
    } else {
      for (let i = 0; i < this.listLegalDoc.length; i++) {
        this.listLegalDoc[i].IsCheck = true;
      }
    }

    for (let i = 0; i < this.listLegalDoc.length; i++) {
      if (this.listLegalDoc[i].IsCheck == true) {
        this.listId.push(this.listLegalDoc[i].LegalDocId);
      }

    }

    console.log(this.listLegalDoc);
    console.log(this.listId);

  }

  //Chon binh luan xoa
  CheckDeleteAddress(item) {
    console.log(item.IsCheck);
    this.listId = [];
    if (item.IsCheck == false || item.IsCheck == undefined) {
      for (let i = 0; i < this.listLegalDoc.length; i++) {
        if (this.listLegalDoc[i].LegalDocId == item.LegalDocId) {
          this.listLegalDoc[i].IsCheck = false;
        }

      }
    } else {
      for (let i = 0; i < this.listLegalDoc.length; i++) {
        if (this.listLegalDoc[i].LegalDocId == item.LegalDocId) {
          this.listLegalDoc[i].IsCheck = true;
        }

      }
    }

    for (let i = 0; i < this.listLegalDoc.length; i++) {
      if (this.listLegalDoc[i].IsCheck == true) {
        this.listId.push(this.listLegalDoc[i].LegalDocId);
      }

    }
    if (this.listId.length < this.listLegalDoc.length) {
      this.CheckitemAll = false;
    }
    if (this.listId.length == this.listLegalDoc.length) {
      this.CheckitemAll = true;
    }
    console.log(this.listId);
  }

  //Xoa nhieu
  DeleteMuntiAddress() {
    console.log(this.listId.length);
    if (this.listId.length > 0) {
      this.http.put('/api/LegalDoc/deletes', this.listId, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListLegalDoc();

            this.viewRef.clear();
            this.toastSuccess("Xóa thành công!");
            this.listId = [];
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
      this.toastError("Chưa có văn bản nào được chọn !");
    }
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
  closeNoityfy() {
    this.isNoitify = true;
  }
  ChangeTitle(key) {
    if (this.Item.LegalDocId == undefined) {
      switch (key) {
        case 1:
          this.Item.Url = this.common.ConvertUrl(this.Item.Name);
          break;
        case 2:
          break;
        default:
          break;
      }
    }
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
  RemoveFile() {
    this.Item.Attactment = undefined;
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
    //if (item.extension)
    var obj = {
      "LinkFile": item.url + "/" + item.name,
      "Extension": item.extension,
    }
    this.http.post('/api/upload/convertFileToByte', obj, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.Item.Attactment = item.url + "/" + item.name;
          this.Item.AttactmentName = item.name;
          this.Item.AttactmentBit = res["data"]["DataByte"];
          this.Item.Extension = item.extension;
          console.log(this.Item);
          this.OpenMediaFile.hide();
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
