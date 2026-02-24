import { NgFor, NgIf } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-create-post',
  standalone: true,
  imports: [NgIf, NgFor, ReactiveFormsModule,MatIconModule],
  templateUrl: './create-post.component.html',
  styleUrl: './create-post.component.scss',
})
export class CreatePostComponent {
  @Output() close = new EventEmitter<void>();

  postForm: FormGroup;
  attachments: File[] = [];
  isSubmitting = false;

  constructor(private fb: FormBuilder) {
    this.postForm = this.fb.group({
      title: [''],
      content: ['', Validators.required],
      attachments: [null],
    });
  }

  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.attachments = Array.from(input.files);
    }
  }

  submit() {
    if (this.postForm.invalid) return;

    this.isSubmitting = true;
    const formData = new FormData();
    formData.append('title', this.postForm.value.title);
    formData.append('content', this.postForm.value.content);
    this.attachments.forEach((file) => formData.append('attachments', file));

    // this.postService.createPost(formData).subscribe({
    //   next: () => {
    //     this.isSubmitting = false;
    //     this.postForm.reset();
    //     this.attachments = [];
    //     this.close.emit(); // close modal
    //   },
    //   error: () => {
    //     this.isSubmitting = false;
    //     alert('Failed to create post. Try again.');
    //   },
    // });
  }

  closeModal() {
    this.close.emit();
  }
}
