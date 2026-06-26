import { NgModule } from '@angular/core';
import { AppSharedModule } from '@app/shared/app-shared.module';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './dashboard.component';
@NgModule({
    declarations: [
        DashboardComponent
    ],
    imports: [
        AppSharedModule,
        DashboardRoutingModule
    ]
})
export class DashboardModule {
}
