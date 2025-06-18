import { Injector, Component, OnInit, Inject, ViewChild,Output, Input, EventEmitter } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ThemesLayoutBaseComponent } from '@app/shared/layout/themes/themes-layout-base.component';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { DOCUMENT } from '@angular/common';
import { OffcanvasOptions } from '@metronic/app/core/_base/layout/directives/offcanvas.directive';
import { AppConsts } from '@shared/AppConsts';
import { ToggleOptions } from '@metronic/app/core/_base/layout/directives/toggle.directive';
import { EscrowDetailsServiceProxy,SrEscrowsServiceProxy } from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { TabsComponent } from '@app/tabs/tabs.component';
import {FileViewComponent} from '../../../../main/File/filelist.component'
import { PermissionCheckerService } from 'abp-ng2-module';
import { AppNavigationService } from '../../nav/app-navigation.service';
@Component({
    templateUrl: './default-layout.component.html',
    selector: 'default-layout',
    animations: [appModuleAnimation()]

    
})
export class DefaultLayoutComponent extends ThemesLayoutBaseComponent implements OnInit {
fullname:any;
company:any;
tempcompany:any;
path:any;
display:any;
display1;
detail:any;
companyLogo;
//defaultLogo = AppConsts.appBaseUrl + '/assets/common/images/Escrow-logo.png';
defaultLogo:any;
tempdefaultLogo:any;
@Input() person;
isAdmin:boolean = false;
isMenuToggled: boolean = false;
dlogo;
    menuCanvasOptions: OffcanvasOptions = {
        baseClass: 'aside',
        overlay: true,
        closeBy: 'kt_aside_close_btn',
        toggleBy: 'kt_aside_mobile_toggle'
    };

    userMenuToggleOptions: ToggleOptions = {
        target: this.document.body,
        targetState: 'topbar-mobile-on',
        toggleState: 'active'
    };

    remoteServiceBaseUrl: string = AppConsts.remoteServiceBaseUrl;

    constructor(
        injector: Injector,
        public router: Router,
        private escrowDetailsServiceProxy: EscrowDetailsServiceProxy,
        private srEscrowsServiceProxy:SrEscrowsServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _appNavigationService:AppNavigationService,
        @Inject(DOCUMENT) private document: Document,
        private _permissionChecker: PermissionCheckerService
         
    ) {
        super(injector);
    }
    @ViewChild('personEdit') editPersonTemplate;
    @ViewChild('fileManager') fileManagerTemplate;
    @ViewChild('userDashboard') userDashboardTemplate;
    @ViewChild(TabsComponent) tabsComponent;
    @ViewChild('about') aboutTemplate;
   
    ngOnInit() {
        this._appNavigationService.menuToggle$.subscribe((state) => {
            
            this.isMenuToggled = state;  
          });

        if(this._activatedRoute.snapshot.queryParams['sc']!=undefined){
        this.tempcompany = atob(this._activatedRoute.snapshot.queryParams['sc']);}
        this.defaultLogo = "";

     this.isAdmin =  this._permissionChecker.isGranted('Pages.Administration.Users')

        if(!this.isAdmin){
        this.currentTheme.baseSettings.menu.fixedAside =true;
        this.currentTheme.baseSettings.footer.fixedFooter =false;
        this.currentTheme.baseSettings.menu.searchActive =false;
        this.currentTheme.baseSettings.menu.defaultMinimizedAside =true;
        this.currentTheme.baseSettings.menu.allowAsideMinimizing =false;
        this.currentTheme.baseSettings.subHeader.fixedSubHeader =true;
        this.currentTheme.baseSettings.header.desktopFixedHeader =true;
        this.currentTheme.baseSettings.header.mobileFixedHeader =false;
        //this.defaultLogo=this.path;
        }else{

        }
        this.fullname = this.appSession.user.name +" "+this.appSession.user.surname;
        this.escrowDetailsServiceProxy.getAll(undefined,undefined,this.appSession.user.userName,undefined,undefined,undefined,undefined,undefined,undefined,undefined)
        .subscribe((result:any)=>
        {
            let eFilter = result['items'];
            for(let i=0;i<result['items'].length;i++)
       {
        let items = eFilter[0];
       items = items['escrowDetail'];
       //this.tempcompany = items['company']
       this.srEscrowsServiceProxy.getAll(undefined,undefined,undefined,undefined,undefined,undefined,
        undefined,undefined,items['company'],undefined,undefined,undefined,undefined,undefined,undefined,undefined)
        .subscribe((response:any)=>{
            let eFilter = response['items'];
            for(let i=0;i<response['items'].length;i++)
       {
           
        let items = eFilter[0];
        items = items['srEscrow'];
        this.tempdefaultLogo=items['logo'];
        this.showerHeaderB();
       }
        });
       }
        });
        this.installationMode = UrlHelper.isInstallUrl(location.href);
    }
    showerHeaderB(){
        
        if (this.router.url.includes('/File', 4)) {this.display="display:none;"
        this.display1="height: 47px; width: 100px; margin: 0px 0px 0px 0px;"
             this.detail = "Escrow Secure Web Portal";
            this.company = this.tempcompany;
            this.companyLogo = this.tempdefaultLogo;
         } else {this.display1="display:none;"
                this.display="height: 47px; width: 100px; margin: 16px 0px 0px -60px;"
                this.detail = "Escrow Secure Web Portal";
            this.defaultLogo = AppConsts.appBaseUrl + '/assets/common/images/Escrow-logo.png';
            
          }
        }
       

      
        onOpenAbout(person, refresh) {
               
            let escrow=atob(person.e);
            let userType = atob(person.u);
            this.person = person
                this.tabsComponent.openTab(escrow, this.aboutTemplate, person, true, refresh);                
          }

      


  
}
