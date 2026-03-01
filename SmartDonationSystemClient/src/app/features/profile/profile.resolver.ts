import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { UserService } from '../../core/services/user.service';
import { firstValueFrom } from 'rxjs';

export const profileResolver: ResolveFn<any> = async (route) => {
  const userService = inject(UserService);
  const userId = route.paramMap.get('id')!;
  const user = await firstValueFrom(userService.getUser(userId));
  return user;
};
