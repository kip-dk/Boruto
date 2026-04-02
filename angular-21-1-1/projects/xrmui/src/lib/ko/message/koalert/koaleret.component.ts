import { NgClass } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
    selector: 'ko-alert',
    templateUrl: './koAlert.component.html',
    styleUrls: ['./koAlert.component.scss'],
    imports:[NgClass]
})

export class KoAlertComponent {

  @Input('align') align: string = 'left';
    constructor() {
    }
}
