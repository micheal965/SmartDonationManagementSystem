import { EditUserModel } from './../../features/profile/models/edit-user-profile.model';
import { ApiResult } from './../../shared/models/api-result-model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { AuthService } from '../../features/auth/services/auth.service';
import { apiBaseUrl } from '../utils/app.config';
import { UserProfile } from '../../shared/models/user-profile.model';
import { map, Observable, switchMap, tap } from 'rxjs';
import { UserPostsDto } from '../../features/profile/models/user-posts.model';
import { UserReactionsDto } from '../../features/profile/models/user-reactions.model';
import { UserCommentsDto } from '../../features/profile/models/user-comments.model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  profile = signal<UserProfile | null>(null);

  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  getUser(userId: string): Observable<UserProfile> {
    const params = new HttpParams().set('UserId', userId);

    return this.http
      .get<
        ApiResult<UserProfile>
      >(`${apiBaseUrl}/userProfile/get-user-data`, { params })
      .pipe(map((res) => res.data));
  }
  searchUsers(query: string): Observable<any[]> {
    return this.http
      .get<
        ApiResult<any[]>
      >(`${apiBaseUrl}/userProfile/search-user?query=${query}`)
      .pipe(map((res) => res.data));
  }
  deleteUserSoft(): Observable<object> {
    return this.http
      .delete<ApiResult<object>>(`${apiBaseUrl}/userProfile/delete-user-soft`)
      .pipe(switchMap(() => this.authService.logout()));
  }
  updateUser(updateUserDto: EditUserModel): Observable<ApiResult<object>> {
    return this.http.put<ApiResult<object>>(
      `${apiBaseUrl}/userProfile/update-user`,
      updateUserDto,
    );
  }
  getUserPosts(userId: string): Observable<UserPostsDto> {
    const params = new HttpParams().set('UserId', userId);

    return this.http
      .get<
        ApiResult<UserPostsDto>
      >(`${apiBaseUrl}/userProfile/get-user-posts`, { params })
      .pipe(map((res) => res.data));
  }

  getUserComments(userId: string): Observable<UserCommentsDto> {
    const params = new HttpParams().set('UserId', userId);

    return this.http
      .get<
        ApiResult<UserCommentsDto>
      >(`${apiBaseUrl}/userProfile/get-user-comments`, { params })
      .pipe(map((res) => res.data));
  }

  getUserReacts(userId: string): Observable<UserReactionsDto> {
    const params = new HttpParams().set('UserId', userId);

    return this.http
      .get<
        ApiResult<UserReactionsDto>
      >(`${apiBaseUrl}/userProfile/get-user-reactions`, { params })
      .pipe(map((res) => res.data));
  }
  deleteUserProfilePicture(): Observable<object> {
    return this.http.delete<ApiResult<UserReactionsDto>>(
      `${apiBaseUrl}/userProfile/delete-profile-picture`,
    );
  }
  updateProfilePicture(
    profilePicture: File,
  ): Observable<{ pictureUrl: string }> {
    const formData = new FormData();
    formData.append('profilePicture', profilePicture);

    return this.http
      .post<
        ApiResult<{ pictureUrl: string }>
      >(`${apiBaseUrl}/userProfile/set-profile-picture`, formData)
      .pipe(map((res) => res.data));
  }
  loadProfile(): void {
    const currentUserId = this.authService.userData.id;

    this.getUser(currentUserId).subscribe({
      next: (user) => this.profile.set(user),
      error: () => this.profile.set(null),
    });
  }
}
