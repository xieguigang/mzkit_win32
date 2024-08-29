/// <reference path="../../../ux/mzkit_desktop/d/three/index.d.ts" />
/// <reference path="../../../ux/mzkit_desktop/d/linq.d.ts" />
declare namespace apps.viewer {
    const Empty: readonly any[];
    interface OrbitControls {
    }
    interface Stats {
        get dom(): HTMLElement;
        begin(): void;
        end(): void;
    }
    interface GUI {
        add(volconfig: volconfig, name: string, arg2?: any, arg3?: any, arg4?: any): any;
        addFolder(name: string): GUI;
    }
    interface volconfig {
        clim1: number;
        clim2: number;
        renderstyle: string;
        isothreshold: number;
        colormap: string;
        showAxis: boolean;
        enableDamping: boolean;
        enableClipping: boolean;
        clip_x: number;
        clip_y: number;
        clip_z: number;
    }
    interface NRRDLoader {
    }
    function cm_names(): {};
    function cm_textures(callback: Delegate.Action): {};
    class three_app extends Bootstrap {
        renderer: THREE.WebGLRenderer;
        scene: THREE.Scene;
        camera: THREE.OrthographicCamera;
        controls: OrbitControls;
        material: THREE.ShaderMaterial;
        volconfig: volconfig;
        cmtextures: any;
        stats: Stats;
        planes: THREE.Plane[];
        axesHelper: THREE.AxesHelper;
        get appName(): string;
        protected init(): void;
        /**
         * Load the data ...
        */
        loadNrrdModel(path: string): void;
        loadAsciiModel(path: string): void;
        loadVolumeModel(volume: any): void;
        updateUniforms(): void;
        onWindowResize(): void;
        render(): void;
    }
}
declare namespace app.desktop {
    /**
     * main function for run start of the desktop app
    */
    function run(): void;
}
declare namespace app.desktop {
    /**
     * the mzkit desktop app
    */
    const mzkit: mzkit_desktop;
    interface mzkit_desktop {
        get_3d_MALDI_url(): Promise<string>;
        open_MALDI_model(): any;
        GetNewsFeedJSON(): Promise<string>;
        GetScatter(): Promise<string>;
        Click(tag: string): any;
        GetUMAPFile(): Promise<string>;
        GetMatrixDims(): Promise<string>;
        Run(knn: number, knniter: number, localConnectivity: number, bandwidth: number, learningRate: number, spectral_cos: boolean): Promise<boolean>;
        RunKmeans(k: number, bisecting: boolean): Promise<boolean>;
        RunDbScan(min_pts: number, eps: number): Promise<boolean>;
        RunGraph(cutoff: number): Promise<boolean>;
        Download(): Promise<string>;
        GetLCMSScatter(): Promise<string>;
        GetColors(name?: string): Promise<string>;
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
        ShowXic(data_id: string): Promise<boolean>;
        ShowLcmsScatter(sample_name: string): Promise<boolean>;
        ViewSpectral(xcms_id: string, sample: string, db_xref: string): Promise<boolean>;
        /**
         * actions for create new library file
        */
        NewLibrary(): Promise<boolean>;
        /**
         * a general method(across multiple host pages) for save general page data
        */
        Save(value?: string): void;
        InstallLocal(): void;
        SetStatus(id: string, status: string): void;
        GetPlugins(): Promise<string>;
        Exec(id: string): void;
        loadSettings(): Promise<string>;
        getProfile(name: string): Promise<string>;
        getAllAdducts(): Promise<string>;
        close(): void;
        SelectFolder(): Promise<string>;
        GetFiles(dir: string): Promise<string>;
        BuildPkg(folder: string): Promise<boolean>;
        GetServicesList(): Promise<string>;
    }
}
declare namespace gl_plot {
    /**
     * get id
    */
    interface IdReader<T> {
        (x: T): string;
    }
    /**
     * get scatter point:
     *
     * lcms scatter: mz, rt, intensity
     * gcxgc peaks: rt1, rt2, intensity
    */
    interface PointReader<T> {
        (x: T): number[];
    }
    interface IntensityReader<T> {
        (x: T): number;
    }
    interface LabelReader<T> {
        (args: {
            dataIndex: number;
            data: number[];
        }, data: T[]): string;
    }
    class echart_peak3D<T> {
        private read_id;
        private read_point;
        private read_intensity;
        private read_label;
        private xlab;
        private ylab;
        private zlab;
        colors: string[];
        layers: Dictionary<T[]>;
        constructor(read_id: IdReader<T>, read_point: PointReader<T>, read_intensity: IntensityReader<T>, read_label: LabelReader<T>, xlab: string, ylab: string, zlab?: string);
        private scatter_group;
        load_cluster(data: T[]): gl_plot.scatter3d_options;
        private format_axisLabel;
    }
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
        private loadList;
        private showClassRoom;
    }
}
declare namespace apps.biodeep {
    class reportViewer extends Bootstrap {
        get appName(): string;
        protected init(): void;
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
        Protocol: string;
        Port: number;
        PID: number;
        CPU: number;
        Memory: number | string;
        isAlive: boolean | string;
        StartTime: string;
        CommandLine: string;
    }
}
declare namespace apps.systems {
    const element_columns: ({
        title: string;
        field: string;
        sortable: boolean;
        width: number;
        editable: boolean;
    } | {
        title: string;
        field: string;
        sortable: boolean;
        width: number;
        editable: {
            type: string;
        };
    })[];
    interface BootstrapTable {
        bootstrapTable(arg1: any, arg2?: any): any;
    }
    interface mzkit_configs {
        "remember_location": boolean;
        "remember_layout": boolean;
        "language": 0 | 1 | 2;
        "xic_ppm": number;
        "fragment_cutoff": "relative" | "quantile";
        "fragment_cutoff_value": number;
        "colorset": string[];
        "fill_plot_area": boolean;
        "formula_search": {
            "naturalProductProfile": element_profile;
            "smallMoleculeProfile": element_profile;
            "elements": {};
        };
        "formula_ppm": number;
        "formula_adducts": string[];
        "layout_iterations": number;
        "stiffness": number;
        "repulsion": number;
        "damping": number;
        "node_identical": number;
        "node_similar": number;
        "edge_filter": number;
        "node_radius_min": number;
        "node_radius_max": number;
        "link_width_min": number;
        "link_width_max": number;
    }
    interface element_count {
        atom: string;
        min: number;
        max: number;
    }
    interface element_profile {
        "type": "Wiley" | "DNP";
        "isCommon": boolean;
    }
    class settings extends Bootstrap {
        get appName(): string;
        static mzkit_configs: mzkit_configs;
        protected init(): void;
        remember_location_onchange(value: string | string[]): void;
        remember_layout_onchange(value: string | string[]): void;
        language_onchange(value: string | string[]): void;
        fill_plot_area_onchange(value: string | string[]): void;
        private loadConfigs;
        private static defaultSettings;
        /**
         * auto binding of the [min,max] value range form control
        */
        private static bindRangeDisplayValue;
        /**
         * get table html UI for create custom element profiles
        */
        private static getElementProfileTable;
        private static load_profileTable;
        private static loadProfileTable;
        private static closeAll;
        private profile_name;
        copy_profile_onchange(value: string | string[]): void;
        reset_profile_onclick(): void;
        preset_colorset_onchange(value: string | string[]): void;
        private static loadColorList;
        private static appendColor;
        private static getColorList;
        add_color_onclick(): void;
        clear_colors_onclick(): void;
        private static show;
        mzkit_page_btn_onclick(): void;
        msraw_btn_onclick(): void;
        chromagram_btn_onclick(): void;
        formula_btn_onclick(): void;
        profile_btn_onclick(): void;
        add_element_onclick(): void;
        molecule_networking_btn_onclick(): void;
        /**
         * save profile table as custom profiles
        */
        save_elements_onclick(): void;
        static invoke_save(): void;
        apply_settings_onclick(): void;
        close_page(): void;
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
    interface gcxgc_peak {
        t1: number;
        t2: number;
        into: number;
    }
    class GCxGCPeaksViewer extends Bootstrap {
        get appName(): string;
        private peaks3D;
        private create_viewer;
        private label;
        protected init(): void;
        render3DScatter(dataset: gcxgc_peak[]): void;
    }
}
declare namespace apps.viewer {
    class lcmsLibrary extends Bootstrap {
        get appName(): string;
        private libfiles;
        private page;
        private page_size;
        protected init(): void;
        private static showLoader;
        private static hideLoader;
        private reloadLibs;
        private loadfiles;
        private customMenu;
        private menu_new;
        private menu_open;
        static openLibfile(filepath: string, vm?: lcmsLibrary): void;
        private list_data;
        /**
         * .lib-id
        */
        private hookSpectralLinkOpen;
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
        private peaks3D;
        private create_viewer;
        private label;
        protected init(): void;
        render3DScatter(dataset: ms1_scatter[]): void;
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
    class svgViewer extends Bootstrap {
        get appName(): string;
        protected init(): void;
        /**
         * @param url the base64 encoded svg image data
        */
        static setSvgUrl(url: string): void;
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
