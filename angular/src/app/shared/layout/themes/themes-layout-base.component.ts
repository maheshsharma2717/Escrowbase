import { Component, Injector, Inject, ViewChild, OnInit, AfterViewInit } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TenantLoginInfoDto, EditionPaymentType, SubscriptionStartType, EscrowDetailsServiceProxy, SrEscrowsServiceProxy } from '@shared/service-proxies/service-proxies';
import { Router, NavigationEnd, ActivatedRoute } from '@angular/router';
import { PermissionCheckerService } from 'abp-ng2-module';
import { AppNavigationService } from '../nav/app-navigation.service';
import { LayoutTabService } from '../layout-tab.service';
import { TabsComponent } from '@app/tabs/tabs.component';
import * as moment from 'moment';

@Component({ template: '' })
export class ThemesLayoutBaseComponent extends AppComponentBase {

    tenant: TenantLoginInfoDto = new TenantLoginInfoDto();
    subscriptionStartType = SubscriptionStartType;
    editionPaymentType: typeof EditionPaymentType = EditionPaymentType;
    installationMode = true;

    fullname: any;
    company: any;
    tempcompany: any;
    path: any;
    display: any;
    display1: any;
    detail: any;
    companyLogo: any;
    defaultLogo = AppConsts.appBaseUrl + '/assets/common/images/Escrow-logo.png';
    tempdefaultLogo: any;
    person: any;
    isAdmin: boolean = false;
    isAdminPage: boolean = false;
    isMenuToggled: boolean = false;
    dlogo: any;

    router: Router;
    escrowDetailsServiceProxy: EscrowDetailsServiceProxy;
    srEscrowsServiceProxy: SrEscrowsServiceProxy;
    _activatedRoute: ActivatedRoute;
    _appNavigationService: AppNavigationService;
    _permissionChecker: PermissionCheckerService;
    _layoutTabService: LayoutTabService;

    @ViewChild(TabsComponent) tabsComponent: TabsComponent;
    @ViewChild('about') aboutTemplate: any;

    constructor(
        injector: Injector
    ) {
        super(injector);
        this.router = injector.get(Router);
        this.escrowDetailsServiceProxy = injector.get(EscrowDetailsServiceProxy);
        this.srEscrowsServiceProxy = injector.get(SrEscrowsServiceProxy);
        this._activatedRoute = injector.get(ActivatedRoute);
        this._appNavigationService = injector.get(AppNavigationService);
        this._permissionChecker = injector.get(PermissionCheckerService);
        this._layoutTabService = injector.get(LayoutTabService);

        this._layoutTabService.openAbout$.subscribe(data => {
            this.onOpenAbout(data.person, data.refresh);
        });

        this._layoutTabService.selectDashboardTab$.subscribe(() => {
            if (this.tabsComponent && this.tabsComponent.tabs && this.tabsComponent.tabs.first) {
                this.tabsComponent.selectTab1(this.tabsComponent.tabs.first);
            }
        });

        this._appNavigationService.menuToggle$.subscribe((state) => {
            this.isMenuToggled = state;
        });

        const checkAdmin = (url: string) => {
            if (url.includes('Userdashboard')) {
                return false;
            }
            return url.includes('/app/admin/') || 
                   url.includes('/app/main/srEnterprise') || 
                   url.includes('/app/main/srEscrowClient') ||
                   url.includes('/app/admin/users') ||
                   url.includes('/app/admin/roles');
        };

        this.isAdminPage = checkAdmin(this.router.url);

        this.router.events.subscribe((event) => {
            if (event instanceof NavigationEnd) {
                this.isAdminPage = checkAdmin(event.urlAfterRedirects || event.url);
            }
        });

        if (this._activatedRoute.snapshot.queryParams['sc'] != undefined) {
            this.tempcompany = atob(this._activatedRoute.snapshot.queryParams['sc']);
        }
        this.detail = "Escrow Secure Web Portal";
        this.isAdmin = this._permissionChecker.isGranted('Pages.Administration.Users');

        this.fullname = this.appSession.user.name + " " + this.appSession.user.surname;
        this.escrowDetailsServiceProxy.getAll(undefined, undefined, this.appSession.user.userName, undefined, undefined, undefined, undefined, undefined, undefined, undefined)
            .subscribe((result: any) => {
                let eFilter = result['items'];
                if (eFilter && eFilter.length > 0) {
                    for (let i = 0; i < eFilter.length; i++) {
                        let items = eFilter[0];
                        items = items['escrowDetail'];
                        this.srEscrowsServiceProxy.getAll(undefined, undefined, undefined, undefined, undefined, undefined,
                            undefined, undefined, items['company'], undefined, undefined, undefined, undefined, undefined, undefined, undefined)
                            .subscribe((response: any) => {
                                let rFilter = response['items'];
                                if (rFilter && rFilter.length > 0) {
                                    for (let j = 0; j < rFilter.length; j++) {
                                        let rItems = rFilter[0];
                                        rItems = rItems['srEscrow'];
                                        this.tempdefaultLogo = rItems['logo'];
                                        this.showerHeaderB();
                                    }
                                }
                            });
                    }
                }
            });
    }

    showerHeaderB() {
        if (this.router.url.includes('/File', 4)) {
            this.display = "display:none;"
            this.display1 = "height: 47px; width: 100px; margin: 0px 0px 0px 0px;"
            this.detail = "Escrow Secure Web Portal";
            this.company = this.tempcompany;
            this.companyLogo = this.tempdefaultLogo;
        } else {
            this.display1 = "display:none;"
            this.display = "height: 47px; width: 100px; margin: 16px 0px 0px -60px;"
            this.detail = "Escrow Secure Web Portal";
            this.defaultLogo = AppConsts.appBaseUrl + '/assets/common/images/Escrow-logo.png';
        }
    }

    onOpenAbout(person, refresh) {
        let escrow = atob(person.e);
        let userType = atob(person.u);
        this.person = person;
        
        const navEntry = performance.getEntriesByType("navigation")[0] as any;
        const isReload = navEntry?.type === "reload";
        const isDashboardRoute = this.router.url?.includes('/main/dashboard') || this.router.url?.includes('/main/Userdashboard');
        const savedTab = localStorage.getItem('activeTab');
        
        const shouldSkipSelection = isReload && isDashboardRoute && refresh && savedTab !== escrow;
        
        if (this.tabsComponent) {
            let existingTab = this.tabsComponent.dynamicTabs.find(tab => tab.title === escrow);
            if (existingTab) {
                if (!shouldSkipSelection) {
                    this.tabsComponent.selectTab(existingTab);
                }
            } else {
                this.tabsComponent.openTab(escrow, this.aboutTemplate, person, true, refresh);
                if (!shouldSkipSelection) {
                    setTimeout(() => {
                        let newTab = this.tabsComponent.dynamicTabs.find(tab => tab.title === escrow);
                        if (newTab) {
                            this.tabsComponent.selectTab(newTab);
                        }
                    }, 0);
                }
            }
        }
    }

    subscriptionStatusBarVisible(): boolean {
        return this.appSession.tenantId > 0 && (this.appSession.tenant.isInTrialPeriod || this.subscriptionIsExpiringSoon());
    }

    subscriptionIsExpiringSoon(): boolean {
        if (this.appSession.tenant.subscriptionEndDateUtc) {
            return moment().utc().add(AppConsts.subscriptionExpireNootifyDayCount, 'days') >= moment(this.appSession.tenant.subscriptionEndDateUtc);
        }
        return false;
    }

    getSubscriptionExpiringDayCount(): number {
        if (!this.appSession.tenant.subscriptionEndDateUtc) {
            return 0;
        }
        return Math.round(moment.utc(this.appSession.tenant.subscriptionEndDateUtc).diff(moment().utc(), 'days', true));
    }

    getTrialSubscriptionNotification(): string {
        return this.l(
            'TrialSubscriptionNotification',
            `<strong>${this.appSession.tenant.edition.displayName}</strong>`,
            `<a href="/account/buy?editionPaymentType=${this.editionPaymentType.BuyNow}&editionId=${this.appSession.tenant.edition.id}&tenantId=${this.appSession.tenant.id}">${this.l('ClickHere')}</a>`
        );
    }

    getExpireNotification(localizationKey: string): string {
        return this.l(localizationKey, this.getSubscriptionExpiringDayCount());
    }

    isMobileDevice(): boolean {
        return KTUtil.isMobileDevice();
    }
}
