import { computed, Directive, input, signal } from "@angular/core";
import { Form } from '../api/form.interface';

@Directive({
    selector: '[formScope]',
    exportAs: 'formScope',
    standalone: true,
})
export class FormScopeDirective {
    formScope = input.required<Form>();
}