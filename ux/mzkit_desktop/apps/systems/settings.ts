namespace apps.systems {

    const pages: string[] = [
        "mzkit_page", "msraw_page"
    ];

    export class settings extends Bootstrap {

        get appName(): string {
            return "mzkit/settings";
        }

        protected init(): void {

        }

        private static closeAll() {
            for (let page of pages) {
                $ts(`#${page}`).hide();
            }
        }

        public mzkit_page_btn_onclick() {
            settings.closeAll();
            $ts("#mzkit_page").show();
        }

        public msraw_btn_onclick() {
            settings.closeAll();
            $ts("#msraw_page").show();
        }
    }
}