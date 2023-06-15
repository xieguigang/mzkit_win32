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

    export interface cluster_data {
        cluster: number | string;
        scatter: number[][];
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
            const clusters: cluster_data[] = [];

            for (let group of $from(dataset).GroupBy(a => a.class).ToArray()) {
                const matrix: number[][] = [];
                const id: string = group.Key;

                for (let i of group.ToArray()) {
                    matrix.push([i.x, i.y, i.z]);
                }

                clusters.push({
                    cluster: id,
                    scatter: matrix
                });
            }

            const render = new gl_plot.scatter3d<cluster_data[]>(
                clusterViewer.load_cluster,
                "viewer"
            );

            render.plot(clusters);
        }

        public static load_cluster(data: cluster_data[]): gl_plot.scatter3d_options {
            const paper = echart_app.paper;
            const class_labels = $from(data).Select(r => r.cluster).Distinct().ToArray();
            const numeric_cluster = $from(class_labels).All(si => Strings.isIntegerPattern(si.toString()));
            const scatter3D = $from(data)
                .Select(function (r) {
                    return <gl_plot.gl_scatter_data>{
                        type: 'scatter3D',
                        name: numeric_cluster ? `cluster_${r.cluster}` : r.cluster.toString(),
                        symbolSize: 3,
                        dimensions: [
                            'x',
                            'y',
                            'z'
                        ],
                        data: r.scatter,
                        symbol: 'triangle',
                        itemStyle: {
                            // borderWidth: 0.5,
                            color: paper[class_labels.indexOf(r.cluster.toString())],
                            // borderColor: 'rgba(255,255,255,0.8)'//边框样式
                        }
                    };
                })
                .ToArray();

            return {
                grid3D: {},
                xAxis3D: { type: 'value', name: 'x' },
                yAxis3D: { type: 'value', name: 'y' },
                zAxis3D: { type: 'value', name: 'z' },
                series: scatter3D
            };
        }

    }
}