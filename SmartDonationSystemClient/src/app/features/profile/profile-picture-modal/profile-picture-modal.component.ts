import { CommonModule } from '@angular/common';
import {
  Component,
  EventEmitter,
  HostListener,
  Input,
  Output,
} from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-profile-picture-modal',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './profile-picture-modal.component.html',
  styleUrl: './profile-picture-modal.component.scss',
})
export class ProfilePictureModalComponent {
  @Input() isOpen = false;
  @Input() imageUrl: string | null = null;
  @Input() isCurrentMainProfile: boolean = false;

  @Output() close = new EventEmitter<void>();
  @Output() update = new EventEmitter<File>();
  @Output() delete = new EventEmitter<void>();

  closeModal() {
    this.close.emit();
  }
  onUpdate(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const file = input.files[0];

    const reader = new FileReader();
    reader.onload = () => {
      this.imageUrl = reader.result as string;
    };
    reader.readAsDataURL(file);
    this.update.emit(file);

    this.closeModal();
  }
  onDelete() {
    this.delete.emit();

    this.closeModal();
  }
  @HostListener('document:keydown.escape')
  onEsc() {
    if (this.isOpen) this.closeModal();
  }
}
