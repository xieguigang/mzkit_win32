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

        // index page for get news feeds from biodeep
        GetNewsFeedJSON(): Promise<string>;

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
        RunKmeans(k: number, bisecting: boolean): Promise<boolean>;
        RunDbScan(min_pts: number, eps: number): Promise<boolean>;
        RunGraph(cutoff: number): Promise<boolean>;
        Download(): Promise<string>;

        GetLCMSScatter(): Promise<string>;
        GetColors(name?: string): Promise<string>;

        // LCMS library

        /**
         * Scan the library list that installed in the local filesystem.
        */
        ScanLibraries(): Promise<string>;
        OpenLibrary(path: string): Promise<boolean>;
        GetPage(page: number, page_size: number): Promise<string>;
        Query(name: string): Promise<string>;
        ShowSpectral(data_id: string): Promise<boolean>;
        AlignSpectral(data_id: string): Promise<boolean>;
        FindExactMass(mass: number): Promise<boolean>;

        ShowROIGroups(xcms_id: string, mz: number, rt: number): boolean;
        ShowXic(data_id: string): Promise<boolean>;
        ShowLcmsScatter(sample_name: string): Promise<boolean>;
        ViewSpectral(xcms_id: string, sample: string, db_xref: string): Promise<boolean>;

        /**
         * actions for create new library file
        */
        NewLibrary(): Promise<boolean>;

        // plugin manager

        /**
         * a general method(across multiple host pages) for save general page data
        */
        Save(value?: string): void;
        InstallLocal(): void;
        SetStatus(id: string, status: string): void;
        GetPlugins(): Promise<string>;
        Exec(id: string): void;

        // settings & configuration
        loadSettings(): Promise<string>;
        getProfile(name: string): Promise<string>;
        getAllAdducts(): Promise<string>;
        close(): void;

        // plugin creator
        SelectFolder(): Promise<string>;
        GetFiles(dir: string): Promise<string>;
        BuildPkg(folder: string): Promise<boolean>;

        // ServicesManager
        GetServicesList(): Promise<string>;
    }
}