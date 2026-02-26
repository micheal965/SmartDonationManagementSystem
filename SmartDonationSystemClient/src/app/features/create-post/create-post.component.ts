import { NgFor, NgIf } from '@angular/common';
import { Component, EventEmitter, inject, Output } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { Category } from '../../shared/models/category-model';
import { CategoryService } from '../../core/services/category.service';
import { minMaxFilesValidator } from '../../shared/validators/files.validator';
@Component({
  selector: 'app-create-post',
  standalone: true,
  imports: [NgIf, NgFor, ReactiveFormsModule, MatIconModule],
  templateUrl: './create-post.component.html',
  styleUrl: './create-post.component.scss',
})
export class CreatePostComponent {
  private fb = inject(FormBuilder);

  @Output() closed = new EventEmitter<void>();
  @Output() submitted = new EventEmitter<any>();

  readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.minLength(3)]],
    categoryId: ['', Validators.required],
    content: ['', [Validators.required, Validators.minLength(10)]],
    attachments: this.fb.nonNullable.control<File[]>(
      [],
      minMaxFilesValidator(0, 5),
    ),
  });
  categories: Category[] = [];
  isLoading: boolean = false;
  constructor(private categoryService: CategoryService) {}

  ngOnInit() {
    // load categories
    this.categoryService.getCategories().subscribe((cats) => {
      this.categories = cats;
    });

    // watch for category selection changes
    this.form
      .get('categoryId')
      ?.valueChanges.subscribe((selectedId: string) => {
        const numericId = Number(selectedId);
        const selectedCategory = this.categories.find(
          (cat) => cat.id === numericId,
        );

        if (selectedCategory?.name === 'Medical') {
          // attachments required for Medical
          this.attachmentsControl.setValidators(minMaxFilesValidator(1, 5));
        } else {
          // attachments optional for other categories or no category selected
          this.attachmentsControl.setValidators(minMaxFilesValidator(0, 5));
        }
        this.attachmentsControl.markAllAsTouched();
        this.attachmentsControl.updateValueAndValidity();
      });
  }

  get f() {
    return this.form.controls;
  }

  get attachmentsControl(): FormControl<File[]> {
    return this.form.get('attachments') as FormControl<File[]>;
  }
  get isAttachmentsOptional(): boolean {
    const selectedId = this.form.get('categoryId')?.value;
    const selectedCategory = this.categories.find(
      (cat) => cat.id === Number(selectedId),
    );
    return selectedCategory?.name !== 'Medical';
  }
  close() {
    this.closed.emit();
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitted.emit(this.form.getRawValue());
  }

  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files) return;

    const files = Array.from(input.files);

    const currentFiles = this.attachmentsControl.value || [];
    const maxFiles = 5;

    // Check if adding new files would exceed the limit
    if (currentFiles.length >= maxFiles) {
      alert(`You can only upload up to ${maxFiles} attachments.`);
      input.value = '';
      return;
    }

    const remainingSlots = maxFiles - currentFiles.length;
    const filesToAdd = files.slice(0, remainingSlots);

    this.attachmentsControl.setValue([...currentFiles, ...filesToAdd]);
    this.attachmentsControl.markAsTouched();
    this.attachmentsControl.updateValueAndValidity();

    input.value = '';
  }
  removeFile(index: number) {
    const updated = [...this.attachmentsControl.value];
    updated.splice(index, 1);
    this.attachmentsControl.setValue(updated);

    this.attachmentsControl.markAsTouched();
    this.attachmentsControl.updateValueAndValidity();
  }
}
