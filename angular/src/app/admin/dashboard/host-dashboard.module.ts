import { NgModule } from '@angular/core';
import { AppSharedModule } from '@app/shared/app-shared.module';
import { HostDashboardRoutingModule } from './host-dashboard-routing.module';
import { HostDashboardComponent } from './host-dashboard.component';
@NgModule({
    declarations: [
        HostDashboardComponent
    ],
    imports: [
        AppSharedModule,
        HostDashboardRoutingModule
    ]
})
export class HostDashboardModule {
}
