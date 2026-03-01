import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function birthDateValidator(minAge: number = 0): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    if (!value) return null;

    const birthDate = new Date(value);
    const today = new Date();

    if (birthDate > today) {
      return { futureDate: true };
    }

    if (minAge > 0) {
      const age = today.getFullYear() - birthDate.getFullYear();
      const monthDiff = today.getMonth() - birthDate.getMonth();
      const dayDiff = today.getDate() - birthDate.getDate();
      if (
        age < minAge ||
        (age === minAge && (monthDiff < 0 || (monthDiff === 0 && dayDiff < 0)))
      ) {
        return { tooYoung: { requiredAge: minAge } };
      }
    }

    return null;
  };
}
