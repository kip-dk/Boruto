import { Directive, input } from "@angular/core";
import { ChangedNotifier } from "../api/changednotifier.interface";

@Directive({
    selector: '[changeScope]',
    exportAs: 'changeScope',
    standalone: true,
})
export class ChangeScopeDirective {
    changeScope = input.required<ChangedNotifier>();
    
    notifyChanged(): void {
        const cp = this.changeScope();
        if (cp) {
            cp.notifyOnChanged.call(cp);
            const oc = cp.onChanged;

            if (oc) {
                oc.call(cp);
            }
        }
    }
}