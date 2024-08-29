/// <reference path="./d/linq.d.ts" />
/// <reference path="./apps/viewer/three_app.ts" />

namespace app.desktop {

    /**
     * main function for run start of the desktop app
    */
    export function run() {
        // mzkit system pages
        Router.AddAppHandler(new apps.home());
        Router.AddAppHandler(new apps.systems.pluginMgr());
        Router.AddAppHandler(new apps.systems.pluginPkg());
        Router.AddAppHandler(new apps.systems.servicesManager());
        Router.AddAppHandler(new apps.systems.settings());

        // data analysis & data visualization
        Router.AddAppHandler(new apps.viewer.three_app());
        Router.AddAppHandler(new apps.viewer.clusterViewer());
        Router.AddAppHandler(new apps.viewer.LCMSScatterViewer());
        Router.AddAppHandler(new apps.viewer.GCxGCPeaksViewer());
        Router.AddAppHandler(new apps.viewer.OpenseadragonSlideViewer());
        Router.AddAppHandler(new apps.viewer.umap());
        Router.AddAppHandler(new apps.viewer.lcmsLibrary());
        Router.AddAppHandler(new apps.viewer.svgViewer());

        Router.AddAppHandler(new apps.biodeep.reportViewer());

        Router.RunApp();
    }
}

$ts.mode = Modes.development;
$ts(app.desktop.run);