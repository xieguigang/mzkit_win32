///<reference path="./linq.d.ts">
var app;
(function (app) {
    var desktop;
    (function (desktop) {
        function run() {
            Router.AddAppHandler(new apps.three_app());
        }
        desktop.run = run;
    })(desktop = app.desktop || (app.desktop = {}));
})(app || (app = {}));
$ts.mode = Modes.debug;
$ts(app.desktop.run);
var apps;
(function (apps) {
    class three_app extends Bootstrap {
        get appName() {
            return "3d/three";
        }
        init() {
            // throw new Error("Method not implemented.");
        }
    }
    apps.three_app = three_app;
})(apps || (apps = {}));
//# sourceMappingURL=mzkit_desktop.js.map