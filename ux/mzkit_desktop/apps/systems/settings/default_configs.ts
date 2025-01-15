namespace apps.systems.settings_default {

    export function defaultSettings(): mzkit_configs {
        return <mzkit_configs>{
            // mzkit app
            ui: {
                "language": "System",
                "rememberWindowsLocation": true,
                "rememberLayouts": true,
            },

            // raw file viewer
            // chromagram plot
            viewer: {
                "XIC_da": 0.05,
                "ppm_error": 20,
                "colorSet": [],
                "method": "relative",
                "intoCutoff": 0.05,
                "quantile": 0.65,
                "fill": true
            },

            precursor_search: {
                ppm: 20,
                positive: settings_default.default_adducts_pos,
                negative: settings_default.default_adducts_neg
            }
        }
    }
}