import { Component, Injector, ViewChild, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
    UserLoginServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { Table } from 'primeng/table';
import { filter as _filter } from 'lodash-es';
import { finalize } from 'rxjs/operators';
import { DateTimeService } from '@app/shared/common/timing/date-time.service';
import { DateTime } from 'luxon';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { PrimengTableHelper } from 'shared/helpers/PrimengTableHelper';

@Component({
    templateUrl: './login-attempts.component.html',
    animations: [appModuleAnimation()]
})
export class LoginAttemptsComponent extends AppComponentBase implements OnInit {

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    public filter: string;
    public dateRange: DateTime[] = [this._dateTimeService.getStartOfWeek(), this._dateTimeService.getEndOfDay()];
    primengTableHelper = new PrimengTableHelper();

    constructor(
        injector: Injector,
        private _dateTimeService: DateTimeService,
        private _userLoginService: UserLoginServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
    }

    getLoginAttempts(event?: LazyLoadEvent): void {
        this.primengTableHelper.showLoadingIndicator();

        this._userLoginService.getRecentUserLoginAttempts()
            .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
            .subscribe(result => {
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.totalRecordsCount = result.items ? result.items.length : 0;
                this.primengTableHelper.hideLoadingIndicator();
            });
    }
}
