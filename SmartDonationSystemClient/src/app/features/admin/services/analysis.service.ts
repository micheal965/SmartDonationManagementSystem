import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AnalysisModel } from '../models/analysis.model';
import { ApiResult } from '../../../shared/models/api-result-model';
import { map, Observable } from 'rxjs';
import { apiBaseUrl } from '../../../core/utils/app.config';

@Injectable({
  providedIn: 'root',
})
export class AnalysisService {
  constructor(private http: HttpClient) {}

  getAnalysis(fromDate?: string, toDate?: string): Observable<AnalysisModel> {
    let params = new HttpParams();
    
    if (fromDate) params = params.set('fromDate', fromDate);
    if (toDate) params = params.set('toDate', toDate);

    return this.http
      .get<
        ApiResult<AnalysisModel>
      >(`${apiBaseUrl}/admin/AnalysisManagement/analysis`, { params })
      .pipe(map((res) => res.data));
  }
}
