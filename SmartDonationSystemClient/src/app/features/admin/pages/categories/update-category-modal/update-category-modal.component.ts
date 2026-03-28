import { Component, EventEmitter, Input, Output } from '@angular/core';
import { UpdateCategoryDto } from '../../../models/update-category.model';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { CreateCategoryModalComponent } from "../create-category-modal/create-category-modal.component";
import { MatIcon } from "@angular/material/icon";

@Component({
  selector: 'app-update-category-modal',
  standalone: true,
  imports: [FormsModule, NgIf, CreateCategoryModalComponent, MatIcon],
  templateUrl: './update-category-modal.component.html',
  styleUrl: './update-category-modal.component.scss',
})
export class UpdateCategoryModalComponent {
  @Input() isOpen = false;

  @Input() formData = {
    id: '',
    name: '',
    description: '',
  };

  @Output() closeModal = new EventEmitter<void>();
  @Output() update = new EventEmitter<any>();

  close() {
    this.closeModal.emit();
  }

  submit() {
    this.update.emit(this.formData);
    this.close();
  }
}
