import { NgClass } from '@angular/common';
import { AfterContentInit, Component, computed, effect, ElementRef, inject, input, signal } from '@angular/core';
import { MatIcon } from '@angular/material/icon';

@Component({
    selector: 'xrmui-icon',
    templateUrl: './icon.xrmui.html',
    styleUrl: './icon.xrmui.scss',
    imports: [NgClass,MatIcon]
})
export class XrmuiIcon implements AfterContentInit {

  private host: ElementRef = inject(ElementRef);
    _icon = input<string>('', { alias: 'icon' });
  disabled = input<boolean>(false);
  type = input<'mat' | 'fa'>('mat');

  icon = signal<string>('');

  classes = computed(() => {
    const i = this.icon();
    const d = this.disabled();
    const t = this.type();

    if (t == 'mat') {
      if (d) return ["disabled"];
      return [];
    }

    if (d) {
      return ['fa',i,'disabled'];
    }
    return ['fa',i];
  });

  constructor() {
    effect(() => {
      const i = this._icon();
      if (i != '') {
        this.icon.set(i);
      }
    });
  }

  ngAfterContentInit(): void {
    const i = this._icon();
    if (i == '') {
      const content = this.host.nativeElement.textContent?.trim() ?? '';
      if (content != '') {
        this.icon.set(content);
      }
    }
  }

}
