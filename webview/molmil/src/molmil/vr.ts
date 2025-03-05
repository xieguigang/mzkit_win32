namespace molmil {

    export function startWebVR(that) {
        //canvas.requestPointerLock(that.canvas);

        molmil.vrDisplays[0].requestPresent([{ source: that.canvas }]).then(function () {
            molmil.vrDisplay = molmil.vrDisplays[0];
            that.renderer.reinitRenderer();
            //molmil.vrDisplay.resetPose(); // deprecated
            var leftEye = molmil.vrDisplay.getEyeParameters('left');
            var rightEye = molmil.vrDisplay.getEyeParameters('right');
            that.renderer.width = that.width = that.canvas.width = Math.max(leftEye.renderWidth, rightEye.renderWidth) * 2;
            that.renderer.height = that.height = that.canvas.height = Math.max(leftEye.renderHeight, rightEye.renderHeight);
            molmil.configBox.stereoMode = 3;
            that.renderer.camera.z = that.calcZ();

            //molmil.pointerLoc_setup(that.canvas);

            window.addEventListener('vrdisplaypresentchange', function () {
                if (molmil.vrDisplay.isPresenting) return;
                molmil.configBox.stereoMode = 0;
                that.canvas.update = true; // draw scene
            });
            that.canvas.update = true; // draw scene
        });
    };

    export function initVR(soup, callback) {
        var initFakeVR = function () {
            var dep = document.createElement("script")
            dep.src = molmil.settings.src + "lib/webvr-polyfill.min.js";
            dep.onload = function () {
                var config = {
                    // Scales the recommended buffer size reported by WebVR, which can improve
                    // performance.
                    BUFFER_SCALE: 1.0, // Default: 0.5.
                }
                var polyfill = new WebVRPolyfill(config);
                navigator.getVRDisplays().then(function (displays) {
                    if (displays.length) { molmil.vrDisplays = displays; molmil.VRstatus = true; molmil.initVR(soup, callback); }
                    else { molmil.VRstatus = false; callback(); }
                });
            };
            var head = document.getElementsByTagName("head")[0];
            head.appendChild(dep);
        }
        if (!molmil.VRstatus) {
            if (navigator.getVRDisplays) {
                navigator.getVRDisplays().then(function (displays) {
                    if (displays.length) {
                        molmil.vrDisplays = displays; molmil.VRstatus = true; molmil.initVR(soup, callback);
                    } else initFakeVR();
                }).catch(function () { initFakeVR(); });
            }
            else initFakeVR();
        }
        else {
            if (soup) molmil.startWebVR(soup);
            if (callback) callback();
        }
    }

}