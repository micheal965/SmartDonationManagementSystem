import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiResult } from '../../../shared/models/api-result-model';
import { apiBaseUrl } from '../../../core/utils/app.config';
import { CategoryToReturnDto } from '../models/category.model';
import { UpdateCategoryDto } from '../models/update-category.model';

@Injectable({
  providedIn: 'root',
})
export class CategoriesService {
  constructor(private http: HttpClient) {}

  createCategory(
    categoryName: string,
    description: string,
  ): Observable<ApiResult<CategoryToReturnDto>> {
    let params = new HttpParams()
      .set('categoryName', categoryName)
      .set('description', description);
    return this.http.post<ApiResult<CategoryToReturnDto>>(
      `${apiBaseUrl}/admin/CategoryManagement/create-category`,
      {},
      { params },
    );
  }
  getCategories(): Observable<CategoryToReturnDto[]> {
    return this.http
      .get<
        ApiResult<CategoryToReturnDto[]>
      >(`${apiBaseUrl}/admin/CategoryManagement/get-categories`, {})
      .pipe(map((res) => res.data));
  }
  deleteCategory(id: number): Observable<ApiResult<object>> {
    let params = new HttpParams().set('categoryId', id);

    return this.http.delete<ApiResult<object>>(
      `${apiBaseUrl}/admin/CategoryManagement/delete-category`,
      { params },
    );
  }
  updateCategory(
    updateCategoryDto: UpdateCategoryDto,
  ): Observable<ApiResult<object>> {
    return this.http.patch<ApiResult<object>>(
      `${apiBaseUrl}/admin/CategoryManagement/update-category`,
      updateCategoryDto,
    );
  }
}
