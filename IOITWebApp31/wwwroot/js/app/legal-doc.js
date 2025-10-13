myApp.controller('LegalDocController', ['$scope', '$http', '$mdDialog', '$mdToast', 'config', 'cfpLoadingBar', 'md5', '$window', 'app', function LegalDocController($scope, $http, $mdDialog, $mdToast, config, cfpLoadingBar, md5, $window, app) {
    $scope.regexEmail = config.regexEmail;
    $scope.regexPhone = config.regexPhone;
    $scope.contact = {};
    $scope.disableBtn = {};
    $scope.query = "1=1";
    $scope.textSreach = "";
    $scope.Year = null;
    $scope.agencyIssueC = null;

    $scope.init = function () {
        $scope.loadListLegalDoc();
        $scope.GetListTypeAttributeItem();

    };


    //get list LegalDoc
    $scope.loadListLegalDoc = function () {
        //var query = "TypeOriginId=3";
        $http.get("/web/LegalDoc/GetByPage?page=1&query=" + $scope.query+"&order_by=", {
            headers: {}
        }).then(function (data, status, headers) {
            cfpLoadingBar.complete();
            if (data.data.meta.error_code === 200) {
                $scope.listLegalDoc = data.data.data;

                
            }
        });
    };
    $scope.SrearchLegalDoc = function () {
        $scope.Year = $scope.Year + '';
        $scope.agencyIssueC = $scope.agencyIssueC + '';
        let query = '1=1';
        if ($scope.textSreach != '' && $scope.textSreach != undefined) {
            query += ' and (Name.Contains("' + $scope.textSreach + '") OR Code.Contains("' + $scope.textSreach + '") )'
        }
        if ($scope.agencyIssueC !== '999' && $scope.agencyIssueC != undefined) {
            query += ' and AgencyIssued.Contains("' + $scope.agencyIssueC + '")'
        }
        if ($scope.Year !== '999' && $scope.Year != undefined) {
            query += ' and YearIssue =' + $scope.Year;
        }
        $scope.query = query;
        $scope.loadListLegalDoc();
    }

}]);