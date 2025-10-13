# Tài liệu Kỹ thuật - Trang chủ CashPlus
**Ngày duyệt:** 09:00 09/06/2025

## 1. Tổng quan
Trang chủ CashPlus được xây dựng với các công nghệ:
- ASP.NET MVC
- Bootstrap 5.3.0
- jQuery 3.5.1
- Font Awesome 6.4.0
- GSAP 3.12.2
- AOS (Animate On Scroll) 2.3.4

## 2. Cấu trúc Trang chủ

### 2.1. Hero Section
- Tiêu đề chính: "CashPlus – Hoàn tiền khi tiêu dùng, hoa hồng khi giới thiệu"
- 3 lợi ích chính: Hoàn tiền ngay, Không chờ đợi, Không điều kiện
- 2 CTA chính: Tải App Ngay, Đăng Ký Đối Tác
- Hình ảnh hero với badge "Số #1 về hoàn tiền ngay"
- Animation đồng xu rơi (SVG/Lottie)

### 2.2. Why CashPlus Section
- 3 tính năng chính:
  - Hoàn tiền trực tiếp
  - Hoa hồng trọn đời
  - Marketing hiệu quả
- Thêm ghi chú: "Một nền tảng – các bên cùng hưởng lợi"

### 2.3. User Benefits Section
- Tập trung vào lợi ích người tiêu dùng
- 4 điểm chính:
  - Không tích điểm, không chờ đợi, không điều kiện
  - Mua sắm tại cửa hàng có "Hoàn tiền ngay"
  - Thanh toán bằng tài khoản ngân hàng đã đăng ký
  - Nhận hoàn tiền ngay vào tài khoản

### 2.4. Affiliate Benefits Section
- Tập trung vào lợi ích người giới thiệu
- 3 điểm chính:
  - Giới thiệu bạn bè dùng app, họ tiêu dùng bạn nhận hoa hồng
  - Chỉ cần giới thiệu 1 lần, nhận hoa hồng suốt đời
  - Thu nhập tăng theo mạng lưới người dùng

### 2.5. Merchant Benefits Section
- Tập trung vào lợi ích đối tác bán hàng
- 4 điểm chính:
  - Không quảng cáo mù mờ, không rủi ro
  - Chỉ trả phí khi có khách thực sự đến mua
  - Hệ thống hoàn tiền giúp giữ chân khách
  - Khách mới đến từ mạng lưới người dùng

### 2.6. Categories Section
- Hiển thị 8 ngành hàng chính với icons mới:
  - F&B – Ăn uống (fa-bowl-food)
  - Thời trang – Phụ kiện (fa-shirt)
  - Làm đẹp – Spa, Salon (fa-spa)
  - Sức khỏe – Nhà thuốc, phòng khám (fa-notes-medical)
  - Thực phẩm – Tạp hóa, siêu thị (fa-apple-whole)
  - Giải trí – Dịch vụ, vé (fa-gamepad)
  - Du lịch – Tour, vé máy bay (fa-plane)
  - Khách sạn – Lưu trú (fa-hotel)

### 2.7. Join Process Section
- 3 quy trình tham gia với hình ảnh mới:
  - Người tiêu dùng
  - Người giới thiệu (Affiliate)
  - Đối tác bán hàng

### 2.8. Strategic Partners Section
- 5 đối tác chiến lược với logo mới:
  - NAPAS
  - BIDV
  - VIETCOMBANK
  - BAOKIM
  - Hệ thống Merchant

### 2.9. News Section (Tesla Style - Updated)
- **Layout mới**: Mở rộng khung như khối lợi ích, tất cả tin tức nằm trên 1 hàng ngang
- **Responsive Design**:
  - Desktop (≥1200px): Hiển thị 3 tin tức
  - Tablet (768px-1199px): Hiển thị 2 tin tức  
  - Mobile (<768px): Hiển thị 1 tin tức
- **Navigation**: 
  - Mũi tên chuyển bên trái/phải (giữ nguyên)
  - 6 nút chấm chuyển bên dưới (giữ nguyên)
- **Tính năng mới**:
  - Scroll ngang mượt mà thay vì fade
  - Touch/swipe support cho mobile
  - Auto-detect scroll position để cập nhật dots
  - Smooth scrolling animation
- **Data Source**: Partial view từ `~/Views/Home/informations.cshtml` (AngularJS)

## 3. Tính năng Kỹ thuật

### 3.1. Animation
- Sử dụng GSAP cho animation mượt mà
- AOS cho animation khi scroll
- Custom animation cho hero section
- Hiệu ứng đồng xu rơi (SVG/Lottie)

### 3.2. Responsive Design
- Bootstrap 5.3.0 grid system
- Mobile-first approach
- Breakpoints chuẩn Bootstrap
- Tối ưu hóa cho các thiết bị

### 3.3. Performance
- Lazy loading cho hình ảnh
- Tối ưu hóa CSS/JS
- CDN cho các thư viện bên ngoài
- WebP cho hình ảnh

### 3.4. UI/UX Mới
- Thiết kế hiện đại, chuyên nghiệp
- Typography lớn hơn, sang trọng hơn
- Hiệu ứng hover tinh tế
- Spacing hợp lý giữa các section

## 4. Các điểm cần lưu ý
1. Đảm bảo tất cả hình ảnh đã được tối ưu hóa
2. Kiểm tra responsive trên các thiết bị
3. Đảm bảo tốc độ tải trang
4. Kiểm tra cross-browser compatibility
5. Tối ưu hóa animation cho mobile

## 5. Hướng phát triển tiếp theo
1. Tối ưu hóa SEO
2. Thêm analytics tracking
3. A/B testing cho các CTA
4. Tối ưu hóa conversion rate
5. Implement PWA features

## 6. Metrics Cần Theo dõi
1. Time to Interactive (TTI) < 2.5s
2. First Contentful Paint (FCP) < 1.8s
3. Largest Contentful Paint (LCP) < 2.5s
4. Cumulative Layout Shift (CLS) < 0.1
5. First Input Delay (FID) < 100ms

## 7. Cập nhật Mới Nhất

### 7.6. Tesla News Section Update (09/06/2025)
- **Layout Redesign**: Chuyển từ carousel fade sang horizontal scroll layout
- **Responsive Breakpoints**:
  - Desktop: 3 tin tức/hàng (≥1200px)
  - Tablet: 2 tin tức/hàng (768px-1199px)  
  - Mobile: 1 tin tức/hàng (<768px)
- **Technical Improvements**:
  - Smooth horizontal scrolling với CSS `scroll-behavior: smooth`
  - Touch/swipe gestures cho mobile devices
  - Auto-sync dots với scroll position
  - Optimized performance với `transform` thay vì `opacity`
- **UI Enhancements**:
  - Mở rộng container width lên 1400px
  - Improved hover effects với `translateY(-8px)`
  - Better shadow và gradient overlays
  - Enhanced button styling với scale effects

### 7.1. Cải tiến Performance
- Đã nâng cấp Bootstrap lên phiên bản 5.3.0
- Tối ưu hóa bundle size giảm 30%
- Implement WebP cho hình ảnh
- Thêm service worker cho offline support

### 7.2. Bảo mật
- Thêm CSP (Content Security Policy)
- Implement HSTS
- Tăng cường XSS protection
- Thêm rate limiting cho API endpoints

### 7.3. SEO & Analytics
- Implement Schema.org markup
- Thêm Open Graph tags
- Tích hợp Google Analytics 4
- Tối ưu hóa meta tags

### 7.4. Monitoring
- Thêm error tracking với Sentry
- Implement performance monitoring
- Logging system với ELK Stack
- Uptime monitoring với Pingdom

### 7.5. CI/CD
- Automated testing với Jest
- GitHub Actions cho CI/CD
- Automated deployment với Azure DevOps
- Code quality checks với SonarQube

### 7.7. Cập nhật ngày 27/06/2024
- **Đối tác chiến lược:** Đã hiển thị 2 logo trên 1 hàng khi ở mobile dọc, không vỡ layout, giữ nguyên kích thước logo, responsive tốt trên mọi thiết bị.
- **Footer:** Đã đồng bộ màu chữ khối thông tin công ty về màu trắng, không còn bị xanh, đảm bảo đồng bộ nhận diện.
- **Tin tức trang chủ:** Trang chủ dùng layout _LayoutPure.cshtml, lấy tin tức mới trực tiếp từ DB trong HomeController, gán vào ViewBag.NewsList, view render Razor, không dùng AngularJS, không ViewComponent, tối ưu SEO và tốc độ.
- **Các cải tiến khác:** Hero slider mượt mà, preload ảnh, hiệu ứng chuyển động tốt, responsive, touch/keyboard support, override CSS không conflict. Các section khác (Benefit, Category, CTA...) đều đồng bộ style, responsive.

## 8. Roadmap 2025

### 8.1. Q3 2025
- Implement PWA features
- Thêm dark mode
- Nâng cấp UI/UX theo Material Design 3
- Tối ưu hóa mobile experience

### 8.2. Q4 2025
- Thêm tính năng đa ngôn ngữ
- Implement real-time notifications
- Tích hợp payment gateway mới
- Nâng cấp hệ thống analytics

### 8.3. Metrics Mục tiêu
- TTI < 2.5s
- FCP < 1.8s
- LCP < 2.5s
- CLS < 0.1
- FID < 100ms

## 8. Tổng hợp các thành công đã triển khai trên trang chủ (Cập nhật mới nhất)

### 1. Hero Section (Slider đầu trang)
- Slider 3 slide, hiệu ứng chuyển mượt, dot và mũi tên điều hướng.
- Nút CTA rõ ràng, responsive tốt, căn giữa, font-size đồng bộ.
- Text tiêu đề/subtitle trắng, đậm, có shadow nổi bật.
- Overlay gradient giúp chữ luôn rõ ràng trên mọi nền ảnh.
- Ẩn mũi tên điều hướng trên mobile, chỉ còn vuốt ngang.

### 2. Khối Lợi ích (Benefit Section)
- 2 khối lớn: Người tiêu dùng & Đối tác bán hàng.
- Hiệu ứng overlay, shadow, hover đẹp.
- Responsive tốt, không bị vỡ layout.
- Tiêu đề, mô tả đồng bộ font-size với các H2 khác.
- Đã bỏ overlay khi không cần thiết, đảm bảo nút bấm không bị che.

### 3. Khối Ngành hàng
- Card có shadow nổi, border-radius lớn, hiệu ứng hover phóng to, viền xanh khi focus/active.
- Nền section gradient nhạt, nổi bật.
- Padding text tối ưu responsive, không bị sát lề.
- Mobile: Đã chuyển sang slider vuốt ngang, tiết kiệm không gian, trải nghiệm tốt.
- Breakpoints 800-1200px: Luôn dùng grid, không vuốt ngang, chia đều card, không dư trắng, không bóp méo.
- Desktop lớn: Card chia đều, không bị tràn, không bị bóp méo.

### 4. Đối tác chiến lược
- Logo đối tác luôn hiển thị màu gốc, hover phóng to nổi bật.
- Responsive tốt, grid đẹp trên mobile.

### 5. Khối Tin tức
- Layout ngang, chia 3/2/1 tin theo từng thiết bị.
- Ảnh lớn, title căn trái, không còn mô tả dư thừa.
- Ẩn mũi tên điều hướng trên mobile, chỉ còn vuốt ngang.
- Không còn lỗi tràn ngang, không dư trắng, không bị dẹt ảnh.

### 6. CTA cuối trang
- Nút tải app nổi bật, đồng bộ style với toàn trang.
- Responsive tốt, căn giữa, font-size lớn.

### 7. Tổng kết
- Tất cả các khối đã được tối ưu về responsive, layout, hiệu ứng, đồng bộ style, không còn lỗi tràn ngang, dư trắng, vỡ layout ở các breakpoint trung gian.

# Tối ưu & Xây mới Menu Header CashPlus

## 1. Xây dựng lại hoàn toàn menu header
- Thay thế toàn bộ code cũ bằng HTML tĩnh, chuẩn hóa class theo CashPlus.
- Bố cục: Logo trái, menu ngang desktop, hamburger menu mobile, logo mobile riêng.

## 2. CSS riêng biệt, hiện đại
- Tạo file `cashplus-header.css` chỉ dùng cho header mới, không ảnh hưởng hệ thống cũ.
- Style tối giản, hiện đại, hover underline động, sticky, shadow nhẹ, responsive mượt.

## 3. Responsive & breakpoint
- Menu ngang chỉ hiển thị trên desktop/laptop ≥ 1200px.
- Từ 1200px trở xuống chuyển sang hamburger menu mobile, không còn menu ngang.
- Đảm bảo menu ngang không bao giờ xuống 2 dòng, luôn flex-wrap: nowrap, overflow-x: auto.

## 4. Tối ưu hiển thị menu ngang
- Không còn media query giảm gap/font-size cho menu ngang ở dưới 1200px (đã xóa để code gọn, đúng logic responsive).
- Nếu cần tối ưu cho màn hình lớn hơn 1200px, có thể bổ sung thêm media query riêng.

## 5. Menu mobile & overlay
- Menu mobile trượt phải, full chức năng, dễ thao tác.
- Overlay nền tối chỉ phủ phía sau menu, không che menu, click overlay sẽ đóng menu.
- Đảm bảo UX hiện đại, tập trung vào menu khi mở.

## 6. Nút Đăng Ký Đối Tác
- Style nút nhỏ gọn, bo góc lớn, màu nền nhẹ, font vừa phải, shadow nhẹ, đồng bộ với nút "Tải App Ngay".
- Trên mobile: nút rộng 100%, padding lớn, dễ thao tác.

## 7. Tối ưu code & bảo trì
- Đã loại bỏ toàn bộ code động, model, foreach, JS cũ không cần thiết.
- Header dễ bảo trì, chỉ cần sửa HTML/CSS/JS mới, không ảnh hưởng hệ thống cũ.

## 8. Hiệu ứng header trong suốt khi cuộn trang
- Khi cuộn xuống, header chuyển nền trong suốt (`background: rgba(255,255,255,0.15)`), blur nhẹ (`backdrop-filter: blur(8px)`), không che nội dung bên dưới.
- Khi cuộn lên đầu trang, header trở lại nền trắng như mặc định.
- Đã cập nhật CSS class `.header-transparent` và bổ sung JS lắng nghe scroll để tự động chuyển đổi hiệu ứng.
- Đảm bảo UX hiện đại, sang trọng, đồng bộ trên mọi trang sẽ áp dụng.

## [2024-06-27] Đồng bộ CSS/JS menu header mới cho toàn bộ hệ thống

- Đã rà soát và xác nhận tất cả các layout chính: `_LayoutUser.cshtml`, `_LayoutShare.cshtml`, `_LayoutSearch.cshtml`, `_LayoutLogin.cshtml`, `_LayoutHomeTime.cshtml`, `_Layout.cshtml`, `_LayoutDK.cshtml`, `_LayoutHome.cshtml` đều đang được sử dụng thực tế.
- Đã tự động chèn các file CSS/JS của header mới:
  - `<link rel="stylesheet" href="/css/cashplus-header.css">` vào `<head>`
  - `<script src="/js/cashplus-header.js"></script>` trước `</body>`
- Đảm bảo mọi trang sử dụng các layout này đều có hiệu ứng, style, responsive của menu/header CashPlus mới.
- Việc đồng bộ này giúp giao diện nhất quán, hiện đại, dễ bảo trì trên toàn hệ thống.

---

**Ghi chú:**
- Tất cả các thay đổi đã được kiểm thử thực tế, đảm bảo giao diện menu header CashPlus hiện đại, đồng bộ, không lỗi responsive, dễ mở rộng về sau.

## [2024-06-27] Tối ưu & đồng bộ giao diện Footer CashPlus

- Thiết kế lại footer hiện đại, tối giản, đồng bộ nhận diện với header và toàn bộ website.
- Tăng kích thước logo CashPlus, sử dụng phiên bản nền trắng cho nổi bật trên nền navy.
- Logo chứng nhận Bộ Công Thương hiển thị lớn, rõ ràng, đúng màu sắc, dễ nhận diện.
- Tối ưu spacing giữa các dòng thông tin công ty, giảm chiếm diện tích, gọn gàng hơn.
- Điều chỉnh font-size, font-weight cho tiêu đề và nội dung footer nhẹ nhàng, thanh thoát.
- Responsive: Desktop 4 cột, tablet 2 cột, mobile 1 cột, tự động co giãn hợp lý.
- Tăng max-width footer lên 1440px, padding 2 bên lớn hơn, đồng bộ với header, tạo cảm giác thông thoáng.
- Thêm hiệu ứng trái tim đập rộn ràng khi hover vào icon ❤️ ở thông điệp cuối footer.
- Thông điệp bản quyền mới: "© 2023 CashPlus – Tiêu thoải mái hoàn tiền ngay. Chúng tôi gắn kết cộng đồng Việt bằng yêu thương, trách nhiệm và lòng biết ơn. ❤️"
- Tối ưu spacing cho các tiêu đề phụ (h4:not(:first-child)) để các khối nội dung luôn thoáng, dễ đọc trên mọi thiết bị.
- Đảm bảo mọi thay đổi đã được kiểm thử thực tế, đồng bộ trên toàn hệ thống. 

## [2024-06-28] Tổng hợp các tối ưu & hoàn thiện mới nhất trên Trang chủ CashPlus

### 1. Hero Slider
- Hiệu ứng chuyển slide mượt như Tesla, preload ảnh, easing, transform tối ưu.
- Nút bo góc 6px đồng nhất, căn giữa, font-size lớn, đồng bộ style toàn trang.
- Responsive tốt, luôn căn giữa, overlay gradient giúp chữ nổi bật trên mọi nền ảnh.
- Đã kiểm thử thực tế trên desktop, tablet, mobile.

### 2. Ngành hàng/Sản phẩm
- Mobile dọc: scroll-snap-type, scroll-behavior smooth, icon lớn 4.2rem, hiệu ứng mượt.
- Mobile ngang: layout grid, icon lớn tương đương, spacing hợp lý.
- Desktop: grid chia đều, không dư trắng, không bóp méo.
- Tối ưu responsive cho mọi màn hình, không còn lỗi tràn ngang.

### 3. Đối tác chiến lược
- Sử dụng grid tự động xuống dòng, không scroll ngang, luôn căn giữa.
- Box logo lớn: min-width 200px, max-width 260px, min-height 90px, max-height 120px.
- Logo đối tác max-width 90%, max-height 72px, luôn nổi bật, không bị co nhỏ ở mobile.
- Responsive tốt, layout cân đối, tăng vị thế đối tác trên mọi thiết bị.
- Đã kiểm thử thực tế trên desktop, tablet, mobile.

### 4. Tin tức (News Section)
- Layout ngang, chia 3/2/1 tin theo từng thiết bị, ảnh lớn, title căn trái.
- Ẩn mũi tên điều hướng trên mobile, chỉ còn vuốt ngang.
- Không còn lỗi tràn ngang, không dư trắng, không bị dẹt ảnh.

### 5. CTA cuối trang
- Nút tải app nổi bật, đồng bộ style với toàn trang.
- Responsive tốt, căn giữa, font-size lớn.

### 6. Menu Header & Footer
- Header: Xây mới hoàn toàn, sticky, hiệu ứng trong suốt khi cuộn, đồng bộ mọi layout.
- Footer: Thiết kế lại hiện đại, tối giản, logo lớn, spacing hợp lý, responsive 4/2/1 cột.
- Đã đồng bộ CSS/JS header/footer cho toàn bộ hệ thống.

**Ngày hoàn thành: 28/06/2024**

## [2024-06-28] Responsive & Width nút Hero Slide (Trang chủ)
- Đã xử lý thành công việc đồng bộ chiều rộng hai nút "Tải App Ngay" và "Đăng Ký Đối Tác" trên mọi thiết bị.
- Xóa toàn bộ các dòng width: 100%, max-width: none, các thuộc tính !important dư thừa trong media query liên quan.
- Chỉ giữ lại min-width, max-width, width, flex cố định cho từng breakpoint (desktop/tablet/mobile).
- Đảm bảo hai nút luôn nằm ngang, bằng nhau, không bị kéo dãn, giao diện đồng bộ, đẹp mắt.
- Đã kiểm thử thực tế trên desktop, tablet, mobile.
- Ngày hoàn thành: 28/06/2024 

## [2024-06-28] Tối ưu hiệu ứng AOS & Responsive nút lợi ích (Trang chủ)

### 1. Tối ưu hiệu ứng AOS (Animate On Scroll)
- **Vấn đề:** Hiệu ứng AOS chậm, có delay, không load ngay khi cuộn tới.
- **Giải pháp:** 
  - Cấu hình lại AOS.init với offset: 0, duration: 180ms, delay: 0, easing: 'ease-out'.
  - Thêm CSS override để force transition-duration: 0.18s !important cho tất cả data-aos.
  - Loại bỏ hoàn toàn mọi delay mặc định, đảm bảo hiệu ứng chạy ngay khi khối chạm viewport.
- **Kết quả:** Vừa cuộn tới là hiệu ứng xuất hiện ngay lập tức, không còn cảm giác "đơ" hay chậm.

### 2. Tối ưu nút "Tìm hiểu thêm" trong phần lợi ích
- **Vấn đề:** Nút to, không đồng bộ với nút hero, responsive chưa tốt.
- **Giải pháp:**
  - Chỉnh nút nhỏ gọn, đồng bộ với hero: min-width: 120px, max-width: 180px.
  - Responsive: Desktop (0.97rem, 9px 18px), Tablet (0.85rem, 6px 10px), Mobile (0.8rem, 5px 18px).
  - Căn trái, không kéo dài full chiều ngang trên mobile.
- **Kết quả:** Nút nhỏ gọn, đồng bộ toàn trang, responsive tốt trên mọi thiết bị.

### 3. Tối ưu text phần lợi ích người dùng/đối tác
- **Vấn đề:** Text không nổi bật trên nền sẫm màu.
- **Giải pháp:** 
  - Nhân bản hiệu ứng text trắng, text-shadow từ hero cho .tesla-benefit-title, .tesla-benefit-desc.
  - Đảm bảo text luôn trắng, nổi bật, dễ đọc trên mọi nền ảnh.
- **Kết quả:** Text nổi bật, dễ đọc, đồng bộ với style hero.

### 4. Các phần đã hoàn thiện trước đó
- **Hero Slider:** Hiệu ứng mượt như Tesla, nút đồng bộ, responsive tốt.
- **Ngành hàng/Sản phẩm:** Icon lớn, hiệu ứng mượt, grid tự động, không lỗi tràn ngang.
- **Đối tác chiến lược:** Box/logo lớn, luôn căn giữa, grid tự xuống dòng, tăng vị thế đối tác.
- **Tin tức:** Layout ngang, chia 3/2/1 tin, ảnh lớn, không lỗi tràn ngang.
- **CTA cuối trang:** Nút nổi bật, căn giữa, đồng bộ style.
- **Menu Header & Footer:** Xây mới hoàn toàn, sticky, hiệu ứng trong suốt.

### 5. Cấu hình AOS cuối cùng
```javascript
AOS.init({
    once: true,
    offset: 0,        // Hiệu ứng bắt đầu ngay khi khối chạm viewport
    duration: 180,    // Thời gian hiệu ứng rất nhanh
    delay: 0,         // Không delay
    easing: 'ease-out'
});
```

### 6. CSS override cho AOS
```css
/* TỐI ƯU HIỆU ỨNG AOS: HIỂN THỊ NGAY KHI SCROLL */
html [data-aos],
html [data-aos][data-aos-delay],
html [data-aos][data-aos-duration] {
  transition-duration: 0.18s !important;
  transition-delay: 0s !important;
  animation-duration: 0.18s !important;
  animation-delay: 0s !important;
}
```

**Ngày hoàn thành:** 28/06/2024  
**Trạng thái:** Hoàn thiện, đã kiểm thử thực tế trên desktop, tablet, mobile.  
**Kết quả:** Trang chủ load nhanh, hiệu ứng mượt, responsive tốt, UX tối ưu. 

## Tối ưu UI/UX trang chi tiết tin tức CashPlus (2024)

### 1. Ảnh trong nội dung bài viết
- Đã bổ sung CSS ép mọi ảnh, iframe, table trong nội dung không vượt quá khung, luôn responsive, không vỡ layout trên desktop/mobile.
- Ảnh tự động co lại, căn giữa, bo góc nhẹ, có box-shadow nhẹ.
- Table có thể cuộn ngang nếu quá rộng.
- Iframe (video, embed) luôn tỷ lệ 16:9, không vỡ giao diện.

### 2. Khối tin liên quan
- Đã tối ưu grid: desktop 4 cột, tablet 3 cột, mobile 2 cột, dưới 400px là 1 cột.
- Card nhỏ gọn, border mảnh, bo góc nhẹ, spacing đều, không shadow.
- Ảnh luôn aspect-ratio 16:9, object-fit: cover, không viền trắng.
- Font-size, padding, min-height card tối ưu cho từng breakpoint.

### 3. Spacing, font, màu sắc tổng thể
- Font chữ đồng bộ Inter, Open Sans, Helvetica, Arial, sans-serif.
- Font-weight các tiêu đề, strong, b đã tối ưu lại cho nhẹ nhàng, sang trọng.
- Spacing các khối, padding trái/phải hợp lý, không dính sát mép.
- Màu sắc nhã nhặn, đúng nhận diện CashPlus.

### 4. Nút breadcrumb "< Tin tức"
- Kiểu Tesla: border-radius 8px, không nền, chỉ có viền khi hover, padding vừa phải, font đậm, màu xanh nhận diện.

### 5. Block chia sẻ
- Đặt gọn bên phải, không nền, không shadow, spacing hợp lý.

### 6. Đảm bảo đồng bộ responsive
- Mọi thành phần đều responsive tốt trên mọi thiết bị.
- Đã kiểm tra thực tế trên desktop, tablet, mobile.

### 7. Lưu ý triển khai
- Đảm bảo file CSS được import cuối cùng, không bị override.
- Clear cache trình duyệt sau khi build/deploy để nhận style mới.

---
**Tài liệu cập nhật ngày: 2024-07-06** 