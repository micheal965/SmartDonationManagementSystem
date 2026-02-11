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
  private router = inject(Router);
  private toastr = inject(ToastrService);
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
      [Validators.required, Validators.minLength(8), passwordStrengthValidator()],
    ],
    //Step 2
    FullName: ['', [Validators.required]],
    BirthDate: ['', [Validators.required]],
    PhoneNumber: ['', [Validators.required]],
    Address: [''],
    ProfilePicture: [null, Validators.required],
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
    return this.registerForm.get('profilePicture');
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
  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    const file = input.files[0];
    (this.registerForm as any).patchValue({ ProfilePicture: file });
    this.profilePicture?.updateValueAndValidity();

    // Preview
    const reader = new FileReader();
    reader.onload = () => {
      this.imagePreview = reader.result;
    };
    reader.readAsDataURL(file);
  }

  onSubmit() {
    if (this.registerForm.invalid) return;

    this.isLoading = true;
    const formData = new FormData();
    Object.entries(this.registerForm.value).forEach(([key, value]) => {
      if (value !== null && value !== undefined) {
        formData.append(key, value as any);
      }
    });
    this.authService
      .register(formData)
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
