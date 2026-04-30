import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { DonationDetails } from '../../models/donation-details.model';
import { PaymentsService } from '../../services/payments.service';

export const paymentDetailsResolver: ResolveFn<DonationDetails> = async (
  route,
) => {
  const paymentService = inject(PaymentsService);
  const id = Number(route.paramMap.get('id')!);
  const donation = await firstValueFrom(paymentService.getDonationById(id));
  return donation;
};
