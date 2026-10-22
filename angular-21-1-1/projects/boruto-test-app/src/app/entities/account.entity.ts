import { computed, signal } from "@angular/core";
import { Entity, EntityReference, OptionSetValue } from "boruto-xrmservice";
import { ISelectable } from "boruto-xrmui";
import { MultiOptionSetValue } from "projects/xrm/src/lib/models/multioptionsetvalue.model";

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
    parentaccountid: EntityReference = new EntityReference().meta("accounts","parentaccountid");
    createdon: Date = new Date();
    lastonholdtime: Date | null = null;
    ownershipcode: OptionSetValue = new OptionSetValue();
    bor_groups = new MultiOptionSetValue();


    override OnChanged(): void {
        console.log('her');
    }

    numberOfChangedFields = computed(() => this.changes().length);

    meta(): Account {
        this.lastonholdtime = new Date();
        return this;
    }

    onFetch() {
        console.log(this);
    }
}