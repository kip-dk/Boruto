import { WritableSignal } from "@angular/core";

export interface OptionSetValue {
    value$: WritableSignal<number | undefined>;
    name$: WritableSignal<string>
}