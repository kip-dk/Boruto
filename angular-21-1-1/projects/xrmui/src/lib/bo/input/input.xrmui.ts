import { NgClass } from '@angular/common';
import { Component, computed, effect, ElementRef, EventEmitter, input, Input, isSignal, model, OnChanges, output, Output, QueryList, Signal, signal, SimpleChanges, ViewChild, ViewChildren } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';
import { ISelectable } from '../api/iselectable.interface';
import { ISearchService } from '../api/isearchservice.interface';
import { TextFieldModule } from '@angular/cdk/text-field';
import { MatDatepickerInputEvent, MatDatepickerModule } from '@angular/material/datepicker';
import { OptionSetValue } from '../api/optionsetvalue.interface';

export const NAVIGATIONKEYS = ['ArrowUp','ArrowDown','ArrowLeft','ArrowRight','Escape','Tab','Enter','Backspace','Delete','End','Home','Shift','CapsLock','Insert','PageUp','PageDown','PageDown','PageDown'];
export const NUMBERS = ['0','1','2','3','4','5','6','7','8','9'];
export const DECIMALS = ['0','1','2','3','4','5','6','7','8','9',','];
export const CTRL_KEYS = ['c','C','v','V','x','X'];

@Component({
    selector: 'xrmui-input',
    templateUrl: './input.xrmui.html',
    styleUrl: './input.xrmui.scss',
    imports: [FormsModule, NgClass, MatIcon,TextFieldModule,MatDatepickerModule]
})
export class XrmuiInput {
    @ViewChild('inputfield') searchElement?: ElementRef;
    @ViewChild('textareafield') textareaElement?: ElementRef;
    @ViewChild('datefield') datefieldElement?: ElementRef;
    @ViewChildren('option') options!: QueryList<ElementRef<HTMLDivElement>>;

    label = input<string>('');
    shortLabel = input<boolean | 'above'>(false, {alias: 'short-label'});
    placeholder = input<string>('');

    value = model<string | number | Date | null | undefined>(null);
    disabled = input<boolean>(false);
    showlock = input<boolean>(true);
    required = input<boolean>(false);
    type = input<'text' | 'number' | 'password'>('text');
    setfocus = model<boolean>(false);
    autocomplete = input<ISearchService | null>( null);
    selectable = input<ISelectable[] | null>(null);
    selected = model<ISelectable | undefined>(undefined);
    optionsetvalue = input<OptionSetValue | null>(null);
    onselect = output<ISelectable | undefined>();
    error = input<boolean>(false);
    validate = model<'number' | 'decimal' | 'date' | null>(null);
    onfocusEvent = output<void>({ alias:'focus' });
    onblurEvent = output<void>({alias: 'blur'});
    click = output<void>();
    resizeable = input<boolean>(true);
    info = input<string | null>(null);
    notdark = input<boolean>(false);
    autoopenonblank = input<boolean>(false);
    decimalsUsed = output<number>();
    onEnter = output<void>();

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

    shadowValue = signal<string>('');
    shadowDate = signal<Date | null>(null);

    isselect = computed(() => {
      const s = this.selectable();
      if (s && s.length > 0) return true;
      return false;
    });

  constructor() {
    effect(() => {
      const sf = this.setfocus();
      if (sf == true) {
        this.focusMe(undefined);
        setTimeout(() => {
          this.setfocus.set(false);
        },10);
      }
    });

    effect(() => {
      const type = this.type();
      const validate = this.validate();
      if (type == 'number' && validate == null) {
        this.validate.set('decimal');
      }
    });

    effect(() => {
      const value = this.value();
      if (value == null) {
        this.shadowValue.set('');
        this.shadowDate.set(null);
        return;
      }

      if (this.validate() == 'decimal') {
        this.shadowValue.set(value.toString().replace('.',','));
        return;
      }

      if (this.validate() == 'date') {
        this.shadowDate.set(value as Date);
        return;
      }

      this.shadowValue.set(value.toString());
    });

    effect(()=> {
      const sel = this.selected();
      if (sel) {
        this.shadowValue.set(sel.name ?? '');
      }
    });

    effect(() => {
      const osv = this.optionsetvalue();
      const sels = this.selectable();
      if (osv && osv.value != undefined && sels && sels.length > 0) {
        const sel = sels.find(r => r.id == osv.value?.toString());
        if (sel) {
          this.selected.set(sel);
        }
      };
    })
  }


  pickDate(d: MatDatepickerInputEvent<any,any>) {
    this.shadowDate.set(d.value);
    this.value.set(d.value);
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
    const sv = this.shadowValue();
    if (sv == '') {
      this.value.set(null);
    } else {
      const next = sv.replace(',','.');
      switch (this.validate()) {
        case 'number': {
          this.value.set(Number(next));
          break;
        }
        case 'decimal': {
          this.value.set(Number(next));
          break;
        }
        default:
          this.value.set(sv);

      }

      if (this.type() == 'number' && this.validate() == 'decimal') {
        var spl = next.split('.');
        if (spl.length <= 1) {
          this.decimalsUsed.emit(0);
        } else {
          this.decimalsUsed.emit(spl[1].length);
        }
      }
    }
    this.searchbyname();
  }

  onFocus() {
    this.hasfocus.set(true);
    this.onfocusEvent.emit();

    if (this.autoopenonblank() && this.autocomplete()) {
      const cv = this.shadowValue() ?? '';
      if (cv == '') {
        this.searchbyname();
      }
    }
  }

  onBlur() {
    setTimeout(() => {
    this.hasfocus.set(false);
    this.showitems.set(false);

    const sel = this.selected();
    if (sel && sel.name != this.shadowValue()) {
      this.selected.set(undefined);
      this.onselect.emit(undefined); 
      this.shadowValue.set('');
    }

    this.onblurEvent.emit();
    },200);
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

    this.onEnter.emit();

    if (this.showitems() == false) {
      this.searchbyname();
      return;
    }

    if (this.current != null) {
      this.value.set(this.current.name ?? '');
      this.selected.set(this.current);
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
    if (this.validate() != null) {
      if (NAVIGATIONKEYS.indexOf(k.key) >= 0) {
        return;
      }

      if (k.ctrlKey) {
        if (CTRL_KEYS.indexOf(k.key) >= 0) {
          return;
        }
      }

      if (this.validate() == 'number') {
        if (NUMBERS.indexOf(k.key) < 0) {
          e.preventDefault();
          e.stopPropagation(); 
        }
      }

      if (this.validate() == 'decimal') {
        if (DECIMALS.indexOf(k.key) < 0) {
          e.preventDefault();
          e.stopPropagation(); 
        }
      }
    }
  }


  setchoice(v: Event) {
    const sel = this.selectable();
    if (sel && sel.length > 0) {
      const value = (v.target as HTMLSelectElement).value;
      const next = sel.find(r => r.id == value);
      if (next) {
          const osv = this.optionsetvalue();
          if (osv) {
            osv.name = next.name ?? 'Unknown';
            osv.value = Number(next.id);
          }
          this.selected.set(next);
      }
    }
  }

  select(e: Event, v: ISelectable) {
    e.stopImmediatePropagation();
    e.stopPropagation();
    this.value.set(v.name ?? '');
    this.selected.set(v);
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
    this.ensureCurrentVisible();
  }

  private ensureCurrentVisible(): void {

    if (this.current != undefined) {
    const index = this.itemlist().indexOf(this.current);
    if (index < 0) {
        return;
    }

    this.options.get(index)?.nativeElement.scrollIntoView({
        block: 'nearest',
        inline: 'nearest'
    });
  }
}

  focusMe(e?: Event) {
    e?.stopPropagation();

    if (!this.disabled() && this.searchElement != null) {
      this.searchElement.nativeElement.focus();
    }

    if (!this.disabled() && this.textareaElement != null) {
      this.textareaElement.nativeElement.focus();
    }

    if (!this.disabled() && this.datefieldElement != null) {
      this.datefieldElement.nativeElement.focus();
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
    const ap = this.autocomplete();
    if (ap != null) {
      this.currentindex = -1;
      this.current = undefined;
      this.searchnumber++;
      var next = this.searchnumber;
      var nextlist = await ap.search(this.value()?.toString() ?? '');
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
