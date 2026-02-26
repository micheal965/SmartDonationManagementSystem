import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function passwordStrengthValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value as string;

    if (!value) return null; // don't validate empty value here

    const hasUpperCase = /[A-Z]/.test(value);
    const hasLowerCase = /[a-z]/.test(value);
    const hasNumber = /[0-9]/.test(value); // ✅ new check

    const errors: ValidationErrors = {};

    if (!hasUpperCase)
      errors['noUpperCase'] =
        'Password must have at least one uppercase letter';

    if (!hasLowerCase)
      errors['noLowerCase'] =
        'Password must have at least one lowercase letter';

    if (!hasNumber)
      errors['noNumber'] = 'Password must have at least one number';

    return Object.keys(errors).length ? errors : null;
  };
}
