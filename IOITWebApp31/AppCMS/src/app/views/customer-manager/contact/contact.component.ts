import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { ToastrService } from 'ngx-toastr';
import { Contact } from '../../../data/model';
import { Paging, QueryFilter } from '../../../data/dt';
import { TypeContact, ActionTable, domain, domainDebug } from '../../../data/const';

@Component({
    selector: 'app-contact',
    templateUrl: './contact.component.html',
    styleUrls: ['./contact.component.scss']
})
export class ContactComponent implements OnInit {
    @ViewChild('ContactModal') public ContactModal: ModalDirective;
    public paging: Paging;
    public q: QueryFilter;
    public listContact = [];
    public httpOptions: any;
    public TypeContact = TypeContact;
    public domain: string;
    public Item: Contact;

    public ActionTable = ActionTable;
    public ActionId: number;
    public CheckAll: boolean;
    public staticDomain: string;
    public staticDomainMedia: string;
    public domainDebug = domainDebug;
    public isNoitify: boolean = false;
    constructor(
        public http: HttpClient,
        public modalDialogService: ModalDialogService,
        public viewRef: ViewContainerRef,
        public toastr: ToastrService,
    ) {
        this.paging = new Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "CreatedAt Desc";
        this.paging.item_count = 0;

        this.q = new QueryFilter();
        this.q.txtSearch = "";

        this.Item = new Contact();

        this.httpOptions = {
            headers: new HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        }
    }

  ngOnInit() {
        this.domain = domain;
        this.GetListContact();
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
    //GET
    GetListContact() {
        this.http.get('/api/contact/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.listContact = res["data"];
                    this.paging.item_count = res["metadata"];
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
        this.GetListContact();
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
        let query = '';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and (FullName.Contains("' + this.q.txtSearch + '") Or Email.Contains("' + this.q.txtSearch + '"))';
            }
            else {
                query += '(FullName.Contains("' + this.q.txtSearch + '") or Email.Contains("' + this.q.txtSearch + '"))';
            }
        }

      if (this.q["TypeContactId"] != undefined) {
          
            if (query != '') {
              query += ' and TypeContact=' + this.q["TypeContactId"];
            }
            else {
              query += 'TypeContact=' + this.q["TypeContactId"];
            }
        }

        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;

        this.GetListContact();
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

        this.GetListContact();
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

    //Mở modal thêm mới
    OpenContactModal(item) {
        this.Item = new Contact();
        //this.file.nativeElement.value = "";
        //this.message = undefined;
        //this.progress = undefined;
        if (item != undefined) {
            this.Item = JSON.parse(JSON.stringify(item));
        }
        console.log(this.Item);
        this.ContactModal.show();
    }

    //Thêm mới danh mục trang
    SaveContact() {
        //if (this.Item.Code == undefined || this.Item.Code == '') {
        //    this.toastWarning("Chưa nhập Mã ngôn ngữ!");
        //    return;
        //} else if (this.Item.Code.replace(/ /g, '') == '') {
        //    this.toastWarning("Chưa nhập mã!");
        //    return;
        //} else if (this.Item.Name == undefined || this.Item.Name == '') {
        //    this.toastWarning("Chưa nhập Tên ngôn ngữ!");
        //    return;
        //} else if (this.Item.Name.replace(/ /g, '') == '') {
        //    this.toastWarning("Chưa nhập tên!");
        //    return;
        //}

        if (this.Item.ContactId == undefined) {
            this.http.post('/api/contact', this.Item, this.httpOptions).subscribe(
                (res) => {
                    if (res["meta"]["error_code"] == 200) {
                        this.GetListContact();
                        this.ContactModal.hide();
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
            this.http.put('/api/contact/' + this.Item.ContactId, this.Item, this.httpOptions).subscribe(
                (res) => {
                    if (res["meta"]["error_code"] == 200) {
                        this.GetListContact();
                        this.ContactModal.hide();
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
                        this.http.delete('/api/contact/' + Id, this.httpOptions).subscribe(
                            (res) => {
                                if (res["meta"]["error_code"] == 200) {
                                    this.GetListContact();
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
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-danger',

                }
            ],
        });
    }

    CheckActionTable(ContactId) {
        if (ContactId == undefined) {
            let CheckAll = this.CheckAll;
            this.listContact.forEach(item => {
                item.Action = CheckAll;
            });
        }
        else {
            let CheckAll = true;
            for (let i = 0; i < this.listContact.length; i++) {
                if (!this.listContact[i].Action) {
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
                this.listContact.forEach(item => {
                    if (item.Action == true) {
                        data.push(item.ContactId);
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
                                    this.http.put('/api/contact/deletes', data, this.httpOptions).subscribe(
                                        (res) => {
                                            if (res["meta"]["error_code"] == 200) {
                                                this.toastSuccess("Xóa thành công!");
                                                this.GetListContact();
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
  closeNoityfy() {
    this.isNoitify = true;
  }
}
