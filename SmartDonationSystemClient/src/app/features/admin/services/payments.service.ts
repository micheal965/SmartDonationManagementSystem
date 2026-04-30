import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiResult } from '../../../shared/models/api-result-model';
import { apiBaseUrl } from '../../../core/utils/app.config';

@Injectable({
    providedIn: 'root',
})
export class PaymentsService {
    constructor(private http: HttpClient) { }

    getDonations(
        pageNumber: number = 1,
        pageSize: number = 5,
        status?: string,
    ): Observable<ApiResult<any>> {
        let params = `?pageNumber=${pageNumber}&pageSize=${pageSize}`;
        if (status) params += `&status=${status}`;
        return this.http.get<ApiResult<any>>(
            `${apiBaseUrl}/admin/PaymentManagement/get-donations${params}`,
        );
    }

    getDonationById(id: number): Observable<any> {
        return this.http
            .get<ApiResult<any>>(
                `${apiBaseUrl}/admin/PaymentManagement/get-donation/${id}`,
            )
            .pipe(map((res) => res.data));
    }

    approveDonation(id: number): Observable<ApiResult<any>> {
        return this.http.post<ApiResult<any>>(
            `${apiBaseUrl}/admin/PaymentManagement/approve/${id}`,
            {},
        );
    }

    getTotalCollectedAmount(): Observable<number> {
        return this.http
            .get<ApiResult<number>>(
                `${apiBaseUrl}/admin/PaymentManagement/total-collected`,
            )
            .pipe(map((res) => res.data));
    }
}
