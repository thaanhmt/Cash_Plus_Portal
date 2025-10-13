// Hero Slider Tesla Style - Pure CSS Transform với Fade In/Out mượt mà
// Đảm bảo DOM đã sẵn sàng

document.addEventListener('DOMContentLoaded', function() {
    const slides = document.querySelectorAll('.hero-slide');
    const dots = document.querySelectorAll('.hero-slide-dot');
    const prevBtn = document.querySelector('.hero-slider-arrow.left');
    const nextBtn = document.querySelector('.hero-slider-arrow.right');
    const slider = document.querySelector('.hero-slider');
    
    let currentSlide = 0;
    const slideInterval = 5000; // 5 seconds
    let slideTimer;
    let isTransitioning = false;
    
    // Đồng bộ với CSS transition duration
    let transitionDuration = 800; // Desktop
    if (window.innerWidth <= 600) {
        transitionDuration = 500; // Mobile nhỏ
    } else if (window.innerWidth <= 900) {
        transitionDuration = 600; // Mobile
    } else if (window.innerWidth <= 1200) {
        transitionDuration = 700; // Tablet
    }

    // Preload images để tránh giật
    function preloadImages() {
        slides.forEach(slide => {
            const bgImage = slide.style.backgroundImage;
            if (bgImage) {
                const url = bgImage.replace(/url\(['"]?(.*?)['"]?\)/i, '$1');
                const img = new Image();
                img.src = url;
            }
        });
    }

    // Tesla-style smooth transition với fade in/out mượt mà
    function showSlide(index) {
        if (isTransitioning || index === currentSlide) return;
        isTransitioning = true;

        const currentSlideElement = slides[currentSlide];
        const nextSlideElement = slides[index];

        // Fade out slide hiện tại
        currentSlideElement.classList.remove('active');
        
        // Fade in slide mới với delay nhỏ để tạo hiệu ứng crossfade
        setTimeout(() => {
            nextSlideElement.classList.add('active');
        }, 100);

        // Update dots với animation mượt
        dots.forEach((dot, i) => {
            dot.classList.toggle('active', i === index);
        });

        // Reset transition flag sau khi CSS transition hoàn thành
        setTimeout(() => {
            isTransitioning = false;
        }, transitionDuration + 200); // Buffer thêm 200ms để đảm bảo

        currentSlide = index;
    }

    function nextSlide() {
        let next = (currentSlide + 1) % slides.length;
        showSlide(next);
    }

    function prevSlide() {
        let prev = (currentSlide - 1 + slides.length) % slides.length;
        showSlide(prev);
    }

    // Event listeners với debounce và preventDefault
    dots.forEach((dot, i) => {
        dot.addEventListener('click', (e) => {
            e.preventDefault();
            e.stopPropagation();
            if (!isTransitioning) {
            showSlide(i);
            resetTimer();
            }
        });
    });

    if (prevBtn && nextBtn) {
        prevBtn.addEventListener('click', (e) => {
            e.preventDefault();
            e.stopPropagation();
            if (!isTransitioning) {
            prevSlide();
            resetTimer();
            }
        });
        nextBtn.addEventListener('click', (e) => {
            e.preventDefault();
            e.stopPropagation();
            if (!isTransitioning) {
            nextSlide();
            resetTimer();
            }
        });
    }

    // Auto slide với kiểm tra transition
    function startTimer() {
        slideTimer = setInterval(() => {
            if (!isTransitioning) {
                nextSlide();
            }
        }, slideInterval);
    }

    function resetTimer() {
        clearInterval(slideTimer);
        startTimer();
    }

    // Pause on hover
    if (slider) {
        slider.addEventListener('mouseenter', () => {
            clearInterval(slideTimer);
        });
        slider.addEventListener('mouseleave', () => {
            startTimer();
        });
    }

    // Touch/swipe support với improved performance và smooth transition
    let startX = 0;
    let startY = 0;
    let endX = 0;
    let endY = 0;
    let isSwiping = false;
    let touchStartTime = 0;
    let swipeThreshold = 50; // Khoảng cách tối thiểu để swipe
    let maxSwipeTime = 300; // Thời gian tối đa cho swipe

    if (slider) {
        slider.addEventListener('touchstart', (e) => {
            if (isTransitioning) return; // Không cho phép swipe khi đang transition
            
            startX = e.touches[0].clientX;
            startY = e.touches[0].clientY;
            touchStartTime = Date.now();
            isSwiping = true;
        }, { passive: true });

        slider.addEventListener('touchmove', (e) => {
            if (!isSwiping) return;
            
            endX = e.touches[0].clientX;
            endY = e.touches[0].clientY;
            
            // Prevent default để tránh scroll page
            const deltaX = Math.abs(endX - startX);
            const deltaY = Math.abs(endY - startY);
            
            if (deltaX > deltaY && deltaX > 10) {
                e.preventDefault();
            }
        }, { passive: false });

        slider.addEventListener('touchend', (e) => {
            if (!isSwiping || isTransitioning) {
                isSwiping = false;
                return;
            }
            
            const deltaX = endX - startX;
            const deltaY = endY - startY;
            const deltaTime = Date.now() - touchStartTime;
            const maxVerticalSwipe = 100;

            // Kiểm tra điều kiện swipe hợp lệ
            if (Math.abs(deltaX) > swipeThreshold && 
                Math.abs(deltaY) < maxVerticalSwipe && 
                deltaTime < maxSwipeTime) {
                
                if (deltaX > 0) {
                    // Swipe right - prev slide
                prevSlide();
                resetTimer();
                } else {
                    // Swipe left - next slide
                    nextSlide();
                    resetTimer();
                }
            }
            
            isSwiping = false;
        }, { passive: true });
    }

    // Keyboard navigation với preventDefault
    document.addEventListener('keydown', (e) => {
        if (isTransitioning) return;
        
        if (e.key === 'ArrowLeft') {
            e.preventDefault();
            prevSlide();
            resetTimer();
        } else if (e.key === 'ArrowRight') {
            e.preventDefault();
                nextSlide();
                resetTimer();
            }
        });

    // Initialize với delay để đảm bảo CSS đã load
    preloadImages();
    
    // Không cần thêm active cho slide đầu tiên vì HTML đã có
    // Chỉ cần đảm bảo dots được sync
    setTimeout(() => {
        if (slides.length > 0) {
            // Đảm bảo slide đầu tiên active và dots sync
            slides[0].classList.add('active');
            dots[0].classList.add('active');
            startTimer();
        }
    }, 500);

    // Căn chỉnh chiều cao Hero Slider để vừa vặn màn hình
    function setHeroHeight() {
        const header = document.querySelector('.cashplus-header');
        const heroSlider = document.querySelector('.hero-slider');
        if (header && heroSlider) {
            const headerHeight = header.offsetHeight;
            // Chỉ set height nếu header có chiều cao > 0 để tránh lỗi
            if (headerHeight > 0) {
                heroSlider.style.height = `calc(100vh - ${headerHeight}px)`;
            }
        }
    }

    // Chạy hàm khi DOM sẵn sàng, khi resize và quan trọng nhất là KHI TẢI XONG TOÀN BỘ TRANG
    setHeroHeight();
    window.addEventListener('resize', setHeroHeight);
    window.addEventListener('load', setHeroHeight); // Đảm bảo tính toán lại sau khi tất cả tài nguyên đã tải
}); // Kết thúc DOMContentLoaded

// AngularJS cho phần Tin tức CashPlus (tối ưu nhỏ gọn, không phụ thuộc)
if (window.angular) {
    angular.module('NewsApp', [])
        .controller('NewsController', function($scope, $http) {
            $scope.dataTinTuc = [];
            $http.get('/web/news/GetNews').then(function(response) {
                if (response.data && response.data.data) {
                    $scope.dataTinTuc = response.data.data;
                }
            });
        });
}

// --- Download App Page Custom JS (Dowloadnow.cshtml) ---
// Smart App Banner cho iOS
// <meta name="apple-itunes-app" content="app-id=6459993279"> (nên để trong <head> của layout)
// App Intent cho Android
if (/android/i.test(navigator.userAgent)) {
    var playUrl = "intent://cashplus.vn/#Intent;scheme=https;package=com.cashbackplus;end";
    if (window.navigator && window.navigator.standalone === false) {
        // Đã cài app, đề xuất mở app
        window.location = playUrl;
    }
}
// --- End Download App Page Custom JS --- 