import { Component, ViewEncapsulation, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'ko-title-panel',
    templateUrl: './kotitlepanel.component.html',
    styleUrls: ['./kotitlepanel.component.scss'],
    imports: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    encapsulation: ViewEncapsulation.None
  
})

export class KoTitlePanelComponent {
    constructor() {
    }
}
