import { AbpSessionService } from 'abp-ng2-module';
import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { SessionServiceProxy, UpdateUserSignInTokenOutput } from '@shared/service-proxies/service-proxies';
import { UrlHelper } from 'shared/helpers/UrlHelper';
import { ExternalLoginProvider, LoginService } from './login.service';
import { ReCaptchaV3Service } from 'ngx-captcha';
import { AppConsts } from '@shared/AppConsts';
import { CookieConsentService } from '@shared/common/session/cookie-consent.service';

import { AppAuthService } from '@app/shared/common/auth/app-auth.service';
import { LocalStorageService } from '@shared/utils/local-storage.service';
import { LoginRoutingModule } from './login-routing.module';

@Component({
    templateUrl: './login.component.html',
    animations: [accountModuleAnimation()],
    styleUrls: ['./login.component.less']
})
export class LoginComponent extends AppComponentBase implements OnInit {
    submitting = false;
    isMultiTenancyEnabled: boolean = this.multiTenancy.isEnabled;
    recaptchaSiteKey: string = AppConsts.recaptchaSiteKey;
    username: any;
    password: any;
    demo: any;
    escrow:any;
    orderby: string;
    fieldTextType: boolean;

    constructor(
        injector: Injector,
        public loginService: LoginService,
        private _router: Router,
        private _sessionService: AbpSessionService,
        private _sessionAppService: SessionServiceProxy,
        private _reCaptchaV3Service: ReCaptchaV3Service,
        private _activateroute: ActivatedRoute,
        private route: ActivatedRoute,
         private _authService: AppAuthService,
        private _localStorageService: LocalStorageService
            ) {
        super(injector);
    }

    get multiTenancySideIsTeanant(): boolean {
        return this._sessionService.tenantId > 0;
    }

    get isTenantSelfRegistrationAllowed(): boolean {
        return this.setting.getBoolean('App.TenantManagement.AllowSelfRegistration');
    }

    get isSelfRegistrationAllowed(): boolean {
        if (!this._sessionService.tenantId) {
            return false;
        }

        return this.setting.getBoolean('App.UserManagement.AllowSelfRegistration');
    }
   
    ngOnInit(): void {
       
        this.escrow = this.route.snapshot.queryParams['Escrow'];
         //this.escrow="0007"
        if(this.escrow != null || this.escrow != undefined || this.escrow!=""){
           // localStorage.clear();
         this._localStorageService.setItem("Escrow", this.escrow);

        

         // JavaScript
        //  var a = document.getElementById('opener'), w;        
        //  a.onclick = function() {
        //    if (!w || w.closed) {
        //    //  w = window.open("https://www.google.com","_blank","menubar = 0, scrollbars = 0");
        //    } else {
        //      console.log('window is already opened');
        //    }
        //    w.focus();
        //  };





        }
debugger;
        if (this._sessionService.userId > 0 && UrlHelper.getReturnUrl() && UrlHelper.getSingleSignIn()) {
            this._sessionAppService.updateUserSignInToken()
                .subscribe((result: UpdateUserSignInTokenOutput) => {
                    debugger;
                    const initialReturnUrl = UrlHelper.getReturnUrl();
                    const returnUrl = initialReturnUrl + (initialReturnUrl.indexOf('?') >= 0 ? '&' : '?') +
                        'accessToken=' + result.signInToken +
                        '&userId=' + result.encodedUserId +
                        '&tenantId=' + result.encodedTenantId;
                      
                    location.href = returnUrl;
                   
                });
        }
        this.demo = this.route.snapshot.queryParams['username'];


        
        
        if(this.demo!=null)
        {
        this.loginService.authenticateModel.userNameOrEmailAddress = atob(this.demo);
        }
        let state = UrlHelper.getQueryParametersUsingHash().state;
        if (state && state.indexOf('openIdConnect') >= 0) {
            this.loginService.openIdConnectLoginCallback({});
        }
        this.route.queryParams
        .subscribe(params => {
            debugger;
            
           this.username = params.UsernameOrEmailAddress;
           this.password = params.password;
           if(this.username!=undefined && this.password!=undefined )
           {
               
           this.loginService.authenticateModel.userNameOrEmailAddress = atob(this.username);
           this.loginService.authenticateModel.password = atob(this.password);
           this.login();
        }else if(params.logout){
            this._authService.logout();
            
               
        }
        }
        
      );
    }

    login(): void {
        debugger;
       // window.close();
        try{
        let recaptchaCallback = (token: string) => {
            this.showMainSpinner();

            this.submitting = true;
            this.loginService.authenticate(
                () => {
                    this.submitting = false;

                    this.hideMainSpinner();
                },
                null,
                token
            );
        };

        if (this.useCaptcha) {
            this._reCaptchaV3Service.execute(this.recaptchaSiteKey, 'login', (token) => {
                recaptchaCallback(token);
            });
        } else {
            recaptchaCallback(null);
        }
    }catch(error){

    }
    }


    externalLogin(provider: ExternalLoginProvider) {
        this.loginService.externalAuthenticate(provider);
    }

    get useCaptcha(): boolean {
        return this.setting.getBoolean('App.UserManagement.UseCaptchaOnLogin');
    }



toggleFieldTextType() {
  this.fieldTextType = !this.fieldTextType;
}
}
