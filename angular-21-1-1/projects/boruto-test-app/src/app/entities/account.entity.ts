import { computed, signal } from "@angular/core";
import { Entity } from "boruto-xrmservice";
import { ISelectable } from "boruto-xrmui";

export class Account extends Entity implements ISelectable {
    constructor() {
        super(
            'accounts', 
            'accountid'
            , true
            ,undefined,
            [
                "numberOfChangedFields"
            ]);
    }

    accountnumber: string | null = null;
    name: string = '';

    numberofemployees = signal<number>(0);

    numberOfChangedFields = computed(() => this.changes().length);

    meta(): Account {
        return this;
    }
}