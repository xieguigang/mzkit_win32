namespace apps.viewer {

    export class svgViewer extends Bootstrap {

        get appName(): string {
            return "svg-viewer";
        }

        protected init(): void {

        }

        public static setSvgUrl(url: string) {
            (<HTMLImageElement><HTMLElement>$ts("#viewer")).src = url;
        }
    }
}