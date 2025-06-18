import { Pipe, PipeTransform } from '@angular/core';
import * as moment from 'moment';

@Pipe({ name: 'momentFromNow' })

export class MomentFromNowPipe implements PipeTransform {
    transform(value: string | Date): string {
        if (!value) {
          return '';
        }
    
        // Force moment to treat the input as local time, ignoring timezone
        const localTime = moment.parseZone(value).local(); // Treat as local and ignore timezone
        return localTime.fromNow();
      }
}

