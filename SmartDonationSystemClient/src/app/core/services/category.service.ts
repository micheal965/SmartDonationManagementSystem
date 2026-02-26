import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, of, tap } from 'rxjs';
import { ApiResult } from '../../shared/models/api-result-model';
import { Category } from '../../shared/models/category-model';
import { apiBaseUrl } from '../utils/app.config';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  constructor(private http: HttpClient) {}
  private cache: Category[] | null = null;

  getCategories(): Observable<Category[]> {
    if (this.cache) return of(this.cache);
    return this.http
      .get<ApiResult<Category[]>>(`${apiBaseUrl}/Category/get-categories`)
      .pipe(
        map((res) => res.data),
        tap((cats) => (this.cache = cats)),
      );
  }
}
