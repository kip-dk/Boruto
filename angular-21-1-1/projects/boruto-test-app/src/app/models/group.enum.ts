import { enumToSelectable } from "boruto-xrmui";

export enum GroupEnum {
    A_group = 100000000,
    B_group = 100000001,
    C_group = 100000002,
    D_group = 100000003,
    E_group = 100000004,
    F_group = 100000005
}

export const GroupOptions = enumToSelectable(GroupEnum, {
    [GroupEnum.A_group]: "match_case",
    [GroupEnum.B_group]: "sd",
    [GroupEnum.C_group]: "keyboard_external_input",
    [GroupEnum.D_group]: "ev_mobiledata_badge",
    [GroupEnum.E_group]: "shift_lock",
    [GroupEnum.F_group]: "speed_1_25"
})