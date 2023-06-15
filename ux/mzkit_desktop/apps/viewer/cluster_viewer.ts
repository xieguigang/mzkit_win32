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
                series: scatter3D,
                tooltip: {//提示框组件，用于配置鼠标滑过或点击图表时的显示框。
                    show: true, // 是否显示
                    trigger: 'item', // 触发类型  'item'图形触发：散点图，饼图等无类目轴的图表中使用； 'axis'坐标轴触发；'none'：什么都不触发。
                    axisPointer: { // 坐标轴指示器配置项。
                        type: 'cross', // 'line' 直线指示器  'shadow' 阴影指示器  'none' 无指示器  'cross' 十字准星指示器。
                    },
                    // showContent: true, //是否显示提示框浮层，默认显示。
                    // triggerOn: 'mouseover', // 触发时机'click'鼠标点击时触发。 
                    backgroundColor: 'rgba(50,50,50,0.7)', // 提示框浮层的背景颜色。
                    borderColor: '#333', // 提示框浮层的边框颜色。
                    borderWidth: 0, // 提示框浮层的边框宽。
                    padding: 5, // 提示框浮层内边距，
                    textStyle: { // 提示框浮层的文本样式。
                        color: '#fff',
                        fontStyle: 'normal',
                        fontWeight: 'normal',
                        fontFamily: 'sans-serif',
                        fontSize: 14,
                    },
                    // 提示框浮层内容格式器，支持字符串模板和回调函数两种形式。
                    // 模板变量有 {a}, {b}，{c}，分别表示系列名，数据名，数据值等
                    // formatter: '{a}--{b} 的成绩是 {c}'
                    formatter: function (arg) {
                        return JSON.stringify(arg);
                    }
                }
            };
        }

    }
}