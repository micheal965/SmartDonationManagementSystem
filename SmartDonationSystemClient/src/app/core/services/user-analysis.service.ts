import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { apiBaseUrl } from '../utils/app.config';
import { ApiResult } from '../../shared/models/api-result-model';

export interface Trend {
  date: string;
  value: number;
}

export interface CategoryDistribution {
  categoryName: string;
  totalAmount: number;
  donationCount: number;
}

export interface DonorImpact {
  totalDonated: number;
  totalCausesSupported: number;
  categoriesSupported: CategoryDistribution[];
  donationTrend: Trend[];
}

export interface RequesterImpact {
  totalFundsRaised: number;
  totalNeedsFulfilled: number;
  activeNeeds: number;
  fundsRaisedTrend: Trend[];
}

export interface UserAnalysis {
  donorImpact: DonorImpact;
  requesterImpact: RequesterImpact;
}

@Injectable({
  providedIn: 'root'
})
export class UserAnalysisService {
  private http = inject(HttpClient);

  getMyImpact(): Observable<UserAnalysis> {
    return this.http.get<ApiResult<UserAnalysis>>(`${apiBaseUrl}/UserAnalysis/my-impact`)
      .pipe(map(res => res.data));
  }
}
