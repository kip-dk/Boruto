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
        this.changeScope().notifyOnChanged();
    }
}