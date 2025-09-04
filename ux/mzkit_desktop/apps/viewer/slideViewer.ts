// openseadragon slide viewer

namespace apps.viewer {

    export class OpenseadragonSlideViewer extends Bootstrap {

        public get appName(): string {
            return "openseadragon";
        }

        private getDziSrc(): string {
            return (<any>window).chrome.webview.hostObjects.dzi;
        }

        private static viewer: any;

        protected async init() {
            const dzi = await this.getDziSrc();
            const OpenSeadragon = (<any>window).OpenSeadragon;

            console.log("get the source deep zoom image url:");
            console.log(dzi);

            OpenseadragonSlideViewer.viewer = OpenSeadragon({
                id: "seadragon-viewer",
                prefixUrl: "/openseadragon/images/",
                tileSources: [
                    dzi
                ],
                showNavigator: true,
                navigatorPosition: "BOTTOM_RIGHT",
                // Initial rotation angle
                degrees: 0,
                // Show rotation buttons
                showRotationControl: true,
                // Enable touch rotation on tactile devices
                gestureSettingsTouch: {
                    pinchRotate: true
                }
            });
        }

        public static ExportViewImage() {
            let viewer = OpenseadragonSlideViewer.viewer.drawer;
            let img: string = viewer.canvas.toDataURL("image/png");

            console.log("get web capture image exports:");
            console.log(img);

            DOM.download("capture.png", img, true);
        }

        public static CaptureSlideImage(): string {
            let viewer = OpenseadragonSlideViewer.viewer.drawer;
            let img: string = viewer.canvas.toDataURL("image/png");

            console.log("get web capture image exports:");
            console.log(img);

            return img;
        }
    }
}