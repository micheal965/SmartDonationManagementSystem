import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { apiBaseUrl } from '../../../core/utils/app.config';
import { ReportRequest } from '../models/report-request.model';

@Injectable({
  providedIn: 'root'
})
export class ReportsService {
  constructor(private http: HttpClient) { }

  generatePdfReport(request: ReportRequest): Observable<Blob> {
    console.log('Generating PDF report for', request);
    return this.http.post(`${apiBaseUrl}/admin/ReportManagement/pdf`, request, {
      responseType: 'blob'
    });
  }

  downloadBlob(blob: Blob, fileName: string): void {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    link.click();
    window.URL.revokeObjectURL(url);
  }
}
