import { signal, untracked } from "@angular/core";
import { OptionSetValue } from "./optionsetvalue.model";

export class MultiOptionSetValue {
    public values = signal<OptionSetValue[]>([]);
    public initialValue = '';

    fromPayload(a: any, f: string) {
        const values = a[f] as string;
        const names = a[f+"@OData.Community.Display.V1.FormattedValue"] as string;
        if (values == undefined || values == null || values.length == 0) {
            this.values.set([]);
            return;
        }
        const _v = values.split(',').map(r => Number(r));
        const _n = names.split(';');
        const curr = untracked(() => this.values());
        const result = _v.map((value, i) => new OptionSetValue(value, _n[i]?.trim() ?? ''));

        const changed = curr.length !== result.length ||
            curr.some(c => !result.some(r => r.value === c.value));

        if (changed) {
            this.values.set(result);
        }
        this.initialValue = values ?? '';
    } 

    changed(): boolean {
        const vals = untracked(() => this.values());
        if (this.initialValue.length == 0 && vals.length == 0) return false;

        const initialNumbers = this.initialValue.split(',').map(r => Number(r));
        const curNums = vals.map(v => v.value ?? -1);

        return initialNumbers.length != curNums.length ||
            initialNumbers.some(c => !curNums.some(r => r == c));
    }

    toJsonProperty(): any {
        const r = untracked(() => this.values());
        if (r.length > 0) {
            return r.map(o => o.value?.toString()).join(",");
        }
        return null;
    }
}