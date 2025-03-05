/// <reference path="./general.ts" />

namespace molmil_dep {

    export class _dBT {

        public MSIE: boolean;
        public MFF: boolean;
        public GC: boolean;
        public ASF: boolean;
        public OO: boolean;
        public internal: boolean;
        public webkit: boolean;
        public gecko: boolean;
        public presto: boolean;
        public mobile: boolean;
        public denyASYNC: boolean;
        public touchBased: boolean;
        public GWP: boolean;
        public mac: boolean;
        public slow: boolean;
        public name: string;
        public language: string;
        public unknownBrowser: boolean;
        public majorVersion: number;
        public minorVersion: number;
        public ios_version;
        public featureSupport: {
            svg: boolean,

        };
        public useHistoryAPI: boolean;
        public version;
        public crossDomainScripting: boolean;
        public secureHTTP: boolean

        constructor() {
            this.MSIE = false; // internet explorer
            this.MFF = false; // firefox
            this.GC = false; // chrome
            this.ASF = false; // safari
            this.OO = false; // opera
            this.internal = false; // PhantomJS_internal
            this.webkit = false;
            this.gecko = false;
            this.presto = false;
            this.mobile = false;
            this.denyASYNC = false;
            this.touchBased = false;
            this.GWP = false; // Google Web Preview
            this.mac = false;
            this.slow = false;
            this.name = "Unknown";
            this.language = "en";
            this.unknownBrowser = false;
            this.majorVersion = 0;
            this.minorVersion = 0;
            this.ios_version = null;
            this.featureSupport = <any>{};
            try { this.featureSupport.svg = document.implementation.hasFeature("http://www.w3.org/TR/SVG11/feature#BasicStructure", "1.1"); }
            catch (e) { this.featureSupport.svg = false; }

            var version = "0.0";
            if (navigator.userAgent.indexOf("MSIE") != -1) { this.MSIE = true; this.name = "Microsoft Internet Explorer"; version = navigator.userAgent.split("MSIE")[1].split(";")[0]; }
            else if (navigator.userAgent.indexOf("Trident") != -1) { this.MSIE = true; this.name = "Microsoft Internet Explorer"; version = navigator.userAgent.split("rv:")[1]; }
            else if (navigator.userAgent.indexOf("Firefox") != -1) { this.MFF = true; this.name = "Mozilla Firefox"; this.gecko = true; version = navigator.userAgent.split("Firefox/")[1]; }
            else if (navigator.userAgent.indexOf("Chrome") != -1) { this.GC = true; this.name = "Google Chrome"; this.webkit = true; version = molmil_dep.Strip(navigator.userAgent.split("Chrome/")[1].split(" ")[0]); }
            //else if (navigator.userAgent.indexOf("PhantomJS") != -1) {this.internal = true; this.GC = true; this.name="PhantomJS"; this.webkit = true; version = "20";}
            else if (navigator.userAgent.indexOf("phantomJS_hijax") != -1) {
                this.GC = true; this.name = "PhantomJS"; this.webkit = true; version = "20"; this.useHistoryAPI = false; this.denyASYNC = true; this.internal = true; this.slow = true;
            }
            else if (navigator.userAgent.indexOf("CriOS") != -1) { this.GC = true; this.name = "Google Chrome"; this.webkit = true; version = molmil_dep.Strip(navigator.userAgent.split("CriOS/")[1].split(" ")[0]); }
            else if (navigator.userAgent.indexOf("Android") != -1) { this.GC = true; this.webkit = true; version = molmil_dep.Strip(navigator.userAgent.split("AppleWebKit/")[1].split(" ")[0]); }
            else if (navigator.userAgent.indexOf("Opera") != -1) { this.OO = true; this.name = "Opera"; this.presto = true; version = navigator.userAgent.split("Version/")[1]; }
            else if (navigator.userAgent.indexOf("Safari") != -1) { this.ASF = true; this.name = "Apple Safari"; this.webkit = true; version = navigator.userAgent.split("Version/")[1]; }
            if (navigator.userAgent.toLowerCase().indexOf("mobile") != -1 || navigator.userAgent.toLowerCase().indexOf("android") != -1 || navigator.msMaxTouchPoints) { this.mobile = true; }
            if (this.webkit && navigator.userAgent.indexOf("Google Web Preview") != -1) { this.denyASYNC = true; this.GWP = true; }
            if (navigator.userAgent.indexOf("Googlebot") != -1) { this.webkit = true; this.denyASYNC = true; this.GWP = true; }
            if (navigator.userLanguage) { this.language = navigator.userLanguage.toLowerCase(); }
            else if (navigator.language) { this.language = navigator.language.toLowerCase(); }
            if (navigator.userAgent.indexOf("Mac") != -1) this.mac = true;
            else if (navigator.language) { this.language = navigator.language; }
            //if (languageList.hasOwnProperty(this.language)) this.language = languageList[this.language];
            if (!version) version = "0.0";
            this.majorVersion = parseInt(version.split(".")[0]);
            this.minorVersion = parseInt(version.split(".")[1]);
            this.version = parseFloat(this.majorVersion + "." + this.minorVersion);
            if (navigator.userAgent.indexOf("like Mac OS X") != -1) {
                try {
                    var ios_version = (navigator.userAgent.split(" OS ")[1].split(" ")[0].split("_"));
                    this.ios_version = ios_version[0] + "." + ios_version[1];
                }
                catch (e) { this.ios_version = 0; }
            }
            if (this.mobile) this.slow = true;
            try { this.crossDomainScripting = ("withCredentials" in new XMLHttpRequest()); }
            catch (e) { this.crossDomainScripting = false; }
            this.secureHTTP = false;
            //if ((useSSL && this.crossDomainScripting) || HOST.indexOf("https://") != -1) this.secureHTTP = true;
            if (!this.webkit && !this.gecko && !this.MSIE && !this.OO) { this.unknownBrowser = true; }
            this.touchBased = molmil_dep.isTouchDevice();
            if (this.MFF && this.majorVersion < 4) this.featureSupport.svg = false;
        }
    }

    export const dBT: _dBT = new _dBT();
}