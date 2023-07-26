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

        protected init(): void {
            app.desktop.mzkit.GetLCMSScatter().then(async function (data) {
                const json_str: string = await data;
                const scatter: ms1_scatter[] = JSON.parse(json_str);

                if (isNullOrEmpty(scatter)) {
                    LCMSScatterViewer.render3DScatter([]);
                } else {
                    LCMSScatterViewer.render3DScatter(scatter);
                }
            });
        }

        public static render3DScatter(dataset: ms1_scatter[]) {
            const render = new gl_plot.scatter3d<ms1_scatter[]>(
                LCMSScatterViewer.load_cluster,
                "viewer"
            );

            const div = $ts("#viewer");

            render.plot(dataset);
            render.chartObj.on("click", function (par: any) {
                // // console.log(par);

                // const i = par.dataIndex;
                // const category = par.seriesName;
                // const labels = spot_labels.Item(category);
                // const spot_id: string = labels[i];

                // // console.log(spot_id);
                // app.desktop.mzkit.Click(spot_id);
            });

            const resize_canvas = function () {
                const padding = 25;

                div.style.width = (window.innerWidth - padding) + "px";
                div.style.height = (window.innerHeight - padding) + "px";

                render.chartObj.resize();
            };

            window.onresize = () => resize_canvas();
            resize_canvas();
        }

        private static scatter_group(data: ms1_scatter[]) {
            // if (data.length > 4) {
            //     let scatter = $from(data).OrderByDescending(r => r.intensity + 0.0).ToArray();
            //     let max_into = scatter[3].intensity;

            //     for (let i = 0; i < 3; ++i) {
            //         scatter[i].intensity = max_into;
            //     }

            //     data = scatter;
            // }

            return <gl_plot.gl_scatter_data>{
                type: 'scatter3D',
                name: "LCMS Ms2 Spectrum", // format_tag(r),
                spot_labels: $from(data).Select(r => r.id).ToArray(),
                symbolSize: 5,
                dimensions: [
                    'Scan Time(s)',
                    'M/Z',
                    'Intensity'
                ],
                data: $from(data).Select(r => [r.scan_time, r.mz, r.intensity]).ToArray(),
                symbol: 'circle',
                itemStyle: {
                    // borderWidth: 0.5,
                    color: "red"// paper[class_labels.indexOf(r.cluster.toString())],
                    // borderColor: 'rgba(255,255,255,0.8)'//边框样式
                }
            };
        }

        public static load_cluster(data: ms1_scatter[]): gl_plot.scatter3d_options {
            const paper = echart_app.paper;
            // const class_labels = $from(data).Select(r => r.cluster).Distinct().ToArray();
            // const numeric_cluster = $from(class_labels).All(si => Strings.isIntegerPattern(si.toString()));
            // const format_tag = clusterViewer.format_cluster_tag(data);
            // const scatter3D = $from(data)
            //     .Select(function (r) {

            //     })
            //     .ToArray();
            // const spot_labels = $from(data).ToDictionary(d => format_tag(d), d => d.labels);
            const scatter3D = [LCMSScatterViewer.scatter_group(data)];

            return <any>{
                grid3D: {},
                xAxis3D: { type: 'value', name: 'Scan Time(s)' },
                yAxis3D: { type: 'value', name: 'M/Z' },
                zAxis3D: { type: 'value', name: 'Intensity' },
                series: scatter3D,
                tooltip: {//提示框组件，用于配置鼠标滑过或点击图表时的显示框。
                    show: true, // 是否显示
                    trigger: 'item', // 触发类型  'item'图形触发：散点图，饼图等无类目轴的图表中使用； 'axis'坐标轴触发；'none'：什么都不触发。
                    axisPointer: { // 坐标轴指示器配置项。
                        type: 'cross', // 'line' 直线指示器  'shadow' 阴影指示器  'none' 无指示器  'cross' 十字准星指示器。
                    },
                    // showContent: true, //是否显示提示框浮层，默认显示。
                    // triggerOn: 'mouseover', // 触发时机'click'鼠标点击时触发。 
                    backgroundColor: 'white', // 提示框浮层的背景颜色。
                    borderColor: '#333', // 提示框浮层的边框颜色。
                    borderWidth: 0, // 提示框浮层的边框宽。
                    padding: 5, // 提示框浮层内边距，
                    textStyle: { // 提示框浮层的文本样式。
                        color: 'skyblue',
                        fontStyle: 'normal',
                        fontWeight: 'normal',
                        fontFamily: 'sans-serif',
                        fontSize: 16,
                    },
                    // 提示框浮层内容格式器，支持字符串模板和回调函数两种形式。
                    // 模板变量有 {a}, {b}，{c}，分别表示系列名，数据名，数据值等
                    // formatter: '{a}--{b} 的成绩是 {c}'
                    formatter: function (arg) {
                        // console.log(arg);

                        const i = arg.dataIndex;
                        const labels = data;// spot_labels.Item(arg.seriesName);
                        const ms1: number[] = arg.data;
                        const rt = Math.round(ms1[0]);
                        const mz = Strings.round(ms1[1]);
                        const into = ms1[2].toExponential(2); // Math.pow(1.125, ms1[2]).toExponential(2);

                        return `${arg.seriesName} spot:<${labels[i].id}> m/z: ${mz}@${rt}s intensity=${into}`;
                    }
                },
                legend: {
                    orient: 'vertical',
                    x: 'right',
                    y: 'center'
                }
            };
        }
    }
}