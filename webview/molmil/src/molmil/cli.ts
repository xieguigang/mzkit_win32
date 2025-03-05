namespace molmil {

    // ** Molmil's command line interface **

    export function checkRebuild() {
        var soup = molmil.cli_soup || molmil.fetchCanvas().molmilViewer;
        if (soup.renderer.rebuildRequired) soup.renderer.initBuffers();
        soup.renderer.rebuildRequired = false;
        soup.renderer.canvas.update = true;
    }

    interface environment {
        fileObjects: {}
        setTimeout;
        clearTimeout
        navigator
        window;
        console;
        cli_canvas;
        cli_soup;
        global;
        colors;
    }

    export const commandLines = {};

    export class commandLine {

        environment: environment = <any>{
            fileObjects: {}
        };
        commandBuffer = [];
        icon;
        consoleBox;
        canvas;
        soup;
        run;
        logBox;
        initDone;
        altCommandIF;

        constructor(canvas) {
            for (var e in window)
                this.environment[e] = undefined;

            this.environment.setTimeout = function (cb, tm) { window.setTimeout(cb, tm); }
            this.environment.clearTimeout = function () { window.clearTimeout(); }
            this.environment.navigator = window.navigator;
            this.environment.window = this;

            this.environment.console = {};

            var exports = ["molmil", "molmil_dep", "glMatrix", "mat2", "mat2", "mat3", "mat4", "quat", "quat2", "vec2", "vec3", "vec4"];

            //this.environment.molmil = molmil;
            //this.environment.molmil_dep = molmil_dep;

            for (var i = 0; i < exports.length; i++) this.environment[exports[i]] = window[exports[i]];

            this.canvas = this.environment.cli_canvas = canvas; this.soup = this.environment.cli_soup = canvas.molmilViewer;
            canvas.commandLine = this;
            this.environment.global = this.environment;
            this.buildGUI();

            this.environment.console.debugMode = true;

            this.environment.console.log = function () {
                if (this.debugMode) console.log.apply(console, arguments);
                var __arguments = Array.prototype.slice.call(arguments, 0);
                var tmp = document.createElement("div"); tmp.textContent = __arguments.join(", ");
                this.custom(tmp);
            };
            this.environment.console.warning = function () {
                if (this.debugMode) console.warning.apply(console, arguments);
                var __arguments = Array.prototype.slice.call(arguments, 0);
                var tmp = document.createElement("div"); tmp.textContent = __arguments.join(", ");
                tmp.style.color = "yellow";
                this.custom(tmp);
            };
            this.environment.console.error = function () {
                if (this.debugMode) console.error.apply(console, arguments);
                var __arguments = Array.prototype.slice.call(arguments, 0);
                //console.error.apply(console, __arguments);
                var tmp = document.createElement("div"); tmp.textContent = __arguments.join(", ");
                tmp.style.color = "red";
                this.custom(tmp);
            };
            this.environment.console.logCommand = function () {
                if (this.cli.environment.scriptUrl) return;
                var __arguments = Array.prototype.slice.call(arguments, 0);
                var tmp = document.createElement("div"); tmp.textContent = __arguments.join("\n");
                tmp.style.color = "#00BFFF";
                this.custom(tmp, true);
            };
            this.environment.console.custom = function (obj, noPopup) {
                if (!obj.textContent) return;
                this.logBox.appendChild(obj);
                this.logBox.scrollTop = this.logBox.scrollHeight;
                if (noPopup != true) this.logBox.icon.onclick(true);
            };
            this.environment.console.runCommand = function (command, priority) {
                if (!molmil.isBalancedStatement(command)) return;
                if (this.cli.environment.cli_canvas.commandLine.initDone === undefined) return molmil_dep.asyncStart(this.runCommand, [command, priority], this, 10);

                var sub_commands = [], startIdx = 0, idx = 0, sc, tmpIdx;
                var n = 0;
                while (idx < command.length) {
                    idx = command.indexOf(";", startIdx);
                    if (idx == -1) {
                        sub_commands.push(command.substr(startIdx).trim());
                        break;
                    }

                    sc = command.substring(startIdx, idx).trim();

                    while (!molmil.isBalancedStatement(sc)) {
                        tmpIdx = idx + 1;
                        idx = command.indexOf(";", tmpIdx);
                        sc = command.substring(startIdx, idx).trim();
                    }
                    sub_commands.push(sc);

                    startIdx = idx + 1;
                }

                if (priority) { var buffer = this.cli.commandBuffer; this.cli.commandBuffer = []; }

                for (var i = 0; i < sub_commands.length; i++) {
                    sub_commands[i] = sub_commands[i].trim();
                    this.logCommand(sub_commands[i]);
                    this.cli.eval(sub_commands[i]);
                    this.backlog.unshift(sub_commands[i]);
                }
                this.cli.eval("molmil.checkRebuild();");
                if (priority) { for (var i = 0; i < buffer.length; i++) this.cli.eval(buffer[i]); }

                /*
                if (command.indexOf("{") == -1 && command.indexOf(";") != -1) {
                  var sub_commands = command.split(";");
                  for (var i=0; i<sub_commands.length; i++) {
                    sub_commands[i] = sub_commands[i].trim();
                    this.logCommand(sub_commands[i]);
                    this.cli.eval(sub_commands[i]);
                    this.backlog.unshift(sub_commands[i]);
                  }
                }
                else {
                  command = command.trim();
                  this.logCommand(command);
                  this.cli.eval(command);
                  this.backlog.unshift(command);
                }
                */
                this.buffer = ""; this.blSel = -1;
                if (this.cli.environment.cli_canvas.commandLine.initDone == false) this.cli.environment.cli_canvas.commandLine.initDone = true;
            };
            this.run = function (command) { this.environment.console.runCommand(command); }
            this.environment.console.backlog = [];
            this.environment.console.buffer = "";
            this.environment.console.blSel = -1;


            this.environment.console.logBox = this.logBox; this.environment.console.cli = this;

            this.environment.colors = { red: [255, 0, 0, 255], green: [0, 255, 0, 255], blue: [0, 0, 255, 255], grey: [100, 100, 100, 255], magenta: [255, 0, 255, 255], cyan: [0, 255, 255, 255], yellow: [255, 255, 0, 255], black: [0, 0, 0, 255], white: [255, 255, 255, 255], purple: [128, 0, 128, 255], orange: [255, 165, 0, 255] };

            //this.bindNullInterface();


            // this.bindPymolInterface();

            var init = function () {
                if (this.environment.console.backlog.length == 0) this.initDone = true;
                else this.initDone = false;
                this.bindPymolInterface();
                this.icon.show(true); this.icon.hide();
                if (molmil.onInterfacebind) molmil.onInterfacebind(this);
            };

            if (!molmil.commandLines.pyMol) {
                molmil.loadPlugin(molmil.settings.src + "plugins/pymol-script.js", init, this, [], true);
            }
        };

        buildGUI() {
            this.consoleBox = this.canvas.parentNode.pushNode("span");
            this.consoleBox.className = "molmil_UI_cl_box"; this.consoleBox.style.overflow = "initial";

            this.logBox = this.consoleBox.pushNode("span");
            this.logBox.icon = this.icon = this.consoleBox.pushNode("span");
            this.icon.innerHTML = "<"; this.icon.title = "Display command line";
            this.icon.className = "molmil_UI_cl_icon";

            this.icon.show = function (nofocus) {
                this.innerHTML = ">"; this.title = "Hide command line";

                this.cli.logBox.style.borderBottom = "";
                this.cli.logBox.style.borderRadius = ".33em";

                this.inp.style.display = this.cli.logBox.style.display = "initial";
                this.cli.consoleBox.style.height = this.cli.consoleBox.style.maxHeight = "calc(" + this.cli.canvas.clientHeight + "px - 6em)";
                this.cli.consoleBox.style.overflow = "";
                this.cli.logBox.style.overflow = "";
                this.cli.logBox.style.pointerEvents = "";
                if (!nofocus) this.inp.focus();
            }
            this.icon.hide = function () {
                this.innerHTML = "<"; this.title = "Display command line";
                this.inp.style.display = this.cli.logBox.style.display = "";
            }
            this.icon.onclick = function (mini) {
                if (mini == true) {
                    if (this.inp.style.display == "") {
                        this.cli.logBox.style.display = "initial";
                        this.cli.consoleBox.style.height = "8em";
                        this.cli.logBox.style.pointerEvents = "none";
                        this.cli.logBox.style.overflow = "hidden";
                        this.logBox.scrollTop = this.logBox.scrollHeight;
                    }
                    return;
                }
                if (this.inp.style.display == "initial") this.hide();
                else this.show();
            };
            this.inp = this.icon.inp = this.consoleBox.pushNode("span");
            this.inp.className = "molmil_UI_cl_input"; this.inp.style.display = "none";
            this.inp.contentEditable = true;
            this.inp.cli = this.icon.cli = this;
            this.icon.logBox = this.inp.logBox = this.logBox;

            this.logBox.className = "molmil_UI_cl_logbox"; this.logBox.style.display = "none";

            this.inp.console = this.environment.console;

            this.inp.onkeydown = function (e) {
                var command = this.textContent;
                if (e.keyCode == 13 && !e.shiftKey && molmil.isBalancedStatement(command)) {
                    this.console.runCommand(command);
                    this.textContent = "";
                    return false;
                }
                if (e.keyCode == 38 && e.ctrlKey) {
                    this.console.blSel++;
                    if (this.console.blSel >= this.console.backlog.length) this.console.blSel = this.console.backlog.length - 1;
                    this.textContent = this.console.backlog[this.console.blSel];
                }
                if (e.keyCode == 40 && e.ctrlKey) {
                    this.console.blSel--;
                    if (this.console.blSel < -1) this.console.blSel = 0;
                    if (this.console.blSel == -1) this.textContent = this.console.buffer;
                    else this.textContent = this.console.backlog[this.console.blSel];
                }
                if (this.console.blSel == -1) this.console.buffer = command;
            };
        };



        eval(command) {
            // instead of having two command lines (e.g. pymol & javascript), only have one command line (javascript), but rewrite the incoming /command/ from pymol --> javascript...
            command = command.trim();
            if (!command) return;

            if (this.soup.downloadInProgress || !this.initDone) {
                this.commandBuffer.push(command);
                if (this.commandBuffer.length == 1) this.wait4Download();
                return;
            }

            molmil.cli_canvas = this.canvas; molmil.cli_soup = this.canvas.molmilViewer;
            if (!this.altCommandIF(this.environment, command)) this.runCommand.apply(this.environment, [command]);
            molmil.cli_canvas = molmil.cli_soup = null;
        };

        wait4Download() {
            if (this.soup.downloadInProgress || !this.initDone) {
                var that = this;
                setTimeout(function () { that.wait4Download(); }, 100);
                return;
            }

            var buffer = this.commandBuffer; this.commandBuffer = [];
            for (var i = 0; i < buffer.length; i++) {
                this.eval(buffer[i]);
            }
        };

        runCommand(command) { // note the /this/ stuff will not work properly... if there are many functions and internal /var/s...
            if (command.match(/\bfunction\b/)) command = command.replace(/(\b|;)function\s+(\w+)/g, "$1global.$2 = function"); // make sure that functions are stored in /this/ and not in the local scope...
            else command = (' ' + command).replace(/(\s|;)var\s+(\w+)\s*=/g, "$1global.$2 ="); // make sure that variables are stored in /this/ and not in the local scope...
            command = command.replace(/(\s|;)return\sthis;/g, "$1return window;"); // make sure that it is impossible to get back the real window object
            try { with (this) { eval(command); } }
            catch (e) { this.console.error(e); }
        };


        bindNullInterface() {
            if (this.altCommandIF) this.environment.console.log("Previous command interface unbound.");
            this.altCommandIF = function (env, command) { return false; };
        }

        bindPDBjViewerInterface() {
            return molmil.loadPlugin(molmil.settings.src + "plugins/jv-script.js", this.bindPDBjViewerInterface, this);
        }
    };

    export function color2rgba(clr) {
        var simple_colors = { aliceblue: 'f0f8ff', antiquewhite: 'faebd7', aqua: '00ffff', aquamarine: '7fffd4', azure: 'f0ffff', beige: 'f5f5dc', bisque: 'ffe4c4', black: '000000', blanchedalmond: 'ffebcd', blue: '0000ff', blueviolet: '8a2be2', brown: 'a52a2a', burlywood: 'deb887', cadetblue: '5f9ea0', chartreuse: '7fff00', chocolate: 'd2691e', coral: 'ff7f50', cornflowerblue: '6495ed', cornsilk: 'fff8dc', crimson: 'dc143c', cyan: '00ffff', darkblue: '00008b', darkcyan: '008b8b', darkgoldenrod: 'b8860b', darkgray: 'a9a9a9', darkgreen: '006400', darkkhaki: 'bdb76b', darkmagenta: '8b008b', darkolivegreen: '556b2f', darkorange: 'ff8c00', darkorchid: '9932cc', darkred: '8b0000', darksalmon: 'e9967a', darkseagreen: '8fbc8f', darkslateblue: '483d8b', darkslategray: '2f4f4f', darkturquoise: '00ced1', darkviolet: '9400d3', deeppink: 'ff1493', deepskyblue: '00bfff', dimgray: '696969', dodgerblue: '1e90ff', feldspar: 'd19275', firebrick: 'b22222', floralwhite: 'fffaf0', forestgreen: '228b22', fuchsia: 'ff00ff', gainsboro: 'dcdcdc', ghostwhite: 'f8f8ff', gold: 'ffd700', goldenrod: 'daa520', gray: '808080', green: '008000', greenyellow: 'adff2f', honeydew: 'f0fff0', hotpink: 'ff69b4', indianred: 'cd5c5c', indigo: '4b0082', ivory: 'fffff0', khaki: 'f0e68c', lavender: 'e6e6fa', lavenderblush: 'fff0f5', lawngreen: '7cfc00', lemonchiffon: 'fffacd', lightblue: 'add8e6', lightcoral: 'f08080', lightcyan: 'e0ffff', lightgoldenrodyellow: 'fafad2', lightgrey: 'd3d3d3', lightgreen: '90ee90', lightpink: 'ffb6c1', lightsalmon: 'ffa07a', lightseagreen: '20b2aa', lightskyblue: '87cefa', lightslateblue: '8470ff', lightslategray: '778899', lightsteelblue: 'b0c4de', lightyellow: 'ffffe0', lime: '00ff00', limegreen: '32cd32', linen: 'faf0e6', magenta: 'ff00ff', maroon: '800000', mediumaquamarine: '66cdaa', mediumblue: '0000cd', mediumorchid: 'ba55d3', mediumpurple: '9370d8', mediumseagreen: '3cb371', mediumslateblue: '7b68ee', mediumspringgreen: '00fa9a', mediumturquoise: '48d1cc', mediumvioletred: 'c71585', midnightblue: '191970', mintcream: 'f5fffa', mistyrose: 'ffe4e1', moccasin: 'ffe4b5', navajowhite: 'ffdead', navy: '000080', oldlace: 'fdf5e6', olive: '808000', olivedrab: '6b8e23', orange: 'ffa500', orangered: 'ff4500', orchid: 'da70d6', palegoldenrod: 'eee8aa', palegreen: '98fb98', paleturquoise: 'afeeee', palevioletred: 'd87093', papayawhip: 'ffefd5', peachpuff: 'ffdab9', peru: 'cd853f', pink: 'ffc0cb', plum: 'dda0dd', powderblue: 'b0e0e6', purple: '800080', red: 'ff0000', rosybrown: 'bc8f8f', royalblue: '4169e1', saddlebrown: '8b4513', salmon: 'fa8072', sandybrown: 'f4a460', seagreen: '2e8b57', seashell: 'fff5ee', sienna: 'a0522d', silver: 'c0c0c0', skyblue: '87ceeb', slateblue: '6a5acd', slategray: '708090', snow: 'fffafa', springgreen: '00ff7f', steelblue: '4682b4', tan: 'd2b48c', teal: '008080', thistle: 'd8bfd8', tomato: 'ff6347', turquoise: '40e0d0', violet: 'ee82ee', violetred: 'd02090', wheat: 'f5deb3', white: 'ffffff', whitesmoke: 'f5f5f5', yellow: 'ffff00', yellowgreen: '9acd32' };

        if (simple_colors.hasOwnProperty(clr.toLowerCase())) clr = "#" + simple_colors[clr.toLowerCase()];
        if (clr.substr(0, 1) == "#") {
            var hex = clr.substr(1, 7)
            return [parseInt(hex.substring(0, 2), 16), parseInt(hex.substring(2, 4), 16), parseInt(hex.substring(4, 6), 16), 255];
        }

        return clr;

    }

    export function xhr(url) {
        if (window.hasOwnProperty("rewriteURL")) url = rewriteURL(url);
        if (window.hasOwnProperty("customXHR")) var request = customXHR(url);
        else var request = new molmil_dep.CallRemote("GET");
        request.URL = url;
        return request;
    };

    export function loadScript(url) {
        var cc = this.cli_canvas;
        var cli = cc.commandLine;


        cc.molmilViewer.downloadInProgress++;

        var request = molmil.xhr(url);
        request.ASYNC = true;
        request.OnDone = function () {
            cc.molmilViewer.downloadInProgress--;
            var pathconv = url.split("=");
            pathconv[pathconv.length - 1] = pathconv[pathconv.length - 1].substr(0, pathconv[pathconv.length - 1].lastIndexOf('/'))
            if (pathconv[pathconv.length - 1]) pathconv[pathconv.length - 1] += "/";
            cc.molmilViewer.__cwd__ = cli.environment.scriptUrl = pathconv.join("=");
            cli.environment.console.runCommand(this.request.responseText.replace(/\/\*[\s\S]*?\*\/|([^:]|^)\/\/.*$/gm, ""), true);
        }
        for (var e in cc.molmilViewer.extraREST) request.AddParameter(e, cc.molmilViewer.extraREST[e]);
        for (var e in cc.molmilViewer.extraRESTHeaders) request.headers[e] = cc.molmilViewer.extraRESTHeaders[e];
        request.Send();
    }

    export function isBalancedStatement(string) {
        var parentheses = "[]{}()", stack = [], i, character, bracePosition;
        for (i = 0; character = string[i]; i++) {
            bracePosition = parentheses.indexOf(character);
            if (bracePosition === -1) continue;

            if (bracePosition % 2 === 0) stack.push(bracePosition + 1);
            else if (stack.pop() !== bracePosition) return false;
        }
        return stack.length === 0;
    }

    // END
}