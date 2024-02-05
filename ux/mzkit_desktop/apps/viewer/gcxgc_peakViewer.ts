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
        }
    }
}