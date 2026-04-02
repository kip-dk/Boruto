import { Component, Input, ElementRef, ViewChild, ViewEncapsulation, OnChanges, AfterViewInit, ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'ko-table-panel',
  templateUrl: './koTablePanel.component.html',
  styleUrls: ['./koTablePanel.component.css']
})

export class KoTablePanelComponent implements OnChanges, AfterViewInit {

  constructor(private cdRef: ChangeDetectorRef) {
  }

  @ViewChild('tableheader') tableheader!: ElementRef;
  @ViewChild('tablebody') tablebody!: ElementRef;
  @ViewChild('tablefooter') tablefooter!: ElementRef;

  @Input() height: string = '40px';

  topHeight!: string;
  fotHeight!: string;

  hasTop: boolean = true;
  hasFot: boolean = true;

  topStyle: any = {};
  bodStyle: any = {};
  fotStyle: any = {};

  ngOnChanges() {
    this.doSetStyle();
  }

  ngAfterViewInit(): void {
    this.hasTop = this.tableheader.nativeElement != null && this.tableheader.nativeElement.children.length > 0;
    this.hasFot = this.tablefooter.nativeElement != null && this.tablefooter.nativeElement.children.length > 0;
    this.cdRef.detectChanges();

    this.setStyle();
  }

  updateScroll() {
    const header = this.tableheader.nativeElement as HTMLElement;
    const body = this.tablebody.nativeElement as HTMLElement;
    const footer = this.tablefooter.nativeElement as HTMLElement;

    var toScroll = this.hasFot ? footer.scrollLeft : body.scrollLeft;

    if (this.hasTop) {
      header.scrollLeft = toScroll;
    }

    if (this.hasFot) {
      footer.scrollLeft = toScroll;
    }

    body.scrollLeft = toScroll;
  }

  private setStyle(): void {
    if (this.height == null) {
      this.height = '40px';
    }

    var spl = this.height.split(' ');
    if (spl.length == 2) {
      this.topHeight = spl[0];
      this.fotHeight = spl[1];
    } else {
      this.topHeight = spl[0];
      this.fotHeight = spl[0];
    }
    setTimeout(() => this.doSetStyle(), 10);
  }

  private doSetStyle(): void {
    if (this.hasTop && this.hasFot) {
      this.topStyle = {
        height: this.topHeight,
        display: 'block'
      };

      this.bodStyle = {
        top: this.topHeight,
        bottom: this.fotHeight,
        "overflow-x": "hidden"
      };

      this.fotStyle = {
        height: this.fotHeight,
        display:' block'
      }
      return;
    }

    if (this.hasTop && !this.hasFot) {
      this.topStyle = {
        height: this.topHeight,
        display: 'block'
      };

      this.bodStyle = {
        top: this.topHeight,
        bottom: 0,
        "overflow-x" : "auto"
      };

      this.fotStyle = {
        display: 'none'
      }
      return;
    }

    if (!this.hasTop && this.hasFot) {
      this.topStyle = {
        display: 'none'
      };

      this.bodStyle = {
        top: 0,
        bottom: this.fotHeight
      };

      this.fotStyle = {
        height: this.fotHeight,
        display: 'block'
      }
      return;
    }

    if (!this.hasTop && !this.hasFot) {
      this.topStyle = {
        display: 'none'
      };

      this.bodStyle = {
        top: 0,
        bottom: 0
      };

      this.fotStyle = {
        display: 'none'
      }
    }
  }
}
