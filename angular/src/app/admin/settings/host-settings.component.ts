import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
    ComboboxItemDto,
    CommonLookupServiceProxy,
    SettingScopes,
    HostSettingsEditDto,
    HostSettingsServiceProxy,
    SendTestEmailInput,
    JsonClaimMapDto
} from '@shared/service-proxies/service-proxies';
import { KeyValueListManagerComponent } from '@app/shared/common/key-value-list-manager/key-value-list-manager.component';
import { HttpClient } from '@angular/common/http'
import { AppConsts } from '@shared/AppConsts';
@Component({
    templateUrl: './host-settings.component.html',
    animations: [appModuleAnimation()]
})
export class HostSettingsComponent extends AppComponentBase implements OnInit {
    @ViewChild('wsFederationClaimsMappingManager') wsFederationClaimsMappingManager: KeyValueListManagerComponent;
    @ViewChild('openIdConnectClaimsMappingManager') openIdConnectClaimsMappingManager: KeyValueListManagerComponent;
    loading = false;
    hostSettings: HostSettingsEditDto;
    editions: ComboboxItemDto[] = undefined;
    testEmailAddress: string = undefined;
    showTimezoneSelection = abp.clock.provider.supportsMultipleTimezone;
    defaultTimezoneScope: SettingScopes = SettingScopes.Application;
    usingDefaultTimeZone = false;
    initialTimeZone: string = undefined;
    enabledSocialLoginSettings: string[];
    wsFederationClaimMappings: { key: string, value: string }[];
    openIdConnectClaimMappings: { key: string, value: string }[];
    eSignCompanies: { companyName: string; isActive: boolean }[] = [];
    selectedCompany: string;
    apiUrl: string = '';

    ///eSignCompanies: EsignCompanyDto[] = [];
    constructor(
        injector: Injector,
        private _hostSettingService: HostSettingsServiceProxy,
        private _commonLookupService: CommonLookupServiceProxy,
        private http: HttpClient
    ) {
        super(injector);
    }

    // loadHostSettings(): void {
    //     const self = this;
    //     self._hostSettingService.getAllSettings()
    //         .subscribe(setting => {
    //             self.hostSettings = setting;
    //             self.initialTimeZone = setting.general.timezone;
    //             self.usingDefaultTimeZone = setting.general.timezoneForComparison === self.setting.get('Abp.Timing.TimeZone');

    //             this.wsFederationClaimMappings = this.hostSettings.externalLoginProviderSettings.openIdConnectClaimsMapping
    //                 .map(item => {
    //                     return {
    //                         key: item.key,
    //                         value: item.claim
    //                     };
    //                 });
    //             this.openIdConnectClaimMappings = this.hostSettings.externalLoginProviderSettings.openIdConnectClaimsMapping
    //                 .map(item => {
    //                     return {
    //                         key: item.key,
    //                         value: item.claim
    //                     };
    //                 });
    //         });
    // }

    loadHostSettings(): void {
        this._hostSettingService.getAllSettings().subscribe(setting => {
            this.hostSettings = setting;
            this.initialTimeZone = setting.general.timezone;
            this.usingDefaultTimeZone =
                setting.general.timezoneForComparison === this.setting.get('Abp.Timing.TimeZone');

            // Handle openId and wsFederation mappings
            this.wsFederationClaimMappings = this.hostSettings.externalLoginProviderSettings.wsFederationClaimsMapping?.map(item => ({
                key: item.key,
                value: item.claim
            })) || [];

            this.openIdConnectClaimMappings = this.hostSettings.externalLoginProviderSettings.openIdConnectClaimsMapping?.map(item => ({
                key: item.key,
                value: item.claim
            })) || [];

            (this.hostSettings as any).eSign = (this.hostSettings as any).eSign ?? {
                preferredVendor: 'Zoho',
                zoho: { clientId: '' },
                docusign: { clientId: '' },
                sutisign: { apiKey: '' }
            };
        });
    }

    loadEditions(): void {
        const self = this;
        self._commonLookupService.getEditionsForCombobox(false).subscribe((result) => {
            self.editions = result.items;
            const notAssignedEdition = new ComboboxItemDto();
            notAssignedEdition.value = null;
            notAssignedEdition.displayText = self.l('NotAssigned');
            self.editions.unshift(notAssignedEdition);
        });
    }

    init(): void {
        const self = this;
        self.testEmailAddress = self.appSession.user.emailAddress;
        self.showTimezoneSelection = abp.clock.provider.supportsMultipleTimezone;
        self.loadHostSettings();
        self.loadEditions();
        self.loadSocialLoginSettings();
    }

    ngOnInit(): void {
        debugger;
        this.init();

        const url = AppConsts.remoteServiceBaseUrl;

        this.http.get<any>(url + '/Home/GetAllCompaniesAsync')
            .subscribe(response => {
                if (response && response.result) {
                    this.eSignCompanies = response.result;
                    const activeCompany = this.eSignCompanies.find(c => c.isActive);
                    if (activeCompany) {
                        this.selectedCompany = activeCompany.companyName;
                    }
                }
            });
    }


    sendTestEmail(): void {
        const self = this;
        const input = new SendTestEmailInput();
        input.emailAddress = self.testEmailAddress;
        self._hostSettingService.sendTestEmail(input).subscribe(result => {
            self.notify.info(self.l('TestEmailSentSuccessfully'));
        });
    }

    mapClaims(): void {
        if (this.wsFederationClaimsMappingManager) {
            this.hostSettings.externalLoginProviderSettings.wsFederationClaimsMapping = this.wsFederationClaimsMappingManager.getItems()
                .map(item =>
                    new JsonClaimMapDto({
                        key: item.key,
                        claim: item.value
                    })
                );
        }

        if (this.openIdConnectClaimsMappingManager) {
            this.hostSettings.externalLoginProviderSettings.openIdConnectClaimsMapping = this.openIdConnectClaimsMappingManager.getItems()
                .map(item =>
                    new JsonClaimMapDto({
                        key: item.key,
                        claim: item.value
                    })
                );
        }
    }

    saveAll(): void {
        const self = this;
        self.mapClaims();
        if (!self.hostSettings.tenantManagement.defaultEditionId || self.hostSettings.tenantManagement.defaultEditionId.toString() === 'null') {
            self.hostSettings.tenantManagement.defaultEditionId = null;
        }

        self._hostSettingService.updateAllSettings(self.hostSettings).subscribe(result => {
            self.notify.info(self.l('SavedSuccessfully'));

            if (abp.clock.provider.supportsMultipleTimezone && self.usingDefaultTimeZone && self.initialTimeZone !== self.hostSettings.general.timezone) {
                self.message.info(self.l('TimeZoneSettingChangedRefreshPageNotification')).then(() => {
                    window.location.reload();
                });
            }
        });
    }

    loadSocialLoginSettings(): void {
        const self = this;
        this._hostSettingService.getEnabledSocialLoginSettings()
            .subscribe(setting => {
                self.enabledSocialLoginSettings = setting.enabledSocialLoginSettings;
            });
    }

    // loadCompanies(): void {
    //     debugger;
    //     this._http.get<any>('https://localhost:44301/api/services/app/Home/GetAllCompaniesAsync')
    //         .subscribe(response => {
    //             if (response && response.result) {
    //                 this.eSignCompanies = response.result;
    //             }
    //         });
    // }


    saveSelectedCompany() {
        if (!this.selectedCompany) {
            console.warn("No company selected.");
            return;
        }
        let url = AppConsts.remoteServiceBaseUrl;

        this.http.post<any>(
            url + '/Home/SetActiveCompany',
            JSON.stringify(this.selectedCompany),
            {
                headers: { 'Content-Type': 'application/json' }
            }
        ).subscribe({
            next: (res) => {
                 abp.notify.success("Company updated successfully", 'Success');
            },
            error: (err) => {
                 abp.notify.error('Failed to update company:', err);
            }
        });
    }

}