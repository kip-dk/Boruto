import { NgClass } from '@angular/common';
import { Component, ElementRef, EventEmitter, Input, OnChanges, Output, SimpleChanges, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';
import { ISelectable } from '../api/iselectable.interface';
import { ISearchService } from '../api/isearchservice.interface';

export const NAVIGATIONKEYS = ['ArrowUp','ArrowDown','ArrowLeft','ArrowRight','Escape','Tab','Enter','Backspace','Delete','End','Home','Shift','CapsLock','Insert','PageUp','PageDown','PageDown','PageDown'];
export const NUMBERS = ['0','1','2','3','4','5','6','7','8','9'];
export const DECIMALS = ['0','1','2','3','4','5','6','7','8','9',','];
export const CTRL_KEYS = ['c','C','v','V','x','X'];

@Component({
    selector: 'xrmui-input',
    templateUrl: './input.xrmui.html',
    styleUrl: './input.xrmui.scss',
    imports: [FormsModule, NgClass, MatIcon]
})
export class XrmuiInput implements OnChanges {
    @ViewChild('inputfield') searchElement?: ElementRef;
    @Input('label') label: string = '';
    @Input('short-label') shortLabel: boolean = false;
    @Input('placeholder') placeholder: string = '';
    @Input('value') value: string | number | null = null;
    @Output('valueChange') valueChange: EventEmitter<string | number | null> = new EventEmitter<string | number |null>();
    @Input('disabled') disabled: boolean = false;
    @Input('showlock') showlock: boolean = true;
    @Input('required') required: boolean = false;
    @Input('type') type: 'text' | 'number' | 'password' = 'text';
    @Input('setfocus') setfocus: boolean = false;
    @Output('setfocusChange') setfocusChange: EventEmitter<boolean> = new EventEmitter<boolean>(); 
    @Input('autocomplete') autocomplete: ISearchService | null = null;
    @Output('onselect') onselect: EventEmitter<ISelectable> = new EventEmitter<ISelectable>(); 
    @Input('error') error: boolean = false;
    @Input('message') message: string = '';
    @Input('validate') validate: 'number' | 'decimal' | null = null;
    @Output('focus') onfocusEvent: EventEmitter<void> = new EventEmitter();
    @Output('blur') onblurEvent: EventEmitter<void> = new EventEmitter();
    @Output('click') click: EventEmitter<void> = new EventEmitter();
    @Output('decimalsUsed') decimalsUsed: EventEmitter<number> = new EventEmitter();
    @Output('onEnter') onEnter: EventEmitter<number> = new EventEmitter();

    hasfocus: boolean = false;
    search: string = '';

    items: ISelectable[] | null = null;
    showitems: boolean = false;
    searchthread: any | null = null;
    searchnumber: number = 0;

    current?: ISelectable;
    currentindex: number = -1;

    shadowValue: string = '';

  constructor() {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.setfocus == true) {
      this.setfocus = false;
      this.focusMe(undefined);

      setTimeout(() => {
        this.setfocusChange.emit(false);
      }, 10);
    }

    if (this.type == 'number' && this.validate == null) {
      this.validate = 'decimal';
    }

    if (this.value == null) {
      this.shadowValue = '';
    } else {
      if (this.validate == 'decimal') {
        this.shadowValue = this.value.toString().replace('.',',');
      } else {
        this.shadowValue = this.value.toString();
      }
    }
  }

  formatNumber(n: number): string {
    if (Math.abs(n) < 1e21) {
      return n.toLocaleString('fullwide', { useGrouping: false });
    } else {
      // fallback for very large numbers beyond JS number precision
      return n.toString();
    }
  }

  onValueChanged() {
    if (this.shadowValue == '') {
      this.valueChange.emit(null);
    } else {
      const next = this.shadowValue.replace(',','.');
      switch (this.validate) {
        case 'number': {
          this.value = Number(this.shadowValue);
          break;
        }
        case 'decimal': {
          this.value = Number(next);
          break;
        }
        default:
          this.value = this.shadowValue;

      }
      this.valueChange.emit(this.value)

      if (this.decimalsUsed.observed) {
        var spl = next.split('.');
        if (spl.length <= 1) {
          this.decimalsUsed.next(0);
        } else {
          this.decimalsUsed.next(spl[1].length);
        }
      }
    }
    this.searchbyname();
  }

  onFocus() {
    this.hasfocus = true;
    this.onfocusEvent.emit();
  }

  onBlur() {
    this.hasfocus = false;
    this.showitems = false;
    this.onblurEvent.emit();
  }

  ondown(e: Event) {
    e.stopPropagation();
    e.preventDefault();
    this.next(1);

  }

  onup(e: Event) {
    e.stopPropagation();
    e.preventDefault();
    this.next(-1);
  }

  onenter(e: Event) {
    e.stopPropagation();
    e.preventDefault();

    if (this.onEnter.observed)  {
      this.onEnter.emit();
      return;
    }
    
    if (this.showitems == false) {
      this.searchbyname();
      return;
    }

    if (this.current != null) {
      this.value = this.current.name ?? '';
      this.onselect.emit(this.current);
      this.showitems = false;
    }
  }

  onesc(e: Event) {
    this.showitems = false;
  }

  onClick()  {
    this.click.emit();
  }

  onkeydown(e: Event) {
    const k = e as KeyboardEvent;
    if (this.validate != null) {
      if (NAVIGATIONKEYS.indexOf(k.key) >= 0) {
        return;
      }

      if (k.ctrlKey) {
        if (CTRL_KEYS.indexOf(k.key) >= 0) {
          return;
        }
      }

      if (this.validate == 'number') {
        if (NUMBERS.indexOf(k.key) < 0) {
          e.preventDefault();
          e.stopPropagation(); 
        }
      }

      if (this.validate == 'decimal') {
        if (DECIMALS.indexOf(k.key) < 0) {
          e.preventDefault();
          e.stopPropagation(); 
        }
      }
    }
  }

  private next(e: number) {
    if (this.items != null && this.items.length > 0) {
      this.currentindex = this.currentindex + e;
      if (this.currentindex >= this.items.length) {
        this.currentindex = 0;
      }

      if (this.currentindex == -1) {
        this.currentindex = this.items.length - 1;
      }

      if (this.currentindex == -2) {
        this.currentindex = this.items.length - 1;
      }
      this.current = this.items[this.currentindex]
    }
  }

  focusMe(e?: Event) {
    e?.stopPropagation();

    if (!this.disabled && this.searchElement != null) {
      this.searchElement.nativeElement.focus();
    }

    this.click.emit();
  }

  private searchbyname() {
    if (this.searchthread != null) {
      clearTimeout(this.searchthread);
    }

    this.searchthread = setTimeout(() => {
      this.dosearchbyname();
    }, 600)

  }

  private async dosearchbyname() {
    if (this.autocomplete != null) {
      this.currentindex = -1;
      this.current = undefined;
      this.searchnumber++;
      var next = this.searchnumber;
      var nextlist = await this.autocomplete.search(this.value?.toString() ?? '');
      if (this.searchnumber == next) {
        this.items = nextlist;
      }

      if (this.items != null && this.items.length > 0) {
        this.showitems = true;
      }
    }
  }
}
