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

        private renderer: THREE.WebGLRenderer;
        private camera: THREE.PerspectiveCamera;
        private scene: THREE.Scene;

        private get canvas(): HTMLCanvasElement {
            return this.renderer.domElement;
        }

        protected init(): void {
            const vm = this;

            this.renderer = new THREE.WebGLRenderer({
                antialias: true
            });
            this.camera = new THREE.PerspectiveCamera(60, 1, 1, 1000);
            this.scene = new THREE.Scene();
            this.camera.position.setScalar(150);

            app.desktop.mzkit.GetLCMSScatter().then(async function (data) {
                const json_str: string = await data;
                const scatter: ms1_scatter[] = JSON.parse(json_str);

                if (isNullOrEmpty(scatter)) {
                    vm.render3DScatter([]);
                } else {
                    vm.render3DScatter(scatter);
                }
            });
        }

        public render3DScatter(dataset: ms1_scatter[]) {
            var canvas = this.renderer.domElement;
            var camera = this.camera;

            document.body.appendChild(canvas);

            var controls = new THREE.OrbitControls(camera, canvas);
            var light = new THREE.DirectionalLight(0xffffff, 1.5);

            light.position.setScalar(100);
            this.scene.add(light);
            this.scene.add(new THREE.AmbientLight(0xffffff, 0.5));

            var points3d: THREE.Vector3[] = [];

            for (let p of dataset) {
                points3d.push(new THREE.Vector3(p.mz, p.scan_time, p.intensity));
            }

            var geom = new THREE.BufferGeometry().setFromPoints(points3d);
            var cloud = new THREE.Points(
                geom,
                new THREE.PointsMaterial({ color: 0x99ccff, size: 2 })
            );
            this.scene.add(cloud);

            // triangulate x, z
            var indexDelaunay = Delaunator.from(
                points3d.map(v => {
                    return [v.x, v.z];
                })
            );

            var meshIndex = []; // delaunay index => three.js index
            for (let i = 0; i < indexDelaunay.triangles.length; i++) {
                meshIndex.push(indexDelaunay.triangles[i]);
            }

            geom.setIndex(meshIndex); // add three.js index to the existing geometry
            geom.computeVertexNormals();
            var mesh = new THREE.Mesh(
                geom, // re-use the existing geometry
                new THREE.MeshLambertMaterial({ color: "purple", wireframe: true })
            );
            this.scene.add(mesh);

            var gui = new dat.GUI();
            gui.add(mesh.material, "wireframe");

            this.render();
        }

        private render() {
            const canvas = this.canvas;
            const camera = this.camera;

            if (this.resize(this.renderer)) {
                camera.aspect = canvas.clientWidth / canvas.clientHeight;
                camera.updateProjectionMatrix();
            }

            this.renderer.render(this.scene, camera);

            requestAnimationFrame(() => this.render());
        }

        private resize(renderer: THREE.WebGLRenderer) {
            const canvas = renderer.domElement;
            const width = canvas.clientWidth;
            const height = canvas.clientHeight;
            const needResize = canvas.width !== width || canvas.height !== height;

            if (needResize) {
                renderer.setSize(width, height, false);
            }

            return needResize;
        }
    }
}