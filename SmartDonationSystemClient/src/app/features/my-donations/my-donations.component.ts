import { DonationService } from '../../core/services/donation.service';
import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { PaginatedResponse } from '../../shared/models/paginated-response.model';
import { MyDonation } from './models/my-donation.model';

@Component({
  selector: 'app-my-donations',
  standalone: true,
  imports: [CommonModule, MatIconModule, RouterModule],
  templateUrl: './my-donations.component.html',
  styleUrl: './my-donations.component.scss',
})
export class MyDonationsComponent implements OnInit {
  private donationService = inject(DonationService);

  paginatedDonations?: PaginatedResponse<MyDonation>;
  pageNumber = 1;
  pageSize = 6;
  statusFilter?: string;

  statuses = [
    { label: 'All', value: undefined },
    { label: 'Pending', value: 'Pending' },
    { label: 'Paid', value: 'Paid' },
    { label: 'Processed', value: 'Processed' },
    { label: 'Failed', value: 'Failed' },
  ];

  ngOnInit(): void {
    this.loadDonations();
  }

  loadDonations(): void {
    this.donationService
      .getMyDonations(this.pageNumber, this.pageSize, this.statusFilter)
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
      : 'text-gray-400 border-transparent hover:text-gray-600';
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

  getStatusIcon(status: string): string {
    switch (status) {
      case 'Paid':
      case 'Succeeded':
        return 'check_circle';
      case 'Processed':
        return 'verified';
      case 'Pending':
        return 'schedule';
      case 'Failed':
        return 'cancel';
      default:
        return 'help';
    }
  }
}
