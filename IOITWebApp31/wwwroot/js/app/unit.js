
myApp.controller('UnitController', ['$scope', '$http', '$mdDialog', 'config', 'cfpLoadingBar', 'app', '$cookies', '$rootScope', '$window', 'ngDialog', '$uibModal', function UnitController($scope, $http, $mdDialog, config, cfpLoadingBar, app, $cookies, $rootScope, $window, ngDialog, $uibModal) {
    $scope.page = 1;
    $scope.page_size = 10;
    $scope.query = "1=1";
    $scope.q = {};
    $scope.orderby = "";
    $scope.item_count = 0;
    $scope.IdFile = 0;
    $scope.disableBtn = { btSubmit: false };
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

    $scope.init = function (data, id) {
        cfpLoadingBar.start();
        if (data != undefined) {
            $scope.customerId = data.CustomerId;
            $scope.access_token = data.access_token;
           
        }
        $scope.q = {};
        $scope.unit = {};
        $scope.unit.StatusView = true;

        if (id != undefined) {
            $scope.unitId = id;
            $scope.loadDataById();
        }
        else {
            $scope.loadProvince();
            $scope.loadUnit();
            $scope.loadData();
        }
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
            url: '/web/unit/getByPagePost',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $scope.listUnits = data.data;
                $scope.item_count = data.metadata;
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

        $http.get("/web/unit/GetById/" + $scope.unitId, {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.unit = angular.copy(data.data.data);
                $scope.unit.StatusView = $scope.unit.Status == 1 ? true : false;
                $scope.listDistict= [];
                $scope.listWards = [];
                $scope.loadUnit();
                $scope.loadProvince();
                $scope.loadDistrict();
                $scope.loadWards();
            }
        });
    };

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

    $scope.loadDistrict = function () {
        var query = "ProvinceId=" + $scope.unit.ProvinceId;
        $http.get("/web/other/listDistrict?page=1&query=" + query + "&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listDistrict = data.data.data;
                if ($scope.unitId != undefined) {
                    for (var i = 0; i < $scope.listDistrict.length; i++) {
                        if ($scope.listDistrict[i].DistrictId == $scope.unit.DistrictId) {
                            $scope.dataDistrict = $scope.listDistrict[i];
                            break;
                        }
                    }
                }
            }
        });
    };

    $scope.loadWards = function () {
        var query = "DistrictId=" + $scope.unit.DistrictId;
        $http.get("/web/other/listWards?page=1&query=" + query + "&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listWards = data.data.data;
                if ($scope.unitId != undefined) {
                    for (var i = 0; i < $scope.listWards.length; i++) {
                        if ($scope.listWards[i].WardId == $scope.unit.WardId) {
                            $scope.dataWards = $scope.listWards[i];
                            break;
                        }
                    }
                }
            }
        });
    };

    $scope.loadCountry = function () {
        //var query = "TypeCategoryId=14";
        $http.get("/web/other/listCountry?page=1&query=1=1&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listCountry = data.data.data;
            }
        });
    };

    $scope.loadUnit = function () {
        //var query = "1=1";
        $http.get("/web/unit/listUnit", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listUnits = data.data.data;
                if ($scope.unitId != undefined) {
                    for (var i = 0; i < $scope.listUnits.length; i++) {
                        if ($scope.listUnits[i].UnitId == $scope.unit.UnitParentId) {
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
                $scope.unit.UnitParentId = object ? object.UnitId : null;
                break;
            case 2:
                $scope.unit.ProvinceId = object ? object.ProvinceId : null;
                $scope.loadDistrict();
                break;
            case 3:
                $scope.unit.DistrictId = object ? object.DistrictId : null;
                $scope.loadWards();
                break;
            case 4:
                $scope.unit.WardId = object ? object.WardId : null;
                break;
            case 5:
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
            query += ' AND (Name.Contains("' + $scope.q.txtSearch + '") or Code.Contains("' + $scope.q.txtSearch + '") or ShortName.Contains("' + $scope.q.txtSearch + '"))';
        }
        $scope.query = query;
        $scope.loadData();
    }

    $scope.goManagerUnit = function () {
        $window.location.href = '/danh-sach-to-chuc';
    };

    $scope.SaveData = function (status) {
        if ($scope.unit.UnitParentId === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa chọn cơ quan/tổ chức liên trên')
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
        if ($scope.unit.Code === '' || $scope.unit.Code === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập mã định danh cơ quan/tổ chức')
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
        if ($scope.unit.Name === '' || $scope.unit.Name === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập tên cơ quan/tổ chức!')
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

        $scope.disableBtn.btSubmit = true;
        cfpLoadingBar.start();
        var obj = angular.copy($scope.unit);
        obj.CreatedId = $scope.customerId;
        obj.UpdatedId = $scope.customerId;
        obj.Status = obj.StatusView ? 1 : 10;
        console.log($scope.unit.DateNumber);
        if (obj.DateNumber != undefined && obj.DateNumber != '') {
            var dateFull = new Date(obj.DateNumber);
            obj.DateNumber = dateFull.getFullYear() + "/" + (dateFull.getMonth() + 1) + "/" + dateFull.getDate();
        }

        if ($scope.unit.UnitId == undefined) {
            var post = $http({
                method: 'POST',
                url: '/web/unit',
                data: obj,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });

            post.success(function successCallback(data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                if (data.meta.error_code === 200) {
                    $scope.goManagerUnit();
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
        else {
            var post = $http({
                method: 'PUT',
                url: '/web/unit/' + obj.UnitId,
                data: obj,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });

            post.success(function successCallback(data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                if (data.meta.error_code === 200) {
                    $scope.goManagerUnit();
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
                url: '/web/unit/' + item.UnitId,
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

    $scope.uploadLogo = function (e) {
        if (e === undefined) return;
        if (e.files.length <= 0) return;

        var fd = new FormData();
        fd.append("file", e.files[0]);
        cfpLoadingBar.start();
        var post = $http({
            method: 'POST',
            url: '/web/upload/uploadMedia/8',
            data: fd,
            headers: {
                "Content-Type": undefined,
                'Authorization': 'bearer ' + $scope.access_token
            }
        });

        post.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $scope.unit.Image = data.data[0];
                //var oFReader = new FileReader();
                //oFReader.readAsDataURL(document.getElementById("uploadImage").files[0]);
                //oFReader.onload = function (oFREvent) {
                //    document.getElementById("uploadPreview").src = oFREvent.target.result;
                //};
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