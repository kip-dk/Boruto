import { Directive, ElementRef, input, output, effect, inject } from '@angular/core';

@Directive({ selector: '[ko-focus]' })
export class KoFocusDirective {

private hostElement: ElementRef = inject(ElementRef);

    focus = input<boolean>(false, { alias: 'ko-focus' });
    focusChange = output<boolean>({ alias: 'ko-focusChange' });

    constructor() { 
        effect(() => {
            const isFocused = this.focus();
            if (isFocused) {
                setTimeout(() => {
                    this.hostElement.nativeElement.focus();
                    this.focusChange.emit(false);
                },10);
            }
        })
    }
}
