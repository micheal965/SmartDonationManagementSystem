import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PaginatedResponse } from '../../../shared/models/paginated-response.model';
import { PostToReturnDto } from '../models/post.model';
import { map, Observable } from 'rxjs';
import { ApiResult } from '../../../shared/models/api-result-model';
import { apiBaseUrl } from '../../../core/utils/app.config';

@Injectable({
  providedIn: 'root',
})
export class PostsService {
  constructor(private http: HttpClient) {}

  getUsers(
    pageNumber: number,
    pageSize: number,
    postStatus?: string | null,
  ): Observable<PaginatedResponse<PostToReturnDto>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (postStatus) params = params.set('postStatus', postStatus);

    return this.http
      .get<
        ApiResult<PaginatedResponse<PostToReturnDto>>
      >(`${apiBaseUrl}/admin/PostManagement/posts`, { params })
      .pipe(map((res) => res.data));
  }
}
