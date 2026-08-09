import { NgStyle } from '@angular/common';
import { Component, effect, input, Signal, signal, ViewEncapsulation, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'ko-horizontal-split',
  templateUrl: './kohorizontalsplit.component.html',
  styleUrls: ['./kohorizontalsplit.component.scss'],
  imports: [NgStyle],
  changeDetection: ChangeDetectionStrategy.Eager,
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

  topStyle = signal<any>({});
  bottomStyle = signal<any>({});

  private init(): void {
    if (this.fixed() == 'top') {
      this.topStyle.set({
        height: this.height()
      });
      this.bottomStyle.set( {
        top: this.height()
      });
    } else {
      this.topStyle.set({
        bottom: this.height()
      });
      this.bottomStyle.set({
        height: this.height()
      });
    }
  }
}
