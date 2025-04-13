namespace molmil {

    export const nfilesproc = {
        // [0]
        nfiles: 0,
        // [1]
        maxfiles: 0,
        // [2]
        data: []
    };

    export function renderOnlyFinal(soup, structures) {
        nfilesproc.nfiles++;
        if (Array.isArray(structures)) nfilesproc.data = nfilesproc.data.concat(structures)
        else nfilesproc.data.push(structures);
        if (nfilesproc.nfiles < nfilesproc.maxfiles) return;
        molmil.displayEntry(nfilesproc.data, 1);
        molmil.colorEntry(nfilesproc.data, 1, null, true, soup);
        nfilesproc.data = [];
    }
}