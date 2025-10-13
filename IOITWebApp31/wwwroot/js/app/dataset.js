
myApp.controller('DataSetController', ['$scope', '$http', '$mdDialog', 'config', 'cfpLoadingBar', 'app', '$cookies', '$rootScope', '$window', 'ngDialog', '$uibModal', '$sce', function DataSetController($scope, $http, $mdDialog, config, cfpLoadingBar, app, $cookies, $rootScope, $window, ngDialog, $uibModal, $sce) {
    $scope.page = 1;
    $scope.page_size = 10;
    $scope.query = "1=1";
    $scope.q = {};
    $scope.orderby = "";
    $scope.item_count = 0;
    $scope.IdFile = 0;
    $scope.disableBtn = { btSubmit: false };
    $scope.listConfirmStatus = [
        {
            "Id": 1,
            "Name": "Duyệt"
        },
        {
            "Id": 2,
            "Name": "Không duyệt"
        }
    ];
    $scope.listDataStatus = [
        {
            "Id": 10,
            "Name": "Tạo mới"
        },
        {
            "Id": 3,
            "Name": "Chờ phê duyệt"
        },
        {
            "Id": 2,
            "Name": "Đã duyệt"
        },
        {
            "Id": 1,
            "Name": "Đã công khai"
        },
        {
            "Id": 4,
            "Name": "Không duyệt"
        }
    ];
    //
    $scope.page_ceph = 1;
    $scope.page_size_ceph = 10;
    $scope.query_ceph = "1=1";
    $scope.item_count_ceph = 0;
    //CKEDITOR.replace('contents');
    //CKEDITOR.instances.editor1.on('change', function () {
    //    $scope.$apply(function () {
    //        $scope.myContent = CKEDITOR.instances.contents.getData();
    //    });
    //});
    $scope.init = function (data, id) {
        cfpLoadingBar.start();
        $scope.isViewCeph = true;
        //
        $scope.isViewKuberflow = false;
        $scope.isViewSparkWorker = false;
        $scope.isViewSparkMaster = false;
        $scope.isViewNifi = false;
        $scope.isViewAirByte = false;
        //
        if (data != undefined) {
            $scope.customerId = data.CustomerId;
            $scope.access_token = data.access_token;
        }
        $scope.q = {};
        $scope.confirmData = {};
        $scope.dataSet = {};
        $scope.dataSet.listFiles = [];
        $scope.loadData();
        $scope.dataExtentions = undefined;

        if (id != undefined) {
            $scope.dataSetId = id;
            $scope.loadDataById();
        }
        else {
            $scope.loadApplicationRangeTree();
            $scope.loadResearchAreaTree();
            $scope.loadDataExtentions();
            $scope.loadDataLicecses();
            $scope.loadDataFolders();
        }
    };

    $scope.loadData = function () {
        $scope.q.page = $scope.page;
        $scope.q.page_size = $scope.page_size;
        $scope.q.query = $scope.query;
        $scope.q.orderby = $scope.orderby;
        if ($scope.q.ApplicationRangeId == undefined) $scope.q.ApplicationRangeId = -1;
        if ($scope.q.ResearchAreaId == undefined) $scope.q.ResearchAreaId = -1;
        if ($scope.q.Extention == undefined) $scope.q.Extention = -1;
        var obj = angular.copy($scope.q);
        if (obj.DateStart != undefined && obj.DateStart != '') {
            obj.DateStart = obj.DateStart.getFullYear() + "/" + (obj.DateStart.getMonth() + 1) + "/" + obj.DateStart.getDate();
        }
        if (obj.DateEnd != undefined && obj.DateEnd != '') {
            obj.DateEnd = obj.DateEnd.getFullYear() + "/" + (obj.DateEnd.getMonth() + 1) + "/" + obj.DateEnd.getDate();
        }
        var post = $http({
            method: 'POST',
            url: '/web/dataSet/getByPagePost',
            data: obj,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                $scope.listDataSet = data.data;
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

        $http.get("/web/dataSet/" + $scope.dataSetId, {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.dataSet = angular.copy(data.data.data);
                $scope.dataSet.Contents = $sce.trustAsHtml($scope.dataSet.Contents);
                //Đánh lại stt
                var k = 1;
                for (var i = 0; i < $scope.dataSet.listFiles.length; i++) {
                    if ($scope.dataSet.listFiles[i].Status != 99) {
                        $scope.dataSet.listFiles[i].STT = k;
                        k++;
                    }
                }
                $scope.listSelectAR = [];
                $scope.listSelectRA = [];
                $scope.loadApplicationRangeTree();
                $scope.loadResearchAreaTree();
                $scope.loadDataLicecses();
            }
        });
    };

    $scope.loadApplicationRange = function () {
        var query = "TypeCategoryId=15";
        $http.get("/web/category/GetByPage?page=1&query=" + query + "&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listApplicationRange = data.data.data;
            }
        });
    };

    $scope.loadApplicationRangeTree = function () {
        var query = "arr=15&langId=1";
        $http.get("/web/category/GetByTree?" + query, {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listApplicationRange = data.data.data;
                if ($scope.dataSetId != undefined) {
                    for (var i = 0; i < $scope.listApplicationRange.length; i++) {
                        for (var j = 0; j < $scope.dataSet.applicationRange.length; j++) {
                            if ($scope.listApplicationRange[i].CategoryId == $scope.dataSet.applicationRange[j].CategoryId) {
                                $scope.listApplicationRange[i].Check = true;
                                $scope.listSelectAR.push($scope.listApplicationRange[i]);
                                break;
                            }
                        }
                    }
                }
            }
        });
    };

    $scope.loadResearchArea = function () {
        var query = "TypeCategoryId=14";
        $http.get("/web/category/GetByPage?page=1&query=" + query + "&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listResearchArea = data.data.data;
            }
        });
    };

    $scope.loadResearchAreaTree = function () {
        var query = "arr=14&langId=1";
        $http.get("/web/category/GetByTree?" + query, {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listResearchArea = data.data.data;
                if ($scope.dataSetId != undefined) {
                    for (var i = 0; i < $scope.listResearchArea.length; i++) {
                        for (var j = 0; j < $scope.dataSet.researchArea.length; j++) {
                            if ($scope.listResearchArea[i].CategoryId == $scope.dataSet.researchArea[j].CategoryId) {
                                $scope.listResearchArea[i].Check = true;
                                $scope.listSelectRA.push($scope.listResearchArea[i]);
                                break;
                            }
                        }
                    }
                }
            }
        });
    };

    $scope.loadResearchArea = function () {
        var query = "TypeCategoryId=14";
        $http.get("/web/category/GetByPage?page=1&query=" + query + "&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listResearchArea = data.data.data;
            }
        });
    };

    $scope.loadDataExtentions = function () {
        $http.get("/web/other/listExtentions?page=1&query=1=1&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listDataExtentions = data.data.data;
            }
        });
    };

    $scope.loadDataLicecses = function () {
        $http.get("/web/other/listLicecses?page=1&query=1=1&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listDataLicecses = data.data.data;
                if ($scope.dataSetId != undefined) {
                    for (var i = 0; i < $scope.listDataLicecses.length; i++) {
                        if ($scope.listDataLicecses[i].NewsId == $scope.dataSet.LicenseId) {
                            $scope.dataLicecses = $scope.listDataLicecses[i];
                            break;
                        }
                    }
                }
            }
        });
    };

    $scope.loadDataFolders = function () {
        $http.get("/web/s3File/getAllFolders?page=" + $scope.page_ceph + "&page_size=" + $scope.page_size_ceph
            + "&query=" + $scope.query_ceph +"&order_by=", {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listFolders = data.data.data;
                $scope.item_count_ceph = data.data.metadata;
            }
        });
    };

    $scope.changeValue = function (object, type) {
        switch (type) {
            case 1:
                $scope.q.ApplicationRangeId = object ? object.CategoryId : null;
                break;
            case 2:
                $scope.q.ResearchAreaId = object ? object.CategoryId : null;
                break;
            case 3:
                $scope.confirmData.DataSetStatus = object ? object.Id : null;
                break;
            case 4:
                $scope.q.Status = object ? object.Id : null;
                break;
            case 5:
                $scope.q.Extention = object ? object.Extension : null;
                break;
            case 6:
                $scope.dataSet.LicenseId = object ? object.NewsId : null;
                break;
            default:
                break;
        }
    }

    $scope.clearDataSelected = function (type) {
        switch (type) {
            case 1:
                $scope.q.ApplicationRangeId = undefined;
                break;
            case 2:
                $scope.q.ResearchAreaId = undefined;
                break;
            case 3:
                $scope.confirmData.DataSetStatus = undefined;
                break;
            case 4:
                $scope.q.Status = undefined;
                break;
            case 5:
                $scope.q.Extention = undefined;
                $scope.dataExtentions = undefined;
                console.log("vào đây:" + $scope.dataExtentions);
                break;
            default:
                break;
        }
    }

    $scope.onQueryChange = function () {
        var query = '1=1';
        if ($scope.q.Status != undefined) {
            if ($scope.q.Status != 4)
                query += ' AND Status=' + $scope.q.Status;
            else
                query += ' AND (Status=4 OR Status=5)';
        }
        if ($scope.q.txtSearch != undefined && $scope.q.txtSearch != '') {
            query += ' AND (Title.Contains("' + $scope.q.txtSearch + '") or AuthorName.Contains("' + $scope.q.txtSearch + '"))';
        }
        $scope.query = query;
        $scope.loadData();
    }

    $scope.onQueryChangeCeph = function () {
        //var query = '1=1';
        //if ($scope.q.txtSearch != undefined && $scope.q.txtSearch != '') {
        //    query += ' AND Name.Contains("' + $scope.q.txtSearch + '")';
        //}
        $scope.query_ceph = $scope.q.txtSearch;
        $scope.loadDataFolders();
    }

    var modalApplicationRange;
    $scope.openApplicationRangeModal = function () {
        modalApplicationRange = $uibModal.open({
            templateUrl: '/template/angular/application-range.html',
            windowClass: 'modal-backdrop in modal-dialog-centered',
            backdrop: 'static',
            scope: $scope,
            //size: 'lg'
        });
    };

    $scope.closeApplicationRangeModal = function () {
        modalApplicationRange.dismiss('cancel');
    };

    $scope.CheckApplicationRange = function (CategoryId, curItem) {
        let Check = curItem.Check;
        const listCat = curItem.Genealogy.split('_');

        this.listApplicationRange.forEach(item => {
            if (Check) {
                if (listCat.indexOf(item.CategoryId.toString()) != -1) {
                    item.Check = Check;
                }
            }
            else {
                if (item.Genealogy.indexOf(CategoryId.toString()) != -1) {
                    item.Check = false;
                }
            }
        });
    };

    $scope.selectApplicationRange = function () {
        $scope.listSelectAR = [];
        $scope.listApplicationRange.forEach(item => {
            if (item.Check) {
                $scope.listSelectAR.push(item);
            }
        });
        $scope.closeApplicationRangeModal();
    };

    var modalResearchArea;
    $scope.openResearchAreaModal = function () {
        modalResearchArea = $uibModal.open({
            templateUrl: '/template/angular/research-area.html',
            windowClass: 'modal-backdrop in modal-dialog-centered',
            backdrop: 'static',
            scope: $scope,
            //size: 'lg'
        });
    };

    $scope.closeResearchArea = function () {
        modalResearchArea.dismiss('cancel');
    };

    $scope.CheckResearchArea = function (CategoryId, curItem) {
        let Check = curItem.Check;
        const listCat = curItem.Genealogy.split('_');

        this.listResearchArea.forEach(item => {
            if (Check) {
                if (listCat.indexOf(item.CategoryId.toString()) != -1) {
                    item.Check = Check;
                }
            }
            else {
                if (item.Genealogy.indexOf(CategoryId.toString()) != -1) {
                    item.Check = false;
                }
            }
        });
    };

    $scope.selectResearchArea = function () {
        $scope.listSelectRA = [];
        $scope.listResearchArea.forEach(item => {
            if (item.Check) {
                $scope.listSelectRA.push(item);
            }
        });
        $scope.closeResearchArea();
    };

    var modalConfirmData;
    $scope.openConfirmDataModal = function (type, status) {
        $scope.TypeConfirm = type;
        $scope.disabledStatus = false;
        if (status != 0) {
            $scope.dataSetStatus = {
                "Id": 2,
                "Name": "Không duyệt"
            };
            $scope.confirmData.DataSetStatus = 2;
            $scope.disabledStatus = true;
        }
        if (type == 1 && status == 0)
            $scope.Title = "Phê duyệt dữ liệu nội bộ Tổ chức/phòng ban";
        else if (type == 1 && status == 1)
            $scope.Title = "Hủy phê duyệt dữ liệu nội bộ Tổ chức/phòng ban";
        else if (type == 2 && status == 0)
            $scope.Title = "Phê duyệt dữ liệu công khai Tổ chức/phòng ban";
        else if (type == 2 && status == 2)
            $scope.Title = "Hủy phê duyệt dữ liệu công khai Tổ chức/phòng ban";
        modalConfirmData = $uibModal.open({
            templateUrl: '/template/angular/confirm-data.html',
            windowClass: 'modal-backdrop in modal-dialog-centered',
            backdrop: 'static',
            scope: $scope,
            //size: 'lg'
        });
    };

    $scope.closeConfirmData = function () {
        modalConfirmData.dismiss('cancel');
    };

    $scope.SaveConfirm = function (status) {
        if ($scope.confirmData.DataSetStatus <= 0 || $scope.confirmData.DataSetStatus === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa chọn trạng thái phê duyệt!')
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
        if ($scope.confirmData.DataSetStatus == 2) {
            if ($scope.confirmData.Confirms === '' || $scope.confirmData.Confirms === undefined) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Thông báo')
                        .textContent('Chưa nhập lý do không duyệt!')
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
        }
        $scope.confirmData.DataSetId = $scope.dataSetId;
        $scope.confirmData.Type = $scope.TypeConfirm;
        $scope.confirmData.CreatedId = $scope.customerId;
        $scope.confirmData.UpdatedId = $scope.customerId;

        if ($scope.dataSetId != undefined) {
            $scope.disableBtn.btSubmit = true;
            cfpLoadingBar.start();
            var obj = angular.copy($scope.confirmData);

            var post = $http({
                method: 'POST',
                url: '/web/dataSetApproved',
                data: obj,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });

            post.success(function successCallback(data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                if (data.meta.error_code === 200) {
                    $scope.loadDataById();
                    $scope.closeConfirmData();
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

    $scope.goManagerData = function () {
        $window.location.href = '/quan-ly-du-lieu';
    };

    $scope.SaveAndSend = function (status) {
        //console.log($scope.dataSet.Description);
        //$scope.dataSet.Description = angular.element(document.querySelector('#contents')).val();
        //console.log($scope.dataSet.Description);
        if ($scope.dataSet.Title === '' || $scope.dataSet.Title === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập tiêu đề!')
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
        if ($scope.listSelectAR === undefined || $scope.listSelectAR.length <= 0) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa chọn phạm vi ứng dụng!')
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
        if ($scope.listSelectRA === undefined || $scope.listSelectRA.length <= 0) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa chọn lĩnh vực nghiên cứu!')
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
        if ($scope.dataSet.Contents === '' || $scope.dataSet.Contents === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập mô tả nội dung!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
            });
            return;
        }
        if ($scope.dataSet.AuthorName === '' || $scope.dataSet.AuthorName === undefined) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa nhập tác giả!')
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
        if ($scope.dataSet.LicenseId === undefined || $scope.dataSet.LicenseId <= 0) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Thông báo')
                    .textContent('Chưa chọn bản quyền!')
                    .ok('Đóng')
                    .fullscreen(true)
            ).finally(function () {
                //switch (type) {
                //    case 1:
                //        $scope.focusElement("PasswordDk");
                //        break;
                //    case 2:
                //        $scope.focusElement("PasswordMb");
                //        break;
                //    default:
                //        break;
                //}
            });
            return;
        }

        $scope.disableBtn.btSubmit = true;
        cfpLoadingBar.start();
        var obj = angular.copy($scope.dataSet);
        obj.applicationRange = $scope.listSelectAR;
        obj.researchArea = $scope.listSelectRA;
        obj.Status = status;

        if ($scope.dataSet.DataSetId == undefined) {
            var post = $http({
                method: 'POST',
                url: '/web/dataSet',
                data: obj,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });

            post.success(function successCallback(data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                if (data.meta.error_code === 200) {
                    $scope.goManagerData();
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
                url: '/web/dataSet/' + obj.DataSetId,
                data: obj,
                headers: { 'Authorization': 'bearer ' + $scope.access_token }
            });

            post.success(function successCallback(data, status, headers, config) {
                $scope.disableBtn.btSubmit = false;
                cfpLoadingBar.complete();
                if (data.meta.error_code === 200) {
                    $scope.goManagerData();
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

    $scope.uploadFileDatas = function (e) {
        if (e === undefined) return;
        if (e.files.length <= 0) return;

        var fd = new FormData();
        for (var i = 0; i < e.files.length; i++) {
            fd.append("file", e.files[i]);
        }
        cfpLoadingBar.start();
        var post = $http({
            method: 'POST',
            url: '/web/S3File/uploadFiles',
            data: fd,
            headers: {
                "Content-Type": undefined,
                'Authorization': 'bearer ' + $scope.access_token
            }
        });

        post.success(function successCallback(data, status, headers, config) {
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                for (var i = 0; i < data.data.length; i++) {
                    $scope.dataSet.listFiles.push(data.data[i]);
                }
                //Đánh lại stt
                var k = 1;
                for (var i = 0; i < $scope.dataSet.listFiles.length; i++) {
                    if ($scope.dataSet.listFiles[i].Status != 99) {
                        $scope.dataSet.listFiles[i].STT = k;
                        k++;
                    }
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

    $scope.DeleteFile = function (item) {
        item.Status = 99;
        //Đánh lại stt
        var k = 1;
        for (var i = 0; i < $scope.dataSet.listFiles.length; i++) {
            if ($scope.dataSet.listFiles[i].Status != 99) {
                $scope.dataSet.listFiles[i].STT = k;
                k++;
            }
        }
    };

    $scope.ViewFile = function (id) {
        //$scope.IdFile = id;
        $http.get("/web/S3File/viewFile/" + id, {
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.attactment = data.data.data;
                $scope.dataFile = 'data:image/png;base64,' + $scope.attactment.Note;
                $scope.pdfBase64 = "data:application/pdf;base64," + $scope.attactment.Note;
            }
        });
    };

    $scope.DownloadFileRar = function (id) {
        $http.get("/web/S3File/downloadFiles/" + $scope.dataSetId + "/" + $scope.customerId, {
            headers: { 'Authorization': 'bearer ' + $scope.access_token },
            responseType: 'arraybuffer'
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            header = data.headers();
            var filename = "datasets.zip";//header['x-filename'];
            var contentType = ".rar";//header['content-type'];

            var linkElement = document.createElement('a');
            try {
                var blob = new Blob([data.data], { type: contentType });
                var url = window.URL.createObjectURL(blob);

                linkElement.setAttribute('href', url);
                linkElement.setAttribute("download", filename);

                var clickEvent = new MouseEvent("click", {
                    "view": window,
                    "bubbles": true,
                    "cancelable": false
                });
                linkElement.dispatchEvent(clickEvent);
            } catch (ex) {
                console.log(ex);
            }
        });
    }

    $scope.DownloadFile = function (id) {
        //$scope.IdFile = id;
        $http.get("/web/S3File/downloadOneFile/" + $scope.dataSetId + "/" + $scope.customerId + "/" + id, {
            headers: { 'Authorization': 'bearer ' + $scope.access_token },
            responseType: 'arraybuffer'
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            header = data.headers();
            console.log(headers);
            console.log(data);
            var filename = header['x-filename'];
            var contentType = header['content-type'];

            var linkElement = document.createElement('a');
            try {
                var blob = new Blob([data.data], { type: contentType });
                var url = window.URL.createObjectURL(blob);

                linkElement.setAttribute('href', url);
                linkElement.setAttribute("download", filename);

                var clickEvent = new MouseEvent("click", {
                    "view": window,
                    "bubbles": true,
                    "cancelable": false
                });
                linkElement.dispatchEvent(clickEvent);
            } catch (ex) {
                console.log(ex);
            }
        });
    };

    $scope.deleteData = function (item) {
        var confirm = $mdDialog.confirm()
            .title('Thông báo')
            .textContent('Bạn có chắc chắn muốn xóa bộ dữ liệu này?')
            .ok('Đồng ý')
            .cancel('Hủy');

        $mdDialog.show(confirm).then(function () {
            var remove = $http({
                method: 'DELETE',
                url: '/web/dataSet/' + item.DataSetId,
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

    $scope.expandDetail = function (item) {
        //$scope.isViewCeph = true;
        //$scope.isViewData = false;
        //$scope.dataSet = {};
        for (var i = 0; i < $scope.listFolders.length; i++) {
            if (item.Name != $scope.listFolders[i].Name)
                $scope.listFolders[i].IsView = false;
        }
        item.IsView = !item.IsView;
        //lấy ra các file ko có tên là MetaDataSet.json
        //$scope.checkMeta = false;
        $scope.listFileCephs = [];
        for (var i = 0; i < item.listFiles.length; i++) {
            if (item.listFiles[i].Name != "MetaDataSet.json") {
                $scope.listFileCephs.push(item.listFiles[i]);
            //    $scope.checkMeta = true;
            }
            //else
            //    $scope.itemMeta = item.listFiles[i];
        }
        //$scope.folderFileCeph = item;
        ////
        //if ($scope.checkMeta) {
        //    $scope.SelectFileCeph($scope.itemMeta);
        //}
        //else {

        //}
    };

    $scope.selectData = function (item) {
        $scope.isViewCeph = true;
        $scope.isViewData = false;
        $scope.dataSet = {};
        for (var i = 0; i < $scope.listFolders.length; i++) {
            if (item.Name != $scope.listFolders[i].Name)
                $scope.listFolders[i].IsView = false;
        }
        item.IsView = !item.IsView;
        //lấy ra các file ko có tên là MetaDataSet.json
        $scope.checkMeta = false;
        $scope.listFileCephs = [];
        for (var i = 0; i < item.listFiles.length; i++) {
            if (item.listFiles[i].Name != "MetaDataSet.json") {
                $scope.listFileCephs.push(item.listFiles[i]);
                $scope.checkMeta = true;
            }
            else
                $scope.itemMeta = item.listFiles[i];
        }
        $scope.folderFileCeph = item;
        //
        if ($scope.checkMeta) {
            $scope.SelectFileCeph($scope.itemMeta);
        }
        else {

        }
    };

    $scope.SelectFileCeph = function (item) {
        $scope.listSelectAR = [];
        $scope.listSelectRA = [];
        $scope.dataSet = {};
        var post = $http({
            method: 'POST',
            url: '/web/S3File/selectFileCeph',
            data: item,
            headers: { 'Authorization': 'bearer ' + $scope.access_token }
        });

        post.success(function successCallback(data, status, headers, config) {
            $scope.disableBtn.btSubmit = false;
            cfpLoadingBar.complete();
            if (data.meta.error_code === 200) {
                //
                $scope.isViewCeph = false;
                $scope.isViewData = true;
                $scope.dataJson = data.data;
                
                $scope.dataSet.Title = $scope.dataJson.tieudebodulieu;
                $scope.dataSet.Contents = $scope.dataJson.motanoidungbodulieu;
                $scope.dataSet.AuthorName = $scope.dataJson.tentacgia;
                $scope.dataSet.AuthorEmail = $scope.dataJson.diachiemail;
                $scope.dataSet.AuthorPhone = $scope.dataJson.sdttacgia;
                $scope.dataSet.Version = $scope.dataJson.phienbantailieu;
                $scope.dataSet.Note = $scope.dataJson.nguon;
                $scope.dataSet.folderFileCeph = $scope.folderFileCeph;
                //
                $scope.dataSet.listFiles = $scope.listFileCephs;
                //
                if ($scope.dataJson.phamviungdung != undefined) {
                    for (var i = 0; i < $scope.listApplicationRange.length; i++) {
                        for (var j = 0; j < $scope.dataJson.phamviungdung.length; j++) {
                            if ($scope.listApplicationRange[i].Name.trim() == $scope.dataJson.phamviungdung[j].trim()) {
                                $scope.listApplicationRange[i].Check = true;
                                $scope.listSelectAR.push($scope.listApplicationRange[i]);
                                break;
                            }
                        }
                    }
                }
                //
                if ($scope.dataJson.linhvucnghiencuu != undefined) {
                    for (var i = 0; i < $scope.listResearchArea.length; i++) {
                        for (var j = 0; j < $scope.dataJson.linhvucnghiencuu.length; j++) {
                            if ($scope.listResearchArea[i].Name.trim() == $scope.dataJson.linhvucnghiencuu[j].trim()) {
                                $scope.listResearchArea[i].Check = true;
                                $scope.listSelectRA.push($scope.listResearchArea[i]);
                                break;
                            }
                        }
                    }
                }
                
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
        //$http.get("/web/S3File/selectFileCeph/" + item.Link, {
        //    headers: { 'Authorization': 'bearer ' + $scope.access_token }
        //}).then(function (data, status, headers) {
        //    cfpLoadingBar.complete();
        //    if (data.data.meta.error_code === 200) {
        //        $scope.attactment = data.data.data;
        //        //$scope.dataFile = 'data:image/png;base64,' + $scope.attactment.Note;
        //        //$scope.pdfBase64 = "data:application/pdf;base64," + $scope.attactment.Note;
        //    }
        //});
    };

    $scope.goDataCeph = function () {
        $scope.isViewCeph = true;
        $scope.isViewData = false;
        $scope.dataSet = {};
    };

    $scope.ViewManageLink = function (type) {
        if (type == 1) {
            $scope.isViewKuberflow = true;
            $scope.isViewSparkWorker = false;
            $scope.isViewSparkMaster = false;
            $scope.isViewNifi = false;
            $scope.isViewAirByte = false;
        }
        else if (type == 2) {
            $scope.isViewKuberflow = false;
            $scope.isViewSparkWorker = true;
            $scope.isViewSparkMaster = false;
            $scope.isViewNifi = false;
            $scope.isViewAirByte = false;
        }
        else if (type == 3) {
            $scope.isViewKuberflow = false;
            $scope.isViewSparkWorker = false;
            $scope.isViewSparkMaster = true;
            $scope.isViewNifi = false;
            $scope.isViewAirByte = false;
        }
        else if (type == 4) {
            $scope.isViewKuberflow = false;
            $scope.isViewSparkWorker = false;
            $scope.isViewSparkMaster = false;
            $scope.isViewNifi = true;
            $scope.isViewAirByte = false;
        }
        else if (type == 5) {
            $scope.isViewKuberflow = false;
            $scope.isViewSparkWorker = false;
            $scope.isViewSparkMaster = false;
            $scope.isViewNifi = false;
            $scope.isViewAirByte = true;
        }
    };


}]);