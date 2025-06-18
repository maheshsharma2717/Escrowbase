import { Component, Injector, OnInit, Input } from '@angular/core';
import { ThemesLayoutBaseComponent } from '../themes/themes-layout-base.component';
import { LinkedUserDto, ProfileServiceProxy, UserLinkServiceProxy,SrFileMappingsServiceProxy } from '@shared/service-proxies/service-proxies';
import { LinkedAccountService } from '@app/shared/layout/linked-account.service';
import { AbpMultiTenancyService, AbpSessionService } from 'abp-ng2-module';
import { AppAuthService } from '@app/shared/common/auth/app-auth.service';
import { ImpersonationService } from '@app/admin/users/impersonation.service';
import { OffcanvasOptions } from '@metronic/app/core/_base/layout/directives/offcanvas.directive';
import { ActivatedRoute , Router} from '@angular/router';
import { AppConsts } from '@shared/AppConsts';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'user-menu',
    templateUrl: './user-menu.component.html'
})
export class UserMenuComponent extends ThemesLayoutBaseComponent implements OnInit {

    @Input() iconOnly = false;

    @Input() togglerCssClass = 'btn btn-icon w-auto btn-clean d-flex align-items-center btn-lg px-2';
    @Input() textCssClass = 'text-dark-50 font-weight-bolder font-size-base d-none d-md-inline mr-3';
    @Input() symbolCssClass = 'symbol symbol-35 symbol-light-success';
    @Input() symbolTextCssClass = 'symbol-label font-size-h5 font-weight-bold';

    usernameFirstLetter = '';

    profilePicture = AppConsts.appBaseUrl + '/assets/common/images/default-profile-picture.png';
    shownLoginName = '';
    tenancyName = '';
    userName = '';
    userEmail = '';
    userType: string;
    output: any;
    collect: any;
    showLogout = false;
    recentlyLinkedUsers: LinkedUserDto[];
    isImpersonatedLogin = false;
    isMultiTenancyEnabled = false;

    offcanvasOptions: OffcanvasOptions = {
        overlay: true,
        baseClass: 'offcanvas',
        placement: 'right',
        closeBy: 'kt_demo_panel_close',
        toggleBy: 'kt_quick_user_toggle'
    };

    constructor(
        injector: Injector,
        private http: HttpClient,
        private _linkedAccountService: LinkedAccountService,
        private _abpMultiTenancyService: AbpMultiTenancyService,
        private _profileServiceProxy: ProfileServiceProxy,
        private _userLinkServiceProxy: UserLinkServiceProxy,
        private _authService: AppAuthService,
        private _impersonationService: ImpersonationService,
        private _abpSessionService: AbpSessionService,
        private _SrFileMappingsServiceProxy: SrFileMappingsServiceProxy,
        private _router: Router,
        //private _UserTypeDto: UserTypeDto
    ) {
        super(injector);
      if(abp.session.userId == 1)
      {
      }  
      else
      {
   this._SrFileMappingsServiceProxy.getAll(abp.session.userId.toString(),undefined,undefined,undefined,undefined,
    undefined,undefined,undefined,undefined,undefined,undefined).subscribe(result =>{
    //    this.output = [];
    //    //this.output = result['result'];
    //    this.output = result['0'];
    //    this.output = this.output['srFileMapping'];
    //    this.collect = this.output = this.output['action'];
    //    this.collect = this.collect.substring(this.collect.indexOf('{')+1);
    //    this.userType = this.collect.substring(0,this.collect.indexOf('-'));
    });
}
    }

    ngOnInit(): void {
        
        this.isImpersonatedLogin = this._abpSessionService.impersonatorUserId > 0;
        this.isMultiTenancyEnabled = this._abpMultiTenancyService.isEnabled;
        this.setCurrentLoginInformations();
        this.getProfilePicture();
        this.getRecentlyLinkedUsers();
        this.registerToEvents();
        this.usernameFirstLetter = this.appSession.user.userName.substring(0, 1).toUpperCase();

        console.log(this._router.url);
        //if(this._router.url == "/app/main/Userdashboard"){
            this.showLogout = true;
      //  }
    }

    setCurrentLoginInformations(): void {                  
        //let userType = atob(person.u);
        this.shownLoginName = this.appSession.getShownLoginName();
        this.tenancyName = this.appSession.tenancyName;
        this.userName = this.appSession.user.name+" "+this.appSession.user.surname;
        //this.userType.userType.type;
        this.userEmail = this.appSession.user.userName;
        this.getUserType();
    }

    getUserType(){
        let url = AppConsts.remoteServiceBaseUrl;
        let folderPath = url + '/Home/';
        this.primengTableHelper.showLoadingIndicator();
        this.http.get(folderPath + "GetUserCompanyDetails?username=" + this.appSession.user.emailAddress)
            .subscribe((res :any)=> {
                  if(res.result.length >0){
                    this.userType = res.result[0].type
                }
              
              
            })
    }

    getShownUserName(linkedUser: LinkedUserDto): string {
        if (!this._abpMultiTenancyService.isEnabled) {
            return linkedUser.username;
        }
       

        return (linkedUser.tenantId ? linkedUser.tenancyName : '.') + '\\' + linkedUser.username;
    }

    getProfilePicture(): void {
        this._profileServiceProxy.getProfilePicture().subscribe(result => {
            if (result && result.profilePicture) {
                this.profilePicture = 'data:image/jpeg;base64,' + result.profilePicture;
            }
        });
    }

    getRecentlyLinkedUsers(): void {
        this._userLinkServiceProxy.getRecentlyUsedLinkedUsers().subscribe(result => {
            this.recentlyLinkedUsers = result.items;
        });
    }


    showLoginAttempts(): void {
        abp.event.trigger('app.show.loginAttemptsModal');
    }

    showLinkedAccounts(): void {
        abp.event.trigger('app.show.linkedAccountsModal');
    }

    showUserDelegations(): void {
        abp.event.trigger('app.show.userDelegationsModal');
    }

    changePassword(): void {
        abp.event.trigger('app.show.changePasswordModal');
       
    }
    sms(): void {
        abp.event.trigger('app.show.smsModal');
    }

    changeProfilePicture(): void {
        abp.event.trigger('app.show.changeProfilePictureModal');
    }

    changeMySettings(): void {
        abp.event.trigger('app.show.mySettingsModal');
    }

    registerToEvents() {
        abp.event.on('profilePictureChanged', () => {
            this.getProfilePicture();
        });

        abp.event.on('app.getRecentlyLinkedUsers', () => {
            this.getRecentlyLinkedUsers();
        });

        abp.event.on('app.onMySettingsModalSaved', () => {
            this.onMySettingsModalSaved();
        });
    }

    logout(): void {
        this._authService.logout();
        
    }

    onMySettingsModalSaved(): void {
        this.shownLoginName = this.appSession.getShownLoginName();
    }

    backToMyAccount(): void {
        this._impersonationService.backToImpersonator();
    }

    switchToLinkedUser(linkedUser: LinkedUserDto): void {
        this._linkedAccountService.switchToAccount(linkedUser.id, linkedUser.tenantId);
    }

    downloadCollectedData(): void {
        this._profileServiceProxy.prepareCollectedData().subscribe(() => {
            this.message.success(this.l('GdprDataPrepareStartedNotification'));
        });
    }
    
}
