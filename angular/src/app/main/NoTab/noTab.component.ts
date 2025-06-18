import { Component, Injector, ViewEncapsulation, ViewChild, HostListener } from '@angular/core';
import { ActivatedRoute , Router} from '@angular/router';
import { EscrowClientsServiceProxy, EscrowClientDto,SrFileMappingsServiceProxy, EscrowDetailsServiceProxy,SrEscrowsServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';

declare var window: Window & typeof globalThis;

@Component({
    templateUrl: './notab.component.html'
})



export class NoTabComponent extends AppComponentBase {
    
   
    constructor(
        injector: Injector
    ) {
        super(injector);
    }
    @HostListener('window:beforeunload')
    async ngOnDestroytest() {
          
        localStorage.removeItem('notab');

    }

 
 
    ngOnInit(): void {
          
        localStorage.setItem('notab','false');
    }


}