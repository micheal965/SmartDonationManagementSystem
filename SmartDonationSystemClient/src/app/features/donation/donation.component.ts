import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { PaymentService } from '../../core/services/payment.service';
import { Post } from '../feed/models/post.model';
import { DonationDto } from '../../shared/models/donation.model';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-donation',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatIconModule, RouterLink],
  templateUrl: './donation.component.html',
  styleUrl: './donation.component.scss',
})
export class DonationComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private paymentService = inject(PaymentService);
  private titleService = inject(Title);

  post!: Post;
  donationForm: FormGroup;
  isSubmitting = false;

  gateways = [
    { id: 'Stripe', name: 'Stripe', icon: 'payments', description: 'Credit/Debit Card' },
    { id: 'Paymob', name: 'Paymob', icon: 'account_balance_wallet', description: 'Card / Vodafone Cash / Meeza' },
  ];

  constructor() {
    this.donationForm = this.fb.group({
      amount: [50, [Validators.required, Validators.min(50)]],
      gateway: ['Stripe', Validators.required],
    });
  }

  ngOnInit(): void {
    this.post = this.route.snapshot.data['post'];
    if (this.post) {
      this.titleService.setTitle(`Donate to ${this.post.title}`);
    } else {
      this.router.navigate(['/feed']);
    }
  }

  selectGateway(gatewayId: string): void {
    this.donationForm.patchValue({ gateway: gatewayId });
  }

  onSubmit(): void {
    if (this.donationForm.invalid || this.isSubmitting) return;

    this.isSubmitting = true;
    const donationDto: DonationDto = {
      PostId: this.post.id,
      Amount: this.donationForm.value.amount,
      Gateway: this.donationForm.value.gateway,
    };

    this.paymentService.redirectToCheckout(donationDto);
  }
}
