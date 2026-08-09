import { Component, ViewEncapsulation, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'ko-horizontal-scroll',
    templateUrl: './kohorizontalscroll.component.html',
    styleUrls: ['./kohorizontalscroll.component.scss'],
    imports: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    encapsulation: ViewEncapsulation.None
  
})

export class KoHorizontalScrollComponent {
    constructor() {
    }
}
