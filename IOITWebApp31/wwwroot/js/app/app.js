var myApp = angular.module('IOITWeb', ['angular-loading-bar', 'ngMaterial', 'ngMd5', 'ngCookies', 'vcRecaptcha', 'ui.bootstrap', 'ui.select', 'ngSanitize', 'ngDialog']);

myApp.value('config', {
    domain: 'https://localhost:44325/',
    domainPay: 'https://mtf.onepay.vn/paygate/vpcpay.op',
    //domainPay: 'https://onepay.vn/paygate/vpcpay.op',
    lang: 'vn',
    exchangeRate: '1',
    title: 'APC VIET NAM',
    againLink: 'tmdt.cnttvietnam.com.vn/gio-hang.html',
    paymentLink: 'thuc-hien-thanh-toan.html',
    resultfLink: 'ket-qua-don-hang.html',
    cardList: '970436',
    //opMerchant: 'OP_LTEVISA',
    //opAccessCode: 'E1101B05',
    opMerchant: 'TESTONEPAY',
    opAccessCode: '6BEB2546',
    keyCaptcha: '6LcPV8oUAAAAAJnnznu4E6jaNrXEWZdsrC3mRj6T',
    secretCaptcha: '6LcPV8oUAAAAAMT4e20qOOhLu4EeSWZv32-jDye6',
    regexEmail: /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
    regexPhone: /^(0[35789])[0-9]{8}$/,
    usernameBKAV: "RegisterMrc",
    passwordBKAV: "Ats@123@123",
    usernameCMS: "taikhoan1test",
    passwordCMS: "Abc123.",
    usernameAppCP:"0379685933",
    passwordAppCP:"123",
    usernameBKAVDEMO: "0110379214",
    passwordBKAVDEMO: "679434",
    apiUrlVNPT: "https://api.idg.vnpt.vn",
    accesstokenVNPT: "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJjZDMwNmE1YS02MDM1LTExZWYtYTQwZi02MzMyYWFjNmQ4YzUiLCJhdWQiOlsicmVzdHNlcnZpY2UiXSwidXNlcl9uYW1lIjoia3l0aHVhdEBjYXNocGx1cy52biIsInNjb3BlIjpbInJlYWQiXSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3QiLCJuYW1lIjoia3l0aHVhdEBjYXNocGx1cy52biIsInV1aWRfYWNjb3VudCI6ImNkMzA2YTVhLTYwMzUtMTFlZi1hNDBmLTYzMzJhYWM2ZDhjNSIsImF1dGhvcml0aWVzIjpbIlVTRVIiXSwianRpIjoiNzExMzZkZDgtZjQ0NS00NGY5LWE2ZTYtZTM0MTM4ZDE1YzRiIiwiY2xpZW50X2lkIjoiYWRtaW5hcHAifQ.eALharbG4K26_M5RndV_637V2XpdVEPrnFR0ZedCPNLNC08ASs4NyKuD_W_CVxRNfBbb5ekH4ZaxxI7pKU3PHmWxtS2u4bEF5h2G69BJ19hBMzyTyt5G_esqkjnT0sgrzAWVnvO19N7NSiGxcsc0S6iib0j93wZXxBUu-JYiWNQzJSlSvKyghimMOR6opNZBOm64mVFK3-Tad4COj9JbRpRNPsLLJAW9wp1yncVyRTs59urVhumjO84yIJ5-m3f9SQjgoSMFUseNpu2dtzrgy1nSY_Yn5iRAijLSrPkjrLUwLFh6ycaE8f8O-rzogpXxqTT5Ok_uix4B9UmNRJ0nUQ",
    tokenIdVnpt: "203e2c2f-6c38-1963-e063-62199f0a0d81",
    tokenKeyVnpt: "MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAK930H3AhjA4QS/uXjejQVj9+7TV/XVKLqRcvegid5++3RK8BJL+d0W0e9iQtFUwIn8fFNQoIcULgrhi+fL9JHsCAwEAAQ==",

    // token dang ky kinh doanh
    tokenIdVnptv1: "2041b14f-855f-636e-e063-63199f0a8487",
    tokenKeyVnptv1: "MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBALQ6yVfzFD+sWAKI97mKn8NQpeQ51jBadpWpu7MDDeDXzM8PJBdiSUaI0Cyv0uMsPt0QGzB2FTFV50N45pcHjz8CAwEAAQ==",

});

myApp.config(['cfpLoadingBarProvider', function (cfpLoadingBarProvider) {
    cfpLoadingBarProvider.includeSpinner = false;
}]);

myApp.config(function ($sceProvider) {
    $sceProvider.enabled(false);
});

myApp.filter('formatVND', function() {
    return function(input) {
        if (isNaN(input)) return input; // Kiểm tra nếu không phải là số
        return input.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".") + " VND"; // Định dạng giá
    };
});


myApp.config(function ($mdDateLocaleProvider) {
    $mdDateLocaleProvider.formatDate = function (date) {

        return date ? moment(date).format('DD-MM-YYYY') : '';

        //var tempDate = moment(date);
        //console.log(tempDate.isValid());
        //return tempDate.isValid() ? tempDate.format('DD-MM-YYYY') : '';
    };

    $mdDateLocaleProvider.parseDate = function (dateString) {
        var m = moment(dateString, 'DD-MM-YYYY', true);
        return m.isValid() ? m.toDate() : new Date(NaN);
    };

    $mdDateLocaleProvider.months = ['Tháng Một', 'Tháng Hai', 'Tháng Ba', 'Tháng Tư', 'Tháng Năm', 'Tháng Sáu',
                                  'Tháng Bảy', 'Tháng Tám', 'Tháng Chín', 'Tháng Mười', 'Tháng Mười Một', 'Tháng Mười Hai'];
    $mdDateLocaleProvider.shortMonths = ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
                                    'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'];
    $mdDateLocaleProvider.days = ['Chủ Nhật', 'Thứ Hai', 'Thứ Ba', 'Thứ Tư', 'Thứ Năm', 'Thứ Sáu', 'Thứ Bảy'];
    $mdDateLocaleProvider.shortDays = ['Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7', 'Chủ Nhật'];
    $mdDateLocaleProvider.firstDayOfWeek = 0;

    //$mdDateLocaleProvider.weekNumberFormatter = function (weekNumber) {
    //    return 'Tuần ' + weekNumber;
    //};
});

myApp.factory('app', function () {
    return {
        data: {
            domain: 'https://localhost:44325/',
            CustomerId: -1,
            Email: '',
            FullName: '',
            Avata: '',
            Address: '',
            Password: '',
            PhomeNumber: '',
            access_token: '',
            Sex: ''
        },
        updateData: function (CustomerId, CustomerEmail, CustomerFullName, CustomerAvata, CustomerAddress, CustomerPassword, CustomerPhoneNumber, access_token, CustomerSex) {
            this.data.CustomerId = CustomerId;
            this.data.Email = CustomerEmail;
            this.data.FullName = CustomerFullName;
            this.data.Avata = CustomerAvata;
            this.data.Address = CustomerAddress;
            this.data.Password = CustomerPassword;
            this.data.PhomeNumber = CustomerPhoneNumber;
            this.data.access_token = access_token;
            this.data.Sex = CustomerSex;
        }
    };
});

//myApp.config(function () {

//    var config = {
//        apiKey: "AIzaSyBMiFAeztwaRURX1LW7JXnaKsp5gLsmc_M",
//        authDomain: "autionkoi.firebaseapp.com",
//        databaseURL: "https://autionkoi.firebaseio.com",
//        projectId: "autionkoi",
//        storageBucket: "",
//        messagingSenderId: "212371371238",
//        appId: "1:212371371238:web:7b1831199020752f"
//    };
//    firebase.initializeApp(config);

//});

myApp.filter('iif', function () {
    return function (input, trueValue, falseValue) {
        return input ? trueValue : falseValue;
    };
});

myApp.filter("formatPrice", function () {
    return function (price, digits, thoSeperator, decSeperator, bdisplayprice) {
        //console.log("displayprice: " + price);
        var i;
        if (price === null || price === '') {
            return '';
        }
        price = (typeof price === "undefined") ? 0 : price;
        digits = (typeof digits === "undefined") ? 3 : digits;
        bdisplayprice = (typeof bdisplayprice === "undefined") ? true : bdisplayprice;
        thoSeperator = (typeof thoSeperator === "undefined") ? "." : thoSeperator;
        decSeperator = (typeof decSeperator === "undefined") ? "," : decSeperator;
        price = (typeof price === undefined) ? "0" : price;

        if (price !== 0) {
            if (digits === 0)
                price = Math.round(price);
            //console.log(price);
            var prices = 0 - price;
            if (price > 0) {
                prices = price;
            }
            prices = prices + "";
            var _temp = prices.split('.');
            var dig = (typeof _temp[1] === "undefined") ? "00" : _temp[1];
            if (bdisplayprice && parseInt(dig, 10) === 0) {
                dig = "";
            } else {
                dig = dig + "";
                if (dig.length > digits) {
                    dig = (Math.round(parseFloat("0." + dig) * Math.pow(10, digits))) + "";
                }
                for (i = dig.length; i < digits; i++) {
                    dig += "0";
                }
            }
            var num = _temp[0];
            var s = "",
                ii = 0;
            for (i = num.length - 1; i > -1; i--) {
                s = ((ii++ % 3 === 2) ? ((i > 0) ? thoSeperator : "") : "") + num.substr(i, 1) + s;
            }
        }
        else {
            s = 0;
        }

        if (price < 0) {
            s = '- ' + s;
        }
        if (dig > 0) {
            return s + decSeperator + dig;
        }
        else {
            return s;
        }
    }
});

myApp.filter('propsFilter', function () {
    return function (items, props) {
        var out = [];

        if (angular.isArray(items)) {
            var keys = Object.keys(props);

            items.forEach(function (item) {
                var itemMatches = false;

                for (var i = 0; i < keys.length; i++) {
                    var prop = keys[i];
                    var text = props[prop].toLowerCase();
                    if (item[prop].toString().toLowerCase().indexOf(text) !== -1) {
                        itemMatches = true;
                        break;
                    }
                }

                if (itemMatches) {
                    out.push(item);
                }
            });
        } else {
            out = items;
        }

        return out;
    };
});

//Đinh dạng giá trong ô input
myApp.filter("displayprice", function () {
    return function (input) {
        //console.log("displayprice: " + input);
        input = (typeof input === 'undefined' || input === '') ? "" : input + "";
        if (parseInt(input) === 0) {
            input = 0 + "";
        }
        var comma = ",";
        var num = parseInt(input) ? parseInt(input.replace(/\./g, '')) : input;
        //var num = parseInt(input) ? parseInt(input.replace(/[^\d|\-+|\.+]/g, '')) : input;

        //console.log("displayprice2: " + input);
        var nums = 0;
        if (num >= 0) {
            nums = num;
        }
        else {
            nums = 0 - num;
        }
        nums = nums + "";

        var str = "";

        var k = (nums.length % 3);
        if (k > 0) {
            str += nums.substring(0, k) + comma;
        }

        while (k < nums.length) {

            str += nums.substring(k, k + 3) + comma;
            k = k + 3;
        }
        if (num >= 0) {
            str = str.substring(0, str.length - 1);
        }
        else {
            str = "-" + str.substring(0, str.length - 1);
        }
        return str;
    }
});

myApp.directive('format', ['$filter', function ($filter) {
    return {
        require: '?ngModel',
        link: function (scope, elem, attrs, ctrl) {
            if (!ctrl) return;

            ctrl.$formatters.unshift(function (a) {
                return $filter(attrs.format)(ctrl.$modelValue, "", 0)
            });

            elem.bind('blur', function (event) {
                var plainNumber = elem.val().replace(/[^\d|\-+|\.+]/g, '');
                elem.val($filter(attrs.format)(plainNumber, "", 0));
            });
        }
    };
}]);

myApp.directive('clickEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.clickEnter);
                });

                event.preventDefault();
            }
        });
    };
});

myApp.directive('ckEditor', function () {
    return {
        require: '?ngModel',
        link: function (scope, elm, attr, ngModel) {
            var ck = CKEDITOR.replace(elm[0]);

            if (!ngModel) return;

            ck.on('pasteState', function () {
                scope.$apply(function () {
                    ngModel.$setViewValue(ck.getData());
                });
            });

            ngModel.$render = function (value) {
                ck.setData(ngModel.$viewValue);
            };
        }
    };
});

myApp.directive('format', ['$filter', function ($filter) {
    return {
        require: '?ngModel',
        link: function (scope, elem, attrs, ctrl) {
            if (!ctrl) return;
            
            ctrl.$formatters.unshift(function (a) {
                return $filter(attrs.format)(ctrl.$modelValue)
            });

            ctrl.$parsers.unshift(function (viewValue) {
                let plainNumber = viewValue.replace(/[^\d|\-+|\.+]/g, '');
                elem.val($filter(attrs.format)(plainNumber));
                return plainNumber;
            });
        }
    };
}]);
