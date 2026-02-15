import { HowItWorksComponent } from './../../components/how-it-works/how-it-works.component';
import { Component } from '@angular/core';
import { HeroComponent } from '../../components/hero/hero.component';
import { AboutUsComponent } from '../../components/about-us/about-us.component';
import { OurTeamComponent } from '../../components/our-team/our-team.component';

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [
    HeroComponent,
    AboutUsComponent,
    HowItWorksComponent,
    OurTeamComponent,
  ],
  templateUrl: './landing-page.component.html',
})
export class LandingPageComponent {}
