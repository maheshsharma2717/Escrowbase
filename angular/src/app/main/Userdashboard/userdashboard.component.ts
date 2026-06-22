import { Component, Injector, ViewEncapsulation, ViewChild, EventEmitter, HostListener, OnInit, Inject, Output, ComponentFactoryResolver, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EscrowClientsServiceProxy, EscrowClientDto, SrFileMappingsServiceProxy, EscrowDetailsServiceProxy, SrEscrowsServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditEscrowClientModalComponent } from '../srEscrowClient/escrowClients/create-or-edit-escrowClient-modal.component';
import { ViewEscrowClientModalComponent } from '../srEscrowClient/escrowClients/view-escrowClient-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';

import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppAuthService } from '@app/shared/common/auth/app-auth.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';
import { ChatSignalrService } from '@app/shared/layout/chat/chat-signalr.service';
import { LocalStorageService } from '@shared/utils/local-storage.service';
import { timeStamp } from 'console';
import { LoginRoutingModule } from '@account/login/login-routing.module';
import { WindowUtils } from 'msal';
declare var window: Window & typeof globalThis;
import { LayoutTabService } from '@app/shared/layout/layout-tab.service';
import { CookieConsentService } from '@shared/common/session/cookie-consent.service';
import { SharedServices } from '../../shared/common/Shared/SharedService';
declare var chrome: any;
declare var abp: any;
import Swal from 'sweetalert2/dist/sweetalert2.js';
@Component({
    templateUrl: './userdashboard.component.html',
    styles: [`
        :host ::ng-deep .p-datatable .p-datatable-tbody > tr > td {
            padding: 4px 8px !important;
        }
    `]
})



export class UserDashboardComponent extends AppComponentBase implements OnInit {

    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('createOrEditEscrowClientModal', { static: true }) createOrEditEscrowClientModal: CreateOrEditEscrowClientModalComponent;
    @ViewChild('viewEscrowClientModalComponent', { static: true }) viewEscrowClientModal: ViewEscrowClientModalComponent;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('fileManager', { static: true }) fileManagerTemplate: any;

    //  @ViewChild('TabsComponent' ,{ static: true }) tabsComponent : TabsComponent;

    advancedFiltersAreShown = false;
    filterText = '';
    escrowNumberFilter = '';
    nameFilter = '';
    emailFilter = '';
    eFilter = [];
    output = [];
    collect = '';
    collect1 = '';
    collect2 = '';
    collect3 = '';
    company = '';
    link = '';
    fileid = '';
    fileids = '';
    token = '';
    type: string;
    escrowid: string;
    data: string;
    escrowList: Array<escrows> = [];

    _entityTypeFullName = 'SR.EscrowBaseWeb.SREscrowClient.EscrowClient';
    entityHistoryEnabled = false;
    folderPath: string;
    counter: number = 0;
    loading: boolean;
    router: any;

    dataNew: any = {}
    filterLabel: any = "";
    searchText: any = "";
    escrow: any;
    ///////////////////
    isFormSubmitted = false;
    selectedESignCompany: string = '';
    eSignCreds: any = {
        clientId: '',
        clientSecret: '',
        apiAccountId: '',
        userId: '',
        folderId: '',
        refreshToken: '',
        accessToken: '',
        accessTokenTime: ''
    };
    isAdminAssigned: boolean = false;
    showESignModal: boolean = false;
    tempDataNew: any = null;
    isEOXUser: boolean = false;

    constructor(
        injector: Injector,
        private http: HttpClient,
        private _localStorageService: LocalStorageService,
        private _authService: AppAuthService,
        private ChatSignalrService: ChatSignalrService,
        private _router: Router,
        private route: ActivatedRoute,
        private _layoutTabService: LayoutTabService,
        private sharedServices: SharedServices,
        private _changeDetectorRef: ChangeDetectorRef
    ) {
        super(injector);

    }

    openNewTab() {

        var myWindow = window.open("", "_self",);
        if (!this.checkForDuplicateTab()) {
            myWindow.location.href = "https://www.escrowbaseweb.com/app/main/Userdashboard", "_self";
        }
    }


    checkForDuplicateTab() {

        var currentUrl = window.location.href;
        var tabs = window.opener ? window.opener.window.frames : window.frames;
        if (tabs.length == 0) {
            //tabs = JSON.parse(localStorage.getItem("TabList"));
        }
        if (tabs != null && tabs.length > 0) {

        }
        for (var i = 0; i < tabs.length; i++) {
            try {
                localStorage.setItem("TabList", JSON.stringify(tabs))
                var tabUrl = tabs[i].location.href;
                if (tabUrl === currentUrl) {
                    tabs[i].focus();
                    window.close();
                    return true;
                }
            } catch (e) { }
        }
        return false;
    }

    ngOnInit(): void {
        this.sharedServices.escrow$.subscribe((data) => {
            console.log("Received escrow data:", data);
            this.checkAndOpenFile(data);
        });
        console.log('Current Route' + this.router);
        if (localStorage.getItem('homeOpened') == 'true') {
            localStorage.setItem('notab', 'false');

        }

        // this.checkForDuplicateTab()
        localStorage.setItem('homeOpened', 'true');
        this._localStorageService.setItem("Log", "VAl");
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
        this.ChatSignalrService.data.subscribe(res => {

            if (res) {
                this.getEscrowClients(null, true);
            }
        });

        this.getEscrowClients(null, true);

        // Subscribe to global Edit E-Sign request
        this._layoutTabService.requestEditESignCreds$.subscribe(() => {
            const activeTab = localStorage.getItem('activeTab');
            
            let targetEscrowId = activeTab;

            // If we're on the dashboard, use the selected checkbox escrow
            if (!activeTab || activeTab === 'Dashboard') {
                targetEscrowId = localStorage.getItem('selectedDashboardEscrow');
                if (!targetEscrowId) {
                    abp.message.warn('Please select an Escrow using the checkbox in the grid first to edit its settings.', 'No Escrow Selected');
                    return;
                }
            }

            // Find the escrow in our list matching the targetEscrowId (which is the escrowid)
            const escrow = this.escrowList.find(x => x.escrowid === targetEscrowId);
            if (escrow) {
                // Ensure dataNew format is passed
                const dataNew = escrow.dataNew || {
                    c: btoa(this.validFileName(escrow.company) || ''),
                    e: btoa(escrow.escrowid || ''),
                    sc: btoa(escrow.subCompany || ''),
                    u: btoa(escrow.type || ''),
                    isId: true
                };
                this.editESignCreds(dataNew);
            } else {
                abp.message.warn('Could not find the details for the selected escrow.', 'Error');
            }
        });
    }

    selectedDashboardEscrow: string = '';

    selectEscrowForEdit(record: any, isChecked: boolean) {
        if (isChecked) {
            this.selectedDashboardEscrow = record.escrowid;
            localStorage.setItem('selectedDashboardEscrow', record.escrowid);
        } else {
            this.selectedDashboardEscrow = '';
            localStorage.removeItem('selectedDashboardEscrow');
        }
    }

    isEscrowSelectedForEdit(escrowid: string): boolean {
        return this.selectedDashboardEscrow === escrowid;
    }

    ngAfterViewInit(): void { }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return this.isGrantedAny('Pages.Administration.AuditLogs') && customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }


    validFileName(folderName) {


        let newString = folderName.replace("<", "(").replace(">", ")").replace(":", ";").replace("*", "'").replace("/", "-").replace("?", "+").replace("|", "_").replace("*", ".").replace("\/", "=");
        let str = newString.charAt(newString.length - 1)
        if (str == ".") {
            newString = newString.replace(str, "");

        }
        return newString
    }

    OpenOnSame() {

        window.location.replace("https://www.escrowbaseweb.com/app/main/Userdashboard")
    }

    availableEscrows: any[] = [];
    recentEscrows: any[] = [];
    selectedEscrow: any = null;

    populateRecentDropdown() {
        const url = AppConsts.remoteServiceBaseUrl + "/api/services/app/CurrentEscrows/GetAll";
        this.http.get(url).subscribe((res: any) => {
            const items = res?.result?.items;
            if (items && items.length > 0) {
                const currentEscrow = items[0].currentEscrow;
                const { escrowNo, companyName, subCompanyName } = currentEscrow;

                if (escrowNo) {
                    // Only show the current escrow if this user actually has it in their escrow list
                    const userOwnsEscrow = this.escrowList.some(x => x.escrowid === escrowNo);
                    if (!userOwnsEscrow) {
                        this.assembleDropdown(null);
                    } else {
                        const url1 = AppConsts.remoteServiceBaseUrl + "/api/services/app/SrInvitationRecords/GetAll";
                        this.http.get(url1).subscribe((invRes: any) => {
                            const userId = this.appSession.user.emailAddress.trim();
                            const records = invRes?.result?.items;
                            let userType = "EOX";

                            if (records) {
                                const match = records.find((x: any) =>
                                    x.srInvitationRecord.email === userId &&
                                    x.srInvitationRecord.escrowNumber === escrowNo &&
                                    x.srInvitationRecord.escrowCompany === companyName
                                );
                                if (match) {
                                    userType = match.srInvitationRecord.usertype;
                                }
                            }
                            userType = userType.replace('{', '').replace('}', '');

                            const dataNew = {
                                c: btoa(companyName),
                                e: btoa(escrowNo),
                                sc: btoa(subCompanyName),
                                u: btoa(userType),
                                isId: false
                            };

                            this.assembleDropdown({
                                label: `Current escrow :   ${escrowNo} - ${companyName}`,
                                value: dataNew
                            });
                        }, error => {
                            this.assembleDropdown(null);
                        });
                    }
                } else {
                    this.assembleDropdown(null);
                }
            } else {
                this.assembleDropdown(null);
            }
        }, error => {
            this.assembleDropdown(null);
        });
    }

    assembleDropdown(currentEscrowItem) {
        this.availableEscrows = [];
        setTimeout(() => {
            this.selectedEscrow = null;
            this._changeDetectorRef.detectChanges();
        });

        // Add "Current/Selected" escrow item first if it exists and is valid
        if (currentEscrowItem && currentEscrowItem.value !== '') {
            this.availableEscrows.push(currentEscrowItem);
        } else {
            // Keep the placeholder if no current escrow, but ensure it doesn't duplicate the HTML "Select Escrow"
            this.availableEscrows.push({ label: 'Current escrow not sent', value: '', disabled: true });
            this.selectedEscrow = "";
        }

        // Fetch recent escrows from backend
        const url = AppConsts.remoteServiceBaseUrl + "/api/services/app/EscrowAccessHistories/GetRecentEscrows";

        let headers = new HttpHeaders();
        if (abp.auth.getToken()) {
            headers = headers.set('Authorization', 'Bearer ' + abp.auth.getToken());
        }

        this.http.get(url, { headers: headers }).subscribe((res: any) => {
            const items = res?.result;
            if (items && items.length > 0) {
                this.recentEscrows = items; // Store for fallback lookup

                // Patch the current escrow item with the real ID if it matches one of the recent items
                // This ensures LogAccess receives the ID even for the 'Current' selection
                if (this.availableEscrows.length > 0 && this.availableEscrows[0].value && this.availableEscrows[0].value.e) {
                    const currentNum = atob(this.availableEscrows[0].value.e);
                    const match = items.find(i => i.escrowNumber === currentNum);
                    if (match) {
                        this.availableEscrows[0].value.realId = btoa(match.escrowId.toString());
                    }
                }

                items.forEach(item => {
                    const label = `${item.escrowNumber} - ${item.companyName}`;


                    // Skip invalid items (e.g. no escrowId)
                    if (!item.escrowId) {
                        return;
                    }

                    // Only show recent escrows the user actually has access to
                    const userOwnsThis = this.escrowList.some(x => x.escrowid === item.escrowNumber);
                    if (!userOwnsThis) {
                        return;
                    }

                    const dataNew = {
                        c: btoa(item.companyName || ''),
                        e: btoa(item.escrowNumber || ''), // Use escrowNumber for correct tab labeling
                        realId: btoa(item.escrowId.toString()), // Store valid ID for logging
                        sc: btoa(item.subCompanyName || ''),
                        u: btoa(item.userType || ''),
                        isId: false
                    };

                    this.availableEscrows.push({
                        label: label,
                        value: dataNew
                    });
                });
            }
        });
    }

    onEscrowSelect() {
        if (this.selectedEscrow) {
            this.checkAndOpenFile(this.selectedEscrow);
            setTimeout(() => {
                this.selectedEscrow = null;
                this._changeDetectorRef.detectChanges();
            });
        }
    }

    getEscrowClients(event?: LazyLoadEvent, IsRefresh: boolean = false) {
        //this.MyRefresh();

        if (IsRefresh) {
            this.loading = true;
        }

        this.loading = false;
        if (this.primengTableHelper.shouldResetPaging(event) && !IsRefresh) {

            this.paginator.changePage(0);
            return;
        }

        let url = AppConsts.remoteServiceBaseUrl;
        this.folderPath = url + '/Home/';
        this.primengTableHelper.showLoadingIndicator();
        this.http.get(this.folderPath + "GetUserCompanyDetails?username=" + this.appSession.user.emailAddress, { reportProgress: true, observe: 'events' })
            .subscribe(res => {
                this.loading = false;
                let geh = [];
                let store = [];
                let split;
                this.output = [];
                this.eFilter = [];
                if (res['body'] != undefined) {

                    geh = res['body']['result'];
                }
                for (let i = 0; i < geh.length; i++) {

                    let customObj = new escrows();
                    this.output = geh[i];
                    customObj.address = this.output['address'];
                    customObj.buyer = "Bob Buyer";
                    customObj.seller = "Sam Seller";
                    customObj.escrowid = this.output['escrowId'];
                    customObj.type = this.output['type'];
                    customObj.company = this.output['company'];
                    customObj.subCompany = this.output['subCompany'];
                    store = this.output['type'].split(',');
                    for (let i = 0; i < store.length; i++) {
                        split = store[0];
                    }
                    let com = this.output['company'];
                    com = this.validFileName(com);
                    let sub = this.output['subCompany'];

                    customObj.link = "/app/main/File?u=" + btoa(split) + "&e=" + btoa(this.output['escrowId']) + "&c=" + btoa(com) + "&sc=" + btoa(sub);
                    this.data = "/app/main/File?u=" + btoa(split) + "&e=" + btoa(this.output['escrowId']) + "&c=" + btoa(com) + "&sc=" + btoa(sub);

                    customObj.dataNew = {
                        c: btoa(com),
                        e: btoa(this.output['escrowId']),
                        sc: btoa(sub),
                        u: btoa(split),
                        isId: true
                    }
                    this.escrowList.push(customObj);
                    //this._layoutTabService.openAbout(customObj.dataNew, true);
                }

                // Check if any record corresponds to an EOX user
                this.isEOXUser = this.escrowList.some(x => x.type && x.type.includes("EOX"));

                this.escrowList = this.escrowList.filter((test, index, array) =>
                    index === array.findIndex((findTest) =>
                        findTest.escrowid === test.escrowid && findTest.type === test.type && findTest.subCompany === test.subCompany
                    )
                );

                this.escrowList = this.escrowList.filter((el, i, a) => i === a.indexOf(el))
                if (this.searchText) {
                    this.filterGrid()
                } else {
                    this.primengTableHelper.totalRecordsCount = this.escrowList.length;
                    this.primengTableHelper.records = this.escrowList;
                }
                this.primengTableHelper.hideLoadingIndicator();

                // Auto-populate escrow type dropdown if EOX user
                if (this.isEOXUser) {
                    this.populateRecentDropdown();
                }


                this.fileid = localStorage.getItem('EscrowBaseWeb/abpzerotemplate_local_storage/Escrow');
                if (this.fileid != null && this.fileid !== undefined && this.fileid != "") {

                    this.fileid = this.fileid.replace('"', '').replace('"', '');
                    console.log("Escrow local" + this.fileid);
                    //this.fileid="0003";

                    if (this.fileid != null || this.fileid != undefined) {

                        let esc = [];
                        esc = this.escrowList;
                        for (let i = 0; i < esc.length; i++) {
                            let data = esc[i];
                            if (data.escrowid == this.fileid) {
                                let c = data.company;
                                let com = this.validFileName(c);
                                let sc = data.subCompany;
                                let e = data.escrowid;
                                let u = data.type;

                                let a = 'https://www.escrowbaseweb.com/app/main/File?u=' + btoa(u) + "&e=" + btoa(e) + "&c=" + btoa(com) + "&sc=" + btoa(sc);
                                //window.open( a,  "_blank");
                                data.dataNew1 = {

                                    c: btoa(c),
                                    e: btoa(e),
                                    sc: btoa(sc),
                                    u: btoa(u),
                                }

                                // this.escrowList.push(data.dataNew1);

                                this.onOpenFileManager(data.dataNew1, false);
                                //this._layoutTabService.openAbout(data.dataNew1, true);


                                // localStorage.removeItem('EscrowBaseWeb/abpzerotemplate_local_storage/Escrow');


                                return;
                            };
                        }
                    };


                    return;
                };

            });


        if (!IsRefresh) {
            setTimeout(() => {
                this.MyRefresh();
            }, 500)

        }

        const navEntry = performance.getEntriesByType('navigation')[0] as any;
        const isReload = navEntry?.type === 'reload';
        const isDashboardRoute = this._router.url?.includes('/main/dashboard') || this._router.url?.includes('/main/Userdashboard');

        const listOfOpenTab = localStorage.getItem('OpenTabList');
        if (listOfOpenTab != null && listOfOpenTab !== '') {
            const list = JSON.parse(listOfOpenTab);

            for (let i = 0; i < list.length; i++) {
                const data = {
                    c: list[i].c,
                    e: list[i].e,
                    sc: list[i].sc,
                    u: list[i].u,
                };
                setTimeout(() => {
                    this._layoutTabService.openAbout(data, true);
                }, 500);
            }

            const savedTab = localStorage.getItem('activeTab');
            if (isReload && isDashboardRoute && (!savedTab || savedTab === 'Dashboard')) {
                setTimeout(() => {
                    // Select the dashboard tab (first static tab) after tabs are restored
                    this._layoutTabService.selectDashboardTab();
                }, 1000);
            }
        }
    }


    MyRefresh() {


        this.getEscrowClients(null, true);


    };

    reloadPage(): void {

        this.paginator.changePage(this.paginator.getPage());

    }

    createEscrowClient(): void {
        this.createOrEditEscrowClientModal.show();
    }


    showHistory(escrowClient: EscrowClientDto): void {
        this.entityTypeHistoryModal.show({
            entityId: escrowClient.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

    onOpenFileManager(dataNew, fromUI) {
        console.log('onOpenFileManager called', dataNew, fromUI);
        if (fromUI == true) {
            var queryParams = dataNew;
            const Key = 'accessTYpe' + atob(queryParams['e']);
            const userType = atob(queryParams['u']);
            localStorage.setItem(Key, userType);

            // Log access to backend ONLY if userType is EOX
            if (userType === 'EOX') {
                console.log('Calling logEscrowAccess for EOX user');

                let idToLog = atob(queryParams['e']);
                let isIdLog = queryParams['isId'];

                if (queryParams['realId']) {
                    idToLog = atob(queryParams['realId']);
                    isIdLog = true;
                }

                this.logEscrowAccess(idToLog, isIdLog);
            }
        }

        this._layoutTabService.openAbout(dataNew, false);
        this.setListOfOpenTab(dataNew);
        localStorage.setItem('activeTab', atob(dataNew.e));
    }

    logEscrowAccess(escrowIdOrNumber: string, isId: boolean = false) {
        console.log('logEscrowAccess called', escrowIdOrNumber, isId);
        let id: number | null = null;
        let num: string | null = null;

        const parsedId = parseInt(escrowIdOrNumber);
        console.log('Attempting to parse:', escrowIdOrNumber, '→ parseInt result:', parsedId, 'isNaN:', isNaN(parsedId));

        if (!isNaN(parsedId) && parsedId > 0) {
            // Successfully parsed as a numeric ID
            id = parsedId;
            console.log('Successfully parsed as numeric ID:', id);
        } else {
            // Not a valid number, treat as escrow number
            num = escrowIdOrNumber;
            console.log('Treating as escrowNumber:', num);

            // Fallback lookup from recent list
            if (this.recentEscrows && this.recentEscrows.length > 0) {
                const match = this.recentEscrows.find(x => x.escrowNumber == num);
                if (match) {
                    id = match.escrowId;
                    console.log('Resolved ID from recentEscrows:', id);
                }
            }
        }

        let url = AppConsts.remoteServiceBaseUrl + "/api/services/app/EscrowAccessHistories/LogAccess";
        const body = {
            escrowId: id,
            escrowNumber: num
        };

        console.log('LogAccess URL:', url, 'Body:', body);

        let headers = new HttpHeaders();
        if (abp.auth.getToken()) {
            headers = headers.set('Authorization', 'Bearer ' + abp.auth.getToken());
        }

        this.http.post(url, body, { headers: headers }).subscribe(() => {
            console.log('LogAccess success, populating dropdown (with delay)');
            // Add slight delay to ensure backend commit propagation
            setTimeout(() => {
                this.populateRecentDropdown();
            }, 500);
        }, error => {
            console.error('LogAccess failed', error);
        });
    }


    SearchChange() {
        this.searchText = "";
        this.filterGrid();
    }

    filterGrid() {
        if (this.filterLabel) {
            if (this.searchText) {
                const find = this.escrowList;
                let data = find.filter(x => x[this.filterLabel].toLowerCase().includes(this.searchText.toLowerCase()))

                this.primengTableHelper.records = data;
                this.primengTableHelper.totalRecordsCount = data.length;

            } else {

                this.primengTableHelper.records = this.escrowList;
                this.primengTableHelper.totalRecordsCount = this.escrowList.length;
            }
        } else {
            this.primengTableHelper.records = this.escrowList;
            this.primengTableHelper.totalRecordsCount = this.escrowList.length;
        }
    }

    setListOfOpenTab(item) {
        var listOfOpenTab = localStorage.getItem("OpenTabList")
        if (listOfOpenTab != "" && listOfOpenTab != null && listOfOpenTab != undefined) {
            var listData: any = [];
            listData = JSON.parse(listOfOpenTab)
            listData.push(item);
            listData = JSON.stringify(listData);
            localStorage.setItem("OpenTabList", listData);

        }
        else {
            var listData: any = [];
            listData.push(item);
            listData = JSON.stringify(listData);
            localStorage.setItem("OpenTabList", listData);
        }
    }
    checkAndOpenFile(dataNew: any) {
        const enterpriseName = atob(dataNew.c);
        const url = AppConsts.remoteServiceBaseUrl + "/GetEsignStatus?escrowId=" + enterpriseName;
        this.http.get(url).subscribe({
            next: (res: any) => {
                // const result = res?.result;
                const result = res?.result?.result;
                if (result?.hasCredentials) {
                    dataNew.enterpriseId = result.enterpriseId;
                    dataNew.enterpriseName = result.enterpriseName;
                    this.onOpenFileManager(dataNew, true);
                } else {
                    this.tempDataNew = {
                        ...dataNew,
                        enterpriseId: result?.enterpriseId,
                        enterpriseName: result?.enterpriseName
                    };
                    this.selectedESignCompany = 'admin-suggested';
                    this.onESignCompanyChange();
                    this.showESignModal = true;
                }
            },
            error: () => {
                this.message.error("Unable to check E-Sign configuration.");
            }
        });
    }


    editESignCreds(dataNew: any) {
        const enterpriseId = atob(dataNew.c);
        const url = AppConsts.remoteServiceBaseUrl + "/GetUserCreds?enterpriseId=" + enterpriseId;

        this.http.get(url).subscribe({
            next: (res: any) => {
                if (res?.success && res?.result) {
                    const creds = res.result.result;

                    // Fill dropdown & credentials
                    this.selectedESignCompany = creds.isAdminAssigned ? 'admin-suggested' : creds.eSignProviderCode;
                    this.eSignCreds = {
                        clientId: creds.eSignClientId,
                        clientSecret: creds.eSignClientSecret,
                        apiAccountId: creds.eSignApiAccountId,
                        userId: creds.eSignUserId,
                        folderId: creds.eSignFolderId,
                        refreshToken: creds.refreshToken,
                        accessToken: creds.accessToken,
                        //accessTokenTime: creds.accessTokenTime
                        accessTokenTime: this.convertToLocalDateTimeInputFormat(creds.accessTokenTime)
                    };

                    this.tempDataNew = {
                        ...dataNew,
                        enterpriseId: creds.enterpriseId,
                        enterpriseName: creds.enterpriseName
                    };

                    this.isAdminAssigned = creds.isAdminAssigned;
                    this.showESignModal = true; // open modal for edit
                } else {
                    this.message.error("No credentials found to edit.");
                }
            },
            error: () => {
                this.message.error("Unable to fetch credentials.");
            }
        });
    }

    // To get the time while edit 
    convertToLocalDateTimeInputFormat(utcString: string): string | null {
        if (!utcString) return null;
        const date = new Date(utcString);
        const offset = date.getTimezoneOffset();
        const localDate = new Date(date.getTime() - offset * 60000);
        return localDate.toISOString().slice(0, 16); // 'YYYY-MM-DDTHH:mm'
    }


    saveESignAndContinue() {
        this.isFormSubmitted = true;
        // Required fields for all companies
        if (!this.selectedESignCompany || !this.selectedESignCompany.trim()) {
            this.message.warn("Please select an E-Sign company.");
            return;
        }
        if (this.selectedESignCompany !== 'admin-suggested') {
            if (!this.eSignCreds.clientId || !this.eSignCreds.clientId.trim()) {
                this.message.warn("Client ID / Access Key is required.");
                return;
            }
            if (!this.eSignCreds.clientSecret || !this.eSignCreds.clientSecret.trim()) {
                this.message.warn("Client Secret / Product Key is required.");
                return;
            }
            if (this.selectedESignCompany !== '2001') {
                if (!this.eSignCreds.apiAccountId || !this.eSignCreds.apiAccountId.trim()) {
                    this.message.warn("API Account ID / Company ID is required.");
                    return;
                }
                if (!this.eSignCreds.userId || !this.eSignCreds.userId.trim()) {
                    this.message.warn("User ID is required.");
                    return;
                }
            }
            if (this.selectedESignCompany === '2001') {
                if (!this.eSignCreds.folderId || !this.eSignCreds.folderId.trim()) {
                    this.message.warn("Folder ID is required.");
                    return;
                }
            }
            if (this.selectedESignCompany !== '4001') {
                if (!this.eSignCreds.refreshToken || !this.eSignCreds.refreshToken.trim()) {
                    this.message.warn("Refresh Token is required.");
                    return;
                }
            }
        }
        // If all required fields are filled, proceed with save logic
        // ...existing save logic here...
        const isAdminFlow = this.selectedESignCompany === 'admin-suggested';
        const payload = {
            enterpriseId: this.tempDataNew.enterpriseId,
            eSignProviderCode: isAdminFlow ? null : this.selectedESignCompany,
            eSignClientId: isAdminFlow ? null : this.eSignCreds.clientId,
            eSignClientSecret: isAdminFlow ? null : this.eSignCreds.clientSecret,
            eSignApiAccountId: isAdminFlow ? null : this.eSignCreds.apiAccountId,
            eSignUserId: isAdminFlow ? null : this.eSignCreds.userId,
            eSignFolderId: isAdminFlow ? null : this.eSignCreds.folderId,
            refreshToken: isAdminFlow ? null : this.eSignCreds.refreshToken,
            accessToken: isAdminFlow ? null : this.eSignCreds.accessToken,
            accessTokenTime: isAdminFlow ? null : this.eSignCreds.accessTokenTime,
            isAdminAssigned: isAdminFlow,
            isActive: !isAdminFlow
        };
        this.http.post(AppConsts.remoteServiceBaseUrl + "/SaveUserCreds", payload)
            .subscribe((response: any) => {
                if (response?.success) {
                    this.showESignModal = false;
                    this.isFormSubmitted = false;

                    // reload record from server
                    this.http.get(AppConsts.remoteServiceBaseUrl + "/GetUserCreds?enterpriseId=" + this.tempDataNew.enterpriseId)
                        .subscribe((updated: any) => {
                            this.tempDataNew = { ...this.tempDataNew, ...updated };
                            this.onOpenFileManager(this.tempDataNew, true);
                        });

                } else {
                    this.message.error('Failed to save e-sign credentials.');
                }
            }, error => {
                this.message.error('API Error while saving credentials.');
            });

    }

    closeModal() {
        this.showESignModal = false;
    }

    onESignCompanyChange() {
        this.eSignCreds = {
            clientId: '',
            clientSecret: '',
            apiAccountId: '',
            userId: '',
            folderId: '',
            refreshToken: '',
            accessToken: '',
            accessTokenTime: ''
        };

        // Update admin flag based on selection
        this.isAdminAssigned = (this.selectedESignCompany === 'admin-suggested');
    }
}

export class escrows {
    public escrowid: string;
    public type: string;
    public company: string;
    public subCompany: string;
    public link: string;
    public address: string;
    public buyer: string;
    public seller: string;
    public dataNew: any = {}
    constructor() { }

}
