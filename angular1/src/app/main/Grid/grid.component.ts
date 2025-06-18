import { NgModule } from '@angular/core';
import { GridModule, ToolbarService  } from '@syncfusion/ej2-angular-grids';
import { Component, Injector, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DashboardCustomizationConst } from '@app/shared/common/customizable-dashboard/DashboardCustomizationConsts';

@Component({
    templateUrl: './grid.component.html',
    styleUrls: ['./grid.component.less'],
    encapsulation: ViewEncapsulation.None,
	providers: [ GridModule, ToolbarService]
})
 
export class GridViewComponent extends AppComponentBase {
    dashboardName = DashboardCustomizationConst.dashboardNames.defaultTenantDashboard;
 }
