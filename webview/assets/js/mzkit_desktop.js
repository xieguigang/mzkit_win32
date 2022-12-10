/// <reference path="../d/three/index.d.ts" />
var apps;
(function (apps) {
    class three_app extends Bootstrap {
        get appName() {
            return "3d_three";
        }
        initGui() {
            //声明一个保存需求修改的相关数据的对象
            this.gui = {};
            var datGui = new dat.GUI();
            //将设置属性添加到gui当中，gui.add(对象，属性，最小值，最大值）
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
        initModel() {
            //轴辅助 （每一个轴的长度）
            var object = new THREE.AxesHelper(500);
            this.scene.add(object);
            //创建THREE.PointCloud粒子的容器
            var geometry = new THREE.Geometry();
            //创建THREE.PointCloud纹理
            var material = new THREE.PointCloudMaterial({ size: 4, vertexColors: true, color: 0xffffff });
            //循环将粒子的颜色和位置添加到网格当中
            for (var x = -5; x <= 5; x++) {
                for (var y = -5; y <= 5; y++) {
                    var particle = new THREE.Vector3(x * 10, y * 10, 0);
                    geometry.vertices.push(particle);
                    geometry.colors.push(new THREE.Color(+three_app.randomColor()));
                }
            }
            //实例化THREE.PointCloud
            var cloud = new THREE.PointCloud(geometry, material);
            this.scene.add(cloud);
        }
        //随机生成颜色
        static randomColor() {
            var arrHex = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f"], strHex = "0x", index;
            for (var i = 0; i < 6; i++) {
                index = Math.round(Math.random() * 15);
                strHex += arrHex[index];
            }
            return strHex;
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
            this.initRender();
            this.initScene();
            this.initCamera();
            this.initLight();
            this.initModel();
            this.initControls();
            this.initStats();
            this.initGui();
            this.animate();
            window.onresize = () => this.onWindowResize();
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
        function run() {
            Router.AddAppHandler(new apps.three_app());
        }
        desktop.run = run;
    })(desktop = app.desktop || (app.desktop = {}));
})(app || (app = {}));
$ts.mode = Modes.debug;
$ts(app.desktop.run);
//# sourceMappingURL=mzkit_desktop.js.map