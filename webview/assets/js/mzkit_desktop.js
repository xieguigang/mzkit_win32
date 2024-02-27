var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (g && (g = 0, op[0] && (_ = 0)), _) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var __spreadArray = (this && this.__spreadArray) || function (to, from, pack) {
    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
        if (ar || !(i in from)) {
            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
            ar[i] = from[i];
        }
    }
    return to.concat(ar || Array.prototype.slice.call(from));
};
/// <reference path="../../d/three/index.d.ts" />
var apps;
(function (apps) {
    var viewer;
    (function (viewer) {
        var window = globalThis.window;
        viewer.Empty = Object.freeze([]);
        var cmtextures = ["gray", "viridis", "jet", "rainbow", "typhoon", "magma", "plasma", "mako", "rocket", "turbo"];
        function cm_names() {
            var names = {};
            for (var _i = 0, cmtextures_1 = cmtextures; _i < cmtextures_1.length; _i++) {
                var name_1 = cmtextures_1[_i];
                names[name_1] = name_1;
            }
            return names;
        }
        viewer.cm_names = cm_names;
        function cm_textures(callback) {
            var textures = {};
            for (var _i = 0, cmtextures_2 = cmtextures; _i < cmtextures_2.length; _i++) {
                var name_2 = cmtextures_2[_i];
                textures[name_2] = new THREE.TextureLoader().load("/vendor/three/textures/cm_".concat(name_2, ".png"), function () { return callback(); });
            }
            return textures;
        }
        viewer.cm_textures = cm_textures;
        var three_app = /** @class */ (function (_super) {
            __extends(three_app, _super);
            function three_app() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            Object.defineProperty(three_app.prototype, "appName", {
                get: function () {
                    return "three-3d";
                },
                enumerable: false,
                configurable: true
            });
            three_app.prototype.init = function () {
                var _this = this;
                var scene = new THREE.Scene();
                var globalPlane = new THREE.Plane(new THREE.Vector3(-1, 0, 0), 100);
                var globalPlanes = [globalPlane];
                var vm = this;
                // Create renderer
                var renderer = new THREE.WebGLRenderer({ antialias: false });
                renderer.setPixelRatio(window.devicePixelRatio);
                renderer.setSize(window.innerWidth, window.innerHeight);
                renderer.localClippingEnabled = true;
                // GUI sets it to globalPlanes
                renderer.clippingPlanes = globalPlanes;
                document.body.appendChild(renderer.domElement);
                console.log(renderer);
                // Create camera (The volume renderer does not work very well with perspective yet)
                var h = 512; // frustum height
                var aspect = window.innerWidth / window.innerHeight;
                var left_1 = h * aspect / 2;
                var camera = new THREE.OrthographicCamera(-left_1, left_1, h / 2, -h / 2, 1, 1000);
                camera.position.set(-128, -128, 0);
                camera.up.set(0, 0, 1); // In our data, z is up
                console.log(camera);
                this.scene = scene;
                this.renderer = renderer;
                this.camera = camera;
                this.globalPlane = globalPlane;
                // Create controls
                var controls = new window.OrbitControls(camera, renderer.domElement);
                controls.addEventListener('change', function () { return _this.render(); });
                controls.target.set(128, 128, 128);
                controls.minZoom = 0.25;
                controls.maxZoom = 5;
                controls.enablePan = true;
                controls.screenSpacePanning = true;
                controls.enableDamping = false;
                controls.update();
                // scene.add( new AxesHelper( 128 ) );
                // Lighting is baked into the shader a.t.m.
                // let dirLight = new DirectionalLight( 0xffffff );
                // The gui for interaction
                var volconfig = {
                    clim1: 0, clim2: 1, renderstyle: 'iso', isothreshold: 0.15, colormap: 'jet',
                    left: camera.left, right: camera.right, top: camera.top, bottom: camera.bottom,
                    showAxis: true,
                    enableDamping: controls.enableDamping,
                    enableClipping: false,
                    clip_x: globalPlane.constant,
                    clip_y: globalPlane.constant,
                    clip_z: globalPlane.constant
                };
                var gui = new window.GUI();
                var renderArgs = gui.addFolder("Render");
                var controlArgs = gui.addFolder("Controls");
                // const globalClipping = gui.addFolder("Volume Clipping");
                renderArgs.add(volconfig, 'clim1', 0, 1, 0.01).onChange(function () { return _this.updateUniforms(); });
                renderArgs.add(volconfig, 'clim2', 0, 1, 0.01).onChange(function () { return _this.updateUniforms(); });
                renderArgs.add(volconfig, 'colormap', cm_names()).onChange(function () { return _this.updateUniforms(); });
                renderArgs.add(volconfig, 'renderstyle', { mip: 'mip', iso: 'iso' }).onChange(function () { return _this.updateUniforms(); });
                renderArgs.add(volconfig, 'isothreshold', 0, 1, 0.01).onChange(function () { return _this.updateUniforms(); });
                renderArgs.add(volconfig, 'showAxis').onChange(function (value) {
                    vm.axesHelper.visible = value;
                    vm.render();
                });
                controlArgs.add(volconfig, 'enableDamping').onChange(function (value) {
                    controls.enableDamping = value;
                    controls.update();
                });
                // globalClipping.add(volconfig, "enableClipping").onChange(function (value) {
                // });
                // globalClipping.add(volconfig, "clip_x", -512, 512, 1).onChange((value) => {
                //     camera.position.setX(value);
                //     camera.updateProjectionMatrix();
                //     vm.render();
                // });
                // globalClipping.add(volconfig, "clip_y", -512, 512, 1).onChange((value) => {
                //     camera.position.setY(value);
                //     camera.updateProjectionMatrix();
                //     vm.render();
                // });
                // globalClipping.add(volconfig, "clip_z", -512, 512, 1).onChange((value) => {
                //     camera.position.setZ(value);
                //     camera.updateProjectionMatrix();
                //     vm.render();
                // });
                // Stats
                var stats = new window.Stats();
                document.body.appendChild(stats.dom);
                this.controls = controls;
                this.stats = stats;
                this.volconfig = volconfig;
                if ($ts("@data:format") == "nrrd") {
                    // Load the default model data ...
                    this.loadNrrdModel($ts("@data:default-maldi"));
                }
                else {
                    this.loadAsciiModel($ts("@data:default-maldi"));
                }
                window.addEventListener('resize', function () { return _this.onWindowResize(); });
            };
            /**
             * Load the data ...
            */
            three_app.prototype.loadNrrdModel = function (path) {
                var _this = this;
                new window.NRRDLoader().load(path, function (volume) { return _this.loadVolumeModel(volume); });
            };
            three_app.prototype.loadAsciiModel = function (path) {
                var _this = this;
                new window.ASCIILoader().load(path, function (volume) { return _this.loadVolumeModel(volume); });
            };
            three_app.prototype.loadVolumeModel = function (volume) {
                var _this = this;
                // Texture to hold the volume. We have scalars, so we put our data in the red channel.
                // THREEJS will select R32F (33326) based on the THREE.RedFormat and THREE.FloatType.
                // Also see https://www.khronos.org/registry/webgl/specs/latest/2.0/#TEXTURE_TYPES_FORMATS_FROM_DOM_ELEMENTS_TABLE
                // TODO: look the dtype up in the volume metadata
                var texture = new THREE.Data3DTexture(volume.data, volume.xLength, volume.yLength, volume.zLength);
                texture.format = THREE.RedFormat;
                texture.type = THREE.FloatType;
                texture.minFilter = texture.magFilter = THREE.LinearFilter;
                texture.unpackAlignment = 1;
                texture.needsUpdate = true;
                console.log("inspect of your 3d model data:");
                console.log(volume);
                // Colormap textures
                this.cmtextures = cm_textures(function () { return _this.render(); });
                // Material
                var shader = window.VolumeRenderShader1;
                var uniforms = THREE.UniformsUtils.clone(shader.uniforms);
                var volconfig = this.volconfig;
                uniforms['u_data'].value = texture;
                uniforms['u_size'].value.set(volume.xLength, volume.yLength, volume.zLength);
                uniforms['u_clim'].value.set(volconfig.clim1, volconfig.clim2);
                uniforms['u_renderstyle'].value = volconfig.renderstyle == 'mip' ? 0 : 1; // 0: MIP, 1: ISO
                uniforms['u_renderthreshold'].value = volconfig.isothreshold; // For ISO renderstyle
                uniforms['u_cmdata'].value = this.cmtextures[volconfig.colormap];
                // const localPlane = new THREE.Plane(new THREE.Vector3(0, - 1, 0), 20);
                this.material = new THREE.ShaderMaterial({
                    uniforms: uniforms,
                    vertexShader: shader.vertexShader,
                    fragmentShader: shader.fragmentShader,
                    side: THREE.DoubleSide, // The volume shader uses the backface as its "reference point"
                    // ***** Clipping setup (material): *****
                    // clippingPlanes: [localPlane],
                    // clipShadows: true
                });
                // THREE.Mesh
                var geometry = new THREE.BoxGeometry(volume.xLength, volume.yLength, volume.zLength);
                geometry.translate(volume.xLength / 2 - 0.5, volume.yLength / 2 - 0.5, volume.zLength / 2 - 0.5);
                var mesh = new THREE.Mesh(geometry, this.material);
                this.scene.add(mesh);
                var axesHelper = new THREE.AxesHelper(256);
                this.scene.add(axesHelper);
                this.axesHelper = axesHelper;
                // const helper = new THREE.PlaneHelper(this.globalPlane, 300, 0xffff00);
                // this.scene.add(helper);
                // const helper2 = new THREE.PlaneHelper(localPlane, 300, 0xffff00);
                // this.scene.add(helper2);
                this.render();
            };
            three_app.prototype.updateUniforms = function () {
                var material = this.material;
                var volconfig = this.volconfig;
                material.uniforms['u_clim'].value.set(volconfig.clim1, volconfig.clim2);
                material.uniforms['u_renderstyle'].value = volconfig.renderstyle == 'mip' ? 0 : 1; // 0: MIP, 1: ISO
                material.uniforms['u_renderthreshold'].value = volconfig.isothreshold; // For ISO renderstyle
                material.uniforms['u_cmdata'].value = this.cmtextures[volconfig.colormap];
                this.render();
            };
            three_app.prototype.onWindowResize = function () {
                this.renderer.setSize(window.innerWidth, window.innerHeight);
                var camera = this.camera;
                var aspect = window.innerWidth / window.innerHeight;
                var frustumHeight = camera.top - camera.bottom;
                camera.left = -frustumHeight * aspect / 2;
                camera.right = frustumHeight * aspect / 2;
                camera.updateProjectionMatrix();
                this.render();
            };
            three_app.prototype.render = function () {
                var stats = this.stats;
                if (!isNullOrUndefined(stats)) {
                    this.stats.begin();
                    this.renderer.render(this.scene, this.camera);
                    this.stats.end();
                }
                else {
                    this.renderer.render(this.scene, this.camera);
                }
            };
            return three_app;
        }(Bootstrap));
        viewer.three_app = three_app;
    })(viewer = apps.viewer || (apps.viewer = {}));
})(apps || (apps = {}));
/// <reference path="./d/linq.d.ts" />
/// <reference path="./apps/viewer/three_app.ts" />
var app;
(function (app) {
    var desktop;
    (function (desktop) {
        /**
         * main function for run start of the desktop app
        */
        function run() {
            // mzkit system pages
            Router.AddAppHandler(new apps.home());
            Router.AddAppHandler(new apps.systems.pluginMgr());
            Router.AddAppHandler(new apps.systems.pluginPkg());
            Router.AddAppHandler(new apps.systems.servicesManager());
            Router.AddAppHandler(new apps.systems.settings());
            // data analysis & data visualization
            Router.AddAppHandler(new apps.viewer.three_app());
            Router.AddAppHandler(new apps.viewer.clusterViewer());
            Router.AddAppHandler(new apps.viewer.LCMSScatterViewer());
            Router.AddAppHandler(new apps.viewer.GCxGCPeaksViewer());
            Router.AddAppHandler(new apps.viewer.OpenseadragonSlideViewer());
            Router.AddAppHandler(new apps.viewer.umap());
            Router.AddAppHandler(new apps.viewer.lcmsLibrary());
            Router.AddAppHandler(new apps.viewer.svgViewer());
            Router.RunApp();
        }
        desktop.run = run;
    })(desktop = app.desktop || (app.desktop = {}));
})(app || (app = {}));
$ts.mode = Modes.development;
$ts(app.desktop.run);
var app;
(function (app) {
    var desktop;
    (function (desktop) {
        /**
         * the mzkit desktop app
        */
        desktop.mzkit = getHostObject();
        /**
         * get global desktop object to the visualbasic clr runtime environment
        */
        function getHostObject() {
            try {
                return window.chrome.webview.hostObjects.mzkit;
            }
            catch (_a) {
                return null;
            }
        }
    })(desktop = app.desktop || (app.desktop = {}));
})(app || (app = {}));
var gl_plot;
(function (gl_plot) {
    var echart_peak3D = /** @class */ (function () {
        function echart_peak3D(read_id, read_point, read_intensity, read_label, xlab, ylab, zlab) {
            if (zlab === void 0) { zlab = "Intensity"; }
            this.read_id = read_id;
            this.read_point = read_point;
            this.read_intensity = read_intensity;
            this.read_label = read_label;
            this.xlab = xlab;
            this.ylab = ylab;
            this.zlab = zlab;
            this.layers = new Dictionary();
        }
        echart_peak3D.prototype.scatter_group = function (data, color, label) {
            // const seq = $from(data);
            // const r_range = globalThis.data.NumericRange.Create(seq.Select(a => a.mz));
            // const g_range = globalThis.data.NumericRange.Create(seq.Select(a => a.scan_time));
            // const b_range = globalThis.data.NumericRange.Create(seq.Select(a => a.intensity));
            // const byte_range = new globalThis.data.NumericRange(0, 255);
            var _this = this;
            return {
                type: 'bar3D',
                shading: 'color', // color, lambert, realistic
                barSize: 0.1,
                name: "Intensity ".concat(label), // format_tag(r),
                spot_labels: $from(data).Select(function (r) { return _this.read_id(r); }).ToArray(),
                symbolSize: 1,
                dimensions: [
                    this.xlab,
                    this.ylab,
                    this.zlab
                ],
                data: $from(data).Select(function (r) { return _this.read_point(r); }).ToArray(),
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
        };
        echart_peak3D.prototype.load_cluster = function (data) {
            var _this = this;
            var seq = $from(data);
            var max = seq.Select(function (a) { return _this.read_intensity(a); }).Max();
            var d = max / this.colors.length;
            // // const class_labels = $from(data).Select(r => r.cluster).Distinct().ToArray();
            // // const numeric_cluster = $from(class_labels).All(si => Strings.isIntegerPattern(si.toString()));
            // // const format_tag = clusterViewer.format_cluster_tag(data);
            // // const scatter3D = $from(data)
            // //     .Select(function (r) {
            // //     })
            // //     .ToArray();
            // // const spot_labels = $from(data).ToDictionary(d => format_tag(d), d => d.labels);
            var scatter3D = [];
            var i = 0;
            var _loop_1 = function (min) {
                var l0 = min + d;
                var subset = seq.Where(function (a) {
                    var into = _this.read_intensity(a);
                    return into > min && into < l0;
                }).ToArray();
                var color = this_1.colors[i++];
                var label = "".concat(min.toExponential(1), " ~ ").concat(l0.toExponential(1));
                this_1.layers.Add("Intensity ".concat(label), subset);
                scatter3D.push(this_1.scatter_group(subset, color, label));
            };
            var this_1 = this;
            for (var min = 0; min < max; min = min + d) {
                _loop_1(min);
            }
            // const scatter3D = [LCMSScatterViewer.scatter_group(data)];
            return {
                grid3D: {
                    color: "white",
                    axisPointer: {
                        show: false
                    },
                    viewControl: {
                        distance: 200,
                        beta: -20,
                        panMouseButton: 'right', //平移操作使用的鼠标按键
                        rotateMouseButton: 'left', //旋转操作使用的鼠标按键
                        alpha: 30 // 让canvas在x轴有一定的倾斜角度
                    },
                    postEffect: {
                        enable: false,
                        SSAO: {
                            radius: 1, //环境光遮蔽的采样半径。半径越大效果越自然
                            intensity: 1, //环境光遮蔽的强度
                            enable: false
                        }
                    },
                    temporalSuperSampling: {
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
                        formatter: function (value) { return _this.format_axisLabel(value); }
                    }
                },
                series: scatter3D,
                tooltip: {
                    show: true, // 是否显示
                    trigger: 'item', // 触发类型  'item'图形触发：散点图，饼图等无类目轴的图表中使用； 'axis'坐标轴触发；'none'：什么都不触发。
                    axisPointer: {
                        type: 'cross', // 'line' 直线指示器  'shadow' 阴影指示器  'none' 无指示器  'cross' 十字准星指示器。
                    },
                    // showContent: true, //是否显示提示框浮层，默认显示。
                    // triggerOn: 'mouseover', // 触发时机'click'鼠标点击时触发。 
                    backgroundColor: 'white', // 提示框浮层的背景颜色。
                    borderColor: '#333', // 提示框浮层的边框颜色。
                    borderWidth: 0, // 提示框浮层的边框宽。
                    padding: 5, // 提示框浮层内边距，
                    textStyle: {
                        color: 'darkblue',
                        fontStyle: 'normal',
                        fontWeight: 'normal',
                        fontFamily: 'sans-serif',
                        fontSize: 12,
                    },
                    // 提示框浮层内容格式器，支持字符串模板和回调函数两种形式。
                    // 模板变量有 {a}, {b}，{c}，分别表示系列名，数据名，数据值等
                    // formatter: '{a}--{b} 的成绩是 {c}'
                    formatter: function (arg) { return _this.read_label(arg, data); }
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
        };
        echart_peak3D.prototype.format_axisLabel = function (value) {
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
                    }
                    else if (res[i] == ".")
                        continue;
                    else
                        break;
                }
                var v = parseFloat(res);
                v = v * Math.pow(10, numN2);
                return v.toString() + "e-" + numN2;
            }
            else if (num1 > 4) {
                if (res[0] == "-")
                    numN1 = num1 - 2;
                else
                    numN1 = num1 - 1;
                var v = parseFloat(res);
                v = v / Math.pow(10, numN1);
                if (num2 > 4)
                    v = v.toFixed(4);
                return v.toString() + "e" + numN1;
            }
            else
                return parseFloat(res);
        };
        return echart_peak3D;
    }());
    gl_plot.echart_peak3D = echart_peak3D;
})(gl_plot || (gl_plot = {}));
/**
 * Read of 3d model file blob
*/
var ModelReader = /** @class */ (function () {
    /**
     * @param bin the data should be in network byte order
    */
    function ModelReader(bin) {
        this.bin = bin;
        this.pointCloud = [];
        this.palette = [];
        // npoints
        var view = new DataView(bin.buffer, 0, 8);
        var npoints = view.getInt32(0);
        // ncolors
        var ncolors = view.getInt32(4);
        // html color literal string array
        // is fixed size         
        // #rrggbb   
        var colorEnds = 8 + ncolors * 7;
        var stringBuf = bin.slice(8, colorEnds);
        var strings = String.fromCharCode.apply(null, stringBuf);
        for (var i = 0; i < ncolors; i++) {
            this.palette.push(strings.substring(i * 7, (i + 1) * 7));
        }
        view = new DataView(bin.buffer, colorEnds);
        for (var i = 0; i < npoints; i++) {
            var offset = i * (8 + 8 + 8 + 8 + 4);
            var x = view.getFloat64(offset) / 10;
            var y = view.getFloat64(offset + 8) / 10;
            var z = view.getFloat64(offset + 16) / 10;
            var data_1 = view.getFloat64(offset + 24);
            var clr = view.getInt32(offset + 32);
            this.pointCloud.push({
                x: x, y: y, z: z,
                intensity: data_1,
                color: this.palette[clr]
            });
        }
        this.centroid();
        // this.cubic_scale();
    }
    ModelReader.prototype.cubic_scale = function () {
        var v = this.getVector3();
        var cubic = new data.NumericRange(-100, 100);
        var x = new data.NumericRange($ts(v.x).Min(), $ts(v.x).Max());
        var y = new data.NumericRange($ts(v.y).Min(), $ts(v.y).Max());
        var z = new data.NumericRange($ts(v.z).Min(), $ts(v.z).Max());
        var rad = 45 * Math.PI / 180;
        var cosa = Math.cos(rad);
        var sina = Math.sin(rad);
        var Xn = 0;
        var Zn = 0;
        var f = Math.pow(10, 15);
        for (var _i = 0, _a = this.pointCloud; _i < _a.length; _i++) {
            var point = _a[_i];
            point.x = x.ScaleMapping(point.x, cubic);
            point.y = y.ScaleMapping(point.y, cubic);
            point.z = z.ScaleMapping(point.z, cubic);
            Zn = point.z * cosa - point.x * sina;
            Xn = point.z * sina + point.x * cosa;
            point.x = Xn;
            point.z = Zn * f;
        }
    };
    ModelReader.prototype.centroid = function () {
        var v = this.getVector3();
        var offset_x = $ts(v.x).Sum() / this.pointCloud.length;
        var offset_y = $ts(v.y).Sum() / this.pointCloud.length;
        var offset_z = $ts(v.z).Sum() / this.pointCloud.length;
        for (var _i = 0, _a = this.pointCloud; _i < _a.length; _i++) {
            var point = _a[_i];
            point.x = point.x - offset_x;
            point.y = point.y - offset_y;
            point.z = point.z - offset_z;
        }
    };
    ModelReader.prototype.getVector3 = function () {
        var x = [];
        var y = [];
        var z = [];
        for (var _i = 0, _a = this.pointCloud; _i < _a.length; _i++) {
            var point = _a[_i];
            x.push(point.x);
            y.push(point.y);
            z.push(point.z);
        }
        return {
            x: x,
            y: y,
            z: z
        };
    };
    ModelReader.prototype.loadPointCloudModel = function (canvas) {
        // 轴辅助 （每一个轴的长度）
        // 红色代表 X 轴. 绿色代表 Y 轴. 蓝色代表 Z 轴.
        var object = new THREE.AxesHelper(300);
        //创建THREE.PointCloud粒子的容器
        var geometry = new THREE.Geometry();
        //创建THREE.PointCloud纹理
        var material = new THREE.PointCloudMaterial({
            size: 0.5,
            vertexColors: true,
            color: 0xffffff
        });
        canvas.scene.add(object);
        //循环将粒子的颜色和位置添加到网格当中
        // for (var x = -5; x <= 5; x++) {
        //     for (var y = -5; y <= 5; y++) {
        //         var particle = new THREE.Vector3(x * 10, y * 10, 0);
        //         geometry.vertices.push(particle);
        //         geometry.colors.push(new THREE.Color(+three_app.randomColor()));
        //     }
        // }
        for (var _i = 0, _a = this.pointCloud; _i < _a.length; _i++) {
            var point = _a[_i];
            var particle = new THREE.Vector3(point.x, point.y, point.z);
            geometry.vertices.push(particle);
            geometry.colors.push(new THREE.Color(point.color));
        }
        //实例化THREE.PointCloud
        canvas.scene.add(new THREE.PointCloud(geometry, material));
    };
    return ModelReader;
}());
/// <reference path="../d/linq.d.ts" />
var apps;
(function (apps) {
    apps.biodeep_classroom = 'http://v2.biodeep.cn/api/nmdx-cloud-basic/km-curriculum-info/cloud/list?pageNo=1&pageSize=12&sort=new';
    apps.biodeep_viewVideo = 'http://v2.biodeep.cn/class/detail?id=%s&page=class';
    var home = /** @class */ (function (_super) {
        __extends(home, _super);
        function home() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        Object.defineProperty(home.prototype, "appName", {
            get: function () {
                return "home";
            },
            enumerable: false,
            configurable: true
        });
        home.prototype.init = function () {
            var _this = this;
            $ts.getText(apps.biodeep_classroom, function (text) { return _this.loadList(text); });
        };
        home.prototype.loadList = function (json_str) {
            try {
                this.showClassRoom(JSON.parse(json_str));
            }
            catch (_a) {
                console.error("invalid json response text:");
                console.error(json_str);
            }
        };
        home.prototype.showClassRoom = function (res) {
            var success = res.success, result = res.result;
            var newsList = $ts("#newsList");
            if (!(success && result)) {
                return newsList.hide();
            }
            else {
                // newsList.show();
            }
            for (var _i = 0, _a = result.records; _i < _a.length; _i++) {
                var item = _a[_i];
                var liItem = "\n                    <img class=\"news-pic\" src=\"".concat(item.imgUrl, "\" />\n                    <div class=\"news-txt\">\n                        <a href=\"").concat(sprintf(apps.biodeep_viewVideo, item.id), "\">").concat(item.title, "</a>\n                        <span>").concat(item.createTime, "</span>\n                    </div>");
                var li = $ts("<li>").display(liItem);
                newsList.appendElement(li);
            }
        };
        return home;
    }(Bootstrap));
    apps.home = home;
})(apps || (apps = {}));
var apps;
(function (apps) {
    var systems;
    (function (systems) {
        var pluginMgr = /** @class */ (function (_super) {
            __extends(pluginMgr, _super);
            function pluginMgr() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            Object.defineProperty(pluginMgr.prototype, "appName", {
                get: function () {
                    return "pluginMgr";
                },
                enumerable: false,
                configurable: true
            });
            ;
            pluginMgr.prototype.init = function () {
                var vm = this;
                app.desktop.mzkit
                    .GetPlugins()
                    .then(function (json) {
                    return __awaiter(this, void 0, void 0, function () {
                        var json_str, list, mgr, _i, list_1, plugin;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, json];
                                case 1:
                                    json_str = _a.sent();
                                    list = JSON.parse(json_str);
                                    mgr = $ts("#plugin-list").clear();
                                    console.log("get plugin list:");
                                    console.table(list);
                                    console.log("json string source:");
                                    console.log(json_str);
                                    for (_i = 0, list_1 = list; _i < list_1.length; _i++) {
                                        plugin = list_1[_i];
                                        vm.addPlugin(mgr, plugin);
                                    }
                                    $ts.select(".deactive").onClick(function (e) { return vm.setPluginStatus(e, "disable"); });
                                    $ts.select(".edit").onClick(function (e) { return vm.setPluginStatus(e, "active"); });
                                    $ts.select(".delete");
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            pluginMgr.prototype.setPluginStatus = function (e, stat) {
                app.desktop.mzkit.SetStatus(e.getAttribute("data"), stat);
                location.reload();
            };
            pluginMgr.prototype.addPlugin = function (mgr, plugin) {
                var type = (plugin.status == "disable" || plugin.status == "incompatible") ? "inactive" : "active";
                var row = $ts("<tr>", { class: type });
                var action = type == "active" ? "<span class=\"deactivate\">\n            <a href=\"#\" class=\"deactive\" data=\"".concat(plugin.id, "\">Deactivate</a>\n        </span>") : "<span class=\"activate\">\n        <a href=\"#\" class=\"edit\" data=\"".concat(plugin.id, "\">Activate</a> <!--|\n    </span>\n    <span class=\"delete\">\n        <a href=\"#\" class=\"delete\" data=\"").concat(plugin.id, "\">Delete</a>\n    </span>-->");
                var html = "\n            \n            <th scope=\"row\" class=\"check-column\">\n                <input type=\"checkbox\" name=\"check_plugins\" />\n            </th>\n            <td class=\"plugin-title column-primary\">\n                <strong><a href=\"#\" onclick=\"app.desktop.mzkit.Exec('".concat(plugin.id, "')\">").concat(plugin.name, "</a></strong>\n                <div class=\"row-actions visible\">\n                    ").concat(action, "\n                </div>        \n            </td>\n            <td class=\"column-description desc\">\n                <div class=\"plugin-description\">\n                    <p>\n                        ").concat(plugin.desc, "\n                    </p>\n                </div>\n                <div class=\"").concat(type, " second plugin-version-author-uri\">\n                    Version ").concat(plugin.ver, " | By\n                    <a href=\"#\">").concat(plugin.author, "</a> |\n                    <a href=\"").concat(plugin.url, "\" class=\"thickbox open-plugin-details-modal\">View details</a>\n                </div>\n            </td>\n            <td class=\"column-auto-updates\">\n                \n            </td>     \n            ");
                mgr.appendChild(row.display(html));
            };
            pluginMgr.prototype.install_local_onclick = function () {
                app.desktop.mzkit.InstallLocal();
            };
            return pluginMgr;
        }(Bootstrap));
        systems.pluginMgr = pluginMgr;
    })(systems = apps.systems || (apps.systems = {}));
})(apps || (apps = {}));
var apps;
(function (apps) {
    var systems;
    (function (systems) {
        var pluginPkg = /** @class */ (function (_super) {
            __extends(pluginPkg, _super);
            function pluginPkg() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            Object.defineProperty(pluginPkg.prototype, "appName", {
                get: function () {
                    return "pluginPkg";
                },
                enumerable: false,
                configurable: true
            });
            ;
            pluginPkg.prototype.init = function () {
                // throw new Error("Method not implemented.");
            };
            pluginPkg.prototype.dir_onchange = function (value) {
                console.log(value);
                app.desktop.mzkit.GetFiles(value).then(function (json) {
                    return __awaiter(this, void 0, void 0, function () {
                        var files, _a, _b;
                        return __generator(this, function (_c) {
                            switch (_c.label) {
                                case 0:
                                    _b = (_a = JSON).parse;
                                    return [4 /*yield*/, json];
                                case 1:
                                    files = _b.apply(_a, [_c.sent()]);
                                    console.log(files);
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            pluginPkg.prototype.selectFolder_onclick = function () {
                var vm = this;
                app.desktop.mzkit.SelectFolder().then(function (dir) {
                    return __awaiter(this, void 0, void 0, function () {
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, dir];
                                case 1:
                                    dir = _a.sent();
                                    if (!Strings.Empty(dir)) {
                                        $input("#dir").value = dir;
                                        vm.dir_onchange(dir);
                                    }
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            pluginPkg.prototype.build_onclick = function () {
                var vm = this;
                var dir = $input("#dir").value.toString();
                console.log("Build plugin package: ".concat(dir, "!"));
                app.desktop.mzkit.BuildPkg(dir).then(function (flag) {
                    return __awaiter(this, void 0, void 0, function () {
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, flag];
                                case 1:
                                    flag = _a.sent();
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            return pluginPkg;
        }(Bootstrap));
        systems.pluginPkg = pluginPkg;
    })(systems = apps.systems || (apps.systems = {}));
})(apps || (apps = {}));
var apps;
(function (apps) {
    var systems;
    (function (systems) {
        var servicesManager = /** @class */ (function (_super) {
            __extends(servicesManager, _super);
            function servicesManager() {
                var _this = _super !== null && _super.apply(this, arguments) || this;
                _this.cpu = new Dictionary();
                _this.memory = new Dictionary();
                return _this;
            }
            Object.defineProperty(servicesManager.prototype, "appName", {
                get: function () {
                    return "mzkit/services";
                },
                enumerable: false,
                configurable: true
            });
            ;
            servicesManager.prototype.init = function () {
                var _this = this;
                this.startUpdateTask();
                setInterval(function () { return _this.startUpdateTask(); }, 1500);
            };
            /**
             * on update a frame display
            */
            servicesManager.prototype.startUpdateTask = function () {
                var vm = this;
                app.desktop.mzkit
                    .GetServicesList()
                    .then(function (json) {
                    return __awaiter(this, void 0, void 0, function () {
                        var fetch, list;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, json];
                                case 1:
                                    fetch = _a.sent();
                                    list = JSON.parse(fetch);
                                    vm.loadServicesList(list);
                                    vm.onDraw();
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            /**
             * tick loop frame
            */
            servicesManager.prototype.loadServicesList = function (list) {
                var vm = this;
                for (var _i = 0, list_2 = list; _i < list_2.length; _i++) {
                    var svr = list_2[_i];
                    var id = "P".concat(svr.PID);
                    if (!vm.cpu.ContainsKey(id)) {
                        vm.cpu.Add(id, { svr: svr, Counter: [] });
                        vm.memory.Add(id, { svr: svr, Counter: [] });
                    }
                    if (svr.isAlive) {
                        vm.cpu.Item(id).Counter.push(svr.CPU);
                        vm.memory.Item(id).Counter.push(svr.Memory);
                    }
                    svr.Memory = Strings.Lanudry(svr.Memory);
                    svr.isAlive = svr.isAlive ? "Running" : "Stopped";
                }
                $ts("#num-svr").display(list.length.toString());
                $ts("#services-list").clear();
                list = $from(list).Select(function (s) {
                    return {
                        PID: s.PID,
                        Name: s.Name,
                        Description: s.Description,
                        Protocol: s.Protocol,
                        Port: s.Port,
                        CPU: s.CPU,
                        Memory: s.Memory,
                        StartTime: s.StartTime,
                        isAlive: s.isAlive
                    };
                }).ToArray();
                $ts.appendTable(list, "#services-list", null, { class: [] }, function (o, r) { return vm.styleEachRow(o, r); });
            };
            servicesManager.prototype.onDraw = function () {
                var vm = this;
                if (!vm.plot) {
                    return;
                }
                else if (!(vm.cpu_chart && vm.mem_chart)) {
                    return;
                }
                var cpu = vm.plot.cpu;
                var mem = vm.plot.memory;
                var panel = $ts("#service-info").clear();
                var x = __spreadArray([], cpu.Counter, true).map(function (_, index) { return index + 1; });
                panel.display($ts("<h3>").display(cpu.svr.Name));
                panel.appendElement($ts("<p>").display(cpu.svr.Description));
                panel.appendElement($ts("<p>").display(cpu.svr.StartTime));
                panel.appendElement($ts("<p>").display("Startup: <pre><code>".concat(cpu.svr.CommandLine, "</code></pre>")));
                if (this.refresh) {
                    this.cpu_chart.plot({ x: x, y: cpu.Counter, title: "Performance Counter (CPU history)" });
                    this.mem_chart.plot({ x: x, y: mem.Counter, title: "Performance Counter (Memory history)" });
                }
                else {
                    this.cpu_chart.chartObj.setOption({ series: [{ data: servicesManager.history(x, cpu.Counter) }] });
                    this.mem_chart.chartObj.setOption({ series: [{ data: servicesManager.history(x, mem.Counter) }] });
                }
                this.refresh = false;
            };
            servicesManager.history = function (x, y) {
                var bars = [];
                for (var i = 0; i < x.length; i++) {
                    bars.push([x[i], y[i]]);
                }
                return bars;
            };
            servicesManager.counterChart = function (data) {
                return {
                    title: {
                        text: data.title
                    },
                    xAxis: {
                        type: 'value',
                        splitLine: {
                            show: true
                        }
                    },
                    yAxis: {
                        type: 'value',
                        boundaryGap: [0, '100%'],
                        splitLine: {
                            show: true
                        }
                    },
                    series: [
                        {
                            name: 'Performance Counter',
                            type: 'line',
                            showSymbol: false,
                            data: servicesManager.history(data.x, data.y),
                            areaStyle: {}
                        }
                    ]
                };
            };
            servicesManager.prototype.updatePlotHost = function () {
                this.cpu_chart = new plot.histogramPlot(servicesManager.counterChart, "cpu-history");
                this.mem_chart = new plot.histogramPlot(servicesManager.counterChart, "mem-history");
                this.refresh = true;
            };
            servicesManager.prototype.styleEachRow = function (svr, row) {
                var vm = this;
                if (!(svr.isAlive == "Running")) {
                    row.classList.add("disabled");
                }
                row.onclick = function () {
                    var id = "P".concat(svr.PID);
                    var cpu = vm.cpu.Item(id);
                    var mem = vm.memory.Item(id);
                    if (vm.plot) {
                        if (vm.plot.cpu.svr.PID != cpu.svr.PID) {
                            // create new plot
                            vm.updatePlotHost();
                        }
                    }
                    else {
                        // create new plot
                        vm.updatePlotHost();
                    }
                    vm.plot = { cpu: cpu, memory: mem };
                    // draw echart
                    vm.onDraw();
                };
            };
            return servicesManager;
        }(Bootstrap));
        systems.servicesManager = servicesManager;
    })(systems = apps.systems || (apps.systems = {}));
})(apps || (apps = {}));
var apps;
(function (apps) {
    var systems;
    (function (systems) {
        var pages = {
            "mzkit_page": "MZKit Settings",
            "msraw_page": "Raw File Viewer",
            "chromagram_page": "Chromagram Plot Styles",
            "formula_page": "Formula Search",
            "element_profile_page": "Formula Search Profile",
            "molecule_networking_page": "Molecular Networking"
        };
        systems.element_columns = [{
                title: "Atom Element",
                field: "atom",
                sortable: true,
                width: 200,
                editable: true,
            }, {
                title: "Min",
                field: "min",
                sortable: true,
                width: 200,
                editable: {
                    type: "number"
                }
            }, {
                title: "Max",
                field: "max",
                sortable: true,
                width: 200,
                editable: {
                    type: "number"
                }
            }];
        var settings = /** @class */ (function (_super) {
            __extends(settings, _super);
            function settings() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            Object.defineProperty(settings.prototype, "appName", {
                get: function () {
                    return "mzkit/settings";
                },
                enumerable: false,
                configurable: true
            });
            settings.prototype.init = function () {
                var vm = this;
                vm.mzkit_page_btn_onclick();
                app.desktop.mzkit.loadSettings()
                    .then(function (json) {
                    return __awaiter(this, void 0, void 0, function () {
                        var json_str, settings, configs;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, json];
                                case 1:
                                    json_str = _a.sent();
                                    settings = JSON.parse(json_str) || {};
                                    configs = apps.systems.settings.defaultSettings();
                                    console.log("get mzkit configurations:");
                                    console.log(settings);
                                    // deal with the possible null reference value
                                    settings.precursor_search = settings.precursor_search || {};
                                    settings.ui = settings.ui || {};
                                    // make data object conversion
                                    configs.formula_ppm = settings.precursor_search.ppm || 5;
                                    configs.formula_adducts = settings.precursor_search.precursor_types || [];
                                    configs.remember_layout = settings.ui.rememberLayouts || "true";
                                    configs.remember_location = settings.ui.rememberWindowsLocation || "true";
                                    configs.language = settings.ui.language || 2;
                                    vm.loadConfigs(configs);
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            settings.prototype.remember_location_onchange = function (value) {
                settings.mzkit_configs.remember_location = (Array.isArray(value) ? value[0] : value);
            };
            settings.prototype.remember_layout_onchange = function (value) {
                settings.mzkit_configs.remember_layout = (Array.isArray(value) ? value[0] : value);
            };
            settings.prototype.language_onchange = function (value) {
                settings.mzkit_configs.language = (Array.isArray(value) ? value[0] : value);
            };
            settings.prototype.loadConfigs = function (configs) {
                var formula_profiles = configs.formula_search;
                settings.mzkit_configs = configs;
                settings.load_profileTable(configs);
                settings.bindRangeDisplayValue(configs, function (config) {
                    // save
                });
                $ts.value("#language", configs.language);
                $ts.value("#remember_location", configs["remember_location"]);
                $ts.value("#remember_layout", configs["remember_layout"]);
                $ts.value("#fragment_cutoff", configs["fragment_cutoff"]);
                $ts.value("#fill-plot-area", configs["fill-plot-area"]);
                $ts.value("#small_molecule_profile", formula_profiles.smallMoleculeProfile.type);
                $ts.value("#sm_common", formula_profiles.smallMoleculeProfile.isCommon);
                $ts.value("#np_profile", formula_profiles.naturalProductProfile.type);
                $ts.value("#np_common", formula_profiles.naturalProductProfile.isCommon);
                app.desktop.mzkit.getAllAdducts()
                    .then(function (json) {
                    return __awaiter(this, void 0, void 0, function () {
                        var json_str, list, adducts, selected, _i, list_3, adduct, key_id, value, checked;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, json];
                                case 1:
                                    json_str = _a.sent();
                                    list = JSON.parse(json_str);
                                    adducts = $ts("#formula_adducts").clear();
                                    selected = configs.formula_adducts || [];
                                    for (_i = 0, list_3 = list; _i < list_3.length; _i++) {
                                        adduct = list_3[_i];
                                        key_id = adduct;
                                        value = selected.indexOf(adduct) > -1;
                                        checked = value ? "checked" : "";
                                        adducts.appendElement($ts("<li>", { class: "list-group-item" }).display("\n                            <input class=\"form-check-input me-1\" \n                                type=\"checkbox\" \n                                value=\"\"\n                                id=\"".concat(key_id, "\" ").concat(checked, ">\n                            <label class=\"form-check-label\" for=\"").concat(key_id, "\">").concat(adduct, "</label>")));
                                    }
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            settings.defaultSettings = function () {
                return {
                    // mzkit app
                    "remember_location": true,
                    "remember_layout": true,
                    "language": 2,
                    // raw file viewer
                    "xic_ppm": 10,
                    "fragment_cutoff": "relative",
                    "fragment_cutoff_value": 0.05,
                    // chromagram plot
                    "colorset": [],
                    "fill-plot-area": true,
                    // preset element profiles
                    "formula_search": {
                        "smallMoleculeProfile": { type: "Wiley", isCommon: true },
                        "naturalProductProfile": { type: "Wiley", isCommon: true },
                        "elements": {},
                    },
                    "formula_ppm": 20,
                    "formula_adducts": [],
                    // molecular networking
                    "layout_iterations": 100,
                    // graph layouts
                    "stiffness": 41.76,
                    "repulsion": 10000,
                    "damping": 0.41,
                    // spectrum tree
                    "node_identical": 0.85,
                    "node_similar": 0.8,
                    "edge_filter": 0.8,
                    // network styling
                    "node_radius_min": 1,
                    "node_radius_max": 30,
                    "link_width_min": 1,
                    "link_width_max": 12
                };
            };
            /**
             * auto binding of the [min,max] value range form control
            */
            settings.bindRangeDisplayValue = function (configs, callback) {
                var inputs = $ts.select(".form-range");
                var labels = $ts.select(".form-label").ToDictionary(function (l) { return l.getAttribute("for"); }, function (lb) { return lb; });
                var label_text0 = $ts.select(".form-label").ToDictionary(function (l) { return l.getAttribute("for"); }, function (lb) { return lb.innerText; });
                var _loop_2 = function (range) {
                    var id = range.id;
                    var label_text_raw = label_text0.Item(id);
                    var label_ctl = labels.Item(id);
                    var label_update = function () {
                        label_ctl.innerText = "".concat(label_text_raw, " (").concat(range.value, ")");
                        configs[id] = range.value;
                        callback(configs);
                    };
                    range.onchange = label_update;
                    if (!isNullOrUndefined(configs[id])) {
                        range.value = configs[id];
                        label_update();
                    }
                };
                for (var _i = 0, _a = inputs.Select(function (i) { return i; }).ToArray(); _i < _a.length; _i++) {
                    var range = _a[_i];
                    _loop_2(range);
                }
            };
            /**
             * get table html UI for create custom element profiles
            */
            settings.getElementProfileTable = function () {
                return $("#custom_element_profile");
            };
            settings.load_profileTable = function (configs) {
                var bootstrap = settings.getElementProfileTable();
                var tableOptions = {
                    columns: systems.element_columns,
                    editable: true, //editable需要设置为 true
                    striped: true,
                    clickToSelect: true
                };
                var profiles = configs.formula_search.elements || {};
                var elements = $from(Object.keys(profiles))
                    .Select(function (atom) {
                    return {
                        atom: atom,
                        min: profiles[atom].min,
                        max: profiles[atom].max
                    };
                }).ToArray();
                bootstrap.bootstrapTable(tableOptions);
                settings.loadProfileTable(elements, bootstrap);
            };
            settings.loadProfileTable = function (elements, bootstrap) {
                if (bootstrap === void 0) { bootstrap = settings.getElementProfileTable(); }
                bootstrap.bootstrapTable("removeAll");
                bootstrap.bootstrapTable("load", elements);
            };
            settings.closeAll = function () {
                for (var _i = 0, _a = Object.keys(pages); _i < _a.length; _i++) {
                    var page = _a[_i];
                    $ts("#".concat(page)).hide();
                }
                return this;
            };
            settings.prototype.copy_profile_onchange = function (value) {
                if (Array.isArray(value)) {
                    value = value[0];
                }
                console.log("get profile name: ".concat(value, "!"));
                this.profile_name = value;
            };
            settings.prototype.reset_profile_onclick = function () {
                // load from mzkit host
                app.desktop.mzkit.getProfile(this.profile_name || "Custom_Profile")
                    .then(function (json) {
                    return __awaiter(this, void 0, void 0, function () {
                        var json_str, preset;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, json];
                                case 1:
                                    json_str = _a.sent();
                                    preset = JSON.parse(json_str);
                                    console.log(preset);
                                    if (isNullOrUndefined(preset)) {
                                        settings.loadProfileTable([]);
                                    }
                                    else if (isNullOrEmpty(preset.candidateElements)) {
                                        settings.loadProfileTable([]);
                                    }
                                    else {
                                        settings.loadProfileTable($from(preset.candidateElements).Select(function (c) {
                                            return {
                                                "atom": c.Element,
                                                "min": c.MinCount,
                                                "max": c.MaxCount
                                            };
                                        }).ToArray());
                                    }
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            settings.show = function (page_id) {
                $ts("#".concat(page_id)).show();
                $ts("#title").display(pages[page_id]);
            };
            settings.prototype.mzkit_page_btn_onclick = function () {
                settings.closeAll().show("mzkit_page");
            };
            settings.prototype.msraw_btn_onclick = function () {
                settings.closeAll().show("msraw_page");
            };
            settings.prototype.chromagram_btn_onclick = function () {
                settings.closeAll().show("chromagram_page");
            };
            settings.prototype.formula_btn_onclick = function () {
                settings.closeAll().show("formula_page");
            };
            settings.prototype.profile_btn_onclick = function () {
                settings.closeAll().show("element_profile_page");
            };
            settings.prototype.add_element_onclick = function () {
                settings.getElementProfileTable().bootstrapTable('append', [{ "atom": "", "min": 0, "max": 0 }]);
            };
            settings.prototype.molecule_networking_btn_onclick = function () {
                settings.closeAll().show("molecule_networking_page");
            };
            /**
             * save profile table as custom profiles
            */
            settings.prototype.save_elements_onclick = function () {
                var table = settings.getElementProfileTable();
                var data = table.bootstrapTable("getData");
                console.log("get element profiles:");
                console.table(data);
                app.desktop.mzkit.SetStatus("save_elements", JSON.stringify(data));
            };
            settings.invoke_save = function () {
                console.log("invoke settings save action!");
                app.desktop.mzkit
                    .Save(JSON.stringify(settings.mzkit_configs))
                    .then(function () {
                    return __awaiter(this, void 0, void 0, function () {
                        return __generator(this, function (_a) {
                            console.log("done!");
                            return [2 /*return*/];
                        });
                    });
                });
            };
            settings.mzkit_configs = null;
            return settings;
        }(Bootstrap));
        systems.settings = settings;
    })(systems = apps.systems || (apps.systems = {}));
})(apps || (apps = {}));
var apps;
(function (apps) {
    var viewer;
    (function (viewer) {
        /**
         * #viewer
        */
        var clusterViewer = /** @class */ (function (_super) {
            __extends(clusterViewer, _super);
            function clusterViewer() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            Object.defineProperty(clusterViewer.prototype, "appName", {
                get: function () {
                    return "clusterViewer";
                },
                enumerable: false,
                configurable: true
            });
            ;
            clusterViewer.prototype.init = function () {
                app.desktop.mzkit.GetScatter()
                    .then(function (data) {
                    return __awaiter(this, void 0, void 0, function () {
                        var json, scatter;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, data];
                                case 1:
                                    json = _a.sent();
                                    scatter = JSON.parse(json);
                                    if (isNullOrEmpty(scatter)) {
                                        clusterViewer.render3DScatter([]);
                                    }
                                    else {
                                        clusterViewer.render3DScatter(scatter);
                                    }
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            clusterViewer.render3DScatter = function (dataset, hook_resize) {
                if (hook_resize === void 0) { hook_resize = true; }
                var clusters = [];
                for (var _i = 0, _a = $from(dataset).GroupBy(function (a) { return a.class; }).ToArray(); _i < _a.length; _i++) {
                    var group = _a[_i];
                    var matrix = [];
                    var id = group.Key;
                    var labels = [];
                    for (var _b = 0, _c = group.ToArray(); _b < _c.length; _b++) {
                        var i = _c[_b];
                        matrix.push([i.x, i.y, i.z]);
                        labels.push(i.label);
                    }
                    clusters.push({
                        cluster: id,
                        scatter: matrix,
                        labels: labels
                    });
                }
                var format_tag = clusterViewer.format_cluster_tag(clusters);
                var render = new gl_plot.scatter3d(clusterViewer.load_cluster, "viewer");
                var spot_labels = $from(clusters).ToDictionary(function (d) { return format_tag(d); }, function (d) { return d.labels; });
                var div = $ts("#viewer");
                render.plot(clusters);
                render.chartObj.on("click", function (par) {
                    // console.log(par);
                    var i = par.dataIndex;
                    var category = par.seriesName;
                    var labels = spot_labels.Item(category);
                    var spot_id = labels[i];
                    // console.log(spot_id);
                    app.desktop.mzkit.Click(spot_id);
                });
                var resize_canvas = function () {
                    var padding = 25;
                    div.style.width = (window.innerWidth - padding) + "px";
                    div.style.height = (window.innerHeight - padding) + "px";
                    render.chartObj.resize();
                };
                if (hook_resize) {
                    window.onresize = function () { return resize_canvas(); };
                    resize_canvas();
                }
            };
            clusterViewer.format_cluster_tag = function (data) {
                var class_labels = $from(data).Select(function (r) { return r.cluster; }).Distinct().ToArray();
                var numeric_cluster = $from(class_labels).All(function (si) { return Strings.isIntegerPattern(si.toString()); });
                return (function (r) {
                    return numeric_cluster ? "cluster_".concat(r.cluster) : r.cluster.toString();
                });
            };
            clusterViewer.load_cluster = function (data) {
                var paper = echart_app.paper;
                var class_labels = $from(data).Select(function (r) { return r.cluster; }).Distinct().ToArray();
                var numeric_cluster = $from(class_labels).All(function (si) { return Strings.isIntegerPattern(si.toString()); });
                var format_tag = clusterViewer.format_cluster_tag(data);
                var scatter3D = $from(data)
                    .Select(function (r) {
                    return {
                        type: 'scatter3D',
                        name: format_tag(r),
                        spot_labels: r.labels,
                        symbolSize: 5,
                        dimensions: [
                            'x',
                            'y',
                            'z'
                        ],
                        data: r.scatter,
                        symbol: 'circle',
                        itemStyle: {
                            // borderWidth: 0.5,
                            color: paper[class_labels.indexOf(r.cluster.toString())],
                            // borderColor: 'rgba(255,255,255,0.8)'//边框样式
                        }
                    };
                })
                    .ToArray();
                var spot_labels = $from(data).ToDictionary(function (d) { return format_tag(d); }, function (d) { return d.labels; });
                return {
                    grid3D: {},
                    xAxis3D: { type: 'value', name: 'x' },
                    yAxis3D: { type: 'value', name: 'y' },
                    zAxis3D: { type: 'value', name: 'z' },
                    series: scatter3D,
                    tooltip: {
                        show: true, // 是否显示
                        trigger: 'item', // 触发类型  'item'图形触发：散点图，饼图等无类目轴的图表中使用； 'axis'坐标轴触发；'none'：什么都不触发。
                        axisPointer: {
                            type: 'cross', // 'line' 直线指示器  'shadow' 阴影指示器  'none' 无指示器  'cross' 十字准星指示器。
                        },
                        // showContent: true, //是否显示提示框浮层，默认显示。
                        // triggerOn: 'mouseover', // 触发时机'click'鼠标点击时触发。 
                        backgroundColor: 'white', // 提示框浮层的背景颜色。
                        borderColor: '#333', // 提示框浮层的边框颜色。
                        borderWidth: 0, // 提示框浮层的边框宽。
                        padding: 5, // 提示框浮层内边距，
                        textStyle: {
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
                            var i = arg.dataIndex;
                            var labels = spot_labels.Item(arg.seriesName);
                            var f = arg.data;
                            var r = $from(f).Select(function (n) { return Strings.round(n, 4); }).ToArray();
                            return "".concat(arg.seriesName, " spot:<").concat(labels[i], "> scatter3:").concat(JSON.stringify(r));
                        }
                    },
                    legend: {
                        orient: 'vertical',
                        x: 'right',
                        y: 'center'
                    }
                };
            };
            return clusterViewer;
        }(Bootstrap));
        viewer.clusterViewer = clusterViewer;
    })(viewer = apps.viewer || (apps.viewer = {}));
})(apps || (apps = {}));
var apps;
(function (apps) {
    var viewer;
    (function (viewer) {
        var GCxGCPeaksViewer = /** @class */ (function (_super) {
            __extends(GCxGCPeaksViewer, _super);
            function GCxGCPeaksViewer() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            Object.defineProperty(GCxGCPeaksViewer.prototype, "appName", {
                get: function () {
                    return "gcxgc-peaks";
                },
                enumerable: false,
                configurable: true
            });
            GCxGCPeaksViewer.prototype.create_viewer = function () {
                var _this = this;
                this.peaks3D = new gl_plot.echart_peak3D(function (x) { return null; }, function (x) { return [x.t1, x.t2, x.into]; }, function (x) { return x.into; }, function (arg, x) { return _this.label(arg, x); }, 'Time Dimension1(min)', "Time Dimension2(s)");
                return this;
            };
            GCxGCPeaksViewer.prototype.label = function (arg, data) {
                // console.log(arg);
                var i = arg.dataIndex;
                var labels = data; // spot_labels.Item(arg.seriesName);
                var ms1 = arg.data;
                var rt = Math.round(ms1[0]);
                var mz = Strings.round(ms1[1]);
                var into = ms1[2].toExponential(2); // Math.pow(1.125, ms1[2]).toExponential(2);
                return "GCxGC: ".concat(mz, "@").concat(rt, "s intensity=").concat(into);
            };
            GCxGCPeaksViewer.prototype.init = function () {
                var vm = this.create_viewer();
                app.desktop.mzkit.GetLCMSScatter().then(function (data) {
                    return __awaiter(this, void 0, void 0, function () {
                        var json_str, scatter;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, data];
                                case 1:
                                    json_str = _a.sent();
                                    scatter = JSON.parse(json_str);
                                    app.desktop.mzkit.GetColors().then(function (ls) {
                                        return __awaiter(this, void 0, void 0, function () {
                                            var str, colors, _i, _a, code;
                                            return __generator(this, function (_b) {
                                                switch (_b.label) {
                                                    case 0: return [4 /*yield*/, ls];
                                                    case 1:
                                                        str = _b.sent();
                                                        colors = JSON.parse(str);
                                                        vm.peaks3D.colors = colors;
                                                        for (_i = 0, _a = vm.peaks3D.colors; _i < _a.length; _i++) {
                                                            code = _a[_i];
                                                            TypeScript.logging.log(code, code);
                                                        }
                                                        if (isNullOrEmpty(scatter)) {
                                                            vm.render3DScatter([]);
                                                        }
                                                        else {
                                                            vm.render3DScatter(scatter);
                                                        }
                                                        return [2 /*return*/];
                                                }
                                            });
                                        });
                                    });
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            GCxGCPeaksViewer.prototype.render3DScatter = function (dataset) {
                var _this = this;
                var render = new gl_plot.scatter3d(function (ls) { return _this.peaks3D.load_cluster(ls); }, "viewer");
                var div = $ts("#viewer");
                var vm = this;
                // render.chartObj.showLoading();
                render.plot(dataset);
                render.chartObj.on("click", function (par) {
                    // console.log(par);
                    var i = par.dataIndex;
                    var category = par.seriesName;
                    var labels = vm.peaks3D.layers.Item(category);
                    // const spot_id: string = labels[i].id;
                    // console.log(spot_id);
                    // alert(spot_id);
                    // app.desktop.mzkit.Click(spot_id);
                });
                var resize_canvas = function () {
                    var padding = 18;
                    div.style.width = (window.innerWidth - padding) + "px";
                    div.style.height = (window.innerHeight - padding) + "px";
                    render.chartObj.resize();
                };
                window.onresize = function () { return resize_canvas(); };
                resize_canvas();
            };
            return GCxGCPeaksViewer;
        }(Bootstrap));
        viewer.GCxGCPeaksViewer = GCxGCPeaksViewer;
    })(viewer = apps.viewer || (apps.viewer = {}));
})(apps || (apps = {}));
var apps;
(function (apps) {
    var viewer;
    (function (viewer) {
        var SmilesDrawer = window.SmilesDrawer;
        var lcmsLibrary = /** @class */ (function (_super) {
            __extends(lcmsLibrary, _super);
            function lcmsLibrary() {
                var _this = _super !== null && _super.apply(this, arguments) || this;
                _this.page = 1;
                _this.page_size = 100;
                return _this;
            }
            Object.defineProperty(lcmsLibrary.prototype, "appName", {
                get: function () {
                    return "lcms-library";
                },
                enumerable: false,
                configurable: true
            });
            lcmsLibrary.prototype.init = function () {
                // this.reloadLibs();
                try {
                    lcmsLibrary.hideLoader();
                    this.list_data();
                }
                catch (ex) {
                }
            };
            lcmsLibrary.showLoader = function () {
                $ts("#loader").show();
                $ts("#explorer").interactive(false);
            };
            lcmsLibrary.hideLoader = function () {
                $ts("#loader").hide();
                $ts("#explorer").interactive(true);
            };
            lcmsLibrary.prototype.reloadLibs = function () {
                var vm = this;
                app.desktop.mzkit.ScanLibraries()
                    .then(function (str) {
                    return __awaiter(this, void 0, void 0, function () {
                        var pull_str, list, _i, list_4, file, name_3;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, str];
                                case 1:
                                    pull_str = _a.sent();
                                    list = JSON.parse(pull_str);
                                    vm.libfiles = {};
                                    for (_i = 0, list_4 = list; _i < list_4.length; _i++) {
                                        file = list_4[_i];
                                        name_3 = file.split(/[\\/]/ig);
                                        name_3 = name_3[name_3.length - 1];
                                        vm.libfiles[$ts.baseName(name_3)] = file;
                                    }
                                    console.log("get lcms-library files:");
                                    console.table(vm.libfiles);
                                    vm.loadfiles();
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            lcmsLibrary.prototype.loadfiles = function () {
                var _this = this;
                var div = $('#lcms-libfiles');
                var root_dir = {
                    'text': 'LCMS Library',
                    'state': {
                        'opened': true,
                        'selected': true
                    },
                    'children': []
                };
                var libfiles = [];
                for (var _i = 0, _a = Object.keys(this.libfiles); _i < _a.length; _i++) {
                    var key = _a[_i];
                    libfiles.push({
                        text: key
                    });
                }
                root_dir.children = libfiles;
                div.empty();
                div.jstree({
                    'core': {
                        "animation": 0,
                        "check_callback": true,
                        'data': [root_dir]
                    },
                    "plugins": [
                        "contextmenu", "dnd", "search",
                        "state", "types", "wholerow"
                    ],
                    "contextmenu": { items: function (node) { return _this.customMenu(node); } }
                });
            };
            lcmsLibrary.prototype.customMenu = function (node) {
                var vm = this;
                // The default set of all items
                var items = {
                    openItem: this.menu_open(),
                    newItem: this.menu_new()
                };
                return items;
            };
            lcmsLibrary.prototype.menu_new = function () {
                var vm = this;
                return {
                    label: "New Library",
                    action: function (a) {
                        console.log("start to create a new library file!");
                        app.desktop.mzkit.NewLibrary()
                            .then(function (b) {
                            return __awaiter(this, void 0, void 0, function () {
                                var flag;
                                return __generator(this, function (_a) {
                                    switch (_a.label) {
                                        case 0: return [4 /*yield*/, b];
                                        case 1:
                                            flag = _a.sent();
                                            if (flag) {
                                                // reload the library list
                                                // vm.reloadLibs();
                                                location.reload();
                                            }
                                            return [2 /*return*/];
                                    }
                                });
                            });
                        });
                    }
                };
            };
            lcmsLibrary.prototype.menu_open = function () {
                var vm = this;
                return {
                    label: "Open",
                    action: function (a) {
                        var n = a.reference[0];
                        var key = Strings.Trim(n.innerText);
                        var filepath = vm.libfiles[key];
                        lcmsLibrary.openLibfile(filepath, vm);
                    }
                };
            };
            lcmsLibrary.openLibfile = function (filepath, vm) {
                if (vm === void 0) { vm = null; }
                lcmsLibrary.showLoader();
                if ((!vm) || typeof vm == "string") {
                    vm = Router.currentAppPage();
                }
                console.log("open a libfile:");
                // console.log(a);
                // console.log(key);
                console.log(filepath);
                app.desktop.mzkit.OpenLibrary(filepath)
                    .then(function (b) {
                    return __awaiter(this, void 0, void 0, function () {
                        var check;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, b];
                                case 1:
                                    check = _a.sent();
                                    if (check) {
                                        console.log("Open library file success!");
                                        vm.list_data();
                                    }
                                    else {
                                        console.log("Error while trying to open the LCMS library file!");
                                    }
                                    lcmsLibrary.hideLoader();
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            lcmsLibrary.prototype.list_data = function () {
                var vm = this;
                app.desktop.mzkit.GetPage(this.page, this.page_size)
                    .then(function (str) {
                    return __awaiter(this, void 0, void 0, function () {
                        var json, list;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, str];
                                case 1:
                                    json = _a.sent();
                                    list = JSON.parse(json);
                                    vm.show_page(list);
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            /**
             * .lib-id
            */
            lcmsLibrary.prototype.hookSpectralLinkOpen = function (liblinkClass, libsearchClass, libmassSearchClass) {
                $ts.select("." + liblinkClass).onClick(function (a) {
                    var data_id = a.getAttribute("data_id");
                    app.desktop.mzkit
                        .ShowSpectral(data_id)
                        .then(function (b) {
                        return __awaiter(this, void 0, void 0, function () {
                            var flag;
                            return __generator(this, function (_a) {
                                switch (_a.label) {
                                    case 0: return [4 /*yield*/, b];
                                    case 1:
                                        flag = _a.sent();
                                        if (flag) {
                                        }
                                        else {
                                        }
                                        return [2 /*return*/];
                                }
                            });
                        });
                    });
                });
                $ts.select("." + libsearchClass).onClick(function (a) {
                    var data_id = a.getAttribute("data_id");
                    lcmsLibrary.showLoader();
                    app.desktop.mzkit
                        .AlignSpectral(data_id)
                        .then(function (b) {
                        return __awaiter(this, void 0, void 0, function () {
                            var flag;
                            return __generator(this, function (_a) {
                                switch (_a.label) {
                                    case 0: return [4 /*yield*/, b];
                                    case 1:
                                        flag = _a.sent();
                                        if (flag) {
                                        }
                                        else {
                                        }
                                        lcmsLibrary.hideLoader();
                                        return [2 /*return*/];
                                }
                            });
                        });
                    });
                });
                $ts.select("." + libmassSearchClass).onClick(function (a) {
                    var mass = parseFloat(a.getAttribute("data"));
                    lcmsLibrary.showLoader();
                    app.desktop.mzkit
                        .FindExactMass(mass)
                        .then(function (b) {
                        return __awaiter(this, void 0, void 0, function () {
                            var flag;
                            return __generator(this, function (_a) {
                                switch (_a.label) {
                                    case 0: return [4 /*yield*/, b];
                                    case 1:
                                        flag = _a.sent();
                                        if (flag) {
                                        }
                                        else {
                                        }
                                        lcmsLibrary.hideLoader();
                                        return [2 /*return*/];
                                }
                            });
                        });
                    });
                });
            };
            lcmsLibrary.prototype.show_page = function (list) {
                var list_page = $ts("#list-page").clear();
                var liblinkClass = "lib-id";
                var libsearchClass = "lib-query";
                var libmassSearchClass = "lib-mass-query";
                console.log("get page data:");
                console.log(list);
                for (var _i = 0, list_5 = list; _i < list_5.length; _i++) {
                    var meta = list_5[_i];
                    var xrefs = "";
                    var xref_data = meta.xref || {};
                    for (var _a = 0, xref_keys_1 = xref_keys; _a < xref_keys_1.length; _a++) {
                        var key = xref_keys_1[_a];
                        var val = xref_data[key] || "";
                        if (Array.isArray(val)) {
                            val = val.join("; ");
                        }
                        if (!Strings.Empty(val, true)) {
                            // if (key == "SMILES" || key == "InChIkey" || key == "InChI") {
                            //     val = `<pre><code>${val}</code></pre>`;
                            // }
                            xrefs = xrefs + "<span>".concat(key, ": </span> ").concat(val, " <br />");
                        }
                    }
                    list_page.appendElement($ts("<div class='row'>").display("\n                <div class=\"span4\">\n                    <h5>".concat(meta.name, " [\n                        <a class=\"").concat(liblinkClass, "\" href=\"#\" onclick=\"javascript:void(0);\" data_id=\"").concat(meta.ID, "\">").concat(meta.ID, "</a> \n                        <a href=\"#\" class=\"fa-solid fa-magnifying-glass ").concat(libsearchClass, "\" data_id=\"").concat(meta.ID, "\" onclick=\"javascript:void(0);\"></a>\n                    ]</h5>\n                    <p>\n                    <span>Formula: </span> ").concat(meta.formula, " <br />\n                    <span>Exact Mass: </span> ").concat(meta.exact_mass, " <a href=\"#\" class=\"fa-solid fa-magnifying-glass ").concat(libmassSearchClass, "\" data=\"").concat(meta.exact_mass, "\" onclick=\"javascript:void(0);\"></a> <br />                       \n                    </p>\n                    <p>").concat(meta.description, "</p>\n                </div>\n                <div class=\"span4\">\n                    <p>\n                    ").concat(xrefs, "\n                    </p>\n                </div>\n                <div class=\"span4\">\n                    <canvas class=\"smiles-viewer\" id=\"").concat(meta.ID.replace(".", "_").replace(" ", "_"), "\" width=\"200\" height=\"150\" data=\"").concat(this.get_smiles(meta), "\">\n                    </canvas>\n                </div>\n                ")));
                }
                this.hookSpectralLinkOpen(liblinkClass, libsearchClass, libmassSearchClass);
                var options = {
                    width: 200,
                    height: 150
                };
                // Initialize the drawer to draw to canvas
                var smilesDrawer = new SmilesDrawer.Drawer(options);
                // Alternatively, initialize the SVG drawer:
                // let svgDrawer = new SmilesDrawer.SvgDrawer(options);
                $ts.select(".smiles-viewer")
                    .ForEach(function (a) {
                    var input_value = a.getAttribute("data");
                    var id = a.getAttribute("id");
                    if (!Strings.Empty(input_value, true)) {
                        // Clean the input (remove unrecognized characters, such as spaces and tabs) and parse it
                        SmilesDrawer.parse(input_value, function (tree) {
                            // Draw to the canvas
                            smilesDrawer.draw(tree, id, "light", false);
                            // Alternatively, draw to SVG:
                            // svgDrawer.draw(tree, 'output-svg', 'dark', false);
                        });
                    }
                });
            };
            lcmsLibrary.prototype.get_smiles = function (meta) {
                if (meta.xref) {
                    return meta.xref.SMILES || null;
                }
                else {
                    return null;
                }
            };
            lcmsLibrary.prototype.query_onclick = function () {
                var q = $ts.value("#get-query");
                var vm = this;
                app.desktop.mzkit.Query(q)
                    .then(function (str) {
                    return __awaiter(this, void 0, void 0, function () {
                        var json, list;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, str];
                                case 1:
                                    json = _a.sent();
                                    list = JSON.parse(json);
                                    vm.show_page(list);
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            return lcmsLibrary;
        }(Bootstrap));
        viewer.lcmsLibrary = lcmsLibrary;
        var xref_keys = ["chebi",
            "KEGG",
            "pubchem",
            "HMDB",
            "metlin",
            "DrugBank",
            "ChEMBL",
            "Wikipedia",
            "lipidmaps",
            "MeSH",
            "ChemIDplus",
            "MetaCyc",
            "KNApSAcK",
            "CAS",
            "InChIkey",
            "InChI",
            "SMILES"];
    })(viewer = apps.viewer || (apps.viewer = {}));
})(apps || (apps = {}));
var apps;
(function (apps) {
    var viewer;
    (function (viewer) {
        var LCMSScatterViewer = /** @class */ (function (_super) {
            __extends(LCMSScatterViewer, _super);
            function LCMSScatterViewer() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            Object.defineProperty(LCMSScatterViewer.prototype, "appName", {
                get: function () {
                    return "lcms-scatter";
                },
                enumerable: false,
                configurable: true
            });
            LCMSScatterViewer.prototype.create_viewer = function () {
                var _this = this;
                this.peaks3D = new gl_plot.echart_peak3D(function (x) { return x.id; }, function (x) { return [x.mz, x.scan_time, x.intensity]; }, function (x) { return x.intensity; }, function (arg, x) { return _this.label(arg, x); }, 'Scan Time(s)', "MZ");
                return this;
            };
            LCMSScatterViewer.prototype.label = function (arg, data) {
                // console.log(arg);
                var i = arg.dataIndex;
                var labels = data; // spot_labels.Item(arg.seriesName);
                var ms1 = arg.data;
                var rt = Math.round(ms1[0]);
                var mz = Strings.round(ms1[1]);
                var into = ms1[2].toExponential(2); // Math.pow(1.125, ms1[2]).toExponential(2);
                return "<".concat(labels[i].id, "> m/z: ").concat(mz, "@").concat(rt, "s intensity=").concat(into);
            };
            LCMSScatterViewer.prototype.init = function () {
                var vm = this.create_viewer();
                app.desktop.mzkit.GetLCMSScatter().then(function (data) {
                    return __awaiter(this, void 0, void 0, function () {
                        var json_str, scatter;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, data];
                                case 1:
                                    json_str = _a.sent();
                                    scatter = JSON.parse(json_str);
                                    app.desktop.mzkit.GetColors().then(function (ls) {
                                        return __awaiter(this, void 0, void 0, function () {
                                            var str, colors, _i, _a, code;
                                            return __generator(this, function (_b) {
                                                switch (_b.label) {
                                                    case 0: return [4 /*yield*/, ls];
                                                    case 1:
                                                        str = _b.sent();
                                                        colors = JSON.parse(str);
                                                        vm.peaks3D.colors = colors;
                                                        for (_i = 0, _a = vm.peaks3D.colors; _i < _a.length; _i++) {
                                                            code = _a[_i];
                                                            TypeScript.logging.log(code, code);
                                                        }
                                                        if (isNullOrEmpty(scatter)) {
                                                            vm.render3DScatter([]);
                                                        }
                                                        else {
                                                            vm.render3DScatter(scatter);
                                                        }
                                                        return [2 /*return*/];
                                                }
                                            });
                                        });
                                    });
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            LCMSScatterViewer.prototype.render3DScatter = function (dataset) {
                var _this = this;
                var render = new gl_plot.scatter3d(function (ls) { return _this.peaks3D.load_cluster(ls); }, "viewer");
                var div = $ts("#viewer");
                var vm = this;
                // render.chartObj.showLoading();
                render.plot(dataset);
                render.chartObj.on("click", function (par) {
                    // console.log(par);
                    var i = par.dataIndex;
                    var category = par.seriesName;
                    var labels = vm.peaks3D.layers.Item(category);
                    var spot_id = labels[i].id;
                    // console.log(spot_id);
                    // alert(spot_id);
                    app.desktop.mzkit.Click(spot_id);
                });
                var resize_canvas = function () {
                    var padding = 18;
                    div.style.width = (window.innerWidth - padding) + "px";
                    div.style.height = (window.innerHeight - padding) + "px";
                    render.chartObj.resize();
                };
                window.onresize = function () { return resize_canvas(); };
                resize_canvas();
            };
            return LCMSScatterViewer;
        }(Bootstrap));
        viewer.LCMSScatterViewer = LCMSScatterViewer;
    })(viewer = apps.viewer || (apps.viewer = {}));
})(apps || (apps = {}));
// openseadragon slide viewer
var apps;
(function (apps) {
    var viewer;
    (function (viewer_1) {
        var OpenseadragonSlideViewer = /** @class */ (function (_super) {
            __extends(OpenseadragonSlideViewer, _super);
            function OpenseadragonSlideViewer() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            Object.defineProperty(OpenseadragonSlideViewer.prototype, "appName", {
                get: function () {
                    return "openseadragon";
                },
                enumerable: false,
                configurable: true
            });
            OpenseadragonSlideViewer.prototype.getDziSrc = function () {
                return window.chrome.webview.hostObjects.dzi;
            };
            OpenseadragonSlideViewer.prototype.init = function () {
                return __awaiter(this, void 0, void 0, function () {
                    var dzi, OpenSeadragon;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0: return [4 /*yield*/, this.getDziSrc()];
                            case 1:
                                dzi = _a.sent();
                                OpenSeadragon = window.OpenSeadragon;
                                console.log("get the source deep zoom image url:");
                                console.log(dzi);
                                OpenseadragonSlideViewer.viewer = OpenSeadragon({
                                    id: "seadragon-viewer",
                                    prefixUrl: "/openseadragon/images/",
                                    tileSources: [
                                        dzi
                                    ],
                                    showNavigator: true,
                                    navigatorPosition: "BOTTOM_RIGHT",
                                    // Initial rotation angle
                                    degrees: 0,
                                    // Show rotation buttons
                                    showRotationControl: true,
                                    // Enable touch rotation on tactile devices
                                    gestureSettingsTouch: {
                                        pinchRotate: true
                                    }
                                });
                                return [2 /*return*/];
                        }
                    });
                });
            };
            OpenseadragonSlideViewer.ExportViewImage = function () {
                var viewer = OpenseadragonSlideViewer.viewer.drawer;
                var img = viewer.canvas.toDataURL("image/png");
                console.log("get web capture image exports:");
                console.log(img);
                DOM.download("capture.png", img, true);
            };
            return OpenseadragonSlideViewer;
        }(Bootstrap));
        viewer_1.OpenseadragonSlideViewer = OpenseadragonSlideViewer;
    })(viewer = apps.viewer || (apps.viewer = {}));
})(apps || (apps = {}));
var apps;
(function (apps) {
    var viewer;
    (function (viewer) {
        var svgViewer = /** @class */ (function (_super) {
            __extends(svgViewer, _super);
            function svgViewer() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            Object.defineProperty(svgViewer.prototype, "appName", {
                get: function () {
                    return "svg-viewer";
                },
                enumerable: false,
                configurable: true
            });
            svgViewer.prototype.init = function () {
                app.desktop.mzkit.Download().then(function (uri) {
                    return __awaiter(this, void 0, void 0, function () {
                        var url;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, uri];
                                case 1:
                                    url = _a.sent();
                                    console.log(url);
                                    svgViewer.setSvgUrl(url);
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            /**
             * @param url the base64 encoded svg image data
            */
            svgViewer.setSvgUrl = function (url) {
                $ts("#viewer").src = url;
            };
            return svgViewer;
        }(Bootstrap));
        viewer.svgViewer = svgViewer;
    })(viewer = apps.viewer || (apps.viewer = {}));
})(apps || (apps = {}));
var apps;
(function (apps) {
    var viewer;
    (function (viewer) {
        var umap = /** @class */ (function (_super) {
            __extends(umap, _super);
            function umap() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            Object.defineProperty(umap.prototype, "appName", {
                get: function () {
                    return "umap";
                },
                enumerable: false,
                configurable: true
            });
            umap.prototype.init = function () {
                app.desktop.mzkit.GetMatrixDims()
                    .then(function (json) {
                    return __awaiter(this, void 0, void 0, function () {
                        var str, ints;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, json];
                                case 1:
                                    str = _a.sent();
                                    ints = JSON.parse(str);
                                    $ts("#nfeatures").display(ints[0].toString());
                                    $ts("#nsamples").display(ints[1].toString());
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
                this.loadUMAP();
                this.selectMethod("kmeans");
            };
            umap.prototype.knn_onchange = function (val) {
                $ts("#knn-value").display(val);
            };
            umap.prototype.KnnIter_onchange = function (val) {
                $ts("#knnItr-value").display(val);
            };
            umap.prototype.localConnectivity_onchange = function (val) {
                $ts("#localConnect-value").display(val);
            };
            umap.prototype.bandwidth_onchange = function (val) {
                $ts("#bandwidth-value").display(val);
            };
            umap.prototype.learningRate_onchange = function (val) {
                $ts("#learningRate-value").display(val);
            };
            umap.prototype.kmeans_onchange = function (val) {
                $ts("#kmeans-value").display(val);
            };
            umap.prototype.min_pts_onchange = function (val) {
                $ts("#min_pts-value").display(val);
            };
            umap.prototype.eps_onchange = function (val) {
                $ts("#eps-value").display(val);
            };
            umap.prototype.identical_onchange = function (val) {
                $ts("#identical-value").display(val);
            };
            umap.prototype.run_umap_onclick = function () {
                var vm = this;
                vm.showSpinner();
                $goto("#spinner");
                app.desktop.mzkit.Run(parseInt($ts.value("#knn").toString()), parseInt($ts.value("#KnnIter").toString()), parseFloat($ts.value("#localConnectivity").toString()), parseFloat($ts.value("#bandwidth").toString()), parseFloat($ts.value("#learningRate").toString()), parseBoolean($ts.value("#spectral_cos").toString())).then(function (b) {
                    return __awaiter(this, void 0, void 0, function () {
                        var flag;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, b];
                                case 1:
                                    flag = _a.sent();
                                    if (flag) {
                                        vm.loadUMAP();
                                    }
                                    else {
                                    }
                                    vm.hideSpinner();
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            umap.prototype.run_kmeans_onclick = function () {
                var vm = this;
                var bisecting_kmeans = parseBoolean($ts.value("#bisecting_kmeans"));
                var k = parseInt($ts.value("#kmeans").toString());
                console.log("Bisecting K-Means:");
                console.log($ts.value("#bisecting_kmeans"));
                vm.showSpinner();
                $goto("#spinner");
                app.desktop.mzkit
                    .RunKmeans(k, bisecting_kmeans)
                    .then(function (b) {
                    return __awaiter(this, void 0, void 0, function () {
                        var flag;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, b];
                                case 1:
                                    flag = _a.sent();
                                    if (flag) {
                                        vm.loadUMAP();
                                    }
                                    vm.hideSpinner();
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            umap.prototype.run_graph_onclick = function () {
                var vm = this;
                vm.showSpinner();
                $goto("#spinner");
                app.desktop.mzkit
                    .RunGraph(parseFloat($ts.value("#identical").toString()))
                    .then(function (b) {
                    return __awaiter(this, void 0, void 0, function () {
                        var flag;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, b];
                                case 1:
                                    flag = _a.sent();
                                    if (flag) {
                                        vm.loadUMAP();
                                    }
                                    vm.hideSpinner();
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            umap.prototype.run_dbscan_onclick = function () {
                var vm = this;
                vm.showSpinner();
                $goto("#spinner");
                app.desktop.mzkit
                    .RunDbScan(parseInt($ts.value("#min_pts").toString()), parseFloat($ts.value("#eps").toString()))
                    .then(function (b) {
                    return __awaiter(this, void 0, void 0, function () {
                        var flag;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, b];
                                case 1:
                                    flag = _a.sent();
                                    if (flag) {
                                        vm.loadUMAP();
                                    }
                                    vm.hideSpinner();
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            umap.prototype.showSpinner = function () {
                document.getElementById('spinner')
                    .style.display = 'block';
                $ts("#manifold").interactive(false);
                $ts("#clustering").interactive(false);
            };
            umap.prototype.hideSpinner = function () {
                document.getElementById('spinner')
                    .style.display = 'none';
                $ts("#manifold").interactive(true);
                $ts("#clustering").interactive(true);
            };
            umap.prototype.download_onclick = function () {
                app.desktop.mzkit.Download().then(function (str) {
                    return __awaiter(this, void 0, void 0, function () {
                        var csv, data;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, str];
                                case 1:
                                    csv = _a.sent();
                                    data = {
                                        mime_type: "plain/text",
                                        data: Base64.encode(csv)
                                    };
                                    if (!Strings.Empty(csv, true)) {
                                        DOM.download("umap.csv", data, false);
                                    }
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            umap.prototype.save_onclick = function () {
                var vm = this;
                this.showSpinner();
                app.desktop.mzkit.Save().then(function () {
                    return __awaiter(this, void 0, void 0, function () {
                        return __generator(this, function (_a) {
                            console.log("done!");
                            vm.hideSpinner();
                            return [2 /*return*/];
                        });
                    });
                });
            };
            umap.prototype.loadUMAP = function () {
                app.desktop.mzkit.GetUMAPFile()
                    .then(function (str) {
                    return __awaiter(this, void 0, void 0, function () {
                        var filepath;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, str];
                                case 1:
                                    filepath = _a.sent();
                                    console.log(filepath);
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
                app.desktop.mzkit.GetScatter()
                    .then(function (str) {
                    return __awaiter(this, void 0, void 0, function () {
                        var json, scatter;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, str];
                                case 1:
                                    json = _a.sent();
                                    scatter = JSON.parse(json);
                                    if (isNullOrEmpty(scatter)) {
                                        viewer.clusterViewer.render3DScatter([], false);
                                    }
                                    else {
                                        viewer.clusterViewer.render3DScatter(scatter, false);
                                    }
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
            };
            umap.prototype.kmeans_method_onchange = function (val) {
                this.selectMethod($ts.select.getOption(".select-method"));
            };
            umap.prototype.dbscan_method_onchange = function (val) {
                this.selectMethod($ts.select.getOption(".select-method"));
            };
            umap.prototype.graph_method_onchange = function (val) {
                this.selectMethod($ts.select.getOption(".select-method"));
            };
            umap.prototype.selectMethod = function (method) {
                for (var _i = 0, _a = ["kmean-card", "dbscan-card", "graph-card"]; _i < _a.length; _i++) {
                    var id = _a[_i];
                    $ts("#".concat(id)).interactive(false);
                }
                switch (method) {
                    case "kmeans":
                        $ts("#kmean-card").interactive(true);
                        break;
                    case "dbscan":
                        $ts("#dbscan-card").interactive(true);
                        break;
                    case "graph":
                        $ts("#graph-card").interactive(true);
                        break;
                }
            };
            return umap;
        }(Bootstrap));
        viewer.umap = umap;
    })(viewer = apps.viewer || (apps.viewer = {}));
})(apps || (apps = {}));
//# sourceMappingURL=mzkit_desktop.js.map