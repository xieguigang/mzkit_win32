module molmil_dep {

    // general functions

    export function Strip(String, what) { return String.replace(/^\s+|\s+$/g, ""); }

    export function isTouchDevice() {
        try {
            var el = document.createElement("DIV");
            el.setAttribute('ongesturestart', 'return;');
            return typeof el.ongesturestart == "function";
        } catch (e) { return false; }
    }


    export function dBT() {
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
        this.featureSupport = {};
        try { this.featureSupport.svg = document.implementation.hasFeature("http://www.w3.org/TR/SVG11/feature#BasicStructure", "1.1"); }
        catch (e) { this.featureSupport.svg = false; }

        var version = "0.0";
        if (navigator.userAgent.indexOf("MSIE") != -1) { this.MSIE = true; this.name = "Microsoft Internet Explorer"; version = navigator.userAgent.split("MSIE")[1].split(";")[0]; }
        else if (navigator.userAgent.indexOf("Trident") != -1) { this.MSIE = true; this.name = "Microsoft Internet Explorer"; version = navigator.userAgent.split("rv:")[1]; }
        else if (navigator.userAgent.indexOf("Firefox") != -1) { this.MFF = true; this.name = "Mozilla Firefox"; this.gecko = true; version = navigator.userAgent.split("Firefox/")[1]; }
        else if (navigator.userAgent.indexOf("Chrome") != -1) { this.GC = true; this.name = "Google Chrome"; this.webkit = true; version = molmil_dep.Strip(navigator.userAgent.split("Chrome/")[1].split(" ")[0]); }
        //else if (navigator.userAgent.indexOf("PhantomJS") != -1) {this.internal = true; this.GC = true; this.name="PhantomJS"; this.webkit = true; version = "20";}
        else if (navigator.userAgent.indexOf("phantomJS_hijax") != -1) { this.GC = true; this.name = "PhantomJS"; this.webkit = true; version = "20"; useHistoryAPI = false; this.denyASYNC = true; this.internal = true; this.slow = true; }
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



    export function getKeyFromObject(obj, key, defaultReturn) { return key in obj ? obj[key] : defaultReturn; }

    export function $(e) { return document.getElementById(e); }
    export function dcE(e) { return document.createElement(e); }
    export function dcEi(e, i) { var E = document.createElement(e); E.innerHTML = i; return E; }


    export function isNumber(n) { return !isNaN(parseFloat(n)) && isFinite(n); }

    export function Partition(String, point) {
        var results = ["", "", ""];
        var loc = String.indexOf(point);
        if (loc == -1) { results[0] = String; }
        else { results[0] = String.substr(0, loc); results[1] = point; results[2] = String.substr(loc + 1); }
        return results;
    }


    export function Clear(what) { if (what && what.hasChildNodes()) { while (what.childNodes.length > 0) { what.removeChild(what.firstChild); } } };

    export function expandCatTree(ev, genOnly) {
        var target = this.parentNode;
        if (typeof (target.kids) == "undefined") {
            target.kids = target.pushNode("DIV");
            target.kids.style.display = "none";
            target.kids.expandableTarget = target.expandableTarget;
            target.expandFunc(target.kids, target.payload);
            target.kids.style.marginLeft = "1.25em";
        }
        if (genOnly) return;
        if (target.kids.style.display == "none") {
            target.plus.innerHTML = "-";
            target.kids.style.display = "";
        }
        else {
            target.plus.innerHTML = "+";
            target.kids.style.display = "none";
        }

        if (target.expandableTarget) {
            target.expandableTarget.contentBox.style.height = target.expandableTarget.contentBox.firstChild.clientHeight + "px";
        }
    }


    export function initCheck() {
        if (molmil.configBox.initFinished) return;
        if (!window.glMatrix) return;
        molmil.configBox.initFinished = true;
    };

    export function init() {
        if (!window.molmil.configBox || molmil.configBox.initFinished) return;
        var deps = molmil.settings.dependencies, obj;
        var head = document.getElementsByTagName("head")[0];
        for (var i = 0; i < deps.length; i++) {
            obj = molmil_dep.dcE("script");
            if (deps[i].indexOf("//") == -1) obj.src = molmil.settings.src + deps[i];
            else obj.src = deps[i];
            obj.onload = molmil_dep.initCheck;
            head.appendChild(obj);
        }
        var css = ["molmil.css"];
        for (var i = 0; i < css.length; i++) {
            obj = document.createElement("link"); obj.rel = "StyleSheet"; obj.href = molmil.settings.src + css[i];
            obj = head.appendChild(obj);
        }
    };

    export function findPos(obj) {
        var x = 0;
        var y = 0;
        if (obj.offsetParent) {
            x += obj.offsetLeft;
            y += obj.offsetTop;
            while (obj = obj.offsetParent) {
                x += obj.offsetLeft;
                y += obj.offsetTop;
            }
        }
        return [x, y];
    };

    export function asyncStart(func, argList, thisArg, n) { return setTimeout(function () { func.apply(thisArg, argList); }, n ? n : 0); }

    export function focusOnTextEnd(el) {
        try {
            if (typeof el.selectionStart == "number") { el.selectionStart = el.selectionEnd = el.value.length; }
            else if (typeof el.createTextRange != "undefined") {
                el.focus();
                var range = el.createTextRange();
                range.collapse(false);
                range.select();
            }
        }
        catch (e) { }
    }

    export function Rounding(what, decimals) {
        decimals = Math.pow(10, decimals);
        return Math.round(what * decimals) / decimals;
    }

    export function strRounding(what, decimals) {
        var out = molmil_dep.Rounding(what, decimals) + "";
        if (decimals > 0) {
            if (out.indexOf(".") == -1) out += ".";
            else decimals -= out.split(".")[1].length;
            for (var i = 0; i < decimals; i++) out += "0";
        }
        return out;
    }

    export function naturalSort(a, b) {
        function chunkify(t) {
            var tz = [], x = 0, y = -1, n = 0, i, j; var m;
            while (i = (j = t.charAt(x++)).charCodeAt(0)) {
                m = (i == 46 || (i >= 48 && i <= 57));
                if (m !== n) { tz[++y] = ""; n = m; }
                tz[y] += j;
            }
            return tz;
        }

        var aa = chunkify(a);
        var bb = chunkify(b);
        var c, d;
        for (var x = 0; aa[x] && bb[x]; x++) {
            if (aa[x] !== bb[x]) {
                c = Number(aa[x]), d = Number(bb[x]);
                if (c == aa[x] && d == bb[x]) { return c - d; }
                else return (aa[x] > bb[x]) ? 1 : -1;
            }
        }
        return aa.length - bb.length;
    }

    export function createINPUT(TYPE, NAME, VALUE) {
        var input;
        input = molmil_dep.dcE("INPUT"); input.type = TYPE;
        input.name = (NAME ? NAME : ""); input.value = (VALUE ? VALUE : "");
        return input;
    };

    export function createTextBox(value, maxWidth) {
        var tb = molmil_dep.dcE("INPUT");
        tb.className = "textBox";
        tb.setAttribute("type", "text");
        if (maxWidth) { tb.style.maxWidth = maxWidth; tb.style.width = ""; }
        tb.getContent = function () { return this.value; };
        tb.setContent = function (value) { this.value = value; };
        if (value) tb.setAttribute("value", value);
        return tb;
    }
}