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
            this.selectMethod("kmeans");
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

        public kmeans_onchange(val: string) {
            $ts("#kmeans-value").display(val);
        }

        public min_pts_onchange(val: string) {
            $ts("#min_pts-value").display(val);
        }

        public eps_onchange(val: string) {
            $ts("#eps-value").display(val);
        }

        public identical_onchange(val: string) {
            $ts("#identical-value").display(val);
        }

        public run_umap_onclick() {
            const vm = this;

            vm.showSpinner();
            $goto("#spinner");
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

                vm.hideSpinner();
            });
        }

        public run_kmeans_onclick() {
            const vm = this;

            vm.showSpinner();
            $goto("#spinner");
            app.desktop.mzkit
                .RunKmeans(parseInt($ts.value("#kmeans").toString()))
                .then(async function (b) {
                    const flag = await b;

                    if (flag) {
                        vm.loadUMAP();
                    }

                    vm.hideSpinner();
                });
        }

        public run_graph_onclick() {
            const vm = this;

            vm.showSpinner();
            $goto("#spinner");
            app.desktop.mzkit
                .RunGraph(parseFloat($ts.value("#identical").toString()))
                .then(async function (b) {
                    const flag = await b;

                    if (flag) {
                        vm.loadUMAP();
                    }

                    vm.hideSpinner();
                });
        }

        public run_dbscan_onclick() {
            const vm = this;

            vm.showSpinner();
            $goto("#spinner");
            app.desktop.mzkit
                .RunDbScan(
                    parseInt($ts.value("#min_pts").toString()),
                    parseFloat($ts.value("#eps").toString())
                )
                .then(async function (b) {
                    const flag = await b;

                    if (flag) {
                        vm.loadUMAP();
                    }

                    vm.hideSpinner();
                });
        }

        showSpinner() {
            document.getElementById('spinner')
                .style.display = 'block';

            $ts("#manifold").interactive(false);
            $ts("#clustering").interactive(false);
        }

        hideSpinner() {
            document.getElementById('spinner')
                .style.display = 'none';

            $ts("#manifold").interactive(true);
            $ts("#clustering").interactive(true);
        }

        public download_onclick() {
            app.desktop.mzkit.Download().then(async function (str) {
                const csv: string = await str;
                const data: DataURI = <DataURI>{
                    mime_type: "plain/text",
                    data: Base64.encode(csv)
                }

                if (!Strings.Empty(csv, true)) {
                    DOM.download("umap.csv", data, false);
                }
            });
        }

        public save_onclick() {
            const vm = this;

            this.showSpinner();
            app.desktop.mzkit.Save().then(async function () {
                console.log("done!");
                vm.hideSpinner();
            });
        }

        private loadUMAP() {
            app.desktop.mzkit.GetUMAPFile()
                .then(async function (str) {
                    const filepath: string = await str;
                    console.log(filepath);
                });
            app.desktop.mzkit.GetScatter()
                .then(async function (str) {
                    const json: string = await str;
                    const scatter: scatterPoint[] = JSON.parse(json);

                    if (isNullOrEmpty(scatter)) {
                        clusterViewer.render3DScatter([], false);
                    } else {
                        clusterViewer.render3DScatter(scatter, false);
                    }
                });
        }

        public kmeans_method_onchange(val: string) {
            this.selectMethod($ts.select.getOption(".select-method"));
        }

        public dbscan_method_onchange(val: string) {
            this.selectMethod($ts.select.getOption(".select-method"));
        }

        public graph_method_onchange(val: string) {
            this.selectMethod($ts.select.getOption(".select-method"));
        }

        private selectMethod(method: string) {
            for (let id of ["kmean-card", "dbscan-card", "graph-card"]) {
                $ts(`#${id}`).interactive(false);
            }

            switch (method) {
                case "kmeans": $ts("#kmean-card").interactive(true); break;
                case "dbscan": $ts("#dbscan-card").interactive(true); break;
                case "graph": $ts("#graph-card").interactive(true); break;
            }
        }
    }
}