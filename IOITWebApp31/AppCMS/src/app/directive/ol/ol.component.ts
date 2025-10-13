import { Component, OnInit, Input, OnDestroy, ChangeDetectionStrategy } from '@angular/core';
import { CallCategoryFunctionService } from '../../service/call-category-function.service';
import { Subscription } from 'rxjs/Subscription';
import { domainImage, domainMedia } from '../../data/const';
import { CheckRole } from '../../data/dt';
import { CommonService } from '../../service/common.service';

@Component({
    selector: 'ol',
    templateUrl: './ol.component.html',
    styleUrls: ['./ol.component.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class OlComponent implements OnInit, OnDestroy {
    @Input('data') items: Array<Object>;
    @Input('key') key: string;
    @Input('hasAction') hasAction: boolean;
    @Input('listLanguage') listLanguage: Array<Object>;
    @Input('languageId') languageId: number;
  @Input('functionCode') functionCode: string;
  @Input('hasAdd') hasAdd: boolean;
    public domainImage = domainImage;
  public domainMedia = domainMedia;
  public CheckRole: CheckRole;

    subscription: Subscription;

  constructor(private callCategoryFunctionService: CallCategoryFunctionService,
    public common: CommonService) {
        // this.subscription = this.callCategoryFunctionService.getAction().subscribe(action => {
        // 	if (action.TypeAction == 4) {
        // 		this.SaveCategorySort();
        // 	}
        // });
     
    }

    ngOnInit() {
        //console.log(this.listLanguage);
      this.CheckRole = new CheckRole();
      //console.log(this.items);
      this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 0);
      this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 1);
      this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 2);
      this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 3);
    }

    ngOnDestroy() {
        // this.subscription.unsubscribe();
    }

    AddCate(CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 1, false);
    }

    UpdateCate(CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 2, false);
    }

  ViewCate(CategoryId) {
    this.callCategoryFunctionService.sendAction(CategoryId, 8, false);
  }

    DeleteCate(CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 3, false);
    }

    AddCateLang(CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 5, false);
    }

    UpdateCateLang(CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 6, false);
    }

    SaveCategorySort() {
        //console.log(this.items);
    }

    ShowHide(CategoryId, IsShow) {
        this.callCategoryFunctionService.sendAction(CategoryId, 7, IsShow);
    }

}
