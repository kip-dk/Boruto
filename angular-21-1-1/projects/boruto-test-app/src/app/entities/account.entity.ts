import { Entity } from "boruto-xrmservice";
import { ISelectable } from "boruto-xrmui";

export class Account extends Entity implements ISelectable {
    constructor() {
        super('accounts', 'accountid', true);
    }

    accountnumber: string | null = null;
    name: string = '';

    meta(): Account {
        return this;
    }
}