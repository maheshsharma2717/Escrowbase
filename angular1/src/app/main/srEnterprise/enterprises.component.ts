import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { EnterprisesServiceProxy, EnterpriseDto  } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditEnterpriseModalComponent } from './create-or-edit-enterprise-modal.component';

import { ViewEnterpriseModalComponent } from './view-enterprise-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
    templateUrl: './enterprises.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class EnterprisesComponent extends AppComponentBase {
    
    @ViewChild('createOrEditEnterpriseModal', { static: true }) createOrEditEnterpriseModal: CreateOrEditEnterpriseModalComponent;
    @ViewChild('viewEnterpriseModalComponent', { static: true }) viewEnterpriseModal: ViewEnterpriseModalComponent;
     
    @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;    
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    advancedFiltersAreShown = false;
    filterText = '';
    enterpriseNameFilter = '';
    emailFilter = '';
    phoneFilter = '';


    _entityTypeFullName = 'SR.EscrowBaseWeb.SREnterprise.Enterprise';
    entityHistoryEnabled = false;

    constructor(
        injector: Injector,
        private _enterprisesServiceProxy: EnterprisesServiceProxy,
        private _notifyService: NotifyService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService
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

    getEnterprises(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._enterprisesServiceProxy.getAll(
            this.filterText,
            this.enterpriseNameFilter,
            this.emailFilter,
            this.phoneFilter,
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        ).subscribe(result => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createEnterprise(): void {
        this.createOrEditEnterpriseModal.show();        
    }


    showHistory(enterprise: EnterpriseDto): void {
        this.entityTypeHistoryModal.show({
            entityId: enterprise.id.toString(),
            entityTypeFullName: this._entityTypeFullName,
            entityTypeDescription: ''
        });
    }

   NewEnterprise(id): void{
    
    
     }

    deleteEnterprise(enterprise: EnterpriseDto): void {
        this.message.confirm(
            '',
            this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._enterprisesServiceProxy.delete(enterprise.id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    exportToExcel(): void {
        this._enterprisesServiceProxy.getEnterprisesToExcel(
        this.filterText,
            this.enterpriseNameFilter,
            this.emailFilter,
            this.phoneFilter,
        )
        .subscribe(result => {
            this._fileDownloadService.downloadTempFile(result);
         });
    }
}
