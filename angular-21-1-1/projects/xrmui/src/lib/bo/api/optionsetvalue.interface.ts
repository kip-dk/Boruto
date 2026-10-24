import { signal, Signal, WritableSignal } from "@angular/core";

export interface OptionSetValue {
    value$: Signal<number | undefined>;
    name$: Signal<string | undefined>;
    set(id: number |undefined, name: string| undefined): void;
}


export function createOptionSetValue(value: number, name: string): OptionSetValue & { value: number; name: string } {
    const value$ = signal<number | undefined>(value);
    const name$ = signal<string | undefined>(name);
    
    return {
        name: name,
        value: value,
        value$,
        name$,
        set(id, name) {
            value$.set(id);
            name$.set(name);
        }
    };
}