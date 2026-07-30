import { NgClass } from '@angular/common';
import { Component, computed, DestroyRef, effect, ElementRef, EventEmitter, inject, input, Input, isSignal, model, OnChanges, output, Output, QueryList, Signal, signal, SimpleChanges, ViewChild, ViewChildren } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';
import { ISelectable } from '../api/iselectable.interface';
import { ISearchService } from '../api/isearchservice.interface';
import { TextFieldModule } from '@angular/cdk/text-field';
import { MatDatepickerInputEvent, MatDatepickerModule } from '@angular/material/datepicker';
import { OptionSetValue } from '../api/optionsetvalue.interface';
import { ChangeScopeDirective } from '../changedscope/changedscope.directive';
import { EntityReference } from '../api/entityreference.interface';
import { FormScopeDirective } from '../form/formscope.directive';
import { nFormater } from '../models/nformat.model';
import { MatCheckboxModule } from '@angular/material/checkbox';

export const NAVIGATIONKEYS = ['ArrowUp','ArrowDown','ArrowLeft','ArrowRight','Escape','Tab','Enter','Backspace','Delete','End','Home','Shift','CapsLock','Insert','PageUp','PageDown','PageDown','PageDown'];
export const NUMBERS = ['0','1','2','3','4','5','6','7','8','9'];
export const DECIMALS = ['0','1','2','3','4','5','6','7','8','9',','];
export const CTRL_KEYS = ['c','C','v','V','x','X'];

@Component({
    selector: 'xrmui-input',
    templateUrl: './input.xrmui.html',
    styleUrl: './input.xrmui.scss',
    imports: [FormsModule, NgClass, MatIcon,TextFieldModule,MatDatepickerModule,MatCheckboxModule]
})
export class XrmuiInput {
    @ViewChild('inputfield') searchElement?: ElementRef;
    @ViewChild('textareafield') textareaElement?: ElementRef;
    @ViewChild('datefield') datefieldElement?: ElementRef;
    @ViewChildren('option') options!: QueryList<ElementRef<HTMLDivElement>>;

    private changeScope = inject(ChangeScopeDirective, {optional: true,host: true });
    private formScope = inject(FormScopeDirective, {optional: true,host: true });

    private destroyRef = inject(DestroyRef);
    private blurThead? : any;

    label = input<string>('');
    _shortLabel = input<boolean | 'above' | undefined>(undefined, {alias: 'short-label'});

    shortLabel = computed(() => {
      const sl = this._shortLabel();
      if (sl) return sl;
      if (this.formScope) {
        if (this.formScope.formScope().labeltype == "short") return true;
        if (this.formScope.formScope().labeltype == "above") return "above";
      }
      return false;
    });

    placeholder = input<string>('');

    value = model<string | number | Date | boolean | null | undefined>(null);
    _disabled = input<boolean | undefined>(undefined, { alias: 'disabled'});
    disabled = computed(() => {

      const d = this._disabled();
      if (d != undefined) {
        return d;
      }

      if (this.formScope && this.formScope.formScope().disabled != undefined) {
        return this.formScope.formScope().disabled ?? false;
      }

      return false;
    });

    _showlock = input<boolean | undefined>(undefined, {alias: 'showlock'});
    showlock = computed(() => {
      const sl = this._showlock();
      if (sl != undefined) {
        return sl;
      }

      if (this.formScope && this.formScope.formScope().showlock != undefined) {
        return this.formScope.formScope().showlock ?? true;
      }
      return true;
    });

    required = input<boolean>(false);
    type = input<'text' | 'number' | 'password' | 'checkbox'>('text');
    setfocus = model<boolean>(false);
    autocomplete = input<ISearchService | null>( null);
    selectable = input<ISelectable[] | null>(null);
    selected = model<ISelectable | undefined>(undefined);
    optionsetvalue = input<OptionSetValue | null>(null);
    entityreference = input<EntityReference | null>(null);
    onselect = output<ISelectable | undefined>();
    error = input<boolean>(false);
    validate = model<'number' | 'decimal' | 'date' | 'checkbox' | null>(null);
    decimals = input<number | undefined>(undefined);
    onfocusEvent = output<void>({ alias:'focus' });
    onblurEvent = output<void>({alias: 'blur'});
    click = output<void>();
    resizeable = input<boolean>(true);
    info = input<string | null>(null);
    notdark = input<boolean>(false);
    _autoopenonblank = input<boolean | undefined>(undefined, { alias: "autoopenonblank" });
    nullable = input<boolean>(true);

    autoopenonblank = computed(() => {
      const au = this._autoopenonblank();
      if (au != undefined) {
        return au;
      }

      if (this.formScope && this.formScope.formScope().autoopenonblank != undefined) {
        return this.formScope.formScope().autoopenonblank ?? false;
      }
      return false;
    });

    decimalsUsed = output<number>();
    onEnter = output<void>();

    toolIcon = input<string | null>(null);
    toolClick = output<void>();

    numberoflines = input(1);
    maxlength = input<number | null>(null);

    hasfocus = signal(false);
    search: string = '';

    // private items: ISelectable[] | null = null;
    itemlist = signal<ISelectable[]>([]);
    showitems = signal(false);
    searchthread: any | null = null;
    searchnumber: number = 0;

    current?: ISelectable;
    currentindex: number = -1;

    shadowValue = signal<string>('');
    shadowDate = signal<Date | null>(null);

    shadowBool = signal<boolean>(false);

    isselect = computed(() => {
      const s = this.selectable();
      if (s && s.length > 0) return true;
      return false;
    });

    isnumber = computed(() => this.validate() == 'decimal' || this.validate() == 'number');

    inputclasses = computed(() => {
      if (!this.isnumber()) return this.type();
      return this.type() + ' ' + 'number';
    }); 

  constructor() {
    this.destroyRef.onDestroy(() => {
      if (this.blurThead) {
        clearTimeout(this.blurThead);
      }
    });

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

      if (type == 'checkbox' && validate != 'checkbox') {
        this.validate.set('checkbox');
      }
    });

    effect(() => {
      const value = this.value();
      if (value == null) {
        this.shadowValue.set('');
        this.shadowDate.set(null);
        this.shadowBool.set(false);
        return;
      }

      if (this.validate() == 'decimal' || this.validate() == 'number') {
        this.setNumberValueString();
        return;
      }

      if (this.validate() == 'date') {
        this.shadowDate.set(value as Date);
        return;
      }

      if (this.validate() == 'checkbox') {
        this.shadowBool.set(value as boolean);
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
      if (osv) {
        const n = osv.name$();
        this.shadowValue.set(n ?? '');
      }
      
      if (osv && sels && sels.length > 0) {
        const v = osv.value$();
        if (v != undefined) {
          const sel = sels.find(r => r.id == v.toString());
          if (sel) {
            this.selected.set(sel);
          }
        }
      };
    });

    effect(() => {
      const re = this.entityreference();
      if (re) {
        const name = re.name$() ?? '';
        this.shadowValue.set(name);
      }
    });

    effect(() => {
      const ap = this.autocomplete();

      if (ap) {
        this.itemlist.set(ap.items());
      }
    });
  }


  pickDate(d: MatDatepickerInputEvent<any,any>) {
    this.shadowDate.set(d.value);
    this.value.set(d.value);
    this.notifyOnChange();
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
      if (this.nullable() == true) {
        this.value.set(null);
      } else {
        this.value.set('');
      }
    } else {
      const next = sv.replaceAll('.','').replace(',','.');
      switch (this.validate()) {
        case 'number': {
          // sync value on blue ... this is to early
          break;
        }
        case 'decimal': {
          // syncvalue on blur ... this is to early
          break;
        }
        default:
          this.value.set(sv);

      }
    }
    this.searchbyname();
  }

  onCheckboxToggle() {
    this.value.set(this.shadowBool());
    this.notifyOnChange();
  }

  onFocus() {
    this.hasfocus.set(true);
    this.onfocusEvent.emit();

    if (this.autoopenonblank() && this.autocomplete()) {
      const cv = this.shadowValue() ?? '';
      if (cv == '') {
        this.searchbyname(true);
      }
    }
  }

  onBlur() {
    this.handleNumber();
    this.blurThead = setTimeout(() => {
      this.hasfocus.set(false);
      this.showitems.set(false);

      const sel = this.selected();

      const sv = this.shadowValue();

      if (sel && sel.name != sv) {
        this.selected.set(undefined);
        this.onselect.emit(undefined); 
        this.shadowValue.set('');
        this.notifyOnChange();
      }

        const re = this.entityreference();
        if (re && re.name$() != sv) {
          re.set(undefined, undefined);
          this.onselect.emit(undefined);
          this.shadowValue.set('');
          this.notifyOnChange();
        }

        const validate = this.validate();
        if (validate == 'decimal' || validate == 'number') {
          this.setNumberValueString();
        }
        this.onblurEvent.emit();
    },200);
  }

  onBlurCheckbox() {
    setTimeout(() => {
        this.hasfocus.set(false);
        this.onblurEvent.emit();
    },200);
  }

  onBlurDate() {
    setTimeout(() => {
        this.hasfocus.set(false);
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

      const re = this.entityreference();
      if (re) {
        re.set(this.current.id, this.current.name);
      }

      this.selected.set(this.current);
      this.onselect.emit(this.current);
      this.showitems.set(false);
      this.shadowValue.set(this.current.name ?? '');
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
            osv.set(Number(next.id), next.name)
          }
          this.selected.set(next);
          this.notifyOnChange();
      }
    }
  }

  select(e: Event, v: ISelectable) {
    e.stopImmediatePropagation();
    e.stopPropagation();
    this.value.set(v.name ?? '');

    const re = this.entityreference();
    if (re) {
      re.set(v.id, v.name);
    }

    this.selected.set(v);
    this.onselect.emit(v);
    this.notifyOnChange();
  }

  private next(e: number) {
    const items = this.itemlist();
    if (items.length > 0) {
      this.currentindex = this.currentindex + e;
      if (this.currentindex >= items.length) {
        this.currentindex = 0;
      }

      if (this.currentindex == -1) {
        this.currentindex = items.length - 1;
      }

      if (this.currentindex == -2) {
        this.currentindex = items.length - 1;
      }
      this.current = items[this.currentindex]
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

  toolClicked(e: Event) {
    e.stopPropagation();
    this.toolClick.emit();
  }

  private searchbyname(ommitcm?: boolean) {
    if (this.searchthread != null) {
      clearTimeout(this.searchthread);
    }

    this.searchthread = setTimeout(() => {
      this.dosearchbyname();
    }, 600)
  }

  private async dosearchbyname(ommitcm?: boolean) {
    const ap = this.autocomplete();
    if (ap) {
      await ap.search(this.shadowValue());
      this.currentindex = -1;
      this.current = undefined;
      this.showitems.set(true);
    }

    if (!ap && ommitcm != true) {
      this.notifyOnChange();
    } 
  }

  private notifyOnChange() {
    this.changeScope?.notifyChanged();
  }

  private handleNumber() {
    let sv = this.shadowValue();
    if (sv == '') {
      this.value.set(null);
    } else {
      const next = sv.replaceAll('.','').replace(',','.');
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
  }


  private setNumberValueString() {
    const v = Number(this.value());
    let decimals = this.decimals();

    if (decimals == undefined && this.validate() == 'number') {
      decimals = 0;
    }

    if (decimals != undefined) {
      this.shadowValue.set(nFormater.format(v, decimals))
    } else {
      this.shadowValue.set(nFormater.flex(v));
    }
  }
}
