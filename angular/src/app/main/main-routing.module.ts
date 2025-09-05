import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { EscrowClientsComponent } from './srEscrowClient/escrowClients/escrowClients.component';
import { UserDashboardComponent } from './Userdashboard/userdashboard.component';

import { NoTabComponent } from './NoTab/noTab.component';
import { UserDashboardAdminComponent } from './UserdashboardAdmin/userdashboardAdmin.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { EnterprisesComponent } from './srEnterprise/enterprises.component';
//import { GridViewComponent } from './Grid/grid.component';
import { FileViewComponent } from './File/filelist.component';
import { ManageLinkedAccountsModalComponent } from './ManageAccount/manageaccount.component';
import { EsignCompleteComponent } from './File/esign-complete/esign-complete.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                    { path: 'srEscrowClient/escrowClients', component: EscrowClientsComponent, data: { permission: 'Pages.EscrowClients' } },
                    { path: 'srEnterprise', component: EnterprisesComponent, data: { permission: 'Pages.Enterprises' } },
                    { path: 'dashboard', component: DashboardComponent, data: { permission: 'Pages.Tenant.Dashboard' } },
                    { path: 'File', component: FileViewComponent, data: { permission: 'Pages.SrFileMappings' } },
                    { path: 'Userdashboard', component: UserDashboardComponent, data: { permission: 'Pages.SrFileMappings' } },
                    { path: 'Notab', component:NoTabComponent, data: { permission: 'Pages.SrFileMappings' } },
                    { path: 'UserdashboardAdmin', component: UserDashboardAdminComponent, data: { permission: 'Pages.SrFileMappings' } },
                    { path: 'ManageAccount', component: ManageLinkedAccountsModalComponent, data: { permission: 'Pages.SrFileMappings' } },
                    //{ path: 'Grid', component: GridViewComponent, data: { permission: 'Pages.EscrowClients' } },
                   // { path: 'esign-complete', component: EsignCompleteComponent },
                    { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
                    { path: '**', redirectTo: 'dashboard' },

                ]
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class MainRoutingModule { }
