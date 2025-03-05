namespace molmil {

    export function generateSphereImposterTexture(res, gl) {
        var lightPos = [50.0, 50.0, 100.0 + 250.0], u, v, r, theta, phi, pos = [0., 0., 0.], lightDir = [0., 0., 0.], viewDir = [0., 0., 0.], reflectDir = [0., 0., 0.], lambertian, specular;

        var out = new Uint8Array(res * res * 4); // RGB set to lambertian, specular, boolean

        for (var i = 0; i < res; i++) {
            u = ((i / res) * 2.0) - 1.;
            for (var j = 0; j < res; j++) {
                v = ((j / res) * 2.0) - 1.;
                r = Math.sqrt(Math.pow(u, 2.0) + Math.pow(v, 2.0));
                if (r < 1.0) {
                    pos[0] = u;
                    pos[1] = v;
                    pos[2] = Math.sqrt(1 - pos[0] * pos[0] - pos[1] * pos[1]);

                    lightDir[0] = lightPos[0] - pos[0]; lightDir[1] = lightPos[1] - pos[1]; lightDir[2] = lightPos[2] - pos[2]; vec3.normalize(lightDir, lightDir);

                    lambertian = Math.min(Math.max(vec3.dot(pos, lightDir), 0.0), 1.0);

                    specular = vec3.dot(pos, lightDir);
                    reflectDir[0] = lightDir[0] - 2.0 * specular * pos[0];
                    reflectDir[1] = lightDir[1] - 2.0 * specular * pos[1];
                    reflectDir[2] = lightDir[2] - 2.0 * specular * pos[2];
                    vec3.negate(viewDir, pos); vec3.normalize(viewDir, viewDir);
                    specular = Math.pow(Math.max(vec3.dot(reflectDir, viewDir), 0.0), 64.0) * .5;

                    out[((j * (res * 4)) + (i * 4)) + 0] = lambertian * 255.;
                    out[((j * (res * 4)) + (i * 4)) + 1] = specular * 255.; // specular doesn't work properly...
                    out[((j * (res * 4)) + (i * 4)) + 2] = pos[2] * 255.;
                    out[((j * (res * 4)) + (i * 4)) + 3] = 255.;
                }
                else {
                    out[((j * (res * 4)) + (i * 4)) + 0] = 0.0;
                    out[((j * (res * 4)) + (i * 4)) + 1] = 0.0;
                    out[((j * (res * 4)) + (i * 4)) + 2] = 0.0;
                    out[((j * (res * 4)) + (i * 4)) + 3] = 0.0;
                }
            }
        }

        var texture = gl.createTexture();
        gl.bindTexture(gl.TEXTURE_2D, texture);
        gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, true);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
        gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, res, res, 0, gl.RGBA, gl.UNSIGNED_BYTE, out);
        gl.bindTexture(gl.TEXTURE_2D, null);
        texture.loaded = true;

        return texture;
    }

    export class render {


        // ** renderer object **

        constructor(soup) { // render object
            this.canvas = null;

            this.reloadSettings();

            this.programs = [];

            this.clearCut = 0;
            this.fogStart = 0;
            this.camera = new molmil.glCamera();
            this.soup = soup;
            this.modelViewMatrix = mat4.create();
            this.projectionMatrix = mat4.create();

            this.pitch = 0;
            this.heading = 0;
            this.roll = 0;
            this.TransX = 0;
            this.TransY = 0;
            this.TransZ = 0;
            this.pitchAngle = 0;
            this.headingAngle = 0;
            this.rollAngle = 0;

            this.TransB = [.5, .5, 0];

            this.animationMode = false;
            this.framesDefinition = []; // list of modelObject and polygonObject objects for each frame
            this.framesBuffer = []; // list of vertex data for each frame



            this.textures = {};
            this.FBOs = {};
            this.shaders = {};
            this.buffers = {};
            this.gl = null;
            this.modelId = 0;
            // new init...

            this.cameraHistory = {};

        };

        // ** resets the buffer **
        clear() {
            for (var i = 0; i < this.programs.length; i++) {
                this.gl.deleteBuffer(this.programs[i].vertexBuffer);
                this.gl.deleteBuffer(this.programs[i].indexBuffer);
            }
            this.programs = [];
            this.program1 = this.program2 = this.program3 = this.program4 = this.program5 = undefined;
            this.cameraHistory = {};
        };

        addProgram(program) {
            if (program.settings.alphaMode) this.programs.push(program);
            else this.programs.unshift(program);
        }

        removeProgram(program) {
            if (program instanceof Array) {
                for (var i = 0; i < program.length; i++) this.removeProgram(program[i]);
                return;
            }
            var ndx = this.programs.indexOf(program);
            if (ndx > -1) this.programs.splice(ndx, 1);
        }

        removeProgramByName(idname) {
            var ndx = -1;
            for (var i = 0; i < this.programs.length; i++) if (this.programs[i].idname == idname) ndx = i;
            if (ndx > -1) this.programs.splice(ndx, 1);
        }

        reloadSettings() {
            this.QLV = molmil.localStorageGET("molmil.settings_QLV");
            if (this.QLV == null) {
                this.QLV = 2;
                try { localStorage.setItem("molmil.settings_QLV", this.QLV); }
                catch (e) { }
            }
            else this.QLV = parseInt(this.QLV);
            if (molmil.configBox.liteMode) this.QLV = 1;
            if (this.gl) this.gl.clearColor.apply(this.gl, molmil.configBox.BGCOLOR);
            this.settings = {};
        };

        selectDefaultContext() {
            this.gl = this.defaultContext;
        };

        selectDataContext() {
            if (!this.dataContext) this.dataContext = this.canvas.getContext("webgl", { preserveDrawingBuffer: true }) || this.canvas.getContext("experimental-webgl", { preserveDrawingBuffer: true });
            this.gl = this.dataContext;
        };



        // ** initiates the WebGL context **
        initGL(canvas, width, height) {
            var glAttribs = molmil.configBox.glAttribs || {};
            if (molmil.vrDisplay && molmil.vrDisplay.capabilities.hasExternalDisplay) glAttribs.preserveDrawingBuffer = true;

            if (molmil.configBox.webGL2) {
                if (window.WebGL2RenderingContext) {
                    this.defaultContext = canvas.getContext("webgl2", glAttribs);
                    if (!this.defaultContext) molmil.configBox.webGL2 = false;
                    else {
                        molmil.configBox.OES_element_index_uint = true;
                        molmil.configBox.EXT_frag_depth = true;
                    }
                }
                else molmil.configBox.webGL2 = false;
            }
            if (!molmil.configBox.webGL2) {
                this.defaultContext = this.defaultContext || canvas.getContext("webgl", glAttribs) || canvas.getContext("experimental-webgl", glAttribs);
            }

            canvas.renderer = this;

            this.FBOs = {};

            if (!this.defaultContext) {
                this.altCanvas = molmil.__webglNotSupported__(canvas);
                return false;
            }
            this.gl = this.defaultContext;
            this.gl.boundAttributes = {};

            if (!molmil.configBox.webGL2) {
                molmil.configBox.OES_element_index_uint = this.gl.getExtension('OES_element_index_uint');
            }

            this.textures.atom_imposter = molmil.generateSphereImposterTexture(128, this.gl);

            this.gl.INDEXINT = molmil.configBox.OES_element_index_uint ? this.gl.UNSIGNED_INT : this.gl.UNSIGNED_SHORT;
            this.gl.__glAttribs = glAttribs;

            this.width = width | canvas.width;
            this.height = height | canvas.height;

            this.gl.viewportWidth = this.width;
            this.gl.viewportHeight = this.height;

            canvas.onmousedown = molmil.handle_molmilViewer_mouseDown;
            document.onmouseup = molmil.handle_molmilViewer_mouseUp;
            document.onmousemove = molmil.handle_molmilViewer_mouseMove;

            if (!molmil.vrDisplay) {

                window.addEventListener("vrdisplaypointerrestricted", function () {
                    canvas.requestPointerLock();
                }, false);

                window.addEventListener("vrdisplaypointerunrestricted", function () {
                    document.exitPointerLock()
                    canvas.bindTouch();
                }, false);

                //window.addEventListener('vrdisplaypointerrestricted', () => webglCanvas.requestPointerLock(), false);
                //window.addEventListener('vrdisplaypointerunrestricted', document.exitPointerLock(), false);

                canvas.bindMouseTouch = function () {
                    canvas.addEventListener("wheel", molmil.handle_molmilViewer_mouseScroll, false);
                    canvas.addEventListener("touchstart", molmil.handle_molmilViewer_touchStart, false);
                    canvas.addEventListener("touchmove", molmil.handle_molmilViewer_touchMove, false);
                    canvas.addEventListener("touchend", molmil.handle_molmilViewer_touchEnd, false);
                };

                canvas.bindMouseTouch();
                canvas.addEventListener("webglcontextlost", function (event) { event.preventDefault(); }, false);
                canvas.addEventListener("webglcontextrestored", function () {
                    this.reinitRenderer();
                }, false);
            }

            this.gl.clearColor.apply(this.gl, molmil.configBox.BGCOLOR);
            this.gl.enable(this.gl.DEPTH_TEST);

            if (molmil.configBox.cullFace) {
                this.gl.enable(this.gl.CULL_FACE);
                this.gl.cullFace(this.gl.BACK);
            }

            this.canvas = canvas;

            var h = this.gl.getParameter(this.gl.ALIASED_LINE_WIDTH_RANGE);
            this.angle = h && h.length == 2 && h[0] == 1 && h[1] == 1;

            if (this.billboardProgram) {
                for (var i = 0; i < this.billboardProgram.data.length; i++) this.billboardProgram.data[i].status = false
            }
            else {
                // add labels program here...
                this.billboardProgram = {};
                this.billboardProgram.data = this.soup.texturedBillBoards; this.billboardProgram.renderer = this;
                this.billboardProgram.render = function (modelViewMatrix, COR) {
                    //first check for new/modified billboards (texturedBillBoards[i].status == false)

                    var N = [], i, status = true, scaleFactor;
                    for (i = 0; i < this.data.length; i++) {
                        if (!this.data[i].status) { this.data[i].status = true; status = false; }
                        if (!this.data[i].display) continue;
                        N.push(this.data[i]);
                    }

                    if (!status) { // if the number of visible billboards has changed, rebuild the vertex array...
                        var ssOffset = [[-1, +1], [+1, +1], [+1, -1], [-1, -1]], n, p = 0, vP = 0, vP16 = 0, iP = 0;
                        var vBuffer = new Float32Array(N.length * 4 * 4), iBuffer; // x, y, z, offset
                        if (molmil.configBox.OES_element_index_uint) iBuffer = new Uint32Array(N.length * 2 * 3);
                        else iBuffer = new Uint16Array(N.length * 2 * 3);
                        var vBuffer16 = new Uint16Array(vBuffer.buffer);

                        for (i = 0; i < N.length; i++) {
                            for (n = 0; n < 4; n++) {
                                vBuffer[vP++] = N[i].settings.xyz[0];
                                vBuffer[vP++] = N[i].settings.xyz[1];
                                vBuffer[vP++] = N[i].settings.xyz[2];

                                vBuffer16[vP16 + 6] = ssOffset[n][0];
                                vBuffer16[vP16 + 7] = ssOffset[n][1];
                                vP++

                                vP16 += 8;
                            }
                            iBuffer[iP++] = 3 + p;
                            iBuffer[iP++] = 2 + p;
                            iBuffer[iP++] = 1 + p;
                            iBuffer[iP++] = 3 + p;
                            iBuffer[iP++] = 1 + p;
                            iBuffer[iP++] = 0 + p;
                            p += 4;
                        }

                        var vbuffer = this.renderer.gl.createBuffer();
                        this.renderer.gl.bindBuffer(this.renderer.gl.ARRAY_BUFFER, vbuffer);
                        this.renderer.gl.bufferData(this.renderer.gl.ARRAY_BUFFER, vBuffer, this.renderer.gl.STATIC_DRAW);

                        var ibuffer = this.renderer.gl.createBuffer();
                        this.renderer.gl.bindBuffer(this.renderer.gl.ELEMENT_ARRAY_BUFFER, ibuffer);
                        this.renderer.gl.bufferData(this.renderer.gl.ELEMENT_ARRAY_BUFFER, iBuffer, this.renderer.gl.STATIC_DRAW);

                        this.nElements = iBuffer.length;
                        this.vertexBuffer = vbuffer;
                        this.indexBuffer = ibuffer;
                    }

                    if (N == 0) return;

                    if (!this.shader) {
                        this.shader = this.renderer.shaders.billboardShader;
                        this.attributes = this.shader.attributes;
                    }

                    // pre-render

                    var normalMatrix = mat3.create();
                    mat3.normalFromMat4(normalMatrix, modelViewMatrix);

                    this.renderer.gl.useProgram(this.shader.program);
                    this.renderer.gl.uniformMatrix4fv(this.shader.uniforms.modelViewMatrix, false, modelViewMatrix);
                    this.renderer.gl.uniformMatrix3fv(this.shader.uniforms.normalMatrix, false, normalMatrix);
                    this.renderer.gl.uniformMatrix4fv(this.shader.uniforms.projectionMatrix, false, this.renderer.projectionMatrix);
                    this.renderer.gl.uniform3f(this.shader.uniforms.COR, COR[0], COR[1], COR[2]);
                    this.renderer.gl.uniform1f(this.shader.uniforms.focus, this.renderer.fogStart);
                    this.renderer.gl.uniform1f(this.shader.uniforms.fogSpan, this.renderer.fogSpan);
                    this.renderer.gl.uniform1f(this.shader.uniforms.disableFog, false);
                    if (this.renderer.settings.slab) {
                        this.renderer.gl.uniform1f(this.shader.uniforms.slabNear, -modelViewMatrix[14] + this.renderer.settings.slabNear - molmil.configBox.zNear);
                        this.renderer.gl.uniform1f(this.shader.uniforms.slabFar, -modelViewMatrix[14] + this.renderer.settings.slabFar - molmil.configBox.zNear);
                    }
                    if (molmil.configBox.fogColor) this.renderer.gl.uniform4f(this.shader.uniforms.backgroundColor, molmil.configBox.fogColor[0], molmil.configBox.fogColor[1], molmil.configBox.fogColor[2], 1.0);
                    else this.renderer.gl.uniform4f(this.shader.uniforms.backgroundColor, molmil.configBox.BGCOLOR[0], molmil.configBox.BGCOLOR[1], molmil.configBox.BGCOLOR[2], 1.0);

                    // render
                    this.renderer.gl.bindBuffer(this.renderer.gl.ARRAY_BUFFER, this.vertexBuffer);
                    this.renderer.gl.bindBuffer(this.renderer.gl.ELEMENT_ARRAY_BUFFER, this.indexBuffer);

                    molmil.resetAttributes(this.renderer.gl);
                    molmil.bindAttribute(this.renderer.gl, this.attributes.in_Position, 3, this.renderer.gl.FLOAT, false, 16, 0);
                    molmil.bindAttribute(this.renderer.gl, this.attributes.in_ScreenSpaceOffset, 2, this.renderer.gl.SHORT, false, 16, 12);
                    molmil.clearAttributes(this.renderer.gl);

                    this.renderer.gl.enable(this.renderer.gl.BLEND);
                    this.renderer.gl.blendFunc(this.renderer.gl.ONE, this.renderer.gl.ONE_MINUS_SRC_ALPHA);
                    this.renderer.gl.depthMask(false);


                    for (i = 0; i < N.length; i++) {
                        // set uniforms (e.g. screen-space translation...)

                        this.renderer.gl.activeTexture(this.renderer.gl.TEXTURE0);
                        this.renderer.gl.bindTexture(this.renderer.gl.TEXTURE_2D, N[i].texture);
                        this.renderer.gl.uniform1i(this.shader.uniforms.textureMap, 0);

                        scaleFactor = 0.0003 * .5 * (N[i].settings.scaleFactor || 1);
                        if (molmil.configBox.stereoMode == 3 && molmil.vrDisplay && !N[i].settings.skipVRscale) scaleFactor *= 4;

                        if (N[i].settings.customWidth && N[i].settings.customHeight) scaleFactor *= Math.min(N[i].settings.customHeight / N[i].texture.renderHeight, N[i].settings.customWidth / N[i].texture.renderWidth);
                        this.renderer.gl.uniform1f(this.shader.uniforms.scaleFactor, scaleFactor);

                        if (N[i].settings.viewpointAligned) this.renderer.gl.uniform1i(this.shader.uniforms.renderMode, 1);
                        else this.renderer.gl.uniform1i(this.shader.uniforms.renderMode, 0);

                        this.renderer.gl.uniform2f(this.shader.uniforms.sizeOffset, N[i].texture.renderWidth, N[i].texture.renderHeight);
                        this.renderer.gl.uniform3f(this.shader.uniforms.positionOffset, N[i].settings.dx, N[i].settings.dy, N[i].settings.dz);

                        if (N[i].settings.alwaysFront) {
                            this.renderer.gl.disable(this.renderer.gl.DEPTH_TEST);
                            this.renderer.gl.uniform1f(this.shader.uniforms.disableFog, true);
                        }
                        this.renderer.gl.drawElements(this.renderer.gl.TRIANGLES, 6, this.renderer.gl.INDEXINT, i * 24);
                        if (N[i].settings.alwaysFront) {
                            this.renderer.gl.enable(this.renderer.gl.DEPTH_TEST);
                            this.renderer.gl.uniform1f(this.shader.uniforms.disableFog, false);
                        }
                    }

                    this.renderer.gl.disable(this.renderer.gl.BLEND);
                    this.renderer.gl.depthMask(true);

                };
                this.billboardProgram.renderPicking = function () { };
            }

            for (var e in molmil.shapes3d) molmil.geometry.updateNormals(molmil.shapes3d[e]);

            return true;
        };

        reinitRenderer() {
            this.initGL(this);
            // we need to recompile the shaders (in case the GPU changed...)
            molmil.shaderEngine.recompile(this);
            this.buffers.atomSelectionBuffer = undefined;
            this.initBuffers();
        };

        initRenderer() { // use this to initalize the renderer, such as call initShaders...
            this.initShaders(molmil.configBox.glsl_shaders);
            this.resizeViewPort();
            molmil.geometry.registerPrograms(this, true);
        };


        initShaders(programs) {
            var renderer = this;
            var name, fragmentShader, vertexShader, e, ploc, programs = molmil.configBox.glsl_shaders, exts, bad;
            for (var p = 0; p < programs.length; p++) {
                ploc = programs[p][0];
                name = programs[p][1] || ploc.replace(/\\/g, '/').replace(/.*\//, '').split(".")[0];
                exts = programs[p].length > 3 ? programs[p][3] : []; bad = false;
                for (var i = 0; i < exts.length; i++) {
                    if (!molmil.configBox[exts[i]]) molmil.configBox[exts[i]] = this.gl.getExtension(exts[i]);
                    bad = bad || !molmil.configBox[exts[i]];
                }
                if (bad) continue;
                if (!molmil.shaderEngine.code.hasOwnProperty(molmil.settings.src + ploc)) {
                    var request = new molmil_dep.CallRemote("GET"); request.ASYNC = false;
                    if (ploc.indexOf("//") == -1) request.Send(molmil.settings.src + ploc);
                    else request.Send(ploc);
                    molmil.shaderEngine.code[molmil.settings.src + ploc] = request.request.responseText;
                }
            }

            var name, fragmentShader, vertexShader, e, ploc, defines, vertDefines, fragDefines, programs = molmil.configBox.glsl_shaders, program, source;
            for (var p = 0; p < programs.length; p++) {
                ploc = programs[p][0];
                name = programs[p][1] || ploc.replace(/\\/g, '/').replace(/.*\//, '').split(".")[0];
                exts = programs[p].length > 3 ? programs[p][3] : []; bad = false;
                bad = false, vertDefines = "", fragDefines = "";
                for (var i = 0; i < exts.length; i++) {
                    if (!molmil.configBox[exts[i]]) bad = true;
                }
                if (bad) continue;
                e = molmil.shaderEngine.code[molmil.settings.src + ploc];
                if (!e) continue;
                source = e.split("//#");
                program = JSON.parse(source[0]);
                program.name = name;
                program.vertexShader = source[1].substr(7);
                program.fragmentShader = source[2].substr(9);
                program.defines = programs[p][2] || ""; program.vertDefines = vertDefines; program.fragDefines = fragDefines;
                program.compile = function (global_defines) {
                    this.program = renderer.gl.createProgram();
                    molmil.setupShader(renderer.gl, this.name + "_v", this.program, global_defines + this.vertDefines + this.defines + this.vertexShader, renderer.gl.VERTEX_SHADER);
                    molmil.setupShader(renderer.gl, this.name + "_f", this.program, global_defines + this.fragDefines + this.defines + this.fragmentShader, renderer.gl.FRAGMENT_SHADER);
                    renderer.gl.linkProgram(this.program);
                    if (!renderer.gl.getProgramParameter(this.program, renderer.gl.LINK_STATUS)) { console.log("Could not initialise shaders for " + this.name); }
                    renderer.gl.useProgram(this.program);
                    for (e in this.attributes) {
                        this.attributes[e] = renderer.gl.getAttribLocation(this.program, e);
                    }
                    for (e in this.uniforms) { this.uniforms[e] = renderer.gl.getUniformLocation(this.program, e); }
                };
                renderer.shaders[name] = program;
            }
            molmil.shaderEngine.recompile(renderer);
        }




        // ** updates the atom selection **
        updateSelection() {
            var selectionData = new Float32Array(this.soup.atomSelection.length * 8 * 6), rgb = [255, 255, 0];

            var r;

            for (var i = 0, p = 0, j; i < this.soup.atomSelection.length; i++) {
                if (this.soup.atomSelection[i].displayMode == 1) r = molmil_dep.getKeyFromObject(molmil.configBox.vdwR, this.soup.atomSelection[i].element, molmil.configBox.vdwR.DUMMY) * 1.1;
                else r = 0.5;

                for (j = 0; j < 6; j++) {
                    selectionData[p++] = this.soup.atomSelection[i].chain.modelsXYZ[this.modelId][this.soup.atomSelection[i].xyz];
                    selectionData[p++] = this.soup.atomSelection[i].chain.modelsXYZ[this.modelId][this.soup.atomSelection[i].xyz + 1];
                    selectionData[p++] = this.soup.atomSelection[i].chain.modelsXYZ[this.modelId][this.soup.atomSelection[i].xyz + 2];

                    selectionData[p++] = rgb[0];
                    selectionData[p++] = rgb[1];
                    selectionData[p++] = rgb[2];

                    if (j == 0 || j == 5) {
                        selectionData[p++] = -r;
                        selectionData[p++] = +r;
                    }
                    else if (j == 1) {
                        selectionData[p++] = +r;
                        selectionData[p++] = +r;
                    }
                    else if (j == 2 || j == 3) {
                        selectionData[p++] = +r;
                        selectionData[p++] = -r;
                    }
                    else if (j == 4) {
                        selectionData[p++] = -r;
                        selectionData[p++] = -r;
                    }
                }
            }

            if (!this.buffers.atomSelectionBuffer) this.buffers.atomSelectionBuffer = this.gl.createBuffer();
            this.gl.bindBuffer(this.gl.ARRAY_BUFFER, this.buffers.atomSelectionBuffer);
            this.gl.bufferData(this.gl.ARRAY_BUFFER, selectionData, this.gl.STATIC_DRAW);

            this.buffers.atomSelectionBuffer.items = this.soup.atomSelection.length * 6;
        };

        // create an api so that the buffers can be initialised for each model without uploading to the gpu...

        selectFrame(i, detail_or) {
            this.modelId = molmil.geometry.modelId = i;
            molmil.geometry.reInitChains = true;
            molmil.geometry.generate(this.soup.structures, this, detail_or);
            for (var i = 0; i < this.soup.texturedBillBoards.length; i++) this.soup.texturedBillBoards[i].dynamicsUpdate();
            this.initBD = true;
        }

        initBuffers() {
            molmil.geometry.generate(this.soup.structures, this);
            this.updateSelection();
            this.initBD = true;
        };

        // ** action when screen is clicked on **
        renderPicking() {
            if (!this.initBD) return;
            this.gl.clearColor(0, 0, 0, 0);
            this.gl.clear(this.gl.COLOR_BUFFER_BIT | this.gl.DEPTH_BUFFER_BIT);

            var COR = [0, 0, 0];
            var tmp = mat3.create(); mat3.fromMat4(tmp, this.modelViewMatrix);
            vec3.transformMat3(COR, this.soup.COR, tmp);

            for (var p = 0; p < this.programs.length; p++) this.programs[p].renderPicking(this.modelViewMatrix, COR);


            this.gl.useProgram(null);
        };

        renderPrograms(COR) {
            for (var p = 0; p < this.programs.length; p++) {
                if (this.programs[p].nElements && this.programs[p].status) {
                    if (this.programs[p].settings.alphaMode) this.programs[p].alphaMode_opaque_render(this.modelViewMatrix, COR);
                    else if (!this.programs[p].settings.alphaSet) this.programs[p].render(this.modelViewMatrix, COR);
                }
            }

            for (var p = 0; p < this.programs.length; p++) {
                if (this.programs[p].nElements && this.programs[p].status && (this.programs[p].settings.alphaMode || this.programs[p].settings.alphaSet)) this.programs[p].render(this.modelViewMatrix, COR);
            }

            // pass1: render only opaque meshes (standard)
            // pass2: render only transparent meshes to the z-buffer (so not to the frame buffer)
            // pass3: render only transparent meshes to frame buffer, alpha blending it (discard any pixel not equal to that in the depth buffer)

        }

        // ** renders the scene **
        render() {
            if (!this.canvas.update || !this.initBD) {
                if (molmil.vrDisplay) {
                    var frameData = new VRFrameData(); molmil.vrDisplay.getFrameData(frameData);
                    var curFramePose = frameData.pose;
                    if (curFramePose.position == null && curFramePose.orientation == null) {
                        if (molmil.vrDisplay.displayName == "HTC Vive DVT") molmil.vrDisplay.submitFrame();
                        // maybe the above also needs to be enabled for the fujitsu one...
                        return;
                    }
                }
                else return;
            }
            else if (molmil.vrDisplay) {
                var frameData = new VRFrameData(); molmil.vrDisplay.getFrameData(frameData);
                var curFramePose = frameData.pose;
            }

            // figure out whether the VR scene has been updated, if not -> return to save power (and prevent overheating...)

            if (frameData && this.camera.vrXYZupdated) {
                var tempMat = mat4.copy(mat4.create(), frameData.leftViewMatrix);
                tempMat[12] = tempMat[13] = tempMat[14];
                var invMat = mat4.invert(mat4.create(), tempMat);
                var trans = vec3.transformMat4([0, 0, 0], this.camera.vrXYZ, invMat);
                this.camera.x += trans[0]; this.camera.y += trans[1]; this.camera.z += trans[2];
                this.camera.vrXYZ[0] = this.camera.vrXYZ[1] = this.camera.vrXYZ[2] = 0.0;
                this.camera.vrXYZupdated = false;
            }
            else if (frameData && molmil.vrDisplay.displayName.toLowerCase().indexOf("cardboard") != -1) {
                var epsilon1 = 0.0001, epsilon2 = 0.1, epsilon_cutoff = 0.001;
                // now, use two versions of epsilon...
                if (this.camera.vrMatrix_backup) {
                    var a = this.camera.vrMatrix_backup, b = frameData.leftViewMatrix, now = Date.now();
                    var temp = [Math.abs(a[0] - b[0]), Math.abs(a[1] - b[1]), Math.abs(a[2] - b[2]), Math.abs(a[3] - b[3]), Math.abs(a[4] - b[4]), Math.abs(a[5] - b[5]), Math.abs(a[6] - b[6]), Math.abs(a[7] - b[7]), Math.abs(a[8] - b[8]), Math.abs(a[9] - b[9]), Math.abs(a[10] - b[10]), Math.abs(a[11] - b[11]), Math.abs(a[12] - b[12]), Math.abs(a[13] - b[13]), Math.abs(a[14] - b[14]), Math.abs(a[15] - b[15])], maxDiff;
                    maxDiff = Math.max.apply(null, temp);

                    if (maxDiff < this.camera.vrMatrix_epsilon) {
                        this.canvas.update = false;
                        return;
                    }

                    // let's render
                    if (this.camera.vrMatrix_epsilon == epsilon1) { // currently fast-polling mode
                        if (maxDiff < epsilon_cutoff) { // not moving that much (twilight zone)
                            if (this.camera.vrMatrix_timestamp + 2000 < now) { // so the matrix has been more-or-less stable at least for two seconds 
                                this.camera.vrMatrix_epsilon = epsilon2; // switch to slow-polling mode
                                this.camera.vrMatrix_timestamp = now; // reset the timer
                            }
                        }
                        else this.camera.vrMatrix_timestamp = now;
                    }
                    else { // currently low-polling mode
                        this.camera.vrMatrix_epsilon = epsilon1; // switch to fast-polling mode
                        this.camera.vrMatrix_timestamp = now; // reset the timer
                    }
                }

                if (!this.camera.vrMatrix_backup) { // only set this for cardboard renderer...
                    this.camera.vrMatrix_backup = mat4.create();
                    this.camera.vrMatrix_epsilon = epsilon1;
                    this.camera.vrMatrix_timestamp = now;
                }
                mat4.copy(this.camera.vrMatrix_backup, frameData.leftViewMatrix);
            }

            if (this.canvas.update || molmil.vrDisplay) {
                if (this.canvas.update != -1) {
                    for (var c = 0; c < this.soup.canvases.length; c++) if (this.soup.canvases[c] != this.canvas) this.soup.canvases[c].update = -1;
                }
                this.canvas.update = false;

                this.camera.pitchAngle = this.pitchAngle || this.pitch * Math.min(Math.max((Math.pow(Math.abs(this.camera.z), .1) * .25), .5), 2.);
                this.camera.headingAngle = this.headingAngle || this.heading * Math.min(Math.max((Math.pow(Math.abs(this.camera.z), .1) * .25), .5), 2.);
                this.camera.rollAngle = this.rollAngle || this.roll * Math.min(Math.max((Math.pow(Math.abs(this.camera.z), .1) * .25), .5), 2.);


                if (this.TransX || this.TransY) {
                    var invMat = mat4.invert(mat4.create(), this.projectionMatrix);
                    var zPos = (-this.camera.z - molmil.configBox.zNear) / (molmil.configBox.zFar - molmil.configBox.zNear);

                    var from = vec3.lerp([0, 0, 0], molmil.unproject(this.TransB[0], this.TransB[1], 0, invMat), molmil.unproject(this.TransB[0], this.TransB[1], 1, invMat), zPos);
                    this.TransB[0] += this.TransX / this.width; this.TransB[1] -= this.TransY / this.height;
                    var to = vec3.lerp([0, 0, 0], molmil.unproject(this.TransB[0], this.TransB[1], 0, invMat), molmil.unproject(this.TransB[0], this.TransB[1], 1, invMat), zPos);

                    this.camera.x += 2 * (to[0] - from[0]);
                    this.camera.y += 2 * (to[1] - from[1]);
                }
                this.camera.z += this.TransZ * Math.max(-this.camera.z / (this.width * .5), 0.05);

                if (this.camera.z > this.maxRange) this.camera.z = this.maxRange;
                if (this.camera.z < -(this.maxRange + molmil.configBox.zFar)) this.camera.z = -(this.maxRange + molmil.configBox.zFar);

                this.pitch = 0, this.heading = 0, this.TransX = 0, this.TransY = 0, this.TransZ = 0, this.pitchAngle = 0, this.headingAngle = 0, this.rollAngle = 0;

                this.camera.positionCamera();

                this.modelViewMatrix = this.camera.generateMatrix();

                if (molmil.configBox.projectionMode == 2) {
                    var zoomFraction = -(this.camera.z * 2) / molmil.configBox.zFar;
                    mat4.ortho(this.projectionMatrix, -this.width * zoomFraction, this.width * zoomFraction, -this.height * zoomFraction, this.height * zoomFraction, Math.max(molmil.configBox.zNear, 0.1), this.camera.z + (molmil.configBox.zFar * 10));
                }
            }
            else {
                var frameData = new VRFrameData();
                molmil.vrDisplay.getFrameData(frameData);
                this.canvas.update = false;
            }

            if (this.customFogRange) {
                this.fogStart = this.customFogRange[0];
                this.fogSpan = this.customFogRange[1];
            }
            else {
                this.clearCut = ((1.0 - Math.abs(this.camera.QView[0])) * this.soup.stdXYZ[0]) + ((1.0 - Math.abs(this.camera.QView[1])) * this.soup.stdXYZ[1]) + ((1.0 - Math.abs(this.camera.QView[2])) * this.soup.stdXYZ[2]);
                this.fogStart = -this.camera.z - (this.clearCut * .5);
                if (this.fogStart < molmil.configBox.zNear + 1) this.fogStart = molmil.configBox.zNear + 1;
                this.fogSpan = this.fogStart + (this.clearCut * 2);
            }

            // rendering
            var BGCOLOR = molmil.configBox.BGCOLOR.slice();
            if (BGCOLOR[3] == 0) BGCOLOR[0] = BGCOLOR[1] = BGCOLOR[2] = 0;

            var COR = [0, 0, 0];
            var tmp = mat3.create(); mat3.fromMat4(tmp, this.modelViewMatrix);
            vec3.transformMat3(COR, this.soup.COR, tmp);

            if (molmil.configBox.jitRenderFunc) molmil.configBox.jitRenderFunc.apply(this, [{ frameData: frameData }]);

            if (molmil.configBox.stereoMode && (molmil.configBox.stereoMode != 3 || molmil.vrDisplay)) {
                if (!this.FBOs.hasOwnProperty("stereoLeft")) {
                    this.FBOs.stereoLeft = new molmil.FBO(this.gl, this.width, this.height);
                    this.FBOs.stereoLeft.multisample = "stereoLeft";
                    this.FBOs.stereoLeft.addTexture("stereoLeft", this.gl.RGBA, this.gl.RGBA);//GL2.GL_RGB32F, this.gl.GL_RGB
                    this.FBOs.stereoLeft.setup();

                    this.FBOs.stereoRight = new molmil.FBO(this.gl, this.width, this.height);
                    this.FBOs.stereoRight.multisample = "stereoRight";
                    this.FBOs.stereoRight.addTexture("stereoRight", this.gl.RGBA, this.gl.RGBA);//GL2.GL_RGB32F, this.gl.GL_RGB
                    this.FBOs.stereoRight.setup();
                }

                if (molmil.configBox.stereoMode != 3) {

                    var tmpVal = this.modelViewMatrix[12];
                    var sCC = molmil.configBox.stereoCameraConfig; //[bottom, top, a, b, c, eyeSep, convergence, zNear, zfar];
                    var left, right, zNear = sCC[7], zFar = sCC[8];

                    // left eye
                    this.modelViewMatrix[12] = tmpVal - sCC[5] * .5;

                    left = -sCC[3] * (zNear / sCC[6]);
                    right = sCC[4] * (zNear / sCC[6]);
                    mat4.frustum(this.projectionMatrix, left, right, sCC[0], sCC[1], zNear, zFar);
                }
                else var tmpMMat = mat4.create(), tmp2MMat = mat4.create(), tmp3MVec = vec3.create();

                if (molmil.configBox.stereoMode == 1) { // anaglyph
                    this.FBOs.stereoLeft.bind();
                    this.gl.clearColor.apply(this.gl, BGCOLOR); this.gl.clear(this.gl.COLOR_BUFFER_BIT | this.gl.DEPTH_BUFFER_BIT);
                    this.renderPrograms(COR);
                    this.FBOs.stereoLeft.post();
                    this.FBOs.stereoLeft.unbind();
                }
                else if (molmil.configBox.stereoMode == 2) { // side-by-side
                    this.gl.viewport(this.width, 0, this.width, this.height);
                    this.gl.scissor(this.width, 0, this.width, this.height);
                    this.gl.clearColor.apply(this.gl, BGCOLOR); this.gl.clear(this.gl.COLOR_BUFFER_BIT | this.gl.DEPTH_BUFFER_BIT);
                    this.renderPrograms(COR);
                }
                else if (molmil.configBox.stereoMode == 4) { // crossed-eye
                    this.gl.viewport(0, 0, this.width, this.height);
                    this.gl.scissor(0, 0, this.width, this.height);
                    this.gl.clearColor.apply(this.gl, BGCOLOR); this.gl.clear(this.gl.COLOR_BUFFER_BIT | this.gl.DEPTH_BUFFER_BIT);
                    this.renderPrograms(COR);
                }
                else if (molmil.configBox.stereoMode == 3) { // webvr
                    mat4.copy(tmp2MMat, frameData.leftViewMatrix);
                    tmp2MMat[12] *= 100; tmp2MMat[13] *= 100; tmp2MMat[14] *= 100;

                    tmp3MVec[0] = tmp2MMat[12] - frameData.leftViewMatrix[12];
                    tmp3MVec[1] = tmp2MMat[13] - frameData.leftViewMatrix[13];
                    tmp3MVec[2] = tmp2MMat[14] - frameData.leftViewMatrix[14];

                    if (molmil.configBox.altVR) mat4.multiply(tmpMMat, this.modelViewMatrix, tmp2MMat);
                    else mat4.multiply(tmpMMat, tmp2MMat, this.modelViewMatrix);

                    var tmp = mat3.create(); mat3.fromMat4(tmp, tmpMMat);
                    vec3.transformMat3(COR, this.soup.COR, tmp);
                    this.projectionMatrix = frameData.leftProjectionMatrix;
                    this.gl.viewport(0, 0, this.width * 0.5, this.height);

                    this.gl.clear(this.gl.COLOR_BUFFER_BIT | this.gl.DEPTH_BUFFER_BIT);
                    for (var p = 0; p < this.programs.length; p++) if (this.programs[p].nElements && this.programs[p].status) this.programs[p].render(tmpMMat, COR);
                    this.billboardProgram.render(tmpMMat, COR);
                }

                if (molmil.configBox.stereoMode != 3) {
                    // right eye
                    this.modelViewMatrix[12] = tmpVal + sCC[5] * .5;

                    left = -sCC[4] * (zNear / sCC[6]);
                    right = sCC[3] * (zNear / sCC[6]);
                    mat4.frustum(this.projectionMatrix, left, right, sCC[0], sCC[1], zNear, zFar);
                }

                if (molmil.configBox.stereoMode == 1) { // anaglyph
                    this.FBOs.stereoRight.bind();
                    this.gl.clearColor.apply(this.gl, BGCOLOR); this.gl.clear(this.gl.COLOR_BUFFER_BIT | this.gl.DEPTH_BUFFER_BIT);
                    this.renderPrograms(COR);
                    this.FBOs.stereoRight.post();
                    this.FBOs.stereoRight.unbind();
                }
                else if (molmil.configBox.stereoMode == 2) { // side-by-side
                    this.gl.viewport(0, 0, this.width, this.height);
                    this.gl.scissor(0, 0, this.width, this.height);
                    this.renderPrograms(COR);
                }
                else if (molmil.configBox.stereoMode == 4) { // crossed-eye
                    this.gl.viewport(this.width, 0, this.width, this.height);
                    this.gl.scissor(this.width, 0, this.width, this.height);
                    this.renderPrograms(COR);
                }
                else if (molmil.configBox.stereoMode == 3) { // webvr
                    mat4.copy(tmp2MMat, frameData.rightViewMatrix);
                    tmp2MMat[12] += tmp3MVec[0];
                    tmp2MMat[13] += tmp3MVec[1];
                    tmp2MMat[14] += tmp3MVec[2];

                    if (molmil.configBox.altVR) mat4.multiply(tmpMMat, this.modelViewMatrix, tmp2MMat);
                    else mat4.multiply(tmpMMat, tmp2MMat, this.modelViewMatrix);

                    var tmp = mat3.create(); mat3.fromMat4(tmp, tmpMMat);
                    vec3.transformMat3(COR, this.soup.COR, tmp);
                    this.projectionMatrix = frameData.rightProjectionMatrix;
                    this.gl.viewport(this.width * 0.5, 0, this.width * 0.5, this.height);

                    var tmp = mat3.create(); mat3.fromMat4(tmp, tmpMMat);
                    vec3.transformMat3(COR, this.soup.COR, tmp);

                    for (var p = 0; p < this.programs.length; p++) if (this.programs[p].nElements && this.programs[p].status) this.programs[p].render(tmpMMat, COR);
                    this.billboardProgram.render(tmpMMat, COR);
                }

                if (molmil.configBox.stereoMode == 1) {
                    var shader = this.shaders.anaglyph;

                    this.gl.clearColor.apply(this.gl, BGCOLOR); this.gl.clear(this.gl.COLOR_BUFFER_BIT | this.gl.DEPTH_BUFFER_BIT);

                    this.gl.useProgram(shader.program);

                    this.FBOs.stereoLeft.bindTextureToUniform("stereoLeft", shader.uniforms.stereoLeft, 0);
                    this.FBOs.stereoRight.bindTextureToUniform("stereoRight", shader.uniforms.stereoRight, 1);

                    var buffer = this.gl.createBuffer();
                    this.gl.bindBuffer(this.gl.ARRAY_BUFFER, buffer);
                    this.gl.bufferData(this.gl.ARRAY_BUFFER, new Float32Array([-1.0, -1.0, 1.0, -1.0, -1.0, 1.0, -1.0, 1.0, 1.0, -1.0, 1.0, 1.0]), this.gl.STATIC_DRAW);

                    molmil.resetAttributes(this.gl);
                    molmil.bindAttribute(this.gl, shader.attributes.in_Position, 2, this.gl.FLOAT, false, 0, 0);
                    molmil.clearAttributes(this.gl);

                    this.gl.drawArrays(this.gl.TRIANGLES, 0, 6);
                }

                // reset
                if (molmil.configBox.stereoMode != 3) this.modelViewMatrix[12] = tmpVal;
            }
            else {
                this.gl.clearColor.apply(this.gl, BGCOLOR);
                this.gl.clear(this.gl.COLOR_BUFFER_BIT | this.gl.DEPTH_BUFFER_BIT);
                this.renderPrograms(COR);
                this.billboardProgram.render(this.modelViewMatrix, COR);
            }

            if (this.buffers.atomSelectionBuffer) { // this doesn't work properly with stereoscopy...
                this.renderAtomSelection(this.modelViewMatrix, COR);
            }

            if (molmil.configBox.stereoMode == 3 && molmil.vrDisplay) {
                molmil.vrDisplay.submitFrame();
            }

            if (this.onRenderFinish) this.onRenderFinish();

            this.gl.useProgram(null);
        }

        initFBOs() {
            if ("scene" in this.FBOs) {
                this.FBOs.depth.resize(this.width, this.height);
                this.FBOs.scene.resize(this.width, this.height);
            }
            else {
                this.FBOs.depth = new molmil.FBO(this.gl, this.width, this.height);
                this.FBOs.depth.addTexture("depthBuffer", this.gl.RGBA, this.gl.RGBA);//GL2.GL_RGB32F, this.gl.GL_RGB
                this.FBOs.depth.setup();

                this.FBOs.scene = new molmil.FBO(this.gl, this.width, this.height);
                this.FBOs.scene.addTexture("colourBuffer", this.gl.RGBA, this.gl.RGBA);//GL2.GL_RGB32F, this.gl.GL_RGB
                this.FBOs.scene.setup();
            }
        };

        resizeViewPort() {
            this.width = this.canvas.width; this.height = this.canvas.height;
            if (molmil.configBox.stereoMode == 2 || molmil.configBox.stereoMode == 4) this.width *= 0.5;
            if (molmil.configBox.projectionMode == 1) {
                mat4.perspective(this.projectionMatrix, molmil.configBox.camera_fovy * (Math.PI / 180), this.width / this.height, molmil.configBox.zNear, molmil.configBox.zFar);
            }

            var convergence = molmil.configBox.zNear * molmil.configBox.stereoFocalFraction;
            var eyeSep = convergence / molmil.configBox.stereoEyeSepFraction;
            var top, bottom, a, b, c;
            top = molmil.configBox.zNear * Math.tan(molmil.configBox.camera_fovy) * .25;
            bottom = -top;
            a = (this.width / this.height) * Math.tan(molmil.configBox.camera_fovy) * convergence;
            b = a - (eyeSep / 2);
            c = a + (eyeSep / 2);

            delete this.FBOs.stereoLeft;

            molmil.configBox.stereoCameraConfig = [bottom, top, a, b, c, eyeSep, convergence, molmil.configBox.zNear * .25, molmil.configBox.zFar * .25];

            this.gl.viewport(0, 0, this.width, this.height);
            //this.initFBOs();
        };

        renderAtomSelection(modelViewMatrix, COR) {
            if (!this.buffers.atomSelectionBuffer.items) return;
            this.gl.useProgram(this.shaders.atomSelection.program);
            this.gl.uniform3f(this.shaders.atomSelection.uniforms.COR, COR[0], COR[1], COR[2]);
            this.gl.uniformMatrix4fv(this.shaders.atomSelection.uniforms.modelViewMatrix, false, modelViewMatrix);
            this.gl.uniformMatrix4fv(this.shaders.atomSelection.uniforms.projectionMatrix, false, this.projectionMatrix);

            this.gl.bindBuffer(this.gl.ARRAY_BUFFER, this.buffers.atomSelectionBuffer);

            molmil.resetAttributes(this.gl);
            molmil.bindAttribute(this.gl, this.shaders.atomSelection.attributes.in_Position, 3, this.gl.FLOAT, false, 32, 0);
            molmil.bindAttribute(this.gl, this.shaders.atomSelection.attributes.in_Colour, 3, this.gl.FLOAT, false, 32, 12);
            molmil.bindAttribute(this.gl, this.shaders.atomSelection.attributes.in_ScreenSpaceOffset, 2, this.gl.FLOAT, false, 32, 24);
            molmil.clearAttributes(this.gl);


            this.gl.enable(this.gl.BLEND); if (molmil.configBox.cullFace) { this.gl.disable(this.gl.CULL_FACE); }
            this.gl.blendEquation(this.gl.FUNC_ADD); this.gl.blendFunc(this.gl.SRC_ALPHA, this.gl.ONE_MINUS_SRC_ALPHA);
            this.gl.depthMask(false);

            this.gl.drawArrays(this.gl.TRIANGLES, 0, this.buffers.atomSelectionBuffer.items);

            if (molmil.configBox.cullFace) { this.gl.enable(this.gl.CULL_FACE); } this.gl.disable(this.gl.BLEND);
            this.gl.disable(this.gl.BLEND);
            this.gl.depthMask(true);
        };

    }
}