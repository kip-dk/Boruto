import { Component, input, output, signal, inject, ElementRef, AfterViewInit, ViewEncapsulation } from '@angular/core';

import { KoNavigation } from '../../models/navigation.interface';
import { NgClass } from '@angular/common';

@Component({
  selector: 'ko-top-menu',
  templateUrl: './kotopmenu.component.html',
  styleUrls: ['./kotopmenu.component.scss'],
  imports: [NgClass],
  encapsulation: ViewEncapsulation.None
})
export class KoTopMenuComponent implements AfterViewInit {

  private er: ElementRef = inject(ElementRef);

  items = input<(KoNavigation)[]>([]);
  current = input<string | null>(null);
  currentChange = output<string | null>();

  over = signal<KoNavigation | null>(null);

  topcornerwidth = signal<string>('');

  constructor() {
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.topcornerwidth.set(this.er.nativeElement.offsetHeight.toString() + "px");
    }, 1);
  }

  setCurrent(item: KoNavigation): void {
    if (!item.disabled) {
      if (item.click == null) {
        this.currentChange.emit(item.id);
      } else {
        item.click();
      }
    }
  }

  itemwidth(v: KoNavigation) {
    if (v.id == "##divider##") {
      return '0';
    }
    return this.topcornerwidth();
  }


  overItem(v: KoNavigation | null): void {
    if (v != null && !v.disabled) {
      this.over.set(v);
    }
  }

  outItem(): void {
    this.over.set(null);
  }
}
