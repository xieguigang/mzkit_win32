module molmil_dep {

    // general functions

    export function Strip(String, what?) {
        return String.replace(/^\s+|\s+$/g, "");
    }

    export function isTouchDevice() {
        try {
            var el = document.createElement("DIV");
            el.setAttribute('ongesturestart', 'return;');
            return typeof el.ongesturestart == "function";
        } catch (e) { return false; }
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

    export function isObject(item) {
        return (typeof item === "object" && !Array.isArray(item) && item !== null);
    }
}