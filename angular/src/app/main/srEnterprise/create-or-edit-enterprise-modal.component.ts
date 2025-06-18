import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { EnterprisesServiceProxy, CreateOrEditEnterpriseDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { id } from '@swimlane/ngx-charts';

@Component({
    selector: 'createOrEditEnterpriseModal',
    templateUrl: './create-or-edit-enterprise-modal.component.html'
})
export class CreateOrEditEnterpriseModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    enterprise: CreateOrEditEnterpriseDto = new CreateOrEditEnterpriseDto();

enterPriseId : number;

    constructor(
        injector: Injector,
        private _enterprisesServiceProxy: EnterprisesServiceProxy
    ) {
        super(injector);
    }

    show(enterpriseId?: number): void {
        if (!enterpriseId) {
            this.enterprise = new CreateOrEditEnterpriseDto();
            this.enterprise.id = enterpriseId;

            this.active = true;
            this.modal.show();
        } else {
            this._enterprisesServiceProxy.getEnterpriseForEdit(enterpriseId).subscribe(result => {
                this.enterprise = result.enterprise;


                this.active = true;
                this.modal.show();
            });
        }
        
    }



    save(): void {
            this.saving = true;
if(this.enterPriseId){
    this.enterprise.parentId = this.enterPriseId;
    
}
			this.enterprise.parentId = this.enterPriseId;
            this._enterprisesServiceProxy.createOrEdit(this.enterprise)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }







    close(): void {
        this.active = false;
        this.enterPriseId = null;
        this.modal.hide();
    }

    createEnterPrise(enterpriseId?: number): void {
        if (enterpriseId) {
            
            
     this.enterPriseId = enterpriseId;
            this.active = true;
            this.modal.show();
        }  
        
    }
}
