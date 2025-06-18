import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
    EsignCompanyMappingsServiceProxy,
    CreateOrEditEsignCompanyMappingDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DateTime } from 'luxon';

import { DateTimeService } from '@app/shared/common/timing/date-time.service';

@Component({
    selector: 'createOrEditEsignCompanyMappingModal',
    templateUrl: './create-or-edit-esignCompanyMapping-modal.component.html',
})
export class CreateOrEditEsignCompanyMappingModalComponent extends AppComponentBase implements OnInit {
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    esignCompanyMapping: CreateOrEditEsignCompanyMappingDto = new CreateOrEditEsignCompanyMappingDto();

    constructor(
        injector: Injector,
        private _esignCompanyMappingsServiceProxy: EsignCompanyMappingsServiceProxy,
        private _dateTimeService: DateTimeService
    ) {
        super(injector);
    }

    show(esignCompanyMappingId?: number): void {
        if (!esignCompanyMappingId) {
            this.esignCompanyMapping = new CreateOrEditEsignCompanyMappingDto();
            this.esignCompanyMapping.id = esignCompanyMappingId;

            this.active = true;
            this.modal.show();
        } else {
            this._esignCompanyMappingsServiceProxy
                .getEsignCompanyMappingForEdit(esignCompanyMappingId)
                .subscribe((result) => {
                    this.esignCompanyMapping = result.esignCompanyMapping;

                    this.active = true;
                    this.modal.show();
                });
        }
    }

    save(): void {
        this.saving = true;

        this._esignCompanyMappingsServiceProxy
            .createOrEdit(this.esignCompanyMapping)
            .pipe(
                finalize(() => {
                    this.saving = false;
                })
            )
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

    ngOnInit(): void {}
}
