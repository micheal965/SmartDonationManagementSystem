import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function minMaxFilesValidator(min: number, max: number): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const files = control.value as File[];

    if (!files || files.length < min)
      return { minFiles: { required: min, actual: files?.length || 0 } };

    if (files.length > max)
      return { maxFiles: { allowed: max, actual: files.length } };

    return null;
  };
}
