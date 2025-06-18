import { Component, Injector, Input, OnInit } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { AppNavigationService } from '@app/shared/layout/nav/app-navigation.service';
 
import { AppComponentBase } from '@shared/common/app-component-base';
import { PermissionCheckerService } from 'abp-ng2-module';

export class BreadcrumbItem {
    text: string;
    routerLink?: string;
    navigationExtras?: NavigationExtras;
   
    constructor(text: string, routerLink?: string, navigationExtras?: NavigationExtras,     
        
    ) {
        this.text = text;
        this.routerLink = routerLink;
        this.navigationExtras = navigationExtras;
    }

    isLink(): boolean {
        return !!this.routerLink;
    }
}

@Component({
    selector: 'sub-header',
    templateUrl: './sub-header.component.html'
})
export class SubHeaderComponent extends AppComponentBase implements OnInit {

    @Input() title: string;
    @Input() description: string;
    @Input() breadcrumbs: BreadcrumbItem[];
    isAdmin:boolean = false;
    isMenuToggled:boolean = false;
    constructor(
        private _router: Router,
        private _permissionChecker: PermissionCheckerService,
        private _appNavigationService: AppNavigationService,
        injector: Injector
    ) {
        super(injector);
        this.isAdmin =  this._permissionChecker.isGranted('Pages.Administration.Users')
    }

    ngOnInit() {
        this._appNavigationService.menuToggle$.subscribe((state) => {
            this.isMenuToggled = state;  
          });
        }

    goToBreadcrumb(breadcrumb: BreadcrumbItem): void {
        if (!breadcrumb.routerLink) {
            return;
        }

        if (breadcrumb.navigationExtras) {
            this._router.navigate([breadcrumb.routerLink], breadcrumb.navigationExtras);
        } else {
            this._router.navigate([breadcrumb.routerLink]);
        }
    }
}
