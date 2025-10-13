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
exports.OlUnitComponent = void 0;
var core_1 = require("@angular/core");
var call_category_function_service_1 = require("../../service/call-category-function.service");
var const_1 = require("../../data/const");
var dt_1 = require("../../data/dt");
var common_service_1 = require("../../service/common.service");
var OlUnitComponent = /** @class */ (function () {
    function OlUnitComponent(callCategoryFunctionService, common) {
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
    OlUnitComponent.prototype.ngOnInit = function () {
        //console.log(this.listLanguage);
        this.CheckRole = new dt_1.CheckRole();
        console.log(this.functionCode);
        this.CheckRole.View = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 0);
        this.CheckRole.Create = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 1);
        this.CheckRole.Update = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 2);
        this.CheckRole.Delete = this.common.CheckAccessKeyRole(localStorage.getItem("access_key"), this.functionCode, 3);
    };
    OlUnitComponent.prototype.ngOnDestroy = function () {
        // this.subscription.unsubscribe();
    };
    OlUnitComponent.prototype.AddCate = function (CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 1, false);
    };
    OlUnitComponent.prototype.UpdateCate = function (CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 2, false);
    };
    OlUnitComponent.prototype.ViewCate = function (CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 8, false);
    };
    OlUnitComponent.prototype.DeleteCate = function (CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 3, false);
    };
    OlUnitComponent.prototype.AddCateLang = function (CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 5, false);
    };
    OlUnitComponent.prototype.UpdateCateLang = function (CategoryId) {
        this.callCategoryFunctionService.sendAction(CategoryId, 6, false);
    };
    OlUnitComponent.prototype.SaveCategorySort = function () {
        console.log(this.items);
    };
    OlUnitComponent.prototype.ShowHide = function (CategoryId, IsShow) {
        this.callCategoryFunctionService.sendAction(CategoryId, 7, IsShow);
    };
    __decorate([
        core_1.Input('data'),
        __metadata("design:type", Array)
    ], OlUnitComponent.prototype, "items", void 0);
    __decorate([
        core_1.Input('key'),
        __metadata("design:type", String)
    ], OlUnitComponent.prototype, "key", void 0);
    __decorate([
        core_1.Input('hasAction'),
        __metadata("design:type", Boolean)
    ], OlUnitComponent.prototype, "hasAction", void 0);
    __decorate([
        core_1.Input('listLanguage'),
        __metadata("design:type", Array)
    ], OlUnitComponent.prototype, "listLanguage", void 0);
    __decorate([
        core_1.Input('languageId'),
        __metadata("design:type", Number)
    ], OlUnitComponent.prototype, "languageId", void 0);
    __decorate([
        core_1.Input('functionCode'),
        __metadata("design:type", String)
    ], OlUnitComponent.prototype, "functionCode", void 0);
    OlUnitComponent = __decorate([
        core_1.Component({
            selector: 'ol-unit',
            templateUrl: './ol-unit.component.html',
            styleUrls: ['./ol-unit.component.css'],
            changeDetection: core_1.ChangeDetectionStrategy.OnPush
        }),
        __metadata("design:paramtypes", [call_category_function_service_1.CallCategoryFunctionService,
            common_service_1.CommonService])
    ], OlUnitComponent);
    return OlUnitComponent;
}());
exports.OlUnitComponent = OlUnitComponent;
//# sourceMappingURL=ol-unit.component.js.map