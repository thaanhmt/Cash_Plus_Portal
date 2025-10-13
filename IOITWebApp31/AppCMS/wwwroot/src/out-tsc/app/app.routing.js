"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.AppRoutingModule = exports.routes = void 0;
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
// Import Containers
var containers_1 = require("./containers");
var _404_component_1 = require("./views/error/404.component");
var login_component_1 = require("./views/login/login.component");
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
var partner_component_1 = require("./views/category/partner/partner.component");
var branch_component_1 = require("./views/general-manager/branch/branch.component");
var service_component_1 = require("./views/service/service.component");
var contact_component_1 = require("./views/customer-manager/contact/contact.component");
var attribuite_component_1 = require("./views/product-manager/attribuite/attribuite.component");
var review_product_component_1 = require("./views/product-manager/review-product/review-product.component");
var tag_product_component_1 = require("./views/product-manager/tag-product/tag-product.component");
var tag_news_component_1 = require("./views/news-manager/tag-news/tag-news.component");
var auth_guard_1 = require("./auth.guard");
var comment_post_component_1 = require("./views/content/comment-post/comment-post.component");
var legal_doc_component_1 = require("./views/legal-doc/legal-doc.component");
var comment_product_component_1 = require("./views/content/comment-product/comment-product.component");
var legal_doc_category_component_1 = require("./views/legal-doc-category/legal-doc-category.component");
var dictionary_component_1 = require("./views/dictionary/dictionary.component");
var news_editer_component_1 = require("./views/news-manager/news-editer/news-editer.component");
var publication_text_component_1 = require("./views/publication/publication-text.component");
var log_component_1 = require("./views/system-manager/log/log.component");
var author_component_1 = require("./views/general-manager/author/author.component");
var backlink_component_1 = require("./views/news-manager/backlink/backlink.component");
var news_cash_component_1 = require("./views/news-manager/news-cash/news-cash.component");
var upload_component_1 = require("./views/upload/upload.component");
//
var user_data_component_1 = require("./views/system-manager/user-data/user-data.component");
var application_range_component_1 = require("./views/category/application-range/application-range.component");
var unit_component_1 = require("./views/category/unit/unit.component");
var research_area_component_1 = require("./views/category/research-area/research-area.component");
var type_slide_component_1 = require("./views/category/type-slide/type-slide.component");
var config_star_component_1 = require("./views/setting-manager/config-star/config-star.component");
var dataset_component_1 = require("./views/data-manager/dataset/dataset.component");
var dataset_view_component_1 = require("./views/data-manager/dataset-view/dataset-view.component");
var dataset_down_component_1 = require("./views/data-manager/dataset-down/dataset-down.component");
exports.routes = [
    {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
    },
    {
        path: 'login',
        component: login_component_1.LoginComponent,
        data: {
            title: 'Đăng nhập'
        },
        canActivate: [auth_guard_1.AuthGuard]
    },
    {
        path: '',
        component: containers_1.DefaultLayoutComponent,
        data: {
            title: ''
        },
        children: [
            {
                path: 'config/category-menu',
                component: menu_component_1.MenuComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Danh mục menu'
                }
            },
            {
                path: 'product/category-product',
                component: product_component_1.CateProductComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Danh mục sản phẩm'
                }
            },
            {
                path: 'category/category-news',
                component: news_component_1.NewsComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Danh mục tin tức'
                }
            },
            {
                path: 'dictionary',
                component: dictionary_component_1.DictionaryComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Quản lý từ điển'
                }
            },
            {
                path: 'config/category-page',
                component: page_component_1.PageComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Danh mục trang'
                }
            },
            {
                path: 'product/category-manufacturer',
                component: manufacturer_component_1.ManufacturerComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Nhà sản xuất'
                }
            },
            {
                path: 'category/category-rank',
                component: rank_component_1.RankComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Danh mục khoảng'
                }
            },
            {
                path: 'product/trademark',
                component: trademark_component_1.TrademarkComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Thương hiệu'
                }
            },
            {
                path: 'config/config-thumb',
                component: config_thumb_component_1.ConfigThumbComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Cấu hình thumb'
                }
            },
            {
                path: 'config/config-general',
                component: config_general_component_1.ConfigGeneralComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Cấu hình chung'
                }
            },
            {
                path: 'config/config-table',
                component: config_table_component_1.ConfigTableComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Cấu hình bảng'
                }
            },
            {
                path: 'data/news-post',
                component: news_text_component_1.NewsTextComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Tin tức'
                }
            },
            {
                path: 'block',
                component: block_component_1.BlockComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Khối nội dung'
                }
            },
            {
                path: 'content/comment',
                component: comment_component_1.CommentComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Bình luận'
                }
            },
            {
                path: 'news/comment',
                component: comment_post_component_1.CommentPostComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Bình luận bài viết'
                }
            },
            {
                path: 'product/comment-product',
                component: comment_product_component_1.CommentProductComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Bình luận sản phẩm'
                }
            },
            {
                path: 'customer/list-customer',
                component: customer_component_1.CustomerComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Khách hàng'
                }
            },
            {
                path: 'dashboard',
                component: dashboard_component_1.DashboardComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Trang tổng quan'
                }
            },
            {
                path: 'data/faqs',
                component: legal_doc_component_1.LegalDocComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Quản lý câu hỏi'
                }
            },
            {
                path: 'legal-doc-manager/legal-doc-category',
                component: legal_doc_category_component_1.LegalDocCategoryComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Danh mục văn bản'
                }
            },
            {
                path: 'general/company',
                component: company_component_1.CompanyComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'công ty'
                }
            },
            {
                path: 'general/website',
                component: website_component_1.WebsiteComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Website'
                }
            },
            {
                path: 'general/bank',
                component: bank_component_1.BankComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Ngân hàng'
                }
            },
            {
                path: 'general/department',
                component: department_component_1.DepartmentComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Bộ phận'
                }
            },
            {
                path: 'general/position',
                component: position_component_1.PositionComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Chức vụ'
                }
            },
            {
                path: 'general/type-attribute',
                component: type_attribute_component_1.TypeAttributeComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Loại hình'
                }
            },
            {
                path: 'general/language',
                component: language_component_1.LanguageComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Ngôn ngữ'
                }
            },
            {
                path: 'content/material',
                component: material_component_1.MaterialComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Bảng giá'
                }
            },
            {
                path: 'order/list-order',
                component: order_component_1.OrderComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Đơn hàng'
                }
            },
            {
                path: 'product/list-product',
                component: product_component_2.ProductComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Sản phẩm'
                }
            },
            {
                path: 'config/slide',
                component: slide_component_1.SlideComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Slide'
                }
            },
            {
                path: 'system/function',
                component: function_component_1.FunctionComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Quản lý chức năng'
                }
            },
            {
                path: 'system/role',
                component: role_component_1.RoleComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Quản lý nhóm quyền'
                }
            },
            {
                path: 'system/user',
                component: user_component_1.UserComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Người dùng hệ thống'
                }
            },
            {
                path: 'system/user-data',
                component: user_data_component_1.UserDataComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Người dùng'
                }
            },
            {
                path: 'category/partner',
                component: partner_component_1.PartnerComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Đối tác'
                }
            },
            {
                path: 'general/branch',
                component: branch_component_1.BranchComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Chi nhánh'
                }
            },
            {
                path: 'service',
                component: service_component_1.ServiceComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Dịch vụ'
                }
            },
            {
                path: 'data/contact',
                component: contact_component_1.ContactComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Liên hệ'
                }
            },
            {
                path: 'product/attribuite',
                component: attribuite_component_1.AttribuiteComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Thuộc tính sản phẩm'
                }
            },
            {
                path: 'product/review-product',
                component: review_product_component_1.ReviewProductComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Đánh giá sản phẩm'
                }
            },
            {
                path: 'product/tag-product',
                component: tag_product_component_1.TagProductComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Tag sản phẩm'
                }
            },
            {
                path: 'news/tag-news',
                component: tag_news_component_1.TagNewsComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Tag tin tức'
                }
            },
            {
                path: 'news/ratify',
                component: news_ratify_component_1.NewsRatifyComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Viết bài'
                }
            },
            {
                path: 'news/news-editer',
                component: news_editer_component_1.NewsEditerComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Biên tập'
                }
            },
            {
                path: 'news/browsing',
                component: news_browsing_component_1.NewsBrowsingComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Kiểm duyệt'
                }
            },
            {
                path: 'an-pham',
                component: publication_text_component_1.PublicationComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Ấn Phẩm'
                }
            },
            {
                path: 'upload',
                component: upload_component_1.UploadComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Media'
                }
            },
            {
                path: 'system/log',
                component: log_component_1.LogComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Nhật ký hệ thống'
                }
            },
            {
                path: 'general/author',
                component: author_component_1.AuthorComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Tác giả'
                }
            },
            {
                path: 'news/backlink',
                component: backlink_component_1.BacklinkComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Liên kết'
                }
            },
            {
                path: 'news/news-cash',
                component: news_cash_component_1.NewsCashComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Nhuận bút'
                }
            },
            {
                path: 'category/unit',
                component: unit_component_1.UnitComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Danh mục Cơ quan/tổ chức'
                }
            },
            {
                path: 'category/application-range',
                component: application_range_component_1.ApplicationRangeComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Phạm vi ứng dụng'
                }
            },
            {
                path: 'category/research-area',
                component: research_area_component_1.ResearchAreaComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Lĩnh vực nghiên cứu'
                }
            },
            {
                path: 'category/question',
                component: legal_doc_category_component_1.LegalDocCategoryComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Danh mục câu hỏi'
                }
            },
            {
                path: 'category/type-slide',
                component: type_slide_component_1.TypeSlideComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Danh mục slide'
                }
            },
            {
                path: 'config/config-star',
                component: config_star_component_1.ConfigStarComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Cấu hình sao'
                }
            },
            {
                path: 'data/dataset-list',
                component: dataset_component_1.DatasetComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Bộ dữ liệu'
                }
            },
            {
                path: 'data/dataset-view',
                component: dataset_view_component_1.DatasetViewComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Lượt xem dữ liệu'
                }
            },
            {
                path: 'data/dataset-download',
                component: dataset_down_component_1.DatasetDownComponent,
                canActivate: [auth_guard_1.AuthGuard],
                data: {
                    title: 'Lượt tải dữ liệu'
                }
            },
        ]
    },
    {
        path: '**',
        component: _404_component_1.P404Component,
        pathMatch: 'full'
    }
];
var AppRoutingModule = /** @class */ (function () {
    function AppRoutingModule() {
    }
    AppRoutingModule = __decorate([
        core_1.NgModule({
            imports: [router_1.RouterModule.forRoot(exports.routes, { useHash: false })],
            exports: [router_1.RouterModule],
            providers: []
        })
    ], AppRoutingModule);
    return AppRoutingModule;
}());
exports.AppRoutingModule = AppRoutingModule;
//# sourceMappingURL=app.routing.js.map