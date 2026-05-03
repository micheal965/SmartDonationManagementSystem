import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { PaginatedResponse } from '../../../../shared/models/paginated-response.model';
import { DonationToReturn } from '../../models/donation.model';
import { RouterModule } from '@angular/router';
import { PaymentsService } from '../../services/payments.service';

@Component({
  selector: 'app-payments',
  standalone: true,
  imports: [CommonModule, MatIconModule, RouterModule],
  templateUrl: './payments.component.html',
})
export class PaymentsComponent implements OnInit {
  private paymentService = inject(PaymentsService);

  paginatedDonations?: PaginatedResponse<DonationToReturn>;
  pageNumber = 1;
  pageSize = 5;
  statusFilter?: string;
  totalCollected: number = 0;

  statuses = [
    { label: 'All Payments', value: undefined },
    { label: 'Pending', value: 'Pending' },
    { label: 'Paid', value: 'Paid' },
    { label: 'Processed', value: 'Processed' },
    { label: 'Failed', value: 'Failed' },
  ];
  get visiblePages(): number[] {
    const maxVisible = 5;

    const start = Math.max(0, this.pageNumber - Math.floor(maxVisible / 2));
    const end = start + maxVisible;

    return this.pages.slice(start, end);
  }

  ngOnInit(): void {
    this.loadDonations();
    this.loadTotalCollected();
  }

  loadTotalCollected(): void {
    this.paymentService.getTotalCollectedAmount().subscribe((res) => {
      this.totalCollected = res;
    });
  }

  loadDonations(): void {
    this.paymentService
      .getDonations(this.pageNumber, this.pageSize, this.statusFilter)
      .subscribe((res) => {
        this.paginatedDonations = res.data;
      });
  }

  filterByStatus(status?: string): void {
    this.statusFilter = status;
    this.pageNumber = 1;
    this.loadDonations();
  }

  getFilterClass(status?: string): string {
    return this.statusFilter === status
      ? 'text-indigo-600 border-indigo-600'
      : 'text-slate-400 border-transparent hover:text-slate-600';
  }

  nextPage(): void {
    if (this.pageNumber < this.totalPages) {
      this.pageNumber++;
      this.loadDonations();
    }
  }

  prevPage(): void {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.loadDonations();
    }
  }

  changePageNumber(page: number): void {
    this.pageNumber = page;
    this.loadDonations();
  }

  get totalPages(): number {
    return this.paginatedDonations?.totalPages || 0;
  }

  get pages(): number[] {
    const pages = [];
    for (let i = 1; i <= this.totalPages; i++) {
      pages.push(i);
    }
    return pages;
  }

  getTo(): number {
    return Math.min(
      this.pageNumber * this.pageSize,
      this.paginatedDonations?.totalCount || 0,
    );
  }
}
