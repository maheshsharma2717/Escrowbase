import { Injector, Component, ViewEncapsulation, Inject } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DOCUMENT } from '@angular/common';
import { EscrowDetailsServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './default-brand.component.html',
    selector: 'default-brand',
    encapsulation: ViewEncapsulation.None
})
export class DefaultBrandComponent extends AppComponentBase {
company:any;
condition:boolean;
    defaultLogo = AppConsts.appBaseUrl + '/assets/common/images/Escrow-logo.png';
    remoteServiceBaseUrl: string = AppConsts.remoteServiceBaseUrl;

    constructor(
        injector: Injector,
        private escrowDetailsServiceProxy: EscrowDetailsServiceProxy,
        @Inject(DOCUMENT) private document: Document
    ) {
        super(injector);
        this.escrowDetailsServiceProxy.getAll(undefined,undefined,this.appSession.user.userName,undefined,undefined,undefined,undefined,undefined,undefined,undefined)
        .subscribe((result:any)=>
        {
            let eFilter = result['items'];
            for(let i=0;i<result['items'].length;i++)
       {
        let items = eFilter[0];
       items = items['escrowDetail'];
       this.company = items['company']
       }
        });
    }

    toggleLeftAside(): void {
        this.document.body.classList.toggle('aside-minimize');
        this.triggerAsideToggleClickEvent();
    }

    triggerAsideToggleClickEvent(): void {
        this.condition = true;
        abp.event.trigger('app.kt_aside_toggler.onClick');
    }
}
