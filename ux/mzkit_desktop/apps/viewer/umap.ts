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
        }

        public knn_onchange(val: string) {
            console.log(val);
        }

        public run_umap_onclick() {
            
        }
    }
}