namespace apps.systems {

    const pages: {} = {
        "mzkit_page": "MZKit Settings",
        "msraw_page": "Raw File Viewer",
        "chromagram_page": "Chromagram Plot Styles",
        "formula_page": "Formula Search",
        "element_profile_page": "Formula Search Profile",
        "molecule_networking_page": "Molecular Networking"
    };

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

    export interface BootstrapTable {
        bootstrapTable(arg1: any, arg2?: any);
    }

    export interface mzkit_configs {
        // mzkit app
        "remember-location": boolean;
        "remember-layout": boolean;
        "language": 0 | 1 | 2;

        // raw file viewer
        "xic_ppm": number;
        "fragment_cutoff": "relative" | "quantile";
        "fragment_cutoff_value": number;

        // chromagram plot
        "colorset": string[];
        "fill-plot-area": boolean;

        // preset element profiles
        "formula_search": {
            "naturalProductProfile": element_profile,
            "smallMoleculeProfile": element_profile,
            "elements": element_count[]
        };

        "formula_ppm": number;
        "formula_adducts": string[];

        // molecular networking
        "layout_iterations": number;

        // graph layouts
        "stiffness": number;
        "repulsion": number;
        "damping": number;

        // spectrum tree
        "node_identical": number;
        "node_similar": number;
        "edge_filter": number;

        // network styling
        "node_radius_min": number;
        "node_radius_max": number;

        "link_width_min": number;
        "link_width_max": number;
    }

    export interface element_count {
        atom: string;
        min: number;
        max: number;
    }

    export interface element_profile {
        "type": "Wiley" | "DNP";
        "isCommon": boolean;
    }

    export class settings extends Bootstrap {

        get appName(): string {
            return "mzkit/settings";
        }

        private mzkit_configs: mzkit_configs = null;

        protected init(): void {
            const vm = this;

            vm.mzkit_page_btn_onclick();

            app.desktop.mzkit.loadSettings()
                .then(async function (json) {
                    const json_str: string = await json;
                    const settings = JSON.parse(json_str);

                    console.log("get mzkit configurations:");
                    console.log(settings);

                    vm.loadConfigs(apps.systems.settings.defaultSettings());
                });
        }

        private loadConfigs(configs: mzkit_configs) {
            const formula_profiles = configs.formula_search;

            this.mzkit_configs = configs;

            settings.load_profileTable(configs);
            settings.bindRangeDisplayValue(configs, function (config) {
                // save
            });

            $ts.value("#language", configs.language);
            $ts.value("#remember-location", configs["remember-location"]);
            $ts.value("#remember-layout", configs["remember-layout"]);

            $ts.value("#fragment_cutoff", configs["fragment_cutoff"]);
            $ts.value("#fill-plot-area", configs["fill-plot-area"]);

            $ts.value("#small_molecule_profile", formula_profiles.smallMoleculeProfile.type);
            $ts.value("#sm_common", formula_profiles.smallMoleculeProfile.isCommon);

            $ts.value("#np_profile", formula_profiles.naturalProductProfile.type);
            $ts.value("#np_common", formula_profiles.naturalProductProfile.isCommon);
        }

        private static defaultSettings(): mzkit_configs {
            return <mzkit_configs>{
                // mzkit app
                "remember-location": true,
                "remember-layout": true,
                "language": 2,

                // raw file viewer
                "xic_ppm": 10,
                "fragment_cutoff": "relative",
                "fragment_cutoff_value": 0.05,

                // chromagram plot
                "colorset": [],
                "fill-plot-area": true,

                // preset element profiles
                "formula_search": {
                    "smallMoleculeProfile": <element_profile>{ type: "Wiley", isCommon: true },
                    "naturalProductProfile": <element_profile>{ type: "Wiley", isCommon: true },
                    "elements": [
                        <element_count>{ atom: "C", min: 1, max: 100 },
                        <element_count>{ atom: "H", min: 1, max: 1000 },
                        <element_count>{ atom: "O", min: 0, max: 100 }
                    ],
                },
                "formula_ppm": 20,
                "formula_adducts": [],

                // molecular networking
                "layout_iterations": 100,

                // graph layouts
                "stiffness": 41.76,
                "repulsion": 10000,
                "damping": 0.41,

                // spectrum tree
                "node_identical": 0.85,
                "node_similar": 0.8,
                "edge_filter": 0.8,

                // network styling
                "node_radius_min": 1,
                "node_radius_max": 30,

                "link_width_min": 1,
                "link_width_max": 12
            }
        }

        private static bindRangeDisplayValue(configs: mzkit_configs, callback: (c: mzkit_configs) => void) {
            const inputs = $ts.select(".form-range");
            const labels = $ts.select(".form-label").ToDictionary(l => l.getAttribute("for"), lb => lb);
            const label_text0 = $ts.select(".form-label").ToDictionary(l => l.getAttribute("for"), lb => lb.innerText);

            for (let range of inputs.Select(i => <HTMLInputElement>i).ToArray()) {
                const id = range.id;
                const label_text_raw = label_text0.Item(id);
                const label_ctl = labels.Item(id);
                const label_update = function () {
                    label_ctl.innerText = `${label_text_raw} (${range.value})`;
                    configs[id] = range.value;
                    callback(configs);
                };

                range.onchange = label_update;

                if (!isNullOrUndefined(configs[id])) {
                    range.value = configs[id];
                    label_update();
                }
            }
        }

        private static getElementProfileTable(): BootstrapTable {
            return <any>$("#custom_element_profile");
        }

        private static load_profileTable(configs: mzkit_configs) {
            const bootstrap: BootstrapTable = settings.getElementProfileTable();
            const tableOptions = {
                columns: element_columns,
                editable: true, //editable需要设置为 true
                striped: true,
                clickToSelect: true
            }
            bootstrap.bootstrapTable(tableOptions);
            bootstrap.bootstrapTable("load", configs.custom_element_profile || []);
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