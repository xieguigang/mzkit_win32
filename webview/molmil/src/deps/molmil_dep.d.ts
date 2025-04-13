/// <reference types="node" />
declare namespace molmil_dep {
    function extendsArray(): void;
}
declare namespace molmil_dep {
    interface XDomainRequest extends XMLHttpRequest {
        XHRO: any;
        CRO: CallRemote;
        ERROR: boolean;
        loadHandler: any;
        silent: any;
    }
    export class CallRemote {
        formData: any;
        request: XDomainRequest;
        Method: any;
        parameters: string;
        headers: {};
        ASYNC: boolean;
        ctype: string;
        responseType: any;
        forceSSL: boolean;
        URL: any;
        constructor(method: any, crossDomain: any);
        initRequest(crossDomain: any): void;
        AddParameter(variable: any, value: any): void;
        AddParameters(list: any): void;
        Send(URL: any, silent: any): void;
        getURL(URL: any): string;
    }
    export {};
}
declare namespace molmil_dep {
    interface MolmilElement extends Element {
        setClass: any;
        removeClass: any;
        switchClass: any;
        pushNode: any;
    }
    function extendsElement(): void;
}
declare namespace molmil_dep {
    function extendsObject(): void;
}
declare namespace molmil_dep {
    function Strip(String: any, what?: any): any;
    function isTouchDevice(): boolean;
    function getKeyFromObject(obj: any, key: any, defaultReturn: any): any;
    function $(e: any): HTMLElement;
    function dcE(e: any): any;
    function dcEi(e: any, i: any): any;
    function isNumber(n: any): boolean;
    function Partition(String: any, point: any): string[];
    function Clear(what: any): void;
    function expandCatTree(ev: any, genOnly: any): void;
    function initCheck(): void;
    function init(): void;
    function findPos(obj: any): number[];
    function asyncStart(func: any, argList: any, thisArg: any, n: any): NodeJS.Timer;
    function focusOnTextEnd(el: any): void;
    function Rounding(what: any, decimals: any): number;
    function strRounding(what: any, decimals: any): string;
    function naturalSort(a: any, b: any): number;
    function createINPUT(TYPE: any, NAME: any, VALUE: any): any;
    function createTextBox(value: any, maxWidth: any): any;
    function isObject(item: any): boolean;
}
declare namespace molmil_dep {
    class _dBT {
        MSIE: boolean;
        MFF: boolean;
        GC: boolean;
        ASF: boolean;
        OO: boolean;
        internal: boolean;
        webkit: boolean;
        gecko: boolean;
        presto: boolean;
        mobile: boolean;
        denyASYNC: boolean;
        touchBased: boolean;
        GWP: boolean;
        mac: boolean;
        slow: boolean;
        name: string;
        language: string;
        unknownBrowser: boolean;
        majorVersion: number;
        minorVersion: number;
        ios_version: any;
        featureSupport: {
            svg: boolean;
        };
        useHistoryAPI: boolean;
        version: any;
        crossDomainScripting: boolean;
        secureHTTP: boolean;
        constructor();
    }
    const dBT: _dBT;
}
declare namespace molmil_dep {
    var staticHOST: string;
    var fontSize: number;
}
