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
exports.ConfigMailComponent = void 0;
var core_1 = require("@angular/core");
var ConfigMailComponent = /** @class */ (function () {
    function ConfigMailComponent() {
    }
    ConfigMailComponent.prototype.ngOnInit = function () {
    };
    ConfigMailComponent = __decorate([
        core_1.Component({
            selector: 'app-config-mail',
            templateUrl: './config-mail.component.html',
            styleUrls: ['./config-mail.component.scss']
        }),
        __metadata("design:paramtypes", [])
    ], ConfigMailComponent);
    return ConfigMailComponent;
}());
exports.ConfigMailComponent = ConfigMailComponent;
//# sourceMappingURL=config-mail.component.js.map