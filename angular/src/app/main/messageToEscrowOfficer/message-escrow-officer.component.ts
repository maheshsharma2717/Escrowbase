import { Component, Injector, ViewEncapsulation, ViewChild, Output, EventEmitter } from '@angular/core';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EscrowDirectMessageDetailsesServiceProxy, EscrowDetailsServiceProxy, ReminderTypeList } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import * as _ from 'lodash';
import { ModalDirective } from 'ngx-bootstrap/modal';



@Component({
    selector: 'app-message-escrow-officer',
    templateUrl: './message-escrow-officer.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class MessageEscrowOfficerComponent extends AppComponentBase {

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    escrowDirectMeesage: string = '';
    isEscrowOfficerUserExist: boolean = false;
    escrowOfficerUser: any = {};
    toMessage: boolean = false;
    email: boolean = false;
    directMessage: boolean = false;
    checkMessageType: boolean = false;
    reminderTypeList: ReminderTypeList[] = [];
    historyTabActive: boolean = false;
    isSaving: boolean = false;
    constructor(
        private _notifyService: NotifyService,
        private _escrowDirectMessageDetailsesServiceProxy: EscrowDirectMessageDetailsesServiceProxy,
        private _escrowDetailsServiceProxy: EscrowDetailsServiceProxy,
        injector: Injector,
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.getEscrowOfficerDetails();
        this.loadMessageHistory();
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }


    loadMessageHistory(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        this.primengTableHelper.showLoadingIndicator();
        this._escrowDirectMessageDetailsesServiceProxy.getAll
            (undefined,
                undefined,
                this.appSession.userId.toString(),
                this.primengTableHelper.getSorting(this.dataTable),
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)

            ).subscribe((result: any) => {

                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.hideLoadingIndicator();
            })
    }

    sendReminderToEscrow() {
        debugger;
        if (this.toMessage) {
            let reminderType = new ReminderTypeList()
            reminderType.reminderType = "Message"
            this.reminderTypeList.push(reminderType)
        }
        if (this.email) {
            let reminderType = new ReminderTypeList()
            reminderType.reminderType = "Email"
            this.reminderTypeList.push(reminderType)
        }
        if (this.directMessage) {
            let reminderType = new ReminderTypeList()
            reminderType.reminderType = "DirectMessage"
            this.reminderTypeList.push(reminderType)
        }
        if (this.reminderTypeList.length == 0) {
            this.checkMessageType = true
            return;
        }
        this.isSaving = true;

        let escrow = localStorage.getItem("activeTab")
        let userType = localStorage.getItem("accessTYpe" + escrow);
        let data: any = {};
        data.id = null;
        data.message = this.escrowDirectMeesage
        data.status = true;
        data.escrowUserId = this.escrowOfficerUser.userId;
        data.senderUserId = this.appSession.userId
        data.reminderType = this.reminderTypeList;
        data.escrowNumber = escrow;
        data.userType = userType;
        this._escrowDirectMessageDetailsesServiceProxy.createOrEdit(data).subscribe((response: any) => {
            this.reminderTypeList = [];
            this.toMessage = false;
            this.email = false;
            this.directMessage = false;
            this.escrowDirectMeesage = "";
            this.isSaving = false;
            this._notifyService.success("Message sent Successfully", "Message To Escrow Officer");
            this.reloadPage()
        })
    }
    show() {
        this.modal.show();
    }
    historyTabEvent(event: any) {
        alert("")
        this.historyTabActive = true;
    }
    directMessageTabEvent(event: any) {
        alert("")

        this.historyTabActive = false;
    }
    close() {
        this.reminderTypeList = [];
        this.toMessage = false;
        this.email = false;
        this.directMessage = false;
        this.escrowDirectMeesage = "";
        this.isSaving = false;
        this.modal.hide();
    }
    getEscrowOfficerDetails() {
        debugger
        var escrow = localStorage.getItem("activeTab");
        this._escrowDetailsServiceProxy.getEscrowOfficerDetails(escrow).subscribe((response: any) => {


            this.escrowOfficerUser = response;
            if (this.escrowOfficerUser) {
                this.isEscrowOfficerUserExist = true;
            } else {
                this.isEscrowOfficerUserExist = false;
            }

        })
    }

    onCheckboxChange(type: string, event: Event): void {

        const isChecked = (event.target as HTMLInputElement).checked;
        if (type === 'message') {
            this.toMessage = isChecked;
        }

        else if (type === 'email') {
            this.email = isChecked;
        }

        else if (type === 'directMessage') {
            this.directMessage = isChecked;
        }
    }
}
