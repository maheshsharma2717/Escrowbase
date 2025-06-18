import { CommonModule,DOCUMENT } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HttpClientJsonpModule } from '@angular/common/http';

import { AppCommonModule } from '@app/shared/common/app-common.module';
import { EscrowClientsComponent } from './srEscrowClient/escrowClients/escrowClients.component';
import { ViewEscrowClientModalComponent } from './srEscrowClient/escrowClients/view-escrowClient-modal.component';
import { CreateOrEditEscrowClientModalComponent } from './srEscrowClient/escrowClients/create-or-edit-escrowClient-modal.component';

import { UtilsModule } from '@shared/utils/utils.module';
import { CountoModule } from 'angular2-counto';
import { ModalModule } from 'ngx-bootstrap/modal';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { PopoverModule } from 'ngx-bootstrap/popover';
import { UserDashboardComponent } from './Userdashboard/userdashboard.component';

import { NoTabComponent } from './NoTab/noTab.component';
import { UserDashboardAdminComponent } from './UserdashboardAdmin/userdashboardAdmin.component';
import { MainRoutingModule } from './main-routing.module';
import { NgxChartsModule } from '@swimlane/ngx-charts';

import { BsDatepickerConfig, BsDaterangepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { NgxBootstrapDatePickerConfigService } from 'assets/ngx-bootstrap/ngx-bootstrap-datepicker-config.service';

import { EnterprisesComponent } from './srEnterprise/enterprises.component';
import { ViewEnterpriseModalComponent } from './srEnterprise/view-enterprise-modal.component';
import { CreateOrEditEnterpriseModalComponent } from './srEnterprise/create-or-edit-enterprise-modal.component';

import { AutoCompleteModule } from 'primeng/autocomplete';
import { PaginatorModule } from 'primeng/paginator';
import { EditorModule } from 'primeng/editor';
import { InputMaskModule } from 'primeng/inputmask';
import { ManageLinkedAccountsModalComponent } from './ManageAccount/manageaccount.component';
import { FileUploadModule } from 'primeng/fileupload';
import { TableModule } from 'primeng/table';
import {FileViewComponent} from '../main/File/filelist.component'
import {SrFileMappingsServiceProxy,CreateOrEditUserFileLogDto} from '@shared/service-proxies/service-proxies'
//import { ButtonModule, CheckBoxModule   } from '@syncfusion/ej2-angular-buttons';

//import { ContextMenuModule ,ToolbarModule  } from '@syncfusion/ej2-angular-navigations';
//import {FileManagerModule, NavigationPaneService, ToolbarService, DetailsViewService } from '@syncfusion/ej2-angular-filemanager';
import { map } from 'rxjs/operators';
import { DxFileManagerModule, DxPopupModule } from 'devextreme-angular';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import RemoteFileSystemProvider from 'devextreme/file_management/remote_provider';
import { from } from 'rxjs';

NgxBootstrapDatePickerConfigService.registerNgxBootstrapDatePickerLocales();

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ModalModule,
        TabsModule,
        TooltipModule,
        AppCommonModule,
        UtilsModule,
        MainRoutingModule,
        CountoModule,
        NgxChartsModule,
        BsDatepickerModule.forRoot(),
        BsDropdownModule.forRoot(),
        PopoverModule.forRoot(),
        FileUploadModule,
		AutoCompleteModule,
		PaginatorModule,
		EditorModule,
        InputMaskModule,
        TableModule,
        //FileManagerAllModule,
        //CheckBoxModule, ButtonModule, ContextMenuModule,FileManagerModule, ToolbarModule,
        HttpClientModule,HttpClientJsonpModule,
        DxFileManagerModule,
        DxPopupModule,
        NgxExtendedPdfViewerModule
    ],
    declarations: [
		EscrowClientsComponent,
        //ManageLinkedAccountsModalComponent,
		ViewEscrowClientModalComponent,
		CreateOrEditEscrowClientModalComponent,
        UserDashboardComponent,
        NoTabComponent,
        UserDashboardAdminComponent,
        EnterprisesComponent,
        CreateOrEditEnterpriseModalComponent,
        ViewEnterpriseModalComponent
       
    ],
    providers: [
        { provide: BsDatepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig },
        { provide: BsDaterangepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig },
        { provide: BsLocaleService, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale },
        SrFileMappingsServiceProxy
    ]
})
export class MainModule { }
