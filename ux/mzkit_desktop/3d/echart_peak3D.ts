namespace gl_plot {

    /**
     * get id
    */
    export interface IdReader<T> { (x: T): string; }
    /**
     * get scatter point:
     * 
     * lcms scatter: mz, rt, intensity
     * gcxgc peaks: rt1, rt2, intensity
    */
    export interface PointReader<T> { (x: T): number[]; }
    export interface IntensityReader<T> { (x: T): number; }
    export interface LabelReader<T> { (args: { dataIndex: number, data: number[] }, data: T[]): string; }

    export class echart_peak3D<T> {

        public colors: string[];
        public layers: Dictionary<T[]>;

        constructor(
            private read_id: IdReader<T>,
            private read_point: PointReader<T>,
            private read_intensity: IntensityReader<T>,
            private read_label: LabelReader<T>,
            private xlab: string,
            private ylab: string,
            private zlab: string = "Intensity") {

            this.layers = new Dictionary<T[]>();
        }

        private scatter_group(data: T[], color: string, label: string) {
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
                spot_labels: $from(data).Select(r => this.read_id(r)).ToArray(),
                symbolSize: 1,
                dimensions: [
                    this.xlab,
                    this.ylab,
                    this.zlab
                ],
                data: $from(data).Select(r => this.read_point(r)).ToArray(),
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

        public load_cluster(data: T[]): gl_plot.scatter3d_options {
            const seq = $from(data);
            const max = seq.Select(a => this.read_intensity(a)).Max();
            const d = max / this.colors.length;
            // // const class_labels = $from(data).Select(r => r.cluster).Distinct().ToArray();
            // // const numeric_cluster = $from(class_labels).All(si => Strings.isIntegerPattern(si.toString()));
            // // const format_tag = clusterViewer.format_cluster_tag(data);
            // // const scatter3D = $from(data)
            // //     .Select(function (r) {
            // //     })
            // //     .ToArray();
            // // const spot_labels = $from(data).ToDictionary(d => format_tag(d), d => d.labels);
            let scatter3D = [];
            let i = 0;

            for (let min = 0; min < max; min = min + d) {
                const l0 = min + d;
                const subset = seq.Where(a => {
                    let into = this.read_intensity(a);
                    return into > min && into < l0
                }).ToArray();
                const color = this.colors[i++];
                const label = `${min.toExponential(1)} ~ ${l0.toExponential(1)}`;

                this.layers.Add(`Intensity ${label}`, subset);
                scatter3D.push(this.scatter_group(subset, color, label));
            }

            // const scatter3D = [LCMSScatterViewer.scatter_group(data)];
            return <any>{
                grid3D: {
                    color: "white",
                    axisPointer: {
                        show: false
                    },
                    viewControl: {
                        distance: 200,
                        beta: -20,
                        panMouseButton: 'right',//平移操作使用的鼠标按键
                        rotateMouseButton: 'left',//旋转操作使用的鼠标按键
                        alpha: 30 // 让canvas在x轴有一定的倾斜角度
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
                    boxDepth: 100
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
                backgroundColor: '#e7e7e7',
                xAxis3D: { type: 'value', name: this.xlab, color: "white" },
                yAxis3D: { type: 'value', name: this.ylab, color: "white" },
                zAxis3D: {
                    type: 'value', name: this.zlab, color: "white",
                    axisLabel: {
                        formatter: value => this.format_axisLabel(value)
                    }
                },
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
                    formatter: (arg) => this.read_label(arg, data)
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

        private format_axisLabel(value: string) {
            var res = value.toString();
            var numN1 = 0;
            var numN2 = 1;
            var num1 = 0;
            var num2 = 0;
            var t1 = 1;
            for (var k = 0; k < res.length; k++) {
                if (res[k] == ".")
                    t1 = 0;
                if (t1)
                    num1++;
                else
                    num2++;
            }

            if (Math.abs(parseFloat(res)) < 1 && res.length > 4) {
                for (var i = 2; i < res.length; i++) {
                    if (res[i] == "0") {
                        numN2++;
                    } else if (res[i] == ".")
                        continue;
                    else
                        break;
                }
                var v = parseFloat(res);
                v = v * Math.pow(10, numN2);
                return v.toString() + "e-" + numN2;
            } else if (num1 > 4) {
                if (res[0] == "-")
                    numN1 = num1 - 2;
                else
                    numN1 = num1 - 1;
                var v = parseFloat(res);
                v = v / Math.pow(10, numN1);
                if (num2 > 4)
                    v = <any>v.toFixed(4);
                return v.toString() + "e" + numN1;
            } else
                return parseFloat(res);
        }
    }
}