import { Routes } from '@angular/router';
import { FirstpageComponent } from './components/firstpage/firstpage.component';
import { KoComponent } from './components/ko/ko.component';

export const routes: Routes = [
    { path: '', component: KoComponent },
    { path: 'firstpage', component: FirstpageComponent }
];
