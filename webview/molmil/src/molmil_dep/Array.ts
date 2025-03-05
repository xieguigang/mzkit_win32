namespace molmil_dep {

    interface MolmilArray {
        prototype: MolmilArray;
        unique;
        includes;
        Remove;
    }

    export function extendsArray() {
        let array: MolmilArray = <any>window.Array;

        if (!array.unique) array.prototype.unique = function () {
            var o = {}, i, l = this.length, r = [];
            for (i = 0; i < l; i += 1) o[this[i]] = this[i];
            for (i in o) r.push(o[i]);
            return r;
        };

        if (!array.prototype.includes) {
            Object.defineProperty(Array.prototype, 'includes', {
                value: function (searchElement, fromIndex) {
                    if (this == null) throw new TypeError('"this" is null or not defined');
                    var o = Object(this);
                    var len = o.length >>> 0;
                    if (len === 0) return false;
                    var n = fromIndex | 0;
                    var k = Math.max(n >= 0 ? n : len - Math.abs(n), 0);
                    function sameValueZero(x, y) { return x === y || (typeof x === 'number' && typeof y === 'number' && isNaN(x) && isNaN(y)); }
                    while (k < len) {
                        if (sameValueZero(o[k], searchElement)) return true;
                        k++;
                    }
                    return false;
                }
            });
        }

        array.prototype.Remove = function (s) {
            for (var i = 0; i < this.length; i++) { if (s == this[i]) { this.splice(i, 1); } }
        };
    }
}