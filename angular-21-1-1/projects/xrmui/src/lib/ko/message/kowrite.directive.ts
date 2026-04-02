import { Directive, ElementRef, HostListener, input, AfterViewInit } from '@angular/core';

@Directive({ selector: '[ko-write]' })
export class KoWriteDirective implements AfterViewInit {
  constructor(private hostElement: ElementRef) { }

  direction = input<string>('up');


  @HostListener('window:resize', [])
  onResize() {
    this.ngAfterViewInit();
  }

  ngAfterViewInit(): void {
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
