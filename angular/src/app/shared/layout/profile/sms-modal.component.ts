import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, ElementRef, Injector, ViewChild } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ChangePasswordInput, PasswordComplexitySetting, ProfileServiceProxy } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'smsModal',
    templateUrl: './sms-modal.component.html',
    styleUrls: ['./sms-modal.component.less']
})
export class smsModalComponent extends AppComponentBase {

    @ViewChild('smsModal', {static: true}) modal: ModalDirective;
    typedata:any;
    SMSprefrence:any;
    SMSprefrences:any=[];
    checkbox:any=[];
    saving = false;
    active = false;
    path1:any;
    data:any;
    bs:any;
    useridd:any;
    spinnerUpl: boolean;
    file: any;
    isFile: boolean;
    fileName: string;
    
    constructor(
        
    private http: HttpClient,
        injector: Injector,
        private _profileService: ProfileServiceProxy
    ) {
        super(injector);
        let url = AppConsts.remoteServiceBaseUrl;
        this.path1=url;
        this.useridd = abp.session.userId.toString();
        this.SMSprefrences = [
            {Ichecked:false,
                function:"Upload",
                userid:parseInt(this.useridd),
                lable:"File Upload"
            },
                {Ichecked:false,
                    function:"Edit",
                lable:"File Edit",
                userid:parseInt(this.useridd)},
                    {Ichecked:false,
                        function:"Sign",
                lable:"File Signed",
                userid:parseInt(this.useridd)}
            ];
    }

    show(): void {
        this.active = true;
            this.modal.show();
            
    //         this.http.post(this.path1 +"/Home/getusersmsprefrence",id,  { reportProgress: true, observe: 'events' })
    // .subscribe(res => {
    // });
    const headers = new HttpHeaders({ 'userid': this.useridd});
    headers.append('Content-Type','application/json');
    this.http.get<any>(this.path1 +"/Home/getusersmsprefrence", { headers:headers}).subscribe((response: any) => {
       
       let val= response.result;
       for(var i = 0; i < val.length; i++) {
        let item = val[i];
        let functionName = item['functionName'];
 let find = this.SMSprefrences.find(x=> x.function == functionName)
 if(find != null){
    find.Ichecked = true;
 }



       }
      
   });
    }

    onShown(): void {
        document.getElementById('sms').focus();
    }

    close(): void {
        this.active = false;
        this.SMSprefrences=[];
        this.modal.hide();
    }

    Enterprise(event, val){
        
       // let data  = this.SMSprefrence;
       this.checkbox= event.currentTarget.checked;
         if(this.checkbox){
            var find = this.SMSprefrences.find(x=> x.function == val)
            if(find != null){
                find.Ichecked = true;
            }
         }else{
            var find = this.SMSprefrences.find(x=> x.function == val)
            if(find != null){
                find.Ichecked = false;
            }
         }
      }

    Save() {  
  
  
  let data = this.SMSprefrences.filter(x=> x.Ichecked == true)

this.http.post(this.path1 +"/Home/usersmsprefrence",data,  { reportProgress: true, observe: 'events' })
.subscribe(res => {

    abp.notify.success('SMS Setting Saved', 'Success');
    this.modal.hide();

});
  console.log(this.SMSprefrences);
  
    }
}
