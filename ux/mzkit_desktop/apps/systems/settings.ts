namespace apps.systems {

    const pages: {} = {
        "mzkit_page": "MZKit Settings",
        "msraw_page": "Raw File Viewer",
        "chromagram_page": "Chromagram Plot Styles"
    };

    export class settings extends Bootstrap {

        get appName(): string {
            return "mzkit/settings";
        }

        protected init(): void {
            this.mzkit_page_btn_onclick();
        }

        private static closeAll(): typeof settings {
            for (let page of Object.keys(pages)) {
                $ts(`#${page}`).hide();
            }

            return this;
        }

        private static show(page_id: string) {
            $ts(`#${page_id}`).show();
            $ts("#title").display(pages[page_id]);
        }

        public mzkit_page_btn_onclick() {
            settings.closeAll().show("mzkit_page");
        }

        public msraw_btn_onclick() {
            settings.closeAll().show("msraw_page");
        }

        public chromagram_btn_onclick() {
            settings.closeAll().show("chromagram_page");
        }
    }
}