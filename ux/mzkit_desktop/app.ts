///<reference path="./linq.d.ts">

namespace app.desktop {

    export function run() {
        Router.AddAppHandler(new apps.three_app());
    }
}

$ts.mode = Modes.debug;
$ts(app.desktop.run);