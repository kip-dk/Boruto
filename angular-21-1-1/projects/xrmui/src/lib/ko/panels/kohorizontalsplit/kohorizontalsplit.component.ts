import { Component, Input, OnInit, OnChanges } from '@angular/core';


@Component({
  selector: 'ko-horizontal-split',
  templateUrl: './koHorizontalSplit.component.html',
  styleUrls: ['./koHorizontalSplit.component.css']
})

export class KoHorizontalSplitComponent implements OnInit, OnChanges {

  private thread: any;
  constructor() {
  }

  @Input() height: string = '50%';
  @Input() fixed: string = 'top';


  topStyle: any;
  bottomStyle: any;


  ngOnInit() {
    this.init();
  }

  ngOnChanges(): void {
    if (this.thread != null) clearTimeout(this.thread);
    this.thread = setTimeout(() => {
      this.init();
    }, 500);
  }

  private init(): void {
    if (this.height == null) {
      this.height = '50%';
    }

    if (this.fixed == null) {
      this.fixed = 'top';
    }

    if (this.fixed == 'top') {
      this.topStyle = {
        height: this.height
      };
      this.bottomStyle = {
        top: this.height
      }
    } else {
      this.topStyle = {
        bottom: this.height
      }
      this.bottomStyle = {
        height: this.height
      }
    }
  }
}
