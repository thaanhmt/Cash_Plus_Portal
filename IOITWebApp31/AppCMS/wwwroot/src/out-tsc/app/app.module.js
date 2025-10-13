"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __spreadArray = (this && this.__spreadArray) || function (to, from) {
    for (var i = 0, il = from.length, j = to.length; i < il; i++, j++)
        to[j] = from[i];
    return to;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.AppModule = void 0;
var platform_browser_1 = require("@angular/platform-browser");
var core_1 = require("@angular/core");
var common_1 = require("@angular/common");
var ngx_perfect_scrollbar_1 = require("ngx-perfect-scrollbar");
var ngx_cookie_service_1 = require("ngx-cookie-service");
var auth_guard_1 = require("./auth.guard");
var forms_1 = require("@angular/forms");
var http_1 = require("@angular/common/http");
var ngx_modal_dialog_1 = require("ngx-modal-dialog");
var modal_1 = require("ngx-bootstrap/modal");
var DEFAULT_PERFECT_SCROLLBAR_CONFIG = {
    suppressScrollX: true
};
var app_component_1 = require("./app.component");
// Import containers
var containers_1 = require("./containers");
var _404_component_1 = require("./views/error/404.component");
var _500_component_1 = require("./views/error/500.component");
var login_component_1 = require("./views/login/login.component");
var APP_CONTAINERS = [
    containers_1.DefaultLayoutComponent
];
var angular_1 = require("@coreui/angular");
// Import routing module
var app_routing_1 = require("./app.routing");
// Import 3rd party components
var dropdown_1 = require("ngx-bootstrap/dropdown");
var tabs_1 = require("ngx-bootstrap/tabs");
var ng2_charts_1 = require("ng2-charts/ng2-charts");
var ngx_toastr_1 = require("ngx-toastr");
var animations_1 = require("@angular/platform-browser/animations");
var http_client_1 = require("@ngx-loading-bar/http-client");
var pagination_1 = require("ngx-bootstrap/pagination");
var ng2_ckeditor_1 = require("ng2-ckeditor");
var ng_select_1 = require("@ng-select/ng-select");
var tooltip_1 = require("ngx-bootstrap/tooltip");
var buttons_1 = require("ngx-bootstrap/buttons");
var pre_cli_directive_1 = require("./directive/preventClick/pre-cli.directive");
var ol_component_1 = require("./directive/ol/ol.component");
var menu_component_1 = require("./views/theme-manager/menu/menu.component");
var news_component_1 = require("./views/news-manager/news-category/news.component");
var page_component_1 = require("./views/page-manager/page.component");
var product_component_1 = require("./views/product-manager/product-category/product.component");
var manufacturer_component_1 = require("./views/product-manager/manufacturer/manufacturer.component");
var rank_component_1 = require("./views/category/rank/rank.component");
var trademark_component_1 = require("./views/category/trademark/trademark.component");
var config_thumb_component_1 = require("./views/setting-manager/config-thumb/config-thumb.component");
var config_general_component_1 = require("./views/setting-manager/config-general/config-general.component");
var config_table_component_1 = require("./views/setting-manager/config-table/config-table.component");
var news_text_component_1 = require("./views/news-manager/news/news-text.component");
var news_ratify_component_1 = require("./views/news-manager/ratify/news-ratify.component");
var news_browsing_component_1 = require("./views/news-manager/browsing/news-browsing.component");
var block_component_1 = require("./views/theme-manager/block/block.component");
var comment_component_1 = require("./views/content/comment/comment.component");
var customer_component_1 = require("./views/customer-manager/customer/customer.component");
var dashboard_component_1 = require("./views/dashboard/dashboard.component");
var company_component_1 = require("./views/general-manager/company/company.component");
var website_component_1 = require("./views/general-manager/website/website.component");
var bank_component_1 = require("./views/general-manager/bank/bank.component");
var department_component_1 = require("./views/general-manager/department/department.component");
var position_component_1 = require("./views/general-manager/position/position.component");
var type_attribute_component_1 = require("./views/general-manager/type-attribute/type-attribute.component");
var language_component_1 = require("./views/general-manager/language/language.component");
var material_component_1 = require("./views/material/material.component");
var order_component_1 = require("./views/order-manager/order/order.component");
var product_component_2 = require("./views/product-manager/product/product.component");
var slide_component_1 = require("./views/slide/slide.component");
var function_component_1 = require("./views/system-manager/function/function.component");
var role_component_1 = require("./views/system-manager/role/role.component");
var user_component_1 = require("./views/system-manager/user/user.component");
var ng_pick_datetime_1 = require("ng-pick-datetime");
var ng2_currency_mask_1 = require("ng2-currency-mask");
var partner_component_1 = require("./views/category/partner/partner.component");
var branch_component_1 = require("./views/general-manager/branch/branch.component");
var service_component_1 = require("./views/service/service.component");
var contact_component_1 = require("./views/customer-manager/contact/contact.component");
var ngx_sortable_1 = require("ngx-sortable");
var ol_category_component_1 = require("./directive/ol-category/ol-category.component");
var ol_unit_component_1 = require("./directive/ol-unit/ol-unit.component");
var check_box_component_1 = require("./directive/check-box/check-box.component");
var attribuite_component_1 = require("./views/product-manager/attribuite/attribuite.component");
var truncate_pipe_1 = require("./pipe/truncate.pipe");
var review_product_component_1 = require("./views/product-manager/review-product/review-product.component");
var tag_product_component_1 = require("./views/product-manager/tag-product/tag-product.component");
var tag_news_component_1 = require("./views/news-manager/tag-news/tag-news.component");
var comment_post_component_1 = require("./views/content/comment-post/comment-post.component");
var legal_doc_component_1 = require("./views/legal-doc/legal-doc.component");
var comment_product_component_1 = require("./views/content/comment-product/comment-product.component");
var legal_doc_category_component_1 = require("./views/legal-doc-category/legal-doc-category.component");
var dictionary_component_1 = require("./views/dictionary/dictionary.component");
var search_pipe_1 = require("./search.pipe");
var news_editer_component_1 = require("./views/news-manager/news-editer/news-editer.component");
var publication_text_component_1 = require("./views/publication/publication-text.component");
var upload_component_1 = require("./views/upload/upload.component");
var log_component_1 = require("./views/system-manager/log/log.component");
var author_component_1 = require("./views/general-manager/author/author.component");
var backlink_component_1 = require("./views/news-manager/backlink/backlink.component");
var news_cash_component_1 = require("./views/news-manager/news-cash/news-cash.component");
var user_data_component_1 = require("./views/system-manager/user-data/user-data.component");
var application_range_component_1 = require("./views/category/application-range/application-range.component");
var unit_component_1 = require("./views/category/unit/unit.component");
var research_area_component_1 = require("./views/category/research-area/research-area.component");
var type_slide_component_1 = require("./views/category/type-slide/type-slide.component");
var config_star_component_1 = require("./views/setting-manager/config-star/config-star.component");
var dataset_component_1 = require("./views/data-manager/dataset/dataset.component");
var dataset_view_component_1 = require("./views/data-manager/dataset-view/dataset-view.component");
var dataset_down_component_1 = require("./views/data-manager/dataset-down/dataset-down.component");
var config_mail_component_1 = require("./views/setting-manager/config-mail/config-mail.component");
var AppModule = /** @class */ (function () {
    function AppModule() {
    }
    AppModule = __decorate([
        core_1.NgModule({
            imports: [
                platform_browser_1.BrowserModule,
                app_routing_1.AppRoutingModule,
                angular_1.AppAsideModule,
                angular_1.AppBreadcrumbModule.forRoot(),
                angular_1.AppFooterModule,
                angular_1.AppHeaderModule,
                angular_1.AppSidebarModule,
                ngx_perfect_scrollbar_1.PerfectScrollbarModule,
                dropdown_1.BsDropdownModule.forRoot(),
                tabs_1.TabsModule.forRoot(),
                ng2_charts_1.ChartsModule,
                forms_1.FormsModule,
                forms_1.ReactiveFormsModule,
                http_1.HttpClientModule,
                animations_1.BrowserAnimationsModule,
                ngx_toastr_1.ToastrModule.forRoot(),
                http_client_1.LoadingBarHttpClientModule,
                ngx_modal_dialog_1.ModalDialogModule.forRoot(),
                modal_1.ModalModule.forRoot(),
                pagination_1.PaginationModule.forRoot(),
                ng_select_1.NgSelectModule,
                tooltip_1.TooltipModule.forRoot(),
                ng2_ckeditor_1.CKEditorModule,
                buttons_1.ButtonsModule,
                ng_pick_datetime_1.OwlDateTimeModule,
                ng_pick_datetime_1.OwlNativeDateTimeModule,
                ng2_currency_mask_1.CurrencyMaskModule,
                ngx_sortable_1.NgxSortableModule
                //CKEditorModule,
            ],
            declarations: __spreadArray(__spreadArray([
                app_component_1.AppComponent
            ], APP_CONTAINERS), [
                _404_component_1.P404Component,
                _500_component_1.P500Component,
                login_component_1.LoginComponent,
                menu_component_1.MenuComponent,
                news_component_1.NewsComponent,
                page_component_1.PageComponent,
                product_component_1.CateProductComponent,
                manufacturer_component_1.ManufacturerComponent,
                rank_component_1.RankComponent,
                trademark_component_1.TrademarkComponent,
                config_thumb_component_1.ConfigThumbComponent,
                config_general_component_1.ConfigGeneralComponent,
                config_table_component_1.ConfigTableComponent,
                news_text_component_1.NewsTextComponent,
                news_ratify_component_1.NewsRatifyComponent,
                news_browsing_component_1.NewsBrowsingComponent,
                block_component_1.BlockComponent,
                comment_component_1.CommentComponent,
                customer_component_1.CustomerComponent,
                dashboard_component_1.DashboardComponent,
                company_component_1.CompanyComponent,
                website_component_1.WebsiteComponent,
                bank_component_1.BankComponent,
                department_component_1.DepartmentComponent,
                position_component_1.PositionComponent,
                type_attribute_component_1.TypeAttributeComponent,
                language_component_1.LanguageComponent,
                material_component_1.MaterialComponent,
                order_component_1.OrderComponent,
                product_component_2.ProductComponent,
                slide_component_1.SlideComponent,
                function_component_1.FunctionComponent,
                role_component_1.RoleComponent,
                user_component_1.UserComponent,
                ol_component_1.OlComponent,
                ol_unit_component_1.OlUnitComponent,
                pre_cli_directive_1.PreCliDirective,
                partner_component_1.PartnerComponent,
                branch_component_1.BranchComponent,
                service_component_1.ServiceComponent,
                contact_component_1.ContactComponent,
                ol_category_component_1.OlCategoryComponent,
                check_box_component_1.CheckBoxComponent,
                attribuite_component_1.AttribuiteComponent,
                truncate_pipe_1.TruncatePipe,
                review_product_component_1.ReviewProductComponent,
                tag_product_component_1.TagProductComponent,
                tag_news_component_1.TagNewsComponent,
                comment_post_component_1.CommentPostComponent,
                legal_doc_component_1.LegalDocComponent,
                comment_product_component_1.CommentProductComponent,
                legal_doc_category_component_1.LegalDocCategoryComponent,
                dictionary_component_1.DictionaryComponent,
                search_pipe_1.SearchPipe,
                news_editer_component_1.NewsEditerComponent,
                publication_text_component_1.PublicationComponent,
                upload_component_1.UploadComponent,
                log_component_1.LogComponent,
                author_component_1.AuthorComponent,
                backlink_component_1.BacklinkComponent,
                news_cash_component_1.NewsCashComponent,
                user_data_component_1.UserDataComponent,
                application_range_component_1.ApplicationRangeComponent,
                unit_component_1.UnitComponent,
                research_area_component_1.ResearchAreaComponent,
                type_slide_component_1.TypeSlideComponent,
                config_star_component_1.ConfigStarComponent,
                dataset_component_1.DatasetComponent,
                dataset_view_component_1.DatasetViewComponent,
                dataset_down_component_1.DatasetDownComponent,
                config_mail_component_1.ConfigMailComponent
            ]),
            exports: [pre_cli_directive_1.PreCliDirective],
            providers: [auth_guard_1.AuthGuard, ngx_cookie_service_1.CookieService, common_1.DatePipe, { provide: common_1.APP_BASE_HREF, useValue: '/cms' }],
            bootstrap: [app_component_1.AppComponent]
        })
    ], AppModule);
    return AppModule;
}());
exports.AppModule = AppModule;
//# sourceMappingURL=app.module.js.map