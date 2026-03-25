import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'shortNumber',
  standalone: true,
})
export class ShortNumberPipe implements PipeTransform {
  transform(value: number | string | undefined): string {
    if (value === null || value === undefined || value === '') return '0';

    const num = Number(value);
    if (isNaN(num)) return '0';

    if (num >= 1000000) {
      return (num / 1000000).toFixed(1).replace(/\.0$/, '') + 'M';
    }
    if (num >= 1000) {
      return (num / 1000).toFixed(1).replace(/\.0$/, '') + 'k';
    }

    return num.toString();
  }
}
