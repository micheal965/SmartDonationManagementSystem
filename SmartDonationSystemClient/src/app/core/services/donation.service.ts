import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { apiBaseUrl } from '../utils/app.config';
import { ApiResult } from '../../shared/models/api-result-model';
import { map, Observable } from 'rxjs';
import { DonationDto } from '../../shared/models/donation.model';
import { PaginatedResponse } from '../../shared/models/paginated-response.model';
import { MyDonation } from '../../features/my-donations/models/my-donation.model';

@Injectable({
  providedIn: 'root',
})
export class DonationService {
  private http = inject(HttpClient);

  createCheckoutSession(donation: DonationDto): Observable<string> {
    return this.http
      .post<ApiResult<string>>(`${apiBaseUrl}/payment/create`, donation)
      .pipe(map((res) => res.data));
  }

  getMyDonations(
    pageNumber: number = 1,
    pageSize: number = 5,
    status?: string,
  ): Observable<ApiResult<PaginatedResponse<MyDonation>>> {
    let params = `?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    if (status) params += `&status=${status}`;
    return this.http.get<ApiResult<PaginatedResponse<MyDonation>>>(
      `${apiBaseUrl}/payment/my-donations${params}`,
    );
  }

  redirectToCheckout(donation: DonationDto): void {
    this.createCheckoutSession(donation).subscribe({
      next: (url) => {
        if (url) window.location.href = url;
      },
    });
  }
}
