import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { UserProfile } from '../../../shared/models/user-profile.model';
import { birthDateValidator } from '../../../shared/validators/BirthDate.validator';
import { NgIf } from '@angular/common';
import { EditUserModel } from '../models/edit-user-profile.model';

@Component({
  selector: 'app-edit-profile',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf],
  templateUrl: './edit-profile.component.html',
  styleUrl: './edit-profile.component.scss',
})
export class EditProfileComponent {
  private fb = inject(FormBuilder);
  @Input() user!: UserProfile;
  @Output() save = new EventEmitter<EditUserModel>();
  @Output() cancel = new EventEmitter<void>();
  @Output() deleteAccount = new EventEmitter<void>();

  editProfileForm!: FormGroup;

  ngOnInit(): void {
    this.editProfileForm = this.fb.group({
      fullName: [this.user.fullName],
      role: [this.user.role],
      birthDate: [this.user.birthDate, [birthDateValidator(18)]],
      address: [this.user.address],
      phoneNumber: [this.user.phoneNumber],
    });
  }
  onDeleteAccount() {
    const confirmed = confirm(
      'Are you sure you want to delete your account? This action cannot be undone.',
    );
    if (confirmed) {
      this.deleteAccount.emit();
    }
  }
  onSave() {
    if (this.editProfileForm.valid) {
      const updatedUser: EditUserModel = this.editProfileForm.value;
      this.save.emit(updatedUser);
    }
  }

  onCancel() {
    this.cancel.emit();
  }
}
