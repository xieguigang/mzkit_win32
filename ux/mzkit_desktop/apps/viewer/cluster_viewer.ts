namespace apps.viewer {

    /**
     * UMAPPoint
    */
    export interface scatterPoint {
        x: number;
        y: number;
        z: number;

        /**
         * which cluster(color) that current spot it belongs to
        */
        class: string;

        /**
         * label id of current spot
        */
        label: string;
    }

    /**
     * #viewer
    */
    export class clusterViewer extends Bootstrap {

        public get appName(): string {
            return "clusterViewer";
        };

        protected init(): void {
            app.desktop.mzkit.GetScatter()
                .then(async function (data) {
                    const json: string = await data;
                    const scatter: scatterPoint[] = JSON.parse(json);

                    if (isNullOrEmpty(scatter)) {
                        clusterViewer.render3DScatter([]);
                    } else {
                        clusterViewer.render3DScatter(scatter);
                    }
                });
        }

        public static render3DScatter(dataset: scatterPoint[]) {
            const clusters = $from(dataset).GroupBy(a => a.class);
        }
    }
}