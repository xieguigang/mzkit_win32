namespace apps.systems {

    export interface mzkit_configs {
        // mzkit app
        "remember_location": boolean;
        "remember_layout": boolean;
        "language": 0 | 1 | 2;

        // raw file viewer
        "xic_da": number;
        "fragment_cutoff": "relative" | "quantile";
        "fragment_cutoff_value": number;

        // chromagram plot
        "colorset": string[];
        "fill_plot_area": boolean;

        // preset element profiles
        "formula_search": {
            "naturalProductProfile": element_profile,
            "smallMoleculeProfile": element_profile,
            "elements": {}
        };

        "formula_ppm": number;
        "formula_adducts": {
            pos: string[],
            neg: string[]
        };

        // molecular networking
        "layout_iterations": number;

        // graph layouts
        "stiffness": number;
        "repulsion": number;
        "damping": number;

        // spectrum tree
        "node_identical": number;
        "node_similar": number;
        "edge_filter": number;

        // network styling
        "node_radius_min": number;
        "node_radius_max": number;

        "link_width_min": number;
        "link_width_max": number;
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