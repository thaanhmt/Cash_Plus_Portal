
myApp.controller('NotificationController', ['$scope', '$http', '$mdDialog', 'config', 'cfpLoadingBar', 'app', '$cookies', '$rootScope', '$window', 'ngDialog', '$uibModal', function NotificationController($scope, $http, $mdDialog, config, cfpLoadingBar, app, $cookies, $rootScope, $window, ngDialog, $uibModal) {
    $scope.page = 1;
    $scope.page_size = 10;
    $scope.query = "1=1";
    $scope.q = {};
    $scope.orderby = "";
    $scope.item_count = 0;
    $scope.listNotification = [];
    $scope.listCheck = [];
    $scope.disableBtn = { btSubmit: false };

    $scope.init = function (data, id) {
        cfpLoadingBar.start();
        if (data != undefined) {
            $scope.customerId = data.CustomerId;
            $scope.access_token = data.access_token;
           
        }
        $scope.q = {};
        $scope.notification = {};

        if (id != undefined) {
            $scope.userId = id;
            $scope.loadDataById();
        }
        else {
            $scope.loadData();
            $scope.loadSetting();
        }
    };

    $scope.loadData = function () {
        //$scope.q.page = $scope.page;
        //$scope.q.page_size = $scope.page_size;
        //$scope.q.query = $scope.query;
        //$scope.q.orderby = $scope.orderby;
        //var obj = angular.copy($scope.q);
        //if (obj.DateStart != undefined && obj.DateStart != '') {
        //    obj.DateStart = obj.DateStart.getFullYear() + "/" + (obj.DateStart.getMonth() + 1) + "/" + obj.DateStart.getDate();
        //}
        //if (obj.DateEnd != undefined && obj.DateEnd != '') {
        //    obj.DateEnd = obj.DateEnd.getFullYear() + "/" + (obj.DateEnd.getMonth() + 1) + "/" + obj.DateEnd.getDate();
        //}
        //var post = $http({
        //    method: 'POST',
        //    url: '/web/customer/GetByPagePost',
        //    data: obj,
        //    headers: { 'Authorization': 'bearer ' + $scope.access_token }
        //});

        //post.success(function successCallback(data, status, headers, config) {
        //    $scope.disableBtn.btSubmit = false;
        //    cfpLoadingBar.complete();
        //    if (data.meta.error_code === 200) {
        //        $scope.listCustomers = data.data;
        //        $scope.item_count = data.metadata;
        //    }
        //    else {
        //        $mdDialog.show(
        //            $mdDialog.alert()
        //                .clickOutsideToClose(true)
        //                .title('Thông tin')
        //                .textContent(data.meta.error_message)
        //                .ok('Đóng')
        //                .fullscreen(true)
        //        );
        //    }
        //}).error(function (data, status, headers, config) {
        //    $scope.disableBtn.btSubmit = false;
        //    cfpLoadingBar.complete();
        //    $mdDialog.show(
        //        $mdDialog.alert()
        //            .clickOutsideToClose(true)
        //            .title('Thông báo')
        //            .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
        //            .ok('Đóng')
        //            .fullscreen(true)
        //    );
        //});

        $http.get("/web/notification/GetByPage?page=" + $scope.page + "&page_size=" + $scope.page_size + "&query=" + $scope.query + "&order_by=" + $scope.orderby, {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listNotification = data.data.data;
                for (let i = 0; i < $scope.listNotification.length; i++) {
                    for (let j = 0; j < $scope.listCheck.length; j++) {
                        if ($scope.listNotification[i].NotificationId == $scope.listCheck[j]) {
                            $scope.listNotification[i].Action = true;
                            break;
                        }
                    }
                }
                $scope.item_count = data.data.metadata.Sum;
                $scope.numberRead = data.data.metadata.Normal;
                $scope.numberNotRead = data.data.metadata.Temp;
            }
        });
    };

    $scope.loadSetting = function () {

        $http.get("/web/notification/getSetting/" + $scope.customerId, {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.notification = angular.copy(data.data.data);
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
                $scope.customer.CountryId = object ? object.CountryId : null;
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

    $scope.onQueryChange = function (type) {
        var query = '1=1';
        if (type != 0) {
            query += ' AND Status=' + type;
        }
        //if ($scope.q.txtSearch != undefined && $scope.q.txtSearch != '') {
        //    query += ' AND (FullName.Contains("' + $scope.q.txtSearch + '") or Email.Contains("' + $scope.q.txtSearch + '") or Phone.Contains("' + $scope.q.txtSearch + '"))';
        //}
        $scope.query = query;
        $scope.loadData();
    }

    $scope.goManagerNotifi = function () {
        $window.location.href = '/quan-ly-thong-bao';
    };

    $scope.ChangeSetting = function (status) {
        cfpLoadingBar.start();
        var obj = angular.copy($scope.notification);

        var post = $http({
            method: 'PUT',
            url: '/web/notification/changeSetting/' + $scope.customerId,
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
                        .title('Thông báo')
                        .textContent('Thay đổi cấu hình thông báo thành công!')
                        .ok('Đóng')
                        .fullscreen(true)
                );
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
    };

    $scope.ReadData = function () {
        var confirm = $mdDialog.confirm()
            .title('Thông báo')
            .textContent('Bạn có chắc chắn muốn đánh dấu đã đọc các bản ghi đã chọn?')
            .ok('Đồng ý')
            .cancel('Hủy');

        $mdDialog.show(confirm).then(function () {
            var obj = angular.copy($scope.listCheck);
            var remove = $http({
                method: 'PUT',
                url: '/web/notification/readNotifications/' + $scope.customerId,
                data: obj,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });

            remove.success(function (data, status, headers, config) {
                if (data.meta.error_code == 200) {
                    //$mdToast.show($mdToast.simple()
                    //    .textContent('Đánh dấu thành công!')
                    //    .position('fixed bottom right')
                    //    .hideDelay(3000));
                    $scope.goManagerNotifi();
                }
                else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
                            .ok('Đóng')
                            .fullscreen(true)
                    );
                }
            }).error(function () {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
                        .ok('Đóng')
                        .fullscreen(true)
                );
            });
        });
    }

    $scope.ReadDataAll = function () {
        var confirm = $mdDialog.confirm()
            .title('Thông báo')
            .textContent('Bạn có chắc chắn muốn đánh dấu tất cả đã đọc?')
            .ok('Đồng ý')
            .cancel('Hủy');

        $mdDialog.show(confirm).then(function () {
            var obj = angular.copy($scope.listCheck);
            var remove = $http({
                method: 'PUT',
                url: '/web/notification/readAllNotifications/' + $scope.customerId,
                data: obj,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });

            remove.success(function (data, status, headers, config) {
                if (data.meta.error_code == 200) {
                    //$mdToast.show($mdToast.simple()
                    //    .textContent('Đánh dấu thành công!')
                    //    .position('fixed bottom right')
                    //    .hideDelay(3000));
                    $scope.goManagerNotifi();
                }
                else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
                            .ok('Đóng')
                            .fullscreen(true)
                    );
                }
            }).error(function () {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
                        .ok('Đóng')
                        .fullscreen(true)
                );
            });
        });
    }

    $scope.DeleteData = function () {
        var confirm = $mdDialog.confirm()
            .title('Thông báo')
            .textContent('Bạn có chắc chắn muốn xóa các bản ghi đã chọn?')
            .ok('Đồng ý')
            .cancel('Hủy');

        $mdDialog.show(confirm).then(function () {
            var obj = angular.copy($scope.listCheck);
            var remove = $http({
                method: 'PUT',
                url: '/web/notification/deleteNotifications/' + $scope.customerId,
                data: obj,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });

            remove.success(function (data, status, headers, config) {
                if (data.meta.error_code == 200) {
                    //$mdToast.show($mdToast.simple()
                    //    .textContent('Đánh dấu thành công!')
                    //    .position('fixed bottom right')
                    //    .hideDelay(3000));
                    $scope.goManagerNotifi();
                }
                else {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Thông tin')
                            .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
                            .ok('Đóng')
                            .fullscreen(true)
                    );
                }
            }).error(function () {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông tin')
                        .textContent('Đã xả ra lỗi! Xin vui lòng thử lại sau.')
                        .ok('Đóng')
                        .fullscreen(true)
                );
            });
        });
    }

    $scope.CheckActionTable = function (item) {
        //if (UserId == undefined) {
        //    let CheckAll = this.CheckAll;
        //    this.listUser.forEach(item => {
        //        item.Action = CheckAll;
        //    });
        //}
        //else {
        let check = false;
        for (let i = 0; i < $scope.listCheck.length; i++) {
            if ($scope.listCheck[i] == item) {
                check = true;
                //Xóa phần tử ra khỏi list
                $scope.listCheck.splice(i, 1);
                break;
            }
        }
        //thêm vào list
        if (!check)
            $scope.listCheck.push(item);

        console.log($scope.listCheck);

    }

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

}]);