import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiResult } from '../../../shared/models/api-result-model';
import { PaginatedResponse } from '../../../shared/models/paginated-response.model';
import { apiBaseUrl } from '../../../core/utils/app.config';
import { Post } from '../models/post.model';

@Injectable({
  providedIn: 'root',
})
export class FeedService {
  constructor(private http: HttpClient) {}

  getPosts(
    pageNumber: number,
    pageSize: number,
    categoryName: string | null,
    sortBy: string,
  ): Observable<{ items: Post[]; hasNext: boolean }> {
    const paramsObj: any = {
      pageNumber,
      pageSize,
      sortBy,
    };

    if (categoryName) paramsObj.categoryName = categoryName;

    const params = new HttpParams({ fromObject: paramsObj });

    return this.http
      .get<
        ApiResult<PaginatedResponse<Post>>
      >(`${apiBaseUrl}/Post/get-posts`, { params })
      .pipe(
        map((response) => {
          const data = response.data;
          return {
            items: data.items,
            hasNext: data.pageNumber < data.totalPages,
          };
        }),
      );
  }
  reactToPost(postId: number) {
    var params = new HttpParams().set('postId', postId);
    return this.http.post<ApiResult<object>>(
      `${apiBaseUrl}/Reaction/react`,
      {},
      {
        params,
      },
    );
  }
}
