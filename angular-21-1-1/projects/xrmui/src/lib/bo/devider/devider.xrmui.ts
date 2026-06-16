import { NgClass } from '@angular/common';
import { Component, input } from '@angular/core';

@Component({
    selector: 'xrmui-devider',
    templateUrl: './devider.xrmui.html',
    styleUrl: './devider.xrmui.scss',
    imports: [NgClass]
})
export class XrmuiDevider {
    orientation = input<'horizontal' | 'vertical'>('vertical');
  constructor() {
  }

}
