namespace apps.viewer {

    export interface gcxgc_peak {
        t1: number;
        t2: number;
        into: number;
    }

    export class GCxGCPeaksViewer extends Bootstrap {

        get appName(): string {
            return "gcxgc-peaks";
        }

        private colors: string[];

        protected init(): void {

        }
    }
}