import { Component, ViewEncapsulation, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'ko-vertical-scroll',
    templateUrl: './koverticalscroll.component.html',
    styleUrls: ['./koverticalscroll.component.scss'],
    imports: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    encapsulation: ViewEncapsulation.None
})
export class KoVerticalScrollComponent {
    constructor() {
    }
}
