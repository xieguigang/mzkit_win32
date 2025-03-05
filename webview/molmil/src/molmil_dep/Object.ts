namespace molmil_dep {

    interface MolmilObject {
        values
    }

    export function extendsObject() {
        var obj: MolmilObject = <any>window.Object;

        if (!obj.values) {
            obj.values = function (obj) {
                return Object.keys(obj).map(function (key) { return obj[key]; });
            }
        }
    }
}