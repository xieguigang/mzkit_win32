namespace molmil_dep {

    export interface MolmilElement extends Element {
        setClass;
        removeClass;
        switchClass;
        pushNode;
    }

    export function extendsElement() {
        const prop: MolmilElement = <any>window.Element.prototype;

        prop.setClass = function (className) {
            if (this.className.indexOf(className) == -1) this.className += " " + className;
        }
        prop.removeClass = function (className) {
            this.className = this.className.replace(new RegExp("\\b" + className + "\\b", "g"), "").trim();
        }
        prop.switchClass = function (className, sw) {
            let tmp = this.className.replace(new RegExp("\\b" + className + "\\b", "g"), sw);
            if (tmp == this.className) this.setClass(sw);
            else this.className = tmp;
        }
        prop.pushNode = function (node, ih, className) {
            if (typeof (node) == "string") {
                if (node == "") return this.appendChild(document.createTextNode(ih));
                else var obj = this.appendChild(molmil_dep.dcE(node));
            }
            else var obj = this.appendChild(node);
            if (ih) obj.innerHTML = ih;
            if (className) obj.className = className;
            return obj;
        }
    }
}