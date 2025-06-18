import { PermissionCheckerService } from 'abp-ng2-module';
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { AppAuthService } from '@app/shared/common/auth/app-auth.service';
import { UrlHelper } from '@shared/helpers/UrlHelper';

import { LocalStorageService } from '@shared/utils/local-storage.service';
@Injectable()
export class AccountRouteGuard implements CanActivate {
    escrow: any;


    constructor(
        private _permissionChecker: PermissionCheckerService,
        private _router: Router,
        private _sessionService: AppSessionService,
        private _authService: AppAuthService,
        private _localStorageService: LocalStorageService
        ) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        
        if (route.queryParams['ss'] && route.queryParams['ss'] === 'true') {
            return true;
        }

        //EscrowBase Window app Logout & reduct
          
        if(route.queryParams['UsernameOrEmailAddress'] && route.queryParams['password'] ){
            
            var usernames = atob(route.queryParams['UsernameOrEmailAddress']);
            if (this._sessionService.user) {
                if (usernames != this._sessionService.user.userName) {
            var text ='/account/login?' + 'UsernameOrEmailAddress=' + route.queryParams['UsernameOrEmailAddress'] +'&password=' + route.queryParams['password'] ;
            this._authService.logout(true, text);
                }}
        }


        //EscrowBase Window app Logout & reduct from exe
          
        if(route.queryParams['UsernameOrEmailAddress'] && route.queryParams['password'] ){
            
            this.escrow = route.queryParams['Escrow'] ;
            if(this.escrow != null || this.escrow != undefined || this.escrow!=""){
                // localStorage.clear();
              this._localStorageService.setItem("Escrow", this.escrow);
            }

            var usernames = atob(route.queryParams['UsernameOrEmailAddress']);
            if (this._sessionService.user) {
                if (usernames != this._sessionService.user.userName) {
            var text ='/account/login?' + 'UsernameOrEmailAddress=' + route.queryParams['UsernameOrEmailAddress'] +'&password=' + route.queryParams['password'] ;
            this._authService.logout(true, text);
                }}
        }




         //EscrowBase user login Logout & reduct
           
         if(route.queryParams['UsernameOrEmailAddress'] && route.queryParams['password'] && route.queryParams['Escrow'] ){
            
              
            var usernames = atob(route.queryParams['UsernameOrEmailAddress']);
            if (this._sessionService.user) {
                if (usernames != this._sessionService.user.userName) {
            var text ='/account/login?' + 'UsernameOrEmailAddress=' + route.queryParams['UsernameOrEmailAddress'] +'&password=' + route.queryParams['password']+'&Escrow=' + route.queryParams['Escrow'] ;
            this._authService.logout(true, text);
                }
                else{
                    var text ='/account/login?' + 'UsernameOrEmailAddress=' + route.queryParams['UsernameOrEmailAddress'] +'&password=' + route.queryParams['password']+'&Escrow=' + route.queryParams['Escrow'] ;
                }
                }
        }

        // Invitation email Logout 
        if (route.queryParams['un']) {
            
            var usernames = atob(route.queryParams['un']);
            if (this._sessionService.user) {
                if (usernames != this._sessionService.user.userName) {
                    
                    var text ='/account/reset-password?' + 'ui=' + route.queryParams['ui'] + '&rc=' + route.queryParams['rc'] + '&n=' + route.queryParams['n'] + '&un=' + route.queryParams['un'] ;
                     this._authService.logout(true, text);
                }
            }
        }
        if (this._sessionService.user) {
            this._router.navigate([this.selectBestRoute()]);
            return false;
        }


        return true;
    }
    SelectRoute(){

 return '/account/reset-password';
    }

    selectBestRoute(): string {

        if (this._permissionChecker.isGranted('Pages.Administration.Users')) {
            return '/app/admin/hostDashboard';
        }

        if (this._permissionChecker.isGranted('Pages.SrFileMappings')) {
            
            return '/app/main/userdashboard';
        }
        

        return '/app/notifications';
    }
}
