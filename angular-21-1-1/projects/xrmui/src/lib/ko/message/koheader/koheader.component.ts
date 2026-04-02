import { NgClass } from '@angular/common';
import { Component, Input, Output, EventEmitter, SimpleChanges } from '@angular/core';

@Component({
    selector: 'ko-header',
    templateUrl: './koheader.component.html',
    styleUrls: ['./koheader.component.scss'],
    imports:[NgClass]
})

export class KoHeaderComponent {

  @Input('align') align: string = 'left';
    constructor() {
    }
}
