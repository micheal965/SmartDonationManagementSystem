import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { UsersService } from '../../services/users.service';

export const userDetailsResolver: ResolveFn<any> = async (route) => {
  const userService = inject(UsersService);
  const userId = route.paramMap.get('id')!;
  const user = await firstValueFrom(userService.getUserDetails(userId));
  return user;
};
