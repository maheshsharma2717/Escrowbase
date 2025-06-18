
import { Component, Input } from '@angular/core';

@Component({
  selector: 'my-tab',
  styles: [
    `
    .pane{
      padding-bottom:30px;
      
    }
  `
  ],
  template: `
    <div [hidden]="!active" >
      <ng-content></ng-content>
      <ng-container *ngIf="template"
        [ngTemplateOutlet]="template"
        [ngTemplateOutletContext]="{ person: dataContext }"
      >
      </ng-container>
    </div>
  `
})
export class TabComponent {
    
  @Input('tabTitle') title: string;
  @Input() active = false;
  @Input() isCloseable = false;
  @Input() template;
  @Input() dataContext;
}
