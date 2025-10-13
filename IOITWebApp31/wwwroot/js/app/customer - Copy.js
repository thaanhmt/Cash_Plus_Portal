
myApp.controller('CustomerController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', 'vcRecaptchaService', function CustomerController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app, vcRecaptchaService) {
    $scope.page = 1;
    $scope.page_size = 9;
    $scope.query = "1=1";
    $scope.q = {};
    $scope.EditCustomer = {};
    $scope.password = {};
    $scope.orderby = "";
    $scope.item_count = 0;
    $scope.disableBtn = { btRegister: false, btLogin: false, btResetPass: false, submitRecover: false };
    $scope.regexEmail = config.regexEmail;
    $scope.regexPhone = config.regexPhone;

    //var vm = this;
    $scope.publicKey = "6Ld_EMEfAAAAAJBZTDIdpXims5GZHQBRLhc0XErX";

    $scope.init = function (data, url) {
        $scope.isLogin = false;
        $scope.EditCustomer = {};
        $scope.password = {};
        //$scope.LanguageId = LanguageId + "";
        $scope.customer = {};
        if (data != undefined) {
            $scope.customerId = data.CustomerId;
            $scope.customer.Email = data.Email;
            //$scope.EditCustomer.FullName = data.FullName;
            $scope.EditCustomer.Avata = data.Avata;
            //$scope.EditCustomer.Address = data.Address;
            //$scope.EditCustomer.PhomeNumber = data.PhomeNumber;
            //$scope.EditCustomer.Sex = data.Sex;
            $scope.access_token = data.access_token;

        }
        $scope.loadUnit();
        //$scope.StatutOrId = 1;
        //$scope.CustomerListOrder();
        //$scope.ChangeStatutsOrder($scope.StatutOrId);

        //$scope.listOrder = JSON.parse($window.localStorage.getItem("Order"));
        //console.log($scope.customerId);

        //var url_string = window.location.href;
        $scope.url = "/";
        if (url != undefined && url != null)
            $scope.url = url + "";

        $scope.register = {};
        $scope.login = {};
        $scope.resetPass = {};
        $scope.reset = false;

    };

    $scope.loadUnit = function () {
        $http.get("/web/unit/listUnitPublish", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listUnits = data.data.data;
            }
        });
    };

    $scope.showResetPass = function () {
        $scope.reset = !$scope.reset;
    };

    $scope.submitLogin = function () {
        var email = angular.element(document.querySelector('#txtLoginEmail')).val();
        var password = angular.element(document.querySelector('#txtLoginPassword')).val();
        if (email === '' || email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập email đăng nhập!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("txtLoginEmail");
            });

            return;
        }

        if (password === '' || password === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mật khẩu!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("txtLoginPassword");
            });
            return;
        }

        if (vcRecaptchaService.getResponse() === "") {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông tin')
                    .textContent('Chưa xác thực người dùng!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
        }
        else {

            $scope.login = {
                "email": email,
                "password": password
            };
            $scope.disableBtn.btLogin = true;
            cfpLoadingBar.start();

            var post = $http({
                method: 'POST',
                url: '/web/customer/login',
                data: $scope.login,
                headers: {}
            });

            post.success(function successCallback(data, status, headers, config) {
                cfpLoadingBar.complete();
                $scope.disableBtn.btLogin = false;
                if (data.meta.error_code === 200) {
                    cfpLoadingBar.complete();
                    $scope.isLogin = true;
                    $window.location.href = $scope.url;
                    $scope.login = {};
                }
                else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông báo')
                            .textContent(data.meta.error_message)
                            .ok('Đóng')
                            .fullscreen(true)
                    );
                    return;
                }
            }).error(function (data, status, headers, config) {
                $scope.disableBtn.btLogin = false;
                cfpLoadingBar.complete();
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi! Xin vui lòng thử lại sau.')
                        .ok('Đóng')
                        .fullscreen(true)
                );
                return;
            });
        }
    };

    $scope.submitRecover = function () {
        var email = angular.element(document.querySelector('#txtEmailRecover')).val();
        if (email === '' || email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn vui lòng nhập email để lấy lại mật khẩu!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        }

        let obj = {
            "email": email
        };

        $scope.disableBtn.submitRecover = true;
        cfpLoadingBar.start();

        var post = $http({
            method: 'POST',
            url: '/web/customer/RecoverPasssword',
            data: obj,
            headers: {}
        });

        post.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            $scope.disableBtn.submitRecover = false;
            if (data.meta.error_code === 200) {
                $window.location.href = '/thankyou';
                //    var confirm = $mdDialog.confirm()
                //        .title('Thông báo')
                //        .textContent(data.meta.error_message)
                //        .ok('Về trang chủ')
                //        .cancel('Đóng');

                //    $mdDialog.show(confirm).then(function () {
                //        $scope.goHome();
                //    });
            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(true)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.submitRecover = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
                    .ok('Đóng')
                    .fullscreen(true)
            );
        });
    };

    $scope.focusElement = function (id) {
        document.getElementById(id).focus();
    };

    $scope.changeValue = function (object, type) {
        switch (type) {
            case 1:
                $scope.register.UnitId = object ? object.UnitId : null;
                break;
            default:
                break;
        }
    }

    $scope.RegisterMember = function (type) {

        if ($scope.register.Email === '' || $scope.register.Email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập email!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("EmailDk");
                        break;
                    case 2:
                        $scope.focusElement("EmailMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if (!$scope.checkEmailFormat($scope.register.Email)) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Email đã nhập chưa chính xác!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("EmailDk");
                        break;
                    case 2:
                        $scope.focusElement("EmailMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.register.FullName === '' || $scope.register.FullName === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập Họ và tên!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("FullNameDk");
                        break;
                    case 2:
                        $scope.focusElement("FullNameMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.register.Password === '' || $scope.register.Password === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mật khẩu!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("PasswordDk");
                        break;
                    case 2:
                        $scope.focusElement("PasswordMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.register.ConfirmPassword === '' || $scope.register.ConfirmPassword === undefined || $scope.register.Password !== $scope.register.ConfirmPassword) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Mật khẩu xác nhận chưa nhập hoặc nhập chưa chính xác!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("ConfirmPasswordDk");
                        break;
                    case 2:
                        $scope.focusElement("ConfirmPasswordMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.register.Phone === '' || $scope.register.Phone === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập Số điện thoại!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("PhoneDk");
                        break;
                    case 2:
                        $scope.focusElement("PhoneMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if (!$scope.checkPhoneFormat($scope.register.Phone)) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Số điện thoại đã nhập không chính xác!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("PhoneDk");
                        break;
                    case 2:
                        $scope.focusElement("PhoneMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.register.IsUnit) {
            if ($scope.register.UnitId === '' || $scope.register.UnitId === undefined) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Chưa chọn cơ quan/tổ chức!')
                        .ok('Đóng')
                        .fullscreen(true)
                ).finally(function () {
                    switch (type) {
                        case 1:
                            $scope.focusElement("UnitDk");
                            break;
                        case 2:
                            $scope.focusElement("UnitMb");
                            break;
                        default:
                            break;
                    }
                });
                return;
            }
        }
        else {
            $scope.register.UnitId = undefined;
        }

        if (vcRecaptchaService.getResponse() === "") {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông tin')
                    .textContent('Chưa xác thực người dùng!')
                    .ok('Đóng')
                    .fullscreen(true)
            );
        }
        else {
            cfpLoadingBar.start();

            $scope.disableBtn.btRegister = true;
            cfpLoadingBar.start();
            var obj = angular.copy(this.register);

            var post = $http({
                method: 'POST',
                url: '/web/customer/register',
                data: obj,
                headers: {}
            });

            post.success(function successCallback(data, status, headers, config) {
                $scope.disableBtn.btRegister = false;
                cfpLoadingBar.complete();
                if (data.meta.error_code === 200) {
                    var confirm = $mdDialog.confirm()
                        .title('Thông báo')
                        .textContent('Bạn đã đăng ký thành công, bạn vào mail để lấy mã xác thực!')
                        .ok('Đến trang xác thực')
                        .cancel('Hủy');

                    $mdDialog.show(confirm).then(function () {
                        $window.location.href = data.data.KeyRandom + '';
                    });

                }
                else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent(data.meta.error_message)
                            .ok('Đóng')
                            .fullscreen(true)
                    );
                }
            }).error(function (data, status, headers, config) {
                $scope.disableBtn.btRegister = false;
                cfpLoadingBar.complete();
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
                        .ok('Đóng')
                        .fullscreen(true)
                );
            });
        }
    };

    //$scope.checkEmailFormat = function (email) {
    //    var emailRegex = new RegExp(/^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(\\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*))(?<=[a-z0-9])([\w\.-]+)?@[a-zA-Z0-9]+([\.-]{1}[a-zA-Z0-9]+)*(\.[a-zA-Z]{2,})$/);
    //    if (!emailRegex.test(email)) {
    //        return false;
    //    }
    //    return true;
    //};

    $scope.checkEmailFormat = function (email) {
        var emailRegex = new RegExp(/^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/);
        if (!emailRegex.test(email)) {
            return false;
        }
        return true;
    };


    $scope.checkPhoneFormat = function (phone) {
        // Định dạng phone
        var phoneRegex = /^(0|\+84)\d{9}$/;
        if (!phoneRegex.test(phone)) {
            return false;
        }
        return true;
    };

    $scope.SettingPass = function (type, id) {

        if ($scope.register.KeyRandom === '' || $scope.register.KeyRandom === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập Mã xác thực!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("FullNameDk");
                        break;
                    case 2:
                        $scope.focusElement("FullNameMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }

        $scope.disableBtn.btRegister = true;
        cfpLoadingBar.start();
        var obj = angular.copy(this.register);

        var post = $http({
            method: 'POST',
            url: '/web/customer/settingPass/' + id,
            data: obj,
            headers: {}
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btRegister = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                var confirm = $mdDialog.confirm()
                    .title('Thông báo')
                    .textContent('Bạn đã xác thực thành công!')
                    .ok('Đến trang đăng nhập')
                    .cancel('Hủy');

                $mdDialog.show(confirm).then(function () {
                    $window.location.href = '/dang-nhap';
                });

            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(true)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btRegister = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
                    .ok('Đóng')
                    .fullscreen(true)
            );
        });
    };

    $scope.ResetPass = function (type, id) {
        if ($scope.register.KeyRandom === '' || $scope.register.KeyRandom === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập Mã xác thực!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("FullNameDk");
                        break;
                    case 2:
                        $scope.focusElement("FullNameMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.register.Password === '' || $scope.register.Password === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mật khẩu!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("PasswordDk");
                        break;
                    case 2:
                        $scope.focusElement("PasswordMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }
        if ($scope.register.ConfirmPassword === '' || $scope.register.ConfirmPassword === undefined || $scope.register.Password !== $scope.register.ConfirmPassword) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Mật khẩu xác nhận chưa nhập hoặc nhập chưa chính xác!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                switch (type) {
                    case 1:
                        $scope.focusElement("ConfirmPasswordDk");
                        break;
                    case 2:
                        $scope.focusElement("ConfirmPasswordMb");
                        break;
                    default:
                        break;
                }
            });
            return;
        }

        $scope.disableBtn.btRegister = true;
        cfpLoadingBar.start();
        var obj = angular.copy(this.register);

        var post = $http({
            method: 'POST',
            url: '/web/customer/resetPass/' + id,
            data: obj,
            headers: {}
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btRegister = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                var confirm = $mdDialog.confirm()
                    .title('Thông báo')
                    .textContent('Bạn đã thiết lập mật khẩu thành công!')
                    .ok('Đến trang đăng nhập')
                    .cancel('Hủy');

                $mdDialog.show(confirm).then(function () {
                    $window.location.href = '/dang-nhap';
                });

            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(true)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btRegister = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
                    .ok('Đóng')
                    .fullscreen(true)
            );
        });
    };

    // dang xuat
    $scope.signOut = function () {
        $window.localStorage.removeItem("Timeout");
        $window.localStorage.removeItem("Order");
        $http.get("/web/customer/logout", {
            headers: {}
        }).then(function (data, status, headers) {
            if (data.data.meta.error_code === 200) {
                $window.localStorage.removeItem("nickName");
                $window.localStorage.removeItem("checkNickName");
                $scope.access_token = '';
                $scope.customerId = -1;
                $scope.nickName = '';
                $window.checkNickName = undefined;
                $window.location.href = '/';
            }
        });
    };

    // gio hang mini
    $scope.ShowDetailCart = function () {
        let check = true;
        $scope.totalPriceOrder = null;
        $scope.listOrder = JSON.parse($window.localStorage.getItem("Order"));
        if ($scope.listOrder !== undefined) {
            angular.forEach($scope.listOrder, function (item, key) {
                if (item.PriceSpecial) {
                    $scope.totalPriceOrder = $scope.totalPriceOrder + (item.PriceSpecial * item.quantity);
                }
                else {
                    check = false;
                }
            });
        }

        $scope.totalPriceOrder = check ? $scope.totalPriceOrder : null;

        $('.giohang').asidebar('open');
    };

    $scope.RemoveProductOrder = function (ProductId) {
        $scope.listOrder = JSON.parse($window.localStorage.getItem("Order"));
        if ($scope.listOrder !== undefined) {
            angular.forEach($scope.listOrder, function (item, key) {
                if (item.ProductId === ProductId) {
                    $scope.listOrder.splice(key, 1);
                }
            });

            if ($scope.listOrder.length === 0) $scope.listOrder = null;
        }

        $window.localStorage.setItem("Order", JSON.stringify($scope.listOrder));
        var totalPriceOrder = null;
        var quantity = 0;
        let check = true;
        if ($scope.listOrder !== undefined) {
            angular.forEach($scope.listOrder, function (item, key) {
                if (item.PriceSpecial) {
                    totalPriceOrder = totalPriceOrder + (item.PriceSpecial * item.quantity);
                }
                else {
                    check = false;
                }
            });
            quantity = $scope.listOrder.length;
        }

        $scope.quantity = quantity;
        $scope.totalPriceOrder = check ? totalPriceOrder : null;
        $rootScope.$emit("ListenMiniOrder", {});
    };

    //Luu thong tin sua user
    $scope.SaveEditUser = function () {

        if ($scope.EditCustomer.Email == '' || $scope.EditCustomer.Email == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập email !')
                    .ok('Đóng')
                    .fullscreen(true)
            );
            return;
        }

        $scope.disableBtn.btSubmit = true;
        cfpLoadingBar.start();

        $scope.EditCustomer.CustomerId = $scope.customerId;
        var post = $http({
            method: 'POST',
            url: '/web/Customer/UpdateInfoCustomer/' + $scope.customerId,
            data: $scope.EditCustomer,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            $scope.disableBtn.btSubmit = false;
            if (data.meta.error_code === 200) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Cập nhật thông tin thành công')
                        .ok('Đóng')
                        .fullscreen(true)
                );

            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(true)
                );
            }
        }).error(function (data, status, headers, config) {
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
                    .ok('Đóng')
                    .fullscreen(true)
            );
        });
    };

    // upload avata
    $scope.uploadAvatar = function (e) {
        if (e === undefined) return;
        if (e.files.length <= 0) return;

        var fd = new FormData();
        fd.append("file", e.files[0]);
        cfpLoadingBar.start();
        var post = $http({
            method: 'POST',
            url: '/web/upload/uploadImage/6',
            data: fd,
            headers: {
                "Content-Type": undefined
            }
        });

        post.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $scope.register.Avata = data.data[0];
                if ($scope.customerId != undefined) {

                    $scope.EditCustomer.CustomerId = $scope.customerId;
                    $scope.EditCustomer.Avata = data.data[0];
                    var obj = angular.copy($scope.EditCustomer);

                    var post = $http({
                        method: 'PUT',
                        url: '/web/customer/UpdateAvata',
                        data: obj,
                        headers: { 'Authorization': 'bearer ' + $scope.access_token }
                    });

                    post.success(function successCallback(data, status, headers, config) {
                        $scope.disableBtn.btSubmit = false;
                        cfpLoadingBar.complete();
                        if (data.meta.error_code === 200) {
                            $window.location.href = '/thong-tin-tai-khoan';
                        }
                        else {
                            $mdDialog.show(
                                $mdDialog.alert()
                                    .clickOutsideToClose(true)
                                    .title('Thông tin')
                                    .textContent(data.meta.error_message)
                                    .ok('Đóng')
                                    .fullscreen(true)
                            );
                        }
                    }).error(function (data, status, headers, config) {
                        $scope.disableBtn.btSubmit = false;
                        cfpLoadingBar.complete();
                        $mdDialog.show(
                            $mdDialog.alert()
                                .clickOutsideToClose(true)
                                .title('Thông báo')
                                .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
                                .ok('Đóng')
                                .fullscreen(true)
                        );
                    });
                }
            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(true)
                );
            }
        }).error(function (data, status, headers, config) {
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
                    .ok('Đóng')
                    .fullscreen(true)
            );
        });
    };
    // Phan trang

    $scope.ParseNumberToArray = function () {
        var floor = Math.floor($scope.item_count / $scope.page_size);
        var LayDu = $scope.item_count % $scope.page_size;
        floor = LayDu > 0 ? floor + 1 : floor;
        floor = floor === 0 ? 1 : floor;
        $scope.NumberOfPage = floor;
        return new Array(floor);
    };

    $scope.ChangePage = function (cs, page) {
        switch (cs) {
            case 1:
                $scope.page = $scope.page - 1;
                break;
            case 2:
                $scope.page = page;
                break;
            case 3:
                $scope.page = $scope.page + 1;
                break;
            case 4:
                $scope.page = $scope.page - 1;
                break;
            default:
                break;
        }
        $scope.ChangeStatutsOrder($scope.StatutOrId);
    };

    $scope.ChangePassword = function () {
        if ($scope.password.PasswordOld === '' || $scope.password.PasswordOld === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mật khẩu cũ!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("password");
            });
            return;
        }

        if ($scope.password.PasswordNew === '' || $scope.password.PasswordNew === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mật khẩu mới!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("new-password");
            });
            return;
        }

        if ($scope.password.ConfirmPassword === '' || $scope.password.ConfirmPassword === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mật khẩu xác nhận !')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("confirm-password");
            });
            return;
        }

        if ($scope.password.PasswordNew !== $scope.password.ConfirmPassword) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Mật khẩu xác nhận phải trùng với mật khẩu mới!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                $scope.focusElement("confirm-password");
            });
            return;
        }

        $scope.disableBtn.btSubmit = true;
        cfpLoadingBar.start();
        let obj = {
            "UserId": $scope.customerId,
            "PasswordOld": $scope.password.PasswordOld,
            "PasswordNew": $scope.password.PasswordNew
        };

        var post = $http({
            method: 'POST',
            url: '/web/customer/ChangePasssword',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $scope.password = {};
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đổi mật khẩu thành công')
                        .ok('Đóng')
                        .fullscreen(true)
                );
                $window.location.href = '/dang-xuat';
            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(true)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
                    .ok('Đóng')
                    .fullscreen(true)
            );
        });

    };

}]);