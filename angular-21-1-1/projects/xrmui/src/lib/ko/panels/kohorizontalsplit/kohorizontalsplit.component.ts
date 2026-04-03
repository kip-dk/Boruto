import { NgStyle } from '@angular/common';
import { Component, effect, input, ViewEncapsulation } from '@angular/core';


@Component({
  selector: 'ko-horizontal-split',
  templateUrl: './kohorizontalsplit.component.html',
  styleUrls: ['./kohorizontalsplit.component.scss'],
  imports: [NgStyle],
  encapsulation: ViewEncapsulation.None
})

export class KoHorizontalSplitComponent {

  height = input<string>("50%");
  fixed = input<'top' | 'bottom'>('top');

  constructor() {
    effect(() => {
      const h = this.height();
      const f = this.fixed();
      this.init();
    });
  }

  topStyle: any;
  bottomStyle: any;

  private init(): void {
    if (this.fixed() == 'top') {
      this.topStyle = {
        height: this.height()
      };
      this.bottomStyle = {
        top: this.height()
      }
    } else {
      this.topStyle = {
        bottom: this.height()
      }
      this.bottomStyle = {
        height: this.height()
      }
    }
  }
}
