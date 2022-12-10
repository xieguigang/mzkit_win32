/// <reference path="../../../ux/mzkit_desktop/d/linq.d.ts" />
/// <reference path="../../../ux/mzkit_desktop/d/three/index.d.ts" />
declare namespace app.desktop {
    function run(): void;
}
declare namespace apps {
    class three_app extends Bootstrap {
        readonly appName: string;
        private renderer;
        private camera;
        private scene;
        private light;
        private stats;
        private controls;
        private gui;
        private initGui;
        private initControls;
        private initStats;
        private initRender;
        private initCamera;
        private initScene;
        private initLight;
        private initModel;
        private static randomColor;
        private render;
        private onWindowResize;
        private animate;
        protected init(): void;
    }
}
