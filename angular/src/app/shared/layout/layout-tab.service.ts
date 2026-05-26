import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class LayoutTabService {
    private openAboutSource = new Subject<{person: any, refresh: boolean}>();
    private selectDashboardTabSource = new Subject<void>();
    
    openAbout$ = this.openAboutSource.asObservable();
    selectDashboardTab$ = this.selectDashboardTabSource.asObservable();
    
    openAbout(person: any, refresh: boolean) {
        this.openAboutSource.next({person, refresh});
    }

    selectDashboardTab() {
        this.selectDashboardTabSource.next();
    }
}