namespace molmil {

    export function setOnContextMenu(obj, func, lefttoo) {
        obj.oncontextmenu = func;
        if (lefttoo) obj.onclick = func;

        obj.addEventListener("touchstart", molmil.handle_contextMenu_touchStart, false);
        obj.addEventListener("touchend", molmil.handle_contextMenu_touchEnd, false);
    }

    // ** drag-and-drop support for various files **
    export function bindCanvasInputs(canvas) {
        if (!canvas.molmilViewer.UI) {
            canvas.molmilViewer.UI = new molmil.UI(canvas.molmilViewer);
            canvas.molmilViewer.UI.init();
            canvas.molmilViewer.animation = new molmil.animationObj(canvas.molmilViewer);
        }

        var mjsFunc = function (canvas, fr) {
            if (fr.filename.endsWith(".mjs")) {
                fr.onload = function (e) {
                    canvas.commandLine.environment.scriptUrl = "";
                    canvas.commandLine.environment.console.runCommand(e.target.result.replace(/\/\*[\s\S]*?\*\/|([^:]|^)\/\/.*$/gm, ""));
                }
                fr.readAsText(fr.fileHandle);
                return true;
            }
        };

        var cancelDB = function (ev) {
            ev.preventDefault();
            return false;
        }

        var nfilesproc = [0, 0, []];
        var renderOnlyFinal = function (soup, structures) {
            nfilesproc[0]++;
            if (Array.isArray(structures)) nfilesproc[2] = nfilesproc[2].concat(structures)
            else nfilesproc[2].push(structures);
            if (nfilesproc[0] < nfilesproc[1]) return;
            molmil.displayEntry(nfilesproc[2], 1);
            molmil.colorEntry(nfilesproc[2], 1, null, true, soup);
            nfilesproc[2] = [];
        }

        var dropDB = function (ev) {
            ev.preventDefault();
            processFiles(ev.dataTransfer.items, ev.dataTransfer.files);
            return false;
        }

        var pasteDB = function (ev) {
            if (ev.srcElement.className.includes("molmil_UI_cl_input")) return;
            ev.preventDefault();
            processFiles([], ev.clipboardData.files);
        };

        var processFiles = function (files, files2) {
            var fr, i, j, mjsFile = null, file;

            var dict = {}, item, entry, bakacounter = 0;

            var bakacheck = function () {
                if (bakacounter > 0) return;

                var items = Object.keys(dict).sort(molmil_dep.naturalSort);
                var count = items.length;
                for (i = 0; i < count; i++) {
                    if (items[i].toLowerCase().endsWith(".mjs")) { mjsFile = items[i]; break; }
                }

                if (mjsFile != null) {
                    canvas.mjs_fileBin = {};
                    for (i = 0; i < count; i++) {
                        file = dict[items[i]];
                        if (file instanceof File) {
                            fr = new FileReader();
                            fr.filename = file.name;
                            fr.fileHandle = file;
                        }
                        else fr = file;
                        canvas.mjs_fileBin[items[i]] = fr;
                    }
                    mjsFunc(canvas, canvas.mjs_fileBin[mjsFile]);
                    return false;
                }
                nfilesproc[1] = count;

                for (i = 0; i < count; i++) {
                    file = dict[items[i]];
                    if (file instanceof File) {
                        fr = new FileReader();
                        fr.filename = file.name;
                        fr.fileHandle = file;
                    }
                    else fr = file;
                    var ok = false;

                    for (j = 0; j < canvas.inputFunctions.length; j++) {
                        if (canvas.inputFunctions[j](canvas, fr)) { ok = true; break; }
                    }

                    if (!ok) {
                        for (var j in molmil.formatList) {
                            if (typeof molmil.formatList[j] != "function" || !cmd[1].endsWith(j)) continue;
                            molmil.loadFilePointer(fr, molmil.formatList[j], canvas);
                            break;
                        }
                    }
                }


            };

            var processEntry = function (item) {
                if (item.isFile) {
                    bakacounter += 1;
                    item.file(function (baka) {
                        dict[item.fullPath.replace(/^\//, "")] = baka;
                        bakacounter -= 1;
                        bakacheck();
                    });
                }
                if (!item.isDirectory) return;
                var directoryReader = item.createReader();
                bakacounter += 1;
                directoryReader.readEntries(function (entries) { entries.forEach(processEntry); bakacounter -= 1; });
            };

            for (var i = 0; i < files.length; i++) {
                item = files[i];
                if (item.kind != 'file') continue;
                if (item.getAsEntry) entry = item.getAsEntry();
                else if (item.webkitGetAsEntry) entry = item.webkitGetAsEntry();
                else { dict = null; break; } // not supported
                processEntry(entry);
            }

            if (dict == null || files.length == 0) {
                var count = 0, files = [];
                try {
                    files = files2;
                    count = files.length;
                } catch (e) { }

                var dict = {};
                for (i = 0; i < count; i++) dict[files[i].name] = files[i];
                bakacheck();
            }
        };



        window.addEventListener("paste", pasteDB);
        canvas.addEventListener("dragover", cancelDB);
        canvas.addEventListener("dragenter", cancelDB);
        canvas.addEventListener("drop", dropDB);

        // mjs file
        canvas.inputFunctions.push(mjsFunc);

        // .gz file
        canvas.inputFunctions.push(function (canvas, fr) {
            if (fr.filename.endsWith(".gz")) {
                if (!window.hasOwnProperty("pako")) { var obj = molmil_dep.dcE("script"); obj.src = molmil.settings.src + "lib/pako.js"; document.getElementsByTagName("head")[0].appendChild(obj); }
                fr.onload = function (e) {
                    if (!window.hasOwnProperty("pako")) return molmil_dep.asyncStart(this.onload, [e], this, 50);

                    var fakeObj = { filename: fr.filename.substr(0, fr.filename.length - 3) };
                    fakeObj.readAsText = function () { var out = pako.inflate(new Uint8Array(e.target.result), { to: "string" }); this.onload({ target: { result: out } }); };
                    fakeObj.readAsArrayBuffer = function () { var out = pako.inflate(new Uint8Array(e.target.result)); this.onload({ target: { result: out.buffer } }); };

                    for (j = 0; j < canvas.inputFunctions.length; j++) {
                        if (canvas.inputFunctions[j](canvas, fakeObj)) break;
                    }
                }
                canvas.molmilViewer.downloadInProgress = true;
                fr.readAsArrayBuffer(fr.fileHandle);
                return true;
            }
        });

        // .zip file
        canvas.inputFunctions.push(function (canvas, fr) {
            if (fr.filename.endsWith(".zip")) {
                if (!window.hasOwnProperty("unzip")) { var obj = molmil_dep.dcE("script"); obj.src = molmil.settings.src + "lib/unzipit.min.js"; document.getElementsByTagName("head")[0].appendChild(obj); }
                fr.onload = function (e) {
                    if (!window.hasOwnProperty("unzipit")) return molmil_dep.asyncStart(this.onload, [e], this, 50);
                    unzipit.unzip(e.target.result).then(function (zipfile) {
                        var files = Object.values(zipfile.entries), files2 = [];
                        for (var f = 0; f < files.length; f++) {
                            if (files[f].isDirectory) continue;
                            files2.push({
                                name: files[f].name,
                                filename: files[f].name,
                                fileObj: files[f],
                                onload: function () { },
                                readAsText: function () {
                                    var fobj = this;
                                    this.fileObj.text().then(function (data) { fobj.onload({ target: { result: data } }); });
                                },
                                readAsArrayBuffer: function () {
                                    var fobj = this;
                                    this.fileObj.arrayBuffer().then(function (data) { fobj.onload({ target: { result: data } }); });
                                }
                            });
                        }
                        processFiles([], files2);
                        canvas.molmilViewer.downloadInProgress = false;
                    });
                }
                fr.readAsArrayBuffer(fr.fileHandle);
                canvas.molmilViewer.downloadInProgress = true;
                return true;
            }
        });


        // pdb file
        canvas.inputFunctions.push(function (canvas, fr) {
            if (fr.filename.endsWith(".pdb") || fr.filename.endsWith(".ent")) {
                fr.onload = function (e) {
                    canvas.molmilViewer.loadStructureData(e.target.result, 4, this.filename, renderOnlyFinal);
                    delete canvas.molmilViewer.downloadInProgress;
                }
                canvas.molmilViewer.downloadInProgress = true;
                fr.readAsText(fr.fileHandle);
                return true;
            }
        });

        // mmtf file
        canvas.inputFunctions.push(function (canvas, fr) {
            if (fr.filename.endsWith(".mmtf")) {
                fr.onload = function (e) {
                    canvas.molmilViewer.loadStructureData(e.target.result, "mmtf", this.filename, renderOnlyFinal);
                    delete canvas.molmilViewer.downloadInProgress;
                }
                canvas.molmilViewer.downloadInProgress = true;
                fr.readAsText(fr.fileHandle);
                return true;
            }
        });

        // cif file
        canvas.inputFunctions.push(function (canvas, fr) {
            if (fr.filename.endsWith(".cif")) {
                fr.onload = function (e) {
                    canvas.molmilViewer.loadStructureData(e.target.result, 'cif', this.filename, renderOnlyFinal);
                    delete canvas.molmilViewer.downloadInProgress;
                }
                canvas.molmilViewer.downloadInProgress = true;
                fr.readAsText(fr.fileHandle);
                return true;
            }
        });

        // gro file
        canvas.inputFunctions.push(function (canvas, fr) {
            if (fr.filename.endsWith(".gro")) {
                fr.onload = function (e) {
                    canvas.molmilViewer.loadStructureData(e.target.result, 7, this.filename, renderOnlyFinal);
                    delete canvas.molmilViewer.downloadInProgress;
                }
                canvas.molmilViewer.downloadInProgress = true;
                fr.readAsText(fr.fileHandle);
                return true;
            }
        });

        // gromacs trajectory file (trr)
        canvas.inputFunctions.push(function (canvas, fr) {
            if (fr.filename.endsWith(".trr")) {
                fr.onload = function (e) {
                    canvas.molmilViewer.loadStructureData(e.target.result, "gromacs-trr", this.filename, renderOnlyFinal);
                    delete canvas.molmilViewer.downloadInProgress;
                }
                canvas.molmilViewer.downloadInProgress = true;
                fr.readAsArrayBuffer(fr.fileHandle);
                return true;
            }
        });

        // gromacs trajectory file (xtc)
        canvas.inputFunctions.push(function (canvas, fr) {
            if (fr.filename.endsWith(".xtc")) {
                fr.onload = function (e) {
                    canvas.molmilViewer.loadStructureData(e.target.result, "gromacs-xtc", this.filename, renderOnlyFinal);
                    delete canvas.molmilViewer.downloadInProgress;
                }
                canvas.molmilViewer.downloadInProgress = true;
                fr.readAsArrayBuffer(fr.fileHandle);
                return true;
            }
        });

        // mypresto trajectory file
        canvas.inputFunctions.push(function (canvas, fr) {
            if (fr.filename.endsWith(".cor") || fr.filename.endsWith(".cod")) {
                fr.onload = function (e) {
                    canvas.molmilViewer.loadStructureData(e.target.result, "presto-traj", this.filename, renderOnlyFinal);
                    delete canvas.molmilViewer.downloadInProgress;
                }
                canvas.molmilViewer.downloadInProgress = true;
                fr.readAsArrayBuffer(fr.fileHandle);
                return true;
            }
        });

        // mypresto mnt file
        canvas.inputFunctions.push(function (canvas, fr) {
            if (fr.filename.endsWith(".mnt")) {
                fr.onload = function (e) {
                    canvas.molmilViewer.loadStructureData(e.target.result, "presto-mnt", this.filename, renderOnlyFinal);
                    delete canvas.molmilViewer.downloadInProgress;
                }
                canvas.molmilViewer.downloadInProgress = true;
                fr.readAsArrayBuffer(fr.fileHandle);
                return true;
            }
        });

        // mpbf
        canvas.inputFunctions.push(function (canvas, fr) {
            if (fr.filename.endsWith(".mpbf")) {
                fr.onload = function (e) {
                    canvas.molmilViewer.loadStructureData(e.target.result, 8, this.filename, renderOnlyFinal);
                    delete canvas.molmilViewer.downloadInProgress;
                }
                canvas.molmilViewer.downloadInProgress = true;
                fr.readAsArrayBuffer(fr.fileHandle);
                return true;
            }
        });

        // ccp4
        canvas.inputFunctions.push(function (canvas, fr) {
            if (fr.filename.endsWith(".ccp4")) {
                fr.onload = function (e) {
                    canvas.molmilViewer.UI.ccp4_input_popup(e.target.result, this.filename, renderOnlyFinal);
                    delete canvas.molmilViewer.downloadInProgress;
                }
                canvas.molmilViewer.downloadInProgress = true;
                fr.readAsArrayBuffer(fr.fileHandle);
                return true;
            }
        });

        // mdl
        canvas.inputFunctions.push(function (canvas, fr) {
            if (fr.filename.endsWith(".mdl") || fr.filename.endsWith(".mol") || fr.filename.endsWith(".sdf")) {
                fr.onload = function (e) {
                    canvas.molmilViewer.loadStructureData(e.target.result, 'mdl', this.filename, renderOnlyFinal);
                    delete canvas.molmilViewer.downloadInProgress;
                }
                canvas.molmilViewer.downloadInProgress = true;
                fr.readAsText(fr.fileHandle);
                return true;
            }
        });

        // mol2
        canvas.inputFunctions.push(function (canvas, fr) {
            if (fr.filename.endsWith(".mol2")) {
                fr.onload = function (e) {
                    canvas.molmilViewer.loadStructureData(e.target.result, 'mol2', this.filename, renderOnlyFinal);
                    delete canvas.molmilViewer.downloadInProgress;
                }
                canvas.molmilViewer.downloadInProgress = true;
                fr.readAsText(fr.fileHandle);
                return true;
            }
        });

        // xyz
        canvas.inputFunctions.push(function (canvas, fr) {
            if (fr.filename.endsWith(".xyz")) {
                fr.onload = function (e) {
                    canvas.molmilViewer.UI.xyz_input_popup(e.target.result, this.filename, renderOnlyFinal);
                    delete canvas.molmilViewer.downloadInProgress;
                }
                canvas.molmilViewer.downloadInProgress = true;
                fr.readAsText(fr.fileHandle);
                return true;
            }
        });

        // obj
        canvas.inputFunctions.push(function (canvas, fr) {
            if (fr.filename.endsWith(".obj")) {
                fr.onload = function (e) {
                    canvas.molmilViewer.loadStructureData(e.target.result, "obj", this.filename, renderOnlyFinal);
                    delete canvas.molmilViewer.downloadInProgress;
                }
                canvas.molmilViewer.downloadInProgress = true;
                fr.readAsText(fr.fileHandle);
                return true;
            }
        });

        // wrl
        canvas.inputFunctions.push(function (canvas, fr) {
            if (fr.filename.endsWith(".wrl")) {
                fr.onload = function (e) {
                    canvas.molmilViewer.loadStructureData(e.target.result, "wrl", this.filename, renderOnlyFinal);
                    delete canvas.molmilViewer.downloadInProgress;
                }
                canvas.molmilViewer.downloadInProgress = true;
                fr.readAsText(fr.fileHandle);
                return true;
            }
        });
    };

    // ** video support **
    export function initVideo(UI) {
        if (window.initVideo) {
            molmil_dep.asyncStart(UI.videoRenderer, [], UI, 0);
            return;
        }
        if (molmil.settings.molmil_video_url === undefined && window.SharedArrayBuffer !== undefined) {
            var head = document.getElementsByTagName("head")[0];
            var obj = molmil_dep.dcE("script"); obj.src = molmil.settings.src + "lib/ffmpeg_handler.js";
            obj.onload = function () { UI.videoRenderer(); };
            head.appendChild(obj);
            return;
        }
        if (molmil.settings.molmil_video_url === undefined) {
            console.error("Current configuration is not compatible with video output...");
            return;
        }
        var request = new molmil_dep.CallRemote("POST"); request.ASYNC = true; request.UI = UI;
        request.OnDone = function () {
            var jso = JSON.parse(this.request.responseText);
            if (!jso.found) return this.OnError();
            molmil_dep.asyncStart(this.UI.videoRenderer, [], this.UI, 0);
        };
        request.OnError = function () {
            alert("The support server to construct the video could not be found...");
        };
        request.Send(molmil.settings.molmil_video_url + "has_molmil_video_support");
    }
}