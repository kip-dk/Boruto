import { enumToSelectable } from "boruto-xrmui";

export enum OwnershipcodeEnum {
    Offentlig = 1,
    Privat = 2,
    Datterselskab = 3,
    Andet = 4
}

export const OwnershipcodeOptions = enumToSelectable(OwnershipcodeEnum, 
    {
        [OwnershipcodeEnum.Offentlig] : "location_city",
        [OwnershipcodeEnum.Privat]: "factory",
        [OwnershipcodeEnum.Datterselskab]: "account_tree",
        [OwnershipcodeEnum.Andet]: "other_houses"
    }
);