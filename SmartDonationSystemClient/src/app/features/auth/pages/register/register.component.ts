import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
  ɵInternalFormsSharedModule,
} from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { finalize } from 'rxjs';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { passwordStrengthValidator } from '../../../../shared/validators/password.validator';
import { CloudService } from '../../services/cloud.service';
import { HttpEventType } from '@angular/common/http';
import { AiService } from '../../../../core/services/ai.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, NgClass, NgFor],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private cloudService = inject(CloudService);
  private aiService = inject(AiService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  isImageUploading: boolean = false;
  isLoading: boolean = false;
  currentStep = 1;
  totalSteps = 2;
  imagePreview: string | ArrayBuffer | null = null;
  steps = [{ label: 'Account type & ID' }, { label: 'Personal Info' }];
  registerForm = this.fb.group({
    //Step 1
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
    //Step 2
    FullName: ['', [Validators.required]],
    BirthDate: ['', [Validators.required]],
    PhoneNumber: ['', [Validators.required]],
    Address: [''],
    ProfilePictureUrl: [null, Validators.required],
  });

  //Step 1
  get role() {
    return this.registerForm.get('Role');
  }
  get identityNumber() {
    return this.registerForm.get('IdentityNumber');
  }
  get password() {
    return this.registerForm.get('Password');
  }
  //Step 2
  get fullName() {
    return this.registerForm.get('FullName');
  }
  get birthDate() {
    return this.registerForm.get('BirthDate');
  }
  get phoneNumber() {
    return this.registerForm.get('PhoneNumber');
  }
  get profilePicture() {
    return this.registerForm.get('ProfilePictureUrl');
  }

  selectRole(role: 'Requester' | 'Donor') {
    this.role?.setValue(role);
  }
  prev() {
    this.currentStep--;
  }
  next() {
    if (this.isStepValid()) {
      this.currentStep++;
    }
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

  onFileSelected(event: any) {
    this.isLoading = true;
    const file = event.target.files[0];
    if (file) {
      this.aiService.extractIdData(file).then((data) => {
        this.registerForm.patchValue({
          FullName: data.full_name,
          IdentityNumber: data.identity_number,
          BirthDate: data.birth_date,
          Address: data.address,
        });
        this.isLoading = false;
      });
    }
  }

  onSubmit() {
    if (this.registerForm.invalid) return;

    this.isLoading = true;
    this.authService
      .register(this.registerForm.value)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (res) => {
          this.toastr.success(res.message);
          this.router.navigate(['/signin']);
        },
      });
  }

  private isStepValid(): boolean {
    const stepControls: any = {
      1: ['IdentityNumber', 'Password', 'Role'],
      2: ['FullName', 'BirthDate', 'PhoneNumber', 'Address'],
    };

    stepControls[this.currentStep].forEach((c: string) => {
      this.registerForm.get(c)?.markAllAsTouched();
    });
    return stepControls[this.currentStep].every(
      (c: string) => this.registerForm.get(c)?.valid,
    );
  }
}
