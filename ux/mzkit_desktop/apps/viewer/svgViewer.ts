namespace apps.viewer {

    export class svgViewer extends Bootstrap {

        get appName(): string {
            return "svg-viewer";
        }

        protected init(): void {
            app.desktop.mzkit.Download().then(async function (uri) {
                const url = await uri;

                console.log(url);
                svgViewer.setSvgUrl(url);
            });
        }

        /**
         * @param url the base64 encoded svg image data
        */
        public static setSvgUrl(url: string) {
            (<HTMLImageElement><HTMLElement>$ts("#viewer")).src = url;
        }
    }
}