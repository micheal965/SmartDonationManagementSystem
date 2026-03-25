import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import {
  provideRouter,
  withInMemoryScrolling,
  withRouterConfig,
  withViewTransitions,
} from '@angular/router';
import { routes } from './app.routes';
import { provideClientHydration } from '@angular/platform-browser';
import {
  provideHttpClient,
  withFetch,
  withInterceptors,
  withInterceptorsFromDi,
} from '@angular/common/http';
import {
  BrowserAnimationsModule,
  provideAnimations,
} from '@angular/platform-browser/animations';
import { provideToastr, ToastrModule } from 'ngx-toastr';
import { NgxSpinnerModule } from 'ngx-spinner';

import { authInterceptor } from './core/interceptors/auth.interceptor';
import { toastrInterceptor } from './core/interceptors/toastr.interceptor';
import { loadingInterceptor } from './core/interceptors/loading.interceptor';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { credentialsInterceptor } from './core/interceptors/credentials.interceptor';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';

export const appConfig: ApplicationConfig = {
  providers: [
    provideCharts(withDefaultRegisterables()),
    provideRouter(
      routes,
      withInMemoryScrolling({
        anchorScrolling: 'enabled',
      }),
      withViewTransitions(),
    ),
    provideClientHydration(),
    provideHttpClient(
      withInterceptorsFromDi(),
      withFetch(),
      withInterceptors([
        credentialsInterceptor,
        authInterceptor,
        toastrInterceptor,
        loadingInterceptor,
      ]),
    ),
    provideAnimations(),
    provideToastr(),
    importProvidersFrom(
      BrowserAnimationsModule,
      NgxSpinnerModule,
      ToastrModule.forRoot({
        positionClass: 'toast-top-right',
        timeOut: 3000,
        closeButton: true,
        progressBar: true,
      }),
    ),
    provideAnimationsAsync(),
  ],
};
