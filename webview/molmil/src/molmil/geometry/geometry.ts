namespace molmil {
    // geometry functions

    // ** generates a PLY file **
    export function exportPLY(soup, file) {
        return molmil.loadPlugin(molmil.settings.src + "plugins/savers.js", this.exportPLY, this, [soup, file]);
    };

    export function exportMPBF(soup) {
        return molmil.loadPlugin(molmil.settings.src + "plugins/savers.js", this.exportMPBF, this, [soup]);
    };

    // ** generates an STL file **
    export function exportSTL(soup) {
        return molmil.loadPlugin(molmil.settings.src + "plugins/savers.js", this.exportSTL, this, [soup]);
    };


    export function buCheck(assembly_id, displayMode, colorMode, struct, soup) {
        return molmil.loadPlugin(molmil.settings.src + "plugins/misc.js", this.buCheck, this, [assembly_id, displayMode, colorMode, struct, soup]);
    }

    export function findResidueRings(molObj) {
        var bondInfo = {}, atomRef = {}, i;
        for (i = 0; i < molObj.atoms.length; i++) {
            bondInfo[molObj.atoms[i].AID] = [];
            atomRef[molObj.atoms[i].AID] = molObj.atoms[i];
        }
        for (var i = 0; i < molObj.chain.bonds.length; i++) {
            if (molObj.chain.bonds[i][0].AID in bondInfo) bondInfo[molObj.chain.bonds[i][0].AID].push(molObj.chain.bonds[i][1]);
            if (molObj.chain.bonds[i][1].AID in bondInfo) bondInfo[molObj.chain.bonds[i][1].AID].push(molObj.chain.bonds[i][0]);
        }

        var ringlist = {}, N = 0;
        var scancyclic = function (atom, seq) {
            N++;
            var newseq = seq.slice(), b, bonds = bondInfo[atom.AID], midx; newseq.push(atom.AID);
            if (bonds === undefined) return;
            if (N > 1e6) return; // give up on broken mess...
            for (b = 0; b < bonds.length; b++) {
                midx = newseq.indexOf(bonds[b].AID);
                if (midx == -1) scancyclic(bonds[b], newseq);
                else if (newseq.length - midx > 5) ringlist[newseq.slice().sort(function (a, b) { return a - b; }).join("-")] = newseq.map(function (x) { return atomRef[x]; });
            }
            return bonds.length;
        };
        scancyclic(molObj.atoms[0], []);
        return Object.values(ringlist);
    };

    export class _geometry {

        // ** geometry object, used to generate protein geometry; atoms, bonds, loops, helices, sheets **


        templates = { sphere: { base: {} }, cylinder: [], dome: {} };
        detail_lvs = 5;
        dome = [0, 0, -1];
        radius = .25;
        sheetHeight = .125;
        skipClearBuffer = false;
        onGenerate = null;

        generator(objects, soup, name, programOptions) {
            return molmil.loadPlugin(molmil.settings.src + "plugins/misc.js", this.generator, this, [objects, soup, name, programOptions]);
        }


        getSphere(r, detail_lv) {
            if (detail_lv > 6) detail_lv = 6;
            if (r in this.templates.sphere && detail_lv in this.templates.sphere[r]) return this.templates.sphere[r][detail_lv];
            else return this.generateSphere(r, detail_lv);
            // pull a sphere from the this.templates.sphere object if it exists for the given radius
        }

        getCylinder(detail_lv) {
            return this.templates.cylinder[detail_lv] || molmil.geometry.generateCylinder(detail_lv)[detail_lv];
        }

        generateCylinder(detail_lv = null) {
            // generates a cylinder with length 0.5
            // afterwards apply a transformation matrix ()
            // 5
            // 10
            // 20
            // 40

            var nop, rad, theta, p, x, y, z, dij, that = this;
            var DO = function (lod) {
                nop = (lod + 1) * 4;
                theta = 0.0;
                rad = 2.0 / nop;
                that.templates.cylinder.push({ vertices: [], normals: [], indices: [] });
                for (p = 0; p < nop; p++) {
                    x = Math.cos(theta * Math.PI);
                    y = Math.sin(theta * Math.PI);
                    that.templates.cylinder[lod].vertices.push(x, y, .0);
                    that.templates.cylinder[lod].vertices.push(x, y, .5);
                    dij = Math.sqrt(x * x + y * y);
                    that.templates.cylinder[lod].normals.push(x / dij, y / dij, .0);
                    that.templates.cylinder[lod].normals.push(x / dij, y / dij, .0);
                    theta += rad;
                }

                for (p = 0; p < (nop - 1) * 2; p += 2) {
                    that.templates.cylinder[lod].indices.push(p, p + 2, p + 3);
                    that.templates.cylinder[lod].indices.push(p + 3, p + 1, p);
                }
                that.templates.cylinder[lod].indices.push(p, 0, 1);
                that.templates.cylinder[lod].indices.push(1, p + 1, p);
            }
            if (detail_lv) DO(detail_lv);
            else { for (var lod = 0; lod < this.detail_lvs; lod++) DO(lod); }
            return this.templates.cylinder;
        };

        generateSphere(r, detail_lv) {
            this.templates.sphere[r] = {};
            var i, nfo, sphere;
            if (!this.templates.sphere.base[detail_lv]) sphere = this.templates.sphere.base[detail_lv] = molmil.octaSphereBuilder(detail_lv);
            else sphere = this.templates.sphere.base[detail_lv];
            nfo = { vertices: [], normals: [], indices: [] };
            for (i = 0; i < sphere.vertices.length; i++) nfo.vertices.push(sphere.vertices[i][0] * r, sphere.vertices[i][1] * r, sphere.vertices[i][2] * r);
            for (i = 0; i < sphere.faces.length; i++) nfo.indices.push(sphere.faces[i][0], sphere.faces[i][1], sphere.faces[i][2]);
            for (i = 0; i < sphere.vertices.length; i++) nfo.normals.push(sphere.vertices[i][0], sphere.vertices[i][1], sphere.vertices[i][2]);
            this.templates.sphere[r][detail_lv] = nfo;
            return nfo;
        }


        // three levels;
        // - atom (ball/sticks/CA-trace)
        // - wireframe
        // - cartoon/rocket

        // ** main **
        generate(structures, render, detail_or) {
            this.reset();
            var chains = [], cchains = [];
            for (var s = 0, c; s < structures.length; s++) {
                if (!(structures[s] instanceof molmil.entryObject) || structures[s].display == false) continue;
                for (var c = 0; c < structures[s].chains.length; c++) {
                    if (!structures[s].chains[c]) continue;
                    if (structures[s].chains[c].display && structures[s].chains[c].molecules.length > 0 && !structures[s].chains[c].isHet && !structures[s].chains[c].molecules[0].water) cchains.push(structures[s].chains[c]);
                    chains.push(structures[s].chains[c]);
                }
            }
            this.initChains(chains, render, detail_or);
            this.initCartoon(cchains);

            if (molmil.configBox.EXT_frag_depth && molmil.configBox.imposterSpheres) this.generateAtomsImposters();
            else this.generateAtoms();
            this.generateBonds();
            this.generateWireframe();
            this.generateCartoon();
            this.generateSNFG();
            //this.generateRockets();

            this.generateSurfaces(cchains, render.soup);

            this.registerPrograms(render);

            if (!this.skipClearBuffer) this.reset();
            if (this.onGenerate) molmil_dep.asyncStart(this.onGenerate[0], this.onGenerate[1], this.onGenerate[2], 0);
        };

        build_simple_render_program(vertices_, indices_, renderer, settings) {
            settings = settings || {};
            if (settings.hasOwnProperty("setBuffers")) {
                var program = settings;
                settings = program.settings;
            }
            else {
                var program = {}; program.settings = settings;
            }

            program.renderer = renderer;

            //settings.solid = false;

            program.setBuffers = function (vertices, indices) {
                var vbuffer = this.renderer.gl.createBuffer();
                this.renderer.gl.bindBuffer(this.renderer.gl.ARRAY_BUFFER, vbuffer);
                this.renderer.gl.bufferData(this.renderer.gl.ARRAY_BUFFER, vertices, this.renderer.gl.STATIC_DRAW);

                var ibuffer = this.renderer.gl.createBuffer();
                this.renderer.gl.bindBuffer(this.renderer.gl.ELEMENT_ARRAY_BUFFER, ibuffer);
                this.renderer.gl.bufferData(this.renderer.gl.ELEMENT_ARRAY_BUFFER, indices, this.renderer.gl.STATIC_DRAW);
                this.size = [vertices.length, indices.length];

                this.nElements = indices.length;
                this.vertexBuffer = vbuffer;
                this.indexBuffer = ibuffer;

                if (molmil.geometry.skipClearBuffer || settings.storeVertices) {
                    this.data = { vertices: vertices, indices: indices, vertexSize: 7 };
                }

            };
            if (vertices_ && indices_) program.setBuffers(vertices_, indices_);

            program.toggleWF = function () {
                if (this.settings.solid) {
                    if (this.settings.wireframeIdxs) {
                        var ibuffer = this.renderer.gl.createBuffer();
                        this.renderer.gl.bindBuffer(this.renderer.gl.ELEMENT_ARRAY_BUFFER, ibuffer);
                        this.renderer.gl.bufferData(this.renderer.gl.ELEMENT_ARRAY_BUFFER, this.settings.wireframeIdxs, this.renderer.gl.STATIC_DRAW);
                        this.indexBuffer = ibuffer;
                        this.nElements = this.settings.wireframeIdxs.length;
                    }
                    this.settings.solid = false;
                }
                else {
                    if (this.settings.triangleIdxs) {
                        var ibuffer = this.renderer.gl.createBuffer();
                        this.renderer.gl.bindBuffer(this.renderer.gl.ELEMENT_ARRAY_BUFFER, ibuffer);
                        this.renderer.gl.bufferData(this.renderer.gl.ELEMENT_ARRAY_BUFFER, this.settings.triangleIdxs, this.renderer.gl.STATIC_DRAW);
                        this.indexBuffer = ibuffer;
                        this.nElements = this.settings.triangleIdxs.length;
                    }
                    this.settings.solid = true;
                }
                if (this.rebuild) this.rebuild();
            };

            if (!settings.solid && !settings.lines_render && !program.rebuild) { settings.solid = true; program.toggleWF(); }

            program.rebuild = function (resetBuffers) {
                if (resetBuffers) molmil.geometry.build_simple_render_program(vertices_, indices_, this.renderer, this);
                else molmil.geometry.build_simple_render_program(null, null, this.renderer, this);
            };

            program.angle = renderer.angle;

            program.point_shader = renderer.shaders.points;
            if (program.point_shader) program.point_attributes = program.point_shader.attributes;

            if (settings.uniform_color || settings.rgba) {
                if (settings.alphaSet) {
                    program.pre_shader = renderer.shaders.alpha_dummy;
                    program.standard_shader = renderer.shaders.standard_alphaSet_uniform_color;
                }
                else program.standard_shader = renderer.shaders.standard_uniform_color;
                program.wireframe_shader = renderer.shaders.lines_uniform_color;
                program.point_shader = renderer.shaders.points_uniform_color;
            }
            else {
                if ("alphaMode" in settings) {
                    program.pre_shader = renderer.shaders.alpha_dummy;
                    program.standard_shader = renderer.shaders.standard_alpha;
                    program.standard_shader_opaque = renderer.shaders.standard_shader_opaque;
                    program.standard_shader_transparent = renderer.shaders.standard_shader_transparent;
                }
                else if ("alphaSet" in settings) {
                    program.pre_shader = renderer.shaders.alpha_dummy;
                    program.standard_shader = renderer.shaders.standard_alphaSet;
                }
                else if ("slab" in settings) {
                    if (settings.slabColor) program.standard_shader = renderer.shaders.standard_slabColor;
                    else program.standard_shader = renderer.shaders.standard_slab;
                }
                else program.standard_shader = renderer.shaders.standard;

                program.wireframe_shader = renderer.shaders.lines;
                program.point_shader = renderer.shaders.points;
            }

            program.standard_attributes = program.standard_shader.attributes;
            if (program.pre_shader) program.pre_attributes = program.pre_shader.attributes;
            program.wireframe_attributes = program.wireframe_shader.attributes;

            //program.point_attributes = program.point_shader.attributes;

            program.status = true;

            program.point_render = function (modelViewMatrix, COR, i) {
                if (!this.status) return;

                this.renderer.gl.activeTexture(this.renderer.gl.TEXTURE0);
                this.renderer.gl.bindTexture(this.renderer.gl.TEXTURE_2D, renderer.textures.atom_imposter);

                var normalMatrix = mat3.create();
                this.renderer.gl.useProgram(this.point_shader.program);
                this.renderer.gl.uniformMatrix4fv(this.point_shader.uniforms.modelViewMatrix, false, modelViewMatrix);
                this.renderer.gl.uniformMatrix4fv(this.point_shader.uniforms.projectionMatrix, false, this.renderer.projectionMatrix);
                this.renderer.gl.uniform3f(this.point_shader.uniforms.COR, COR[0], COR[1], COR[2]);
                this.renderer.gl.uniform1f(this.point_shader.uniforms.focus, this.renderer.fogStart);
                this.renderer.gl.uniform1f(this.point_shader.uniforms.fogSpan, this.renderer.fogSpan);

                this.renderer.gl.uniform1f(this.point_shader.uniforms.zFar, molmil.configBox.zFar);
                this.renderer.gl.uniform1f(this.point_shader.uniforms.zNear, molmil.configBox.zNear);
                this.renderer.gl.uniform1i(this.point_shader.uniforms.textureMap, 0);

                this.renderer.gl.uniform1f(this.point_shader.uniforms.zInvDiff, 1. / ((1. / molmil.configBox.zFar) - (1. / molmil.configBox.zNear)));
                this.renderer.gl.uniform1f(this.point_shader.uniforms.zNearInv, 1. / molmil.configBox.zNear);


                this.renderer.gl.enable(this.renderer.gl.DEPTH_TEST);

                if (this.settings.rgba) this.renderer.gl.uniform3f(this.point_shader.uniforms.uniform_color, this.settings.rgba[0] / 255, this.settings.rgba[1] / 255, this.settings.rgba[2] / 255);
                else if (this.settings.uniform_color) this.renderer.gl.uniform3f(this.point_shader.uniforms.uniform_color, this.uniform_color[i][0] / 255, this.uniform_color[i][1] / 255, this.uniform_color[i][2] / 255);

                if (this.renderer.settings.slab) {
                    this.renderer.gl.uniform1f(this.point_shader.uniforms.slabNear, -modelViewMatrix[14] + this.renderer.settings.slabNear - molmil.configBox.zNear);
                    this.renderer.gl.uniform1f(this.point_shader.uniforms.slabFar, -modelViewMatrix[14] + this.renderer.settings.slabFar - molmil.configBox.zNear);
                }

                if (molmil.configBox.fogColor) this.renderer.gl.uniform4f(this.point_shader.uniforms.backgroundColor, molmil.configBox.fogColor[0], molmil.configBox.fogColor[1], molmil.configBox.fogColor[2], 1.0);
                else this.renderer.gl.uniform4f(this.point_shader.uniforms.backgroundColor, molmil.configBox.BGCOLOR[0], molmil.configBox.BGCOLOR[1], molmil.configBox.BGCOLOR[2], 1.0);

                this.renderer.gl.bindBuffer(this.renderer.gl.ARRAY_BUFFER, this.vertexBuffer);

                molmil.resetAttributes(this.renderer.gl);

                if (this.settings.has_ID) {
                    molmil.bindAttribute(this.renderer.gl, this.point_attributes.in_Position, 3, this.renderer.gl.FLOAT, false, 28, 0);
                    molmil.bindAttribute(this.renderer.gl, this.point_attributes.in_radius, 1, this.renderer.gl.FLOAT, false, 28, 12);
                    if (!this.settings.uniform_color && !this.settings.rgba) molmil.bindAttribute(this.renderer.gl, this.point_attributes.in_Colour, 4, this.renderer.gl.UNSIGNED_BYTE, true, 28, 16);
                    molmil.bindAttribute(this.renderer.gl, this.point_attributes.in_ScreenSpaceOffset, 2, this.renderer.gl.SHORT, false, 28, 20);
                    //molmil.bindAttribute(this.renderer.gl, this.point_attributes.in_ID, 1, this.renderer.gl.FLOAT, false, 28, 24);
                }
                else {
                    molmil.bindAttribute(this.renderer.gl, this.point_attributes.in_Position, 3, this.renderer.gl.FLOAT, false, 24, 0);
                    molmil.bindAttribute(this.renderer.gl, this.point_attributes.in_radius, 1, this.renderer.gl.FLOAT, false, 24, 12);
                    if (!this.settings.uniform_color && !this.settings.rgba) molmil.bindAttribute(this.renderer.gl, this.point_attributes.in_Colour, 4, this.renderer.gl.UNSIGNED_BYTE, true, 24, 16);
                    molmil.bindAttribute(this.renderer.gl, this.point_attributes.in_ScreenSpaceOffset, 2, this.renderer.gl.SHORT, false, 24, 20);
                }
                molmil.clearAttributes(this.renderer.gl);



                this.renderer.gl.bindBuffer(this.renderer.gl.ELEMENT_ARRAY_BUFFER, this.indexBuffer);

                if (this.angle) { // angle sucks, it only allows a maximum of 3M "vertices" to be drawn per call...
                    var dv = 0, vtd;
                    while ((vtd = Math.min(this.nElements - dv, 3000000)) > 0) { this.renderer.gl.drawElements(this.renderer.gl.TRIANGLES, vtd, this.renderer.gl.INDEXINT, dv * 4); dv += vtd; }
                }
                else this.renderer.gl.drawElements(this.renderer.gl.TRIANGLES, this.nElements, gl.INDEXINT, 0);

            };

            program.wireframe_render = function (modelViewMatrix, COR, i) {
                if (!this.status) return;
                i = i || 0;
                var normalMatrix = mat3.create();
                this.renderer.gl.useProgram(this.wireframe_shader.program);
                this.renderer.gl.uniformMatrix4fv(this.wireframe_shader.uniforms.modelViewMatrix, false, modelViewMatrix);
                this.renderer.gl.uniformMatrix4fv(this.wireframe_shader.uniforms.projectionMatrix, false, this.renderer.projectionMatrix);
                this.renderer.gl.uniform3f(this.wireframe_shader.uniforms.COR, COR[0], COR[1], COR[2]);
                this.renderer.gl.uniform1f(this.wireframe_shader.uniforms.focus, this.renderer.fogStart);
                this.renderer.gl.uniform1f(this.wireframe_shader.uniforms.fogSpan, this.renderer.fogSpan);
                if (this.settings.rgba) this.renderer.gl.uniform3f(this.wireframe_shader.uniforms.uniform_color, this.settings.rgba[0] / 255, this.settings.rgba[1] / 255, this.settings.rgba[2] / 255);
                else if (this.settings.uniform_color) this.renderer.gl.uniform3f(this.wireframe_shader.uniforms.uniform_color, this.uniform_color[i][0] / 255, this.uniform_color[i][1] / 255, this.uniform_color[i][2] / 255);

                if (this.renderer.settings.slab) {
                    this.renderer.gl.uniform1f(this.wireframe_shader.uniforms.slabNear, -modelViewMatrix[14] + this.renderer.settings.slabNear - molmil.configBox.zNear);
                    this.renderer.gl.uniform1f(this.wireframe_shader.uniforms.slabFar, -modelViewMatrix[14] + this.renderer.settings.slabFar - molmil.configBox.zNear);
                }

                if (molmil.configBox.fogColor) this.renderer.gl.uniform4f(this.wireframe_shader.uniforms.backgroundColor, molmil.configBox.fogColor[0], molmil.configBox.fogColor[1], molmil.configBox.fogColor[2], 1.0);
                else this.renderer.gl.uniform4f(this.wireframe_shader.uniforms.backgroundColor, molmil.configBox.BGCOLOR[0], molmil.configBox.BGCOLOR[1], molmil.configBox.BGCOLOR[2], 1.0);

                this.renderer.gl.bindBuffer(this.renderer.gl.ARRAY_BUFFER, this.vertexBuffer);

                molmil.resetAttributes(this.renderer.gl);
                if (this.settings.lines_render) {
                    if (this.settings.has_ID) {
                        molmil.bindAttribute(this.renderer.gl, this.wireframe_attributes.in_Position, 3, this.renderer.gl.FLOAT, false, 20, 0);
                        if (!this.settings.uniform_color && !this.settings.rgba) molmil.bindAttribute(this.renderer.gl, this.wireframe_attributes.in_Colour, 4, this.renderer.gl.UNSIGNED_BYTE, true, 20, 12);
                        //molmil.bindAttribute(this.renderer.gl, this.wireframe_attributes.in_ID, 1, this.renderer.gl.FLOAT, false, 20, 16);
                    }
                    else {
                        molmil.bindAttribute(this.renderer.gl, this.wireframe_attributes.in_Position, 3, this.renderer.gl.FLOAT, false, 16, 0);
                        if (!this.settings.uniform_color && !this.settings.rgba) molmil.bindAttribute(this.renderer.gl, this.wireframe_attributes.in_Colour, 4, this.renderer.gl.UNSIGNED_BYTE, true, 16, 12);
                    }
                }
                else {
                    if (this.settings.has_ID) {
                        molmil.bindAttribute(this.renderer.gl, this.wireframe_attributes.in_Position, 3, this.renderer.gl.FLOAT, false, 32, 0);
                        //molmil.bindAttribute(this.renderer.gl, this.wireframe_attributes.in_Normal, 3, this.renderer.gl.FLOAT, false, 32, 12);
                        if (!this.settings.uniform_color && !this.settings.rgba) molmil.bindAttribute(this.renderer.gl, this.wireframe_attributes.in_Colour, 4, this.renderer.gl.UNSIGNED_BYTE, true, 32, 24);
                        //molmil.bindAttribute(this.renderer.gl, this.wireframe_attributes.in_ID, 1, this.renderer.gl.FLOAT, false, 32, 28);
                    }
                    else {
                        molmil.bindAttribute(this.renderer.gl, this.wireframe_attributes.in_Position, 3, this.renderer.gl.FLOAT, false, 28, 0);
                        //molmil.bindAttribute(this.renderer.gl, this.wireframe_attributes.in_Normal, 3, this.renderer.gl.FLOAT, false, 28, 12);
                        if (!this.settings.uniform_color && !this.settings.rgba) molmil.bindAttribute(this.renderer.gl, this.wireframe_attributes.in_Colour, 4, this.renderer.gl.UNSIGNED_BYTE, true, 28, 24);
                    }
                }
                molmil.clearAttributes(this.renderer.gl);

                this.renderer.gl.bindBuffer(this.renderer.gl.ELEMENT_ARRAY_BUFFER, this.indexBuffer);

                if (this.angle) { // angle sucks, it only allows a maximum of 3M "vertices" to be drawn per call...
                    var dv = 0, vtd;
                    while ((vtd = Math.min(this.nElements - dv, 3000000)) > 0) { this.renderer.gl.drawElements(this.renderer.gl.LINES, vtd, this.renderer.gl.INDEXINT, dv * 4); dv += vtd; }
                }
                else this.renderer.gl.drawElements(this.renderer.gl.LINES, this.nElements, this.renderer.gl.INDEXINT, 0);
            };

            program.alphaPre = function (modelViewMatrix, COR) {
                this.renderer.gl.useProgram(this.pre_shader.program);

                this.renderer.gl.uniformMatrix4fv(this.pre_shader.uniforms.modelViewMatrix, false, modelViewMatrix);
                this.renderer.gl.uniformMatrix4fv(this.pre_shader.uniforms.projectionMatrix, false, this.renderer.projectionMatrix);
                this.renderer.gl.uniform3f(this.pre_shader.uniforms.COR, COR[0], COR[1], COR[2]);

                this.renderer.gl.bindBuffer(this.renderer.gl.ARRAY_BUFFER, this.vertexBuffer);

                molmil.resetAttributes(this.renderer.gl);

                if (this.settings.has_ID) {
                    molmil.bindAttribute(this.renderer.gl, this.standard_attributes.in_Position, 3, this.renderer.gl.FLOAT, false, 32, 0);
                    if (!this.settings.uniform_color) molmil.bindAttribute(this.renderer.gl, this.standard_attributes.in_Colour, 4, this.renderer.gl.UNSIGNED_BYTE, true, 32, 24);
                }
                else {
                    molmil.bindAttribute(this.renderer.gl, this.standard_attributes.in_Position, 3, this.renderer.gl.FLOAT, false, 28, 0);
                    if (!this.settings.uniform_color && !this.settings.rgba) molmil.bindAttribute(this.renderer.gl, this.standard_attributes.in_Colour, 4, this.renderer.gl.UNSIGNED_BYTE, true, 28, 24);
                }
                molmil.clearAttributes(this.renderer.gl);

                this.renderer.gl.bindBuffer(this.renderer.gl.ELEMENT_ARRAY_BUFFER, this.indexBuffer);

                this.renderer.gl.colorMask(false, false, false, false);

                if (this.angle) { // angle sucks, it only allows a maximum of 3M "vertices" to be drawn per call...
                    var dv = 0, vtd;
                    while ((vtd = Math.min(this.nElements - dv, 3000000)) > 0) { this.renderer.gl.drawElements(this.renderer.gl.TRIANGLES, vtd, this.renderer.gl.INDEXINT, dv * 4); dv += vtd; }
                }
                else this.renderer.gl.drawElements(this.renderer.gl.TRIANGLES, this.nElements, this.renderer.gl.INDEXINT, 0);

                this.renderer.gl.colorMask(true, true, true, true);
            }

            program.standard_render_core = function (modelViewMatrix, COR, i, opaqueMode) {
                i = i || 0;

                var normalMatrix = mat3.create();
                mat3.normalFromMat4(normalMatrix, modelViewMatrix);

                var shader = this.standard_shader;
                if (opaqueMode == 1) shader = this.standard_shader_opaque;
                else if (opaqueMode == 2) shader = this.standard_shader_transparent;

                this.renderer.gl.useProgram(shader.program);
                this.renderer.gl.uniformMatrix4fv(shader.uniforms.modelViewMatrix, false, modelViewMatrix);
                this.renderer.gl.uniformMatrix3fv(shader.uniforms.normalMatrix, false, normalMatrix);
                this.renderer.gl.uniformMatrix4fv(shader.uniforms.projectionMatrix, false, this.renderer.projectionMatrix);
                this.renderer.gl.uniform3f(shader.uniforms.COR, COR[0], COR[1], COR[2]);
                this.renderer.gl.uniform1f(shader.uniforms.focus, this.renderer.fogStart);
                this.renderer.gl.uniform1f(shader.uniforms.fogSpan, this.renderer.fogSpan);
                if (this.settings.rgba) {
                    if (this.settings.alphaMode) this.renderer.gl.uniform3f(shader.uniforms.uniform_color, this.settings.rgba[0] / 255, this.settings.rgba[1] / 255, this.settings.rgba[2] / 255, this.settings.rgba[3] / 255);
                    else this.renderer.gl.uniform3f(shader.uniforms.uniform_color, this.settings.rgba[0] / 255, this.settings.rgba[1] / 255, this.settings.rgba[2] / 255);
                }
                else if (this.settings.uniform_color) {
                    if (this.settings.alphaMode) this.renderer.gl.uniform3f(shader.uniforms.uniform_color, this.uniform_color[i][0] / 255, this.uniform_color[i][1] / 255, this.uniform_color[i][2] / 255, this.uniform_color[i][3] / 255);
                    else this.renderer.gl.uniform3f(shader.uniforms.uniform_color, this.uniform_color[i][0] / 255, this.uniform_color[i][1] / 255, this.uniform_color[i][2] / 255);
                }

                if (molmil.configBox.fogColor) this.renderer.gl.uniform4f(shader.uniforms.backgroundColor, molmil.configBox.fogColor[0], molmil.configBox.fogColor[1], molmil.configBox.fogColor[2], 1.0);
                else this.renderer.gl.uniform4f(shader.uniforms.backgroundColor, molmil.configBox.BGCOLOR[0], molmil.configBox.BGCOLOR[1], molmil.configBox.BGCOLOR[2], 1.0);
                if (this.settings.slab) {
                    if (this.settings.slabColor) { this.renderer.gl.uniform4f(shader.uniforms.slabColor, this.settings.slabColor[0], this.settings.slabColor[1], this.settings.slabColor[2], this.settings.slabColor[3]); }
                    this.renderer.gl.uniform1f(shader.uniforms.slabNear, -modelViewMatrix[14] + this.settings.slabNear - molmil.configBox.zNear);
                    this.renderer.gl.uniform1f(shader.uniforms.slabFar, -modelViewMatrix[14] + this.settings.slabFar - molmil.configBox.zNear);
                }
                else if (this.renderer.settings.slab) {
                    this.renderer.gl.uniform1f(shader.uniforms.slabNear, -modelViewMatrix[14] + this.renderer.settings.slabNear - molmil.configBox.zNear);
                    this.renderer.gl.uniform1f(shader.uniforms.slabFar, -modelViewMatrix[14] + this.renderer.settings.slabFar - molmil.configBox.zNear);
                }

                if ("alphaSet" in this.settings) {
                    this.renderer.gl.uniform1f(shader.uniforms.alpha, this.settings.alphaSet);
                }

                this.renderer.gl.bindBuffer(this.renderer.gl.ARRAY_BUFFER, this.vertexBuffer);

                molmil.resetAttributes(this.renderer.gl);
                if (this.settings.has_ID) {
                    molmil.bindAttribute(this.renderer.gl, this.standard_attributes.in_Position, 3, this.renderer.gl.FLOAT, false, 32, 0);
                    molmil.bindAttribute(this.renderer.gl, this.standard_attributes.in_Normal, 3, this.renderer.gl.FLOAT, false, 32, 12);
                    if (!this.settings.uniform_color) molmil.bindAttribute(this.renderer.gl, this.standard_attributes.in_Colour, 4, this.renderer.gl.UNSIGNED_BYTE, true, 32, 24);
                    //molmil.bindAttribute(this.renderer.gl, this.standard_attributes.in_ID, 1, this.renderer.gl.FLOAT, false, 32, 28);
                }
                else {
                    molmil.bindAttribute(this.renderer.gl, this.standard_attributes.in_Position, 3, this.renderer.gl.FLOAT, false, 28, 0);
                    molmil.bindAttribute(this.renderer.gl, this.standard_attributes.in_Normal, 3, this.renderer.gl.FLOAT, false, 28, 12);
                    if (!this.settings.uniform_color && !this.settings.rgba) molmil.bindAttribute(this.renderer.gl, this.standard_attributes.in_Colour, 4, this.renderer.gl.UNSIGNED_BYTE, true, 28, 24);
                }
                molmil.clearAttributes(this.renderer.gl);

                if (this.settings.alphaMode || "alphaSet" in this.settings) {
                    this.renderer.gl.enable(this.renderer.gl.BLEND);
                    this.renderer.gl.blendEquation(this.renderer.gl.FUNC_ADD);
                    this.renderer.gl.blendFunc(this.renderer.gl.SRC_ALPHA, this.renderer.gl.ONE_MINUS_SRC_ALPHA);
                }

                this.renderer.gl.bindBuffer(this.renderer.gl.ELEMENT_ARRAY_BUFFER, this.indexBuffer);

                if (this.angle) { // angle sucks, it only allows a maximum of 3M "vertices" to be drawn per call...
                    var dv = 0, vtd;
                    while ((vtd = Math.min(this.nElements - dv, 3000000)) > 0) { this.renderer.gl.drawElements(this.renderer.gl.TRIANGLES, vtd, this.renderer.gl.INDEXINT, dv * 4); dv += vtd; }
                }
                else this.renderer.gl.drawElements(this.renderer.gl.TRIANGLES, this.nElements, this.renderer.gl.INDEXINT, 0);
            }

            program.alphaMode_opaque_render = function (modelViewMatrix, COR, i) {
                if (!this.status) return;
                program.standard_render_core(modelViewMatrix, COR, i, 1); // opaque only
            };

            program.standard_render = function (modelViewMatrix, COR, i) {
                if (!this.status) return;
                if (this.settings.alphaSet) {
                    if (molmil.configBox.cullFace) { this.renderer.gl.disable(this.renderer.gl.CULL_FACE); }
                    this.alphaPre(modelViewMatrix, COR); // do pre thing
                    program.standard_render_core(modelViewMatrix, COR, i, 0);
                    if (molmil.configBox.cullFace) { this.renderer.gl.enable(this.renderer.gl.CULL_FACE); this.renderer.gl.cullFace(this.renderer.gl.BACK); }
                    this.renderer.gl.disable(this.renderer.gl.BLEND);
                }
                else if (this.settings.alphaMode && this.standard_shader_opaque) {
                    //program.standard_render_core(modelViewMatrix, COR, i, 1); // opaque only
                    if (molmil.configBox.cullFace) { this.renderer.gl.disable(this.renderer.gl.CULL_FACE); }
                    this.alphaPre(modelViewMatrix, COR); // do pre thing
                    program.standard_render_core(modelViewMatrix, COR, i, 2); // render transparent only
                    if (molmil.configBox.cullFace) { this.renderer.gl.enable(this.renderer.gl.CULL_FACE); this.renderer.gl.cullFace(this.renderer.gl.BACK); }
                    this.renderer.gl.disable(this.renderer.gl.BLEND);
                }
                else {
                    if (this.settings.disableCulling) this.renderer.gl.disable(this.renderer.gl.CULL_FACE);
                    program.standard_render_core(modelViewMatrix, COR, i, 0);
                    if (this.settings.disableCulling) { this.renderer.gl.enable(this.renderer.gl.CULL_FACE); this.renderer.gl.cullFace(this.renderer.gl.BACK); }
                }
            };

            if (!settings.multiMatrix) {
                if (settings.imposterPoints) program.render = program.point_render;
                else if (!settings.solid) program.render = program.wireframe_render;
                else program.render = program.standard_render;
            }
            else {
                program.render = function (modelViewMatrix, COR) {
                    var mat = mat4.create();
                    for (var i = 0; i < this.matrices.length; i++) {
                        mat4.multiply(mat, modelViewMatrix, this.matrices[i]);
                        this.render_internal(mat, COR, i);
                    }
                };
                if (!settings.solid) { program.render_internal = program.wireframe_render; program.shader = program.wireframe_shader; }
                else { program.render_internal = program.standard_render; program.shader = program.standard_shader; }
            }

            if (settings.has_ID) {
                if (settings.lines_render) {

                    program.pickingShader = renderer.shaders.linesPicking;
                    program.pickingAttributes = renderer.shaders.linesPicking.attributes;

                    program.renderPicking = function (modelViewMatrix, COR) {
                        if (!this.status || !this.vertexBuffer) return;
                        this.renderer.gl.useProgram(this.pickingShader.program);
                        this.renderer.gl.uniformMatrix4fv(this.pickingShader.uniforms.modelViewMatrix, false, modelViewMatrix);
                        this.renderer.gl.uniformMatrix4fv(this.pickingShader.uniforms.projectionMatrix, false, this.renderer.projectionMatrix);
                        this.renderer.gl.uniform3f(this.pickingShader.uniforms.COR, COR[0], COR[1], COR[2]);

                        this.renderer.gl.bindBuffer(this.renderer.gl.ARRAY_BUFFER, this.vertexBuffer);

                        molmil.resetAttributes(this.renderer.gl);
                        molmil.bindAttribute(this.renderer.gl, this.pickingAttributes.in_Position, 3, this.renderer.gl.FLOAT, false, 20, 0);
                        molmil.bindAttribute(this.renderer.gl, this.pickingAttributes.in_ID, 1, this.renderer.gl.FLOAT, false, 20, 16);
                        molmil.clearAttributes(this.renderer.gl);

                        this.renderer.gl.bindBuffer(this.renderer.gl.ELEMENT_ARRAY_BUFFER, this.indexBuffer);
                        if (this.angle) { // angle sucks, it only allows a maximum of 3M "vertices" to be drawn per call...
                            var dv = 0, vtd;
                            while ((vtd = Math.min(this.nElements - dv, 3000000)) > 0) { this.renderer.gl.drawElements(this.renderer.gl.POINTS, vtd, this.renderer.gl.INDEXINT, dv * 4); dv += vtd; }
                        }
                        else this.renderer.gl.drawElements(this.renderer.gl.POINTS, this.nElements, this.renderer.gl.INDEXINT, 0);
                    };
                }
                else {
                    program.pickingShader = renderer.shaders.picking;
                    program.pickingAttributes = renderer.shaders.picking.attributes;

                    program.renderPicking = function (modelViewMatrix, COR) {
                        if (!this.status || !this.vertexBuffer) return;
                        this.renderer.gl.useProgram(this.pickingShader.program);
                        this.renderer.gl.uniformMatrix4fv(this.pickingShader.uniforms.modelViewMatrix, false, modelViewMatrix);
                        this.renderer.gl.uniformMatrix4fv(this.pickingShader.uniforms.projectionMatrix, false, this.renderer.projectionMatrix);
                        this.renderer.gl.uniform3f(this.pickingShader.uniforms.COR, COR[0], COR[1], COR[2]);

                        this.renderer.gl.bindBuffer(this.renderer.gl.ARRAY_BUFFER, this.vertexBuffer);

                        molmil.resetAttributes(this.renderer.gl);
                        molmil.bindAttribute(this.renderer.gl, this.pickingAttributes.in_Position, 3, this.renderer.gl.FLOAT, false, 32, 0);
                        molmil.bindAttribute(this.renderer.gl, this.pickingAttributes.in_ID, 1, this.renderer.gl.FLOAT, false, 32, 28);
                        molmil.clearAttributes(this.renderer.gl);

                        this.renderer.gl.bindBuffer(this.renderer.gl.ELEMENT_ARRAY_BUFFER, this.indexBuffer);
                        if (this.angle) { // angle sucks, it only allows a maximum of 3M "vertices" to be drawn per call...
                            var dv = 0, vtd;
                            while ((vtd = Math.min(this.nElements - dv, 3000000)) > 0) { this.renderer.gl.drawElements(this.renderer.gl.TRIANGLES, vtd, this.renderer.gl.INDEXINT, dv * 4); dv += vtd; }
                        }
                        else this.renderer.gl.drawElements(this.renderer.gl.TRIANGLES, this.nElements, this.renderer.gl.INDEXINT, 0);
                    };
                }
            }
            else program.renderPicking = function () { };

            return program;
        };

        // ** creates and registers the programs within the renderer object **
        registerPrograms(renderer, initOnly) {
            if (!renderer.program1 || !renderer.gl.programInit) {
                renderer.program1 = this.build_simple_render_program(null, null, renderer, { has_ID: true, solid: true, alphaMode: this.buffer1 ? this.buffer1.alphaMode : false });
                renderer.addProgram(renderer.program1);
            }
            else renderer.program1.settings.alphaMode = this.buffer1 ? this.buffer1.alphaMode : false;
            if (!renderer.program2 || !renderer.gl.programInit) {
                renderer.program2 = this.build_simple_render_program(null, null, renderer, { has_ID: true, lines_render: true });
                renderer.addProgram(renderer.program2);
            }
            if (!renderer.program3 || !renderer.gl.programInit) {
                renderer.program3 = this.build_simple_render_program(null, null, renderer, { has_ID: true, solid: true, alphaMode: this.buffer3 ? this.buffer3.alphaMode : false });
                renderer.addProgram(renderer.program3);
            }
            else renderer.program3.settings.alphaMode = this.buffer3 ? this.buffer3.alphaMode : false;
            if (!renderer.program4 || this.buffer4.reinit || !renderer.gl.programInit) {
                if (renderer.program4) renderer.programs.splice(renderer.programs.indexOf(renderer.program4), 1);
                renderer.program4 = this.build_simple_render_program(null, null, renderer, { has_ID: false, solid: true, alphaMode: this.buffer4 ? this.buffer4.alphaMode : false });
                renderer.addProgram(renderer.program4);
                renderer.program4.vertexSize = 7;
            }

            if (molmil.configBox.EXT_frag_depth && molmil.configBox.imposterSpheres) {

                if (!renderer.program5 || !renderer.gl.programInit) {
                    renderer.program5 = this.build_simple_render_program(null, null, renderer, { has_ID: true, imposterPoints: true });
                    renderer.addProgram(renderer.program5);
                }
                renderer.program5.setBuffers(this.buffer5.vertexBuffer, this.buffer5.indexBuffer);
            }

            if (initOnly) return;

            renderer.program1.setBuffers(this.buffer1.vertexBuffer, this.buffer1.indexBuffer);
            renderer.program2.setBuffers(this.buffer2.vertexBuffer, this.buffer2.indexBuffer);
            renderer.program3.setBuffers(this.buffer3.vertexBuffer, this.buffer3.indexBuffer);
            renderer.program4.setBuffers(this.buffer4.vertexBuffer, this.buffer4.indexBuffer);

            if (molmil.configBox.liteMode) molmil.geometry.reset();

            renderer.gl.programInit = true;
        }

        // ** resets all geometry related buffered data **

        reset() {
            this.atoms2draw = [];
            this.xna2draw = [];
            this.wfatoms2draw = [];
            this.trace = [];
            this.bonds2draw = [];
            this.lines2draw = [];
            this.bondRef = {};

            this.buffer1 = { vP: 0, iP: 0, alphaMode: false }; // atoms & bonds
            this.buffer2 = { vP: 0, iP: 0 }; // wireframe
            this.buffer3 = { vP: 0, iP: 0 }; // cartoon
            this.buffer4 = { vP: 0, iP: 0 }; // surfaces
            this.buffer5 = { vP: 0, iP: 0 }; // points
        }

        // calculate normals for shapes...

        updateNormals(obj) {
            if (obj.normals) return;

            var vertices = obj.vertices, indices = obj.indices;

            if (obj.rgba2idxs) {
                var in_rgba1 = [], in_rgba2 = [], vmapping = {}, rgba2idxs = obj.rgba2idxs;
                for (i = 0; i < indices.length; i++) {
                    if (rgba2idxs.includes(i)) { in_rgba2.push(indices[i][0]); in_rgba2.push(indices[i][1]); in_rgba2.push(indices[i][2]); }
                    else { in_rgba1.push(indices[i][0]); in_rgba1.push(indices[i][1]); in_rgba1.push(indices[i][2]); }
                }

                for (i = 0; i < in_rgba2.length; i++) {
                    if (in_rgba1.includes(in_rgba2[i])) {
                        vmapping[in_rgba2[i]] = vertices.length;
                        vertices.push(vertices[in_rgba2[i]].slice());
                    }
                }
                var tmp = [];
                for (i = 0; i < indices.length; i++) {
                    if (rgba2idxs.includes(i)) {
                        if (indices[i][0] in vmapping) indices[i][0] = vmapping[indices[i][0]];
                        if (indices[i][1] in vmapping) indices[i][1] = vmapping[indices[i][1]];
                        if (indices[i][2] in vmapping) indices[i][2] = vmapping[indices[i][2]];
                    }
                    tmp.push(indices[i][0]);
                    tmp.push(indices[i][1]);
                    tmp.push(indices[i][2]);
                }

                obj.rgba2idxs = new Int8Array(vertices.length);

                for (i = 0; i < rgba2idxs.length; i++) {
                    obj.rgba2idxs[indices[rgba2idxs[i]][0]] = 1;
                    obj.rgba2idxs[indices[rgba2idxs[i]][1]] = 1;
                    obj.rgba2idxs[indices[rgba2idxs[i]][2]] = 1;
                }
            }
            else obj.rgba2idxs = new Int8Array(vertices.length);

            obj.nov = vertices.length; obj.noi = indices.length;
            obj.vertices = new Float32Array(vertices.length * 3);
            obj.normals = new Float32Array(vertices.length * 3);
            obj.indices = new Float32Array(indices.length * 3);

            var i, ii, p1 = vec3.create(), p2 = vec3.create(), a = vec3.create(), b = vec3.create(), c, face_normals = [], faceidxs = [], max_r = 0, r;
            for (i = 0; i < vertices.length; i++) {
                r = vec3.length(vertices[i]);
                if (r > max_r) max_r = r;
                faceidxs.push([]);
            }
            var sf = 0.5 / max_r;
            for (i = 0; i < vertices.length; i++) vec3.scale(vertices[i], vertices[i], sf);
            for (i = 0, ii = 0; i < indices.length; i++) {
                vec3.subtract(a, vertices[indices[i][0]], vertices[indices[i][1]]); vec3.normalize(a, a);
                vec3.subtract(b, vertices[indices[i][0]], vertices[indices[i][2]]); vec3.normalize(b, b);

                c = vec3.create();
                vec3.cross(c, a, b); vec3.normalize(c, c);
                face_normals.push(c);

                faceidxs[indices[i][0]].push(face_normals.length - 1);
                faceidxs[indices[i][1]].push(face_normals.length - 1);
                faceidxs[indices[i][2]].push(face_normals.length - 1);

                obj.indices[ii++] = indices[i][0]; obj.indices[ii++] = indices[i][1]; obj.indices[ii++] = indices[i][2];
            }

            var normal, f;
            for (i = 0, ii = 0; i < vertices.length; i++) {
                normal = [0, 0, 0];
                for (f = 0; f < faceidxs[i].length; f++) vec3.add(normal, normal, face_normals[faceidxs[i][f]]);
                vec3.normalize(normal, normal);
                obj.normals[ii] = normal[0]; obj.normals[ii + 1] = normal[1]; obj.normals[ii + 2] = normal[2];
                obj.vertices[ii++] = vertices[i][0]; obj.vertices[ii++] = vertices[i][1]; obj.vertices[ii++] = vertices[i][2];
            }
        };

        // ** calculates and initiates buffer sizes **
        initChains(chains, render, detail_or) {
            detail_or = detail_or || 0;

            var chain, a, b;

            var atoms2draw = this.atoms2draw; var wfatoms2draw = this.wfatoms2draw; var xna2draw = this.xna2draw;

            var bonds2draw = this.bonds2draw; var lines2draw = this.lines2draw; var bondRef = this.bondRef;
            var snfg_objs = this.snfg_objs = [], upvec_scan = {};

            var obj, Xpos, xyzRef, norm_upvec = vec3.create(), uvsi;
            var a2, a3, xyz = vec3.create(), xyz2 = vec3.create(), xyz3 = vec3.create(), v1 = vec3.create(), v2 = vec3.create(), v3 = vec3.create();

            var nor = 0, nob = 0, stat, snfg_nov = 0, snfg_noi = 0, snfg_nos = 0, snfg_nob = 0, modelId = this.modelId;

            // add code for SNFG...
            // fixed number of vertices, except for spheres

            for (var c = 0; c < chains.length; c++) {
                chain = chains[c]; stat = false;
                if (!chain.display) continue;

                for (var a = 0; a < chain.atoms.length; a++) {
                    if (chain.atoms[a].displayMode == 0 || !chain.atoms[a].display) continue; // don't display
                    else if (chain.atoms[a].displayMode == 4) wfatoms2draw.push(chain.atoms[a]); // wireframe (for wireframe use gl_lines)
                    else atoms2draw.push(chain.atoms[a]);
                    stat = true;
                }

                if (stat && !chain.bondsOK) render.soup.buildBondList(chain, false);

                for (var b = 0; b < chain.bonds.length; b++) {
                    if (chain.bonds[b][0].displayMode < 2 || chain.bonds[b][1].displayMode < 2 || !chain.bonds[b][0].display || !chain.bonds[b][1].display) continue;
                    if (chain.bonds[b][0].displayMode == 4 || chain.bonds[b][1].displayMode == 4) {
                        if (chain.bonds[b][0].displayMode != 4) wfatoms2draw.push(chain.bonds[b][0]);
                        if (chain.bonds[b][1].displayMode != 4) wfatoms2draw.push(chain.bonds[b][1]);
                        lines2draw.push(chain.bonds[b]);
                    }
                    else {
                        bonds2draw.push(chain.bonds[b]);
                        nob += chain.bonds[b][2] * 2;
                        if (!bondRef.hasOwnProperty(chain.bonds[b][0].AID)) bondRef[chain.bonds[b][0].AID] = [];
                        if (!bondRef.hasOwnProperty(chain.bonds[b][1].AID)) bondRef[chain.bonds[b][1].AID] = [];
                        bondRef[chain.bonds[b][0].AID].push(chain.bonds[b][1]);
                        bondRef[chain.bonds[b][1].AID].push(chain.bonds[b][0]);
                    }
                }

                if (chain.displayMode == 1) {
                    if (!chain.bondsOK) render.soup.buildBondList(chain, false);
                    for (var m = 0; m < chain.molecules.length; m++) {
                        if (chain.molecules[m].CA) {
                            if (chain.molecules[m].next && chain.molecules[m].next.displayMode == 1 && chain.molecules[m].next.CA) {
                                atoms2draw.push(chain.molecules[m].CA);
                                bonds2draw.push([chain.molecules[m].CA, chain.molecules[m].next.CA]);
                                nob += 2;
                            }
                            else if (chain.molecules[m].previous && chain.molecules[m].previous.displayMode == 1 && chain.molecules[m].previous.CA) atoms2draw.push(chain.molecules[m].CA);
                        }
                    }
                }

                if (chain.displayMode == 3 && chain.molecules.length && chain.molecules[0].xna) { // default?

                    for (var m = 0; m < chain.molecules.length; m++) {
                        if (chain.molecules[0].displayMode == 3 && chain.molecules[m].outer && chain.molecules[m].CA) xna2draw.push([chain.molecules[m].CA, chain.molecules[m].outer]);
                    }
                }

                if (chain.SNFG && (chain.displayMode == 3 || chain.displayMode == 4)) {
                    xyzRef = chain.modelsXYZ[modelId || 0];
                    for (var m = 0; m < chain.molecules.length; m++) {
                        if (!chain.molecules[m].SNFG) continue;
                        obj = { center: [0, 0, 0], id: snfg_objs.length };
                        chain.molecules[m].SNFG_obj = obj;

                        // also, see if we can calculate the backvector of the ring, then that should be used...
                        var rings = findResidueRings(chain.molecules[m]);
                        if (rings.length) {
                            rings = rings[0];
                            obj.backvec = vec3.create();
                            for (var a = 0, a2, a3; a < rings.length; a++) {
                                Xpos = rings[a].xyz;

                                obj.center[0] += xyzRef[Xpos];
                                obj.center[1] += xyzRef[Xpos + 1];
                                obj.center[2] += xyzRef[Xpos + 2];

                                a2 = a + 1; if (a2 >= rings.length) a2 -= rings.length;
                                a3 = a + 2; if (a3 >= rings.length) a3 -= rings.length;
                                Xpos2 = rings[a2].xyz;
                                Xpos3 = rings[a3].xyz;

                                xyz[0] = xyzRef[Xpos];
                                xyz[1] = xyzRef[Xpos + 1];
                                xyz[2] = xyzRef[Xpos + 2];

                                Xpos = rings[a2].xyz; xyz2[0] = xyzRef[Xpos]; xyz2[1] = xyzRef[Xpos + 1]; xyz2[2] = xyzRef[Xpos + 2];
                                Xpos = rings[a3].xyz; xyz3[0] = xyzRef[Xpos]; xyz3[1] = xyzRef[Xpos + 1]; xyz3[2] = xyzRef[Xpos + 2];

                                vec3.subtract(v1, xyz, xyz2);
                                vec3.subtract(v2, xyz, xyz3);
                                vec3.cross(v3, v1, v2);
                                if (a > 0 && vec3.dot(v3, obj.backvec) < 0) vec3.subtract(obj.backvec, obj.backvec, v3);
                                else vec3.add(obj.backvec, obj.backvec, v3);
                            }
                            vec3.normalize(obj.backvec, obj.backvec);
                            obj.center[0] /= rings.length;
                            obj.center[1] /= rings.length;
                            obj.center[2] /= rings.length;
                        }
                        else {
                            for (var a = 0; a < chain.molecules[m].atoms.length; a++) {
                                Xpos = chain.molecules[m].atoms[a].xyz;
                                obj.center[0] += xyzRef[Xpos];
                                obj.center[1] += xyzRef[Xpos + 1];
                                obj.center[2] += xyzRef[Xpos + 2];
                            }
                            obj.center[0] /= chain.molecules[m].atoms.length;
                            obj.center[1] /= chain.molecules[m].atoms.length;
                            obj.center[2] /= chain.molecules[m].atoms.length;
                        }
                        obj.mode = chain.displayMode;

                        obj.mesh = molmil.SNFG[chain.molecules[m].name] || molmil.SNFG.__UNKNOWN__;
                        if (obj.mesh.type == "sphere") snfg_nos++;
                        else if (obj.mesh.type in molmil.shapes3d) {
                            snfg_nov += molmil.shapes3d[obj.mesh.type].nov || molmil.shapes3d[obj.mesh.type].vertices.length;
                            snfg_noi += (molmil.shapes3d[obj.mesh.type].noi || molmil.shapes3d[obj.mesh.type].indices.length) * 3;
                        }
                        snfg_objs.push(obj);
                        upvec_scan[obj.id] = vec4.create();
                    }
                }
                if (chain.displayMode > 1 && (chain.displayMode != molmil.displayMode_ChainSurfaceCG || chain.displayMode != molmil.displayMode_ChainSurfaceSimple)) {
                    if (!chain.twoDcache || this.reInitChains) molmil.prepare2DRepr(chain, modelId || 0);
                    nor += chain.molecules.length;
                }
            }

            var assignSNFG_obj = function (mol) {
                mol.SNFG_obj = { center: [0, 0, 0] };
                xyzRef = mol.chain.modelsXYZ[modelId || 0];
                for (var a = 0; a < mol.atoms.length; a++) {
                    Xpos = mol.atoms[a].xyz;
                    mol.SNFG_obj.center[0] += xyzRef[Xpos];
                    mol.SNFG_obj.center[1] += xyzRef[Xpos + 1];
                    mol.SNFG_obj.center[2] += xyzRef[Xpos + 2];
                }
                mol.SNFG_obj.center[0] /= mol.atoms.length;
                mol.SNFG_obj.center[1] /= mol.atoms.length;
                mol.SNFG_obj.center[2] /= mol.atoms.length;

                var nearest = [1e9, -1], dx, dy, dz, r2;

                for (var a = 0; a < mol.atoms.length; a++) {
                    Xpos = mol.atoms[a].xyz;

                    dx = xyzRef[Xpos] - mol.SNFG_obj.center[0]; dy = xyzRef[Xpos + 1] - mol.SNFG_obj.center[1]; dz = xyzRef[Xpos + 2] - mol.SNFG_obj.center[2];
                    r2 = dx * dx + dy * dy + dz * dz;
                    if (r2 < nearest[0]) nearest = [r2, Xpos];
                }
                Xpos = nearest[1];
                mol.SNFG_obj.center[0] = xyzRef[Xpos];
                mol.SNFG_obj.center[1] = xyzRef[Xpos + 1];
                mol.SNFG_obj.center[2] = xyzRef[Xpos + 2];
            }

            for (var c = 0; c < chains.length; c++) {
                chain = chains[c];
                if (!chain.display) continue;
                if (chain.SNFG && (chain.displayMode == 3 || chain.displayMode == 4)) {
                    if (chain.displayMode == 3) snfg_nob += chain.branches.length;
                    for (var m = 0; m < chain.branches.length; m++) {
                        obj = { mesh: { type: "cylinder", rgba: [127, 127, 127, 255] }, radius: 0.25 };
                        obj.center = chain.branches[m][0].SNFG_obj.center;
                        // what to do if it's connected to a protein? -> get the Calpha instead...
                        if (chain.branches[m][0].CA) {
                            if (!chain.branches[m][1].SNFG_obj) assignSNFG_obj(chain.branches[m][1]);
                            xyzRef = chain.branches[m][0].chain.modelsXYZ[modelId || 0];
                            Xpos = chain.branches[m][0].CA.xyz;
                            xyz[0] = xyzRef[Xpos]; xyz[1] = xyzRef[Xpos + 1]; xyz[2] = xyzRef[Xpos + 2];
                            obj.backvec = [xyz[0] - chain.branches[m][1].SNFG_obj.center[0], xyz[1] - chain.branches[m][1].SNFG_obj.center[1], xyz - chain.branches[m][1].SNFG_obj.center[2]];
                        }
                        else if (chain.branches[m][1].CA) {
                            if (!chain.branches[m][0].SNFG_obj) assignSNFG_obj(chain.branches[m][0]);
                            xyzRef = chain.branches[m][1].chain.modelsXYZ[modelId || 0];
                            Xpos = chain.branches[m][1].CA.xyz;
                            xyz[0] = xyzRef[Xpos]; xyz[1] = xyzRef[Xpos + 1]; xyz[2] = xyzRef[Xpos + 2];
                            obj.backvec = [chain.branches[m][0].SNFG_obj.center[0] - xyz[0], chain.branches[m][0].SNFG_obj.center[1] - xyz[1], chain.branches[m][0].SNFG_obj.center[2] - xyz[2]];
                        }
                        else {
                            if (!chain.branches[m][0].SNFG_obj) assignSNFG_obj(chain.branches[m][0]);
                            if (!chain.branches[m][1].SNFG_obj) assignSNFG_obj(chain.branches[m][1]);
                            obj.backvec = [chain.branches[m][0].SNFG_obj.center[0] - chain.branches[m][1].SNFG_obj.center[0], chain.branches[m][0].SNFG_obj.center[1] - chain.branches[m][1].SNFG_obj.center[1], chain.branches[m][0].SNFG_obj.center[2] - chain.branches[m][1].SNFG_obj.center[2]];
                        }
                        obj.length = vec3.length(obj.backvec);
                        vec3.normalize(obj.backvec, obj.backvec);
                        if (chain.displayMode == 3) snfg_objs.push(obj);
                        if (chain.branches[m][0].SNFG) {
                            uvsi = upvec_scan[chain.branches[m][0].SNFG_obj.id];
                            if (uvsi[4] == 0) vec3.add(uvsi, uvsi, obj.backvec);
                            else {
                                if (vec3.dot(obj.backvec, uvsi) < 0) vec3.subtract(uvsi, uvsi, obj.backvec);
                                else vec3.add(uvsi, uvsi, obj.backvec);
                            }
                            uvsi[3]++;
                        }

                        if (chain.branches[m][1].SNFG) {
                            uvsi = upvec_scan[chain.branches[m][1].SNFG_obj.id];
                            if (uvsi) {
                                if (uvsi[4] == 0) vec3.add(uvsi, uvsi, obj.backvec);
                                else {
                                    if (vec3.dot(obj.backvec, uvsi) < 0) vec3.subtract(uvsi, uvsi, obj.backvec);
                                    else vec3.add(uvsi, uvsi, obj.backvec);
                                }
                                uvsi[3]++;
                            }
                        }
                    }

                    for (var m in upvec_scan) {
                        snfg_objs[m].sidevec = vec3.normalize(vec3.create(), upvec_scan[m]);
                        if (snfg_objs[m].backvec) {
                            vec3.cross(v1, snfg_objs[m].sidevec, snfg_objs[m].backvec);
                            vec3.cross(snfg_objs[m].sidevec, v1, snfg_objs[m].backvec);
                            delete snfg_objs[m].backvec;
                        }
                    }
                }
            }

            this.reInitChains = false;

            var detail_lv = molmil.configBox.QLV_SETTINGS[render.QLV].SPHERE_TESS_LV;
            var CB_NOI = molmil.configBox.QLV_SETTINGS[render.QLV].CB_NOI;
            var CB_NOVPR = molmil.configBox.QLV_SETTINGS[render.QLV].CB_NOVPR;

            if (molmil.configBox.EXT_frag_depth && molmil.configBox.imposterSpheres) {
                var vs = 0;
            }
            else {
                var vs = (atoms2draw.length + (xna2draw.length * 2)) * (6 * Math.pow(4, detail_lv + 1));
                vs += snfg_nos * 6 * Math.pow(4, detail_lv + 1);
            }

            vs += (bonds2draw.length + (xna2draw.length * 0.5)) * (detail_lv + 1) * 4 * 2;
            vs += nor * CB_NOI * CB_NOVPR;
            vs += snfg_nov + (snfg_nob * (detail_lv + 1) * 4);

            if (molmil.configBox.customDetailLV) detail_lv = this.detail_lv = molmil.configBox.customDetailLV(vs, detail_lv);
            else {
                if (vs > 1e7) detail_lv -= 1;
                if (vs > 3e7) detail_lv -= 1;
                if (vs > 1e8) detail_lv -= 1;
                if (vs < 2.5e5 && detail_lv < 3) detail_lv += 1;
                if (typeof molmil.configBox.strictDetailLV == "number" && detail_lv < molmil.configBox.strictDetailLV) detail_lv = molmil.configBox.strictDetailLV;
                else if (molmil.configBox.strictDetailLV == true) molmil.configBox.strictDetailLV = molmil.configBox.QLV_SETTINGS[render.QLV].SPHERE_TESS_LV;
                //detail_lv = 1;
                detail_lv = this.detail_lv = Math.max(detail_lv + detail_or, 0);
                if (molmil.configBox.liteMode && !molmil.configBox.strictDetailLV) detail_lv = this.detail_lv = 1;
            }

            // use a separate detail lv for atoms in case of < 250 atoms --> higher quality

            this.noi = molmil.configBox.QLV_SETTINGS[this.detail_lv].CB_NOI; // number of interpolation points per residue
            this.novpr = molmil.configBox.QLV_SETTINGS[this.detail_lv].CB_NOVPR;
            if (molmil.configBox.liteMode) this.noi = 1;

            var buffer1 = this.buffer1, buffer2 = this.buffer2;

            var vs = 0, is = 0;

            var nspheres = atoms2draw.length + xna2draw.length * 2 + snfg_nos;

            if (molmil.configBox.EXT_frag_depth && molmil.configBox.imposterSpheres) {
                this.buffer5.vertexBuffer = new Float32Array(nspheres * 4 * 7); // x, y, z, r, ss_offset (4x gl.BYTE), rgba, aid
                if (molmil.configBox.OES_element_index_uint) this.buffer5.indexBuffer = new Uint32Array(nspheres * 6);
                else this.buffer5.indexBuffer = new Uint16Array(nspheres * 4);
            }
            else {
                var sphere = this.getSphere(1, detail_lv);
                vs += (sphere.vertices.length / 3) * nspheres;
                is += sphere.indices.length * nspheres;
            }

            nob += xna2draw.length;
            nob += snfg_nob; // snfg branch connectors
            var cylinder = this.getCylinder(detail_lv);

            vs += (cylinder.vertices.length / 3) * nob;
            is += cylinder.indices.length * nob;

            // snfg objects (except spheres)
            vs += snfg_nov;
            is += snfg_noi;

            buffer1.vertexBuffer = new Float32Array(vs * 8); // x, y, z, nx, ny, nz, rgba, aid
            buffer1.vertexBuffer8 = new Uint8Array(buffer1.vertexBuffer.buffer);
            if (molmil.configBox.OES_element_index_uint) buffer1.indexBuffer = new Uint32Array(is);
            else buffer1.indexBuffer = new Uint16Array(is);

            buffer2.vertexBuffer = new Float32Array(wfatoms2draw.length * 5); // x, y, z, r, rgba, aid
            buffer2.vertexBuffer8 = new Uint8Array(buffer2.vertexBuffer.buffer);
            if (molmil.configBox.OES_element_index_uint) buffer2.indexBuffer = new Uint32Array(lines2draw.length * 2);
            else buffer2.indexBuffer = new Uint16Array(lines2draw.length * 2);
        };



        // ** builds a cartoon representation **
        initCartoon(chains) {
            var c, chain, b, vs = 0, is = 0, currentBlock, nor = 0, cartoonChains = this.cartoonChains = [], nowp, nowps = this.nowps = [];

            var noi = this.noi;
            var novpr = this.novpr;

            if (this.dome[2] != this.detail_lv) {
                var dome = this.dome = [molmil.buildOctaDome(molmil.configBox.QLV_SETTINGS[this.detail_lv].CB_DOME_TESS_LV, 0), molmil.buildOctaDome(molmil.configBox.QLV_SETTINGS[this.detail_lv].CB_DOME_TESS_LV, 1), this.detail_lv];
                var fixMat = mat4.create();
                mat4.rotateZ(fixMat, fixMat, (45 * Math.PI) / 180);
                for (var i = 0; i < this.dome[0].vertices.length; i++) {
                    vec3.transformMat4(this.dome[0].vertices[i], this.dome[0].vertices[i], fixMat);
                    vec3.transformMat4(this.dome[1].vertices[i], this.dome[1].vertices[i], fixMat);
                }
                var ringTemplate = this.ringTemplate = [];

                var nop = novpr, theta = 0.0, rad = 2.0 / nop, x, y, p, dij;
                for (p = 0; p < nop; p++) {
                    x = Math.cos(theta * Math.PI);
                    y = Math.sin(theta * Math.PI);
                    ringTemplate.push([x, y, 0]);
                    theta += rad;
                }

                this.squareVertices = [
                    [-1, 1, 0],
                    [1, 1, 0],
                    [1, -1, 0],
                    [-1, -1, 0],
                    [-1, 1, 0] // cheat
                ];

                this.squareVerticesN = [
                    [-1, 0, 0],
                    [0, 1, 0],
                    [1, 0, 0],
                    [0, -1, 0],
                    [-1, 0, 0] // cheat
                ];

                this.squareVerticesNhead = [
                    [-1, 0, 0],
                    [29 / 41, 29 / 41, 0],
                    [1, 0, 0],
                    [-29 / 41, -29 / 41, 0],
                    [-1, 0, 0] // cheat
                ];

            }
            else {
                var dome = this.dome;
            }

            for (c = 0; c < chains.length; c++) {

                chain = chains[c];
                if (chain.displayMode < 2 || chain.displayMode == molmil.displayMode_ChainSurfaceCG || chain.displayMode == molmil.displayMode_ChainSurfaceSimple || chain.SNFG) continue;
                nowp = 0;
                cartoonChains.push(chain);

                for (b = 0; b < chain.twoDcache.length; b++) {
                    currentBlock = chain.twoDcache[b];

                    if (chain.displayMode > 2 && currentBlock.sndStruc == 2) { // sheet

                        if (!currentBlock.isFirst) { // N-cap
                            vs += novpr;
                            is += novpr * 2;
                        }
                        vs += 4 + (currentBlock.molecules.length * 8 * (noi + 1)) + 8 + novpr; // open + body + arrow-heads'back + close
                        is += 2 + (currentBlock.molecules.length * 8 * (noi + 1)) + (novpr * 2);
                        if (!currentBlock.isLast) vs += novpr; // next loop start

                        nowp += currentBlock.molecules.length * (noi + 1);
                        if (currentBlock.isLast) nowp -= noi;
                    }
                    else if (currentBlock.rocket) { // cylinder
                        if (currentBlock.skip) continue;
                        // for cylinders, there are no molecules, but waypoints instead...

                        // something is still going wrong somewhere (maybe the connection points between cylinders and loops???)

                        vs += novpr + 1 + (currentBlock.waypoints.length * novpr) + novpr + 1;
                        is += novpr + (currentBlock.waypoints.length * novpr * 2) + novpr;

                        if (!currentBlock.isFirst) {
                            vs += novpr * (Math.round(noi * .5) + 1);
                            is += novpr * 2 * (Math.round(noi * .5) + 1);
                        }

                        if (!currentBlock.isLast) {
                            vs += novpr; // next loop start
                            vs += novpr * (Math.round(noi * .5) + 2);
                            is += novpr * 2 * (Math.round(noi * .5) + 2);
                        }

                        nowp += currentBlock.molecules.length * (noi + 1);

                    }
                    else { // helix/loop
                        if (currentBlock.isFirst) { vs += dome[0].vertices.length; is += dome[0].faces.length - (novpr * 2); } // N-term cap
                        if (currentBlock.isLast) { vs += dome[0].vertices.length + novpr; is += dome[0].faces.length + (novpr * 2); } // C-term cap
                        vs += currentBlock.molecules.length * novpr * (noi + 1);
                        is += currentBlock.molecules.length * novpr * 2 * (noi + 1);

                        nowp += currentBlock.molecules.length * (noi + 1);

                        if (currentBlock.isLast) {
                            nowp -= noi;
                            vs -= novpr * (noi + 1);
                            is -= novpr * 2 * (noi + 1);
                        }
                    }
                }
                nowps.push(nowp);
            }

            var buffer3 = this.buffer3;
            buffer3.alphaMode = false;
            buffer3.vertexBuffer = new Float32Array(vs * 8); // x, y, z, nx, ny, nz, rgba, aid
            buffer3.vertexBuffer8 = new Uint8Array(buffer3.vertexBuffer.buffer);
            if (molmil.configBox.OES_element_index_uint) buffer3.indexBuffer = new Uint32Array(is * 3);
            else buffer3.indexBuffer = new Uint16Array(is * 3);
        };

        generateAtomsImposters() {
            var atoms2draw = this.atoms2draw, xna2draw = this.xna2draw, vdwR = molmil.configBox.vdwR, r, sphere, a, v, rgba,
                vBuffer = this.buffer5.vertexBuffer, vP = this.buffer5.vP, iP = this.buffer5.iP,
                vP8 = vP * 4, vP16 = vP * 2;

            var vBuffer8 = new Uint8Array(this.buffer5.vertexBuffer.buffer);
            var vBuffer16 = new Uint16Array(this.buffer5.vertexBuffer.buffer);

            var iBuffer = this.buffer5.indexBuffer;

            var mdl = this.modelId || 0, tmp;

            var x, y, z, n;

            var ssOffset = [[-1, +1], [+1, +1], [+1, -1], [-1, -1]];

            var p = vP / 7;

            for (a = 0; a < atoms2draw.length; a++) {
                if (atoms2draw[a].displayMode == 1) r = molmil_dep.getKeyFromObject(vdwR, atoms2draw[a].element, vdwR.DUMMY) * molmil.configBox.vdwSphereMultiplier;
                else if (atoms2draw[a].displayMode == 2) r = .33;
                else r = atoms2draw[a].stickRadius || molmil.configBox.stickRadius;

                tmp = mdl;
                if (atoms2draw[a].chain.modelsXYZ.length <= mdl) mdl = 0;

                x = atoms2draw[a].chain.modelsXYZ[mdl][atoms2draw[a].xyz];
                y = atoms2draw[a].chain.modelsXYZ[mdl][atoms2draw[a].xyz + 1];
                z = atoms2draw[a].chain.modelsXYZ[mdl][atoms2draw[a].xyz + 2];
                rgba = atoms2draw[a].rgba;

                for (n = 0; n < 4; n++) {
                    vBuffer[vP++] = x;
                    vBuffer[vP++] = y;
                    vBuffer[vP++] = z;
                    vBuffer[vP++] = r;

                    vBuffer8[vP8 + 16] = rgba[0];
                    vBuffer8[vP8 + 17] = rgba[1];
                    vBuffer8[vP8 + 18] = rgba[2];
                    vBuffer8[vP8 + 19] = rgba[3];
                    vP++

                    vBuffer16[vP16 + 10] = ssOffset[n][0];
                    vBuffer16[vP16 + 11] = ssOffset[n][1];
                    vP++

                    vBuffer[vP++] = atoms2draw[a].AID; // ID
                    vP8 += 28; vP16 += 14;
                }

                iBuffer[iP++] = 3 + p;
                iBuffer[iP++] = 2 + p;
                iBuffer[iP++] = 1 + p;
                iBuffer[iP++] = 3 + p;
                iBuffer[iP++] = 1 + p;
                iBuffer[iP++] = 0 + p;
                p += 4;
            }

            var atomlist = [];
            for (a = 0; a < xna2draw.length; a++) { atomlist.push(xna2draw[a][0]); atomlist.push(xna2draw[a][1]); }

            for (a = 0; a < atomlist.length; a++) {
                if (atomlist[a].displayMode == 1) r = molmil_dep.getKeyFromObject(vdwR, atomlist[a].element, vdwR.DUMMY) * molmil.configBox.vdwSphereMultiplier;
                else if (atomlist[a].displayMode == 2) r = .33;
                else r = atomlist[a].stickRadius || molmil.configBox.stickRadius;

                tmp = mdl;
                if (atomlist[a].chain.modelsXYZ.length <= mdl) mdl = 0;

                x = atomlist[a].chain.modelsXYZ[mdl][atomlist[a].xyz];
                y = atomlist[a].chain.modelsXYZ[mdl][atomlist[a].xyz + 1];
                z = atomlist[a].chain.modelsXYZ[mdl][atomlist[a].xyz + 2];
                rgba = atomlist[a].molecule.rgba;

                for (n = 0; n < 4; n++) {
                    vBuffer[vP++] = x;
                    vBuffer[vP++] = y;
                    vBuffer[vP++] = z;
                    vBuffer[vP++] = r;

                    vBuffer8[vP8 + 16] = rgba[0];
                    vBuffer8[vP8 + 17] = rgba[1];
                    vBuffer8[vP8 + 18] = rgba[2];
                    vBuffer8[vP8 + 19] = rgba[3];
                    vP++

                    vBuffer16[vP16 + 10] = ssOffset[n][0];
                    vBuffer16[vP16 + 11] = ssOffset[n][1];
                    vP++

                    vBuffer[vP++] = atomlist[a].AID; // ID
                    vP8 += 28; vP16 += 14;
                }

                iBuffer[iP++] = 3 + p;
                iBuffer[iP++] = 2 + p;
                iBuffer[iP++] = 1 + p;
                iBuffer[iP++] = 3 + p;
                iBuffer[iP++] = 1 + p;
                iBuffer[iP++] = 0 + p;
                p += 4;
            }

            this.buffer5.vP = vP;
            this.buffer5.iP = iP;
        };

        // ** build atoms representation (spheres) **
        generateAtoms() {
            var atoms2draw = this.atoms2draw, xna2draw = this.xna2draw, vdwR = molmil.configBox.vdwR, r, sphere, a, v, rgba, vBuffer = this.buffer1.vertexBuffer, iBuffer = this.buffer1.indexBuffer, vP = this.buffer1.vP, iP = this.buffer1.iP, detail_lv = this.detail_lv,
                vBuffer8 = this.buffer1.vertexBuffer8, vP8 = vP * 4;

            var p = vP / 8;

            var mdl = this.modelId || 0, tmp;

            var x, y, z;

            sphere = this.getSphere(1.7, detail_lv);
            var nov = sphere.vertices.length / 3;

            for (a = 0; a < atoms2draw.length; a++) {
                if (atoms2draw[a].displayMode == 1) r = molmil_dep.getKeyFromObject(vdwR, atoms2draw[a].element, vdwR.DUMMY) * molmil.configBox.vdwSphereMultiplier;
                else if (atoms2draw[a].displayMode == 2) r = .33;
                else r = atoms2draw[a].stickRadius || molmil.configBox.stickRadius;

                sphere = this.getSphere(r, detail_lv);

                for (v = 0; v < sphere.indices.length; v++, iP++) iBuffer[iP] = sphere.indices[v] + p;

                tmp = mdl;
                if (atoms2draw[a].chain.modelsXYZ.length <= mdl) mdl = 0;

                x = atoms2draw[a].chain.modelsXYZ[mdl][atoms2draw[a].xyz];
                y = atoms2draw[a].chain.modelsXYZ[mdl][atoms2draw[a].xyz + 1];
                z = atoms2draw[a].chain.modelsXYZ[mdl][atoms2draw[a].xyz + 2];
                rgba = atoms2draw[a].rgba;
                if (rgba[3] != 255) this.buffer1.alphaMode = true;

                for (v = 0; v < sphere.vertices.length; v += 3, vP8 += 32) {
                    vBuffer[vP++] = sphere.vertices[v] + x;
                    vBuffer[vP++] = sphere.vertices[v + 1] + y;
                    vBuffer[vP++] = sphere.vertices[v + 2] + z;

                    vBuffer[vP++] = sphere.normals[v];
                    vBuffer[vP++] = sphere.normals[v + 1];
                    vBuffer[vP++] = sphere.normals[v + 2];

                    vBuffer8[vP8 + 24] = rgba[0];
                    vBuffer8[vP8 + 25] = rgba[1];
                    vBuffer8[vP8 + 26] = rgba[2];
                    vBuffer8[vP8 + 27] = rgba[3];
                    vP++

                    vBuffer[vP++] = atoms2draw[a].AID; // ID
                }
                p += nov;
            }

            var atomlist = [];
            for (a = 0; a < xna2draw.length; a++) { atomlist.push(xna2draw[a][0]); atomlist.push(xna2draw[a][1]); }

            for (a = 0; a < atomlist.length; a++) {
                if (atomlist[a].displayMode == 1) r = molmil_dep.getKeyFromObject(vdwR, atomlist[a].element, vdwR.DUMMY) * molmil.configBox.vdwSphereMultiplier;
                else if (atomlist[a].displayMode == 2) r = .33;
                else r = atomlist[a].stickRadius || molmil.configBox.stickRadius;

                sphere = this.getSphere(r, detail_lv);

                for (v = 0; v < sphere.indices.length; v++, iP++) iBuffer[iP] = sphere.indices[v] + p;

                tmp = mdl;
                if (atomlist[a].chain.modelsXYZ.length <= mdl) mdl = 0;

                x = atomlist[a].chain.modelsXYZ[mdl][atomlist[a].xyz];
                y = atomlist[a].chain.modelsXYZ[mdl][atomlist[a].xyz + 1];
                z = atomlist[a].chain.modelsXYZ[mdl][atomlist[a].xyz + 2];
                rgba = atomlist[a].molecule.rgba;

                for (v = 0; v < sphere.vertices.length; v += 3, vP8 += 32) {
                    vBuffer[vP++] = sphere.vertices[v] + x;
                    vBuffer[vP++] = sphere.vertices[v + 1] + y;
                    vBuffer[vP++] = sphere.vertices[v + 2] + z;

                    vBuffer[vP++] = sphere.normals[v];
                    vBuffer[vP++] = sphere.normals[v + 1];
                    vBuffer[vP++] = sphere.normals[v + 2];

                    vBuffer8[vP8 + 24] = rgba[0];
                    vBuffer8[vP8 + 25] = rgba[1];
                    vBuffer8[vP8 + 26] = rgba[2];
                    vBuffer8[vP8 + 27] = rgba[3];
                    vP++

                    vBuffer[vP++] = atomlist[a].AID; // ID
                }
                p += nov;
            }

            this.buffer1.vP = vP;
            this.buffer1.iP = iP;
        };

        // ** build bond representation (cylinders) **
        generateBonds() {
            var cylinder = this.getCylinder(this.detail_lv);

            var mdl = this.modelId || 0, tmp;
            var bonds2draw = this.bonds2draw,
                vBuffer = this.buffer1.vertexBuffer, iBuffer = this.buffer1.indexBuffer, vP = this.buffer1.vP, iP = this.buffer1.iP, detail_lv = this.detail_lv,
                vBuffer8 = this.buffer1.vertexBuffer8, vP8 = vP * 4;

            var nov = cylinder.vertices.length / 3;

            var r, offsetX, offsetY, offsetZ, v1 = [0, 0, 0], v2 = [0, 0, 0], c1 = [0, 0, 0];

            var p = vP / 8, x, y, z, x2, y2, z2, m, rgba, v;

            var rotationMatrix = mat4.create();
            var vertex = [0, 0, 0, 0], normal = [0, 0, 0, 0], dx, dy, dz, dij, angle;

            //bonds
            for (var b = 0; b < bonds2draw.length; b++) {
                m = bonds2draw[b][2] == 2 ? 4 : 2;

                tmp = mdl;
                if (bonds2draw[b][0].chain.modelsXYZ.length <= mdl) mdl = 0;

                x = bonds2draw[b][0].chain.modelsXYZ[mdl][bonds2draw[b][0].xyz];
                y = bonds2draw[b][0].chain.modelsXYZ[mdl][bonds2draw[b][0].xyz + 1];
                z = bonds2draw[b][0].chain.modelsXYZ[mdl][bonds2draw[b][0].xyz + 2];

                x2 = bonds2draw[b][1].chain.modelsXYZ[mdl][bonds2draw[b][1].xyz];
                y2 = bonds2draw[b][1].chain.modelsXYZ[mdl][bonds2draw[b][1].xyz + 1];
                z2 = bonds2draw[b][1].chain.modelsXYZ[mdl][bonds2draw[b][1].xyz + 2];

                dx = x - x2;
                dy = y - y2;
                dz = z - z2;
                dij = Math.sqrt((dx * dx) + (dy * dy) + (dz * dz));

                dx /= dij; dy /= dij; dz /= dij;
                angle = Math.acos(-dz);


                mat4.identity(rotationMatrix);
                mat4.rotate(rotationMatrix, rotationMatrix, angle, [dy, -dx, 0.0]);

                r = bonds2draw[b][0].stickRadius || molmil.configBox.stickRadius;
                offsetX = 0;
                offsetY = 0;
                offsetZ = 0;

                if (bonds2draw[b][2] == 2) {
                    r = .075;
                    offsetX = -.075;
                }
                rgba = bonds2draw[b][0].rgba;

                for (v = 0; v < cylinder.indices.length; v++, iP++) iBuffer[iP] = cylinder.indices[v] + p; // a2
                for (v = 0; v < cylinder.vertices.length; v += 3, vP8 += 32) {
                    vec3.transformMat4(vertex, [(cylinder.vertices[v] * r) + offsetX, (cylinder.vertices[v + 1] * r) + offsetY, (cylinder.vertices[v + 2] + offsetZ) * dij], rotationMatrix);
                    vec3.transformMat4(normal, [cylinder.normals[v], cylinder.normals[v + 1], cylinder.normals[v + 2]], rotationMatrix);

                    vBuffer[vP++] = vertex[0] + x;
                    vBuffer[vP++] = vertex[1] + y;
                    vBuffer[vP++] = vertex[2] + z;

                    vBuffer[vP++] = normal[0];
                    vBuffer[vP++] = normal[1];
                    vBuffer[vP++] = normal[2];

                    vBuffer8[vP8 + 24] = rgba[0];
                    vBuffer8[vP8 + 25] = rgba[1];
                    vBuffer8[vP8 + 26] = rgba[2];
                    vBuffer8[vP8 + 27] = rgba[3];
                    vP++

                    vBuffer[vP++] = 0; // ID
                }

                p += nov;

                offsetZ += .5;
                rgba = bonds2draw[b][1].rgba;


                for (v = 0; v < cylinder.indices.length; v++, iP++) iBuffer[iP] = cylinder.indices[v] + p; // a2

                for (v = 0; v < cylinder.vertices.length; v += 3, vP8 += 32) {
                    vec3.transformMat4(vertex, [(cylinder.vertices[v] * r) + offsetX, (cylinder.vertices[v + 1] * r) + offsetY, (cylinder.vertices[v + 2] + offsetZ) * dij], rotationMatrix);
                    vec3.transformMat4(normal, [cylinder.normals[v], cylinder.normals[v + 1], cylinder.normals[v + 2]], rotationMatrix);

                    vBuffer[vP++] = vertex[0] + x;
                    vBuffer[vP++] = vertex[1] + y;
                    vBuffer[vP++] = vertex[2] + z;

                    vBuffer[vP++] = normal[0];
                    vBuffer[vP++] = normal[1];
                    vBuffer[vP++] = normal[2];

                    vBuffer8[vP8 + 24] = rgba[0];
                    vBuffer8[vP8 + 25] = rgba[1];
                    vBuffer8[vP8 + 26] = rgba[2];
                    vBuffer8[vP8 + 27] = rgba[3];
                    vP++

                    vBuffer[vP++] = 0; // ID
                }

                p += nov;


                if (bonds2draw[b][2] == 2) {
                    r = .075;
                    offsetX = .075;
                    offsetZ = 0;
                    rgba = bonds2draw[b][0].rgba;

                    for (v = 0; v < cylinder.indices.length; v++, iP++) iBuffer[iP] = cylinder.indices[v] + p; // a2

                    for (v = 0; v < cylinder.vertices.length; v += 3, vP8 += 32) {
                        vec3.transformMat4(vertex, [(cylinder.vertices[v] * r) + offsetX, (cylinder.vertices[v + 1] * r) + offsetY, (cylinder.vertices[v + 2] + offsetZ) * dij], rotationMatrix);
                        vec3.transformMat4(normal, [cylinder.normals[v], cylinder.normals[v + 1], cylinder.normals[v + 2]], rotationMatrix);

                        vBuffer[vP++] = vertex[0] + x;
                        vBuffer[vP++] = vertex[1] + y;
                        vBuffer[vP++] = vertex[2] + z;

                        vBuffer[vP++] = normal[0];
                        vBuffer[vP++] = normal[1];
                        vBuffer[vP++] = normal[2];

                        vBuffer8[vP8 + 24] = rgba[0];
                        vBuffer8[vP8 + 25] = rgba[1];
                        vBuffer8[vP8 + 26] = rgba[2];
                        vBuffer8[vP8 + 27] = rgba[3];
                        vP++

                        vBuffer[vP++] = 0; // ID
                    }

                    p += nov;


                    offsetZ += .5;
                    rgba = bonds2draw[b][1].rgba;


                    for (v = 0; v < cylinder.indices.length; v++, iP++) iBuffer[iP] = cylinder.indices[v] + p; // a2

                    for (v = 0; v < cylinder.vertices.length; v += 3, vP8 += 32) {
                        vec3.transformMat4(vertex, [(cylinder.vertices[v] * r) + offsetX, (cylinder.vertices[v + 1] * r) + offsetY, (cylinder.vertices[v + 2] + offsetZ) * dij], rotationMatrix);
                        vec3.transformMat4(normal, [cylinder.normals[v], cylinder.normals[v + 1], cylinder.normals[v + 2]], rotationMatrix);

                        vBuffer[vP++] = vertex[0] + x;
                        vBuffer[vP++] = vertex[1] + y;
                        vBuffer[vP++] = vertex[2] + z;

                        vBuffer[vP++] = normal[0];
                        vBuffer[vP++] = normal[1];
                        vBuffer[vP++] = normal[2];

                        vBuffer8[vP8 + 24] = rgba[0];
                        vBuffer8[vP8 + 25] = rgba[1];
                        vBuffer8[vP8 + 26] = rgba[2];
                        vBuffer8[vP8 + 27] = rgba[3];
                        vP++

                        vBuffer[vP++] = 0; // ID
                    }

                    p += nov;

                }

            }

            var xna2draw = this.xna2draw;

            for (var b = 0; b < xna2draw.length; b++) {

                tmp = mdl;
                if (xna2draw[b][0].chain.modelsXYZ.length <= mdl) mdl = 0;

                x = xna2draw[b][0].chain.modelsXYZ[mdl][xna2draw[b][0].xyz];
                y = xna2draw[b][0].chain.modelsXYZ[mdl][xna2draw[b][0].xyz + 1];
                z = xna2draw[b][0].chain.modelsXYZ[mdl][xna2draw[b][0].xyz + 2];

                x2 = xna2draw[b][1].chain.modelsXYZ[mdl][xna2draw[b][1].xyz];
                y2 = xna2draw[b][1].chain.modelsXYZ[mdl][xna2draw[b][1].xyz + 1];
                z2 = xna2draw[b][1].chain.modelsXYZ[mdl][xna2draw[b][1].xyz + 2];

                dx = x - x2;
                dy = y - y2;
                dz = z - z2;
                dij = Math.sqrt((dx * dx) + (dy * dy) + (dz * dz));

                dx /= dij; dy /= dij; dz /= dij;
                angle = Math.acos(-dz);

                mat4.identity(rotationMatrix);
                mat4.rotate(rotationMatrix, rotationMatrix, angle, [dy, -dx, 0.0]);

                r = xna2draw[b][0].stickRadius || molmil.configBox.stickRadius;
                offsetX = 0;
                offsetY = 0;
                offsetZ = 0;

                if (xna2draw[b][2] == 2) {
                    r = .075;
                    offsetX = -.075;
                }
                rgba = xna2draw[b][0].molecule.rgba;

                for (v = 0; v < cylinder.indices.length; v++, iP++) iBuffer[iP] = cylinder.indices[v] + p; // a2
                for (v = 0; v < cylinder.vertices.length; v += 3, vP8 += 32) {
                    vec3.transformMat4(vertex, [(cylinder.vertices[v] * r) + offsetX, (cylinder.vertices[v + 1] * r) + offsetY, (cylinder.vertices[v + 2] + offsetZ) * dij * 2], rotationMatrix);
                    vec3.transformMat4(normal, [cylinder.normals[v], cylinder.normals[v + 1], cylinder.normals[v + 2]], rotationMatrix);

                    vBuffer[vP++] = vertex[0] + x;
                    vBuffer[vP++] = vertex[1] + y;
                    vBuffer[vP++] = vertex[2] + z;

                    vBuffer[vP++] = normal[0];
                    vBuffer[vP++] = normal[1];
                    vBuffer[vP++] = normal[2];

                    vBuffer8[vP8 + 24] = rgba[0];
                    vBuffer8[vP8 + 25] = rgba[1];
                    vBuffer8[vP8 + 26] = rgba[2];
                    vBuffer8[vP8 + 27] = rgba[3];
                    vP++

                    vBuffer[vP++] = 0; // ID
                }

                p += nov;
            }


            this.buffer1.vP = vP;
            this.buffer1.iP = iP;

        };

        // ** build wireframe representation **
        generateWireframe() {
            var wfatoms2draw = this.wfatoms2draw, lines2draw = this.lines2draw, a,
                vBuffer = this.buffer2.vertexBuffer, iBuffer = this.buffer2.indexBuffer, vP = this.buffer2.vP, iP = this.buffer2.iP,
                vBuffer8 = this.buffer2.vertexBuffer8, vP8 = vP * 4;

            var shf = 0.5;

            var p = vP / 5;
            var wfatomsRef = {};
            var mdl = this.modelId || 0;

            for (var a = 0; a < wfatoms2draw.length; a++, p++, vP8 += 20) {
                wfatomsRef[wfatoms2draw[a].AID] = p;
                vBuffer[vP++] = wfatoms2draw[a].chain.modelsXYZ[mdl][wfatoms2draw[a].xyz];
                vBuffer[vP++] = wfatoms2draw[a].chain.modelsXYZ[mdl][wfatoms2draw[a].xyz + 1];
                vBuffer[vP++] = wfatoms2draw[a].chain.modelsXYZ[mdl][wfatoms2draw[a].xyz + 2];

                vBuffer8[vP8 + 12] = wfatoms2draw[a].rgba[0];
                vBuffer8[vP8 + 13] = wfatoms2draw[a].rgba[1];
                vBuffer8[vP8 + 14] = wfatoms2draw[a].rgba[2];
                vBuffer8[vP8 + 16] = wfatoms2draw[a].rgba[3];
                vP++

                vBuffer[vP++] = wfatoms2draw[a].AID;
            }

            for (var b = 0; b < lines2draw.length; b++) {
                iBuffer[iP++] = wfatomsRef[lines2draw[b][0].AID];
                iBuffer[iP++] = wfatomsRef[lines2draw[b][1].AID];
            }

            this.buffer2.vP = vP;
            this.buffer2.iP = iP;
        };

        // ** build coarse surface representation **
        generateSurfaces(chains, soup) {
            var c, surf, surfaces = [], surfaces2 = [], verts = 0, idcs = 0, settings, alpha = false;

            for (c = 0; c < chains.length; c++) {
                if (chains[c].displayMode == molmil.displayMode_ChainSurfaceCG) {
                    if (chains[c].HQsurface) surf = molmil.coarseSurface(chains[c], molmil.configBox.HQsurface_gridSpacing, 1.4);
                    else surf = molmil.coarseSurface(chains[c], 7.5, 7.5 * .75, { deproj: true });
                    surf.rgba = chains[c].rgba;
                    surfaces.push(surf);
                    verts += surf.vertices.length;
                    idcs += surf.faces.length * 3;
                    alpha = alpha || surf.rgba[3] != 255;
                }
                else if (chains[c].displayMode == molmil.displayMode_ChainSurfaceSimple) {
                    settings = chains[c].displaySettings || {};
                    settings.skipProgram = true;
                    surf = molmil.tubeSurface(chains[c], settings, soup);
                    verts += surf.vBuffer.length;
                    idcs += surf.iBuffer.length;
                    surf.rgba = chains[c].rgba;
                    surfaces2.push(surf);
                    alpha = alpha || surf.rgba[3] != 255;
                }
            }

            var vertices = new Float32Array(verts * 7); // x, y, z, nx, ny, nz, rgba
            var vertices8 = new Uint8Array(vertices.buffer);
            var indices = new Uint32Array(idcs);
            var m = 0, m8 = 0, s, rgba, offset = 0, i = 0;
            for (s = 0; s < surfaces.length; s++) {
                surf = surfaces[s]; rgba = surf.rgba;
                for (c = 0; c < surf.vertices.length; c++, m8 += 28) {
                    vertices[m++] = surf.vertices[c][0];
                    vertices[m++] = surf.vertices[c][1];
                    vertices[m++] = surf.vertices[c][2];

                    vertices[m++] = surf.normals[c][0];
                    vertices[m++] = surf.normals[c][1];
                    vertices[m++] = surf.normals[c][2];


                    vertices8[m8 + 24] = rgba[0];
                    vertices8[m8 + 25] = rgba[1];
                    vertices8[m8 + 26] = rgba[2];
                    vertices8[m8 + 27] = rgba[3];
                    m++; // color
                }

                for (c = 0; c < surf.faces.length; c++) {
                    indices[i++] = surf.faces[c][0] + offset; indices[i++] = surf.faces[c][1] + offset; indices[i++] = surf.faces[c][2] + offset;
                }
                offset += surf.vertices.length;
            }



            for (s = 0; s < surfaces2.length; s++) {
                surf = surfaces2[s];

                vertices.set(surf.vBuffer, m);
                m += surf.vBuffer.length;
                m8 += surf.vBuffer.length * 4;

                for (c = 0; c < surf.iBuffer.length; c++) indices[i++] = surf.iBuffer[c] + offset;

                offset += surf.vBuffer.length / 7;
            }

            var buffer4 = this.buffer4;
            if (buffer4.alphaMode != alpha) buffer4.reinit = true;
            buffer4.alphaMode = alpha;
            buffer4.vertexBuffer = vertices;
            buffer4.vertexBuffer8 = vertices8;
            buffer4.indexBuffer = indices;
            buffer4.vertexSize = 7;
        }

        // ** build cartoon representation **
        generateCartoon() {
            var chains = this.cartoonChains, c, b, b2, m, chain, line, tangents, binormals, normals, rgba, aid, ref, i, TG, BN, N, t = 0, normal, binormal, vec = [0, 0, 0], delta = 0.0001, t_ranges, BNs, currentBlock;
            var noi = this.noi;
            var novpr = this.novpr;

            var nowp, wp, tmp, rotationMatrix, identityMatrix, theta, smallest;

            // in the future speed this up a bit by using a line[idx]/tangents[idx] instead of .push()

            var int_ratio = [];
            for (i = 0; i < noi + 1; i++) int_ratio.push(i / (noi + 1));
            for (c = 0; c < chains.length; c++) {
                nowp = this.nowps[c]; wp = 0;
                tangents = [], binormals = [], normals = [], rgba = new Array(nowp), aid = new Float32Array(nowp);
                chain = chains[c]; line = []; BNs = []; t_ranges = [];
                if (chain.displayMode == 0) continue;

                for (b = 0; b < chain.twoDcache.length; b++) {
                    t_ranges.push(line.length);
                    currentBlock = chain.twoDcache[b];
                    if (currentBlock.rocket) { // cylinder
                        if (currentBlock.skip) continue;

                        currentBlock.rocketPre = 0;
                        currentBlock.rocketPost = 0;

                        if (!currentBlock.isFirst) {
                            currentBlock.rocketPre = Math.round(noi * .5) + 2;
                            molmil.hermiteInterpolate(chain.twoDcache[b - 1].xyz[m], currentBlock.waypoints[0], chain.twoDcache[b - 1].tangents[m], currentBlock.waypoint_tangents[0], Math.round(noi * .5) + 1, line, tangents);
                            for (i = 0; i < Math.round(noi * .5) + 2; i++) {
                                rgba[wp] = currentBlock.molecules[0].rgba;
                                aid[wp] = 0;
                                wp++;
                            }
                        }

                        // add waypoints instead of CAs, also no interpolation
                        for (m = 0; m < currentBlock.waypoints.length; m++) {
                            rgba[wp] = currentBlock.molecules[0].rgba;
                            aid[wp] = 0;
                            wp++;
                            line.push(currentBlock.waypoints[m]);
                            tangents.push(currentBlock.waypoint_tangents[m]);
                        }

                        // for some reason this doesn't work properly...
                        if (!currentBlock.isLast) {
                            currentBlock.rocketPost = Math.round(noi * .5) + 2;

                            for (b2 = b + 1; b2 < chain.twoDcache.length; b2++) if (!chain.twoDcache[b2].skip) break;

                            tmp = tangents.length;

                            molmil.hermiteInterpolate(currentBlock.waypoints[m - 1], chain.twoDcache[b2].xyz[0], currentBlock.waypoint_tangents[m - 1], chain.twoDcache[b2].tangents[0], Math.round(noi * .5) + 1, line, tangents);

                            vec[0] = tangents[tmp][0] + tangents[tmp + 1][0]; vec[1] = tangents[tmp][1] + tangents[tmp + 1][1]; vec[2] = tangents[tmp][2] + tangents[tmp + 1][2];
                            vec3.normalize(vec, vec);
                            tangents[tmp][0] = vec[0]; tangents[tmp][1] = vec[1]; tangents[tmp][2] = vec[2];


                            for (i = 0; i < Math.round(noi * .5) + 2; i++) {
                                rgba[wp] = currentBlock.molecules[0].rgba;
                                aid[wp] = 0;
                                wp++;
                            }

                        }

                    }
                    else {
                        for (m = 0; m < currentBlock.molecules.length - 1; m++) {
                            molmil.hermiteInterpolate(currentBlock.xyz[m], currentBlock.xyz[m + 1], currentBlock.tangents[m], currentBlock.tangents[m + 1], noi, line, tangents);
                            for (i = 0; i < noi + 1; i++) {
                                rgba[wp] = currentBlock.molecules[m].rgba;
                                aid[wp] = currentBlock.molecules[m].CA.AID;
                                wp++;
                            }
                        }
                        if (!currentBlock.isLast) {
                            if (!chain.twoDcache[b + 1].rocket) {
                                molmil.hermiteInterpolate(currentBlock.xyz[m], chain.twoDcache[b + 1].xyz[0], currentBlock.tangents[m], currentBlock.tangents[m + 1], noi, line, tangents);
                                for (i = 0; i < noi + 1; i++) {
                                    rgba[wp] = currentBlock.molecules[m].rgba;
                                    aid[wp] = currentBlock.molecules[m].CA.AID;
                                    wp++;
                                }
                            }
                        }
                        else {
                            line.push([currentBlock.xyz[m][0], currentBlock.xyz[m][1], currentBlock.xyz[m][2]]);
                            if (tangents.length) tangents.push(tangents[wp - 1]);
                            else tangents.push([1, 0, 0]);
                            rgba[wp] = currentBlock.molecules[m].rgba;
                            aid[wp] = currentBlock.molecules[m].CA.AID;
                            wp++;
                        }
                    }
                }
                t_ranges.push(line.length);

                for (b = 0, t = 0; b < chain.twoDcache.length; b++) {
                    currentBlock = chain.twoDcache[b];

                    //if (chain.displayMode > 2 && (currentBlock.sndStruc == 2 || currentBlock.sndStruc == 3) && t > 0) { // the problem is here...
                    if (chain.displayMode > 2 && (currentBlock.sndStruc == 2 || currentBlock.sndStruc == 3) && !currentBlock.rocket) {
                        ref = null;
                        if (currentBlock.sndStruc == 2) {
                            ref = currentBlock.normals;
                            tmp = currentBlock.molecules.length - (currentBlock.isLast ? 1 : 0);
                            for (m = 0; m < tmp; m++) {
                                for (i = 0; i < noi + 1; i++, t++) {
                                    TG = tangents[t];
                                    N = vec3.lerp([0, 0, 0], ref[m], ref[m + 1], int_ratio[i]); vec3.normalize(N, N);
                                    vec3.scaleAndAdd(N, N, TG, -vec3.dot(N, TG) / vec3.dot(TG, TG)); vec3.normalize(N, N);
                                    normals.push(N);
                                    BN = vec3.cross([0, 0, 0], N, TG); vec3.normalize(BN, BN); vec3.negate(BN, BN); binormals.push(BN);
                                }
                            }
                            if (tmp != currentBlock.molecules.length) { // last one...
                                TG = tangents[t];
                                N = vec3.lerp([0, 0, 0], ref[m], ref[m + 1], int_ratio[i]); vec3.normalize(N, N);
                                vec3.scaleAndAdd(N, N, TG, -vec3.dot(N, TG) / vec3.dot(TG, TG)); vec3.normalize(N, N);
                                normals.push(N);
                                BN = vec3.cross([0, 0, 0], N, TG); vec3.normalize(BN, BN); vec3.negate(BN, BN); binormals.push(BN);
                                t++
                            }
                        }
                        else if (currentBlock.sndStruc == 3) {
                            ref = currentBlock.binormals;
                            if (!currentBlock.isFirst && !currentBlock.Nresume && rotationMatrix) {
                                vec3.cross(vec, tangents[t - 1], tangents[t]);
                                if (vec3.length(vec) > delta) {
                                    vec3.normalize(vec, vec);
                                    theta = Math.acos(Math.max(-1, Math.min(1, vec3.dot(tangents[t - 1], tangents[t]))));
                                    mat4.rotate(rotationMatrix, identityMatrix, theta, vec);
                                    vec3.transformMat4(normal, normal, rotationMatrix);
                                }
                                vec3.cross(binormal, tangents[t], normal);
                                ref[0] = [binormal[0], binormal[1], binormal[2]];

                                if (vec3.dot(ref[0], ref[1]) < 0) { // meant for loop-->helix transition
                                    tmp = currentBlock; i = 1;
                                    while (true) {
                                        for (m = i; m < tmp.molecules.length; m++) vec3.negate(tmp.binormals[m], tmp.binormals[m]);
                                        tmp.invertedBinormals = !tmp.invertedBinormals;

                                        tmp = tmp.nextBlock; i = 0;
                                        if (!tmp) break;
                                    }
                                }
                            }

                            tmp = currentBlock.molecules.length - (currentBlock.isLast ? 1 : 0);

                            for (m = 0; m < tmp; m++) {
                                for (i = 0; i < noi + 1; i++, t++) {
                                    TG = tangents[t];
                                    BN = vec3.lerp([0, 0, 0], ref[m], ref[m + 1], int_ratio[i]); vec3.normalize(BN, BN);
                                    vec3.scaleAndAdd(BN, BN, TG, -vec3.dot(BN, TG) / vec3.dot(TG, TG)); vec3.normalize(BN, BN);
                                    binormals.push(BN);
                                    N = vec3.cross([0, 0, 0], BN, TG); vec3.normalize(N, N); normals.push(N);
                                }
                            }
                            if (tmp != currentBlock.molecules.length) { // last one...
                                TG = tangents[t];
                                BN = [ref[m + 1][0], ref[m + 1][1], ref[m + 1][2]]; vec3.normalize(BN, BN);
                                vec3.scaleAndAdd(BN, BN, TG, -vec3.dot(BN, TG) / vec3.dot(TG, TG)); vec3.normalize(BN, BN);
                                binormals.push(BN);
                                N = vec3.cross([0, 0, 0], BN, TG); vec3.normalize(N, N); normals.push(N);
                                t++
                            }
                        }
                    }
                    else { // otherwise --> PTF
                        t = normals.length;

                        rotationMatrix = mat4.create(), identityMatrix = mat4.create();
                        if (t == 0) {
                            smallest = Number.MAX_VALUE;
                            if (tangents[0][0] <= smallest) { smallest = tangents[0][0]; normal = [1, 0, 0]; }
                            if (tangents[0][1] <= smallest) { smallest = tangents[0][1]; normal = [0, 1, 0]; }
                            if (tangents[0][2] <= smallest) { smallest = tangents[0][2]; normal = [0, 0, 1]; }
                            vec3.cross(vec, tangents[0], normal); vec3.normalize(vec, vec);
                            vec3.cross(normal, tangents[0], vec); vec3.cross(binormal = [0, 0, 0], tangents[0], normal);
                            normals.push([normal[0], normal[1], normal[2]]); binormals.push([binormal[0], binormal[1], binormal[2]]);
                            t++;
                        }
                        else { normal = [normals[t - 1][0], normals[t - 1][1], normals[t - 1][2]]; binormal = [binormals[t - 1][0], binormals[t - 1][1], binormals[t - 1][2]]; }
                        for (; t < t_ranges[b + 1]; t++) {
                            vec3.cross(vec, tangents[t - 1], tangents[t]);
                            if (vec3.length(vec) > delta) {
                                vec3.normalize(vec, vec);
                                theta = Math.acos(Math.max(-1, Math.min(1, vec3.dot(tangents[t - 1], tangents[t]))));
                                mat4.rotate(rotationMatrix, identityMatrix, theta, vec);
                                vec3.transformMat4(normal, normal, rotationMatrix);
                            }
                            vec3.cross(binormal, tangents[t], normal);
                            normals.push([normal[0], normal[1], normal[2]]); binormals.push([binormal[0], binormal[1], binormal[2]]);
                        }
                    }
                }

                normals.push(normals[normals.length - 1]);
                binormals.push(binormals[binormals.length - 1]);

                t = 0;

                for (b = 0; b < chain.twoDcache.length; b++) {
                    currentBlock = chain.twoDcache[b];
                    if (chain.displayMode > 2) {
                        if (currentBlock.sndStruc == 2) {
                            t = this.buildSheet(t_ranges[b], t_ranges[b + 1], line, tangents, normals, binormals, rgba, aid, currentBlock.isFirst, currentBlock.isLast, chain.cartoonRadius || this.radius); // bad
                            continue;
                        }
                        else if (currentBlock.rocket) {
                            if (currentBlock.skip) continue;


                            t = this.buildLoop(t_ranges[b], t_ranges[b] + currentBlock.rocketPre + (currentBlock.rocketPre ? 1 : 0), line, tangents, normals, binormals, rgba, aid, chain.cartoonRadius || this.radius);
                            t = this.buildRocket(t_ranges[b] + currentBlock.rocketPre, t_ranges[b + 1] - currentBlock.rocketPost, line, tangents, normals, binormals, rgba, aid, currentBlock.isLast, chain.cartoonRadius || this.radius);
                            t = this.buildLoop(t_ranges[b + 1] - currentBlock.rocketPost, t_ranges[b + 1], line, tangents, normals, binormals, rgba, aid, chain.cartoonRadius || this.radius);

                            continue;
                        }
                        else if (currentBlock.sndStruc == 3) { // caps also need to be added...
                            if (currentBlock.isFirst) {
                                this.buildLoopNcap(t_ranges[b], line, tangents, normals, binormals, rgba, aid, chain.cartoonRadius || this.radius); t++;
                                t_ranges[b] += 1;
                            }
                            t = this.buildHelix(t_ranges[b], t_ranges[b + 1], line, tangents, normals, binormals, rgba, aid, currentBlock, chain.cartoonRadius || this.radius);
                            if (currentBlock.isLast) {
                                this.buildLoopCcap(t_ranges[b + 1] - 1, line, tangents, normals, binormals, rgba, aid, chain.cartoonRadius || this.radius);
                            }
                            continue;
                        }
                    }

                    if (currentBlock.isFirst) {
                        this.buildLoopNcap(t_ranges[b], line, tangents, normals, binormals, rgba, aid, chain.cartoonRadius || this.radius); t++;
                        t_ranges[b] += 1;
                    }

                    t = this.buildLoop(t_ranges[b], t_ranges[b + 1], line, tangents, normals, binormals, rgba, aid, chain.cartoonRadius || this.radius);

                    if (currentBlock.isLast) {
                        this.buildLoopCcap(t_ranges[b + 1] - 1, line, tangents, normals, binormals, rgba, aid, chain.cartoonRadius || this.radius);
                    }
                }
            }
        };

        // ** n-terminal loop (cap) **
        buildLoopNcap(t, P, T, N, B, rgba, aid, coreRadius) {
            var dome = this.dome[0], radius = coreRadius, i, ringTemplate = this.ringTemplate;


            var vBuffer = this.buffer3.vertexBuffer, iBuffer = this.buffer3.indexBuffer, vP = this.buffer3.vP, iP = this.buffer3.iP, Px, Py, Pz, Nx, Ny, Nz, Bx, By, Bz, Tx, Ty, Tz, rgba_, aid_,
                vBuffer8 = this.buffer3.vertexBuffer8, vP8 = vP * 4;
            var p = vP * .125;

            Px = P[t][0], Py = P[t][1], Pz = P[t][2], Tx = T[t][0], Ty = T[t][1], Tz = T[t][2], Nx = N[t][0], Ny = N[t][1], Nz = N[t][2], Bx = B[t][0], By = B[t][1], Bz = B[t][2], rgba_ = rgba[t], aid_ = aid[t];
            if (rgba_[3] != 255) this.buffer3.alphaMode = true;
            for (i = 0; i < dome.vertices.length; i++, vP8 += 32) {
                vBuffer[vP++] = radius * dome.vertices[i][0] * Nx + radius * dome.vertices[i][1] * Bx + radius * dome.vertices[i][2] * Tx + Px;
                vBuffer[vP++] = radius * dome.vertices[i][0] * Ny + radius * dome.vertices[i][1] * By + radius * dome.vertices[i][2] * Ty + Py;
                vBuffer[vP++] = radius * dome.vertices[i][0] * Nz + radius * dome.vertices[i][1] * Bz + radius * dome.vertices[i][2] * Tz + Pz;

                vBuffer[vP++] = dome.vertices[i][0] * Nx + dome.vertices[i][1] * Bx + dome.vertices[i][2] * Tx;
                vBuffer[vP++] = dome.vertices[i][0] * Ny + dome.vertices[i][1] * By + dome.vertices[i][2] * Ty;
                vBuffer[vP++] = dome.vertices[i][0] * Nz + dome.vertices[i][1] * Bz + dome.vertices[i][2] * Tz;

                vBuffer8[vP8 + 24] = rgba_[0];
                vBuffer8[vP8 + 25] = rgba_[1];
                vBuffer8[vP8 + 26] = rgba_[2];
                vBuffer8[vP8 + 27] = rgba_[3];
                vP++;

                vBuffer[vP++] = aid[t]; // ID
            }

            for (i = 0; i < dome.faces.length; i++) {
                iBuffer[iP++] = dome.faces[i][0] + p;
                iBuffer[iP++] = dome.faces[i][1] + p;
                iBuffer[iP++] = dome.faces[i][2] + p;
            }

            for (i = 0; i < ringTemplate.length; i++, vP8 += 32) {
                vBuffer[vP++] = radius * ringTemplate[i][0] * Nx + radius * ringTemplate[i][1] * Bx + Px;
                vBuffer[vP++] = radius * ringTemplate[i][0] * Ny + radius * ringTemplate[i][1] * By + Py;
                vBuffer[vP++] = radius * ringTemplate[i][0] * Nz + radius * ringTemplate[i][1] * Bz + Pz;

                vBuffer[vP++] = ringTemplate[i][0] * Nx + ringTemplate[i][1] * Bx;
                vBuffer[vP++] = ringTemplate[i][0] * Ny + ringTemplate[i][1] * By;
                vBuffer[vP++] = ringTemplate[i][0] * Nz + ringTemplate[i][1] * Bz;

                vBuffer8[vP8 + 24] = rgba_[0];
                vBuffer8[vP8 + 25] = rgba_[1];
                vBuffer8[vP8 + 26] = rgba_[2];
                vBuffer8[vP8 + 27] = rgba_[3];
                vP++;

                vBuffer[vP++] = aid[t]; // ID
            }

            this.buffer3.vP = vP;
            this.buffer3.iP = iP;
        };

        // ** c-terminal loop (cap) **
        buildLoopCcap(t, P, T, N, B, rgba, aid, coreRadius) {
            var dome = this.dome[1], radius = coreRadius, i, ringTemplate = this.ringTemplate;

            var vBuffer = this.buffer3.vertexBuffer, iBuffer = this.buffer3.indexBuffer, vP = this.buffer3.vP, iP = this.buffer3.iP, Px, Py, Pz, Nx, Ny, Nz, Bx, By, Bz, Tx, Ty, Tz, rgba_, aid_,
                vBuffer8 = this.buffer3.vertexBuffer8, vP8 = vP * 4;
            var p = vP * .125;

            Px = P[t][0], Py = P[t][1], Pz = P[t][2], Tx = T[t][0], Ty = T[t][1], Tz = T[t][2], Nx = N[t][0], Ny = N[t][1], Nz = N[t][2], Bx = B[t][0], By = B[t][1], Bz = B[t][2], rgba_ = rgba[t], aid_ = aid[t];
            if (rgba_[3] != 255) this.buffer3.alphaMode = true;
            for (i = 0; i < dome.vertices.length; i++, vP8 += 32) {
                vBuffer[vP++] = radius * dome.vertices[i][0] * Nx + radius * dome.vertices[i][1] * Bx + radius * dome.vertices[i][2] * Tx + Px;
                vBuffer[vP++] = radius * dome.vertices[i][0] * Ny + radius * dome.vertices[i][1] * By + radius * dome.vertices[i][2] * Ty + Py;
                vBuffer[vP++] = radius * dome.vertices[i][0] * Nz + radius * dome.vertices[i][1] * Bz + radius * dome.vertices[i][2] * Tz + Pz;

                vBuffer[vP++] = dome.vertices[i][0] * Nx + dome.vertices[i][1] * Bx + dome.vertices[i][2] * Tx;
                vBuffer[vP++] = dome.vertices[i][0] * Ny + dome.vertices[i][1] * By + dome.vertices[i][2] * Ty;
                vBuffer[vP++] = dome.vertices[i][0] * Nz + dome.vertices[i][1] * Bz + dome.vertices[i][2] * Tz;

                vBuffer8[vP8 + 24] = rgba_[0];
                vBuffer8[vP8 + 25] = rgba_[1];
                vBuffer8[vP8 + 26] = rgba_[2];
                vBuffer8[vP8 + 27] = rgba_[3];
                vP++;

                vBuffer[vP++] = aid_; // ID
            }

            for (i = 0; i < dome.faces.length; i++) {
                iBuffer[iP++] = dome.faces[i][0] + p;
                iBuffer[iP++] = dome.faces[i][1] + p;
                iBuffer[iP++] = dome.faces[i][2] + p;
            }

            this.buffer3.vP = vP;
            this.buffer3.iP = iP;
        };

        // ** build loop representation **
        buildLoop(t, t_next, P, T, N, B, rgba, aid, coreRadius) {
            var dome = this.dome[0], radius = coreRadius, i, novpr = this.novpr;

            var ringTemplate = this.ringTemplate, radius = coreRadius, Px, Py, Pz, Nx, Ny, Nz, Bx, By, Bz, Tx, Ty, Tz, rgba_, aid_;

            var vBuffer = this.buffer3.vertexBuffer, iBuffer = this.buffer3.indexBuffer, vP = this.buffer3.vP, iP = this.buffer3.iP,
                vBuffer8 = this.buffer3.vertexBuffer8, vP8 = vP * 4;
            var p = vP * .125;
            var p_pre = p - novpr;

            for (t; t < t_next; t++) {
                Px = P[t][0], Py = P[t][1], Pz = P[t][2], Tx = T[t][0], Ty = T[t][1], Tz = T[t][2], Nx = N[t][0], Ny = N[t][1], Nz = N[t][2], Bx = B[t][0], By = B[t][1], Bz = B[t][2], rgba_ = rgba[t], aid_ = aid[t];
                if (rgba_[3] != 255) this.buffer3.alphaMode = true;
                for (i = 0; i < ringTemplate.length; i++, vP8 += 32) {
                    vBuffer[vP++] = radius * ringTemplate[i][0] * Nx + radius * ringTemplate[i][1] * Bx + Px;
                    vBuffer[vP++] = radius * ringTemplate[i][0] * Ny + radius * ringTemplate[i][1] * By + Py;
                    vBuffer[vP++] = radius * ringTemplate[i][0] * Nz + radius * ringTemplate[i][1] * Bz + Pz;

                    vBuffer[vP++] = ringTemplate[i][0] * Nx + ringTemplate[i][1] * Bx;
                    vBuffer[vP++] = ringTemplate[i][0] * Ny + ringTemplate[i][1] * By;
                    vBuffer[vP++] = ringTemplate[i][0] * Nz + ringTemplate[i][1] * Bz;

                    vBuffer8[vP8 + 24] = rgba_[0];
                    vBuffer8[vP8 + 25] = rgba_[1];
                    vBuffer8[vP8 + 26] = rgba_[2];
                    vBuffer8[vP8 + 27] = rgba_[3];
                    vP++;

                    vBuffer[vP++] = aid_; // ID
                }

                for (i = 0; i < (novpr * 2) - 2; i += 2, p += 1, p_pre += 1) {
                    iBuffer[iP++] = p;
                    iBuffer[iP++] = p_pre;
                    iBuffer[iP++] = p_pre + 1;

                    iBuffer[iP++] = p_pre + 1;
                    iBuffer[iP++] = p + 1;
                    iBuffer[iP++] = p;
                }
                iBuffer[iP++] = p;
                iBuffer[iP++] = p_pre;
                iBuffer[iP++] = p_pre - (novpr - 1);

                iBuffer[iP++] = p_pre - (novpr - 1);
                iBuffer[iP++] = p - (novpr - 1);
                iBuffer[iP++] = p;

                p++; p_pre++;
            }

            this.buffer3.vP = vP;
            this.buffer3.iP = iP;
            return t;
        };


        // note that everything has been optimized for an alpha helix...
        // how does this work for other helices???
        buildRocket(t, t_next, P, T, N, B, rgba, aid, isLast, coreRadius) {
            var radius = 1.15, i, novpr = this.novpr, go = false;
            var ringTemplate = this.ringTemplate, Px, Py, Pz, Nx, Ny, Nz, Bx, By, Bz, Tx, Ty, Tz, rgba_, aid_;

            var vBuffer = this.buffer3.vertexBuffer, iBuffer = this.buffer3.indexBuffer, vP = this.buffer3.vP, iP = this.buffer3.iP,
                vBuffer8 = this.buffer3.vertexBuffer8, vP8 = vP * 4;
            var p = vP * .125;
            var p_pre = p - novpr;

            var before = vP;

            // N-terminal cap: flat (1-ring + 1 vertices)

            Px = P[t][0], Py = P[t][1], Pz = P[t][2], Tx = T[t][0], Ty = T[t][1], Tz = T[t][2], Nx = N[t][0], Ny = N[t][1], Nz = N[t][2], Bx = B[t][0], By = B[t][1], Bz = B[t][2], rgba_ = rgba[t], aid_ = aid[t];
            if (rgba_[3] != 255) this.buffer3.alphaMode = true;

            vBuffer[vP++] = Px;
            vBuffer[vP++] = Py;
            vBuffer[vP++] = Pz;

            vBuffer[vP++] = -Tx;
            vBuffer[vP++] = -Ty;
            vBuffer[vP++] = -Tz;

            vBuffer8[vP8 + 24] = rgba_[0];
            vBuffer8[vP8 + 25] = rgba_[1];
            vBuffer8[vP8 + 26] = rgba_[2];
            vBuffer8[vP8 + 27] = rgba_[3];
            vP++;

            vBuffer[vP++] = aid_; // ID
            vP8 += 32;

            for (i = 0; i < ringTemplate.length; i++, vP8 += 32) {
                vBuffer[vP++] = radius * ringTemplate[i][0] * Nx + radius * ringTemplate[i][1] * Bx + Px;
                vBuffer[vP++] = radius * ringTemplate[i][0] * Ny + radius * ringTemplate[i][1] * By + Py;
                vBuffer[vP++] = radius * ringTemplate[i][0] * Nz + radius * ringTemplate[i][1] * Bz + Pz;

                vBuffer[vP++] = -Tx;
                vBuffer[vP++] = -Ty;
                vBuffer[vP++] = -Tz;

                vBuffer8[vP8 + 24] = rgba_[0];
                vBuffer8[vP8 + 25] = rgba_[1];
                vBuffer8[vP8 + 26] = rgba_[2];
                vBuffer8[vP8 + 27] = rgba_[3];
                vP++;

                vBuffer[vP++] = aid_; // ID
            }

            for (i = 0; i < ringTemplate.length; i++) {
                iBuffer[iP++] = p + i + 2;
                iBuffer[iP++] = p + i + 1;
                iBuffer[iP++] = p;
            }
            iBuffer[iP - 3] = p + 1;

            p += ringTemplate.length + 1;
            p_pre = p - novpr;

            // center region (cylinder)

            for (t; t < t_next; t++) {
                Px = P[t][0], Py = P[t][1], Pz = P[t][2], Tx = T[t][0], Ty = T[t][1], Tz = T[t][2], Nx = N[t][0], Ny = N[t][1], Nz = N[t][2], Bx = B[t][0], By = B[t][1], Bz = B[t][2], rgba_ = rgba[t], aid_ = aid[t];
                if (rgba_[3] != 255) this.buffer3.alphaMode = true;
                for (i = 0; i < ringTemplate.length; i++, vP8 += 32) {
                    vBuffer[vP++] = radius * ringTemplate[i][0] * Nx + radius * ringTemplate[i][1] * Bx + Px;
                    vBuffer[vP++] = radius * ringTemplate[i][0] * Ny + radius * ringTemplate[i][1] * By + Py;
                    vBuffer[vP++] = radius * ringTemplate[i][0] * Nz + radius * ringTemplate[i][1] * Bz + Pz;

                    vBuffer[vP++] = ringTemplate[i][0] * Nx + ringTemplate[i][1] * Bx;
                    vBuffer[vP++] = ringTemplate[i][0] * Ny + ringTemplate[i][1] * By;
                    vBuffer[vP++] = ringTemplate[i][0] * Nz + ringTemplate[i][1] * Bz;

                    vBuffer8[vP8 + 24] = rgba_[0];
                    vBuffer8[vP8 + 25] = rgba_[1];
                    vBuffer8[vP8 + 26] = rgba_[2];
                    vBuffer8[vP8 + 27] = rgba_[3];
                    vP++;

                    vBuffer[vP++] = aid_; // ID
                }

                for (i = 0; i < (novpr * 2) - 2; i += 2, p += 1, p_pre += 1) {
                    iBuffer[iP++] = p + novpr;
                    iBuffer[iP++] = p_pre + novpr;
                    iBuffer[iP++] = p_pre + 1 + novpr;

                    iBuffer[iP++] = p_pre + 1 + novpr;
                    iBuffer[iP++] = p + 1 + novpr;
                    iBuffer[iP++] = p + novpr;
                }
                iBuffer[iP++] = p + novpr;
                iBuffer[iP++] = p_pre + novpr;
                iBuffer[iP++] = p_pre - (novpr - 1) + novpr;

                iBuffer[iP++] = p_pre - (novpr - 1) + novpr;
                iBuffer[iP++] = p - (novpr - 1) + novpr;
                iBuffer[iP++] = p + novpr;

                p++; p_pre++;
            }

            for (i = vP - (ringTemplate.length * 8); i < vP; i += 8) {
                vBuffer[i] -= Tx * 2;
                vBuffer[i + 1] -= Ty * 2;
                vBuffer[i + 2] -= Tz * 2;
            }


            for (i = 0; i < ringTemplate.length; i++, vP8 += 32) {
                vBuffer[vP++] = radius * ringTemplate[i][0] * Nx + radius * ringTemplate[i][1] * Bx + Px - Tx * 2;
                vBuffer[vP++] = radius * ringTemplate[i][0] * Ny + radius * ringTemplate[i][1] * By + Py - Ty * 2;
                vBuffer[vP++] = radius * ringTemplate[i][0] * Nz + radius * ringTemplate[i][1] * Bz + Pz - Tz * 2;

                vBuffer[vP++] = ringTemplate[i][0] * Nx + ringTemplate[i][1] * Bx;
                vBuffer[vP++] = ringTemplate[i][0] * Ny + ringTemplate[i][1] * By;
                vBuffer[vP++] = ringTemplate[i][0] * Nz + ringTemplate[i][1] * Bz;

                vBuffer8[vP8 + 24] = rgba_[0];
                vBuffer8[vP8 + 25] = rgba_[1];
                vBuffer8[vP8 + 26] = rgba_[2];
                vBuffer8[vP8 + 27] = rgba_[3];
                vP++;

                vBuffer[vP++] = aid_; // ID
            }


            // add the same head as for sheets...

            vBuffer[vP++] = Px;
            vBuffer[vP++] = Py;
            vBuffer[vP++] = Pz;

            vBuffer[vP++] = Tx;
            vBuffer[vP++] = Ty;
            vBuffer[vP++] = Tz;

            vBuffer8[vP8 + 24] = rgba_[0];
            vBuffer8[vP8 + 25] = rgba_[1];
            vBuffer8[vP8 + 26] = rgba_[2];
            vBuffer8[vP8 + 27] = rgba_[3];
            vP++;
            vP8 += 32;

            vBuffer[vP++] = aid_; // ID

            for (i = 0; i < ringTemplate.length; i++) {
                iBuffer[iP++] = p + ringTemplate.length;
                iBuffer[iP++] = p + i;
                iBuffer[iP++] = p + i + 1;
            }
            iBuffer[iP - 1] = p;

            p += ringTemplate.length + 1;
            p_pre = p - novpr;


            radius = coreRadius;

            if (!isLast) {
                var h = coreRadius;

                for (i = 0; i < ringTemplate.length; i++, vP8 += 32) {
                    vBuffer[vP++] = h * ringTemplate[i][0] * Nx + h * ringTemplate[i][1] * Bx + Px - Tx * radius * 2;
                    vBuffer[vP++] = h * ringTemplate[i][0] * Ny + h * ringTemplate[i][1] * By + Py - Ty * radius * 2;
                    vBuffer[vP++] = h * ringTemplate[i][0] * Nz + h * ringTemplate[i][1] * Bz + Pz - Tz * radius * 2;

                    vBuffer[vP++] = ringTemplate[i][0] * Nx + ringTemplate[i][1] * Bx;
                    vBuffer[vP++] = ringTemplate[i][0] * Ny + ringTemplate[i][1] * By;
                    vBuffer[vP++] = ringTemplate[i][0] * Nz + ringTemplate[i][1] * Bz;

                    vBuffer8[vP8 + 24] = rgba_[0];
                    vBuffer8[vP8 + 25] = rgba_[1];
                    vBuffer8[vP8 + 26] = rgba_[2];
                    vBuffer8[vP8 + 27] = rgba_[3];
                    vP++;

                    vBuffer[vP++] = aid_; // ID
                }
            }


            this.buffer3.vP = vP;
            this.buffer3.iP = iP;
            return t;

        }

        // ** build helix representation **
        buildHelix(t, t_next, P, T, N, B, rgba, aid, currentBlock, coreRadius) {
            var dome = this.dome[0], radius = coreRadius, i, novpr = this.novpr;

            var ringTemplate = this.ringTemplate, radius = coreRadius, Px, Py, Pz, Nx, Ny, Nz, Bx, By, Bz, Tx, Ty, Tz, rgba_, aid_;
            var cartoon_highlight_color = molmil.configBox.cartoon_highlight_color;

            var vBuffer = this.buffer3.vertexBuffer, iBuffer = this.buffer3.indexBuffer, vP = this.buffer3.vP, iP = this.buffer3.iP,
                vBuffer8 = this.buffer3.vertexBuffer8, vP8 = vP * 4;
            var p = vP * .125;
            var p_pre = p - novpr;

            var factor, Ys;

            var invertedBinormals = currentBlock.invertedBinormals, Nresume = currentBlock.Nresume, Cresume = currentBlock.Cresume;

            var tmp = [0, 0], noi = this.noi, t_start = t, n = Nresume ? noi : 0;
            for (t; t < t_next; t++) {
                Px = P[t][0], Py = P[t][1], Pz = P[t][2], Tx = T[t][0], Ty = T[t][1], Tz = T[t][2], Nx = N[t][0], Ny = N[t][1], Nz = N[t][2], Bx = B[t][0], By = B[t][1], Bz = B[t][2], rgba_ = rgba[t], aid_ = aid[t];
                if (rgba_[3] != 255) this.buffer3.alphaMode = true;

                if (!Nresume && t < t_start + noi) { factor = (n / noi); n++; }
                else if (!Cresume && t > t_next - noi - 2) { factor = (n / (noi)); n--; }
                else factor = 1.0;
                Ys = (5 * factor) + 1.0;

                for (i = 0; i < ringTemplate.length; i++, vP8 += 32) {
                    vBuffer[vP++] = radius * ringTemplate[i][0] * Nx + Ys * radius * ringTemplate[i][1] * Bx + Px;
                    vBuffer[vP++] = radius * ringTemplate[i][0] * Ny + Ys * radius * ringTemplate[i][1] * By + Py;
                    vBuffer[vP++] = radius * ringTemplate[i][0] * Nz + Ys * radius * ringTemplate[i][1] * Bz + Pz;

                    tmp[0] = Ys * ringTemplate[i][0]; tmp[1] = ringTemplate[i][1]; vec2.normalize(tmp, tmp);
                    vBuffer[vP++] = tmp[0] * Nx + tmp[1] * Bx;
                    vBuffer[vP++] = tmp[0] * Ny + tmp[1] * By;
                    vBuffer[vP++] = tmp[0] * Nz + tmp[1] * Bz;

                    if ((factor > .5 && ((invertedBinormals && vBuffer[vP - 3] * Nx + vBuffer[vP - 2] * Ny + vBuffer[vP - 1] * Nz < -0.01) || (!invertedBinormals && vBuffer[vP - 3] * Nx + vBuffer[vP - 2] * Ny + vBuffer[vP - 1] * Nz > 0.01))) && cartoon_highlight_color != -1) {
                        vBuffer8[vP8 + 24] = cartoon_highlight_color[0];
                        vBuffer8[vP8 + 25] = cartoon_highlight_color[1];
                        vBuffer8[vP8 + 26] = cartoon_highlight_color[2];
                        vBuffer8[vP8 + 27] = rgba_[3];
                    }
                    else {
                        vBuffer8[vP8 + 24] = rgba_[0];
                        vBuffer8[vP8 + 25] = rgba_[1];
                        vBuffer8[vP8 + 26] = rgba_[2];
                        vBuffer8[vP8 + 27] = rgba_[3];
                    }
                    vP++;

                    vBuffer[vP++] = aid_; // ID
                }

                for (i = 0; i < (novpr * 2) - 2; i += 2, p += 1, p_pre += 1) {
                    iBuffer[iP++] = p;
                    iBuffer[iP++] = p_pre;
                    iBuffer[iP++] = p_pre + 1;

                    iBuffer[iP++] = p_pre + 1;
                    iBuffer[iP++] = p + 1;
                    iBuffer[iP++] = p;
                }
                iBuffer[iP++] = p;
                iBuffer[iP++] = p_pre;
                iBuffer[iP++] = p_pre - (novpr - 1);

                iBuffer[iP++] = p_pre - (novpr - 1);
                iBuffer[iP++] = p - (novpr - 1);
                iBuffer[iP++] = p;

                p++; p_pre++;
            }

            this.buffer3.vP = vP;
            this.buffer3.iP = iP;
            return t;
        };

        // ** build sheet representation **
        buildSheet(t, t_next, P, T, N, B, rgba, aid, isFirst, isLast, coreRadius) {
            var dome = this.dome[0], radius = coreRadius, i, novpr = this.novpr;
            var ringTemplate = this.ringTemplate, radius = coreRadius, Px, Py, Pz, Nx, Ny, Nz, Bx, By, Bz, Tx, Ty, Tz, rgba_, aid_;

            var cartoon_highlight_color = molmil.configBox.cartoon_highlight_color;


            var vBuffer = this.buffer3.vertexBuffer, iBuffer = this.buffer3.indexBuffer, vP = this.buffer3.vP, iP = this.buffer3.iP,
                vBuffer8 = this.buffer3.vertexBuffer8, vP8 = vP * 4;
            var p = vP * .125;
            var p_pre = p - novpr;
            var squareVertices = this.squareVertices, noi = this.noi, squareVerticesN = this.squareVerticesN, squareVerticesNhead = this.squareVerticesNhead, flag;

            var h = this.sheetHeight, w = h * 8;

            if (!isFirst) {
                Px = (P[t][0] * .75) + (P[t + 1][0] * .25), Py = (P[t][1] * .75) + (P[t][1] * .25), Pz = (P[t][2] * .75) + (P[t][2] * .25), Tx = T[t - 1][0], Ty = T[t - 1][1], Tz = T[t - 1][2],
                    Nx = N[t - 1][0], Ny = N[t - 1][1], Nz = N[t - 1][2], Bx = B[t - 1][0], By = B[t - 1][1], Bz = B[t - 1][2], rgba_ = rgba[t - 1], aid_ = aid[t];
                if (rgba_[3] != 255) this.buffer3.alphaMode = true;
                for (i = 0; i < ringTemplate.length; i++, vP8 += 32) {
                    vBuffer[vP++] = h * .5 * ringTemplate[i][0] * Nx + h * .5 * ringTemplate[i][1] * Bx + Px;
                    vBuffer[vP++] = h * .5 * ringTemplate[i][0] * Ny + h * .5 * ringTemplate[i][1] * By + Py;
                    vBuffer[vP++] = h * .5 * ringTemplate[i][0] * Nz + h * .5 * ringTemplate[i][1] * Bz + Pz;

                    vBuffer[vP++] = ((ringTemplate[i][0] * Nx + ringTemplate[i][1] * Bx) + Tx) * .5;
                    vBuffer[vP++] = ((ringTemplate[i][0] * Ny + ringTemplate[i][1] * By) + Ty) * .5;
                    vBuffer[vP++] = ((ringTemplate[i][0] * Nz + ringTemplate[i][1] * Bz) + Tz) * .5;

                    vBuffer8[vP8 + 24] = rgba_[0];
                    vBuffer8[vP8 + 25] = rgba_[1];
                    vBuffer8[vP8 + 26] = rgba_[2];
                    vBuffer8[vP8 + 27] = rgba_[3];
                    vP++;

                    vBuffer[vP++] = aid[t - 1]; // ID
                }

                for (i = 0; i < (novpr * 2) - 2; i += 2, p += 1, p_pre += 1) {
                    iBuffer[iP++] = p;
                    iBuffer[iP++] = p_pre;
                    iBuffer[iP++] = p_pre + 1;

                    iBuffer[iP++] = p_pre + 1;
                    iBuffer[iP++] = p + 1;
                    iBuffer[iP++] = p;
                }
                iBuffer[iP++] = p;
                iBuffer[iP++] = p_pre;
                iBuffer[iP++] = p_pre - (novpr - 1);

                iBuffer[iP++] = p_pre - (novpr - 1);
                iBuffer[iP++] = p - (novpr - 1);
                iBuffer[iP++] = p;

                p++; p_pre++;


            }

            // draw arrow bottom

            Px = P[t][0], Py = P[t][1], Pz = P[t][2], Tx = T[t][0], Ty = T[t][1], Tz = T[t][2], Nx = N[t][0], Ny = N[t][1], Nz = N[t][2], Bx = B[t][0], By = B[t][1], Bz = B[t][2], rgba_ = rgba[t], aid_ = aid[t];
            if (rgba_[3] != 255) this.buffer3.alphaMode = true;
            for (i = 0; i < 4; i++, vP8 += 32) {
                vBuffer[vP++] = h * squareVertices[i][0] * Nx + w * squareVertices[i][1] * Bx + Px;
                vBuffer[vP++] = h * squareVertices[i][0] * Ny + w * squareVertices[i][1] * By + Py;
                vBuffer[vP++] = h * squareVertices[i][0] * Nz + w * squareVertices[i][1] * Bz + Pz;

                vBuffer[vP++] = -Tx;
                vBuffer[vP++] = -Ty;
                vBuffer[vP++] = -Tz;

                if (cartoon_highlight_color != -1) {
                    vBuffer8[vP8 + 24] = cartoon_highlight_color[0];
                    vBuffer8[vP8 + 25] = cartoon_highlight_color[1];
                    vBuffer8[vP8 + 26] = cartoon_highlight_color[2];
                    vBuffer8[vP8 + 27] = rgba_[3];
                }
                else {
                    vBuffer8[vP8 + 24] = rgba_[0];
                    vBuffer8[vP8 + 25] = rgba_[1];
                    vBuffer8[vP8 + 26] = rgba_[2];
                    vBuffer8[vP8 + 27] = rgba_[3];
                }

                vP++;

                vBuffer[vP++] = aid_; // ID
            }

            iBuffer[iP++] = p; iBuffer[iP++] = p + 1; iBuffer[iP++] = p + 2; iBuffer[iP++] = p; iBuffer[iP++] = p + 2; iBuffer[iP++] = p + 3;

            p += 4;
            // draw arrow tail

            var as = Math.ceil(noi / 1.5);
            var dw = (h * 12) / as;
            as = t_next - as;


            var n = 0;
            for (t; t < as; t++, n++) {
                Px = P[t][0], Py = P[t][1], Pz = P[t][2], Tx = T[t][0], Ty = T[t][1], Tz = T[t][2], Nx = N[t][0], Ny = N[t][1], Nz = N[t][2], Bx = B[t][0], By = B[t][1], Bz = B[t][2], rgba_ = rgba[t], aid_ = aid[t];
                if (rgba_[3] != 255) this.buffer3.alphaMode = true;
                for (i = 0; i < 4; i++, vP8 += 32) {

                    vBuffer[vP++] = h * squareVertices[i][0] * Nx + w * squareVertices[i][1] * Bx + Px;
                    vBuffer[vP++] = h * squareVertices[i][0] * Ny + w * squareVertices[i][1] * By + Py;
                    vBuffer[vP++] = h * squareVertices[i][0] * Nz + w * squareVertices[i][1] * Bz + Pz;

                    vBuffer[vP++] = squareVerticesN[i][0] * Nx + squareVerticesN[i][1] * Bx;
                    vBuffer[vP++] = squareVerticesN[i][0] * Ny + squareVerticesN[i][1] * By;
                    vBuffer[vP++] = squareVerticesN[i][0] * Nz + squareVerticesN[i][1] * Bz;

                    if ((i == 1 || i == 3) && cartoon_highlight_color != -1) {
                        vBuffer8[vP8 + 24] = cartoon_highlight_color[0];
                        vBuffer8[vP8 + 25] = cartoon_highlight_color[1];
                        vBuffer8[vP8 + 26] = cartoon_highlight_color[2];
                        vBuffer8[vP8 + 27] = rgba_[3];
                    }
                    else {
                        vBuffer8[vP8 + 24] = rgba_[0];
                        vBuffer8[vP8 + 25] = rgba_[1];
                        vBuffer8[vP8 + 26] = rgba_[2];
                        vBuffer8[vP8 + 27] = rgba_[3];
                    }
                    vP++;

                    vBuffer[vP++] = aid_; // ID

                    vP8 += 32;

                    //========


                    vBuffer[vP++] = h * squareVertices[i][0] * Nx + w * squareVertices[i][1] * Bx + Px;
                    vBuffer[vP++] = h * squareVertices[i][0] * Ny + w * squareVertices[i][1] * By + Py;
                    vBuffer[vP++] = h * squareVertices[i][0] * Nz + w * squareVertices[i][1] * Bz + Pz;

                    vBuffer[vP++] = squareVerticesN[i + 1][0] * Nx + squareVerticesN[i + 1][1] * Bx;
                    vBuffer[vP++] = squareVerticesN[i + 1][0] * Ny + squareVerticesN[i + 1][1] * By;
                    vBuffer[vP++] = squareVerticesN[i + 1][0] * Nz + squareVerticesN[i + 1][1] * Bz;

                    if ((i == 0 || i == 2) && cartoon_highlight_color != -1) {
                        vBuffer8[vP8 + 24] = cartoon_highlight_color[0];
                        vBuffer8[vP8 + 25] = cartoon_highlight_color[1];
                        vBuffer8[vP8 + 26] = cartoon_highlight_color[2];
                        vBuffer8[vP8 + 27] = rgba_[3];
                    }
                    else {
                        vBuffer8[vP8 + 24] = rgba_[0];
                        vBuffer8[vP8 + 25] = rgba_[1];
                        vBuffer8[vP8 + 26] = rgba_[2];
                        vBuffer8[vP8 + 27] = rgba_[3];
                    }
                    vP++;


                    vBuffer[vP++] = aid_; // ID
                }

                if (n > 0) {
                    iBuffer[iP++] = p - 8; iBuffer[iP++] = p - 1; iBuffer[iP++] = p + 7; iBuffer[iP++] = p - 8; iBuffer[iP++] = p + 7; iBuffer[iP++] = p + 0;
                    iBuffer[iP++] = p - 7; iBuffer[iP++] = p + 1; iBuffer[iP++] = p + 2; iBuffer[iP++] = p - 7; iBuffer[iP++] = p + 2; iBuffer[iP++] = p - 6;
                    iBuffer[iP++] = p - 5; iBuffer[iP++] = p + 3; iBuffer[iP++] = p + 4; iBuffer[iP++] = p - 5; iBuffer[iP++] = p + 4; iBuffer[iP++] = p - 4;
                    iBuffer[iP++] = p - 2; iBuffer[iP++] = p - 3; iBuffer[iP++] = p + 5; iBuffer[iP++] = p - 2; iBuffer[iP++] = p + 5; iBuffer[iP++] = p + 6;
                }

                p += 8;
            }

            w = h * 12;
            t--;
            if (isLast) as--;

            for (; t < t_next; t++, n++) {
                if (t >= as) w -= dw;

                Px = P[t][0], Py = P[t][1], Pz = P[t][2], Tx = T[t][0], Ty = T[t][1], Tz = T[t][2], Nx = N[t][0], Ny = N[t][1], Nz = N[t][2], Bx = B[t][0], By = B[t][1], Bz = B[t][2], rgba_ = rgba[t], aid_ = aid[t];
                if (rgba_[3] != 255) this.buffer3.alphaMode = true;
                for (i = 0; i < 4; i++, vP8 += 32) {

                    vBuffer[vP++] = h * squareVertices[i][0] * Nx + w * squareVertices[i][1] * Bx + Px;
                    vBuffer[vP++] = h * squareVertices[i][0] * Ny + w * squareVertices[i][1] * By + Py;
                    vBuffer[vP++] = h * squareVertices[i][0] * Nz + w * squareVertices[i][1] * Bz + Pz;

                    vBuffer[vP++] = squareVerticesNhead[i][0] * Nx + squareVerticesNhead[i][1] * Bx;
                    vBuffer[vP++] = squareVerticesNhead[i][0] * Ny + squareVerticesNhead[i][1] * By;
                    vBuffer[vP++] = squareVerticesNhead[i][0] * Nz + squareVerticesNhead[i][1] * Bz;

                    if ((i == 1 || i == 3) && cartoon_highlight_color != -1) {
                        vBuffer8[vP8 + 24] = cartoon_highlight_color[0];
                        vBuffer8[vP8 + 25] = cartoon_highlight_color[1];
                        vBuffer8[vP8 + 26] = cartoon_highlight_color[2];
                        vBuffer8[vP8 + 27] = rgba_[3];
                    }
                    else {
                        vBuffer8[vP8 + 24] = rgba_[0];
                        vBuffer8[vP8 + 25] = rgba_[1];
                        vBuffer8[vP8 + 26] = rgba_[2];
                        vBuffer8[vP8 + 27] = rgba_[3];
                    }
                    vP++;

                    vP8 += 32;


                    vBuffer[vP++] = aid_; // ID


                    //========


                    vBuffer[vP++] = h * squareVertices[i][0] * Nx + w * squareVertices[i][1] * Bx + Px;
                    vBuffer[vP++] = h * squareVertices[i][0] * Ny + w * squareVertices[i][1] * By + Py;
                    vBuffer[vP++] = h * squareVertices[i][0] * Nz + w * squareVertices[i][1] * Bz + Pz;

                    vBuffer[vP++] = squareVerticesNhead[i + 1][0] * Nx + squareVerticesNhead[i + 1][1] * Bx;
                    vBuffer[vP++] = squareVerticesNhead[i + 1][0] * Ny + squareVerticesNhead[i + 1][1] * By;
                    vBuffer[vP++] = squareVerticesNhead[i + 1][0] * Nz + squareVerticesNhead[i + 1][1] * Bz;

                    if ((i == 0 || i == 2) && cartoon_highlight_color != -1) {
                        vBuffer8[vP8 + 24] = cartoon_highlight_color[0];
                        vBuffer8[vP8 + 25] = cartoon_highlight_color[1];
                        vBuffer8[vP8 + 26] = cartoon_highlight_color[2];
                        vBuffer8[vP8 + 27] = rgba_[3];
                    }
                    else {
                        vBuffer8[vP8 + 24] = rgba_[0];
                        vBuffer8[vP8 + 25] = rgba_[1];
                        vBuffer8[vP8 + 26] = rgba_[2];
                        vBuffer8[vP8 + 27] = rgba_[3];
                    }
                    vP++;

                    vBuffer[vP++] = aid_; // ID
                }

                if (n > 0) {
                    iBuffer[iP++] = p - 8; iBuffer[iP++] = p - 1; iBuffer[iP++] = p + 7; iBuffer[iP++] = p - 8; iBuffer[iP++] = p + 7; iBuffer[iP++] = p + 0;
                    iBuffer[iP++] = p - 7; iBuffer[iP++] = p + 1; iBuffer[iP++] = p + 2; iBuffer[iP++] = p - 7; iBuffer[iP++] = p + 2; iBuffer[iP++] = p - 6;
                    iBuffer[iP++] = p - 5; iBuffer[iP++] = p + 3; iBuffer[iP++] = p + 4; iBuffer[iP++] = p - 5; iBuffer[iP++] = p + 4; iBuffer[iP++] = p - 4;
                    iBuffer[iP++] = p - 2; iBuffer[iP++] = p - 3; iBuffer[iP++] = p + 5; iBuffer[iP++] = p - 2; iBuffer[iP++] = p + 5; iBuffer[iP++] = p + 6;
                }

                p += 8;
            }

            // draw arrow head
            if (!isLast) {
                Tx = T[t][0], Ty = T[t][1], Tz = T[t][2], Nx = N[t][0], Ny = N[t][1], Nz = N[t][2], Bx = B[t][0], By = B[t][1], Bz = B[t][2], rgba_ = rgba[t], aid_ = aid[t];
                if (rgba_[3] != 255) this.buffer3.alphaMode = true;
                for (i = 0; i < ringTemplate.length; i++, vP8 += 32) {
                    vBuffer[vP++] = Px;
                    vBuffer[vP++] = Py;
                    vBuffer[vP++] = Pz;

                    vBuffer[vP++] = -Tx;
                    vBuffer[vP++] = -Ty;
                    vBuffer[vP++] = -Tz;

                    vBuffer8[vP8 + 24] = rgba_[0];
                    vBuffer8[vP8 + 25] = rgba_[1];
                    vBuffer8[vP8 + 26] = rgba_[2];
                    vBuffer8[vP8 + 27] = rgba_[3];
                    vP++;

                    vBuffer[vP++] = aid[t]; // ID
                }

                p += ringTemplate.length;

                for (i = 0; i < ringTemplate.length; i++, vP8 += 32) {
                    vBuffer[vP++] = h * ringTemplate[i][0] * Nx + h * ringTemplate[i][1] * Bx + Px;
                    vBuffer[vP++] = h * ringTemplate[i][0] * Ny + h * ringTemplate[i][1] * By + Py;
                    vBuffer[vP++] = h * ringTemplate[i][0] * Nz + h * ringTemplate[i][1] * Bz + Pz;

                    vBuffer[vP++] = ((ringTemplate[i][0] * Nx + ringTemplate[i][1] * Bx) - Tx) * .5;
                    vBuffer[vP++] = ((ringTemplate[i][0] * Ny + ringTemplate[i][1] * By) - Ty) * .5;
                    vBuffer[vP++] = ((ringTemplate[i][0] * Nz + ringTemplate[i][1] * Bz) - Tz) * .5;

                    vBuffer8[vP8 + 24] = rgba_[0];
                    vBuffer8[vP8 + 25] = rgba_[1];
                    vBuffer8[vP8 + 26] = rgba_[2];
                    vBuffer8[vP8 + 27] = rgba_[3];
                    vP++;

                    vBuffer[vP++] = aid[t]; // ID
                }

                p_pre = p - novpr;
                for (i = 0; i < (novpr * 2) - 2; i += 2, p += 1, p_pre += 1) {
                    iBuffer[iP++] = p;
                    iBuffer[iP++] = p_pre;
                    iBuffer[iP++] = p_pre + 1;

                    iBuffer[iP++] = p_pre + 1;
                    iBuffer[iP++] = p + 1;
                    iBuffer[iP++] = p;
                }
                iBuffer[iP++] = p;
                iBuffer[iP++] = p_pre;
                iBuffer[iP++] = p_pre - (novpr - 1);

                iBuffer[iP++] = p_pre - (novpr - 1);
                iBuffer[iP++] = p - (novpr - 1);
                iBuffer[iP++] = p;

                p++; p_pre++;
            }

            this.buffer3.vP = vP;
            this.buffer3.iP = iP;
            return t;
        };

        generateSNFG() {
            // generate snfg objects...

            var defaultRadius = 4.0, radius, length, transVec, rotationMatrix = mat4.create(), angle, vertex = vec3.create(), normal = vec3.create();

            var sphere = this.getSphere(0.5, this.detail_lv);

            var cylinder = this.getCylinder(this.detail_lv);

            var mdl = this.modelId || 0, tmp;
            var bonds2draw = this.bonds2draw,
                vBuffer = this.buffer1.vertexBuffer, iBuffer = this.buffer1.indexBuffer, vP = this.buffer1.vP, iP = this.buffer1.iP, detail_lv = this.detail_lv,
                vBuffer8 = this.buffer1.vertexBuffer8, vP8 = vP * 4, p = vP / 8;

            // loop over everything...

            var obj, mesh, rgbas = [null, null], rgba;
            for (var i = 0; i < this.snfg_objs.length; i++) {
                obj = this.snfg_objs[i];

                if (obj.mesh.type == "sphere") mesh = sphere;
                else if (obj.mesh.type == "cylinder") mesh = cylinder;
                else mesh = molmil.shapes3d[obj.mesh.type];

                for (v = 0; v < mesh.indices.length; v++, iP++) iBuffer[iP] = mesh.indices[v] + p;

                rgbas[0] = obj.mesh.rgba;
                rgbas[1] = obj.mesh.rgba2 || obj.mesh.rgba;
                radius = obj.radius;
                if (!radius) radius = obj.mode == 3 ? 4 : 2;
                length = (obj.length || 0.5) * 2;
                mat4.identity(rotationMatrix);
                if (obj.backvec) {
                    angle = Math.acos(-obj.backvec[2]);
                    mat4.rotate(rotationMatrix, rotationMatrix, angle, [obj.backvec[1], -obj.backvec[0], 0.0]);
                }
                else if (obj.upvec) {
                    angle = Math.acos(-obj.upvec[1]);
                    mat4.rotate(rotationMatrix, rotationMatrix, angle, [-obj.upvec[2], 0.0, obj.upvec[0]]);
                }
                else if (obj.sidevec) {
                    angle = Math.acos(-obj.sidevec[0]);
                    mat4.rotate(rotationMatrix, rotationMatrix, angle, [0.0, obj.sidevec[2], -obj.sidevec[1]]);
                }

                if (obj.radius) length /= radius;
                for (v = 0; v < mesh.vertices.length; v += 3, vP8 += 32) {
                    vec3.transformMat4(vertex, [mesh.vertices[v] * radius, mesh.vertices[v + 1] * radius, mesh.vertices[v + 2] * radius * length], rotationMatrix);
                    vec3.transformMat4(normal, [mesh.normals[v], mesh.normals[v + 1], mesh.normals[v + 2]], rotationMatrix);

                    vBuffer[vP++] = vertex[0] + obj.center[0];
                    vBuffer[vP++] = vertex[1] + obj.center[1];
                    vBuffer[vP++] = vertex[2] + obj.center[2];

                    vBuffer[vP++] = normal[0];
                    vBuffer[vP++] = normal[1];
                    vBuffer[vP++] = normal[2];

                    rgba = rgbas[mesh.rgba2idxs ? mesh.rgba2idxs[v / 3] : 0];
                    vBuffer8[vP8 + 24] = rgba[0];
                    vBuffer8[vP8 + 25] = rgba[1];
                    vBuffer8[vP8 + 26] = rgba[2];
                    vBuffer8[vP8 + 27] = rgba[3];
                    vP++

                    vBuffer[vP++] = -1; // ID
                }
                p += mesh.vertices.length / 3;

            }

            this.buffer1.vP = vP;
            this.buffer1.iP = iP;
        }


    }

    export const geometry: _geometry = new _geometry();

    molmil.geometry.generateCylinder();
}