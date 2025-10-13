import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Subject } from 'rxjs/Subject';

@Injectable({
	providedIn: 'root'
})
export class CallCategoryFunctionService {

	private subject = new Subject<any>();

	//Gửi thông tin gọi đến hàm với TypeAction 1 = thêm mới, 2 = sửa , 3 = xóa, 4 = lưu thông tin sắp xếp,
	//5 thêm danh mục với ngôn ngữ mới, 6 sửa danh mục với ngôn ngữ mới, 7 thay đổi trạng thái danh mục, 8 là xem danh mục
	sendAction(CategoryId: number, TypeAction: Number, IsShow: boolean) {
		this.subject.next({ CategoryId: CategoryId, TypeAction: TypeAction, IsShow: IsShow });
	}

	//Nhận thông tin là hàm đã đc gọi
	getAction(): Observable<any> {
		return this.subject.asObservable();
	}

}
