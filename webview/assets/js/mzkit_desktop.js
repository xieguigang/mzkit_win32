var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
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
/// <reference path="../d/three/index.d.ts" />
var apps;
(function (apps) {
    var three_app = /** @class */ (function (_super) {
        __extends(three_app, _super);
        function three_app() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        Object.defineProperty(three_app.prototype, "appName", {
            get: function () {
                return "3d_three";
            },
            enumerable: true,
            configurable: true
        });
        three_app.prototype.initControls = function () {
            this.controls = new THREE.OrbitControls(this.camera, this.renderer.domElement);
            // 如果使用animate方法时，将此函数删除
            //controls.addEventListener( 'change', render );
            // 使动画循环使用时阻尼或自转 意思是否有惯性
            this.controls.enableDamping = true;
            //动态阻尼系数 就是鼠标拖拽旋转灵敏度
            //controls.dampingFactor = 0.25;
            //是否可以缩放
            this.controls.enableZoom = true;
            //是否自动旋转
            this.controls.autoRotate = false;
            //设置相机距离原点的最远距离
            this.controls.minDistance = 20;
            //设置相机距离原点的最远距离
            this.controls.maxDistance = 10000;
            //是否开启右键拖拽
            this.controls.enablePan = true;
        };
        three_app.prototype.initStats = function () {
            this.stats = new Stats();
            document.body.appendChild(this.stats.dom);
        };
        three_app.prototype.initRender = function () {
            this.renderer = new THREE.WebGLRenderer({ antialias: true });
            //renderer.setClearColor(new THREE.Color(0xEEEEEE, 1.0)); //设置背景颜色
            this.renderer.setSize(window.innerWidth, window.innerHeight);
            document.body.appendChild(this.renderer.domElement);
        };
        three_app.prototype.initCamera = function () {
            this.camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 1, 10000);
            this.camera.position.set(0, 0, 200);
        };
        three_app.prototype.initScene = function () {
            this.scene = new THREE.Scene();
        };
        three_app.prototype.initLight = function () {
            this.scene.add(new THREE.AmbientLight(0x404040));
            this.light = new THREE.DirectionalLight(0xffffff);
            this.light.position.set(1, 1, 1);
            this.scene.add(this.light);
        };
        three_app.prototype.initModel = function (model) {
            console.log("load 3d point cloud model!");
            console.log(model);
            model.loadPointCloudModel(this);
        };
        three_app.prototype.render = function () {
            this.renderer.render(this.scene, this.camera);
        };
        //窗口变动触发的函数
        three_app.prototype.onWindowResize = function () {
            this.camera.aspect = window.innerWidth / window.innerHeight;
            this.camera.updateProjectionMatrix();
            this.render();
            this.renderer.setSize(window.innerWidth, window.innerHeight);
        };
        three_app.prototype.animate = function () {
            var _this = this;
            //更新控制器
            this.controls.update();
            this.render();
            //更新性能插件
            this.stats.update();
            requestAnimationFrame(function () { return _this.animate(); });
        };
        three_app.prototype.init = function () {
            var _this = this;
            var vm = this;
            if (app.desktop.mzkit) {
                app.desktop.mzkit
                    .get_3d_MALDI_url()
                    .then(function (url) {
                    return __awaiter(this, void 0, void 0, function () {
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, url];
                                case 1:
                                    url = _a.sent();
                                    vm.setup_device(url);
                                    return [2 /*return*/];
                            }
                        });
                    });
                });
                three_app.open = function () {
                    app.desktop.mzkit
                        .open_MALDI_model()
                        .then(function () {
                        return __awaiter(this, void 0, void 0, function () {
                            return __generator(this, function (_a) {
                                vm.init();
                                return [2 /*return*/];
                            });
                        });
                    });
                };
            }
            else {
                $ts("#init-logo").show();
            }
            window.onresize = function () { return _this.onWindowResize(); };
        };
        three_app.prototype.setup_device = function (url) {
            var vm = this;
            HttpHelpers.getBlob(url, function (buffer) {
                try {
                    var model = new ModelReader(buffer);
                    vm.initRender();
                    vm.initScene();
                    vm.initCamera();
                    vm.initLight();
                    vm.initModel(model);
                    vm.initControls();
                    vm.initStats();
                    vm.animate();
                    $ts("#init-logo").hide();
                }
                catch (_a) {
                    // do nothing
                }
            });
        };
        return three_app;
    }(Bootstrap));
    apps.three_app = three_app;
})(apps || (apps = {}));
/// <reference path="./d/linq.d.ts" />
/// <reference path="./apps/three_app.ts" />
var app;
(function (app) {
    var desktop;
    (function (desktop) {
        desktop.mzkit = getHostObject();
        function getHostObject() {
            try {
                return window.chrome.webview.hostObjects.mzkit;
            }
            catch (_a) {
                return null;
            }
        }
        function run() {
            Router.AddAppHandler(new apps.home());
            Router.AddAppHandler(new apps.pluginMgr());
            Router.AddAppHandler(new apps.three_app());
            Router.AddAppHandler(new apps.clusterViewer());
            Router.RunApp();
        }
        desktop.run = run;
    })(desktop = app.desktop || (app.desktop = {}));
})(app || (app = {}));
$ts.mode = Modes.debug;
$ts(app.desktop.run);
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
var apps;
(function (apps) {
    var clusterViewer = /** @class */ (function (_super) {
        __extends(clusterViewer, _super);
        function clusterViewer() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        Object.defineProperty(clusterViewer.prototype, "appName", {
            get: function () {
                return "clusterViewer";
            },
            enumerable: true,
            configurable: true
        });
        ;
        clusterViewer.prototype.init = function () {
            // throw new Error("Method not implemented.");
        };
        return clusterViewer;
    }(Bootstrap));
    apps.clusterViewer = clusterViewer;
})(apps || (apps = {}));
/// <reference path="../d/linq.d.ts" />
var apps;
(function (apps) {
    var home = /** @class */ (function (_super) {
        __extends(home, _super);
        function home() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        Object.defineProperty(home.prototype, "appName", {
            get: function () {
                return "home";
            },
            enumerable: true,
            configurable: true
        });
        home.prototype.init = function () {
            // throw new Error("Method not implemented.");
        };
        return home;
    }(Bootstrap));
    apps.home = home;
})(apps || (apps = {}));
var apps;
(function (apps) {
    var pluginMgr = /** @class */ (function (_super) {
        __extends(pluginMgr, _super);
        function pluginMgr() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        Object.defineProperty(pluginMgr.prototype, "appName", {
            get: function () {
                return "pluginMgr";
            },
            enumerable: true,
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
            var action = type == "active" ? "<span class=\"deactivate\">\n            <a href=\"#\" class=\"deactive\" data=\"" + plugin.id + "\">Deactivate</a>\n        </span>" : "<span class=\"activate\">\n        <a href=\"#\" class=\"edit\" data=\"" + plugin.id + "\">Activate</a> <!--|\n    </span>\n    <span class=\"delete\">\n        <a href=\"#\" class=\"delete\" data=\"" + plugin.id + "\">Delete</a>\n    </span>-->";
            var html = "\n            \n            <th scope=\"row\" class=\"check-column\">\n                <input type=\"checkbox\" name=\"check_plugins\" />\n            </th>\n            <td class=\"plugin-title column-primary\">\n                <strong>" + plugin.name + "</strong>\n                <div class=\"row-actions visible\">\n                    " + action + "\n                </div>        \n            </td>\n            <td class=\"column-description desc\">\n                <div class=\"plugin-description\">\n                    <p>\n                        " + plugin.desc + "\n                    </p>\n                </div>\n                <div class=\"" + type + " second plugin-version-author-uri\">\n                    Version " + plugin.ver + " | By\n                    <a href=\"#\">" + plugin.author + "</a> |\n                    <a href=\"" + plugin.url + "\" class=\"thickbox open-plugin-details-modal\">View details</a>\n                </div>\n            </td>\n            <td class=\"column-auto-updates\">\n                \n            </td>     \n            ";
            mgr.appendChild(row.display(html));
        };
        pluginMgr.prototype.install_local_onclick = function () {
            app.desktop.mzkit.InstallLocal();
        };
        return pluginMgr;
    }(Bootstrap));
    apps.pluginMgr = pluginMgr;
})(apps || (apps = {}));
//# sourceMappingURL=mzkit_desktop.js.map