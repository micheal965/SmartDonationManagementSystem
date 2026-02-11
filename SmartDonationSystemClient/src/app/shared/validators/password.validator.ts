import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function passwordStrengthValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value as string;

    if (!value) return null; // don't validate empty value here

    const hasUpperCase = /[A-Z]/.test(value);
    const hasLowerCase = /[a-z]/.test(value);

    const errors: ValidationErrors = {};

    if (!hasUpperCase)
      errors['noUpperCase'] =
        'Password must have at least one uppercase letter';

    if (!hasLowerCase)
      errors['noLowerCase'] =
        'Password must have at least one lowercase letter';

    return Object.keys(errors).length ? errors : null;
  };
}
