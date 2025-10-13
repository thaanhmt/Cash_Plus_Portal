import { Component, OnInit, ViewChild, ViewContainerRef, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { domainImage, ActionTable } from '../../../data/const';
import { Branch } from '../../../data/model';
import { ToastrService } from 'ngx-toastr';
import { DatePipe } from '@angular/common';
import { Md5 } from 'ts-md5/dist/md5';
import { Paging, QueryFilter } from '../../../data/dt';

@Component({
	selector: 'app-branch',
	templateUrl: './branch.component.html',
	styleUrls: ['./branch.component.scss']
})
export class BranchComponent implements OnInit {
	@ViewChild('BranchModal') public BranchModal: ModalDirective;
	@ViewChild('file') file: ElementRef;

	public paging: Paging;
	public q: QueryFilter;
    public listBranch = [];
    public listLanguage = [];
	public ckeConfig: any;
	public Item: Branch;
	public progress: number;
	public domainImage = domainImage;
	public httpOptions: any;

	public ActionTable = ActionTable;
	public ActionId: number;
	public CheckAll: boolean;

	constructor(
		public http: HttpClient,
		public modalDialogService: ModalDialogService,
		public viewRef: ViewContainerRef,
		public toastr: ToastrService
	) {
		this.Item = new Branch();
		this.paging = new Paging();
		this.paging.page = 1;
		this.paging.page_size = 10;
		this.paging.query = "1=1";
		this.paging.order_by = "BranchId Desc";
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
		this.GetListBranch();
        this.GetListLanguage();
	}
	GetListBranch() {
		this.http.get('/api/branch/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
			(res) => {
				if (res["meta"]["error_code"] == 200) {
					this.listBranch = res["data"];
					this.paging.item_count = res["metadata"];
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
                    if (this.listLanguage.length == 1) {
                        this.Item.LanguageId = this.listLanguage[0].LanguageId;
                    }
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
		this.GetListBranch();
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

		this.GetListBranch();
	}

	//Mở modal thêm mới
	OpenBranchModal(item) {
		this.Item = new Branch();
		this.file.nativeElement.value = "";
		this.progress = undefined;
		if (item != undefined) {
			this.Item = JSON.parse(JSON.stringify(item));
		}
		this.BranchModal.show();
	}

	//Thêm mới khách hàng
	SaveBranch() {
		if (this.Item.Code == undefined || this.Item.Code == '') {
			this.toastWarning("Chưa nhập mã!");
			return;
		} else if (this.Item.Code.replace(/ /g, '') == '') {
			this.toastWarning("Chưa nhập mã!");
			return;
		} else if (this.Item.Name == undefined || this.Item.Name == '') {
			this.toastWarning("Chưa nhập tên!");
			return;
		} else if (this.Item.Name.replace(/ /g, '') == '') {
			this.toastWarning("Chưa nhập tên!");
			return;
		} else if (this.Item.Email == undefined || this.Item.Email == '') {
			this.toastWarning("Chưa nhập email!");
			return;
		} else if (this.Item.Email.replace(/ /g, '') == '') {
			this.toastWarning("Chưa nhập email!");
			return;
		} else if (this.Item.Phone == undefined || this.Item.Phone == '') {
			this.toastWarning("Chưa nhập số điện thoại!");
			return;
		}

		this.Item.UserId = parseInt(localStorage.getItem("userId"));

		if (this.Item.BranchId == undefined) {
			let obj = JSON.parse(JSON.stringify(this.Item));

			this.http.post('/api/Branch', obj, this.httpOptions).subscribe(
				(res) => {
					if (res["meta"]["error_code"] == 200) {
						this.GetListBranch();
						this.BranchModal.hide();
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
			this.http.put('/api/Branch/' + this.Item.BranchId, this.Item, this.httpOptions).subscribe(
				(res) => {
					if (res["meta"]["error_code"] == 200) {
						this.GetListBranch();
						this.BranchModal.hide();
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
						this.DeleteBranch(Id);
					}
				},
				{
					text: 'Đóng',
					buttonClass: 'btn btn-default',

				}
			],
		});
	}

	DeleteBranch(Id) {
		this.http.delete('/api/Branch/' + Id, this.httpOptions).subscribe(
			(res) => {
				if (res["meta"]["error_code"] == 200) {
					this.GetListBranch();
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

	upload(files) {
		if (files.length === 0)
			return;

		const formData = new FormData();

		for (let file of files)
			formData.append(file.name, file);
		console.log(formData);
		const uploadReq = new HttpRequest('POST', 'api/upload/uploadImage/6', formData, {
			headers: new HttpHeaders({
				'Authorization': 'bearer ' + localStorage.getItem("access_token")
			}),
			reportProgress: true,
		});

		this.http.request(uploadReq).subscribe(event => {
			if (event.type === HttpEventType.UploadProgress)
				this.progress = Math.round(100 * event.loaded / event.total);
			else if (event.type === HttpEventType.Response) {
				this.Item.Avatar = event.body["data"].toString();
			}
		});
	}
	//
	RemoveImage() {
		this.Item.Avatar = undefined;
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

		this.GetListBranch();
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

	CheckActionTable(BranchId) {
		if (BranchId == undefined) {
			let CheckAll = this.CheckAll;
			this.listBranch.forEach(item => {
				item.Action = CheckAll;
			});
		}
		else {
			let CheckAll = true;
			for (let i = 0; i < this.listBranch.length; i++) {
				if (!this.listBranch[i].Action) {
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
				this.listBranch.forEach(item => {
					if (item.Action == true) {
						data.push(item.BranchId);
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
									this.http.put('/api/branch/deletes', data, this.httpOptions).subscribe(
										(res) => {
											if (res["meta"]["error_code"] == 200) {
												this.toastSuccess("Xóa thành công!");
												this.GetListBranch();
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
