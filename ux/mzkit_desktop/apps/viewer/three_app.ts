/// <reference path="../../d/three/index.d.ts" />

namespace apps.viewer {

    const window = <any>globalThis.window;

    export const Empty: readonly any[] = Object.freeze([]);

    export interface OrbitControls { }

    export interface Stats {
        get dom(): HTMLElement;

        begin(): void;
        end(): void;
    }

    export interface GUI {
        add(volconfig: volconfig, name: string, arg2?: any, arg3?: any, arg4?: any): any;
        addFolder(name: string): GUI;
    }

    export interface volconfig {
        // render
        clim1: number; clim2: number; renderstyle: string; isothreshold: number; colormap: string;
        showAxis: boolean;
        // controls
        enableDamping: boolean;
        // cliping
        enableClipping: boolean;
        clip_x: number;
        clip_y: number;
        clip_z: number;
    }

    export interface NRRDLoader { }

    const cmtextures: string[] = ["gray", "viridis", "jet", "rainbow", "typhoon", "magma", "plasma", "mako", "rocket", "turbo"];

    export function cm_names() {
        let names = {};

        for (let name of cmtextures) {
            names[name] = name;
        }

        return names;
    }

    export function cm_textures(callback: Delegate.Action) {
        let textures = {};

        for (let name of cmtextures) {
            textures[name] = new THREE.TextureLoader().load(`/vendor/three/textures/cm_${name}.png`, () => callback());
        }

        return textures;
    }

    export class three_app extends Bootstrap {

        public renderer: THREE.WebGLRenderer;
        public scene: THREE.Scene;
        public camera: THREE.OrthographicCamera;
        public controls: OrbitControls;
        public material: THREE.ShaderMaterial;
        public volconfig: volconfig;
        public cmtextures;
        public stats: Stats;
        public planes: THREE.Plane[];
        public axesHelper: THREE.AxesHelper;

        public get appName(): string {
            return "three-3d";
        }

        protected init(): void {
            const scene = new THREE.Scene();
            const vm = this;

            this.planes = [
                new THREE.Plane(new THREE.Vector3(- 1, 0, 0), 0),
                new THREE.Plane(new THREE.Vector3(0, - 1, 0), 0),
                new THREE.Plane(new THREE.Vector3(0, 0, - 1), 0)
            ];

            // Create renderer
            const renderer = new THREE.WebGLRenderer({ antialias: false });
            renderer.setPixelRatio(window.devicePixelRatio);
            renderer.setSize(window.innerWidth, window.innerHeight);
            renderer.localClippingEnabled = true;
            // GUI sets it to globalPlanes
            renderer.clippingPlanes = this.planes;

            document.body.appendChild(renderer.domElement);
            console.log(renderer);

            // Create camera (The volume renderer does not work very well with perspective yet)
            const h = 512; // frustum height
            const aspect = window.innerWidth / window.innerHeight;
            const left_1 = h * aspect / 2;
            const camera = new THREE.OrthographicCamera(- left_1, left_1, h / 2, - h / 2, 1, 1000);
            camera.position.set(-128, -128, 0);
            camera.up.set(0, 0, 1); // In our data, z is up

            console.log(camera);

            this.scene = scene;
            this.renderer = renderer;
            this.camera = camera;

            // Create controls
            const controls = new window.OrbitControls(camera, renderer.domElement);
            controls.addEventListener('change', () => this.render());
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
            const volconfig = {
                clim1: 0, clim2: 1, renderstyle: 'iso', isothreshold: 0.15, colormap: 'jet',
                left: camera.left, right: camera.right, top: camera.top, bottom: camera.bottom,
                showAxis: true,
                enableDamping: controls.enableDamping,
                enableClipping: false,
                clip_x: this.planes[0].constant,
                clip_y: this.planes[1].constant,
                clip_z: this.planes[2].constant
            };
            const gui: GUI = new window.GUI();
            const renderArgs = gui.addFolder("Render");
            const controlArgs = gui.addFolder("Controls");
            // const globalClipping = gui.addFolder("Volume Clipping");

            renderArgs.add(volconfig, 'clim1', 0, 1, 0.01).onChange(() => this.updateUniforms());
            renderArgs.add(volconfig, 'clim2', 0, 1, 0.01).onChange(() => this.updateUniforms());
            renderArgs.add(volconfig, 'colormap', cm_names()).onChange(() => this.updateUniforms());
            renderArgs.add(volconfig, 'renderstyle', { mip: 'mip', iso: 'iso' }).onChange(() => this.updateUniforms());
            renderArgs.add(volconfig, 'isothreshold', 0, 1, 0.01).onChange(() => this.updateUniforms());
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
            controlArgs.add(volconfig, "clip_z", -10, 10, 1).onChange((value) => {
                //  camera.position.setZ(value);
                //  camera.updateProjectionMatrix();
                vm.planes[2].constant = value;
                vm.render();
            });

            // globalClipping.add(volconfig, "clip_z", -512, 512, 1).onChange((value) => {
            //     camera.position.setZ(value);
            //     camera.updateProjectionMatrix();
            //     vm.render();
            // });

            // Stats
            const stats: Stats = new window.Stats();
            document.body.appendChild(stats.dom);

            this.controls = controls;
            this.stats = stats;
            this.volconfig = volconfig;

            if (<any>$ts("@data:format") == "nrrd") {
                // Load the default model data ...
                this.loadNrrdModel(<any>$ts("@data:default-maldi"));
            } else {
                this.loadAsciiModel(<any>$ts("@data:default-maldi"));
            }

            window.addEventListener('resize', () => this.onWindowResize());
        }

        /**
         * Load the data ...
        */
        public loadNrrdModel(path: string) {
            new window.NRRDLoader().load(path, volume => this.loadVolumeModel(volume));
        }

        public loadAsciiModel(path: string) {
            new window.ASCIILoader().load(path, volume => this.loadVolumeModel(volume));
        }

        public loadVolumeModel(volume) {
            // Texture to hold the volume. We have scalars, so we put our data in the red channel.
            // THREEJS will select R32F (33326) based on the THREE.RedFormat and THREE.FloatType.
            // Also see https://www.khronos.org/registry/webgl/specs/latest/2.0/#TEXTURE_TYPES_FORMATS_FROM_DOM_ELEMENTS_TABLE
            // TODO: look the dtype up in the volume metadata
            const texture = new THREE.Data3DTexture(volume.data, volume.xLength, volume.yLength, volume.zLength);
            texture.format = THREE.RedFormat;
            texture.type = THREE.FloatType;
            texture.minFilter = texture.magFilter = THREE.LinearFilter;
            texture.unpackAlignment = 1;
            texture.needsUpdate = true;

            console.log("inspect of your 3d model data:");
            console.log(volume);

            // Colormap textures
            this.cmtextures = cm_textures(() => this.render());

            // Material
            const shader = window.VolumeRenderShader1;
            const uniforms = THREE.UniformsUtils.clone(shader.uniforms);
            const volconfig = this.volconfig;

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
                clippingPlanes: this.planes,
                clipShadows: true,
                blending: THREE.AdditiveBlending,
                transparent: true,
                depthWrite: false,
                clipping: true
            });

            // THREE.Mesh
            const geometry = new THREE.BoxGeometry(volume.xLength, volume.yLength, volume.zLength);
            geometry.translate(volume.xLength / 2 - 0.5, volume.yLength / 2 - 0.5, volume.zLength / 2 - 0.5);

            const mesh = new THREE.Mesh(geometry, this.material);
            this.scene.add(mesh);

            const axesHelper = new THREE.AxesHelper(256);
            this.scene.add(axesHelper);
            this.axesHelper = axesHelper;

            // const helper = new THREE.PlaneHelper(this.globalPlane, 300, 0xffff00);
            // this.scene.add(helper);

            // const helper2 = new THREE.PlaneHelper(localPlane, 300, 0xffff00);
            // this.scene.add(helper2);

            this.render();
        }

        updateUniforms() {
            const material = this.material;
            const volconfig = this.volconfig;

            material.uniforms['u_clim'].value.set(volconfig.clim1, volconfig.clim2);
            material.uniforms['u_renderstyle'].value = volconfig.renderstyle == 'mip' ? 0 : 1; // 0: MIP, 1: ISO
            material.uniforms['u_renderthreshold'].value = volconfig.isothreshold; // For ISO renderstyle
            material.uniforms['u_cmdata'].value = this.cmtextures[volconfig.colormap];

            this.render();
        }

        onWindowResize() {
            this.renderer.setSize(window.innerWidth, window.innerHeight);

            const camera = this.camera;
            const aspect = window.innerWidth / window.innerHeight;
            const frustumHeight = camera.top - camera.bottom;

            camera.left = - frustumHeight * aspect / 2;
            camera.right = frustumHeight * aspect / 2;
            camera.updateProjectionMatrix();

            this.render();
        }

        render() {
            var stats = this.stats;

            if (!isNullOrUndefined(stats)) {
                this.stats.begin();
                this.renderer.render(this.scene, this.camera);
                this.stats.end();
            } else {
                this.renderer.render(this.scene, this.camera);
            }
        }
    }
}