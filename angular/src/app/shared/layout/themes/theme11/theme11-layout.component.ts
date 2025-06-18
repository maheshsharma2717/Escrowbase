import { Injector, ElementRef, Component, OnInit, Inject } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ThemesLayoutBaseComponent } from '@app/shared/layout/themes/themes-layout-base.component';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { AppConsts } from '@shared/AppConsts';
import { OffcanvasOptions } from '@metronic/app/core/_base/layout/directives/offcanvas.directive';
import { ToggleOptions } from '@metronic/app/core/_base/layout/directives/toggle.directive';
import { DOCUMENT } from '@angular/common';
import { EscrowDetailsServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './theme11-layout.component.html',
    selector: 'theme11-layout',
    animations: [appModuleAnimation()]
})
export class Theme11LayoutComponent extends ThemesLayoutBaseComponent implements OnInit {

    userMenuCanvas;
    asideMenuCanvas;
    company;

    remoteServiceBaseUrl: string = AppConsts.remoteServiceBaseUrl;

    constructor(
        injector: Injector,
        private escrowDetailsServiceProxy: EscrowDetailsServiceProxy,
        @Inject(DOCUMENT) private document: Document
    ) {
        super(injector);
    }

    ngOnInit() {
        this.installationMode = UrlHelper.isInstallUrl(location.href);
        this.escrowDetailsServiceProxy.getAll(undefined,undefined,this.appSession.user.userName,undefined,undefined,undefined,undefined,undefined,undefined,undefined)
        .subscribe((result:any)=>
        {
            result
        });
        this.defaultLogo = AppConsts.appBaseUrl + '/assets/common/images/Escrow-logo.png';

        this.userMenuCanvas = new KTOffcanvas(this.document.getElementById('kt_header_topbar'), {
            overlay: true,
            baseClass: 'topbar',
            toggleBy: 'kt_header_mobile_topbar_toggle'
        });

        this.asideMenuCanvas = new KTOffcanvas(this.document.getElementById('kt_header_bottom'), {
            overlay: true,
            baseClass: 'header-bottom',
            toggleBy: 'kt_header_mobile_toggle'
        });
    }
}
