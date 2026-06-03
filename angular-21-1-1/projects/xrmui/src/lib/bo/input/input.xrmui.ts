import { NgClass } from '@angular/common';
import { Component, ElementRef, EventEmitter, input, Input, OnChanges, Output, signal, SimpleChanges, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';
import { ISelectable } from '../api/iselectable.interface';
import { ISearchService } from '../api/isearchservice.interface';
import { TextFieldModule } from '@angular/cdk/text-field';

export const NAVIGATIONKEYS = ['ArrowUp','ArrowDown','ArrowLeft','ArrowRight','Escape','Tab','Enter','Backspace','Delete','End','Home','Shift','CapsLock','Insert','PageUp','PageDown','PageDown','PageDown'];
export const NUMBERS = ['0','1','2','3','4','5','6','7','8','9'];
export const DECIMALS = ['0','1','2','3','4','5','6','7','8','9',','];
export const CTRL_KEYS = ['c','C','v','V','x','X'];

@Component({
    selector: 'xrmui-input',
    templateUrl: './input.xrmui.html',
    styleUrl: './input.xrmui.scss',
    imports: [FormsModule, NgClass, MatIcon,TextFieldModule]
})
export class XrmuiInput implements OnChanges {
    @ViewChild('inputfield') searchElement?: ElementRef;
    @ViewChild('textareafield') textareaElement?: ElementRef;
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
    @Input('selected') selected: ISelectable | undefined = undefined;
    @Output('onselect') onselect: EventEmitter<ISelectable> = new EventEmitter<ISelectable>(); 
    @Input('error') error: boolean = false;
    @Input('message') message: string = '';
    @Input('validate') validate: 'number' | 'decimal' | null = null;
    @Output('focus') onfocusEvent: EventEmitter<void> = new EventEmitter();
    @Output('blur') onblurEvent: EventEmitter<void> = new EventEmitter();
    @Output('click') click: EventEmitter<void> = new EventEmitter();
    @Output('decimalsUsed') decimalsUsed: EventEmitter<number> = new EventEmitter();
    @Output('onEnter') onEnter: EventEmitter<number> = new EventEmitter();

    numberoflines = input(1);
    maxlength = input<number | null>(null);

    hasfocus = signal(false);
    search: string = '';

    private items: ISelectable[] | null = null;
    itemlist = signal<ISelectable[]>([]);
    showitems = signal(false);
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
    this.hasfocus.set(true);
    this.onfocusEvent.emit();
  }

  onBlur() {
    setTimeout(() => {
    this.hasfocus.set(false);
    this.showitems.set(false);

    if (this.selected && this.selected.name != this.shadowValue) {
      this.selected = undefined;
      this.onselect.emit(undefined); 
      this.shadowValue = '';
    }

    this.onblurEvent.emit();
    },100);
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

    if (this.showitems() == false) {
      this.searchbyname();
      return;
    }

    if (this.current != null) {
      this.value = this.current.name ?? '';
      this.selected = this.current;
      this.onselect.emit(this.current);
      this.showitems.set(false);
    }
  }

  onesc(e: Event) {
    this.showitems.set(false);
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


  select(e: Event, v: ISelectable) {
    e.stopImmediatePropagation();
    e.stopPropagation();
    this.value = v.name ?? '';
    this.selected = v;
    this.onselect.emit(v);
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

    if (!this.disabled && this.textareaElement != null) {
      this.textareaElement.nativeElement.focus();
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
        this.itemlist.set(this.items);
        this.showitems.set(true);
      } else {
        this.itemlist.set([]);
        this.showitems.set(false);
      }
    }
  }
}
