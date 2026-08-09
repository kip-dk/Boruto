import { Component, input, output, ElementRef, AfterViewInit, inject, ViewEncapsulation, signal, ChangeDetectionStrategy } from '@angular/core';

import { KoDivider, KoNavigation, KoNavigationByCssClass, KoNavigationByImageUrl } from '../../models/navigation.interface';
import { NgClass } from '@angular/common';

@Component({
  selector: 'ko-left-menu',
  templateUrl: './koleftmenu.component.html',
  styleUrls: ['./koleftmenu.component.scss'],
  imports: [NgClass],
  changeDetection: ChangeDetectionStrategy.Eager,
  encapsulation: ViewEncapsulation.None
})
export class KoLeftMenuComponent implements AfterViewInit {
  private er: ElementRef = inject(ElementRef);

  items = input<(KoNavigation)[]>([]);
  current = input<string | null>(null);
  currentChange = output<string | null>();

  private itemheight$ = signal<string>('');
  over = signal<KoNavigation | null>(null);

  constructor() {
  }


  itemheight(v: KoNavigation) {
    if (v.id == "##divider##") {
      return '2px';
    }
    return this.itemheight$();
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.itemheight$.set( (this.er.nativeElement.offsetWidth - 40).toString() + "px");
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

  overItem(v: KoNavigation | null): void {
    if (v != null && !v.disabled) {
      this.over.set(v);
    }
  }

  outItem(): void {
    this.over.set(null);
  }
}
