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
      title: 'Help Ahmed',
    },
    {
      url: './assets/poor-photos/poor2.jpg',
      title: 'Support Family',
    },
    {
      url: './assets/poor-photos/poor3.jpg',
      title: 'Support Family',
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
          delay: 1500,
          disableOnInteraction: false,
        },
      });
    });
  }
}
