import { NgClass } from '@angular/common';
import { AfterContentInit, Component, computed, effect, ElementRef, inject, input, signal, ChangeDetectionStrategy } from '@angular/core';
import { MatIcon } from '@angular/material/icon';

@Component({
    selector: 'xrmui-icon',
    templateUrl: './icon.xrmui.html',
    styleUrl: './icon.xrmui.scss',
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [NgClass,MatIcon]
})
export class XrmuiIcon implements AfterContentInit {

  private host: ElementRef = inject(ElementRef);
    _icon = input<string>('', { alias: 'icon' });
  disabled = input<boolean>(false);
  type = input<'mat' | 'fa'>('mat');
  size = input<'12'|'24'|'36'|'48' | undefined>();

  icon = signal<string>('');

  classes = computed(() => {
    const i = this.icon();
    const d = this.disabled();
    const t = this.type();
    const f = this.size();

    const res = [] as string[];

    if (f) {
      res.push('f'+f);
    }

    if (d) {
      res.push('disabled')
    }

    if (t == 'mat') {
      return res;
    }

    res.push('fa');
    res.push(i);
    return res;
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
