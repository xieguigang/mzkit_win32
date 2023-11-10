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
        var window = globalThis.window;
        var Potree = window.Potree;
        var proj4 = window.proj4;
        // D:/mzkit/dist/bin/Rstudio/bin/Rserve.exe --listen /wwwroot "D:\mzkit\dist\bin/../../src/mzkit/webview/" /port 6001 --parent=8124 --attach F:/Temp/mzkit_win32/9465ce9a7904ba9fa5a354804734cbc4
        var three_app = /** @class */ (function (_super) {
            __extends(three_app, _super);
            function three_app() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            Object.defineProperty(three_app.prototype, "appName", {
                get: function () {
                    return "three-3d";
                },
                enumerable: true,
                configurable: true
            });
            three_app.prototype.init = function () {
                var _this = this;
                this.potreeViewer = new Potree.Viewer(document.getElementById("potree_render_area"), {
                    useDefaultRenderLoop: false
                });
                this.potreeViewer.setEDLEnabled(true);
                this.potreeViewer.setFOV(60);
                this.potreeViewer.setPointBudget(3000000);
                this.potreeViewer.setBackground(null);
                this.potreeViewer.setDescription("");
                this.potreeViewer.loadGUI(function () {
                    _this.potreeViewer.setLanguage('en');
                    $("#menu_appearance").next().show();
                    $("#menu_tools").next().show();
                    $("#menu_scene").next().show();
                    _this.potreeViewer.toggleSidebar();
                });
                // CA13
                Potree.loadPointCloud("/cloud.js", "model", function (e) { return _this.loadModel(e); });
                requestAnimationFrame(function (t) { return _this.loop(t); });
            };
            three_app.prototype.loop = function (timestamp) {
                var _this = this;
                requestAnimationFrame(function (t) { return _this.loop(t); });
                this.potreeViewer.update(this.potreeViewer.clock.getDelta(), timestamp);
                this.potreeViewer.render();
            };
            three_app.prototype.createAnnotations = function () {
                var aRoot = this.potreeViewer.scene.annotations;
                var aCA13 = new Potree.Annotation({
                    title: "CA13",
                    position: [675036.45, 3850315.78, 65076.70],
                    cameraPosition: [675036.45, 3850315.78, 65076.70],
                    cameraTarget: [692869.03, 3925774.14, 1581.51],
                });
                aRoot.add(aCA13);
                var aSanSimeon = new Potree.Annotation({
                    title: "San Simeon",
                    position: [664147.50, 3946008.73, 16.30],
                    cameraPosition: [664941.80, 3943568.06, 1925.30],
                    cameraTarget: [664147.50, 3946008.73, 16.30],
                });
                aCA13.add(aSanSimeon);
                var aHearstCastle = new Potree.Annotation({
                    title: "Hearst Castle",
                    position: [665744.56, 3950567.52, 500.48],
                    cameraPosition: [665692.66, 3950521.65, 542.02],
                    cameraTarget: [665744.56, 3950567.52, 500.48],
                });
                aCA13.add(aHearstCastle);
                var aMorroBay = new Potree.Annotation({
                    title: "Morro Bay",
                    position: [695483.33, 3916430.09, 25.75],
                    cameraPosition: [694114.65, 3911176.26, 3402.33],
                    cameraTarget: [695483.33, 3916430.09, 25.75],
                });
                aCA13.add(aMorroBay);
                var aMorroRock = new Potree.Annotation({
                    title: "Morro Rock",
                    position: [693729.66, 3916085.19, 90.35],
                    cameraPosition: [693512.77, 3915375.61, 342.33],
                    cameraTarget: [693729.66, 3916085.19, 90.35],
                });
                aMorroBay.add(aMorroRock);
                var aMorroBayMutualWaterCo = new Potree.Annotation({
                    title: "Morro Bay Mutual Water Co",
                    position: [694699.45, 3916425.75, 39.78],
                    cameraPosition: [694377.64, 3916289.32, 218.40],
                    cameraTarget: [694699.45, 3916425.75, 39.78],
                });
                aMorroBay.add(aMorroBayMutualWaterCo);
                var aLilaKeiserPark = new Potree.Annotation({
                    title: "Lila Keiser Park",
                    position: [694674.99, 3917070.49, 10.86],
                    cameraPosition: [694452.59, 3916845.14, 298.64],
                    cameraTarget: [694674.99, 3917070.49, 10.86],
                });
                aMorroBay.add(aLilaKeiserPark);
                var aSanLuisObispo = new Potree.Annotation({
                    title: "San Luis Obispo",
                    position: [712573.39, 3907588.33, 146.44],
                    cameraPosition: [711158.29, 3907019.82, 1335.89],
                    cameraTarget: [712573.39, 3907588.33, 146.44],
                });
                aCA13.add(aSanLuisObispo);
                var aLopezHill = new Potree.Annotation({
                    title: "Lopez Hill",
                    position: [728635.63, 3895761.56, 456.33],
                    cameraPosition: [728277.24, 3895282.29, 821.51],
                    cameraTarget: [728635.63, 3895761.56, 456.33],
                });
                aCA13.add(aLopezHill);
                var aWhaleRockReservoir = new Potree.Annotation({
                    title: "Whale Rock Reservoir",
                    position: [692845.46, 3925528.53, 140.91],
                    cameraPosition: [693073.32, 3922354.02, 2154.17],
                    cameraTarget: [692845.46, 3925528.53, 140.91],
                });
                aCA13.add(aWhaleRockReservoir);
            };
            three_app.prototype.createVolume = function (scene, material) {
                var _this = this;
                var aRoot = scene.annotations;
                var elTitle = $("\n            <span>\n                Tree Returns:\n                <img name=\"action_return_number\" src=\"" + Potree.resourcePath + "/icons/return_number.svg\" class=\"annotation-action-icon\"/>\n                <img name=\"action_rgb\" src=\"" + Potree.resourcePath + "/icons/rgb.png\" class=\"annotation-action-icon\"/>\n            </span>");
                elTitle.find("img[name=action_return_number]").click(function () {
                    event.stopPropagation();
                    material.activeAttributeName = "return_number";
                    material.pointSizeType = Potree.PointSizeType.FIXED;
                    material.size = 5;
                    _this.potreeViewer.setClipTask(Potree.ClipTask.SHOW_INSIDE);
                });
                elTitle.find("img[name=action_rgb]").click(function () {
                    event.stopPropagation();
                    material.activeAttributeName = "rgba";
                    material.pointSizeType = Potree.PointSizeType.ADAPTIVE;
                    material.size = 1;
                    _this.potreeViewer.setClipTask(Potree.ClipTask.HIGHLIGHT);
                });
                elTitle.toString = function () { return "Tree Returns"; };
                var aTreeReturns = new Potree.Annotation({
                    title: elTitle,
                    position: [675756.75, 3937590.94, 80.21],
                    cameraPosition: [675715.78, 3937700.36, 115.95],
                    cameraTarget: [675756.75, 3937590.94, 80.21],
                });
                aRoot.add(aTreeReturns);
                aTreeReturns.domElement.find(".annotation-action-icon:first").css("filter", "invert(1)");
                var volume = new Potree.BoxVolume();
                volume.position.set(675755.4039368022, 3937586.911614576, 85);
                volume.scale.set(119.87189835418388, 68.3925257233834, 51.757483718373265);
                volume.rotation.set(0, 0, 0.8819755090987993, "XYZ");
                volume.clip = true;
                volume.visible = false;
                volume.name = "Trees";
                scene.addVolume(volume);
            };
            three_app.prototype.loadModel = function (e) {
                var pointcloud = e.pointcloud;
                var scene = this.potreeViewer.scene;
                var material = pointcloud.material;
                scene.addPointCloud(pointcloud);
                material.size = 1;
                material.pointSizeType = Potree.PointSizeType.ADAPTIVE;
                this.potreeViewer.scene.view.setView([675036.45, 3850315.78, 65076.70], [692869.03, 3925774.14, 1581.51]);
                var pointcloudProjection = e.pointcloud.projection;
                var mapProjection = proj4.defs("WGS84");
                window.toMap = proj4(pointcloudProjection, mapProjection);
                window.toScene = proj4(mapProjection, pointcloudProjection);
                // ANNOTATIONS
                this.createAnnotations();
                // TREE RETURNS POI - ANNOTATION & VOLUME
                this.createVolume(scene, material);
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
            // data analysis & data visualization
            Router.AddAppHandler(new apps.viewer.three_app());
            Router.AddAppHandler(new apps.viewer.clusterViewer());
            Router.AddAppHandler(new apps.viewer.LCMSScatterViewer());
            Router.AddAppHandler(new apps.viewer.OpenseadragonSlideViewer());
            Router.AddAppHandler(new apps.viewer.umap());
            Router.AddAppHandler(new apps.viewer.lcmsLibrary());
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
            /**
             * tick loop frame
            */
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
                var x = __spreadArrays(cpu.Counter).map(function (_, index) { return index + 1; });
                panel.display($ts("<h3>").display(cpu.svr.Name));
                panel.appendElement($ts("<p>").display(cpu.svr.Description));
                panel.appendElement($ts("<p>").display(cpu.svr.StartTime));
                panel.appendElement($ts("<p>").display("Startup: <pre><code>" + cpu.svr.CommandLine + "</code></pre>"));
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
                            var f = arg.data;
                            var r = $from(f).Select(function (n) { return Strings.round(n, 4); }).ToArray();
                            return arg.seriesName + " spot:<" + labels[i] + "> scatter3:" + JSON.stringify(r);
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
                enumerable: true,
                configurable: true
            });
            lcmsLibrary.prototype.init = function () {
                // this.reloadLibs();
                try {
                    this.list_data();
                }
                catch (ex) {
                }
            };
            lcmsLibrary.prototype.reloadLibs = function () {
                var vm = this;
                app.desktop.mzkit.ScanLibraries()
                    .then(function (str) {
                    return __awaiter(this, void 0, void 0, function () {
                        var pull_str, list, _i, list_3, file, name_1;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0: return [4 /*yield*/, str];
                                case 1:
                                    pull_str = _a.sent();
                                    list = JSON.parse(pull_str);
                                    vm.libfiles = {};
                                    for (_i = 0, list_3 = list; _i < list_3.length; _i++) {
                                        file = list_3[_i];
                                        name_1 = file.split(/[\\/]/ig);
                                        name_1 = name_1[name_1.length - 1];
                                        vm.libfiles[$ts.baseName(name_1)] = file;
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
            lcmsLibrary.prototype.hookSpectralLinkOpen = function (liblinkClass, libsearchClass) {
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
                console.log("get page data:");
                console.log(list);
                for (var _i = 0, list_4 = list; _i < list_4.length; _i++) {
                    var meta = list_4[_i];
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
                            xrefs = xrefs + ("<span>" + key + ": </span> " + val + " <br />");
                        }
                    }
                    list_page.appendElement($ts("<div class='row'>").display("\n                <div class=\"span4\">\n                    <h5>" + meta.name + " [\n                        <a class=\"" + liblinkClass + "\" href=\"#\" onclick=\"javascript:void(0);\" data_id=\"" + meta.ID + "\">" + meta.ID + "</a> \n                        <a href=\"#\" class=\"fa-solid fa-magnifying-glass " + libsearchClass + "\" data_id=\"" + meta.ID + "\" onclick=\"javascript:void(0);\"></a>\n                    ]</h5>\n                    <p>\n                    <span>Formula: </span> " + meta.formula + " <br />\n                    <span>Exact Mass: </span> " + meta.exact_mass + " <br />                       \n                    </p>\n                    <p>" + meta.description + "</p>\n                </div>\n                <div class=\"span4\">\n                    <p>\n                    " + xrefs + "\n                    </p>\n                </div>\n                <div class=\"span4\">\n                    <canvas class=\"smiles-viewer\" id=\"" + meta.ID.replace(".", "_").replace(" ", "_") + "\" width=\"200\" height=\"150\" data=\"" + this.get_smiles(meta) + "\">\n                    </canvas>\n                </div>\n                "));
                }
                this.hookSpectralLinkOpen(liblinkClass, libsearchClass);
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
                                            var str, colors, _i, _a, code;
                                            return __generator(this, function (_b) {
                                                switch (_b.label) {
                                                    case 0: return [4 /*yield*/, ls];
                                                    case 1:
                                                        str = _b.sent();
                                                        colors = JSON.parse(str);
                                                        vm.colors = colors;
                                                        vm.layers = new Dictionary();
                                                        for (_i = 0, _a = vm.colors; _i < _a.length; _i++) {
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
                var render = new gl_plot.scatter3d(function (ls) { return _this.load_cluster(ls); }, "viewer");
                var div = $ts("#viewer");
                var vm = this;
                // render.chartObj.showLoading();
                render.plot(dataset);
                render.chartObj.on("click", function (par) {
                    // console.log(par);
                    var i = par.dataIndex;
                    var category = par.seriesName;
                    var labels = vm.layers.Item(category);
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
                    name: "Intensity " + label,
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
                        show: false
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
                    var color = this_1.colors[i++];
                    var label = min.toExponential(1) + " ~ " + l0.toExponential(1);
                    this_1.layers.Add("Intensity " + label, subset);
                    scatter3D.push(LCMSScatterViewer.scatter_group(subset, color, label));
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
                            panMouseButton: 'right',
                            rotateMouseButton: 'left',
                            alpha: 30 // 让canvas在x轴有一定的倾斜角度
                        },
                        postEffect: {
                            enable: false,
                            SSAO: {
                                radius: 1,
                                intensity: 1,
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
                    xAxis3D: { type: 'value', name: 'Scan Time(s)', color: "white" },
                    yAxis3D: { type: 'value', name: 'M/Z', color: "white" },
                    zAxis3D: {
                        type: 'value', name: 'Intensity', color: "white",
                        axisLabel: {
                            formatter: function (value) {
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
                            }
                        }
                    },
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
                            var i = arg.dataIndex;
                            var labels = data; // spot_labels.Item(arg.seriesName);
                            var ms1 = arg.data;
                            var rt = Math.round(ms1[0]);
                            var mz = Strings.round(ms1[1]);
                            var into = ms1[2].toExponential(2); // Math.pow(1.125, ms1[2]).toExponential(2);
                            return "<" + labels[i].id + "> m/z: " + mz + "@" + rt + "s intensity=" + into;
                        }
                    },
                };
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
                enumerable: true,
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
        var umap = /** @class */ (function (_super) {
            __extends(umap, _super);
            function umap() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            Object.defineProperty(umap.prototype, "appName", {
                get: function () {
                    return "umap";
                },
                enumerable: true,
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
                vm.showSpinner();
                $goto("#spinner");
                app.desktop.mzkit
                    .RunKmeans(parseInt($ts.value("#kmeans").toString()))
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
                    $ts("#" + id).interactive(false);
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