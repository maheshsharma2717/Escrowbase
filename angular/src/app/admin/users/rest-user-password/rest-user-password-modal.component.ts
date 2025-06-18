import { AfterViewChecked, Component, ElementRef, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrUpdateUserInput, OrganizationUnitDto, PasswordComplexitySetting, ProfileServiceProxy, UserEditDto, UserRoleDto, UserServiceProxy, GetUserForEditOutput,AccountServiceProxy } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';


import * as _ from 'lodash';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'resetUserPasswordModal',
    templateUrl: './rest-user-password-modal.component.html',
    styles: []
})
export class ResetUserPasswordModalComponent extends AppComponentBase {

    @ViewChild('resetUserPasswordModal', { static: true }) modal: ModalDirective;
 
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

     
    active;
    saving;
    password;
    confirmPassword;
    userId
    submited :boolean = false;
    constructor(
        injector: Injector,
        private _accountServiceProxy: AccountServiceProxy,
      
    ) {
        super(injector);
    }

    show(userId?: number): void {
        this.password = "";
        this.confirmPassword = "";
        this.submited = false
       this.userId =userId;
            this.active = true;
            this.modal.show();
    } 
   
    save(){
    
        if(!this.password || !this.confirmPassword){
            this.submited = true;
            return;
        }
        if( this.password === this.confirmPassword){
        let data :any = {
userId :  this.userId ,
password:this.password
        }
        this._accountServiceProxy.resetPasswordByAdmin(data).subscribe(result =>{
            this.modal.hide();
        })
    }
    }
    onShown(){

    }
    get passwordsMatch(): boolean {
        return this.password === this.confirmPassword;
      }
      close(){
        this.modal.hide();
      }
     
}
