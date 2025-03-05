namespace molmil {

    export function localStorageGET(field, except) {
        try { return localStorage.getItem(field) || except; }
        catch (e) { return except; }
    };
}