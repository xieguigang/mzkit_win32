namespace apps.biodeep {

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
        }
    }
}