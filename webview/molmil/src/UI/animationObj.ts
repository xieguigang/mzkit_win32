namespace molmil {

    export class animationObj {

        soup;
        renderer;
        frameNo;
        motionMode;
        init;
        delay;
        frameAction;
        detail_or;
        infoBox;
        number_of_frames;
        TID;
        playing;

        constructor(soup) {
            this.soup = soup;
            this.renderer = soup.renderer;
            this.frameNo = 0;
            this.motionMode = 1;
            this.init = false;
            this.delay = 66;
            this.frameAction = function () { };
            this.detail_or = -1;
            this.infoBox = null;
        }

        initialise(infoBox = null) { // redo
            this.renderer.animationMode = true;
            this.number_of_frames = this.soup.structures.length ? this.soup.structures[0].number_of_frames : 0;
            this.frameNo = this.renderer.modelId;
            this.init = true;
            this.infoBox = infoBox;
        };

        updateInfoBox() {
            if (!this.infoBox) return;
            if (this.infoBox.timeBox) {
                if (this.soup.frameInfo) this.infoBox.timeBox.innerHTML = this.soup.frameInfo[this.frameNo][1].toFixed(1) + " ps (" + (this.frameNo + 1) + ")";
                else this.infoBox.timeBox.innerHTML = this.frameNo;
            }
            if (this.infoBox.sliderBox) {
                if (this.soup.frameInfo) this.infoBox.timeBox.value = this.soup.frameInfo[this.frameNo][1];
                else this.infoBox.timeBox.value = this.frameNo;
                this.infoBox.sliderBox.value = this.frameNo;
            }
        };

        beginning() {
            if (!this.init) this.initialise();
            this.frameNo = 0;
            this.renderer.selectFrame(this.frameNo, this.detail_or);
            this.soup.canvas.update = true;
            this.frameAction();
            this.updateInfoBox();
            if (molmil.settings.recordingMode) this.renderer.render();
        };

        go2Frame(fid) {
            if (!this.init) this.initialise();
            this.frameNo = fid;
            this.renderer.selectFrame(this.frameNo, this.detail_or);
            this.soup.canvas.update = true;
            this.frameAction();
            this.updateInfoBox();
        }


        previous() {
            if (!this.init) this.initialise();
            this.frameNo -= 1;
            if (this.frameNo < 0) this.frameNo = 0;
            this.renderer.selectFrame(this.frameNo, this.detail_or);
            this.soup.canvas.update = true;
            this.frameAction();
            this.updateInfoBox();
        };

        pause() {
            if (this.TID) {
                clearTimeout(this.TID);
                this.TID = null;
            }
        };

        play() {
            if (!this.init) this.initialise();
            this.playing = true;
            if (this.motionMode == 2) this.TID = molmil_dep.asyncStart(this.backwardRenderer, [], this, this.delay);
            else this.TID = molmil_dep.asyncStart(this.forwardRenderer, [], this, this.delay);
        };

        next() {
            if (!this.init) this.initialise();
            this.frameNo += 1;
            //if (this.frameNo >= this.renderer.framesBuffer.length) this.frameNo = this.renderer.framesBuffer.length-1;
            if (this.frameNo >= this.number_of_frames) this.frameNo = this.number_of_frames - 1;
            this.renderer.selectFrame(this.frameNo, this.detail_or);
            this.soup.canvas.update = true;
            this.frameAction();
            this.updateInfoBox();
        };

        end() {
            if (!this.init) this.initialise();
            //this.frameNo = this.renderer.framesBuffer.length-1;
            this.frameNo = this.number_of_frames - 1;
            this.renderer.selectFrame(this.frameNo, this.detail_or);
            this.soup.canvas.update = true;
            this.frameAction();
            this.updateInfoBox();
            if (molmil.settings.recordingMode) this.renderer.render();
        };

        forwardRenderer() {
            if (this.number_of_frames < 2) return;
            this.frameNo += 1;
            if (this.frameNo >= this.number_of_frames) {
                if (this.motionMode == 3 || this.motionMode == 3.5) { this.frameNo -= 1; return this.backwardRenderer(); }
                else this.playing = false;
            }
            else {
                this.renderer.selectFrame(this.frameNo, this.detail_or);
                this.soup.canvas.update = true;
                this.TID = molmil_dep.asyncStart(this.forwardRenderer, [], this, this.delay);
                this.frameAction();
                this.updateInfoBox();
                if (molmil.settings.recordingMode) this.renderer.render();
            }
        };

        backwardRenderer() {
            if (this.number_of_frames < 2) return;
            this.frameNo -= 1;
            if (this.frameNo < 0) {
                if (this.motionMode == 3 && !molmil.settings.recordingMode) { this.frameNo += 1; return this.forwardRenderer(); }
                else this.playing = false;
            }
            else {
                this.renderer.selectFrame(this.frameNo, this.detail_or);
                this.soup.canvas.update = true;
                this.TID = molmil_dep.asyncStart(this.backwardRenderer, [], this, this.delay);
                this.frameAction();
                this.updateInfoBox();
                if (molmil.settings.recordingMode) this.renderer.render();
            }
        };
    }
}