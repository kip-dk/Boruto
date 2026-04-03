import { NgStyle } from '@angular/common';
import { Component, input, ElementRef, ViewChild, ViewEncapsulation, OnChanges, AfterViewInit, ChangeDetectorRef, inject, effect, signal } from '@angular/core';

@Component({
  selector: 'ko-table-panel',
  templateUrl: './kotablepanel.component.html',
  styleUrls: ['./kotablepanel.component.scss'],
  imports: [NgStyle],
  encapsulation: ViewEncapsulation.None

})

export class KoTablePanelComponent implements AfterViewInit {

  @ViewChild('tableheader') tableheader!: ElementRef;
  @ViewChild('tablebody') tablebody!: ElementRef;
  @ViewChild('tablefooter') tablefooter!: ElementRef;

  height = input<string>('40px');

  private cdRef: ChangeDetectorRef = inject(ChangeDetectorRef);

  constructor() {
    effect(() => {
      const h = this.height();
      this.doSetStyle();

    });
  }


  private topHeight!: string;
  private fotHeight!: string;

  private hasTop: boolean = true;
  private hasFot: boolean = true;

  topStyle = signal<any>({});
  bodStyle = signal<any>({});
  fotStyle = signal<any>({});

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

    var spl = this.height().split(' ');
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
      this.topStyle.set({
        height: this.topHeight,
        display: 'block'
      });

      this.bodStyle.set({
        top: this.topHeight,
        bottom: this.fotHeight,
        "overflow-x": "hidden"
      });

      this.fotStyle.set({
        height: this.fotHeight,
        display:' block'
      })
      return;
    }

    if (this.hasTop && !this.hasFot) {
      this.topStyle.set({
        height: this.topHeight,
        display: 'block'
      });

      this.bodStyle.set({
        top: this.topHeight,
        bottom: 0,
        "overflow-x" : "auto"
      });

      this.fotStyle.set({
        display: 'none'
      });
      return;
    }

    if (!this.hasTop && this.hasFot) {
      this.topStyle.set({
        display: 'none'
      });

      this.bodStyle.set({
        top: 0,
        bottom: this.fotHeight
      });

      this.fotStyle.set({
        height: this.fotHeight,
        display: 'block'
      });
      return;
    }

    if (!this.hasTop && !this.hasFot) {
      this.topStyle.set({
        display: 'none'
      });

      this.bodStyle.set({
        top: 0,
        bottom: 0
      });

      this.fotStyle.set({
        display: 'none'
      })
    }
  }
}
