namespace apps.viewer {

    export interface gcxgc_peak {
        t1: number;
        t2: number;
        into: number;
    }

    export class GCxGCPeaksViewer extends Bootstrap {

        get appName(): string {
            return "gcxgc-peaks";
        }

        private peaks3D: gl_plot.echart_peak3D<gcxgc_peak>;

        private create_viewer() {
            this.peaks3D = new gl_plot.echart_peak3D<gcxgc_peak>(
                (x) => null,
                (x) => [x.t1, x.t2, x.into],
                (x) => x.into,
                (arg, x) => this.label(arg, x),
                'Time Dimension1(min)', "Time Dimension2(s)"
            );

            return this;
        }

        private label(arg: { dataIndex: number, data: number[] }, data: gcxgc_peak[]) {
            // console.log(arg);
            const i = arg.dataIndex;
            const labels = data;// spot_labels.Item(arg.seriesName);
            const ms1: number[] = arg.data;
            const rt = Math.round(ms1[0]);
            const mz = Strings.round(ms1[1]);
            const into = ms1[2].toExponential(2); // Math.pow(1.125, ms1[2]).toExponential(2);

            return `GCxGC: ${mz}@${rt}s intensity=${into}`;
        }

        protected init(): void {
            const vm = this.create_viewer();

            app.desktop.mzkit.GetLCMSScatter().then(async function (data) {
                const json_str: string = await data;
                const scatter: gcxgc_peak[] = JSON.parse(json_str);

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

        public render3DScatter(dataset: gcxgc_peak[]) {
            const render = new gl_plot.scatter3d<gcxgc_peak[]>(
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
                // const spot_id: string = labels[i].id;
                // console.log(spot_id);
                // alert(spot_id);

                // app.desktop.mzkit.Click(spot_id);
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