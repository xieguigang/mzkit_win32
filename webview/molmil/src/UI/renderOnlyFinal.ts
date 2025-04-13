namespace molmil {

    export const nfilesproc = [0, 0, []];

    export function renderOnlyFinal(soup, structures) {
        nfilesproc[0]++;
        if (Array.isArray(structures)) nfilesproc[2] = nfilesproc[2].concat(structures)
        else nfilesproc[2].push(structures);
        if (nfilesproc[0] < nfilesproc[1]) return;
        molmil.displayEntry(nfilesproc[2], 1);
        molmil.colorEntry(nfilesproc[2], 1, null, true, soup);
        nfilesproc[2] = [];
    }
}