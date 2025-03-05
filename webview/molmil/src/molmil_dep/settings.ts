module molmil_dep {
    // settings

    export var staticHOST: string = "";
    export var fontSize: number = 16;

    try {
        document.body.style.fontSize = "";
        molmil_dep.fontSize = Number(getComputedStyle(document.body, "").fontSize.match(/(\d*(\.\d*)?)px/)[1]);
    }
    catch (e) {
        molmil_dep.fontSize = 16;
    };
}