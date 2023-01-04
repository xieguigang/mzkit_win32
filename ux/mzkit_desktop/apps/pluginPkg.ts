namespace apps {

    export class pluginPkg extends Bootstrap {

        public get appName(): string {
            return "pluginPkg";
        };

        protected init(): void {
            // throw new Error("Method not implemented.");
        }

        public dir_onchange(value: string) {
            console.log(value);
        }

        public selectFolder_onclick() {
            const vm = this;

            app.desktop.mzkit.SelectFolder().then(async function (dir) {
                dir = await dir;

                if (!Strings.Empty(dir)) {
                    vm.dir_onchange(dir);
                }
            })
        }
    }
}