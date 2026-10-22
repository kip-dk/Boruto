import { Signal, WritableSignal } from "@angular/core";
import { OptionSetValue } from "./optionsetvalue.interface";

export interface MultiOptionSetValue {
    values: WritableSignal<OptionSetValue[]>;
}