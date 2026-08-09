import { NgClass } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, ChangeDetectionStrategy } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { IMenu } from '../api/imenu.interface';
import { XrmuiSpinner } from '../spinner/spinner.xrmui';

@Component({
    selector: 'xrmui-button',
    templateUrl: './button.xrmui.html',
    styleUrl: './button.xrmui.scss',
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [MatMenuModule, NgClass, MatIcon, XrmuiSpinner]
})
export class XrmuiButton implements OnChanges {
    search: string = '';
    @Input('label') label: string = '';
    @Input('icon') icon: string = '';
    @Input('disabled') disabled: boolean = false;
    @Input('working') working: boolean = false;
    @Output('click') clicked: EventEmitter<void | IMenu> = new EventEmitter<void | IMenu>();

    @Input('menu') menu: IMenu[] = [];
    @Input('current') current?: IMenu;
    @Input('selected') selected: boolean = false;

    isCurrent: boolean = false;

  constructor() {
  }

  ignore(e: Event) {
    e.preventDefault();
    e.stopPropagation();
  }

  onClick(e: Event, m?: IMenu) {
    if (!this.disabled) {
      if (m != undefined) {
        this.clicked.emit(m);
      } else {
        e.stopPropagation();
        this.clicked.emit();
      }
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.isCurrent = this.menu.length > 0 && this.current != undefined && this.menu.find(m => m == this.current) != null;
  }
}
