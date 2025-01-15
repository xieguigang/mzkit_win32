namespace apps.systems {

    const pages: {} = {
        "mzkit_page": "MZKit Settings",
        "msraw_page": "Raw File Viewer",
        "chromagram_page": "Chromagram Plot Styles",
        "formula_page": "Formula Search",
        "element_profile_page": "Formula Search Profile",
        "molecule_networking_page": "Molecular Networking"
    };
    const $: jQuery = (<any>window).$;

    function logicalDefault(logic: any, _default: boolean): boolean {
        if (isNullOrUndefined(logic) || isNullOrEmpty(logic)) {
            return _default;
        } else if (typeof logic == "number") {
            return logic != 0.0;
        } else if (typeof logic == "string") {
            return parseBoolean(logic);
        } else if (typeof logic == "boolean") {
            return logic;
        } else {
            return logic;
        }
    }

    export class settings extends Bootstrap {

        get appName(): string {
            return "mzkit/settings";
        }

        public static mzkit_configs: mzkit_configs = null;

        protected init(): void {
            const vm = this;

            vm.mzkit_page_btn_onclick();

            app.desktop.mzkit.loadSettings()
                .then(async function (json) {
                    const json_str: string = await json;

                    console.log("view of the default config json:");
                    console.log(json_str);

                    vm.load_settings_json(json_str);
                });
        }

        /**
         * load settings profile data on application startup
        */
        private load_settings_json(json_str: string) {
            const settings = JSON.parse(json_str) || {};
            const configs = apps.systems.settings.defaultSettings();

            console.log("get mzkit configurations:");
            console.log(settings);

            // deal with the possible null reference value
            settings.precursor_search = settings.precursor_search || {};
            settings.ui = settings.ui || {};
            settings.viewer = settings.viewer || {};

            // make data object conversion
            configs.formula_ppm = settings.precursor_search.ppm || 5;
            configs.formula_adducts = settings.precursor_search.precursor_types || {
                pos: [],
                neg: []
            };

            configs.remember_layout = logicalDefault(settings.ui.rememberLayouts, true);
            configs.remember_location = logicalDefault(settings.ui.rememberWindowsLocation, true);
            configs.language = settings.ui.language || 2;

            configs.colorset = settings.viewer.colorSet || [];
            configs.fill_plot_area = logicalDefault(settings.viewer.fill, true);
            configs.xic_da = settings.xic_da;

            this.loadConfigs(configs);
        }

        public remember_location_onchange(value: string | string[]) {
            settings.mzkit_configs.remember_location = <any>(Array.isArray(value) ? value[0] : value);
        }

        public remember_layout_onchange(value: string | string[]) {
            settings.mzkit_configs.remember_layout = <any>(Array.isArray(value) ? value[0] : value);
        }

        public language_onchange(value: string | string[]) {
            settings.mzkit_configs.language = <any>(Array.isArray(value) ? value[0] : value);
        }

        public fill_plot_area_onchange(value: string | string[]) {
            settings.mzkit_configs.fill_plot_area = <any>(Array.isArray(value) ? value[0] : value);
        }

        /**
         * load settings on application startup
        */
        private loadConfigs(configs: mzkit_configs) {
            const formula_profiles = configs.formula_search;

            settings.mzkit_configs = configs;
            settings.loadColorList(configs.colorset);
            settings.load_profileTable(configs);
            settings.bindRangeDisplayValue(configs, function (config) {
                // save
            });

            $ts.value("#language", configs.language);
            $ts.value("#remember_location", configs["remember_location"]);
            $ts.value("#remember_layout", configs["remember_layout"]);

            $ts.value("#fragment_cutoff", configs["fragment_cutoff"]);
            $ts.value("#fill_plot_area", configs["fill_plot_area"]);

            $ts.value("#small_molecule_profile", formula_profiles.smallMoleculeProfile.type);
            $ts.value("#sm_common", formula_profiles.smallMoleculeProfile.isCommon);

            $ts.value("#np_profile", formula_profiles.naturalProductProfile.type);
            $ts.value("#np_common", formula_profiles.naturalProductProfile.isCommon);

            app.desktop.mzkit.getAllAdducts()
                .then(async function (json) {
                    const json_str: string = await json;
                    const list: string[] = JSON.parse(json_str);
                    const selected = configs.formula_adducts || {
                        pos: [], neg: []
                    };

                    if (isNullOrEmpty(selected.pos)) {
                        selected.pos = settings_default.default_adducts_pos;
                    }
                    if (isNullOrEmpty(selected.neg)) {
                        selected.neg = settings_default.default_adducts_neg;
                    }

                    $ts("#adducts_pos").clear();
                    $ts("#adducts_neg").clear();

                    // for (let adduct of list) {
                    //     const key_id: string = adduct;
                    //     const value: boolean = selected.indexOf(adduct) > -1;
                    //     const checked: string = value ? "checked" : "";

                    //     adducts.appendElement($ts("<li>", { class: "list-group-item" }).display(`
                    //         <input class="form-check-input me-1" 
                    //             type="checkbox" 
                    //             value=""
                    //             id="${key_id}" ${checked}>
                    //         <label class="form-check-label" for="${key_id}">${adduct}</label>`));
                    // }

                    $ts.value("#adducts_pos", selected.pos.join("\n"));
                    $ts.value("#adducts_neg", selected.neg.join("\n"));
                });
        }

        private static defaultSettings(): mzkit_configs {
            return <mzkit_configs>{
                // mzkit app
                "remember_location": true,
                "remember_layout": true,
                "language": 2,

                // raw file viewer
                "xic_da": 0.05,
                "fragment_cutoff": "relative",
                "fragment_cutoff_value": 0.05,

                // chromagram plot
                "colorset": [],
                "fill_plot_area": true,

                // preset element profiles
                "formula_search": {
                    "smallMoleculeProfile": <element_profile>{ type: "Wiley", isCommon: true },
                    "naturalProductProfile": <element_profile>{ type: "Wiley", isCommon: true },
                    "elements": {},
                },
                "formula_ppm": 20,
                "formula_adducts": {
                    pos: settings_default.default_adducts_pos,
                    neg: settings_default.default_adducts_neg
                },

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

        /**
         * auto binding of the [min,max] value range form control
        */
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

        /**
         * get table html UI for create custom element profiles
        */
        private static getElementProfileTable(): BootstrapTable {
            return <any>$("#custom_element_profile");
        }

        private static load_profileTable(configs: mzkit_configs) {
            const bootstrap: BootstrapTable = settings.getElementProfileTable();
            const tableOptions = {
                columns: settings_default.element_columns,
                editable: true, //editable需要设置为 true
                striped: true,
                clickToSelect: true
            }
            const profiles = configs.formula_search.elements || {};
            const elements: element_count[] = $from(Object.keys(profiles))
                .Select(function (atom) {
                    return <element_count>{
                        atom: atom,
                        min: profiles[atom].min,
                        max: profiles[atom].max
                    }
                }).ToArray();

            bootstrap.bootstrapTable(tableOptions);
            settings.loadProfileTable(elements, bootstrap);
        }

        private static loadProfileTable(elements: element_count[], bootstrap: BootstrapTable = settings.getElementProfileTable()) {
            bootstrap.bootstrapTable("removeAll");
            bootstrap.bootstrapTable("load", elements);
        }

        private static closeAll(): typeof settings {
            for (let page of Object.keys(pages)) {
                $ts(`#${page}`).hide();
            }

            return this;
        }

        private profile_name: string;

        public copy_profile_onchange(value: string | string[]) {
            if (Array.isArray(value)) {
                value = value[0];
            }

            console.log(`get profile name: ${value}!`);
            this.profile_name = value;
        }

        public reset_profile_onclick() {
            // load from mzkit host
            app.desktop.mzkit.getProfile(this.profile_name || "Custom_Profile")
                .then(async function (json) {
                    const json_str: string = await json;
                    const preset = JSON.parse(json_str);

                    console.log(preset);

                    if (isNullOrUndefined(preset)) {
                        settings.loadProfileTable([]);
                    } else if (isNullOrEmpty(preset.candidateElements)) {
                        settings.loadProfileTable([]);
                    } else {
                        settings.loadProfileTable($from(preset.candidateElements).Select(function (c: any) {
                            return <element_count>{
                                "atom": c.Element,
                                "min": c.MinCount,
                                "max": c.MaxCount
                            };
                        }).ToArray());
                    }
                });
        }

        public preset_colorset_onchange(value: string | string[]) {
            app.desktop.mzkit.GetColors((Array.isArray(value) ? value[0] : value))
                .then(async function (json) {
                    const json_str: string = await json;
                    const colors: string[] = JSON.parse(json_str);

                    settings.loadColorList(colors);
                });
        }

        private static loadColorList(colors: string | string[]) {
            const list = $ts("#colorset").clear();

            if (typeof colors === "string") {
                colors = [colors];
            }

            if (!isNullOrUndefined(colors)) {
                for (let color of colors) {
                    settings.appendColor(list, color);
                }
            }
        }

        private static appendColor(list: IHTMLElement, color: string) {
            list.appendElement($ts("<a>", {
                href: "javascript:void(0);",
                class: ["list-group-item", "list-group-item-action"]
            }).display(`<span style="background-color:${color}">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span> ${color}`)
            );
        }

        private static getColorList(): string[] {
            const list = $ts("#colorset");
            const links = list.getElementsByTagName("span");
            const colors: string[] = [];

            for (let i: number = 0; i < links.length; i++) {
                let rgb_color = links.item(i).style.backgroundColor;
                let colorVal = TypeScript.ColorManager.toColorObject(rgb_color);
                let html_color: string = colorVal.toHexString();

                colors.push(html_color);
            }

            console.log("get color list for run plot:");
            console.log(colors);

            return colors;
        }

        public add_color_onclick() {
            const link: any = $('#mycolor')
            const color: string = link.colorpicker("val");

            settings.appendColor($ts("#colorset"), color);
        }

        public clear_colors_onclick() {
            $ts("#colorset").clear();
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
            settings.getElementProfileTable().bootstrapTable('append', [{
                "atom": "",
                "min": 0,
                "max": 0
            }]);
        }

        public molecule_networking_btn_onclick() {
            settings.closeAll().show("molecule_networking_page");
        }

        /**
         * save profile table as custom profiles
        */
        public save_elements_onclick() {
            const table = settings.getElementProfileTable();
            const data: element_count[] = table.bootstrapTable("getData");

            console.log("get element profiles:");
            console.table(data);

            app.desktop.mzkit.SetStatus("save_elements", JSON.stringify(data));
        }

        public static invoke_save() {
            console.log("invoke settings save action!");

            settings.mzkit_configs.colorset = settings.getColorList();

            // do save configuration
            app.desktop.mzkit
                .Save(JSON.stringify(settings.mzkit_configs))
                .then(async function () {
                    console.log("done!");
                });
        }

        public apply_settings_onclick() {
            settings.invoke_save();
        }

        public close_page() {
            app.desktop.mzkit.close();
        }
    }
}