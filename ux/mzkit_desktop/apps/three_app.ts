/// <reference path="../d/three/index.d.ts" />

namespace apps {

    export class three_app extends Bootstrap {

        public get appName(): string {
            return "3d_three";
        }

        public scene: THREE.Scene;

        private renderer;
        private camera;
        private light;
        //初始化性能插件
        private stats;
        //用户交互插件 鼠标左键按住旋转，右键按住平移，滚轮缩放
        private controls;

        private initControls() {
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

        private initStats() {
            this.stats = new Stats();
            document.body.appendChild(this.stats.dom);
        }

        private initRender() {
            this.renderer = new THREE.WebGLRenderer({ antialias: true });
            //renderer.setClearColor(new THREE.Color(0xEEEEEE, 1.0)); //设置背景颜色
            this.renderer.setSize(window.innerWidth, window.innerHeight);

            document.body.appendChild(this.renderer.domElement);
        }

        private initCamera() {
            this.camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 1, 10000);
            this.camera.position.set(0, 0, 200);
        }

        private initScene() {
            this.scene = new THREE.Scene();
        }

        private initLight() {
            this.scene.add(new THREE.AmbientLight(0x404040));

            this.light = new THREE.DirectionalLight(0xffffff);
            this.light.position.set(1, 1, 1);
            this.scene.add(this.light);
        }

        private initModel(model: ModelReader) {
            console.log("load 3d point cloud model!");
            console.log(model);

            model.loadPointCloudModel(this);
        }

        private render() {
            this.renderer.render(this.scene, this.camera);
        }

        //窗口变动触发的函数
        private onWindowResize() {
            this.camera.aspect = window.innerWidth / window.innerHeight;
            this.camera.updateProjectionMatrix();
            this.render();
            this.renderer.setSize(window.innerWidth, window.innerHeight);
        }

        private animate() {
            //更新控制器
            this.controls.update();
            this.render();
            //更新性能插件
            this.stats.update();

            requestAnimationFrame(() => this.animate());
        }

        protected init(): void {
            const vm = this;

            if (app.desktop.mzkit) {
                app.desktop.mzkit
                    .get_3d_MALDI_url()
                    .then(async function (url) {
                        url = await url;
                        vm.setup_device(url);
                    });

                three_app.open = function () {
                    app.desktop.mzkit
                        .open_MALDI_model()
                        .then(async function () {
                            vm.init();
                        });
                }
            } else {
                $ts("#init-logo").show();
            }

            window.onresize = () => this.onWindowResize();
        }

        private setup_device(url: string) {
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
                } catch {
                    // do nothing
                }
            });
        }

        public static open: Delegate.Action;
    }
}