import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { passwordStrengthValidator } from '../../../../../shared/validators/password.validator';
import { birthDateValidator } from '../../../../../shared/validators/BirthDate.validator';
import { finalize } from 'rxjs';
import { CloudService } from '../../../../../core/services/cloud.service';
import { HttpEventType } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { UsersService } from '../../../services/users.service';

@Component({
  selector: 'app-add-user-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-user-modal.component.html',
  styleUrl: './add-user-modal.component.scss',
})
export class AddUserModalComponent {
  private fb = inject(FormBuilder);
  private cloudService = inject(CloudService);
  private userService = inject(UsersService);
  private toastr = inject(ToastrService);

  @Input({ required: true }) isModalOpen: boolean = true;
  @Output() close = new EventEmitter<void>();

  isLoading: boolean = false;
  isImageUploading: boolean = false;
  imagePreview: string | ArrayBuffer | null = null;

  registerForm = this.fb.group({
    Role: ['', [Validators.required]],
    IdentityNumber: ['', [Validators.required, Validators.pattern(/^\d{14}$/)]],
    Password: [
      '',
      [
        Validators.required,
        Validators.minLength(8),
        passwordStrengthValidator(),
      ],
    ],
    FullName: ['', [Validators.required]],
    BirthDate: ['', [Validators.required, birthDateValidator(18)]],
    PhoneNumber: ['', [Validators.required]],
    Address: [''],
    ProfilePictureUrl: [null, Validators.required],
  });

  get identityNumber() {
    return this.registerForm.get('IdentityNumber');
  }
  get password() {
    return this.registerForm.get('Password');
  }
  get fullName() {
    return this.registerForm.get('FullName');
  }
  get birthDate() {
    return this.registerForm.get('BirthDate');
  }
  get phoneNumber() {
    return this.registerForm.get('PhoneNumber');
  }
  get role() {
    return this.registerForm.get('Role');
  }
  get profilePicture() {
    return this.registerForm.get('ProfilePictureUrl');
  }

  closeModal() {
    this.close.emit();

    this.registerForm.reset();

    this.imagePreview = null;
  }
  onSubmit() {
    if (this.registerForm.invalid) return;

    this.isLoading = true;

    this.userService
      .addNewUser(this.registerForm.value)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (res) => {
          this.toastr.success(res.message);
          this.closeModal();
        },
      });
  }
  selectRole(role: 'Admin' | 'Requester' | 'Donor') {
    this.role?.setValue(role);
  }

  onProfilePictureSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    const file = input.files[0];

    // 1. Preview للصورة (زي ما إنت عامل)
    const reader = new FileReader();
    reader.onload = () => {
      this.imagePreview = reader.result;
    };
    reader.readAsDataURL(file);

    this.isImageUploading = true;

    this.cloudService
      .getCloudinarySignature('user_profile_pictures')
      .pipe(finalize(() => (this.isImageUploading = false)))
      .subscribe({
        next: (sigData) => {
          this.cloudService
            .uploadToCloudinary(file, sigData)
            .pipe(finalize(() => (this.isImageUploading = false)))
            .subscribe({
              next: (event: any) => {
                if (event.type === HttpEventType.Response) {
                  this.profilePicture?.patchValue(event.body.secure_url);
                  this.isImageUploading = false;
                }
              },
            });
        },
      });
  }
}
