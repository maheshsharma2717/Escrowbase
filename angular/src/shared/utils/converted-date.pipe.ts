import { Pipe, PipeTransform } from '@angular/core';
import { DateTime } from 'luxon';

@Pipe({
  name: 'convertedDate',
})
export class ConvertedDatePipe implements PipeTransform {
  transform(utcDate: DateTime | string | Date | null | undefined): string {
    if (!utcDate) {
      return ''; // Handle null or undefined gracefully
    }

    let localDate: DateTime;

    // Handle Luxon DateTime
    if (utcDate instanceof DateTime) {
      localDate = utcDate.toLocal(); // Convert to local timezone
    } else if (utcDate instanceof Date) {
      localDate = DateTime.fromJSDate(utcDate, { zone: 'utc' }).toLocal(); // Convert UTC Date to local
    } else if (typeof utcDate === 'string') {
      localDate = DateTime.fromISO(utcDate, { zone: 'utc' }).toLocal(); // Convert UTC string to local
    } else {
      console.error('Invalid type for ConvertedDatePipe:', utcDate);
      return 'Invalid date';
    }

    // Format the date as `dd-MM-yyyy` or `dd-MM-yyyy HH:mm:ss`
    const hasTime = localDate.hour !== 0 || localDate.minute !== 0 || localDate.second !== 0;
    return hasTime
      ? localDate.toFormat('dd-MM-yyyy HH:mm:ss')
      : localDate.toFormat('dd-MM-yyyy');
  }
}
