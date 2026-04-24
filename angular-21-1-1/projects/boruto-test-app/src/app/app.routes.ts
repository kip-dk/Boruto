import { Routes } from '@angular/router';
import { FirstpageComponent } from './components/firstpage/firstpage.component';
import { KoComponent } from './components/ko/ko.component';
import { BoComponent } from './components/bo/bo.component';
import { LoComponent } from './components/lo/lo.component';

export const routes: Routes = [
    { path: '', component: BoComponent },
    { path: 'ko', component: KoComponent },
    { path: 'bo', component: BoComponent },
    { path: 'lo', component: LoComponent },
    { path: 'firstpage', component: FirstpageComponent }
];
