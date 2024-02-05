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

        private peaks3D: gl_plot.echart_peak3D<ms1_scatter>;

        private create_viewer() {
            this.peaks3D = new gl_plot.echart_peak3D<ms1_scatter>(
                x => x.id,
                x => [x.mz, x.scan_time, x.intensity],
                x => x.intensity,
                'Scan Time(s)', "MZ"
            );
        }

        protected init(): void {
            const vm = this;

            vm.create_viewer();

            app.desktop.mzkit.GetLCMSScatter().then(async function (data) {
                const json_str: string = await data;
                const scatter: ms1_scatter[] = JSON.parse(json_str);

                app.desktop.mzkit.GetColors().then(async function (ls) {
                    const str: string = await ls;
                    const colors: string[] = JSON.parse(str);

                    vm.peaks3D.colors = colors;

                    for (let code of vm.peaks3D.colors) {
                        TypeScript.logging.log(code, code);
                    }

                    if (isNullOrEmpty(scatter)) {
                        vm.render3DScatter([]);
                    } else {
                        vm.render3DScatter(scatter);
                    }
                });
            });
        }

        public render3DScatter(dataset: ms1_scatter[]) {
            const render = new gl_plot.scatter3d<ms1_scatter[]>(
                (ls: any) => this.peaks3D.load_cluster(ls),
                "viewer"
            );
            const div = $ts("#viewer");
            const vm = this;

            // render.chartObj.showLoading();
            render.plot(dataset);
            render.chartObj.on("click", function (par: any) {
                // console.log(par);
                const i = par.dataIndex;
                const category = par.seriesName;
                const labels = vm.peaks3D.layers.Item(category);
                const spot_id: string = labels[i].id;
                // console.log(spot_id);
                // alert(spot_id);

                app.desktop.mzkit.Click(spot_id);
            });
            const resize_canvas = function () {
                const padding = 18;
                div.style.width = (window.innerWidth - padding) + "px";
                div.style.height = (window.innerHeight - padding) + "px";
                render.chartObj.resize();
            };
            window.onresize = () => resize_canvas();
            resize_canvas();
        }
    }
}