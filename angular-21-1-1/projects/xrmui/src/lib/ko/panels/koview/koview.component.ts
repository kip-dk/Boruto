import { Component, ViewEncapsulation, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'ko-view',
    templateUrl: './koview.component.html',
    styleUrls: ['./koview.component.scss'],
    imports: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    encapsulation: ViewEncapsulation.None
})
export class KoViewComponent {
    constructor() {
    }
}
