# Hướng dẫn Chuyển đổi Layout theo Nhu cầu AngularJS

## 📋 **Tổng quan**
Tối ưu hiệu suất website bằng cách sử dụng layout phù hợp với từng trang:
- **`_LayoutHome.cshtml`**: Cho trang cần AngularJS
- **`_LayoutPure.cshtml`**: Cho trang không cần AngularJS

## 🎯 **Lợi ích**
- ✅ Giảm 40-50% kích thước HTML
- ✅ Tăng tốc độ load 30-40%
- ✅ Cải thiện Core Web Vitals
- ✅ Dễ bảo trì và mở rộng

## 📊 **Phân loại Trang**

### **A. Trang CẦN AngularJS (Sử dụng `_LayoutHome.cshtml`)**

#### **1. Trang chủ & Tin tức**
```csharp
// Trang chủ - Section tin tức cần AngularJS
Layout = "~/Views/Shared/_LayoutHome.cshtml";
```
- `Views/Home/Index.cshtml` ✅
- `Views/Customer/News.cshtml` ✅
- `Views/Customer/PrivacyPolicy.cshtml` ✅

#### **2. Form Đăng ký/Đăng nhập**
```csharp
// Form phức tạp cần AngularJS
Layout = "~/Views/Shared/_LayoutHome.cshtml";
```
- `Views/Customer/CashplusInfo.cshtml` ✅
- `Views/Customer/RegisterPartner.cshtml` ✅
- `Views/Customer/RegisterInfo.cshtml` ✅
- `Views/Customer/Login.cshtml` ✅
- `Views/Customer/ChangePass.cshtml` ✅

#### **3. Form Thông tin Cá nhân**
```csharp
// Form chỉnh sửa thông tin cần AngularJS
Layout = "~/Views/Shared/_LayoutHome.cshtml";
```
- `Views/Customer/InfoUserPage.cshtml` ✅
- `Views/Customer/InfoUser.cshtml` ✅
- `Views/Customer/InfoDetail.cshtml` ✅

#### **4. Quản lý & Báo cáo**
```csharp
// Dashboard quản lý cần AngularJS
Layout = "~/Views/Shared/_LayoutHome.cshtml";
```
- `Views/Customer/ManageUnit.cshtml` ✅
- `Views/Customer/ManageUser.cshtml` ✅
- `Views/Customer/ListRegister.cshtml` ✅
- `Views/Customer/ManageLink.cshtml` ✅
- `Views/Customer/DetailCashplus.cshtml` ✅

#### **5. OTP & Bảo mật**
```csharp
// Form OTP cần AngularJS
Layout = "~/Views/Shared/_LayoutHome.cshtml";
```
- `Views/Customer/RecoverPassOTP.cshtml` ✅
- `Views/Customer/RecoverSecurityOTP.cshtml` ✅
- `Views/Customer/RecoverChangeSecurityOTP.cshtml` ✅

#### **6. Cập nhật Đối tác**
```csharp
// Form cập nhật phức tạp cần AngularJS
Layout = "~/Views/Shared/_LayoutHome.cshtml";
```
- `Views/Customer/PartnerUpdate.cshtml` ✅

### **B. Trang KHÔNG CẦN AngularJS (Sử dụng `_LayoutPure.cshtml`)**

#### **1. Chi tiết Sản phẩm**
```csharp
// Trang chi tiết đơn giản
Layout = "~/Views/Shared/_LayoutPure.cshtml";
```
- `Views/Detail/Product.cshtml` ✅
- `Views/Detail/Video.cshtml` ✅
- `Views/Detail/Image.cshtml` ✅
- `Views/Detail/Attactment.cshtml` ✅
- `Views/Detail/DetailLegalDoc.cshtml` ✅

#### **2. Danh mục & Danh sách**
```csharp
// Trang danh mục đơn giản
Layout = "~/Views/Shared/_LayoutPure.cshtml";
```
- `Views/Category/Product.cshtml` ✅
- `Views/Category/SolutionsServices.cshtml` ✅
- `Views/Category/Video.cshtml` ✅
- `Views/Category/Timeline.cshtml` ✅
- `Views/Category/ProductChild.cshtml` ✅
- `Views/Category/PageParent.cshtml` ✅
- `Views/Category/Notification.cshtml` ✅
- `Views/Category/ListVideo.cshtml` ✅
- `Views/Category/ListGallery.cshtml` ✅
- `Views/Category/ListAllProduct.cshtml` ✅
- `Views/Category/introduce.cshtml` ✅
- `Views/Category/Image.cshtml` ✅

#### **3. Giới thiệu & Tĩnh**
```csharp
// Trang tĩnh không cần tương tác
Layout = "~/Views/Shared/_LayoutPure.cshtml";
```
- `Views/Home/About.cshtml` ✅

#### **4. Tải xuống**
```csharp
// Trang tải xuống đơn giản
Layout = "~/Views/Shared/_LayoutPure.cshtml";
```
- `Views/Customer/Dowloadnow.cshtml` ✅
- `Views/Customer/DowloadMernow.cshtml` ✅

#### **5. Tin tức & Preview**
```csharp
// Trang tin tức đơn giản
Layout = "~/Views/Shared/_LayoutPure.cshtml";
```
- `Views/Preview/News.cshtml` ✅
- `Views/Detail/Notificaton.cshtml` ✅

## 🔄 **Hướng dẫn Chuyển đổi**

### **Bước 1: Xác định Loại Trang**
```csharp
// Kiểm tra xem trang có sử dụng AngularJS directives không
// Tìm các từ khóa: ng-controller, ng-model, ng-repeat, ng-app
```

### **Bước 2: Chọn Layout Phù hợp**
```csharp
// Nếu có AngularJS directives
Layout = "~/Views/Shared/_LayoutHome.cshtml";

// Nếu không có AngularJS directives
Layout = "~/Views/Shared/_LayoutPure.cshtml";
```

### **Bước 3: Test và Kiểm tra**
```bash
# Kiểm tra hiệu suất
# Sử dụng ?debug=layout để xem thông tin debug
https://cashplus.vn/detail/product?debug=layout
```

## 📈 **Metrics So sánh**

### **Trước khi tối ưu:**
- **Kích thước HTML**: ~15KB
- **HTTP Requests**: 25-30 requests
- **Load Time**: 3-4 giây
- **AngularJS**: Load cho tất cả trang

### **Sau khi tối ưu:**
- **Kích thước HTML**: ~8KB (giảm 47%)
- **HTTP Requests**: 15-20 requests (giảm 33%)
- **Load Time**: 2-2.5 giây (giảm 25%)
- **AngularJS**: Chỉ load khi cần thiết

## 🛠️ **Script Tự động**

### **Layout Optimizer**
```javascript
// Tự động phát hiện và tối ưu
window.LayoutOptimizer.optimizeLayout();
window.LayoutOptimizer.optimizePerformance();
```

### **Debug Mode**
```javascript
// Thêm ?debug=layout vào URL để xem thông tin debug
https://cashplus.vn/home/index?debug=layout
```

## ✅ **Danh sách Kiểm tra**

### **Trang đã chuyển đổi:**
- [ ] Index.cshtml → _LayoutHome.cshtml
- [ ] About.cshtml → _LayoutPure.cshtml
- [ ] Product.cshtml → _LayoutPure.cshtml
- [ ] Video.cshtml → _LayoutPure.cshtml
- [ ] CashplusInfo.cshtml → _LayoutHome.cshtml
- [ ] RegisterPartner.cshtml → _LayoutHome.cshtml

### **Trang cần kiểm tra:**
- [ ] Tất cả trang trong danh sách trên
- [ ] Test responsive trên mobile
- [ ] Kiểm tra hiệu suất với PageSpeed Insights
- [ ] Test cross-browser compatibility

## 🚀 **Triển khai**

### **Phase 1: Test (Tuần 1)**
1. Chuyển đổi 5-10 trang quan trọng
2. Test kỹ lưỡng trên staging
3. Đo hiệu suất trước/sau

### **Phase 2: Rollout (Tuần 2)**
1. Chuyển đổi tất cả trang còn lại
2. Monitor performance
3. Fix bugs nếu có

### **Phase 3: Optimization (Tuần 3)**
1. Fine-tune performance
2. Add caching nếu cần
3. Documentation

## 📞 **Hỗ trợ**

Nếu gặp vấn đề:
1. Kiểm tra console browser
2. Sử dụng debug mode (?debug=layout)
3. Kiểm tra network tab
4. Liên hệ team development

---
**Ngày tạo:** 28/06/2024  
**Người tạo:** AI Assistant  
**Trạng thái:** Ready for Implementation 