namespace apps.viewer {

    export class lcmsLibrary extends Bootstrap {

        public get appName(): string {
            return "lcms-library";
        }

        private libfiles: {};
        private page: number = 1;
        private page_size: number = 100;

        protected init(): void {
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
                openItem: { // The "delete" menu item
                    label: "Open",
                    action: function (a) {
                        let n: HTMLElement = a.reference[0];
                        let key = Strings.Trim(n.innerText);
                        let filepath = vm.libfiles[key];

                        console.log("open a libfile:");
                        console.log(a);
                        console.log(key);
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
                }
            };

            return items;
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

        private show_page(list: MetaLib[]) {
            const list_page = $ts("#list-page").clear();

            console.log("get page data:");
            console.log(list);

            for (let meta of list) {
                list_page.appendElement($ts("<div>").display(`
                <h5>${meta.name} [<a>${meta.ID}</a>]</h5>
                <canvas class="smiles-viewer" id="${meta.ID.replace(".", "_").replace(" ", "_")}" width="200" height="150" data="${this.get_smiles(meta)}"></canvas>
                <p>
                <span>Formula: </span> ${meta.formula} <br />
                <span>Exact Mass: </span> ${meta.exact_mass} <br />                       
                </p>
                <p>${meta.description}</p>
                `));
            }

            let options = {};
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
}