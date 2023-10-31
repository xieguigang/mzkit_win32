namespace apps.viewer {

    export class lcmsLibrary extends Bootstrap {

        public get appName(): string {
            return "lcms-library";
        }

        private libfiles: {};

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
                                } else {
                                    console.log("Error while trying to open the LCMS library file!");
                                }
                            });
                    }
                }
            };

            return items;
        }
    }
}