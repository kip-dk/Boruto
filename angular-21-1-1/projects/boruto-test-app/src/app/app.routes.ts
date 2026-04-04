import { Routes } from '@angular/router';
import { FirstpageComponent } from './components/firstpage/firstpage.component';
import { KoComponent } from './components/ko/ko.component';
import { BoComponent } from './components/bo/bo.component';

export const routes: Routes = [
    { path: '', component: BoComponent },
    { path: 'ko', component: KoComponent },
    { path: 'bo', component: BoComponent },
    { path: 'firstpage', component: FirstpageComponent }
];
