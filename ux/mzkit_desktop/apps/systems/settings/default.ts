namespace apps.systems.settings_default {

    export const element_columns = [{
        title: "Atom Element",
        field: "atom",
        sortable: true,
        width: 200,
        editable: true,
    }, {
        title: "Min",
        field: "min",
        sortable: true,
        width: 200,
        editable: {
            type: "number"
        }
    }, {
        title: "Max",
        field: "max",
        sortable: true,
        width: 200,
        editable: {
            type: "number"
        }
    }];

    export const default_adducts_pos: string[] = ["[M]+", "[M+H]+", "[M+Na]+", "[M+NH4]+", "[2M+H]+", "[M-H2O+H]+", "[M-2H2O+H]+"];
    export const default_adducts_neg: string[] = ["[M-H]-", "[M+Cl]-", "[M-H2O-H]-", "[2M-H]-", "[M+COOH]-"];
}