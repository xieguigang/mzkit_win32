/// <reference path="../../plugins/UI.d.ts"/>

namespace molmil {

    export interface options {
        enable: string[];
        environment;
        callback;
    }

    export function autoSetup(options: options = <any>{}, canvas = null) {

        if (!options.hasOwnProperty("enable")) options.enable = ["ui", "cli", "cli-hash", "drag-n-drop"];

        if (!canvas) {
            for (var i = 0; i < molmil.canvasList.length; i++) molmil.autoSetup(options, molmil.canvasList[i]);
            var viewers: HTMLElement[] = <any>document.getElementsByClassName("molmilViewer");
            if (viewers.length == 0) viewers = [document.getElementById("molmilViewer")];
            for (var i = 0; i < viewers.length; i++) if (viewers[i] && !viewers[i].molmilViewer) canvas = molmil.autoSetup(options, viewers[i]);
            return canvas;
        }

        if (!canvas.molmilViewer) molmil.createViewer(canvas);
        if (canvas.setupDone) return;

        if (options.enable.includes("cli") && !canvas.commandLine) {
            var cli = new molmil.commandLine(canvas);
            if (options.environment) {
                for (var e in options.environment) cli.environment[e] = options.environment[e];
            }

            if (options.enable.includes("cli-hash")) {
                var hash = window.location.hash ? window.location.hash.substr(1) : "";
                if (hash) cli.environment.console.runCommand(decodeURIComponent(hash));

                window.onhashchange = function () {
                    var hash = window.location.hash ? window.location.hash.substr(1) : "";
                    if (hash) { molmil.clear(canvas); cli.environment.console.runCommand(decodeURIComponent(hash)); }
                }
            }

            if (window.onkeyup == null) {
                var lastPress = 0;
                window.onkeyup = function (ev) {
                    if (ev.keyCode == 27) {
                        var now = (new Date()).getTime();
                        if (now - lastPress < 250) cli.icon.onclick();
                        lastPress = now;
                    }
                }
            }
        }
        else if (options.enable.includes("cli-hash") && !canvas.commandLine) {
            var hash = window.location.hash ? window.location.hash.substr(1) : "";
            if (hash) {
                var cli = new molmil.commandLine(canvas);
                if (options.environment) { for (var e in options.environment) cli.environment[e] = options.environment[e]; }
                cli.environment.console.runCommand(decodeURIComponent(hash));
                cli.consoleBox.style.display = "none";
            }
        }

        var wait = false;
        if (options.enable.includes("ui") && !molmil.UI) {
            wait = true;
            molmil.loadPlugin(molmil.settings.src + "plugins/UI.js", null, null, null, true);
        }
        if (wait) return molmil_dep.asyncStart(molmil.autoSetup, [options, canvas], this, 10);

        if (options.enable.includes("ui")) {
            canvas.molmilViewer.UI = new molmil.UI(canvas.molmilViewer);
            canvas.molmilViewer.UI.init();
            canvas.molmilViewer.animation = new molmil.animationObj(canvas.molmilViewer);
        }

        if (options.enable.includes("drag-n-drop")) molmil.bindCanvasInputs(canvas);

        var commandBuffer = [];

        try {
            commandBuffer = window.sessionStorage.commandBuffer ? JSON.parse(window.sessionStorage.commandBuffer) : [];
        }
        catch (e) { }
        for (var i = 0; i < commandBuffer.length; i++)
            processExternalCommand(commandBuffer[i]);

        canvas.setupDone = true;
        if (options.callback) options.callback();
    };

}