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

        private colors: string[];

        protected init(): void {
            const vm = this;

            app.desktop.mzkit.GetLCMSScatter().then(async function (data) {
                const json_str: string = await data;
                const scatter: ms1_scatter[] = JSON.parse(json_str);

                app.desktop.mzkit.GetColors().then(async function (ls) {
                    const str: string = await ls;
                    const colors: string[] = JSON.parse(str);

                    vm.colors = colors;

                    for (let code of vm.colors) {
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

        public render3DScatter(dataset: ms1_scatter[]) {
            const render = new gl_plot.scatter3d<ms1_scatter[]>(
                (ls: any) => this.load_cluster(ls),
                "viewer"
            );
            const div = $ts("#viewer");

            // render.chartObj.showLoading();
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
        private static scatter_group(data: ms1_scatter[], color: string, label: string) {
            // const seq = $from(data);
            // const r_range = globalThis.data.NumericRange.Create(seq.Select(a => a.mz));
            // const g_range = globalThis.data.NumericRange.Create(seq.Select(a => a.scan_time));
            // const b_range = globalThis.data.NumericRange.Create(seq.Select(a => a.intensity));
            // const byte_range = new globalThis.data.NumericRange(0, 255);

            return <gl_plot.gl_scatter_data>{
                type: 'bar3D',
                shading: 'color',  // color, lambert, realistic
                barSize: 0.1,
                name: `Intensity ${label}`, // format_tag(r),
                spot_labels: $from(data).Select(r => r.id).ToArray(),
                symbolSize: 1,
                dimensions: [
                    'Scan Time(s)',
                    'M/Z',
                    'Intensity'
                ],
                data: $from(data).Select(r => [r.scan_time, r.mz, r.intensity]).ToArray(),
                symbol: 'circle',
                itemStyle: {
                    // borderWidth: 0.5,
                    // color: <any>function (params) {
                    //     var i: number = params.dataIndex;
                    //     var p = data[i];
                    //     var r = r_range.ScaleMapping(p.mz, byte_range);
                    //     var g = g_range.ScaleMapping(p.scan_time, byte_range);
                    //     var b = b_range.ScaleMapping(p.intensity, byte_range);

                    //     return 'rgb(' + [r, g, b].join(',') + ')';
                    // }
                    color: color
                },
                wireframe: {
                    show: false
                }
            };
        }
        public load_cluster(data: ms1_scatter[]): gl_plot.scatter3d_options {
            const seq = $from(data);
            const max = seq.Select(a => a.intensity).Max();
            const d = max / this.colors.length;
            // // const class_labels = $from(data).Select(r => r.cluster).Distinct().ToArray();
            // // const numeric_cluster = $from(class_labels).All(si => Strings.isIntegerPattern(si.toString()));
            // // const format_tag = clusterViewer.format_cluster_tag(data);
            // // const scatter3D = $from(data)
            // //     .Select(function (r) {
            // //     })
            // //     .ToArray();
            // // const spot_labels = $from(data).ToDictionary(d => format_tag(d), d => d.labels);
            const scatter3D = [];
            let i = 0;
            for (let min = 0; min < max; min = min + d) {
                const l0 = min + d;
                const subset = seq.Where(a => a.intensity > min && a.intensity < l0).ToArray();
                const color = this.colors[i++];
                const label = `${min.toExponential(1)} ~ ${l0.toExponential(1)}`;

                scatter3D.push(LCMSScatterViewer.scatter_group(subset, color, label));
            }

            // const scatter3D = [LCMSScatterViewer.scatter_group(data)];
            return <any>{
                grid3D: {
                    axisPointer: {
                        show: false
                    },
                    viewControl: {
                        distance: 300,
                        beta: -30,
                        panMouseButton: 'right',//平移操作使用的鼠标按键
                        rotateMouseButton: 'left',//旋转操作使用的鼠标按键
                        alpha: 50 // 让canvas在x轴有一定的倾斜角度
                    },
                    postEffect: {
                        enable: false,
                        SSAO: {//环境光遮蔽
                            radius: 1,//环境光遮蔽的采样半径。半径越大效果越自然
                            intensity: 1,//环境光遮蔽的强度
                            enable: false
                        }
                    },
                    temporalSuperSampling: {//分帧超采样。在开启 postEffect 后，WebGL 默认的 MSAA 会无法使用,分帧超采样用来解决锯齿的问题
                        enable: false
                    },
                    boxDepth: 120
                    // light: {
                    //     main: {
                    //         shadow: false,
                    //         intensity: 10
                    //     },
                    //     ambientCubemap: {
                    //         texture: "/assets/canyon.hdr",
                    //         exposure: 2,
                    //         diffuseIntensity: 0.2,
                    //         specularIntensity: 1
                    //     },
                    //     enable: false
                    // }
                },
                backgroundColor: 'black',
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
                        color: 'darkblue',
                        fontStyle: 'normal',
                        fontWeight: 'normal',
                        fontFamily: 'sans-serif',
                        fontSize: 12,
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
                        return `<${labels[i].id}> m/z: ${mz}@${rt}s intensity=${into}`;
                    }
                },
                // visualMap: {
                //     max: max,
                //     inRange: {
                //         color: this.colors
                //     }
                // }
                // legend: {
                //     orient: 'vertical',
                //     x: 'right',
                //     y: 'center'
                // }
            };
        }
    }
}