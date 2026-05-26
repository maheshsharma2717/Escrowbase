import { Injector, Component, OnInit, Inject, ViewChild, Output, Input, EventEmitter } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ThemesLayoutBaseComponent } from '@app/shared/layout/themes/themes-layout-base.component';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { DOCUMENT } from '@angular/common';
import { OffcanvasOptions } from '@metronic/app/core/_base/layout/directives/offcanvas.directive';
import { AppConsts } from '@shared/AppConsts';
import { ToggleOptions } from '@metronic/app/core/_base/layout/directives/toggle.directive';
import { EscrowDetailsServiceProxy, SrEscrowsServiceProxy } from '@shared/service-proxies/service-proxies';
import { Router, NavigationEnd } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { TabsComponent } from '@app/tabs/tabs.component';
import { PermissionCheckerService } from 'abp-ng2-module';
import { AppNavigationService } from '../../nav/app-navigation.service';
@Component({
    templateUrl: './default-layout.component.html',
    selector: 'default-layout',
    animations: [appModuleAnimation()]


})
export class DefaultLayoutComponent extends ThemesLayoutBaseComponent implements OnInit {
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
        @Inject(DOCUMENT) private document: Document
    ) {
        super(injector);
    }
    
    @ViewChild('personEdit') editPersonTemplate;
    @ViewChild('fileManager') fileManagerTemplate;
    @ViewChild('userDashboard') userDashboardTemplate;

    ngOnInit() {
        if (this.isAdminPage) {
            this.document.body.classList.add('aside-enabled');
            this.document.body.classList.add('admin-layout');
        } else {
            this.document.body.classList.remove('aside-enabled');
            this.document.body.classList.remove('aside-fixed');
            this.document.body.classList.remove('admin-layout');
        }

        this.router.events.subscribe((event) => {
            if (event instanceof NavigationEnd) {
                if (this.isAdminPage) {
                    this.document.body.classList.add('aside-enabled');
                    this.document.body.classList.add('admin-layout');
                } else {
                    this.document.body.classList.remove('aside-enabled');
                    this.document.body.classList.remove('aside-fixed');
                    this.document.body.classList.remove('admin-layout');
                }
            }
        });

        if (!this.isAdmin) {
            this.currentTheme.baseSettings.menu.fixedAside = true;
            this.currentTheme.baseSettings.footer.fixedFooter = false;
            this.currentTheme.baseSettings.menu.searchActive = false;
            this.currentTheme.baseSettings.menu.defaultMinimizedAside = true;
            this.currentTheme.baseSettings.menu.allowAsideMinimizing = false;
            this.currentTheme.baseSettings.subHeader.fixedSubHeader = true;
            this.currentTheme.baseSettings.header.desktopFixedHeader = true;
            this.currentTheme.baseSettings.header.mobileFixedHeader = false;
        }

        this.installationMode = UrlHelper.isInstallUrl(location.href);
    }
}
