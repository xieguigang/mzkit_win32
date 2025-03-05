/// <reference path="./settings.ts" />
/// <reference path="./CallRemote.ts" />
/// <reference path="./general.ts" />

/*!
 * molmil_dep.js
 *
 * Molmil molecular web viewer: https://github.com/gjbekker/molmil
 * 
 * By Gert-Jan Bekker
 * License: LGPLv3
 *   See https://github.com/gjbekker/molmil/blob/master/LICENCE.md
 */

Element.prototype.setClass = function (className) { if (this.className.indexOf(className) == -1) this.className += " " + className; }
Element.prototype.removeClass = function (className) { this.className = this.className.replace(new RegExp("\\b" + className + "\\b", "g"), "").trim(); }
Element.prototype.switchClass = function (className, sw) {
  tmp = this.className.replace(new RegExp("\\b" + className + "\\b", "g"), sw);
  if (tmp == this.className) this.setClass(sw);
  else this.className = tmp;
}


Element.prototype.pushNode = function (node, ih, className) {
  if (typeof (node) == "string") {
    if (node == "") return this.appendChild(document.createTextNode(ih));
    else var obj = this.appendChild(molmil_dep.dcE(node));
  }
  else var obj = this.appendChild(node);
  if (ih) obj.innerHTML = ih;
  if (className) obj.className = className;
  return obj;
}

if (!Array.unique) Array.prototype.unique = function () {
  var o = {}, i, l = this.length, r = [];
  for (i = 0; i < l; i += 1) o[this[i]] = this[i];
  for (i in o) r.push(o[i]);
  return r;
};


if (!Array.prototype.includes) {
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

Array.prototype.Remove = function (s) { for (var i = 0; i < this.length; i++) { if (s == this[i]) { this.splice(i, 1); } } };

if (!Object.values) { Object.values = function (obj) { return Object.keys(obj).map(function (key) { return obj[key]; }); } }

molmil_dep.isObject = function (item) { return (typeof item === "object" && !Array.isArray(item) && item !== null); }


molmil_dep.init();
