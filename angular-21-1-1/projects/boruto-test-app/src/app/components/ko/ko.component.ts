import { Component, inject, signal } from '@angular/core';
import { XrmContextService } from 'xrm';
import { KoAlertComponent, KoButtonGroupComponent, KoDecimalDirective, KoFocusDirective } from 'xrmui'

@Component({
  selector: 'app-ko',
  templateUrl: './ko.component.html',
  styleUrl: './ko.component.scss',
  imports: [KoButtonGroupComponent, KoDecimalDirective,KoFocusDirective,KoAlertComponent]
})
export class KoComponent {
  form: XrmContextService = inject(XrmContextService);

  mynumber = signal<number | null>(456.78);
  align = signal<string>('right');

  infocus = signal<boolean>(false);

  setLeft() {
    this.align.set('left');
  }

  setFocus() {
    this.infocus.set(true);
  }

}
