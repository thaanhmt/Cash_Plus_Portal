jQuery(document).ready(function ($) {
    $(document).on('ready', function () {
        $('.slide-home').slick({
            dots: false,
            infinite: true,
            speed: 1000,
            slidesToScroll: 1,
            slidesToShow: 3,
            prevArrow: '<i class="btn-prev"></i>',
            nextArrow: '<i class="btn-next"></i>',
			swipeToSlide: true,
        });
        $('.main-app').slick({
            dots: false,
            infinite: true,
            speed: 300,
            slidesToScroll: 1,
            slidesToShow: 5,
            autoplay: false,
            autoplaySpeed: 3000,
            prevArrow: '<i class="btn-prev-2"></i>',
            nextArrow: '<i class="btn-next-2"></i>',
			swipeToSlide: true,
            responsive: [
				{
					breakpoint: 1024,
					settings: {
						slidesToShow: 4,
						slidesToScroll: 1,
						infinite: true,
						}
				},
				{
					breakpoint: 600,
					settings: {
						slidesToShow: 3,
						slidesToScroll: 1
					}
				},
				{
					breakpoint: 480,
					settings: {
						slidesToShow: 2,
						slidesToScroll: 1
					}
				}
			]
        });
		$('.main-data').slick({
            dots: false,
            infinite: true,
            speed: 300,
            slidesToScroll: 1,
            slidesToShow: 9,
            autoplay: false,
            autoplaySpeed: 3000,
            prevArrow: '<i class="btn-prev-2"></i>',
            nextArrow: '<i class="btn-next-2"></i>',
			swipeToSlide: true,
            responsive: [
				{
					breakpoint: 1024,
					settings: {
						slidesToShow: 7,
						slidesToScroll: 1,
						infinite: true,
						}
				},
				{
					breakpoint: 850,
					settings: {
						slidesToShow: 5,
						slidesToScroll: 1
					}
				},
				{
					breakpoint: 600,
					settings: {
						slidesToShow: 4,
						slidesToScroll: 1
					}
				},
				{
					breakpoint: 480,
					settings: {
						slidesToShow: 3,
						slidesToScroll: 1
					}
				}
			]
        });
        $('.main-partner').slick({
            dots: false,
            infinite: true,
            speed: 300,
            slidesToScroll: 1,
            slidesToShow: 5,
            autoplay: true,
            autoplaySpeed: 3000,
            prevArrow: '<i class="btn-prev-3"></i>',
            nextArrow: '<i class="btn-next-3"></i>',
			swipeToSlide: true,
            responsive: [
				{
					breakpoint: 1024,
					settings: {
						slidesToShow: 5,
						slidesToScroll: 1,
						infinite: true,
						}
				},
				{
					breakpoint: 600,
					settings: {
						slidesToShow: 3,
						slidesToScroll: 1
					}
				},
				{
					breakpoint: 480,
					settings: {
						slidesToShow: 2,
						slidesToScroll: 1
					}
				}
			]
        });
    });
});