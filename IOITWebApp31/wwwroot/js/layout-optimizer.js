/**
 * Layout Optimizer - Tự động chuyển đổi layout theo nhu cầu AngularJS
 * Tối ưu hiệu suất trang web bằng cách chỉ load AngularJS khi cần thiết
 */

(function() {
    'use strict';
    
    // Danh sách các trang CẦN AngularJS
    const ANGULAR_PAGES = [
        // Trang chủ (section tin tức)
        '/home/index',
        '/',
        
        // Form đăng ký/đăng nhập
        '/customer/cashplusinfo',
        '/customer/registerpartner',
        '/customer/registerinfo',
        '/customer/login',
        '/customer/changepass',
        '/customer/recoverpassotp',
        '/customer/recoversecurityotp',
        '/customer/recoverchangesecurityotp',
        
        // Form thông tin cá nhân
        '/customer/infouserpage',
        '/customer/infouser',
        '/customer/infodetail',
        
        // Quản lý
        '/customer/manageunit',
        '/customer/manageuser',
        '/customer/listregister',
        '/customer/managelink',
        '/customer/detailcashplus',
        
        // Cập nhật đối tác
        '/customer/partnerupdate',
        
        // Tin tức (chi tiết)
        '/customer/news',
        '/customer/privacypolicy'
    ];
    
    // Danh sách các trang KHÔNG CẦN AngularJS
    const PURE_PAGES = [
        // Chi tiết sản phẩm
        '/detail/product',
        '/detail/video',
        '/detail/image',
        '/detail/attactment',
        '/detail/detaillegaldoc',
        
        // Danh mục
        '/category/product',
        '/category/solutionsservices',
        '/category/video',
        '/category/timeline',
        '/category/productchild',
        '/category/pageparent',
        '/category/notification',
        '/category/listvideo',
        '/category/listgallery',
        '/category/listallproduct',
        '/category/introduce',
        '/category/image',
        
        // Giới thiệu
        '/home/about',
        
        // Tải xuống
        '/customer/dowloadnow',
        '/customer/dowloadmernow',
        
        // Tin tức
        '/preview/news',
        '/detail/notificaton'
    ];
    
    /**
     * Kiểm tra xem trang hiện tại có cần AngularJS không
     */
    function needsAngularJS() {
        const currentPath = window.location.pathname.toLowerCase();
        
        // Kiểm tra trong danh sách trang cần AngularJS
        for (let page of ANGULAR_PAGES) {
            if (currentPath.includes(page.toLowerCase())) {
                return true;
            }
        }
        
        // Kiểm tra trong danh sách trang không cần AngularJS
        for (let page of PURE_PAGES) {
            if (currentPath.includes(page.toLowerCase())) {
                return false;
            }
        }
        
        // Mặc định: không cần AngularJS
        return false;
    }
    
    /**
     * Chuyển đổi layout dựa trên nhu cầu AngularJS
     */
    function optimizeLayout() {
        const needsAngular = needsAngularJS();
        const currentLayout = document.querySelector('html').getAttribute('ng-app');
        
        console.log('Layout Optimizer:', {
            currentPath: window.location.pathname,
            needsAngular: needsAngular,
            currentLayout: currentLayout ? 'Angular' : 'Pure'
        });
        
        // Nếu trang cần AngularJS nhưng đang dùng layout Pure
        if (needsAngular && !currentLayout) {
            console.warn('Trang này cần AngularJS nhưng đang dùng layout Pure!');
            // Có thể reload với layout AngularJS
            // window.location.reload();
        }
        
        // Nếu trang không cần AngularJS nhưng đang dùng layout AngularJS
        if (!needsAngular && currentLayout) {
            console.warn('Trang này không cần AngularJS nhưng đang dùng layout AngularJS!');
            // Có thể reload với layout Pure
            // window.location.reload();
        }
    }
    
    /**
     * Tối ưu hiệu suất cho trang không cần AngularJS
     */
    function optimizePerformance() {
        if (!needsAngularJS()) {
            // Loại bỏ các script AngularJS không cần thiết
            const angularScripts = document.querySelectorAll('script[src*="angular"]');
            angularScripts.forEach(script => {
                if (script.src.includes('angular')) {
                    console.log('Removing AngularJS script:', script.src);
                    script.remove();
                }
            });
            
            // Loại bỏ các CSS AngularJS không cần thiết
            const angularStyles = document.querySelectorAll('link[href*="angular"]');
            angularStyles.forEach(style => {
                if (style.href.includes('angular')) {
                    console.log('Removing AngularJS CSS:', style.href);
                    style.remove();
                }
            });
        }
    }
    
    /**
     * Thêm thông tin debug
     */
    function addDebugInfo() {
        if (window.location.search.includes('debug=layout')) {
            const debugInfo = document.createElement('div');
            debugInfo.style.cssText = `
                position: fixed;
                top: 10px;
                right: 10px;
                background: rgba(0,0,0,0.8);
                color: white;
                padding: 10px;
                border-radius: 5px;
                font-family: monospace;
                font-size: 12px;
                z-index: 9999;
            `;
            debugInfo.innerHTML = `
                <strong>Layout Debug:</strong><br>
                Path: ${window.location.pathname}<br>
                Needs Angular: ${needsAngularJS()}<br>
                Current Layout: ${document.querySelector('html').getAttribute('ng-app') ? 'Angular' : 'Pure'}<br>
                Scripts: ${document.querySelectorAll('script').length}<br>
                Styles: ${document.querySelectorAll('link[rel="stylesheet"]').length}
            `;
            document.body.appendChild(debugInfo);
        }
    }
    
    // Khởi tạo khi DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function() {
            optimizeLayout();
            optimizePerformance();
            addDebugInfo();
        });
    } else {
        optimizeLayout();
        optimizePerformance();
        addDebugInfo();
    }
    
    // Export cho sử dụng global
    window.LayoutOptimizer = {
        needsAngularJS: needsAngularJS,
        optimizeLayout: optimizeLayout,
        optimizePerformance: optimizePerformance
    };
    
})(); 