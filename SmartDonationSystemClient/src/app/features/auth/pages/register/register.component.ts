import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators, ɵInternalFormsSharedModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule,NgIf],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);

  registerForm = this.fb.group({
    fullName:['',[Validators.required]],
    birthDate:['',[Validators.required]],
    role:['',[Validators.required]],
    phoneNumber:['',[Validators.required]],
    address:[''],
    identityNumber: ['', [Validators.required, Validators.pattern(/^\d{14}$/)]],
    password: ['', Validators.required],
  });
  onSubmit(){
    console.log(this.registerForm.value);
  }
}
