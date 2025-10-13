myApp.controller('UserUnitController', ['$scope', '$http', '$mdDialog', 'config', 'cfpLoadingBar', 'app', '$cookies', '$rootScope', '$window', 'ngDialog', '$uibModal', 'vcRecaptchaService', function UserUnitController($scope, $http, $mdDialog, config, cfpLoadingBar, app, $cookies, $rootScope, $window, ngDialog, $uibModal, vcRecaptchaService) {
    $scope.page = 1;
    $scope.page_size = 10;
    $scope.query = "1=1";
    $scope.register = {};
    $scope.TinTuc = {};
    $scope.message = {};
    $scope.q = {};
    $scope.rowIndex = 1;
    $scope.orderby = "";
    $scope.item_count = 0;
    $scope.IdFile = 0;
    $scope.disableBtn = { btSubmit: false };
    $scope.linkFB = '';
    $scope.linkIns = '';
    $scope.linkTiktok = '';
    $scope.ItemId = '';
    $scope.SocialNetWorks = [];
    $scope.listSchool = [];
    $scope.disableFB = true;
    $scope.itemFb = false;
    $scope.itemIns = false;
    $scope.itemTikTok = false;
    $scope.item = false;
    $scope.items = 0;
    $scope.School = null;
    $scope.contact = {};
    $scope.RegisterCode = {};
    $scope.UpdatePartner = {};
    $scope.dataBaomat = '';
    $scope.countdown = 60;
    $scope.disableButton = false;
    $scope.checkBox = false;
    $scope.Vido = 0;
    $scope.Kinhdo = 0;
    $scope.listStatus = [
        {
            "Id": 1,
            "Name": "Sử dụng"
        },
        {
            "Id": 10,
            "Name": "Không sử dụng"
        }
    ];
    $scope.listSexs = [
        {
            "Id": 1,
            "Name": "Nam"
        },
        {
            "Id": 2,
            "Name": "Nữ"
        },
        {
            "Id": 3,
            "Name": "Khác"
        }
    ];

    $scope.listTypeId = [
        {
            "Id": 1,
            "Name": "Căn cước công dân"
        },
        {
            "Id": 2,
            "Name": "Hộ chiếu"
        }
    ];

    $scope.listSchool = [
        {
            "Id": 1,
            "Name": "TRƯỜNG CAO ĐẲNG DU LỊCH VÀ THƯƠNG MẠI HÀ NỘI"
        },
        {
            "Id": 2,
            "Name": "TRƯỜNG CAO ĐẲNG CÔNG THƯƠNG VIỆT NAM"
        },
        {
            "Id": 3,
            "Name": "TRƯỜNG KHÁC"
        }
    ];
    $scope.listLink = [
        {
            "Id": 1,
            "Name": undefined,
        }
    ];

    $scope.url = "/";

    $scope.publicKey = "6Ld_EMEfAAAAAJBZTDIdpXims5GZHQBRLhc0XErX";


    $scope.init = function (data, id) {
        cfpLoadingBar.start();

        if (data != undefined) {
            $scope.customerId = data.CustomerId;
            $scope.access_token = data.access_token;

        }
        $scope.q = {};
        $scope.customer = {};
        $scope.partner = {};
        $scope.register = {};
        $scope.GroupSV = {};
        $scope.customer.StatusView = true;

        if (id != undefined) {
            $scope.userId = id;
            $scope.loadDataById();
            $scope.loadDataByIdMe();

        }
        else {
            //$scope.loadProvince();
            $scope.loadUnit();
            $scope.loadTypeAttributeItem(4);
            $scope.loadTypeAttributeItem(25);
            $scope.loadTypeAttributeItem(26);
            $scope.loadCountry();
            $scope.loadResearchArea();
            $scope.loadRole(1);
            $scope.loadData();

        }
    };

    $scope.getGroupSV = function () {
        var get = $http({
            method: 'GET',
            url: 'web/customer/GetGroupByPage',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.GroupSV = response.data.data;
            $scope.GroupSVCT = $scope.GroupSV.filter(function (item) {
                return item.Location == 2;
            });
            $scope.GroupSVTM = $scope.GroupSV.filter(function (item) {
                return item.Location == 1;
            });
            $scope.topGroup = [];

            $scope.topGroup = $scope.GroupSV.filter(function (item) {
                return item.IsGroup == true;
            }).sort(function (item1, item2) {
                var datetop3gr = new Date(item1.UpdatedAt);
                var datetop3gr2 = new Date(item2.UpdatedAt);
                return datetop3gr - datetop3gr2;
            });
            console.log($scope.topGroup);
        });
    }

    $scope.getSlide = function () {
        var get = $http({
            method: 'GET',
            url: 'web/slide/GetSlide',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.Slideanh = response.data.data[0].Image;
            $scope.UrlSlideanh = response.data.data[0].Url;
        });
    }

    $scope.getlistProvince = function () {
        var get = $http({
            method: 'GET',
            url: 'web/other / listProvince ? page = 1 & query=1= 1 & order_by=',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.listProvince = response.data.data;
        });
    }
    $scope.loadProvince = function () {
        //var query = "TypeCategoryId=15";
        $http.get("/web/other/listProvince?page=1&query=1=1&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listProvince = data.data.data;
                if ($scope.unitId != undefined) {
                    for (var i = 0; i < $scope.listProvince.length; i++) {
                        if ($scope.listProvince[i].ProvinceId == $scope.unit.ProvinceId) {
                            $scope.dataProvince = $scope.listProvince[i];
                            break;
                        }
                    }
                }
            }
        });
    };



    $scope.getGroupbyId = function () {
        var urlParams = new URLSearchParams(window.location.search);
        var typeAttribute = urlParams.get('TypeAttributeID');

        if (typeAttribute) {
            var url = '/web/customer/GetGroup/' + typeAttribute;
        } else {

            var typeId = localStorage.getItem('TypeAttId');
            var url = '/web/customer/GetGroup/' + typeId;
        }
        var config = {
            method: 'GET',
            url: url,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        };

        $http(config)
            .then(function (response) {
                $scope.NhomSV = response.data.data;
                $scope.listSV = response.data.data.listCustomer;
            });
    }
    $scope.goDetailGroupSV = function (id) {
        $scope.TypeAttID = id;
        var url = '/chi-tiet-nhom/' + id;
        if ($scope.ItemId !== '') {
            localStorage.setItem('TypeAttId', $scope.TypeAttID);
            localStorage.setItem('urldetailgroup', url);
            $window.location.href = url;
        }
    };

    $scope.getListRegister = function () {
        $scope.setTime();
        $scope.getGroupSV();
        var urlParams = new URLSearchParams(window.location.search);
        var schoolCode = urlParams.get('schoolCode');

        for (var i = 0; i < $scope.listSchool.length; i++) {
            if ($scope.listSchool[i].Id == schoolCode) {
                $scope.School = $scope.listSchool[i];
                break;
            }
        }
        var get = $http({
            method: 'GET',
            url: 'web/customer/list-register',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.register = response.data.data;
            $scope.customer = response.data.data;

            $scope.register = $scope.register.filter(function (item) {
                return item.SchoolCode == schoolCode;
            });

            $scope.register1 = response.data.data.filter(function (item) {
                return item.SchoolCode == 1;
            });

            $scope.register2 = response.data.data.filter(function (item) {
                return item.SchoolCode == 2;
            });


            // danh sách được chọn theo trường B2
            $scope.registerTM = response.data.data.filter(function (item) {
                return item.SchoolCode === 1 && item.StepTwo === true;
            }).sort(function (item1, item2) {
                var date1 = new Date(item1.UpdatedAt);
                var date2 = new Date(item2.UpdatedAt);
                return date1 - date2;
            });

            $scope.registerCT = response.data.data.filter(function (item) {
                return item.SchoolCode === 2 && item.StepTwo === true;
            }).sort(function (item1, item2) {
                var date1b2 = new Date(item1.UpdatedAt);
                var date2b2 = new Date(item2.UpdatedAt);
                return date1b2 - date2b2;
            });
            // danh sách được chọn theo trường B4
            $scope.registerTMb4 = response.data.data.filter(function (item) {
                return item.SchoolCode === 1 && item.StepFour === true;
            }).sort(function (item1, item2) {
                var date1b4 = new Date(item1.UpdatedAt);
                var date2b4 = new Date(item2.UpdatedAt);
                return date1b4 - date2b4;
            });

            $scope.registerCTb4 = response.data.data.filter(function (item) {
                return item.SchoolCode === 2 && item.StepFour === true;
            }).sort(function (item1, item2) {
                var date1b4 = new Date(item1.UpdatedAt);
                var date2b4 = new Date(item2.UpdatedAt);
                return date1b4 - date2b4;
            });
            // danh sách được chọn theo trường B5
            $scope.registerTMb5 = response.data.data.filter(function (item) {
                return item.SchoolCode === 1 && item.StepFive === true;
            }).sort(function (item1, item2) {
                var date1b5 = new Date(item1.UpdatedAt);
                var date2b5 = new Date(item2.UpdatedAt);
                return date1b5 - date2b5;
            });

            $scope.registerCTb5 = response.data.data.filter(function (item) {
                return item.SchoolCode === 2 && item.StepFive === true;
            }).sort(function (item1, item2) {
                var date1b5 = new Date(item1.UpdatedAt);
                var date2b5 = new Date(item2.UpdatedAt);
                return date1b5 - date2b5;
            });


            // danh sách được chọn theo trường vòng 4
            $scope.topThree = [];

            $scope.topThree = response.data.data.filter(function (item) {
                return item.TopThree === true;
            }).sort(function (item1, item2) {
                var datetop3sv = new Date(item1.UpdatedAt);
                var datetop3sv2 = new Date(item2.UpdatedAt);
                return datetop3sv - datetop3sv2;
            });

            console.log($scope.topThree);



            //for (var i = 0; i < response.data.data.length; i++) {
            //    var total = response.data.data[i].AcademicRankId + response.data.data[i].TypeId + response.data.data[i].UnitId;
            //    if ($scope.topThree.length < 3 || total > $scope.topThree[2].total) {
            //        $scope.topThree.push({ data: response.data.data[i], total: total });
            //        $scope.topThree.sort(function (a, b) {
            //            return b.total - a.total;
            //        });
            //        $scope.topThree = $scope.topThree.slice(0, 3);
            //    }
            //}


        });
    };

    $scope.goDetail = function (id) {
        $scope.ItemId = id;
        var url = '/chi-tiet/' + id;
        if ($scope.ItemId !== '') {
            localStorage.setItem('itemId', $scope.ItemId);
            localStorage.setItem('local', url);
            //$window.location.href = url;
        }
    };
    $scope.goDetails = function (id) {
        $scope.ItemId = id;
        var url = '/chi-tiet/';
        if ($scope.ItemId !== '') {
            localStorage.setItem('itemId', $scope.ItemId);
            localStorage.setItem('local', url);
            $window.location.href = url;
        }
    };

    $scope.getMessage = function () {
        var url = '/web/comment/GetCommentByJoinnerId/' + $scope.register.CustomerId;

        var get = $http({
            method: 'Get',
            //url: 'web/customer/new-register',
            url: url,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        get.then(function (data, status, headers) {
            $scope.message = data.data.data;
        });
    };
    $scope.getListRegisterById = function () {
        var urlParams = new URLSearchParams(window.location.search);
        var studentCode = urlParams.get('StudentCode');

        /*12/9*/

        var shareButton = document.getElementById("share-button");
        var shareLink = document.getElementById("share-link");

        var shareUrl = "https://www.facebook.com/sharer/sharer.php?u=https%3A%2F%2Fcashplus.vn%2Fchi-tiet%2F%3FStudentCode=" + studentCode;

        shareLink.setAttribute("href", shareUrl);
        var fbShare = window.location.href;



        shareButton.setAttribute("data-href", fbShare);

        if (studentCode) {
            var url = '/web/customer/getByStudentCode/' + studentCode;
        } else {

            var itemId = localStorage.getItem('itemId');
            var url = '/web/customer/getByStudentCode/' + itemId;
        }
        var config = {
            method: 'GET',
            url: url,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        };

        $http(config)
            .then(function (response) {
                var data = response.data;

                $scope.register = data.data;
                $scope.SocialNetworks = JSON.parse($scope.register.SocialNetworks);
                console.log($scope.register.AcademicRankId)
                $scope.likeCount = $scope.register.AcademicRankId ? $scope.register.AcademicRankId : 0;
                console.log($scope.likeCount)

                var likeCountElement = document.getElementById('likeCount');
                likeCountElement.textContent = $scope.likeCount;

                if ($scope.SocialNetworks) {
                    for (let i = 0; i < $scope.SocialNetworks.length; i++) {
                        if ($scope.SocialNetworks[i].id === 1) {
                            $scope.itemFb = true;
                        }
                        if ($scope.SocialNetworks[i].id === 2) {
                            $scope.itemIns = true;
                        }
                        if ($scope.SocialNetworks[i].id === 3) {
                            $scope.itemTikTok = true;
                        }
                    }
                }
                $scope.SocialNetworks = Object.values(
                    $scope.SocialNetworks.reduce(function (acc, current) {
                        acc[current.id] = current;
                        return acc;
                    }, {})
                );

                $scope.getMessage()

            })
            .catch(function (error) {
                console.log('Error:', error);
            });
    };

    $scope.addComment = function () {
        $scope.message.CustomerId = $scope.register.CustomerId;
        var obj = {
            'CustomerId': $scope.message.CustomerId,
            'Name': $scope.message.Name,
            'Email': $scope.message.Email,
            'Contents': $scope.message.Contents,
        };
        var post = $http({
            method: 'POST',
            url: '/web/comment/AddNewComment',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent("Cảm ơn bạn đã bình chọn thành công!")
                        .ok('Đóng')
                        .fullscreen(false)
                );
                $scope.message.Name = '';
                $scope.message.Email = '';
                $scope.message.Contents = '';
            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });
    };



    $scope.loadData = function () {
        $scope.q.page = $scope.page;
        $scope.q.page_size = $scope.page_size;
        $scope.q.query = $scope.query;
        $scope.q.orderby = $scope.orderby;
        var obj = angular.copy($scope.q);
        if (obj.DateStart != undefined && obj.DateStart != '') {
            obj.DateStart = obj.DateStart.getFullYear() + "/" + (obj.DateStart.getMonth() + 1) + "/" + obj.DateStart.getDate();
        }
        if (obj.DateEnd != undefined && obj.DateEnd != '') {
            obj.DateEnd = obj.DateEnd.getFullYear() + "/" + (obj.DateEnd.getMonth() + 1) + "/" + obj.DateEnd.getDate();
        }
        var post = $http({
            method: 'POST',
            url: '/web/customer/GetByPagePost',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $scope.listCustomers = data.data;
                $scope.item_count = data.metadata;
            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });

        //$http.get("/web/dataSet/GetByPage?page=" + $scope.page + "&page_size=" + $scope.page_size + "&query=" + $scope.query + "&order_by=" + $scope.orderby, {
        //    headers: { 'Authorization': 'bearer ' + $scope.access_token }
        //}).then(function (data, status, headers) {
        //    cfpLoadingBar.complete();
        //    if (data.data.meta.error_code === 200) {
        //        $scope.listDataSet = data.data.data;
        //        $scope.item_count = data.data.metadata;
        //    }
        //});
    };

    $scope.loadDataById = function () {

        $http.get("/web/customer/GetById/" + $scope.userId, {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.customer = angular.copy(data.data.data);
                $scope.customer.StatusView = $scope.customer.Status == 1 ? true : false;
                $scope.loadUnit();
                $scope.loadTypeAttributeItem(4);
                $scope.loadTypeAttributeItem(25);
                $scope.loadTypeAttributeItem(26);
                $scope.loadCountry();
                $scope.loadResearchArea();
                $scope.loadRole(1);
                for (var i = 0; i < $scope.listSexs.length; i++) {
                    if ($scope.listSexs[i].Id == $scope.customer.Sex) {
                        $scope.dataSex = $scope.listSexs[i];
                        break;
                    }
                }
                for (var i = 0; i < $scope.listTypeId.length; i++) {
                    if ($scope.listTypeId[i].Id == $scope.customer.TypeId) {
                        $scope.dataTypeId = $scope.listTypeId[i];
                        break;
                    }
                }
                for (var i = 0; i < $scope.listSchool.length; i++) {
                    $scope.dataSchool = $scope.listSchool[i];
                    break;
                }
            }
        });
    };

    $scope.loadDataByIdMe = function () {

        $http.get("/web/customer/getByIdMe/" + $scope.userId, {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.customer = angular.copy(data.data.data);
                $scope.customer.StatusView = $scope.customer.Status == 1 ? true : false;
                $scope.loadUnit();
                $scope.loadTypeAttributeItem(4);
                $scope.loadTypeAttributeItem(25);
                $scope.loadTypeAttributeItem(26);
                $scope.loadCountry();
                $scope.loadResearchArea();
                $scope.loadRole(1);
                for (var i = 0; i < $scope.listSexs.length; i++) {
                    if ($scope.listSexs[i].Id == $scope.customer.Sex) {
                        $scope.dataSex = $scope.listSexs[i];
                        break;
                    }
                }
                for (var i = 0; i < $scope.listTypeId.length; i++) {
                    if ($scope.listTypeId[i].Id == $scope.customer.TypeId) {
                        $scope.dataTypeId = $scope.listTypeId[i];
                        break;
                    }
                }
                for (var i = 0; i < $scope.listSchool.length; i++) {
                    $scope.dataSchool = $scope.listSchool[i];
                    break;
                }
            }
        });
    };

    $scope.loadTypeAttributeItem = function (id) {
        $http.get("/web/other/listTypeItem?page=1&query=TypeAttributeId=" + id + "&order_by=TypeAttributeItemId Asc", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                if (id == 4) {
                    $scope.listPosition = data.data.data;
                    if ($scope.userId != undefined) {
                        for (var i = 0; i < $scope.listPosition.length; i++) {
                            if ($scope.listPosition[i].TypeAttributeItemId == $scope.customer.PositionId) {
                                $scope.dataPosition = $scope.listPosition[i];
                                break;
                            }
                        }
                    }
                }
                else if (id == 25) {
                    $scope.listAcademicRank = data.data.data;
                    if ($scope.userId != undefined) {
                        for (var i = 0; i < $scope.listAcademicRank.length; i++) {
                            if ($scope.listAcademicRank[i].TypeAttributeItemId == $scope.customer.AcademicRankId) {
                                $scope.dataAcademicRank = $scope.listAcademicRank[i];
                                break;
                            }
                        }
                    }
                }
                else if (id == 26) {
                    $scope.listDegree = data.data.data;
                    if ($scope.userId != undefined) {
                        for (var i = 0; i < $scope.listDegree.length; i++) {
                            if ($scope.listDegree[i].TypeAttributeItemId == $scope.customer.DegreeId) {
                                $scope.dataDegree = $scope.listDegree[i];
                                break;
                            }
                        }
                    }
                }
            }
        });
    };

    $scope.loadResearchArea = function () {
        let type = 15;
        $http.get("/web/category/GetByPage?page=1&query=TypeCategoryId=" + type + "&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listResearchArea = data.data.data;
                //if ($scope.userId != undefined) {
                //    for (var i = 0; i < $scope.listRoles.length; i++) {
                //        if ($scope.listRoles[i].RoleId == $scope.customer.RoleId) {
                //            $scope.listRoles = $scope.listRoles[i];
                //            break;
                //        }
                //    }
                //}
            }
        });
    };


    $scope.loadCountry = function () {
        $http.get("/web/other/listCountry?page=1&query=1=1&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listCountry = data.data.data;
                if ($scope.userId != undefined) {
                    for (var i = 0; i < $scope.listCountry.length; i++) {
                        if ($scope.listCountry[i].CountryId == $scope.customer.CountryId) {
                            $scope.dataCountry = $scope.listCountry[i];
                            break;
                        }
                    }
                }
            }
        });
    };

    $scope.loadUnit = function () {
        $http.get("/web/unit/listUnit", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listUnits = data.data.data;
                if ($scope.userId != undefined) {
                    for (var i = 0; i < $scope.listUnits.length; i++) {
                        if ($scope.listUnits[i].UnitId == $scope.customer.UnitId) {
                            $scope.dataUnit = $scope.listUnits[i];
                            break;
                        }
                    }
                }
            }
        });
    };


    $scope.changeValue = function (object, type) {
        switch (type) {
            case 1:
                $scope.customer.UnitId = object ? object.UnitId : null;
                break;
            case 2:
                $scope.customer.Sex = object ? object.Id : null;
                break;
            case 3:
                $scope.customer.SchoolCode = object ? object.Id : null;
                $scope.items = $scope.customer.SchoolCode;
                break;
            case 4:
                $scope.customer.TypeId = object ? object.Id : null;
                break;
            case 5:
                $scope.customer.PositionId = object ? object.TypeAttributeItemId : null;
                break;
            case 6:
                $scope.customer.AcademicRankId = object ? object.TypeAttributeItemId : null;
                break;
            case 7:
                $scope.customer.DegreeId = object ? object.TypeAttributeItemId : null;
                break;
            //case 8:
            //    $scope.customer.listRA = object ? object.TypeAttributeItemId : null;
            //    break;
            //case 9:
            //    $scope.customer.listMU = object ? object.TypeAttributeItemId : null;
            //    break;
            case 10:
                $scope.customer.RoleId = object ? object.RoleId : null;
                break;
            case 11:
                $scope.q.Status = object ? object.Id : null;
                break;
            default:
                break;
        }
    }

    $scope.onQueryChange = function () {
        var query = '1=1';
        if ($scope.q.Status != undefined) {
            query += ' AND Status=' + $scope.q.Status;
        }
        if ($scope.q.txtSearch != undefined && $scope.q.txtSearch != '') {
            query += ' AND (FullName.Contains("' + $scope.q.txtSearch + '") or Email.Contains("' + $scope.q.txtSearch + '") or Phone.Contains("' + $scope.q.txtSearch + '"))';
        }
        $scope.query = query;
        $scope.loadData();
    }

    $scope.goManagerUser = function () {
        $window.location.href = '/nguoi-dung-to-chuc';
    };

    $scope.goInfoUser = function () {
        $window.location.href = '/thong-tin-tai-khoan';
    };


    $scope.SaveData = function (kk) {
        if ($scope.customer.Email === '' || $scope.customer.Email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập địa chỉ Email')
                    .ok('Đóng')
                    .fullscreen(false)
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
        if ($scope.customer.UnitId === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa chọn cơ quan/tổ chức')
                    .ok('Đóng')
                    .fullscreen(false)
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
        if ($scope.customer.FullName === '' || $scope.customer.FullName === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập tên người dùng!')
                    .ok('Đóng')
                    .fullscreen(false)
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

        $scope.disableBtn.btSubmit = true;
        cfpLoadingBar.start();
        var obj = angular.copy($scope.customer);
        obj.CreatedId = $scope.customerId;
        obj.UpdatedId = $scope.customerId;
        obj.Status = obj.StatusView ? 1 : 98;
        if (obj.DateNumber != undefined && obj.DateNumber != '') {
            var dateFull = new Date(obj.DateNumber);
            obj.DateNumber = dateFull.getFullYear() + "/" + (dateFull.getMonth() + 1) + "/" + dateFull.getDate();
        }
        if (obj.Birthday != undefined && obj.Birthday != '') {
            var dateFull = new Date(obj.Birthday);
            obj.Birthday = dateFull.getFullYear() + "/" + (dateFull.getMonth() + 1) + "/" + dateFull.getDate();
        }

        if ($scope.customer.CustomerId == undefined) {
            var post = $http({
                method: 'POST',
                url: '/web/customer',
                data: obj,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });

            post.success(function successCallback(data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                if (data.meta.error_code === 200) {
                    $scope.goManagerUser();
                }
                else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent(data.meta.error_message)
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }).error(function (data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            });
        }
        else {
            var post = $http({
                method: 'PUT',
                url: '/web/customer/' + obj.CustomerId,
                data: obj,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });

            post.success(function successCallback(data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                if (data.meta.error_code === 200) {
                    if (kk == 1) {
                        $mdDialog.show(
                            $mdDialog.alert()
                                .clickOutsideToClose(true)
                                .title('Thông báo')
                                .textContent('Cập nhật thông tin thành công!')
                                .ok('Đóng')
                                .fullscreen(false)
                        );
                    }
                    else {
                        $scope.goManagerUser();
                    }
                }
                else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent(data.meta.error_message)
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }).error(function (data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            });
        }
    };

    $scope.SaveDataMe = function (kk) {

        if ($scope.customer.Email === '' || $scope.customer.Email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập địa chỉ Email')
                    .ok('Đóng')
                    .fullscreen(false)
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
        if ($scope.customer.UnitId === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa chọn cơ quan/tổ chức')
                    .ok('Đóng')
                    .fullscreen(false)
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
        if ($scope.customer.FullName === '' || $scope.customer.FullName === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập tên người dùng!')
                    .ok('Đóng')
                    .fullscreen(false)
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

        $scope.disableBtn.btSubmit = true;
        cfpLoadingBar.start();
        var obj = angular.copy($scope.customer);
        obj.CreatedId = $scope.customerId;
        obj.UpdatedId = $scope.customerId;
        obj.Status = obj.StatusView ? 1 : 98;
        if (obj.DateNumber != undefined && obj.DateNumber != '') {
            var dateFull = new Date(obj.DateNumber);
            obj.DateNumber = dateFull.getFullYear() + "/" + (dateFull.getMonth() + 1) + "/" + dateFull.getDate();
        }
        if (obj.Birthday != undefined && obj.Birthday != '') {
            var dateFull = new Date(obj.Birthday);
            obj.Birthday = dateFull.getFullYear() + "/" + (dateFull.getMonth() + 1) + "/" + dateFull.getDate();
        }

        var post = $http({
            method: 'PUT',
            url: '/web/customer/updateInfo/' + obj.CustomerId,
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                //if (kk == 1) {
                //    $mdDialog.show(
                //        $mdDialog.alert()
                //            .clickOutsideToClose(true)
                //            .title('Thông báo')
                //            .textContent('Cập nhật thông tin thành công!')
                //            .ok('Đóng')
                //            .fullscreen(false)
                //    );
                //}
                //else {
                $scope.goInfoUser();
                /*}*/
            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông tin')
                    .textContent(data.meta.error_message)
                    .ok('Đóng')
                    .fullscreen(false)
            );

        }).catch(function (error) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });
    };



    $scope.deleteData = function (item) {
        var confirm = $mdDialog.confirm()
            .title('Thông báo')
            .textContent('Bạn có chắc chắn muốn xóa bản ghi này?')
            .ok('Đồng ý')
            .cancel('Hủy');

        $mdDialog.show(confirm).then(function () {
            var remove = $http({
                method: 'DELETE',
                url: '/web/customer/' + item.CustomerId,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });

            remove.success(function (data, status, headers, config) {
                if (data.meta.error_code == 200) {
                    $scope.loadData();
                    $mdToast.show($mdToast.simple()
                        .textContent('Xóa thành công!')
                        .position('fixed bottom right')
                        .hideDelay(3000));
                }
                else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }).error(function () {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            });
        });
    }

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
        // haohv 

        post.success(function successCallback(data, status, headers, config) {
            post.success(function successCallback(data, status, headers, config) {
                cfpLoadingBar.complete();
                if (data.meta.error_code === 200) {
                    if ($scope.customer === undefined) {
                        $scope.customer = {};
                        $scope.customer.Avata = data.data[0];
                    } else {
                        $scope.customer.Avata = data.data[0];
                    }
                }
                else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông báo')
                            .textContent(data.meta.error_message)
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }).error(function (data, status, headers, config) {
                cfpLoadingBar.complete();
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            });

            //    cfpLoadingBar.complete();
            //    if (data.meta.error_code === 200) {
            //        $scope.EditCustomer = {};
            //        $scope.EditCustomer.CustomerId = $scope.customerId;
            //        $scope.EditCustomer.Avata = data.data[0];
            //        var obj = angular.copy($scope.EditCustomer);
            //        var post = $http({
            //            method: 'PUT',
            //            url: '/web/customer/updateAvata/1',
            //            data: obj,
            //            headers: { 'Authorization': 'bearer ' + $scope.access_token }
            //        });

            //        post.success(function successCallback(data, status, headers, config) {
            //            $scope.disableBtn.btSubmit = false;
            //            cfpLoadingBar.complete();
            //            if (data.meta.error_code === 200) {
            //                $scope.goInfoUser();
            //            }
            //            else {
            //                $mdDialog.show(
            //                    $mdDialog.alert()
            //                        .clickOutsideToClose(true)
            //                        .title('Thông tin')
            //                        .textContent(data.meta.error_message)
            //                        .ok('Đóng')
            //                        .fullscreen(false)
            //                );
            //            }
            //        }).error(function (data, status, headers, config) {
            //            $scope.disableBtn.btSubmit = false;
            //            cfpLoadingBar.complete();
            //            $mdDialog.show(
            //                $mdDialog.alert()
            //                    .clickOutsideToClose(true)
            //                    .title('Thông báo')
            //                    .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
            //                    .ok('Đóng')
            //                    .fullscreen(false)
            //            );
            //        });
            //    }
            //    else {
            //        $mdDialog.show(
            //            $mdDialog.alert()
            //                .clickOutsideToClose(true)
            //                .title('Thông báo')
            //                .textContent(data.meta.error_message)
            //                .ok('Đóng')
            //                .fullscreen(false)
            //        );
            //    }
            //}).error(function (data, status, headers, config) {
            //    cfpLoadingBar.complete();
            //    $mdDialog.show(
            //        $mdDialog.alert()
            //            .clickOutsideToClose(true)
            //            .title('Thông báo')
            //            .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
            //            .ok('Đóng')
            //            .fullscreen(false)
            //    );
        });
    };


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
        $scope.loadProduct();
    };
    $scope.clearAvatar = function () {
        var confirm = $mdDialog.confirm()
            .title('Thông báo')
            .textContent('Bạn có chắc chắn muốn xóa ảnh đại diện?')
            .ok('Đồng ý')
            .cancel('Hủy');
        $mdDialog.show(confirm).then(function () {
            $scope.disableBtn.btSubmit = true;
            cfpLoadingBar.start();
            $scope.UpdatePartner.avatar == '';
        })
    }
    
    $scope.clearDocument = function (documentItem) {
        var index = $scope.UpdatePartner.list_documents.indexOf(documentItem);
        if (index !== -1) {
            $scope.UpdatePartner.list_documents.splice(index, 1);
        }
    };
   


    $scope.clearFile = function () {
        var confirm = $mdDialog.confirm()
            .title('Thông báo')
            .textContent('Bạn có chắc chắn muốn xóa ảnh đại diện?')
            .ok('Đồng ý')
            .cancel('Hủy');

        $mdDialog.show(confirm).then(function () {
            $scope.disableBtn.btSubmit = true;
            cfpLoadingBar.start();
            $scope.EditCustomer = {};
            $scope.EditCustomer.CustomerId = $scope.customerId;
            $scope.EditCustomer.Avata = undefined;
            var obj = angular.copy($scope.EditCustomer);

            var post = $http({
                method: 'PUT',
                url: '/web/customer/updateAvata/2',
                data: obj,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });

            post.success(function successCallback(data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                if (data.meta.error_code === 200) {
                    angular.element("input[type='file']").val(null);
                    $scope.customer.Avata = undefined;
                    $scope.goInfoUser();
                }
                else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent(data.meta.error_message)
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }).error(function (data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            });
        });
    };
    $scope.onSubmit = function (kk) {
        if ($scope.customer?.FullName === '' || $scope.customer?.FullName === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập tên người dùng!')
                    .ok('Đóng')
                    .fullscreen(false)
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
        if ($scope.customer.StudentCode === '' || $scope.customer.StudentCode === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập mã học sinh viên!')
                    .ok('Đóng')
                    .fullscreen(false)
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
        if ($scope.customer.Sex === '' || $scope.customer.Sex === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa chọn giới tính!')
                    .ok('Đóng')
                    .fullscreen(false)
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
        if ($scope.customer.SchoolCode === '' || $scope.customer.SchoolCode === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa chọn trường!')
                    .ok('Đóng')
                    .fullscreen(false)
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
        if ($scope.customer.StudentYear === '' || $scope.customer.StudentYear === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập khóa học!')
                    .ok('Đóng')
                    .fullscreen(false)
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
        if ($scope.customer.StudentClass === '' || $scope.customer.StudentClass === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập lớp học!')
                    .ok('Đóng')
                    .fullscreen(false)
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
        if ($scope.customer.SchoolCode == 3) {
            if ($scope.customer.Note === '' || $scope.customer.Note === undefined) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Chưa nhập tên trường!')
                        .ok('Đóng')
                        .fullscreen(false)
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
        }
        if ($scope.customer.Email === '' || $scope.customer.Email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập Email!')
                    .ok('Đóng')
                    .fullscreen(false)
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
        if ($scope.customer.Phone === '' || $scope.customer.Phone === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập số điện thoại!')
                    .ok('Đóng')
                    .fullscreen(false)
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
        if ($scope.linkFB === '' || $scope.linkFB === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập đường dẫn Facebook!')
                    .ok('Đóng')
                    .fullscreen(false)
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

        if ($scope.disableFB) {
            $scope.SocialNetWorks.push({ id: 1, link: $scope.linkFB });
        }
        if ($scope.disableIns) {
            $scope.SocialNetWorks.push({ id: 2, link: $scope.linkIns });
        }
        if ($scope.disableTiktok) {
            $scope.SocialNetWorks.push({ id: 3, link: $scope.linkTiktok });
        }
        cfpLoadingBar.start();
        var obj = angular.copy($scope.customer);

        obj.SocialNetWorks = JSON.stringify($scope.SocialNetWorks);// haohv

        if (vcRecaptchaService.getResponse() === "") {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông tin')
                    .textContent('Chưa xác thực người dùng!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        } else {
            var post = $http({
                method: 'POST',
                url: 'web/customer/new-register',
                data: obj,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });
            post.success(function successCallback(data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                if (data.meta.error_code === 200) {
                    if (kk == 1) {
                        $scope.showDialog()
                    }
                }
                else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent(data.meta.error_message)
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }).error(function (data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            });

        }
    };
    $scope.openPopup = function () {
        $mdDialog.show({
            templateUrl: '/dang-ky-thong-tin',
            controller: 'UserUnitController',
            parent: angular.element(document.body),
            clickOutsideToClose: true,
            fullscreen: false
        });
    };
    $scope.showDialog = function () {
        $mdDialog.show(
            $mdDialog.alert()
                .clickOutsideToClose(true)
                .title('Thông báo')
                .textContent('Cảm ơn bạn đã đăng ký tham dự chương trình Chiến Binh Khởi Nghiệp. Thông tin của bạn sẽ sớm được đăng tải trên Website của chương trình!')
                .ok('Đóng')
                .fullscreen(false)
        ).finally(function () {
            $window.location.href = '/';
        });
    };
    $scope.formatDate = function (dateString) {
        var dateObj = new Date(dateString);
        var options = {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
        };
        return dateObj.toLocaleString('en-US', options);
    };
    $scope.shareURL = window.location.href;
    $scope.encodedURL = 'https://www.facebook.com/sharer/sharer.php?u={{$scope.shareURL}}%2Fdocs%2Fplugins%2F&amp;src=sdkpreparse';

    $scope.setTime = function () {
        var currentDate = new Date();
        console.log(currentDate)
        // Đặt ngày giới hạn (vd: 31/8)
        var limitDate = new Date(currentDate.getFullYear(), 8, 10); // Tháng 7 vì JavaScript tính tháng từ 0 (0 - 11)

        // Kiểm tra nếu ngày hiện tại sau ngày giới hạn

    }

    $scope.changeValueSchool = function (object) {
        console.log(object);

        if (object) {
            console.log($scope.register)
            $scope.register = $scope.customer.reduce(function (acc, item) {
                if (item.SchoolCode === object.Id) {
                    acc.push(item);
                }
                return acc;
            }, []);
        } else {
            $scope.register = [];
        }

    };

    $scope.showScrollButton = false;

    // Xử lý sự kiện cuộn trang
    angular.element($window).bind('scroll', function () {
        var header = angular.element(document.getElementById("header"));
        var headerOffset = header.offset().top;
        var scrollTop = angular.element($window).scrollTop();


        if ($window.pageYOffset > 100) {
            $scope.isSticky = true;
            $scope.showScrollButton = true;
        } else {
            $scope.isSticky = false;
            $scope.showScrollButton = false;
        }
        $scope.$apply();
    });

    // Hàm xử lý khi nhấp vào nút "Quay lại đầu trang"
    $scope.scrollToTop = function () {
        $window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    };

    $scope.SubmitOTP = function () {
        console.log("Test Submit")
        if ($scope.RegisterCode?.phone_number === '' || $scope.RegisterCode?.phone_number === undefined) {
            $scope.isOTP = true;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập số điện thoại')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        var obj = angular.copy({ phone_number: $scope.RegisterCode.phone_number });
        var post = $http({
            method: 'POST',
            url: '//apigw.cashplus.vn/api/app/auth/sendOTPRegister',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.code == 200) {
                if (data.data.is_account === false) {
                    $scope.disableButton = true;
                    var inputField = document.querySelector('[ng-click="SubmitOTP()"]');
                    console.log(inputField.style.cssText)
                    if (inputField.style.cssText === 'pointer-events: all;') {
                        inputField.style.cssText = 'pointer-events: none;';
                    } else {
                        inputField.style.cssText = 'pointer-events: all;';
                    }

                    $scope.isOTP = false;
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent('Đã gửi mã OTP. Bạn hãy kiểm tra OTP ở điện thoại và nhập mã vào ô bên dưới!')
                            .ok('Đóng')
                            .fullscreen(false)
                    );

                    if ($scope.disableButton == true) {
                        $scope.onTime();
                    }
                }
                else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent('Số điện thoại đã được đăng ký tài khoản khác! Vui lòng nhập lại số điện thoại.')
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }

            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent(data.error)
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });
    }

    $scope.onTime = function () {
        console.log($scope.disableButton);
        var timer = setInterval(function () {
            $scope.countdown -= 1;
            $scope.$apply(); // Cập nhật giá trị trên giao diện

            if ($scope.countdown === 0) {
                clearInterval(timer);
                $scope.disableButton = false; // Kích hoạt lại nút
                document.getElementById("idspan").innerHTML = '<span id="idspan" class="text-danger text-time" ng-if="disableButton" ng-bind-html="countdown"></span>';
                $scope.countdown = 60;
                var inputField = document.querySelector('[ng-click="SubmitOTP()"]');
                if (inputField.style.cssText === 'pointer-events: all;') {
                    inputField.style.cssText = 'pointer-events: none;';
                } else {
                    inputField.style.cssText = 'pointer-events: all;';
                }
                return;
            }
        }, 1000);
    };

    $scope.SubmitContact = function () {
        if ($scope.contact.FullName === '' || $scope.contact?.FullName === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập họ tên!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.contact.Email === '' || $scope.contact?.Email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập email!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.contact.Phone === '' || $scope.contact?.Phone === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Số điện thoại không hợp lệ!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.contact.NewsId === '' || $scope.contact?.NewsId === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn thành phố!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.contact.Title === '' || $scope.contact?.Title === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập lĩnh vực kinh doanh!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }

        var obj = $scope.contact
        var post = $http({
            method: 'POST',
            url: '/web/contact/SendContact',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        post.success(function successCallback(data) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Bạn đã gửi thông tin liên hệ thành công. Trong thời gian sớm nhất chúng tôi sẽ liên hệ và trao đổi với bạn. Cảm ơn bạn!')
                        .ok('Đóng')
                        .fullscreen(false)
                ).finally(function () {
                    $window.location.href = 'https://cashplus.vn';
                });
            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi! Xin vui lòng thử lại sau.')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });
    }
    /*Đăng ký tài khoản APP qua mã giới thiệu*/
    $scope.SubmitRegister = function () {
        if ($scope.RegisterCode?.phone_number === '' || $scope.RegisterCode?.phone_number === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số điện thoại')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCode?.otp_code === '' || $scope.RegisterCode?.otp_code === undefined) {
            $scope.isOTP = true;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mã OTP')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCode?.password === '' || $scope.RegisterCode?.password === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mật khẩu')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCode?.ConfirmPassword === '' || $scope.RegisterCode?.ConfirmPassword === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập lại mật khẩu')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCode?.ConfirmPassword !== $scope.RegisterCode?.password) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Mật khẩu nhập lại chưa chính xác. Vui lòng nhập lại!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCode?.phone_number === '' || $scope.RegisterCode?.phone_number === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số điện thoại')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterCode?.email === '' || $scope.RegisterCode?.email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số điện thoại')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.checkBox === false) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Vui lòng đồng ý với các điều khoản sử dụng và chính sách bảo mật của CashPlus')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        var obj = angular.copy($scope.RegisterCode);
        console.log(obj)
        var post = $http({
            method: 'POST',
            url: '//apigw.cashplus.vn/api/app/auth/register',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.error === 200) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Xin chúc mừng, bạn đã đăng ký tài khoản thành công!')
                        .ok('Đóng')
                        .fullscreen(false)
                ).finally(function () {
                    $window.location.href = 'https://cashplus.vn';
                });
            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Khách hàng đã tồn tại')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });
    }

    $scope.getQRInfo = function () {
        var urlParams = new URLSearchParams(window.location.search);
        var obj = urlParams.get('sharecode');

        var post = $http({
            method: 'POST',
            url: '//apigw.cashplus.vn/api/app/auth/checkShareCode',
            data: {
                "share_code": obj
            },
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.then(function (response) {
            $scope.QRInfo = response.data.data;
            $scope.RegisterCode.full_name = $scope.QRInfo.full_name;
        });
    };

    $scope.onRegister = function () {
        var urlParams = new URLSearchParams(window.location.search);
        $scope.QRCode = urlParams.get('sharecode');
        console.log($scope.QRCode)
        $scope.RegisterCode.share_code = $scope.QRCode;
        $scope.getQRInfo();
    };
    /*huy*/
    $scope.getTinTucBaoMat = function () {

        var get = $http({
            method: 'GET',
            url: 'https://apigw.cashplus.vn/api/staticpage/getByCode/cspl',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.dataBaomat = response.data.data.content;
        });
    };
    // thành phố
    $scope.getProvince = function () {

        var get = $http({
            method: 'GET',
            url: '//apigw.cashplus.vn/api/portal/province',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.Province = response.data.data;
        });
    };
    $scope.changeValueProvince = function (selectedItem) {
        if (selectedItem) {
            $scope.selectedProvince = selectedItem;
            $scope.getDistrict(selectedItem.id);
            return selectedItem;
        } else {
            $scope.selectedProvince = {};
        }
    };
    // phường
    $scope.getDistrict = function (selectedProvince) {

        var get = $http({
            method: 'GET',
            url: '//apigw.cashplus.vn/api/portal/provinceBy/' + selectedProvince,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.District = response.data.data;
        });
    };
    $scope.changeValueDistrict = function (selectedItem) {
        if (selectedItem) {
            $scope.selectedDictrict = selectedItem;
            $scope.getWard(selectedItem.id);
            return selectedItem;

        } else {
            $scope.selectedDictrict = {};
        }
    };
    // xã
    $scope.getWard = function (selectedDictrict) {

        var get = $http({
            method: 'GET',
            url: '//apigw.cashplus.vn/api/portal/provinceBy/' + selectedDictrict,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.Ward = response.data.data;
        });
    };
    $scope.changeValueWard = function (selectedItem) {
        console.log(selectedItem)
        if (selectedItem) {
            $scope.selectedWard = selectedItem;
            return selectedItem;
        } else {
            $scope.selectedWard = {};
        }
    };
    // quốc gia 
    // xã
    $scope.getCountries = function () {

        var get = $http({
            method: 'GET',
            url: '//apigw.cashplus.vn/api/portal/nation',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.Country = response.data.data;
        });
    };
    $scope.changeValueCountry = function (selectedItem) {
        console.log(selectedItem)
        if (selectedItem) {
            $scope.selectedCountry = selectedItem;
            return selectedItem;
        } else {
            $scope.selectedCountry = {};
        }
    };
    // mô hình kinh doanh
    $scope.getStoretype = function () {

        var get = $http({
            method: 'GET',
            url: '//apigw.cashplus.vn/api/portal/otherListByCode/store_type',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.Storetype = response.data.data;
        });
    };
    var mscn = document.getElementById("mscn");
    var msdn = document.getElementById("msdn");

    $scope.changeValueTypeStore = function (selectedItem) {
        console.log(selectedItem)
        if (selectedItem) {
            $scope.selectedStoreType = selectedItem;
            if (selectedItem.id == 7) {
                mscn.style.display = "block";
                msdn.style.display = "none";
            } else {
                msdn.style.display = "block";
                mscn.style.display = "none";
            }
        } else {
            $scope.selectedStoreType = {};
        }
    };
    // loại dịch vụ
    $scope.randomString = function (length) {
        const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
        let result = '';
        const charactersLength = characters.length;

        for (let i = 0; i < length; i++) {
            const randomIndex = Math.floor(Math.random() * charactersLength);
            result += characters.charAt(randomIndex);
        }

        return result;
    }
    $scope.getServicetype = function () {

        var get = $http({
            method: 'GET',
            url: '//apigw.cashplus.vn/api/portal/servicetype',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        get.then(function (response) {
            $scope.Servicetype = response.data.data;

        });
    };
    $scope.changeValueServicetype = function (selectedItem) {
        console.log(selectedItem)
        if (selectedItem) {
            $scope.selectedServicetype = selectedItem;
            console.log($scope.selectedServicetype);
            var get = $http({
                method: 'GET',
                url: '//apigw.cashplus.vn/api/portal/servicetype',
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });

            get.then(function (response) {
                $scope.Servicetype = response.data.data;
                $scope.codeservicetype = response.data.data.code;
                for (var i = 0; i < response.data.data.length; i++) {
                    if (response.data.data[i].id == selectedItem.id) {
                        $scope.code_partner = response.data.data[i].code + $scope.randomString(8);
                        $scope.discount_rate = response.data.data[i].discount_rate;
                        if ($scope.discount_rate == null) {
                            $scope.discount_rate = 0;
                        }
                    }
                }
            });
            return selectedItem;

        } else {
            $scope.selectedServicetype = {};
        }
    };
    //
    // lấy kinh độ vĩ độ

    $scope.getKinhdovido = function (item) {
        if ("geolocation" in navigator) {
            // Xác định vị trí

            navigator.geolocation.getCurrentPosition(function (position) {
                function showPosition(position) {

                }
                var latitude = position.coords.latitude;
                var longitude = position.coords.longitude;
                $scope.Vido = latitude;
                $scope.Kinhdo = longitude;
                console.log("Vĩ độ: " + latitude);
                console.log("Kinh độ: " + longitude);

                // Sử dụng dữ liệu vĩ độ và kinh độ ở đây
            });
        } else {
            console.log("Trình duyệt không hỗ trợ định vị.");
        }
    }




    $scope.getAddress = function () {
        $scope.getProvince();
        $scope.getStoretype();
        $scope.getServicetype();
        $scope.getCountries();
        $scope.Xacnhandkdt();
        $scope.getKinhdovido();
        $scope.datas = localStorage.getItem('PartnerInfo');
        $scope.UpdatePartner = JSON.parse($scope.datas)
        console.log($scope.UpdatePartner);
        $scope.getDistrict($scope.UpdatePartner.province_id);
        $scope.getWard($scope.UpdatePartner.district_id);
        //$scope.submitStoreInfo();
    };

    $scope.initContact = function () {
        $scope.getlistProvince();
        $scope.loadProvince();
    };

    /*huy*/
    $scope.goBack = function () {
        $window.history.back();
    };

    $scope.getTinTucMoi = function () {
        $scope.getSlide();
        var urlParams = new URLSearchParams(window.location.search);
        var newsId = urlParams.get('newsId');
        var get = $http({
            method: 'GET',
            url: '/web/news/GetNews',
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        $scope.baiviet = [];
        get.then(function (response) {
            $scope.dataTinTuc = response.data.data;
        });
    };

    $scope.getchitietTinTucMoi = function () {
        var urlParams = new URLSearchParams(window.location.search);
        var newsId = urlParams.get('newsId');
        var post = $http({
            method: 'GET',
            url: '/web/news/GetNewById' + newsId,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        post.then(function (response) {
            $scope.TinTuc = response.data.data;
            
        });
    };

    $scope.like = function () {
        console.log('Bạn đã like rồi!')
        var isLiked = localStorage.getItem('isLiked');
        if (isLiked) {
            alert('Bạn đã bình chọn rồi!');
            return;
        }


        $scope.likeUpdate();
    }

    $scope.likeUpdate = function () {
        var obj = $scope.register;
        obj.TypeThirdId = $scope.likeCount;
        var post = $http({
            method: 'PUT',
            url: '/web/customer/updateRegister/' + $scope.register.CustomerId,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                // Tăng số lượng like
                var likeCountElement = document.getElementById('likeCount');
                $scope.likeCount += 1;
                likeCountElement.textContent = $scope.likeCount;
                // Lưu trạng thái đã like vào Local Storage
                localStorage.setItem('isLiked', true);
            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent(data.meta.error_message)
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông tin')
                    .textContent(data.meta.error_message)
                    .ok('Đóng')
                    .fullscreen(false)
            );

        }).catch(function (error) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });
    };
    /*Kiểm tra đăng ký đối tác - Buoc 1*/
    $scope.SubmitRegisterPartner = function () {
        if ($scope.RegisterPartner?.name === '' || $scope.RegisterPartner?.name === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập tên cửa hàng')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterPartner?.store_owner === '' || $scope.RegisterPartner?.store_owner === undefined) {
            $scope.isOTP = true;
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập tên người đại diện')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterPartner?.phone === '' || $scope.RegisterPartner?.phone === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số điện thoại hoặc số điện thoại không đúng!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterPartner?.email === '' || $scope.RegisterPartner?.email === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập email')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterPartner?.store_type_id == '' || $scope.RegisterPartner?.store_type_id == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn mô hình kinh doanh!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.RegisterPartner?.address === '' || $scope.RegisterPartner?.address === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập địa chỉ')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        var obj = {
            'name': $scope.RegisterPartner.name,
            'store_owner': $scope.RegisterPartner.store_owner,
            'store_type_id': $scope.RegisterPartner.store_type_id,
            'phone': $scope.RegisterPartner.phone,
            'email': $scope.RegisterPartner.email,
            'address': $scope.RegisterPartner.address,
            'province_id': $scope.RegisterPartner.province_id,
            'district_id': $scope.RegisterPartner.district_id,
            'ward_id': $scope.RegisterPartner.ward_id,
        };
        var post = $http({
            method: 'POST',
            url: '//apigw.cashplus.vn/api/portal/registerStore',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.code == 200) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Đăng Ký Đối Tác')
                        .textContent('Bạn đã đăng ký thành công đối tác của CashPlus. Mời bạn kiểm tra email để hoàn thiện đăng ký và xác thực đối tác.')
                        .ok('Đóng')
                        .fullscreen(false)
                ).finally(function () {
                    $window.location.href = '/'
                });
            }
            else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo Đăng Ký Đối Tác')
                        .textContent('Đã có lỗi xảy ra khi đăng ký đối tác. Xin vui lòng thử lại!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo Đăng Ký Đối Tác')
                    .textContent('Đã có lỗi xảy ra khi đăng ký đối tác. Xin vui lòng thử lại!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });
    }
/*Xác thức mã đăng ký và email người giới thiệu - Bước 2*/
    $scope.Xacnhandkdt = function () {
        var urlParams = new URLSearchParams(window.location.search);
        $scope.logincode = urlParams.get('login_code');
        $scope.login_code = $scope.logincode;
        console.log($scope.login_code);
    }
    $scope.submitStoreInfo = function () {
        var obj = {
            'login_code': $scope.login_code,
            'support_person_email': $scope.partner?.support_person_email ? $scope.partner.support_person_email : '',
        };
        var post = $http({
            method: 'POST',
            url: '//apigw.cashplus.vn/api/portal/getStoreInfoByCode',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.code == 200) {
                $scope.UpdatePartner = JSON.stringify(data.data); 
                localStorage.setItem('PartnerInfo', $scope.UpdatePartner);
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Đăng Ký Đối Tác')
                        .textContent('Chúc mừng bạn đã gửi thông tin thành công. Mời bạn tiếp tục thêm các thông tin dưới đây để hoàn thành việc đăng ký!')
                        .ok('Đóng')
                        .fullscreen(false)
                ).finally(function () {
                    $window.location.href = '/nhap-thong-tin-dang-ky?login_code=' + $scope.login_code;
                });
            } else {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo Đăng Ký Đối Tác')
                        .textContent('Đã có lỗi trong quá trình đăng ký đối tác. Bạn vui lòng thử lại sau! '+ data.error)
                        .ok('Đóng')
                        .fullscreen(false)
                )
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo Đăng Ký Đối Tác')
                    .textContent('Đã có lỗi trong quá trình đăng ký đối tác. Bạn vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });
    }
/*Điền các thông tin đăng ký đối tác chuyên sâu - Bước 3*/
    $scope.submitupdateStore = function () {
        if ($scope.UpdatePartner?.name == '' || $scope.UpdatePartner?.name == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập tên đăng ký kinh doanh')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner?.service_type_id == '' || $scope.UpdatePartner?.service_type_id == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn loại dịch vụ')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner?.email == '' || $scope.UpdatePartner?.email == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập email')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.store_type_id == '' || $scope.UpdatePartner?.store_type_id == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn mô hình dịch vụ')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.phone == '' || $scope.UpdatePartner?.phone == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số điện thoại')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.store_owner == '' || $scope.UpdatePartner?.store_owner == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập tên người đại diện')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.start_hour == '' || $scope.UpdatePartner?.start_hour == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập giờ làm việc')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.end_hour == '' || $scope.UpdatePartner?.end_hour == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập giờ đóng cửa')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.license_date == '' || $scope.UpdatePartner?.license_date == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn ngày đăng ký kinh doanh')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.license_owner == '' || $scope.UpdatePartner?.license_owner == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập tên người sở hữu')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.license_person_number < 0) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Số người đồng sở hữu phải lớn hơn 0')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.license_birth_date == '' || $scope.UpdatePartner?.license_birth_date == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn ngày sinh')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.identifier_nation_id == '' || $scope.UpdatePartner?.identifier_nation_id == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn quốc tịch')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.identifier_province_id == '' || $scope.UpdatePartner?.identifier_province_id == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn thành phố')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.discount_rate == '' || $scope.discount_rate == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập phần trăm chiết khấu')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.discount_rate < 10) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Phầm trăm chiết khấu cần >= 10. Mời bạn nhập lại!')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.indetifier_no == '' || $scope.UpdatePartner.indetifier_no == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập số CMND/CCCD')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.identifier_date == '' || $scope.UpdatePartner?.identifier_date == undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn ngày cấp')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.identifier_at === '' || $scope.UpdatePartner?.identifier_at === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập nơi cấp')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.identifier_date_expire === '' || $scope.UpdatePartner?.identifier_date_expire === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn ngày hết hạn')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.username === '' || $scope.UpdatePartner?.username === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập tài khoản')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.password === '' || $scope.UpdatePartner?.password === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập mật khẩu')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.identifier_address === '' || $scope.UpdatePartner?.identifier_address === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa nhập địa chỉ thường trú')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        if ($scope.UpdatePartner.avatar === '' || $scope.UpdatePartner?.avatar === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Bạn chưa chọn ảnh đại diện để hiển thị trên App')
                    .ok('Đóng')
                    .fullscreen(false)
            )
            return;
        }
        var endhour = Date($scope.UpdatePartner.end_hour);
        var starthour = Date($scope.UpdatePartner.start_hour);
        $scope.Vido = document.getElementById("Vido").value;
        $scope.Kinhdo = document.getElementById("Kinhdo").value;
        var obj = {
            'login_code': $scope.login_code,
            'code': $scope.code_partner ? $scope.code_partner : '',
            'name': $scope.UpdatePartner.name ? $scope.UpdatePartner.name : '',
            'service_type_id': $scope.UpdatePartner.service_type_id ? $scope.UpdatePartner.service_type_id : '',
            'store_type_id': $scope.UpdatePartner.store_type_id ? $scope.UpdatePartner.store_type_id : '',
            'phone': $scope.UpdatePartner.phone ? $scope.UpdatePartner.phone : '',
            'email': $scope.UpdatePartner.email ? $scope.UpdatePartner.email : '',
            'store_owner': $scope.UpdatePartner.store_owner ? $scope.UpdatePartner.store_owner : '',
            'start_hour': starthour ? moment(starthour).format("hh:mm A") : '',
            'end_hour': endhour ? moment(endhour).format("hh:mm A") : '',
            'working_day': $scope.UpdatePartner.working_day ? $scope.UpdatePartner.working_day : '',
            'username': $scope.UpdatePartner.username ? $scope.UpdatePartner.username : '',
            'password': $scope.UpdatePartner.password ? $scope.UpdatePartner.password : '',
            'description': $scope.UpdatePartner.description ? $scope.UpdatePartner.description : '',
            'product_label_id': $scope.UpdatePartner.product_label_id ? $scope.UpdatePartner.product_label_id : '',
            'discount_rate': $scope.discount_rate ? $scope.discount_rate : 0,
            'province_id': $scope.UpdatePartner.province_id ? $scope.UpdatePartner.province_id : null,
            'district_id': $scope.UpdatePartner.district_id ? $scope.UpdatePartner.district_id : null,
            'ward_id': $scope.UpdatePartner.ward_id ? $scope.UpdatePartner.ward_id : null,
            'address': $scope.UpdatePartner.address ? $scope.UpdatePartner.address : null,
            'latitude': $scope.Vido ? $scope.Vido : null,
            'longtitude': $scope.Kinhdo ? $scope.Kinhdo : null,
            'license_no': $scope.UpdatePartner.license_no,
            'license_person_number': $scope.UpdatePartner.license_person_number ? $scope.UpdatePartner.license_person_number : 0,
            'license_image': $scope.UpdatePartner.license_image,
            'license_date': $scope.UpdatePartner.license_date ? moment($scope.UpdatePartner.license_date).format('DD/MM/YYYY') : '',
            'license_owner': $scope.UpdatePartner.license_owner ? $scope.UpdatePartner.license_owner : '',
            'license_birth_date': $scope.UpdatePartner.license_birth_date ? moment($scope.UpdatePartner.license_birth_date).format('DD/MM/YYYY') : '',
            'license_nation_id': $scope.UpdatePartner.license_nation_id ? $scope.UpdatePartner.license_nation_id : null,
            'indetifier_no': $scope.UpdatePartner.indetifier_no ? $scope.UpdatePartner.indetifier_no : 0,
            'identifier_date': $scope.UpdatePartner.identifier_date ? moment($scope.UpdatePartner.identifier_date).format('DD/MM/YYYY') : '',
            'identifier_at': $scope.UpdatePartner.identifier_at ? $scope.UpdatePartner.identifier_at : '',
            'identifier_date_expire': $scope.UpdatePartner.identifier_date_expire ? moment($scope.UpdatePartner.identifier_date_expire).format('DD/MM/YYYY') : '',
            'identifier_address': $scope.UpdatePartner.identifier_address ? $scope.UpdatePartner.identifier_address : '',
            'identifier_nation_id': $scope.UpdatePartner.identifier_nation_id ? $scope.UpdatePartner.identifier_nation_id : null,
            'identifier_province_id': $scope.UpdatePartner.identifier_province_id ? $scope.UpdatePartner.identifier_province_id : null,
            'is_same_address': $scope.UpdatePartner.is_same_address ? $scope.UpdatePartner.is_same_address : false,
            'now_address': $scope.UpdatePartner.now_address ? $scope.UpdatePartner.now_address : '',
            'now_province_id': $scope.UpdatePartner.now_province_id ? $scope.UpdatePartner.now_province_id : null,
            'identifier_front_image': $scope.UpdatePartner.identifier_front_image ? $scope.UpdatePartner.identifier_front_image : '',
            'identifier_back_image': $scope.UpdatePartner.identifier_back_image ? $scope.UpdatePartner.identifier_back_image : '',
            'avatar': $scope.UpdatePartner.avatar ? $scope.UpdatePartner.avatar : '',
            'list_documents': $scope.UpdatePartner.list_documents ? $scope.UpdatePartner.list_documents : '',
            'now_address': $scope.UpdatePartner.now_address ? $scope.UpdatePartner.now_address : '',
            'now_nation_id': $scope.UpdatePartner.now_nation_id ? $scope.UpdatePartner.now_nation_id : '',
            'now_province_id': $scope.UpdatePartner.now_province_id ? $scope.UpdatePartner.now_province_id : '',
        };
        var post = $http({
            method: 'POST',
            url: '//apigw.cashplus.vn/api/portal/updateInfoStore',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });
        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.code == 200) {
                console.log(data)
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Đăng Ký Đối Tác Thành Công')
                        .textContent('Cảm ơn bạn đã cập nhật thông tin đăng ký đối tác thành công. Trong thời gian sớm nhất, bộ phận chuyên môn của CashPlus sẽ liên hệ hỗ trợ bạn hoàn thiện hợp đồng!')
                        .ok('Đóng')
                        .fullscreen(false)
                ).finally(function () {
                    $window.location.href = '/'
                });
            }
        }).error(function (data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                    .ok('Đóng')
                    .fullscreen(false)
            );
        });
    }
    $scope.uploadAvatarPartner = function (e) {
        if (e === undefined) return;
        if (e.files.length <= 0) return;
        var avatarpartner = document.getElementById("avatarpartner");
        var fd = new FormData();
        fd.append("files", e.files[0]);
        console.log(fd);
        cfpLoadingBar.start();
        var post = $http({
            method: 'POST',
            url: 'https://apigw.cashplus.vn/api/upload/uploadfile',
            data: fd,
            headers: {
                "Content-Type": undefined
            }
        });
        // haohv
        // Thêm đường dẫn cho ảnh
        post.success(function successCallback(data, status, headers, config) {
            post.success(function successCallback(data, status, headers, config) {
                cfpLoadingBar.complete();
                if (data.status === 200) {
                    $scope.UpdatePartner.avatar = 'download/loyalty/' + data.data[0].name;
                    avatarpartner.style.display = "block";
                }
                else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông báo')
                            .textContent('Tải ảnh đại diện không thành công')
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }).error(function (data, status, headers, config) {
                cfpLoadingBar.complete();
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            });

        });
    };


    $scope.uploadDKKDPartner = function (e) {
        if (e === undefined) return;
        if (e.files.length <= 0) return;
        var anhgpdk = document.getElementById("uploadedImage4");
        var anhgpdkupload = document.getElementById("preview4");
        var fd = new FormData();
        fd.append("files", e.files[0]);
        console.log(fd);
        cfpLoadingBar.start();
        var post = $http({
            method: 'POST',
            url: 'https://apigw.cashplus.vn/api/upload/uploadfile',
            data: fd,
            headers: {
                "Content-Type": undefined
            }
        });
        // haohv 

        post.success(function successCallback(data, status, headers, config) {
            post.success(function successCallback(data, status, headers, config) {
                cfpLoadingBar.complete();
                if (data.status === 200) {
                    anhgpdk.style.display = "none";
                    anhgpdkupload.style.display = "block";
                    $scope.UpdatePartner.license_image = 'download/loyalty/' + data.data[0].name;
                }
                else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông báo')
                            .textContent('Tải giấy phép kinh doanh không thành công')
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }).error(function (data, status, headers, config) {
                cfpLoadingBar.complete();
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            });

        });
    };

    $scope.UpdatePartner.list_documents = [];
    $scope.uploadFilePartner = function (e) {
        if (e === undefined) return;
        if (e.files.length <= 0) return;
        var anhfiles = document.getElementById("imgContainer2");
        var fd = new FormData();
        fd.append("files", e.files[0]);
        console.log(fd);
        cfpLoadingBar.start();
        var post = $http({
            method: 'POST',
            url: 'https://apigw.cashplus.vn/api/upload/uploadfile',
            data: fd,
            headers: {
                "Content-Type": undefined
            }
        });
        // haohv 

        post.success(function successCallback(data, status, headers, config) {
            post.success(function successCallback(data, status, headers, config) {
                cfpLoadingBar.complete();
                if (data.status === 200) {
                    anhfiles.style.display = "none";
                    $scope.multifiles = {};
                    const uploadedFile = data.data;

                    for (var i = 0; i < uploadedFile.length; i++) {
                        const fileName = "download/loyalty/" + uploadedFile[i];
                        $scope.UpdatePartner.list_documents.push(fileName);
                        console.log($scope.UpdatePartner.list_documents);
                    }
                }
                else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông báo')
                            .textContent('Tải tài liệu không thành công')
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }).error(function (data, status, headers, config) {
                cfpLoadingBar.complete();
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            });

        });
    };

    $scope.uploadCMNDMT = function (e) {
        if (e === undefined) return;
        if (e.files.length <= 0) return;
        var cmndmt = document.getElementById("cccd1");
        var cmnd1 = document.getElementById("uploadedImage2");
        var fd = new FormData();
        fd.append("files", e.files[0]);
        cfpLoadingBar.start();
        var post = $http({
            method: 'POST',
            url: 'https://apigw.cashplus.vn/api/upload/uploadfile',
            data: fd,
            headers: {
                "Content-Type": undefined
            }
        });
        // haohv 

        post.success(function successCallback(data, status, headers, config) {
            post.success(function successCallback(data, status, headers, config) {
                cfpLoadingBar.complete();
                if (data.status == 200) {
                    $scope.UpdatePartner.identifier_front_image = 'download/loyalty/' + data.data[0].name;
                    cmndmt.style.display = "block";
                    cmnd1.style.display = "none";
                }
                else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông báo')
                            .textContent('Tải ảnh CMND mặt trước không thành công')
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }).error(function (data, status, headers, config) {
                cfpLoadingBar.complete();
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            });

        });
    };
    $scope.uploadCMNDMS = function (e) {
        if (e === undefined) return;
        if (e.files.length <= 0) return;
        var cmndmt = document.getElementById("cccd2");
        var cmnd1 = document.getElementById("uploadedImage3");
        var fd = new FormData();
        fd.append("files", e.files[0]);
        cfpLoadingBar.start();
        var post = $http({
            method: 'POST',
            url: 'https://apigw.cashplus.vn/api/upload/uploadfile',
            data: fd,
            headers: {
                "Content-Type": undefined
            }
        });
        // haohv 

        post.success(function successCallback(data, status, headers, config) {
            post.success(function successCallback(data, status, headers, config) {
                cfpLoadingBar.complete();
                if (data.status == 200) {
                    $scope.UpdatePartner.identifier_back_image = 'download/loyalty/' + data.data[0].name;
                    cmndmt.style.display = "block";
                    cmnd1.style.display = "none";
                }
                else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông báo')
                            .textContent('Tải ảnh CMND mặt sau không thành công')
                            .ok('Đóng')
                            .fullscreen(false)
                    );
                }
            }).error(function (data, status, headers, config) {
                cfpLoadingBar.complete();
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Đã xảy ra lỗi. Xin vui lòng thử lại sau!')
                        .ok('Đóng')
                        .fullscreen(false)
                );
            });

        });
    };


    $scope.updateNowAddress = function () {

        if ($scope.UpdatePartner.is_same_address && $scope.UpdatePartner.is_same_address == true) {
            $scope.UpdatePartner.now_address = $scope.UpdatePartner.identifier_address;
            $scope.UpdatePartner.now_nation_id = $scope.UpdatePartner.identifier_nation_id;
            $scope.UpdatePartner.now_province_id = $scope.UpdatePartner.identifier_province_id;
        } else {
            $scope.UpdatePartner.now_address = $scope.UpdatePartner.now_address;
            $scope.UpdatePartner.now_nation_id = $scope.UpdatePartner.now_nation_id;
            $scope.UpdatePartner.now_province_id = $scope.UpdatePartner.now_province_id;
        }
    };

}]);

