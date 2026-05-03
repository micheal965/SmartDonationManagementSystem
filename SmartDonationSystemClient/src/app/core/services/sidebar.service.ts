import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { apiBaseUrl } from '../utils/app.config';
import { ApiResult } from '../../shared/models/api-result-model';

export interface LiveImpact {
  donorName: string;
  donorPicture: string;
  amount: number;
  postTitle: string;
  createdAt: Date;
}

export interface TrendingNeed {
  postId: number;
  title: string;
  categoryName: string;
  priorityLevel: number;
  targetMoney?: number;
  collectedMoney?: number;
}

export interface TotalImpact {
  totalAmountToday: number;
  verifiedCasesCount: number;
}

export interface SidebarData {
  liveImpacts: LiveImpact[];
  trendingNeeds: TrendingNeed[];
  totalImpact: TotalImpact;
}

@Injectable({
  providedIn: 'root',
})
export class SidebarService {
  private http = inject(HttpClient);

  getSidebarData(): Observable<SidebarData> {
    return this.http
      .get<ApiResult<SidebarData>>(`${apiBaseUrl}/sidebar/data`)
      .pipe(map((res) => res.data));
  }
}
