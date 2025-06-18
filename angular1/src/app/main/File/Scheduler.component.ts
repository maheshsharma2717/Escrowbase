import { Component, ElementRef, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LinkToUserInput, UserLinkServiceProxy } from '@shared/service-proxies/service-proxies';
import { ModalDirective,BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'Scheduler',
    templateUrl: './Scheduler.component.html'
})
export class SchedulerModalComponent extends AppComponentBase {

    @ViewChild('Scheduler', {static: true}) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    linkUser: LinkToUserInput = new LinkToUserInput();

    constructor(
        injector: Injector,
        private _userLinkService: UserLinkServiceProxy
    ) {
        super(injector);
    }

    show(): void {
        this.modal.show();
    }

    onShown(): void {
    }

    save(): void {
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
