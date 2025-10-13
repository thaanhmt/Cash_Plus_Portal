import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { ConfigThumb } from '../../../data/model';
import { ToastrService } from 'ngx-toastr';
import { TypeUpload, ActionTable } from '../../../data/const';
import { Paging, QueryFilter } from '../../../data/dt';

@Component({
    selector: 'app-config-thumb',
    templateUrl: './config-thumb.component.html',
    styleUrls: ['./config-thumb.component.scss']
})
export class ConfigThumbComponent implements OnInit {
    @ViewChild('ConfigThumbModal') public ConfigThumbModal: ModalDirective;

    public paging: Paging;
    public q: QueryFilter;

    public listConfigThumb = [];

    public ckeConfig: any;

    public typeUpload = TypeUpload;

    public Item: ConfigThumb;

    public httpOptions: any;
    public isNoitify: boolean = false;
    public ActionTable = ActionTable;
    public ActionId: number;
    public CheckAll: boolean;

    constructor(
        public http: HttpClient,
        public modalDialogService: ModalDialogService,
        public viewRef: ViewContainerRef,
        public toastr: ToastrService,
    ) {
        this.Item = new ConfigThumb();
        this.paging = new Paging();
        this.paging.page = 1;
        this.paging.page_size = 10;
        this.paging.query = "1=1";
        this.paging.order_by = "ConfigThumbId Desc";
        this.paging.item_count = 0;

        this.q = new QueryFilter();
        this.q.txtSearch = "";

        this.httpOptions = {
            headers: new HttpHeaders({
                'Authorization': 'bearer ' + localStorage.getItem("access_token")
            })
        }
    }

    ngOnInit() {
        this.GetListConfigThumb();
    }

    //GET danh sách ảnh thumb
    GetListConfigThumb() {
        this.http.get('/api/ConfigThumb/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.listConfigThumb = res["data"];
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
        this.GetListConfigThumb();
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


    QueryChanged() {
        let query = '';
        if (this.q.txtSearch != undefined && this.q.txtSearch != '') {
            if (query != '') {
                query += ' and Name.Contains("' + this.q.txtSearch + '")';
            }
            else {
                query += 'Name.Contains("' + this.q.txtSearch + '")';
            }
        }

        if (this.q["Type"] != undefined) {
            if (query != '') {
                query += ' and Type=' + this.q["Type"];
            }
            else {
                query += 'Type=' + this.q["Type"];
            }
        }

        if (query == '')
            this.paging.query = '1=1';
        else
            this.paging.query = query;

        this.GetListConfigThumb();
    }

    //Mở modal
    OpenConfigThumbModal(item) {
        this.Item = new ConfigThumb();
        if (item != undefined) {
            this.Item = JSON.parse(JSON.stringify(item));
        }
        this.ConfigThumbModal.show();
    }

    //Thêm mới
    SaveConfigThumb() {
        if (this.Item.Name == undefined || this.Item.Name == '') {
            this.toastWarning("Chưa nhập Tên hiển thị!");
            return;
        } else if (this.Item.Width == undefined) {
            this.toastWarning("Chưa nhập chiều rộng!");
            return;
        } else if (this.Item.Height == undefined) {
            this.toastWarning("Chưa nhập chiều cao!");
            return;
        } else if (this.Item.Type == undefined) {
            this.toastWarning("Chưa nhập loại thumb");
            return;
        }

        this.Item.UserId = parseInt(localStorage.getItem("userId"));
        this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
        this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));

        if (this.Item.ConfigThumbId == undefined) {
            this.http.post('/api/ConfigThumb', this.Item, this.httpOptions).subscribe(
                (res) => {
                    if (res["meta"]["error_code"] == 200) {
                        this.GetListConfigThumb();
                        this.ConfigThumbModal.hide();
                        this.toastSuccess("Thêm thành công!");
                    }
                    else {
                        this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
                    }
                },
                (err) => {
                    this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
                }
            );
        }
        else {
            this.http.put('/api/ConfigThumb/' + this.Item.ConfigThumbId, this.Item, this.httpOptions).subscribe(
                (res) => {
                    if (res["meta"]["error_code"] == 200) {
                        this.GetListConfigThumb();
                        this.ConfigThumbModal.hide();
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
                        console.log('OnAction');
                        this.Delete(Id);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-danger',

                }
            ],
        });
    }

    Delete(Id) {
        this.http.delete('/api/ConfigThumb/' + Id, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.GetListConfigThumb();
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

    //auto gen thumbs
    ShowConfirmGenThumb(type, width) {
        this.modalDialogService.openDialog(this.viewRef, {
            title: 'Xác nhận',
            childComponent: SimpleModalComponent,
            data: {
                text: "Bạn có chắc chắn muốn sinh thumb có kích thước " + width + "?"
            },
            actionButtons: [
                {
                    text: 'Đồng ý',
                    buttonClass: 'btn btn-success',
                    onAction: () => {
                        console.log('OnAction');
                        this.AutoGenThumbs(type, width);
                    }
                },
                {
                    text: 'Đóng',
                    buttonClass: 'btn btn-danger',

                }
            ],
        });
    }

    AutoGenThumbs(type, width) {
        this.http.post('/api/upload/autoGenThumbs/' + type + '/' + width, null, this.httpOptions).subscribe(
            (res) => {
                if (res["meta"]["error_code"] == 200) {
                    this.viewRef.clear();
                    this.toastSuccess("Sinh thumb thành công!");
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

        this.GetListConfigThumb();
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

    FindTypeThumb(id) {
        for (var i = 0; i < this.typeUpload.length; i++) {
            if (this.typeUpload[i].Id == id)
                return this.typeUpload[i].Name;
        }
    }

    CheckActionTable(ConfigThumbId) {
        if (ConfigThumbId == undefined) {
            let CheckAll = this.CheckAll;
            this.listConfigThumb.forEach(item => {
                item.Action = CheckAll;
            });
        }
        else {
            let CheckAll = true;
            for (let i = 0; i < this.listConfigThumb.length; i++) {
                if (!this.listConfigThumb[i].Action) {
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
                this.listConfigThumb.forEach(item => {
                    if (item.Action == true) {
                        data.push(item.ConfigThumbId);
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
                                    this.http.put('/api/ConfigThumb/deletes', data, this.httpOptions).subscribe(
                                        (res) => {
                                            if (res["meta"]["error_code"] == 200) {
                                                this.toastSuccess("Xóa thành công!");
                                                this.GetListConfigThumb();
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
