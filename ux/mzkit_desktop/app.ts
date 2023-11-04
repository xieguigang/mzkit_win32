/// <reference path="./d/linq.d.ts" />
/// <reference path="./apps/viewer/three_app.ts" />

namespace app.desktop {

    export const mzkit: mzkit_desktop = getHostObject();

    function getHostObject() {
        try {
            return (<any>window).chrome.webview.hostObjects.mzkit;
        } catch {
            return null;
        }
    }

    export interface mzkit_desktop {
        // 3d maldi model viewer
        get_3d_MALDI_url(): Promise<string>;
        open_MALDI_model();

        // 3d scatter model viewer
        GetScatter(): Promise<string>;
        Click(tag: string);

        GetLCMSScatter(): Promise<string>;
        GetColors(): Promise<string>;

        // LCMS library
        ScanLibraries(): Promise<string>;
        OpenLibrary(path: string): Promise<boolean>;
        GetPage(page: number, page_size: number): Promise<string>;
        Query(name: string): Promise<string>;

        // plugin manager
        Save(): void;
        InstallLocal(): void;
        SetStatus(id: string, status: string): void;
        GetPlugins(): Promise<string>;
        Exec(id: string): void;

        // plugin creator
        SelectFolder(): Promise<string>;
        GetFiles(dir: string): Promise<string>;
        BuildPkg(folder: string): Promise<boolean>;

        // ServicesManager
        GetServicesList(): Promise<string>;
    }

    export function run() {
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
}

$ts.mode = Modes.development;
$ts(app.desktop.run);