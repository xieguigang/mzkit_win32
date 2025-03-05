namespace molmil {

    // ** settings objects **

    // modify the render engine so that each input entry is loaded into a different render object
    // then, set it up so that each object can be clipped differently
    // molmil.entryObject.programs = [];
    // dynamically update the programs (e.g. if a wireframe one isn't necessary, don't use it)

    export var canvasList = [];
    export var mouseDown = false;
    export var mouseDownS = {};
    export var mouseMoved = false;
    export var Xcoord = 0;
    export var Ycoord = 0;
    export var Zcoord = 0;
    export var activeCanvas = null;
    export var touchList = null;
    export var touchMode = false;
    export var preRenderFuncs = [];
    export var longTouchTID = null;
    export var previousTouchEvent = null;
    export var ignoreBlackList = false;
    export var vrDisplay = null;
    export var vrPose = [0, 0, 0];
    export var vrOrient = [0, 0, 0, 0];
    export var pdbj_data = "https://data.pdbjlc1.pdbj.org/";

    // switch PDBj URLs to newweb file service
    export const settings_default = {
        src: document.currentScript ? document.currentScript.src.split("/").slice(0, -1).join("/") + "/" : "https://pdbj.org/molmil2/",
        pdb_url: molmil.pdbj_data + "pdbjplus/data/pdb/mmjson/__ID__.json",
        pdb_chain_url: molmil.pdbj_data + "pdbjplus/data/pdb/mmjson-chain/__ID__-chain.json",
        comp_url: molmil.pdbj_data + "pdbjplus/data/cc/mmjson/__ID__.json",
        data_url: molmil.pdbj_data,
        newweb_rest: molmil.pdbj_data = window.location.host.endsWith(".pdbj.org") ? "https://" + window.location.host + (window.location.pathname.startsWith("/molmil_dev/") ? "/dev" : "") + "/rest/newweb/" : "https://pdbj.org" + (window.location.pathname.startsWith("/molmil_dev/") ? "/dev" : "") + "/rest/newweb/",
        /* change the implementation to force usage of molmil-app */
        molmil_video_url: undefined,
        dependencies: ["lib/gl-matrix-min.js"],
    };

    export var cli_canvas = null;
    export var cli_soup = null;

    export var settings = (<any>window).molmil_settings || settings_default;

    for (var e in molmil.settings_default)
        if (!molmil.settings.hasOwnProperty(e))
            molmil.settings[e] = molmil.settings_default[e];

    export const configBox = {
        version: 2,
        buildDate: null,
        initFinished: false,
        liteMode: false,
        cullFace: true,
        loadModelsSeparately: false,
        wheelZoomRequiresCtrl: false,
        recordingMode: false,
        save_pdb_chain_only_change: false,
        xna_force_C1p: undefined,

        // Co, Mo, D, Ru, W, Q, YB, gd, ir, os, Y, sm, pr, tb, re, eu, ta, rh, lu, ho

        vdwR: {
            DUMMY: 1.7,
            H: 1.2,
            He: 1.4,
            Li: 1.82,
            Be: 1.53,
            B: 1.92,
            C: 1.7,
            N: 1.55,
            O: 1.52,
            F: 1.47,
            Ne: 1.54,
            Na: 2.27,
            Mg: 1.73,
            Al: 1.84,
            Si: 2.1,
            P: 1.8,
            S: 1.8,
            Cl: 1.75,
            Ar: 1.88,
            K: 2.75,
            Ca: 2.31,
            Sc: 2.11,
            Ti: 1.47,
            V: 1.34,
            Cr: 1.27,
            Mn: 1.26,
            Fe: 1.26,
            Ni: 1.63,
            Cu: 1.4,
            Zn: 1.39,
            Ga: 1.87,
            Ge: 2.11,
            As: 1.85,
            Se: 1.9,
            Br: 1.85,
            Kr: 2.02,
            Rb: 3.03,
            Sr: 2.49,
            Pd: 1.63,
            Ag: 1.72,
            Cd: 1.58,
            In: 1.93,
            Sn: 2.17,
            Sb: 2.06,
            Te: 2.06,
            I: 1.98,
            Xe: 2.16,
            Cs: 3.43,
            Ba: 2.68,
            Pt: 1.75,
            Au: 1.66,
            Hg: 1.55,
            Tl: 1.96,
            Pb: 2.02,
            Bi: 2.07,
            Po: 1.97,
            At: 2.02,
            Rn: 2.2,
            Fr: 3.48,
            Ra: 2.83,
            U: 1.86
        },

        MW: {
            H: 1.0079,
            He: 4.0026,
            Li: 6.941,
            Be: 9.0122,
            B: 10.811,
            C: 12.0107,
            N: 14.0067,
            O: 15.9994,
            F: 18.9984,
            Ne: 20.1797,
            Na: 22.9897,
            Mg: 24.305,
            Al: 26.9815,
            Si: 28.0855,
            P: 30.9738,
            S: 32.065,
            Cl: 35.453,
            K: 39.0983,
            Ar: 39.948,
            Ca: 40.078,
            Sc: 44.9559,
            Ti: 47.867,
            V: 50.9415,
            Cr: 51.9961,
            Mn: 54.938,
            Fe: 55.845,
            Ni: 58.6934,
            Co: 58.9332,
            Cu: 63.546,
            Zn: 65.39,
            Ga: 69.723,
            Ge: 72.64,
            As: 74.9216,
            Se: 78.96,
            Br: 79.904,
            Kr: 83.8,
            Rb: 85.4678,
            Sr: 87.62,
            Y: 88.9059,
            Zr: 91.224,
            Nb: 92.9064,
            Mo: 95.94,
            Tc: 98,
            Ru: 101.07,
            Rh: 102.9055,
            Pd: 106.42,
            Ag: 107.8682,
            Cd: 112.411,
            In: 114.818,
            Sn: 118.71,
            Sb: 121.76,
            I: 126.9045,
            Te: 127.6,
            Xe: 131.293,
            Cs: 132.9055,
            Ba: 137.327,
            La: 138.9055,
            Ce: 140.116,
            Pr: 140.9077,
            Nd: 144.24,
            Pm: 145,
            Sm: 150.36,
            Eu: 151.964,
            Gd: 157.25,
            Tb: 158.9253,
            Dy: 162.5,
            Ho: 164.9303,
            Er: 167.259,
            Tm: 168.9342,
            Yb: 173.04,
            Lu: 174.967,
            Hf: 178.49,
            Ta: 180.9479,
            W: 183.84,
            Re: 186.207,
            Os: 190.23,
            Ir: 192.217,
            Pt: 195.078,
            Au: 196.9665,
            Hg: 200.59,
            Tl: 204.3833,
            Pb: 207.2,
            Bi: 208.9804,
            Po: 209,
            At: 210,
            Rn: 222,
            Fr: 223,
            Ra: 226,
            Ac: 227,
            Pa: 231.0359,
            Th: 232.0381,
            Np: 237,
            U: 238.0289,
            Am: 243,
            Pu: 244,
            Cm: 247,
            Bk: 247,
            Cf: 251,
            Es: 252,
            Fm: 257,
            Md: 258,
            No: 259,
            Rf: 261,
            Lr: 262,
            Db: 262,
            Bh: 264,
            Sg: 266,
            Mt: 268,
            Rg: 272,
            Hs: 277
        },

        sndStrucInfo: { 1: [255, 255, 255], 2: [255, 255, 0], 3: [255, 0, 255], 4: [0, 0, 255] },

        zNear: 10.0,
        zFar: 20000.0,

        QLV_SETTINGS: [
            { SPHERE_TESS_LV: 0, CB_NOI: 2, CB_NOVPR: 6, CB_DOME_TESS_LV: 0 },
            { SPHERE_TESS_LV: 1, CB_NOI: 3, CB_NOVPR: 10, CB_DOME_TESS_LV: 1 },
            { SPHERE_TESS_LV: 2, CB_NOI: 8, CB_NOVPR: 10, CB_DOME_TESS_LV: 2 },
            { SPHERE_TESS_LV: 3, CB_NOI: 12, CB_NOVPR: 15, CB_DOME_TESS_LV: 3 },
            { SPHERE_TESS_LV: 4, CB_NOI: 16, CB_NOVPR: 31, CB_DOME_TESS_LV: 4 }],


        OES_element_index_uint: null,
        EXT_frag_depth: null,
        imposterSpheres: false,
        BGCOLOR: [1.0, 1.0, 1.0, 1.0],

        backboneAtoms4Display: { "N": 1, "C": 1, "O": 1, "H": 1, "OXT": 1, "OC1": 1, "OC2": 1, "H1": 1, "H2": 1, "H3": 1, "HA": 1, "HA2": 1, "HA3": 1, "CN": 1, "CM": 1, "CH3": 1 },
        projectionMode: 1, // 1: perspective, 2: orthographic
        colorMode: 1, // 1: rasmol, 2: jmol

        smoothFactor: 2,

        glsl_shaders: [
            ["shaders/standard.glsl"],
            ["shaders/lines.glsl"],
            ["shaders/picking.glsl"],
            ["shaders/linesPicking.glsl"],
            ["shaders/atomSelection.glsl"],
            //    ["shaders/imposterPoints.glsl", "points", "", ["EXT_frag_depth"]], // upgrade this to webgl2...
            ["shaders/lines.glsl", "lines_uniform_color", "#define UNIFORM_COLOR 1\n"],
            ["shaders/alpha_dummy.glsl", "alpha_dummy", ""],
            ["shaders/standard.glsl", "standard_alpha", "#define ALPHA_MODE 1\n"],
            ["shaders/standard.glsl", "standard_alphaSet", "#define ALPHA_SET 1\n"],
            ["shaders/standard.glsl", "standard_uniform_color", "#define UNIFORM_COLOR 1\n"],
            ["shaders/standard.glsl", "standard_shader_opaque", "#define ALPHA_MODE 1\n#define OPAQUE_ONLY 1"],
            ["shaders/standard.glsl", "standard_shader_transparent", "#define ALPHA_MODE 1\n#define TRANSPARENT_ONLY 1"],
            ["shaders/standard.glsl", "standard_alpha_uniform_color", "#define ALPHA_MODE 1\n#define UNIFORM_COLOR 1\n"],
            ["shaders/standard.glsl", "standard_alphaSet_uniform_color", "#define ALPHA_SET 1\n#define UNIFORM_COLOR 1\n"],
            ["shaders/standard.glsl", "standard_slab", "#define ENABLE_SLAB 1\n"], // standard_slab
            ["shaders/standard.glsl", "standard_slabColor", "#define ENABLE_SLAB 1\n#define ENABLE_SLABCOLOR 1\n"], // standard_slabColor
            //["shaders/imposterPoints.glsl", "points_uniform_color", "#define UNIFORM_COLOR 1\n", ["EXT_frag_depth"]],  // upgrade this to webgl2...
            ["shaders/anaglyph.glsl"],
            ["shaders/billboardShader.glsl"]
        ],
        glsl_fog: 0, // 0: off, 1: on
        skipClearGeometryBuffer: true,
        stereoMode: 0,
        stereoFocalFraction: 1.5,
        stereoEyeSepFraction: 30,
        camera_fovy: 22.5,
        HQsurface_gridSpacing: 1.0,
        webGL2: false,
        vdwSphereMultiplier: 1.0,
        stickRadius: 0.15,
        groupColorFirstH: 240,
        groupColorLastH: 0,
        cartoon_highlight_color: [255, 255, 255, 255],
        connect_cutoff: 0.35,
        orientMODELs: false,
        chainHideCutoff: 300,
        video_framerate: 15,
        video_path: "video.mp4",
        skipChainSplitting: false
    };

    export const AATypes = {
        "ALA": 1, "CYS": 1, "ASP": 1, "GLU": 1, "PHE": 1, "GLY": 1, "HIS": 1, "ILE": 1, "LYS": 1, "LEU": 1, "MET": 1, "ASN": 1, "PRO": 1, "GLN": 1, "ARG": 1,
        "SER": 1, "THR": 1, "VAL": 1, "TRP": 1, "TYR": 1, "ACE": 1, "NME": 1, "NH2": 1, "HIP": 1, "HIE": 1, "HID": 1, "CYX": 1, "PTR": 1,
        "A": 1, "T": 1, "G": 1, "C": 1, "DA": 1, "DT": 1, "DG": 1, "DC": 1, "U": 1, "DU": 1, "U5": 1, "U3": 1, "A5": 1, "MSE": 1, "SEQ": 1, "CSW": 1, "ALY": 1, "CYM": 1
    };

    export const AATypesBase = {
        "ALA": 1, "CYS": 1,
        "ASP": 1, "GLU": 1, "PHE": 1, "GLY": 1, "HIS": 1, "ILE": 1, "LYS": 1, "LEU": 1, "MET": 1, "ASN": 1, "PRO": 1, "GLN": 1, "ARG": 1, "SER": 1, "THR": 1, "VAL": 1, "TRP": 1, "TYR": 1, "ACE": 1, "NME": 1, "NH2": 1, "HIP": 1, "HIE": 1, "HID": 1, "CYM": 1
    };

    // display modes
    export var displayMode_None = 0;
    export var displayMode_Visible = 0.5;
    export var displayMode_Default = 1;
    export var displayMode_Spacefill = 2;
    export var displayMode_Spacefill_SC = 2.5;
    export var displayMode_BallStick = 3;
    export var displayMode_BallStick_SC = 3.5;
    export var displayMode_Stick = 4;
    export var displayMode_Stick_SC = 4.5;
    export var displayMode_Wireframe = 5;
    export var displayMode_Wireframe_SC = 5.5;
    export var displayMode_CaTrace = 6;
    export var displayMode_Tube = 7;
    export var displayMode_Cartoon = 8;
    export var displayMode_CartoonRocket = 8.5;
    export var displayMode_ChainSurfaceCG = 10;
    export var displayMode_ChainSurfaceSimple = 11;

    export var displayMode_XNA = 400;

    // color modes
    export var colorEntry_Default = 1;
    export var colorEntry_Structure = 2;
    export var colorEntry_CPK = 3;
    export var colorEntry_Group = 4;
    export var colorEntry_Chain = 5;
    export var colorEntry_Custom = 6;
    export var colorEntry_ChainAlt = 7;
    export var colorEntry_ABEGO = 8;
    export var colorEntry_Entity = 9;

}