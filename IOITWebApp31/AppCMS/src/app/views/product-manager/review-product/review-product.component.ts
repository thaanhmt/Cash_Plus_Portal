import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { Paging, QueryFilter } from '../../../data/dt';
import { ProductReviewStatus, ActionTable } from '../../../data/const';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';




@Component({
	selector: 'app-review-product',
	templateUrl: './review-product.component.html',
	styleUrls: ['./review-product.component.scss']
})
export class ReviewProductComponent implements OnInit {
	public paging: Paging;
	public q: QueryFilter;

	public listProductReviews = [];
	public ProductReviewStatus = ProductReviewStatus;

	public httpOptions: any;
	public ActionTable = ActionTable;
	public ActionId: number;
	public CheckAll: boolean;
	public total: any;

	constructor(public http: HttpClient, public toastr: ToastrService, public modalDialogService: ModalDialogService, public viewRef: ViewContainerRef) {
		this.paging = new Paging();
		this.paging.page = 1;
		this.paging.page_size = 10;
		this.paging.query = "1=1";
		this.paging.order_by = "";
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
		this.GetListProductReviews();
	}

	GetListProductReviews() {
		this.http.get('/api/product/ProductReview/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
			(res) => {
				if (res["meta"]["error_code"] == 200) {
					this.listProductReviews = res["data"];
					this.paging.item_count = res["metadata"].Sum;
					this.total = res["metadata"];
				}
			},
			(err) => {
				console.log("Error: connect to API");
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

		this.GetListProductReviews();
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

	//Chuyển trang
	PageChanged(event) {
		this.paging.page = event.page;
		this.GetListProductReviews();
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
				query += ' and (Name.Contains("' + this.q.txtSearch + '") OR Email.Contains("' + this.q.txtSearch + '") OR ProductName.Contains("' + this.q.txtSearch + '"))';
			}
			else {
				query += '(Name.Contains("' + this.q.txtSearch + '") OR Email.Contains("' + this.q.txtSearch + '") OR ProductName.Contains("' + this.q.txtSearch + '"))';
			}
		}

		if (this.q["Type"] != undefined) {
			if (query != '') {
				query += ' and Status=' + this.q["Type"];
			}
			else {
				query += 'Status=' + this.q["Type"];
			}
		}

		if (query == '')
			this.paging.query = '1=1';
		else
			this.paging.query = query;

		this.GetListProductReviews();
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
						this.http.delete('/api/Product/deleteProductReview/' + Id, this.httpOptions).subscribe(
							(res) => {
								if (res["meta"]["error_code"] == 200) {
									this.GetListProductReviews();
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
					buttonClass: 'btn btn-default',

				}
			],
		});
	}

	CheckActionTable(ProductReviewId) {
		if (ProductReviewId == undefined) {
			let CheckAll = this.CheckAll;
			this.listProductReviews.forEach(item => {
				item.Action = CheckAll;
			});
		}
		else {
			let CheckAll = true;
			for (let i = 0; i < this.listProductReviews.length; i++) {
				if (!this.listProductReviews[i].Action) {
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
				this.listProductReviews.forEach(item => {
					if (item.Action == true) {
						data.push(item.ProductReviewId);
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
									this.http.put('/api/Product/deleteProductReviews', data, this.httpOptions).subscribe(
										(res) => {
											if (res["meta"]["error_code"] == 200) {
												this.toastSuccess("Xóa thành công!");
												this.GetListProductReviews();
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
