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

    export enum Languages {
        System = 0,
        Chinese = 1,
        English = 2
    }

    // 函数：将字符串转换为对应的枚举值（数字）
    export function stringToLanguage(languageString: string): string {
        switch (languageString) {
            case 'System':
                return Languages.System.toString();
            case 'Chinese':
                return Languages.Chinese.toString();
            case 'English':
                return Languages.English.toString();
            default:
                return "0"; // 如果输入的字符串不匹配任何枚举值，则返回undefined
        }
    }

    // 函数：将枚举值（数字）转换回对应的字符串
    export function languageToString(languageNumber: number): string | undefined {
        switch (languageNumber) {
            case Languages.System:
                return 'System';
            case Languages.Chinese:
                return 'Chinese';
            case Languages.English:
                return 'English';
            default:
                return 'System'; // 如果输入的数字不匹配任何枚举值，则返回undefined
        }
    }

    export function logicalDefault(logic: any, _default: boolean): boolean {
        if (isNullOrUndefined(logic) || isNullOrEmpty(logic)) {
            return _default;
        } else if (typeof logic == "number") {
            return logic != 0.0;
        } else if (typeof logic == "string") {
            return parseBoolean(logic);
        } else if (typeof logic == "boolean") {
            return logic;
        } else {
            return logic;
        }
    }
}