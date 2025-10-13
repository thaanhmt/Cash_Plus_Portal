import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

// Import Containers
import { DefaultLayoutComponent } from './containers';

import { P404Component } from './views/error/404.component';
import { P500Component } from './views/error/500.component';
import { LoginComponent } from './views/login/login.component';

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

import { PartnerComponent } from './views/category/partner/partner.component';

import { BranchComponent } from './views/general-manager/branch/branch.component';

import { ServiceComponent } from './views/service/service.component';

import { ContactComponent } from './views/customer-manager/contact/contact.component';

import { AttribuiteComponent } from './views/product-manager/attribuite/attribuite.component';

import { ReviewProductComponent } from './views/product-manager/review-product/review-product.component';

import { TagProductComponent } from './views/product-manager/tag-product/tag-product.component';

import { TagNewsComponent } from './views/news-manager/tag-news/tag-news.component';

import { AuthGuard } from './auth.guard';
import { CommentPostComponent } from './views/content/comment-post/comment-post.component';
import { LegalDocComponent } from './views/legal-doc/legal-doc.component';
import { CommentProductComponent } from './views/content/comment-product/comment-product.component';
import { LegalDocCategoryComponent } from './views/legal-doc-category/legal-doc-category.component';
import { DictionaryComponent } from './views/dictionary/dictionary.component';
import { NewsEditerComponent } from './views/news-manager/news-editer/news-editer.component';

import { PublicationComponent } from './views/publication/publication-text.component';
import { LogComponent } from './views/system-manager/log/log.component';
import { AuthorComponent } from './views/general-manager/author/author.component';
import { BacklinkComponent } from './views/news-manager/backlink/backlink.component';
import { NewsCashComponent } from './views/news-manager/news-cash/news-cash.component';
import { UploadComponent } from './views/upload/upload.component';
//
import { UserDataComponent } from './views/system-manager/user-data/user-data.component';
import { ApplicationRangeComponent } from './views/category/application-range/application-range.component';
import { UnitComponent } from './views/category/unit/unit.component';
import { ResearchAreaComponent } from './views/category/research-area/research-area.component';
import { TypeSlideComponent } from './views/category/type-slide/type-slide.component';
import { ConfigStarComponent } from './views/setting-manager/config-star/config-star.component';
import { DatasetComponent } from './views/data-manager/dataset/dataset.component';
import { DatasetViewComponent } from './views/data-manager/dataset-view/dataset-view.component';
import { DatasetDownComponent } from './views/data-manager/dataset-down/dataset-down.component';
import { StudentListComponent } from './views/system-manager/students/student-list.component';
import { StudentCommentComponent } from './views/system-manager/student-comment/student-comment.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: LoginComponent,
    data: {
      title: 'Đăng nhập'
    },
    canActivate: [AuthGuard]
  },
  {
    path: '',
    component: DefaultLayoutComponent,
    data: {
      title: ''
    },
    children: [
      {
        path: 'config/category-menu',
        component: MenuComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Danh mục menu'
        }
      },
      {
        path: 'product/category-product',
        component: CateProductComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Danh mục sản phẩm'
        }
      },
      {
        path: 'category/category-news',
        component: NewsComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Danh mục tin tức'
        }
      },
      {
        path: 'dictionary',
        component: DictionaryComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Quản lý từ điển'
        }
      },
      {
        path: 'config/category-page',
        component: PageComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Danh mục trang'
        }
      },
      {
        path: 'product/category-manufacturer',
        component: ManufacturerComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Nhà sản xuất'
        }
      },
      {
        path: 'category/category-rank',
        component: RankComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Danh mục khoảng'
        }
      },
      {
        path: 'product/trademark',
        component: TrademarkComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Thương hiệu'
        }
      },
      {
        path: 'config/config-thumb',
        component: ConfigThumbComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Cấu hình thumb'
        }
      },
      {
        path: 'config/config-general',
        component: ConfigGeneralComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Cấu hình chung'
        }
      },
      {
        path: 'config/config-table',
        component: ConfigTableComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Cấu hình bảng'
        }
      },
      {
        path: 'data/news-post',
        component: NewsTextComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Tin tức'
        }
      },
      {
        path: 'block',
        component: BlockComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Khối nội dung'
        }
      },
      {
        path: 'content/comment',
        component: CommentComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Bình luận'
        }
      },
      {
        path: 'news/comment',
        component: CommentPostComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Bình luận bài viết'
        }
      },
      {
        path: 'product/comment-product',
        component: CommentProductComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Bình luận sản phẩm'
        }
      },
      {
        path: 'customer/list-customer',
        component: CustomerComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Khách hàng'
        }
      },
      {
        path: 'dashboard',
        component: DashboardComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Trang tổng quan'
        }
      },
      {
        path: 'data/faqs',
        component: LegalDocComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Quản lý câu hỏi'
        }
      },
      {
        path: 'legal-doc-manager/legal-doc-category',
        component: LegalDocCategoryComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Danh mục văn bản'
        }
      },
      {
        path: 'general/company',
        component: CompanyComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'công ty'
        }
      },
      {
        path: 'general/website',
        component: WebsiteComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Website'
        }
      },
      {
        path: 'general/bank',
        component: BankComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Ngân hàng'
        }
      }
      ,
      {
        path: 'general/department',
        component: DepartmentComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Bộ phận'
        }
      },
      {
        path: 'general/position',
        component: PositionComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Chức vụ'
        }
      },
      {
        path: 'category/type-attribute',
        component: TypeAttributeComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Loại hình'
        }
      },
      {
        path: 'general/language',
        component: LanguageComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Ngôn ngữ'
        }
      },
      {
        path: 'content/material',
        component: MaterialComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Bảng giá'
        }
      },
      {
        path: 'order/list-order',
        component: OrderComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Đơn hàng'
        }
      },
      {
        path: 'product/list-product',
        component: ProductComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Sản phẩm'
        }
      },
      {
        path: 'config/slide',
        component: SlideComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Slide'
        }
      },
      {
        path: 'system/function',
        component: FunctionComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Quản lý chức năng'
        }
      },
      {
        path: 'system/role',
        component: RoleComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Quản lý nhóm quyền'
        }
      },
      {
        path: 'system/user',
        component: UserComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Người dùng hệ thống'
        }
      },
      {
        path: 'system/user-data',
        component: UserDataComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Người dùng'
        }
      },
      {
        path: 'category/partner',
        component: PartnerComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Đối tác'
        }
      },
      {
        path: 'general/branch',
        component: BranchComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Chi nhánh'
        }
      },
      {
        path: 'service',
        component: ServiceComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Dịch vụ'
        }
      },
      {
        path: 'data/contact',
        component: ContactComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Liên hệ'
        }
      },
      {
        path: 'product/attribuite',
        component: AttribuiteComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Thuộc tính sản phẩm'
        }
      },
      {
        path: 'product/review-product',
        component: ReviewProductComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Đánh giá sản phẩm'
        }
      },
      {
        path: 'product/tag-product',
        component: TagProductComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Tag sản phẩm'
        }
      },
      {
        path: 'news/tag-news',
        component: TagNewsComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Tag tin tức'
        }
      },
      {
        path: 'news/ratify',
        component: NewsRatifyComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Viết bài'
        }
      },
      {
        path: 'news/news-editer',
        component: NewsEditerComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Biên tập'
        }
      },
      {
        path: 'news/browsing',
        component: NewsBrowsingComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Kiểm duyệt'
        }
      },
      {
        path: 'an-pham',
        component: PublicationComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Ấn Phẩm'
        }
      },
      {
        path: 'upload',
        component: UploadComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Media'
        }
      },
      {
        path: 'system/log',
        component: LogComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Nhật ký hệ thống'
        }
      },
      {
        path: 'general/author',
        component: AuthorComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Tác giả'
        }
      },
      {
        path: 'news/backlink',
        component: BacklinkComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Liên kết'
        }
      },
      {
        path: 'news/news-cash',
        component: NewsCashComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Nhuận bút'
        }
      },
      {
        path: 'category/unit',
        component: UnitComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Danh mục Cơ quan/tổ chức'
        }
      },
      {
        path: 'category/application-range',
        component: ApplicationRangeComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Phạm vi ứng dụng'
        }
      },
      {
        path: 'category/research-area',
        component: ResearchAreaComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Lĩnh vực nghiên cứu'
        }
      },
      {
        path: 'category/question',
        component: LegalDocCategoryComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Danh mục câu hỏi'
        }
      },
      {
        path: 'category/type-slide',
        component: TypeSlideComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Danh mục slide'
        }
      },
      {
        path: 'config/config-star',
        component: ConfigStarComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Cấu hình sao'
        }
      },
      {
        path: 'data/dataset-list',
        component: DatasetComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Bộ dữ liệu'
        }
      },
      {
        path: 'data/dataset-view',
        component: DatasetViewComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Lượt xem dữ liệu'
        }
      },
      {
        path: 'data/dataset-download',
        component: DatasetDownComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Lượt tải dữ liệu'
        }
      },
      // haohv
      {
        path: 'category/join-member',
        component: StudentListComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Danh sách sinh viên'
        }
      },
      {
        path: 'category/comments-join-member',
        component: StudentCommentComponent,
        canActivate: [AuthGuard],
        data: {
          title: 'Danh sách bình luận người tham gia'
        }
      },
    ]
  },
  {
    path: '**',
    component: P404Component,
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: false })],
  exports: [RouterModule],
  providers: []
})
export class AppRoutingModule { }
