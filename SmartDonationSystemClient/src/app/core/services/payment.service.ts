import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { apiBaseUrl } from '../utils/app.config';
import { ApiResult } from '../../shared/models/api-result-model';
import { map, Observable } from 'rxjs';
import { DonationDto } from '../../shared/models/donation.model';

@Injectable({
  providedIn: 'root',
})
export class PaymentService {
  private http = inject(HttpClient);

  createCheckoutSession(donation: DonationDto): Observable<string> {
    return this.http
      .post<ApiResult<string>>(`${apiBaseUrl}/payment/create`, donation)
      .pipe(map((res) => res.data));
  }

  redirectToCheckout(donation: DonationDto): void {
    this.createCheckoutSession(donation).subscribe({
      next: (url) => {
        if (url) window.location.href = url;
      },
    });
  }
}
