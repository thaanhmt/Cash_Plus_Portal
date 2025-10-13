import { Component, OnInit, ViewContainerRef, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../service/auth.service';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Md5 } from 'ts-md5/dist/md5';
import { CookieService } from 'ngx-cookie-service';
import { ToastrService } from 'ngx-toastr';
import { ModalDialogService, SimpleModalComponent } from 'ngx-modal-dialog';
import { AuthGuard } from '../../auth.guard';
import { DOCUMENT, Title } from "@angular/platform-browser";

const httpOptions = {
	headers: new HttpHeaders({
    'Content-Type': 'application/json'
  })
}

const md5 = new Md5();

const formData = new FormData();

@Component({
  selector: 'app-dashboard',
  templateUrl: 'login.component.html',
  styleUrls: ['login.component.css']
})
export class LoginComponent implements OnInit {
  public showPassword = false;
	loginForm: FormGroup;
	message: string;
	returnUrl: string;
  submitted: Boolean;

  constructor(

    @Inject(DOCUMENT) private document: any,
    private formBuilder: FormBuilder,
    private router: Router,
    public authService: AuthService,
    public http: HttpClient,
    private cookieService: CookieService,
    private toastr: ToastrService,
    private modalDialogService: ModalDialogService,
    private viewRef: ViewContainerRef,
    private authGuard: AuthGuard,
    private title: Title
  ) {  }
  elem;
  ngOnInit() {
    this.elem = document.documentElement;
    //this.cookieService.set('Expire', '');
    this.title.setTitle("Đăng nhập");
    this.submitted = false;
    this.loginForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
    this.returnUrl = '/dashboard';
    // if(this.authGuard.canActivate) {
    //   this.router.navigate([this.returnUrl]);
    // }
    //this.authService.logout();
  }

  get f() { return this.loginForm.controls };

  login() {
    this.submitted = false;
    if (this.loginForm.invalid) {
      this.submitted = true;
      return;
    }
    else {
      let email = this.f.username.value;
      let password = Md5.hashStr(this.f.password.value);

      this.http.post('/api/user/login', JSON.stringify({ email, password }), httpOptions).subscribe(
        (res) => {
          let data = JSON.stringify(res);
          if (res["meta"]["error_code"] == 200) {
            this.cookieService.set('accessToken', res["data"]["access_token"].toString(), 2147483647, '/');
            this.cookieService.set('Expire', Date.now().toLocaleString(), 0.1);

            localStorage.setItem('isLoggedIn', "true");
            localStorage.setItem('data', res.toString());
            localStorage.setItem('access_token', res["data"]["access_token"].toString());
            localStorage.setItem('access_key', res["data"]["access_key"].toString());
            localStorage.setItem('userId', res["data"]["userId"].toString());
            localStorage.setItem('userMapId', res["data"]["userMapId"].toString());
            localStorage.setItem('userName', res["data"]["userName"].toString());
            localStorage.setItem('avata', res["data"]["avata"] != undefined ? res["data"]["avata"].toString() : undefined);
            localStorage.setItem('fullName', res["data"]["fullName"] != undefined ? res["data"]["fullName"].toString() : undefined);
            localStorage.setItem('companyId', res["data"]["companyId"] != undefined ? res["data"]["companyId"].toString() : undefined);
            localStorage.setItem('languageId', res["data"]["languageId"] ? res["data"]["languageId"].toString() : undefined);
            localStorage.setItem('languageCode', res["data"]["languageCode"] ? res["data"]["languageCode"].toString() : undefined);
            localStorage.setItem('websiteId', res["data"]["websiteId"] != undefined ? res["data"]["websiteId"].toString() : undefined);
            localStorage.setItem('roleCode', res["data"]["roleCode"] != undefined ? res["data"]["roleCode"].toString() : undefined);
            localStorage.setItem('roleName', res["data"]["roleName"] != undefined ? res["data"]["roleName"].toString() : undefined);
            localStorage.setItem('menu', JSON.stringify(res["data"]["listMenus"]));
            this.cookieService.set('Expire', Date.now().toLocaleString(), 0.1);

            this.router.navigate([this.returnUrl]);
          }
          else {
            this.submitted = true;
            this.message = "Tài khoản hoặc mật khẩu không đúng";
            this.router.navigate(['/login']);
          }
        },
        (err) => {
          this.showConfirm("Đăng nhập không thành công. Xin vui lòng thử lại sau!");
        }
      );
    }
  }


  toat(): void {
    this.toastr.success('Website hiện tạm đóng tạo tài khoản', 'Thông báo');
  }
  toatCommingSoon(): void {
    this.toastr.warning('Chức năng tạm đóng', 'Comming Soon');
  }
  showConfirm(message) {
    this.modalDialogService.openDialog(this.viewRef, {
      title: 'Thông báo',
      childComponent: SimpleModalComponent,
      data: {
        text: message
      },
      actionButtons: [
        {
          text: 'Xác nhận',
          buttonClass: 'btn btn-success'
        }
      ],
    });
  }
  openToggleShowPassWord() {
    this.showPassword = !this.showPassword;
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
}
