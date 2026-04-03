import { Component, ViewEncapsulation } from '@angular/core';

@Component({
    selector: 'ko-view',
    templateUrl: './koview.component.html',
    styleUrls: ['./koview.component.scss'],
    imports: [],
    encapsulation: ViewEncapsulation.None
})
export class KoViewComponent {
    constructor() {
    }
}
