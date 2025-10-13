# FAQ Component - Hướng dẫn sử dụng

## Tổng quan

FAQ Component đã được tối ưu hóa từ file gốc 85KB xuống còn khoảng 15KB, với cấu trúc modular và dễ bảo trì.

## Cấu trúc files

```
Views/Home/
├── FAQ.cshtml              # Trang FAQ chính
├── _FAQCategory.cshtml     # Component tái sử dụng
└── README_FAQ.md          # Hướng dẫn này

wwwroot/
├── css/
│   └── home.index.css     # CSS cho FAQ (đã thêm)
└── js/
    └── faq.js            # JavaScript cho FAQ
```

## Tính năng chính

### 1. Tìm kiếm thông minh
- Tìm kiếm real-time với debounce 300ms
- Highlight từ khóa tìm kiếm
- Tự động mở rộng câu hỏi có kết quả
- Hiển thị trạng thái "không tìm thấy"

### 2. Accordion tương tác
- Mở/đóng câu hỏi với animation mượt mà
- Chỉ mở một câu hỏi tại một thời điểm
- Keyboard navigation (Enter, Space, Escape)
- Accessibility support (ARIA attributes)

### 3. Responsive Design
- Tối ưu cho mobile, tablet, desktop
- Grid layout linh hoạt
- Typography scale phù hợp

## Cách sử dụng

### 1. Thêm FAQ Category mới

```csharp
@await Html.PartialAsync("_FAQCategory", new { 
    Title = "Tiêu đề nhóm câu hỏi",
    CategoryId = "unique-id",
    Questions = new[] {
        new { Id = 1, Question = "Câu hỏi?", Answer = "Trả lời..." },
        new { Id = 2, Question = "Câu hỏi khác?", Answer = "Trả lời khác..." }
    }
})
```

### 2. Tùy chỉnh CSS

Các class CSS chính:
- `.faq-search-wrapper` - Container tìm kiếm
- `.faq-category` - Container nhóm câu hỏi
- `.faq-item` - Item câu hỏi
- `.faq-question` - Phần câu hỏi
- `.faq-answer` - Phần trả lời

### 3. Tùy chỉnh JavaScript

File `faq.js` chứa class `FAQManager` với các method:
- `performSearch(query)` - Thực hiện tìm kiếm
- `toggleAnswer(element)` - Mở/đóng câu trả lời
- `highlightText(element, query)` - Highlight từ khóa

## Lợi ích của việc tối ưu

### 1. Hiệu suất
- Giảm 80% kích thước file (85KB → 15KB)
- Tách CSS/JS riêng biệt, có thể cache
- Lazy loading cho component

### 2. Bảo trì
- Code modular, dễ sửa đổi
- Tái sử dụng component
- Separation of concerns

### 3. UX/UI
- Giao diện hiện đại, responsive
- Animation mượt mà
- Accessibility tốt hơn
- Tìm kiếm thông minh

### 4. SEO
- Semantic HTML
- Structured data ready
- Fast loading

## Cấu hình

### 1. Layout
Sử dụng `_LayoutPure.cshtml` với CSS từ `home.index.css`

### 2. Dependencies
- Font Awesome 6.4.0
- jQuery (có sẵn trong layout)
- Bootstrap (có sẵn trong layout)

### 3. Browser Support
- Chrome 60+
- Firefox 55+
- Safari 12+
- Edge 79+

## Troubleshooting

### 1. JavaScript không hoạt động
- Kiểm tra console errors
- Đảm bảo file `faq.js` được load
- Kiểm tra jQuery dependency

### 2. CSS không áp dụng
- Kiểm tra file `home.index.css` được load
- Clear browser cache
- Kiểm tra CSS variables

### 3. Search không hoạt động
- Kiểm tra ID `faqSearch` và `clearSearch`
- Đảm bảo FAQ items có class `.faq-item`

## Migration từ version cũ

### 1. Backup
```bash
cp FAQ.cshtml FAQ_backup.cshtml
```

### 2. Thay thế
- Thay thế toàn bộ nội dung FAQ.cshtml
- Thêm CSS vào home.index.css
- Tạo file faq.js mới

### 3. Test
- Kiểm tra tìm kiếm
- Kiểm tra accordion
- Kiểm tra responsive
- Kiểm tra accessibility

## Performance Tips

### 1. Lazy Loading
```javascript
// Load FAQ chỉ khi cần
if (document.querySelector('.faq-category')) {
    new FAQManager();
}
```

### 2. Debounce Search
```javascript
// Đã implement sẵn 300ms debounce
```

### 3. CSS Optimization
```css
/* Sử dụng CSS variables cho consistency */
:root {
    --faq-primary: #393089;
    --faq-secondary: #f15725;
}
```

## Future Enhancements

### 1. Analytics
- Track search queries
- Track FAQ interactions
- A/B testing

### 2. Advanced Search
- Fuzzy search
- Search in answers
- Search suggestions

### 3. Content Management
- Dynamic FAQ loading
- Admin interface
- Multi-language support

## Support

Nếu có vấn đề, vui lòng:
1. Kiểm tra console errors
2. Test trên browser khác
3. Clear cache và reload
4. Liên hệ development team 