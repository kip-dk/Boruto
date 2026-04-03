import { Directive, ElementRef, HostListener, input, Output, EventEmitter, SimpleChanges, inject } from '@angular/core';

@Directive({
  selector: '[ko-over]'
})
export class KoOverDirective {

  private el: ElementRef = inject(ElementRef);

  constructor() {
  }

  current = input.required<any>({alias:'ko-over'});

  @HostListener('mouseover', ['$event']) onMouseover($event: MouseEvent) {
    const me = this.current();
    if (this.current != null) {
      me['over'] = true;
    }
  }

  @HostListener('mouseleave', ['$event']) onMouseleave($event: MouseEvent) {
    const me = this.current();
    if (this.current != null) {
      delete me['over'];
    }
  }
}
