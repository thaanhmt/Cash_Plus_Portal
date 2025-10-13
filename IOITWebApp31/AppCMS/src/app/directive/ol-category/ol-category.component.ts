import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { CallCategoryFunctionService } from '../../service/call-category-function.service';
import { Subscription } from 'rxjs/Subscription';


@Component({
	selector: 'ol-category',
	templateUrl: './ol-category.component.html',
	styleUrls: ['./ol-category.component.scss']
})
export class OlCategoryComponent implements OnInit, OnDestroy {
	@Input('data') items: Array<Object>;
	@Input('key') key: string;
	@Input('IsParent') IsParent: boolean;
	@Input('hasAction') hasAction: boolean;


	subscription: Subscription;


	constructor(private callCategoryFunctionService: CallCategoryFunctionService) {
		this.subscription = this.callCategoryFunctionService.getAction().subscribe(action => {
			if (action.TypeAction == 4) {
				this.SaveCategorySort();
			}
		});
	}

	ngOnInit() {
	}

	ngOnDestroy() {
		this.subscription.unsubscribe();
	}

	AddCate(CategoryId) {
		this.callCategoryFunctionService.sendAction(CategoryId, 1, false);
	}

	UpdateCate(CategoryId) {
		this.callCategoryFunctionService.sendAction(CategoryId, 2, false);
	}

	DeleteCate(CategoryId) {
		this.callCategoryFunctionService.sendAction(CategoryId, 3, false);
	}

	SaveCategorySort() {
		console.log(this.items);
	}
}
