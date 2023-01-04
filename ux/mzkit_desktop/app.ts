/// <reference path="./d/linq.d.ts" />
/// <reference path="./apps/three_app.ts" />

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
    }

    export function run() {
        Router.AddAppHandler(new apps.home());
        Router.AddAppHandler(new apps.pluginMgr());
        Router.AddAppHandler(new apps.pluginPkg());

        Router.AddAppHandler(new apps.three_app());
        Router.AddAppHandler(new apps.clusterViewer());

        Router.RunApp();
    }
}

$ts.mode = Modes.debug;
$ts(app.desktop.run);