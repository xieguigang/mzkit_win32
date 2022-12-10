/// <reference path="./d/linq.d.ts" />
/// <reference path="./apps/three_app.ts" />

namespace app.desktop {

    export function run() {
        Router.AddAppHandler(new apps.three_app());

        Router.RunApp();
    }
}

$ts.mode = Modes.debug;
$ts(app.desktop.run);