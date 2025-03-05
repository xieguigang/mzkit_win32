namespace molmil {

    export const defaultSettings_label = { dx: 0.0, dy: 0.0, dz: 0.0, color: [0, 255, 0], fontSize: 20 };

    export class labelObject {

        constructor(soup) {
            this.soup = soup;
            this.texture = null;
            this.settings = JSON.parse(JSON.stringify(molmil.defaultSettings_label));
            this.xyz = [0.0, 0.0, 0.0];
            this.display = true;
            this.text = "";
            this.status = false;
        }
    }
}