import { NgClass } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { XrmContextService } from 'xrm';
import { KoNavigationByCssClass, KoDivider, KoNavigation, KiponUiModule } from 'xrmui'

@Component({
  selector: 'app-ko',
  templateUrl: './ko.component.html',
  styleUrl: './ko.component.scss',
  imports: [KiponUiModule]
})
export class KoComponent {
  form: XrmContextService = inject(XrmContextService);

  mynumber = signal<number | null>(456.78);
  align = signal<string>('right');

  infocus = signal<boolean>(false);

  myoverthing: any = {};

  readonly menus: (KoNavigation)[] = [
    new KoNavigationByCssClass("1","red", false, "hallo red", function(){ alert('hello red clicked') }),
    new KoNavigationByCssClass("2","yellow", false, "hallo yellow", function(){ alert('hello yellow clicked') }),
    new KoDivider(),
    new KoNavigationByCssClass("3","green", false, "hallo green", function() { alert('hello green clicked') }),
  ]

  setLeft() {
    this.align.set('left');
  }

  setFocus() {
    this.infocus.set(true);
  }

  numbers = Array.from({ length: 120 }, (_, i) => i);
}
