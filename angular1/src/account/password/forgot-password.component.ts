import { Component, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppUrlService } from '@shared/common/nav/app-url.service';
import { AccountServiceProxy, SendPasswordResetCodeInput,SecurityQuestionsServiceProxy,UserAnswersServiceProxy,UserServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';

@Component({
    templateUrl: './forgot-password.component.html',
    animations: [accountModuleAnimation()]
})
export class ForgotPasswordComponent extends AppComponentBase {

    model: SendPasswordResetCodeInput = new SendPasswordResetCodeInput();

    Verifying:boolean=false;
    quesData:any;
    quesData1:any;
    quesData2:any;
    otp;
    interval;
    mailcheck:boolean=true;
    showotp:boolean=false;
    config = {
        allowNumbersOnly: false,
        length: 6,
        isPasswordInput: false,
        disableAutoFocus: false,
        placeholder: '0',
        inputStyles: {
            'width': '30px',
            'height': '50px'
          }
      };
    text:string="Send Verification Code";
    timeLeft:any=59;
    answer:any;
    answer1:any;
    answer2:any;
    verified:boolean=false;
    toggleSecurityQuestion:boolean=false;
    securityQuestion:any;
    securityQuestion1:any;
    securityQuestion2:any;
    name:string;
    userid:number;
    phone:string;
    constructor (
        injector: Injector,
        private http: HttpClient,
        private _userAnswersServiceProxy: UserAnswersServiceProxy,
        private _router: Router,
        private _userServiceProxy:UserServiceProxy
        ) {
        super(injector);
    }

    save(): void {
        this.Verifying = true;
        this.verify();
        // this._accountService.sendPasswordResetCode(this.model)
        //     .pipe(finalize(() => { this.Verifying = false; }))
        //     .subscribe(() => {
        //         this.message.success(this.l('PasswordResetMailSentMessage'), this.l('MailSent')).then(() => {
        //             this._router.navigate(['account/login']);
        //         });this._router.navigate(['account/login']);
        //     });
    }

    checkuser(){
        this._userServiceProxy.getUsers(this.model.emailAddress,undefined,undefined,undefined,undefined,undefined,undefined).subscribe((res)=>{
if(res.items.length>0){
let result:any[] = res['items'];
let items:any[] = result['0'];
this.name=items['name']+" "+items['surname'];
this.userid=items['id'];
this.phone=items['phoneNumber'];
this.toggleSecurityQuestion=true;
this.mailcheck=false;
this.getQuestion();

}else{
    alert("!Wrong Email Id")
}
        });
    }

    onOtpChange(event){
         this.otp = event;
     }

     Resend()
    {
        let url = AppConsts.remoteServiceBaseUrl;
        this.http.get(url+"/Home/forgotPassword?uid="+this.userid+"&un="+this.model.emailAddress+"&phoneno="+this.phone+"&code=&status=Code")
      .subscribe((res)=> {
        this.timeLeft = 90;
        this.startTimer();
      },
      error => console.log(error));
    }

    getQuestion(){
        this.quesData=[];
        this.quesData1=[];
        this.quesData2=[];
        this._userAnswersServiceProxy.getAll(
            this.userid.toString(),
            undefined,
            undefined,
            undefined,undefined
        ).subscribe((result) =>{
         let data = result.items;
         data.forEach(element => {
             this.quesData.push(element.userAnswer.question);
             this.quesData1.push(element.userAnswer.question);
             this.quesData2.push(element.userAnswer.question);
         });
        });
    }

    startTimer() {
        if(this.timeLeft==90){
            clearInterval(this.interval);
        }
        this.interval = setInterval(() => {
            if(this.timeLeft > 0) {
                this.timeLeft--;
              } else {
                this.timeLeft = "00";
              }
        },1000);
        }

    verifyAnswer(){
        let url = AppConsts.remoteServiceBaseUrl;
        let conf="";
        this._userAnswersServiceProxy.getAll(
            this.userid.toString(),
            undefined,
            undefined,
            undefined,
            undefined
        ).subscribe((result) =>{
         let data = result.items;
         data.forEach(element => {
             
             if(element.userAnswer.question==this.securityQuestion){
                 if(element.userAnswer.answer==this.answer)
                 {
                        conf+="1";
                 }
                 else{
                     alert("First Answer Wrong");
                 }
             }
             if(element.userAnswer.question==this.securityQuestion1){
                if(element.userAnswer.answer==this.answer1)
                {
                    conf+="2";
                }
                else{
                    alert("Second Answer Wrong");
                }
            }
            if(element.userAnswer.question==this.securityQuestion2){
                if(element.userAnswer.answer==this.answer2)
                {
                       conf+="3";
                }
                else{
                    alert("Third Answer Wrong");
                }
            }
            
         });
         if(conf=="123"){
            this.http.get(url+"/Home/forgotPassword?uid="+this.userid+"&un="+this.model.emailAddress+"&phoneno="+this.phone+"&code=&status=Code")
                .subscribe((res)=> {
            this.showotp=true;
                 this.toggleSecurityQuestion=false;
                 this.timeLeft=90;
         this.startTimer();
                });
        }
        });
    }

    verify(){
        let url = AppConsts.remoteServiceBaseUrl;
        this.http.get(url+"/Home/forgotPassword?uid="+this.userid+"&un="+this.model.emailAddress+"&phoneno="+this.phone+"&code="+this.otp+"&status=Reset")
      .subscribe((res)=> {
        this.message.success(this.l('PasswordResetMailSentMessage'), this.l('MailSent')).then(() => {
            this._router.navigate(['account/login']);});
      });
        
    }
}
