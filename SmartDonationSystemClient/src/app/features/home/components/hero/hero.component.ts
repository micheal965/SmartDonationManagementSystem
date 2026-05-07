import { isPlatformBrowser, NgFor } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  Inject,
  PLATFORM_ID,
} from '@angular/core';
import { RouterLink } from '@angular/router';
import Swiper from 'swiper';
import { Pagination, Autoplay } from 'swiper/modules';

Swiper.use([Pagination, Autoplay]);

@Component({
  selector: 'app-hero',
  standalone: true,
  imports: [RouterLink, NgFor],
  templateUrl: './hero.component.html',
  styleUrl: './hero.component.scss',
})
export class HeroComponent {
  images = [
    {
      url: './assets/poor-photos/poor1.jpg',
      title: 'Support a Family in Need',
      description:
        'A family in Egypt facing daily financial hardship, in need of basic living support and essential care.',
    },
    {
      url: './assets/poor-photos/poor2.jpg',
      title: 'Help Provide Daily Essentials',
      description:
        'Contribute to helping individuals in underserved communities in Egypt access food, clothing, and basic necessities.',
    },
    {
      url: './assets/poor-photos/poor3.jpg',
      title: 'Bring Hope to a Struggling Family',
      description:
        'Many families across Egypt struggle to afford basic needs; your support can help improve their daily lives.',
    },
  ];
  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private cdr: ChangeDetectorRef,
  ) {}
  ngAfterViewInit() {
    if (!isPlatformBrowser(this.platformId)) return;

    this.cdr.detectChanges();

    requestAnimationFrame(() => {
      new Swiper('.mySwiper', {
        loop: true,
        spaceBetween: 10,

        pagination: {
          el: '.swiper-pagination',
          clickable: true,
        },

        autoplay: {
          delay: 2000,
          disableOnInteraction: false,
        },
      });
    });
  }
}
