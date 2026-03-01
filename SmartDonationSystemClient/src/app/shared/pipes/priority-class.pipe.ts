import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'priorityClass',
  standalone: true,
})
export class PriorityClassPipe implements PipeTransform {
  transform(level: number): string {
    switch (level) {
      case 5:
        return 'bg-[#c53030] text-white'; // Urgent
      case 4:
        return 'bg-[#dd6b20] text-white'; // High
      case 3:
        return 'bg-[#d69e2e] text-black'; // Medium
      case 2:
        return 'bg-[#38b2ac] text-black'; // Low
      case 1:
        return 'bg-[#3182ce] text-white'; // Very Low
      default:
        return 'bg-[#e2e8f0] text-black'; // Default
    }
  }
}
