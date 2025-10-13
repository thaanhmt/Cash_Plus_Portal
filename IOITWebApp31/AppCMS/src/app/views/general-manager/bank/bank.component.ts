import { Component, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { typeCategoryPage } from '../../../data/const';
import { Bank } from '../../../data/model';
import { ToastrService } from 'ngx-toastr';



@Component({
  selector: 'app-bank',
  templateUrl: './bank.component.html',
  styleUrls: ['./bank.component.scss']
})
export class BankComponent implements OnInit {
  @ViewChild('AddModal') public addModal: ModalDirective;
  @ViewChild('EditModal') public editModal: ModalDirective;

  public paging: any;
  public q: any;

  public listBank = [];
  public listCompany = [];
  public showSort: boolean[] = [true, false];

  public ckeConfig: any;

  public newItem: Bank;
  public editItem: Bank;

  public httpOptions: any;
  

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
  ) {
    this.newItem = new Bank();
    this.editItem = new Bank();
    this.paging = {
      page: 1,
      page_size: 10,
      query: '1=1',
      order_by: '',
      item_count: 0
    };

    this.q = {
      txtSearch: ''
    }

    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }
  }

  ngOnInit() {
    this.ckeConfig = {
      allowedContent: false,
      extraPlugins: 'divarea',
      forcePasteAsPlainText: true
    };
    this.GetListBank();
  }

  Sort(IsK, s, i) {
    if (IsK) {
      this.paging.order_by = s + " asc";
    }
    else {
      this.paging.order_by = s + " desc";
    }
    this.GetListBank();
    //this.showSort = !IsK;
    for (var j = 0; j < this.showSort.length; j++) {
      if (j == i) {
        this.showSort[j] = IsK;
      }
      else {
        this.showSort[j] = false;
      }
    }
  }

  //GET
  GetListBank() {
    this.http.get('/api/bank/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listBank = res["data"];
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
    this.GetListBank();
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

    if (query == '')
      this.paging.query = '1=1';
    else
      this.paging.query = query;

    this.GetListBank();
  }

  //Mở modal
  OpenAddModal() {
    this.newItem = new Bank();
    //this.GetListCompany(undefined);
    this.addModal.show();
  }
  
  //Thêm mới
  AddFunc() {
    if (this.newItem.Name == undefined || this.newItem.Name == '') {
      this.toastWarning("Chưa nhập Tên!");
      return;
    } else if (this.newItem.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên!");
      return;
    } else if (this.newItem.AccountId == undefined || this.newItem.AccountId == '') {
      this.toastWarning("Chưa nhập tài khoản!");
      return;
    } else if (this.newItem.AccountId.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tài khoản!");
      return;
    } else if (this.newItem.AccountName == undefined || this.newItem.AccountName == '') {
      this.toastWarning("Chưa nhập tên tài khoản");
      return;
    } else if (this.newItem.AccountName.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên tài khoản!");
      return;
    } else if (this.newItem.BranchName == undefined || this.newItem.BranchName == '') {
      this.toastWarning("Chưa nhập tên chi nhánh!");
      return;
    } else if (this.newItem.BranchName.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập chi nhánh!");
      return;
    }
    this.newItem.UserId = parseInt(localStorage.getItem("userId"));
    this.newItem.CompanyId = parseInt(localStorage.getItem("companyId"));
    this.http.post('/api/Bank', this.newItem, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListBank();
          this.addModal.hide();
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

  //Mở edit Modal
  OpenEditModal(item) {
    this.editItem = new Bank();
    this.editItem = Object.assign(this.editItem, item);
    this.editModal.show();
  }

  // cập nhật
  EditFunc() {
    if (this.editItem.Name == undefined || this.editItem.Name == '') {
      this.toastWarning("Chưa nhập Tên!");
      return;
    } else if (this.editItem.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên!");
      return;
    } else if (this.editItem.AccountId == undefined || this.editItem.AccountId == '') {
      this.toastWarning("Chưa nhập tài khoản!");
      return;
    } else if (this.editItem.AccountId.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tài khoản!");
      return;
    } else if (this.editItem.AccountName == undefined || this.editItem.AccountName == '') {
      this.toastWarning("Chưa nhập tên tài khoản");
      return;
    } else if (this.editItem.AccountName.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên tài khoản!");
      return;
    } else if (this.editItem.BranchName == undefined || this.editItem.BranchName == '') {
      this.toastWarning("Chưa nhập tên chi nhánh!");
      return;
    } else if (this.editItem.BranchName.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên chi nhánh!");
      return;
    }
    this.editItem.UserId = parseInt(localStorage.getItem("userId"));
    this.editItem.CompanyId = parseInt(localStorage.getItem("companyId"));
    this.http.put('/api/Bank/' + this.editItem.BankId, this.editItem, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListBank();
          this.editModal.hide();
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
          buttonClass: 'btn btn-default',

        }
      ],
    });
  }

  Delete(Id) {
    this.http.delete('/api/Bank/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListBank();
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
  //
}
