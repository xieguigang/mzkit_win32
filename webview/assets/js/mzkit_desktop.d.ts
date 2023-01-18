/// <reference path="../../../ux/mzkit_desktop/d/three/index.d.ts" />
/// <reference path="../../../ux/mzkit_desktop/d/linq.d.ts" />
declare namespace apps.viewer {
    class three_app extends Bootstrap {
        get appName(): string;
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
        static open: Delegate.Action;
    }
}
declare namespace app.desktop {
    const mzkit: mzkit_desktop;
    interface mzkit_desktop {
        get_3d_MALDI_url(): Promise<string>;
        open_MALDI_model(): any;
        Save(): void;
        InstallLocal(): void;
        SetStatus(id: string, status: string): void;
        GetPlugins(): Promise<string>;
        Exec(id: string): void;
        SelectFolder(): Promise<string>;
        GetFiles(dir: string): Promise<string>;
        BuildPkg(folder: string): Promise<boolean>;
        GetServicesList(): Promise<string>;
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
        get appName(): string;
        protected init(): void;
    }
}
declare namespace apps.systems {
    class pluginMgr extends Bootstrap {
        get appName(): string;
        protected init(): void;
        private setPluginStatus;
        private addPlugin;
        install_local_onclick(): void;
    }
    interface plugin {
        id: string;
        name: string;
        desc: string;
        ver: string;
        author: string;
        url: string;
        status: "active" | "disable" | "incompatible";
    }
}
declare namespace apps.systems {
    class pluginPkg extends Bootstrap {
        get appName(): string;
        protected init(): void;
        dir_onchange(value: string): void;
        selectFolder_onclick(): void;
        build_onclick(): void;
    }
}
declare namespace apps.systems {
    class servicesManager extends Bootstrap {
        get appName(): string;
        protected init(): void;
        private startUpdateTask;
        private loadServicesList;
        private static styleEachRow;
    }
    interface Service {
        name: string;
        description: string;
        port: number;
        PID: number;
        cpu: number;
        mem: number | string;
        isAlive: boolean;
    }
}
declare namespace apps.viewer {
    class clusterViewer extends Bootstrap {
        get appName(): string;
        protected init(): void;
    }
}
