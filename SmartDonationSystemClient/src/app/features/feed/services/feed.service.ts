import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, of, tap } from 'rxjs';
import { ApiResult } from '../../../shared/models/api-result-model';
import { PaginatedResponse } from '../../../shared/models/paginated-response.model';
import { apiBaseUrl } from '../../../core/utils/app.config';
import { Post } from '../models/post.model';
import { CreateCommentDto } from '../models/create-comment.model';
import { Comment } from '../models/post-comments.model';

@Injectable({
  providedIn: 'root',
})
export class FeedService {
  constructor(private http: HttpClient) {}

  createPost(
    title: string,
    content: string,
    categoryId: string,
    postPicture: File,
    attachments?: File[],
  ): Observable<ApiResult<object>> {
    const formData = new FormData();
    formData.append('title', title);
    formData.append('content', content);
    formData.append('categoryId', categoryId);
    formData.append('postPicture', postPicture);
    if (attachments && attachments.length > 0) {
      attachments.forEach((file, index) => {
        formData.append('attachments', file);
      });
    }
    return this.http.post<ApiResult<object>>(
      `${apiBaseUrl}/Post/create-post`,
      formData,
    );
  }

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

  getPostById(id: number): Observable<Post> {
    return this.http
      .get<ApiResult<Post>>(`${apiBaseUrl}/Post/get-post/${id}`)
      .pipe(map((res) => res.data));
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

  addComment(comment: CreateCommentDto): Observable<Comment> {
    return this.http
      .post<ApiResult<Comment>>(`${apiBaseUrl}/Comment/create-comment`, comment)
      .pipe(map((res) => res.data));
  }
  getPostCommentsById(id: number): Observable<any> {
    return this.http
      .get<ApiResult<any>>(`${apiBaseUrl}/Comment/get-post-comments/${id}`)
      .pipe(map((res) => res.data));
  }
}
