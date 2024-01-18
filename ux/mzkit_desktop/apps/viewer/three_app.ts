/// <reference path="../../d/three/index.d.ts" />

namespace apps.viewer {

    const window = <any>globalThis.window;

    export interface OrbitControls { }

    export interface GUI {
        add(volconfig: volconfig, name: string, arg2?: any, arg3?: any, arg4?: any): any;
    }

    export interface volconfig {
        clim1: number; clim2: number;
        renderstyle: string;
        isothreshold: number;
        colormap: string;
        clipIntersection: boolean;
        planeConstant: number;
        showHelpers: boolean;
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
        public clipPlanes: THREE.Plane[];
        public model: THREE.Mesh;
        public helpers: THREE.Group;

        public get appName(): string {
            return "three-3d";
        }

        protected init(): void {
            const scene = new THREE.Scene();
            const clipPlanes: THREE.Plane[] = [
                new THREE.Plane(new THREE.Vector3(1, 0, 0), 0),
                new THREE.Plane(new THREE.Vector3(0, - 1, 0), 0),
                new THREE.Plane(new THREE.Vector3(0, 0, - 1), 0)
            ];

            // Create renderer
            const renderer = new THREE.WebGLRenderer({ antialias: false });
            renderer.setPixelRatio(window.devicePixelRatio);
            renderer.setSize(window.innerWidth, window.innerHeight);
            renderer.localClippingEnabled = true;

            document.body.appendChild(renderer.domElement);
            console.log(renderer);

            // Create camera (The volume renderer does not work very well with perspective yet)
            const h = 512; // frustum height
            const aspect = window.innerWidth / window.innerHeight;
            const camera = new THREE.OrthographicCamera(- h * aspect / 2, h * aspect / 2, h / 2, - h / 2, 1, 1000);
            camera.position.set(-64, -64, 128);
            camera.up.set(0, 0, 1); // In our data, z is up

            this.scene = scene;
            this.renderer = renderer;
            this.camera = camera;
            this.clipPlanes = clipPlanes;

            // Create controls
            const controls = new window.OrbitControls(camera, renderer.domElement);
            // use only if there is no animation loop
            controls.addEventListener('change', () => this.render());
            controls.target.set(128, 128, 128);
            controls.minZoom = 0.25;
            controls.maxZoom = 5;
            // controls.minDistance = 1;
            // controls.maxDistance = 10;
            controls.enablePan = true;
            controls.screenSpacePanning = true;
            controls.update();

            // const light = new THREE.HemisphereLight(0xffffff, 0x080808, 4.5);
            // light.position.set(- 1.25, 1, 1.25);
            // scene.add(light);

            // scene.add( new AxesHelper( 128 ) );

            // Lighting is baked into the shader a.t.m.
            // let dirLight = new DirectionalLight( 0xffffff );

            // helpers

            const helpers = new THREE.Group();
            helpers.add(new THREE.PlaneHelper(clipPlanes[0], 2, 0xff0000));
            helpers.add(new THREE.PlaneHelper(clipPlanes[1], 2, 0x00ff00));
            helpers.add(new THREE.PlaneHelper(clipPlanes[2], 2, 0x0000ff));
            helpers.visible = false;
            scene.add(helpers);

            const gui: GUI = new window.GUI();
            // The gui for interaction
            // parameter object
            const volconfig: volconfig = {
                clim1: 0, clim2: 1, renderstyle: 'iso', isothreshold: 0.15,
                colormap: 'jet',
                clipIntersection: true,
                planeConstant: 0,
                showHelpers: false
            };

            gui.add(volconfig, 'clim1', 0, 1, 0.01).onChange(() => this.updateUniforms());
            gui.add(volconfig, 'clim2', 0, 1, 0.01).onChange(() => this.updateUniforms());
            gui.add(volconfig, 'colormap', cm_names()).onChange(() => this.updateUniforms());
            gui.add(volconfig, 'renderstyle', { mip: 'mip', iso: 'iso' }).onChange(() => this.updateUniforms());
            gui.add(volconfig, 'isothreshold', 0, 1, 0.01).onChange(() => this.updateUniforms());

            // webgl_clipping_intersection
            gui.add(volconfig, 'clipIntersection').onChange(() => this.updateUniforms());
            gui.add(volconfig, 'planeConstant', -1, 1, 0.01).onChange(() => this.updateUniforms());
            gui.add(volconfig, 'showHelpers').onChange(() => this.updateUniforms());

            this.controls = controls;
            this.volconfig = volconfig;
            this.helpers = helpers;

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

            this.material = new THREE.ShaderMaterial({
                uniforms: uniforms,
                vertexShader: shader.vertexShader,
                fragmentShader: shader.fragmentShader,
                side: THREE.BackSide, // The volume shader uses the backface as its "reference point"
                clippingPlanes: this.clipPlanes,
                clipIntersection: volconfig.clipIntersection
            });

            // THREE.Mesh
            const geometry = new THREE.BoxGeometry(volume.xLength, volume.yLength, volume.zLength);
            geometry.translate(
                volume.xLength / 2 - 0.5,
                volume.yLength / 2 - 0.5,
                volume.zLength / 2 - 0.5
            );

            const mesh = new THREE.Mesh(geometry, this.material);

            this.scene.add(mesh);
            this.model = mesh;

            this.render();
        }

        updateUniforms() {
            const material = this.material;
            const volconfig = this.volconfig;

            material.uniforms['u_clim'].value.set(volconfig.clim1, volconfig.clim2);
            material.uniforms['u_renderstyle'].value = volconfig.renderstyle == 'mip' ? 0 : 1; // 0: MIP, 1: ISO
            material.uniforms['u_renderthreshold'].value = volconfig.isothreshold; // For ISO renderstyle
            material.uniforms['u_cmdata'].value = this.cmtextures[volconfig.colormap];

            this.update_clipIntersection(volconfig.clipIntersection);
            this.update_planeConstant(volconfig.planeConstant);
            this.update_showHelpers(volconfig.showHelpers);

            this.render();
        }

        update_showHelpers(value: boolean) {
            this.helpers.visible = value;
        }

        update_planeConstant(value: number) {
            const clipPlanes = this.clipPlanes;

            for (let j = 0; j < clipPlanes.length; j++) {
                clipPlanes[j].constant = value;
            }
        }

        update_clipIntersection(value: boolean) {
            this.model.material.clipIntersection = value;
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
            this.renderer.render(this.scene, this.camera);
        }
    }
}