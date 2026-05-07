import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { ToastrService } from 'ngx-toastr';
import { finalize } from 'rxjs';
import { ReportsService } from '../../services/reports.service';
import {
  ReportRequest,
  ReportType,
  ReportFilter,
} from '../../models/report-request.model';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, FormsModule, MatIconModule],
  templateUrl: './reports.component.html',
  styleUrl: './reports.component.scss',
})
export class ReportsComponent {
  private reportsService = inject(ReportsService);
  private toastr = inject(ToastrService);

  reportTypes = [
    { label: 'Donations Report', value: ReportType.DonationsReport },
    { label: 'Posts Report', value: ReportType.PostsReport },
    { label: 'Users Report', value: ReportType.UsersReport },
  ];

  operators = [
    { label: 'Equals', value: 'eq' },
    { label: 'Not Equals', value: 'neq' },
    { label: 'Contains', value: 'contains' },
    { label: 'Greater Than', value: 'gt' },
    { label: 'Less Than', value: 'lt' },
  ];

  fieldsMap: Record<ReportType, { label: string; value: string }[]> = {
    [ReportType.DonationsReport]: [
      { label: 'Status', value: 'Status' },
      { label: 'Type', value: 'Type' },
      { label: 'Payment Gateway', value: 'PaymentGateway' },
      { label: 'Amount', value: 'Amount' },
      { label: 'Donor Name', value: 'DonorName' },
      { label: 'Created At', value: 'CreatedAt' },
    ],
    [ReportType.PostsReport]: [
      { label: 'Status', value: 'Status' },
      { label: 'Category Name', value: 'CategoryName' },
      { label: 'Created By Role', value: 'CreatedByRole' },
      { label: 'Target Money', value: 'TargetMoney' },
      { label: 'Created At', value: 'CreatedAt' },
    ],
    [ReportType.UsersReport]: [
      { label: 'Full Name', value: 'FullName' },
      { label: 'Phone Number', value: 'PhoneNumber' },
      { label: 'Is Deleted', value: 'IsDeleted' },
      { label: 'Birth Date', value: 'BirthDate' },
    ],
  };

  request: ReportRequest = {
    reportType: ReportType.DonationsReport,
    filters: [],
    dateFrom: null,
    dateTo: null,
    page: 1,
    pageSize: 50,
  };

  isGenerating = false;

  get availableFields() {
    return this.fieldsMap[this.request.reportType] || [];
  }

  addFilter() {
    this.request.filters.push({ field: '', operator: 'eq', value: '' });
  }

  removeFilter(index: number) {
    this.request.filters.splice(index, 1);
  }

  onReportTypeChange() {
    this.request.filters = [];
  }

  generateReport() {
    this.isGenerating = true;

    // Sanitize request: remove empty filters and ensure empty dates are null
    const sanitizedRequest: ReportRequest = {
      ...this.request,
      reportType: Number(this.request.reportType),
      filters: this.request.filters.filter((f) => f.field && f.value),
      dateFrom: this.request.dateFrom || null,
      dateTo: this.request.dateTo || null,
    };

    console.log('Sending sanitized request:', sanitizedRequest);

    this.reportsService
      .generatePdfReport(sanitizedRequest)
      .pipe(finalize(() => (this.isGenerating = false)))
      .subscribe({
        next: (blob) => {
          const fileName = `${this.reportTypes.find((t) => t.value == this.request.reportType)?.label.replace(' ', '_')}_${new Date().getTime()}.pdf`;
          this.reportsService.downloadBlob(blob, fileName);
          this.toastr.success('Report generated successfully');
        },
        error: async (err) => {
          console.error('Report generation error:', err);

          // Try to extract error message from Blob if it's a 400/500
          if (err.error instanceof Blob) {
            try {
              const text = await err.error.text();
              const errorObj = JSON.parse(text);
              this.toastr.error(errorObj.message || 'Error generating report');
            } catch {
              this.toastr.error(
                'Error generating report. Please check your filters.',
              );
            }
          } else {
            this.toastr.error('Error generating report');
          }
        },
      });
  }
}
