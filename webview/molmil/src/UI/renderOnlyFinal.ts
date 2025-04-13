namespace molmil {

    export const nfilesproc = { nfiles: 0, maxfiles: 0, data: [] };

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