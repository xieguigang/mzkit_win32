namespace app.desktop {

    /**
     * the mzkit desktop app
    */
    export const mzkit: mzkit_desktop = getHostObject();

    /**
     * get global desktop object to the visualbasic clr runtime environment
    */
    function getHostObject() {
        try {
            return (<any>window).chrome.webview.hostObjects.mzkit;
        } catch {
            return null;
        }
    }

    export interface mzkit_desktop {
        // 3d maldi model viewer
        get_3d_MALDI_url(): Promise<string>;
        open_MALDI_model();

        // 3d scatter model viewer
        GetScatter(): Promise<string>;
        Click(tag: string);
        // umap analysis
        GetUMAPFile(): Promise<string>;
        GetMatrixDims(): Promise<string>;
        Run(knn: number, knniter: number,
            localConnectivity: number,
            bandwidth: number,
            learningRate: number,
            spectral_cos: boolean): Promise<boolean>;
        RunKmeans(k: number): Promise<boolean>;
        RunDbScan(min_pts: number, eps: number): Promise<boolean>;
        RunGraph(cutoff: number): Promise<boolean>;
        Download(): Promise<string>;

        GetLCMSScatter(): Promise<string>;
        GetColors(): Promise<string>;

        // LCMS library

        /**
         * Scan the library list that installed in the local filesystem.
        */
        ScanLibraries(): Promise<string>;
        OpenLibrary(path: string): Promise<boolean>;
        GetPage(page: number, page_size: number): Promise<string>;
        Query(name: string): Promise<string>;
        /**
         * actions for create new library file
        */
        NewLibrary(): Promise<boolean>;

        // plugin manager
        Save(): void;
        InstallLocal(): void;
        SetStatus(id: string, status: string): void;
        GetPlugins(): Promise<string>;
        Exec(id: string): void;

        // plugin creator
        SelectFolder(): Promise<string>;
        GetFiles(dir: string): Promise<string>;
        BuildPkg(folder: string): Promise<boolean>;

        // ServicesManager
        GetServicesList(): Promise<string>;
    }
}