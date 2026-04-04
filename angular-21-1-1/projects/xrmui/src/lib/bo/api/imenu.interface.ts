
export interface IMenu {
    id: number;
    label?: string;
    icon?: string;
    disabled?: boolean;
    badget?: number;
    key?: string;
    click?(): void;
    clickAsync?(): Promise<void>;
    effects?: string[];
}