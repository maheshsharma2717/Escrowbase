import { Component, Injector, ViewEncapsulation, ViewChild, EventEmitter, HostListener, OnInit, Inject, Output, ComponentFactoryResolver } from '@angular/core';
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
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';

import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppAuthService } from '@app/shared/common/auth/app-auth.service';
import { HttpClient } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';
import { ChatSignalrService } from '@app/shared/layout/chat/chat-signalr.service';
import { LocalStorageService } from '@shared/utils/local-storage.service';
import { timeStamp } from 'console';
import { LoginRoutingModule } from '@account/login/login-routing.module';
import { WindowUtils } from 'msal';
declare var window: Window & typeof globalThis;
import { FileViewComponent } from '../File/filelist.component';
import { TabsComponent } from '../../tabs/tabs.component';
import { DefaultLayoutComponent } from '../../shared/layout/themes/default/default-layout.component'
import { CookieConsentService } from '@shared/common/session/cookie-consent.service';

declare var chrome: any;
import Swal from 'sweetalert2/dist/sweetalert2.js';
@Component({

    templateUrl: './userdashboard.component.html'
})



export class UserDashboardComponent extends AppComponentBase implements OnInit {

    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('createOrEditEscrowClientModal', { static: true }) createOrEditEscrowClientModal: CreateOrEditEscrowClientModalComponent;
    @ViewChild('viewEscrowClientModalComponent', { static: true }) viewEscrowClientModal: ViewEscrowClientModalComponent;

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('fileManager', { static: true }) fileManagerTemplate: FileViewComponent;

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
    fileids='';
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



    constructor(
        injector: Injector,
        private http: HttpClient,
        private _localStorageService: LocalStorageService,
        private _authService: AppAuthService,
        private ChatSignalrService: ChatSignalrService,
        private _router: Router,
        private route: ActivatedRoute,
        private _defaultLayoutComponent: DefaultLayoutComponent
    ) {
        super(injector);

    }

//Commented because it logouts session on refresh
//     @HostListener('window:beforeunload')
//     async ngOnDestroytest() {
// debugger;
//         console.log(this._router.url);

//         if (localStorage.getItem('homeOpened') == 'true') {

//             localStorage.removeItem('homeOpened');
//             localStorage.setItem('homeOpened', 'false');
//         }
       
//            if (localStorage.getItem('Signing') != 'true'){
            
//             this._authService.logout();
//                 localStorage.removeItem('OpenTabList');
//                 abp.auth.clearToken();
//                 abp.auth.clearRefreshToken();
//            }
//            else{}
//     }


    openNewTab() {
        var myWindow = window.open("", "_self",);
        if (!this.checkForDuplicateTab()) {
            myWindow.location.href = "https://www.escrowbaseweb.com/app/main/Userdashboard", "_self";
        }
    }


    checkForDuplicateTab() {
        debugger;
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
      


        console.log('Current Route' + this.router);
        if (localStorage.getItem('homeOpened') == 'true') {

            // setTimeout (window.close, 5000);
            // window.top.close();
            //window.close();
            localStorage.setItem('notab', 'false');
           // let b = 'https://www.escrowbaseweb.com/app/main/Notab';

            //Swal.fire('Escrowbase already open in another tab');
            //   window.location.href = b;


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


    }


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
                    }
                    this.escrowList.push(customObj);
                    //this._defaultLayoutComponent.onOpenAbout(customObj.dataNew);
                }
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


                  
                var listOfOpenTab = localStorage.getItem("OpenTabList")
                if (listOfOpenTab != null || listOfOpenTab !== undefined || listOfOpenTab != "") {
                if (listOfOpenTab != "" && listOfOpenTab != null && listOfOpenTab != undefined) {
                    var list = JSON.parse(listOfOpenTab);
                    list.forEach(element => {
                        debugger;
                        let data=element;
                         data.dataNew2 = {
        
                            c: data.c,
                            e: data.e,
                            sc:data.sc,
                            u: data.u,
                        }
                        debugger;
                        // this.escrowList.push(data.dataNew2);
                       
                        this._defaultLayoutComponent.onOpenAbout(data.dataNew2);
        
                    });
                }}
            

             
            

debugger;
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
                               this.onOpenFileManager(data.dataNew1);
                                //this._defaultLayoutComponent.onOpenAbout(data.dataNew1);

                              
                                localStorage.removeItem('EscrowBaseWeb/abpzerotemplate_local_storage/Escrow');
                                
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

    onOpenFileManager(dataNew) {
        debugger;
        this.setListOfOpenTab(dataNew);
        this._defaultLayoutComponent.onOpenAbout(dataNew);
    }
    SearchChange() {
        this.searchText = "";
        this.filterGrid();
    }
    filterGrid() {
        debugger;
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

    // loadTabFromStorage() {
    //     debugger;
       
    //     var listOfOpenTab = localStorage.getItem("OpenTabList")
    //     if (listOfOpenTab != null || listOfOpenTab !== undefined || listOfOpenTab != "") {
    //     if (listOfOpenTab != "" && listOfOpenTab != null && listOfOpenTab != undefined) {
    //         var list = JSON.parse(listOfOpenTab);
    //         list.forEach(element => {
    //             debugger;
    //             let data=element;
    //              data.dataNew2 = {

    //                 c: data.c,
    //                 e: data.e,
    //                 sc:data.sc,
    //                 u: data.u,
    //             }
    //             debugger;
    //             // this.escrowList.push(data.dataNew2);
               
    //             this._defaultLayoutComponent.onOpenAbout(data.dataNew2);

    //         });
    //     }}
    // }

    setListOfOpenTab(item){
        debugger;
        var listOfOpenTab = localStorage.getItem("OpenTabList")
        if (listOfOpenTab != "" && listOfOpenTab != null && listOfOpenTab != undefined) {
            var listData: any = [];
            listData = JSON.parse(listOfOpenTab)
            listData.push(item);
            listData = JSON.stringify(listData);
                localStorage.setItem("OpenTabList",listData);
            
            
        }
        else{
            var listData :any =[];
            listData.push(item);
            listData = JSON.stringify(listData);
            localStorage.setItem("OpenTabList",listData);
        }
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
    constructor() {
    }
}