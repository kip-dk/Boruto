import { NgClass } from '@angular/common';
import { Component, Input, ViewEncapsulation } from '@angular/core';

@Component({
    selector: 'ko-alert',
    templateUrl: './koalert.component.html',
    styleUrls: ['./koalert.component.scss'],
    imports:[NgClass],
    encapsulation: ViewEncapsulation.None
})

export class KoAlertComponent {

  @Input('align') align: string = 'left';
    constructor() {
    }
}
