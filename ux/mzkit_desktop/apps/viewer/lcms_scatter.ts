namespace apps.viewer {

    export interface ms1_scatter {
        id: string;
        mz: number;
        scan_time: number;
        intensity: number;
    }

    export class LCMSScatterViewer extends Bootstrap {

        public get appName(): string {
            return "lcms-scatter";
        }

        protected init(): void {
            const vm = this;

            app.desktop.mzkit.GetLCMSScatter().then(async function (data) {
                const json_str: string = await data;
                const scatter: ms1_scatter[] = JSON.parse(json_str);

                if (isNullOrEmpty(scatter)) {
                    vm.render3DScatter([]);
                } else {
                    vm.render3DScatter(scatter);
                }
            });
        }

        public render3DScatter(dataset: ms1_scatter[]) {
            
           
        }       
    }
}