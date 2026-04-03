import { Component, inject, signal } from '@angular/core';
import { XrmContextService } from 'xrm';
import { KoAlertComponent, KoButtonGroupComponent, KoDecimalDirective, KoFocusDirective, KoHeaderComponent, KoNavigationByCssClass, KoNavigationByImageUrl, KoWriteDirective, KoLeftMenuComponent, KoDivider, KoNavigation, KoTopMenuComponent } from 'xrmui'

@Component({
  selector: 'app-ko',
  templateUrl: './ko.component.html',
  styleUrl: './ko.component.scss',
  imports: [KoButtonGroupComponent, KoDecimalDirective, KoFocusDirective, KoAlertComponent, KoHeaderComponent, KoWriteDirective, KoLeftMenuComponent,KoTopMenuComponent]
})
export class KoComponent {
  form: XrmContextService = inject(XrmContextService);

  mynumber = signal<number | null>(456.78);
  align = signal<string>('right');

  infocus = signal<boolean>(false);

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

}
