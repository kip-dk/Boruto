import { Signal } from "@angular/core";

export interface IMenu {
    id: string | number;
    label?: string;
    icon?: string;
    disabled?: boolean;
    badget?: number;
    key?: string;
    visible?: Signal<boolean>;
    error?: Signal<boolean>;
    click?(): void;
    clickAsync?(): Promise<void>;
    effects?: string[];
}