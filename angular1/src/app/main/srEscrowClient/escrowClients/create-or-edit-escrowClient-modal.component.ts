import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { EscrowClientsServiceProxy, CreateOrEditEscrowClientDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditEscrowClientModal',
    templateUrl: './create-or-edit-escrowClient-modal.component.html'
})
export class CreateOrEditEscrowClientModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    escrowClient: CreateOrEditEscrowClientDto = new CreateOrEditEscrowClientDto();



    constructor(
        injector: Injector,
        private _escrowClientsServiceProxy: EscrowClientsServiceProxy
    ) {
        super(injector);
    }

    show(escrowClientId?: number): void {

        if (!escrowClientId) {
            this.escrowClient = new CreateOrEditEscrowClientDto();
            this.escrowClient.id = escrowClientId;

            this.active = true;
            this.modal.show();
        } else {
            this._escrowClientsServiceProxy.getEscrowClientForEdit(escrowClientId).subscribe(result => {
                this.escrowClient = result.escrowClient;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
            this._escrowClientsServiceProxy.createOrEdit(this.escrowClient)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }







    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
