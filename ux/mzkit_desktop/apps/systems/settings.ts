namespace apps.systems {

    const pages: {} = {
        "mzkit_page": "MZKit Settings",
        "msraw_page": "Raw File Viewer",
        "chromagram_page": "Chromagram Plot Styles",
        "formula_page": "Formula Search",
        "element_profile_page": "Formula Search Profile",
        "molecule_networking_page": "Molecular Networking"
    };

    export interface BootstrapTable {
        bootstrapTable(arg1: any, arg2?: any);
    }

    export class settings extends Bootstrap {

        get appName(): string {
            return "mzkit/settings";
        }

        protected init(): void {
            this.mzkit_page_btn_onclick();
            this.load_profileTable();
        }

        private static getElementProfileTable(): BootstrapTable {
            return <any>$("#tableDiv");
        }

        private load_profileTable() {
            const bootstrap: BootstrapTable = settings.getElementProfileTable();

            let data = [];
            let columns = [{
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
                    type: "number",

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

            let tableOptions = {
                columns: columns,
                editable: true, //editable需要设置为 true
                striped: true,
                clickToSelect: true
            }
            bootstrap.bootstrapTable(tableOptions);
            bootstrap.bootstrapTable("load", data);
        }

        private static closeAll(): typeof settings {
            for (let page of Object.keys(pages)) {
                $ts(`#${page}`).hide();
            }

            return this;
        }

        private static show(page_id: string) {
            $ts(`#${page_id}`).show();
            $ts("#title").display(pages[page_id]);
        }

        public mzkit_page_btn_onclick() {
            settings.closeAll().show("mzkit_page");
        }

        public msraw_btn_onclick() {
            settings.closeAll().show("msraw_page");
        }

        public chromagram_btn_onclick() {
            settings.closeAll().show("chromagram_page");
        }

        public formula_btn_onclick() {
            settings.closeAll().show("formula_page");
        }

        public profile_btn_onclick() {
            settings.closeAll().show("element_profile_page");
        }

        public add_element_onclick() {
            settings.getElementProfileTable().bootstrapTable('append', [{ "atom": "", "min": 0, "max": 0 }]);
        }

        public molecule_networking_btn_onclick() {
            settings.closeAll().show("molecule_networking_page");
        }
    }
}