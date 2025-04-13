namespace molmil {
    
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