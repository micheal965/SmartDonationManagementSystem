import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PaginatedResponse } from '../../../shared/models/paginated-response.model';
import { UserToReturnDto } from '../models/user-model';
import { map, Observable } from 'rxjs';
import { ApiResult } from '../../../shared/models/api-result-model';
import { apiBaseUrl } from '../../../core/utils/app.config';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  constructor(private http: HttpClient) {}

  getUsers(
    pageNumber: number,
    pageSize: number,
    role?: string | null,
  ): Observable<PaginatedResponse<UserToReturnDto>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (role) params = params.set('role', role);

    return this.http
      .get<
        ApiResult<PaginatedResponse<UserToReturnDto>>
      >(`${apiBaseUrl}/admin/UserManagement/get-users`, { params })
      .pipe(map((res) => res.data));
  }

  DeleteUserSoft(userId: string): Observable<ApiResult<object>> {
    let params = new HttpParams().set('userId', userId);

    return this.http.delete<ApiResult<object>>(
      `${apiBaseUrl}/admin/UserManagement/toggle-user-soft-delete`,
      { params },
    );
  }
}
