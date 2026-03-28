import { NgIf } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-create-category-modal',
  standalone: true,
  imports: [FormsModule, NgIf],
  templateUrl: './create-category-modal.component.html',
  styleUrl: './create-category-modal.component.scss',
})
export class CreateCategoryModalComponent {
  @Input() isOpen: boolean = false;

  @Output() close = new EventEmitter<void>();
  @Output() submitForm = new EventEmitter<{
    categoryName: string;
    description: string;
  }>();

  category = {
    categoryName: '',
    description: '',
  };

  onClose() {
    this.close.emit();
  }

  onSubmit() {
    this.submitForm.emit(this.category);
    this.resetForm();
    this.onClose();
  }
  resetForm() {
    this.category = {
      categoryName: '',
      description: '',
    };
  }
}
