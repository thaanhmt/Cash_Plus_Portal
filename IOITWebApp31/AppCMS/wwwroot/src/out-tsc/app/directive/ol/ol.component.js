"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.OlComponent = void 0;
var core_1 = require("@angular/core");
var call_category_function_service_1 = require("../../service/call-category-function.service");
var const_1 = require("../../data/const");
var dt_1 = require("../../data/dt");
var common_service_1 = require("../../service/common.service");
var OlComponent = /** @class */ (function () {
    function OlComponent(callCategoryFunctionService, common) {
        // this.subscription = this.callCategoryFunctionService.getAction().subscribe(action => {
        // 	if (action.TypeAction == 4) {
        // 		this.SaveCategorySort();
        // 	}
        // });
        this.callCategoryFunctionService = callCategoryFunctionService;
        this.common = common;
        this.domainImage = const_1.domainImage;
        this.domainMedia = const_1.domainMedia;
    }
    OlComponent.prototype.ngOnInit = function () {
        //console.log(this.listLanguage);
        this.CheckRole = new dt_1.CheckRole();
        //console.log(this.items);
        this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 0);
        this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 1);
        this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 2);
        this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 3);
    };
    OlComponent.prototype.ngOnDestroy = function () {
        // this.subscription.unsubscribe();
    };
    OlComponent.prototype.AddCate = function (CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 1, false);
    };
    OlComponent.prototype.UpdateCate = function (CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 2, false);
    };
    OlComponent.prototype.ViewCate = function (CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 8, false);
    };
    OlComponent.prototype.DeleteCate = function (CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 3, false);
    };
    OlComponent.prototype.AddCateLang = function (CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 5, false);
    };
    OlComponent.prototype.UpdateCateLang = function (CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 6, false);
    };
    OlComponent.prototype.SaveCategorySort = function () {
        //console.log(this.items);
    };
    OlComponent.prototype.ShowHide = function (CategoryId, IsShow) {
        this.callCategoryFunctionService.sendAction(CategoryId, 7, IsShow);
    };
    __decorate([
        core_1.Input('data'),
        __metadata("design:type", Array)
    ], OlComponent.prototype, "items", void 0);
    __decorate([
        core_1.Input('key'),
        __metadata("design:type", String)
    ], OlComponent.prototype, "key", void 0);
    __decorate([
        core_1.Input('hasAction'),
        __metadata("design:type", Boolean)
    ], OlComponent.prototype, "hasAction", void 0);
    __decorate([
        core_1.Input('listLanguage'),
        __metadata("design:type", Array)
    ], OlComponent.prototype, "listLanguage", void 0);
    __decorate([
        core_1.Input('languageId'),
        __metadata("design:type", Number)
    ], OlComponent.prototype, "languageId", void 0);
    __decorate([
        core_1.Input('functionCode'),
        __metadata("design:type", String)
    ], OlComponent.prototype, "functionCode", void 0);
    __decorate([
        core_1.Input('hasAdd'),
        __metadata("design:type", Boolean)
    ], OlComponent.prototype, "hasAdd", void 0);
    OlComponent = __decorate([
        core_1.Component({
            selector: 'ol',
            templateUrl: './ol.component.html',
            styleUrls: ['./ol.component.css'],
            changeDetection: core_1.ChangeDetectionStrategy.OnPush
        }),
        __metadata("design:paramtypes", [call_category_function_service_1.CallCategoryFunctionService,
            common_service_1.CommonService])
    ], OlComponent);
    return OlComponent;
}());
exports.OlComponent = OlComponent;
//# sourceMappingURL=ol.component.js.map