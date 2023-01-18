namespace apps.viewer {

    export interface scatterPoint {
        x: number;
        y: number;
        z: number;

        /**
         * which cluster(color) that current spot it belongs to
        */
        cluster: string;

        /**
         * label id of current spot
        */
        id: string;
    }

    /**
     * #viewer
    */
    export class clusterViewer extends Bootstrap {

        public get appName(): string {
            return "clusterViewer";
        };

        protected init(): void {
            // throw new Error("Method not implemented.");
        }

        public static render3DScatter(dataset: scatterPoint[]) {

        }
    }
}