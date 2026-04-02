import { NgClass } from '@angular/common';
import { Component, input, ViewEncapsulation } from '@angular/core';

@Component({
    selector: 'ko-header',
    templateUrl: './koheader.component.html',
    styleUrls: ['./koheader.component.scss'],
    imports:[NgClass],
    encapsulation: ViewEncapsulation.None
})

export class KoHeaderComponent {

  align = input<string>('left');

}
