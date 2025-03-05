/// <reference path="./cli.ts" />
/// <reference path="./settings.ts" />
/// <reference path="./color.ts" />

/// <reference path="../../molmil_dep.d.ts" /> 
/// <reference path="./molmil.ts" />

/// <reference path="./extends.ts" />
/// <reference path="./canvas/SNFG.ts" />
/// <reference path="./geometry/geometry.ts" />

/// <reference path="./canvas/FBO.ts" />
/// <reference path="./canvas/glCamera.ts" />
/// <reference path="./canvas/render.ts" />
/// <reference path="./canvas/shaderEngine.ts" />
/// <reference path="./canvas/SNFG.ts" />
/// <reference path="./canvas/viewer.ts" />

/*!
 * molmil.js
 *
 * Molmil molecular web viewer: https://github.com/gjbekker/molmil
 * 
 * By Gert-Jan Bekker
 * License: LGPLv3
 *   See https://github.com/gjbekker/molmil/blob/master/LICENCE.md
 */

window.addEventListener("message", function (e) {
    var commandBuffer = [];

    try {
        commandBuffer = window.sessionStorage.commandBuffer ? JSON.parse(window.sessionStorage.commandBuffer) : [];
    }
    catch (e) {
        console.error(e);
    }
    e.data.event = e;
    molmil.processExternalCommand(e.data, commandBuffer);
    try {
        window.sessionStorage.commandBuffer = JSON.stringify(commandBuffer);
    }
    catch (e) {
        console.error(e);
    }
}, false);

if (typeof (requestAnimationFrame) != "undefined") molmil.animate_molmilViewers();

if (!window.molmil_dep) {
    var dep = document.createElement("script")
    dep.src = molmil.settings.src + "molmil_dep.js";
    var head = document.getElementsByTagName("head")[0];
    head.appendChild(dep);
}

molmil.initSettings();