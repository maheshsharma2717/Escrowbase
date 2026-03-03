import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class SharedServices {
  private escrowSource = new Subject<any>();
  escrow$ = this.escrowSource.asObservable();

  sendEscrow(data: any) {
    this.escrowSource.next(data);
  }
  
}
