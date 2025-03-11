module localfile {

    export var base64: string = "";
    export var parse: (base64: string) => void;

    export function clear() {
        localfile.base64 = "";
    }

    export function commit() {
        if (!isNullOrUndefined(localfile.parse)) {
            localfile.parse(base64);
        }
    }

    export function load(block: string) {
        localfile.base64 = localfile.base64 + block;
    }
}