namespace apps.systems {

    /**
     * the settings class model in mzkit_win32 program
    */
    export interface mzkit_configs {
        "precursor_search": {
            "ppm": number,
            "positive": string[],
            "negative": string[]
        },
        "formula_search": {
            elements: {},
            smallMoleculeProfile: element_profile,
            naturalProductProfile: element_profile
        },
        "ui": {
            "x"?: number,
            "y"?: number,
            "width"?: number,
            "height"?: number,
            "window"?: string,
            "language": string,
            "rememberWindowsLocation": boolean,
            "rememberLayouts": boolean,
            "fileExplorerDock"?: string,
            "featureListDock"?: string,
            "OutputDock"?: string,
            "propertyWindowDock"?: string,
            "taskListDock"?: string
        },
        "viewer": {
            "XIC_da": number,
            "ppm_error": number,
            "colorSet": string[],
            "method": string,
            "intoCutoff": number,
            "quantile": number,
            "fill": boolean
        },
        "network": any,
        "licensed": {},
        "version": string,
        "random": string,
        "recentFiles": string[],
        "local_blender": boolean,
        "workspaceFile": any,
        "biodeep": any,
        "msi_filters": {
            "filters": string[],
            "flags": boolean[]
        },
        "tissue_map": {
            "editor": {
                "point_size": number,
                "point_color": string,
                "show_points": boolean,
                "line_width": number,
                "dash": boolean,
                "line_color": string
            },
            "region_prefix": string,
            "opacity": number,
            "spot_size": number,
            "color_scaler": string,
            "bootstrapping": {
                "nsamples": number,
                "coverage": number
            }
        },
        "MRMLibfile": string,
        "QuantifyIonLibfile": any,
        "pubchemWebCache": string
    }

    export interface element_count {
        atom: string;
        min: number;
        max: number;
    }

    export interface element_profile {
        "type": "Wiley" | "DNP";
        "isCommon": boolean;
    }

    export interface BootstrapTable {
        bootstrapTable(arg1: any, arg2?: any);
    }
}