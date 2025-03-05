namespace molmil {

    export const formatList = {};
    
    molmil.formatList[".pdb"] = molmil.formatList[".ent"] = "pdb";
    molmil.formatList[".mmtf"] = "mmtf";
    molmil.formatList[".cif"] = "mmcif";
    molmil.formatList[".gro"] = "gro";
    molmil.formatList[".trr"] = "gromacs-trr";
    molmil.formatList[".xtc"] = "gromacs-xtc";
    molmil.formatList[".cor"] = molmil.formatList[".cod"] = "psygene-traj";
    molmil.formatList[".mnt"] = "presto-mnt";
    molmil.formatList[".mpbf"] = "mpbf";
    molmil.formatList[".ccp4"] = "ccp4";
    molmil.formatList[".mdl"] = molmil.formatList[".mol"] = molmil.formatList[".sdl"] = molmil.formatList[".sdf"] = "mdl";
    molmil.formatList[".mol2"] = "mol2";
    molmil.formatList[".xyz"] = "xyz";
    molmil.formatList[".obj"] = "obj";
    molmil.formatList[".wrl"] = "wrl";
    molmil.formatList[".stl"] = "stl";
    molmil.formatList[".ply"] = "ply";
    molmil.formatList[".mjs"] = "mjs";

    export function guess_format(name) {
        var format = null;
        var fname = name.split("/").slice(-1)[0].replace(".gz", "");
        for (var ext in molmil.formatList) {
            if (fname.substr(fname.length - ext.length) == ext) { format = molmil.formatList[ext]; break; }
        }
        return format;
    };

    export function loadFilePointer(fileObj, func, canvas) {
        fileObj.onload = function (e) {
            func(e.target.result, this.filename);
            canvas.molmilViewer.downloadInProgress--;
        }
        canvas.molmilViewer.downloadInProgress++;
        fileObj.readAsText(fileObj.fileHandle);
        return true;
    }
}