// Hiệu ứng mở/đóng menu mobile cho header CashPlus

document.addEventListener('DOMContentLoaded', function() {
  var menuToggle = document.querySelector('.cashplus-menu-toggle');
  var menuMobile = document.querySelector('.cashplus-menu-mobile');
  var menuClose = document.querySelector('.cashplus-menu-close');

  // Overlay menu mobile
  var overlay = document.querySelector('.cashplus-menu-overlay');
  var menuMobileParent = menuMobile && menuMobile.parentNode;
  if (!overlay) {
    overlay = document.createElement('div');
    overlay.className = 'cashplus-menu-overlay';
    if (menuMobileParent && menuMobile) {
      menuMobileParent.insertBefore(overlay, menuMobile);
    } else {
      document.body.appendChild(overlay);
    }
  }

  function openMenuMobile() {
    menuMobile.classList.add('open');
    overlay.classList.add('open');
    document.body.style.overflow = 'hidden';
  }
  function closeMenuMobile() {
    menuMobile.classList.remove('open');
    overlay.classList.remove('open');
    document.body.style.overflow = '';
  }
  if (menuToggle && menuMobile) {
    menuToggle.addEventListener('click', openMenuMobile);
  }
  if (menuClose && menuMobile) {
    menuClose.addEventListener('click', closeMenuMobile);
  }
  overlay.addEventListener('click', closeMenuMobile);

  // Đóng menu khi click ra ngoài menu mobile
  menuMobile && menuMobile.addEventListener('click', function(e) {
    if (e.target === menuMobile) {
      menuMobile.classList.remove('open');
      document.body.style.overflow = '';
    }
  });

  // Fix menu ngang tablet không bị xuống dòng
  var cashplusMenuUl = document.querySelector('.cashplus-menu ul');
  if (cashplusMenuUl) {
    cashplusMenuUl.style.flexWrap = 'nowrap';
    cashplusMenuUl.style.overflowX = 'auto';
  }
});

document.addEventListener('scroll', function() {
  var header = document.querySelector('.cashplus-header');
  if (!header) return;
  if (window.scrollY > 0) {
    header.classList.add('header-transparent');
  } else {
    header.classList.remove('header-transparent');
  }
}); 