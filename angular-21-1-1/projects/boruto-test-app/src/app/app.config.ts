import { ApplicationConfig, LOCALE_ID, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { DateAdapter, provideNativeDateAdapter } from '@angular/material/core';

import da from '@angular/common/locales/da';
import { registerLocaleData } from '@angular/common';
import { DKCustomDateAdapter } from 'boruto-xrmui';

registerLocaleData(da);

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideNativeDateAdapter(),
    { provide: LOCALE_ID, useValue: 'da-DK' },
    { provide: DateAdapter, useClass: DKCustomDateAdapter }  ]
};
