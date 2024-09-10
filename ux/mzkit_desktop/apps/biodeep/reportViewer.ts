namespace apps.biodeep {

    /**
     * Helper module code for view the biodeep annotation workflow result
    */
    export class reportViewer extends Bootstrap {

        get appName(): string {
            return "biodeep_report";
        }

        protected init(): void {
            $ts.select(".meta_header").onClick(async function (a) {
                await app.desktop.mzkit.ShowXic(a.getAttribute("xcms_id"));
            });
            $ts.select(".sample_name").onClick(async function (a) {
                await app.desktop.mzkit.ShowLcmsScatter(a.getAttribute("data_name"));
            });
            $ts.select(".score").onClick(async function (a) {
                let xcms_id = a.getAttribute("data_id");
                let sample = a.getAttribute("data_sample");
                let biodeep_id = a.getAttribute("biodeep_id");

                await app.desktop.mzkit.ViewSpectral(xcms_id, sample, biodeep_id);
            });
            $ts.select(".ROI").onClick(async function (a) {
                let mz = parseFloat(a.getAttribute("mz"));
                let rt = parseFloat(a.getAttribute("rt"));

                await app.desktop.mzkit.ShowROIGroups(mz, rt);
            });
            $ts.select("path").onClick(async function (a) {
                let xcms_id = a.getAttribute("xcms_id");
                let sample = a.getAttribute("source");
                let biodeep_id = a.getAttribute("biodeep_id");

                await app.desktop.mzkit.ViewSpectral(xcms_id, sample, biodeep_id);
            })
        }
    }
}