import { Component, input, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'xrmui-ribbon',
    templateUrl: './ribbon.xrmui.html',
    changeDetection: ChangeDetectionStrategy.Eager,
    styleUrl: './ribbon.xrmui.scss'
})
export class XrmuiRibbon {

  fill = input<boolean>(true);

  constructor() {
  }

}
