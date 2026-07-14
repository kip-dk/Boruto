import { Signal, WritableSignal } from "@angular/core";

export interface OptionSetValue {
    value$: Signal<number | undefined>;
    name$: Signal<string | undefined>
    set(id: number |undefined, name: string| undefined): void;
}