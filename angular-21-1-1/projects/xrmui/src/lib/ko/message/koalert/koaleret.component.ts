import { NgClass } from '@angular/common';
import { Component, Input, ViewEncapsulation } from '@angular/core';

@Component({
    selector: 'ko-alert',
    templateUrl: './koAlert.component.html',
    styleUrls: ['./koAlert.component.scss'],
    imports:[NgClass],
    encapsulation: ViewEncapsulation.None
})

export class KoAlertComponent {

  @Input('align') align: string = 'left';
    constructor() {
    }
}
