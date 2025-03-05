namespace molmil {

    export class viewer {

        canvas;
        renderer;
        defaultCanvas;
        onAtomPick;
        downloadInProgress;

        SID;

        // ** main molmil object **

        constructor(canvas) {
            this.canvas = canvas;
            this.renderer = new molmil.render(this);
            this.defaultCanvas = [canvas, this.renderer];
            this.onAtomPick = function () { };
            this.downloadInProgress = 0;

            this.clear();
        };

        toString() { return " SOUP"; };

        reloadSettings() {
            this.renderer.reloadSettings();
        };

        clear() {
            this.structureFiles = [];
            this.trajectory = [];
            this.structures = [];
            this.chains = [];

            this.files = [];
            this.infoBag = {};

            this.bumats = [];
            this.poly_asym_ids = [];
            this.BUcache = {};
            this.BUmode = false;
            this.avgX = 0.0;
            this.avgY = 0.0;
            this.avgZ = 0.0;
            this.stdX = 0.0;
            this.stdY = 0.0;
            this.stdZ = 0.0;
            this.avgXYZ = [];
            this.stdXYZ = [];
            this.COR = [0, 0, 0];
            this.geomRanges = [0, 0, 0, 0, 0, 0];
            this.maxRange = 0;
            this.AisB = true;

            this.texturedBillBoards = [];

            //this.polygonData = [];
            this.atomRef = {};
            this.AID = 1;
            this.CID = 1;
            this.MID = 1;
            this.SID = 1;

            this.BU = false;

            this.atomSelection = [];

            this.showWaters = false;
            this.showHydrogens = false;

            this.sceneBU = null;

            this.canvases = [];

            this.SCstuff = false;

            if (this.canvas) {
                this.canvas.atomCORset = false;
                this.canvases = [this.canvas];
            }
            this.renderer.clear();

            this.renderer.camera.z_set = false;

            this.extraREST = {};
            this.extraRESTHeaders = {};
        };


        gotoMol(mol) {
            // for residue:
            // along N-CA-C axis
            // then zoom out 10A?

            var one = mol.N;
            var two = mol.CA;
            var three = mol.C;

            if (!(mol.N && mol.CA && mol.C)) {

                var xyz = mol.chain.modelsXYZ[this.renderer.modelId], x, y, z, COG = [0, 0, 0];

                for (var i = 0; i < mol.atoms.length; i++) {
                    x = xyz[mol.atoms[i].xyz]; y = xyz[mol.atoms[i].xyz + 1]; z = xyz[mol.atoms[i].xyz + 2];
                    COG[0] += x;
                    COG[1] += y;
                    COG[2] += z;
                }
                COG[0] /= mol.atoms.length;
                COG[1] /= mol.atoms.length;
                COG[2] /= mol.atoms.length;

                var nearest = [1e99, -1], r;
                for (var i = 0; i < mol.atoms.length; i++) {
                    x = xyz[mol.atoms[i].xyz]; y = xyz[mol.atoms[i].xyz + 1]; z = xyz[mol.atoms[i].xyz + 2];

                    r = Math.pow(COG[0] - x, 2) + Math.pow(COG[1] - y, 2) + Math.pow(COG[2] - z, 2);
                    if (r < nearest[0]) { nearest[0] = r; nearest[1] = i }
                }

                two = mol.atoms[nearest[1]];

                var x2, y2, z2, farthest = [-1, -1, -1];
                for (var i = 0; i < mol.atoms.length; i++) {
                    x = xyz[mol.atoms[i].xyz]; y = xyz[mol.atoms[i].xyz + 1]; z = xyz[mol.atoms[i].xyz + 2];
                    for (var j = i + 1; j < mol.atoms.length; j++) {
                        x2 = xyz[mol.atoms[j].xyz]; y2 = xyz[mol.atoms[j].xyz + 1]; z2 = xyz[mol.atoms[j].xyz + 2];

                        r = Math.pow(x - x2, 2) + Math.pow(y - y2, 2) + Math.pow(z - z2, 2);
                        if (r > farthest[0]) {
                            farthest[0] = r;
                            farthest[1] = i;
                            farthest[2] = j;
                        }
                    }
                }

                one = mol.atoms[farthest[1]];
                three = mol.atoms[farthest[2]];
            }

            this.renderer.camera.reset();
            if (this.canvas.atomCORset) this.resetCOR();

            if (!one || !three || one == two || two == three) {
                var x = xyz[two.xyz]; var y = xyz[two.xyz + 1]; var z = xyz[two.xyz + 2];

                this.renderer.camera.x = -x + this.COR[0];
                this.renderer.camera.y = -y + this.COR[1];
                this.renderer.camera.z = -z - molmil.configBox.zNear + this.COR[2] - 2;

                return;
            }

            // norm(CA - ((C+N)*.5))
            var xyz1, xyz2, xyz3, vec = [0, 0, 0];
            var xyz1 = [mol.chain.modelsXYZ[this.renderer.modelId][one.xyz], mol.chain.modelsXYZ[this.renderer.modelId][one.xyz + 1], mol.chain.modelsXYZ[this.renderer.modelId][one.xyz + 2]];
            var xyz2 = [mol.chain.modelsXYZ[this.renderer.modelId][three.xyz], mol.chain.modelsXYZ[this.renderer.modelId][three.xyz + 1], mol.chain.modelsXYZ[this.renderer.modelId][three.xyz + 2]];
            var xyz3 = [mol.chain.modelsXYZ[this.renderer.modelId][two.xyz], mol.chain.modelsXYZ[this.renderer.modelId][two.xyz + 1], mol.chain.modelsXYZ[this.renderer.modelId][two.xyz + 2]];

            xyz1[0] -= this.avgXYZ[0]; xyz1[1] -= this.avgXYZ[1]; xyz1[2] -= this.avgXYZ[2];
            xyz2[0] -= this.avgXYZ[0]; xyz2[1] -= this.avgXYZ[1]; xyz2[2] -= this.avgXYZ[2];
            xyz3[0] -= this.avgXYZ[0]; xyz3[1] -= this.avgXYZ[1]; xyz3[2] -= this.avgXYZ[2];

            vec[0] = xyz3[0] - ((xyz1[0] + xyz2[0]) * .5);
            vec[1] = xyz3[1] - ((xyz1[1] + xyz2[1]) * .5);
            vec[2] = xyz3[2] - ((xyz1[2] + xyz2[2]) * .5);
            vec3.normalize(vec, vec);

            var A = [xyz1[0] - xyz3[0], xyz1[1] - xyz3[1], xyz1[2] - xyz3[2]]; vec3.normalize(A, A);
            var B = [xyz2[0] - xyz3[0], xyz2[1] - xyz3[1], xyz2[2] - xyz3[2]]; vec3.normalize(B, B);
            var C = vec3.cross([0, 0, 0], A, B); vec3.normalize(C, C);

            var eye = [vec[0] * 5 - xyz3[0], vec[1] * 5 - xyz3[1], vec[2] * 5 - xyz3[2]];
            var s = vec3.cross([0, 0, 0], vec, C); vec3.normalize(s, s);
            var u = vec3.cross([0, 0, 0], s, vec);

            var matrix = mat4.create();
            matrix[0] = s[0]; matrix[4] = s[1]; matrix[8] = s[2];
            matrix[1] = u[0]; matrix[5] = u[1]; matrix[9] = u[2];
            matrix[2] = -vec[0]; matrix[6] = -vec[1]; matrix[10] = -vec[2];
            matrix[12] = -vec3.dot(s, eye); matrix[13] = -vec3.dot(u, eye); matrix[14] = -vec3.dot(vec, eye);

            this.renderer.camera.x = -matrix[12];
            this.renderer.camera.y = -matrix[13];
            this.renderer.camera.z = matrix[14] - molmil.configBox.zNear;

            quat.fromMat3(this.renderer.camera.QView, mat3.fromMat4(mat3.create(), matrix));
            quat.normalize(this.renderer.camera.QView, this.renderer.camera.QView);

            this.canvas.update = true;
        };

        waterToggle(show) {
            for (var m = 0, c, a; m < this.structures.length; m++) {
                if (!this.structures[m].chains) continue;
                for (c = 0; c < this.structures[m].chains.length; c++) {
                    if (this.structures[m].chains[c].molecules.length && this.structures[m].chains[c].molecules[0].water) this.structures[m].chains[c].display = show;
                }
            }
            this.showWaters = show;
        };

        hydrogenToggle(show) {
            for (var m = 0, c, a; m < this.structures.length; m++) {
                if (!this.structures[m].chains) continue;
                for (c = 0; c < this.structures[m].chains.length; c++) {
                    for (a = 0; a < this.structures[m].chains[c].atoms.length; a++) {
                        if (this.structures[m].chains[c].atoms[a].element == "H" || this.structures[m].chains[c].atoms[a].element == "D") this.structures[m].chains[c].atoms[a].display = show;
                    }
                }
            }

            this.showHydrogens = show;
        };

        restoreDefaultCanvas(canvas) {
            this.canvas = this.defaultCanvas[0];
            this.renderer = this.defaultCanvas[1];
        }

        // ** this function is executed when a user clicks on something on the screen **
        selectObject(x, y, event) {
            y = this.canvas.height - y;
            var gl = this.renderer.gl;
            if (!this.renderer.FBOs.pickingBuffer) {
                this.renderer.FBOs.pickingBuffer = new molmil.FBO(gl, this.renderer.width, this.renderer.height);
                this.renderer.FBOs.pickingBuffer.addTexture("colourBuffer", gl.RGBA, gl.RGBA);
                this.renderer.FBOs.pickingBuffer.setup();
            }
            else if (this.renderer.FBOs.pickingBuffer.width != this.renderer.width || this.renderer.FBOs.pickingBuffer.height != this.renderer.height) this.renderer.FBOs.pickingBuffer.resize(this.renderer.width, this.renderer.height);

            this.renderer.FBOs.pickingBuffer.bind();
            this.renderer.renderPicking();
            var data = new Uint8Array(4);
            gl.readPixels(x, y, 1, 1, gl.RGBA, gl.UNSIGNED_BYTE, data);
            var ID = Math.round(vec4.dot([data[0] / 255., data[1] / 255., data[2] / 255., data[3] / 255.], [1.0 / (255.0 * 255.0 * 255.0), 1.0 / (255.0 * 255.0), 1.0 / 255.0, 1.0]) * 4228250625.); //4228250625
            this.renderer.FBOs.pickingBuffer.unbind();
            if (ID != 0) {
                var atom = this.atomRef[ID];
                if (atom) {
                    var cbox = this.canvas.commandLine ? this.canvas.commandLine.environment.console : console;
                    cbox.log("Clicked on atom: " + atom); console.log("Clicked on atom: ", atom);
                    if (event && event.ctrlKey) {
                        var add = true;
                        for (var i = 0; i < this.atomSelection.length; i++) if (this.atomSelection[i] == atom) { add = false; break; }
                        if (add) this.atomSelection.push(atom);
                        if (this.atomSelection.length == 2) {
                            cbox.log("Distance: ", molmil.calcMMDistance(this.atomSelection[0], this.atomSelection[1], this), " | ", this.atomSelection[0], this.atomSelection[1]);
                        }
                        else if (this.atomSelection.length == 3) {
                            cbox.log("Angle: ", molmil.calcMMAngle(this.atomSelection[0], this.atomSelection[1], this.atomSelection[2], this), " | ", this.atomSelection[0], this.atomSelection[1], this.atomSelection[2]);
                        }
                        else if (this.atomSelection.length == 4) {
                            cbox.log("Torsion: ", molmil.calcMMTorsion(this.atomSelection[0], this.atomSelection[1], this.atomSelection[2], this.atomSelection[3], this), " | ", this.atomSelection[0], this.atomSelection[1], this.atomSelection[2], this.atomSelection[3]);
                        }
                    }
                    else this.atomSelection = [atom];
                    this.onAtomPick(atom);
                    this.canvas.renderer.updateSelection();
                }
            }
            else {
                this.atomSelection = [];
                this.canvas.renderer.updateSelection();
            }
            this.canvas.update = true;
        };

        // ** loads a file from a URL **
        loadStructure(loc, format, ondone, settings) { // ignore format here...
            if (this.hasOwnProperty("__cwd__")) {
                var r = new RegExp('^(?:[a-z]+:)?//', 'i');
                if (!r.test(loc) && !loc.startsWith(this.__cwd__)) loc = this.__cwd__ + loc;
            }
            else if (molmil.hasOwnProperty("__cwd__")) {
                var r = new RegExp('^(?:[a-z]+:)?//', 'i');
                if (!r.test(loc) && !loc.startsWith(molmil.__cwd__)) loc = molmil.__cwd__ + loc;
            }

            if (!format) format = molmil.guess_format(loc);

            if (format == "mjs") return molmil.loadScript(loc);

            var preloadLoadersJS = [molmil.settings.src + "plugins/loaders.js"];

            this.downloadInProgress++;

            settings = settings || {};
            if (settings.bakadl) {
                delete settings.bakadl;
                this.downloadInProgress--;
            }
            var gz = loc.substr(-3).toLowerCase() == ".gz" && !settings.no_pako_gz;
            if (settings.gzipped == true || settings.gzipped == 1) gz = true;
            if (gz && !window.hasOwnProperty("pako")) {
                settings.bakadl = true;
                var head = document.getElementsByTagName("head")[0];
                var obj = molmil_dep.dcE("script"); obj.src = molmil.settings.src + "lib/pako.js";
                obj.soup = this; obj.argList = [loc, format, ondone, settings]; obj.onload = function () { molmil_dep.asyncStart(this.soup.loadStructure, this.argList, this.soup, 0); };
                head.appendChild(obj);
                return;
            }

            var request = molmil.xhr(loc); //loc = request.URL;

            var async = molmil_dep.getKeyFromObject(settings || {}, "async", true); request.ASYNC = async; request.target = this;
            request.gz = gz;
            if (request.gz) request.responseType = "arraybuffer";
            if (this.onloaderror) request.OnError = this.onloaderror;
            request.filename = settings.object || loc.substr(loc.lastIndexOf("/") + 1);

            if (format == 1 || (format + "").toLowerCase() == "mmjson") {
                preloadLoadersJS = [];
                var loc_ = loc;
                if (async && !request.gz) request.responseType = "json"; // add gzip support...
                request.parse = function () {
                    if (this.gz) var jso = JSON.parse(pako.inflate(new Uint8Array(this.request.response), { to: "string" }));
                    else var jso = request.request.response;
                    if (typeof jso != "object" && jso != null) jso = JSON.parse(this.request.responseText);
                    else if (jso == null) { throw ""; }
                    return this.target.load_PDBx(jso, this.pdbid, settings);
                };
            }
            else if (format == 2 || (format + "").toLowerCase() == "mmcif") {
                settings.bakadl = true;
                if (!window.CIFparser) return molmil.loadPlugin(molmil.settings.src + "lib/cif.js", this.loadStructure, this, [loc, format, ondone, settings]);
                preloadLoadersJS = [];
                request.parse = function () {
                    return this.target.load_mmCIF(this.gz ? pako.inflate(new Uint8Array(this.request.response), { to: "string" }) : this.request.responseText, settings);
                };
            }
            else if (format == 3 || (format + "").toLowerCase() == "pdbml") {
                settings.bakadl = true;
                if (!window.loadPDBML) return molmil.loadPlugin(molmil.settings.src + "lib/cif.js", this.loadStructure, this, [loc, format, ondone, settings]);
                preloadLoadersJS = [];
                request.parse = function () {
                    return this.target.load_PDBML(this.gz ? pako.inflate(new Uint8Array(this.request.response), { to: "string" }) : this.request.responseXML, settings);
                };
            }
            else if (format == 4 || (format + "").toLowerCase() == "pdb") {
                request.parse = function () {
                    return this.target.load_PDB(this.gz ? pako.inflate(new Uint8Array(this.request.response), { to: "string" }) : this.request.responseText, this.filename);
                };
            }
            else if ((format + "").toLowerCase() == "mmtf") {
                settings.bakadl = true;
                if (!window.MMTF) return molmil.loadPlugin(molmil.settings.src + "lib/mmtf.js", this.loadStructure, this, [loc, format, ondone, settings]);
                request.ASYNC = true; request.responseType = "arraybuffer";
                request.parse = function () {
                    return this.target.load_MMTF(this.gz ? pako.inflate(new Uint8Array(this.request.response)) : this.request.response, this.filename, settings);
                };
            }
            else if (format == 5 || (format + "").toLowerCase() == "polygon-xml") {
                request.parse = function () {
                    return this.target.load_polygonXML(this.gz ? pako.inflate(new Uint8Array(this.request.response), { to: "string" }) : this.request.responseXML || this.request.responseText, this.filename, settings);
                };
            }
            else if (format == 6 || (format + "").toLowerCase() == "polygon-json") {
                request.ASYNC = true; request.responseType = "json";
                request.parse = function () {
                    var jso = this.gz ? JSON.parse(pako.inflate(new Uint8Array(this.request.response), { to: "string" })) : request.request.response;
                    if (typeof jso != "object" && jso != null) jso = JSON.parse(this.request.responseText);
                    return this.target.load_polygonJSON(jso, this.filename);
                };
            }
            else if (format == 7 || (format + "").toLowerCase() == "gro") {
                request.parse = function () {
                    return this.target.load_GRO(this.gz ? pako.inflate(new Uint8Array(this.request.response), { to: "string" }) : this.request.responseText, this.filename);
                };
            }
            else if (format == 8 || (format + "").toLowerCase() == "mpbf") {
                request.ASYNC = true; request.responseType = "arraybuffer";
                request.parse = function () {
                    return this.target.load_MPBF(this.gz ? pako.inflate(new Uint8Array(this.request.response)).buffer : this.request.response, this.filename, settings);
                };
            }
            else if ((format + "").toLowerCase() == "ccp4") {
                preloadLoadersJS.push(molmil.settings.src + "plugins/misc.js");
                request.ASYNC = true; request.responseType = "arraybuffer";
                request.parse = function () {
                    return this.target.load_ccp4(this.gz ? pako.inflate(new Uint8Array(this.request.response)) : this.request.response, this.filename, settings);
                };
            }
            else if ((format + "").toLowerCase() == "obj") {
                request.parse = function () {
                    return this.target.load_obj(this.gz ? pako.inflate(new Uint8Array(this.request.response), { to: "string" }) : this.request.responseText, this.filename, settings);
                };
            }
            else if ((format + "").toLowerCase() == "wrl") {
                preloadLoadersJS.push(molmil.settings.src + "plugins/vrml.js");
                preloadLoadersJS.push(molmil.settings.src + "lib/vrml.min.js");
                request.parse = function () {
                    return this.target.load_wrl(this.gz ? pako.inflate(new Uint8Array(this.request.response), { to: "string" }) : this.request.responseText, this.filename, settings);
                };
            }
            else if ((format + "").toLowerCase() == "stl") {
                request.ASYNC = true; request.responseType = "arraybuffer";
                request.parse = function () {
                    return this.target.load_stl(this.gz ? pako.inflate(new Uint8Array(this.request.response)) : this.request.response, this.filename);
                };
            }
            else if ((format + "").toLowerCase() == "ply") {
                request.ASYNC = true; request.responseType = "arraybuffer";
                request.parse = function () {
                    return this.target.load_ply(this.gz ? pako.inflate(new Uint8Array(this.request.response)) : this.request.response, this.filename);
                };
            }
            else if ((format + "").toLowerCase() == "mdl") {
                request.parse = function () {
                    return this.target.load_mdl(this.gz ? pako.inflate(new Uint8Array(this.request.response), { to: "string" }) : this.request.responseText, this.filename);
                };
            }
            else if ((format + "").toLowerCase() == "mol2") {
                request.parse = function () {
                    return this.target.load_mol2(this.gz ? pako.inflate(new Uint8Array(this.request.response), { to: "string" }) : this.request.responseText, this.filename);
                };
            }
            else if ((format + "").toLowerCase() == "xyz") {
                request.parse = function () {
                    return this.target.load_xyz(this.gz ? pako.inflate(new Uint8Array(this.request.response), { to: "string" }) : this.request.responseText, this.filename, settings);
                };
            }
            else if ((format + "").toLowerCase() == "gromacs-trr") {
                request.ASYNC = true; request.responseType = "arraybuffer";
                request.parse = function () {
                    this.target.loadGromacsTRR(this.request.response, this.filename);
                    return this.target.structures[0];
                };
            }
            else if ((format + "").toLowerCase() == "gromacs-xtc") {
                request.ASYNC = true; request.responseType = "arraybuffer";
                request.parse = function () {
                    this.target.loadGromacsXTC(this.request.response, settings || {});
                    return null;
                };
            }
            else if ((format + "").toLowerCase() == "psygene-traj" || (format + "").toLowerCase() == "presto-traj") {
                request.ASYNC = true; request.responseType = "arraybuffer";
                request.parse = function () {
                    var buffer = this.request.response;
                    this.target.loadMyPrestoTrj(buffer, molmil_dep.getKeyFromObject(settings || {}, "fxcell", null));
                    return this.target.structures[0];
                };
            }
            else if ((format + "").toLowerCase() == "presto-mnt") {
                request.ASYNC = true; request.responseType = "arraybuffer";
                request.parse = function () {
                    var buffer = this.request.response;
                    this.target.loadMyPrestoMnt(buffer, molmil_dep.getKeyFromObject(settings || {}, "fxcell", null));
                    return this.target.structures[0];
                };
            }
            else if (typeof (format) == "function") {
                request.ASYNC = async;
                request.parseFunction = format;
                if (settings.responseType) request.responseType = settings.responseType;
                request.parse = function () {
                    if (settings.responseType == "arraybuffer" || settings.responseType == "json") return this.parseFunction.apply(this.target, [this.request.response, this.filename]);
                    else return this.parseFunction.apply(this.target, [this.request.responseText, this.filename])
                };
            }
            else {
                var fakeObj = { filename: loc, readAsText: function () { }, readAsArrayBuffer: function () { } };
                for (var j = 0; j < this.canvas.inputFunctions.length; j++) {
                    if (this.canvas.inputFunctions[j](this.canvas, fakeObj)) {
                        request.inputFunction = this.canvas.inputFunctions[j];
                        request.canvas = this.canvas;
                        request.parse = function () {
                            var fakeObj = {
                                filename: loc,
                                readAsText: function () { fakeObj.onload({ target: { result: request.request.responseText } }); },
                                readAsArrayBuffer: function () { fakeObj.onload({ target: { result: request.request.response } }); }
                            };
                            this.inputFunction(this.canvas, fakeObj);
                        };
                        break;
                    }
                }
                if (request.inputFunction === undefined) { console.log("Unknown format: " + format); return; }
            }

            request.ondone = ondone;
            request.OnDone = function () {
                this.target.downloadInProgress--;

                var structures = this.parse();
                if (!structures) {
                    if (settings.alwaysCallONDONE) ondone();
                    return;
                }
                if (!(structures instanceof Array)) structures = [structures];
                if (structures.length == 0) {
                    if (settings.alwaysCallONDONE) ondone();
                    return;
                }

                var id = this.target.SID++;
                if (structures.length == 1) structures[0].meta.idnr = "#" + id;
                else {
                    for (var s = 0; s < structures.length; s++) {
                        if (structures[s].meta.modelnr) structures[s].meta.idnr = "#" + id + "." + structures[s].meta.modelnr;
                        else { id = this.target.SID++; structures[s].meta.idnr = "#" + id; }
                    }
                }
                molmil.safeStartViewer(this.target.canvas);
                if (ondone) ondone(this.target, structures.length == 1 ? structures[0] : structures);
                else {
                    molmil.displayEntry(structures, 1);
                    molmil.colorEntry(structures, 1, null, true, this.target);
                }
                if (this.target.UI) this.target.UI.resetRM();
            };
            if (preloadLoadersJS.length) {
                for (var i = 0; i < preloadLoadersJS.length; i++) molmil.loadPlugin(preloadLoadersJS[i], null, null, null, request.async);
            }

            for (var e in this.extraREST) request.AddParameter(e, this.extraREST[e]);
            for (var e in this.extraRESTHeaders) request.headers[e] = this.extraRESTHeaders[e];
            request.Send();
        };

        loadGromacsXTC(buffer, settings) {
            return molmil.loadPlugin(molmil.settings.src + "plugins/md-anal.js", this.loadGromacsXTC, this, [buffer, settings]);
        }

        loadGromacsTRR(buffer) {
            return molmil.loadPlugin(molmil.settings.src + "plugins/md-anal.js", this.loadGromacsTRR, this, [buffer]);
        }

        loadMyPrestoMnt(buffer, fxcell) {
            return molmil.loadPlugin(molmil.settings.src + "plugins/md-anal.js", this.loadMyPrestoMnt, this, [buffer, fxcell]);
        }

        loadMyPrestoTrj(buffer, fxcell) {
            return molmil.loadPlugin(molmil.settings.src + "plugins/md-anal.js", this.loadMyPrestoTrj, this, [buffer, fxcell]);
        }

        // ** loads arbitrary data **
        loadStructureData(data, format, filename, ondone, settings) {
            if (!format) format = molmil.guess_format(filename);
            var structures;
            if (format == 1 || (format + "").toLowerCase() == "mmjson") structures = this.load_PDBx(typeof data == "string" ? JSON.parse(data) : data, settings);
            else if (format == 2 || (format + "").toLowerCase() == "cif") {
                if (!window.CIFparser) return molmil.loadPlugin(molmil.settings.src + "lib/cif.js", this.loadStructureData, this, [data, format, filename, ondone, settings]);
                structures = this.load_mmCIF(data, settings);
            }
            else if (format == 3 || (format + "").toLowerCase() == "pdbml") {
                if (!window.loadPDBML) return molmil.loadPlugin(molmil.settings.src + "lib/cif.js", this.loadStructureData, this, [data, format, filename, ondone, settings]);
                structures = this.load_PDBML(data, settings);
            }
            else if (format == 4 || (format + "").toLowerCase() == "pdb") structures = this.load_PDB(data, filename);
            else if ((format + "").toLowerCase() == "mmtf") {
                if (!window.MMTF) return molmil.loadPlugin(molmil.settings.src + "lib/mmtf.js", this.loadStructureData, this, [data, format, filename, ondone, settings]);
                structures = this.load_MMTF(data, filename);
            }
            else if (format == 5 || (format + "").toLowerCase() == "polygon-xml") structures = this.load_polygonXML(data, filename, settings);
            else if (format == 6 || (format + "").toLowerCase() == "polygon-json") structures = this.load_polygonJSON(typeof data == "object" ? data : JSON.parse(data), filename);
            else if (format == 7 || (format + "").toLowerCase() == "gro") structures = this.load_GRO(data, filename);
            else if (format == 8 || (format + "").toLowerCase() == "mpbf") structures = this.load_MPBF(data, filename, settings);
            else if ((format + "").toLowerCase() == "ply") structures = this.load_ply(data, filename, settings);
            else if ((format + "").toLowerCase() == "mdl") structures = this.load_mdl(data, filename);
            else if ((format + "").toLowerCase() == "mol2") structures = this.load_mol2(data, filename);
            else if ((format + "").toLowerCase() == "xyz") structures = this.load_xyz(data, filename, settings);
            else if ((format + "").toLowerCase() == "ccp4") structures = this.load_ccp4(data, filename, settings);
            else if ((format + "").toLowerCase() == "stl") structures = this.load_stl(data, filename, settings);
            else if ((format + "").toLowerCase() == "obj") structures = this.load_obj(data, filename, settings);
            else if ((format + "").toLowerCase() == "wrl") {
                if (!this.load_wrl) return molmil.loadPlugin(molmil.settings.src + "plugins/vrml.js", this.loadStructureData, this, [data, format, filename, ondone, settings], true);
                if (!window.vrmlParser) return molmil.loadPlugin(molmil.settings.src + "lib/vrml.min.js", this.loadStructureData, this, [data, format, filename, ondone, settings], true);
                structures = this.load_wrl(data, filename, settings);
            }
            else if ((format + "").toLowerCase() == "psygene-traj" || (format + "").toLowerCase() == "presto-traj") structures = this.loadMyPrestoTrj(data, molmil_dep.getKeyFromObject(settings || {}, "fxcell", null));
            else if ((format + "").toLowerCase() == "presto-mnt") structures = this.loadMyPrestoMnt(data, molmil_dep.getKeyFromObject(settings || {}, "fxcell", null));
            else if ((format + "").toLowerCase() == "gromacs-trr") structures = this.loadGromacsTRR(data);
            else if ((format + "").toLowerCase() == "gromacs-xtc") structures = this.loadGromacsXTC(data);
            else if (format == "mjs") {
                this.__cwd__ = this.canvas.commandLine.environment.scriptUrl = filename.substr(0, filename.lastIndexOf('/')) + "/";
                this.canvas.commandLine.environment.console.runCommand(data.replace(/\/\*[\s\S]*?\*\/|([^:]|^)\/\/.*$/gm, ""));
            }
            if (!structures) return;
            if (!(structures instanceof Array)) structures = [structures];
            if (structures.length == 0) return;
            var id = this.SID++;
            if (structures.length == 1) structures[0].meta.idnr = "#" + id;
            else {
                for (var s = 0; s < structures.length; s++) {
                    if (structures[s].meta.modelnr) structures[s].meta.idnr = "#" + id + "." + structures[s].meta.modelnr;
                    else { id = this.SID++; structures[s].meta.idnr = "#" + id; }
                }
            }
            if (this.canvas) molmil.safeStartViewer(this.canvas);
            if (ondone) ondone(this, structures.length == 1 ? structures[0] : structures);
            else if (this.customLoaderFunc) this.customLoaderFunc(this, structures.length == 1 ? structures[0] : structures);
            else {
                molmil.displayEntry(structures, 1);
                molmil.colorEntry(structures, 1, null, true, this);
            }
            if (this.UI) this.UI.resetRM();
        };

        // ** connects amino bonds within a chain object **
        buildAminoChain(chain) {
            if (chain.isHet || (chain.molecules.length && (chain.molecules[0].water))) return;
            var snfg = true;
            for (var m1 = 0; m1 < chain.molecules.length; m1++) if (!chain.molecules[m1].SNFG) snfg = false;
            if (snfg) return this.buildSNFG(chain);
            if (chain.molecules.length == 1 && chain.molecules[0].xna) {
                chain.molecules[0].ligand = chain.isHet = true; chain.molecules[0].xna = false;
                delete chain.molecules[0].N;
                delete chain.molecules[0].CA;
                delete chain.molecules[0].C;
                return;
            }

            var m1, m2, xyz1, xyz2, rC, newChains, struc = chain.entry, dx, dy, dz, r, tmpArray;
            var xyzRef = chain.modelsXYZ[0];
            chain.bonds = [];

            for (m1 = 0; m1 < chain.molecules.length; m1++) {
                rC = 17;
                if (chain.molecules[m1].xna) {
                    rC = 100; tmpArray = [];
                    for (m2 = 0; m2 < chain.molecules[m1].atoms.length; m2++) tmpArray.push(chain.molecules[m1].atoms[m2].atomName);
                    if (!chain.molecules[m1].CA && tmpArray.indexOf("O5'") != -1) chain.molecules[m1].CA = chain.molecules[m1].atoms[[tmpArray.indexOf("O5'")]];
                    if (tmpArray.indexOf("O2") != -1) {
                        m2 = tmpArray.indexOf("N3");
                        if (m2 != -1) chain.molecules[m1].outer = chain.molecules[m1].atoms[m2];
                    }
                    else {
                        m2 = tmpArray.indexOf("N1");
                        if (m2 != -1) chain.molecules[m1].outer = chain.molecules[m1].atoms[m2];
                    }
                }
                for (m2 = m1 + 1; m2 < chain.molecules.length; m2++) {
                    if (chain.molecules[m1].xna != chain.molecules[m2].xna) continue;
                    if (chain.molecules[m1].C && chain.molecules[m2].N) {
                        xyz1 = chain.molecules[m1].C.xyz;
                        xyz2 = chain.molecules[m2].N.xyz;
                        dx = xyzRef[xyz1] - xyzRef[xyz2]; dx *= dx;
                        dy = xyzRef[xyz1 + 1] - xyzRef[xyz2 + 1]; dy *= dy;
                        dz = xyzRef[xyz1 + 2] - xyzRef[xyz2 + 2]; dz *= dz;
                        r = dx + dy + dz;

                        if (r <= 3.0) {
                            chain.molecules[m1].next = chain.molecules[m2];
                            chain.molecules[m2].previous = chain.molecules[m1];
                            chain.bonds.push([chain.molecules[m1].C, chain.molecules[m2].N, 1]);
                            break;
                        }
                        else if (chain.molecules[m2].C && chain.molecules[m1].N) { // for weird entries like 2n4n
                            xyz1 = chain.molecules[m2].C.xyz;
                            xyz2 = chain.molecules[m1].N.xyz;
                            dx = xyzRef[xyz1] - xyzRef[xyz2]; dx *= dx;
                            dy = xyzRef[xyz1 + 1] - xyzRef[xyz2 + 1]; dy *= dy;
                            dz = xyzRef[xyz1 + 2] - xyzRef[xyz2 + 2]; dz *= dz;
                            r = dx + dy + dz;

                            if (r <= 3.0) {
                                chain.molecules[m1].next = chain.molecules[m2];
                                chain.molecules[m2].previous = chain.molecules[m1];
                                chain.bonds.push([chain.molecules[m1].C, chain.molecules[m2].N, 1]);
                                break;
                            }
                        }
                    }
                    else if (chain.molecules[m1].CA && chain.molecules[m2].CA && m2 == m1 + 1) { // only allow sequential
                        xyz1 = chain.molecules[m1].CA.xyz;
                        xyz2 = chain.molecules[m2].CA.xyz;
                        dx = xyzRef[xyz1] - xyzRef[xyz2]; dx *= dx;
                        dy = xyzRef[xyz1 + 1] - xyzRef[xyz2 + 1]; dy *= dy;
                        dz = xyzRef[xyz1 + 2] - xyzRef[xyz2 + 2]; dz *= dz;
                        r = dx + dy + dz;

                        if (r <= rC) {
                            chain.molecules[m1].next = chain.molecules[m2];
                            chain.molecules[m2].previous = chain.molecules[m1];
                            break;
                        }
                    }
                }
                // cyclic check...
                if (chain.molecules[0].N && chain.molecules[m1].C) {
                    xyz1 = chain.molecules[0].N.xyz;
                    xyz2 = chain.molecules[m1].C.xyz;
                    dx = xyzRef[xyz1] - xyzRef[xyz2]; dx *= dx;
                    dy = xyzRef[xyz1 + 1] - xyzRef[xyz2 + 1]; dy *= dy;
                    dz = xyzRef[xyz1 + 2] - xyzRef[xyz2 + 2]; dz *= dz;
                    r = dx + dy + dz;
                    if (r <= 3.0) {
                        chain.bonds.push([chain.molecules[0].N, chain.molecules[m1].C, 1]);
                        chain.showBBatoms.push(chain.molecules[0].N);
                        chain.showBBatoms.push(chain.molecules[m1].C);
                        chain.isCyclic = true;
                    }
                }
            }
        };

        buildSNFG(chain) {
            chain.SNFG = true;
            chain.branches = [];
            if (chain.bonds.length == 0 && chain.atoms.length > 1) return this.buildBondList(chain);

            for (var i = 0; i < chain.bonds.length; i++) {
                if (chain.bonds[i][0].molecule != chain.bonds[i][1].molecule) {
                    chain.bonds[i][0].molecule.weirdAA = chain.bonds[i][1].molecule.weirdAA = false;
                    if (chain.bonds[i][0].molecule.CA) {
                        chain.bonds[i][0].molecule.snfg_con = chain.bonds[i][1].molecule;
                        chain.bonds[i][1].molecule.res_con = chain.bonds[i][0].molecule
                    }
                    else if (chain.bonds[i][1].molecule.CA) {
                        chain.bonds[i][1].molecule.snfg_con = chain.bonds[i][0].molecule;
                        chain.bonds[i][0].molecule.res_con = chain.bonds[i][1].molecule;
                    }
                    chain.branches.push([chain.bonds[i][0].molecule, chain.bonds[i][1].molecule]);
                }
            }
        };

        buildMolBondList(chain, rebuild) {
            var m1, m2, SG1, SG2;
            var dx, dy, dz, r, a1, a2, xyz1, xyz2, vdwR = molmil.configBox.vdwR;

            var x1, x2, y1, y2, z1, z2;

            if ((rebuild || chain.bonds.length == 0) && !chain.SNFG) this.buildAminoChain(chain);

            // bonds
            var xyzRef = chain.modelsXYZ[0], ligand = false;

            if (chain.struct_conn && chain.struct_conn.length) {
                var a1, a2, snfg = true;
                for (m1 = 0; m1 < chain.molecules.length; m1++) {
                    if (!chain.molecules[m1].SNFG) snfg = false;
                    molmil.buildBondsList4Molecule(chain.bonds, chain.molecules[m1], xyzRef);
                }
                for (m1 = 0; m1 < chain.struct_conn.length; m1++) {

                    chain.bonds.push(chain.struct_conn[m1]);
                    a1 = chain.struct_conn[m1][0]; a2 = chain.struct_conn[m1][1];

                    if (a1.molecule.SNFG && a2.molecule.SNFG) continue;
                    if ((a1.molecule.next != a2.molecule && a1.molecule.previous != a2.molecule) || !a1.molecule.CA || !a2.molecule.CA || a1.molecule == a2.molecule) {
                        chain.showBBatoms.push(chain.struct_conn[m1][0]);
                        chain.showBBatoms.push(chain.struct_conn[m1][1]);
                        if (chain.struct_conn[m1][0].molecule.CA) chain.showBBatoms.push(chain.struct_conn[m1][0].molecule.CA);
                        if (chain.struct_conn[m1][1].molecule.CA) chain.showBBatoms.push(chain.struct_conn[m1][1].molecule.CA);
                    }
                }
                if (snfg) this.buildSNFG(chain);
                return;
            }

            if (molmil.configBox.skipComplexBondSearch) return;

            var snfg = true;
            for (m1 = 0; m1 < chain.molecules.length; m1++) {
                if (!chain.molecules[m1].SNFG) snfg = false;
                SG1 = chain.bonds.length;

                if (chain.molecules[m1].CA) {
                    // cysteine bonds
                    if (chain.molecules[m1].name == "CYS") { // this doesn't work in lazy mode...
                        SG1 = molmil.getAtomFromMolecule(chain.molecules[m1], "SG");
                        if (!SG1) continue;
                        for (m2 = m1 + 1; m2 < chain.molecules.length; m2++) if (chain.molecules[m1].CA) {
                            if (chain.molecules[m2].name != "CYS") continue;
                            SG2 = molmil.getAtomFromMolecule(chain.molecules[m2], "SG");
                            if (!SG2) continue;
                            xyz1 = SG1.xyz; xyz2 = SG2.xyz;
                            dx = xyzRef[xyz1] - xyzRef[xyz2]; dx *= dx;
                            dy = xyzRef[xyz1 + 1] - xyzRef[xyz2 + 1]; dy *= dy;
                            dz = xyzRef[xyz1 + 2] - xyzRef[xyz2 + 2]; dz *= dz;
                            r = dx + dy + dz;
                            if (r <= 5) { chain.bonds.push([SG1, SG2, 1]); SG1.molecule.weirdAA = SG2.molecule.weirdAA = true; break; }
                        }
                    }
                }
                else if (chain.molecules[m1].ligand && !chain.molecules[m1].water && chain.molecules[m1].atoms.length > 1 && chain.molecules.length < 10) {
                    var altchain, altxyzRef;
                    for (c = 0; c < chain.entry.chains.length; c++) { // for every chain
                        altchain = chain.entry.chains[c];
                        if (altchain == chain || altchain.ligand) continue;
                        altxyzRef = altchain.modelsXYZ[0];
                        for (a1 = 0; a1 < chain.molecules[m1].atoms.length; a1++) { // for every atom in THIS chain
                            xyz1 = chain.molecules[m1].atoms[a1].xyz;
                            x1 = xyzRef[xyz1]; y1 = xyzRef[xyz1 + 1]; z1 = xyzRef[xyz1 + 2];

                            for (m2 = 0; m2 < altchain.molecules.length; m2++) { // for every residue in OTHER chains
                                if (!altchain.molecules[m2].ligand && !altchain.molecules[m2].CA) continue;
                                if (altchain.molecules[m2].water || altchain.molecules[m2].atoms.length == 1) continue;
                                if (altchain.molecules[m2].CA) {
                                    xyz2 = altchain.molecules[m2].CA.xyz;
                                    dx = x1 - altxyzRef[xyz2]; dx *= dx;
                                    dy = y1 - altxyzRef[xyz2 + 1]; dy *= dy;
                                    dz = z1 - altxyzRef[xyz2 + 2]; dz *= dz;
                                    r = dx + dy + dz;
                                    if (r > 40) continue; // very unlikely that this residue is close enough to make a covalent bond...
                                }
                                for (a2 = 0; a2 < altchain.molecules[m2].atoms.length; a2++) { // for every atom in OTHER chains
                                    xyz2 = altchain.molecules[m2].atoms[a2].xyz;
                                    dx = x1 - altxyzRef[xyz2]; dx *= dx;
                                    dy = y1 - altxyzRef[xyz2 + 1]; dy *= dy;
                                    dz = z1 - altxyzRef[xyz2 + 2]; dz *= dz;
                                    r = dx + dy + dz;
                                    if (r > 6) continue; // unlikely that it'll create a covalent bond
                                    maxDistance = molmil.configBox.connect_cutoff;
                                    maxDistance += ((vdwR[chain.molecules[m1].atoms[a1].element] || 1.8) + (vdwR[altchain.molecules[m2].atoms[a2].element] || 1.8)) * .5;
                                    if (chain.molecules[m1].atoms[a1].element == "H" || altchain.molecules[m2].atoms[a2].element == "H") maxDistance -= .2;
                                    maxDistance *= maxDistance;

                                    if (r <= maxDistance) chain.bonds.push([chain.molecules[m1].atoms[a1], altchain.molecules[m2].atoms[a2], 1]);
                                }
                            }
                        }
                    }

                    for (m2 = 0; m2 < chain.molecules.length; m2++) { // handle suger molecules...
                        if (m2 < m1 + 1 && chain.molecules[m2].ligand) continue;
                        for (a1 = 0; a1 < chain.molecules[m1].atoms.length; a1++) {
                            if (chain.molecules[m1].atoms[a1].element == "H" || chain.molecules[m1].atoms[a1].element == "D") continue
                            xyz1 = chain.molecules[m1].atoms[a1].xyz;
                            for (a2 = 0; a2 < chain.molecules[m2].atoms.length; a2++) {
                                if (chain.molecules[m2].atoms[a2].element == "H" || chain.molecules[m2].atoms[a2].element == "D") continue;
                                xyz2 = chain.molecules[m2].atoms[a2].xyz;

                                dx = xyzRef[xyz1] - xyzRef[xyz2]; dx *= dx;
                                dy = xyzRef[xyz1 + 1] - xyzRef[xyz2 + 1]; dy *= dy;
                                dz = xyzRef[xyz1 + 2] - xyzRef[xyz2 + 2]; dz *= dz;
                                r = dx + dy + dz;

                                maxDistance = molmil.configBox.connect_cutoff;
                                maxDistance += ((vdwR[chain.molecules[m1].atoms[a1].element] || 1.8) + (vdwR[chain.molecules[m2].atoms[a2].element] || 1.8)) * .5;
                                maxDistance *= maxDistance;
                                if (r <= maxDistance) chain.bonds.push([chain.molecules[m1].atoms[a1], chain.molecules[m2].atoms[a2], 1]);
                            }
                        }
                    }
                }
            }

            if (snfg) this.buildSNFG(chain);
        }


        // ** builds list of bonds within a chain object **
        buildBondList(chain, rebuild) {
            var m1;

            if ((rebuild || chain.bonds.length == 0) && !chain.SNFG) this.buildAminoChain(chain);

            // bonds
            var xyzRef = chain.modelsXYZ[0], ligand = false;
            chain.bondsOK = true;

            if (chain.struct_conn && chain.struct_conn.length) return;

            for (m1 = 0; m1 < chain.molecules.length; m1++) {
                molmil.buildBondsList4Molecule(chain.bonds, chain.molecules[m1], xyzRef);
            }
        };

        getChain(struc, cid) {
            var chain = [];
            for (var c = 0; c < struc.chains.length; c++) { if (struc.chains[c].name == cid) chain.push(struc.chains[c]); }
            return chain;
        }

        getChainAuth(struc, cid) {
            var chain = [];
            for (var c = 0; c < struc.chains.length; c++) { if (struc.chains[c].authName == cid) chain.push(struc.chains[c]); }
            return chain;
        }

        getMolObject4Chain(chain, id) {
            if (!(chain instanceof Array)) chain = [chain];
            for (var c = 0, m; c < chain.length; c++) {
                for (m = 0; m < chain[c].molecules.length; m++) {
                    if (chain[c].molecules[m].id == id) return chain[c].molecules[m];
                }
            }
            return null;
        }

        getMolObject4ChainAlt(chain, RSID) {
            if (!(chain instanceof Array)) chain = [chain];
            for (var c = 0, m; c < chain.length; c++) {
                for (m = 0; m < chain[c].molecules.length; m++) {
                    if (chain[c].molecules[m].RSID == RSID) return chain[c].molecules[m];
                }
            }
            return null;
        }


        // ** Load CCP4 data **


        load_obj(data, filename, settings) {
            return molmil.loadPlugin(molmil.settings.src + "plugins/loaders.js", this.load_obj, this, [data, filename, settings]);
        };

        load_ccp4(buffer, filename, settings) {
            //if (! molmil.conditionalPluginLoad(molmil.settings.src+"plugins/loaders.js", this.load_ccp4, this, [buffer, filename, settings])) return;
            return molmil.loadPlugin(molmil.settings.src + "plugins/loaders.js", this.load_ccp4, this, [buffer, filename, settings]);
        };

        load_stl(buffer, filename, settings) {
            return molmil.loadPlugin(molmil.settings.src + "plugins/loaders.js", this.load_stl, this, [buffer, filename, settings]);
        };


        // ** loads MPBF data **
        load_MPBF(buffer, filename, settings) {
            return molmil.loadPlugin(molmil.settings.src + "plugins/loaders.js", this.load_MPBF, this, [buffer, filename, settings]);
        }

        // ** loads XYZ data **

        load_xyz(data, filename, settings) {
            return molmil.loadPlugin(molmil.settings.src + "plugins/loaders.js", this.load_xyz, this, [data, filename, settings]);
        }

        // ** loads MOL2 data **

        load_mol2(data, filename) {
            return molmil.loadPlugin(molmil.settings.src + "plugins/loaders.js", this.load_mol2, this, [data, filename]);
        };

        // ** loads MDL MOL data **
        load_mdl(data, filename) {
            return molmil.loadPlugin(molmil.settings.src + "plugins/loaders.js", this.load_mdl, this, [data, filename]);
        };

        // ** loads GRO data **
        load_GRO(data, filename) {
            return molmil.loadPlugin(molmil.settings.src + "plugins/loaders.js", this.load_GRO, this, [data, filename]);
        };

        // ** loads PDB data **
        load_PDB(data, filename) {
            return molmil.loadPlugin(molmil.settings.src + "plugins/loaders.js", this.load_PDB, this, [data, filename]);
        };

        // ** loads MMTF data **
        load_MMTF(data, filename) {
            return molmil.loadPlugin(molmil.settings.src + "plugins/loaders.js", this.load_MMTF, this, [data, filename]);
        };
        // ** calculates the optimal zoom amount **
        calcZ(geomRanges) {
            var test = geomRanges;
            geomRanges = geomRanges || this.geomRanges;
            //var mx = Math.max(Math.abs(geomRanges[0]), Math.abs(geomRanges[1]), Math.abs(geomRanges[2]), Math.abs(geomRanges[3]), Math.abs(geomRanges[4]), Math.abs(geomRanges[5]));
            var mx = Math.max(geomRanges[1] - geomRanges[0], geomRanges[3] - geomRanges[2], geomRanges[4] - geomRanges[5]);

            if (test) this.renderer.maxRange = (Math.max(Math.abs(geomRanges[1] - geomRanges[0]), Math.abs(geomRanges[3] - geomRanges[2]), Math.abs(geomRanges[5] - geomRanges[4])) * .5) - molmil.configBox.zNear - 5;
            if (molmil.configBox.stereoMode) {
                if (molmil.configBox.stereoMode == 3) return -(mx * molmil.vrDisplay.depthFar / 4500) - molmil.vrDisplay.depthNear - 1;
                else return -(mx * molmil.configBox.zFar / 9000) - molmil.configBox.zNear - 1;
            }

            if (molmil.configBox.projectionMode == 1) {
                var zmove = ((mx / Math.sin(molmil.configBox.camera_fovy * (Math.PI / 180))) * 1.125), aspect = this.renderer.height / this.renderer.width;
                if (aspect > 1) zmove *= aspect;
                return -zmove - molmil.configBox.zNear - 2;
            }
            else if (molmil.configBox.projectionMode == 2) return -((mx / Math.min(this.renderer.width, this.renderer.height)) * molmil.configBox.zFar * (.5)) - molmil.configBox.zNear - 1;
        }

        // ** loads polygon-JSON data **
        load_polygonJSON(jso, filename, settings) { // this should be modified to use the modern renderer function instead
            return molmil.loadPlugin(molmil.settings.src + "plugins/loaders.js", this.load_polygonJSON, this, [jso, filename, settings]);
        };

        // ** loads polygon-XML data **
        load_polygonXML(xml, filename, settings) {
            return molmil.loadPlugin(molmil.settings.src + "plugins/loaders.js", this.load_polygonXML, this, [xml, filename, settings]);
        };

        // ** loads mmcif data **
        load_mmCIF(data, settings) {
            var jso = loadCIF(data);
            return this.load_PDBx(jso, settings);
        };

        // ** loads pdbml data **
        load_PDBML(xml, settings) {
            if (typeof xml == "string") {
                var parser = new DOMParser();
                xml = parser.parseFromString(xml, "text/xml");
            }
            var jso = loadPDBML(xml);
            return this.load_PDBx(jso, settings);
        };

        // ** loads PDBx formatted data such as mmcif, pdbml and mmjson **
        load_PDBx(mmjso, settings) { // this should be updated for the new model system
            var entries = Object.keys(mmjso), structs = [], offset, isHet;
            settings = settings || {};
            for (var e = 0; e < entries.length; e++) {

                //var entryId = Object.keys(mmjso)[0].substr(5).split("-")[0];
                var entryId = entries[e].substr(5).split("-")[0];
                var pdb = mmjso[entries[e]];

                var atom_site = pdb.atom_site || pdb.chem_comp_atom || pdb.pdbx_chem_comp_model_atom || pdb.
                    ihm_starting_model_coord
                    || null;

                if (!atom_site) continue;

                var isCC = !pdb.hasOwnProperty("atom_site");

                var Cartn_x = atom_site.Cartn_x || (atom_site.pdbx_model_Cartn_x_ideal && atom_site.pdbx_model_Cartn_x_ideal[0] != null ? atom_site.pdbx_model_Cartn_x_ideal : atom_site.model_Cartn_x) || atom_site.model_Cartn_x; // x
                var Cartn_y = atom_site.Cartn_y || (atom_site.pdbx_model_Cartn_y_ideal && atom_site.pdbx_model_Cartn_x_ideal[0] != null ? atom_site.pdbx_model_Cartn_y_ideal : atom_site.model_Cartn_y) || atom_site.model_Cartn_y; // y
                var Cartn_z = atom_site.Cartn_z || (atom_site.pdbx_model_Cartn_z_ideal && atom_site.pdbx_model_Cartn_x_ideal[0] != null ? atom_site.pdbx_model_Cartn_z_ideal : atom_site.model_Cartn_z) || atom_site.model_Cartn_z; // z

                var id = atom_site.id; // aid

                var auth_seq_id = atom_site.auth_seq_id || []; // residue id
                var auth_comp_id = atom_site.auth_comp_id; // residue name
                var label_seq_id = atom_site.label_seq_id || []; // residue id
                var label_comp_id = atom_site.label_comp_id || atom_site.comp_id || atom_site.model_id || []; // residue name

                var label_asym_id = atom_site.label_asym_id || atom_site.auth_asym_id || []; // chain label
                var auth_asym_id = atom_site.auth_asym_id || label_asym_id; // chain name

                var label_alt_id = atom_site.label_alt_id || [];

                var auth_atom_id = atom_site.auth_atom_id || atom_site.atom_id || []; // atom name
                var label_atom_id = atom_site.label_atom_id || []; // atom name
                var type_symbol = atom_site.type_symbol || []; // Element

                var pdbx_PDB_model_num = atom_site.pdbx_PDB_model_num;
                var group_PDB = atom_site.group_PDB || [];
                var B_iso_or_equiv = atom_site.B_iso_or_equiv || [];
                var label_entity_id = atom_site.label_entity_id || [];

                var pdbx_PDB_ins_code = atom_site.pdbx_PDB_ins_code || [];
                var currentChain = null; var ccid = false; var currentMol = null; var cmid = false; var atom;

                var struc = null, Xpos, cmnum, Xpos_first = {}, isLigand, alt_loc_handler = null;

                var polyTypes = {}, ligands = {};
                try { for (var i = 0; i < pdb.entity_poly_seq.mon_id.length; i++) polyTypes[pdb.entity_poly_seq.mon_id[i]] = false; }
                catch (e) { }
                //try {for (var i=0; i<pdb.chem_comp.id.length; i++) if (pdb.chem_comp.mon_nstd_flag[i]) polyTypes[pdb.chem_comp.id[i]] = true;}
                try {
                    for (var i = 0; i < pdb.chem_comp.id.length; i++) {
                        if (pdb.chem_comp.mon_nstd_flag[i] || pdb.chem_comp.type[i].toLowerCase().indexOf("peptide") != -1) polyTypes[pdb.chem_comp.id[i]] = true;
                        else if (pdb.chem_comp.type[i] == "non-polymer") ligands[pdb.chem_comp.id[i]] = true;
                    }
                    polyTypes.ACE = polyTypes.NME = true;
                }
                catch (e) { polyTypes = molmil.AATypes; }

                var branch_ids = {};
                if (pdb.pdbx_entity_branch) { for (var i = 0; i < pdb.pdbx_entity_branch.entity_id.length; i++) branch_ids[pdb.pdbx_entity_branch.entity_id[i]] = true; }

                var baka = 0;
                for (var a = 0; a < Cartn_x.length; a++) {
                    if (Cartn_x[a] == null) continue;
                    if (Cartn_x[a] == 0 && Cartn_y[a] == 0 && Cartn_z[a] == 0) {
                        baka++;
                        if (baka > 1) continue;
                    }

                    if ((pdbx_PDB_model_num && pdbx_PDB_model_num[a] != cmnum) || !struc) {
                        if (struc && !molmil.configBox.loadModelsSeparately) break;
                        this.structures.push(struc = new molmil.entryObject({ id: entryId })); structs.push(struc);
                        if (pdbx_PDB_model_num) struc.meta.modelnr = pdbx_PDB_model_num[a];
                        cmnum = pdbx_PDB_model_num ? pdbx_PDB_model_num[a] : 0; ccid = cmid = false;
                    }

                    if ((label_asym_id && label_asym_id[a] != ccid) || !currentChain) {
                        this.chains.push(currentChain = new molmil.chainObject(label_asym_id[a] || "", struc)); struc.chains.push(currentChain);
                        currentChain.authName = auth_asym_id[a]; // afterwards get rid of this and set chain.name to auth_asym_id[a]...
                        currentChain.labelName = label_asym_id[a];
                        currentChain.CID = this.CID++;
                        ccid = label_asym_id[a]; currentMol = null;
                        Xpos_first[currentChain] = currentChain.modelsXYZ[0].length;
                        currentChain.entity_id = label_entity_id[a];
                    }

                    if ((label_seq_id[a] || auth_seq_id[a]) != cmid || !currentMol || cmid == -1 || currentMol.name != (label_comp_id[a] || auth_comp_id[a])) {
                        currentChain.molecules.push(currentMol = new molmil.molObject((label_comp_id[a] || auth_comp_id[a]), (label_seq_id[a] || auth_seq_id[a]), currentChain));
                        currentMol.RSID = (auth_seq_id[a] || label_seq_id[a] || "") + (pdbx_PDB_ins_code[a] || "");
                        currentMol.MID = this.MID++;
                        cmid = (label_seq_id[a] || auth_seq_id[a]);
                        if ((isCC || !polyTypes.hasOwnProperty(currentMol.name)) && (currentMol.name == "HOH" || currentMol.name == "DOD" || currentMol.name == "WAT")) { currentMol.water = true; currentMol.ligand = false; }
                        if (polyTypes.hasOwnProperty(currentMol.name) && polyTypes[currentMol.name] == false) currentMol.weirdAA = true;
                        isLigand = isCC || ligands.hasOwnProperty(currentMol.name);
                        if (currentMol.name in molmil.SNFG || label_entity_id[a] in branch_ids) currentMol.SNFG = true;
                    }

                    Xpos = currentChain.modelsXYZ[0].length;
                    currentChain.modelsXYZ[0].push(Cartn_x[a], Cartn_y[a], Cartn_z[a]);

                    currentMol.atoms.push(atom = new molmil.atomObject(Xpos, auth_atom_id[a] || label_atom_id[a] || "", type_symbol[a] || "", currentMol, currentChain));

                    if (label_alt_id[a]) {
                        if (alt_loc_handler == null) alt_loc_handler = label_alt_id[a];
                        if (label_alt_id[a] != alt_loc_handler) atom.display = false;
                    }

                    if (!atom.element) {
                        for (offset = 0; offset < atom.atomName.length; offset++) if (!molmil_dep.isNumber(atom.atomName[offset])) break;
                        if (atom.atomName.length > 1 && !molmil_dep.isNumber(atom.atomName[1]) && atom.atomName[1] == atom.atomName[1].toLowerCase()) atom.element = atom.atomName.substring(offset, offset + 2);
                        else atom.element = atom.atomName.substring(offset, offset + 1);
                    }
                    if (atom.element == "H") atom.display = this.showHydrogens;

                    isHet = true;
                    if (group_PDB.length) { if (group_PDB[a] != "HETATM" || polyTypes.hasOwnProperty(currentMol.name)) isHet = false; }
                    else isHet = false;

                    atom.label_alt_id = label_alt_id[a] || "";
                    if (currentMol.water) currentChain.display = this.showWaters;
                    if (atom.element == "H") atom.display = this.showHydrogens;
                    else if (atom.display) {
                        currentChain.isHet = false;
                        if (atom.atomName == "N") { currentMol.N = atom; currentMol.ligand = isLigand; }
                        else if (atom.atomName == "CA") { currentMol.CA = atom; currentMol.ligand = isLigand; }
                        else if (atom.atomName == "C") { currentMol.C = atom; currentMol.ligand = isLigand; }
                        else if (atom.atomName == "O") { currentMol.O = atom; currentMol.ligand = isLigand; currentMol.xna = false; }
                        //do special stuff for dna/rna
                        else if (!isHet && atom.atomName == "P" && !(currentMol.N || currentMol.CA) && !molmil.configBox.xna_force_C1p) { currentMol.N = currentMol.CA = atom; currentMol.xna = true; currentMol.ligand = isLigand; }
                        else if (!isHet && atom.atomName == "C1'" && !(currentMol.N || currentMol.CA) && molmil.configBox.xna_force_C1p) { currentMol.CA = atom; currentMol.xna = true; currentMol.ligand = isLigand; }
                        else if (!isHet && atom.atomName == "O3'" && !(currentMol.C)) { currentMol.C = atom; currentMol.xna = true; currentMol.ligand = isLigand; }
                    }
                    else {
                        currentChain.isHet = false;
                        if (atom.atomName == "N") { currentMol.N = currentMol.N || atom; currentMol.ligand = isLigand; }
                        else if (atom.atomName == "CA") { currentMol.CA = currentMol.CA || atom; currentMol.ligand = isLigand; }
                        else if (atom.atomName == "C") { currentMol.C = currentMol.C || atom; currentMol.ligand = isLigand; }
                        else if (atom.atomName == "O") { currentMol.O = currentMol.O || atom; currentMol.ligand = isLigand; currentMol.xna = false; }
                        //do special stuff for dna/rna
                        else if (!isHet && atom.atomName == "P" && !(currentMol.N || currentMol.CA) && !molmil.configBox.xna_force_C1p) { currentMol.N = currentMol.CA = atom; currentMol.xna = true; currentMol.ligand = isLigand; }
                        else if (!isHet && atom.atomName == "C1'" && !(currentMol.N || currentMol.CA) && molmil.configBox.xna_force_C1p) { currentMol.CA = atom; currentMol.xna = true; currentMol.ligand = isLigand; }
                        else if (!isHet && atom.atomName == "O3'" && !(currentMol.C)) { currentMol.C = currentMol.C || atom; currentMol.xna = true; currentMol.ligand = isLigand; }
                    }

                    atom.Bfactor = B_iso_or_equiv[a] || 0.0;

                    currentChain.atoms.push(atom);
                    atom.AID = this.AID++;
                    this.atomRef[atom.AID] = atom;
                }

                struc.meta.pdbid = entryId.toLowerCase();
                if (pdb.meta_pdbjplus) {
                    currentChain.name = pdb.meta_pdbjplus.auth_asym_id;
                    currentChain.authName = pdb.meta_pdbjplus.label_asym_id;
                    struc.meta.pdbid = pdb.meta_pdbjplus.pdbid;
                }

                struc.structureTransform = null;
                if (pdb.hasOwnProperty("atom_sites")) {
                    var scaleN = [pdb.atom_sites["fract_transf_matrix[1][1]"], pdb.atom_sites["fract_transf_matrix[1][2]"], pdb.atom_sites["fract_transf_matrix[1][3]"], pdb.atom_sites["fract_transf_matrix[2][1]"], pdb.atom_sites["fract_transf_matrix[2][2]"], pdb.atom_sites["fract_transf_matrix[2][3]"], pdb.atom_sites["fract_transf_matrix[3][1]"], pdb.atom_sites["fract_transf_matrix[3][2]"], pdb.atom_sites["fract_transf_matrix[3][3]"]];

                    var badY = false, badZ = false, unit, vec;
                    //console.log(scaleN);
                    if (scaleN[3] != 0 || scaleN[5] != 0) {
                        //badY = true;
                        unit = [0, 1, 0];
                        vec = [scaleN[3], scaleN[4], scaleN[5]];
                        //console.log(vec); ///////////////
                    }
                    else if (scaleN[6] != 0 || scaleN[7] != 0) {
                        badZ = true;
                        unit = [0, 0, 1];
                        vec = [scaleN[6], scaleN[7], scaleN[8]];
                    }

                    // 4xxd: badY
                    // 1gof: badZ

                    if (badY || badZ) { // deal with badly oriented structures
                        var uvw, rcos, rsin, R;
                        vec3.normalize(vec, vec);

                        uvw = vec3.cross([0, 0, 0], vec, unit);
                        rcos = vec3.dot(vec, unit);
                        rsin = vec3.length(uvw);
                        if (Math.abs(rsin) > 1e-6) { uvw[0] /= rsin; uvw[1] /= rsin; uvw[2] /= rsin; }

                        R = mat3.create();
                        mat3.multiplyScalar(R, R, rcos);

                        R[0] += 0;
                        R[1] += -uvw[2] * rsin;
                        R[2] += uvw[1] * rsin;
                        R[3] += uvw[2] * rsin;
                        R[4] += 0;
                        R[5] += -uvw[0] * rsin;
                        R[6] += -uvw[1] * rsin;
                        R[7] += uvw[0] * rsin;
                        R[8] += 0;

                        R[0] += uvw[0] * uvw[0] * (1 - rcos);
                        R[1] += uvw[1] * uvw[0] * (1 - rcos);
                        R[2] += uvw[2] * uvw[0] * (1 - rcos);
                        R[3] += uvw[0] * uvw[1] * (1 - rcos);
                        R[4] += uvw[1] * uvw[1] * (1 - rcos);
                        R[5] += uvw[2] * uvw[1] * (1 - rcos);
                        R[6] += uvw[0] * uvw[2] * (1 - rcos);
                        R[7] += uvw[1] * uvw[2] * (1 - rcos);
                        R[8] += uvw[2] * uvw[2] * (1 - rcos);

                        mat3.transpose(R, R);
                        struc.structureTransform = R;

                        var temp = [0, 0, 0], c, i;
                        for (c = 0; c < struc.chains.length; c++) {
                            currentChain = struc.chains[c];
                            for (i = 0; i < currentChain.modelsXYZ[0].length; i += 3) {
                                temp[0] = currentChain.modelsXYZ[0][i]; temp[1] = currentChain.modelsXYZ[0][i + 1]; temp[2] = currentChain.modelsXYZ[0][i + 2];
                                vec3.transformMat3(temp, temp, R);
                                currentChain.modelsXYZ[0][i] = temp[0]; currentChain.modelsXYZ[0][i + 1] = temp[1]; currentChain.modelsXYZ[0][i + 2] = temp[2];
                            }
                        }
                    }

                    // deal with fract_transf_vector[1] somehow 
                }

                var cid = 0, xyzs;
                for (; a < Cartn_x.length; a++) {
                    if (pdbx_PDB_model_num && pdbx_PDB_model_num[a] != cmnum) {
                        cmnum = pdbx_PDB_model_num ? pdbx_PDB_model_num[a] : 0; ccid = null; cid = -1;
                    }

                    if (label_asym_id[a] != ccid || !currentChain) {
                        ccid = label_asym_id[a]; cid += 1;
                        if (cid >= struc.chains.length) xyzs = []; // trash
                        else struc.chains[cid].modelsXYZ.push(xyzs = []);
                    }

                    xyzs.push(Cartn_x[a], Cartn_y[a], Cartn_z[a]);
                }

                struc.number_of_frames = struc.chains.length ? struc.chains[0].modelsXYZ.length : 0;
                this.calculateCOG();

                for (var s = 0; s < structs.length; s++) {

                    struc = structs[s];

                    // loop over all residues and set showSC to true if a weird residue...
                    for (var c = 0, m, mol; c < struc.chains.length; c++) {
                        for (m = 0; m < struc.chains[c].molecules.length; m++) {
                            mol = struc.chains[c].molecules[m];
                            if (!mol.ligand && !mol.water && !mol.xna && !molmil.AATypesBase.hasOwnProperty(mol.name)) mol.weirdAA = true;
                        }
                    }

                    var struct_conn = pdb.struct_conn || { id: [] }, a1, a2;

                    if (struct_conn.id.length) {
                        var backboneAtoms = molmil.configBox.backboneAtoms4Display;
                        for (c = 0; c < struc.chains.length; c++) struc.chains[c].struct_conn = [];
                        for (var i = 0, c; i < struct_conn.id.length; i++) {
                            if (struct_conn.conn_type_id && (struct_conn.conn_type_id[i] == "hydrog" || struct_conn.conn_type_id[i] == "metalc")) continue;
                            if ((struct_conn.ptnr1_symmetry && struct_conn.ptnr1_symmetry[i] != "1_555") || (struct_conn.ptnr2_symmetry && struct_conn.ptnr2_symmetry[i] != "1_555")) continue;
                            a1 = molmil.searchAtom(struc, struct_conn.ptnr1_label_asym_id[i], struct_conn.ptnr1_auth_seq_id[i], struct_conn.ptnr1_label_atom_id[i]);
                            if (!a1) continue;
                            a2 = molmil.searchAtom(struc, struct_conn.ptnr2_label_asym_id[i], struct_conn.ptnr2_auth_seq_id[i], struct_conn.ptnr2_label_atom_id[i]);
                            if (!a2) continue;
                            a1.chain.struct_conn.push([a1, a2, 1]);
                            if (a1.chain != a2.chain) a2.chain.struct_conn.push([a2, a1, 1]);
                            if (a1.molecule.ligand || !backboneAtoms.hasOwnProperty(a1.atomName)) a1.molecule.weirdAA = true;
                            if (a2.molecule.ligand || !backboneAtoms.hasOwnProperty(a2.atomName)) a2.molecule.weirdAA = true;
                        }
                    }

                    var cb = pdb.chem_comp_bond || pdb.pdbx_chem_comp_model_bond || { comp_id: [] }, cbMat = {};
                    for (var i = 0; i < cb.comp_id.length; i++) {
                        if (!(cb.comp_id[i] in cbMat)) cbMat[cb.comp_id[i]] = {};
                        cbMat[cb.comp_id[i]][cb.atom_id_1[i] + "_" + cb.atom_id_2[i]] = cbMat[cb.comp_id[i]][cb.atom_id_2[i] + "_" + cb.atom_id_1[i]] = cb.value_order[i].toLowerCase() == "doub" ? 2 : 1;
                    }
                    struc.cbMat = cbMat;

                    for (var i = 0; i < struc.chains.length; i++) {
                        if (!struc.chains[i].isHet && struc.chains[i].molecules.length == 1) {
                            struc.chains[i].isHet = true;
                            struc.chains[i].molecules[0].ligand = true;
                        }
                        this.buildMolBondList(struc.chains[i]);
                        var chain = struc.chains[i];
                        chain.molWeight = 0.0;
                        for (a = 0; a < chain.atoms.length; a++) chain.molWeight += molmil.configBox.MW[chain.atoms[a].element] || 0;
                    }

                    //var conf_type_id, beg_auth_asym_id, beg_auth_seq_id, end_auth_asym_id, end_auth_seq_id, start, end;
                    var conf_type_id, beg_label_asym_id, beg_label_seq_id, end_label_asym_id, end_label_seq_id, start, end;

                    var struct_set = false;

                    // add a check here to make sure it's not adding CRAP
                    var struct_conf = pdb.struct_conf;
                    if (struct_conf && Cartn_x.length) {
                        for (var i = 0, m; i < struct_conf.id.length; i++) {
                            conf_type_id = struct_conf.conf_type_id[i];
                            if (conf_type_id == "HELX_P") conf_type_id = 3;
                            //else if (conf_type_id == "TURN_P") conf_type_id = 4;
                            else continue;
                            beg_label_asym_id = struct_conf.beg_label_asym_id[i];
                            beg_label_seq_id = struct_conf.beg_label_seq_id[i];
                            end_label_asym_id = struct_conf.end_label_asym_id[i];
                            end_label_seq_id = struct_conf.end_label_seq_id[i];
                            //if (end_label_seq_id-beg_label_seq_id < 3) conf_type_id = 4;

                            start = this.getMolObject4Chain(this.getChain(struc, beg_label_asym_id), beg_label_seq_id);
                            end = this.getMolObject4Chain(this.getChain(struc, end_label_asym_id), end_label_seq_id);
                            if (start == end) continue;
                            while (end) {
                                if (start == null) break;
                                start.sndStruc = conf_type_id;
                                if (start == end) break;
                                start = start.next;
                            }
                            struct_set = true;
                        }
                    }

                    var struct_sheet_range = pdb.struct_sheet_range;
                    if (struct_sheet_range && Cartn_x.length) {
                        for (var i = 0, m; i < struct_sheet_range.beg_label_asym_id.length; i++) {
                            beg_label_asym_id = struct_sheet_range.beg_label_asym_id[i];
                            beg_label_seq_id = struct_sheet_range.beg_label_seq_id[i];
                            end_label_asym_id = struct_sheet_range.end_label_asym_id[i];
                            end_label_seq_id = struct_sheet_range.end_label_seq_id[i];
                            start = this.getMolObject4Chain(this.getChain(struc, beg_label_asym_id), beg_label_seq_id);
                            end = this.getMolObject4Chain(this.getChain(struc, end_label_asym_id), end_label_seq_id);
                            if (start == end) continue;
                            while (end) {
                                if (start == null) break;
                                start.sndStruc = 2;
                                if (start == end) break;
                                start = start.next;
                            }
                            struct_set = true;
                        }
                    }

                    if (!struct_set) { for (c = 0; c < struc.chains.length; c++) this.ssAssign(struc.chains[c]); }

                    molmil.resetColors(struc, this);

                }

                var poly;
                if (pdb.hasOwnProperty("entity_poly")) {
                    poly = pdb.entity_poly.entity_id;
                    for (var i = 0; i < pdb.struct_asym.id.length; i++) { if (poly.indexOf(pdb.struct_asym.entity_id[i]) != -1) this.poly_asym_ids.push(pdb.struct_asym.id[i]); }
                }

                var pdbx_struct_oper_list = pdb.pdbx_struct_oper_list;
                if (pdbx_struct_oper_list) {
                    var i, length = pdbx_struct_oper_list.type.length;
                    var xmode = !pdbx_struct_oper_list.hasOwnProperty("matrix[1][1]"), mat;
                    for (i = 0; i < length; i++) {
                        mat = mat4.create();

                        mat[0] = pdbx_struct_oper_list[xmode ? "matrix11" : "matrix[1][1]"][i];
                        mat[4] = pdbx_struct_oper_list[xmode ? "matrix12" : "matrix[1][2]"][i];
                        mat[8] = pdbx_struct_oper_list[xmode ? "matrix13" : "matrix[1][3]"][i];
                        mat[12] = pdbx_struct_oper_list[xmode ? "vector1" : "vector[1]"][i];

                        mat[1] = pdbx_struct_oper_list[xmode ? "matrix21" : "matrix[2][1]"][i];
                        mat[5] = pdbx_struct_oper_list[xmode ? "matrix22" : "matrix[2][2]"][i];
                        mat[9] = pdbx_struct_oper_list[xmode ? "matrix23" : "matrix[2][3]"][i];
                        mat[13] = pdbx_struct_oper_list[xmode ? "vector2" : "vector[2]"][i];

                        mat[2] = pdbx_struct_oper_list[xmode ? "matrix31" : "matrix[3][1]"][i];
                        mat[6] = pdbx_struct_oper_list[xmode ? "matrix32" : "matrix[3][2]"][i];
                        mat[10] = pdbx_struct_oper_list[xmode ? "matrix33" : "matrix[3][3]"][i];
                        mat[14] = pdbx_struct_oper_list[xmode ? "vector3" : "vector[3]"][i];

                        if (mat[0] == 1 && mat[1] == 0 && mat[2] == 0 && mat[3] == 0 &&
                            mat[4] == 0 && mat[5] == 1 && mat[6] == 0 && mat[7] == 0 &&
                            mat[8] == 0 && mat[9] == 0 && mat[10] == 1 && mat[11] == 0 &&
                            mat[12] == 0 && mat[13] == 0 && mat[14] == 0 && mat[15] == 1) struc.BUmatrices[pdbx_struct_oper_list.id[i]] = ["identity operation", mat];
                        else struc.BUmatrices[pdbx_struct_oper_list.id[i]] = [pdbx_struct_oper_list.type[i], mat];

                        //struc.BUmatrices[pdbx_struct_oper_list.id[i]] = [pdbx_struct_oper_list.type[i], mat];
                    }

                    this.AisB = (!pdbx_struct_oper_list || !pdb.pdbx_struct_assembly || (pdbx_struct_oper_list.id.length == 1 && pdb.pdbx_struct_assembly.id == 1));

                    var pdbx_struct_assembly_gen = pdb.pdbx_struct_assembly_gen, tmp1, tmp2, tmp3 = mat4.create(), j, k, mats;
                    try { length = pdbx_struct_assembly_gen.assembly_id.length; } catch (e) { length = 0; }
                    var xpnd = function (inp) {
                        tmp2 = [];
                        inp = inp.split(",");
                        for (j = 0; j < inp.length; j++) {
                            if (inp[j].indexOf("-") != -1) {
                                inp[j] = inp[j].replace("(", "").replace(")", "").split("-");
                                for (k = parseInt(inp[j][0]); k < parseInt(inp[j][1]) + 1; k++) tmp2.push(k)
                            }
                            else tmp2.push(inp[j].replace("(", "").replace(")", ""));
                        }
                        return tmp2;
                    }


                    //pdbx_struct_assembly_gen.oper_expression
                    //sum (number of residues in each pdbx_struct_assembly_gen.asym_id_list)
                    // A*B

                    for (i = 0; i < length; i++) {
                        if (!struc.BUassemblies.hasOwnProperty(pdbx_struct_assembly_gen.assembly_id[i])) struc.BUassemblies[pdbx_struct_assembly_gen.assembly_id[i]] = [];
                        mats = [];
                        if (pdbx_struct_assembly_gen.oper_expression[i].indexOf(")(") != -1) {
                            tmp1 = pdbx_struct_assembly_gen.oper_expression[i].split(")(");
                            tmp1[0] = xpnd(tmp1[0].substr(1));
                            tmp1[1] = xpnd(tmp1[1].substr(0, tmp1[1].length - 1));
                            // build new matrices
                            for (j = 0; j < tmp1[0].length; j++) {
                                for (k = 0; k < tmp1[1].length; k++) {
                                    poly = tmp1[0][j] + "-" + tmp1[1][k];
                                    if (!struc.BUmatrices.hasOwnProperty(poly)) {
                                        mat4.multiply(tmp3, struc.BUmatrices[tmp1[0][j]][1], struc.BUmatrices[tmp1[1][k]][1]);
                                        if (tmp3[0] == 1 && tmp3[1] == 0 && tmp3[2] == 0 && tmp3[3] == 0 &&
                                            tmp3[4] == 0 && tmp3[5] == 1 && tmp3[6] == 0 && tmp3[7] == 0 &&
                                            tmp3[8] == 0 && tmp3[9] == 0 && tmp3[10] == 1 && tmp3[11] == 0 &&
                                            tmp3[12] == 0 && tmp3[13] == 0 && tmp3[14] == 0 && tmp3[15] == 1) struc.BUmatrices[poly] = ["identity operation", tmp3];
                                        else struc.BUmatrices[poly] = ["combined", mat4.clone(tmp3)];
                                    }
                                    mats.push(poly);
                                }
                            }
                        }
                        else {
                            mats = xpnd(pdbx_struct_assembly_gen.oper_expression[i]);
                        }
                        struc.BUassemblies[pdbx_struct_assembly_gen.assembly_id[i]].push([mats, pdbx_struct_assembly_gen.asym_id_list[i].split(",")]);
                    }
                }

                this.pdbxData = pdb;
                struc.meta.pdbxData = pdb;
                if (!settings.skipDeleteJSO) delete pdb.atom_site;
            }



            return structs;
        };

        // ** calculates the center of gravity of a system **
        calculateCOG(atomList) {
            if (this.skipCOGupdate) return;
            this.avgX = 0;
            this.avgY = 0;
            this.avgZ = 0;
            this.stdX = 0;
            this.stdY = 0;
            this.stdZ = 0;
            this.avgXYZ = [0, 0, 0];
            this.stdXYZ = [0, 0, 0];
            var n = 0;
            var CA;
            var xyzRef, Xpos;
            var poss = [], ALTs = [];

            this.geomRanges = [0, 0, 0, 0, 0, 0];
            var struct, chain, s, c, m, a, xyz, modelId = this.renderer.modelId;

            //molmil.polygonObject

            if (atomList) {
                for (a = 0; a < atomList.length; a++) {
                    Xpos = atomList[a].xyz;
                    xyzRef = atomList[a].chain.modelsXYZ[modelId];
                    xyz = [xyzRef[Xpos], xyzRef[Xpos + 1], xyzRef[Xpos + 2]];
                    this.avgX += xyz[0];
                    this.avgY += xyz[1];
                    this.avgZ += xyz[2];
                    poss.push(xyz);
                    n++;
                }
            }
            else {
                for (s = 0; s < this.structures.length; s++) {
                    struct = this.structures[s];
                    if (struct instanceof molmil.entryObject) {
                        for (c = 0; c < struct.chains.length; c++) {
                            chain = struct.chains[c];
                            xyzRef = chain.modelsXYZ[modelId];
                            if (xyzRef === undefined) continue;
                            if (chain.molecules.length && chain.molecules[0].water) continue;

                            for (m = 0; m < chain.molecules.length; m++) {
                                if (!chain.molecules[m].display) continue;
                                if (chain.molecules[m].ligand || chain.molecules[m].water) {
                                    for (a = 0; a < chain.molecules[m].atoms.length; a++) {
                                        Xpos = chain.molecules[m].atoms[a].xyz;
                                        xyz = [xyzRef[Xpos], xyzRef[Xpos + 1], xyzRef[Xpos + 2]];
                                        this.avgX += xyz[0];
                                        this.avgY += xyz[1];
                                        this.avgZ += xyz[2];
                                        poss.push(xyz);
                                        n++;
                                    }
                                }
                                else {
                                    CA = chain.molecules[m].CA;
                                    if (CA != null) {
                                        Xpos = CA.xyz;
                                        xyz = [xyzRef[Xpos], xyzRef[Xpos + 1], xyzRef[Xpos + 2]];
                                        this.avgX += xyz[0];
                                        this.avgY += xyz[1];
                                        this.avgZ += xyz[2];
                                        poss.push(xyz);
                                        n++;
                                    }
                                }
                            }
                        }
                    }
                    else if (struct instanceof molmil.polygonObject && struct.meta.COR) {
                        this.avgX += struct.meta.COR[0];
                        this.avgY += struct.meta.COR[1];
                        this.avgZ += struct.meta.COR[2];
                        n += struct.meta.COR[3];
                        if (struct.meta.hasOwnProperty("geomRanges")) ALTs.push(struct.meta.geomRanges, struct.meta.COR[3]);
                    }
                    else if (struct.structures) {
                        for (var i = 0; i < struct.structures.length; i++) {
                            this.avgX += struct.structures[i].meta.COR[0];
                            this.avgY += struct.structures[i].meta.COR[1];
                            this.avgZ += struct.structures[i].meta.COR[2];
                            n += struct.structures[i].meta.COR[3];
                            if (struct.structures[i].meta.hasOwnProperty("geomRanges")) ALTs.push(struct.structures[i].meta.geomRanges, struct.structures[i].meta.COR[3]);
                        }
                    }
                }
            }

            if (n == 0) return;

            this.avgX /= n;
            this.avgY /= n;
            this.avgZ /= n;


            var xMin = 1e99, xMax = -1e99, yMin = 1e99, yMax = -1e99, zMin = 1e99, zMax = -1e99;

            var tmp, n_tmp;
            for (var i = 0; i < poss.length; i++) {
                tmp = poss[i][0] - this.avgX;
                this.stdX += tmp * tmp;
                if (tmp < xMin) xMin = tmp;
                if (tmp > xMax) xMax = tmp;


                tmp = poss[i][1] - this.avgY;
                this.stdY += tmp * tmp;
                if (tmp < yMin) yMin = tmp;
                if (tmp > yMax) yMax = tmp;

                tmp = poss[i][2] - this.avgZ;
                this.stdZ += tmp * tmp;
                if (tmp < zMin) zMin = tmp;
                if (tmp > zMax) zMax = tmp;
            }

            for (var i = 0; i < ALTs.length; i += 2) {
                n_tmp = ALTs[i][1];
                tmp = ALTs[i][0] - this.avgX;
                this.stdX += (tmp * tmp) * n_tmp * .5;
                if (tmp < xMin) xMin = tmp;

                tmp = ALTs[i][1] - this.avgX;
                this.stdX += (tmp * tmp) * n_tmp * .5;
                if (tmp > xMax) xMax = tmp;

                tmp = ALTs[i][2] - this.avgY;
                this.stdY += (tmp * tmp) * n_tmp * .5;
                if (tmp < yMin) yMin = tmp;

                tmp = ALTs[i][3] - this.avgY;
                this.stdY += (tmp * tmp) * n_tmp * .5;
                if (tmp > yMax) yMax = tmp;

                tmp = ALTs[i][4] - this.avgZ;
                this.stdZ += (tmp * tmp) * n_tmp * .5;
                if (tmp < zMin) zMin = tmp;

                tmp = ALTs[i][5] - this.avgZ;
                this.stdZ += (tmp * tmp) * n_tmp * .5;
                if (tmp > zMax) zMax = tmp;
            }

            this.stdX = Math.sqrt(this.stdX / n);
            this.stdY = Math.sqrt(this.stdY / n);
            this.stdZ = Math.sqrt(this.stdZ / n);


            this.avgXYZ = [this.avgX, this.avgY, this.avgZ];
            this.stdXYZ = [this.stdX, this.stdY, this.stdZ];
            this.COR = this.avgXYZ;

            this.geomRanges = [xMin, xMax, yMin, yMax, zMin, zMax];
        }

        // ** assigns the secondary structure elements of a chain object using DSSP or CA torsion angles**
        ssAssign(chainObj) {
            return molmil.loadPlugin(molmil.settings.src + "plugins/loaders.js", this.ssAssign, this, [chainObj]);
        };

        // ** center of rotation manipulation **

        setCOR(selection) {
            var modelId = this.renderer.modelId;
            if (this.lastKnowAS) resetCOR();
            this.lastKnownAS = null;
            if (!this.atomSelection.length && !selection) return;
            if (!selection) selection = [this.atomSelection[0].chain.modelsXYZ[modelId][this.atomSelection[0].xyz], this.atomSelection[0].chain.modelsXYZ[modelId][this.atomSelection[0].xyz + 1], this.atomSelection[0].chain.modelsXYZ[modelId][this.atomSelection[0].xyz + 2]];
            this.lastKnownAS = [selection[0], selection[1], selection[2]];
            var rotMat = mat3.create(); mat3.fromMat4(rotMat, this.renderer.modelViewMatrix);
            var delta = vec3.subtract([0, 0, 0], this.lastKnownAS, this.avgXYZ);
            vec3.transformMat3(delta, delta, rotMat);
            this.renderer.camera.x += delta[0];
            this.renderer.camera.y += delta[1];
            this.renderer.camera.z += delta[2];
            for (var i = 0; i < this.canvases.length; i++) this.canvases[i].molmilViewer.COR = this.lastKnownAS;
            this.canvas.atomCORset = true;
        }

        resetCOR() {
            var rotMat = mat3.create(); mat3.fromMat4(rotMat, this.renderer.modelViewMatrix);
            var delta = vec3.subtract([0, 0, 0], this.avgXYZ, this.lastKnownAS);
            vec3.transformMat3(delta, delta, rotMat);
            this.renderer.camera.x += delta[0];
            this.renderer.camera.y += delta[1];
            this.renderer.camera.z += delta[2];
            for (var i = 0; i < this.canvases.length; i++) this.canvases[i].molmilViewer.COR = this.avgXYZ;
            this.canvas.atomCORset = false;
            this.lastKnownAS = null;
        }

        hideCell() {
            this.renderer.removeProgramByName("molmil.Cell");
        };

        showCell() {
            this.hideCell();
            for (var i = 0; i < this.structures.length; i++) {
                if (this.structures[i].meta.cellLengths) { // for now, just support a simple box...
                    var cell = this.structures[i].meta.cellLengths;
                    var objects = [];

                    var POSs = [
                        [0.0, 0.0, 0.0],
                        [cell[0], 0.0, 0.0],
                        [0.0, cell[1], 0.0],
                        [0.0, 0.0, cell[2]],
                        [cell[0], cell[1], 0.0],
                        [cell[0], 0.0, cell[2]],
                        [0.0, cell[1], cell[2]],
                        [cell[0], cell[1], cell[2]]
                    ];


                    objects.push({ type: "sphere", coords: [POSs[0]], rgba: [0, 0, 0, 255], radius: 0.15 });
                    objects.push({ type: "sphere", coords: [POSs[1]], rgba: [0, 0, 0, 255], radius: 0.15 });
                    objects.push({ type: "sphere", coords: [POSs[2]], rgba: [0, 0, 0, 255], radius: 0.15 });
                    objects.push({ type: "sphere", coords: [POSs[3]], rgba: [0, 0, 0, 255], radius: 0.15 });

                    objects.push({ type: "sphere", coords: [POSs[4]], rgba: [0, 0, 0, 255], radius: 0.15 });
                    objects.push({ type: "sphere", coords: [POSs[5]], rgba: [0, 0, 0, 255], radius: 0.15 });
                    objects.push({ type: "sphere", coords: [POSs[6]], rgba: [0, 0, 0, 255], radius: 0.15 });
                    objects.push({ type: "sphere", coords: [POSs[7]], rgba: [0, 0, 0, 255], radius: 0.15 });

                    objects.push({ type: "cylinder", coords: [POSs[0], POSs[1]], rgba: [0, 0, 0, 255], radius: 0.15 });
                    objects.push({ type: "cylinder", coords: [POSs[0], POSs[2]], rgba: [0, 0, 0, 255], radius: 0.15 });
                    objects.push({ type: "cylinder", coords: [POSs[3], POSs[0]], rgba: [0, 0, 0, 255], radius: 0.15 });

                    objects.push({ type: "cylinder", coords: [POSs[7], POSs[4]], rgba: [0, 0, 0, 255], radius: 0.15 });
                    objects.push({ type: "cylinder", coords: [POSs[5], POSs[7]], rgba: [0, 0, 0, 255], radius: 0.15 });
                    objects.push({ type: "cylinder", coords: [POSs[6], POSs[7]], rgba: [0, 0, 0, 255], radius: 0.15 });

                    objects.push({ type: "cylinder", coords: [POSs[1], POSs[4]], rgba: [0, 0, 0, 255], radius: 0.15 });
                    objects.push({ type: "cylinder", coords: [POSs[5], POSs[1]], rgba: [0, 0, 0, 255], radius: 0.15 });

                    objects.push({ type: "cylinder", coords: [POSs[2], POSs[4]], rgba: [0, 0, 0, 255], radius: 0.15 });
                    objects.push({ type: "cylinder", coords: [POSs[6], POSs[2]], rgba: [0, 0, 0, 255], radius: 0.15 });

                    objects.push({ type: "cylinder", coords: [POSs[3], POSs[5]], rgba: [0, 0, 0, 255], radius: 0.15 });
                    objects.push({ type: "cylinder", coords: [POSs[3], POSs[6]], rgba: [0, 0, 0, 255], radius: 0.15 });

                    return molmil.geometry.generator(objects, this, "molmil.Cell", { solid: true });

                }
            }

        }


    }
}