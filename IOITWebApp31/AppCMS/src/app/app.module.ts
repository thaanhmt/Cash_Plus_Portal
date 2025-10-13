import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { APP_BASE_HREF, LocationStrategy, HashLocationStrategy, DatePipe } from '@angular/common';

import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';

import { CookieService } from 'ngx-cookie-service';
import { AuthGuard } from './auth.guard';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { ModalDialogModule } from 'ngx-modal-dialog';
import { ModalModule } from 'ngx-bootstrap/modal';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
    suppressScrollX: true
};

import { AppComponent } from './app.component';

// Import containers
import { DefaultLayoutComponent } from './containers';

import { P404Component } from './views/error/404.component';
import { P500Component } from './views/error/500.component';
import { LoginComponent } from './views/login/login.component';

const APP_CONTAINERS = [
    DefaultLayoutComponent
];

import {
    AppAsideModule,
    AppBreadcrumbModule,
    AppHeaderModule,
    AppFooterModule,
    AppSidebarModule,

} from '@coreui/angular';

// Import routing module
import { AppRoutingModule } from './app.routing';

// Import 3rd party components
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { ChartsModule } from 'ng2-charts/ng2-charts';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoadingBarHttpClientModule } from '@ngx-loading-bar/http-client';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { CKEditorModule } from 'ng2-ckeditor';
import { NgSelectModule } from '@ng-select/ng-select';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { ButtonsModule } from 'ngx-bootstrap/buttons';

import { PreCliDirective } from './directive/preventClick/pre-cli.directive';
import { OlComponent } from './directive/ol/ol.component';


import { MenuComponent } from './views/theme-manager/menu/menu.component';
import { NewsComponent } from './views/news-manager/news-category/news.component';
import { PageComponent } from './views/page-manager/page.component';
import { CateProductComponent } from './views/product-manager/product-category/product.component';
import { ManufacturerComponent } from './views/product-manager/manufacturer/manufacturer.component';
import { RankComponent } from './views/category/rank/rank.component';
import { TrademarkComponent } from './views/category/trademark/trademark.component';

import { ConfigThumbComponent } from './views/setting-manager/config-thumb/config-thumb.component';
import { ConfigGeneralComponent } from './views/setting-manager/config-general/config-general.component';
import { ConfigTableComponent } from './views/setting-manager/config-table/config-table.component';

import { NewsTextComponent } from './views/news-manager/news/news-text.component';
import { NewsRatifyComponent } from './views/news-manager/ratify/news-ratify.component';
import { NewsBrowsingComponent } from './views/news-manager/browsing/news-browsing.component';
import { BlockComponent } from './views/theme-manager/block/block.component';
import { CommentComponent } from './views/content/comment/comment.component';

import { CustomerComponent } from './views/customer-manager/customer/customer.component';

import { DashboardComponent } from './views/dashboard/dashboard.component';

import { CompanyComponent } from './views/general-manager/company/company.component';
import { WebsiteComponent } from './views/general-manager/website/website.component';
import { BankComponent } from './views/general-manager/bank/bank.component';
import { DepartmentComponent } from './views/general-manager/department/department.component';
import { PositionComponent } from './views/general-manager/position/position.component';
import { TypeAttributeComponent } from './views/general-manager/type-attribute/type-attribute.component';

import { LanguageComponent } from './views/general-manager/language/language.component';

import { MaterialComponent } from './views/material/material.component';

import { OrderComponent } from './views/order-manager/order/order.component';

import { ProductComponent } from './views/product-manager/product/product.component';

import { SlideComponent } from './views/slide/slide.component';

import { FunctionComponent } from './views/system-manager/function/function.component';
import { RoleComponent } from './views/system-manager/role/role.component';

import { UserComponent } from './views/system-manager/user/user.component';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { CurrencyMaskModule } from "ng2-currency-mask";
import { PartnerComponent } from './views/category/partner/partner.component';
import { BranchComponent } from './views/general-manager/branch/branch.component';
import { ServiceComponent } from './views/service/service.component';
import { ContactComponent } from './views/customer-manager/contact/contact.component';
import { NgxSortableModule } from 'ngx-sortable';
import { OlCategoryComponent } from './directive/ol-category/ol-category.component';
import { OlUnitComponent } from './directive/ol-unit/ol-unit.component';
import { CheckBoxComponent } from './directive/check-box/check-box.component';
import { AttribuiteComponent } from './views/product-manager/attribuite/attribuite.component';
import { TruncatePipe } from './pipe/truncate.pipe';
import { ReviewProductComponent } from './views/product-manager/review-product/review-product.component';
import { TagProductComponent } from './views/product-manager/tag-product/tag-product.component';
import { TagNewsComponent } from './views/news-manager/tag-news/tag-news.component';
import { CommentPostComponent } from './views/content/comment-post/comment-post.component';
import { LegalDocComponent } from './views/legal-doc/legal-doc.component';
import { CommentProductComponent } from './views/content/comment-product/comment-product.component';
import { LegalDocCategoryComponent } from './views/legal-doc-category/legal-doc-category.component';
import { DictionaryComponent } from './views/dictionary/dictionary.component';
import { SearchPipe } from './search.pipe';
import { NewsEditerComponent } from './views/news-manager/news-editer/news-editer.component';
import { PublicationComponent } from './views/publication/publication-text.component';
import { UploadComponent } from './views/upload/upload.component';
import { LogComponent } from './views/system-manager/log/log.component';
import { AuthorComponent } from './views/general-manager/author/author.component';
import { BacklinkComponent } from './views/news-manager/backlink/backlink.component';
import { NewsCashComponent } from './views/news-manager/news-cash/news-cash.component';
import { UserDataComponent } from './views/system-manager/user-data/user-data.component';
import { ApplicationRangeComponent } from './views/category/application-range/application-range.component';
import { UnitComponent } from './views/category/unit/unit.component';
import { ResearchAreaComponent } from './views/category/research-area/research-area.component';
import { TypeSlideComponent } from './views/category/type-slide/type-slide.component';
import { ConfigStarComponent } from './views/setting-manager/config-star/config-star.component';
import { DatasetComponent } from './views/data-manager/dataset/dataset.component';
import { DatasetViewComponent } from './views/data-manager/dataset-view/dataset-view.component';
import { DatasetDownComponent } from './views/data-manager/dataset-down/dataset-down.component';
import { ConfigMailComponent } from './views/setting-manager/config-mail/config-mail.component';
import { StudentListComponent } from './views/system-manager/students/student-list.component';
import { StudentCommentComponent } from './views/system-manager/student-comment/student-comment.component';

@NgModule({
    imports: [
        BrowserModule,
        AppRoutingModule,
        AppAsideModule,
        AppBreadcrumbModule.forRoot(),
        AppFooterModule,
        AppHeaderModule,
        AppSidebarModule,
        PerfectScrollbarModule,
        BsDropdownModule.forRoot(),
        TabsModule.forRoot(),
        ChartsModule,
        FormsModule,
        ReactiveFormsModule,
        HttpClientModule,
        BrowserAnimationsModule,
        ToastrModule.forRoot(),
        LoadingBarHttpClientModule,
        ModalDialogModule.forRoot(),
        ModalModule.forRoot(),
        PaginationModule.forRoot(),
        NgSelectModule,
        TooltipModule.forRoot(),
        CKEditorModule,
        ButtonsModule,
        OwlDateTimeModule,
        OwlNativeDateTimeModule,
        CurrencyMaskModule,
        NgxSortableModule
        //CKEditorModule,

    ],
    declarations: [
        AppComponent,
        ...APP_CONTAINERS,
        P404Component,
        P500Component,
        LoginComponent,
        MenuComponent,
        NewsComponent,
        PageComponent,
        CateProductComponent,
        ManufacturerComponent,
        RankComponent,
        TrademarkComponent,
        ConfigThumbComponent,
        ConfigGeneralComponent,
        ConfigTableComponent,
        NewsTextComponent,
        NewsRatifyComponent,
        NewsBrowsingComponent,
        BlockComponent,
        CommentComponent,
        CustomerComponent,
        DashboardComponent,
        CompanyComponent,
        WebsiteComponent,
        BankComponent,
        DepartmentComponent,
        PositionComponent,
        TypeAttributeComponent,
        LanguageComponent,
        MaterialComponent,
        OrderComponent,
        ProductComponent,
        SlideComponent,
        FunctionComponent,
        RoleComponent,
        UserComponent,
        OlComponent,
        OlUnitComponent,
        PreCliDirective,
        PartnerComponent,
        BranchComponent,
        ServiceComponent,
        ContactComponent,
        OlCategoryComponent,
        CheckBoxComponent,
        AttribuiteComponent,
        TruncatePipe,
        ReviewProductComponent,
        TagProductComponent,
        TagNewsComponent,
        CommentPostComponent,
        LegalDocComponent,
        CommentProductComponent,
        LegalDocCategoryComponent,
        DictionaryComponent,
        SearchPipe,
        NewsEditerComponent,
        PublicationComponent,
        UploadComponent,
        LogComponent,
        AuthorComponent,
        BacklinkComponent,
        NewsCashComponent,
        UserDataComponent,
        ApplicationRangeComponent,
        UnitComponent,
        ResearchAreaComponent,
        TypeSlideComponent,
        ConfigStarComponent,
        DatasetComponent,
        DatasetViewComponent,
        DatasetDownComponent,
        ConfigMailComponent,
        StudentListComponent,
        StudentCommentComponent
    ],
    exports: [PreCliDirective],
    providers: [AuthGuard, CookieService, DatePipe, { provide: APP_BASE_HREF, useValue: '/cms' }],
    bootstrap: [AppComponent]
})
export class AppModule { }
