import { AppConsts } from '@shared/AppConsts';
import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetEsignCompanyMappingForViewDto, EsignCompanyMappingDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewEsignCompanyMappingModal',
    templateUrl: './view-esignCompanyMapping-modal.component.html',
})
export class ViewEsignCompanyMappingModalComponent extends AppComponentBase {
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetEsignCompanyMappingForViewDto;

    constructor(injector: Injector) {
        super(injector);
        this.item = new GetEsignCompanyMappingForViewDto();
        this.item.esignCompanyMapping = new EsignCompanyMappingDto();
    }

    show(item: GetEsignCompanyMappingForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
