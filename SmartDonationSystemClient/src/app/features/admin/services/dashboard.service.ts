import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { DashboardModel } from '../models/dashboard.model';
import { AnalysisModel } from '../models/analysis.model';
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

  getAnalysis(fromDate?: string, toDate?: string): Observable<AnalysisModel> {
    let params = new HttpParams();
    if (fromDate) {
      params = params.set('fromDate', fromDate);
    }
    if (toDate) {
      params = params.set('toDate', toDate);
    }

    return this.http
      .get<
        ApiResult<AnalysisModel>
      >(`${apiBaseUrl}/admin/DashboardManagement/analysis`, { params })
      .pipe(map((res) => res.data));
  }
}
