namespace apps.systems {

    const pages: {} = {
        "mzkit_page": "MZKit Settings",
        "msraw_page": "Raw File Viewer",
        "chromagram_page": "Chromagram Plot Styles",
        "formula_page": "Formula Search",
        "element_profile_page": "Formula Search Profile",
        "molecule_networking_page": "Molecular Networking"
    };

    // jquery should be loaded before this application module file
    const $: jQuery = (<any>window).$;

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
            const settings: mzkit_configs = JSON.parse(json_str) || apps.systems.settings_default.defaultSettings();

            console.log("get mzkit configurations:");
            console.log(settings);

            // deal with the possible null reference value
            settings.precursor_search = settings.precursor_search || {
                positive: [],
                negative: [],
                ppm: 20
            };
            settings.ui = settings.ui || {
                "language": "System",
                "rememberWindowsLocation": true,
                "rememberLayouts": true,
            };
            settings.ui.rememberLayouts = settings_default.logicalDefault(settings.ui.rememberLayouts, true);
            settings.ui.rememberWindowsLocation = settings_default.logicalDefault(settings.ui.rememberWindowsLocation, true);
            settings.ui.language = settings_default.stringToLanguage(settings.ui.language);

            settings.MRMLibfile = null;
            settings.QuantifyIonLibfile = null;
            settings.pubchemWebCache = null;
            settings.random = null;
            settings.recentFiles = null;
            settings.workspaceFile = null;

            settings.viewer = settings.viewer || {
                "XIC_da": 0.1,
                "ppm_error": 20,
                "colorSet": [],
                "method": null,
                "intoCutoff": 0.05,
                "quantile": 0.65,
                "fill": true
            };
            settings.viewer.colorSet = settings.viewer.colorSet || [];

            this.loadConfigs(settings);
        }

        public remember_location_onchange(value: string | string[]) {
            settings.mzkit_configs.ui.rememberWindowsLocation = settings_default.logicalDefault(Array.isArray(value) ? value[0] : value, true);
        }

        public remember_layout_onchange(value: string | string[]) {
            settings.mzkit_configs.ui.rememberLayouts = settings_default.logicalDefault(Array.isArray(value) ? value[0] : value, true);
        }

        public language_onchange(value: string | string[]) {
            settings.mzkit_configs.ui.language = settings_default.languageToString(Array.isArray(value) ? value[0] : value);
        }

        public fill_plot_area_onchange(value: string | string[]) {
            settings.mzkit_configs.viewer.fill = settings_default.logicalDefault(Array.isArray(value) ? value[0] : value, true);
        }

        /**
         * load settings on application startup
        */
        private loadConfigs(configs: mzkit_configs) {
            const formula_profiles = configs.formula_search;

            settings.mzkit_configs = configs;
            settings.loadColorList(configs.viewer.colorSet);
            settings.load_profileTable(configs);
            settings.bindRangeDisplayValue(configs, function (config) {
                // save
            });

            $ts.value("#language", configs.ui.language);
            $ts.value("#remember_location", configs.ui.rememberWindowsLocation);
            $ts.value("#remember_layout", configs.ui.rememberLayouts);

            $ts.value("#fragment_cutoff", configs.viewer.intoCutoff);
            $ts.value("#fill_plot_area", configs.viewer.fill);

            $ts.value("#small_molecule_profile", formula_profiles.smallMoleculeProfile.type);
            $ts.value("#sm_common", formula_profiles.smallMoleculeProfile.isCommon);

            $ts.value("#np_profile", formula_profiles.naturalProductProfile.type);
            $ts.value("#np_common", formula_profiles.naturalProductProfile.isCommon);

            app.desktop.mzkit.getAllAdducts()
                .then(async function (json) {
                    const json_str: string = await json;
                    const list: string[] = JSON.parse(json_str);
                    const selected = configs.precursor_search || {
                        positive: [], negative: [], ppm: 20
                    };

                    if (isNullOrEmpty(selected.positive)) {
                        selected.positive = settings_default.default_adducts_pos;
                    }
                    if (isNullOrEmpty(selected.negative)) {
                        selected.negative = settings_default.default_adducts_neg;
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

                    $ts.value("#adducts_pos", selected.positive.join("\n"));
                    $ts.value("#adducts_neg", selected.negative.join("\n"));
                });
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

        public static __dosave: Delegate.Action = null;

        /**
         * display a config page
        */
        private static show(page_id: string, save: Delegate.Action) {
            settings.__dosave = save;

            $ts(`#${page_id}`).show();
            $ts("#title").display(pages[page_id]);
        }

        /**
         * system UI page
        */
        public mzkit_page_btn_onclick() {
            settings.closeAll().show("mzkit_page", () => {

            });
        }

        public msraw_btn_onclick() {
            settings.closeAll().show("msraw_page", () => {

            });
        }

        public chromagram_btn_onclick() {
            settings.closeAll().show("chromagram_page", () => {

            });
        }

        /**
         * config options for formula search and precursor adducts
        */
        public formula_btn_onclick() {
            settings.closeAll().show("formula_page", () => {
                let pos: string[] = Strings.lineTokens($ts.value("#adducts_pos"));
                let neg: string[] = Strings.lineTokens($ts.value("#adducts_neg"));

                app.desktop.mzkit.SaveAdducts(
                    JSON.stringify(pos),
                    JSON.stringify(neg)
                );
            });
        }

        public profile_btn_onclick() {
            settings.closeAll().show("element_profile_page", () => {

            });
        }

        public add_element_onclick() {
            settings.getElementProfileTable().bootstrapTable('append', [{
                "atom": "",
                "min": 0,
                "max": 0
            }]);
        }

        public molecule_networking_btn_onclick() {
            settings.closeAll().show("molecule_networking_page", () => {

            });
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

            // do config of the settings value
            settings.mzkit_configs.viewer.colorSet = settings.getColorList();

            // do save configuration via proxy
            app.desktop.mzkit
                .Save(JSON.stringify(settings.mzkit_configs))
                .then(async () => {
                    console.log("done!");
                });

            if (!isNullOrUndefined(settings.__dosave)) {
                settings.__dosave();
            }
        }

        public apply_settings_onclick() {
            settings.invoke_save();
        }

        public close_page() {
            app.desktop.mzkit.close();
        }
    }
}