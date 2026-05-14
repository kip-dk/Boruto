import { NgClass } from '@angular/common';
import { Component, ElementRef, Input, OnInit } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
    selector: 'xrmui-spinner',
    templateUrl: './spinner.xrmui.html',
    styleUrl: './spinner.xrmui.scss',
    imports: [MatProgressSpinnerModule]
})
export class XrmuiSpinner implements OnInit  {

  @Input('diameter') diameter: number = 50;

  constructor(private elm: ElementRef) {
  }

  ngOnInit() {
    if (this.elm != undefined) {
      var native = this.elm.nativeElement as HTMLElement;
      var parent = native.parentElement;
      if (parent != null) {
        var top = Math.round((parent.offsetHeight - this.diameter) / 2);
        var left = Math.round((parent.offsetWidth - this.diameter) / 2);
        if (top > 0 && left > 0) {
          native.style.position = "absolute";
          native.style.top = top + 'px';
          native.style.left = left + 'px';
        }
      }
    }
  }
}
