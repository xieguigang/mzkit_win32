namespace molmil {

    export class chainObject {
        constructor(name, entry) {
            this.name = name;
            this.authName = name;
            this.molecules = [];
            this.entry = entry;
            this.display = true;
            this.modelsXYZ = [[]];
            this.atoms = [];
            this.bonds = [];
            this.branches = [];
            this.showBBatoms = [];
            this.bondsOK = false;
            this.displayMode = molmil.displayMode_Default;
            this.isHet = true;
            this.rgba = [255, 255, 255, 255];
            this.display = true;
        }

        toString() { return (this.name ? "Chain " + this.name : ""); };
    }
}