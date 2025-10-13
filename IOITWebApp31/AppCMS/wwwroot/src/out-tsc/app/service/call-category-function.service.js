"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.CallCategoryFunctionService = void 0;
var core_1 = require("@angular/core");
var Subject_1 = require("rxjs/Subject");
var CallCategoryFunctionService = /** @class */ (function () {
    function CallCategoryFunctionService() {
        this.subject = new Subject_1.Subject();
    }
    //Gửi thông tin gọi đến hàm với TypeAction 1 = thêm mới, 2 = sửa , 3 = xóa, 4 = lưu thông tin sắp xếp,
    //5 thêm danh mục với ngôn ngữ mới, 6 sửa danh mục với ngôn ngữ mới, 7 thay đổi trạng thái danh mục, 8 là xem danh mục
    CallCategoryFunctionService.prototype.sendAction = function (CategoryId, TypeAction, IsShow) {
        this.subject.next({ CategoryId: CategoryId, TypeAction: TypeAction, IsShow: IsShow });
    };
    //Nhận thông tin là hàm đã đc gọi
    CallCategoryFunctionService.prototype.getAction = function () {
        return this.subject.asObservable();
    };
    CallCategoryFunctionService = __decorate([
        core_1.Injectable({
            providedIn: 'root'
        })
    ], CallCategoryFunctionService);
    return CallCategoryFunctionService;
}());
exports.CallCategoryFunctionService = CallCategoryFunctionService;
//# sourceMappingURL=call-category-function.service.js.map