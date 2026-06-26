import { Component, forwardRef, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { SettingScopes, NameValueDto } from '@shared/service-proxies/service-proxies';

export enum AbpLoginResultType {
    Success = 1,
    InvalidUserNameOrEmailAddress = 2,
    InvalidPassword = 3,
    UserIsNotActive = 4,
    InvalidTenancyName = 5,
    TenantIsNotActive = 6,
    UserEmailIsNotConfirmed = 7,
    UnknownExternalLogin = 8,
    LockedOut = 9,
    UserPhoneNumberIsNotConfirmed = 10
}
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
    selector: 'login-result-type-combo',
    template: `
    <select class="form-control" [formControl]="selectedLoginResultType">
        <option *ngFor="let loginResultType of loginResultTypes" [value]="loginResultType.value">{{loginResultType.name}}</option>
    </select>`,
    providers: [{
        provide: NG_VALUE_ACCESSOR,
        useExisting: forwardRef(() => LoginResultTypeComboComponent),
        multi: true,
    }]
})
export class LoginResultTypeComboComponent extends AppComponentBase implements OnInit, ControlValueAccessor {

    @Input() defaultTimezoneScope: SettingScopes;

    loginResultTypes: NameValueDto[] = [];
    selectedLoginResultType = new FormControl('');

    loginResultType: AbpLoginResultType;

    onTouched: any = () => { };

    constructor(
        injector: Injector) {
        super(injector);
    }

    ngOnInit(): void {
        this.loginResultTypes.push(new NameValueDto({ name: this.l('All'), value: '' }));
        for (const value in AbpLoginResultType) {
            if (typeof AbpLoginResultType[value] === 'string') {
                continue;
            }

            this.loginResultTypes.push(new NameValueDto({ name: this.l('AbpLoginResultType_' + value), value: value }));
        }
    }

    writeValue(obj: any): void {
        if (this.selectedLoginResultType) {
            this.selectedLoginResultType.setValue(obj);
        }
    }

    registerOnChange(fn: any): void {
        this.selectedLoginResultType.valueChanges.subscribe(fn);
    }

    registerOnTouched(fn: any): void {
        this.onTouched = fn;
    }

    setDisabledState?(isDisabled: boolean): void {
        if (isDisabled) {
            this.selectedLoginResultType.disable();
        } else {
            this.selectedLoginResultType.enable();
        }
    }
}
