import { Component, inject, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { Title } from '@angular/platform-browser';
import { DonationDetails } from '../../../models/donation-details.model';
import { ToastrService } from 'ngx-toastr';
import { PaymentsService } from '../../../services/payments.service';

@Component({
  selector: 'app-payment-details',
  standalone: true,
  imports: [CommonModule, MatIconModule, RouterModule],
  providers: [DatePipe],
  templateUrl: './payment-details.component.html',
  styleUrl: './payment-details.component.scss',
})
export class PaymentDetailsComponent implements OnInit {
  donation!: DonationDetails;
  private route = inject(ActivatedRoute);
  private titleService = inject(Title);
  private paymentService = inject(PaymentsService);
  private toastr = inject(ToastrService);

  ngOnInit(): void {
    this.route.data.subscribe(({ donation }) => {
      this.donation = donation;
      this.titleService.setTitle(`Payment #${this.donation.id} Details`);
    });
  }

  approvePayment() {
    this.paymentService.approveDonation(this.donation.id).subscribe({
      next: (res) => {
        if (res.success) {
          this.toastr.success(res.message);
          this.donation.status = 'Processed';
        }
      }
    });
  }
}
