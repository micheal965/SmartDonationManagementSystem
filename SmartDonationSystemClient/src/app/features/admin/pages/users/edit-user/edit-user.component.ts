import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { finalize } from 'rxjs';
import { CloudService } from '../../../../../core/services/cloud.service';
import { UsersService } from '../../../services/users.service';
import { UserToReturnDto } from '../../../models/user-model';
import { birthDateValidator } from '../../../../../shared/validators/BirthDate.validator';
import { HttpEventType } from '@angular/common/http';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-edit-user',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, MatIconModule],
  templateUrl: './edit-user.component.html',
  styleUrl: './edit-user.component.scss',
})
export class EditUserComponent implements OnInit {
  private fb = inject(FormBuilder);
  private cloudService = inject(CloudService);
  private userService = inject(UsersService);
  private toastr = inject(ToastrService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  user!: UserToReturnDto;
  isLoading: boolean = false;
  isImageUploading: boolean = false;
  imagePreview: string | ArrayBuffer | null = null;

  editForm = this.fb.group({
    id: ['', Validators.required],
    fullName: ['', [Validators.required]],
    role: ['', [Validators.required]],
    identityNumber: ['', [Validators.required, Validators.pattern(/^\d{14}$/)]],
    phoneNumber: ['', [Validators.required]],
    birthDate: ['', [Validators.required, birthDateValidator(18)]],
    address: [''],
    pictureUrl: [null as string | null],
  });

  ngOnInit(): void {
    this.route.data.subscribe(({ user }) => {
      this.user = user;
      this.populateForm(user);
    });
  }

  populateForm(user: UserToReturnDto) {
    this.editForm.patchValue({
      id: user.id,
      fullName: user.fullName,
      role: user.role,
      identityNumber: user.identityNumber,
      phoneNumber: user.phoneNumber,
      birthDate: user.birthDate
        ? new Date(user.birthDate).toISOString().split('T')[0]
        : '',
      address: user.address,
      pictureUrl: user.pictureUrl,
    });
    this.imagePreview = user.pictureUrl || null;
  }

  get identityNumber() {
    return this.editForm.get('identityNumber');
  }
  get fullName() {
    return this.editForm.get('fullName');
  }
  get birthDate() {
    return this.editForm.get('birthDate');
  }
  get phoneNumber() {
    return this.editForm.get('phoneNumber');
  }
  get role() {
    return this.editForm.get('role');
  }
  get pictureUrl() {
    return this.editForm.get('pictureUrl');
  }

  onSubmit() {
    if (this.editForm.invalid) return;

    this.isLoading = true;
    this.userService
      .updateUser(this.editForm.value as any)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (res) => {
          this.toastr.success(res.message);
          this.router.navigate(['/admin/users', res.data.id]);
        },
      });
  }

  selectRole(role: string) {
    this.role?.setValue(role);
  }

  onProfilePictureSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    const file = input.files[0];
    const reader = new FileReader();
    reader.onload = () => {
      this.imagePreview = reader.result;
    };
    reader.readAsDataURL(file);

    this.isImageUploading = true;
    this.cloudService
      .getCloudinarySignature('user_profile_pictures')
      .subscribe({
        next: (sigData) => {
          this.cloudService
            .uploadToCloudinary(file, sigData)
            .pipe(finalize(() => (this.isImageUploading = false)))
            .subscribe({
              next: (event: any) => {
                if (event.type === HttpEventType.Response) {
                  this.pictureUrl?.patchValue(event.body.secure_url);
                }
              },
            });
        },
        error: () => {
          this.isImageUploading = false;
        },
      });
  }
}
