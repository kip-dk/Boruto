import { Directive, ElementRef, HostListener, input, AfterViewInit, inject, effect } from '@angular/core';

@Directive({ selector: '[ko-write]' })
export class KoWriteDirective {

  direction = input<string>('up', { alias: 'ko-write' });
  private hostElement: ElementRef = inject(ElementRef);

  constructor() { 
    effect(() => {
      const dir = this.direction();
      this.render();
    });
  }




  private render(): void {
    var host = this.hostElement.nativeElement as HTMLElement;
    if (host.parentElement) {
      var parent = host.parentElement as HTMLElement;

      parent.style.overflow = "hidden"; 

      var h = parent.offsetHeight;
      var w = parent.offsetWidth;


      if (h && w) {
        host.style.width = h.toString() + "px";
        host.style.height = w.toString() + "px";
        host.style.lineHeight = w.toString() + "px";
        host.style.textAlign = "center";
        host.style.position = "relative";
        host.style.transformOrigin = "0% 0%";
        host.style.display = "inline-block";

        if (this.direction() == "up") {
          host.style.transform = "rotate(-90deg)";
          host.style.top = h.toString() + "px";
        } else {
          host.style.transform = "rotate(90deg)";
          host.style.left = w.toString() + "px";
        }
      }
    }
  }
}
