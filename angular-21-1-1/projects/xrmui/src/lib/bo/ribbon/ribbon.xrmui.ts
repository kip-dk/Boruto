import { Component, input } from '@angular/core';

@Component({
    selector: 'xrmui-ribbon',
    templateUrl: './ribbon.xrmui.html',
    styleUrl: './ribbon.xrmui.scss'
})
export class XrmuiRibbon {

  fill = input<boolean>(true);

  constructor() {
  }

}
