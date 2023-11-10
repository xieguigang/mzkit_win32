namespace apps.viewer {

    const SmilesDrawer = (<any>window).SmilesDrawer;

    export class lcmsLibrary extends Bootstrap {

        public get appName(): string {
            return "lcms-library";
        }

        private libfiles: {};
        private page: number = 1;
        private page_size: number = 100;

        protected init(): void {
            // this.reloadLibs();
            try {
                this.list_data();
            } catch (ex) {

            }
        }

        private reloadLibs() {
            const vm = this;

            app.desktop.mzkit.ScanLibraries()
                .then(async function (str) {
                    let pull_str: string = await str;
                    let list: string[] = JSON.parse(pull_str);

                    vm.libfiles = {};

                    for (let file of list) {
                        let name: string | string[] = file.split(/[\\/]/ig);

                        name = name[name.length - 1];
                        vm.libfiles[$ts.baseName(name)] = file;
                    }

                    console.log("get lcms-library files:");
                    console.table(vm.libfiles);

                    vm.loadfiles();
                });
        }

        private loadfiles() {
            const div: any = $('#lcms-libfiles');
            const root_dir = {
                'text': 'LCMS Library',
                'state': {
                    'opened': true,
                    'selected': true
                },
                'children': [
                ]
            }
            const libfiles = [];

            for (let key of Object.keys(this.libfiles)) {
                libfiles.push({
                    text: key
                });
            }

            root_dir.children = libfiles;
            div.empty();
            div.jstree({
                'core': {
                    "animation": 0,
                    "check_callback": true,
                    'data': [root_dir]
                },
                "plugins": [
                    "contextmenu", "dnd", "search",
                    "state", "types", "wholerow"
                ],
                "contextmenu": { items: node => this.customMenu(node) }
            });
        }

        private customMenu(node) {
            const vm = this;

            // The default set of all items
            var items = {
                openItem: this.menu_open(),
                newItem: this.menu_new()
            };

            return items;
        }

        private menu_new() {
            const vm = this;

            return {
                label: "New Library",
                action: function (a) {
                    console.log("start to create a new library file!");

                    app.desktop.mzkit.NewLibrary()
                        .then(async function (b) {
                            const flag = await b;

                            if (flag) {
                                // reload the library list
                                // vm.reloadLibs();
                                location.reload();
                            }
                        })
                }
            }
        }

        private menu_open() {
            const vm = this;

            return { // The "delete" menu item
                label: "Open",
                action: function (a) {
                    let n: HTMLElement = a.reference[0];
                    let key = Strings.Trim(n.innerText);
                    let filepath = vm.libfiles[key];

                    lcmsLibrary.openLibfile(filepath, vm);
                }
            }
        }

        public static openLibfile(filepath: string, vm: lcmsLibrary = null) {
            if ((!vm) || typeof vm == "string") {
                vm = <lcmsLibrary>Router.currentAppPage();
            }

            console.log("open a libfile:");
            // console.log(a);
            // console.log(key);
            console.log(filepath);

            app.desktop.mzkit.OpenLibrary(filepath)
                .then(async function (b) {
                    let check = await b;

                    if (check) {
                        console.log("Open library file success!");
                        vm.list_data();
                    } else {
                        console.log("Error while trying to open the LCMS library file!");
                    }
                });
        }

        private list_data() {
            const vm = this;

            app.desktop.mzkit.GetPage(this.page, this.page_size)
                .then(async function (str) {
                    let json: string = await str;
                    let list: MetaLib[] = JSON.parse(json);

                    vm.show_page(list);
                });
        }

        /**
         * .lib-id
        */
        private hookSpectralLinkOpen(liblinkClass: string, libsearchClass: string) {
            $ts.select("." + liblinkClass).onClick(function (a) {
                const data_id: string = a.getAttribute("data_id");

                app.desktop.mzkit
                    .ShowSpectral(data_id)
                    .then(async function (b) {
                        const flag = await b;

                        if (flag) {

                        } else {

                        }
                    })
            });
            $ts.select("." + libsearchClass).onClick(function (a) {
                const data_id: string = a.getAttribute("data_id");

                app.desktop.mzkit
                    .AlignSpectral(data_id)
                    .then(async function (b) {
                        const flag = await b;

                        if (flag) {

                        } else {

                        }
                    })
            });
        }

        private show_page(list: MetaLib[]) {
            const list_page = $ts("#list-page").clear();
            const liblinkClass: string = "lib-id";
            const libsearchClass: string = "lib-query";

            console.log("get page data:");
            console.log(list);

            for (let meta of list) {
                let xrefs = "";
                let xref_data = meta.xref || {};

                for (let key of xref_keys) {
                    let val: string[] | string = xref_data[key] || "";

                    if (Array.isArray(val)) {
                        val = val.join("; ");
                    }

                    if (!Strings.Empty(val, true)) {
                        // if (key == "SMILES" || key == "InChIkey" || key == "InChI") {
                        //     val = `<pre><code>${val}</code></pre>`;
                        // }
                        xrefs = xrefs + `<span>${key}: </span> ${val} <br />`;
                    }
                }

                list_page.appendElement($ts("<div class='row'>").display(`
                <div class="span4">
                    <h5>${meta.name} [
                        <a class="${liblinkClass}" href="#" onclick="javascript:void(0);" data_id="${meta.ID}">${meta.ID}</a> 
                        <a href="#" class="fa-solid fa-magnifying-glass ${libsearchClass}" data_id="${meta.ID}" onclick="javascript:void(0);"></a>
                    ]</h5>
                    <p>
                    <span>Formula: </span> ${meta.formula} <br />
                    <span>Exact Mass: </span> ${meta.exact_mass} <br />                       
                    </p>
                    <p>${meta.description}</p>
                </div>
                <div class="span4">
                    <p>
                    ${xrefs}
                    </p>
                </div>
                <div class="span4">
                    <canvas class="smiles-viewer" id="${meta.ID.replace(".", "_").replace(" ", "_")}" width="200" height="150" data="${this.get_smiles(meta)}">
                    </canvas>
                </div>
                `));
            }

            this.hookSpectralLinkOpen(liblinkClass, libsearchClass);

            let options = {
                width: 200,
                height: 150
            };
            // Initialize the drawer to draw to canvas
            let smilesDrawer = new SmilesDrawer.Drawer(options);
            // Alternatively, initialize the SVG drawer:
            // let svgDrawer = new SmilesDrawer.SvgDrawer(options);

            $ts.select(".smiles-viewer")
                .ForEach(function (a) {
                    let input_value = a.getAttribute("data");
                    let id = a.getAttribute("id");

                    if (!Strings.Empty(input_value, true)) {
                        // Clean the input (remove unrecognized characters, such as spaces and tabs) and parse it
                        SmilesDrawer.parse(input_value, function (tree) {
                            // Draw to the canvas
                            smilesDrawer.draw(tree, id, "light", false);
                            // Alternatively, draw to SVG:
                            // svgDrawer.draw(tree, 'output-svg', 'dark', false);
                        });
                    }
                });
        }

        private get_smiles(meta: MetaLib) {
            if (meta.xref) {
                return meta.xref.SMILES || null;
            } else {
                return null;
            }
        }

        public query_onclick() {
            const q: string = $ts.value("#get-query");
            const vm = this;

            app.desktop.mzkit.Query(q)
                .then(async function (str) {
                    let json: string = await str;
                    let list: MetaLib[] = JSON.parse(json);

                    vm.show_page(list);
                });
        }
    }

    export interface MetaLib {
        ID: string;
        formula: string;
        exact_mass: number;

        name: string;
        IUPACName: string;
        description: string;
        synonym: string[];

        xref: xref
    }

    export interface xref {
        chebi: string;
        KEGG: string;
        pubchem: string;
        HMDB: string;
        metlin: string;
        DrugBank: string;
        ChEMBL: string;
        Wikipedia: string;
        lipidmaps: string;
        MeSH: string;
        ChemIDplus: string;
        MetaCyc: string;
        KNApSAcK: string;
        CAS: string[];
        InChIkey: string;
        InChI: string;
        SMILES: string;
    }

    const xref_keys: string[] = ["chebi",
        "KEGG",
        "pubchem",
        "HMDB",
        "metlin",
        "DrugBank",
        "ChEMBL",
        "Wikipedia",
        "lipidmaps",
        "MeSH",
        "ChemIDplus",
        "MetaCyc",
        "KNApSAcK",
        "CAS",
        "InChIkey",
        "InChI",
        "SMILES"];
}