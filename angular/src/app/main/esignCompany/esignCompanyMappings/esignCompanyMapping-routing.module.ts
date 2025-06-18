import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EsignCompanyMappingsComponent } from './esignCompanyMappings.component';

const routes: Routes = [
    {
        path: '',
        component: EsignCompanyMappingsComponent,
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class EsignCompanyMappingRoutingModule {}
