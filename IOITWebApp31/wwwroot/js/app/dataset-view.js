
myApp.controller('DataSetViewController', ['$scope', '$http', '$mdDialog', 'config', 'cfpLoadingBar', 'app', '$cookies', '$rootScope', '$window', 'ngDialog', '$uibModal', function DataSetViewController($scope, $http, $mdDialog, config, cfpLoadingBar, app, $cookies, $rootScope, $window, ngDialog, $uibModal) {

    $scope.init = function (data, id) {
        if (data != undefined && id!=undefined) {
            $scope.customerId = data.CustomerId;
            $scope.access_token = data.access_token;
            $scope.dataSetId = id;
        }
    };

    $scope.ViewFile = function (item) {
        console.log(item);
        $http.get("/web/S3File/viewFile/" + item, {
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
            var filename = "datasets.rar";//header['x-filename'];
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
        $http.get("/web/S3File/downloadOneFile/" + $scope.dataSetId + "/" + $scope.customerId + "/" + id, {
            headers: { 'Authorization': 'bearer ' + $scope.access_token },
            responseType: 'arraybuffer'
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            header = data.headers();
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

}]);