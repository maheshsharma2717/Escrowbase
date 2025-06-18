import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { SecurityQuestionsServiceProxy, CreateOrEditSecurityQuestionDto,CreateOrEditUserAnswerDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'createOrEditSecurityQuestionModal',
    templateUrl: './SecurityQuestionsAsk.html'
})
export class CreateOrEditSecurityQuestionModalComponent extends AppComponentBase {
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    active = false;
    saving = false;

    securityQuestion: CreateOrEditSecurityQuestionDto = new CreateOrEditSecurityQuestionDto();

    quesData:any =[];
    securityQuestion1: string;
    securityQuestion2: string;
    securityQuestion3: string;
    answer1: string;
    answer2: string;
    answer3: string;
    quesData1: any=[];
    quesData2: any=[];
    quesData3: any=[];

    constructor(
        injector: Injector,
        private _securityQuestionsServiceProxy: SecurityQuestionsServiceProxy
    ) {
        super(injector);
        this.getQuestion()
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
        let data =[];
        let create = new  CreateOrEditUserAnswerDto();
      
        create.answer = this.answer1;
        create.question = this.securityQuestion1;
        data.push(create)
        let create1 = new  CreateOrEditUserAnswerDto();
      
        create1.answer = this.answer2;
        create1.question = this.securityQuestion2;
        data.push(create1)
        let create2 = new  CreateOrEditUserAnswerDto();
      
        create2.answer = this.answer3;
        create2.question = this.securityQuestion3;
        data.push(create2)       
       //input.user.userAnswer = data;
 
       this.notify.info(this.l('SavedSuccessfully'));
             
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }

    getQuestion(){
        this._securityQuestionsServiceProxy.getAll(
            undefined,
            undefined,
            undefined,
            10000
        ).subscribe(result =>{
         this.quesData = result.items;
         this.quesData1 = result.items;
         this.quesData2 = result.items;
         this.quesData3 = result.items;
        })
    }
}
