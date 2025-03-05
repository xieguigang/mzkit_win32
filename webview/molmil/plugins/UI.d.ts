/// <reference path="../molmil.d.ts" />
declare namespace molmil {
    class UI {
        constructor(soup: any);
        init(): void;
        deleteEntry(entry: any): void;
        displayEntry(entry: any, dm: any): void;
        editLabel(settings: any): void;
        showContextMenuAtom(x: any, y: any, pageX: any): void;
        showRM(icon: any, reset: any): void;
        showChains(target: any, payload: any): void;
        showResidues(target: any, payload: any): void;
        showLM(icon: any): void;
        view(sub: any): void;
        toggleWaters(show: any): void;
        toggleHydrogens(show: any): void;
        animationPopUp(): void;
        resetRM(): void;
        toggleCLI(): void;
        clear(): void;
        showDialog(func: any): any;
        xyz_input_popup(fp: any, fn: any, cb: any): void;
        ccp4_input_popup(fp: any, fn: any, cb: any): void;
        open(name: any, format: any, ondone: any, oncancel: any, binary: any): any;
        openID(dbid: any): void;
        savePNG(): void;
        videoRenderer(justStart: any): void;
        meshOptionsFunction(payload: any, lv: any, mode: any): void;
        deleteMeshFunction(payload: any, lv: any, mode: any): void;
        displayFunction(payload: any, lv: any, mode: any): void;
        popupMenuBuilder(title: any, fields: any, ondone: any, oncancel: any): void;
        colorFunction(payload: any, lv: any, mode: any, setting: any): void;
        labelFunction(payload: any, lv: any): void;
        initMenus(): void;
        buildComplexMenu(title: any, structure: any, previousCall: any, payload: any): void;
        styleif_au(contentBox: any): void;
        styleif_bu(contentBox: any, afterDL: any): any;
        styleif_cc(contentBox: any, afterDL: any): void;
        drag_panel(title: any, top: any, left: any): any;
        styleif_mesh(mesh: any, ev: any, options: any): void;
        styleif_edmap(contentBox: any, callOptions: any): number;
        styleif_sites(contentBox: any): number;
        styleif_align(contentBox: any): void;
        styleif_settings(contentBox: any): void;
        styleif(showOption: any, callOptions: any): void;
    }
}
declare namespace molmil {
    class animationObj {
        soup: any;
        renderer: any;
        frameNo: any;
        motionMode: any;
        init: any;
        delay: any;
        frameAction: any;
        detail_or: any;
        infoBox: any;
        number_of_frames: any;
        TID: any;
        playing: any;
        constructor(soup: any);
        initialise(infoBox?: any): void;
        updateInfoBox(): void;
        beginning(): void;
        go2Frame(fid: any): void;
        previous(): void;
        pause(): void;
        play(): void;
        next(): void;
        end(): void;
        forwardRenderer(): any;
        backwardRenderer(): any;
    }
}
declare namespace molmil {
    function setOnContextMenu(obj: any, func: any, lefttoo: any): void;
    function initVideo(UI: any): void;
}
