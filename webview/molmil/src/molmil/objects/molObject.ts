namespace molmil {

    export class molObject {
        constructor(name, id, chain) {
            this.atoms = [];
            this.name = name;
            this.id = id;
            this.RSID = id;
            this.chain = chain;
            this.ligand = true;
            this.water = false;
            this.display = 1;
            this.next = null, this.previous = null;
            this.rgba = [0, 0, 0, 255];
            this.sndStruc = 1;
            this.xna = false;
            this.showSC = false;
        }

        toString() {
            var sgl = this.atoms.length > 1; return (sgl ? (this.name || "") + " " : "") + (this.RSID || "") + (this.chain.name ? " - Chain " + this.chain.name : "");
        };
    }
}