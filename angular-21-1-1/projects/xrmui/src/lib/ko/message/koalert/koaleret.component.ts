import { NgClass } from '@angular/common';
import { Component, Input, ViewEncapsulation, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'ko-alert',
    templateUrl: './koalert.component.html',
    styleUrls: ['./koalert.component.scss'],
    imports:[NgClass],
    changeDetection: ChangeDetectionStrategy.Eager,
    encapsulation: ViewEncapsulation.None
})

export class KoAlertComponent {

  @Input('align') align: string = 'left';
    constructor() {
    }
}
