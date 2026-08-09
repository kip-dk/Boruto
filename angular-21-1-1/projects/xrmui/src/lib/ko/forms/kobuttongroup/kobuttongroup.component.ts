import { Component, ViewEncapsulation, ChangeDetectionStrategy } from '@angular/core';


@Component({
    selector: 'ko-button-group',
    templateUrl: './kobuttongroup.component.html',
    styleUrls: ['./kobuttongroup.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    encapsulation: ViewEncapsulation.None
})

export class KoButtonGroupComponent {
    constructor() {
    }
}
