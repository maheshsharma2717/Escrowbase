import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetEscrowClientForViewDto, EscrowClientDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewEscrowClientModal',
    templateUrl: './view-escrowClient-modal.component.html'
})
export class ViewEscrowClientModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetEscrowClientForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetEscrowClientForViewDto();
        this.item.escrowClient = new EscrowClientDto();
    }

    show(item: GetEscrowClientForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
