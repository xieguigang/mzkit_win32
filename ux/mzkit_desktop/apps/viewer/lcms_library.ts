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
                });
        }

    }
}