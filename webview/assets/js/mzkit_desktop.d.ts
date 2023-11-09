/// <reference path="../../../ux/mzkit_desktop/d/three/index.d.ts" />
/// <reference path="../../../ux/mzkit_desktop/d/linq.d.ts" />
declare namespace apps.viewer {
    class three_app extends Bootstrap {
        get appName(): string;
        private potreeViewer;
        protected init(): void;
        private loop;
        private createAnnotations;
        private createVolume;
        private loadModel;
    }
}
declare namespace app.desktop {
    const mzkit: mzkit_desktop;
    interface mzkit_desktop {
        get_3d_MALDI_url(): Promise<string>;
        open_MALDI_model(): any;
        GetScatter(): Promise<string>;
        Click(tag: string): any;
        GetUMAPFile(): Promise<string>;
        GetMatrixDims(): Promise<string>;
        Run(knn: number, knniter: number, localConnectivity: number, bandwidth: number, learningRate: number, spectral_cos: boolean): Promise<boolean>;
        RunKmeans(k: number): Promise<boolean>;
        RunDbScan(min_pts: number, eps: number): Promise<boolean>;
        RunGraph(cutoff: number): Promise<boolean>;
        Download(): Promise<string>;
        GetLCMSScatter(): Promise<string>;
        GetColors(): Promise<string>;
        ScanLibraries(): Promise<string>;
        OpenLibrary(path: string): Promise<boolean>;
        GetPage(page: number, page_size: number): Promise<string>;
        Query(name: string): Promise<string>;
        Save(): void;
        InstallLocal(): void;
        SetStatus(id: string, status: string): void;
        GetPlugins(): Promise<string>;
        Exec(id: string): void;
        SelectFolder(): Promise<string>;
        GetFiles(dir: string): Promise<string>;
        BuildPkg(folder: string): Promise<boolean>;
        GetServicesList(): Promise<string>;
    }
    function run(): void;
}
/**
 * Read of 3d model file blob
*/
declare class ModelReader {
    private bin;
    private pointCloud;
    private palette;
    /**
     * @param bin the data should be in network byte order
    */
    constructor(bin: Uint8Array);
    private cubic_scale;
    private centroid;
    private getVector3;
    loadPointCloudModel(canvas: apps.three_app): void;
}
interface pointCloud {
    x: number;
    y: number;
    z: number;
    intensity: number;
    color: number | string;
}
declare namespace apps {
    const biodeep_classroom: string;
    const biodeep_viewVideo: string;
    interface video {
        imgUrl: string;
        id: string;
        title: string;
        createTime: string;
    }
    class home extends Bootstrap {
        get appName(): string;
        protected init(): void;
        private showClassRoom;
    }
}
declare namespace apps.systems {
    class pluginMgr extends Bootstrap {
        get appName(): string;
        protected init(): void;
        private setPluginStatus;
        private addPlugin;
        install_local_onclick(): void;
    }
    interface plugin {
        id: string;
        name: string;
        desc: string;
        ver: string;
        author: string;
        url: string;
        status: "active" | "disable" | "incompatible";
    }
}
declare namespace apps.systems {
    class pluginPkg extends Bootstrap {
        get appName(): string;
        protected init(): void;
        dir_onchange(value: string): void;
        selectFolder_onclick(): void;
        build_onclick(): void;
    }
}
declare namespace apps.systems {
    interface perfermanceCount {
        svr: Service;
        Counter: number[];
    }
    interface counterData {
        title: string;
        x: number[];
        y: number[];
    }
    class servicesManager extends Bootstrap {
        get appName(): string;
        readonly cpu: Dictionary<perfermanceCount>;
        readonly memory: Dictionary<perfermanceCount>;
        private plot;
        protected init(): void;
        /**
         * on update a frame display
        */
        private startUpdateTask;
        /**
         * tick loop frame
        */
        private loadServicesList;
        private cpu_chart;
        private mem_chart;
        private refresh;
        private onDraw;
        private static history;
        private static counterChart;
        private updatePlotHost;
        private styleEachRow;
    }
    interface Service {
        Name: string;
        Description: string;
        Port: number;
        PID: number;
        CPU: number;
        Memory: number | string;
        isAlive: boolean | string;
        StartTime: string;
        CommandLine: string;
    }
}
declare namespace apps.viewer {
    /**
     * UMAPPoint
    */
    interface scatterPoint {
        x: number;
        y: number;
        z: number;
        /**
         * which cluster(color) that current spot it belongs to
        */
        class: string;
        /**
         * label id of current spot
        */
        label: string;
    }
    interface cluster_data {
        cluster: number | string;
        scatter: number[][];
        labels: string[];
    }
    /**
     * #viewer
    */
    class clusterViewer extends Bootstrap {
        get appName(): string;
        protected init(): void;
        static render3DScatter(dataset: scatterPoint[], hook_resize?: boolean): void;
        private static format_cluster_tag;
        static load_cluster(data: cluster_data[]): gl_plot.scatter3d_options;
    }
}
declare namespace apps.viewer {
    class lcmsLibrary extends Bootstrap {
        get appName(): string;
        private libfiles;
        private page;
        private page_size;
        protected init(): void;
        private loadfiles;
        private customMenu;
        private list_data;
        private show_page;
        private get_smiles;
        query_onclick(): void;
    }
    interface MetaLib {
        ID: string;
        formula: string;
        exact_mass: number;
        name: string;
        IUPACName: string;
        description: string;
        synonym: string[];
        xref: xref;
    }
    interface xref {
        chebi: string;
        KEGG: string;
        pubchem: string;
        HMDB: string;
        metlin: string;
        DrugBank: string;
        ChEMBL: string;
        Wikipedia: string;
        lipidmaps: string;
        MeSH: string;
        ChemIDplus: string;
        MetaCyc: string;
        KNApSAcK: string;
        CAS: string[];
        InChIkey: string;
        InChI: string;
        SMILES: string;
    }
}
declare namespace apps.viewer {
    interface ms1_scatter {
        id: string;
        mz: number;
        scan_time: number;
        intensity: number;
    }
    class LCMSScatterViewer extends Bootstrap {
        get appName(): string;
        private colors;
        private layers;
        protected init(): void;
        render3DScatter(dataset: ms1_scatter[]): void;
        private static scatter_group;
        load_cluster(data: ms1_scatter[]): gl_plot.scatter3d_options;
    }
}
declare namespace apps.viewer {
    class OpenseadragonSlideViewer extends Bootstrap {
        get appName(): string;
        private getDziSrc;
        private static viewer;
        protected init(): Promise<void>;
        static ExportViewImage(): void;
    }
}
declare namespace apps.viewer {
    class umap extends Bootstrap {
        get appName(): string;
        protected init(): void;
        knn_onchange(val: string): void;
        KnnIter_onchange(val: string): void;
        localConnectivity_onchange(val: string): void;
        bandwidth_onchange(val: string): void;
        learningRate_onchange(val: string): void;
        kmeans_onchange(val: string): void;
        min_pts_onchange(val: string): void;
        eps_onchange(val: string): void;
        identical_onchange(val: string): void;
        run_umap_onclick(): void;
        run_kmeans_onclick(): void;
        run_graph_onclick(): void;
        run_dbscan_onclick(): void;
        showSpinner(): void;
        hideSpinner(): void;
        download_onclick(): void;
        save_onclick(): void;
        private loadUMAP;
        kmeans_method_onchange(val: string): void;
        dbscan_method_onchange(val: string): void;
        graph_method_onchange(val: string): void;
        private selectMethod;
    }
}
