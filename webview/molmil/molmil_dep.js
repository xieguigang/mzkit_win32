var molmil_dep;
(function (molmil_dep) {
    function extendsArray() {
        var array = window.Array;
        if (!array.unique)
            array.prototype.unique = function () {
                var o = {}, i, l = this.length, r = [];
                for (i = 0; i < l; i += 1)
                    o[this[i]] = this[i];
                for (i in o)
                    r.push(o[i]);
                return r;
            };
        if (!array.prototype.includes) {
            Object.defineProperty(Array.prototype, 'includes', {
                value: function (searchElement, fromIndex) {
                    if (this == null)
                        throw new TypeError('"this" is null or not defined');
                    var o = Object(this);
                    var len = o.length >>> 0;
                    if (len === 0)
                        return false;
                    var n = fromIndex | 0;
                    var k = Math.max(n >= 0 ? n : len - Math.abs(n), 0);
                    function sameValueZero(x, y) { return x === y || (typeof x === 'number' && typeof y === 'number' && isNaN(x) && isNaN(y)); }
                    while (k < len) {
                        if (sameValueZero(o[k], searchElement))
                            return true;
                        k++;
                    }
                    return false;
                }
            });
        }
        array.prototype.Remove = function (s) {
            for (var i = 0; i < this.length; i++) {
                if (s == this[i]) {
                    this.splice(i, 1);
                }
            }
        };
    }
    molmil_dep.extendsArray = extendsArray;
})(molmil_dep || (molmil_dep = {}));
var molmil_dep;
(function (molmil_dep) {
    var CallRemote = /** @class */ (function () {
        function CallRemote(method, crossDomain) {
            if (crossDomain === void 0) { crossDomain = false; }
            this.formData = null;
            this.request = null;
            this.Method = method;
            this.parameters = [], this.headers = {};
            if (method == "POSTv2" && typeof (FormData) != "undefined") {
                this.formData = new FormData();
            }
            else if (method == "POSTv2") {
                this.Method = "POST";
            }
            this.ASYNC = false;
            this.ctype = "application/x-www-form-urlencoded";
            this.responseType = null;
            this.forceSSL = false;
            this.initRequest(crossDomain);
        }
        CallRemote.prototype.initRequest = function (crossDomain) {
            var stupidIE = molmil_dep.dBT.MSIE && molmil_dep.dBT.majorVersion < 10;
            if (crossDomain && molmil_dep.dBT.MSIE && molmil_dep.dBT.majorVersion < 10 && typeof (XDomainRequest) != "undefined") {
                stupidIE = false;
                this.request = new XDomainRequest();
                this.Method = "POST";
            }
            else
                this.request = new XMLHttpRequest();
            this.request.XHRO = this.request;
            var ref = this.request.CRO = this; // firefox 2.0
            this.request.ERROR = false;
            this.request.loadHandler = function () {
                if (this.CRO.VirtualOnDone) {
                    this.CRO.VirtualOnDone();
                }
                if (this.status && this.status > 399) {
                    this.ERROR = true;
                }
                if (this.CRO.OnDone) {
                    try {
                        if (!this.ERROR)
                            this.CRO.OnDone();
                    }
                    catch (e) {
                        this.ERROR = e;
                    }
                    if (this.ERROR) {
                        if (this.CRO.OnError)
                            this.CRO.OnError();
                        else {
                            if (!this.silent)
                                throw this.ERROR;
                        }
                    }
                }
            };
            if (this.request.onload == null && !stupidIE) {
                this.request.onload = this.request.loadHandler;
            }
            else
                this.request.onreadystatechange = function () {
                    if (this.readyState != 4) {
                        return false;
                    }
                    this.loadHandler();
                };
            if (molmil_dep.dBT.MSIE && typeof (XDomainRequest) != "undefined" && this instanceof XDomainRequest)
                this.request.onload = this.request.onreadystatechange;
        };
        ;
        CallRemote.prototype.AddParameter = function (variable, value) {
            if (this.Method == "POSTv2") {
                this.formData.append(variable, value);
            }
            else {
                this.parameters += (this.parameters.length ? "&" : "") + variable + "=" + encodeURIComponent(value);
            }
        };
        ;
        CallRemote.prototype.AddParameters = function (list) {
            for (var i = 0; i < list.length; i++)
                this.AddParameter(list[i][0], list[i][1]);
        };
        ;
        CallRemote.prototype.Send = function (URL, silent) {
            URL = URL || this.URL;
            if (this.forceSSL)
                URL = URL.replace("http://", "https://");
            if (molmil_dep.dBT.MSIE && molmil_dep.dBT.majorVersion < 10) {
                var domain = URL.split("/");
                for (var i = 0; i < domain.length; i++)
                    if (domain[i].indexOf(".") != -1) {
                        domain = domain[i];
                        break;
                    }
                if (domain != document.domain)
                    this.initRequest(true);
            }
            if (molmil_dep.dBT.denyASYNC) {
                this.ASYNC = false;
            }
            // use withCredentials...
            if (this.Method == "GET") {
                this.request.open("GET", URL + (this.parameters.length ? "?" + this.parameters : ""), this.ASYNC);
                if (this.responseType) {
                    this.request.responseType = this.responseType;
                }
                this.request.send(null);
            }
            else if (this.Method == "POST") {
                this.request.open("POST", URL, this.ASYNC);
                try {
                    this.request.setRequestHeader("Content-Type", this.ctype);
                }
                catch (e) { }
                ; // stupid IE
                for (var e in this.headers) {
                    try {
                        this.request.setRequestHeader(e, this.headers[e]);
                    }
                    catch (e) { }
                    ; //stupid IE
                }
                if (this.responseType) {
                    this.request.responseType = this.responseType;
                }
                this.request.send((this.parameters.length ? this.parameters : null));
            }
            else if (this.Method == "POSTv2" && this.formData) {
                this.request.open("POST", URL, this.ASYNC);
                if (this.responseType) {
                    this.request.responseType = this.responseType;
                }
                this.request.send(this.formData);
            }
            else if (this.Method == "HEAD") {
                this.request.open("HEAD", URL, this.ASYNC);
                this.request.send(null);
            }
            //  if (this.request.ERROR) {throw "ERROR";}
            this.request.silent = silent;
        };
        ;
        CallRemote.prototype.getURL = function (URL) {
            return URL + (this.parameters.length ? "?" + this.parameters : "");
        };
        return CallRemote;
    }());
    molmil_dep.CallRemote = CallRemote;
})(molmil_dep || (molmil_dep = {}));
var molmil_dep;
(function (molmil_dep) {
    function extendsElement() {
        var prop = window.Element.prototype;
        prop.setClass = function (className) {
            if (this.className.indexOf(className) == -1)
                this.className += " " + className;
        };
        prop.removeClass = function (className) {
            this.className = this.className.replace(new RegExp("\\b" + className + "\\b", "g"), "").trim();
        };
        prop.switchClass = function (className, sw) {
            var tmp = this.className.replace(new RegExp("\\b" + className + "\\b", "g"), sw);
            if (tmp == this.className)
                this.setClass(sw);
            else
                this.className = tmp;
        };
        prop.pushNode = function (node, ih, className) {
            if (typeof (node) == "string") {
                if (node == "")
                    return this.appendChild(document.createTextNode(ih));
                else
                    var obj = this.appendChild(molmil_dep.dcE(node));
            }
            else
                var obj = this.appendChild(node);
            if (ih)
                obj.innerHTML = ih;
            if (className)
                obj.className = className;
            return obj;
        };
    }
    molmil_dep.extendsElement = extendsElement;
})(molmil_dep || (molmil_dep = {}));
var molmil_dep;
(function (molmil_dep) {
    function extendsObject() {
        var obj = window.Object;
        if (!obj.values) {
            obj.values = function (obj) {
                return Object.keys(obj).map(function (key) { return obj[key]; });
            };
        }
    }
    molmil_dep.extendsObject = extendsObject;
})(molmil_dep || (molmil_dep = {}));
var molmil_dep;
(function (molmil_dep) {
    // general functions
    function Strip(String, what) {
        return String.replace(/^\s+|\s+$/g, "");
    }
    molmil_dep.Strip = Strip;
    function isTouchDevice() {
        try {
            var el = document.createElement("DIV");
            el.setAttribute('ongesturestart', 'return;');
            return typeof el.ongesturestart == "function";
        }
        catch (e) {
            return false;
        }
    }
    molmil_dep.isTouchDevice = isTouchDevice;
    function getKeyFromObject(obj, key, defaultReturn) { return key in obj ? obj[key] : defaultReturn; }
    molmil_dep.getKeyFromObject = getKeyFromObject;
    function $(e) { return document.getElementById(e); }
    molmil_dep.$ = $;
    function dcE(e) { return document.createElement(e); }
    molmil_dep.dcE = dcE;
    function dcEi(e, i) { var E = document.createElement(e); E.innerHTML = i; return E; }
    molmil_dep.dcEi = dcEi;
    function isNumber(n) { return !isNaN(parseFloat(n)) && isFinite(n); }
    molmil_dep.isNumber = isNumber;
    function Partition(String, point) {
        var results = ["", "", ""];
        var loc = String.indexOf(point);
        if (loc == -1) {
            results[0] = String;
        }
        else {
            results[0] = String.substr(0, loc);
            results[1] = point;
            results[2] = String.substr(loc + 1);
        }
        return results;
    }
    molmil_dep.Partition = Partition;
    function Clear(what) { if (what && what.hasChildNodes()) {
        while (what.childNodes.length > 0) {
            what.removeChild(what.firstChild);
        }
    } }
    molmil_dep.Clear = Clear;
    ;
    function expandCatTree(ev, genOnly) {
        var target = this.parentNode;
        if (typeof (target.kids) == "undefined") {
            target.kids = target.pushNode("DIV");
            target.kids.style.display = "none";
            target.kids.expandableTarget = target.expandableTarget;
            target.expandFunc(target.kids, target.payload);
            target.kids.style.marginLeft = "1.25em";
        }
        if (genOnly)
            return;
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
    molmil_dep.expandCatTree = expandCatTree;
    function initCheck() {
        if (molmil.configBox.initFinished)
            return;
        if (!window.glMatrix)
            return;
        molmil.configBox.initFinished = true;
    }
    molmil_dep.initCheck = initCheck;
    ;
    function init() {
        if (!window.molmil.configBox || molmil.configBox.initFinished)
            return;
        var deps = molmil.settings.dependencies, obj;
        var head = document.getElementsByTagName("head")[0];
        for (var i = 0; i < deps.length; i++) {
            obj = molmil_dep.dcE("script");
            if (deps[i].indexOf("//") == -1)
                obj.src = molmil.settings.src + deps[i];
            else
                obj.src = deps[i];
            obj.onload = molmil_dep.initCheck;
            head.appendChild(obj);
        }
        var css = ["molmil.css"];
        for (var i = 0; i < css.length; i++) {
            obj = document.createElement("link");
            obj.rel = "StyleSheet";
            obj.href = molmil.settings.src + css[i];
            obj = head.appendChild(obj);
        }
    }
    molmil_dep.init = init;
    ;
    function findPos(obj) {
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
    }
    molmil_dep.findPos = findPos;
    ;
    function asyncStart(func, argList, thisArg, n) { return setTimeout(function () { func.apply(thisArg, argList); }, n ? n : 0); }
    molmil_dep.asyncStart = asyncStart;
    function focusOnTextEnd(el) {
        try {
            if (typeof el.selectionStart == "number") {
                el.selectionStart = el.selectionEnd = el.value.length;
            }
            else if (typeof el.createTextRange != "undefined") {
                el.focus();
                var range = el.createTextRange();
                range.collapse(false);
                range.select();
            }
        }
        catch (e) { }
    }
    molmil_dep.focusOnTextEnd = focusOnTextEnd;
    function Rounding(what, decimals) {
        decimals = Math.pow(10, decimals);
        return Math.round(what * decimals) / decimals;
    }
    molmil_dep.Rounding = Rounding;
    function strRounding(what, decimals) {
        var out = molmil_dep.Rounding(what, decimals) + "";
        if (decimals > 0) {
            if (out.indexOf(".") == -1)
                out += ".";
            else
                decimals -= out.split(".")[1].length;
            for (var i = 0; i < decimals; i++)
                out += "0";
        }
        return out;
    }
    molmil_dep.strRounding = strRounding;
    function naturalSort(a, b) {
        function chunkify(t) {
            var tz = [], x = 0, y = -1, n = 0, i, j;
            var m;
            while (i = (j = t.charAt(x++)).charCodeAt(0)) {
                m = (i == 46 || (i >= 48 && i <= 57));
                if (m !== n) {
                    tz[++y] = "";
                    n = m;
                }
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
                if (c == aa[x] && d == bb[x]) {
                    return c - d;
                }
                else
                    return (aa[x] > bb[x]) ? 1 : -1;
            }
        }
        return aa.length - bb.length;
    }
    molmil_dep.naturalSort = naturalSort;
    function createINPUT(TYPE, NAME, VALUE) {
        var input;
        input = molmil_dep.dcE("INPUT");
        input.type = TYPE;
        input.name = (NAME ? NAME : "");
        input.value = (VALUE ? VALUE : "");
        return input;
    }
    molmil_dep.createINPUT = createINPUT;
    ;
    function createTextBox(value, maxWidth) {
        var tb = molmil_dep.dcE("INPUT");
        tb.className = "textBox";
        tb.setAttribute("type", "text");
        if (maxWidth) {
            tb.style.maxWidth = maxWidth;
            tb.style.width = "";
        }
        tb.getContent = function () { return this.value; };
        tb.setContent = function (value) { this.value = value; };
        if (value)
            tb.setAttribute("value", value);
        return tb;
    }
    molmil_dep.createTextBox = createTextBox;
    function isObject(item) {
        return (typeof item === "object" && !Array.isArray(item) && item !== null);
    }
    molmil_dep.isObject = isObject;
})(molmil_dep || (molmil_dep = {}));
/// <reference path="./general.ts" />
var molmil_dep;
(function (molmil_dep) {
    var _dBT = /** @class */ (function () {
        function _dBT() {
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
            try {
                this.featureSupport.svg = document.implementation.hasFeature("http://www.w3.org/TR/SVG11/feature#BasicStructure", "1.1");
            }
            catch (e) {
                this.featureSupport.svg = false;
            }
            var version = "0.0";
            if (navigator.userAgent.indexOf("MSIE") != -1) {
                this.MSIE = true;
                this.name = "Microsoft Internet Explorer";
                version = navigator.userAgent.split("MSIE")[1].split(";")[0];
            }
            else if (navigator.userAgent.indexOf("Trident") != -1) {
                this.MSIE = true;
                this.name = "Microsoft Internet Explorer";
                version = navigator.userAgent.split("rv:")[1];
            }
            else if (navigator.userAgent.indexOf("Firefox") != -1) {
                this.MFF = true;
                this.name = "Mozilla Firefox";
                this.gecko = true;
                version = navigator.userAgent.split("Firefox/")[1];
            }
            else if (navigator.userAgent.indexOf("Chrome") != -1) {
                this.GC = true;
                this.name = "Google Chrome";
                this.webkit = true;
                version = molmil_dep.Strip(navigator.userAgent.split("Chrome/")[1].split(" ")[0]);
            }
            //else if (navigator.userAgent.indexOf("PhantomJS") != -1) {this.internal = true; this.GC = true; this.name="PhantomJS"; this.webkit = true; version = "20";}
            else if (navigator.userAgent.indexOf("phantomJS_hijax") != -1) {
                this.GC = true;
                this.name = "PhantomJS";
                this.webkit = true;
                version = "20";
                this.useHistoryAPI = false;
                this.denyASYNC = true;
                this.internal = true;
                this.slow = true;
            }
            else if (navigator.userAgent.indexOf("CriOS") != -1) {
                this.GC = true;
                this.name = "Google Chrome";
                this.webkit = true;
                version = molmil_dep.Strip(navigator.userAgent.split("CriOS/")[1].split(" ")[0]);
            }
            else if (navigator.userAgent.indexOf("Android") != -1) {
                this.GC = true;
                this.webkit = true;
                version = molmil_dep.Strip(navigator.userAgent.split("AppleWebKit/")[1].split(" ")[0]);
            }
            else if (navigator.userAgent.indexOf("Opera") != -1) {
                this.OO = true;
                this.name = "Opera";
                this.presto = true;
                version = navigator.userAgent.split("Version/")[1];
            }
            else if (navigator.userAgent.indexOf("Safari") != -1) {
                this.ASF = true;
                this.name = "Apple Safari";
                this.webkit = true;
                version = navigator.userAgent.split("Version/")[1];
            }
            if (navigator.userAgent.toLowerCase().indexOf("mobile") != -1 || navigator.userAgent.toLowerCase().indexOf("android") != -1 || navigator.msMaxTouchPoints) {
                this.mobile = true;
            }
            if (this.webkit && navigator.userAgent.indexOf("Google Web Preview") != -1) {
                this.denyASYNC = true;
                this.GWP = true;
            }
            if (navigator.userAgent.indexOf("Googlebot") != -1) {
                this.webkit = true;
                this.denyASYNC = true;
                this.GWP = true;
            }
            if (navigator.userLanguage) {
                this.language = navigator.userLanguage.toLowerCase();
            }
            else if (navigator.language) {
                this.language = navigator.language.toLowerCase();
            }
            if (navigator.userAgent.indexOf("Mac") != -1)
                this.mac = true;
            else if (navigator.language) {
                this.language = navigator.language;
            }
            //if (languageList.hasOwnProperty(this.language)) this.language = languageList[this.language];
            if (!version)
                version = "0.0";
            this.majorVersion = parseInt(version.split(".")[0]);
            this.minorVersion = parseInt(version.split(".")[1]);
            this.version = parseFloat(this.majorVersion + "." + this.minorVersion);
            if (navigator.userAgent.indexOf("like Mac OS X") != -1) {
                try {
                    var ios_version = (navigator.userAgent.split(" OS ")[1].split(" ")[0].split("_"));
                    this.ios_version = ios_version[0] + "." + ios_version[1];
                }
                catch (e) {
                    this.ios_version = 0;
                }
            }
            if (this.mobile)
                this.slow = true;
            try {
                this.crossDomainScripting = ("withCredentials" in new XMLHttpRequest());
            }
            catch (e) {
                this.crossDomainScripting = false;
            }
            this.secureHTTP = false;
            //if ((useSSL && this.crossDomainScripting) || HOST.indexOf("https://") != -1) this.secureHTTP = true;
            if (!this.webkit && !this.gecko && !this.MSIE && !this.OO) {
                this.unknownBrowser = true;
            }
            this.touchBased = molmil_dep.isTouchDevice();
            if (this.MFF && this.majorVersion < 4)
                this.featureSupport.svg = false;
        }
        return _dBT;
    }());
    molmil_dep._dBT = _dBT;
    molmil_dep.dBT = new _dBT();
})(molmil_dep || (molmil_dep = {}));
var molmil_dep;
(function (molmil_dep) {
    // settings
    molmil_dep.staticHOST = "";
    molmil_dep.fontSize = 16;
    try {
        document.body.style.fontSize = "";
        molmil_dep.fontSize = Number(getComputedStyle(document.body, "").fontSize.match(/(\d*(\.\d*)?)px/)[1]);
    }
    catch (e) {
        molmil_dep.fontSize = 16;
    }
    ;
})(molmil_dep || (molmil_dep = {}));
/// <reference path="./settings.ts" />
/// <reference path="./general.ts" />
/// <reference path="./dBT.ts" />
/// <reference path="./CallRemote.ts" />
/// <reference path="./Element.ts" />
/// <reference path="./Object.ts" />
/// <reference path="./Array.ts" />
/*!
 * molmil_dep.js
 *
 * Molmil molecular web viewer: https://github.com/gjbekker/molmil
 *
 * By Gert-Jan Bekker
 * License: LGPLv3
 *   See https://github.com/gjbekker/molmil/blob/master/LICENCE.md
 */
molmil_dep.extendsElement();
molmil_dep.extendsArray();
molmil_dep.extendsObject();
molmil_dep.init();
//# sourceMappingURL=molmil_dep.js.map