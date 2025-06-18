import { Component, ViewChild, Injector, Output, EventEmitter, OnInit} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { EscrowClientsServiceProxy, CreateOrEditEscrowClientDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {Observable} from "@node_modules/rxjs";


@Component({
    templateUrl: './create-or-edit-escrowClient.component.html',
    animations: [appModuleAnimation()]
})
export class CreateOrEditEscrowClientComponent extends AppComponentBase implements OnInit {
    active = false;
    saving = false;
    
    escrowClient: CreateOrEditEscrowClientDto = new CreateOrEditEscrowClientDto();




    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,        
        private _escrowClientsServiceProxy: EscrowClientsServiceProxy,
        private _router: Router
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.show(this._activatedRoute.snapshot.queryParams['id']);
    }

    show(escrowClientId?: number): void {

        if (!escrowClientId) {
            this.escrowClient = new CreateOrEditEscrowClientDto();
            this.escrowClient.id = escrowClientId;

            this.active = true;
        } else {
            this._escrowClientsServiceProxy.getEscrowClientForEdit(escrowClientId).subscribe(result => {
                this.escrowClient = result.escrowClient;


                this.active = true;
            });
        }
        
    }

    private saveInternal(): Observable<void> {
            this.saving = true;
            
        
        return this._escrowClientsServiceProxy.createOrEdit(this.escrowClient)
         .pipe(finalize(() => { 
            this.saving = false;               
            this.notify.info(this.l('SavedSuccessfully'));
         }));
    }
    
    save(): void {
        this.saveInternal().subscribe(x => {
             this._router.navigate( ['/app/admin/srEscrowClient/escrowClients']);
        })
    }
    
    saveAndNew(): void {
        this.saveInternal().subscribe(x => {
            this.escrowClient = new CreateOrEditEscrowClientDto();
        })
    }







}
