import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'priorityLabel',
  standalone: true,
})
export class PriorityLabelPipe implements PipeTransform {
  transform(level: number): string {
    const labels = ['Very Low', 'Low', 'Medium', 'High', 'Urgent'];
    return labels[level - 1] ?? 'Unknown';
  }
}
