namespace apps.biodeep {

    /**
     * Helper module code for view the biodeep annotation workflow result
    */
    export class reportViewer extends Bootstrap {

        get appName(): string {
            return "biodeep_report";
        }

        protected init(): void {
            $ts.select(".meta_header").onClick(async (a) => await app.desktop.mzkit.ShowXic(a.getAttribute("xcms_id")));
            $ts.select(".sample_name").onClick(async (a) => await app.desktop.mzkit.ShowLcmsScatter(a.getAttribute("data_name")));
            $ts.select(".score").onClick(async (a) => await reportViewer.clickScoreLink(a));
            $ts.select(".ROI").onClick(async (a) => await reportViewer.clickROI(a));
            $ts.select("path").onClick(async (a) => await reportViewer.clickScatter(a));
            $ts.select("circle").onClick(async (a) => await reportViewer.clickScatter(a));
        }

        static async clickScoreLink(a: HTMLElement) {
            let xcms_id = a.getAttribute("data_id");
            let sample = a.getAttribute("data_sample");
            let biodeep_id = a.getAttribute("biodeep_id");

            await app.desktop.mzkit.ViewSpectral(xcms_id, sample, biodeep_id);
        }

        static async clickScatter(a: HTMLElement) {
            let xcms_id = a.getAttribute("xcms_id");
            let sample = a.getAttribute("source");
            let biodeep_id = a.getAttribute("biodeep_id");

            await app.desktop.mzkit.ViewSpectral(xcms_id, sample, biodeep_id);
        }

        static async clickROI(a: HTMLElement) {
            let mz = parseFloat(a.getAttribute("mz"));
            let rt = parseFloat(a.getAttribute("rt"));
            let xcms_id = a.getAttribute("xcms_id");

            await app.desktop.mzkit.ShowROIGroups(xcms_id, mz, rt);
        }
    }
}