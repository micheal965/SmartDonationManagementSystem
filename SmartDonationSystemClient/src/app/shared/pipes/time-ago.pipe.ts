import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timeAgo',
  standalone: true,
})
export class TimeAgoPipe implements PipeTransform {
  transform(value: Date | string): string {
    if (!value) return '';

    // 1. Force UTC if it's a string missing a timezone indicator
    let date: Date;
    if (
      typeof value === 'string' &&
      !value.includes('Z') &&
      !value.includes('+')
    ) {
      date = new Date(`${value}Z`);
    } else {
      date = new Date(value);
    }

    const now = new Date();
    const seconds = Math.floor((now.getTime() - date.getTime()) / 1000);

    // If the server time is slightly ahead of the client time due to sync issues
    if (seconds < 0) return 'Just now';

    if (seconds < 60) return 'Just now';

    const intervals = [
      { label: 'year', seconds: 31536000 },
      { label: 'month', seconds: 2592000 },
      { label: 'day', seconds: 86400 },
      { label: 'hour', seconds: 3600 },
      { label: 'minute', seconds: 60 },
    ];

    for (const interval of intervals) {
      const count = Math.floor(seconds / interval.seconds);
      if (count >= 1) {
        return `${count} ${interval.label}${count > 1 ? 's' : ''} ago`;
      }
    }

    return 'Just now';
  }
}
