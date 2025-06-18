import { NgModule } from '@angular/core';
import { AppSharedModule } from '@app/shared/app-shared.module';
import { AdminSharedModule } from '@app/admin/shared/admin-shared.module';
import { EsignCompanyMappingRoutingModule } from './esignCompanyMapping-routing.module';
import { EsignCompanyMappingsComponent } from './esignCompanyMappings.component';
import { CreateOrEditEsignCompanyMappingModalComponent } from './create-or-edit-esignCompanyMapping-modal.component';
import { ViewEsignCompanyMappingModalComponent } from './view-esignCompanyMapping-modal.component';

@NgModule({
    declarations: [
        EsignCompanyMappingsComponent,
        CreateOrEditEsignCompanyMappingModalComponent,
        ViewEsignCompanyMappingModalComponent,
    ],
    imports: [AppSharedModule, EsignCompanyMappingRoutingModule, AdminSharedModule],
})
export class EsignCompanyMappingModule {}
