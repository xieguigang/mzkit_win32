/// <reference path="molmil_dep.d.ts" />
declare namespace molmil {
    const alignInfo: {};
    function align(A: any, B: any, options: any): any;
}
declare namespace molmil {
    export function checkRebuild(): void;
    interface environment {
        fileObjects: {};
        setTimeout: any;
        clearTimeout: any;
        navigator: any;
        window: any;
        console: any;
        cli_canvas: any;
        cli_soup: any;
        global: any;
    }
    export const commandLines: {};
    export class commandLine {
        environment: environment;
        commandBuffer: any[];
        constructor(canvas: any);
        buildGUI(): void;
        eval(command: any): void;
        wait4Download(): void;
        runCommand(command: any): void;
        bindNullInterface(): void;
        bindPDBjViewerInterface(): any;
    }
    export function color2rgba(clr: any): any;
    export function xhr(url: any): any;
    export function loadScript(url: any): void;
    export function isBalancedStatement(string: any): boolean;
    export {};
}
declare namespace molmil {
    var canvasList: any[];
    var mouseDown: boolean;
    var mouseDownS: {};
    var mouseMoved: boolean;
    var Xcoord: number;
    var Ycoord: number;
    var Zcoord: number;
    var activeCanvas: any;
    var touchList: any;
    var touchMode: boolean;
    var preRenderFuncs: any[];
    var longTouchTID: any;
    var previousTouchEvent: any;
    var ignoreBlackList: boolean;
    var vrDisplay: any;
    var vrPose: number[];
    var vrOrient: number[];
    var pdbj_data: string;
    const settings_default: {
        src: string;
        pdb_url: string;
        pdb_chain_url: string;
        comp_url: string;
        data_url: string;
        newweb_rest: string;
        molmil_video_url: any;
        dependencies: string[];
    };
    var cli_canvas: any;
    var cli_soup: any;
    var settings: any;
    const configBox: {
        version: number;
        buildDate: any;
        initFinished: boolean;
        liteMode: boolean;
        cullFace: boolean;
        loadModelsSeparately: boolean;
        wheelZoomRequiresCtrl: boolean;
        recordingMode: boolean;
        save_pdb_chain_only_change: boolean;
        xna_force_C1p: any;
        vdwR: {
            DUMMY: number;
            H: number;
            He: number;
            Li: number;
            Be: number;
            B: number;
            C: number;
            N: number;
            O: number;
            F: number;
            Ne: number;
            Na: number;
            Mg: number;
            Al: number;
            Si: number;
            P: number;
            S: number;
            Cl: number;
            Ar: number;
            K: number;
            Ca: number;
            Sc: number;
            Ti: number;
            V: number;
            Cr: number;
            Mn: number;
            Fe: number;
            Ni: number;
            Cu: number;
            Zn: number;
            Ga: number;
            Ge: number;
            As: number;
            Se: number;
            Br: number;
            Kr: number;
            Rb: number;
            Sr: number;
            Pd: number;
            Ag: number;
            Cd: number;
            In: number;
            Sn: number;
            Sb: number;
            Te: number;
            I: number;
            Xe: number;
            Cs: number;
            Ba: number;
            Pt: number;
            Au: number;
            Hg: number;
            Tl: number;
            Pb: number;
            Bi: number;
            Po: number;
            At: number;
            Rn: number;
            Fr: number;
            Ra: number;
            U: number;
        };
        MW: {
            H: number;
            He: number;
            Li: number;
            Be: number;
            B: number;
            C: number;
            N: number;
            O: number;
            F: number;
            Ne: number;
            Na: number;
            Mg: number;
            Al: number;
            Si: number;
            P: number;
            S: number;
            Cl: number;
            K: number;
            Ar: number;
            Ca: number;
            Sc: number;
            Ti: number;
            V: number;
            Cr: number;
            Mn: number;
            Fe: number;
            Ni: number;
            Co: number;
            Cu: number;
            Zn: number;
            Ga: number;
            Ge: number;
            As: number;
            Se: number;
            Br: number;
            Kr: number;
            Rb: number;
            Sr: number;
            Y: number;
            Zr: number;
            Nb: number;
            Mo: number;
            Tc: number;
            Ru: number;
            Rh: number;
            Pd: number;
            Ag: number;
            Cd: number;
            In: number;
            Sn: number;
            Sb: number;
            I: number;
            Te: number;
            Xe: number;
            Cs: number;
            Ba: number;
            La: number;
            Ce: number;
            Pr: number;
            Nd: number;
            Pm: number;
            Sm: number;
            Eu: number;
            Gd: number;
            Tb: number;
            Dy: number;
            Ho: number;
            Er: number;
            Tm: number;
            Yb: number;
            Lu: number;
            Hf: number;
            Ta: number;
            W: number;
            Re: number;
            Os: number;
            Ir: number;
            Pt: number;
            Au: number;
            Hg: number;
            Tl: number;
            Pb: number;
            Bi: number;
            Po: number;
            At: number;
            Rn: number;
            Fr: number;
            Ra: number;
            Ac: number;
            Pa: number;
            Th: number;
            Np: number;
            U: number;
            Am: number;
            Pu: number;
            Cm: number;
            Bk: number;
            Cf: number;
            Es: number;
            Fm: number;
            Md: number;
            No: number;
            Rf: number;
            Lr: number;
            Db: number;
            Bh: number;
            Sg: number;
            Mt: number;
            Rg: number;
            Hs: number;
        };
        sndStrucInfo: {
            1: number[];
            2: number[];
            3: number[];
            4: number[];
        };
        zNear: number;
        zFar: number;
        QLV_SETTINGS: {
            SPHERE_TESS_LV: number;
            CB_NOI: number;
            CB_NOVPR: number;
            CB_DOME_TESS_LV: number;
        }[];
        OES_element_index_uint: any;
        EXT_frag_depth: any;
        imposterSpheres: boolean;
        BGCOLOR: number[];
        backboneAtoms4Display: {
            N: number;
            C: number;
            O: number;
            H: number;
            OXT: number;
            OC1: number;
            OC2: number;
            H1: number;
            H2: number;
            H3: number;
            HA: number;
            HA2: number;
            HA3: number;
            CN: number;
            CM: number;
            CH3: number;
        };
        projectionMode: number;
        colorMode: number;
        smoothFactor: number;
        glsl_shaders: string[][];
        glsl_fog: number;
        skipClearGeometryBuffer: boolean;
        stereoMode: number;
        stereoFocalFraction: number;
        stereoEyeSepFraction: number;
        camera_fovy: number;
        HQsurface_gridSpacing: number;
        webGL2: boolean;
        vdwSphereMultiplier: number;
        stickRadius: number;
        groupColorFirstH: number;
        groupColorLastH: number;
        cartoon_highlight_color: number[];
        connect_cutoff: number;
        orientMODELs: boolean;
        chainHideCutoff: number;
        video_framerate: number;
        video_path: string;
        skipChainSplitting: boolean;
    };
    const AATypes: {
        ALA: number;
        CYS: number;
        ASP: number;
        GLU: number;
        PHE: number;
        GLY: number;
        HIS: number;
        ILE: number;
        LYS: number;
        LEU: number;
        MET: number;
        ASN: number;
        PRO: number;
        GLN: number;
        ARG: number;
        SER: number;
        THR: number;
        VAL: number;
        TRP: number;
        TYR: number;
        ACE: number;
        NME: number;
        NH2: number;
        HIP: number;
        HIE: number;
        HID: number;
        CYX: number;
        PTR: number;
        A: number;
        T: number;
        G: number;
        C: number;
        DA: number;
        DT: number;
        DG: number;
        DC: number;
        U: number;
        DU: number;
        U5: number;
        U3: number;
        A5: number;
        MSE: number;
        SEQ: number;
        CSW: number;
        ALY: number;
        CYM: number;
    };
    const AATypesBase: {
        ALA: number;
        CYS: number;
        ASP: number;
        GLU: number;
        PHE: number;
        GLY: number;
        HIS: number;
        ILE: number;
        LYS: number;
        LEU: number;
        MET: number;
        ASN: number;
        PRO: number;
        GLN: number;
        ARG: number;
        SER: number;
        THR: number;
        VAL: number;
        TRP: number;
        TYR: number;
        ACE: number;
        NME: number;
        NH2: number;
        HIP: number;
        HIE: number;
        HID: number;
        CYM: number;
    };
    var displayMode_None: number;
    var displayMode_Visible: number;
    var displayMode_Default: number;
    var displayMode_Spacefill: number;
    var displayMode_Spacefill_SC: number;
    var displayMode_BallStick: number;
    var displayMode_BallStick_SC: number;
    var displayMode_Stick: number;
    var displayMode_Stick_SC: number;
    var displayMode_Wireframe: number;
    var displayMode_Wireframe_SC: number;
    var displayMode_CaTrace: number;
    var displayMode_Tube: number;
    var displayMode_Cartoon: number;
    var displayMode_CartoonRocket: number;
    var displayMode_ChainSurfaceCG: number;
    var displayMode_ChainSurfaceSimple: number;
    var displayMode_XNA: number;
    var colorEntry_Default: number;
    var colorEntry_Structure: number;
    var colorEntry_CPK: number;
    var colorEntry_Group: number;
    var colorEntry_Chain: number;
    var colorEntry_Custom: number;
    var colorEntry_ChainAlt: number;
    var colorEntry_ABEGO: number;
    var colorEntry_Entity: number;
}
declare namespace molmil {
    function invertColor(hexTripletColor: any): string;
    function componentToHex(c: any): any;
    function rgb2hex(r: any, g: any, b: any): string;
    function hex2rgb(hex: any): number[];
}
declare namespace molmil {
    function initSettings(): void;
    function updateBGcolor(): void;
    function fetchCanvas(): any;
    function getSelectedAtom(n: any, soup: any): any;
    function savePDB(soup: any, atomSelection: any, modelId: any, file: any): any;
    function saveJSO(soup: any, atomSelection: any, modelId: any, file: any): any;
    function saveBU(assembly_id: any, options: any, struct: any, soup: any): any;
    function priestle_smoothing(points: any, from: any, to: any, skip: any, steps: any): void;
    function polynomialFit(x: any, y: any, order: any): any;
    function polynomialCalc(x: any, polynomial: any): any;
    function prepare2DRepr(chain: any, mdl: any): number;
    function handle_molmilViewer_mouseDown(event: any): void;
    function disableContextMenu(e: any): boolean;
    function getOffset(evt: any): {
        x: number;
        y: number;
    };
    function handle_molmilViewer_mouseUp(event: any): boolean;
    function handle_molmilViewer_mouseMove(event: any): void;
    function infoPopUp(text: any): void;
    function handle_molmilViewer_mouseScroll(event: any): false | void;
    function onDocumentMouseMove(event: any): void;
    function handle_molmilViewer_touchStart(event: any): void;
    function handle_molmilViewer_touchHold(): void;
    function handle_molmilViewer_touchMove(event: any): void;
    function handle_molmilViewer_touchEnd(): void;
    function handle_contextMenu_touchStart(event: any): void;
    function handle_contextMenu_touchHold(): void;
    function handle_contextMenu_touchEnd(event: any): void;
    function toggleEntry(obj: any, dm: any, rebuildGeometry: any, soup: any): void;
    function displayEntry(obj: any, dm: any, rebuildGeometry: any, soup: any, settings: any): void;
    function quickModelColor(type: any, options: any, soup: any): void;
    function colorEntry(obj: any, cm: any, setting: any, rebuildGeometry: any, soup: any): void;
    function getAtomFromMolecule(molecule: any, atomName: any): any;
    function resetColors(struc: any, soup: any): void;
    function fetchFrom(obj: any, what: any): any;
    function getProteinChains(obj: any): any[];
    function getResiduesForChain(chain: any, first: any, last: any): any[];
    function autoGetAtoms(array: any): any[];
    function setCanvas(soupObject: any, canvas: any): void;
    function initTexture(src: any, gl: any): any;
    function handleLoadedTexture(texture: any, gl: any): void;
    function resetAttributes(gl: any): void;
    function bindAttribute(gl: any, index: any, size: any, type: any, normalized: any, stride: any, offset: any): void;
    function clearAttributes(gl: any): void;
    function safeStartViewer(canvas: any): NodeJS.Timer;
    function animate_molmilViewers(): void;
    function unproject(dx: any, dy: any, cz: any, mat: any): number[];
    function hermiteInterpolate(a1: any, a2: any, T1: any, T2: any, nop: any, line: any, tangents: any, post2: any): void;
    function octaSphereBuilder(recursionLevel: any): {
        vertices: any[];
        faces: any[];
    };
    function buildOctaDome(t: any, side: any): {
        vertices: any[];
        faces: any[];
    };
    function buildBondsList4Molecule(bonds: any, molecule: any, xyzRef: any): void;
    function isBlackListed(): boolean;
    function addEnableMolmilButton(canvas: any): any;
    function createViewer(target: any, width: any, height: any, soupObject: any): any;
    function selectQLV(renderer: any, QLV: any, rebuildGeometry: any): void;
    function interpolateHsl(length: any, startH: any, endH: any): any[];
    function interpolateBR(length: any): any[];
    function resetCOG(canvas: any, recalc: any): void;
    function loadFile(loc: any, format: any, cb: any, async: any, soup: any): void;
    function loadPDBlite(pdbid: any, cb: any, async: any, soup: any): void;
    function loadPDB(pdbid: any, cb: any, async: any, soup: any): void;
    function loadCC(comp_id: any, cb: any, async: any, soup: any): void;
    function loadPDBchain(pdbid: any, cb: any, async: any, soup: any): void;
    function clear(canvas: any): void;
    function tubeSurface(chains: any, settings: any, soup: any): any;
    function coarseSurface(chain: any, res: any, probeR: any, settings: any): any;
    function lighterRGB(rgbIn: any, factor: any, nR: any): any[];
    function redistributeRGB(rgb: any): any;
    function toggleBU(assembly_id: any, displayMode: any, colorMode: any, struct: any, soup: any): any;
    function duplicateBU(assembly_id: any, options: any, struct: any, soup: any): any;
    function selectBU(assembly_id: any, displayMode: any, colorMode: any, options: any, struct: any, soup: any): any;
    function findContacts(atoms1: any, atoms2: any, r: any, soup: any): any;
    function calcHbonds(group1: any, group2: any, soup: any): any;
    function attachResidue(parentResidue: any, newResType: any): any;
    function renderHbonds(pairs: any, soup: any, settings: any): any;
    function renderPIinteractions(pairs: any, soup: any): any;
    function renderSaltBridges(pairs: any, soup: any): any[];
    function hslToRgb123(h: any, s: any, l: any): any[];
    function setSlab(near: any, far: any, soup: any): void;
    function selectAtoms(atoms: any, append: any, soup: any): void;
    function resetFocus(soup: any, t: any): void;
    function zoomTo(newCOR: any, oldCOR: any, newXYZ: any, soup: any, t: any): void;
    function selectionFocus(soup: any, t: any): void;
    function searchAtom(struc: any, chainID: any, resID: any, atomID: any): any;
    function findResidue(resInfo: any, soup: any): any[];
    function selectSequence(seq: any, soup: any): any[];
    function calcCenter(input: any): (number | any[])[];
    function addLabel(text: any, settings: any, soup: any): any;
    function mergeStructuresToModels(entries: any): void;
    function splitModelsToStructures(entry: any): void;
    function showNearbyResidues(obj: any, r: any, soup: any): any[];
    function fetchNearbyAtoms(obj: any, r: any, atomList: any, soup: any): any;
    function atoms2objects(atomList: any, exclude: any): any[];
    function atoms2chains(atomList: any, exclude: any): any[];
    function atoms2residues(atomList: any, exclude: any): any[];
    function conditionalPluginLoad(URL: any, callBack: any, self: any, argList: any, async: any): boolean;
    function loadPlugin(URL: any, callBack: any, self: any, argList: any, async?: any): any;
    function colorBfactor(selection: any, soup: any, colorFunc: any): void;
    function pointerLoc_setup(canvas: any): void;
    function pointerLock_update(e: any): void;
    function autoSetup(options: any, canvas: any): any;
    function processExternalCommand(cmd: any, commandBuffer: any): void;
    function bindCanvasInputs(canvas: any): any;
    function promode_elastic(id: any, mode: any, type: any, soup: any): void;
    function transformObject(obj: any, matrix: any): void;
    function cloneObject(obj: any, settings: any): any;
    function orient(atoms: any, soup: any, xyzs: any): void;
    function superpose(A: any, B: any, C: any, modelId: any, iterate: any): any;
    function record(canvas: any, video_path: any, video_framerate: any): void;
    function end_record(canvas: any): void;
    function getState(): void;
}
declare namespace molmil {
    function localStorageGET(field: any, except: any): any;
}
declare namespace molmil {
    const SNFG: any;
    const shapes3d: any;
}
declare namespace molmil {
    function exportPLY(soup: any, file: any): any;
    function exportMPBF(soup: any): any;
    function exportSTL(soup: any): any;
    function buCheck(assembly_id: any, displayMode: any, colorMode: any, struct: any, soup: any): any;
    function findResidueRings(molObj: any): any;
    /**
     * geometry object, used to generate protein geometry; atoms, bonds, loops, helices, sheets
    */
    class _geometry {
        templates: {
            sphere: {
                base: {};
            };
            cylinder: any[];
            dome: {};
        };
        detail_lvs: number;
        dome: number[];
        radius: number;
        sheetHeight: number;
        skipClearBuffer: boolean;
        onGenerate: any;
        modelId: any;
        buffer1: any;
        buffer2: any;
        buffer3: any;
        buffer4: any;
        buffer5: any;
        atoms2draw: any[];
        xna2draw: any[];
        wfatoms2draw: any[];
        trace: any[];
        bonds2draw: any[];
        lines2draw: any[];
        bondRef: {};
        generator(objects: any, soup: any, name: any, programOptions: any): any;
        getSphere(r: any, detail_lv: any): any;
        getCylinder(detail_lv: any): any;
        generateCylinder(detail_lv?: any): any[];
        generateSphere(r: any, detail_lv: any): any;
        generate(structures: any, render: any, detail_or: any): void;
        build_simple_render_program(vertices_: any, indices_: any, renderer: any, settings: any): any;
        registerPrograms(renderer: any, initOnly?: any): void;
        reset(): void;
        updateNormals(obj: any): void;
        initChains(chains: any, render: any, detail_or: any): void;
        initCartoon(chains: any): void;
        generateAtomsImposters(): void;
        generateAtoms(): void;
        generateBonds(): void;
        generateWireframe(): void;
        generateSurfaces(chains: any, soup: any): void;
        generateCartoon(): void;
        buildLoopNcap(t: any, P: any, T: any, N: any, B: any, rgba: any, aid: any, coreRadius: any): void;
        buildLoopCcap(t: any, P: any, T: any, N: any, B: any, rgba: any, aid: any, coreRadius: any): void;
        buildLoop(t: any, t_next: any, P: any, T: any, N: any, B: any, rgba: any, aid: any, coreRadius: any): any;
        buildRocket(t: any, t_next: any, P: any, T: any, N: any, B: any, rgba: any, aid: any, isLast: any, coreRadius: any): any;
        buildHelix(t: any, t_next: any, P: any, T: any, N: any, B: any, rgba: any, aid: any, currentBlock: any, coreRadius: any): any;
        buildSheet(t: any, t_next: any, P: any, T: any, N: any, B: any, rgba: any, aid: any, isFirst: any, isLast: any, coreRadius: any): any;
        generateSNFG(): void;
    }
    const geometry: _geometry;
}
declare namespace molmil {
    class FBO {
        constructor(gl: any, width: any, height: any);
        addTexture(textureID: any, internalFormat: any, format: any): void;
        setup(): void;
        post(): void;
        rebindTextures(unbind: any): void;
        bindTextureToUniform(textureID: any, uniformLocation: any, bindLocation: any): void;
        resize(width: any, height: any): void;
        bind(): void;
        unbind(): void;
    }
}
declare namespace molmil {
    class glCamera {
        constructor();
        reset(): void;
        generateMatrix(): any;
        positionCamera(): void;
    }
}
declare namespace molmil {
    function generateSphereImposterTexture(res: any, gl: any): any;
    class render {
        constructor(soup: any);
        clear(): void;
        addProgram(program: any): void;
        removeProgram(program: any): void;
        removeProgramByName(idname: any): void;
        reloadSettings(): void;
        selectDefaultContext(): void;
        selectDataContext(): void;
        initGL(canvas: any, width: any, height: any): boolean;
        reinitRenderer(): void;
        initRenderer(): void;
        initShaders(programs: any): void;
        updateSelection(): void;
        selectFrame(i: any, detail_or: any): void;
        initBuffers(): void;
        renderPicking(): void;
        renderPrograms(COR: any): void;
        render(): void;
        initFBOs(): void;
        resizeViewPort(): void;
        renderAtomSelection(modelViewMatrix: any, COR: any): void;
    }
}
declare namespace molmil {
    class _shaderEngine {
        code: {};
        recompile(renderer: any): void;
    }
    const shaderEngine: _shaderEngine;
    function setupShader(gl: any, name: any, program: any, src: any, type: any): any;
}
declare namespace molmil {
    class viewer {
        canvas: any;
        renderer: any;
        defaultCanvas: any;
        onAtomPick: any;
        downloadInProgress: any;
        constructor(canvas: any);
        toString(): string;
        reloadSettings(): void;
        clear(): void;
        gotoMol(mol: any): void;
        waterToggle(show: any): void;
        hydrogenToggle(show: any): void;
        restoreDefaultCanvas(canvas: any): void;
        selectObject(x: any, y: any, event: any): void;
        loadStructure(loc: any, format: any, ondone: any, settings: any): any;
        loadGromacsXTC(buffer: any, settings: any): any;
        loadGromacsTRR(buffer: any): any;
        loadMyPrestoMnt(buffer: any, fxcell: any): any;
        loadMyPrestoTrj(buffer: any, fxcell: any): any;
        loadStructureData(data: any, format: any, filename: any, ondone: any, settings: any): any;
        buildAminoChain(chain: any): void;
        buildSNFG(chain: any): void;
        buildMolBondList(chain: any, rebuild: any): void;
        buildBondList(chain: any, rebuild: any): void;
        getChain(struc: any, cid: any): any[];
        getChainAuth(struc: any, cid: any): any[];
        getMolObject4Chain(chain: any, id: any): any;
        getMolObject4ChainAlt(chain: any, RSID: any): any;
        load_obj(data: any, filename: any, settings: any): any;
        load_ccp4(buffer: any, filename: any, settings: any): any;
        load_stl(buffer: any, filename: any, settings: any): any;
        load_MPBF(buffer: any, filename: any, settings: any): any;
        load_xyz(data: any, filename: any, settings: any): any;
        load_mol2(data: any, filename: any): any;
        load_mdl(data: any, filename: any): any;
        load_GRO(data: any, filename: any): any;
        load_PDB(data: any, filename: any): any;
        load_MMTF(data: any, filename: any): any;
        calcZ(geomRanges: any): number;
        load_polygonJSON(jso: any, filename: any, settings: any): any;
        load_polygonXML(xml: any, filename: any, settings: any): any;
        load_mmCIF(data: any, settings: any): any[];
        load_PDBML(xml: any, settings: any): any[];
        load_PDBx(mmjso: any, settings: any): any[];
        calculateCOG(atomList: any): void;
        ssAssign(chainObj: any): any;
        setCOR(selection: any): void;
        resetCOR(): void;
        hideCell(): void;
        showCell(): any;
    }
}
declare namespace molmil {
    const formatList: {};
    function guess_format(name: any): any;
    function loadFilePointer(fileObj: any, func: any, canvas: any): boolean;
}
declare namespace molmil {
    function normalFromMat3(a: any, out: any): any;
    function getAtomXYZ(atom: any, soup: any): any[];
    function calcMMDistance(a1: any, a2: any, soup: any): any;
    function calcAngle(a1: any, a2: any, a3: any): number;
    function calcTorsion(a1: any, a2: any, a3: any, a4: any): number;
    function calcMMAngle(a1: any, a2: any, a3: any, soup: any): number;
    function calcMMTorsion(a1: any, a2: any, a3: any, a4: any, soup: any): number;
    function calculateBBTorsions(chain: any, soup: any): void;
    function linkCanvases(canvases: any): void;
    function __webglNotSupported__(canvas: any): any;
    function calcRMSD(atoms1: any, atoms2: any, transform: any): any;
    function arrayMin(arr: any): any;
    function arrayMax(arr: any): any;
}
declare namespace molmil {
    function startWebVR(that: any): void;
    function initVR(soup: any, callback: any): void;
}
declare namespace molmil {
    class atomObject {
        xyz: any;
        element: any;
        atomName: any;
        displayMode: any;
        display: any;
        rgba: any;
        molecule: any;
        chain: any;
        radius: any;
        AID: any;
        Bfactor: any;
        constructor(Xpos: any, AN: any, element: any, molObj: any, chainObj: any);
        toString(): string;
    }
}
declare namespace molmil {
    class chainObject {
        constructor(name: any, entry: any);
        toString(): string;
    }
}
declare namespace molmil {
    function simpleEntry(): void;
    class entryObject {
        constructor(meta: any);
        toString(): string;
    }
}
declare namespace molmil {
    const defaultSettings_label: {
        dx: number;
        dy: number;
        dz: number;
        color: number[];
        fontSize: number;
    };
    class labelObject {
        constructor(soup: any);
    }
}
declare namespace molmil {
    class molObject {
        constructor(name: any, id: any, chain: any);
        toString(): string;
    }
}
declare namespace molmil {
    class polygonObject {
        constructor(meta: any);
    }
}
