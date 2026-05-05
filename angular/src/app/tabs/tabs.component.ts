import {
  Component, ContentChildren,
  QueryList,
  AfterContentInit,
  ViewChild,
  ComponentFactoryResolver,
  ViewContainerRef,
  OnInit,
  ChangeDetectorRef,
  NgZone
} from '@angular/core';
import { Router } from '@angular/router';
import Swal from 'sweetalert2/dist/sweetalert2.js';

import { TabComponent } from './tab.component';
import { DynamicTabsDirective } from './dynamic-tabs.directive';
import { forEach } from 'lodash-es';
import { SharedService } from '../main/File/UserTypeChangeService';
import { delay, tap, switchMap } from "rxjs/operators";

import { AppSessionService } from '@shared/common/session/app-session.service';
import { AppConsts } from '@shared/AppConsts';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserDashboardComponent } from '../main/Userdashboard/userdashboard.component';
import { SharedServices } from '../shared/common/Shared/SharedService';
import { SignalRHelper } from '../../shared/helpers/SignalRHelper';
import { ChatSignalrService } from '@app/shared/layout/chat/chat-signalr.service';

interface Escrow {
  escrowNo: string;
  companyName: string;
  subCompanyName: string;
  userName: string;
}
@Component({
  selector: 'app-tabs',
  template: `
  <div class="d-flex w-100 align-items-center header-container justify-content-between">
  <ul class="nav nav-tabs tab-bar-custom" role="tablist">
    <li *ngFor="let tab of tabs"
        (click)="selectTab(tab); UpdateLocalStorage(tab)"
        class="nav-item"
        [class.active]="tab.active">
      <a class="nav-link tabs" [class.active]="tab.active">
        {{ tab.title }}
      </a>
    </li>

    <li *ngFor="let tab of dynamicTabs"
        (click)="selectTab(tab); UpdateLocalStorage(tab)"
        class="nav-item"
        [class.active]="tab.active">
      <a class="nav-link tabs d-flex align-items-center" [class.active]="tab.active">
        <span>{{ tab.title }}</span>
        <span class="tab-close ms-2"
              *ngIf="tab.isCloseable"
              (click)="closeTab(tab); $event.stopPropagation()">×</span>
      </a>
    </li>
  </ul>
  </div>
  <ng-content></ng-content>
  <ng-template dynamic-tabs #container></ng-template>
  `,
  styles: [
    `
    .data{
      padding:0 10px;
       color:#000; 
       margin-left:5px;
       border-radius:15px 0 15px 0;
    }

    .tabs {
      color:#000;
      line-height: 30px;      
    }

    .tab-close {
      color : #000;
      text-align : right;
      cursor : pointer;
      padding-left : 4px;
    }
   `
  ],
  styleUrls: ['./tabs.component.css']
})
export class TabsComponent implements AfterContentInit, OnInit {

  dynamicTabs: TabComponent[] = [];

  @ContentChildren(TabComponent) tabs: QueryList<TabComponent>;

  @ViewChild(DynamicTabsDirective) dynamicTabPlaceholder: DynamicTabsDirective;
  @ViewChild('escrowTemplate', { static: true }) escrowTemplate;
  @ViewChild(UserDashboardComponent, { static: true }) userDashboardComp: UserDashboardComponent;
  output = [];
  store = [];
  eFilter = [];
  Record = [];
  escrows: any;
  getEOXuser = "";
  getUsrTyp = "";
  enterprise: any;
  subenterprise: any;
  userName: any;
  activeTabTitle: string = 'Dashboard';
  getEOXENo = "";
  userType = "";
  escrowList: Array<escrows> = [];

  // Once the user explicitly opens/selects a tab after a reload, we should not force-select dashboard anymore.
  private _userInteractedAfterReload = false;
  constructor(
    private _componentFactoryResolver: ComponentFactoryResolver,
    private SharedServices: SharedServices,
    private SharedService: SharedService,
    private http: HttpClient,
    private _appSessionService: AppSessionService,
    private _chatSignalrService: ChatSignalrService,
    private cdr: ChangeDetectorRef,
    private ngZone: NgZone,
    private router: Router) {

  }
  get canShowCurrentEscrowBtn(): boolean {

    return !!(this.escrows && this.getEOXuser && this.getEOXENo);
  }

  ngAfterContentInit() {
    const savedTab = localStorage.getItem("activeTab");

    // subscribe to changes so we re-run restore when projected content becomes available
    if (this.tabs) {
      this.tabs.changes.subscribe(() => {
        this.restoreSavedTab();
      });
    }

    // Try to restore right away (if static tabs are already available)
    this.restoreSavedTab();
  }

  private restoreSavedTab() {
    const savedTab = localStorage.getItem('activeTab');
    const navEntry = performance.getEntriesByType("navigation")[0] as any;
    const isReload = navEntry?.type === "reload";
    const isDashboardRoute = window.location.pathname?.includes('/main/dashboard') || window.location.pathname?.includes('/main/Userdashboard');

    // If reloading on dashboard route, check if we should restore escrow tab or stay on dashboard
    if (isReload && isDashboardRoute && !this._userInteractedAfterReload) {
      // If there's a saved escrow tab, restore it (user was viewing escrow tab when they refreshed)
      if (savedTab && savedTab !== 'Dashboard') {
        // Try to restore the escrow tab - this will be handled by the normal restore logic below
        // Don't return early, let it fall through to restore the escrow tab
      } else {
        // No saved escrow tab, user was on dashboard - stay on dashboard
        const firstStatic = this.tabs?.first;
        if (firstStatic) {
          this.selectTab1(firstStatic);
        }
        return;
      }
    }

    // 1) If no saved tab, select first static tab if exists, else first dynamic, else nothing.
    if (!savedTab) {
      const firstStatic = this.tabs?.first;
      if (firstStatic) {
        this.selectTab1(firstStatic);
        return;
      }
      if (this.dynamicTabs.length > 0) {
        this.selectTab(this.dynamicTabs[0]);
      }
      return;
    }

    // 2) Search static tabs (QueryList) first
    const staticTab = this.tabs?.toArray().find(t => t.title === savedTab);
    if (staticTab) {
      this.selectTab(staticTab);
      return;
    }

    const openTabList = JSON.parse(localStorage.getItem('OpenTabList') || '[]');

    const dynamicTab = this.dynamicTabs.find(t =>
      t.title === savedTab &&
      openTabList.some(o => o.c === t.dataContext?.c)
    );

    if (dynamicTab) {
      this.selectTab(dynamicTab);
      return;
    }


    // 4) Not found — fallback to first available
    const firstStatic = this.tabs?.first;
    if (firstStatic) {
      this.selectTab1(firstStatic);
      return;
    }
    if (this.dynamicTabs.length > 0) {
      this.selectTab(this.dynamicTabs[0]);
    }
  }

  ngOnInit(): void {
    const navEntry = performance.getEntriesByType("navigation")[0] as any;
    const isReload = navEntry?.type === "reload";

    if (isReload) {
      this.clearEscrowSelection();       // 👈 VERY IMPORTANT
      // Only remove activeTab if on dashboard route AND no escrow tab was active
      // If an escrow tab was active, keep activeTab so it can be restored
      const isDashboardRoute = window.location.pathname?.includes('/main/dashboard') || window.location.pathname?.includes('/main/Userdashboard');
      const savedTab = localStorage.getItem('activeTab');
      // Only clear activeTab if user was on dashboard (no escrow tab saved)
      if (isDashboardRoute && (!savedTab || savedTab === 'Dashboard')) {
        localStorage.removeItem('activeTab');
      }
    }

    this._chatSignalrService.data.subscribe(escrow => {
      if (escrow) {
        this.ngZone.run(() => {
          let escrowObj: escrows;
          if (typeof escrow === "string") {
            escrowObj = new escrows();
            escrowObj.escrowNo = escrow;
          } else {
            escrowObj = escrow;
          }

          this.escrowList.push(escrowObj);
          this.GetCurrentEscrow(false);   // 👈 prevent auto-select
        });
      }
    });

    this.GetCurrentEscrow(false);
  }

  async openTab(title: string, template, data, isCloseable = true, refresh) {
    const componentFactory = this._componentFactoryResolver.resolveComponentFactory(
      TabComponent
    );
    const viewContainerRef = this.dynamicTabPlaceholder.viewContainer;
    const componentRef = viewContainerRef.createComponent(componentFactory);
    const instance: TabComponent = componentRef.instance;

    instance.title = title;
    instance.template = template;
    instance.dataContext = data;
    instance.isCloseable = isCloseable;

    this.dynamicTabs.push(instance);

    // 🔥 read localStorage AFTER component exists
    const lstactiveTab = localStorage.getItem('activeTab');

    // Only run restore flow when we're explicitly restoring tabs (refresh=true).
    // Running restore during a user-initiated open can override the user's selection (open tab then bounce back).
    if (refresh) {
      setTimeout(() => this.restoreSavedTab(), 50);
    } else {
      this._userInteractedAfterReload = true;
    }

    if (refresh) {

      let test = this.dynamicTabs.find(tab => tab.title == lstactiveTab);

      if (test) {
        this.selectTab(test);
      } else {
        this.selectTab1(this.tabs.first);
      }

    } else {
      this.selectTab(this.dynamicTabs[this.dynamicTabs.length - 1]);
    }
    this.cdr.detectChanges();
  }


  selectTab(tab: TabComponent) {
    this.tabs.toArray().forEach(t => (t.active = false));
    this.dynamicTabs.forEach(t => (t.active = false));
    tab.active = true;
    this.activeTabTitle = tab.title;
    if (tab.dataContext) {
      const data = tab.dataContext;
      this.escrows = data.escrowNo || this.escrows;
      this.enterprise = data.companyName || this.enterprise;
      this.subenterprise = data.subCompanyName || this.subenterprise;
      // Use userName from dataContext if available, otherwise use current user's full name
      this.userName = data.userName || (this._appSessionService.user.name + " " + this._appSessionService.user.surname);
      this.getEOXENo = this.escrowList.some(e => e.escrowNo === this.escrows) ? this.escrows : "";
    } else {
      // Clear username when switching to Dashboard or other static tabs
      this.userName = "";
    }
    this.UpdateLocalStorage(tab);
  }

  UpdateLocalStorage(tab: TabComponent) {
    localStorage.setItem('activeTab', tab.title);
  }

  CurrentEscrowBtn() {
    if (!this.escrows) return;
    const customObj = new escrows();
    customObj.dataNew = {
      c: btoa(this.enterprise),
      e: btoa(this.escrows),
      sc: btoa(this.subenterprise),
      u: btoa("EOX"),
    };

    this.SharedServices.sendEscrow(customObj.dataNew);
    if (!this.escrowList.find(e => e.escrowNo === this.escrows)) {
      this.escrowList.push(customObj);
    }

    // Open file/dashboard if needed
    // this.userDashboardComp.checkAndOpenFile(customObj.dataNew);
  }

  // GetCurrentEscrow() {
  //   debugger;
  //   const url = AppConsts.remoteServiceBaseUrl + "/api/services/app/CurrentEscrows/GetAll"

  //   this.http.get(url).subscribe({
  //     next: (res: any) => {
  //       const items = res?.result.items;

  //       if (items && items.length > 0) {
  //         // 1. Destructure the nested 'currentEscrow' object from the first item
  //         const {
  //           escrowNo,
  //           companyName,
  //           subCompanyName,
  //           userName
  //         } = items[0].currentEscrow;

  //         // 2. Assign the destructured properties to your component's fields
  //         this.escrows = escrowNo;
  //         this.enterprise = companyName;
  //         this.subenterprise = subCompanyName;
  //         this.userName = userName;


  //         debugger;
  //         const url1 = AppConsts.remoteServiceBaseUrl + "/api/services/app/SrInvitationRecords/GetAll"

  //         this.http.get(url1).subscribe({
  //           next: (res: any) => {
  //             debugger;
  //             const userId = this._appSessionService.user.emailAddress.trim();
  //             // const userId = "chrisford@softwarerealityinc.com";
  //             const result = res?.result.items.filter((x: any) => x.srInvitationRecord.email == userId);

  //             if (result && result.length > 0 ? result[0] : null) {
  //               result.forEach(item => {
  //                 // this.getUsrTyp = item.srInvitationRecord.usertype;
  //                 if (item.srInvitationRecord.usertype == "{EOX}") {
  //                   this.getEOXuser = item.srInvitationRecord.usertype;

  //                 }
  //                 if (item.srInvitationRecord.escrowNumber == this.escrows && item.srInvitationRecord.escrowCompany == this.enterprise) {
  //                   this.getEOXENo = item.srInvitationRecord.escrowNumber
  //                 }
  //                 else {
  //                   this.getEOXENo = "";
  //                 }
  //               });
  //             } else {
  //               console.warn('No Current escrows found.');
  //             }
  //           },
  //           error: (err) => {
  //             console.error('Error fetching CurrentEscrow', err);
  //             // Example: show error toast
  //             // this.message.error("Unable to load Current Escrow.");
  //           }
  //         });

  //       } else {
  //         console.warn('No Current escrows found.');
  //       }

  //     },
  //     error: (err) => {
  //       console.error('Error fetching CurrentEscrow', err);
  //       // Example: show error toast
  //       // this.message.error("Unable to load Current Escrow.");
  //     }
  //   });

  // }

  GetCurrentEscrow(autoSelect: boolean = true) {
    const url = AppConsts.remoteServiceBaseUrl + "/api/services/app/CurrentEscrows/GetAll";
    const url1 = AppConsts.remoteServiceBaseUrl + "/api/services/app/SrInvitationRecords/GetAll";
    this.http.get<any>(url).pipe(
      switchMap(res => {
        const items = res?.result.items;
        if (items && items.length > 0) {
          // Store all currentEscrow objects
          this.escrowList = items.map(x => x.currentEscrow);
        } else {
          this.escrowList = [];
        }
        return this.http.get<any>(url1);
      }),
      tap(res => {
        const userId = this._appSessionService.user.emailAddress.trim();
        const result = res?.result.items.filter(
          (x: any) => x.srInvitationRecord.email === userId
        );

        if (result && result.length > 0) {
          this.getEOXuser = "";
          result.forEach(item => {
            if (item.srInvitationRecord.usertype === "{EOX}") {
              this.getEOXuser = item.srInvitationRecord.usertype;
            }
          });

          const validEscrow = this.escrowList.find(e =>
            result.some(r =>
              r.srInvitationRecord.escrowNumber === e.escrowNo &&
              r.srInvitationRecord.escrowCompany === e.companyName
            )
          );

          if (validEscrow && autoSelect) {
            this.escrows = validEscrow.escrowNo;
            this.enterprise = validEscrow.companyName;
            this.subenterprise = validEscrow.subCompanyName;
            this.userName = validEscrow.userName;
            this.getEOXENo = validEscrow.escrowNo;
          } else {
            this.clearEscrowSelection();
          }

        } else {
          this.clearEscrowSelection();
        }

        this.cdr.detectChanges();
        setTimeout(() => this.restoreSavedTab(), 0);

      })
    ).subscribe({
      error: err => console.error("Error fetching escrows or invitations:", err)
    });
  }

  private clearEscrowSelection() {
    this.escrows = "";
    this.enterprise = "";
    this.subenterprise = "";
    this.userName = "";
    this.getEOXENo = "";
    this.getEOXuser = "";
  }

  selectTab1(tab: TabComponent) {
    // deactivate all tabs
    this.tabs.toArray().forEach(tab => (tab.active = false));
    this.dynamicTabs.forEach(tab => (tab.active = false));

    // activate the tab the user has clicked on.
    tab.active = true;
    this.activeTabTitle = tab.title;

    // Clear username when switching to static tabs (like Dashboard)
    this.userName = "";
  }

  closeTab(tab: TabComponent) {
    debugger;
    for (let i = 0; i < this.dynamicTabs.length; i++) {
      if (this.dynamicTabs[i] === tab) {
        // remove the tab from our array
        this.dynamicTabs.splice(i, 1);
        let viewContainerRef = this.dynamicTabPlaceholder.viewContainer;
        // let viewContainerRef = this.dynamicTabPlaceholder;
        viewContainerRef.remove(i);
        // set tab index to 1st one
        var lstactiveTab = localStorage.getItem('activeTab');
        if (tab.title == lstactiveTab) {
          this.selectTab1(this.tabs.first);
        }
        var listOfOpenTab = localStorage.getItem("OpenTabList")
        if (listOfOpenTab) {
          let storedTabs = JSON.parse(listOfOpenTab);
          // 👉 Remove storage entry by comparing a unique key
          const updatedTabs = storedTabs.filter((t: any) => {
            return t.c !== tab.dataContext.c;   // <-- USE YOUR UNIQUE PROPERTY
          });
          localStorage.setItem("OpenTabList", JSON.stringify(updatedTabs));
        }
      }
    }
  }

  closeActiveTab() {
    const activeTabs = this.dynamicTabs.filter(tab => tab.active);
    if (activeTabs.length > 0) {
      // close the 1st active tab (should only be one at a time)
      this.closeTab(activeTabs[0]);
    }
  }

}

export class escrows {
  public escrowNo: string;
  public companyName: string;
  public subCompanyName: string;
  public userName: string;
  public type?: string;
  public dataNew: any = {};
  constructor() { }
}