import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { SecurityQuestionsServiceProxy, CreateOrEditSecurityQuestionDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditSecurityQuestionModal',
    templateUrl: './create-or-edit-securityQuestion-modal.component.html'
})
export class CreateOrEditSecurityQuestionModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    securityQuestion: CreateOrEditSecurityQuestionDto = new CreateOrEditSecurityQuestionDto();



    constructor(
        injector: Injector,
        private _securityQuestionsServiceProxy: SecurityQuestionsServiceProxy
    ) {
        super(injector);
    }

    show(securityQuestionId?: number): void {

        if (!securityQuestionId) {
            this.securityQuestion = new CreateOrEditSecurityQuestionDto();
            this.securityQuestion.id = securityQuestionId;

            this.active = true;
            this.modal.show();
        } else {
            this._securityQuestionsServiceProxy.getSecurityQuestionForEdit(securityQuestionId).subscribe(result => {
                this.securityQuestion = result.securityQuestion;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
            this._securityQuestionsServiceProxy.createOrEdit(this.securityQuestion)
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
