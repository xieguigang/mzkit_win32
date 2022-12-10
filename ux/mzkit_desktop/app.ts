/// <reference path="./d/linq.d.ts" />
/// <reference path="./apps/three_app.ts" />

namespace app.desktop {

    export const mzkit: mzkit_desktop = (<any>window).chrome.webview.hostObjects.mzkit;

    export interface mzkit_desktop {
        get_3d_MALDI_url(): Promise<string>;
    }

    export function run() {
        Router.AddAppHandler(new apps.three_app());

        Router.RunApp();
    }
}

$ts.mode = Modes.debug;
$ts(app.desktop.run);