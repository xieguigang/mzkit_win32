namespace molmil {

    export const alignInfo = {};

    export function align(A, B, options) {
        // https://github.com/CDCgov/bioseq-js/blob/master/dist/bioseq.js
        if (!window.bioseq) return molmil.loadPlugin("https://raw.githubusercontent.com/CDCgov/bioseq-js/master/dist/bioseq.js", molmil.align, molmil, [A, B, options]);
        options = options || {};
        if (!bioseq.amino_acids) {
            var blosum62 = { "*": { "*": 1, "A": -4, "C": -4, "B": -4, "E": -4, "D": -4, "G": -4, "F": -4, "I": -4, "H": -4, "K": -4, "M": -4, "L": -4, "N": -4, "Q": -4, "P": -4, "S": -4, "R": -4, "T": -4, "W": -4, "V": -4, "Y": -4, "X": -4, "Z": -4 }, "A": { "*": -4, "A": 4, "C": 0, "B": -2, "E": -1, "D": -2, "G": 0, "F": -2, "I": -1, "H": -2, "K": -1, "M": -1, "L": -1, "N": -2, "Q": -1, "P": -1, "S": 1, "R": -1, "T": 0, "W": -3, "V": 0, "Y": -2, "X": 0, "Z": -1 }, "C": { "*": -4, "A": 0, "C": 9, "B": -3, "E": -4, "D": -3, "G": -3, "F": -2, "I": -1, "H": -3, "K": -3, "M": -1, "L": -1, "N": -3, "Q": -3, "P": -3, "S": -1, "R": -3, "T": -1, "W": -2, "V": -1, "Y": -2, "X": -2, "Z": -3 }, "B": { "*": -4, "A": -2, "C": -3, "B": 4, "E": 1, "D": 4, "G": -1, "F": -3, "I": -3, "H": 0, "K": 0, "M": -3, "L": -4, "N": 3, "Q": 0, "P": -2, "S": 0, "R": -1, "T": -1, "W": -4, "V": -3, "Y": -3, "X": -1, "Z": 1 }, "E": { "*": -4, "A": -1, "C": -4, "B": 1, "E": 5, "D": 2, "G": -2, "F": -3, "I": -3, "H": 0, "K": 1, "M": -2, "L": -3, "N": 0, "Q": 2, "P": -1, "S": 0, "R": 0, "T": -1, "W": -3, "V": -2, "Y": -2, "X": -1, "Z": 4 }, "D": { "*": -4, "A": -2, "C": -3, "B": 4, "E": 2, "D": 6, "G": -1, "F": -3, "I": -3, "H": -1, "K": -1, "M": -3, "L": -4, "N": 1, "Q": 0, "P": -1, "S": 0, "R": -2, "T": -1, "W": -4, "V": -3, "Y": -3, "X": -1, "Z": 1 }, "G": { "*": -4, "A": 0, "C": -3, "B": -1, "E": -2, "D": -1, "G": 6, "F": -3, "I": -4, "H": -2, "K": -2, "M": -3, "L": -4, "N": 0, "Q": -2, "P": -2, "S": 0, "R": -2, "T": -2, "W": -2, "V": -3, "Y": -3, "X": -1, "Z": -2 }, "F": { "*": -4, "A": -2, "C": -2, "B": -3, "E": -3, "D": -3, "G": -3, "F": 6, "I": 0, "H": -1, "K": -3, "M": 0, "L": 0, "N": -3, "Q": -3, "P": -4, "S": -2, "R": -3, "T": -2, "W": 1, "V": -1, "Y": 3, "X": -1, "Z": -3 }, "I": { "*": -4, "A": -1, "C": -1, "B": -3, "E": -3, "D": -3, "G": -4, "F": 0, "I": 4, "H": -3, "K": -3, "M": 1, "L": 2, "N": -3, "Q": -3, "P": -3, "S": -2, "R": -3, "T": -1, "W": -3, "V": 3, "Y": -1, "X": -1, "Z": -3 }, "H": { "*": -4, "A": -2, "C": -3, "B": 0, "E": 0, "D": -1, "G": -2, "F": -1, "I": -3, "H": 8, "K": -1, "M": -2, "L": -3, "N": 1, "Q": 0, "P": -2, "S": -1, "R": 0, "T": -2, "W": -2, "V": -3, "Y": 2, "X": -1, "Z": 0 }, "K": { "*": -4, "A": -1, "C": -3, "B": 0, "E": 1, "D": -1, "G": -2, "F": -3, "I": -3, "H": -1, "K": 5, "M": -1, "L": -2, "N": 0, "Q": 1, "P": -1, "S": 0, "R": 2, "T": -1, "W": -3, "V": -2, "Y": -2, "X": -1, "Z": 1 }, "M": { "*": -4, "A": -1, "C": -1, "B": -3, "E": -2, "D": -3, "G": -3, "F": 0, "I": 1, "H": -2, "K": -1, "M": 5, "L": 2, "N": -2, "Q": 0, "P": -2, "S": -1, "R": -1, "T": -1, "W": -1, "V": 1, "Y": -1, "X": -1, "Z": -1 }, "L": { "*": -4, "A": -1, "C": -1, "B": -4, "E": -3, "D": -4, "G": -4, "F": 0, "I": 2, "H": -3, "K": -2, "M": 2, "L": 4, "N": -3, "Q": -2, "P": -3, "S": -2, "R": -2, "T": -1, "W": -2, "V": 1, "Y": -1, "X": -1, "Z": -3 }, "N": { "*": -4, "A": -2, "C": -3, "B": 3, "E": 0, "D": 1, "G": 0, "F": -3, "I": -3, "H": 1, "K": 0, "M": -2, "L": -3, "N": 6, "Q": 0, "P": -2, "S": 1, "R": 0, "T": 0, "W": -4, "V": -3, "Y": -2, "X": -1, "Z": 0 }, "Q": { "*": -4, "A": -1, "C": -3, "B": 0, "E": 2, "D": 0, "G": -2, "F": -3, "I": -3, "H": 0, "K": 1, "M": 0, "L": -2, "N": 0, "Q": 5, "P": -1, "S": 0, "R": 1, "T": -1, "W": -2, "V": -2, "Y": -1, "X": -1, "Z": 3 }, "P": { "*": -4, "A": -1, "C": -3, "B": -2, "E": -1, "D": -1, "G": -2, "F": -4, "I": -3, "H": -2, "K": -1, "M": -2, "L": -3, "N": -2, "Q": -1, "P": 7, "S": -1, "R": -2, "T": -1, "W": -4, "V": -2, "Y": -3, "X": -2, "Z": -1 }, "S": { "*": -4, "A": 1, "C": -1, "B": 0, "E": 0, "D": 0, "G": 0, "F": -2, "I": -2, "H": -1, "K": 0, "M": -1, "L": -2, "N": 1, "Q": 0, "P": -1, "S": 4, "R": -1, "T": 1, "W": -3, "V": -2, "Y": -2, "X": 0, "Z": 0 }, "R": { "*": -4, "A": -1, "C": -3, "B": -1, "E": 0, "D": -2, "G": -2, "F": -3, "I": -3, "H": 0, "K": 2, "M": -1, "L": -2, "N": 0, "Q": 1, "P": -2, "S": -1, "R": 5, "T": -1, "W": -3, "V": -3, "Y": -2, "X": -1, "Z": 0 }, "T": { "*": -4, "A": 0, "C": -1, "B": -1, "E": -1, "D": -1, "G": -2, "F": -2, "I": -1, "H": -2, "K": -1, "M": -1, "L": -1, "N": 0, "Q": -1, "P": -1, "S": 1, "R": -1, "T": 5, "W": -2, "V": 0, "Y": -2, "X": 0, "Z": -1 }, "W": { "*": -4, "A": -3, "C": -2, "B": -4, "E": -3, "D": -4, "G": -2, "F": 1, "I": -3, "H": -2, "K": -3, "M": -1, "L": -2, "N": -4, "Q": -2, "P": -4, "S": -3, "R": -3, "T": -2, "W": 11, "V": -3, "Y": 2, "X": -2, "Z": -3 }, "V": { "*": -4, "A": 0, "C": -1, "B": -3, "E": -2, "D": -3, "G": -3, "F": -1, "I": 3, "H": -3, "K": -2, "M": 1, "L": 1, "N": -3, "Q": -2, "P": -2, "S": -2, "R": -3, "T": 0, "W": -3, "V": 4, "Y": -1, "X": -1, "Z": -2 }, "Y": { "*": -4, "A": -2, "C": -2, "B": -3, "E": -2, "D": -3, "G": -3, "F": 3, "I": -1, "H": 2, "K": -2, "M": -1, "L": -1, "N": -2, "Q": -1, "P": -3, "S": -2, "R": -2, "T": -2, "W": 2, "V": -1, "Y": 7, "X": -1, "Z": -2 }, "X": { "*": -4, "A": 0, "C": -2, "B": -1, "E": -1, "D": -1, "G": -1, "F": -1, "I": -1, "H": -1, "K": -1, "M": -1, "L": -1, "N": -1, "Q": -1, "P": -2, "S": 0, "R": -1, "T": 0, "W": -2, "V": -1, "Y": -1, "X": -1, "Z": -1 }, "Z": { "*": -4, "A": -1, "C": -3, "B": 1, "E": 4, "D": 1, "G": -2, "F": -3, "I": -3, "H": 0, "K": 1, "M": -1, "L": -3, "N": 0, "Q": 3, "P": -1, "S": 0, "R": 0, "T": -1, "W": -3, "V": -2, "Y": -2, "X": -1, "Z": 4 } };
            var alphabet = Object.keys(blosum62);
            bioseq.amino_acids = bioseq.makeAlphabetMap(alphabet.join(""), alphabet.indexOf("*"));

            var i, j, matrix = [];
            for (i = 0; i < alphabet.length; i++) {
                matrix[i] = [];
                for (j = 0; j < alphabet.length; j++) matrix[i][j] = blosum62[alphabet[i]][alphabet[j]];
            }
            bioseq.blosum62 = matrix;
        }

        if (A.molecules[0].xna) {
            var conv = { "A": "A", "C": "C", "T": "T", "G": "G", "DA": "A", "DC": "C", "DT": "T", "DG": "G", "U": "T", "DU": "T" };
            var Aseq = A.molecules.map(function (x) { return conv[x.name] || "*"; }).join("");
            var Bseq = B.molecules.map(function (x) { return conv[x.name] || "*"; }).join("");
            var rst = bioseq.align(Aseq, Bseq, true);
        }
        else {
            var conv = { "ALA": "A", "CYS": "C", "ASP": "D", "GLU": "E", "PHE": "F", "GLY": "G", "HIS": "H", "ILE": "I", "LYS": "K", "LEU": "L", "MET": "M", "ASN": "N", "PRO": "P", "GLN": "Q", "ARG": "R", "SER": "S", "THR": "T", "VAL": "V", "TRP": "W", "TYR": "Y" };
            var Aseq = A.molecules.map(function (x) { return conv[x.name] || "*"; }).join("");
            var Bseq = B.molecules.map(function (x) { return conv[x.name] || "*"; }).join("");
            var rst = bioseq.align(Aseq, Bseq, true, bioseq.blosum62, [12, 1], null, bioseq.amino_acids);
        }

        var fmt = bioseq.cigar2gaps(Aseq, Bseq, rst.position, rst.CIGAR);
        var a = rst.position, b = (rst.CIGAR[0] & 0xf) == 4 ? (rst.CIGAR[0] >> 4) : 0, Aarr = [], Barr = [], align = [];

        Aseq = "-".repeat(b) + fmt[0];
        Bseq = Bseq.substr(0, b) + fmt[1];
        b = 0;
        for (var i = 0; i < Aseq.length; i++) {
            if (Aseq[i] == "-") { b++; align.push(" "); continue; }
            if (Bseq[i] == "-") { a++; align.push(" "); continue; }
            if (A.molecules[a].CA && B.molecules[b].CA) {
                Aarr.push(A.molecules[a].CA);
                Barr.push(B.molecules[b].CA);
                align.push("|");
            }
            a++; b++;
        }

        var Carr = [];
        for (var c = 0; c < B.entry.chains.length; c++) Carr = Carr.concat(B.entry.chains[c].atoms);

        var data = molmil.superpose(Aarr, Barr, Carr, undefined, 2);
        if (data === undefined) return console.error("An error occurred...");

        var oASIdxs = align.map(function (x, i) { return i; }).filter(function (x) { return align[x] == "|"; }).filter(function (x, i) { return data.aligned_indices.includes(i); })
        data.optimized_alignment = align.map(function (x, i) { return oASIdxs.includes(i) ? "|" : " " }).join("");
        data.alignment = align.join("");
        data.seq1 = Aseq;
        data.seq2 = Bseq;
        data.chain1 = A;
        data.chain2 = B;

        ID = A.CID + ":" + B.CID;

        molmil.alignInfo[ID] = data;

        if (!options.skipOrient) molmil.orient(Aarr, A.entry.soup);
        A.entry.soup.renderer.rebuildRequired = true;
        molmil.geometry.reInitChains = true;
    }
}