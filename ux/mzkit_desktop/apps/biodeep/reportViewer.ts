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
                await app.desktop.mzkit.ShowLcmsScatter(a.getAttribute("sample_name"));
            });
        }
    }
}