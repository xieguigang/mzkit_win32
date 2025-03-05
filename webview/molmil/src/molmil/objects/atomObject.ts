namespace molmil {

    // ** data objects **

    export class atomObject {
        xyz;
        element;
        atomName;
        displayMode;
        display;
        rgba;
        molecule;
        chain;
        radius;
        AID;
        Bfactor;

        constructor(Xpos, AN, element, molObj, chainObj) {
            this.xyz = Xpos; // this should become an idx
            this.element = element.charAt(0).toUpperCase() + element.slice(1).toLowerCase();
            this.atomName = AN;
            this.displayMode = 0;
            this.display = 1;
            this.rgba = [0, 0, 0, 255];
            this.molecule = molObj;
            this.chain = chainObj;
            this.radius = 0.0;
            this.AID = 0;
            this.Bfactor = 0;
        }

        toString() {
            var sgl = this.molecule.atoms.length > 1;
            return (sgl ? this.atomName : this.element) + " (" + this.AID + ") - " + (sgl ? (this.molecule.name || "") + " " : "") + (this.molecule.RSID || "") + (this.molecule.chain.name ? " - Chain " + this.molecule.chain.name : "");
        };
    }
}