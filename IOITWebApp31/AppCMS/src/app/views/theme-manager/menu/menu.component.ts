import { Component, OnInit, ViewChild, ViewContainerRef, OnDestroy } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { Menu } from '../../../data/model';
import { ToastrService } from 'ngx-toastr';
import * as $ from 'jquery';
import { Location, LocationStrategy, PathLocationStrategy } from '@angular/common';
import { Paging, QueryFilter } from '../../../data/dt';
declare var loadNestable;
import { domainImage, ActionTable } from '../../../data/const';
import { Router } from '@angular/router';

@Component({
  selector: 'app-menu',
  providers: [Location, {
    provide: LocationStrategy,
    useClass: PathLocationStrategy
  }],
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit, OnDestroy {
  @ViewChild('MenuModal') public MenuModal: ModalDirective;

  PathLocation: Location;

  public paging: Paging;
  public q: QueryFilter;
  public listMenu = [];
  public listCate = [];

  public Item: Menu;

  public httpOptions: any;

  public ActionTable = ActionTable;
  public ActionId: number;
  public CheckAll: boolean;

  public listLanguage = [];
  public isNoitify: boolean = false;
  constructor(
    location: Location,
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
    public router: Router
  ) {
    this.PathLocation = location;
    this.Item = new Menu();

    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "MenuId Desc";
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
    this.GetListMenu();
  }

  ngOnDestroy() {
    this.router.onSameUrlNavigation = 'ignore';
  }


  key: string = 'Children';
  dataOutput: Array<Object> = [{ CategoryId: 0, Name: "Menu", Children: [] }];
  dataInput1: Array<Object> = [];


  GetListMenu() {
    this.http.get('/api/menu/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listMenu = res["data"];
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
    this.GetListMenu();
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

    if (query == '')
      this.paging.query = '1=1';
    else
      this.paging.query = query;

    this.GetListMenu();
  }

  GetListCate() {
    this.http.get('/api/menu/GetCategoryMenu', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.dataInput1 = res["data"];
          this.dataOutput = [{ CategoryId: 0, Name: "Menu", Children: [] }];
          this.MenuModal.show();
        }
      },
      (err) => {
        console.log("Error: connect to API");
        this.toastError("Đã xảy ra lỗi! Xin vui lòng thử lại sau.");
      }
    );
  }

  //Mở modal thêm mới
  OpenAddModal() {
    this.Item = new Menu();
    this.GetListCate();
    loadNestable();
  }

  SaveMenuModal() {
    if (this.Item.Code == undefined || this.Item.Code == '') {
      this.toastWarning("Chưa nhập Mã Menu!");
      return;
    } else if (this.Item.Name == undefined || this.Item.Name == '') {
      this.toastWarning("Chưa nhập Tên Menu!");
      return;
    }

    this.Item.UserId = parseInt(localStorage.getItem("userId"));
    this.Item.LanguageId = parseInt(localStorage.getItem("languageId"));
    this.Item.WebsiteId = parseInt(localStorage.getItem("websiteId"));
    this.Item.CompanyId = parseInt(localStorage.getItem("companyId"));
    let obj = this.Item;
    obj["listMenuItem"] = JSON.parse($('#nestable2-output').val())[0]["children"] != undefined ? JSON.parse($('#nestable2-output').val())[0]["children"] : [];

    if (this.Item.MenuId != undefined) {
      this.http.put('/api/menu/' + obj.MenuId, obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            // this.GetListMenu();
            this.ResetCurrentRouter();
            this.MenuModal.hide();
            this.toastSuccess("Cập nhật thành công!");
            location.reload();
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
      this.http.post('/api/menu', obj, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            // this.GetListMenu();
            this.ResetCurrentRouter();
            this.MenuModal.hide();
            this.toastSuccess("Thêm mới thành công!");
            location.reload();
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


  GetCategoryMenuLeft(id) {
    this.http.get('/api/menu/GetCategoryMenuLeft/' + id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.dataInput1 = res["data"];
          console.log(this.dataInput1);
          loadNestable();
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

  GetCategoryMenuRight(id) {
    this.http.get('/api/menu/GetCategoryMenuRight/' + id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.dataOutput = res["data"];
          console.log(this.dataOutput);
          loadNestable();
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

  OpenEditModal(item) {
    this.dataInput1 = [];
    this.dataOutput = [{ CategoryId: 0, Name: "Menu", Children: [] }];
    this.GetCategoryMenuLeft(item.MenuId);
    this.GetCategoryMenuRight(item.MenuId);
    setTimeout(function() {
      loadNestable();
    }, 1500);
    this.Item = Object.assign(this.Item, item);
    this.MenuModal.show();
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
            this.DeleteMenu(Id);
          }
        },
        {
          text: 'Đóng',
          buttonClass: 'btn btn-danger',

        }
      ],
    });
  }

  DeleteMenu(Id) {
    this.http.delete('/api/menu/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          // this.GetListMenu();
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

    this.GetListMenu();
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

  CheckActionTable(MenuId) {
    if (MenuId == undefined) {
      let CheckAll = this.CheckAll;
      this.listMenu.forEach(item => {
        item.Action = CheckAll;
      });
    }
    else {
      let CheckAll = true;
      for (let i = 0; i < this.listMenu.length; i++) {
        if (!this.listMenu[i].Action) {
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
        this.listMenu.forEach(item => {
          if (item.Action == true) {
            data.push(item.MenuId);
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
                  this.http.put('/api/menu/deletes', data, this.httpOptions).subscribe(
                    (res) => {
                      if (res["meta"]["error_code"] == 200) {
                        this.toastSuccess("Xóa thành công!");
                        this.GetListMenu();
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

  ResetCurrentRouter() {
    this.router.routeReuseStrategy.shouldReuseRoute = function() {
      return false;
    };
    this.router.onSameUrlNavigation = 'reload';
    this.router.navigateByUrl(this.router.url);
  }
  closeNoityfy() {
    this.isNoitify = true;
  }
}
