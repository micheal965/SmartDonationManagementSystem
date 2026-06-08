import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { apiBaseUrl } from '../../../core/utils/app.config';
import { ReportRequest } from '../models/report-request.model';

@Injectable({
  providedIn: 'root',
})
export class ReportsService {
  constructor(private http: HttpClient) {}

  generatePdfReport(request: ReportRequest): Observable<Blob> {
    console.log('Generating PDF report for', request);
    return this.http.post(`${apiBaseUrl}/admin/ReportManagement/pdf`, request, {
      responseType: 'blob',
    });
  }

  openPdf(blob: Blob, fileName: string): void {
    const url = window.URL.createObjectURL(blob);

    // Open PDF in new tab
    const newTab = window.open(url, '_blank');

    // Optional: fallback if popup blocked
    if (!newTab) {
      const link = document.createElement('a');
      link.href = url;
      link.target = '_blank';
      link.click();
    }

    // Optional download button/function
    const downloadLink = document.createElement('a');
    downloadLink.href = url;
    downloadLink.download = fileName;

    // Example: trigger download directly if needed
    // downloadLink.click();

    // Revoke later to avoid memory leak
    setTimeout(() => {
      window.URL.revokeObjectURL(url);
    }, 1000);
  }
}
