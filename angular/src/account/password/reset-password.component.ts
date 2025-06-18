import { Component, Injector, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {formatDate } from '@angular/common';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppUrlService } from '@shared/common/nav/app-url.service';
import { AccountServiceProxy, UserServiceProxy, SrFileMappingsServiceProxy, PasswordComplexitySetting, CreateOrEditUserAnswerDto, UserAnswersServiceProxy, ProfileServiceProxy,SecurityQuestionsServiceProxy, ResetPasswordOutput, ResolveTenantIdInput, SrEscrowsServiceProxy } from '@shared/service-proxies/service-proxies';
import { LoginService } from '../login/login.service';
import { ResetPasswordModel } from './reset-password.model';
import { finalize } from 'rxjs/operators';
import { AppConsts } from '@shared/AppConsts';
import { ReCaptchaV3Service } from 'ngx-captcha';
import { AbpMultiTenancyService, AbpSessionService } from 'abp-ng2-module';
import { CreateOrEditSecurityQuestionModalComponent } from '@account/Questions/SecurityQuestionsAsk';
import { HttpClient } from '@angular/common/http';
import {LoginComponent} from '../login/login.component';
import { AppAuthService } from '@app/shared/common/auth/app-auth.service';
import { debug } from 'console';
enum CheckBoxType { CheckVerify, CheckNotVerify, NONE };

@Component({
    templateUrl: './reset-password.component.html',
    styleUrls: ['./reset-password.component.less'],
    animations: [accountModuleAnimation()]
})
export class ResetPasswordComponent extends AppComponentBase implements OnInit {       
    @ViewChild('my', {static: false}) my: ElementRef;
    @ViewChild('focus', {static: false}) focus: ElementRef;
    @ViewChild('focus1', {static: false}) focus1: ElementRef;
    @ViewChild('focus2', {static: false}) focus2: ElementRef;
    @ViewChild('createOrEditSecurityQuestionModal', { static: true }) createOrEditSecurityQuestionModal: CreateOrEditSecurityQuestionModalComponent;
    recaptchaSiteKey: string = AppConsts.recaptchaSiteKey;
    check_box_type = CheckBoxType;

  currentlyChecked: CheckBoxType;
    model: ResetPasswordModel = new ResetPasswordModel();
    passwordComplexitySetting: PasswordComplexitySetting = new PasswordComplexitySetting();
    saving = false;
text:string="Send Verification Code";
inputhref:any;
btnsubmit:boolean=false;
    quesData:any =[];
    securityQuestion1: string;
    securityQuestion2: string;
    securityQuestion3: string;
    shownLoginName = '';
    tenancyName = '';
    value:string;
    userName = '';
    Caption = '';
    interval;
    interval1;
    mail:any;
    showotp:boolean=false;
    timeLeft: any;
    timer: any = 400;
    SecurityCaption = '';
    mailid = '';
    usernameFirstLetter = '';
    quesData1: any=[];
    quesData2: any=[];
    quesData3: any=[]; 
    toggleSecurityQuestion: boolean = false;
    answer1: string;
    answer2: string;
    answer3: string;
    otp: string;
    resetp:boolean=false;
    isMultiTenancyEnabled = false;
    phoneNumber:any;
    verified:boolean=false;
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
    constructor(
        injector: Injector,
        private http: HttpClient,
        private _accountService: AccountServiceProxy,
        private elementRef: ElementRef,
        private _router: Router,
        private _userServiceProxy: UserServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _loginService: LoginService,
        private _reCaptchaV3Service: ReCaptchaV3Service,
        private _abpMultiTenancyService: AbpMultiTenancyService,
        private _appUrlService: AppUrlService,
        private _authService: AppAuthService,
        private _profileService: ProfileServiceProxy,
        private _securityQuestionsServiceProxy:SecurityQuestionsServiceProxy,
        private _UserAnswersServiceProxy:UserAnswersServiceProxy
    ) {
        super(injector);
        this.inputhref="#";
         this.getQuestion();
    }
    ngOnInit(): void {
        
      debugger
        
        if (this._activatedRoute.snapshot.queryParams['un']) {
            this.mail=atob(this._activatedRoute.snapshot.queryParams['un']);
            this.mailid = atob(this._activatedRoute.snapshot.queryParams['ui']);
            
        };
        this._profileService.getPasswordComplexitySetting().subscribe(result => {
            this.passwordComplexitySetting = result.setting;
            
        });
        if (this._activatedRoute.snapshot.queryParams['c']) {
            this.model.c = this._activatedRoute.snapshot.queryParams['c'];

            this._accountService.resolveTenantId(new ResolveTenantIdInput({ c: this.model.c })).subscribe((tenantId) => {
                this.appSession.changeTenantIfNeeded(
                    tenantId
                );
            });
        } else {
            this.model.userId = this._activatedRoute.snapshot.queryParams['ui'];
            this.model.resetCode = this._activatedRoute.snapshot.queryParams['rc'];
            this.model.userId =parseInt(atob(this.model.userId.toString()));

            this.appSession.changeTenantIfNeeded(
                this.parseTenantId(
                    this._activatedRoute.snapshot.queryParams['tenantId']
                )
            );
        }
        this.isMultiTenancyEnabled = this._abpMultiTenancyService.isEnabled;
        this.setCurrentLoginInformations();
    }

    setCurrentLoginInformations(): void {
        this._userServiceProxy.getUsers(this.mail,undefined,undefined,undefined,undefined,undefined,undefined).subscribe(res=>{
            
            if(res.items[0].passwordResetCode == null || res.items[0].passwordResetCode == undefined || res.items[0].passwordResetCode == "")
            {
               
                this._router.navigate(['account/login']);
                alert("User already exist!");
            }
            else{
            if(res.items.length>0){
            let result:any[] = res['items'];
            let items:any[] = result['0'];
            this.getQuestion();
            let tempdate = new Date(items['signInTokenExpireTimeUtc']);}}});
        if(this._activatedRoute.snapshot.queryParams['n']!=""&&this._activatedRoute.snapshot.queryParams['n']!=null&&this._activatedRoute.snapshot.queryParams['n']!=undefined)
        {
        this.userName = this._activatedRoute.snapshot.queryParams['n'];
        this.userName = atob(this.userName);
        if(this.userName != '')
        {
            this.Caption = 'Create Password';
            this.SecurityCaption = 'Please add your Security Questions to complete your registration';
        }
    }
        else
        {
            this.resetp=true;
            this.Caption = 'Enter Password';
            this.SecurityCaption = 'Please add your Security Questions to start your registration';
            this._userServiceProxy.getUsers(this.mail,undefined,undefined,undefined,undefined,undefined,undefined).subscribe(res=>{
                if(res.items.length>0){
                let result:any[] = res['items'];
                let items:any[] = result['0'];
                this.getQuestion();
                let tempdate = new Date(items['signInTokenExpireTimeUtc'])
                let now = new Date();
                if(now<=tempdate){
                }
                else{
                    alert("!Sorry your reset password session is expired, please reset password again.");
                    return this._router.navigate(['account/login']);
                }
                }else{
                    alert("!Wrong Email Id")
                }
                        });
        }
    }

    get useCaptcha(): boolean {
        return this.setting.getBoolean('App.UserManagement.UseCaptchaOnLogin');
    }

    logout(): void {
        this._authService.logout();
    }

    save(): void {
          
        this.saving = true;
    let resetPassword : any = {};
    resetPassword = this.model;
        if(this.otp){
            resetPassword.isOTP = true;
            resetPassword.otp = this.otp
        }else{
            resetPassword.isOTP = false;
            resetPassword.otp = "";
        }
        this._accountService.resetPassword(resetPassword)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe((result: ResetPasswordOutput) => {
                if (!result.canLogin) {
                    this._router.navigate(['account/login']);
                    return;
                }

                let recaptchaCallback = (token: string) => {
                    // Autheticate
                    this.saving = true;
                    this._loginService.authenticateModel.userNameOrEmailAddress = result.userName;
                    this._loginService.authenticateModel.password = this.model.password;
                    this._loginService.authenticate(() => {
                        this.saving = false;
                    }, null, token);
                };

                if (this.useCaptcha) {
                    this._reCaptchaV3Service.execute(this.recaptchaSiteKey, 'login', (token) => {
                        recaptchaCallback(token);
                    });
                } else {
                    recaptchaCallback(null);
                }
            });
    }

    parseTenantId(tenantIdAsStr?: string): number {
        let tenantId = !tenantIdAsStr ? undefined : parseInt(tenantIdAsStr);
        if (!tenantId) {
            tenantId = undefined;
        }

        return tenantId;
    }
    selectCheckBox(targetType: CheckBoxType) {
        // If the checkbox was already checked, clear the currentlyChecked variable
        if(this.currentlyChecked === targetType) {
          this.currentlyChecked = CheckBoxType.NONE;
          return;
        
        }
        this.currentlyChecked = targetType;
      }

    verifyNumber()
    {
        if(this.phoneNumber=="undefined"&&this.phoneNumber==null)
        {
            //return alert("!Please fill your number for recieve verification code");
        }
        this.showotp=true;
        this.timeLeft="00";

        this.timeLeft=90;
        if(this.timeLeft=="00"||this.timeLeft==90)
          {
            this.startTimer("");
          }
        this.otpTimer("");
        if(this.text=="Send Verification Code"){
        this.text="Verify";
        
        let url = AppConsts.remoteServiceBaseUrl;
        let num = this.phoneNumber.replace(/\D/g, '');
        if(num=="undefined" || num==null)
        {
            return alert("!Please fill your number for recieve verification code");
        }
        this.http.get(url+"/Home/verifyPhoneNumber?uid="+this.model.userId+"&n="+this.userName+"&un="+this.mail+"&phoneno="+num+"&code=")
      .subscribe((res)=> {
        
        let result = res['result'];
        if(result==="queued"){
            this.verified = true;
            abp.notify.success(result, 'success')
        }else
        {
            abp.notify.error(result, 'Error');
        }
        
      },
      error => console.log(error));
    }
    else if(this.text=="Verify"){
          
        if(this.otp!=""&&this.otp!="undefined"&&this.otp!=null)
        {
        if(this.otp.length<6){return alert("Please fill complete verification code");}else{
        let url = AppConsts.remoteServiceBaseUrl;
        let num = this.phoneNumber.replace(/\D/g, '');
        this.http.get(url+"/Home/verifyPhoneNumber?uid="+this.model.userId+"&n="+this.userName+"&un="+this.mail+"&phoneno="+num+"&code="+this.otp)
      .subscribe((res)=> {
        let result = res['result'];
        if(result!=""){
            if(this.timer!=0)
            {
            if(result===this.otp)
            {
                this.verified = true;
                this.showotp = false;
                this.toggleSecurityQuestion=true;
            }
            else{
                alert("!Wrong verification code try again")
            }
            }
        }else
        {
alert("!Code expired");
        }
    },
    error => console.log(error));}
}
else{
    return alert("!Please fill verification code first")
}
    }
    }

    Resend()
    {
        let url = AppConsts.remoteServiceBaseUrl;
        let num = this.phoneNumber.replace(/\D/g, '');
        this.http.get(url+"/Home/verifyPhoneNumber?uid="+this.model.userId+"&n="+this.userName+"&un="+this.mail+"&phoneno="+num+"&code=")
      .subscribe((res)=> {
          if(this.timeLeft=="00")
          {
        this.timeLeft = 90;
        this.startTimer(this.timeLeft);
          }
      },
      error => console.log(error));
    }
    startTimer(display) {
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

    otpTimer(display) {
        this.interval1 = setInterval(() => {
            if(this.timer > 0) {
                
                this.timer--;
                
              } else {
                this.timer = 0;
              }
        },1000);
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

togglePasswordSecurityQuestion() {
      
    if(this.resetp==false){
this.toggleSecurityQuestion = true;
let url = AppConsts.remoteServiceBaseUrl;
//  let num = this.phoneNumber.replace(/\D/g, '');
    this.http.get(url+"/Home/verifyPhoneNumber?uid="+this.model.userId+"&n="+this.userName+"&un="+this.mail+"&code="+ this.otp)
      .subscribe((res)=> {
        let result = res['result'];
        if(result==="accepted"){
            //this.verified = true;
        }else
        {
//  alert(result);
        }
        
      },
      error => console.log(error));
    }
    else{
        this.save();
    }
}
onOtpChange(event){

  //this.ngOtpInputRef.setValue(event);
    this.otp = event;
}
// saveanswer() {
//     if (this.securityQuestion1 && this.securityQuestion2 && this.securityQuestion3) {
//         // All security questions are filled out, proceed with the submission logic
//         // For example, you can submit the form data here
//         console.log("All security questions are filled out. Submitting form...");
//     } else {
//         // If any security question is not filled out, show an error message or handle it accordingly
//         console.log("Please fill out all security questions before submitting.");
//         // You can also display an error message to the user or highlight the unanswered questions
//     }
// } 

phonenumberinput(event){
    this.text="Send Verification Code"
    let newVal = event.target.value.replace(/\D/g, '');
    if (event.keyCode==8 && newVal.length <= 6) {
      newVal = newVal.substring(0, newVal.length - 1);
    }
    if (newVal.length === 0) {
      newVal = '';
    } else if (newVal.length <= 3) {
      newVal = newVal.replace(/^(\d{0,3})/, '($1)');
    } else if (newVal.length <= 6) {
      newVal = newVal.replace(/^(\d{0,3})(\d{0,3})/, '($1) $2');
    } else if (newVal.length <= 10) {
      newVal = newVal.replace(/^(\d{0,3})(\d{0,3})(\d{0,4})/, '($1) $2-$3');
    } else {
      newVal = newVal.substring(0, 10);
      newVal = newVal.replace(/^(\d{0,3})(\d{0,3})(\d{0,4})/, '($1) $2-$3');
    }
    this.phoneNumber=newVal;


   
}
Myfocus(){
   // this.my.open(); 
   
}
 update() {
 this.focus.nativeElement.focus();
    }
   update1() {
this.focus1.nativeElement.focus();
          }         
           update2() {
               this.focus2.nativeElement.focus();
                   }

          
                   saveanswer() {
                      
                    this.btnsubmit = true; // Disable submit button to prevent multiple submissions
                    let data = [];
                
                    // Prepare the data array with user answers
                    let create1 = new CreateOrEditUserAnswerDto();
                    create1.answer = this.answer1;
                    create1.question = this.securityQuestion1;
                    create1.userId = this.model.userId;
                    data.push(create1);
                
                    let create2 = new CreateOrEditUserAnswerDto();
                    create2.answer = this.answer2;
                    create2.question = this.securityQuestion2;
                    create2.userId = this.model.userId;
                    data.push(create2);
                
                    let create3 = new CreateOrEditUserAnswerDto();
                    create3.answer = this.answer3;
                    create3.question = this.securityQuestion3;
                    create3.userId = this.model.userId;
                    data.push(create3);

                    localStorage.clear();
                    sessionStorage.clear(); 
                
                    // Call the service to save the data
                    this._UserAnswersServiceProxy.createAll(data).subscribe(result => {
                        // Clear all local/session storage
                      
                
                        // Perform additional save operation or follow-up actions
                       
                
                        // Optionally, re-enable the submit button after successful save
                        this.btnsubmit = false;
                    }, error => {
                        // Handle error case
                        console.error(error);
                
                        // Re-enable the submit button if there's an error
                        this.btnsubmit = false;
                    });
                    this.save();
                    
                }



}
