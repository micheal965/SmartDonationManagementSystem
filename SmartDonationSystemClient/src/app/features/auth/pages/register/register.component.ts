import { Component, inject, OnInit, PLATFORM_ID } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
  ɵInternalFormsSharedModule,
} from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { NgClass, NgFor, NgIf, isPlatformBrowser } from '@angular/common';
import { finalize } from 'rxjs';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { passwordStrengthValidator } from '../../../../shared/validators/password.validator';
import { CloudService } from '../../../../core/services/cloud.service';
import { HttpEventType } from '@angular/common/http';
import { GeminiService } from '../../../../core/services/gemini.service';
import { birthDateValidator } from '../../../../shared/validators/BirthDate.validator';
import { CategoryService } from '../../../../core/services/category.service';
import { Category } from '../../../../shared/models/category-model';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, NgClass, NgFor],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private cloudService = inject(CloudService);
  private aiService = inject(GeminiService);
  private categoryService = inject(CategoryService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private platformId = inject(PLATFORM_ID);

  isImageUploading: boolean = false;
  isLoading: boolean = false;
  currentStep = 1;
  imagePreview: string | ArrayBuffer | null = null;
  categories: Category[] = [];

  get totalSteps() {
    return this.role?.value === 'Donor' ? 3 : 2;
  }

  get steps() {
    const baseSteps = [
      { label: 'Account type & ID' },
      { label: 'Personal Info' },
      { label: 'Interests' },
    ];
    return baseSteps;
  }

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
    BirthDate: ['', [Validators.required, birthDateValidator(18)]],
    PhoneNumber: ['', [Validators.required]],
    Address: [''],
    ProfilePictureUrl: [null, Validators.required],
    //Step 3
    InterestingCategoriesIds: [[] as number[]],
  });

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.categoryService.getCategories().subscribe({
        next: (cats) => (this.categories = cats),
      });
    }
  }

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
  //Step 3
  get interestingCategoriesIds() {
    return this.registerForm.get('InterestingCategoriesIds');
  }

  toggleCategory(categoryId: number) {
    const currentIds = this.interestingCategoriesIds?.value || [];
    const index = currentIds.indexOf(categoryId);
    if (index > -1) currentIds.splice(index, 1);
    else currentIds.push(categoryId);

    this.interestingCategoriesIds?.setValue([...currentIds]);
  }

  isCategorySelected(categoryId: number): boolean {
    return (this.interestingCategoriesIds?.value || []).includes(categoryId);
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
      2: [
        'FullName',
        'BirthDate',
        'PhoneNumber',
        'Address',
        'ProfilePictureUrl',
      ],
      3: [],
    };

    stepControls[this.currentStep].forEach((c: string) => {
      this.registerForm.get(c)?.markAllAsTouched();
    });
    return stepControls[this.currentStep].every(
      (c: string) => this.registerForm.get(c)?.valid,
    );
  }
}
