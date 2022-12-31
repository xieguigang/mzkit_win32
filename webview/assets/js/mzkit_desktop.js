var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
/// <reference path="../d/three/index.d.ts" />
var apps;
(function (apps) {
    class three_app extends Bootstrap {
        get appName() {
            return "3d_three";
        }
        initControls() {
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
        }
        initStats() {
            this.stats = new Stats();
            document.body.appendChild(this.stats.dom);
        }
        initRender() {
            this.renderer = new THREE.WebGLRenderer({ antialias: true });
            //renderer.setClearColor(new THREE.Color(0xEEEEEE, 1.0)); //设置背景颜色
            this.renderer.setSize(window.innerWidth, window.innerHeight);
            document.body.appendChild(this.renderer.domElement);
        }
        initCamera() {
            this.camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 1, 10000);
            this.camera.position.set(0, 0, 200);
        }
        initScene() {
            this.scene = new THREE.Scene();
        }
        initLight() {
            this.scene.add(new THREE.AmbientLight(0x404040));
            this.light = new THREE.DirectionalLight(0xffffff);
            this.light.position.set(1, 1, 1);
            this.scene.add(this.light);
        }
        initModel(model) {
            console.log("load 3d point cloud model!");
            console.log(model);
            model.loadPointCloudModel(this);
        }
        render() {
            this.renderer.render(this.scene, this.camera);
        }
        //窗口变动触发的函数
        onWindowResize() {
            this.camera.aspect = window.innerWidth / window.innerHeight;
            this.camera.updateProjectionMatrix();
            this.render();
            this.renderer.setSize(window.innerWidth, window.innerHeight);
        }
        animate() {
            //更新控制器
            this.controls.update();
            this.render();
            //更新性能插件
            this.stats.update();
            requestAnimationFrame(() => this.animate());
        }
        init() {
            const vm = this;
            if (app.desktop.mzkit) {
                app.desktop.mzkit
                    .get_3d_MALDI_url()
                    .then(function (url) {
                    return __awaiter(this, void 0, void 0, function* () {
                        url = yield url;
                        vm.setup_device(url);
                    });
                });
                three_app.open = function () {
                    app.desktop.mzkit
                        .open_MALDI_model()
                        .then(function () {
                        return __awaiter(this, void 0, void 0, function* () {
                            vm.init();
                        });
                    });
                };
            }
            else {
                $ts("#init-logo").show();
            }
            window.onresize = () => this.onWindowResize();
        }
        setup_device(url) {
            const vm = this;
            HttpHelpers.getBlob(url, function (buffer) {
                try {
                    const model = new ModelReader(buffer);
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
        }
    }
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
class ModelReader {
    /**
     * @param bin the data should be in network byte order
    */
    constructor(bin) {
        this.bin = bin;
        this.pointCloud = [];
        this.palette = [];
        // npoints
        let view = new DataView(bin.buffer, 0, 8);
        let npoints = view.getInt32(0);
        // ncolors
        let ncolors = view.getInt32(4);
        // html color literal string array
        // is fixed size         
        // #rrggbb   
        let colorEnds = 8 + ncolors * 7;
        let stringBuf = bin.slice(8, colorEnds);
        let strings = String.fromCharCode.apply(null, stringBuf);
        for (let i = 0; i < ncolors; i++) {
            this.palette.push(strings.substring(i * 7, (i + 1) * 7));
        }
        view = new DataView(bin.buffer, colorEnds);
        for (let i = 0; i < npoints; i++) {
            let offset = i * (8 + 8 + 8 + 8 + 4);
            let x = view.getFloat64(offset) / 10;
            let y = view.getFloat64(offset + 8) / 10;
            let z = view.getFloat64(offset + 16) / 10;
            let data = view.getFloat64(offset + 24);
            let clr = view.getInt32(offset + 32);
            this.pointCloud.push({
                x: x, y: y, z: z,
                intensity: data,
                color: this.palette[clr]
            });
        }
        this.centroid();
        // this.cubic_scale();
    }
    cubic_scale() {
        const v = this.getVector3();
        const cubic = new data.NumericRange(-100, 100);
        const x = new data.NumericRange($ts(v.x).Min(), $ts(v.x).Max());
        const y = new data.NumericRange($ts(v.y).Min(), $ts(v.y).Max());
        const z = new data.NumericRange($ts(v.z).Min(), $ts(v.z).Max());
        const rad = 45 * Math.PI / 180;
        const cosa = Math.cos(rad);
        const sina = Math.sin(rad);
        let Xn = 0;
        let Zn = 0;
        let f = Math.pow(10, 15);
        for (let point of this.pointCloud) {
            point.x = x.ScaleMapping(point.x, cubic);
            point.y = y.ScaleMapping(point.y, cubic);
            point.z = z.ScaleMapping(point.z, cubic);
            Zn = point.z * cosa - point.x * sina;
            Xn = point.z * sina + point.x * cosa;
            point.x = Xn;
            point.z = Zn * f;
        }
    }
    centroid() {
        const v = this.getVector3();
        const offset_x = $ts(v.x).Sum() / this.pointCloud.length;
        const offset_y = $ts(v.y).Sum() / this.pointCloud.length;
        const offset_z = $ts(v.z).Sum() / this.pointCloud.length;
        for (let point of this.pointCloud) {
            point.x = point.x - offset_x;
            point.y = point.y - offset_y;
            point.z = point.z - offset_z;
        }
    }
    getVector3() {
        const x = [];
        const y = [];
        const z = [];
        for (let point of this.pointCloud) {
            x.push(point.x);
            y.push(point.y);
            z.push(point.z);
        }
        return {
            x: x,
            y: y,
            z: z
        };
    }
    loadPointCloudModel(canvas) {
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
        for (let point of this.pointCloud) {
            var particle = new THREE.Vector3(point.x, point.y, point.z);
            geometry.vertices.push(particle);
            geometry.colors.push(new THREE.Color(point.color));
        }
        //实例化THREE.PointCloud
        canvas.scene.add(new THREE.PointCloud(geometry, material));
    }
}
/// <reference path="../d/linq.d.ts" />
var apps;
(function (apps) {
    class home extends Bootstrap {
        get appName() {
            return "home";
        }
        init() {
            // throw new Error("Method not implemented.");
        }
    }
    apps.home = home;
})(apps || (apps = {}));
var apps;
(function (apps) {
    class pluginMgr extends Bootstrap {
        get appName() {
            return "pluginMgr";
        }
        ;
        init() {
        }
    }
    apps.pluginMgr = pluginMgr;
})(apps || (apps = {}));
//# sourceMappingURL=mzkit_desktop.js.map