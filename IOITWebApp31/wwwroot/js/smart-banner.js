document.addEventListener('DOMContentLoaded', function() {
    const isAndroid = /Android/i.test(navigator.userAgent);

    if (isAndroid) {
        const banner = document.getElementById('android-smart-banner');
        if (!banner) return;

        const closeButton = document.getElementById('smart-banner-close');
        
        // Check if the banner was closed before in this session
        if (sessionStorage.getItem('smartBannerClosed') !== 'true') {
             // Use a small timeout to ensure the page has rendered before showing the banner
             setTimeout(function() {
                banner.classList.add('show');
             }, 500);
        }

        if(closeButton) {
            closeButton.addEventListener('click', function(e) {
                e.preventDefault();
                banner.classList.remove('show');
                // Remember that the banner was closed for this session
                sessionStorage.setItem('smartBannerClosed', 'true');
            });
        }
    }
}); 