import { Signal, WritableSignal } from "@angular/core";

export interface EntityReference {
    id$: Signal<string |undefined>;
    name$: Signal<string | undefined>;
    set(id: string |undefined, name: string| undefined): void;
}