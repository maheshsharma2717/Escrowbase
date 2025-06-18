import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { EscrowClientsServiceProxy, EscrowClientDto,SrFileMappingsServiceProxy, EscrowDetailsServiceProxy,SrEscrowsServiceProxy } from '@shared/service-proxies/service-proxies';
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
import { HttpClient } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';

@Component({
    templateUrl: './userdashboardAdmin.component.html'
})



export class UserDashboardAdminComponent extends AppComponentBase {
    // dashboardName = DashboardCustomizationConst.dashboardNames.defaultTenantDashboard;
    
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
    @ViewChild('createOrEditEscrowClientModal', { static: true }) createOrEditEscrowClientModal: CreateOrEditEscrowClientModalComponent;
    @ViewChild('viewEscrowClientModalComponent', { static: true }) viewEscrowClientModal: ViewEscrowClientModalComponent;   
    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

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
    type: string;
 escrowid : string;
 escrowList: Array<escrows> = [];


    _entityTypeFullName = 'SR.EscrowBaseWeb.SREscrowClient.EscrowClient';
    entityHistoryEnabled = false;
    folderPath: string;

    constructor(
        injector: Injector,
        private http: HttpClient
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    }

    private setIsEntityHistoryEnabled(): boolean {
        let customSettings = (abp as any).custom;
        return this.isGrantedAny('Pages.Administration.AuditLogs') && customSettings.EntityHistory && customSettings.EntityHistory.isEnabled && _.filter(customSettings.EntityHistory.enabledEntities, entityType => entityType === this._entityTypeFullName).length === 1;
    }

    getEscrowClients(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
        let url = AppConsts.remoteServiceBaseUrl;
        this.folderPath = url + '/Home/';
        this.primengTableHelper.showLoadingIndicator();
        this.http.get(this.folderPath+"GetUserCompanyDetails?username="+this.appSession.user.emailAddress, {reportProgress: true, observe: 'events' })
.subscribe(res=> {
    let geh=[];
    let store =[];
let split;
    this.output = [];
this.eFilter = [];
    if(res['body']!=undefined){
    geh=res['body']['result'];}
    for(let i=0;i<geh.length;i++){
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
       for(let i =0;i<store.length;i++)
       {
           split =store[0];
       }
       let com = this.output['company'].replace(/\s/g, "");
       let sub = this.output['subCompany'].replace(/\s/g, "");
       customObj.link = "/app/main/File?u="+btoa(split)+"&e="+btoa(this.output['escrowId'])+"&c="+btoa(com)+"&sc="+btoa(sub);
       this.escrowList.push(customObj);
    }
    this.escrowList = this.escrowList.filter((test, index, array) =>
     index === array.findIndex((findTest) =>
        findTest.escrowid === test.escrowid && findTest.type === test.type && findTest.subCompany === test.subCompany
     )
  );
       this.escrowList = this.escrowList.filter((el, i, a) => i === a.indexOf(el))
       this.primengTableHelper.totalRecordsCount = this.escrowList.length;
       this.primengTableHelper.records = this.escrowList;
       this.primengTableHelper.hideLoadingIndicator();
});

    }

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

}

export class escrows{
    public escrowid:string;
    public type: string;
    public company: string;
    public subCompany: string;
    public link: string;
public address: string;
public buyer: string;
public seller: string;
    constructor() {
    }
}