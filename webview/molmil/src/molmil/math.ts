namespace molmil {

    export function normalFromMat3(a, out) {
        var a00 = a[0], a01 = a[1], a02 = a[2], a03 = 0,
          a10 = a[3], a11 = a[4], a12 = a[5], a13 = 0,
          a20 = a[6], a21 = a[7], a22 = a[8], a23 = 0,
          a30 = 0, a31 = 0, a32 = 0, a33 = 1,
          b00 = a00 * a11 - a01 * a10,
          b01 = a00 * a12 - a02 * a10,
          b02 = a00 * a13 - a03 * a10,
          b03 = a01 * a12 - a02 * a11,
          b04 = a01 * a13 - a03 * a11,
          b05 = a02 * a13 - a03 * a12,
          b06 = a20 * a31 - a21 * a30,
          b07 = a20 * a32 - a22 * a30,
          b08 = a20 * a33 - a23 * a30,
          b09 = a21 * a32 - a22 * a31,
          b10 = a21 * a33 - a23 * a31,
          b11 = a22 * a33 - a23 * a32,
          // Calculate the determinant
          det = b00 * b11 - b01 * b10 + b02 * b09 + b03 * b08 - b04 * b07 + b05 * b06;
        if (!det) {
          return null;
        }
        det = 1.0 / det;
        out[0] = (a11 * b11 - a12 * b10 + a13 * b09) * det;
        out[1] = (a12 * b08 - a10 * b11 - a13 * b07) * det;
        out[2] = (a10 * b10 - a11 * b08 + a13 * b06) * det;
        out[3] = (a02 * b10 - a01 * b11 - a03 * b09) * det;
        out[4] = (a00 * b11 - a02 * b08 + a03 * b07) * det;
        out[5] = (a01 * b08 - a00 * b10 - a03 * b06) * det;
        out[6] = (a31 * b05 - a32 * b04 + a33 * b03) * det;
        out[7] = (a32 * b02 - a30 * b05 - a33 * b01) * det;
        out[8] = (a30 * b04 - a31 * b02 + a33 * b00) * det;
        return out;
      };

    // ** distance/angle/torsion calculation **

    export function getAtomXYZ(atom, soup) {
        var modelId = soup.renderer.modelId;
        return [atom.chain.modelsXYZ[modelId][atom.xyz], atom.chain.modelsXYZ[modelId][atom.xyz + 1], atom.chain.modelsXYZ[modelId][atom.xyz + 2]];
    }

    export function calcMMDistance(a1, a2, soup) {
        var modelId = soup.renderer.modelId, xyz1, xyz2;
        xyz1 = [a1.chain.modelsXYZ[modelId][a1.xyz], a1.chain.modelsXYZ[modelId][a1.xyz + 1], a1.chain.modelsXYZ[modelId][a1.xyz + 2]];
        xyz2 = [a2.chain.modelsXYZ[modelId][a2.xyz], a2.chain.modelsXYZ[modelId][a2.xyz + 1], a2.chain.modelsXYZ[modelId][a2.xyz + 2]];

        try { return vec3.distance(xyz1, xyz2); }
        catch (e) { return NaN; }
    }

    export function calcAngle(a1, a2, a3) {
        var r2d = 180. / Math.PI, v1 = [0, 0, 0], v2 = [0, 0, 0];
        try {
            vec3.subtract(v1, a1, a2); vec3.normalize(v1, v1);
            vec3.subtract(v2, a3, a2); vec3.normalize(v2, v2);
            return Math.acos(vec3.dot(v1, v2)) * r2d;
        }
        catch (e) { return NaN; }
    }

    export function calcTorsion(a1, a2, a3, a4) {
        var r2d = 180. / Math.PI, b0 = [0, 0, 0], b1 = [0, 0, 0], b2 = [0, 0, 0], v = [0, 0, 0], w = [0, 0, 0], x, y, tmp = [0, 0, 0];

        try {
            vec3.subtract(b0, a2, a1); vec3.negate(b0, b0);
            vec3.subtract(b1, a3, a2); vec3.normalize(b1, b1);
            vec3.subtract(b2, a4, a3);

            vec3.subtract(v, b0, vec3.scale(tmp, b1, vec3.dot(b0, b1)));
            vec3.subtract(w, b2, vec3.scale(tmp, b1, vec3.dot(b2, b1)));
            x = vec3.dot(v, w);
            y = vec3.dot(vec3.cross(tmp, b1, v), w);
            return Math.atan2(y, x) * r2d;
        }
        catch (e) { return NaN; }
    }

    export function calcMMAngle(a1, a2, a3, soup) {
        var modelId = soup.renderer.modelId, xyz1, xyz2, xyz3;
        xyz1 = [a1.chain.modelsXYZ[modelId][a1.xyz], a1.chain.modelsXYZ[modelId][a1.xyz + 1], a1.chain.modelsXYZ[modelId][a1.xyz + 2]];
        xyz2 = [a2.chain.modelsXYZ[modelId][a2.xyz], a2.chain.modelsXYZ[modelId][a2.xyz + 1], a2.chain.modelsXYZ[modelId][a2.xyz + 2]];
        xyz3 = [a3.chain.modelsXYZ[modelId][a3.xyz], a3.chain.modelsXYZ[modelId][a3.xyz + 1], a3.chain.modelsXYZ[modelId][a3.xyz + 2]];

        return molmil.calcAngle(xyz1, xyz2, xyz3);
    }

    export function calcMMTorsion(a1, a2, a3, a4, soup) {
        var modelId = soup.renderer.modelId, xyz1, xyz2, xyz3, xyz4;
        xyz1 = [a1.chain.modelsXYZ[modelId][a1.xyz], a1.chain.modelsXYZ[modelId][a1.xyz + 1], a1.chain.modelsXYZ[modelId][a1.xyz + 2]];
        xyz2 = [a2.chain.modelsXYZ[modelId][a2.xyz], a2.chain.modelsXYZ[modelId][a2.xyz + 1], a2.chain.modelsXYZ[modelId][a2.xyz + 2]];
        xyz3 = [a3.chain.modelsXYZ[modelId][a3.xyz], a3.chain.modelsXYZ[modelId][a3.xyz + 1], a3.chain.modelsXYZ[modelId][a3.xyz + 2]];
        xyz4 = [a4.chain.modelsXYZ[modelId][a4.xyz], a4.chain.modelsXYZ[modelId][a4.xyz + 1], a4.chain.modelsXYZ[modelId][a4.xyz + 2]];

        return molmil.calcTorsion(xyz1, xyz2, xyz3, xyz4);
    }

    export function calculateBBTorsions(chain, soup) {
        // calculate the phi/psi angles for the given chain...
        // phi: C_-N-CA-C
        // psi: N-CA-C-N^
        if (chain.molecules.length < 2) return;

        for (var m = 1; m < chain.molecules.length - 1; m++) {
            chain.molecules[m].phiAngle = molmil.calcMMTorsion(chain.molecules[m - 1].C, chain.molecules[m].N, chain.molecules[m].CA, chain.molecules[m].C, soup);
            chain.molecules[m].psiAngle = molmil.calcMMTorsion(chain.molecules[m].N, chain.molecules[m].CA, chain.molecules[m].C, chain.molecules[m + 1].N, soup);
            chain.molecules[m].omegaAngle = molmil.calcMMTorsion(chain.molecules[m - 1].CA, chain.molecules[m - 1].C, chain.molecules[m].N, chain.molecules[m].CA, soup);
        }

        chain.molecules[0].psiAngle = molmil.calcMMTorsion(chain.molecules[0].N, chain.molecules[0].CA, chain.molecules[0].C, chain.molecules[1].N, soup);
        chain.molecules[m].phiAngle = molmil.calcMMTorsion(chain.molecules[m - 1].C, chain.molecules[m].N, chain.molecules[m].CA, chain.molecules[m].C, soup);
        chain.molecules[m].omegaAngle = molmil.calcMMTorsion(chain.molecules[m - 1].CA, chain.molecules[m - 1].C, chain.molecules[m].N, chain.molecules[m].CA, soup);

        chain.molecules[0].phiAngle = chain.molecules[1].phiAngle;
        chain.molecules[m].psiAngle = chain.molecules[m - 1].psiAngle;
        chain.molecules[0].omegaAngle = chain.molecules[1].omegaAngle;
    };

    // ** used by efsite to synchronize canvases (camera orientation) **
    export function linkCanvases(canvases) {
        for (var i = 1; i < canvases.length; i++) canvases[i].renderer.camera = canvases[0].renderer.camera;
        for (var i = 0; i < canvases.length; i++) canvases[i].molmilViewer.canvases = canvases;
    }

    export function __webglNotSupported__(canvas) {
        if (window["webglNotSupported"]) return webglNotSupported(canvas);
        var div = molmil_dep.dcE("DIV");
        div.innerHTML = "Your browser does not seem to support WebGL. Please visit the <a target=\"blank\" class=\"external\" href=\"http://get.webgl.org/\">WebGL website</a> for more info on how to gain WebGL support on your browser.<br />";
        div.style.border = "1px solid #ddd";
        div.style.margin = div.style.padding = ".25em";
        canvas.parentNode.replaceChild(div, canvas);
        return div;
    };

    // ** RMSD calculation betwen two arrays of atoms **
    export function calcRMSD(atoms1, atoms2, transform) { // use w_k ???
        return molmil.loadPlugin(molmil.settings.src + "plugins/md-anal.js", this.calcRMSD, this, [atoms1, atoms2, transform]);
    }

    // end

    export function arrayMin(arr) {
        return arr.reduce(function (p, v) {
            return (p < v ? p : v);
        });
    }

    export function arrayMax(arr) {
        return arr.reduce(function (p, v) {
            return (p > v ? p : v);
        });
    }
}