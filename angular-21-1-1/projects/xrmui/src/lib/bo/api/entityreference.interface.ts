import { WritableSignal } from "@angular/core";

export interface EntityReference {
    id$: WritableSignal<string |undefined>;
    name$: WritableSignal<string | undefined>;
}