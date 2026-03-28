import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { DashboardModel } from '../models/dashboard.model';
import { ApiResult } from '../../../shared/models/api-result-model';
import { map, Observable } from 'rxjs';
import { apiBaseUrl } from '../../../core/utils/app.config';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  constructor(private http: HttpClient) {}

  getDashboard(): Observable<DashboardModel> {
    return this.http
      .get<
        ApiResult<DashboardModel>
      >(`${apiBaseUrl}/admin/DashboardManagement/dashboard`)
      .pipe(map((res) => res.data));
  }
}
