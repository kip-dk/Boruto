import { NgStyle } from '@angular/common';
import { Component, effect, input, signal, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'ko-vertical-split',
  templateUrl: './koverticalsplit.component.html',
  styleUrls: ['./koverticalsplit.component.scss'],
  imports: [NgStyle],
  encapsulation: ViewEncapsulation.None

})
export class KoVerticalSplitComponent {

  width = input<string>('50%');
  fixed = input<'left' | 'right'>('left');

  constructor() {
    effect(() => {
      const w = this.width();
      const f = this.fixed();
      this.init();
    });
  }

  leftStyle = signal<any>({});
  rightStyle = signal<any>({});


  private init(): void {
    if (this.fixed() == 'left') {
      this.leftStyle.set({
        width: this.width()
      });

      this.rightStyle.set({
        left: this.width()
      });
    } else {
      this.leftStyle.set({
        right: this.width()
      });
      this.rightStyle.set({
        width: this.width()
      });
    }
  }
}
