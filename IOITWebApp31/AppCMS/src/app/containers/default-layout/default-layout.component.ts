import { Component, Input, ViewChild, ElementRef, Inject } from '@angular/core';
import { navItems, domainImage, domain, domainVideos, domainMedia, domainDebug } from './../../data/const';
import { AuthService } from '../../service/auth.service';
import { CookieService } from 'ngx-cookie-service';
import { UserChangePass } from './../../data/model';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { HttpClient, HttpHeaders, HttpRequest, HttpEventType } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { Md5 } from 'ts-md5/dist/md5';
import { User } from '../../data/model';
import { DOCUMENT} from "@angular/platform-browser";
import { Paging } from '../../data/dt';

@Component({
  selector: 'app-dashboard',
  templateUrl: './default-layout.component.html',
  styleUrls: ['./default-layout.component.css']
})
export class DefaultLayoutComponent {
  public listNotification = [];
  public navItem = [];
  public sidebarMinimized = true;
  private changes: MutationObserver;
  /*public element: HTMLElement = document.body;*/
  public userChangePass: UserChangePass = new UserChangePass();
  public domainImage = domainImage;
  public domainMedia = domainMedia;
  public Item: User;
  public ClassTheme: string = localStorage.getItem("ThemeStyle") || '';
  public activeSide: string = localStorage.getItem("Morong") || 'non-active';
  public ChucVu: string = localStorage.getItem("roleName") || '';
  isDark: boolean = localStorage.getItem("ThemeStyle") == "dark" ? true : false;
  isMorong: boolean = localStorage.getItem("Morong") == "active" ? true : false;
  public stick: boolean = false;
  public httpOptions: any;
  scroll: boolean = false;
  date: any;
  seconds: any;
  getDate: any;
  getDay: any;
  timeDay: any;
  getMonth: any;
  getFullYear: any;
  nameDay: string;
  currentLocale: any;
  public paging: Paging;

  @ViewChild('ChangePasswordModal') public changePasswordModal: ModalDirective;
  @ViewChild('modalMyInfo') public modalMyInfo: ModalDirective;
  @ViewChild('supportModal') public supportModal: ModalDirective;
  @ViewChild('file') file: ElementRef;
  @ViewChild('OpenMediaFile') public OpenMediaFile: ModalDirective;

  public isActiveMedia: boolean = true;
  public isActiveUpload: boolean = false;
  public isDelay: boolean = false;

  public domain: string;
  public pagingFile: Paging;
  public countMedia: number;
  public countAllMedia: number;

  public listItemMedia = [];
  public message: string;
  public progress: number;

  public staticDomain: string;
  public staticDomainMedia: string;
  public domainDebug = domainDebug;
  constructor(
    public auth: AuthService,
    public cookie: CookieService,
    public toastr: ToastrService,
    public http: HttpClient,
    @Inject(DOCUMENT) private document: any,
  ) {

    this.paging = new Paging();
    this.paging.page = 1;
    this.paging.page_size = 200;
    this.paging.query = "1=1";
    this.paging.order_by = "";
    this.paging.item_count = 0;

    this.pagingFile = new Paging();
    this.pagingFile.page = 1;
    this.pagingFile.page_size = 24;
    this.pagingFile.query = "1=1";
    this.pagingFile.order_by = "";
    this.pagingFile.item_count = 0;
    this.countMedia = 24;
    
    var json = JSON.parse(localStorage.getItem('menu'));
    this.Item = new User();

    this.navItem.push({
      icon: "fa-light fa-gauge",
      name: "Trang tổng quan",
      url: "/dashboard"
    });

    for (var i = 0; i < json.length; i++) {
      this.navItem.push(this.createMenu(json[i], undefined));
    }

    this.changes = new MutationObserver((mutations) => {
      this.sidebarMinimized = document.body.classList.contains('sidebar-minimized');
    });

    //this.changes.observe(<Element>this.element, {
    //  attributes: true
    //});

    this.myFuntion();

    this.userChangePass.UserId = parseInt(localStorage.getItem("userId"));
    this.userChangePass.UserName = localStorage.getItem("userName");
    this.userChangePass.Avatar = localStorage.getItem("avata");
    this.userChangePass.FullName = localStorage.getItem("fullName");
    this.httpOptions = {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      })
    }

    setInterval(() => {
      const currentDate = new Date();
      this.date = currentDate.toLocaleTimeString('en-US', { hour: '2-digit', hour12: true, minute: '2-digit' }).replace("AM", "SA").replace("PM", "CH");
    }, 30000);

    this.getDate = new Date().getDate();
    this.getDay = new Date().getDay();
    this.getMonth = new Date().getUTCMonth() + 1;
    this.getFullYear = new Date().getFullYear();
    if (this.getDay == 0) {
      this.nameDay = 'Chủ nhật';
    } else if (this.getDay == 1) {
      this.nameDay = 'Thứ hai';
    } else if (this.getDay == 2) {
      this.nameDay = 'Thứ ba';
    } else if (this.getDay == 3) {
      this.nameDay = 'Thứ tư';
    } else if (this.getDay == 4) {
      this.nameDay = 'Thứ năm';
    } else if (this.getDay == 5) {
      this.nameDay = 'Thứ sáu';
    } else if (this.getDay == 6) {
      this.nameDay = 'Thứ bảy';
    }
  }
  elem;
  ngOnInit() {
    this.ChucVu = localStorage.getItem("roleName");
    const currentDate = new Date();
    this.date = currentDate.toLocaleTimeString('en-US', { hour: '2-digit', hour12: true, minute: '2-digit' }).replace("AM", "SA").replace("PM", "CH");
    this.elem = document.documentElement;
    window.addEventListener('scroll', this.scrolling, true);
    //get ds thông báo
    let userId = localStorage.getItem("userId");
    let userMapId = localStorage.getItem("userMapId");
    this.paging.query = "1=1 And UserReadId=" + userMapId;
    this.GetListNotification();
    this.GetListFiles();
    this.GetDomainStatic();
  }
  GetDomainStatic() {
    this.http.get('api/Config/1', this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.staticDomain = res["data"].Website;
          if (res["data"].ModeSite) {
            this.staticDomainMedia = this.domainDebug + 'uploads';
            this.staticDomain = this.domainDebug;
          } else {
            this.staticDomainMedia = this.staticDomain + 'uploads';
            this.staticDomain = res["data"].Website;
          }
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }
  GetListNotification() {
    this.http.get('/api/notification/GetByPage?page=' + this.paging.page + '&page_size=' + this.paging.page_size + '&query=' + this.paging.query + '&order_by=' + this.paging.order_by, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.listNotification = res["data"];
          this.paging.item_count = res["metadata"].Sum;
        }
      },
      (err) => {
        console.log("Error: connect to API");
      }
    );
  }

  scrolling = (s) => {
    let sc = s.target.scrollingElement.scrollTop;
    if (sc >= 25) {
      this.stick = true;
    }
    else {
      this.stick = false;
    }
  }
  
  createMenu(item, urlParent: string) {
    item["name"] = item["Name"];
    item["url"] = urlParent == undefined ? "/" + item["Url"] : urlParent + "/" + item["Url"];
    item["icon"] = item["Icon"];

    delete item["MenuId"];
    delete item["Code"];
    delete item["Name"];
    delete item["MenuParent"];
    delete item["Url"];
    delete item["Icon"];
    delete item["ActiveKey"];
    delete item["Status"];

    if (item["listMenus"].length > 0) {
      item["children"] = [];
      for (var i = 0; i < item["listMenus"].length; i++) {
        item["children"].push(item["listMenus"][i]);
        this.createMenu(item["children"][i], item["url"]);
      }
    }

    delete item["listMenus"];

    return item;
  }

  logout() {
    this.auth.logout();
  }

  myFuntion() {
    // debugger;
    setInterval(() => {
      if (this.cookie.get("Expire") == '' || this.cookie.get("Expire") == undefined || localStorage.getItem('isLoggedIn') != "true") {
        this.auth.logout();
      }
    }, 10000);
  }

  OpenChangePasswordModal() {
    this.userChangePass.PasswordOldE = undefined;
    this.userChangePass.PasswordNewE = undefined;
    this.userChangePass.ConfirmPassword = undefined;
    this.changePasswordModal.show();
  }

  ChangePassword() {
    if (this.userChangePass.PasswordOldE == undefined || this.userChangePass.PasswordOldE == '') {
      this.toastWarning("Chưa nhập Mật khẩu hiện tại!");
      return;
    } else if (this.userChangePass.PasswordNewE == undefined || this.userChangePass.PasswordNewE == '') {
      this.toastWarning("Chưa nhập Mật khẩu mới!");
      return;
    } else if (this.userChangePass.ConfirmPassword == undefined || this.userChangePass.ConfirmPassword == '') {
      this.toastWarning("Chưa nhập Mật khẩu xác nhận!");
      return;
    } else if (this.userChangePass.ConfirmPassword != this.userChangePass.PasswordNewE) {
      this.toastWarning("Mật khẩu xác nhận không đúng!");
      return;
    }

    this.userChangePass.PasswordOld = Md5.hashStr(this.userChangePass.PasswordOldE).toString();
    this.userChangePass.PasswordNew = Md5.hashStr(this.userChangePass.PasswordNewE).toString();

    this.http.put('/api/user/changePass/' + this.userChangePass.UserId, this.userChangePass, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.changePasswordModal.hide();
          this.toastSuccess("Đổi mật khẩu tài khoản thành công!");
        } else if (res["meta"]["error_code"] == 213) {
          this.toastError("Mật khẩu hiện tại không đúng. Vui lòng thử lại!");
        }
        else {
          this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Vui lòng thử lại!");
      }
    );
  }

  //Thông báo
  toastWarning(msg): void {
    this.toastr.warning(msg, 'Cảnh báo');
  }

  toastSuccess(msg): void {
    this.toastr.success(msg, 'Hoàn thành');
  }

  toastError(msg): void {
    this.toastr.error(msg, 'Lỗi');
  }

  OpenModalInfo() {
    this.Item = new User();
    this.file.nativeElement.value = "";

    this.http.get('/api/user/infoUser/' + this.userChangePass.UserId, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.Item = res["data"];
        }
      },
      (err) => {
        console.log("Error: connect to API");
      });
    this.modalMyInfo.show();
  }
  

  upload(files) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);

    const uploadReq = new HttpRequest('POST', 'api/upload/uploadImage/6', formData, {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
      reportProgress: true,
    });

    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress) {
      }
      else if (event.type === HttpEventType.Response) {
        this.Item.Avata = event.body["data"].toString();
      }
    });
  }

  SaveInfo() {
    if (this.Item.FullName == undefined || this.Item.FullName == '') {
      this.toastWarning("Chưa nhập Tên người dùng!");
      return;
    } else if (this.Item.UserName == undefined || this.Item.UserName == '') {
      this.toastWarning("Chưa nhập Tên tài khoản!");
      return;
    }

    this.http.put('/api/user/changeInfoUser', this.Item, this.httpOptions).subscribe(
      (res) => {
        if (res["meta"]["error_code"] == 200) {
          this.modalMyInfo.hide();
          this.toastSuccess("Lưu thông tin thành công!");
        }
        else {
          this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
        }
      },
      (err) => {
        this.toastError("Đã xảy ra lỗi. Xin vui lòng thử lại sau!");
      });
  }
  openFullscreen() {
    if (this.elem.requestFullscreen) {
      this.elem.requestFullscreen();
    } else if (this.elem.mozRequestFullScreen) {
      /* Firefox */
      this.elem.mozRequestFullScreen();
    } else if (this.elem.webkitRequestFullscreen) {
      /* Chrome, Safari and Opera */
      this.elem.webkitRequestFullscreen();
    } else if (this.elem.msRequestFullscreen) {
      /* IE/Edge */
      this.elem.msRequestFullscreen();
    }
    if (this.document.exitFullscreen) {
      this.document.exitFullscreen();
    } else if (this.document.mozCancelFullScreen) {
      /* Firefox */
      this.document.mozCancelFullScreen();
    } else if (this.document.webkitExitFullscreen) {
      /* Chrome, Safari and Opera */
      this.document.webkitExitFullscreen();
    } else if (this.document.msExitFullscreen) {
      /* IE/Edge */
      this.document.msExitFullscreen();
    }
  }
  changeDrankToggle() {
    if (this.isDark == undefined) {
      this.isDark = false;
    }
    this.isDark = !this.isDark;
    if (this.isDark == true) {
      localStorage.setItem('ThemeStyle', 'dark');
    } else {
      localStorage.setItem('ThemeStyle', '');
    }
    this.ClassTheme = localStorage.getItem("ThemeStyle");
  }
  OpenModleSupport() {
    if (this.isMorong == undefined) {
      this.isMorong = false;
    }
    this.isMorong = !this.isMorong;
    if (this.isMorong == true) {
      localStorage.setItem('Morong', 'active');
    } else {
      localStorage.setItem('Morong', 'non-active');
    }
    this.activeSide = localStorage.getItem("Morong");
  }
  messageToast() {
    this.toastr.warning('Hãy quay lại sau!', 'Thông báo');
  }

  OpenMediaModal() {
    this.OpenMediaFile.show();
  }
  CloseMediaModal() {
    this.OpenMediaFile.hide();
  }
  tabHandleMedia() {
    this.isActiveMedia = true;
    this.isActiveUpload = false;
  }
  tabHandleMediaUpload() {
    this.isActiveMedia = false;
    this.isActiveUpload = true;
  }
  upload3(files, cs) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);

    const uploadReq = new HttpRequest('POST', 'api/upload/uploadMedia/8', formData, {
      headers: new HttpHeaders({
        'Authorization': 'bearer ' + localStorage.getItem("access_token")
      }),
      reportProgress: true,
    });

    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress)
        switch (cs) {
          case 1:
            this.progress = Math.round(100 * event.loaded / event.total);
            break;
          default:
            break;
        }
      else if (event.type === HttpEventType.Response) {

        switch (cs) {
          case 1:
            this.isActiveMedia = true;
            this.isActiveUpload = false;
            this.pagingFile.page = 1;
            this.GetListFiles();
            this.message = event.body["data"].toString();
            this.toastSuccess("Tải lên thành công");
            break;
          default:
            break;
        }
      }
    });
  }
  loadMore() {
    this.isDelay = true;
    setTimeout(
      () => {
        this.isDelay = false;
        this.pagingFile.page++;
        this.http.get('/api/fileManager/GetFiles?page=' + this.pagingFile.page + '&page_size=' + this.pagingFile.page_size + '&query='
          + this.pagingFile.query + '&order_by=' + '&select=' + this.pagingFile.select, this.httpOptions).subscribe(
            (res) => {
              if (res["meta"]["error_code"] == 200) {
                this.listItemMedia.push(...res["data"]);
                if (this.countMedia >= this.countAllMedia) {
                  this.countMedia = this.countAllMedia;
                } else {
                  if ((this.countMedia + 24) >= this.countAllMedia) {
                    this.countMedia = this.countAllMedia;
                  } else {
                    this.countMedia += 24;
                  }
                }
              }
            },
            (err) => {
              console.log("Error: connect to API");
            }
          );
      },
      1000
    );
  }

  GetListFiles() {
    this.http.get('/api/fileManager/GetFiles?page=' + this.pagingFile.page + '&page_size=' + this.pagingFile.page_size + '&query='
      + this.pagingFile.query + '&order_by=' + '&select=' + this.pagingFile.select, this.httpOptions).subscribe(
        (res) => {
          if (res["meta"]["error_code"] == 200) {
            this.listItemMedia = res["data"];
            this.countAllMedia = res["metadata"];
            if (this.countAllMedia < 24) this.countMedia = this.countAllMedia;
          }
        },
        (err) => {
          console.log("Error: connect to API");
        }
      );
  }
  RemoveImage() {
    this.Item.Avata = undefined;
    this.file.nativeElement.value = "";
    this.message = undefined;
    this.progress = undefined;
  }
  SeclectMedia(item) {
    this.Item.Avata = item.url + "/" + item.name;
    this.OpenMediaFile.hide();
  }
}
