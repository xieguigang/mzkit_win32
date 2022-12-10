/// <reference path="../../webview/assets/js/linq.d.ts">
var app;
(function (app) {
    var desktop;
    (function (desktop) {
        function run() {
        }
        desktop.run = run;
    })(desktop = app.desktop || (app.desktop = {}));
})(app || (app = {}));
$ts.mode = Modes.debug;
$ts(app.desktop.run);
//# sourceMappingURL=mzkit_desktop.js.map