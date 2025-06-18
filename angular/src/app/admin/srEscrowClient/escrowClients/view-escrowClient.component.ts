import { Component, ViewChild, Injector, Output, EventEmitter, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { EscrowClientsServiceProxy, GetEscrowClientForViewDto, EscrowClientDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';

@Component({
    templateUrl: './view-escrowClient.component.html',
    animations: [appModuleAnimation()]
})
export class ViewEscrowClientComponent extends AppComponentBase implements OnInit {

    active = false;
    saving = false;

    item: GetEscrowClientForViewDto;


    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
         private _escrowClientsServiceProxy: EscrowClientsServiceProxy
    ) {
        super(injector);
        this.item = new GetEscrowClientForViewDto();
        this.item.escrowClient = new EscrowClientDto();        
    }

    ngOnInit(): void {
        this.show(this._activatedRoute.snapshot.queryParams['id']);
    }

    show(escrowClientId: number): void {
      this._escrowClientsServiceProxy.getEscrowClientForView(escrowClientId).subscribe(result => {      
                 this.item = result;
                this.active = true;
            });       
    }
}
