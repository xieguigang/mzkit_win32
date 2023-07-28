var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
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
var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
/// <reference path="../../d/three/index.d.ts" />
var apps;
(function (apps) {
    var viewer;
    (function (viewer) {
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
        viewer.three_app = three_app;
    })(viewer = apps.viewer || (apps.viewer = {}));
})(apps || (apps = {}));
/// <reference path="./d/linq.d.ts" />
/// <reference path="./apps/viewer/three_app.ts" />
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
            // mzkit system pages
            Router.AddAppHandler(new apps.home());
            Router.AddAppHandler(new apps.systems.pluginMgr());
            Router.AddAppHandler(new apps.systems.pluginPkg());
            Router.AddAppHandler(new apps.systems.servicesManager());
            // data analysis & data visualization
            Router.AddAppHandler(new apps.viewer.three_app());
            Router.AddAppHandler(new apps.viewer.clusterViewer());
            Router.AddAppHandler(new apps.viewer.LCMSScatterViewer());
            Router.RunApp();
        }
        desktop.run = run;
    })(desktop = app.desktop || (app.desktop = {}));
})(app || (app = {}));
$ts.mode = Modes.development;
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
            enumerable: true,
            configurable: true
        });
        home.prototype.init = function () {
            var _this = this;
            $ts.getText(apps.biodeep_classroom, function (text) { return _this.showClassRoom(JSON.parse(text)); });
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
                var liItem = "\n                    <img class=\"news-pic\" src=\"" + item.imgUrl + "\" />\n                    <div class=\"news-txt\">\n                        <a href=\"" + sprintf(apps.biodeep_viewVideo, item.id) + "\">" + item.title + "</a>\n                        <span>" + item.createTime + "</span>\n                    </div>";
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
                var html = "\n            \n            <th scope=\"row\" class=\"check-column\">\n                <input type=\"checkbox\" name=\"check_plugins\" />\n            </th>\n            <td class=\"plugin-title column-primary\">\n                <strong><a href=\"#\" onclick=\"app.desktop.mzkit.Exec('" + plugin.id + "')\">" + plugin.name + "</a></strong>\n                <div class=\"row-actions visible\">\n                    " + action + "\n                </div>        \n            </td>\n            <td class=\"column-description desc\">\n                <div class=\"plugin-description\">\n                    <p>\n                        " + plugin.desc + "\n                    </p>\n                </div>\n                <div class=\"" + type + " second plugin-version-author-uri\">\n                    Version " + plugin.ver + " | By\n                    <a href=\"#\">" + plugin.author + "</a> |\n                    <a href=\"" + plugin.url + "\" class=\"thickbox open-plugin-details-modal\">View details</a>\n                </div>\n            </td>\n            <td class=\"column-auto-updates\">\n                \n            </td>     \n            ";
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
                enumerable: true,
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
                console.log("Build plugin package: " + dir + "!");
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
                enumerable: true,
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
            servicesManager.prototype.loadServicesList = function (list) {
                var vm = this;
                for (var _i = 0, list_2 = list; _i < list_2.length; _i++) {
                    var svr = list_2[_i];
                    var id = "P" + svr.PID;
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
                var x = __spreadArrays(cpu.Counter).map(function (_, index) { return index + 1; });
                panel.display($ts("<h3>").display(cpu.svr.Name));
                panel.appendElement($ts("<p>").display(cpu.svr.Description));
                panel.appendElement($ts("<p>").display(cpu.svr.StartTime));
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
                    var id = "P" + svr.PID;
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
                enumerable: true,
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
            clusterViewer.render3DScatter = function (dataset) {
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
                window.onresize = function () { return resize_canvas(); };
                resize_canvas();
            };
            clusterViewer.format_cluster_tag = function (data) {
                var class_labels = $from(data).Select(function (r) { return r.cluster; }).Distinct().ToArray();
                var numeric_cluster = $from(class_labels).All(function (si) { return Strings.isIntegerPattern(si.toString()); });
                return (function (r) {
                    return numeric_cluster ? "cluster_" + r.cluster : r.cluster.toString();
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
                        show: true,
                        trigger: 'item',
                        axisPointer: {
                            type: 'cross',
                        },
                        // showContent: true, //是否显示提示框浮层，默认显示。
                        // triggerOn: 'mouseover', // 触发时机'click'鼠标点击时触发。 
                        backgroundColor: 'white',
                        borderColor: '#333',
                        borderWidth: 0,
                        padding: 5,
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
                            return arg.seriesName + " spot:<" + labels[i] + "> scatter3:" + JSON.stringify(arg.data);
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
        var LCMSScatterViewer = /** @class */ (function (_super) {
            __extends(LCMSScatterViewer, _super);
            function LCMSScatterViewer() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            Object.defineProperty(LCMSScatterViewer.prototype, "appName", {
                get: function () {
                    return "lcms-scatter";
                },
                enumerable: true,
                configurable: true
            });
            LCMSScatterViewer.prototype.init = function () {
                var vm = this;
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
                                            var str, colors;
                                            return __generator(this, function (_a) {
                                                switch (_a.label) {
                                                    case 0: return [4 /*yield*/, ls];
                                                    case 1:
                                                        str = _a.sent();
                                                        colors = JSON.parse(str);
                                                        vm.colors = colors;
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
                var render = new gl_plot.scatter3d(function (ls) { return _this.load_cluster(ls); }, "viewer");
                var div = $ts("#viewer");
                render.plot(dataset);
                render.chartObj.on("click", function (par) {
                    // // console.log(par);
                    // const i = par.dataIndex;
                    // const category = par.seriesName;
                    // const labels = spot_labels.Item(category);
                    // const spot_id: string = labels[i];
                    // // console.log(spot_id);
                    // app.desktop.mzkit.Click(spot_id);
                });
                var resize_canvas = function () {
                    var padding = 25;
                    div.style.width = (window.innerWidth - padding) + "px";
                    div.style.height = (window.innerHeight - padding) + "px";
                    render.chartObj.resize();
                };
                window.onresize = function () { return resize_canvas(); };
                resize_canvas();
            };
            LCMSScatterViewer.scatter_group = function (data, color, label) {
                // const seq = $from(data);
                // const r_range = globalThis.data.NumericRange.Create(seq.Select(a => a.mz));
                // const g_range = globalThis.data.NumericRange.Create(seq.Select(a => a.scan_time));
                // const b_range = globalThis.data.NumericRange.Create(seq.Select(a => a.intensity));
                // const byte_range = new globalThis.data.NumericRange(0, 255);
                return {
                    type: 'bar3D',
                    shading: 'color',
                    barSize: 0.1,
                    name: "Intensity",
                    spot_labels: $from(data).Select(function (r) { return r.id; }).ToArray(),
                    symbolSize: 1,
                    dimensions: [
                        'Scan Time(s)',
                        'M/Z',
                        'Intensity'
                    ],
                    data: $from(data).Select(function (r) { return [r.scan_time, r.mz, r.intensity]; }).ToArray(),
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
                        show: true
                    }
                };
            };
            LCMSScatterViewer.prototype.load_cluster = function (data) {
                var seq = $from(data);
                var max = seq.Select(function (a) { return a.intensity; }).Max();
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
                    var subset = seq.Where(function (a) { return a.intensity > min && a.intensity < l0; }).ToArray();
                    scatter3D.push(LCMSScatterViewer.scatter_group(subset, this_1.colors[i++], min + " ~ " + l0));
                };
                var this_1 = this;
                for (var min = 0; min < max; min = min + d) {
                    _loop_1(min);
                }
                // const scatter3D = [LCMSScatterViewer.scatter_group(data)];
                return {
                    grid3D: {
                        axisPointer: {
                            show: false
                        },
                        viewControl: {
                            distance: 300,
                            beta: -30,
                            panMouseButton: 'left',
                            rotateMouseButton: 'right',
                            alpha: 50 // 让canvas在x轴有一定的倾斜角度
                        },
                        postEffect: {
                            enable: false,
                            SSAO: {
                                radius: 1,
                                intensity: 1,
                                enable: true
                            }
                        },
                        temporalSuperSampling: {
                            enable: true
                        },
                        boxDepth: 120,
                        light: {
                            main: {
                                shadow: true,
                                intensity: 10
                            },
                            ambientCubemap: {
                                texture: "/assets/canyon.hdr",
                                exposure: 2,
                                diffuseIntensity: 0.2,
                                specularIntensity: 1
                            }
                        }
                    },
                    backgroundColor: '#fff',
                    xAxis3D: { type: 'value', name: 'Scan Time(s)' },
                    yAxis3D: { type: 'value', name: 'M/Z' },
                    zAxis3D: { type: 'value', name: 'Intensity' },
                    series: scatter3D,
                    tooltip: {
                        show: true,
                        trigger: 'item',
                        axisPointer: {
                            type: 'cross',
                        },
                        // showContent: true, //是否显示提示框浮层，默认显示。
                        // triggerOn: 'mouseover', // 触发时机'click'鼠标点击时触发。 
                        backgroundColor: 'white',
                        borderColor: '#333',
                        borderWidth: 0,
                        padding: 5,
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
                            var labels = data; // spot_labels.Item(arg.seriesName);
                            var ms1 = arg.data;
                            var rt = Math.round(ms1[0]);
                            var mz = Strings.round(ms1[1]);
                            var into = ms1[2].toExponential(2); // Math.pow(1.125, ms1[2]).toExponential(2);
                            return arg.seriesName + " spot:<" + labels[i].id + "> m/z: " + mz + "@" + rt + "s intensity=" + into;
                        }
                    },
                    legend: {
                        orient: 'vertical',
                        x: 'right',
                        y: 'center'
                    }
                };
            };
            return LCMSScatterViewer;
        }(Bootstrap));
        viewer.LCMSScatterViewer = LCMSScatterViewer;
    })(viewer = apps.viewer || (apps.viewer = {}));
})(apps || (apps = {}));
//# sourceMappingURL=mzkit_desktop.js.map