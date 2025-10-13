import { Component, ElementRef, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpEventType, HttpHeaders, HttpRequest } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { ToastrService } from 'ngx-toastr';
import { TypeAttribute } from '../../../data/model';
import { TypeAttributeItem } from '../../../data/model';
import { Paging, QueryFilter } from '../../../data/dt';
import { ActionTable, domainImage } from '../../../data/const';




@Component({
  selector: 'app-type-attribute',
  templateUrl: './type-attribute.component.html',
  styleUrls: ['./type-attribute.component.scss']
})
export class TypeAttributeComponent implements OnInit {
  @ViewChild('TypeAttributeModal') public TypeAttributeModal: ModalDirective;
  @ViewChild('TypeModal') public TypeModal: ModalDirective;
  @ViewChild('fileHeader') fileHeader: ElementRef;
  @ViewChild('fileFooter') fileFooter: ElementRef;
  @ViewChild('fileBanner') fileBanner: ElementRef;

  public paging: Paging;
  public q: QueryFilter;
  public progress: any;
  public file: any;
  public listTypeAttribute = [];
  public listTypeAttributeItem = [];
  public ckeConfig: any;
  public Item: TypeAttribute;
  public newType: TypeAttributeItem;
  public newTypeCS: any;
  public httpOptions: any;
  public order_by : any
  public ActionTable = ActionTable;
  public ActionId: number;
  public CheckAll: boolean;

  public progressHeader: number;
  public messageHeader: string;

  public progressFooter: number;
  public messageFooter: string;

  public progressBanner: number;
  public messageBanner: string;
  public domainImage = domainImage;
  public isNoitify: boolean = false;
  public listUser: any;
  public Users: any;
  public listSchool = [
    { id: '1', value: 'TRƯỜNG CAO ĐẲNG DU LỊCH VÀ THƯƠNG MẠI HÀ NỘI' },
    { id: '2', value: 'TRƯỜNG CAO ĐẲNG CÔNG THƯƠNG VIỆT NAM' },
    { id: '3', value: 'TRƯỜNG KHÁC' }
  ];

  constructor(
    public http: HttpClient,
    public modalDialogService: ModalDialogService,
    public viewRef: ViewContainerRef,
    public toastr: ToastrService,
  ) {
    this.Item = new TypeAttribute();
    this.newType = new TypeAttributeItem();

    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 10;
    this.paging.query = "1=1";
    this.paging.order_by = "TypeAttributeId Desc";
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
    this.ckeConfig = {
      allowedContent: false,
      extraPlugins: 'divarea',
      forcePasteAsPlainText: true
    };

    this.GetListTypeAttribute();
    this.GetListUser();
  }

  //Get danh sách loại hình
  GetListTypeAttribute() {
    this.http.get('/api/TypeAttribute/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listTypeAttribute = res["data"];
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
    this.GetListTypeAttribute();
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

    this.GetListTypeAttribute();
  }

  //Mở modal thêm mới loại hình
  OpenTypeAttributeModal(item) {
    this.Item = new TypeAttribute();
    this.newType = new TypeAttributeItem();
    //this.fileHeader.nativeElement.value = "";
    this.Item.listAttributeItem = [];
    if (item != undefined) {
      this.Item = JSON.parse(JSON.stringify(item));
    }
    this.TypeAttributeModal.show();
  }

  //Thêm mới loại hình
  SaveTypeAttribute() {
    if (this.Item.Name == undefined || this.Item.Name == '') {
      this.toastWarning("Chưa nhập tên nhóm!");
      return;
    } else if (this.Item.Name.replace(/ /g, '') == '') {
      this.toastWarning("Chưa nhập tên nhóm!");
      return;
    }
    this.Item.UserId = parseInt(localStorage.getItem("userId"));
    this.Item.IsDelete = true;
    this.Item.IsUpdate = true;
    if (this.Item.TypeAttributeId == undefined) {
      this.http.post('/api/TypeAttribute', this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListTypeAttribute();
            this.TypeAttributeModal.hide();
            this.toastSuccess("Thêm mới thành công!");
            window.location.reload();
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
      this.http.put('/api/TypeAttribute/' + this.Item.TypeAttributeId, this.Item, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.GetListTypeAttribute();
            this.TypeAttributeModal.hide();
            this.toastSuccess("Cập nhật thành công!");
            window.location.reload();
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

  // // Mở modal cập nhật TypeAttribute
  // OpenEditModal(item) {
  //   this.editItem = new TypeAttribute();
  //   this.newType = new TypeAttributeItem();
  //   this.editItem = Object.assign(this.editItem, item);
  //   this.showItem = false;
  //   this.editModal.show();
  // }

  // // cập nhật typeAttribute
  // EditFunc() {
  //   if (this.editItem.Name == undefined || this.editItem.Name == '') {
  //     this.toastWarning("Chưa nhập tên!");
  //     return;
  //   }
  //   this.editItem.UserId = parseInt(localStorage.getItem("userId"));


  //   this.http.put('/api/TypeAttribute/' + this.editItem.TypeAttributeId, this.editItem, this.httpOptions).subscribe(
  //     (res) => {
  //       if (res["meta"]["error_code"] == 200) {
  //         this.GetListTypeAttribute();
  //         this.editModal.hide();
  //         this.toastSuccess("Cập nhật thành công!");
  //         console.log(this.editItem.listAttributeItem);
  //       }
  //       else {
  //         this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
  //         console.log("error");
  //       }
  //     },
  //     (err) => {
  //       this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
  //     }
  //   );
  // }


  //Mở modal thêm TypeAttributeItem
  OpenTypeModalModal(i) {
    this.newType = new TypeAttributeItem();
    if (i != undefined) {
      this.newType = JSON.parse(JSON.stringify(this.Item.listAttributeItem[i]));
    }
    this.TypeModal.show();
    
    this.Users = this.listUser.filter((i: any) => i.SchoolCode == this.Item.Location && i.TypeAttributeId == 0);
  }

  //Thêm TypeAttributeItem
  SaveTypeItem() {
    // if (this.newType.Name == undefined || this.newType.Name == '') {
    //   this.toastWarning("Chưa nhập tên thuộc tính!");
    //   return;
    // }


    // if (this.newType.TypeAttributeItemId == undefined) {
    //   this.newType.Status = 1;
    //   this.Item.listAttributeItem.push(this.newType);
    // }
    // else {
    //   for (let i = 0; i < this.Item.listAttributeItem.length; i++) {
    //     if (this.newType.TypeAttributeItemId == this.Item.listAttributeItem[i].TypeAttributeItemId) {
    //       this.Item.listAttributeItem[i] = JSON.parse(JSON.stringify(this.newType));
    //     }
    //   }
    // }
    this.newTypeCS.forEach((item: any) => {
      if (!this.Item.listCustomer) {
        this.Item.listCustomer = []; // Khởi tạo listCustomer là một mảng nếu nó không tồn tại
      }
    
      const exists = this.Item.listCustomer.some((i: any) => i.CustomerId === item.CustomerId);
      if (!exists && !item.TypeAttributeId) {
        this.Item.listCustomer.push(item);
      }
    });
    this.TypeModal.hide()
  }

  onSelectionChange(event: any){
    this.newTypeCS = event;
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

  //Xóa TypeAttribute
  Delete(Id) {
    this.http.delete('/api/TypeAttribute/' + Id, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.GetListTypeAttribute();
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

  //xóa AttributeItem
  DeleteItem(items: any) {
    this.Item.listCustomer = this.Item.listCustomer.filter((item: any) => item.CustomerId !== items)
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

    this.GetListTypeAttribute();
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

  CheckActionTable(TypeAttributeId) {
    if (TypeAttributeId == undefined) {
      let CheckAll = this.CheckAll;
      this.listTypeAttribute.forEach(item => {
        item.Action = CheckAll;
      });
    }
    else {
      let CheckAll = true;
      for (let i = 0; i < this.listTypeAttribute.length; i++) {
        if (!this.listTypeAttribute[i].Action) {
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
        this.listTypeAttribute.forEach(item => {
          if (item.Action == true) {
            data.push(item.TypeAttributeId);
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
                  this.http.put('/api/TypeAttribute/deletes', data, this.httpOptions).subscribe(
                    (res) => {
                      if (res["meta"]["error_code"] == 200) {
                        this.toastSuccess("Xóa thành công!");
                        this.GetListTypeAttribute();
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
  
  upload(files) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);
    console.log(formData);
    const uploadReq = new HttpRequest('POST', 'api/upload/uploadImages/6', formData, {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
      reportProgress: true,
    });

    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress)
        this.progress = Math.round(100 * event.loaded / event.total);
      else if (event.type === HttpEventType.Response) {
        this.Item.Image = event.body["data"].toString();
        console.log("this.Item.Avata", this.Item.Image)
      }
    });
  }
  RemoveImage() {
    this.Item.Image = undefined;
    this.file.nativeElement.value = "";
    this.progress = undefined;
  }
  //Get danh sách danh user
  GetListUser() {
    let data = Object.assign({}, this.q);
    this.order_by = "";

    data.page = this.paging.page;
    data.query = this.paging.query;
    data.order_by = this.order_by;

    this.http.post('/api/customer/GetAllStudent', data, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listUser = res["data"];
          this.Users = [...this.listUser];
          this.paging.item_count = res["metadata"].Sum;
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

}
