import { Directive, ElementRef, HostListener, input, Output, EventEmitter, SimpleChanges, output, effect } from '@angular/core';

@Directive({
  selector: '[ko-decimal]'
})
export class KoDecimalDirective {
  constructor(private el: ElementRef) {
    effect(() => {
      const align = this.align();
      if (this.el.nativeElement) {
        this.el.nativeElement.style['text-align'] = this.align();
      }
    });

    effect(() => {
      const v = this.value();
      this.setValueString();
    })
  }


  decimals = input<number>(2, { alias: 'ko-decimals' });
  value = input<number | null>(null, { alias: 'ko-value' });
  valueChanged = output<number | null>({ alias: 'ko-valueChange' });
  align = input<string>('right');

  controlDown: boolean = false;
  syncThread!: any;

  @HostListener('keydown', ['$event']) onKeydown($event: KeyboardEvent) {
    switch ($event.key) {
      case 'Backspace':
      case 'Tab':
      case 'ArrowLeft':
      case 'ArrowRight':
      case 'Delete':
      case '0':
      case '1':
      case '2':
      case '3':
      case '4':
      case '5':
      case '6':
      case '7':
      case '8':
      case '9':
      case '-': {
        this.bindValueBack();
        return true;
      }
      case ',': {
        if (this.decimals() == 0) {
          return false;
        }
        this.bindValueBack();
        return true;
      }
      case 'Control': {
        this.controlDown = true;
        this.bindValueBack();
        return true;
      }
      case 'c':
      case 'C':
      case 'v':
      case 'V': {
        if (this.controlDown == true) {
          this.bindValueBack();
          return true;
        }
        $event.stopPropagation();
        return false;
      }
      default: {
        $event.stopPropagation();
        return false;
      }
    }
  }

  @HostListener('blur') onBlur() {
    if (this.syncThread != null) clearTimeout(this.syncThread);
    this.syncThread = null;
    this.doBindValueBack();
    this.setValueString();
  }

  private setValueString(): void {
    if (this.el.nativeElement != null) {
      const v = this.value();
      if (v != null) {
        this.el.nativeElement.value = this.numberToString(v, this.decimals());
      } else {
        this.el.nativeElement.value = "";
      }
    }
  }

  private bindValueBack(): void;
  private bindValueBack(timeout: number): void;
  private bindValueBack(timeout: number = 3000): void {
    if (this.syncThread != null) clearTimeout(this.syncThread);
    this.syncThread = setTimeout(() => {
      this.syncThread = null;
      this.doBindValueBack();
    }, timeout);
  }

  private doBindValueBack(): void {
    if (this.el.nativeElement != null) {
      let v = this.el.nativeElement.value;
      if (v == null || v == '') {
        this.valueChanged.emit(null);
        return;
      } else {
        this.valueChanged.emit(Number(v.replace(',','.')))
      }
    }
  }

  private numberToString(value: number, decimals: number): string {
    if (value == null) {
      return '';
    }

    if (decimals == null) {
      return value.toString().replace('.', ',');
    }

    let result = value.toString().replace('.', ',');
    if (decimals <= 0) {
      var split = result.split(',');
      if (split.length == 2 && Number(split[1]) > 0) {
        return Math.round(value).toString().replace(".", ",").split(',')[0];
      }
      return result.split(',')[0];
    }

    if (result.indexOf(',') < 0) {
      result += ',0';
    }

    let decimalPart = result.split(',')[1];
    if (decimalPart.length == decimals) {
      return result;
    }

    if (decimalPart.length <= decimals) {
      return result.split(',')[0] + ',' + String(decimalPart + '00000000000000000' + decimalPart).substring(0, decimals);
    }

    var round = Number('1' + String('00000000000000000').slice((-1) * decimals));

    var roundedValue = Math.round(value * round) / round;

    return this.numberToString(roundedValue, decimals);
  }

}
