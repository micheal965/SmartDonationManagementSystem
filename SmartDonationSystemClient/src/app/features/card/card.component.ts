import { Component, inject, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { Post } from '../feed/models/post.model';
import { TimeAgoPipe } from '../../shared/pipes/time-ago.pipe';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [MatIconModule, TimeAgoPipe, NgClass],
  templateUrl: './card.component.html',
  styleUrl: './card.component.scss',
})
export class CardComponent {
  private sanitizer = inject(DomSanitizer);
  post = input.required<Post>();
  getSafeUrl(url: string): SafeResourceUrl {
    return this.sanitizer.bypassSecurityTrustResourceUrl(url);
  }

  getPriorityLabel(level: number): string {
    const labels = ['Very Low', 'Low', 'Medium', 'High', 'Urgent'];
    return labels[level - 1] ?? 'Unknown';
  }
  getPriorityClass(level: number): string {
    switch (level) {
      case 5:
        return 'bg-[#c53030] text-white'; // Urgent – muted deep red
      case 4:
        return 'bg-[#dd6b20] text-white'; // High – warm amber/orange
      case 3:
        return 'bg-[#d69e2e] text-black'; // Medium – soft gold
      case 2:
        return 'bg-[#38b2ac] text-black'; // Low – calm teal
      case 1:
        return 'bg-[#3182ce] text-white'; // Very Low – professional blue
      default:
        return 'bg-[#e2e8f0] text-black'; // Default – light gray
    }
  }
  goToDetails() {}
}
