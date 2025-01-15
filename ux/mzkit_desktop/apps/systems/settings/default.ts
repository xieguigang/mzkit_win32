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
}