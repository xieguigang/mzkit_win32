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
            app.desktop.mzkit.GetFiles(value).then(async function (json) {
                const files = JSON.parse(await json);

                console.log(files);
            });
        }

        public selectFolder_onclick() {
            const vm = this;

            app.desktop.mzkit.SelectFolder().then(async function (dir) {
                dir = await dir;

                if (!Strings.Empty(dir)) {
                    $input("#dir").value = dir;
                    vm.dir_onchange(dir);
                }
            })
        }

        public build_onclick() {
            const vm = this;
            const dir: string = $input("#dir").value.toString();

            console.log(`Build plugin package: ${dir}!`);

            app.desktop.mzkit.BuildPkg(dir).then(async function (flag) {
                flag = await flag;
            });
        }
    }
}