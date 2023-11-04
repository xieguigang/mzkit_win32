namespace apps.viewer {

    export class umap extends Bootstrap {

        public get appName(): string {
            return "umap";
        }

        protected init(): void {
            app.desktop.mzkit.GetMatrixDims()
                .then(async function (json) {
                    let str = await json;
                    let ints: number[] = JSON.parse(str);

                    $ts("#nfeatures").display(ints[0].toString());
                    $ts("#nsamples").display(ints[1].toString());
                });

            this.loadUMAP();
        }

        public knn_onchange(val: string) {
            $ts("#knn-value").display(val);
        }

        public KnnIter_onchange(val: string) {
            $ts("#knnItr-value").display(val);
        }

        public localConnectivity_onchange(val: string) {
            $ts("#localConnect-value").display(val);
        }

        public bandwidth_onchange(val: string) {
            $ts("#bandwidth-value").display(val);
        }

        public learningRate_onchange(val: string) {
            $ts("#learningRate-value").display(val);
        }

        public run_umap_onclick() {
            const vm = this;
            app.desktop.mzkit.Run(
                parseInt($ts.value("#knn").toString()),
                parseInt($ts.value("#KnnIter").toString()),
                parseFloat($ts.value("#localConnectivity").toString()),
                parseFloat($ts.value("#bandwidth").toString()),
                parseFloat($ts.value("#learningRate").toString()),
                parseBoolean($ts.value("#spectral_cos").toString())
            ).then(async function (b) {
                const flag = await b;

                if (flag) {
                    vm.loadUMAP();
                } else {

                }
            });
        }

        private loadUMAP() {
            app.desktop.mzkit.GetScatter()
                .then(async function (str) {
                    const json: string = await str;
                    const scatter: scatterPoint[] = JSON.parse(json);

                    if (isNullOrEmpty(scatter)) {
                        clusterViewer.render3DScatter([]);
                    } else {
                        clusterViewer.render3DScatter(scatter);
                    }
                });
        }
    }
}