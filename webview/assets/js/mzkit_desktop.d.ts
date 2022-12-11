/// <reference path="../../../ux/mzkit_desktop/d/three/index.d.ts" />
/// <reference path="../../../ux/mzkit_desktop/d/linq.d.ts" />
declare namespace apps {
    class three_app extends Bootstrap {
        readonly appName: string;
        scene: THREE.Scene;
        private renderer;
        private camera;
        private light;
        private stats;
        private controls;
        private initControls;
        private initStats;
        private initRender;
        private initCamera;
        private initScene;
        private initLight;
        private initModel;
        private render;
        private onWindowResize;
        private animate;
        protected init(): void;
        private setup_device;
        open_model_onclick(): void;
    }
}
declare namespace app.desktop {
    const mzkit: mzkit_desktop;
    interface mzkit_desktop {
        get_3d_MALDI_url(): Promise<string>;
        open_MALDI_model(): any;
    }
    function run(): void;
}
/**
 * Read of 3d model file blob
*/
declare class ModelReader {
    private bin;
    private pointCloud;
    private palette;
    /**
     * @param bin the data should be in network byte order
    */
    constructor(bin: Uint8Array);
    private cubic_scale;
    private centroid;
    private getVector3;
    loadPointCloudModel(canvas: apps.three_app): void;
}
interface pointCloud {
    x: number;
    y: number;
    z: number;
    intensity: number;
    color: number | string;
}
declare namespace apps {
    class home extends Bootstrap {
        readonly appName: string;
        protected init(): void;
    }
}
